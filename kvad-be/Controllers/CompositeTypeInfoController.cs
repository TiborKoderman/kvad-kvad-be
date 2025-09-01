using Microsoft.AspNetCore.Mvc;

namespace kvad_be.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompositeTypeInfoController : ControllerBase
{
    /// <summary>
    /// Test endpoint to verify auto-inference is working
    /// </summary>
    [HttpGet("dim7-info")]
    public IActionResult GetDim7Info()
    {
        try
        {
            var info = kvad_be.Extensions.PostgresComposite.PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo(typeof(Dim7));
            
            return Ok(new
            {
                Message = "Auto-inference working for Dim7!",
                TypeName = info.TypeName,
                Schema = info.Schema,
                FullTypeName = info.FullTypeName,
                AutoCreateType = info.AutoCreateType,
                Fields = info.Fields.Select(f => new
                {
                    Name = f.Name,
                    ClrType = f.Type.Name,
                    PgType = f.PgType,
                    PropertyName = f.PropertyInfo.Name,
                    IsAutoInferred = f.Name == f.PropertyInfo.Name && f.PgType == "SMALLINT"
                }).ToArray()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = "Failed to get Dim7 composite type info",
                Details = ex.Message
            });
        }
    }

    /// <summary>
    /// Test endpoint to verify auto-inference is working for Rational
    /// </summary>
    [HttpGet("rational-info")]
    public IActionResult GetRationalInfo()
    {
        try
        {
            var info = kvad_be.Extensions.PostgresComposite.PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo(typeof(Rational));
            
            return Ok(new
            {
                Message = "Auto-inference working for Rational!",
                TypeName = info.TypeName,
                Schema = info.Schema,
                FullTypeName = info.FullTypeName,
                AutoCreateType = info.AutoCreateType,
                Fields = info.Fields.Select(f => new
                {
                    Name = f.Name,
                    ClrType = f.Type.Name,
                    PgType = f.PgType,
                    PropertyName = f.PropertyInfo.Name,
                    IsAutoInferred = f.Name == f.PropertyInfo.Name && f.PgType == "BIGINT"
                }).ToArray()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = "Failed to get Rational composite type info",
                Details = ex.Message
            });
        }
    }
}
