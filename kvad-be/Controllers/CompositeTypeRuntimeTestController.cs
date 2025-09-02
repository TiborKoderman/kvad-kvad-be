using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kvad_be.Database;

namespace kvad_be.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompositeTypeRuntimeTestController : ControllerBase
{
    private readonly AppDbContext _context;

    public CompositeTypeRuntimeTestController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Test runtime composite type functionality by executing raw SQL
    /// </summary>
    [HttpPost("test-composite-types")]
    public async Task<IActionResult> TestCompositeTypes()
    {
        try
        {
            // Test creating PostgreSQL composite types manually
            await _context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO ""SimpleTestEntities"" (""Name"", ""DimensionData"", ""RatioData"") 
                VALUES ('Test1', 'dim7 test', 'rational test');
            ");

            // Test that we can create and store composite values using raw SQL
            await _context.Database.ExecuteSqlRawAsync(@"
                CREATE TEMP TABLE test_composite_table (
                    id SERIAL PRIMARY KEY,
                    test_dim dim7,
                    test_rational rational
                );
                
                INSERT INTO test_composite_table (test_dim, test_rational) 
                VALUES (
                    ROW(1,2,3,4,5,6,7)::dim7,
                    ROW(22,7)::rational
                );
            ");

            // Query the data back
            var result = await _context.Database.SqlQueryRaw<dynamic>(@"
                SELECT test_dim, test_rational FROM test_composite_table;
            ").ToListAsync();

            return Ok(new
            {
                Message = "Composite types are working at the PostgreSQL level!",
                CompositeTypesCreated = new
                {
                    Dim7 = "ROW(1,2,3,4,5,6,7)::dim7",
                    Rational = "ROW(22,7)::rational"
                },
                TestResults = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = "Failed to test composite types",
                Details = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }

    /// <summary>
    /// Test Dim7 and Rational C# types
    /// </summary>
    [HttpGet("test-csharp-types")]
    public IActionResult TestCSharpTypes()
    {
        try
        {
            // Test our C# composite types
            var testDim = new Dim7(1, 2, 3, 4, 5, 6, 7);
            var testRational = new Rational(22, 7);

            return Ok(new
            {
                Message = "C# composite types are working!",
                TestData = new
                {
                    Dimension = new
                    {
                        L = testDim.L,
                        M = testDim.M,
                        T = testDim.T,
                        I = testDim.I,
                        Th = testDim.Th,
                        N = testDim.N,
                        J = testDim.J,
                        ToString = testDim.ToString()
                    },
                    Rational = new
                    {
                        Numerator = testRational.Numerator,
                        Denominator = testRational.Denominator,
                        Decimal = testRational.ToDecimal(),
                        ToString = testRational.ToString()
                    }
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = "Failed to test C# types",
                Details = ex.Message
            });
        }
    }
}
