using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Store.Api.Models;

public partial class StoreContext : DbContext
{
    public StoreContext()
    {
    }

    public StoreContext(DbContextOptions<StoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\local;Database=Store;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Category_1");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.ModifiedBy).HasMaxLength(255);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Discount_1");

            entity.ToTable("Discount");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ActiveFrom).HasColumnType("datetime");
            entity.Property(e => e.ActiveUntil).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(2, 2)");
            entity.Property(e => e.ModifiedBy).HasMaxLength(255);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Product_1");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(255);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PricePerUnit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Roles_1");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(255);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Ticket_1");

            entity.ToTable("Ticket");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(255);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Ticket_Product");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users_1");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(255);
            entity.Property(e => e.ModifiedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(64)
                .IsFixedLength();
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(128)
                .IsFixedLength();
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
