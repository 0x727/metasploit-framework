using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tools;

namespace smbTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string[] args = new string[0];
            scanner.smb.main.Main(args);
        }

        [TestMethod]
        public void TestMethod2()
        {
            ThreadTool objMain = new Tools.ThreadTool();
            objMain.cracker("SMB", "127.0.0.1", 445, "administrator", "123456");
        }

        [TestMethod]
        public void TestMethod3()
        {
            ThreadTool objMain = new Tools.ThreadTool();
            objMain.cracker("SMB", "localhost", 445, "administrator", "123456");
        }
    }
}
