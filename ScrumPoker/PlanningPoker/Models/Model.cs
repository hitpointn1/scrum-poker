using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PlanningPoker
{
    public class PokerPlanningContext : DbContext
    {
        private readonly string _connectionString;
        public PokerPlanningContext (string connection)
        {
            _connectionString = connection;
        }
        public PokerPlanningContext ()
        {
            //_connectionString = @"Server=MSI;Initial Catalog=PokerPlanningDB;Trusted_Connection=True;";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlServer(_connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<PokerRoom> PokerRooms { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
    public class PokerRoom
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TypeCards { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CloseDate { get; set; }
    }
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Role { get; set; }
        public bool IsOnline { get; set; }

        public int PokerRoomId { get; set; }
        public PokerRoom PokerRoom { get; set; }
    }
    public class Message
    {
        public int Id { get; set; }
        public string Context { get; set; }
        public DateTime CreateDate { get; set; }

        public int PokerRoomId { get; set; }
        public PokerRoom PokerRoom { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
    public class Topic
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Marks { get; set; }
        public int Status { get; set; }

        public int PokerRoomId { get; set; }
        public PokerRoom PokerRoom { get; set; }
    }
    public class Card
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int CardValue { get; set; }
        public int IterationNumb { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public int TopicId { get; set; }
        public Topic Topic { get; set; }
    }
}
