using Microsoft.AspNetCore.Mvc;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;
using Serilog;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;

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
        var inFile = form.Files.GetFile(EntityKey.KeyFile);
        if (inFile != null) {
            var inFilePath = Path.Combine(ResManager.GetBookAbsPath(), inFile.FileName);
            if (System.IO.File.Exists(inFilePath)) {
                //return RespError(ControllerError.ErrFileAlreadyExist);
            }
        }

        BasicPath pdfPath;
        // * pdf
        {
            var ret = await CopyBook(form, EntityKey.KeyFile);
            if (ret.Key != null) return ret.Key;
            pdfPath = ret.Val!;
        }

        BasicPath coverPath;
        // * cover
        {
            var ret = await CopyImage(form, EntityKey.KeyCoverFile);
            if (ret.Key != null) return ret.Key;
            coverPath = ret.Val!;
            // insert to db
            var entity = ImageOperator.InsertEntityIfNotExistByFile(new Image {
                Name = coverPath.Name,
                File = coverPath.RefPath,
            });
            if (entity == null) {
                return RespError(ControllerError.ErrFileAlreadyExist);
            }
            coverPath.Extra = entity.GetId();
        }

        var audioPaths = new List<BasicPath>();
        // audio
        {
            var audioFiles = FilterAudioFiles(form);
            foreach (var audioFile in audioFiles) {
                var ret = await CopyAudio(audioFile);
                if (ret.Key == null && ret.Val != null) {
                    var audioPath = ret.Val!;
                    audioPaths.Add(audioPath);
                    // insert to database
                    var entity = AudioOperator.InsertEntityIfNotExistByFile(new Audio {
                        Name = audioPath.Name,
                        File = audioPath.RefPath,
                    });
                    if (entity == null) {
                        Log.Error("Insert " + audioPath.Name + " to db failed");
                        continue;
                    }
                    audioPath.Extra = entity.GetId();
                }   
            }
        }

        var videoPaths = new List<BasicPath>();
        // video
        {
            var videoFiles = FilterVideoFiles(form);
            foreach (var videoFile in videoFiles) {
                var ret = await CopyVideo(videoFile);
                if (ret.Key == null && ret.Val != null) {
                    var videoPath = ret.Val!;
                    videoPaths.Add(videoPath);
                    // insert to database
                    var entity = VideoOperator.InsertEntityIfNotExistByFile(new Video {
                        Name = videoPath.Name,
                        File = videoPath.RefPath,
                    });
                    if (entity == null) {
                        Log.Error("Insert " + videoPath.Name + " to db failed");
                        continue;
                    }
                    videoPath.Extra = entity.GetId();
                }   
            }
        }

        var author = GetFormValue(form, EntityKey.KeyAuthor);
        var name = GetFormValue(form, EntityKey.KeyName);
        var summary = GetFormValue(form, EntityKey.KeySummary);
        var content = GetFormValue(form, EntityKey.KeyContent);
        var details = GetFormValue(form, EntityKey.KeyDetails);

        var book = new Book {
            Author = author,
            Cover = coverPath.RefPath,
            CoverId = coverPath.Extra == null ? "" : coverPath.Extra!,
            Name = name.Length <= 0 ? pdfPath.Name : name,
            Summary = summary,
            Content = content,
            Details = details,
            File = pdfPath.RefPath,
        };
        
        foreach (var audioPath in audioPaths) {
            if (audioPath.Extra != null) {
                book.AudioIds.Add(audioPath.Extra!);   
            }
        }

        foreach (var videoPath in videoPaths) {
            if (videoPath.Extra != null) {
                book.VideoIds.Add(videoPath.Extra!);
            }
        }
        
        var existBook = BookOperator.FindFilePath(book.File);
        if (existBook != null) {
            return RespError(ControllerError.ErrRecordAlreadyExist);
        }
        BookOperator.InsertEntity(book);
        return RespOkData(EntityKey.RespBook, book);
    }

    [HttpGet("query")]
    public async Task<IActionResult> QueryBooks() {
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var books = BookOperator.QueryEntities(page, pageSize);
        FillInBooks(books);
        
        return RespOkData(EntityKey.RespBooks, books);
    }

    [HttpGet("query/like/name")]
    public async Task<IActionResult> QueryBooksLikeName() {
        string? name = Request.Query[EntityKey.KeyName];
        if (IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var books = BookOperator.QueryEntities(name!);
        FillInBooks(books);
        return RespOkData(EntityKey.RespBooks, books);
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> DeleteBook(string id) {
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        return BookOperator.DeleteEntity(id) ? RespOk() : RespError(ControllerError.ErrDeleteFailed);
    }
    
    [HttpGet("count")]
    public async Task<IActionResult> CountBook() {
        return RespOkValue(BookOperator.CountTotalEntities());
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateBook(IFormCollection form) {

        return RespOk();
    }
    
    private void FillInBooks(List<Book>? books) {
        if (books == null) {
            return;
        }

        foreach (var book in books) {
            foreach (var bookAudioId in book.AudioIds) {
                var audio = AudioOperator.FindEntityById(bookAudioId);
                Console.WriteLine("id: " + bookAudioId + ", audio: " + audio);
                if (audio != null) {
                    book.Audios.Add(audio);
                }
            }

            foreach (var bookVideoId in book.VideoIds) {
                var video = VideoOperator.FindEntityById(bookVideoId);
                if (video != null) {
                    book.Videos.Add(video);
                }
            }
        }
    }
}