using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public partial class Vendor
{
    public int Id { get; set; }

    [Display(Name = "Tên nhà cung cấp")]
    [Required(ErrorMessage = "Tên nhà cung cấp bắt buộc phải có.")]
    public string? Name { get; set; }

    public string? Email { get; set; }

    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [Display(Name = "Trạng thái")]
    public int? Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
