using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphicRenamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicRenamer.Tests
{
    [TestClass()]
    public class HeicToJpegTests
    {
        [TestMethod()]
        public void ConvertTest()
        {
            HeicToJpeg.Convert(@"C:\Users\jyo_s\Desktop\IMG_6852.heic", @"C:\Users\jyo_s\Desktop\IMG_6852.jpeg");
        }
    }
}