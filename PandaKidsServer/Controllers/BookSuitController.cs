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
        AppContext = ctx;
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
        var entityId = GetFormValue(form, EntityKey.KeyId);
        var bookId = GetFormValue(form, EntityKey.KeyBookId);
        if (IsEmpty(entityId) || IsEmpty(bookId)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var bookSuit = BookSuitOp.FindEntityById(entityId!);
        if (bookSuit == null) {
            return RespError(ControllerError.ErrNoRecordInDb, "BookSuit:" + entityId);
        }
        var book = BookOp.FindEntityById(bookId!);
        if (book == null) {
            return RespError(ControllerError.ErrNoRecordInDb, "Book:" + bookId);
        }
        
        if (!bookSuit.BookIds.Contains(bookId!)) {
            bookSuit.BookIds.Add(bookId!);
        }
        if (!BookSuitOp.ReplaceEntity(bookSuit)) {
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

        var bookSuit = BookSuitOp.FindEntityById(entityId!);
        if (bookSuit == null) {
            return RespError(ControllerError.ErrNoRecordInDb, "BookSuit:" + entityId);
        }

        bookSuit.BookIds.Remove(bookId!);
        if (!BookSuitOp.ReplaceEntity(bookSuit)) {
            return RespError(ControllerError.ErrReplaceInDbFailed);
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
        FillInBookSuits(bookSuits);
        return RespOkData(EntityKey.RespBookSuits, bookSuits);
    }

    [HttpGet("query/like/name")]
    public IActionResult QueryBookSuitsLikeName() {
        string? name = Request.Query[EntityKey.KeyName];
        if (IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var bookSuits = BookSuitOp.QueryEntitiesLikeName(name!);
        FillInBookSuits(bookSuits);
        return RespOkData(EntityKey.RespBookSuits, bookSuits);
    }

    private void FillInBookSuits(List<BookSuit>? bookSuits) {
        if (bookSuits == null) {
            return;
        }
        foreach (var bookSuit in bookSuits) {
            foreach (var bookId in bookSuit.BookIds) {
                var book = BookOp.FindEntityById(bookId);
                if (book != null) {
                    bookSuit.Books.Add(book);
                }
            }
        }
    }
}