using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCBooksWebApi.Data;
using MVCBooksWebApi.Models;
using MVCBooksWebApi.Models.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBooksWebApi.Controllers
{
	public class BooksController : Controller
	{
		private readonly MvcBooksDbContext mvcBooksDbContext;

		public BooksController(MvcBooksDbContext mvcBooksDbContext)
		{
			this.mvcBooksDbContext = mvcBooksDbContext;
		}

		[HttpGet]
		public async Task<IActionResult> Index(string searchTitle)
		{
			if (!string.IsNullOrEmpty(searchTitle))
			{
				var matchingBooks = await mvcBooksDbContext.Books
					.Where(book => book.Title.Contains(searchTitle))
					.ToListAsync();

				if (matchingBooks.Any())
				{
					return View(matchingBooks);
				}
				else
				{
					ViewBag.ErrorMessage = "Nie znaleziono książki o takim tytule.";
				}
			}

			var books = await mvcBooksDbContext.Books.ToListAsync();
			return View(books);
		}

		[HttpGet]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(AddBookViewModel addBookRequest)
		{
			var book = new Book()
			{
				Id = Guid.NewGuid(),
				Title = addBookRequest.Title,
				Author = addBookRequest.Author,
				ISBN = addBookRequest.ISBN,
				Pages = addBookRequest.Pages,
				Category = addBookRequest.Category,
				Amount = addBookRequest.Amount
			};

			await mvcBooksDbContext.Books.AddAsync(book);
			await mvcBooksDbContext.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> View(Guid id)
		{
			var book = await mvcBooksDbContext.Books.FirstOrDefaultAsync(x => x.Id == id);

			if (book != null)
			{
				var viewModel = new UpdateBookViewModel()
				{
					Id = book.Id,
					Title = book.Title,
					Author = book.Author,
					ISBN = book.ISBN,
					Pages = book.Pages,
					Category = book.Category,
					Amount = book.Amount
				};
				return View("View", viewModel);
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> View(UpdateBookViewModel model)
		{
			var book = await mvcBooksDbContext.Books.FindAsync(model.Id);

			if (book != null)
			{
				book.Title = model.Title;
				book.Author = model.Author;
				book.ISBN = model.ISBN;
				book.Pages = model.Pages;
				book.Category = model.Category;
				book.Amount = model.Amount;

				await mvcBooksDbContext.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(UpdateBookViewModel model)
		{
			var book = await mvcBooksDbContext.Books.FindAsync(model.Id);

			if (book != null)
			{
				mvcBooksDbContext.Books.Remove(book);
				await mvcBooksDbContext.SaveChangesAsync();

				return RedirectToAction("Index");
			}

			return RedirectToAction("Index");
		}
	}
}