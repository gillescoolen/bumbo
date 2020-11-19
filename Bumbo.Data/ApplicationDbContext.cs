using System;
using System.Collections.Generic;
using System.Text;
using Bumbo.Data;
using Bumbo.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Data
{
    public partial class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActualTimeWorked> ActualTimeWorked { get; set; }
        public virtual DbSet<AvailableWorktime> AvailableWorktime { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<FurloughRequest> FurloughRequest { get; set; }
        public virtual DbSet<Norm> Norm { get; set; }
        public virtual DbSet<PlannedWorktime> PlannedWorktime { get; set; }
        public virtual DbSet<Prognoses> Prognoses { get; set; }
        public virtual DbSet<Token> Token { get; set; }
        public override DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ActualTimeWorked>(entity =>
            {
                entity.HasKey(e => new { e.WorkDate, e.UserId })
                    .HasName("PK_Actual_Time_Worked");

                entity.Property(e => e.WorkDate).HasColumnType("date");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ActualTimeWorked)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Actual_Time_Worked_Users");
            });

            builder.Entity<AvailableWorktime>(entity =>
            {
                entity.HasKey(e => new { e.WorkDate, e.UserId })
                    .HasName("PK_Available_Worktime");

                entity.Property(e => e.WorkDate).HasColumnType("date");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AvailableWorktime)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Available_Worktime_Users");
            });

            builder.Entity<Branch>(entity =>
            {
                entity.Property(e => e.HouseNumberLetter)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StreetName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            builder.Entity<FurloughRequest>(entity =>
            {
                entity.HasKey(e => new { e.WorkDate, e.UserId })
                    .HasName("PK_Furlough_Request");

                entity.Property(e => e.WorkDate).HasColumnType("date");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FurloughRequest)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Furlough_Request_Users");
            });

            builder.Entity<Norm>(entity =>
            {
                entity.HasKey(e => new { e.Activity, e.BranchId });

                entity.Property(e => e.Activity)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Norm1).HasColumnName("Norm");

                entity.Property(e => e.NormDescription)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Norm)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Norm_Branch");
            });

            builder.Entity<PlannedWorktime>(entity =>
            {
                entity.HasKey(e => new { e.WorkDate, e.UserId })
                    .HasName("PK_Planned_Worktime");

                entity.Property(e => e.WorkDate)
                    .HasColumnName("workDate")
                    .HasColumnType("date");

                entity.Property(e => e.Section)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PlannedWorktime)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Planned_Worktime_Users");
            });

            builder.Entity<Prognoses>(entity =>
            {
                entity.HasKey(e => new { e.Date, e.BranchId });

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.WeatherDescription)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Prognoses)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prognoses_Branch");
            });

            builder.Entity<Token>(entity =>
            {
                entity.Property(e => e.TokenId)
                    .HasColumnName("tokenID")
                    .ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Token)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Token_Users");
            });

            OnModelCreatingPartial(builder);

            builder.Entity<User>(b =>
            {
                b.ToTable("Users");
            });

            builder.Entity<IdentityUserClaim<int>>(b =>
            {
                b.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<int>>(b =>
            {
                b.ToTable("UserLogins");
            });

            builder.Entity<IdentityUserToken<int>>(b =>
            {
                b.ToTable("UserTokens");
            });

            builder.Entity<IdentityRole<int>>(b =>
            {
                b.ToTable("Roles");
            });

            builder.Entity<IdentityRoleClaim<int>>(b =>
            {
                b.ToTable("RoleClaims");
            });

            builder.Entity<IdentityUserRole<int>>(b =>
            {
                b.ToTable("UserRoles");
            });

            builder.Entity<User>().HasOne(u => u.Branch).WithMany(b => b.Users);
        }

        partial void OnModelCreatingPartial(ModelBuilder builder);
    }
}
