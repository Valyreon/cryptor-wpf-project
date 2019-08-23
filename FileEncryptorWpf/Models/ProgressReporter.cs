using System;

namespace FileEncryptorWpf.Models
{
    public class ProgressReporter
    {
        public ProgressReporter(Action<string> log, Action<int> percentage)
        {
            this.LogIt = log;
            this.PercentageDone = percentage;
        }

        private Action<string> LogIt { get; set; }

        private Action<int> PercentageDone { get; set; }

        public void Log(string line)
        {
            this.LogIt?.Invoke(line);
        }

        public void SetPercentage(int i)
        {
            this.PercentageDone?.Invoke(i);
        }
    }
}
