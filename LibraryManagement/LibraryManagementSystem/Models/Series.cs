using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public partial class Series
{
    public int Id { get; set; }

    [Display(Name = "Bộ sách")]
    [Required(ErrorMessage = "Bộ sách bắt buộc phải có.")]
    public string? Name { get; set; }

    [Display(Name = "Trạng thái")]
    public int? Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
