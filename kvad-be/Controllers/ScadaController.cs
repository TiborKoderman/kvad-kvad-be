using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ScadaController : ControllerBase
{

private readonly AuthService _authService;

public ScadaController(AuthService authService)
{
    _authService = authService;
}

}