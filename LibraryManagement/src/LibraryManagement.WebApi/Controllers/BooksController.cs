using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.Models;
using LibraryManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.WebApi.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<BookModel>>> GetAllAsync()
    {
        var result = await _bookService.GetAllAsync();

        return ToActionResult(result);
    }

    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyCollection<BookModel>>> GetAvailableAsync()
    {
        var result = await _bookService.GetAvailableAsync();

        return ToActionResult(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyCollection<BookModel>>> SearchAsync([FromQuery] string term)
    {
        var result = await _bookService.SearchAsync(new BookQueryRequest { SearchTerm = term });

        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<BookModel>> AddAsync([FromBody] CreateBookRequest request)
    {
        var result = await _bookService.AddAsync(request);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Created("/api/books", result.Value);
    }

    [HttpPut("{code}")]
    public async Task<ActionResult<BookModel>> UpdateAsync(
        string code,
        [FromBody] UpdateBookRequest request)
    {
        var result = await _bookService.UpdateAsync(code, request);

        return ToActionResult(result);
    }

    [HttpDelete("{code}")]
    public async Task<IActionResult> DeleteAsync(string code)
    {
        var result = await _bookService.DeleteAsync(code);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPost("{code}/borrow")]
    public async Task<IActionResult> BorrowAsync(string code)
    {
        var result = await _bookService.BorrowAsync(code);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPost("{code}/return")]
    public async Task<IActionResult> ReturnAsync(string code)
    {
        var result = await _bookService.ReturnAsync(code);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    private ActionResult<T> ToActionResult<T>(Result<T> result)
    {
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}

