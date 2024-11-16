using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Age
{
    public int Id { get; set; }

    public int? FromAge { get; set; }

    public int? ToAge { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
