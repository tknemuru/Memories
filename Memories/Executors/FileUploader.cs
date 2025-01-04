using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Memories.Helplers;
using Memories.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memories.Executors
{
#pragma warning disable CS8604, CS8601
    /// <summary>
    /// ファイルのアップロード機能を提供します。
    /// </summary>
    public class FileUploader
    {
        /// <summary>
        /// ファイルをアップロードします。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>アップロードしたファイルのURL</returns>
        public string Upload(string filePath)
        {
            var param = new FileUploadParameter()
            {
                CredentialFilePath = AppConfig.Get().GetValue<string>("fileUploadCredentials"),
                User = AppConfig.Get().GetValue<string>("fileUploadCredentials"),
                FilePath = filePath,
                UploadFolderId = AppConfig.Get().GetValue<string>("fileUploadFolderId"),
                PermissionMailAdresses = AppConfig.Get().GetSection("fileUploadPermissionMailAdresses").Get<List<string>>()
            };
            return Upload(param);
        }

        /// <summary>
        /// ファイルをアップロードします。
        /// </summary>
        /// <param name="param">パラメータ</param>
        /// <returns>アップロードしたファイルのURL</returns>
        public string Upload(FileUploadParameter param)
        {
            FileHelper.Log($"ファイルアップロード開始 {param.FilePath}");

            string[] scopes = [DriveService.Scope.DriveFile];
            string applicationName = "Memories";

            UserCredential credential;

            // 認証情報を読み込み
            using (var stream = new FileStream(param.CredentialFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    param.User,
                    CancellationToken.None).Result;
            }

            // Driveサービスを作成
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });

            // アップロードするファイルの情報
            var fileName = Path.GetFileName(param.FilePath);
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new[] { param.UploadFolderId }
            };

            // アップロード処理
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(param.FilePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "video/quicktime");
                request.Fields = "id";
                request.Upload();
            }

            // アップロードしたファイルのIDを取得
            var fileId = request.ResponseBody.Id;

            // 共有設定を2つのメールアドレスに設定
            var emailAddresses = param.PermissionMailAdresses;

            foreach (var email in emailAddresses)
            {
                var permission = new Google.Apis.Drive.v3.Data.Permission()
                {
                    Role = "writer",  // 書き込み権限
                    Type = "user",    // ユーザー指定
                    EmailAddress = email
                };
               var perRequest = service.Permissions.Create(permission, fileId);
                // 共有のお知らせメールを送らない
                perRequest.SendNotificationEmail = false;
                perRequest.Execute();
            }

            // アップロード完了の確認
            FileHelper.Log($"ファイルアップロード完了 {param.FilePath}");

            // アップロードしたファイルのURLを返却する
            return $"https://drive.google.com/file/d/{fileId}/view?usp=sharing";
        }
    }
}
