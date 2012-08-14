using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Utils.Extensions
{
    public static class QueueExtensions
    {
        #region Methods

        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> values)
        {
            foreach (T value in values)
                queue.Enqueue(value);
        }

        #endregion Methods
    }
}
