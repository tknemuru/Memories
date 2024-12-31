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
    public class CreationDateTimeRangeFilterTest
    {
        [TestMethod]
        public void 作成日時による絞り込みができる()
        {
            var metas = new List<MovieFileMetadata>
            {
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2023-10-31T23:59:59+0900")
                },
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2023-11-01T00:00:00+0900")
                },
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2024-1-31T23:59:59+0900")
                },
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2024-02-01T00:00:00+0900")
                },
            };
            var expected = new List<MovieFileMetadata>
            {
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2023-11-01T00:00:00+0900")
                },
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2024-1-31T23:59:59+0900")
                },
            };

            var filter = new CreationDateTimeRangeFilter([2], "2023-11");
            var actual = filter.Filter(metas).ToList();
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].CreationDateTime, actual[i].CreationDateTime);
            }
        }

        [TestMethod]
        public void 期間を超える候補を指定した場合は全期間を対象にする()
        {
            var metas = new List<MovieFileMetadata>
            {
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2023-10-31T23:59:59+0900")
                },
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2023-11-01T00:00:00+0900")
                },
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2024-1-31T23:59:59+0900")
                },
                new MovieFileMetadata
                {
                    CreationDateTime = DateTime.Parse("2024-02-01T00:00:00+0900")
                },
            };

            var filter = new CreationDateTimeRangeFilter([99], string.Empty);
            var actual = filter.Filter(metas).ToList();
            Assert.AreEqual(metas.Count, actual.Count);
            for (var i = 0; i < metas.Count; i++)
            {
                Assert.AreEqual(metas[i].CreationDateTime, actual[i].CreationDateTime);
            }
        }
    }
}
