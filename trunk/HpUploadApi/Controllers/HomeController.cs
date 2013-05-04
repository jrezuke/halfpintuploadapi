using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HpUploadApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChecksUpload(HttpPostedFileBase file)
        {
            try
            {
                //nlogger.LogInfo("ChecksUpload");
                if (file != null && file.ContentLength > 0)
                {
                    string key = Request.Form["key"];
                    string institId = Request.Form["institID"];
                    //filename template : 01-0030-7copy.xlsm
                    var fileName = Path.GetFileName(file.FileName);
                    var studyId = fileName.Substring(0, 9);

                    //int iRetVal = DbUtils.IsStudyIDCleared(studyId);
                    //if (iRetVal != 0)
                    //{
                    //    nlogger.LogInfo("ChecksUpload - file name: " + fileName + ", IsStudyCleared: " + iRetVal);
                    //    return Content("IsStudyCleared: " + iRetVal);
                    //}
                    //nlogger.LogInfo("ChecksUpload - file name: " + fileName + ", key: " + key);

                    //if (!SsUtils.VerifyKey(key, fileName, institId))
                    //{
                    //    nlogger.LogInfo("ChecksUpload - bad key - file name: " + fileName + ", key: " + key);
                    //    return Content("Bad key");
                    //}

                    //var folderPath = ConfigurationManager.AppSettings["ChecksUploadPath"].ToString();
                    //var path = Path.Combine(folderPath, institId);

                    ////nlogger.LogInfo("ChecksUpload - path: " + path);
                    //if (!Directory.Exists(path))
                    //    Directory.CreateDirectory(path);

                    //path = Path.Combine(path, fileName);
                    //file.SaveAs(path);
                }
            }
            catch (Exception ex)
            {
                //nlogger.LogError("ChecksUpload - " + ex.Message);
            }

            //nlogger.LogInfo("ChecksUpload - OK");
            return Content("OK");
        }
    }


}
