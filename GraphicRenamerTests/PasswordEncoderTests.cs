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
    public class PasswordEncoderTests
    {
        [TestMethod()]
        public void DecryptTest()
        {
            Assert.AreEqual("test", PasswordEncoder.Decrypt(PasswordEncoder.Encrypt("test")));
            Assert.AreEqual("", PasswordEncoder.Decrypt(""));
            Assert.AreEqual("", PasswordEncoder.Decrypt(null));
        }

        [TestMethod()]
        public void EncryptTest()
        {
            Assert.AreEqual("", PasswordEncoder.Encrypt(""));
            Assert.AreEqual("", PasswordEncoder.Encrypt(null));
        }
    }
}
