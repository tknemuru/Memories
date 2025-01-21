using Memories.Helplers;
using Memories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Filters
{
    /// <summary>
    /// 撮影日・期間・選択するファイル数による均等選択機能を提供します。
    /// </summary>
    public class DateTimeEvenlyFilter : IMovieFileFiltable
    {
        /// <summary>
        /// 選択するファイル数
        /// </summary>
        private int SelectCount { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="selectCount">選択するファイル数</param>
        public DateTimeEvenlyFilter(int selectCount)
        {
            SelectCount = selectCount;
        }

        /// <summary>
        /// ファイル数による期間指定の絞り込み機能を提供します。
        /// </summary>
        /// <param name="metadatas">動画ファイルのメタ情報リスト</param>
        /// <returns>フィルタした動画ファイルのメタ情報リスト</returns>
        public IEnumerable<MovieFileMetadata> Filter(IEnumerable<MovieFileMetadata> metadatas)
        {
            var count = metadatas.Count();
            if (count <= SelectCount)
            {
                FileHelper.Log($"候補のファイル数より、選択するファイル数の方が多いため全件を対象にします。 候補のファイル数: {count} 選択するファイル数: {SelectCount}");
                return metadatas;
            }

            // 対象月を洗い出す
            var months = metadatas
                .Select(m => m.CreationDateTime.ToString("yyyyMM"))
                .Distinct()
                .Order()
                .ToList();
            var monthsCount = months.Count();
            var unit = SelectCount / monthsCount;
            // 余りを加えたファイル数
            var unitSurplus = unit + (SelectCount % monthsCount);
            FileHelper.Log($"対象月数: {monthsCount} ひと月あたりのファイル数: {unit} 最終月のファイル数: {unitSurplus} 対象月： {string.Join(", ", months)}");

            // 月ごとにファイルを決めていく
            var result = new List<MovieFileMetadata>();
            var random = new Random();
            for (var i = 0; i < monthsCount; i++)
            {
                var selectCount = i == monthsCount - 1 ? unitSurplus : unit;
                var targetMonth = months[i];
                var monthResult = metadatas
                    .Where(m => m.CreationDateTime.ToString("yyyyMM") == targetMonth)
                    .Select(m => new { Order = random.Next(count), Metadata = m })
                    .OrderBy(m => m.Order)
                    .Take(selectCount)
                    .Select(m => m.Metadata)
                    .ToList();
                result = result
                    .Union(monthResult)
                    .ToList();
            }
            result = result
                .OrderBy(r => r.CreationDateTime)
                .ToList();

            foreach (var r in result)
            {
                FileHelper.Log($"ファイル名: {r.FileName} 撮影日: {r.CreationDateTime}");
            }

            return result;
        }
    }
}
