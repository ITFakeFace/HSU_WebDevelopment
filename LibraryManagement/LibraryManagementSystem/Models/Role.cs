using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Role : IdentityRole<string>
{
    public Role() : base()
    {

    }

    public Role(string roleName) : base(roleName)
    {

    }

    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
