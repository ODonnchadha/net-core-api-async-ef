using Books.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Books.Api.Contexts
{
    public class BooksContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public BooksContext(DbContextOptions<BooksContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Author>().HasData(
                new Author
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    FirstName = "Flann",
                    LastName = "O'Brien"
                },
                new Author
                {
                    Id = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                    FirstName = "James",
                    LastName = "Joyce"
                },
                new Author
                {
                    Id = Guid.Parse("24810dfc-2d94-4cc7-aab5-cdf98b83f0c9"),
                    FirstName = "G.K.",
                    LastName = "Chesterton"
                },
                new Author
                {
                    Id = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                    FirstName = "Bernard",
                    LastName = "Shaw"
                });

            builder.Entity<Book>().HasData(
                new Book
                {
                    Id = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
                    AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    Title = "The Third Policeman",
                    Description = "The Third Policeman has a fantastic plot of a murderous protagonist let loose on a strange world peopled by fat policemen played against a satire of academic debate on an eccentric philosopher called De Selby. Sergeant Pluck introduces the atomic theory of the bicycle."
                },
                new Book
                {
                    Id = Guid.Parse("d8663e5e-7494-4f81-8739-6e0de1bea7ee"),
                    AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    Title = "At Swim-Two-Birds",
                    Description = "At Swim-Two-Birds works entirely with borrowed (and stolen) characters from other fiction and legend, on the grounds that there are already far too many existing fictional characters."
                },
                new Book
                {
                    Id = Guid.Parse("d173e20d-159e-4127-9ce9-b0ac2564ad97"),
                    AuthorId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                    Title = "A Portrait of the Artist as a Young Man",
                    Description = "A Künstlerroman in a modernist style, this novel traces the religious and intellectual awakening of young Stephen Dedalus, a fictional alter ego of Joyce and an allusion to Daedalus, the consummate craftsman of Greek mythology."
                },
                new Book
                {
                    Id = Guid.Parse("493c3228-3444-4a49-9cc0-e8532edc59b2"),
                    AuthorId = Guid.Parse("24810dfc-2d94-4cc7-aab5-cdf98b83f0c9"),
                    Title = "The Man Who Was Thursday",
                    Description = "The book is sometimes referred to as a metaphysical thriller. The work is prefixed with a poem written to Edmund Clerihew Bentley, revisiting the pair's early history and the challenges presented to their early faith by the times."
                },
                new Book
                {
                    Id = Guid.Parse("40ff5488-fdab-45b5-bc3a-14302d59869a"),
                    AuthorId = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                    Title = "Misalliance",
                    Description = "Misalliance is an ironic examination of the mating instincts of a varied group of people gathered at a wealthy man's country home on a summer weekend."
                });

            base.OnModelCreating(builder);
        }
    }
}
