using Memories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Builders
{
    /// <summary>
    /// ファイルリストの構築機能を提供します。
    /// </summary>
    public class FileListBuilder : IBuildable
    {
        /// <summary>
        /// メタデータのリスト
        /// </summary>
        private IEnumerable<MovieFileMetadata> metadatas;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileListBuilder()
        {
            metadatas = new List<MovieFileMetadata>();
        }

        /// <summary>
        /// メタデータのリストをセットします。
        /// </summary>
        /// <param name="_metadatas">メタデータのリスト</param>
        /// <returns>ビルダー</returns>
        public FileListBuilder SetMetadatas(IEnumerable<MovieFileMetadata> _metadatas)
        {
            metadatas = _metadatas;
            return this;
        }

        /// <summary>
        /// ファイルリストの文字列を構築します。
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            var builder = new StringBuilder();
            foreach (var metadata in metadatas)
            {
                builder.AppendLine($"file '{metadata.FileName}'");
            }
            return builder.ToString();
        }
    }
}
