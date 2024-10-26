using Microsoft.AspNetCore.Mvc;
using PandaKidsServer.Common;
using PandaKidsServer.DB.Entities;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/resources")]
public class ResourcesController(AppContext ctx) : PkBaseController(ctx)  {

    [HttpGet("reload/all")]
    public async Task<IActionResult> ReloadResources() {
        var ret = await AppCtx.GetPresetResManager().ReloadAllResources();
        return RespOk();
    }
    
    [HttpGet("reload/audios")]
    public async Task<IActionResult> ReloadAudios() {
        var ret = await AppCtx.GetPresetResManager().ReloadAudios();
        return RespOk();
    }

    [HttpGet("reload/videos")]
    public async Task<IActionResult> ReloadVideos() {
        var ret = await AppCtx.GetPresetResManager().ReloadVideos();
        return RespOk();
    }
    
    [HttpGet("reload/books")]
    public async Task<IActionResult> ReloadBooks() {
        var ret = await AppCtx.GetPresetResManager().ReloadAudios();
        return RespOk();
    }
    
    [HttpGet("reload/images")]
    public async Task<IActionResult> ReloadImages() {
        var ret = await AppCtx.GetPresetResManager().ReloadAudios();
        return RespOk();
    }
}