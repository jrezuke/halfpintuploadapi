using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NLog;

namespace HpUploadApi.Controllers
{
    public class UploadController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<HttpResponseMessage> PostFormData()
        {
            Logger.Info("Starting api upload");
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                Logger.Info("api upload: IsMimeMultipartContent is false");
            
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new CustomMultipartFormDataStreamProvider(root);
            Logger.Info("after CustomMultipartFormDataStreamProvider instanced: " + provider.ToString());
            try
            {
                var sb = new StringBuilder(); // Holds the response body

                // Read the form data and return an async task.
                Logger.Info("api upload: before await");
            
                await Request.Content.ReadAsMultipartAsync(provider);

                Logger.Info("api upload: after await");
            
                // This illustrates how to get the form data.
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        sb.Append(string.Format("{0}: {1}\n", key, val));
                    }
                }

                Logger.Info("api upload: before foreach file");
                // This illustrates how to get the file names for uploaded files.
                foreach (var file in provider.FileData)
                {
                    var fileInfo = new FileInfo(file.LocalFileName);
                    sb.Append(string.Format("Uploaded file: {0} ({1} bytes)\n", fileInfo.Name, fileInfo.Length));
                }
                Logger.Info("api upload: after foreach file");
                return new HttpResponseMessage()
                {
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

