using Xunit;

namespace kvad_be.Tests.Units
{
    public class UnitParseDefinitionTests
    {
        [Fact]
        public void ParseDefinition_SimpleUnit_ReturnsCorrectExponent()
        {
            // Arrange
            var definition = "kg";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Single(result);
            Assert.True(result.ContainsKey("kg"));
            Assert.Equal(new Rational(1, 1), result["kg"]);
        }

        [Fact]
        public void ParseDefinition_MultipleUnits_ReturnsAllUnits()
        {
            // Arrange
            var definition = "kg m";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("kg"));
            Assert.True(result.ContainsKey("m"));
            Assert.Equal(new Rational(1, 1), result["kg"]);
            Assert.Equal(new Rational(1, 1), result["m"]);
        }

        [Fact]
        public void ParseDefinition_UnitWithExponent_ReturnsCorrectExponent()
        {
            // Arrange
            var definition = "m^2";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Single(result);
            Assert.True(result.ContainsKey("m"));
            Assert.Equal(new Rational(2, 1), result["m"]);
        }

        [Fact]
        public void ParseDefinition_Division_ReturnsNegativeExponents()
        {
            // Arrange
            var definition = "m/s";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("m"));
            Assert.True(result.ContainsKey("s"));
            Assert.Equal(new Rational(1, 1), result["m"]);
            Assert.Equal(new Rational(-1, 1), result["s"]);
        }

        [Fact]
        public void ParseDefinition_ComplexExpression_ReturnsCorrectExponents()
        {
            // Arrange
            var definition = "kg m/(s^2 A)";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal(new Rational(1, 1), result["kg"]);
            Assert.Equal(new Rational(1, 1), result["m"]);
            Assert.Equal(new Rational(-2, 1), result["s"]);
            Assert.Equal(new Rational(-1, 1), result["A"]);
        }

        [Fact]
        public void ParseDefinition_WithNumbers_ReturnsNumbersAsSymbols()
        {
            // Arrange
            var definition = "1000 m";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("1000"));
            Assert.True(result.ContainsKey("m"));
            Assert.Equal(new Rational(1, 1), result["1000"]);
            Assert.Equal(new Rational(1, 1), result["m"]);
        }

        [Fact]
        public void ParseDefinition_DecimalNumbers_ReturnsCorrectly()
        {
            // Arrange
            var definition = "3.14 kg";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("3.14"));
            Assert.True(result.ContainsKey("kg"));
            Assert.Equal(new Rational(1, 1), result["3.14"]);
            Assert.Equal(new Rational(1, 1), result["kg"]);
        }

        [Fact]
        public void ParseDefinition_NumberWithExponent_ReturnsCorrectExponent()
        {
            // Arrange
            var definition = "1000^2 m";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("1000"));
            Assert.True(result.ContainsKey("m"));
            Assert.Equal(new Rational(2, 1), result["1000"]);
            Assert.Equal(new Rational(1, 1), result["m"]);
        }

        [Fact]
        public void ParseDefinition_NegativeExponents_ReturnsCorrectly()
        {
            // Arrange
            var definition = "kg m^-2 s^-1";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(new Rational(1, 1), result["kg"]);
            Assert.Equal(new Rational(-2, 1), result["m"]);
            Assert.Equal(new Rational(-1, 1), result["s"]);
        }

        [Fact]
        public void ParseDefinition_ExplicitMultiplication_WorksLikeSpaces()
        {
            // Arrange
            var definition = "kg*m*s^-2";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(new Rational(1, 1), result["kg"]);
            Assert.Equal(new Rational(1, 1), result["m"]);
            Assert.Equal(new Rational(-2, 1), result["s"]);
        }

        [Fact]
        public void ParseDefinition_RepeatedUnits_AccumulatesExponents()
        {
            // Arrange
            var definition = "m m^2";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Single(result);
            Assert.True(result.ContainsKey("m"));
            Assert.Equal(new Rational(3, 1), result["m"]); // 1 + 2 = 3
        }

        [Fact]
        public void ParseDefinition_ComplexWithNumbers_ReturnsAllComponents()
        {
            // Arrange
            var definition = "42 kg/(3.14 s^2)";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal(new Rational(1, 1), result["42"]);
            Assert.Equal(new Rational(1, 1), result["kg"]);
            Assert.Equal(new Rational(-1, 1), result["3.14"]);
            Assert.Equal(new Rational(-2, 1), result["s"]);
        }

        [Fact]
        public void ParseDefinition_MultipleDivisions_HandlesDenominatorsCorrectly()
        {
            // Arrange - Test "kg/m/s" which should be kg/(m*s)
            var definition = "kg/m/s";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(new Rational(1, 1), result["kg"]);   // numerator
            Assert.Equal(new Rational(-1, 1), result["m"]);   // first denominator  
            Assert.Equal(new Rational(-1, 1), result["s"]);   // second denominator
        }

        [Fact]
        public void ParseDefinition_MultipleDivisionsWithExponents_HandlesCorrectly()
        {
            // Arrange - Test "kg m^2/s/A^3" which should be (kg*m^2)/(s*A^3)
            var definition = "kg m^2/s/A^3";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal(new Rational(1, 1), result["kg"]);   // numerator
            Assert.Equal(new Rational(2, 1), result["m"]);    // numerator with exponent
            Assert.Equal(new Rational(-1, 1), result["s"]);   // first denominator
            Assert.Equal(new Rational(-3, 1), result["A"]);   // second denominator with exponent
        }

        [Fact]
        public void ParseDefinition_MultipleDivisionsWithParentheses_HandlesCorrectly()
        {
            // Arrange - Test "N/(m kg)/s" which should treat (m kg) as one denominator and s as another
            var definition = "N/(m kg)/s";
            
            // Act
            var result = IUnitFactory.ParseDefinition(definition);
            
            // Assert
            Assert.Equal(4, result.Count);
            Assert.Equal(new Rational(1, 1), result["N"]);    // numerator
            Assert.Equal(new Rational(-1, 1), result["m"]);   // from first denominator (m kg)
            Assert.Equal(new Rational(-1, 1), result["kg"]);  // from first denominator (m kg)
            Assert.Equal(new Rational(-1, 1), result["s"]);   // second denominator
        }
    }
}