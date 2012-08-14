using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Synchronization
{
    public abstract class Task
    {
        protected Task(string name)
        {
            Name = name;
            CanDeterminePercentage = false;
        }

        protected Task()
        {
            Name = GetType().Name;
            CanDeterminePercentage = false;
        }

        public abstract int PercentDone { get; }

        public bool CanDeterminePercentage { get; protected set; }

        public string CurrentOperation { get; protected set; }

        public string Name { get; private set; }

        public abstract void Run();

        public bool Done { get; set; }

        public bool IsProcessing { get; set; }

        public string Error { get; set; }
    }
}
