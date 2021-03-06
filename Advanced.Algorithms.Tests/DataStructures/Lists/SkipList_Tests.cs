﻿using Advanced.Algorithms.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Advanced.Algorithms.Tests.DataStructures
{
    [TestClass]
    public class SkipList_Tests
    {
        /// <summary>
        /// A skip list test
        /// </summary>
        [TestMethod]
        public void SkipList_Test()
        {
            var skipList = new SkipList<int>();

            for (int i = 1; i < 100; i++)
            {
                skipList.Insert(i);
            }

            for (int i = 1; i < 100; i++)
            {
                Assert.AreEqual(i, skipList.Find(i));
            }

            Assert.AreEqual(0, skipList.Find(101));


            for (int i = 1; i < 100; i++)
            {
                skipList.Delete(i);
                Assert.AreEqual(0, skipList.Find(i));
            }

            for (int i = 1; i < 50; i++)
            {
                skipList.Insert(i);
            }

            for (int i = 1; i < 50; i++)
            {
                Assert.AreEqual(i, skipList.Find(i));
            }

            for (int i = 1; i < 50; i++)
            {
                skipList.Delete(i);
                Assert.AreEqual(0, skipList.Find(i));
            }
        }
    }
}
