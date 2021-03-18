using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Resto.Front.Api.CheckBookPlugin
{
    public class HttpSenderTask
    {
        public string Url { get; set; }

        public string Json { get; set; }

        public int ttl { get; set; } = 1;

        public HttpMethod Method { get; set; } = HttpMethod.Post;
    }

    public class HttpSender : IDisposable
    {
        public static string PostChequeUrl;
        private static ConcurrentQueue<HttpSenderTask> _tasks = new ConcurrentQueue<HttpSenderTask>();
        private Task _task;
        static ManualResetEvent _waitHandle = new ManualResetEvent(false);
        private bool _isCanceled = false;

        public HttpSender()
        {
            PostChequeUrl = ConfigurationManager.AppSettings["PostChequeUrl"];
        }

        public static void AddTask(HttpSenderTask task)
        {
            PluginContext.Log.Info($"HttpSender add task");
            try
            {
                lock(_tasks)
                {
                    _tasks.Enqueue(task);
                    _waitHandle.Set();
                    PluginContext.Log.Info($"HttpSender task added");
                }
            } 
            catch (Exception e)
            {
                PluginContext.Log.Error($"error enqueue http task {e}");
            }
        }

        public void Run()
        {
            _task = Task.Run(() => Work());
        }

        private void Work()
        {
            while (!_isCanceled || _tasks?.Any() == true)
            {
                _waitHandle.WaitOne();
                lock (_tasks)
                {
                    while (_tasks.TryDequeue(out HttpSenderTask task))
                    {
                        PluginContext.Log.Info($"HttpSender get new task");
                        try
                        {
                            if (task.Method == HttpMethod.Post)
                                Post(task.Url, task.Json);
                            else if (task.Method == HttpMethod.Delete)
                                Delete(task.Url, task.Json);
                            else throw new ArgumentOutOfRangeException($"Unknown http method {task.Method}");
                            PluginContext.Log.Info($"HttpSender task complete");
                        }
                        catch (Exception e)
                        {
                            task.ttl--;
                            PluginContext.Log.Error($"error in http request to {task.Url} {e}");
                            if (task.ttl > 0)
                            {
                                AddTask(task);
                            }
                            Thread.Sleep(1000);
                        }
                    }
                    _waitHandle.Reset();
                }
            }
        }

        private void Post(string url, string json)
        {
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.PostAsync(url, data).ConfigureAwait(false).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    PluginContext.Log.Info($"POST {url}{Environment.NewLine} {json}");
                }
                catch (Exception e)
                {
                    PluginContext.Log.Error($"http post error {url}{Environment.NewLine} {json}", e);
                }
            }
        }

        private void Delete(string url, string guid)
        {
            var urlWithParam = url + $"?guid={guid}";
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.DeleteAsync(urlWithParam).ConfigureAwait(false).GetAwaiter().GetResult();
                    response.EnsureSuccessStatusCode();
                    PluginContext.Log.Info($"DELETE {urlWithParam}");
                }
                catch (Exception e)
                {
                    PluginContext.Log.Error($"http delete error {urlWithParam}", e);
                }
            }
        }

        public void Dispose()
        {
            PluginContext.Log.Info("HttpSender dispose.");
            if (_task != null)
            {
                _isCanceled = true;
                _waitHandle.Set();
                _task.Wait();
            }
        }
    }
}
