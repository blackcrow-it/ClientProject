using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientProject.Services
{
    class APIHandle
    {
        public static string GET_UPLOAD_URL = "https://2-dot-backup-server-002.appspot.com/get-upload-token";
        public static string MEMBER_INFORMATION = "https://2-dot-backup-server-002.appspot.com/_api/v2/members/information";

        public static string API_LOGIN = "http://2-dot-backup-server-002.appspot.com/_api/v2/members/authentication";
   
        public static bool IS_LOGGED = false;
    }
}
