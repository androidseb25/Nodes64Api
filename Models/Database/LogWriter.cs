using System;
namespace Nodes64Api.Models.Database
{
    public class LogWriter
    {
        string logPath = "";
        FileStream? logFile = null;
        StreamWriter? logWriter = null;


        public LogWriter(string FileName)
        {
            logPath = Path.Combine(Directory.GetCurrentDirectory()) + $"/LOGS";
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            logPath += $"/{FileName}.txt";
            if (!File.Exists(logPath))
                logFile = File.Create(logPath);
            else
                logFile = File.OpenWrite(logPath);

            logWriter = new StreamWriter(logFile);
        }

        public void WriteLine(string log)
        {
            logWriter?.WriteLine($"{DateTime.Now:u} : " + log);
        }

        public void CloseLog()
        {
            logWriter?.Dispose();
        }

        protected virtual bool IsFileLocked(string path)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}

