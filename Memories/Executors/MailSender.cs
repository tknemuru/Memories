using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Memories.Models;
using Memories.Helplers;
using Microsoft.Extensions.Configuration;
using Memories.Builders;

namespace Memories.Executors
{
#pragma warning disable CS8604, CS8601
    /// <summary>
    /// メール送信機能を提供します。
    /// </summary>
    public class MailSender
    {
        /// <summary>
        /// ファイルアップロード機能
        /// </summary>
        private FileUploader FileUploader { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileUploader">ファイルアップロード機能</param>
        public MailSender(FileUploader fileUploader)
        {
            FileUploader = fileUploader;
        }

        /// <summary>
        /// メールを送信します。
        /// </summary>
        /// <param name="toAddress">送信先アドレス</param>
        public void Send(string toAddress)
        {
            // 送信するファイルを決める
            var fileMetadata = ExtractSendFile(toAddress);
            var file = fileMetadata.FileName;
            if (string.IsNullOrEmpty(file))
            {
                FileHelper.Log("送信するファイルがなかったため、処理を終了します。");
                return;
            }

            // 未アップの場合は、ファイルをGoogleドライブにアップする
            var fileUrl = fileMetadata.SharedStorageFileUrl;
            if (string.IsNullOrEmpty(fileUrl))
            {
                fileUrl = FileUploader.Upload(file);
            } else
            {
                FileHelper.Log($"Googleドライブにアップロード済のため、URLを流用します。 {fileUrl}");
            }

            // 件名・本文を組み立てる
            var fileNameStrs = Path.GetFileNameWithoutExtension(file).Split('-');
            var start = fileNameStrs[1];
            var end = fileNameStrs[2];
            var tag = AppConfig.Get().GetValue<string>("noticeMailTag");
            var subject = new NoticeMailSubjectBuilder()
                .SetTag(tag)
                .SetStartDateTime(DateTime.ParseExact(start, "yyyyMM", null))
                .SetEndDateTime(DateTime.ParseExact(end, "yyyyMM", null))
                .Build();
            var body = new NoticeMailBodyBuilder()
                .SetTag(tag)
                .SetStartDateTime(DateTime.ParseExact(start, "yyyyMM", null))
                .SetEndDateTime(DateTime.ParseExact(end, "yyyyMM", null))
                .SetFileUrl(fileUrl)
                .Build();

            // パラメータを作成する
            var param = new MailSendParameter()
            {
                FromAddress = AppConfig.Get().GetValue<string>("noticeMailFromAddress"),
                ToAddress = toAddress,
                Subject = subject,
                Body = body,
                AppPassword = AppConfig.Get().GetValue<string>("noticeMailAppPassword"),
                FileName = file,
                SharedStorageFileUrl = fileUrl
            };

            // 送信実行
            Send(param);
        }

        /// <summary>
        /// メールを送信します。
        /// </summary>
        /// <param name="param">送信パラメータ</param>
        public void Send(MailSendParameter param)
        {
            // SmtpClient の設定
            var smtpClient = new SmtpClient(AppConfig.Get().GetValue<string>("noticeMailSmtpClient"))
            {
                Port = 587,  // TLS 用のポート
                Credentials = new NetworkCredential(param.FromAddress, param.AppPassword),
                EnableSsl = true, // SSL/TLS を有効にする
                Timeout = 600000
            };

            // メールメッセージの作成
            var mailMessage = new MailMessage(param.FromAddress, param.ToAddress, param.Subject, param.Body);

            // 添付ファイルを追加
            //foreach (var attachment in param.Attachments)
            //{
            //    mailMessage.Attachments.Add(attachment);
            //}

            // メール送信
            try
            {
                FileHelper.Log("メール送信開始");
                FileHelper.Log(param.ToString());
                smtpClient.Send(mailMessage);
                FileHelper.Log("メール送信完了");

                // 送信済に更新する
                UpdateSendFile(param);
            }
            catch (Exception ex)
            {
                FileHelper.Log($"メール送信に失敗しました。 {ex}");
                throw;
            }
        }

        /// <summary>
        /// 送信対象のファイルを決めて取得します。
        /// </summary>
        /// <param name="toAddress">送信先アドレス</param>
        /// <returns>送信対象のファイルパス</returns>
        private MovieFileMetadata ExtractSendFile(string toAddress)
        {
            var path = AppConfig.Get().GetValue<string>("resultMovieCsvFilePath");

            // 未送信の完成ファイルを取得
            var notSentFiles = CsvHelperWrapper.ReadRecords<MovieFileMetadata>(path)
                .Where(f => !f.HasSentAddresses.Contains(toAddress))
                .OrderBy(f => f.CreationDateTime)
                .ToList();

            if (notSentFiles.Count == 0)
            {
                return new MovieFileMetadata();
            }

            // 未送信ファイルの内、最も古いファイルを送信する
            return notSentFiles.First();
        }

        /// <summary>
        /// 送信済に更新します。
        /// </summary>
        /// <param name="param">送信パラメータ</param>
        private void UpdateSendFile(MailSendParameter param)
        {
            var path = AppConfig.Get().GetValue<string>("resultMovieCsvFilePath");

            var records = CsvHelperWrapper.ReadRecords<MovieFileMetadata>(path);
            var updRecords = records
                .Select(r =>
                {
                    if (r.FileName != param.FileName)
                    {
                        return r;
                    }

                    // 送信済に更新する
                    r.HasSentAddresses.Add(param.ToAddress);
                    // アップしたURLを再利用するために記録しておく
                    r.SharedStorageFileUrl = param.SharedStorageFileUrl;
                    return r;
                });

            CsvHelperWrapper.WriteRecords(updRecords, path);
        }
    }
}
