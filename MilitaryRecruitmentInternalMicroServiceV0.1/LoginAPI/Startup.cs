using Consul;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LoginAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.OpenApi.Models;

namespace LoginAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["jwt:Issuer"],
                        ValidAudience = Configuration["jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:key"]))
                    };
                });
            services.AddMvc();
            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LoginAPI", Version = "v1" });
            });
            services.AddSingleton<IHostedService, ConsulRegisterService>();
            services.Configure<ServiceConfiguration>(Configuration.GetSection("Service"));
            services.Configure<ConsulConfiguration>(Configuration.GetSection("Consul"));

            var consulAddress = Configuration.GetSection("Consul")["Url"];

            services.AddSingleton<IConsulClient, ConsulClient>(provider =>
                new ConsulClient(config => config.Address = new Uri(consulAddress)));

            services.AddDbContext<LoginContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("LoginContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,LoginContext db)
        {
            db.Database.Migrate();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LoginAPI v1"));
            }


            app.UseHttpsRedirection();

            app.UseRouting();

           /* app.UseCors(options =>
            {
                options.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials();
            });*/

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
