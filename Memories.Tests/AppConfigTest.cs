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
            var actual = AppConfig.Get().GetSection("tempDir").Get<string>();
            Assert.AreEqual(@"C:\temp", actual);
        }
    }
}