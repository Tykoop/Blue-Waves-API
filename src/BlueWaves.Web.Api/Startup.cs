namespace Esentis.BlueWaves.Web.Api
{
	using System;
	using System.IdentityModel.Tokens.Jwt;
	using System.IO;
	using System.Reflection;
	using System.Text;

	using Esentis.BlueWaves.Persistence;
	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Web.Api.Helpers;
	using Esentis.BlueWaves.Web.Api.Options;
	using Esentis.BlueWaves.Web.Api.Services;

	using Kritikos.Configuration.Persistence.Extensions;
	using Kritikos.Configuration.Persistence.Interceptors.SaveChanges;
	using Kritikos.Configuration.Persistence.Interceptors.Services;
	using Kritikos.PureMap;
	using Kritikos.PureMap.Contracts;

	using Microsoft.AspNetCore.Authentication.JwtBearer;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Caching.Memory;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.IdentityModel.Tokens;
	using Microsoft.OpenApi.Models;

	using Swashbuckle.AspNetCore.Filters;
	using Swashbuckle.AspNetCore.SwaggerUI;

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

			services.AddControllers()
				.AddControllersAsServices()
				.AddViewComponentsAsServices()
				.AddTagHelpersAsServices();
			services.AddMvc();
			services.AddControllersWithViews();
			services.Configure<JwtOptions>(options => Configuration.GetSection("JWT").Bind(options));
			services.AddSingleton<IPureMapper>(sp => new PureMapper(MappingConfiguration.Mapping));

			services.AddHttpContextAccessor();

			services.AddSingleton<AuditSaveChangesInterceptor<Guid>>();
			services.AddSingleton<TimestampSaveChangesInterceptor>();
			services.AddSingleton<IAuditorProvider<Guid>>(sp =>
				new AuditorProvider(sp.GetRequiredService<IHttpContextAccessor>()));

			services.AddDbContext<BlueWavesDbContext>((serviceProvider, options) =>
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
					.AddInterceptors(
						serviceProvider.GetRequiredService<TimestampSaveChangesInterceptor>(),
						serviceProvider.GetRequiredService<AuditSaveChangesInterceptor<Guid>>())
					.EnableCommonOptions(Environment);
			});
			services.AddHostedService<RefreshTokenCleanService>();

			services.AddIdentity<BlueWavesUser, BlueWavesRole>(c =>
				{
					c.User.RequireUniqueEmail = !isDevelopment;
					c.Password = new PasswordOptions
					{
						RequireDigit = !isDevelopment,
						RequireLowercase = !isDevelopment,
						RequireUppercase = !isDevelopment,
						RequireNonAlphanumeric = !isDevelopment,
						RequiredLength = isDevelopment
							? 4
							: 12,
					};
				})
				.AddEntityFrameworkStores<BlueWavesDbContext>()
				.AddDefaultTokenProviders()
				.AddDefaultUI();

			services.AddSingleton(sp => new MemoryCache(new MemoryCacheOptions()));

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlueWaves.Web.Api", Version = "v1" });
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description =
						"Enter 'Bearer' [space] and then your valid Token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
				});
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme, Id = "Bearer",
							},
						},
						Array.Empty<string>()
					},
				});
				c.DescribeAllParametersInCamelCase();
				c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
				c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
					$"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
			});

			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
			services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidAudience = Configuration["JWT:Audience"],
						ValidIssuer = Configuration["JWT:Issuer"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
					};
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseSwagger();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();

				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlueWaves.Web.Api v1");
					c.DocumentTitle = "Blue Waves API";
					c.DocExpansion(DocExpansion.None);
					c.EnableDeepLinking();
					c.EnableFilter();
					c.EnableValidator();
					c.DisplayOperationId();
					c.DisplayRequestDuration();
				});
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseReDoc(c =>
			{
				c.SpecUrl = "/swagger/v1/swagger.json";
				c.RoutePrefix = "docs";
				c.DocumentTitle = "Blue Waves API";
				c.HideDownloadButton();
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapRazorPages();
			});
		}
	}
}
