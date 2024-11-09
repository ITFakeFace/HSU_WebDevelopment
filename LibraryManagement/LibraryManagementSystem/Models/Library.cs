using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Library
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Address { get; set; }

    public string? Phone { get; set; }

    public TimeOnly? OpenFrom { get; set; }

    public TimeOnly? OpenTo { get; set; }

    public int? Manager { get; set; }

    public int? Status { get; set; }

    public virtual Address? AddressNavigation { get; set; }

    public virtual ICollection<BookInBranch> BookInBranches { get; set; } = new List<BookInBranch>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
