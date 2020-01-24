using AutoMapper;
using Books.Api.Binders;
using Books.Api.Filters;
using Books.Api.Interfaces;
using Books.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Controllers
{
    [ApiController(), Route("api/bookcollections"), BooksResultFilter()]
    public class BookCollectionsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IBooksRepository repository;
        public BookCollectionsController(IMapper mapper, IBooksRepository repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        /// <summary>
        /// e.g.: api/bookcollections/(id1,id2)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("({bookIds})", Name="GetBookCollection")]
        public async Task<IActionResult> GetBookCollection([ModelBinder(BinderType=typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            var entities = await repository.GetBooksAsync(ids);

            if (ids.Count() != entities.Count())
            {
                return NotFound();
            }

            return Ok(entities);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateBookCollection([FromBody]IEnumerable<BookForCreation> books)
        {
            var bookEntities = mapper.Map<IEnumerable<Entities.Book>>(books);

            foreach(var bookEntity in bookEntities)
            {
                repository.AddBook(bookEntity);
            }
            await repository.SaveChangesAsync();

            // Fetch all of the books so that the 'mapped' author is returned.
            var booksToReturn = await repository.GetBooksAsync(bookEntities.Select(b => b.Id).ToList());
            var bookIds = string.Join(",", booksToReturn.Select(b => b.Id));

            return CreatedAtRoute("GetBookCollection", new { ids = bookIds }, booksToReturn);
        }
    }
}