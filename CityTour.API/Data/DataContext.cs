using CityTour.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityTour.API.Data
{
	public class DataContext: DbContext
	{
		public DataContext(DbContextOptions<DataContext> options): base(options)
		{

		}

		public DbSet<Photo> Photos { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<City> Cities { get; set; }
	}
}
