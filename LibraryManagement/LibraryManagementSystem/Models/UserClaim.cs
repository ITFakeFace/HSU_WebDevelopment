using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class UserClaim : IdentityUserClaim<string>
{

    public virtual User User { get; set; } = null!;
}
