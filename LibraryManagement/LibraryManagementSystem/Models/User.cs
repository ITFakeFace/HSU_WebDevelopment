using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class User : IdentityUser<string>
{

    public string? Fullname { get; set; }

    public string? Pid { get; set; }

    public int? Address { get; set; }

    public DateOnly? Dob { get; set; }

    public int? Status { get; set; }

    public virtual Address? AddressNavigation { get; set; }

    public virtual ICollection<BookLoan> BookLoans { get; set; } = new List<BookLoan>();

    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
}
