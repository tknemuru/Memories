using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Converters
{
#pragma warning disable CS8603, CS8765
    /// <summary>
    /// 文字列のリストの変換機能を提供します。
    /// </summary>
    public class StringEnumerableConverter : DefaultTypeConverter
    {
        /// <summary>
        /// 書き込み時に IEnumerable<string> を区切り文字列で結合したの文字列に変換します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="row">行</param>
        /// <param name="memberMapData">データ</param>
        /// <returns>区切り文字列で結合したの文字列</returns>
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value is IEnumerable<string> list)
            {
                return string.Join("|", list);
            }
            return base.ConvertToString(value, row, memberMapData);
        }

        /// <summary>
        /// 読み込み時に文字列を IEnumerable<string> に変換します。
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="row">行</param>
        /// <param name="memberMapData">データ</param>
        /// <returns>区切り文字列で結合したの文字列</returns>
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }
            return text.Split('|')
                .Select(s => s.Trim())
                .ToList();
        }
    }
}
