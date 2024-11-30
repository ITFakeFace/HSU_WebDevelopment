using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public partial class Author
{
    public int Id { get; set; }

    [Display(Name = "Tên Tác giả")]
    [Required(ErrorMessage = "Tên tác giả bắt buộc phải có.")]
    public string? Name { get; set; }

    [Display(Name = "Giới tính")]
    public int? Gender { get; set; }

    [Display(Name = "Quốc tịch")]
    public string? Nation { get; set; }

    [Display(Name = "Trạng thái")]
    public int? Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
