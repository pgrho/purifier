using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.Purifier.Tests
{
    [TestClass]
    public class PurifierTest
    {
        #region Dot Convertion

        #region Dot Char

        [TestMethod]
        public void NounTest1()
            => Assert.AreEqual("みれぃぷり", Purifier.Purify("みれぃ"));

        [TestMethod]
        public void NounTest2()
            => Assert.AreEqual("みれぃぷり。", Purifier.Purify("みれぃ。"));

        [TestMethod]
        public void NounTest3()
            => Assert.AreEqual("みれぃっぷり！", Purifier.Purify("みれぃ！"));

        #endregion Dot Char

        #region Rule for ne

        [TestMethod]
        public void NeTest1()
            => Assert.AreEqual("ななみちゃんぷりね。", Purifier.Purify("ななみちゃんね。"));

        #endregion Rule for ne

        #region Rule for yo

        [TestMethod]
        public void YoTest1()
            => Assert.AreEqual("横暴ぷりよ!", Purifier.Purify("横暴よ!"));

        [TestMethod]
        public void YoTest2()
            => Assert.AreEqual("検事ぷりよ", Purifier.Purify("検事だよ"));

        [TestMethod]
        public void YoTest3()
            => Assert.AreEqual("問題ないぷりよ", Purifier.Purify("問題ないことよ"));

        #endregion Rule for yo

        #region Rule for removing

        [TestMethod]
        public void RemovingTest1()
            => Assert.AreEqual("解散ぷり", Purifier.Purify("解散です"));

        [TestMethod]
        public void RemovingTest2()
            => Assert.AreEqual("完璧ぷり", Purifier.Purify("完璧だ"));

        [TestMethod]
        public void RemovingTest3()
            => Assert.AreEqual("小鳥ちゃんぷり", Purifier.Purify("小鳥ちゃんだわ"));

        [TestMethod]
        public void RemovingTest4()
            => Assert.AreEqual("待っていたぷり", Purifier.Purify("待っていたわ"));

        [TestMethod]
        public void RemovingTest5()
            => Assert.AreEqual("怪盗ぷり", Purifier.Purify("怪盗さ"));

        [TestMethod]
        public void RemovingTest6()
            => Assert.AreEqual("良いぷり…", Purifier.Purify("良いな…"));

        [TestMethod]
        public void RemovingTest7()
            => Assert.AreEqual("美味しいぷり", Purifier.Purify("美味しいの"));

        #endregion Rule for removing

        #endregion Dot Convertion

        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine(Purifier.Purify("僕はみれぃ、我々はアイドルです。"));
        }
    }
}