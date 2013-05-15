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
    public class NovaNetUploadController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<HttpResponseMessage> PostFormData()
        {
            Logger.Info("Starting novanet archives upload");

            if (!Request.Content.IsMimeMultipartContent())
            {
                Logger.Info("not IsMimeMultipartContent");
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //get the query strings
            var qsCol = Request.RequestUri.ParseQueryString();

            var folder = "app_data";
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
                if (e.InnerException != null)
                    Logger.Info("api upload inner exception: " + e.InnerException.Message);
                Logger.Error("exception:", e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
