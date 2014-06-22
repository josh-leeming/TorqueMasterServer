using System;
using System.Collections.Generic;
using System.Net;
using MasterServer.ServiceInterface;
using MasterServer.Torque;
using TinyIoC;

namespace MasterServer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = TinyIoCContainer.Current;

            TorqueMasterServerIoC.ConfigureContainer(container);

            var masterServer = container.Resolve<TorqueMasterServer>();
            container.BuildUp(masterServer);

            masterServer.StartMasterServer();

            System.Console.ReadLine();

            masterServer.StopMasterServer();

            System.Console.ReadLine();
        }
    }

    public static class TorqueMasterServerIoC
    {
        public static void ConfigureContainer(TinyIoCContainer container)
        {
            container.Register(typeof(ILogHandler), typeof(Logger.Log4NetLogger)).AsSingleton();
            container.Register<InMemorySessionManager>().AsSingleton();

            var sessionManager = container.Resolve<InMemorySessionManager>();
            container.Register<ISessionHandler, InMemorySessionManager>(sessionManager);

            container.RegisterMultiple<IMessageLifecycleCallbackHandler>(new List<Type>()
            {
                typeof (SessionMessageLifecycleCallback),
                typeof (SpamManager)
            }).AsSingleton();


            container.Register(typeof(TorqueMasterServer),
                new TorqueMasterServer(container, new IPEndPoint(IPAddress.Any, 28002))); 
        }
    }
}
