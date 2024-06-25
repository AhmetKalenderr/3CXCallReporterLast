using _3CXCallReporterLast.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using _3CXCallReporterLast.Models;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using TCX.Configuration;
using _3CXCallReporterLast.Repository;

namespace _3CXCallReporterLast
{
    public class Startup
    {
        //static ConcurrentDictionary<string, QueueCustom> queueDC = new ConcurrentDictionary<string, QueueCustom>();

        //static ConcurrentDictionary<string, AgentConnection> agentDC = new ConcurrentDictionary<string, AgentConnection>();

        //public void Initiliaze()
        //{
        //    agentDC = new ConcurrentDictionary<string, AgentConnection>();
        //    queueDC = new ConcurrentDictionary<string, QueueCustom>();

        //    foreach (var queue in PhoneSystem.Root.GetQueues().ToList())
        //    {
        //        var q = new QueueCustom();
        //        q.QueueName = queue.Name;
        //        q.QueueNumber = queue.Number;
        //        q.WaitingCount = 0;
        //        q.queueWaitings = new List<QueueWaiting>();
        //        q.Calls = new ConcurrentDictionary<string, StartEnd>();
        //        queueDC.TryAdd(queue.Number, q);
        //    }

        //    foreach (var agent in PhoneSystem.Root.GetExtensions().ToList())
        //    {
        //        var a = new AgentConnection();
        //        a.AgentName = agent.FirstName + " " + agent.LastName;
        //        a.AgentNumber = agent.Number;
        //        a.AgentStatus = agent.IsRegistered ? agent.CurrentProfile.Name : "Offline";
        //        a.ConnectionName = string.Empty;//DBden çekilecek olan kullanýcý ismi
        //        a.LastChangeStatus = "";
        //        a.ConnectionNumber = "";
        //        agentDC.TryAdd(agent.Number, a);
        //        totalAgent++;

