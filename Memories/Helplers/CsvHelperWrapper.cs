using CsvHelper;
using CsvHelper.Configuration;
using Memories.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;

namespace Memories.Helplers
{
    /// <summary>
    /// CsvHelperをラップした補助機能を提供します。
    /// </summary>
    public static class CsvHelperWrapper
    {
        /// <summary>
        /// レコードを読み込みます。
        /// </summary>
        /// <typeparam name="T">レコードの型</typeparam>
        /// <param name="path">ファイルパス</param>
        /// <returns>レコードのリスト</returns>
        public static List<T> ReadRecords<T>(string path)
        {
            if (!File.Exists(path))
            {
                // ファイルが存在しない場合は空リストを返却する
                return new List<T>();
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };

            List<T> records;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, config))
            {
                records = csv.GetRecords<T>().ToList();
            }
            return records;
        }

        /// <summary>
        /// レコードを書き込みます。
        /// </summary>
        /// <typeparam name="T">レコードの型</typeparam>
        /// <param name="records">レコードリスト</param>
        /// <param name="path">ファイルパス</param>
        public static void WriteRecords<T>(IEnumerable<T> records, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }

        /// <summary>
        /// レコードを追記します。
        /// </summary>
        /// <typeparam name="T">レコードの型</typeparam>
        /// <param name="records">レコードリスト</param>
        /// <param name="path">ファイルパス</param>
        public static void WriteRecordsAppend<T>(IEnumerable<T> records, string path)
        {
            IEnumerable<T> result = new List<T>();
            if (File.Exists(path))
            {
                result = ReadRecords<T>(path);
            }

            result = result.Union(records);
           WriteRecords(result, path);
        }

        /// <summary>
        /// レコードを1行追記します。
        /// </summary>
        /// <typeparam name="T">レコードの型</typeparam>
        /// <param name="record">レコード</param>
        /// <param name="path">ファイルパス</param>
        public static void WriteRecordAppend<T>(T record, string path)
        {
            var result = new List<T>();
            if (File.Exists(path))
            {
                result = ReadRecords<T>(path);
            }

            result.Add(record);
            WriteRecords(result, path);
        }
    }
}
