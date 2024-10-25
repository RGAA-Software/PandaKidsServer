using Microsoft.AspNetCore.Mvc;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/imagesuit")]
public class ImageSuitController(AppContext ctx) : PkBaseController(ctx)
{
    private readonly AppContext _appContext = ctx;
    
    [HttpPost("add/categories")]
    public IActionResult AddCategories(IFormCollection form) {
        return RespOk();
    }
    
    [HttpGet("query/category")]
    public IActionResult QueryByCategory() {
        return RespOk();
    }
    
}