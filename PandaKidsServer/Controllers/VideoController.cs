using Microsoft.AspNetCore.Mvc;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/video")]
public class VideoController(AppContext appContext) : PkBaseController(appContext)  {

    [HttpPost("insert")]
    public async Task<IActionResult> InsertVideo(IFormCollection form) {

        return RespOk();
    }

    [HttpPost("delete")]
    public IActionResult DeleteVideo(IFormCollection form) {
        return RespOk();
    }

    [HttpPost("update")]
    public IActionResult UpdateVideo(IFormCollection form) {
        // categories
        return RespOk();
    }

    [HttpGet("query")]
    public IActionResult QueryVideos() {
        return RespOk();
    }

    [HttpGet("query/like/name")]
    public IActionResult QueryVideosLikeName() {
        return RespOk();
    }

    [HttpPost("add/categories")]
    public IActionResult AddCategories(IFormCollection form) {
        return RespOk();
    }
    
    [HttpGet("query/category")]
    public IActionResult QueryByCategory() {
        return RespOk();
    }
}