using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace HuaweiCloud_DDns
{
    internal sealed class CommandListener
    {
        private static CommandListener m_Instance;
        public static CommandListener Instance => m_Instance ?? (m_Instance = new CommandListener());

        private CancellationTokenSource m_CancelToken;
        private NamedPipeServerStream m_Pipe;
        private Task m_Task;

        private bool m_Running = false;

        public static void Begin(string name)
        {
            if (Instance.m_Running) return;

            Instance.m_CancelToken = new CancellationTokenSource();
            Instance.m_Pipe = new NamedPipeServerStream(name, PipeDirection.In);

            Instance.m_Task = Instance.PipeListenerAsyncProc(Instance.m_CancelToken.Token);
            Instance.m_Task.Start();

            Instance.m_Running = true;
        }

        public static void Stop()
        {
            if (!Instance.m_Running) return;

            Instance.m_CancelToken?.Cancel();
            Instance.m_Pipe?.Close();

            Instance.m_Pipe?.Dispose();
            Instance.m_CancelToken?.Dispose();

            Instance.m_Task?.Dispose();

            Instance.m_Running = false;
        }

        private async Task PipeListenerAsyncProc(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        await m_Pipe.WaitForConnectionAsync(ct);
                        using (var reader = new StreamReader(m_Pipe))
                        {
                            var command = await reader.ReadLineAsync();
                            if (!string.IsNullOrEmpty(command))
                                ExecuteCommand(command);
                        }
                        m_Pipe.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    if (!ct.IsCancellationRequested)
                        Debug.Fail(ex.ToString());
                }
            }
        }

        private void ExecuteCommand(string command)
        {
            var jObj = JObject.Parse(command);
            Tasks.TaskManager.ParseTask(jObj);
        }
    }
}
