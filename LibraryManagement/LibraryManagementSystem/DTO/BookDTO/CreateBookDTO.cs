using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DTO.BookDTO
{
    public class CreateBookDTO
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required IEnumerable<int> CategoriesId { get; set; }
        public required IEnumerable<int>AuthorId { get; set; }
        public int? VendorId { get; set; }
        public int? PublishYear { get; set; }
        public int? PageNumber { get; set; }
        public required string Language { get; set; }
        public required string Version { get; set; }
        public int? SeriesId { get; set; }
        public required int PublisherID { get; set; }
        public required string ISBN { get; set; }
        public required List<IFormFile> BookImgs { get; set; }
    }
}
