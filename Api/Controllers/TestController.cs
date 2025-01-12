using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet()]
    public IActionResult Index()
    {
        return Ok("Hello World");
    }
}

[ApiController]
[Route("api/v2/[controller]")]
public class Test2Controller : ControllerBase
{
    [HttpGet()]
    public IActionResult Index()
    {
        return Ok("Hello World v2");
    }
}