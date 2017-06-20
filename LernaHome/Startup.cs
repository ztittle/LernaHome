﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VDS.RDF.Storage;
using System.IO;
using LernaHome.ZWave;
using System.Reflection;
using System.Threading.Tasks;
using ZWaveControllerClient.Serial;
using ZWaveControllerClient.Xml;
using System.Threading;

namespace LernaHome
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dir = Path.GetDirectoryName(typeof(Startup).GetTypeInfo().Assembly.Location);

            var xmlConfig = File.OpenRead($@"{dir}\xml\ZWave_custom_cmd_classes.xml");
            var xmlParser = new ZWaveClassesXmlParser();
            var loggerFactory = new LoggerFactory()
                .AddConsole();
            var zwaveController = new ZWaveSerialController("COM3", xmlParser.Parse(xmlConfig), loggerFactory);
            zwaveController.Connect();

            var store = new InMemoryManager();
            services.AddSingleton<IUpdateableStorage>(store);
            services.AddSingleton(zwaveController);

            Task.Run(async () =>
            {
                await zwaveController.DiscoverNodes(CancellationToken.None);
                await zwaveController.FetchNodeInfo(CancellationToken.None);

                var zwaveTripleCollector = new ZWaveTripleCollector(zwaveController, store);
                zwaveTripleCollector.SaveNodesToStore();
            }).Wait();

            // Add framework services.
            services.AddMvcCore()
                .AddRdfFormatters();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
