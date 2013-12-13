using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NLog;

namespace HpUploadApi.Controllers
{
    public class LogUploadController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<HttpResponseMessage> PostFormData()
        {
            //Logger.Info("Starting novanet archives upload");

            if (!Request.Content.IsMimeMultipartContent())
            {
                Logger.Info("not IsMimeMultipartContent");
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //get the query strings
            var qsCol = Request.RequestUri.ParseQueryString();

            var siteCode = qsCol["siteCode"];
            var computerName = qsCol["computerName"];
            var fileName = qsCol["fileName"];

            string logUploadPath = ConfigurationManager.AppSettings["LogUploadPath"];
            string logFullUploadPath = Path.Combine(logUploadPath, siteCode, computerName);

            if (!Directory.Exists(logFullUploadPath))
            {
                Directory.CreateDirectory(logFullUploadPath);
                Logger.Info("Created folder for: " + logFullUploadPath);
            }

            //string filePathName = Path.Combine(logFullUploadPath, fileName);
            //var fi = new FileInfo(filePathName);
            //if (fi.Exists)
            //{
            //    //Logger.Info("File already exists on the server");
            //    return new HttpResponseMessage
            //    {
            //        StatusCode = HttpStatusCode.Conflict,
            //        Content = new StringContent("Duplicate")
            //    };
            //}
            var provider = new CustomMultipartFormDataStreamProvider(logFullUploadPath);

            try
            {
                //this gets the file stream form the request and saves to the folder
                await Request.Content.ReadAsMultipartAsync(provider);

                //Logger.Info("File uploaded to the server");
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK")
                };

            }
            catch (System.Exception e)
            {
                Logger.Info("api upload exception: " + e.Message);
                if (e.InnerException != null)
                    Logger.Info("api upload inner exception: " + e.InnerException.Message);
                Logger.Error("exception:", e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
