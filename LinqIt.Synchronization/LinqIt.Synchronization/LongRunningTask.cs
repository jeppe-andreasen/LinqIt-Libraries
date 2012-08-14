using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using LinqIt.Ajax;
using LinqIt.Ajax.Parsing;
using LinqIt.Utils.Web;

namespace LinqIt.Synchronization
{
    public class LongRunningTask
    {
        private readonly ConcurrentQueue<Task> _tasks;
        //private string _applicationPath;
        private Thread _thread;

        public LongRunningTask(Page page)
        {
            _tasks = new ConcurrentQueue<Task>();
            //_applicationPath = HttpContext.Current.Server.MapPath("~/");
            Instance = this;
            AjaxUtil.RegisterPageMethod(page, this, "GetLongRunningState");
        }

        public void AddTask(Task task)
        {
            _tasks.Enqueue(task);
        }

        public IEnumerable<Task> Tasks
        {
            get { return _tasks; }
        }

        public void Start()
        {
            ThreadStart threadStart = () => Execute(this);
            _thread = new Thread(threadStart);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public static void Execute(object state)
        {
            var mainTask = (LongRunningTask) state;
            foreach (var task in mainTask._tasks)
            {
                task.IsProcessing = true;
                try
                {
                    task.Run();
                }
                catch(Exception exc)
                {
                    task.Error = exc.ToString();
                    task.IsProcessing = false;
                    break;
                }
                task.IsProcessing = false;
                task.Done = true;
            }
            if (mainTask.OnComplete != null)
                mainTask.OnComplete(mainTask, null);
        }

        public static LongRunningTask Instance
        {
            get { return (LongRunningTask)HttpContext.Current.Session["LongRunningTask"]; }
            private set { HttpContext.Current.Session["LongRunningTask"] = value; }
        }

        [AjaxMethod]
        public static JSONObject GetLongRunningState()
        {
            var result = new JSONObject();
            var isDone = true;
            var html = HtmlWriter.Generate(w => RenderState(ref isDone, w));
            var errors = HtmlWriter.Generate(RenderErrors);
            result.AddValue("html", html);
            result.AddValue("errors", errors);
            result.AddValue("done", isDone);

            if (isDone)
                Instance = null;

            return result;
        }

        private static void RenderState(ref bool isDone, HtmlWriter w)
        {
            if (Instance == null)
                return;

            w.RenderBeginTag(HtmlTextWriterTag.Table);
            foreach (var task in Instance.Tasks)
            {
                w.RenderBeginTag(HtmlTextWriterTag.Tr);
                w.RenderBeginTag(HtmlTextWriterTag.Td, "title");
                w.RenderFullTag(HtmlTextWriterTag.Span, task.Name);
                w.RenderEndTag();
                w.RenderBeginTag(HtmlTextWriterTag.Td, "progress");
                if (!string.IsNullOrEmpty(task.Error))
                {
                    w.RenderFullTag(HtmlTextWriterTag.Span, "Error", "processing");
                }
                else if (task.IsProcessing)
                {
                    var progress = "Processing";
                    if (task.CanDeterminePercentage)
                        progress += " " + task.PercentDone + "%";

                    w.RenderFullTag(HtmlTextWriterTag.Span, progress, "processing");
                    isDone = false;
                }
                else if (task.Done)
                {
                    w.RenderFullTag(HtmlTextWriterTag.Span, "Done", "done");
                }
                else
                {
                    w.Write("&nbsp;");
                    isDone = false;
                }
                w.RenderEndTag();
                w.RenderEndTag(); // tr
            }
            w.RenderEndTag();
        }

        private static void RenderErrors(HtmlWriter writer)
        {
            if (Instance == null)
                return;

            foreach (var task in Instance.Tasks)
            {
                if (!string.IsNullOrEmpty(task.Error))
                    writer.RenderFullTag(HtmlTextWriterTag.P, task.Error);
            }

        }

        public event EventHandler OnComplete;
    }
}
