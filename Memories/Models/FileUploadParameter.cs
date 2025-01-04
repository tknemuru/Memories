using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Models
{
#pragma warning disable CS8618
    /// <summary>
    /// ファイルアップロードパラメータ
    /// </summary>
    public class FileUploadParameter
    {
        /// <summary>
        /// 認証情報ファイル
        /// </summary>
        public string CredentialFilePath { get; set; }

        /// <summary>
        /// ユーザ
        /// </summary>
        public string User {  get; set; }

        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// アップロード先のフォルダID
        /// </summary>
        public string UploadFolderId { get; set; }

        /// <summary>
        /// 共有するメールアドレスリスト
        /// </summary>
        public List<string> PermissionMailAdresses { get; set; } = new List<string>();
    }
}
