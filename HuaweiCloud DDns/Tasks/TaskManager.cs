using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace HuaweiCloud_DDns.Tasks
{
    internal sealed class TaskManager
    {
        private static TaskManager m_Instance;
        public static TaskManager Instance => m_Instance ?? (m_Instance = new TaskManager());

        private readonly Dictionary<TaskTypes, Type> m_TypeMap;
        private readonly HashSet<Task> m_Tasks;

        public IReadOnlyCollection<Task> Tasks => m_Tasks;

        public TaskManager()
        {
            m_TypeMap = new Dictionary<TaskTypes, Type>();
            m_Tasks = new HashSet<Task>();

            var basicType = typeof(Task);
            foreach (var type in GetType().Assembly.GetTypes())
            {
                if (type.IsAssignableFrom(basicType))
                {
                    var attributes = type.GetCustomAttributes<TaskAttribute>();
                    foreach (var attribute in attributes)
                    {
                        if (!m_TypeMap.ContainsKey(attribute.Type))
                            m_TypeMap.Add(attribute.Type, type);
                        else Debug.Fail($"发现重复的Task实现！Class: {type.FullName} Type: {attribute.Type}");
                    }
                }
            }
        }

        public void AddTask(Task task)
        {
            if (!m_Tasks.Contains(task))
            {
                m_Tasks.Add(task);
                task.Active();
            }
        }

        public void RemoveTask(Task task)
        {
            if (m_Tasks.Remove(task))
                task.Deactive();
        }

        public void ParseTasks(JToken json)
        {
            if (json is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    if (array[i] is JObject child)
                    {
                        Debug.WriteLine($"开始解析第 {i} 条配置");
                        ParseTask(child);
                    }
                    else
                    {
                        Debug.Fail("无效的配置数据！");
                        break;
                    }
                }
            }
            else Debug.Fail("无效的配置文件！");
        }

        public void ParseTask(JObject json)
        {
            if (!json.ContainsKey("Type"))
            {
                Debug.Fail($"无法读取的Task配置，未配置TaskType！");
                return;
            }

            var typeId = json["Type"].Value<TaskTypes>();
            if (!m_TypeMap.TryGetValue(typeId, out var type))
            {
                Debug.Fail($"未知的Task类型，类型: {json["Type"].Value<string>()}");
                return;
            }

            var task = Activator.CreateInstance(type) as Task;
            if (!task.TryParse(json))
            {
                Debug.Fail($"无法读取的Task配置，配置内容错误！");
                return;
            }

            AddTask(task);
        }
    }
}
