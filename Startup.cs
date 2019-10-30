using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestPing.Controllers;

namespace TestPing
{
    public class Startup
    {
        public static List<valor> AllList = new List<valor>();
        public static string IP;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            IP = Configuration.GetValue<string>("ip");
            Timer oTimer = new Timer();
            oTimer.Interval = 1000;

            oTimer.Elapsed += EventoIntervalo;

            oTimer.Enabled = true;

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
        }

        private void EventoIntervalo(object sender, ElapsedEventArgs e)
        {
            AllList.Add(ExecuteCommand("ping " + IP));
        }

        static valor ExecuteCommand(string _Command)
        {
            try
            {
                ProcessStartInfo procStartInfo =
                    new ProcessStartInfo("cmd", "/c " + _Command);
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = false;
                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.StandardOutput.ReadLine();
                proc.StandardOutput.ReadLine();
                string result = proc.StandardOutput.ReadLine();
                valor v = new valor()
                {
                    date = DateTime.Now,
                    name = result,
                };
                if (result.Contains(IP))
                {
                    v.status = "ok";
                }
                else
                {
                    v.status = "fail";
                }

                return v;
            }
            catch (Exception e)
            {
                valor v = new valor()
                {
                    date = DateTime.Now,
                    name = "no",
                };

                v.status = "fail";
                return v;
            }
        }
    }
}