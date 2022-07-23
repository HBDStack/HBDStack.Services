﻿// <auto-generated />
using System;
using HBDStack.Services.StateManagement.Storages.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HBDStack.Services.StateManagement.Storages.MySql.Migrations
{
    [DbContext(typeof(MySqlStateContext))]
    partial class MySqlStateContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("HBDStack.Services.StateManagement.Storages.MySql.StateEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset?>("UpdatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("state_Entities");
                });
#pragma warning restore 612, 618
        }
    }
}
