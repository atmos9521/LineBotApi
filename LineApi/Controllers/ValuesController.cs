using LineApi.Models;
using LineApi.MyMethods;
using Microsoft.Ajax.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
                    case "!指令":
                        Message = string.Format("{0}:{1}\n" +
                                                "{2}\n" + 
                                                "{3}"
                                                , "!time", "現在時間"
                                                , "!抽妹子"
                                                , "!抽帥哥"
                                                );
                        break;
                    case "!自我介紹":
                    case "!self":
                        Message = string.Format("{0}\n{1}", "我只是個只會抽妹子的沒用女僕......"
                            , "有意建請找willy.chen抱怨");
                        break;
                    case "!時間":
                    case "!time":
                        Message = string.Format("現在時間:{0}  您說了:{1}", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), ReceivedMessage.events[0].message.text);
                        break;
                    case "!list":

                        break;
                    case "!add":
                        Members NewMber = new Members();
                        NewMber.Member_Group = "初號機";
                        NewMber.Member_LineName = "Lusiya";
                        NewMber.Member_GameName = "◆Lusiya◆";
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
                    case "!pic":
                        //圖片輸出MemoryStream
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        Image_String("中文測試", 20, true, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 0)).Save(ms, ImageFormat.Png);
                        //Response.ClearContent();
                        //Response.ContentType = "image/png";
                        //Response.BinaryWrite(ms.ToArray());
                        break;
                    case "!pic1":
                        string remu = "https://lineapistorage1128.blob.core.windows.net/pic/97196001_1080863122300665_3324166057042984711_n.jpg";
                        isRock.LineBot.Utility.ReplyImageMessage(reqid, remu, remu, MyLineChannelAccessToken);
                        break;
                    case "!pic2":
                        string nezuco = "https://lineapistorage1128.blob.core.windows.net/pic/彌豆子.PNG";
                        isRock.LineBot.Utility.ReplyImageMessage(reqid, nezuco, nezuco, MyLineChannelAccessToken);
                        break;
                    case "!addpic":
                    case "!抽妹子":
                        string[] picURLs = Method.picURLs();
                        if (picURLs.Count() > 0)
                        {
                            Random RandomIndex = new Random();
                            int Rindex = RandomIndex.Next(1, picURLs.Count());
                            int Year = DateTime.Today.Year;
                            int Month = DateTime.Today.Month;
                            int Day = DateTime.Today.Day;
                            Rindex = Rindex * Year * Day / Month;
                            Rindex = Rindex % picURLs.Count();
                            isRock.LineBot.Utility.ReplyImageMessage(reqid, picURLs[Rindex], picURLs[Rindex], MyLineChannelAccessToken);
                        }
                        break;
                    case "!抽帥哥":
                        Message = "我的把拔只愛妹子  沒帥哥照片可給(攤手";
                        break;
                    default:
                        break;
                }

                //回覆用戶
                isRock.LineBot.Utility.ReplyMessage(reqid, Message, MyLineChannelAccessToken);
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
        public void WriteJson(Members NewMember, string url)
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
            str = @"https://lineapistorage1128.blob.core.windows.net/memberlist/MemberList.json";
            str = @"https://lineapistorage1128.file.core.windows.net/jsonfileshare/MemberList.json";
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
            blockBlob.UploadText(ConberJson);
            // Create or overwrite the "[要上傳的檔名]" blob with contents from a local file.
            //using (var fileStream = System.IO.File.OpenRead(str))
            //{
            //    blockBlob.UploadFromStream(fileStream);
            //}
        }

        /*產生圖檔*/
        protected Bitmap Image_String(string font, int font_size, bool font_bold, Color bgcolor, Color color)
        {
            Font f = new System.Drawing.Font("微軟正黑體", font_size, font_bold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular); //文字字型
            Brush b = new System.Drawing.SolidBrush(color); //文字顏色

            //計算文字長寬
            int img_width = 0, img_height = 0;
            using (Graphics gr = Graphics.FromImage(new Bitmap(1, 1)))
            {
                SizeF size = gr.MeasureString(font, f);
                img_width = Convert.ToInt32(size.Width);
                img_height = Convert.ToInt32(size.Height);
                gr.Dispose();
            }

            //圖片產生
            Bitmap image = new Bitmap(img_width, img_height);

            //填滿顏色並透明
            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(bgcolor);
                image = Image_ChangeOpacity(image, 0.5f);
                g.Dispose();
            }

            //文字寫入
            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawString(font, f, b, 0, 0);
                g.Dispose();
            }

            return image;
        }
        /*產生圖檔*/
        protected Bitmap Image_ChangeOpacity(Image img, float opacityvalue)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();
            return bmp;
        }

        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }
        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            ImageFormat format = image.RawFormat;
            if (format.Equals(ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                file += ".icon";
            }
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
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
