using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MotoRental.Models
{
    public partial class Rental_motorbikeContext : DbContext
    {
        public Rental_motorbikeContext()
        {
        }

        public Rental_motorbikeContext(DbContextOptions<Rental_motorbikeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Brand> Brands { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<Displacement> Displacements { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<Rental> Rentals { get; set; } = null!;
        public virtual DbSet<RentalDetail> RentalDetails { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Vehicle> Vehicles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=Rental_motorbike;Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.Property(e => e.BrandName).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(50);
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Cart");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Cart_User");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_Cart_Vehicle");
            });

            modelBuilder.Entity<Displacement>(entity =>
            {
                entity.ToTable("Displacement");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.DisplacementName).HasMaxLength(50);
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.ImageBackSide)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("image_backSide");

                entity.Property(e => e.ImageFont)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("image_font");

                entity.Property(e => e.ImageLeftSide)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("image_leftSide");

                entity.Property(e => e.ImageRightSide)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("image_rightSide");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.LocationName).HasMaxLength(50);
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.ToTable("Rental");

                entity.Property(e => e.DateFrom).HasColumnType("datetime");

                entity.Property(e => e.DateShip).HasColumnType("datetime");

                entity.Property(e => e.DateTo).HasColumnType("datetime");

                entity.Property(e => e.Message).HasColumnType("ntext");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_Rental_Status");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Rentals)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Rental_User");
            });

            modelBuilder.Entity<RentalDetail>(entity =>
            {
                entity.ToTable("RentalDetail");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Rental)
                    .WithMany(p => p.RentalDetails)
                    .HasForeignKey(d => d.RentalId)
                    .HasConstraintName("FK_RentalDetail_Rental");

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.RentalDetails)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_RentalDetail_Vehicle");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(50)
                    .HasColumnName("status_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Salt)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("FK_User_Location");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_User_Role");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.ToTable("Vehicle");

                entity.Property(e => e.LicensePlate).HasMaxLength(11);

                entity.Property(e => e.Overview).HasMaxLength(500);

                entity.Property(e => e.RegDate).HasColumnType("date");

                entity.Property(e => e.UpdationDate).HasColumnType("date");

                entity.Property(e => e.VehicleName).HasMaxLength(50);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Vehicle_Brand");

                entity.HasOne(d => d.Displacement)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.DisplacementId)
                    .HasConstraintName("FK_Vehicle_Displacement");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Vehicle_Image");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Vehicles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Vehicle_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
