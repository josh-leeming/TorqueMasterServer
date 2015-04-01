using System;
using System.Collections.Generic;
using System.Net;
using MasterServer.ServiceInterface;
using MasterServer.Torque;
using TinyIoC;
using CommandLine;

namespace MasterServer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = TinyIoCContainer.Current;

            var options = new Options();
            try
            {
                if (Parser.Default.ParseArguments(args, options))
                {
                    container.Register(options);

                    TorqueMasterServerIoC.ConfigureContainer(container); 

                    var masterServer = container.Resolve<TorqueMasterServer>();
                    container.BuildUp(masterServer);

                    masterServer.StartMasterServer();

                    System.Console.ReadLine();

                    masterServer.StopMasterServer();

                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            System.Console.WriteLine("\nPress any key to exit...");
            System.Console.ReadLine();
        }
    }

    public static class TorqueMasterServerIoC
    {
        public static void ConfigureContainer(TinyIoCContainer container)
        {
            var opts = container.Resolve<Options>();

            container.Register(typeof(ILogHandler), typeof(Logger.Log4NetLogger)).AsSingleton();
            container.Register<InMemorySessionManager>().AsSingleton();

            var sessionManager = container.Resolve<InMemorySessionManager>();
            container.Register<ISessionHandler, InMemorySessionManager>(sessionManager);

            container.RegisterMultiple<IMessageLifecycleCallbackHandler>(new List<Type>()
            {
                typeof (SessionMessageLifecycleCallback),
                typeof (SpamManager)
            }).AsSingleton();

            IPAddress ipAddress;

            if (IPAddress.TryParse(opts.IPAddress, out ipAddress) == false)
            {
                throw new ArgumentException("Failed to parse IP Address: " + opts.IPAddress);
            }

            container.Register(typeof(TorqueMasterServer),
                new TorqueMasterServer(container, new IPEndPoint(ipAddress, opts.Port))); 
        }
    }
}
