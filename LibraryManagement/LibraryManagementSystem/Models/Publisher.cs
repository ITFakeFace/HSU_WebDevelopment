using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public partial class Publisher
{
    public int Id { get; set; }

    [Display(Name = "Nhà xuất bản")]
    [Required(ErrorMessage = "Nhà xuất bản bắt buộc phải có.")]
    public string? Name { get; set; }

    public int? Parent { get; set; }

    [Display(Name = "Trạng thái")]
    public int? Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<Publisher> InverseParentNavigation { get; set; } = new List<Publisher>();

    public virtual Publisher? ParentNavigation { get; set; }
}
