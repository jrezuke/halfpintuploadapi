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
            ikey = ikey * iInstitId;

            int iFromClientKey = int.Parse(key.Substring(6));

            if (iFromClientKey == ikey)
                return true;

            return false;
        }
    }
}