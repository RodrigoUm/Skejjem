using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using System.Threading.Tasks;

namespace Skejjem.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SkejjemController : ControllerBase
{
    private readonly IConfiguration config;
    public SkejjemController(IConfiguration config)
    {
        this.config = config;
    }
    // GET: SkejjumController
    [HttpPost(Name = "skejjem")]
    public async Task<ActionResult> Get([FromBody] TaskRequest request)
    {
        //string taskList = "Drink 2 coffees, Drink 6 cups of water, Eat healthy";
        var skejjem = new Skejjem(config);
        string schedule = await skejjem.GetRecommendedSchedule(request.TaskList);
        var jsonResponse = new
        {
            schedule = schedule
        };
        return Ok(jsonResponse);
    }
}
public class TaskRequest
{
    public string TaskList { get; set; }
}