using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("book")]
public class BookController : PkBaseController
{
    private readonly BookOperator _bookOperator;
    
    public BookController(AppContext ctx) : base(ctx)
    {
        _appContext = ctx;
        _bookOperator = _database.GetBookOperator();
    }
    
    [HttpPost("insert")]
    public async Task<IActionResult> CreateBook(IFormCollection form)
    {
        // * pdf
        {
            var ret = await CopyBook(form, EntityKey.KeyPdfFile);
            if (ret.Key != null)
            {
                return ret.Key;
            }
            var paths = ret.Val;
            // insert to database
            Console.WriteLine("copy to: " + (paths != null ? paths.ToString(): "xx"));
        }
        // * cover
        {
            var ret = await CopyImage(form, EntityKey.KeyCoverFile);
            if (ret.Key != null)
            {
                return ret.Key;
            }
            var paths = ret.Val;
            Console.WriteLine("copy to: " + (paths != null ? paths.ToString(): "xx"));
        }
        // audio
        {
            var ret = await CopyAudio(form, EntityKey.KeyAudioFile);
            if (ret.Key == null && ret.Val != null) // OK
            {
                
            }
        }
        // video
        {
            var ret = await CopyVideo(form, EntityKey.KeyVideoFile);
            if (ret.Key == null && ret.Val != null) // OK
            {
                
            }
        }
        
        if (!form.TryGetValue(EntityKey.KeyName, out var name)
            || !form.TryGetValue(EntityKey.KeySummary, out var summary))
        {
            return RespError(ControllerError.ErrParamErr);
        }

        _bookOperator.InsertEntity(new Book
        {

        });
        
        return RespOk();
    }
}