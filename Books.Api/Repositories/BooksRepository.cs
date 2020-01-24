using Books.Api.Contexts;
using Books.Api.Entities;
using Books.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Repositories
{
    public class BooksRepository : IBooksRepository, IDisposable
    {
        private BooksContext context;
        public BooksRepository(BooksContext context) => this.context = context;
        public async Task<Book> GetBookAsync(Guid id)
        {
            // Off-load long-running synchronous task to another thread.
            var bookPages = await GetBookPages();

            return await context.Books.Include(
                b => b.Author).FirstOrDefaultAsync(
                b => b.Id == id);
        }

        private Task<int> GetBookPages()
        {
            return Task.Run(() =>
            {
                // Legacy code. Used to illustrate async with legacy processes.
                var pageCalculator = new Legacy.ComplicatedPageCalculator { };

               return pageCalculator.CalculateBookPages();
           });
        }

        public async Task<IEnumerable<Book>> GetBooksAsync(IEnumerable<Guid> ids) => await context.Books.Where(b => ids.Contains(b.Id)).Include(b => b.Author).ToListAsync();
        public async Task<IEnumerable<Book>> GetBooksAsync() => await context.Books.Include(b => b.Author).ToListAsync();
        public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() > 0;
        public void AddBook(Book book)
        {
            if (null == book)
            {
                throw new ArgumentNullException(nameof(book));
            }

            context.Add(book);
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.context != null)
                {
                    this.context.Dispose();
                    this.context = null;
                }
            }
        }
        #endregion
    }
}
