namespace Books.Api.ExternalModels
{
    /// <summary>
    /// ExternalModels: Neither entity nor outer-facing DTOs
    /// </summary>
    public class BookCover
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
