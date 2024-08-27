using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace HuaweiCloud_DDns
{
    internal class FileLogListener : TraceListener
    {
        private FileStream stream;
        private StreamWriter writer;

        public FileLogListener(string path)
        {
            stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            writer = new StreamWriter(stream);
            writer.BaseStream.Position = writer.BaseStream.Length;
        }

        public override void Write(string message)
        {
            writer.Write(message);
            writer.Flush();
        }

        public override void WriteLine(string message)
        {
            writer.WriteLine(message);
            writer.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            writer?.Close();
            writer?.Dispose();
            writer = null;
            stream?.Close();
            stream?.Dispose();
            stream = null;
            base.Dispose(disposing);
        }
    }

    internal static class FileLog
    {
        private static List<FileLogListener> listeners = new List<FileLogListener>();

        public static void Initialize(string path)
        {
            try
            {
                var fullPath = Path.GetFullPath(path);
                if (fullPath.StartsWith(Environment.CurrentDirectory))
                    fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                var listener = new FileLogListener(fullPath);
                Debug.AutoFlush = true;
                Debug.Listeners.Add(listener);
                listeners.Add(listener);
            }
            catch (Exception e)
            {
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exception.log"), e.ToString());
            }
        }

        public static void Write(string msg)
        {
            foreach (var listener in listeners)
                listener.Write(msg);
        }

        public static void WriteLine(string msg)
        {
            foreach (var listener in listeners)
                listener.WriteLine(msg);
        }
    }
}
