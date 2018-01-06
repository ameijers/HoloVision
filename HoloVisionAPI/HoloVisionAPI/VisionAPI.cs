using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace HoloVisionAPI
{
    public delegate void OnGetDataCompleted(string id, string json);

    public class VisionAPI : System.Object
    {
        public void GetDataAsync(string id, string url, byte[] body, string key, OnGetDataCompleted handler)
        {

        IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
            async (workItem) =>
            {
                try
                {
                    WebRequest webRequest = WebRequest.Create(url);
                    webRequest.Method = "POST";
                    webRequest.Headers["Content-Type"] = "application/octet-stream";
                    webRequest.Headers["Ocp-Apim-Subscription-Key"] = key;

                    Stream stream = await webRequest.GetRequestStreamAsync();
                    stream.Write(body, 0, body.Length);

                    WebResponse response = await webRequest.GetResponseAsync();
 
                    Stream result = response.GetResponseStream();
                    StreamReader reader = new StreamReader(result);
 
                    string json = reader.ReadToEnd();
 
                    handler(id, json);
                }
                catch (Exception ex)
                {
                    // handle errors
                }
            }
            );

        asyncAction.Completed = new AsyncActionCompletedHandler(GetDataAsyncCompleted);
        }

    private void GetDataAsyncCompleted(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
    {
    }

    }
}
