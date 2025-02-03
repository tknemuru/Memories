using Memories;
using Memories.Executors;
using Memories.Helplers;
using Memories.Models;

public class Program
{
#pragma warning disable CS8604, CS8602
    static void Main(string[] args)
    {
        try
        {
            AppConfig.Init();

            // 引数無しの場合は動画作成を実行する
            var mode = ExeMode.CreateMovie;

            if (args != null && args.Length > 0)
            {
                mode = (ExeMode)int.Parse(args[0]);
            }

            var creator = new MovieCreator();
            switch (mode)
            {
                case ExeMode.CreateMovie:
                    // 作成件数指定無しの場合は1件作成する
                    var count = 1;
                    if (args != null && args.Length > 1)
                    {
                        // 複数件作成したい場合は第二引数で件数を指定する
                        count = int.Parse(args[1]);
                    }

                    FileHelper.Log($"動画作成件数：{count}件");
                    for (var i = 0; i < count; i++)
                    {
                        FileHelper.Log($"{i + 1}件目の動画作成開始");
                        creator.Create();
                        FileHelper.Log($"{i + 1}件目の動画作成完了");
                    }
                    break;
                case ExeMode.SendNoticeMail:
                    if (args == null || args.Length < 2)
                    {
                        throw new ArgumentException("通知メール送信は必ず第二引数に送信先アドレスを指定してください。");
                    }
                    var fileUploader = new FileUploader();
                    new MailSender(fileUploader).Send(args[1]);
                    break;
                case ExeMode.CreateLongSpanMovie:
                    var param = new MovieCreatorParameter();
                    param.RangeCandidate = [int.Parse(args[1])];
                    param.StartMonth = args[2];
                    param.FileCount = int.Parse(args[3]);
                    param.MovieSeconds = int.Parse(args[4]);
                    param.AudioFilePath = args[5];
                    FileHelper.Log("長期間指定の動画作成開始");
                    creator.CreateLongSpanMovie(param);
                    FileHelper.Log($"長期間指定の動画作成完了");
                    break;
            }
        } catch (Exception ex)
        {
            FileHelper.Log($"エラーが発生しました。{ex}");
            throw;
        }
    }
}
