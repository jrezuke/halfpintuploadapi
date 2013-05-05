using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using NLog;

namespace HpUploadApi.Controllers
{
    public class UploadController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<HttpResponseMessage> PostFormData()
        {
            Logger.Info("Starting api upload");
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new CustomMultipartFormDataStreamProvider(root);
            
            try
            {
                var sb = new StringBuilder(); // Holds the response body

                // Read the form data and return an async task.
                Logger.Info("api upload: before await");
                await Request.Content.ReadAsMultipartAsync(provider);
                
                //get the form data.
                string tkey;
                string siteCode;
                string fileName;
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        if (key == "key")
                            tkey = val;
                        if (key == "siteCode")
                            siteCode = val;
                        sb.Append(string.Format("{0}: {1}\n", key, val));
                    }
                }

                Logger.Info("api upload: before foreach file");
                // get the file names for uploaded files.
                foreach (var file in provider.FileData)
                {
                    var fileInfo = new FileInfo(file.LocalFileName);
                    fileName = fileInfo.FullName;
                    sb.Append(string.Format("Uploaded file: {0} ({1} bytes)\n", fileInfo.Name, fileInfo.Length));
                }
                Logger.Info("api upload: after foreach file");
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Accepted,
                    Content = new StringContent(sb.ToString())
                };
                
            }
            catch (System.Exception e)
            {
                Logger.Info("api upload exception: " + e.Message);
                if(e.InnerException != null)
                    Logger.Info("api upload inner exception: " + e.InnerException.Message);
                Logger.Error("exception:",e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        { }


        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
            return name.Replace("\"", string.Empty); //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
        }

        public override Stream GetStream(HttpContent parent, System.Net.Http.Headers.HttpContentHeaders headers)
        {
            
            return base.GetStream(parent, headers);
        }
    }

    //public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    //{
    //    public CustomMultipartFormDataStreamProvider(string path)
    //        : base(path)
    //    {
    //    }

    //    public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
    //    {
    //        //validate headers.ContentDisposition.FileName as it will have the name+extension
    //        //then do something (throw error, continue with base or implement own logic)
    //    }

    //    public override Stream GetStream(HttpContent parent, System.Net.Http.Headers.HttpContentHeaders headers)
    //    {
    //        //validate headers.ContentDisposition.FileName as it will have the name+extension

    //        //then do something (throw error, continue with base or implement own logic)
    //    }
    //}
}

