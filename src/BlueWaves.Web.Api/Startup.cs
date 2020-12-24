namespace Esentis.BlueWaves.Web.Api
{
	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Helpers;

	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Diagnostics;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.OpenApi.Models;

	public class Startup
	{
		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
		{
			Configuration = configuration;
			Environment = environment;
		}

		private IConfiguration Configuration { get; }

		private IWebHostEnvironment Environment { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var isDevelopment = Environment.IsDevelopment();
			var timeStampInterceptor = new TimeStampSaveChangesInterceptor();
			services.AddDbContextPool<BlueWavesDbContext>(options =>
			{
				options.UseNpgsql(
						Configuration.GetConnectionString("BlueWavesApi"),
						options =>
						{
							options.EnableRetryOnFailure()
								.SetPostgresVersion(12, 3)
								.UseNetTopologySuite()
								.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
						})
					.AddInterceptors(timeStampInterceptor)
					.EnableSensitiveDataLogging(isDevelopment)
					.EnableDetailedErrors(isDevelopment)
					.ConfigureWarnings(warn => warn
						.Ignore(CoreEventId.SensitiveDataLoggingEnabledWarning)
						.Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning));
			});

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlueWaves.Web.Api", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();

				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlueWaves.Web.Api v1"));
			}


			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
