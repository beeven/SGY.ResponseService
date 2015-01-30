using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.Reflection;

namespace ResponseService
{
    /// <summary>
    ///  服务安装类
    /// </summary>
    [RunInstaller(true)]
    public class ServiceInstaller : Installer
    {
        private System.ServiceProcess.ServiceInstaller serviceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;

        public ServiceInstaller()
        {
            InitializeComponent();

            // 获取目标服务的安装配置文件
            string t_path = Assembly.GetExecutingAssembly().Location;
            t_path = t_path.Substring(0, t_path.LastIndexOf('\\') + 1);
            string path = t_path + "\\Installer.config";

            InstallerSettings settings = new InstallerSettings(path);

            this.serviceInstaller.ServiceName = settings.ServiceName;
            this.serviceInstaller.DisplayName = settings.DisplayName;
            this.serviceInstaller.Description = settings.Description;
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // ServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstaller});
        }
    }
}
