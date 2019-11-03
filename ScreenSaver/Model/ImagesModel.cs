using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScreenSaver.Model
{
    public class ImageModel
    {
        public int ID { set; get; }
        public string Name { set; get; }
        public string Url { set; get; }
        public DateTime TimeFrom { set; get; }
        public DateTime TimeTo { set; get; }
        public string Type { set; get; }

    }
}