using Books.Api.ExternalModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Books.Api.Interfaces
{
    public interface IBookCoversRepository
    {
        Task<BookCover> GetBookCoverAsync(string coverId);
        Task<IEnumerable<BookCover>> GetBookCoversAsync(Guid bookId);
    }
}
