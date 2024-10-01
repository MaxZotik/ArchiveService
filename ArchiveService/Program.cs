using System;
using System.ServiceProcess;

namespace ArchiveService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            #region Метод для запуска службы как консольное приложение (для отладки приложения)

            //if (Environment.UserInteractive)
            //{
            //    Service1 service1 = new Service1(args);
            //    service1.TestStartupAndStop(args);
            //}
            //else
            //{
            //    ServiceBase[] ServicesToRun;
            //    ServicesToRun = new ServiceBase[]
            //    {
            //    new Service1(args)
            //    };
            //    ServiceBase.Run(ServicesToRun);
            //}

            #endregion

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
