using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.Models;

namespace LibraryManagement.WebApi.Endpoints;

public static class BookEndpoints
{
    public static WebApplication MapBookEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/books").WithTags("Books");

        group.MapGet("/", async (
                string? status,
                string? search,
                IBookService bookService,
                CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyCollection<BookModel>> result;

            if (!string.IsNullOrWhiteSpace(search))
            {
                result = await bookService.SearchAsync(search, cancellationToken);
            }
            else if (string.Equals(status, "available", StringComparison.OrdinalIgnoreCase))
            {
                result = await bookService.GetAvailableAsync(cancellationToken);
            }
            else
            {
                result = await bookService.GetAllAsync(cancellationToken);
            }

            return ToHttpResult(result);
        });

        group.MapPost("/", async (
            CreateBookRequest request,
            IBookService bookService,
            CancellationToken cancellationToken) =>
        {
            var result = await bookService.AddAsync(request, cancellationToken);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Created("/api/books", result.Value);
        });

        group.MapPut("/{code}", async (
            string code,
            UpdateBookRequest request,
            IBookService bookService,
            CancellationToken cancellationToken) =>
        {
            var result = await bookService.UpdateAsync(code, request, cancellationToken);

            return ToHttpResult(result);
        });

        group.MapDelete("/{code}", async (
            string code,
            IBookService bookService,
            CancellationToken cancellationToken) =>
        {
            var result = await bookService.DeleteAsync(code, cancellationToken);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.NoContent();
        });

        group.MapPost("/{code}/borrow", async (
            string code,
            IBookService bookService,
            CancellationToken cancellationToken) =>
        {
            var result = await bookService.BorrowAsync(code, cancellationToken);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.NoContent();
        });

        group.MapPost("/{code}/return", async (
            string code,
            IBookService bookService,
            CancellationToken cancellationToken) =>
        {
            var result = await bookService.ReturnAsync(code, cancellationToken);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.NoContent();
        });

        return app;
    }

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        if (result.IsFailure)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Ok(result.Value);
    }
}
