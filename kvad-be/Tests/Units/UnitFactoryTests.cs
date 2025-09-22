// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Numerics;
// using Xunit;

// namespace kvad_be.Tests.Units
// {
//     public class UnitFactoryGenerateDimensionArrayTests
//     {
//         private static List<Unit> CreateTestUnits()
//         {
//             return new List<Unit>
//             {
//                 new LinearUnit
//                 {
//                     Symbol = "m",
//                     Name = "Meter",
//                     Quantity = "Length",
//                     Dimension = VectorHelper.CreateDimensionVector(1, 0, 0, 0, 0, 0, 0), // Length
//                     Factor = new Rational(1, 1),
//                 },
//                 new LinearUnit
//                 {
//                     Symbol = "kg",
//                     Name = "Kilogram",
//                     Quantity = "Mass",
//                     Dimension = VectorHelper.CreateDimensionVector(0, 1, 0, 0, 0, 0, 0), // Mass
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 },
//                 new LinearUnit
//                 {
//                     Symbol = "s",
//                     Name = "Second",
//                     Quantity = "Time",
//                     Dimension = VectorHelper.CreateDimensionVector(0, 0, 1, 0, 0, 0, 0), // Time
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 },
//                 new LinearUnit
//                 {
//                     Symbol = "A",
//                     Name = "Ampere",
//                     Quantity = "Electric Current",
//                     Dimension = VectorHelper.CreateDimensionVector(0, 0, 0, 1, 0, 0, 0), // Current
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 },
//                 new LinearUnit
//                 {
//                     Symbol = "N",
//                     Name = "Newton",
//                     Quantity = "Force",
//                     Dimension = VectorHelper.CreateDimensionVector(1, 1, -2, 0, 0, 0, 0), // kg⋅m⋅s⁻²
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 },
//                 new LinearUnit
//                 {
//                     Symbol = "Hz",
//                     Name = "Hertz",
//                     Quantity = "Frequency",
//                     Dimension = VectorHelper.CreateDimensionVector(0, 0, -1, 0, 0, 0, 0), // s⁻¹
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 },
//                 new LinearUnit
//                 {
//                     Symbol = "rad",
//                     Name = "Radian",
//                     Quantity = "Angle",
//                     Dimension = VectorHelper.CreateDimensionVector(0, 0, 0, 0, 0, 0, 0), // Dimensionless
//                     Factor = new Rational(1, 1),
//                     Prefixable = false // Angles typically don't use prefixes
//                 }
//             };
//         }

//         private static List<UnitPrefix> CreateTestPrefixes()
//         {
//             return new List<UnitPrefix>
//             {
//                 new UnitPrefix { Symbol = "k", Name = "Kilo", Base = 10, Exponent = 3 },
//                 new UnitPrefix { Symbol = "M", Name = "Mega", Base = 10, Exponent = 6 },
//                 new UnitPrefix { Symbol = "G", Name = "Giga", Base = 10, Exponent = 9 },
//                 new UnitPrefix { Symbol = "m", Name = "Milli", Base = 10, Exponent = -3 },
//                 new UnitPrefix { Symbol = "μ", Name = "Micro", Base = 10, Exponent = -6 },
//                 new UnitPrefix { Symbol = "n", Name = "Nano", Base = 10, Exponent = -9 },
//                 new UnitPrefix { Symbol = "Ki", Name = "Kibi", Base = 2, Exponent = 10 },
//                 new UnitPrefix { Symbol = "Mi", Name = "Mebi", Base = 2, Exponent = 20 }
//             };
//         }

//         [Fact]
//         public void GenerateDimensionArray_SimpleUnit_ReturnsCorrectDimension()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational> { { "m", new Rational(1, 1) } };

//             // Act
//             var (dimension, scaleFactor) = IUnitFactory.GenerateDimensionArray(definition, units, prefixes);

