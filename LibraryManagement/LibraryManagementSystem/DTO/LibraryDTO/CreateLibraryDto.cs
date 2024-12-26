namespace LibraryManagementSystem.DTO.LibraryDTO
{
    public class CreateLibraryDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public int? Manager { get; set; }

        public string Address { get; set; }

        public string Street { get; set; }

        public string Ward { get; set; }

        public string District { get; set; }

        public string City { get; set; }

        public string? Phone { get; set; }

        public TimeOnly? OpenFrom { get; set; }

        public TimeOnly? OpenTo { get; set; }

        public int? Status { get; set; }

    }
}
