using DataSeedForStateLevel.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataSeedForStateLevel.Data
{
	public class DataDbContext : DbContext
	{
		public DataDbContext(DbContextOptions<DataDbContext> options) : base(options) { }

		public DbSet<State> States { get; set; }

		public DbSet<Districts> District { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Define the primary key using Fluent API
			modelBuilder.Entity<State>()
						.HasKey(s => s.StateCode);
			modelBuilder.Entity<State>().HasMany(s => s.districts)
				.WithOne().
				HasForeignKey("StateCode");
		}
	}
}
