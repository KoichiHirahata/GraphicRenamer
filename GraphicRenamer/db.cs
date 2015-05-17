using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Npgsql;
using System.Windows.Forms;

namespace GraphicRenamer
{
    class db
    {
        #region password
        public class PasswordEncoder
        {
            private PasswordEncoder() { }

            // 128bit(16byte)のIV（初期ベクタ）とKey（暗号キー）
            private const string AesIV = @"&%jqiIurtmslLE58";
            private const string AesKey = @"3uJi<9!$kM0lkxme";

            /// 文字列をAESで暗号化
            public static string Encrypt(string text)
            {
                // AES暗号化サービスプロバイダ
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 128;
                aes.IV = Encoding.UTF8.GetBytes(AesIV);
                aes.Key = Encoding.UTF8.GetBytes(AesKey);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // 文字列をバイト型配列に変換
                byte[] src = Encoding.Unicode.GetBytes(text);

                // 暗号化する
                using (ICryptoTransform encrypt = aes.CreateEncryptor())
                {
                    byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                    // バイト型配列からBase64形式の文字列に変換
                    return Convert.ToBase64String(dest);
                }
            }

            /// 文字列をAESで復号化
            public static string Decrypt(string text)
            {
                // AES暗号化サービスプロバイダ
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 128;
                aes.IV = Encoding.UTF8.GetBytes(AesIV);
                aes.Key = Encoding.UTF8.GetBytes(AesKey);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Base64形式の文字列からバイト型配列に変換
                byte[] src = System.Convert.FromBase64String(text);

                // 複号化する
                using (ICryptoTransform decrypt = aes.CreateDecryptor())
                {
                    byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                    return Encoding.Unicode.GetString(dest);
                }
            }

        }
        #endregion

        #region Settings
        public class Settings
        {
            public static string DBSrvIP { get; set; } //IP address of DB server
            public static string DBSrvPort { get; set; } //Port number of DB server
            public static string DBconnectID { get; set; } //ID of DB user
            public static string DBconnectPw { get; set; } //Pw of DB user
            public static string settingFile_location { get; set; } //Config file path
            public static string lang { get; set; } //language
            public static string sslSetting { get; set; } //SSL setting string
            public static string ptInfoPlugin { get; set; } //File location of the plug-in to get patient information

            Settings()
            {
                Settings.DBSrvIP = "";
                Settings.DBSrvPort = "";
                Settings.DBconnectID = "";
                Settings.DBconnectPw = "";
            }

            public static void initiateSettings()
            {
                settingFile_location = Application.StartupPath + "\\settings.config";
                readSettings();
                lang = Application.CurrentCulture.TwoLetterISOLanguageName;
                //Settings.sslSetting = ""; //Use this when you want to connect without using SSL
                sslSetting = "SSL=true;SslMode=Require;"; //Use this when you want to connect using SSL
                ptInfoPlugin = checkPtInfoPlugin();
            }

            public static void saveSettings()
            {
                Settings4file st = new Settings4file();
                st.DBSrvIP = Settings.DBSrvIP;
                st.DBSrvPort = Settings.DBSrvPort;
                st.DBconnectID = Settings.DBconnectID;
                st.DBconnectPw = PasswordEncoder.Encrypt(Settings.DBconnectPw);

                //＜バイナリファイルに書き込む＞
                //BinaryFormatterオブジェクトを作成
                BinaryFormatter bf1 = new BinaryFormatter();
                //ファイルを開く
                System.IO.FileStream fs1 =
                    new System.IO.FileStream(Settings.settingFile_location, System.IO.FileMode.Create);
                //シリアル化し、バイナリファイルに保存する
                bf1.Serialize(fs1, st);
                //閉じる
                fs1.Close();

            }

            public static void readSettings()
            {
                if (System.IO.File.Exists(Settings.settingFile_location))
                {
                    Settings4file st = new Settings4file();

                    //＜バイナリファイルから読み込む＞
                    //BinaryFormatterオブジェクトの作成
                    BinaryFormatter bf2 = new BinaryFormatter();
                    //ファイルを開く
                    System.IO.FileStream fs2 =
                        new System.IO.FileStream(Settings.settingFile_location, System.IO.FileMode.Open);
                    //バイナリファイルから読み込み、逆シリアル化する
                    try
                    {
                        st = (Settings4file)bf2.Deserialize(fs2);
                        fs2.Close();
                    }
                    catch (InvalidOperationException)
                    {
                        DialogResult ret;
                        ret = MessageBox.Show(Properties.Resources.SettingFileBroken, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        fs2.Close();
                        if (ret == DialogResult.Yes)
                        {
                            file_control.delFile(Settings.settingFile_location);
                        }
                    }

                    Settings.DBSrvIP = st.DBSrvIP;
                    Settings.DBSrvPort = st.DBSrvPort;
                    Settings.DBconnectID = st.DBconnectID;
                    Settings.DBconnectPw = PasswordEncoder.Decrypt(st.DBconnectPw);
                }
            }

            public static string checkPtInfoPlugin()
            {
                if (File.Exists(Application.StartupPath + "\\plugins.ini"))
                {
                    string text = file_control.readFromFile(Application.StartupPath + "\\plugins.ini");
                    string plugin_location = file_control.readItemSettingFromText(text, "Patient information=");
                    if (File.Exists(plugin_location))
                    { return plugin_location; }
                    else
                    { return ""; }
                }
                else
                { return ""; }
            }
        }
        #endregion

        #region file_control
        //ファイル保存するためのクラス
        [Serializable()]
        public class Settings4file
        {
            public string DBSrvIP { get; set; } //データベースサーバーのIPアドレスを格納するプロパティ
            public string DBSrvPort { get; set; } //データベースサーバーのポート番号を格納するプロパティ
            public string DBconnectID { get; set; } //データベースに接続するためのIDを格納するプロパティ
            public string DBconnectPw { get; set; } //データベースに接続するためのパスワードを格納するプロパティ
        }

        public class file_control
        {
            public static void delFile(string fileName)
            {
                if (System.IO.File.Exists(fileName) == true)
                {
                    try
                    {
                        System.IO.File.Delete(fileName);
                    }
                    catch (System.IO.IOException)
                    {
                        MessageBox.Show(Properties.Resources.FileBeingUsed, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        MessageBox.Show(Properties.Resources.PermissionDenied, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.FileNotExist, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                    MessageBox.Show("[settings.config]"+Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }
        #endregion
    }
}
