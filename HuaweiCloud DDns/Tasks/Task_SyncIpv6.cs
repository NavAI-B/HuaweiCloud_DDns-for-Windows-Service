using HuaweiCloud.SDK.Dns.V2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace HuaweiCloud_DDns.Tasks
{
    [Task(TaskTypes.Sync_Ipv6)]
    internal sealed class Task_SyncIpv6 : Task
    {
        protected override void OnActive()
        {
            UpdateIpAddress();
            NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
        }

        protected override void OnDeactive()
        {
            NetworkChange.NetworkAddressChanged -= OnNetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged -= OnNetworkAvailabilityChanged;
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
                var request = WebRequest.Create("https://6.ipw.cn/");
                request.Method = "GET";
                var response = request.GetResponse();
                var received = response.GetResponseStream();
                var reader = new StreamReader(received, Encoding.UTF8);
                var cip = reader.ReadToEnd();
                Debug.WriteLine("cip = " + cip);

                var client = new Client(GlobalConfig.SDK_AK, GlobalConfig.SDK_SK, Region);
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
            catch (Exception e) { Debug.Fail(e.ToString()); }
        }
    }
}
