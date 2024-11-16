using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class RoleClaim : IdentityRoleClaim<string>
{
    public virtual Role Role { get; set; } = null!;
}
