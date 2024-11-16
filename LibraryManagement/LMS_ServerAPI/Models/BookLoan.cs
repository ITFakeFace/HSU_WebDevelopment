using System;
using System.Collections.Generic;

namespace LMS_ServerAPI.Models;

public partial class BookLoan
{
    public int Id { get; set; }

    public string User { get; set; } = null!;

    public int Book { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int? IsReturned { get; set; }

    public int? Status { get; set; }

    public virtual Book BookNavigation { get; set; } = null!;

    public virtual User UserNavigation { get; set; } = null!;
}
