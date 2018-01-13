using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace WPFBiz
{
    public class DataManager
    {
        public DataManager(string url)
        {
            this.url = url;
        }
        public string url { get; set; }
        public HttpClient GetHttpClient()
        {
            HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri(this.url);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return _client;
        }
        public T Get<T>(string url) where T: class
        {
            HttpClient client = this.GetHttpClient();
            HttpResponseMessage resp = client.GetAsync(url).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<T>().Result;
        }
        public List<T> GetMany<T>(string urlParameters) where T : class
        {
            HttpClient objHttp = this.GetHttpClient();
            HttpResponseMessage resp = objHttp.GetAsync(urlParameters).Result;
            resp.EnsureSuccessStatusCode();
            return resp.Content.ReadAsAsync<List<T>>().Result;
        }        
    }
}
