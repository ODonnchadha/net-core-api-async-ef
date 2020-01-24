using AutoMapper;
using Books.Api.ExternalModels;
using Books.Api.Filters;
using Books.Api.Interfaces;
using Books.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Books.Api.Controllers
{
    [ApiController(), Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IBookCoversRepository clientRepository;
        private readonly IBooksRepository repository;

        public BooksController(
            IMapper mapper, 
            IBookCoversRepository clientRepository, 
            IBooksRepository repository)
        {
            this.mapper = mapper;
            this.clientRepository = clientRepository;
            this.repository = repository;
        }

        [HttpGet(), BooksResultFilter()]
        public async Task<IActionResult> GetBooks()
        {
            var entites = await repository.GetBooksAsync();
            return Ok(entites);
        }

        // BookResultFilter()
        [HttpGet(), Route("{id}", Name="GetBook"), BookWithCoversResultFilter()]
        public async Task<IActionResult> GetBook(Guid id)
        {
            var entity = await repository.GetBookAsync(id);

            if (null == entity)
            {
                return NotFound();
            }

            var bookCovers = await clientRepository.GetBookCoversAsync(id);

            return Ok((entity, bookCovers));
        }

        [HttpPost(), BookResultFilter()]
        public async Task<IActionResult> CreateBook([FromBody]BookForCreation book)
        {
            var bookEntity = mapper.Map<Entities.Book>(book);

            repository.AddBook(bookEntity);
            await repository.SaveChangesAsync();

            // Fetch all of the books so that the 'mapped' author is returned.
            await repository.GetBookAsync(bookEntity.Id);

            return CreatedAtRoute("GetBook", new { id = bookEntity.Id }, bookEntity);
        }
    }
}