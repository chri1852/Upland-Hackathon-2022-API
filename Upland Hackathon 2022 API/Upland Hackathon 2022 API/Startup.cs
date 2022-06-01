using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UplandHackathon2022.API.Contracts.Constants;
using UplandHackathon2022.API.Infrastructure.Authentication;
using UplandHackathon2022.API.Infrastructure.Repositories;
using UplandHackaton2022.Api.Abstractions;

namespace Upland_Hackathon_2022_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services
                .AddSingleton<ILocalRepository, LocalRepository>()
                .AddSingleton<IUplandThirdPartyApiRepository, UplandThirdPartyApiRepository>()
                .AddSingleton(Configuration);

            services
                .AddAuthentication(options => { options.DefaultScheme = UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme; })
                .AddScheme<UplandHackathon2022AuthSchemeOptions, UplandHackathon2022AuthHandler>(UplandHackathon2022AuthConstants.UplandHackathon2022AuthScheme, options => { });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UplandUsername", policy => policy.RequireClaim("UplandUsername"));
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(
                        "http://localhost:3000",
                        "https://localhost:44337",
                        "https://*.hackup.land",
                        "https://*.hackup.land")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
