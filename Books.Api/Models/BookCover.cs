namespace Books.Api.Models
{
    public class BookCover
    {
        public string Name { get; set; }

        // Manipulate the return. We still fetch the bytes[], but we no longer return them from the API.

        // public byte[] Content { get; set; }
    }
}
