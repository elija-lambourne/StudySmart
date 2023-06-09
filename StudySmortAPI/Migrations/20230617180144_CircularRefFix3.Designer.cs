﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudySmortAPI;

#nullable disable

namespace StudySmortAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230617180144_CircularRefFix3")]
    partial class CircularRefFix3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0-preview.4.23259.3");

            modelBuilder.Entity("StudySmortAPI.Model.Deadline", b =>
                {
                    b.Property<Guid>("DeadlineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTimeUtc")
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("DeadlineId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Deadlines");
                });

            modelBuilder.Entity("StudySmortAPI.Model.Flashcard", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("TEXT");

                    b.Property<int>("CorrectCnt")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("TEXT");

                    b.Property<int>("SkipCnt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Translation")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Word")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("WrongCnt")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Flashcards");
                });

            modelBuilder.Entity("StudySmortAPI.Model.FlashcardCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("FlashcardCategories");
                });

            modelBuilder.Entity("StudySmortAPI.Model.Folder", b =>
                {
                    b.Property<Guid>("FolderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("FolderName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ParentFolderId")
                        .HasColumnType("TEXT");

                    b.HasKey("FolderId");

                    b.HasIndex("ParentFolderId");

                    b.ToTable("Folders");
                });

            modelBuilder.Entity("StudySmortAPI.Model.Notebook", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Pages")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ParentId");

                    b.ToTable("Notebooks");
                });

            modelBuilder.Entity("StudySmortAPI.Model.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("FolderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FolderId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("StudySmortAPI.Model.Deadline", b =>
                {
                    b.HasOne("StudySmortAPI.Model.User", "Owner")
                        .WithMany("Deadlines")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("StudySmortAPI.Model.Flashcard", b =>
                {
                    b.HasOne("StudySmortAPI.Model.FlashcardCategory", "Category")
                        .WithMany("Flashcards")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudySmortAPI.Model.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("StudySmortAPI.Model.FlashcardCategory", b =>
                {
                    b.HasOne("StudySmortAPI.Model.User", "Owner")
                        .WithMany("FlashcardCategories")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("StudySmortAPI.Model.Folder", b =>
                {
                    b.HasOne("StudySmortAPI.Model.Folder", "ParentFolder")
                        .WithMany("ChildFolders")
                        .HasForeignKey("ParentFolderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("ParentFolder");
                });

            modelBuilder.Entity("StudySmortAPI.Model.Notebook", b =>
                {
                    b.HasOne("StudySmortAPI.Model.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudySmortAPI.Model.Folder", "Parent")
                        .WithMany("ChildNotebooks")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("StudySmortAPI.Model.User", b =>
                {
                    b.HasOne("StudySmortAPI.Model.Folder", "RootDir")
                        .WithOne()
                        .HasForeignKey("StudySmortAPI.Model.User", "FolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RootDir");
                });

            modelBuilder.Entity("StudySmortAPI.Model.FlashcardCategory", b =>
                {
                    b.Navigation("Flashcards");
                });

            modelBuilder.Entity("StudySmortAPI.Model.Folder", b =>
                {
                    b.Navigation("ChildFolders");

                    b.Navigation("ChildNotebooks");
                });

            modelBuilder.Entity("StudySmortAPI.Model.User", b =>
                {
                    b.Navigation("Deadlines");

                    b.Navigation("FlashcardCategories");
                });
#pragma warning restore 612, 618
        }
    }
}
