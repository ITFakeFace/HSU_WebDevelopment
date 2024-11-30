using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class City : AddressType
{

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
