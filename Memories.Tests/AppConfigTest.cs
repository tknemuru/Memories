using Microsoft.Extensions.Configuration;

namespace Memories.Tests
{
    [TestClass]
    public class AppConfigTest
    {
        [TestMethod]
        public void 設定情報が正常に読み込める()
        {
            AppConfig.Init("app-config-template.json");
            var actual = AppConfig.Get().GetSection("tempDir").Get<string>();
            Assert.AreEqual(@"C:\temp", actual);
        }
    }
}