//             // Assert
//             Assert.Equal(new short[] { 1, 0, 0, 0, 0, 0, 0 }, dimension.ToArray());
//             Assert.Equal(new Rational(1, 1), scaleFactor);
//         }

//         [Fact]
//         public void GenerateDimensionArray_WithPrefix_CalculatesCorrectScaleFactor()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational> { { "km", new Rational(1, 1) } };

//             // Act
//             var (dimension, scaleFactor) = IUnitFactory.GenerateDimensionArray(definition, units, prefixes);

//             // Assert
//             Assert.Equal(new short[] { 1, 0, 0, 0, 0, 0, 0 }, dimension.ToShortArray());
//             Assert.Equal(new Rational(1000, 1), scaleFactor); // k = 10^3 = 1000
//         }

//         [Fact]
//         public void GenerateDimensionArray_CompoundUnit_CombinesDimensions()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational>
//             {
//                 { "m", new Rational(1, 1) },
//                 { "s", new Rational(-1, 1) }
//             };

//             // Act
//             var (dimension, scaleFactor) = IUnitFactory.GenerateDimensionArray(definition, units, prefixes);

//             // Assert
//             Assert.Equal(new short[] { 1, 0, -1, 0, 0, 0, 0 }, dimension.ToArray()); // m/s
//             Assert.Equal(new Rational(1, 1), scaleFactor);
//         }

//         [Fact]
//         public void GenerateDimensionArray_ComplexUnit_CalculatesCorrectly()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational>
//             {
//                 { "kg", new Rational(1, 1) },
//                 { "m", new Rational(2, 1) },
//                 { "s", new Rational(-2, 1) }
//             };

//             // Act
//             var (dimension, scaleFactor) = IUnitFactory.GenerateDimensionArray(definition, units, prefixes);

//             // Assert
//             Assert.Equal(new short[] { 2, 1, -2, 0, 0, 0, 0 }, dimension.ToArray()); // kg⋅m²⋅s⁻²
//             Assert.Equal(new Rational(1, 1), scaleFactor);
//         }

//         [Fact]
//         public void GenerateDimensionArray_WithPrefixAndExponent_CalculatesCorrectScaleFactor()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational>
//             {
//                 { "km", new Rational(2, 1) } // km²
//             };

//             // Act
//             var (dimension, scaleFactor) = IUnitFactory.GenerateDimensionArray(definition, units, prefixes);

//             // Assert
//             Assert.Equal(new short[] { 2, 0, 0, 0, 0, 0, 0 }, dimension.ToArray()); // Length²
//             Assert.Equal(new Rational(1000000, 1), scaleFactor); // (10³)² = 10⁶
//         }

//         [Fact]
//         public void GenerateDimensionArray_NegativeExponent_CalculatesCorrectScaleFactor()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational>
//             {
//                 { "km", new Rational(-1, 1) } // 1/km
//             };

//             // Act
//             var (dimension, scaleFactor) = IUnitFactory.GenerateDimensionArray(definition, units, prefixes);

//             // Assert
//             Assert.Equal(new short[] { -1, 0, 0, 0, 0, 0, 0 }, dimension.ToArray()); // 1/Length
//             Assert.Equal(new Rational(1, 1000), scaleFactor); // 1/(10³) = 1/1000
//         }

//         [Fact]
//         public void GenerateDimensionArray_MultiplePrefix_CombinesFactors()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational>
//             {
//                 { "km", new Rational(1, 1) },
//                 { "ms", new Rational(-1, 1) } // km/ms
//             };

//             // Act
//             var (dimension, scaleFactor) = IUnitFactory.GenerateDimensionArray(definition, units, prefixes);

//             // Assert
//             Assert.Equal(new short[] { 1, 0, -1, 0, 0, 0, 0 }, dimension.ToShortArray()); // m/s
//             // Scale factor: 1000 * (1/0.001) = 1000 * 1000 = 1,000,000
//             Assert.Equal(new Rational(1000000, 1), scaleFactor);
//         }

