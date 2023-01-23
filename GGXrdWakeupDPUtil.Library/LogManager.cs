using System;
using System.Collections.Concurrent;
using System.Timers;

namespace GGXrdWakeupDPUtil.Library
{
    public class LogManager
    {


        #region Singleton
        private static readonly Lazy<LogManager> lazy = new Lazy<LogManager>(() => new LogManager());
        public static LogManager Instance => lazy.Value;

        private LogManager()
        {
            this._queueTimer.Elapsed += QueueTimer_Elapsed;

            this._queueTimer.Enabled = true;
        }
        #endregion


        public string FileName { get; } = "Log.txt";

        public event EventHandler<string> LineReceived;
        private readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
        private readonly Timer _queueTimer = new Timer(1000);

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
                using (FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileSystemRights.AppendData, FileShare.Write, 4096, FileOptions.None))
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
