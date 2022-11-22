using Microsoft.AspNetCore.Mvc;
using RosanjinGenerator.Services;

namespace RosanjinGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RosanjinController : ControllerBase
{
    readonly RosanjinService rosanjinService;
    public RosanjinController(RosanjinService rosanjinService)
    {
        this.rosanjinService = rosanjinService;
    }

    [HttpGet]
    public IEnumerable<string> Get()
    {
        return rosanjinService.Generate(3);
    }
}