//         [Fact]
//         public void GenerateDimensionArray_NonIntegerExponent_ThrowsException()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational>
//             {
//                 { "m", new Rational(1, 2) } // m^(1/2)
//             };

//             // Act & Assert
//             var exception = Assert.Throws<ArgumentException>(() =>
//                 IUnitFactory.GenerateDimensionArray(definition, units, prefixes));
//             Assert.Contains("Non-integer exponent", exception.Message);
//         }

//         [Fact]
//         public void GenerateDimensionArray_UnknownUnit_ThrowsException()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational>
//             {
//                 { "xyz", new Rational(1, 1) } // Unknown unit
//             };

//             // Act & Assert
//             var exception = Assert.Throws<ArgumentException>(() =>
//                 IUnitFactory.GenerateDimensionArray(definition, units, prefixes));
//             Assert.Contains("Unit 'xyz' not found", exception.Message);
//         }

//         [Fact]
//         public void GenerateDimensionArray_UnknownPrefix_ThrowsException()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational>
//             {
//                 { "Zm", new Rational(1, 1) } // Unknown prefix "Z" with known unit "m"
//             };

//             // Act & Assert
//             var exception = Assert.Throws<ArgumentException>(() =>
//                 IUnitFactory.GenerateDimensionArray(definition, units, prefixes));
            
//             // The SplitSymbolAndPrefix function will not find "Z" as a prefix,
//             // so it will treat "Zm" as an unknown unit
//             Assert.Contains("Unit 'Zm' not found", exception.Message);
//         }

//         [Fact]
//         public void GenerateDimensionArray_EmptyDefinition_ReturnsZeroDimension()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();
//             var definition = new Dictionary<string, Rational>();

//             // Act
//             var (dimension, scaleFactor) = IUnitFactory.GenerateDimensionArray(definition, units, prefixes);

//             // Assert
//             Assert.Equal(new short[] { 0, 0, 0, 0, 0, 0, 0 }, dimension.ToArray());
//             Assert.Equal(new Rational(1, 1), scaleFactor);
//         }

//         [Fact]
//         public void GenerateDimensionArray_PrefixNotFoundInDatabase_ThrowsException()
//         {
//             // Arrange - create a scenario where SplitSymbolAndPrefix identifies a prefix but it's not in the database
//             var units = new List<Unit>
//             {
//                 new LinearUnit
//                 {
//                     Symbol = "m",
//                     Name = "Meter",
//                     Quantity = "Length",
//                     Dimension = VectorHelper.CreateDimensionVector(1, 0, 0, 0, 0, 0, 0),
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 }
//             };
//             var prefixes = new List<UnitPrefix>(); // Empty prefixes list
//             var definition = new Dictionary<string, Rational>
//             {
//                 { "km", new Rational(1, 1) } // "k" should be identified as prefix but won't be found
//             };

//             // Act & Assert
//             var exception = Assert.Throws<ArgumentException>(() =>
//                 IUnitFactory.GenerateDimensionArray(definition, units, prefixes));
            
//             // Since there are no prefixes, SplitSymbolAndPrefix will not identify "k" as a prefix
//             // So "km" will be treated as an unknown unit
//             Assert.Contains("Unit 'km' not found", exception.Message);
//         }
//     }

//     public class UnitFactorySplitSymbolAndPrefixTests
//     {
//         private static List<Unit> CreateTestUnits()
//         {
//             return new List<Unit>
//             {
//                 new LinearUnit
//                 {
//                     Symbol = "m",
//                     Name = "Meter",
//                     Quantity = "Length",
//                     Dimension = VectorHelper.CreateDimensionVector(1, 0, 0, 0, 0, 0, 0),
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 },
//                 new LinearUnit
//                 {
//                     Symbol = "Hz",
//                     Name = "Hertz",
//                     Quantity = "Frequency",
//                     Dimension = VectorHelper.CreateDimensionVector(0, 0, -1, 0, 0, 0, 0),
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 },
//                 new LinearUnit
//                 {
//                     Symbol = "rad",
//                     Name = "Radian",
//                     Quantity = "Angle",
//                     Dimension = VectorHelper.CreateDimensionVector(0, 0, 0, 0, 0, 0, 0),
//                     Factor = new Rational(1, 1),
//                     Prefixable = false // Not prefixable
//                 }
//             };
//         }

