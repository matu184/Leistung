using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Script.Serialization;
using System.Threading;
using System.EnterpriseServices;
using System.Security.Cryptography.X509Certificates;


namespace Leistung
{
    public class Global : System.Web.HttpApplication
    {
        private static List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        private static WebSocketServer server = new WebSocketServer("ws://127.0.0.1:8081");
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        private static PerformanceCounter cpuCounter;
        private static PerformanceCounter ramCounter;
        private static PerformanceCounter diskCounter;
        


        protected void Application_Start(object sender, EventArgs e)
        {
            cpuCounter = new PerformanceCounter("Prozessor", "Prozessorzeit (%)", "_Total");
            ramCounter = new PerformanceCounter("Arbeitsspeicher", "Verfügbare MB");
            diskCounter = new PerformanceCounter("LogicalDisk", "% Free Space", "_Total");

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    allSockets.Add(socket);
                    Nachricht();

                };
                socket.OnClose = () =>
                {
                    allSockets.Remove(socket);
                };

                socket.OnMessage = message =>
                {
                 
                    socket.Send(serializer.Serialize(Nachricht("Hallo")));
                };

            });
            


        }
        protected static void Nachricht()
        {
            string cpuvalue = cpuCounter.NextValue().ToString();
            string diskvalue = diskCounter.NextValue().ToString();
            string ramvalue = ramCounter.NextValue().ToString();
            
            Nachricht zählerNachricht = new Nachricht { Cpu = cpuvalue, Disk = diskvalue, Mem = ramvalue };
            foreach (IWebSocketConnection s in allSockets.ToList())
            {
                s.Send(serializer.Serialize(zählerNachricht));
            }
        } 
        protected static Nachricht Nachricht(string x)
        {
            string cpuvalue = cpuCounter.NextValue().ToString();
            string diskvalue = diskCounter.NextValue().ToString();
            string ramvalue = ramCounter.NextValue().ToString();
            
            Nachricht zählerNachricht = new Nachricht { Cpu = cpuvalue, Disk = diskvalue, Mem = ramvalue };
            return zählerNachricht;
            
        }


    }
    public class Nachricht
    {
        public string Cpu { get; set; }
        public string Mem { get; set; }
        public string Disk { get; set; }
    }
}