using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Series
{
    public int Id { get; set; }

    public int? Name { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
