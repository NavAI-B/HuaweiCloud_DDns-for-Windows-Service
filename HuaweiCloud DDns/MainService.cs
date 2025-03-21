using HuaweiCloud.SDK.Dns.V2.Model;
using HuaweiCloud_DDns.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;

namespace HuaweiCloud_DDns
{
    public partial class MainService : ServiceBase
    {
        private string AK;
        private string SK;

        private string Region;
        private string Zone;
        private string RecorderSet;
        private string NSType;

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            GlobalConfig.ReadConfig();

            FileLog.Initialize(GlobalConfig.LogPath);

            TaskManager.ParseTasks(GlobalConfig.Tasks);
            CommandListener.Begin(GlobalConfig.ListenerPip);
        }

        protected override void OnStop()
        {
            CommandListener.Stop();
            TaskManager.RemoveAllTasks();
        }
    }
}
