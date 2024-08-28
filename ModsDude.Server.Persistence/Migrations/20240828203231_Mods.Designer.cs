﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModsDude.Server.Persistence.DbContexts;

#nullable disable

namespace ModsDude.Server.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240828203231_Mods")]
    partial class Mods
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ModsDude.Server.Domain.Mods.Mod", b =>
                {
                    b.Property<Guid>("RepoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("RepoId", "Id");

                    b.ToTable("Mods");
                });

            modelBuilder.Entity("ModsDude.Server.Domain.Mods.ModVersion", b =>
                {
                    b.Property<Guid>("RepoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ModId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RepoId", "ModId", "Id");

                    b.ToTable("ModVersion");
                });

            modelBuilder.Entity("ModsDude.Server.Domain.Profiles.Profile", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("RepoId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RepoId", "Name")
                        .IsUnique();

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("ModsDude.Server.Domain.RepoMemberships.RepoMembership", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid>("RepoId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "RepoId");

                    b.HasIndex("RepoId");

                    b.ToTable("RepoMemberships");
                });

            modelBuilder.Entity("ModsDude.Server.Domain.Repos.Repo", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModAdapter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SavegameAdapter")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Repos");
                });

            modelBuilder.Entity("ModsDude.Server.Domain.Users.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastSeen")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ProfileLastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ModsDude.Server.Domain.Mods.Mod", b =>
                {
                    b.HasOne("ModsDude.Server.Domain.Repos.Repo", null)
                        .WithMany()
                        .HasForeignKey("RepoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ModsDude.Server.Domain.Mods.ModVersion", b =>
                {
                    b.HasOne("ModsDude.Server.Domain.Mods.Mod", "Mod")
                        .WithMany("Versions")
                        .HasForeignKey("RepoId", "ModId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("ModsDude.Server.Domain.Mods.ModAttribute", "Attributes", b1 =>
                        {
                            b1.Property<Guid>("ModVersionRepoId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ModVersionModId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("ModVersionId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Key")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Value")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("ModVersionRepoId", "ModVersionModId", "ModVersionId", "Id");

                            b1.ToTable("ModAttribute");

                            b1.WithOwner()
                                .HasForeignKey("ModVersionRepoId", "ModVersionModId", "ModVersionId");
                        });

                    b.Navigation("Attributes");

                    b.Navigation("Mod");
                });

            modelBuilder.Entity("ModsDude.Server.Domain.Profiles.Profile", b =>
                {
                    b.HasOne("ModsDude.Server.Domain.Repos.Repo", null)
                        .WithMany()
                        .HasForeignKey("RepoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("ModsDude.Server.Domain.Profiles.ModDependency", "ModDependencies", b1 =>
                        {
                            b1.Property<Guid>("ProfileId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.Property<bool>("LockVersion")
                                .HasColumnType("bit");

                            b1.Property<string>("ModVersionId")
                                .IsRequired()
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("ModVersionModId")
                                .IsRequired()
                                .HasColumnType("nvarchar(450)");

                            b1.Property<Guid>("ModVersionRepoId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("ProfileId", "Id");

                            b1.HasIndex("ModVersionRepoId", "ModVersionModId", "ModVersionId");

                            b1.ToTable("ModDependency");

                            b1.WithOwner()
                                .HasForeignKey("ProfileId");

                            b1.HasOne("ModsDude.Server.Domain.Mods.ModVersion", "ModVersion")
                                .WithMany()
                                .HasForeignKey("ModVersionRepoId", "ModVersionModId", "ModVersionId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.Navigation("ModVersion");
                        });

                    b.Navigation("ModDependencies");
                });

            modelBuilder.Entity("ModsDude.Server.Domain.RepoMemberships.RepoMembership", b =>
                {
                    b.HasOne("ModsDude.Server.Domain.Repos.Repo", null)
                        .WithMany()
                        .HasForeignKey("RepoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ModsDude.Server.Domain.Users.User", null)
                        .WithMany("RepoMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ModsDude.Server.Domain.Mods.Mod", b =>
                {
                    b.Navigation("Versions");
                });

            modelBuilder.Entity("ModsDude.Server.Domain.Users.User", b =>
                {
                    b.Navigation("RepoMemberships");
                });
#pragma warning restore 612, 618
        }
    }
}
