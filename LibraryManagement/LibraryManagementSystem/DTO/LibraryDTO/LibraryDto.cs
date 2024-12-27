namespace LibraryManagementSystem.DTO.LibraryDTO
{
    public class LibraryDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string Address { get; set; }

        public string? Phone { get; set; }

        public TimeOnly? OpenFrom { get; set; }

        public TimeOnly? OpenTo { get; set; }

        public int? Status { get; set; }
    }
}
