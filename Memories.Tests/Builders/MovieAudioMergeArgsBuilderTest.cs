using Memories.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Tests.Builders
{
    [TestClass]
    public class MovieAudioMergeArgsBuilderTest
    {
        [TestMethod]
        public void 動画音声合成コマンドの引数が組み立てられる()
        {
            var actual = new MovieAudioMergeArgsBuilder()
                .SetInputMovieFilePath("movie.MOV")
                .SetInputAudioFilePath("audio.mp3")
                .SetOutputFilePath("temp.MOV")
                .Build();

            Assert.AreEqual("-i \"movie.MOV\" -i \"audio.mp3\" -filter_complex \"[0:a]volume=4.0[a1];[1:a]volume=0.4[a2];[a1][a2]amix=inputs=2:duration=shortest\" -c:v copy -c:a aac -b:a 192k \"temp.MOV\"", actual);
        }
    }
}
