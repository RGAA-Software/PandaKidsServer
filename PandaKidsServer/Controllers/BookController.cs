using Microsoft.AspNetCore.Mvc;
using PandaKidsServer.Common;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("book")]
public class BookController : PkBaseController
{
    public BookController(AppContext ctx) : base(ctx) {
        AppContext = ctx;
    }

    [HttpPost("insert")]
    public async Task<IActionResult> CreateBook(IFormCollection form) {
        BasicType.Paths pdfPaths;
        // * pdf
        {
            var ret = await CopyBook(form, EntityKey.KeyFile);
            if (ret.Key != null) return ret.Key;
            pdfPaths = ret.Val!;
        }

        BasicType.Paths coverPaths;
        // * cover
        {
            var ret = await CopyImage(form, EntityKey.KeyCoverFile);
            if (ret.Key != null) return ret.Key;
            coverPaths = ret.Val!;
        }

        BasicType.Paths? audioPaths = null;
        // audio
        {
            var ret = await CopyAudio(form, EntityKey.KeyAudioFile);
            if (ret.Key == null && ret.Val != null) {
                audioPaths = ret.Val!;
                // insert to database
            }
        }

        BasicType.Paths? videoPaths = null;
        // video
        {
            var ret = await CopyVideo(form, EntityKey.KeyVideoFile);
            if (ret.Key == null && ret.Val != null) {
                videoPaths = ret.Val!;
                // insert to database
            }
        }

        var author = GetFormValue(form, EntityKey.KeyAuthor);
        var name = GetFormValue(form, EntityKey.KeyName);
        var summary = GetFormValue(form, EntityKey.KeySummary);
        var content = GetFormValue(form, EntityKey.KeyContent);
        var details = GetFormValue(form, EntityKey.KeyDetails);

        var book = new Book {
            Author = author,
            CoverPath = coverPaths.RefPath,
            Name = name.Length <= 0 ? pdfPaths.Name : name,
            Summary = summary,
            Content = content,
            Details = details,
            FilePath = pdfPaths.RefPath,

        };
        var existBook = BookOperator.FindBookByPdfPath(book.FilePath);
        if (existBook != null) {
            return RespError(ControllerError.ErrAlreadyExist);
        }
        BookOperator.InsertEntity(book);
        return RespOkData(EntityKey.RespBook, book);
    }
}