using Memories.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Tests
{
    [TestClass]
    public class MovieMetadataArgsBuilderTest
    {
        [TestMethod]
        public void 動画メタデータ取得コマンドの引数が組み立てられる()
        {
            var actual = new MovieMetadataArgsBuilder()
                .SetInputFilePath("movie.MOV")
                .Build();

            Assert.AreEqual("-v quiet -print_format json -show_format -select_streams a:0 \"movie.MOV\"", actual);
        }
    }
}
