using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class LibraryDbContext : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public LibraryDbContext()
    {
    }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Age> Ages { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookImg> BookImgs { get; set; }

    public virtual DbSet<BookInBranch> BookInBranches { get; set; }

    public virtual DbSet<BookLoan> BookLoans { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Library> Libraries { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Series> Series { get; set; }

    public virtual DbSet<Street> Streets { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    public virtual DbSet<Ward> Wards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Address");

            entity.Property(e => e.Name)
                .HasMaxLength(511)
                .IsUnicode(false);

            entity.HasOne(d => d.StreetNavigation).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.Street)
                .HasConstraintName("FK_Address_Street");
        });

        /*var keysProperties = modelBuilder.Model.GetEntityTypes().Select(x => x.FindPrimaryKey()).SelectMany(x => x.Properties);
        foreach (var property in keysProperties)
        {
            property.ValueGenerated = ValueGenerated.OnAdd;
        }*/

        modelBuilder.Entity<Age>(entity =>
        {
            entity.ToTable("Age");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Author");

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Nation)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasMany(d => d.Books).WithMany(p => p.Authors)
                .UsingEntity<Dictionary<string, object>>(
                    "BookAuthor",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("Book")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookAuthor_Book"),
                    l => l.HasOne<Author>().WithMany()
                        .HasForeignKey("Author")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookAuthor_Author"),
                    j =>
                    {
                        j.HasKey("Author", "Book");
                        j.ToTable("BookAuthor");
                    });
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Book");

            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .HasColumnName("ISBN");
            entity.Property(e => e.Language)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Language)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(511);
            entity.Property(e => e.Version)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.PublisherNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.Publisher)
                .HasConstraintName("FK_Book_Publisher");

            entity.HasOne(d => d.SeriesNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.Series)
                .HasConstraintName("FK_Book_Series");

            entity.HasOne(d => d.VendorNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.Vendor)
                .HasConstraintName("FK_Book_Vendor");

            entity.HasMany(d => d.Ages).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookAge",
                    r => r.HasOne<Age>().WithMany()
                        .HasForeignKey("Age")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookAge_Age"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("Book")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookAge_Book"),
                    j =>
                    {
                        j.HasKey("Book", "Age");
                        j.ToTable("BookAge");
                    });
        });

        modelBuilder.Entity<BookImg>(entity =>
        {
            entity.ToTable("BookImg");

            entity.HasOne(d => d.BookNavigation).WithMany(p => p.BookImgs)
                .HasForeignKey(d => d.Book)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookImg_Book");
        });

        modelBuilder.Entity<BookInBranch>(entity =>
        {
            entity.HasKey(e => new { e.Library, e.Book });

            entity.ToTable("BookInBranch");

            entity.Property(e => e.CallNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.BookNavigation).WithMany(p => p.BookInBranches)
                .HasForeignKey(d => d.Book)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookInBranch_Book");

            entity.HasOne(d => d.LibraryNavigation).WithMany(p => p.BookInBranches)
                .HasForeignKey(d => d.Library)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookInBranch_Library");
        });

        modelBuilder.Entity<BookLoan>(entity =>
        {
            entity.ToTable("BookLoan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FromDate).HasColumnType("datetime");
            entity.Property(e => e.ToDate).HasColumnType("datetime");
            entity.Property(e => e.User).HasMaxLength(450);

            entity.HasOne(d => d.BookNavigation).WithMany(p => p.BookLoans)
                .HasForeignKey(d => d.Book)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookLoan_Book");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.BookLoans)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookLoan_User");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasMany(d => d.Books).WithMany(p => p.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategory",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("Book")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookCategory_Book"),
                    l => l.HasOne<Category>().WithMany()
                        .HasForeignKey("Category")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookCategory_Category"),
                    j =>
                    {
                        j.HasKey("Category", "Book");
                        j.ToTable("BookCategory");
                    });
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("City");

            entity.Property(e => e.Name)
                .HasMaxLength(551)
                .IsUnicode(false);
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.ToTable("District");

            entity.Property(e => e.Name)
                .HasMaxLength(551)
                .IsUnicode(false);

            entity.HasOne(d => d.CityNavigation).WithMany(p => p.Districts)
                .HasForeignKey(d => d.City)
                .HasConstraintName("FK_District_City");
        });

        modelBuilder.Entity<Library>(entity =>
        {
            entity.ToTable("Library");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.AddressNavigation).WithMany(p => p.Libraries)
                .HasForeignKey(d => d.Address)
                .HasConstraintName("FK_Library_Address");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("Publisher");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.ParentNavigation).WithMany(p => p.InverseParentNavigation)
                .HasForeignKey(d => d.Parent)
                .HasConstraintName("FK_Publisher_Publisher");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.RoleId).HasMaxLength(450);

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RoleClaims_RoleId");
        });

        modelBuilder.Entity<Street>(entity =>
        {
            entity.ToTable("Street");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.WardNavigation).WithMany(p => p.Streets)
                .HasForeignKey(d => d.Ward)
                .HasConstraintName("FK_Street_Ward");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); // Đảm bảo Id tự động được sinh
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Fullname).HasMaxLength(100);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PID");
            entity.Property(e => e.Gender).HasMaxLength(6);
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.Avatar);
            entity.Property(e => e.CoverAvatar);
            entity.HasOne(d => d.AddressNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Address)
                .HasConstraintName("FK_Users_Address");
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.UserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserClaims_UserId");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserLogins_UserId");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {

            entity.HasOne(d => d.LibraryNavigation).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.Library)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserRoles_Library");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_RoleId");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserRoles_UserId");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserTokens_UserId");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.ToTable("Vendor");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(551);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.ToTable("Ward");

            entity.Property(e => e.Name)
                .HasMaxLength(511)
                .IsUnicode(false);

            entity.HasOne(d => d.DistrictNavigation).WithMany(p => p.Wards)
                .HasForeignKey(d => d.District)
                .HasConstraintName("FK_Ward_District");
        });

        // cắt chuỗi AspNet trước tên của table
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tblName = entityType.GetTableName();
            if (tblName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tblName.Substring(6));
            }
        }
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
