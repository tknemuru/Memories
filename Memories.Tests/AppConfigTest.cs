using Microsoft.Extensions.Configuration;

namespace Memories.Tests
{
    [TestClass]
    public class AppConfigTest
    {
        [TestMethod]
        public void �ݒ��񂪐���ɓǂݍ��߂�()
        {
            AppConfig.Init("app-config-template.json");
            var actual = AppConfig.Get().GetSection("ffmpegPath").Get<string>();
            Assert.AreEqual(@"C:\hoge\ffmpeg.exe", actual);
        }
    }
}