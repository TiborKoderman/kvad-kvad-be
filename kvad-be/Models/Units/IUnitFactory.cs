using kvad_be.Database;

public interface IUnitFactory
{
  public static Unit CreateUnit(string Symbol, string Name, string Quantity, string? Definition)
  {
    return new LinearUnit
    {
      Symbol = Symbol,
      Name = Name,
      Definition = Definition,
      Quantity = Quantity,
      Dimension = [0, 0, 0, 0, 0, 0, 0],
      Factor = new Rational(1, 1) // Set default Factor value
    };
  }

  public static Unit CreateUnit(string Symbol, string Name, string Quantity, short[] Dimension, string? Definition, bool Prefixable = true)
  {
    return new LinearUnit
    {
      Symbol = Symbol,
      Name = Name,
      Definition = Definition,
      Quantity = Quantity,
      Dimension = Dimension,
      Prefixable = Prefixable,
      Factor = new Rational(1, 1) // Set default Factor value
    };
  }

  /** Create a Unit based on the provided parameters.
      Definition is a string that defines the unit in terms of other units.
      Syntax is as follows:
      - Multiplication: use a space or '*', e.g. "N m" or "N*m"
      - Division: use '/', e.g. "m/s"
      - Exponentiation: use '^', e.g. "m^2"
      - Parentheses for grouping, e.g. "kg m/(s^2 A)"
      - Unit symbols must match existing units in the database.
   */
  public static Unit CreateUnit(string Symbol, string Definition, string? Name, AppDbContext context)
  {
    // Parse the definition string
    var parsedDefinition = ParseDefinition(Definition);
    
    // Reduce the definition to separate numeric and unit terms
    var (reducedDefinition, scaleFactor) = ReduceDefinition(parsedDefinition);
    
    // Generate dimension array and calculate total scale factor
    var (dimension, unitScaleFactor) = GenerateDimensionArray(reducedDefinition, context);
    
    // Combine the scale factors
    var totalFactor = (scaleFactor * unitScaleFactor).Normalize();
    
    return new LinearUnit
    {
      Symbol = Symbol,
      Name = Name ?? Symbol,
      Definition = Definition,
      Quantity = "Derived", // TODO: Could be calculated from dimension analysis
      Dimension = dimension,
      Factor = totalFactor,
      Prefixable = true // Default to prefixable for derived units
    };
  }

  // Syntax is as follows:
  // - Multiplication: use a space or '*', e.g. "N m" or "N*m"
  // - Division: use '/', e.g. "m/s"
  // - Exponentiation: use '^', e.g. "m^2"
  // - Parentheses for grouping, e.g. "kg m/(s^2 A)"
  // - Unit symbols must match existing units in the database.
  // Output is a dictionary of {symbol/number => exponent} pairs
  // E.g. "kg m/(s^2 A)" -> {kg: 1/1, m: 1/1, s: -2/1, A: -1/1}
  // Don't handle prefixes here, just raw symbols
  // Handle implicit multiplication (space) and explicit (*)
  // Handle negative exponents
  // Handle parentheses for grouping
  // Handle multiple divisions: "a/b/c" = "a/(b*c)"
  // Do it in a quick and dirty way, no need for full parsing
  // Throw exception on invalid syntax
  // This is just a basic implementation, not a full parser
  // This function does not validate if the units exist in the database
  public static Dictionary<string, Rational> ParseDefinition(string definition)
  {
    var result = new Dictionary<string, Rational>();

    // Split by '/' to handle division - first part is numerator, all others are denominators
    var parts = definition.Split('/');

    // Parse numerator (positive exponents)
    ParseSection(parts[0], result, new Rational(1, 1));

    // Parse all denominators (negative exponents)
    for (int i = 1; i < parts.Length; i++)
    {
      ParseSection(parts[i], result, new Rational(-1, 1));
    }

    return result;
  }

