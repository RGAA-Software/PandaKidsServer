using Microsoft.AspNetCore.Mvc;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/recommend")]
public class RecommendController : PkBaseController
{
    // today's video suits
    private List<VideoSuit> _videoSuits = [];
    
    // today's book suits
    private List<BookSuit> _bookSuits = [];
    
    public RecommendController(AppContext ctx) : base(ctx) {
            
    }
    
    [HttpGet("today/videosuits")]
    public IActionResult GetTodayVideoSuits() {
        var totalCount = VideoSuitOp.CountTotalEntities();
        int recommendCount = 5;
        if (totalCount <= recommendCount) {
            var vs = VideoSuitOp.QueryEntities(1, recommendCount);
            return RespOkData(EntityKey.RespVideoSuits, vs);
        }
        else {
            //todo: UnTested
            var numbers = Common.Common.GenerateUniqueRandomNumbers(1, (int)totalCount);
            var targetVs = new List<VideoSuit>();
            foreach (var n in numbers) {
                var list = VideoSuitOp.QueryEntities(n, 1);
                if (list.Count > 0) {
                    targetVs.Add(list[0]);   
                }
            }
            return RespOkData(EntityKey.RespVideoSuits, targetVs);   
        }
    }
    
}