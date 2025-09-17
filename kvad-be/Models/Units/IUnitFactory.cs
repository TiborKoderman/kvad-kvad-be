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
  public static Unit CreateUnit(string Symbol, string Definition, string? Name)
  {
    // TODO: Implement unit creation from definition string
    // This would parse the definition and create appropriate unit type
    throw new NotImplementedException("Unit creation from definition not yet implemented");
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

}