        //    }
        //}


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }
        public static PhoneSystem phoneSystem { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            CustomDatabaseRepository customerRepo = new CustomDatabaseRepository();
            customerRepo.CreateUserTableIfNotExists();
            try
            {
                var filePath = string.Empty;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    filePath = @"/var/lib/3cxpbx/Bin/3CXPhoneSystem.ini";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    filePath = @"C:\Program Files\3CX Phone System\Bin\3CXPhoneSystem.ini";
                }
                if (!File.Exists(filePath))
                {
                    throw new Exception("Cannot find 3CXPhoneSystem.ini");
                }
                ReadConfiguration(filePath);
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                Connection3cxApi();
                Start3cxEvents();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            services.AddCors();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "_3CXCallReporterLast", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "_3CXCallReporterLast v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(x=> 
                x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return Assembly.LoadFrom(@"/var/lib/3cxpbx/Bin/3cxpscomcpp2.dll");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Assembly.LoadFrom(@"C:\Program Files\3CX Phone System\Bin\3cxpscomcpp2.dll");
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        static string instanceBinPath;

        static void ReadConfiguration(string filePath)
        {
            var content = File.ReadAllLines(filePath);
            Dictionary<string, string> CurrentSection = null;
            string CurrentSectionName = null;
            for (int i = 1; i < content.Length + 1; i++)
            {
                var s = content[i - 1].Trim();
                if (s.StartsWith("["))
                {
                    CurrentSectionName = s.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    CurrentSection = GetConnectionStringClass.iniContent[CurrentSectionName] = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                }
                else if (CurrentSection != null && !string.IsNullOrWhiteSpace(s) && !s.StartsWith("#") && !s.StartsWith(";"))
                {
                    var res = s.Split("=").Select(x => x.Trim()).ToArray();
                    CurrentSection[res[0]] = res[1];
                }
                else
                {

                }
            }
            instanceBinPath = Path.Combine(GetConnectionStringClass.iniContent["General"]["AppPath"], "Bin");
        }

        public static void setPhoneSystem(PhoneSystem phoneSystem)
        {
            Startup.phoneSystem = phoneSystem;
        }

        static void Connection3cxApi()
        {
            try
            {
                PhoneSystem.CfgServerHost = "127.0.0.1";
                PhoneSystem.CfgServerPort = int.Parse(GetConnectionStringClass.iniContent["ConfService"]["ConfPort"]);
                PhoneSystem.CfgServerUser = GetConnectionStringClass.iniContent["ConfService"]["confUser"];
                PhoneSystem.CfgServerPassword = GetConnectionStringClass.iniContent["ConfService"]["confPass"];
                var ps = PhoneSystem.Reset(
                        PhoneSystem.ApplicationName + new Random(Environment.TickCount).Next().ToString(),
                        "127.0.0.1",
                        int.Parse(GetConnectionStringClass.iniContent["ConfService"]["ConfPort"]),
                        GetConnectionStringClass.iniContent["ConfService"]["confUser"],
                        GetConnectionStringClass.iniContent["ConfService"]["confPass"]
                    );
                ps.WaitForConnect(TimeSpan.FromSeconds(30));
                setPhoneSystem(ps);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Start3cxEvents()
        {
            PhoneSystem.Root.Inserted += new TCX.Configuration.NotificationEventHandler(Root_Inserted);
            PhoneSystem.Root.Updated += new TCX.Configuration.NotificationEventHandler(Root_Updated);
            PhoneSystem.Root.Deleted += new TCX.Configuration.NotificationEventHandler(Root_Deleted);
        }

        private void Root_Deleted(object sender, NotificationEventArgs e)
        {
            try
            {
                Listen3CXEvent(EventType.Delete, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Root_Updated(object sender, NotificationEventArgs e)
        {
            try
            {
                Listen3CXEvent(EventType.Update, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Root_Inserted(object sender, NotificationEventArgs e)
        {
            try
            {
                Listen3CXEvent(EventType.Insert, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Listen3CXEvent(EventType ev, NotificationEventArgs e)
        {
            if (e.ConfObject != null)
            {

                switch (e.EntityName)
                {

                    #region Connection
                    case "CONNECTION":
                        try
                        {
                            ActiveConnection acc = e.ConfObject as ActiveConnection;

                            //if (acc.DN is Queue)
                            //{

                            //    try
                            //    {
                            //        Queue queue = acc.DN as Queue;
                            //        QueueCustom queueWaiting = new QueueCustom();
                            //        queueDC[queue.Number].Calls.TryAdd(acc.CallID.ToString(), new StartEnd());

                            //        if (!queueDC.Keys.Contains(queue.Number))
                            //        {
                            //            queueWaiting.QueueName = queue.Name;
                            //            queueWaiting.QueueNumber = queue.Number;
                            //            queueWaiting.Calls = new ConcurrentDictionary<string, StartEnd>();
                            //            queueDC.TryAdd(queue.Number, queueWaiting);
                            //        }

                            //        setQueueListToClient(queueDC.ToString());
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Console.WriteLine(ex.Message);
                            //    }
                            //}
                            #endregion

                            #region AgentEvent

                           
                                try
                                {
                                    Extension extension = (Extension)acc.DN;
                                    AgentConnection agentconnection = new AgentConnection();                                  
                                    agentconnection.AgentName = extension.FirstName + " " + extension.LastName;
                                    agentconnection.AgentNumber = extension.Number;
                                    agentconnection.ConnectionNumber = acc.ExternalParty;
                                    agentconnection.ConnectionTime = DateTime.Now.ToString();
                                    setAgentCallDetail(agentconnection);


                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }

                                #endregion
                            
                            //else if (acc.DN is RoutePoint)
                            //{
                            //    try
                            //    {
                            //        brokerEvent.Enqueue(new QueueEvent() { CallStatus = acc.Status.ToString(), CallId = acc.CallID.ToString(), EntityType = EntityType.CONNECTION, EventType = ev, CreatedTime = DateTime.Now, Destination = Destination.IVR, InternalParty = acc.InternalParty, ConnectionNumber = acc.ExternalParty });
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Console.WriteLine(ex.Message);
                            //    }
                            //}
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;

                        //#endregion
                        #region DN
                        //case "DN":

                        //    try
                        //    {
                        //        DN dn = e.ConfObject as DN;
                        //        if (dn is Extension)
                        //        {
                        //            Extension ext = e.ConfObject as Extension;
                        //            brokerEvent.Enqueue(new QueueEvent() { EntityType = EntityType.DN, EventType = ev, CreatedTime = DateTime.Now, Destination = Destination.Extension, Name = ext.FirstName + " " + ext.LastName, Number = ext.Number, ConnectionNumber = "", CallId = "" });

                        //        }
                        //        else if (dn is Queue)
                        //        {
                        //            Queue que = e.ConfObject as Queue;
                        //            ///
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Console.WriteLine(ex.Message);
                        //    }

                        //    break;
                        #endregion
                }
            }
        }

        public static async void setAgentCallDetail(AgentConnection agentConnection)
        {
            CustomDatabaseRepository customRepo = new CustomDatabaseRepository();
            
            var url = "http://localhost:3005/agentCallDetail";
            AgentRequestModel model = new AgentRequestModel();
            try
            {
                model.Id = customRepo.GetDataByPhoneNumber(agentConnection.ConnectionNumber).Id;
                model.agentDid = agentConnection.AgentNumber;
                model.callerNumber = agentConnection.ConnectionNumber;
                model.callerName = customRepo.GetDataByPhoneNumber(agentConnection.ConnectionNumber).Name;
                model.tc = customRepo.GetDataByPhoneNumber(agentConnection.ConnectionNumber).TC;
                model.note = customRepo.GetDataByPhoneNumber(agentConnection.ConnectionNumber).Note;
                model.payment = customRepo.GetDataByPhoneNumber(agentConnection.ConnectionNumber).Payment;
                var payload = JsonConvert.SerializeObject(model);
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                var client = new HttpClient();
                var response = await client.PostAsync(url, content);
                
               
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
        }
    }
}