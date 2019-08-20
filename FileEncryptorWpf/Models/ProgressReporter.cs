using System;

namespace FileEncryptorWpf.Models
{
    public class ProgressReporter
    {
        private Action<string> logIt { get; set; }
        private Action<int> percentageDone { get; set; }

        public ProgressReporter(Action<string> log, Action<int> percentage)
        {
            this.logIt = log;
            this.percentageDone = percentage;
        }

        public void Log(string line)
        {
            if(logIt != null)
            {
                this.logIt(line);
            }
        }

        public void SetPercentage(int i)
        {
            if (percentageDone != null)
            {
                this.percentageDone(i);
            }
        }
    }
}
