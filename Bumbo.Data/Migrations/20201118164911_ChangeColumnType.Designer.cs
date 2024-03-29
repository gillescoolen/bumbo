﻿// <auto-generated />
using System;
using Bumbo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bumbo.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20201118164911_ChangeColumnType")]
    partial class ChangeColumnType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Bumbo.Data.Models.ActualTimeWorked", b =>
                {
                    b.Property<DateTime>("WorkDate")
                        .HasColumnType("date");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Finish")
                        .HasColumnType("time");

                    b.Property<byte?>("Sickness")
                        .HasColumnType("tinyint");

                    b.Property<TimeSpan>("Start")
                        .HasColumnType("time");

                    b.HasKey("WorkDate", "UserId")
                        .HasName("PK_Actual_Time_Worked");

                    b.HasIndex("UserId");

                    b.ToTable("ActualTimeWorked");
                });

            modelBuilder.Entity("Bumbo.Data.Models.AvailableWorktime", b =>
                {
                    b.Property<DateTime>("WorkDate")
                        .HasColumnType("date");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Finish")
                        .HasColumnType("time");

                    b.Property<int?>("SchoolHoursWorked")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Start")
                        .HasColumnType("time");

                    b.HasKey("WorkDate", "UserId")
                        .HasName("PK_Available_Worktime");

                    b.HasIndex("UserId");

                    b.ToTable("AvailableWorktime");
                });

            modelBuilder.Entity("Bumbo.Data.Models.Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("HouseNumber")
                        .HasColumnType("int");

                    b.Property<string>("HouseNumberLetter")
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("StreetName")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Branch");
                });

            modelBuilder.Entity("Bumbo.Data.Models.FurloughRequest", b =>
                {
                    b.Property<DateTime>("WorkDate")
                        .HasColumnType("date");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<byte>("IsApproved")
                        .HasColumnType("tinyint");

                    b.HasKey("WorkDate", "UserId")
                        .HasName("PK_Furlough_Request");

                    b.HasIndex("UserId");

                    b.ToTable("FurloughRequest");
                });

            modelBuilder.Entity("Bumbo.Data.Models.Norm", b =>
                {
                    b.Property<string>("Activity")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<int>("Norm1")
                        .HasColumnName("Norm")
                        .HasColumnType("int");

                    b.Property<string>("NormDescription")
                        .IsRequired()
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200)
                        .IsUnicode(false);

                    b.HasKey("Activity", "BranchId");

                    b.HasIndex("BranchId");

                    b.ToTable("Norm");
                });

            modelBuilder.Entity("Bumbo.Data.Models.PlannedWorktime", b =>
                {
                    b.Property<DateTime>("WorkDate")
                        .HasColumnName("workDate")
                        .HasColumnType("date");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Finish")
                        .HasColumnType("time");

                    b.Property<string>("Section")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<TimeSpan>("Start")
                        .HasColumnType("time");

                    b.HasKey("WorkDate", "UserId")
                        .HasName("PK_Planned_Worktime");

                    b.HasIndex("UserId");

                    b.ToTable("PlannedWorktime");
                });

            modelBuilder.Entity("Bumbo.Data.Models.Prognoses", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<int>("AmountOfCustomers")
                        .HasColumnType("int");

                    b.Property<int>("AmountOfFreight")
                        .HasColumnType("int");

                    b.Property<string>("WeatherDescription")
                        .IsRequired()
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.HasKey("Date", "BranchId");

                    b.HasIndex("BranchId");

                    b.ToTable("Prognoses");
                });

            modelBuilder.Entity("Bumbo.Data.Models.Token", b =>
                {
                    b.Property<int>("TokenId")
                        .HasColumnName("tokenID")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("TokenId");

                    b.HasIndex("UserId");

                    b.ToTable("Token");
                });

            modelBuilder.Entity("Bumbo.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Bid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfEmployment")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HouseNumber")
                        .HasColumnType("int");

                    b.Property<string>("HouseNumberLetter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IBAN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PhoneNumber")
                        .HasColumnType("int");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("Bumbo.Data.Models.ActualTimeWorked", b =>
                {
                    b.HasOne("Bumbo.Data.User", "User")
                        .WithMany("ActualTimeWorked")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Actual_Time_Worked_Users")
                        .IsRequired();
                });

            modelBuilder.Entity("Bumbo.Data.Models.AvailableWorktime", b =>
                {
                    b.HasOne("Bumbo.Data.User", "User")
                        .WithMany("AvailableWorktime")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Available_Worktime_Users")
                        .IsRequired();
                });

            modelBuilder.Entity("Bumbo.Data.Models.FurloughRequest", b =>
                {
                    b.HasOne("Bumbo.Data.User", "User")
                        .WithMany("FurloughRequest")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Furlough_Request_Users")
                        .IsRequired();
                });

            modelBuilder.Entity("Bumbo.Data.Models.Norm", b =>
                {
                    b.HasOne("Bumbo.Data.Models.Branch", "Branch")
                        .WithMany("Norm")
                        .HasForeignKey("BranchId")
                        .HasConstraintName("FK_Norm_Branch")
                        .IsRequired();
                });

            modelBuilder.Entity("Bumbo.Data.Models.PlannedWorktime", b =>
                {
                    b.HasOne("Bumbo.Data.User", "User")
                        .WithMany("PlannedWorktime")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Planned_Worktime_Users")
                        .IsRequired();
                });

            modelBuilder.Entity("Bumbo.Data.Models.Prognoses", b =>
                {
                    b.HasOne("Bumbo.Data.Models.Branch", "Branch")
                        .WithMany("Prognoses")
                        .HasForeignKey("BranchId")
                        .HasConstraintName("FK_Prognoses_Branch")
                        .IsRequired();
                });

            modelBuilder.Entity("Bumbo.Data.Models.Token", b =>
                {
                    b.HasOne("Bumbo.Data.User", "User")
                        .WithMany("Token")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Token_Users");
                });

            modelBuilder.Entity("Bumbo.Data.User", b =>
                {
                    b.HasOne("Bumbo.Data.Models.Branch", "Branch")
                        .WithMany("Users")
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Bumbo.Data.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Bumbo.Data.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bumbo.Data.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("Bumbo.Data.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
