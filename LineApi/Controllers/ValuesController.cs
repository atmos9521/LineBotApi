using LineApi.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace LineApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        /// Line機器人回覆API
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post()
        {
            string MyLineChannelAccessToken = "Flekh5YFBAXnWNpJf3CBt/oBfx116ZKzb6pkl4ieMRjkm5K4X47yOp8Ozc+XuuyOdG3neWIDNYe29s91vQ1Ienm1JvZO3bYggItzgOn02b+WcLfRXX38AHePg5kANUSxXI4Y3IXUpNLr9xaoMd/7WAdB04t89/1O/w1cDnyilFU=";
            var reqid = isRock.LineBot.Utility.Parsing(Request.Content.ReadAsStringAsync().Result).events[0].replyToken;
            string url = @"https://lineapistorage1128.blob.core.windows.net/memberlist/MemberList.json";
            try
            {
                //取得 http Post RawData(should be JSON)
                string postData = Request.Content.ReadAsStringAsync().Result;
                //剖析JSON
                var ReceivedMessage = isRock.LineBot.Utility.Parsing(postData);                
                //回覆訊息
                string Message = "";
                switch (ReceivedMessage.events[0].message.text)
                {
                    case "!時間":
                    case "!time":
                        Message = string.Format("現在時間:{0}  您說了:{1}", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), ReceivedMessage.events[0].message.text);
                        break;
                    case "!list":

                        break;
                    case "!add":
                        Members NewMber = new Members();
                        NewMber.Member_Group = "新露比";
                        NewMber.Member_LineName = "firechicken";
                        NewMber.Member_GameName = "chicken";
                        WriteJson(NewMber, url);
                        //Message = JsonConvert.SerializeObject("成功");
                        Message = "成功";
                        return Json(Message);
                        break;
                    case "!readjson":
                        //List<Members> MberList = new List<Members>();
                        //Members Mber = new Members();
                        //Mber.Member_Group = "露比";
                        //Mber.Member_LineName = "pekora";
                        //Mber.Member_GameName = "Usaga Pekora";
                        //MberList.Add(Mber);

                        //Members Mber2 = new Members();
                        //Mber2.Member_Group = "大俠";
                        //Mber2.Member_LineName = "Ahoy";
                        //Mber2.Member_GameName = "Marry Lin";
                        //MberList.Add(Mber2);

                        //Members Mber3 = new Members();
                        //Mber3.Member_Group = "月光家族";
                        //Mber3.Member_LineName = "Moona";
                        //Mber3.Member_GameName = "Moona★";
                        //MberList.Add(Mber3);

                        //從url讀json
                        
                        string json = DownloadJsonAsync(url);
                        List<Members> items = JsonConvert.DeserializeObject<List<Members>>(json);
                        foreach (var item in items)
                        {
                            string MemberString = string.Format("Group:{0} LineName:{1} GameName{2}"
                                                  , item.Member_Group, item.Member_LineName, item.Member_GameName);
                            if (Message != "")
                            {
                                Message = string.Format("{0}\n{1}", Message, MemberString);
                            }
                            else
                            {
                                Message = MemberString;
                            }
                        }
                        break;
                    case "!id":
                        Message = ReceivedMessage.events[0].replyToken;
                        break;
                    case "!member":
                        Message = ReceivedMessage.events[0].members.ToString();
                        break;
                    case "!msg":
                        Message = ReceivedMessage.events[0].message.ToString();
                        break;
                    case "!type":
                        Message = ReceivedMessage.events[0].type.ToString();
                        break;
                    case "!getpath":
                        Message = this.GetType().Assembly.Location.ToString().Trim();
                        if (Message.Trim() == "")
                        {
                            Message = "失敗";
                        }
                        break;
                    default:
                        break;
                }

                //回覆用戶
                isRock.LineBot.Utility.ReplyMessage(ReceivedMessage.events[0].replyToken, Message, MyLineChannelAccessToken);
                //回覆API OK
                return Ok();
            }
            catch (Exception ex)
            {
                isRock.LineBot.Utility.ReplyMessage(reqid, ex.ToString(), MyLineChannelAccessToken);
                return Ok();
            }
        }

        //讀URL JSON檔案
        private static string DownloadJsonAsync(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 30000;
            string result = "";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        //寫JSON檔案
        public async Task WriteJson(Members NewMember, string url)
        {
            //JSON寫入到檔案
            string json = DownloadJsonAsync(url);
            List<Members> items = JsonConvert.DeserializeObject<List<Members>>(json);
            items.Add(NewMember);

            //轉成JSON格式
            string ConberJson = JsonConvert.SerializeObject(items);
            //將文字寫入該檔案
            //WindowsAzure.Storage
            string str = System.Environment.CurrentDirectory;//C:\\Program Files (x86)\\IIS Express
            str = @"F:\MyProgram\Git\LineBot_testGroup\2020-11-28-1132\LineApi\LineApi\Models\MemberList.json";
            str = @"https://lineapistorage1128.blob.core.windows.net/memberlist/MemberList.json";
            
            //"F:\\MyProgram\\Git\\LineBot_testGroup\\2020-11-28-1132\\LineApi\\LineApi\\api\\"
            //File.WriteAllText(str, ConberJson);

            // Retrieve storage account from connection string.
            //[Storage的連接字串]
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=lineapistorage1128;AccountKey=gm+xIELINn32aDTH4UEmcp6ea7hl0NW4gWrqrSXpS1dw1S32E0e2NeOW3JxwzYISzSs4gkZTbfXjM2lx9nBfcw==;EndpointSuffix=core.windows.net");
            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve reference to a previously created container.
            //[容器名稱]
            CloudBlobContainer container = blobClient.GetContainerReference("memberlist");

            // Retrieve reference to a blob named "[要上傳的檔名]".
            //[要上傳的檔名]
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("MemberList.json");
            // Create or overwrite the "[要上傳的檔名]" blob with contents from a local file.
            //using (var fileStream = System.IO.File.OpenRead(str))
            //{
            //    blockBlob.UploadFromStream(fileStream);
            //}
        }

        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
