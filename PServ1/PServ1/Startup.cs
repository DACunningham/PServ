using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PServ1.Controllers;
using PServ1.Repositories;

namespace PServ1
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Bearer").AddJwtBearer(options =>
                {
                    options.Audience = "2lkl9n6iuicb798bkkh7rria48";
                    options.Authority = "https://cognito-idp.us-west-2.amazonaws.com/us-west-2_TQaxeubl4";
                });

            // Add custom authorization handlers
            services.AddAuthorization(options =>
            {
                options.AddPolicy("OfficeNumberUnder200", policy => policy.Requirements.Add(new MaximumOfficeNumberRequirement("Admin")));
            });

            services.AddSingleton<IAuthorizationHandler, MaximumOfficeNumberAuthorizationHandler>();

            services.AddMvc();

            services.AddControllers();

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();

            // Add custom services to the DI framework.
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IDBConnection, DBConnection>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
