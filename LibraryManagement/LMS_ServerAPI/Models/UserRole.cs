using System;
using System.Collections.Generic;

namespace LMS_ServerAPI.Models;

public partial class UserRole
{
    public string UserId { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public int? Library { get; set; }

    public virtual Library? LibraryNavigation { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