  private static void ParseSection(string section, Dictionary<string, Rational> result, Rational signMultiplier)
  {
    // Remove parentheses and normalize spaces
    section = section.Replace("(", "").Replace(")", "").Replace("*", " ");

    // Regex to match: number OR unit_symbol, optionally followed by ^exponent
    // Matches: "1000", "kg", "m^2", "s^-1", "42^3", etc.
    var matches = System.Text.RegularExpressions.Regex.Matches(section, @"([a-zA-Z]+|\d+(?:\.\d+)?)(?:\^(-?\d+))?");

    foreach (System.Text.RegularExpressions.Match match in matches)
    {
      var symbol = match.Groups[1].Value;
      var exponentStr = match.Groups[2].Value;

      // Default exponent is 1 if not specified
      var exponent = string.IsNullOrEmpty(exponentStr) ? new Rational(1, 1) : new Rational(int.Parse(exponentStr), 1);

      // Apply sign multiplier (for denominator terms)
      exponent = exponent * signMultiplier;

      // Add or accumulate exponents for the same symbol
      if (result.ContainsKey(symbol))
        result[symbol] = (result[symbol] + exponent).Normalize();
      else
        result[symbol] = exponent.Normalize();
    }
  }


  //Reduce the definition by combining like terms and removing zero-exponent terms
  // E.g. {kg: 1/1, m: 1/1, s: -2/1, m: 2/1} -> {kg: 1/1, m: 3/1, s: -2/1}
  //Combine all numbers into a single number, e.g. {42: 1/1, 3.14: -1/1} -> Rational(42,3.14)
  //Returns a new dictionary with the reduced definition and a scaler factor
  public static (Dictionary<string, Rational> reducedDefinition, Rational scaleFactor) ReduceDefinition(Dictionary<string, Rational> definition)
  {
    var reduced = new Dictionary<string, Rational>();
    Rational scaleFactor = new Rational(1, 1);

    foreach (var kvp in definition)
    {
      if (kvp.Key.All(char.IsDigit))
      {
        // Combine all numbers into a single number
        scaleFactor *= kvp.Value;
      }
      else
      {
        // Keep non-number terms
        reduced[kvp.Key] = kvp.Value;
      }
    }

    return (reduced, scaleFactor);
  }


  //For each entry in the definition, look up the unit in the database to get its dimension array,
  // also calculate the overall scale factor from the definition
  // E.g. definition = {N: 1/1, m: 1/1, s: -2/1}
  // N -> dimension [1,1,-2,0,0,0,0], factor 1/1
  // m -> dimension [1,0,0,0,0,0,0], factor 1/1
  // s -> dimension [0,0,1,0,0,0,0], factor 1/1
  // Overall dimension = [2,1,-1,0,0,0,0], overall factor = 1/1
  // Detect if the unit has a prefix, and if so, adjust the factor accordingly
  // Throw exception if any unit in the definition is not found in the database
  // When fetching the unit, make sure to get exact match on symbol (no prefixes)
  // E.g. definition = {kN: 1/1, m: 1/1, s: -2/1}
  // kN -> dimension [1,1,-2,0,0,0,0], factor 1000/1

