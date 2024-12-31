using Memories.Filters;
using Memories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Tests.Filters
{
    [TestClass]
    public class FileCountFilterTest
    {
        [TestMethod]
        public void ファイル数による絞り込みができる()
        {
            var metas = new List<MovieFileMetadata>
            {
                new MovieFileMetadata(),
                new MovieFileMetadata(),
                new MovieFileMetadata(),
            };

            var filter = new FileCountFilter(2);
            var actual = filter.Filter(metas).ToList();
            Assert.AreEqual(2, actual.Count);
        }

        [TestMethod]
        public void 候補ファイル数を超える数を指定した場合は全件を対象にする()
        {
            var metas = new List<MovieFileMetadata>
            {
                new MovieFileMetadata(),
                new MovieFileMetadata(),
                new MovieFileMetadata(),
            };

            var filter = new FileCountFilter(99);
            var actual = filter.Filter(metas).ToList();
            Assert.AreEqual(3, actual.Count);
        }
    }
}
