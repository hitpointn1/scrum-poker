using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker.Models
{
    public class PokerPlanningContext : DbContext
    {
        private readonly string _connectionString;
        public PokerPlanningContext(string connection)
        {
            _connectionString = connection;
        }
        public PokerPlanningContext()
        {
            _connectionString = @"Data Source=COMPUTER\SQLSERVER;Initial Catalog=ScrumPoker;Integrated Security=True;";
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
}
