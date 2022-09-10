﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Panopticon.Data.Contexts;

#nullable disable

namespace Panopticon.Data.Migrations
{
    [DbContext(typeof(PanopticonContext))]
    [Migration("20220910150315_AddOOCItem")]
    partial class AddOOCItem
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Panopticon.Shared.Models.Feedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ReportingUser")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("Panopticon.Shared.Models.OOCItem", b =>
                {
                    b.Property<int>("ItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemID"), 1L, 1);

                    b.Property<DateTime>("DateStored")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("DiscordGuildId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("ReportingUserId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("ItemID");

                    b.ToTable("OutOfContextItems");
                });

            modelBuilder.Entity("Panopticon.Shared.Models.UserRecord", b =>
                {
                    b.Property<decimal>("UserId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("LastTimePosted")
                        .HasColumnType("datetime2");

                    b.Property<double>("LibcraftCoinBalance")
                        .HasColumnType("float");

                    b.Property<int>("TablesFlipped")
                        .HasColumnType("int");

                    b.Property<bool>("TimeOut")
                        .HasColumnType("bit");

                    b.HasKey("UserId");

                    b.ToTable("UserRecords");
                });
#pragma warning restore 612, 618
        }
    }
}