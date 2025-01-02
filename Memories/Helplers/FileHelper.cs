using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Memories.Helplers
{
    /// <summary>
    /// ファイル操作に関する補助機能を提供します。
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// エンコードの初期値
        /// </summary>
        private static Encoding DefaultEncoding = new UTF8Encoding();

        /// <summary>
        /// ファイルパスの初期値
        /// </summary>
        private static readonly string DefaultFilePath;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static FileHelper()
        {
            DefaultFilePath = string.Format(@"./log/{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        /// <summary>
        /// <para>文字列をファイルに出力する</para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void Log(string str)
        {
            Console.WriteLine(str);
            WriteLine(str, DefaultFilePath);
        }

        /// <summary>
        /// <para>ファイルから文字列のリストを取得します。</para>
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="encoding">エンコード</param>
        /// <returns>文字列のリスト</returns>
        public static IEnumerable<string> ReadTextLines(string filePath, Encoding? encoding = null)
        {
            if (encoding == null) { encoding = DefaultEncoding; }

            string line;
            using (StreamReader sr = new StreamReader(filePath, encoding))
            {
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
            }
        }

        /// <summary>
        /// jsonファイルを読み込みます。
        /// </summary>
        /// <typeparam name="T">jsonに対応した型</typeparam>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="encoding">エンコード</param>
        /// <returns>jsonファイルから作成したオブジェクト</returns>
        public static T? ReadJson<T>(string filePath, Encoding? encoding = null)
        {
            if (encoding == null) { encoding = DefaultEncoding; }

            using (var sr = new StreamReader(filePath, encoding))
            {
                return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// jsonファイルを読み込みます。
        /// </summary>
        /// <typeparam name="T">jsonに対応した型</typeparam>
        /// <param name="fileByte">ファイルのバイト文字列</param>
        /// <param name="encoding">エンコード</param>
        /// <returns>jsonファイルから作成したオブジェクト</returns>
        public static T? ReadJson<T>(byte[] fileByte, Encoding? encoding = null)
        {
            if (encoding == null) { encoding = DefaultEncoding; }
            return JsonConvert.DeserializeObject<T>(encoding.GetString(fileByte));
        }

        /// <summary>
        /// オブジェクトをjson文字列に変換してファイルに書き込みます。
        /// </summary>
        /// <param name="obj">書き込み対応のオブジェクト</param>
        /// <param name="filePath">書き込むファイルパス</param>
        /// <param name="formatting">フォーマットオプション</param>
        /// <param name="encoding">エンコード</param>
        public static void WriteJson(object obj, string filePath, Formatting formatting, Encoding? encoding = null)
        {
            if (encoding == null) { encoding = DefaultEncoding; }
            CreateDirectory(GetFileDirectory(filePath));

            var json = JsonConvert.SerializeObject(obj, formatting);
            using (var sr = new StreamWriter(filePath, false, encoding))
            {
                sr.Write(json);
            }
        }

        /// <summary>
        /// <para>文字列に改行コードを付与して出力します。</para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void WriteLine(string line, string filePath, bool append = true)
        {
            CreateDirectory(GetFileDirectory(filePath));

            using (StreamWriter sr = new StreamWriter(filePath, append, DefaultEncoding))
            {
                sr.WriteLine(line);
            }
        }

        /// <summary>
        /// <para>文字列をファイルに出力する</para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void Write(string str, string filePath, bool append = true)
        {
            CreateDirectory(GetFileDirectory(filePath));

            using (StreamWriter sr = new StreamWriter(filePath, append, DefaultEncoding))
            {
                sr.Write(str);
            }
        }

        /// <summary>
        /// ディレクトリを作成します。
        /// </summary>
        /// <param name="path">ファイルパス</param>
        public static void CreateDirectory(string path)
        {
            if (File.Exists(path)) { return; }
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// ファイル名を含めたフルパスからファイル名を除いたパスを返します。
        /// </summary>
        /// <param name="fullPath">ファイル名を含めたフルパス</param>
        /// <returns>ファイル名を除いたパス</returns>
        public static string GetFileDirectory(string fullPath, char delimiter = '/')
        {
            List<string> directorys = fullPath.Replace('\\', delimiter).Split(delimiter).ToList();
            if (directorys.Count == 1)
            {
                return fullPath;
            }

            string retPath = "";
            for (int i = 0; i < (directorys.Count - 1); i++)
            {
                retPath += directorys[i] + delimiter;
            }

            return retPath.Substring(0, (retPath.Length - 1));
        }

        /// <summary>
        /// 指定したフォルダ内の全ファイルを再帰的に削除します。
        /// </summary>
        /// <param name="folderPath">削除対象のフォルダパス</param>
        public static void DeleteAllFiles(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }

            foreach (string file in Directory.GetFiles(folderPath))
            {
                File.Delete(file);
            }

            foreach (string subFolder in Directory.GetDirectories(folderPath))
            {
                Directory.Delete(subFolder, true);
            }
        }

        /// <summary>
        /// 指定したフォルダ内の全ファイルを再帰的に移動します。
        /// </summary>
        /// <param name="sourceFolderPath">移動元のフォルダパス</param>
        /// <param name="destFolderPath">移動先のフォルダパス</param>
        public static void MoveAllFiles(string sourceFolderPath, string destFolderPath)
        {
            // 移動先のフォルダが存在しない場合は作成
            if (!Directory.Exists(destFolderPath))
            {
                Directory.CreateDirectory(destFolderPath);
            }

            // 指定したフォルダ内のすべてのファイルを取得
            string[] files = Directory.GetFiles(sourceFolderPath, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // ファイル名を取得
                string fileName = Path.GetFileName(file);

                // 移動先のフルパスを作成
                string destFilePath = Path.Combine(destFolderPath, fileName);

                // ファイルを移動
                File.Move(file, destFilePath);
            }
        }
    }
}
