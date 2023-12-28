using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Brokerzin
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
            .AddHostedMqttServer(mqttServer => mqttServer.WithoutDefaultEndpoint())
            .AddMqttConnectionHandler()
            .AddConnections();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMqtt("/mqtt");
            });

            app.UseMqttServer(server =>
            {
                server.StartedAsync += (async args =>
                {
                    var frameworkName = GetType().Assembly.GetCustomAttribute<TargetFrameworkAttribute>()?
                        .FrameworkName;

                    var msg = new MqttApplicationMessageBuilder()
                        .WithPayload($"Mqtt hosted on {frameworkName} is awesome")
                        .WithTopic("message");

                    //while (true)
                    //{
                    //    try
                    //    {
                    //        await server.(msg.Build());
                    //        msg.WithPayload($"Mqtt hosted on {frameworkName} is still awesome at {DateTime.Now}");
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Console.WriteLine(e);
                    //    }
                    //    finally
                    //    {
                    //        await Task.Delay(TimeSpan.FromSeconds(2));
                    //    }
                    //}
                });
            });

            app.Use((context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Request.Path = "/Index.html";
                }

                return next();
            });

            app.UseStaticFiles();

           
        }
    }
}
