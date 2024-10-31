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
        var ret = AppCtx.GetPresetResManager().ReloadAllResources();
        return RespOk();
    }
}