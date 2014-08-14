using System;
using System.Web;
using System.Web.Http.WebHost;

namespace HpUploadApi.Infrastructure
{
    public class NoBufferPolicySelector : WebHostBufferPolicySelector
    {
        public override bool UseBufferedInputStream(object hostContext)
        {
            var context = hostContext as HttpContextBase;

            if (context != null)
            {
                if (string.Equals(context.Request.RequestContext.RouteData.Values["controller"].ToString(), "upload", StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            return true;
        }
    }
}