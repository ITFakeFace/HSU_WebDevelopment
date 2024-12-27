using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models;

public partial class BookLoan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [JsonIgnore]
    public string User { get; set; } = null!;

    public int Book { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int? IsReturned { get; set; }

    public int? Status { get; set; }

    [JsonIgnore]
    public virtual Book BookNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual User UserNavigation { get; set; } = null!;

    public int Library { get; set; }

    [JsonIgnore]
    public virtual Library LibraryNavigation { get; set; } = null!;
}
