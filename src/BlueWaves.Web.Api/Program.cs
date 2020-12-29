namespace Esentis.BlueWaves.Web.Api
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Model;

	using Kritikos.StructuredLogging.Templates;

	using Microsoft.AspNetCore.Hosting;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;

	using NetTopologySuite.Geometries;

	using Serilog;
	using Serilog.Core;
	using Serilog.Events;
	using Serilog.Exceptions;
	using Serilog.Exceptions.Core;
	using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
	using Serilog.Sinks.SystemConsole.Themes;

	public static class Program
	{
		private static readonly string ApplicationName =
#pragma warning disable CS8602 // Can't be nulled.
#pragma warning disable CS8601 // Possible null reference assignment.
			Assembly.GetAssembly(typeof(Program)).GetName().Name;

		private static readonly LoggingLevelSwitch LevelSwitch = new LoggingLevelSwitch();

		public static async Task Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration().CreateBasicLogger().CreateLogger();
			try
			{
				var host = CreateHostBuilder(args).Build();
				var configuration = host.Services.GetRequiredService<IConfiguration>();
				var environment = host.Services.GetRequiredService<IWebHostEnvironment>();

				Log.Logger = new LoggerConfiguration().CreateActualLogger(configuration, environment).CreateLogger();
				{
					using var scope = host.Services.CreateScope();
					await using var ctx = scope.ServiceProvider.GetRequiredService<BlueWavesDbContext>();
					var migrations = (await ctx.Database.GetPendingMigrationsAsync()).ToList();

					if (migrations.Any())
					{
						Log.Information(AspNetCoreLogTemplates.ApplyingMigrations, string.Join(", ", migrations));
						await ctx.Database.MigrateAsync();
					}
				}

				await host.RunAsync();
			}
			catch (InvalidOperationException e)
			{
				Log.Fatal(e, "Could not apply migrations");
			}
#pragma warning disable CA1031 // Unhandled exception, application terminated
			catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
			{
				Log.Fatal(e, GenericLogTemplates.UnhandledException, e.Message);
			}
			finally
			{
				Log.Information("{Application} shutting down", ApplicationName);
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				}).UseSerilog();

		public static LoggerConfiguration CreateBasicLogger(this LoggerConfiguration logger)
			=> logger
				.MinimumLevel.Verbose()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
				.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
				.MinimumLevel.Override("System", LogEventLevel.Error)
				.Enrich.FromLogContext()
				.Enrich.WithEnvironmentUserName()
				.Enrich.WithMachineName()
				.Enrich.WithProperty("Application", ApplicationName)
				.Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
					.WithDefaultDestructurers()
					.WithDestructurers(new[] { new DbUpdateExceptionDestructurer() })
					.WithRootName("Exception"))
				.WriteTo.Debug()
				.WriteTo.Console(theme: AnsiConsoleTheme.Code);

		public static LoggerConfiguration CreateActualLogger(this LoggerConfiguration logger, IConfiguration configuration, IHostEnvironment environment) =>
			logger.CreateBasicLogger()
				.Enrich.WithProperty("Application", environment.ApplicationName)
				.Enrich.WithProperty("Environment", environment.EnvironmentName)
				.WriteTo.Logger(log => log
					.MinimumLevel.ControlledBy(LevelSwitch)
					.Filter.ByExcluding(configuration["Serilog:Ignored"])
					.WriteTo.File(
						Path.Combine(Directory.GetCurrentDirectory(), "Logs", $"{environment.ApplicationName}-.log"),
						fileSizeLimitBytes: 31457280,
						rollingInterval: RollingInterval.Day,
						rollOnFileSizeLimit: true,
						retainedFileCountLimit: 10,
						shared: true));
	}
}
