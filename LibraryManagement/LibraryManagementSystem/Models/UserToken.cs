using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class UserToken : IdentityUserToken<string>
{
    public virtual User User { get; set; } = null!;
}
