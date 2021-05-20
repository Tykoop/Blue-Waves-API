namespace Esentis.BlueWaves.Web.Api.Services
{
	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Web.Api.Options;

	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;

	public class RefreshTokenCleanService : IHostedService, IDisposable
	{
		private readonly IServiceScopeFactory scope;
		private readonly ILogger<RefreshTokenCleanService> logger;
		private readonly Timer Trigger;
		private readonly JwtOptions jwtOptions;


		public RefreshTokenCleanService(IServiceScopeFactory scopeFactory, IOptions<JwtOptions> opts, ILogger<RefreshTokenCleanService> logger)
		{
			scope = scopeFactory;
			this.logger = logger;
			jwtOptions = opts.Value;
			Trigger = new Timer(DoWork, null, Timeout.Infinite, 0);
		}

		private void DoWork(object state)
		{
			using var s = scope.CreateScope();
			var context = s.ServiceProvider.GetRequiredService<BlueWavesDbContext>();
			var expired = DateTimeOffset.Now.AddDays(-jwtOptions.RefreshTokenDurationInDays);
			var tokens = context.Devices.Where(x => x.UpdatedAt < expired).ToList();
			context.Devices.RemoveRange(tokens);
			context.SaveChanges();
			logger.LogInformation("Removed  {Count} refresh tokens", tokens.Count);
		}

		#region Overrides of BackgroundService
		/// <inheritdoc />
		public void Dispose()
		{
			Trigger.Dispose();
		}
		#endregion

		#region Implementation of IHostedService
		/// <inheritdoc />
		public Task StartAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Starting service {Service}", nameof(RefreshTokenCleanService));

			Trigger.Change(TimeSpan.Zero, TimeSpan.FromSeconds(30));

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public Task StopAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Stopping service {Service}", nameof(RefreshTokenCleanService));
			Trigger.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}
		#endregion

		async ValueTask<int> kati()
		{
			return 0;
		}
	}
}
