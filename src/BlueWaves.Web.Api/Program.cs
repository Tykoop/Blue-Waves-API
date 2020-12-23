namespace Esentis.BlueWaves.Web.Api
{
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Hosting;

	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

            var startup = new Startup(null,null);
            int? age1 = 1;
            int age2 = 1;

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
