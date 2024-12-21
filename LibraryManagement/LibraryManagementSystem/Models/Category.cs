using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public partial class Category
{
    public int Id { get; set; }

    [Display(Name = "Thể loại")]
    [Required(ErrorMessage = "Thể loại bắt buộc phải có.")]
    public string? Name { get; set; }

    public int? Parent { get; set; }

    [Display(Name = "Trạng thái")]
    public int? Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
