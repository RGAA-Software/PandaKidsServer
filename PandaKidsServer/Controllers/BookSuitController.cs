using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;
namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/booksuit")]
public class BookSuitController : PkBaseController
{
    public BookSuitController(AppContext ctx) : base(ctx) {
        AppCtx = ctx;
    }

    [HttpPost("insert")]
    public async Task<IActionResult> InsertBookSuit(IFormCollection form) {
        // name
        var name = GetFormValue(form, EntityKey.KeyName);
        // summary
        var summary = GetFormValue(form, EntityKey.KeySummary);
        // details
        var details = GetFormValue(form, EntityKey.KeyDetails);

        if (IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }
        
        var bookSuit = BookSuitOp.FindEntityByName(name!);
        
        // cover
        BasicPath? coverPath = null;
        var ret = await CopyImage(form, EntityKey.KeyCoverFile);
        if (ret.Val != null) {
            coverPath = ret.Val!;
            // insert to db
            var entity = ImageOp.InsertEntityIfNotExistByFile(new Image {
                Name = coverPath.Name,
                File = coverPath.RefPath,
            });
            if (entity != null) {
                coverPath.Extra = entity.GetId();
            }
        }
        
        // add new one
        if (bookSuit == null) {
            var newBookSuit = new BookSuit {
                Name = name!,
                Summary = summary ?? "",
                Details = details ?? "",
                Cover = coverPath != null ? coverPath!.RefPath : "",
                CoverId = coverPath is { Extra: not null } ? coverPath.Extra! : "",
            };

            if (BookSuitOp.InsertEntity(newBookSuit) == null) {
                return RespError(ControllerError.ErrInsertToDbFailed);
            }

            return RespOk();
        }
        else {
            // update exists one
            bookSuit.Name = name!;
            if (summary != null) {
                bookSuit.Summary = summary;
            }
            if (details != null) {
                bookSuit.Details = details;
            }
            if (coverPath != null) {
                bookSuit.Cover = coverPath.RefPath;
                if (coverPath.Extra != null) {
                    bookSuit.CoverId = coverPath.Extra!;
                }
            }
            
            if (!BookSuitOp.ReplaceEntity(bookSuit)) {
                return RespError(ControllerError.ErrReplaceInDbFailed);
            }
            return RespOk();
        }
    }

    [HttpPost("delete")]
    public IActionResult DeleteBookSuit() {
        string? id = Request.Query[EntityKey.KeyId];
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        return BookSuitOp.DeleteEntity(id!) ? RespOk() : RespError(ControllerError.ErrDeleteFailed);
    }

    [HttpPost("add/book")]
    public IActionResult AddBook(IFormCollection form) {
        var bookSuitId = GetFormValue(form, EntityKey.KeyId);
        var bookId = GetFormValue(form, EntityKey.KeyBookId);
        if (IsEmpty(bookSuitId) || IsEmpty(bookId)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var bookSuit = BookSuitOp.FindEntityById(bookSuitId!);
        if (bookSuit == null) {
            return RespError(ControllerError.ErrNoRecordInDb, "BookSuit:" + bookSuitId);
        }
        var book = BookOp.FindEntityById(bookId!);
        if (book == null) {
            return RespError(ControllerError.ErrNoRecordInDb, "Book:" + bookId);
        }
        book.BookSuitId = bookSuitId!;
        
        if (!BookOp.ReplaceEntity(book)) {
            return RespError(ControllerError.ErrReplaceInDbFailed);
        }
        
        return RespOk();
    }

    [HttpPost("delete/book")]
    public IActionResult DeleteBook(IFormCollection form) {
        var entityId = GetFormValue(form, EntityKey.KeyId);
        var bookId = GetFormValue(form, EntityKey.KeyBookId);
        if (IsEmpty(entityId) || IsEmpty(bookId)) {
            return RespError(ControllerError.ErrParamErr, "Entity id:" + entityId + ", Book id:" + bookId);
        }

        var book = BookOp.FindEntityById(bookId!);
        if (book != null) {
            BookOp.DeleteEntity(bookId!);
        }
        return RespOk();
    }

    [HttpGet("query")]
    public IActionResult QueryBookSuits() {
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var bookSuits = BookSuitOp.QueryEntities(page, pageSize);
        return RespOkData(EntityKey.RespBookSuits, bookSuits);
    }

    [HttpGet("query/like/name")]
    public IActionResult QueryBookSuitsLikeName() {
        string? name = Request.Query[EntityKey.KeyName];
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize) || IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var bookSuits = BookSuitOp.QueryEntitiesLikeName(name!, page, pageSize);
        return RespOkData(EntityKey.RespBookSuits, bookSuits);
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