using System;

namespace HuaweiCloud_DDns.Tasks
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class TaskAttribute : Attribute
    {
        private readonly TaskTypes m_Type;
        public TaskTypes Type => m_Type;

        public TaskAttribute(TaskTypes type) => m_Type = type;
    }
}
