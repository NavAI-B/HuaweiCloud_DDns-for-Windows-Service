using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;

namespace HuaweiCloud_DDns
{
    internal sealed class GlobalConfig
    {
        private static GlobalConfig m_Instance;
        public static GlobalConfig Instance => m_Instance ?? (m_Instance = new GlobalConfig());

        private string m_LogPath = "Service_Log.log";
        private string m_ListenerPip;

        public static string LogPath => Instance.m_LogPath;
        public static string ListenerPip => Instance.m_ListenerPip;

        private string m_SDK_AK;
        private string m_SDK_SK;

        public static string SDK_AK => Instance.m_SDK_AK;
        public static string SDK_SK => Instance.m_SDK_SK;

        private JToken m_Tasks;
        public static JToken Tasks => Instance.m_Tasks;

        public static void ReadConfig()
        {
            try
            {
                var json = JObject.Parse(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")));
                Instance.m_LogPath = json["LOG_PATH"].Value<string>();
                Instance.m_ListenerPip = json["LISTENER_PIP"].Value<string>();

                Instance.m_SDK_AK = json["HUAWEICLOUD_SDK_AK"].Value<string>();
                Instance.m_SDK_SK = json["HUAWEICLOUD_SDK_SK"].Value<string>();

                Instance.m_Tasks = json["TASKS"];
            }
            catch (Exception e)
            {
                FileLog.Initialize("Service_Log.log");
                Debug.Fail(e.ToString());
                throw e;
            }
        }
    }
}
