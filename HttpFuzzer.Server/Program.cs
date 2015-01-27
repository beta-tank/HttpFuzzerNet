using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

/* Source version of this code : http://bartdesmet.net/blogs/bart/archive/2007/02/22/httplistener-for-dummies-a-simple-http-request-reflector.aspx
 * Github mirror : https://gist.github.com/delonnewman/6226881
 */


namespace HttpFuzzer.Server
{
    //Simple console server for testing fuzzer
    //Server must be running as administrator
    internal class Program
    {
        private static void Main()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            listener.Start();
            Console.WriteLine("Listening...");
            for (;;)
            {
                var ctx = listener.GetContext();
                new Thread(new Worker(ctx).ProcessAll).Start();
            }
        }

        #region Nested type: Worker

        private class Worker
        {
            private static int _count = 0;
            private readonly HttpListenerContext context;

            public Worker(HttpListenerContext context)
            {
                if (context == null) throw new ArgumentNullException("context");
                this.context = context;
            }

            public void ProcessAll()
            {
                var msg = context.Request.HttpMethod + " " + context.Request.Url;
                Console.WriteLine(_count + " " + DateTime.Now + " " + msg);
                Interlocked.Increment(ref _count);

                var sb = new StringBuilder();
                sb.Append("<html><body><h1>" + msg + "</h1>");
                DumpRequest(context.Request, sb);
                sb.Append("</body></html>");

                var b = Encoding.UTF8.GetBytes(sb.ToString());
                context.Response.ContentLength64 = b.Length;
                context.Response.OutputStream.Write(b, 0, b.Length);
                context.Response.OutputStream.Close();
            }

            public void ProcessRequest()
            {
                var msg = context.Request.HttpMethod + " " + context.Request.Url;
                Console.WriteLine(msg);

                var sb = new StringBuilder();
                sb.Append("<html><body><h1>" + msg + "</h1>");
                DumpRequest(context.Request, sb);
                sb.Append("</body></html>");

                var b = Encoding.UTF8.GetBytes(sb.ToString());
                context.Response.ContentLength64 = b.Length;
                context.Response.OutputStream.Write(b, 0, b.Length);
                context.Response.OutputStream.Close();
            }

            private void DumpRequest(HttpListenerRequest request, StringBuilder sb)
            {
                DumpObject(request, sb);
            }

            private void DumpObject(object o, StringBuilder sb)
            {
                DumpObject(o, sb, true);
            }

            private void DumpObject(object o, StringBuilder sb, bool ulli)
            {
                if (ulli)
                    sb.Append("<ul>");

                if (o is string || o is int || o is long || o is double)
                {
                    if (ulli)
                        sb.Append("<li>");

                    sb.Append(o);

                    if (ulli)
                        sb.Append("</li>");
                }
                else
                {
                    var t = o.GetType();
                    foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        sb.Append("<li><b>" + p.Name + ":</b> ");
                        object val = null;

                        try
                        {
                            val = p.GetValue(o, null);
                        }
                        catch
                        {
                        }

                        if (val is string || val is int || val is long || val is double)
                            sb.Append(val);
                        else if (val != null)
                        {
                            var arr = val as Array;
                            if (arr == null)
                            {
                                var nv = val as NameValueCollection;
                                if (nv == null)
                                {
                                    var ie = val as IEnumerable;
                                    if (ie == null)
                                        sb.Append(val);
                                    else
                                        foreach (object oo in ie)
                                            DumpObject(oo, sb);
                                }
                                else
                                {
                                    sb.Append("<ul>");
                                    foreach (string key in nv.AllKeys)
                                    {
                                        sb.AppendFormat("<li>{0} = ", key);
                                        DumpObject(nv[key], sb, false);
                                        sb.Append("</li>");
                                    }
                                    sb.Append("</ul>");
                                }
                            }
                            else
                                foreach (object oo in arr)
                                    DumpObject(oo, sb);
                        }
                        else
                        {
                            sb.Append("<i>null</i>");
                        }
                        sb.Append("</li>");
                    }
                }
                if (ulli)
                    sb.Append("</ul>");
            }
        }

        #endregion
    }
}