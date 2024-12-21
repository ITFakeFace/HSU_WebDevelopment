using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public partial class Age
{
    public int Id { get; set; }

    [Display(Name = "Từ tuổi")]
    [Required(ErrorMessage = "Từ tuổi bắt buộc phải có.")]
    public int? FromAge { get; set; }

    [Display(Name = "Đến tuổi")]
    [Required(ErrorMessage = "Đến tuổi bắt buộc phải có.")]
    public int? ToAge { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
