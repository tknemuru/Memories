﻿using Memories.Builders;
using Memories.Executors;
using Memories.Extractors;
using Memories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Tests.Extractors
{
    [TestClass]
    public class LocalMovieFileExtractorTest
    {
        [TestMethod]
        public void メタ情報が抽出できる()
        {
            var builder = new MovieMetadataArgsBuilder();
            var procExecutor = new ProcessExecutor("ffprobe");
            string[] files = Directory.GetFiles(@"..\..\..\Resources\LocalMovieFileExtractor", "*.MOV", SearchOption.AllDirectories);
            var extractor = new LocalMovieFileExtractor(files, builder, procExecutor);
            var actual = extractor.Extract().ToList();
            var expected = new List<MovieFileMetadata>
            {
                new MovieFileMetadata()
                {
                    FileName = @"..\..\..\Resources\LocalMovieFileExtractor\001.MOV",
                    Duration = 21.796667,
                    CreationDateTime = DateTime.Parse("2019-12-14T19:24:30+0900"),
                },
                new MovieFileMetadata()
                {
                    FileName = @"..\..\..\Resources\LocalMovieFileExtractor\002.MOV",
                    Duration = 14.745,
                    CreationDateTime = DateTime.Parse("2018-12-08T18:06:46+0900"),
                },
            };
            Assert.AreEqual(expected.Count, actual.Count);
            for(var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].FileName, actual[i].FileName);
                Assert.AreEqual(expected[i].Duration, actual[i].Duration);
                Assert.AreEqual(expected[i].CreationDateTime, actual[i].CreationDateTime);
            }
        }
    }
}
