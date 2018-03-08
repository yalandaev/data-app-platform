using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAppPlatform.Api.Services;
using DataAppPlatform.Core.DataService.Interfaces;
using DataAppPlatform.DataAccess;
using DataAppPlatform.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace DataAppPlatform.Api
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
            services.AddCors(options =>
            {
                options.AddPolicy("defaultPolicy",
                    policy => policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod());
            });
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()); ;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "DataService API", Version = "v1" });
            });


            // DependencyInjection
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<DataContext>();
            services.AddTransient<ISqlDialectProvider, SqlServerDialectProvider>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("defaultPolicy");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DataService API V1");
            });
        }
    }
}
