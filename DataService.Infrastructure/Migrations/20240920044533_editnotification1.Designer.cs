﻿// <auto-generated />
using System;
using DataService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataService.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240920044533_editnotification1")]
    partial class editnotification1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataService.Infrastructure.Entities.Attendance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DeviceId")
                        .HasColumnType("integer");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("VerifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("VerifyState")
                        .HasColumnType("integer");

                    b.Property<int>("VerifyType")
                        .HasColumnType("integer");

                    b.Property<int>("WorkCode")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Attendances");
                });

            modelBuilder.Entity("DataService.Infrastructure.Entities.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CommKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsConnected")
                        .HasColumnType("boolean");

                    b.Property<string>("Port")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("DataService.Infrastructure.Entities.DeviceEmployee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DeviceId")
                        .HasColumnType("integer");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("DeviceEmployees");
                });

            modelBuilder.Entity("DataService.Infrastructure.Entities.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CardNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Pin")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("character varying(9)");

                    b.Property<int>("Privilege")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("DataService.Infrastructure.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Success")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("DataService.Infrastructure.Entities.Sheet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DeviceId")
                        .HasColumnType("integer");

                    b.Property<string>("DocumentId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SheetName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("Sheets");
                });

            modelBuilder.Entity("DataService.Infrastructure.Entities.DeviceEmployee", b =>
                {
                    b.HasOne("DataService.Infrastructure.Entities.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataService.Infrastructure.Entities.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("DataService.Infrastructure.Entities.Sheet", b =>
                {
                    b.HasOne("DataService.Infrastructure.Entities.Device", "Device")
                        .WithMany("Sheets")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });

            modelBuilder.Entity("DataService.Infrastructure.Entities.Device", b =>
                {
                    b.Navigation("Sheets");
                });
#pragma warning restore 612, 618
        }
    }
}
