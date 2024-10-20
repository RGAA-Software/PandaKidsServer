using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("book/suit")]
public class BookSuitController : PkBaseController
{
    private readonly BookSuitOperator _bookSuitOperator;

    public BookSuitController(AppContext ctx) : base(ctx) {
        AppContext = ctx;
        _bookSuitOperator = AppContext.GetDatabase().GetBookSuitOperator();
    }

    [HttpPost("create")]
    public IActionResult CreateSuit(IFormCollection form) {
        if (!form.TryGetValue(EntityKey.KeyName, out var name)) return RespError(ControllerError.ErrParamErr);

        return Ok();
    }

    [HttpPost("insert")]
    public IActionResult InsertBook(IFormCollection form) {
        foreach (var key in form.Keys) Console.WriteLine("key: " + key + ", val: " + form[key]);

        foreach (var file in form.Files) Console.WriteLine("file: " + file.Name + ", " + file.Length);

        return Ok();
    }

    [HttpPost("delete/{id}")]
    public IActionResult DeleteBook(int id) {
        if (id > 0)
            return Ok("");
        return NotFound();
    }

    [HttpGet("query/{id}")]
    public IActionResult QueryBook(string id) {
        var json = JsonConvert.SerializeObject(new Dictionary<string, string> {
            { "name", id }
        });
        return Ok(json);
    }

    [HttpGet("query")]
    public IActionResult QueryBooks() {
        return Ok();
    }
}