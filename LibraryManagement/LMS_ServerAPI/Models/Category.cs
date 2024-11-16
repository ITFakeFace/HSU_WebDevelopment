using System;
using System.Collections.Generic;

namespace LMS_ServerAPI.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Parent { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
