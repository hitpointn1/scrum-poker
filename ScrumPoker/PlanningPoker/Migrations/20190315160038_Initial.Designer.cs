﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PlanningPoker;

namespace PlanningPoker.Migrations
{
    [DbContext(typeof(PokerPlanningContext))]
    [Migration("20190315160038_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PlanningPoker.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CardValue");

                    b.Property<string>("Comment");

                    b.Property<int>("IterationNumb");

                    b.Property<int>("PlayerId");

                    b.Property<int>("TopicId");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("TopicId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("PlanningPoker.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Context");

                    b.Property<DateTime>("CreateDate");

                    b.Property<int>("PlayerId");

                    b.Property<int>("PokerRoomId");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("PokerRoomId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("PlanningPoker.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("PokerRoomId");

                    b.Property<int>("Role");

                    b.HasKey("Id");

                    b.HasIndex("PokerRoomId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("PlanningPoker.PokerRoom", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CloseDate");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Description");

                    b.Property<string>("Password");

                    b.Property<string>("Title");

                    b.Property<int>("TypeCards");

                    b.HasKey("Id");

                    b.ToTable("PokerRooms");
                });

            modelBuilder.Entity("PlanningPoker.Topic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<int>("Marks");

                    b.Property<int>("PokerRoomId");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("PokerRoomId");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("PlanningPoker.Card", b =>
                {
                    b.HasOne("PlanningPoker.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PlanningPoker.Topic", "Topic")
                        .WithMany()
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PlanningPoker.Message", b =>
                {
                    b.HasOne("PlanningPoker.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PlanningPoker.PokerRoom", "PokerRoom")
                        .WithMany()
                        .HasForeignKey("PokerRoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PlanningPoker.Player", b =>
                {
                    b.HasOne("PlanningPoker.PokerRoom", "PokerRoom")
                        .WithMany()
                        .HasForeignKey("PokerRoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PlanningPoker.Topic", b =>
                {
                    b.HasOne("PlanningPoker.PokerRoom", "PokerRoom")
                        .WithMany()
                        .HasForeignKey("PokerRoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
