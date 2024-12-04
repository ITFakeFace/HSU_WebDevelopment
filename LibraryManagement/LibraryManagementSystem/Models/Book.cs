using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Book
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Language { get; set; }

    public int? Vendor { get; set; }

    public int? Publisher { get; set; }

    public int? PublishYear { get; set; }

    public int? PageNumber { get; set; }

    public string? Isbn { get; set; }

    public string? Version { get; set; }

    public byte[]? Image { get; set; }

    public int? Series { get; set; }
    
    public int? Status { get; set; }

    public virtual ICollection<BookImg> BookImgs { get; set; } = new List<BookImg>();

    public virtual ICollection<BookInBranch> BookInBranches { get; set; } = new List<BookInBranch>();

    public virtual ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();

    public virtual Publisher? PublisherNavigation { get; set; }

    public virtual Series? SeriesNavigation { get; set; }

    public virtual Vendor? VendorNavigation { get; set; }

    public virtual ICollection<Age> Ages { get; set; } = new List<Age>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
