﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stage.Data;

#nullable disable

namespace Stage.Migrations
{
    [DbContext(typeof(ClubSportifDbContext))]
    [Migration("20241020011530_statistiquetow")]
    partial class statistiquetow
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Stage.Models.Cotisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateExpiration")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DatePaiement")
                        .HasColumnType("datetime2");

                    b.Property<int>("MembreId")
                        .HasColumnType("int");

                    b.Property<decimal>("Montant")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MembreId");

                    b.ToTable("Cotisations");
                });

            modelBuilder.Entity("Stage.Models.Entrainement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateDebut")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateFin")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("HeureDebut")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("HeureFin")
                        .HasColumnType("time");

                    b.Property<string>("Lieu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Titre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeEvenement")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Entrainements");
                });

            modelBuilder.Entity("Stage.Models.Membre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateAdhesion")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatutAdhesion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Membres");
                });

            modelBuilder.Entity("Stage.Models.Participation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EntrainementId")
                        .HasColumnType("int");

                    b.Property<int>("MembreId")
                        .HasColumnType("int");

                    b.Property<string>("StatutParticipation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EntrainementId");

                    b.HasIndex("MembreId");

                    b.ToTable("Participations");
                });

            modelBuilder.Entity("Stage.Models.Statistique", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateEntrainement")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateStatistique")
                        .HasColumnType("datetime2");

                    b.Property<int>("EntrainementId")
                        .HasColumnType("int");

                    b.Property<string>("EntrainementTitre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MembreId")
                        .HasColumnType("int");

                    b.Property<string>("MembreNom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MembresAbsents")
                        .HasColumnType("int");

                    b.Property<int>("MembresExcuses")
                        .HasColumnType("int");

                    b.Property<int>("MembresPresents")
                        .HasColumnType("int");

                    b.Property<int?>("ParticipationId")
                        .HasColumnType("int");

                    b.Property<string>("Periode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("PourcentageAbsence")
                        .HasColumnType("decimal(5,2)");

                    b.Property<decimal>("PourcentageExcuses")
                        .HasColumnType("decimal(5,2)");

                    b.Property<decimal>("PourcentagePresence")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int>("TotalMembres")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EntrainementId");

                    b.HasIndex("MembreId");

                    b.HasIndex("ParticipationId");

                    b.ToTable("Statistiques");
                });

            modelBuilder.Entity("Stage.Models.Cotisation", b =>
                {
                    b.HasOne("Stage.Models.Membre", "Membre")
                        .WithMany("Cotisations")
                        .HasForeignKey("MembreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Membre");
                });

            modelBuilder.Entity("Stage.Models.Participation", b =>
                {
                    b.HasOne("Stage.Models.Entrainement", "Entrainement")
                        .WithMany("Participations")
                        .HasForeignKey("EntrainementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stage.Models.Membre", "Membre")
                        .WithMany("Participations")
                        .HasForeignKey("MembreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entrainement");

                    b.Navigation("Membre");
                });

            modelBuilder.Entity("Stage.Models.Statistique", b =>
                {
                    b.HasOne("Stage.Models.Entrainement", "Entrainement")
                        .WithMany("Statistiques")
                        .HasForeignKey("EntrainementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Stage.Models.Membre", null)
                        .WithMany("Statistiques")
                        .HasForeignKey("MembreId");

                    b.HasOne("Stage.Models.Participation", null)
                        .WithMany("Statistiques")
                        .HasForeignKey("ParticipationId");

                    b.Navigation("Entrainement");
                });

            modelBuilder.Entity("Stage.Models.Entrainement", b =>
                {
                    b.Navigation("Participations");

                    b.Navigation("Statistiques");
                });

            modelBuilder.Entity("Stage.Models.Membre", b =>
                {
                    b.Navigation("Cotisations");

                    b.Navigation("Participations");

                    b.Navigation("Statistiques");
                });

            modelBuilder.Entity("Stage.Models.Participation", b =>
                {
                    b.Navigation("Statistiques");
                });
#pragma warning restore 612, 618
        }
    }
}
