using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Timers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using TestPing.Controllers;

namespace TestPing
{
    public class Startup
    {
        public static List<valor> AllList = new List<valor>();
        public static string IP;
        public MongoClient MongoClient;
        public IMongoDatabase IDatabase;
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
            
            MongoClient = new MongoClient(
                "mongodb://root:Provista_123@13.77.174.235:27017"
            );
            IDatabase = MongoClient.GetDatabase("test");
            IP = Configuration.GetValue<string>("ip");
            Timer oTimer = new Timer();
            int time = Configuration.GetValue<int>("time");
            oTimer.Interval =time;

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
            try
            {
                var temp = ExecuteCommand("ping " + IP);    
                var collection = IDatabase.GetCollection<BsonDocument>("status");
                collection.InsertOne(temp);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }


        }

        static BsonDocument ExecuteCommand(string _Command)
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
                
                var document = new BsonDocument
                {
                    { "date", DateTime.Now },
                    { "name", result },
                    { "count", 1 },
                };

                if (result.Contains(IP))
                {
                    document = new BsonDocument
                    {
                        { "date", DateTime.Now },
                        { "name", result },
                        { "status", "ok" },
                    };
                }
                else
                {
                    document = new BsonDocument
                    {
                        { "date", DateTime.Now },
                        { "name", result },
                        { "status", "fail" },
                    };
                }

                return document;
            }
            catch (Exception e)
            {
                   
                var document = new BsonDocument
                {
                    { "date", DateTime.Now },
                    { "name", "fail" },
                    { "status", "fail" },
                };
                return document;
            }
        }
    }
}