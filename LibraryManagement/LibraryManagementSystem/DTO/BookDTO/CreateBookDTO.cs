namespace LibraryManagementSystem.DTO.BookDTO
{
    public class CreateBookDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required int AuthorId { get; set; }
        public int? VendorId { get; set; }
        public int? PublishYear { get; set; }
        public int? PageNumber { get; set; }
        public required string Language { get; set; }
        public required string Version { get; set; }
        public int? SeriesId { get; set; }
        public required string ISBN { get; set; }
    }
}
