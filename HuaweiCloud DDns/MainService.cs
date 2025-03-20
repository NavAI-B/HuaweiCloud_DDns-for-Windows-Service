using HuaweiCloud.SDK.Dns.V2.Model;
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
            var config = JObject.Parse(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")));
            FileLog.Initialize(config.Value<string>("LOG_PATH"));

            AK = config.Value<string>("HUAWEICLOUD_SDK_AK");
            SK = config.Value<string>("HUAWEICLOUD_SDK_SK");
            Region = config.Value<string>("HUAWEICLOUD_DNS_REGION");
            Zone = config.Value<string>("HUAWEICLOUD_DNS_PUBLIC_ZONE");
            RecorderSet = config.Value<string>("HUAWEICLOUD_DNS_RECORDERSET");
            NSType = config.Value<string>("HUAWEICLOUD_DNS_NSTYPE");

            Debug.WriteLine("ak = " + AK);
            Debug.WriteLine("sk = " + SK);
            Debug.WriteLine("region = " + Region);
            Debug.WriteLine("zone = " + Zone);
            Debug.WriteLine("recorderset = " + RecorderSet);
            Debug.WriteLine("nstype = " + NSType);

            UpdateIpAddress();

            NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
        }

        private void OnNetworkAddressChanged(object sender, EventArgs e)
        {
            UpdateIpAddress();
        }

        private void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
                UpdateIpAddress();
        }

        private void UpdateIpAddress()
        {
            try
            {
                var request = WebRequest.Create(NSType == "A" ? "https://4.ipw.cn/" : "https://6.ipw.cn/");
                request.Method = "GET";
                var response = request.GetResponse();
                var received = response.GetResponseStream();
                var reader = new StreamReader(received, Encoding.UTF8);
                var cip = reader.ReadToEnd();
                Debug.WriteLine("cip = " + cip);

                var client = new Client(AK, SK, Region);
                var zones = client.GetPublicZones();
                var zone = zones.Find(i => i.Name == Zone);

                var recordersets = client.GetRecorderSets(zone.Id);
                var recorderset = recordersets.Find(i => i.Name == RecorderSet && i.Type == NSType);
                foreach (var sip in recorderset.Records)
                    Debug.WriteLine("sip = " + sip);
                if (!recorderset.Records.Contains(cip))
                {
                    var req = new UpdateRecordSetRequest
                    {
                        ZoneId = zone.Id,
                        RecordsetId = recorderset.Id,
                        Body = new UpdateRecordSetReq { Records = new List<string> { cip } }
                    };
                    client.SetRecord(req);
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
            }
        }

        protected override void OnStop()
        {
            NetworkChange.NetworkAddressChanged -= OnNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged -= OnNetworkAvailabilityChanged;
        }
    }
}
