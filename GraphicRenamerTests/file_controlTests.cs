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
    public class file_controlTests
    {
        [TestMethod()]
        public void CheckIDTest()
        {
            Assert.AreEqual(true, file_control.CheckID("1234"));
            Assert.AreEqual(true, file_control.CheckID("123456789012"));
            Assert.AreEqual(false, file_control.CheckID("1234567890123"));
            Assert.AreEqual(false, file_control.CheckID("hogehoge"));
            Assert.AreEqual(false, file_control.CheckID("123-456"));
        }

        [TestMethod()]
        public void MakeDirPathTest()
        {
            Assert.AreEqual(@"\9\9\8765\4321", file_control.MakeDirPath("987654321"));
            Assert.AreEqual(@"\8\8765\4321", file_control.MakeDirPath("87654321"));
            Assert.AreEqual(@"\6\65\4321", file_control.MakeDirPath("654321"));
            Assert.AreEqual(@"\5\5\4321", file_control.MakeDirPath("54321"));
            Assert.AreEqual(@"\3\321", file_control.MakeDirPath("321"));
            Assert.AreEqual(@"\1\3", file_control.MakeDirPath("3"));
        }
    }
}