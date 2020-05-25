using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace GGXrdWakeupDPUtil.Library
{
    public class LogManager
    {
        private string _fileName = "Log.txt";

        public event EventHandler<string> LineReceived;
        private readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
        private readonly Timer _queueTimer;


        public LogManager()
        {
            this._queueTimer = new Timer(1000);
            this._queueTimer.Elapsed += QueueTimer_Elapsed;

            this._queueTimer.Enabled = true;
        }

        private void QueueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.DequeueMessage();
        }

        private void DequeueMessage()
        {
            while (_messageQueue.TryDequeue(out var message))
            {
                ProcessMessage(message);
            }
        }


        private void ProcessMessage(string message)
        {
            Console.WriteLine(message);

            LineReceived?.Invoke(null, message);


#if !DEBUG
            try
            {
                using (FileStream fileStream = new FileStream(_fileName, FileMode.OpenOrCreate, FileSystemRights.AppendData, FileShare.Write, 4096, FileOptions.None))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.AutoFlush = true;
                        streamWriter.WriteLine(message);
                    }
                }
            }
            catch (Exception) { }
#endif
        }

        public void WriteLine(string message)
        {
            string line = $"[{DateTimeOffset.Now.ToUniversalTime():yyyy-MM-dd HH:mm:ss}] {message}";
            this._messageQueue.Enqueue(line);

        }

        public void WriteException(Exception exception)
        {
            this.WriteLine($"{exception.GetType().Name}: {exception.Message}\r\n{exception.StackTrace}");
        }
    }
}
