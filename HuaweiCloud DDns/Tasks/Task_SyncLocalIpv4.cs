using HuaweiCloud.SDK.Dns.V2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace HuaweiCloud_DDns.Tasks
{
    [Task(TaskTypes.Sync_LocalIpv4)]
    internal sealed class Task_SyncLocalIpv4 : Task
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
                var records = new List<string>();

                var host = Dns.GetHostName();
                var ipAddresses = Dns.GetHostAddresses(host);

                Debug.WriteLine("LocalIpv4s:");
                foreach (var iPAddress in ipAddresses)
                {
                    if (iPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        var ip = iPAddress.ToString();
                        Debug.WriteLine(ip);
                        records.Add(ip);
                    }
                }

                var client = new Client(GlobalConfig.SDK_AK, GlobalConfig.SDK_SK, Region);
                var zones = client.GetPublicZones();
                var zone = zones.Find(i => i.Name == Zone);

                var recordersets = client.GetRecorderSets(zone.Id);
                var recorderset = recordersets.Find(i => i.Name == RecorderSet && i.Type == NSType);
                var req = new UpdateRecordSetRequest
                {
                    ZoneId = zone.Id,
                    RecordsetId = recorderset.Id,
                    Body = new UpdateRecordSetReq { Records = records }
                };
                client.SetRecord(req);
            }
            catch (Exception e) { Debug.Fail(e.ToString()); }
        }
    }
}
