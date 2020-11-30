using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LineApi.MyMethods
{
    public class LocalFileControl
    {
        /*寫檔案到本機目標 
         *(此專案無使用)
         */
        public static void WriteFileToLocal(string LocalFilePath, string FileData) {
            string str = System.Environment.CurrentDirectory;//C:\\Program Files (x86)\\IIS Express
            //"F:\\MyProgram\\Git\\LineBot_testGroup\\2020-11-28-1132\\LineApi\\LineApi\\api\\"
            File.WriteAllText(LocalFilePath, FileData);
        }
    }
}