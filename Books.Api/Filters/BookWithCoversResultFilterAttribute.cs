using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Books.Api.Filters
{
    public class BookWithCoversResultFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultFromAction = context.Result as ObjectResult;

            if (resultFromAction?.Value == null 
                || resultFromAction.StatusCode < 200 
                || resultFromAction.StatusCode >= 300)
            {
                await next();
                return;
            }

            var (book, covers) = ((Entities.Book, IEnumerable<ExternalModels.BookCover>))resultFromAction.Value;

            var mappedBook = Mapper.Map<Models.BookWithCovers>(book);

            resultFromAction.Value = Mapper.Map(covers, mappedBook);

            await next();
        }
    }
}
