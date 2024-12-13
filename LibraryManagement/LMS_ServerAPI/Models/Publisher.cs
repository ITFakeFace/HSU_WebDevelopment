using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LMS_ServerAPI.Models;

public partial class Publisher
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Parent { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
	[JsonIgnore]
	public virtual ICollection<Publisher> InverseParentNavigation { get; set; } = new List<Publisher>();
	[JsonIgnore]
	public virtual Publisher? ParentNavigation { get; set; }
}
