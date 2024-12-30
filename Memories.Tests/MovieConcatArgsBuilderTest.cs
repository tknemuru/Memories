using Memories.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Tests
{
    [TestClass]
    public class MovieConcatArgsBuilderTest
    {
        [TestMethod]
        public void 動画結合コマンドの引数が組み立てられる()
        {
            var actual = new MovieConcatArgsBuilder()
                .SetInputFileListTextPath("file_list.txt")
                .SetOutputFilePath("temp.MOV")
                .Build();

            Assert.AreEqual("-f concat -safe 0 -i \"file_list.txt\" -c copy \"temp.MOV\"", actual);
        }
    }
}
