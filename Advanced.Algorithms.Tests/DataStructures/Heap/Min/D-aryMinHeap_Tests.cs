﻿using Advanced.Algorithms.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Advanced.Algorithms.Tests.DataStructures
{
    [TestClass]
    public class AsD_aryMinTree_Tests
    {
        /// <summary>
        /// A tree test
        /// </summary>
        [TestMethod]
        public void D_aryHeap_Test()
        {
            var rnd = new Random();
            var initial = Enumerable.Range(0, 51).OrderBy(x => rnd.Next()).ToList();

            //insert test
            var tree = new D_aryMinHeap<int>(4, initial);

            for (int i = 51; i <= 99; i++)
            {
                tree.Insert(i);
            }

            for (int i = 0; i <= 99; i++)
            {
                var min = tree.ExtractMin();
                Assert.AreEqual(min, i);
            }

            var testSeries = Enumerable.Range(1, 49).OrderBy(x => rnd.Next()).ToList();

            foreach (var item in testSeries)
            {
                tree.Insert(item);
            }

            for (int i = 1; i <= 49; i++)
            {
                var min = tree.ExtractMin();
                Assert.AreEqual(min, i);
            }

        }
    }
}
