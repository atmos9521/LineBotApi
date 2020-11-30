using LineApi.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace LineApi.MyMethods
{
    public class JsonMethods
    {
        //讀URL JSON檔案
        public static string DownloadJsonAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 30000;
            string result = "";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        //寫JSON檔案
        public static void WriteToAzureBlob(string SendJsonString
                                          , string url
                                          , string StorageAccountConnectString
                                          , string BlobContainerName
                                          , string BlobFileName
                                          ){
            // Retrieve storage account from connection string.
            //[Storage的連接字串]            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageAccountConnectString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve reference to a previously created container.
            //[容器名稱]
            CloudBlobContainer container = blobClient.GetContainerReference(BlobContainerName);

            // Retrieve reference to a blob named "[要上傳的檔名]".
            //[要上傳的檔名]
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(BlobFileName);

            //SendJsonString "[要上傳的Json字串]"
            blockBlob.UploadText(SendJsonString);
            // Create or overwrite the "[要上傳的檔名]" blob with contents from a local file.
            //using (var fileStream = System.IO.File.OpenRead(str))
            //{
            //    blockBlob.UploadFromStream(fileStream);
            //}
        }
    }
}