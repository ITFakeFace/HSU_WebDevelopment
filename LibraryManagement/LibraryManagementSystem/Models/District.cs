using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class District : AddressType
{

    public int? City { get; set; }

    public virtual City? CityNavigation { get; set; }

    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
