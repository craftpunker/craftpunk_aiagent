using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Battle
{
    class HttpServer : HttpServerBase
    {
        public HttpServer(int maxThreads) : base(maxThreads)
        {

        }
        protected override void ProcessHttpRequest(HttpListenerContext ctx)
        {
            HttpListenerRequest request = ctx.Request;
            HttpListenerResponse response = ctx.Response;
            try
            {
                var utf8WithoutBom = new System.Text.UTF8Encoding(false);
                if (request.HttpMethod == "POST")
                {
                    String url = request.RawUrl;
                    if (_actionDict.ContainsKey(url))
                    {
                        Action<HttpListenerContext> action = _actionDict[url];
                        action(ctx);
                    }
                    else
                    {
                        using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, utf8WithoutBom))
                        {
                            writer.Write("{ \"code\":1,\"msg\":\"url not impl\" }");
                        }
                    }
                } else {
                    using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, utf8WithoutBom))
                    {
                        writer.Write("{ \"code\":2,\"msg\":\"only support POST\" }");
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
            response.Close();
        }
    }
}

