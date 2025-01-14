using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public abstract class HttpServerBase : IDisposable
{
    private readonly HttpListener _listener;
    private readonly Thread _listenerThread;
    private readonly Thread[] _workers;
    private readonly ManualResetEvent _stop, _ready;
    private Queue<HttpListenerContext> _queue;
    private event Action<HttpListenerContext> ProcessRequest;
    public Dictionary<string, Action<HttpListenerContext>> _actionDict;

    public void AddPathListener(string url, Action<HttpListenerContext> action)
    {
        _actionDict.Add(url, action);
    }

    public HttpServerBase(int maxThreads)
    {
        _workers = new Thread[maxThreads];
        _queue = new Queue<HttpListenerContext>();
        _stop = new ManualResetEvent(false);
        _ready = new ManualResetEvent(false);
        _listener = new HttpListener();
        _listenerThread = new Thread(HandleRequests);
        _actionDict = new Dictionary<string, Action<HttpListenerContext>>();
    }

    public void Start(String bindIP, int port)
    {
        ProcessRequest += ProcessHttpRequest;
        _listener.Prefixes.Add(String.Format("http://{0}:{1}/", bindIP, port));
        _listener.Start();
        _listenerThread.Start();

        for (int i = 0; i < _workers.Length; i++)
        {
            _workers[i] = new Thread(Worker);
            _workers[i].Start();
        }
        Console.WriteLine("HttpServer Listen on " + port);
    }

    protected abstract void ProcessHttpRequest(HttpListenerContext ctx);

    public void Dispose()
    {
        Stop();
    }

    public void Stop()
    {
        _stop.Set();
        _listenerThread.Join();
        foreach (Thread worker in _workers)
        {
            worker.Join();
        }
        _listener.Stop();
    }

    private void HandleRequests()
    {
        while (_listener.IsListening)
        {
            var context = _listener.BeginGetContext(ContextReady, null);
            if (0 == WaitHandle.WaitAny(new[] { _stop, context.AsyncWaitHandle }))
            {
                return;
            }
        }
    }

    private void ContextReady(IAsyncResult ar)
    {
        try
        {
            lock (_queue)
            {
                _queue.Enqueue(_listener.EndGetContext(ar));
                _ready.Set();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(string.Format("[HttpServerBase::ContextReady]err:{0}", e.Message));
        }
    }

    private void Worker()
    {
        WaitHandle[] wait = new[] { _ready, _stop };
        while (0 == WaitHandle.WaitAny(wait))
        {
            HttpListenerContext context;
            lock (_queue)
            {
                if (_queue.Count > 0)
                    context = _queue.Dequeue();
                else
                {
                    _ready.Reset();
                    continue;
                }
            }

            try
            {
                ProcessRequest(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("[HttpServerBase::Worker]err:{0}", e.Message));
            }
        }
    }
}