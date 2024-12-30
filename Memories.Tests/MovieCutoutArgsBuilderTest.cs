using Memories.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Tests
{
    [TestClass]
    public class MovieCutoutArgsBuilderTest
    {
        [TestMethod]
        public void 動画切り抜きコマンドの引数が組み立てられる()
        {
            var actual = new MovieCutoutArgsBuilder()
                .SetInputFilePath("001.MOV")
                .SetStartSeconds(10)
                .SetIntervalSecondss(3)
                .SetOutputFilePath("001temp.MOV")
                .Build();

            Assert.AreEqual("-i \"001.MOV\" -ss 10 -t 3 -c copy \"001temp.MOV\"", actual);
        }
    }
}
