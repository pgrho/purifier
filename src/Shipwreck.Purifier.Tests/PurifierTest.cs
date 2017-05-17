using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.Purifier.Tests
{
    [TestClass]
    public class PurifierTest
    {
        [TestMethod]
        public void NounTest1()
            => Assert.AreEqual("みれぃぷり", Purifier.Purify("みれぃ"));

        [TestMethod]
        public void NounTest2()
            => Assert.AreEqual("みれぃぷり。", Purifier.Purify("みれぃ。"));

        [TestMethod]
        public void NounTest3()
            => Assert.AreEqual("みれぃっぷり！", Purifier.Purify("みれぃ！"));

        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine(Purifier.Purify("僕はみれぃ、我々はアイドルです。"));
        }
    }
}
