using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Books.Api.Interfaces
{
    public interface IBooksRepository
    {
        Task<IEnumerable<Entities.Book>> GetBooksAsync();
        Task<Entities.Book> GetBookAsync(Guid id);
        Task<IEnumerable<Entities.Book>> GetBooksAsync(IEnumerable<Guid> ids);
        void AddBook(Entities.Book book);
        Task<bool> SaveChangesAsync();
    }
}
