using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Utils.Extensions;

namespace LinqIt.Utils
{
    public class IterationUtil
    {
        public static IEnumerable<T> FindAllBFS<T>(T root, Func<T, IEnumerable<T>> childFunc, Func<T, bool> filterFunc)
        {
            var queue = new Queue<T>();
            queue.Enqueue(root);
            var result = new List<T>();
            while (queue.Count > 0)
            {
                var tmp = queue.Dequeue();
                if (filterFunc(tmp))
                    result.Add(tmp);
                queue.EnqueueRange(childFunc(tmp));
            }
            return result;
        }

        public static T FindFirstBFS<T>(T root, Func<T, IEnumerable<T>> childFunc, Func<T, bool> filterFunc)
        {
            var queue = new Queue<T>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var tmp = queue.Dequeue();
                if (filterFunc(tmp))
                    return tmp;
                queue.EnqueueRange(childFunc(tmp));
            }
            return default(T);
        }

        public static IEnumerable<T> FindAllDFS<T>(T root, Func<T, IEnumerable<T>> childFunc, Func<T, bool> filterFunc)
        {
            var stack = new Stack<T>();
            stack.Push(root);
            var result = new List<T>();
            while (stack.Count > 0)
            {
                var tmp = stack.Pop();
                if (filterFunc(tmp))
                    result.Add(tmp);
                stack.PushRange(childFunc(tmp).Reverse());
            }
            return result;
        }

        public static T FindFirstDFS<T>(T root, Func<T, IEnumerable<T>> childFunc, Func<T, bool> filterFunc)
        {
            Stack<T> stack = new Stack<T>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var tmp = stack.Pop();
                if (filterFunc(tmp))
                    return tmp;
                stack.PushRange(childFunc(tmp).Reverse());
            }
            return default(T);
        }
    }
}
