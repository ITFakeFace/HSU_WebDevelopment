using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Vendor
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
