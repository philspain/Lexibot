using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;

using LexibotData.DataAccess;
using LexibotLogic.DIContainer;

namespace ServiceInstaller
{
    class Program
    {
        // Path for service to install
        static readonly string _servicePath = "LexibotService.exe";

        static readonly string _serviceName = "LexibotService";

        static ILogger _logger = DependencyFactory.Resolve<ILogger>();

        static void Main(string[] args)
        {
            try
            {
                // attempt to install service
                ServiceController service = ServiceController.GetServices().Where(s => s.ServiceName == _serviceName).FirstOrDefault();

                if (service == null)
                {
                    ManagedInstallerClass.InstallHelper(new string[] { _servicePath });
                }

                // attempt to run service
                service = ServiceController.GetServices().Where(s => s.ServiceName == _serviceName).FirstOrDefault();

                if (service != null && service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message + "\n" + ex.InnerException + "\n" + ex.Source + "\n" + ex.StackTrace;
                _logger.LogError(message);
            }
        }
    }
}