using Microsoft.AspNetCore.Mvc;
using PandaKidsServer.DB.Entities;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/series")]
public class SeriesController : PkBaseController  
{
    public SeriesController(AppContext ctx) : base(ctx) {
        
    }
    
    [HttpGet("query")]
    public IActionResult QuerySeries() {
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize)) {
            return RespError(ControllerError.ErrParamErr);
        }
        
        var series = SeriesOp.QueryEntities(page, pageSize);
        return RespOkData(EntityKey.RespSeries, series);
    }

    [HttpGet("query/like/name")]
    public IActionResult QuerySeriesLikeName() {
        string? name = Request.Query[EntityKey.KeyName];
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize) || IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var series = SeriesOp.QueryEntitiesLikeName(name!, page, pageSize);
        return RespOkData(EntityKey.RespSeries, series);
    }
}