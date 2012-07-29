using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Web;

namespace Wren.Core.Services
{
    public abstract class Service
    {
        public TResult WebGetById<TResult>(String id, String path)
        {
            if (WrenCore.IsOffline)
                return default(TResult);

            WebClient client = new WebClient();

            var url = String.Format("http://{0}/{1}/{2}",
                GetServerAddress(),
                path,
                HttpUtility.UrlEncode(id));

            try
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                var result = client.DownloadString(url);
                return JsonConvert.DeserializeObject<TResult>(result);
            }
            catch (WebException)
            {
                WrenCore.IsOffline = true;
                return default(TResult);
            }
        }

        public TResult WebGet<TResult>(String path, params KeyValuePair<String, String>[] parameters)
        {
            if (WrenCore.IsOffline)
                return default(TResult);

            WebClient client = new WebClient();

            var url = String.Format("http://{0}/{1}?",
                GetServerAddress(),
                path);

            foreach (var p in parameters)
            {
                url = url + HttpUtility.UrlEncode(p.Key) + "=" + HttpUtility.UrlEncode(p.Value) + "&";
            }

            url = url.Remove(url.Length - 2, 2);

            try
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                var result = client.DownloadString(url);
                return JsonConvert.DeserializeObject<TResult>(result);
            }
            catch (WebException)
            {
                WrenCore.IsOffline = true;
                return default(TResult);
            }
        }

        public TResult WebPost<TResult>(IServiceParameters parameters, String path)
        {
            if (WrenCore.IsOffline)
                return default(TResult);

            WebClient client = new WebClient();

            var url = String.Format("http://{0}/{1}",
                GetServerAddress(),
                path);

            try
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                var values = new NameValueCollection();

                foreach (var property in parameters.GetType().GetProperties())
                {
                    Object value = property.GetValue(parameters, null);

                    if (value != null)
                        values.Add(property.Name, value.ToString());
                    else
                        values.Add(property.Name, String.Empty);
                }

                var result = client.UploadValues(url, "POST", values);
                var resultString = Encoding.ASCII.GetString(result);
                return JsonConvert.DeserializeObject<TResult>(resultString);
            }
            catch (WebException)
            {
                WrenCore.IsOffline = true;
                return default(TResult);
            }
        }

        private String GetServerAddress()
        {
            return WrenCore.ServerAddress;
        }
    }
}
