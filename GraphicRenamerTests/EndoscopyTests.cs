using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicRenamer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace GraphicRenamer.Tests
{
    [TestClass()]
    public class EndoscopyTests
    {
        [TestMethod()]
        public void getValueOfTagTest()
        {
            Assert.AreEqual("test taro", Endoscopy.getValueOfTag("abcdefg\r\n<name>test taro</name>abcdefg", "name"));
            Assert.AreEqual("test taro", Endoscopy.getValueOfTag("abcdefg<name>test taro</name>abcdefg", "name"));
            Assert.AreEqual("test taro", Endoscopy.getValueOfTag("<name>test taro</name>", "name"));
            Assert.AreEqual("テスト ﾔｽﾀﾛｳ", Endoscopy.getValueOfTag("8</idno>\r\n<name>テスト ﾔｽﾀﾛｳ</name>\r\n<sex>M</sex>", "name"));
            //Assert.AreEqual("", Endoscopy.getValueOfTag(@"abcdefg\r\n<name>test taro</name>abcdefg", "test")); //MessageBox will open when undergo this test.
        }

        [TestMethod()]
        public void isTimeEmptyTest()
        {
            Assert.IsFalse(Endoscopy.isTimeEmpty("12:00:00"));
            Assert.IsTrue(Endoscopy.isTimeEmpty("  :  :  "));
            Assert.IsTrue(Endoscopy.isTimeEmpty("      "));
            Assert.IsTrue(Endoscopy.isTimeEmpty(""));
        }
    }
}
