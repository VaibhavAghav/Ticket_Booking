using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket_Model;

namespace Ticket_DataAccess
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        public DbSet<Bus> Buses { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<BusRoute> BusRoute { get; set; }
        public DbSet<BusStop> BusStop { get; set; }
        public DbSet<BusBooking> BusBooking { get; set; }
        public DbSet<BusDaily> BusSeatAvailablity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BusRoute relationships
            modelBuilder.Entity<BusRoute>()
                .HasOne(br => br.StartCity)
                .WithMany()
                .HasForeignKey(br => br.StartCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusRoute>()
                .HasOne(br => br.DestinationCity)
                .WithMany()
                .HasForeignKey(br => br.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusRoute>()
                .HasOne(br => br.Bus)
                .WithMany()
                .HasForeignKey(br => br.BusId)
                .OnDelete(DeleteBehavior.Restrict);
           
            // BusBooking Configuration
            modelBuilder.Entity<BusBooking>()
                .HasOne(bb => bb.BusRoute)
                .WithMany()
                .HasForeignKey(bb => bb.BusRouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusBooking>()
                .HasOne(bb => bb.StartCity)
                .WithMany()
                .HasForeignKey(bb => bb.StartCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusBooking>()
                .HasOne(bb => bb.DestinationCity)
                .WithMany()
                .HasForeignKey(bb => bb.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);


            //BusStop Configuration
            modelBuilder.Entity<BusStop>()
            .HasOne(bs => bs.BusRoute)
            .WithMany()
            .HasForeignKey(bs => bs.BusRoutId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusStop>()
                .HasOne(bs => bs.AddCity)
                .WithMany()
                .HasForeignKey(bs => bs.AddCityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusDaily>()
            .HasOne(p => p.StartCity)
            .WithMany()
            .HasForeignKey(p => p.StartCityId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusDaily>()
                .HasOne(p => p.DestinationCity)
                .WithMany()
                .HasForeignKey(p => p.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
