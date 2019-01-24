using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using IRO.Mvc.CoolSwagger;
using IRO.Mvc.Core;
using IRO.Mvc.MvcExceptionHandler;
using IRO.Mvc.MvcExceptionHandler.Models;
using IRO.Mvc.MvcExceptionHandler.Services;
using IRO.Mvc.PureBinding;
using IRO.Mvc.PureBinding.SwaggerSupport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace IRO.Tests.SwashbuckleTest
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
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .InsertJsonPureBinder();

            AddSwaggerGen_Local(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();


            UseSwaggerUI_Local(app);
        }

        void UseSwaggerUI_Local(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.InjectJavascript("https://code.jquery.com/jquery-3.3.1.min.js");
                //c.InjectJavascript("/SwaggerClient/SwaggerInjectedJs/Script.js");
                c.ShowExtensions();
                c.EnableValidator();
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.DisplayOperationId();
                c.DisplayRequestDuration();
            });

            
        }

        void AddSwaggerGen_Local(IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc(
                    "v1",
                    new Info
                    {
                        Title = "My API",
                        Version = "v1",
                        Description = "Api description"
                    });
                opt.SwaggerDocAdditional(doc =>
                {
                    doc.Schemes = new List<string>
                    {
                        "http",
                        "https"
                    };
                });

                opt.EnableAnnotations();
                opt.DescribeAllEnumsAsStrings();
                opt.UseReferencedDefinitionsForEnums();

                //opt.IncludeAllAvailableXmlComments();
                opt.UseCoolSummaryGen();
                opt.UseDefaultIdentityAuthScheme();
                opt.AddSwaggerTagNameOperationFilter();

                opt.AddPureBindingToSwashbuckle();
            });
        }
    }
}
