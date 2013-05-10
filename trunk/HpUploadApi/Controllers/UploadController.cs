using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using HpUploadApi.Utility;
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
                Logger.Info("not IsMimeMultipartContent");
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            
            //get the query strings
            var qsCol = Request.RequestUri.ParseQueryString();

            //Check key - if bad return
            if (!Utils.VerifyKey(qsCol["key"], qsCol["fileName"], qsCol["siteCode"]))
            {
                Logger.Info("ChecksUpload - bad key - file name: " + qsCol["fileName"] + ", key: " + qsCol["key"]);
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotAcceptable,
                    Content = new StringContent("Bad key")
                };
            }

            var savePath = GetSavePath(qsCol["siteCode"], qsCol["fileName"]);
            Logger.Info("Savepath:" + savePath);
            if (!Directory.Exists(savePath))
            {
                Logger.Info("Creating savepath");
                Directory.CreateDirectory(savePath);
            }

            //save the files in this folder
            string folder = savePath; //HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new CustomMultipartFormDataStreamProvider(folder);
            
            try
            {
                Logger.Info("Before  ReadAsMultipartAsync");
                //this gets the file stream form the request and saves to the folder
                await Request.Content.ReadAsMultipartAsync(provider);
                Logger.Info("api upload: after ReadAsMultipartAsync");
                
                
                // get the file info for uploaded file
                //var file = provider.FileData[0];
                //var fi = new FileInfo(file.LocalFileName);
                
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK")
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

        private static string GetSavePath(string siteCode, string fileName)
        {

            var path = fileName.EndsWith(".gif") ? ConfigurationManager.AppSettings["ChartPath"] : ConfigurationManager.AppSettings["ChecksUploadPath"];
            path = Path.Combine(path, siteCode);

            return path;
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
    }
}

