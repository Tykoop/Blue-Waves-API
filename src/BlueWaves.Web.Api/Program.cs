namespace Esentis.BlueWaves.Web.Api
{
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Model;

	using Microsoft.AspNetCore.Hosting;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;

	using NetTopologySuite.Geometries;

	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();
			var scope = host.Services.CreateScope();
			var ctx = scope.ServiceProvider.GetRequiredService<BlueWavesDbContext>();
			await ctx.Database.MigrateAsync();
			var beach = new Beach { Coordinates = new Point(20, 20), Name = "Armenistis" };
			ctx.Beaches.Add(beach);
			await ctx.SaveChangesAsync();
			await host.RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
