using Microsoft.EntityFrameworkCore;
using MVCBooksWebApi.Models.Domain;

namespace MVCBooksWebApi.Data

{
	public class MvcBooksDbContext : DbContext
	{
		public MvcBooksDbContext(DbContextOptions options) : base(options)
		{
		}

        public DbSet<Book> Books { get; set; }
    }
}
