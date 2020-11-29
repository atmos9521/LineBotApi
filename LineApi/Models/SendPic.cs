using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LineApi.Models
{
    public class SendPic
    {
        public string type
        {
            get
            {
                if (type == null)
                {
                    return "image";
                }
                else
                {
                    return type;
                }
            }
            set { type = value; }
        }
        public string originalContentUrl { get; set; }
        public string previewImageUrl { get; set; }
    }
}