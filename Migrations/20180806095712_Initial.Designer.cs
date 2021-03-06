﻿// <auto-generated />
using System;
using HHParser.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HHParser.Migrations
{
    [DbContext(typeof(HHDatabaseContext))]
    [Migration("20180806095712_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HHParser.Models.Category", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Slug");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("HHParser.Models.Subcategory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CategoryId");

                    b.Property<string>("Slug");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Subcategories");
                });

            modelBuilder.Entity("HHParser.Models.Vacancy", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("FinalOffer");

                    b.Property<int?>("InitOffer");

                    b.Property<long?>("SubcategoryId");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("SubcategoryId");

                    b.ToTable("Vacancies");
                });

            modelBuilder.Entity("HHParser.Models.Subcategory", b =>
                {
                    b.HasOne("HHParser.Models.Category", "Category")
                        .WithMany("Subcategories")
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("HHParser.Models.Vacancy", b =>
                {
                    b.HasOne("HHParser.Models.Subcategory", "Subcategory")
                        .WithMany("Vacancies")
                        .HasForeignKey("SubcategoryId");
                });
#pragma warning restore 612, 618
        }
    }
}
