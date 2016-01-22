using System.ServiceProcess;

namespace TigerText_EMR_Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MessageReceiver() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
