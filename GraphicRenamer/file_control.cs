using System;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GraphicRenamer
{
    public class file_control
    {
        public static void delFile(string fileName)
        {
            if (File.Exists(fileName) == true)
            {
                try
                { File.Delete(fileName); }
                catch (IOException)
                { MessageBox.Show(Properties.Resources.FileBeingUsed, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                catch (UnauthorizedAccessException)
                { MessageBox.Show(Properties.Resources.PermissionDenied, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else
            { MessageBox.Show(Properties.Resources.FileNotExist, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public static string readFromFile(string fileName)
        {
            string text = "";
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                { text = sr.ReadToEnd(); }
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return text;
        }

        public static string readItemSettingFromText(string text, string itemName)
        {
            int index;
            index = text.IndexOf(itemName);
            if (index == -1)
            {
                MessageBox.Show("[settings.config]" + "\r\n" + itemName + "\r\n"
                    + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            else
            { return getUntilNewLine(text, index + itemName.Length); }
        }

        public static string getUntilNewLine(string text, int strPoint)
        {
            string ret = "";
            for (int i = strPoint; i < text.Length; i++)
            {
                if ((text[i].ToString() != "\r") && (text[i].ToString() != "\n"))
                { ret += text[i].ToString(); }
                else
                { break; }
            }

            for (int i = 0; i < ret.Length; i++)
            {
                if (ret.Substring(0, 1) == "\t" || ret.Substring(0, 1) == " " || ret.Substring(0, 1) == "　")
                { ret = ret.Substring(1); }
                else
                { break; }
            }

            for (int i = 0; i < ret.Length; i++)
            {
                if (ret.Substring(ret.Length - 1) == "\t" || ret.Substring(ret.Length - 1) == " " || ret.Substring(ret.Length - 1) == "　")
                { ret = ret.Substring(0, ret.Length - 1); }
                else
                { break; }
            }

            return ret;
        }

        /// <summary>
        /// IDの桁数と数字かどうかのチェック
        /// 12桁以上
        /// </summary>
        /// <param name="_id">チェック対象のID</param>
        /// <returns>有効なIDだったらtrue、無効なIDだったらfalse</returns>
        public static bool CheckID(string _id)
        {
            //数字かどうかのチェック
            var isNum = _id.All(char.IsDigit);
            if (isNum == false)
            {
                return false;
            }
            //長さのチェック
            var idLength = _id.Length;
            if (idLength > 12)
            {
                return false;
            }
            return true;
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
                return @"\"+string.Join(@"\", pathList);
            }
        }
    }
}
