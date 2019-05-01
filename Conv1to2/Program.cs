using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Conv1to2
{
    class Program
    {
        private static int totalCount = 0;
        private static int nowCount = 0;

        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Option>(args);
            string oldPath = "";
            string newPath = "";
            if (result.Tag == ParserResultType.Parsed)
            {
                var parsed = (Parsed<Option>)result;
                oldPath = parsed.Value.OldPath;
                newPath = parsed.Value.NewPath;
            }

            if(String.IsNullOrWhiteSpace(oldPath)==false && String.IsNullOrWhiteSpace(newPath) == false)
            {
                //newPathにフォルダ作成
                if (Directory.Exists(newPath) == false)
                {
                    Directory.CreateDirectory(newPath);
                }
                var oldSubDirPaths = Directory.GetDirectories(oldPath);//old内のフォルダ名を取得する
                totalCount = Directory.GetFiles(oldPath, "*", SearchOption.AllDirectories).Count();//全ファイル数

                foreach (var oldSubPath in oldSubDirPaths)
                {
                    var dirName = Path.GetFileName(oldSubPath);//フルパスの最後のフォルダ名を取得
                    var newDirPath = newPath + MakeDirPath(dirName);
                    DirectoryCopy(oldSubPath, newDirPath);//oldPathからnewDirPathにデータコピー
                }
            }
        }


        public static void OutputLog(string _logText)
        {
            using (var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "log.txt", true))
            {
                var datetime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                sw.WriteLine(datetime+", " + _logText);
                sw.Close();
            }
        }

        public static void DirectoryCopy(string sourcePath, string destinationPath)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);
            DirectoryInfo destinationDirectory = new DirectoryInfo(destinationPath);

            //コピー先のディレクトリがなければ作成する
            if (destinationDirectory.Exists == false)
            {
                destinationDirectory.Create();
                destinationDirectory.Attributes = sourceDirectory.Attributes;
            }

            //ファイルのコピー
            foreach (FileInfo fileInfo in sourceDirectory.GetFiles())
            {
                //同じファイルが存在していたら、上書きしない
                try
                {
                    nowCount++;
                    WirteProgress();
                    fileInfo.CopyTo(destinationDirectory.FullName + @"\" + fileInfo.Name, false);

                }
                catch (IOException)
                {
                    OutputLog("コピー先に既にファイルが存在しています。："+fileInfo.FullName);
                }
            }

            //ディレクトリのコピー（再帰を使用）
            foreach (DirectoryInfo directoryInfo in sourceDirectory.GetDirectories())
            {
                DirectoryCopy(directoryInfo.FullName, destinationDirectory.FullName + @"\" + directoryInfo.Name);
            }
        }

        private static void WirteProgress()
        {
            // 進捗を表示
            Console.Write("コピー進捗：" + nowCount + " / " + totalCount);

            // カーソル位置を初期化
            Console.SetCursorPosition(0, Console.CursorTop);

            // （進行が見えるように）処理を100ミリ秒間休止
            //System.Threading.Thread.Sleep(100);
        }
        /// <summary>
        /// 桁数÷4の余りの数だけIDの先頭から数字を取ってきて、その名前のフォルダを利用する（なければ作る）（例: 54321 だったら 5 ）
        /// その下にその次の4桁の番号の名前のフォルダを利用する（なければ作る）（例: 54321 だったら 4321 ）
        /// </summary>
        /// <param name="_id">対象のID</param>
        /// <returns></returns>
        public static string MakeDirPath(string _id)
        {
            var digits = _id.Length;
            if (digits < 4)
            {
                var ret = @"\" + digits.ToString() + @"\" + _id;
                return ret;
            }
            else
            {
                List<string> pathList = new List<string>();
                var temp = digits % 4;//余り
                pathList.Add(digits.ToString());//桁数
                if (temp != 0)
                {
                    pathList.Add(_id.Substring(0, temp));//余りの数だけ文字取得
                }
                pathList.AddRange(Regex.Split(_id.Substring(temp), @"(?<=\G.{4})(?!$)"));//4文字区切りでListにいれる
                return @"\" + string.Join(@"\", pathList);
            }
        }
    }
}
