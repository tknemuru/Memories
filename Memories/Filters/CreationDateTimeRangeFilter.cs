using Memories.Helplers;
using Memories.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Filters
{
#pragma warning disable CS8602
    /// <summary>
    /// 作成日時による期間指定の絞り込み機能を提供します。
    /// </summary>
    public class CreationDateTimeRangeFilter : IMovieFileFiltable
    {
        /// <summary>
        /// 期間（月）の候補リスト
        /// </summary>
        private int[] RangeCandidate {  get; set; }

        /// <summary>
        /// 開始月（yyyy-MM）
        /// </summary>
        private string StartMonth { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="rangeCandidate">期間（月）の候補リスト</param>
        /// <param name="startMonth">開始月（yyyy-MM）</param>
        public CreationDateTimeRangeFilter(int[] rangeCandidate, string startMonth)
        {
            RangeCandidate = rangeCandidate;
            StartMonth = startMonth;
        }

        /// <summary>
        /// 動画ファイルのリストをフィルタします。
        /// </summary>
        /// <param name="metadatas">動画ファイルのメタ情報リスト</param>
        /// <returns>フィルタした動画ファイルのメタ情報リスト</returns>
        public IEnumerable<MovieFileMetadata> Filter(IEnumerable<MovieFileMetadata> metadatas)
        {
            // 作成日時の最小値・最大値を取得
            var min = metadatas
                .Select(m => m.CreationDateTime)
                .Min();
            var max = metadatas
                .Select(m => m.CreationDateTime)
                .Max();
            FileHelper.Log($"作成日時の最小値: {min}");
            FileHelper.Log($"作成日時の最大値: {max}");

            // 期間（月）の候補を決める
            var random = new Random();
            var range = RangeCandidate[random.Next(RangeCandidate.Length)];
            FileHelper.Log($"期間（月）の候補: {range}");

            var yearMonths = metadatas
                .Select(m => m.CreationDateTime.ToString("yyyy-MM"))
                .Distinct()
                .OrderBy(m => m)
                .ToArray();
            var monthCount = yearMonths.Length;
            FileHelper.Log($"月の数: {monthCount}");

            // 開始月を決める
            var startMonth = yearMonths[random.Next(Math.Max(monthCount - range, 0))];
            FileHelper.Log($"開始月の決定（ランダム）: {startMonth}");
            // 開始月の指定がある場合は、指定の値で書き換える
            if (!string.IsNullOrEmpty(StartMonth))
            {
                startMonth = StartMonth;
                FileHelper.Log($"開始月の決定（指定した値で上書き）: {startMonth}");
            }

            // 期間を決める
            range = Math.Min(monthCount, range);
            FileHelper.Log($"期間（月）の決定: {range}");

            // 期間に従った最小値・最大値のDateTimeを生成する
            var endMotnth = DateTime.ParseExact(startMonth, "yyyy-MM", null)
                .AddMonths(range)
                .ToString("yyyy-MM");
            var startYms = startMonth.Split('-');
            var startDateTime = new DateTime(int.Parse(startYms[0]), int.Parse(startYms[1]), 1, 0, 0, 0, 0);
            var endYms = endMotnth.Split("-");
            var endDateTime = new DateTime(int.Parse(endYms[0]), int.Parse(endYms[1])
                , DateTime.DaysInMonth(int.Parse(endYms[0]), int.Parse(endYms[1])), 23, 59, 59, 999);
            FileHelper.Log($"作成日時の絞り込み最小値: {startDateTime}");
            FileHelper.Log($"作成日時の絞り込み最大値: {endDateTime}");

            // 対象を絞り込む
            var result = metadatas
                .Where(m => startDateTime <= m.CreationDateTime && m.CreationDateTime <= endDateTime);
            return result;
        }
    }
}
