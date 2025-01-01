using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Memories
{
#pragma warning disable CS8618
    /// <summary>
    /// アプリケーションの設定情報を保持します。
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// 設定情報
        /// </summary>
        private static IConfigurationRoot Config { get; set; }

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        public static void Init()
        {
            Init("app-config.json");
            Config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("app-config.json", optional: false, reloadOnChange: true)
                .Build();
        }

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        /// <param name="jsonPath">設定情報を定義したjsonファイル</param>
        public static void Init(string jsonPath)
        {
            Config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(jsonPath, optional: false, reloadOnChange: true)
                .Build();
        }

        /// <summary>
        /// 設定情報を取得します。
        /// </summary>
        /// <returns></returns>
        public static IConfigurationRoot Get()
        {
            if (Config == null)
            {
                throw new InvalidOperationException("must call Init.");
            }
            return Config;
        }
    }
}