  // Detect if the unit has a prefix, and if so, adjust the factor accordingly
  // Throw exception if any unit in the definition is not found in the database
  // When fetching the unit, make sure to get exact match on symbol (no prefixes)
  // E.g. definition = {kN: 1/1, m: 1/1, s: -2/1}
  // kN -> dimension [1,1,-2,0,0,0,0], factor 1000/1
  // m -> dimension [1,0,0,0,0,0,0], factor 1/1
  // s -> dimension [0,0,1,0,0,0,0], factor 1/1
  // Overall dimension = [2,1,-1,0,0,0,0], overall factor = 1000/1
  public static (short[] dimensionArray, Rational scaleFactor) GenerateDimensionArray(Dictionary<string, Rational> definition, IEnumerable<Unit> availableUnits, IEnumerable<UnitPrefix> availablePrefixes)
  {
    foreach (var kvp in definition)
    {
      if (kvp.Value.Denominator != 1)
      {
        throw new ArgumentException($"Non-integer exponent {kvp.Value} for unit {kvp.Key} - cannot generate dimension array");
      }
    }

    // Get dimension length from the first available unit (they should all have the same length)
    var firstUnit = availableUnits.FirstOrDefault();
    if (firstUnit == null)
      throw new ArgumentException("No units available in database");
    
    var dimensionLength = firstUnit.Dimension.Length;
    var resultDimension = new short[dimensionLength];
    var totalScaleFactor = new Rational(1, 1);

    foreach (var kvp in definition)
    {
      var symbol = kvp.Key;
      var exponent = (int)kvp.Value.Numerator; // Safe since we validated denominator = 1

      // Split symbol into prefix and base unit
      var (prefix, baseUnit) = SplitSymbolAndPrefix(symbol, availablePrefixes, availableUnits);

      // Find the base unit
      var unit = availableUnits.FirstOrDefault(u => u.Symbol == baseUnit);
      if (unit == null)
      {
        throw new ArgumentException($"Unit '{baseUnit}' not found in database (from symbol '{symbol}')");
      }

      // Calculate prefix factor if any
      var prefixFactor = new Rational(1, 1);
      if (!string.IsNullOrEmpty(prefix))
      {
        var prefixObj = availablePrefixes.FirstOrDefault(p => p.Symbol == prefix);
        if (prefixObj == null)
        {
          throw new ArgumentException($"Prefix '{prefix}' not found in database (from symbol '{symbol}')");
        }

        // Calculate prefix factor: base^exponent
        // E.g. "k" -> 10^3 = 1000, "M" -> 10^6 = 1000000
        var baseValue = prefixObj.Base;
        var prefixExponent = prefixObj.Exponent;
        
        // Calculate base^prefixExponent
        if (baseValue == 10)
        {
          // Handle decimal prefixes efficiently
          if (prefixExponent >= 0)
          {
            prefixFactor = new Rational((long)Math.Pow(10, prefixExponent), 1);
          }
          else
          {
            prefixFactor = new Rational(1, (long)Math.Pow(10, -prefixExponent));
          }
        }
        else if (baseValue == 2)
        {
          // Handle binary prefixes
          if (prefixExponent >= 0)
          {
            prefixFactor = new Rational((long)Math.Pow(2, prefixExponent), 1);
          }
          else
          {
            prefixFactor = new Rational(1, (long)Math.Pow(2, -prefixExponent));
          }
        }
        else
        {
          throw new ArgumentException($"Unsupported prefix base {baseValue} for prefix '{prefix}'");
        }
      }

      // Add unit's dimension contribution (multiplied by exponent)
      for (int i = 0; i < dimensionLength; i++)
      {
        resultDimension[i] += (short)(unit.Dimension[i] * exponent);
      }

      // Calculate scale factor contribution: (unit.Factor * prefixFactor)^exponent
      var unitTotalFactor = unit.Factor * prefixFactor;
      
      // Apply exponent to the factor
      if (exponent > 0)
      {
        for (int i = 0; i < exponent; i++)
        {
          totalScaleFactor *= unitTotalFactor;
        }
      }
      else if (exponent < 0)
      {
        for (int i = 0; i < -exponent; i++)
        {
          totalScaleFactor /= unitTotalFactor;
        }
      }
      // If exponent is 0, no contribution (shouldn't happen since we filter these out)
    }

    return (resultDimension, totalScaleFactor.Normalize());
  }

  // Utility function to split a unit symbol into prefix and base unit
  // Returns (prefix, baseUnit) where prefix is empty string if no prefix found
  // E.g. "km" -> ("k", "m"), "N" -> ("", "N"), "MHz" -> ("M", "Hz")
  // This function requires database context to check for known prefixes
  public static (string prefix, string baseUnit) SplitSymbolAndPrefix(string symbol, IEnumerable<UnitPrefix> availablePrefixes, IEnumerable<Unit> availableUnits)
  {
    if (string.IsNullOrEmpty(symbol))
      return ("", "");

    // Check if symbol exists as-is (no prefix)
    if (availableUnits.Any(u => u.Symbol == symbol))
      return ("", symbol);

    // Try each prefix from longest to shortest to handle cases like "MHz" vs "mHz"
    var prefixesByLength = availablePrefixes.OrderByDescending(p => p.Symbol.Length);
    
    foreach (var prefix in prefixesByLength)
    {
      if (symbol.StartsWith(prefix.Symbol))
      {
        var baseUnit = symbol.Substring(prefix.Symbol.Length);
        
        // Check if the remaining part is a valid unit
        if (availableUnits.Any(u => u.Symbol == baseUnit && u.Prefixable))
        {
          return (prefix.Symbol, baseUnit);
        }
      }
    }

    // No prefix found - treat entire symbol as base unit
    return ("", symbol);
  }

  // Database-driven version of GenerateDimensionArray that fetches units and prefixes from DbContext
  public static (short[] dimensionArray, Rational scaleFactor) GenerateDimensionArray(Dictionary<string, Rational> definition, AppDbContext context)
  {
    return GenerateDimensionArray(definition, context.Units.ToList(), context.Prefixes.ToList());
  }
}