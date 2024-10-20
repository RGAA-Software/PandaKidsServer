using Microsoft.AspNetCore.Mvc;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("image/suit")]
public class ImageSuitController(AppContext ctx) : ControllerBase
{
    private readonly AppContext _appContext = ctx;
}