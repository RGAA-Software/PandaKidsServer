using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("video")]
public class VideoController(AppContext ctx) : ControllerBase
{
    private readonly AppContext _appContext = ctx;

    [HttpPost("insert")]
    public IActionResult InsertVideo(IFormCollection form)
    {
        foreach (var key in form.Keys)
        {
            Console.WriteLine("key: " + key + ", val: " + form[key]);
        }
        
        foreach (var file in form.Files)
        {
            Console.WriteLine("file: " + file.Name + ", " + file.Length);
        }
    
        return Ok();
    }
    
    [HttpPost("delete/{id}")]
    public IActionResult DeleteVideo(int id)
    {
        if (id > 0)
        {
            return Ok("");
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("query/{id}")]
    public IActionResult QueryVideo(string id)
    {
        var json = JsonConvert.SerializeObject(new Dictionary<string, string>()
        {
            { "name", id }
        });
        return Ok(json);
    }
    
    [HttpGet("query")]
    public IActionResult QueryVideos()
    {
        return Ok();
    }
}