//         private static List<UnitPrefix> CreateTestPrefixes()
//         {
//             return new List<UnitPrefix>
//             {
//                 new UnitPrefix { Symbol = "k", Name = "Kilo", Base = 10, Exponent = 3 },
//                 new UnitPrefix { Symbol = "M", Name = "Mega", Base = 10, Exponent = 6 },
//                 new UnitPrefix { Symbol = "m", Name = "Milli", Base = 10, Exponent = -3 },
//                 new UnitPrefix { Symbol = "μ", Name = "Micro", Base = 10, Exponent = -6 },
//                 new UnitPrefix { Symbol = "MHz", Name = "Fake Long Prefix", Base = 10, Exponent = 6 } // Test longer prefix
//             };
//         }

//         [Fact]
//         public void SplitSymbolAndPrefix_SimpleUnit_ReturnsNoPrefix()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();

//             // Act
//             var (prefix, baseUnit) = IUnitFactory.SplitSymbolAndPrefix("m", prefixes, units);

//             // Assert
//             Assert.Equal("", prefix);
//             Assert.Equal("m", baseUnit);
//         }

//         [Fact]
//         public void SplitSymbolAndPrefix_WithPrefix_SplitsCorrectly()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();

//             // Act
//             var (prefix, baseUnit) = IUnitFactory.SplitSymbolAndPrefix("km", prefixes, units);

//             // Assert
//             Assert.Equal("k", prefix);
//             Assert.Equal("m", baseUnit);
//         }

//         [Fact]
//         public void SplitSymbolAndPrefix_LongerPrefix_PrefersLongerMatch()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();

//             // Act - "MHz" could be split as "M" + "Hz" or "MHz" + "" but longer prefix wins
//             var (prefix, baseUnit) = IUnitFactory.SplitSymbolAndPrefix("MHzHz", prefixes, units);

//             // Assert
//             Assert.Equal("MHz", prefix);
//             Assert.Equal("Hz", baseUnit);
//         }

//         [Fact]
//         public void SplitSymbolAndPrefix_NonPrefixableUnit_ReturnsNoPrefix()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();

//             // Act - "krad" should not split because "rad" is not prefixable
//             var (prefix, baseUnit) = IUnitFactory.SplitSymbolAndPrefix("krad", prefixes, units);

//             // Assert
//             Assert.Equal("", prefix);
//             Assert.Equal("krad", baseUnit);
//         }

//         [Fact]
//         public void SplitSymbolAndPrefix_EmptyString_ReturnsEmpty()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();

//             // Act
//             var (prefix, baseUnit) = IUnitFactory.SplitSymbolAndPrefix("", prefixes, units);

//             // Assert
//             Assert.Equal("", prefix);
//             Assert.Equal("", baseUnit);
//         }

//         [Fact]
//         public void SplitSymbolAndPrefix_UnknownSymbol_ReturnsAsBaseUnit()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();

//             // Act
//             var (prefix, baseUnit) = IUnitFactory.SplitSymbolAndPrefix("xyz", prefixes, units);

//             // Assert
//             Assert.Equal("", prefix);
//             Assert.Equal("xyz", baseUnit);
//         }

//         [Fact]
//         public void SplitSymbolAndPrefix_PrefixWithUnknownBase_ReturnsAsBaseUnit()
//         {
//             // Arrange
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();

