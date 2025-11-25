using AuthApi.Enums;
using AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Controllers
{
    public static class BookEndPoints
    {
        public static IEndpointRouteBuilder MapBookEndPoints(this IEndpointRouteBuilder app)
        {

            app.MapPost("/create", async (AppDbContext db, [FromBody] BookCreateModel model) =>
            {
                if (model is null)
                    return Results.BadRequest("Invalid payload");

                // Try convert string to Genre enum
                if (!Enum.TryParse<Genre>(model.Genre, true, out var parsedGenre))
                {
                    var allowed = string.Join(", ", Enum.GetNames(typeof(Genre)));
                    return Results.BadRequest(new
                    {
                        error = "Invalid genre",
                        message = $"Allowed values are: {allowed}"
                    });
                }

                var book = new Book
                {
                    Name = model.Name,
                    ISBN = model.ISBN,
                    DatePublished = model.DatePublished,
                    Author = model.Author,
                    Genre = parsedGenre
                };

                db.Books.Add(book);
                var affected = await db.SaveChangesAsync();

                if (affected > 0)
                    return Results.Ok(new { message = "Book Created Successfully", book });

                return Results.BadRequest("No changes saved");
            });

            app.MapGet("/", async (AppDbContext db) =>
            {
                var books = await db.Books.ToListAsync();
                return Results.Ok(books);
            }).RequireAuthorization();

            app.MapGet("/{id:int}", async (AppDbContext db, int id) =>
            {
                var book = await db.Books.FindAsync(id);

                if (book is null)
                    return Results.NotFound(new { error = $"Book with ID {id} not found" });

                return Results.Ok(book);
            });
            app.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
            {
                var book = await db.Books.FindAsync(id);

                if (book is null)
                    return Results.NotFound(new { error = $"Book with ID {id} not found" });

                db.Books.Remove(book);
                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Book deleted successfully", deletedBook = book });
            }).RequireAuthorization();


            app.MapPut("/update/{id:int}", async (AppDbContext db, int id, BookCreateModel model) =>
            {
                var book = await db.Books.FindAsync(id);

                if (book is null)
                    return Results.NotFound(new { error = $"Book with ID {id} not found" });

                // If Genre provided, convert it
                if (!string.IsNullOrWhiteSpace(model.Genre))
                {
                    if (!Enum.TryParse<Genre>(model.Genre, true, out var parsedGenre))
                    {
                        var allowed = string.Join(", ", Enum.GetNames(typeof(Genre)));
                        return Results.BadRequest(new
                        {
                            error = "Invalid genre",
                            message = $"Allowed values are: {allowed}"
                        });
                    }

                    book.Genre = parsedGenre;
                }

                // Update others only if provided
                if (!string.IsNullOrWhiteSpace(model.Name))
                    book.Name = model.Name;

                if (!string.IsNullOrWhiteSpace(model.ISBN))
                    book.ISBN = model.ISBN;

                if (model.DatePublished != default)
                    book.DatePublished = model.DatePublished;

                if (!string.IsNullOrWhiteSpace(model.Author))
                    book.Author = model.Author;

                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Book updated successfully", book });
            }).RequireAuthorization();



            return app;

        }
    }
}
