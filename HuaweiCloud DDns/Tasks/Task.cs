using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace HuaweiCloud_DDns.Tasks
{
    internal abstract class Task
    {
        private bool m_Actived = false;

        private string m_Region;
        private string m_Zone;
        private string m_RecorderSet;
        private string m_NSType;

        public string Region => m_Region;
        public string Zone => m_Zone;
        public string RecorderSet => m_RecorderSet;
        public string NSType => m_NSType;

        protected virtual void OnActive() { }
        protected virtual void OnDeactive() { }

        public void Active()
        {
            if (m_Actived) return;
            OnActive();
            m_Actived = true;
        }

        public void Deactive()
        {
            if (!m_Actived) return;
            OnDeactive();
            m_Actived = false;
        }

        protected virtual void Parse(JToken json)
        {
            m_Region = json["Region"].Value<string>();
            m_Zone = json["Zone"].Value<string>();
            m_RecorderSet = json["RecorderSet"].Value<string>();
            m_NSType = json["RecorderSet"].Value<string>();
        }

        public bool TryParse(JToken json)
        {
            try { Parse(json); }
            catch (System.Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
            return true;
        }
    }
}
