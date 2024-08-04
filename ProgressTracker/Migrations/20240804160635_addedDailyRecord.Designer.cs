﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProgressTracker.Data;

#nullable disable

namespace ProgressTracker.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240804160635_addedDailyRecord")]
    partial class addedDailyRecord
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProgressTracker.Models.DailyRecordModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<long>("BreakTicks")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("SessionIdsString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TargetTicks")
                        .HasColumnType("bigint");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "Date")
                        .IsUnique();

                    b.ToTable("DailyRecords");
                });

            modelBuilder.Entity("ProgressTracker.Models.SessionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SubjectId")
                        .HasColumnType("int");

                    b.Property<long>("TimeTicks")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
