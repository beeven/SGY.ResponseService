using System;
using System.Collections.Generic;
using System.Text;

namespace ResponseService
{
    /// <summary>
    ///  ������
    /// </summary>
    public class WinService : System.ServiceProcess.ServiceBase
    {
        /// <summary>
        /// The main entry point for the process
        /// </summary>
        static void Main()
        {
            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new WinService() };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ServiceName = "WinService";
        }

        /// <summary>
        ///  �������񷽷�
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            ResponseWrapper.getInstance().Start();
        }

        /// <summary>
        ///  ֹͣ���񷽷�
        /// </summary>
        protected override void OnStop()
        {
            ResponseWrapper.getInstance().Stop();
        }
    }
}
