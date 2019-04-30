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
            //var a = Path.GetFileName(oldPath);//フルパスの最後のフォルダ名を取得

            //old内のフォルダ名を取得する
            //newPathにフォルダ作成
            //Directory.CreateDirectory();
            //oldPathからデータコピー

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
