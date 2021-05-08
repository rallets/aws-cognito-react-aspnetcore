using AwsCognito.Api.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace AwsCognito.Api
{
	public class Startup
	{
		private const string _anyCors = "_anyCors";

		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Allow access to IHttpContextAccessor httpContextAccessor, like _httpContextAccessor.HttpContext.User.Identity.Name
			services.AddHttpContextAccessor(); 

			services.AddCors(options =>
			{
				options.AddPolicy(name: _anyCors, builder =>
					builder
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
						.SetPreflightMaxAge(TimeSpan.FromSeconds(86400))
				);
			});

			services.AddCognitoAuthentication(Configuration);

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "AwsCognito.Api", Version = "v1" });
			});

			// Add DI custom services
			services.AddScoped<IIdentityService, IdentityService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AwsCognito.Api v1"));
			}

			app.UseHttpsRedirection();
			app.UseCors(_anyCors);
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