//             // Act - "kxyz" where "xyz" is not a known unit
//             var (prefix, baseUnit) = IUnitFactory.SplitSymbolAndPrefix("kxyz", prefixes, units);

//             // Assert
//             Assert.Equal("", prefix);
//             Assert.Equal("kxyz", baseUnit);
//         }

//         [Fact]
//         public void SplitSymbolAndPrefix_AmbiguousCase_HandlesCorrectly()
//         {
//             // Arrange - "m" is both a unit and a prefix
//             var units = CreateTestUnits();
//             var prefixes = CreateTestPrefixes();

//             // Act - "mm" could be "m" (meter) or "m" (milli) + "m" (meter)
//             // Should prefer existing unit first, then try prefix
//             var (prefix, baseUnit) = IUnitFactory.SplitSymbolAndPrefix("mm", prefixes, units);

//             // Assert - Should split as milli + meter since "mm" is not a unit by itself
//             Assert.Equal("m", prefix);
//             Assert.Equal("m", baseUnit);
//         }
//     }

//     public class UnitFactoryIntegrationTests
//     {
//         [Fact]
//         public void IntegrationTest_ParseReduceGenerate_WorksTogether()
//         {
//             // Arrange
//             var units = new List<Unit>
//             {
//                 new LinearUnit
//                 {
//                     Symbol = "N",
//                     Name = "Newton",
//                     Quantity = "Force",
//                     Dimension = VectorHelper.CreateDimensionVector(1, 1, -2, 0, 0, 0, 0), // kg⋅m⋅s⁻²
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 },
//                 new LinearUnit
//                 {
//                     Symbol = "m",
//                     Name = "Meter", 
//                     Quantity = "Length",
//                     Dimension = VectorHelper.CreateDimensionVector(1, 0, 0, 0, 0, 0, 0),
//                     Factor = new Rational(1, 1),
//                     Prefixable = true
//                 }
//             };
//             var prefixes = new List<UnitPrefix>
//             {
//                 new UnitPrefix { Symbol = "k", Name = "Kilo", Base = 10, Exponent = 3 }
//             };

//             // Act - Test the full pipeline: Parse -> Reduce -> Generate
//             var definition = "1000 kN m / 3"; // Should be: 1000 * kN * m / 3 = (1000/3) * 1000 * N * m
//             var parsed = IUnitFactory.ParseDefinition(definition);
//             var (reduced, scaleFactor) = IUnitFactory.ReduceDefinition(parsed);
//             var (dimension, unitScaleFactor) = IUnitFactory.GenerateDimensionArray(reduced, units, prefixes);

//             // Assert
//             // Parsed should have: {1000: 1/1, kN: 1/1, m: 1/1, 3: -1/1}
//             Assert.Equal(4, parsed.Count);
//             Assert.Equal(new Rational(1, 1), parsed["1000"]);
//             Assert.Equal(new Rational(1, 1), parsed["kN"]);
//             Assert.Equal(new Rational(1, 1), parsed["m"]);
//             Assert.Equal(new Rational(-1, 1), parsed["3"]);

//             // Reduced should separate numbers from units: {kN: 1/1, m: 1/1} with scaleFactor = 1 * 1 * (-1) = -1
//             Assert.Equal(2, reduced.Count);
//             Assert.Equal(new Rational(1, 1), reduced["kN"]);
//             Assert.Equal(new Rational(1, 1), reduced["m"]);
//             Assert.Equal(new Rational(-1, 1), scaleFactor);

//             // Dimension should be N⋅m = [1,1,-2,0,0,0,0] + [1,0,0,0,0,0,0] = [2,1,-2,0,0,0,0]
//             Assert.Equal(new short[] { 2, 1, -2, 0, 0, 0, 0 }, dimension.ToArray());
            
//             // Unit scale factor should be kN factor * m factor = 1000 * 1 = 1000
//             Assert.Equal(new Rational(1000, 1), unitScaleFactor);
//         }
//     }
// }