using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class BookImg
{
    public int Id { get; set; }

    public int Book { get; set; }

    public byte[]? Image { get; set; }

    public virtual Book BookNavigation { get; set; } = null!;
}
