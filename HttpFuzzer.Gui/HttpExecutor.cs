using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HttpFuzzer.Gui.TestData;

namespace HttpFuzzer.Gui
{
    //Execute http requests asynchronously
    public class HttpExecutor
    {
        
        private static HttpClient client;

        public static bool Ready { get; private set; }

        public static int MaxConnectionsCount
        {
            get
            {
                return ServicePointManager.DefaultConnectionLimit;
            }
            set
            {
                ServicePointManager.DefaultConnectionLimit = value;
            }
        }

        public static string UserAgent
        {
            get { return client.DefaultRequestHeaders.UserAgent.First().Product.ToString(); }
            set
            {
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", value);
            }
        }

        static HttpExecutor()
        {
            Ready = false;
            MaxConnectionsCount = 100;
            Start();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
        }

        //Initialize parameters
        public static void Start()
        {
            if (client != null) return;
            client = new HttpClient();
            Ready = true;
        }

        private static string ParseGetParams(IEnumerable<BaseParameter> parameters)
        {
            if (parameters == null)
            {
                return string.Empty;
            }
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var baseParameter in parameters)
            {
                query[baseParameter.Name] = baseParameter.Value;
            }
           return query.ToString();
        }

        private static IEnumerable<KeyValuePair<string, string>> ParsePostParams(IEnumerable<BaseParameter> parameters)
        {
            if (parameters == null)
            {
                return new List<KeyValuePair<string, string>>();
            }
            return parameters.Select(p => new KeyValuePair<string, string>(p.Name, p.Value)).ToList();
        }

        public static void Stop()
        {
            if (client == null) return;
            client.Dispose();
            client = null;
            Ready = false;
        }

        public static async Task<HttpResponseMessage> Execute(string url, RequestType type , IEnumerable<BaseParameter> rawParameters, CancellationToken token)
        {
            HttpResponseMessage response;
            if (type == RequestType.Get)
            {
                var parameters = ParseGetParams(rawParameters);
                response = await client.GetAsync(url + (parameters == String.Empty ? String.Empty : "?" + parameters), token).ConfigureAwait(false);
            }
            else
            {
                var parameters = ParsePostParams(rawParameters);
                response = await client.PostAsync(url, new FormUrlEncodedContent(parameters), token).ConfigureAwait(false);
            }
            return response;
        }
    }

    public enum RequestType
    {
        Get,
        Post
    }
}
