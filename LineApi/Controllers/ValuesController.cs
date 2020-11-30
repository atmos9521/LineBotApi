using LineApi.Models;
using LineApi.MyMethods;
using Microsoft.Ajax.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            //取用web.config string
            string MyLineChannelAccessToken = ConfigurationManager.AppSettings["LineChannelAccessToken"];
            var reqid = isRock.LineBot.Utility.Parsing(Request.Content.ReadAsStringAsync().Result).events[0].replyToken;
            string url = ConfigurationManager.AppSettings["lineapistorage1128_blob_Jsonfile"];
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
                    case "！指令":
                        Message = string.Format("{0}:{1}\n" +
                                                "{2}\n" + 
                                                "{3}"
                                                , "!time", "現在時間"
                                                , "!抽、!抽妹子"
                                                , "!抽帥哥"
                                                );
                        break;
                    case "!自我介紹":
                    case "!self":
                    case "！自我介紹":
                    case "！self":
                        Message = string.Format("{0}\n{1}", "我只是個只會抽妹子的沒用女僕......"
                            , "有意見請找willy.chen抱怨");
                        break;
                    case "!時間":
                    case "！時間":
                    case "!time":                    
                    case "！time":
                        Message = string.Format("現在時間:{0}  您說了:{1}", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), ReceivedMessage.events[0].message.text);
                        break;
                    case "!抽":
                    case "！抽":
                    case "!抽妹子":                    
                    case "！抽妹子":
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
                            //回傳圖片給user
                            isRock.LineBot.Utility.ReplyImageMessage(reqid, picURLs[Rindex], picURLs[Rindex], MyLineChannelAccessToken);
                        }
                        break;
                    case "!抽帥哥":
                    case "！抽帥哥":
                        Message = "我的把拔只愛妹子  沒帥哥照片可給(攤手";
                        break;
                    case "!list":

                        break;
                    case "!add":
                    case "！add":
                        Members NewMber = new Members();
                        NewMber.Member_Group = "初號機";
                        NewMber.Member_LineName = "Lusiya";
                        NewMber.Member_GameName = "◆Lusiya◆";

                        //JSON寫入到檔案
                        string json = JsonMethods.DownloadJsonAsync(url);
                        //轉成 Members 類別的物件
                        List<Members> items = JsonConvert.DeserializeObject<List<Members>>(json);
                        items.Add(NewMber);

                        //轉成JSON格式
                        string ConverJson = JsonConvert.SerializeObject(items);

                        //寫入Azuer Json檔案
                        JsonMethods.WriteToAzureBlob(ConverJson, url,
                                    ConfigurationManager.AppSettings["StorageConnectionString"],
                                    "memberlist",
                                    "MemberList.json"
                            );
                        //Message = JsonConvert.SerializeObject("成功");
                        Message = "成功";
                        return Json(Message);
                        break;
                    case "!readjson":
                    case "！readjson":
                        //從url讀json
                        string json_read = JsonMethods.DownloadJsonAsync(url);
                        List<Members> items_read = JsonConvert.DeserializeObject<List<Members>>(json_read);
                        foreach (var item in items_read)
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
                    case "！id":
                        Message = ReceivedMessage.events[0].replyToken;
                        break;
                    case "!member":
                    case "！member":
                        Message = ReceivedMessage.events[0].members.ToString();
                        break;
                    case "!msg":
                    case "！msg":
                        Message = ReceivedMessage.events[0].message.ToString();
                        break;
                    case "!type":
                    case "！type":
                        Message = ReceivedMessage.events[0].type.ToString();
                        break;
                    case "!pic":
                        //圖片輸出MemoryStream
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        Image_String("中文測試", 20, true, Color.FromArgb(255, 255, 255), Color.FromArgb(255, 255, 0)).Save(ms, ImageFormat.Png);
                        //Response.ClearContent();
                        //Response.ContentType = "image/png";
                        //Response.BinaryWrite(ms.ToArray());
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
