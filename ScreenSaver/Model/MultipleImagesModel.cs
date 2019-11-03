using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScreenSaver.Model
{
    public class MultipleImagesModel
    {
        public ImageModel images { get; set; }
        public FileModel files { get; set; }
        public int dayInterval { get; set; }
    }
}