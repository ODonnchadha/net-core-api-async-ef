using AutoMapper;
using System.Collections.Generic;

namespace Books.Api.Mappings
{
    /// <summary>
    /// AutoMapper: Organize mapping configurations with 'profiles.'
    /// </summary>
    public class BooksProfile : Profile
    {
        public BooksProfile()
        {
            CreateMap<Entities.Book, Models.Book>()
                .ForMember(destination => destination.Author, options => options
                .MapFrom(source => $"{source.Author.FirstName} {source.Author.LastName}"));

            CreateMap<Models.BookForCreation, Entities.Book>();

            // Map Book Entity. Then Map Covers.
            CreateMap<Entities.Book, Models.BookWithCovers>()
                .ForMember(destination => destination.Author, options => options
                .MapFrom(source => $"{source.Author.FirstName} { source.Author.LastName}"));

            CreateMap<IEnumerable<ExternalModels.BookCover>, Models.BookWithCovers>()
                .ForMember(destination => destination.BookCovers, options => options
                .MapFrom(source => source));
        }
    }
}
