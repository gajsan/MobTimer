using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MobTimer
{
    class MemberListStorage
    {
        private const string MEMBER_LIST_FILE_NAME = "MobMemberList.txt";

        public static void StoreMemberList(List<string> memberList)
        {
            string path = GetFilePath();

            var json = JsonConvert.SerializeObject(memberList);

            try {
                File.AppendAllText(path, json);
            }
            catch { }
        }

        public static List<string> GetMemberList()
        {
            string path = GetFilePath();

            if (!File.Exists(path))
            {
                return new List<string>();
            }
            
            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<string>>(json);
            }
            catch { }

            return new List<string>();
        }

        private static string GetFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), MEMBER_LIST_FILE_NAME);
        }
    }
}
