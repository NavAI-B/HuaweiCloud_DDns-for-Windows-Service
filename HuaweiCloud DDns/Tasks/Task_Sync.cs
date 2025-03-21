using HuaweiCloud.SDK.Dns.V2.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HuaweiCloud_DDns.Tasks
{
    [Task(TaskTypes.Sync)]
    internal sealed class Task_Sync : Task
    {
        private readonly List<string> m_Records = new List<string>();

        protected override void Parse(JToken json)
        {
            base.Parse(json);

            m_Records.Clear();

            var token = json["Records"];
            if (token is JArray array)
            {
                foreach (var item in array)
                    m_Records.Add(item.Value<string>());
            }
            else m_Records.Add(token.Value<string>());
        }

        protected override void OnActive() { SyncRecords(); }

        private void SyncRecords()
        {
            try
            {
                var client = new Client(GlobalConfig.SDK_AK, GlobalConfig.SDK_SK, Region);
                var zones = client.GetPublicZones();
                var zone = zones.Find(i => i.Name == Zone);

                var recordersets = client.GetRecorderSets(zone.Id);
                var recorderset = recordersets.Find(i => i.Name == RecorderSet && i.Type == NSType);
                var req = new UpdateRecordSetRequest
                {
                    ZoneId = zone.Id,
                    RecordsetId = recorderset.Id,
                    Body = new UpdateRecordSetReq { Records = m_Records }
                };
                client.SetRecord(req);
            }
            catch (Exception e) { Debug.Fail(e.ToString()); }

            TaskManager.RemoveTask(this);
        }
    }
}
