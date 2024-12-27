namespace LibraryManagementSystem.DTO.BookLoanDTO
{
    public class CreateBookLoanDto
    {
        public string User { get; set; }
        public int Library { get; set; }
        public int Book { get; set; }
    }
}
