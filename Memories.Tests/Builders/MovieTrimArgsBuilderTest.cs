﻿using Memories.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Tests.Builders
{
    [TestClass]
    public class MovieTrimArgsBuilderTest
    {
        [TestMethod]
        public void 動画切り抜きコマンドの引数が組み立てられる()
        {
            var actual = new MovieTrimArgsBuilder()
                .SetInputFilePath("001.MOV")
                .SetStartSeconds(10)
                .SeTrimSeconds(3)
                .SetOutputFilePath("001temp.MOV")
                .Build();

            Assert.AreEqual("-i \"001.MOV\" -ss 10 -t 3 -vf \"scale=iw*min(1920/iw\\,1080/ih):ih*min(1920/iw\\,1080/ih),pad=1920:1080:(1920-iw*min(1920/iw\\,1080/ih))/2:(1080-ih*min(1920/iw\\,1080/ih))/2\" -c:v libx264 -c:a aac \"001temp.MOV\"", actual);
        }
    }
}
