using System;

namespace FileEncryptorWpf.Models
{
    /// <summary>
    /// An MVVM model class that is used for wrapping two actions for reporting some operations progress, via text and percentage.
    /// </summary>
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
