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
    public class SearchTests
    {
        [TestMethod()]
        public void GetListOfSimplePtInfoTest()
        {
            string str = @"Patient ID:000001000
Patient Name:テスト　太郎
Patient ID:000001001
Patient Name:テスト　花子
";
            string str2 = "";

            List<Search.SimplePtInfo> tempList = FunctionsForSearch.GetListOfSimplePtInfo(str);
            List<Search.SimplePtInfo> tempList2 = FunctionsForSearch.GetListOfSimplePtInfo(str2);

            Assert.AreEqual(2, tempList.Count);
            Assert.AreEqual("000001001", tempList[1].ptId);
            Assert.AreEqual("テスト　花子", tempList[1].ptName);
            Assert.AreEqual(null, tempList2);
        }
    }
}