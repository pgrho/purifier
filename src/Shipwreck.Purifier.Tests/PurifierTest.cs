using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.Purifier.Tests
{
    [TestClass]
    public class PurifierTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine(Purifier.Purify("僕はみれぃ、我々はアイドルです。"));
        }
    }
}
