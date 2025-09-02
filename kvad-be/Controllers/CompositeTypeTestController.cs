using Microsoft.AspNetCore.Mvc;
using kvad_be.Database;

namespace kvad_be.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompositeTypeTestController : ControllerBase
{
    private readonly AppDbContext _context;

    public CompositeTypeTestController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Test endpoint to verify composite type functionality
    /// </summary>
    [HttpGet("test")]
    public IActionResult TestCompositeTypes()
    {
        try
        {
            // Create test data using our composite types
            var testData = new
            {
                Dimension = new Dim7(1, 2, 3, 4, 5, 6, 7),
                Ratio = new Rational(22, 7)
            };

            return Ok(new
            {
                Message = "Composite types are working correctly!",
                TestData = testData,
                Calculations = new
                {
                    RatioDecimal = testData.Ratio.ToDecimal(),
                    DimensionString = testData.Dimension.ToString()
                }
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
    /// Get information about discovered composite types
    /// </summary>
    [HttpGet("types-info")]
    public IActionResult GetCompositeTypesInfo()
    {
        try
        {
            var types = new[]
            {
                typeof(Dim7),
                typeof(Rational)
            };

            var typeInfos = new List<object>();
            
            foreach (var type in types)
            {
                try
                {
                    var info = kvad_be.Extensions.PostgresComposite.PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo(type);
                    var fields = new List<object>();
                    
                    foreach (var field in info.Fields)
                    {
                        fields.Add(new
                        {
                            Name = field.Name,
                            ClrType = field.Type.Name,
                            PgType = field.PgType,
                            PropertyName = field.PropertyInfo.Name
                        });
                    }
                    
                    typeInfos.Add(new
                    {
                        ClrType = type.Name,
                        TypeName = info.TypeName,
                        Schema = info.Schema,
                        FullTypeName = info.FullTypeName,
                        AutoCreateType = info.AutoCreateType,
                        Fields = fields
                    });
                }
                catch (Exception ex)
                {
                    typeInfos.Add(new
                    {
                        ClrType = type.Name,
                        Error = ex.Message
                    });
                }
            }

            return Ok(new
            {
                Message = "Composite type information",
                Types = typeInfos
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = "Failed to get composite types info",
                Details = ex.Message
            });
        }
    }
}
