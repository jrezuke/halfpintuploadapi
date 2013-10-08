using System;
using System.Configuration;
using System.Data.SqlClient;
using NLog;

namespace HpUploadApi.Utility
{
    public static class Utils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool VerifyKey(string key, string fileName, string siteCode)
        {
            int ikey = 0;

            foreach (var c in fileName)
            {
                if (char.IsNumber(c))
                    ikey += int.Parse(c.ToString());
            }
            ikey *= ikey;

            int iInstitId = int.Parse(siteCode);
            ikey = ikey*iInstitId;

            int iFromClientKey = int.Parse(key.Substring(6));

            if (iFromClientKey == ikey)
                return true;

            return false;
        }

        public static int IsStudyCleared(string fileName)
        {
            if (fileName.StartsWith("T"))
                return 2;
            var studyId = fileName.Substring(0, 9);

            int retVal = IsStudyIdCleared(studyId);

            return retVal;
        }


        public static int IsStudyIdCleared(string studyId)
        {
            String strConn = ConfigurationManager.ConnectionStrings["Halfpint"].ToString();

            using (var conn = new SqlConnection(strConn))
            {
                try
                {
                    //throw new Exception("test error");
                    var cmd = new SqlCommand("", conn)
                              {
                                  CommandType = System.Data.CommandType.StoredProcedure,
                                  CommandText = "IsStudyIDCleared"
                              };
                    var param = new SqlParameter("@studyID", studyId);
                    cmd.Parameters.Add(param);

                    conn.Open();
                    var count = (Int32) cmd.ExecuteScalar();
                    if (count == 1)
                        return 1;
                    return 0;
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("IsStudyIdCleared Error", ex);
                    return -1;
                }
            }
        }
    }
}