using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System.Security.Cryptography;
using Npgsql;

namespace GraphicRenamer
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();

            if (System.IO.File.Exists(Application.StartupPath + "\\settings.config"))
            {
                Settings.readSettings();
                cbUseSrv.Checked = Settings.useDB;
                tbDBSrv.Text = Settings.DBSrvIP;
                tbDBsrvPort.Text = Settings.DBSrvPort;
                tbDbID.Text = Settings.DBconnectID;
                tbDBpw.Text = Settings.DBconnectPw;
                setDbPropertyVisibleOrNot();

                if (Settings.imgDir.Length == 0)
                { this.tbSaveDir.Text = "(" + Properties.Resources.NotConfigured + ")"; }
                else
                { this.tbSaveDir.Text = Settings.imgDir; }
            }
            else
            { this.tbSaveDir.Text = "(" + Properties.Resources.InitialSetting + ")"; }

            setDbPropertyVisibleOrNot();
            this.ActiveControl = this.btSave;
        }

        #region Buttons
        private void sansho_Bt_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog(); //余計なことしない方がよさそう。
            fbd.SelectedPath = Application.StartupPath;
            if (fbd.ShowDialog() == DialogResult.OK)
            { tbSaveDir.Text = fbd.SelectedPath; }
        }

        private void btCancel_Click(object sender, EventArgs e)
        { this.Close(); }

        private void btSave_Click(object sender, EventArgs e)
        { saveProcedure(); }

        private void tbSaveDir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && cbUseSrv.Checked == false)
            { saveProcedure(); }
        }

        private void saveProcedure()
        {
            #region ErrorCheck
            if ((tbSaveDir.Text == "(" + Properties.Resources.InitialSetting + ")") || (tbSaveDir.Text == "(" + Properties.Resources.NotConfigured + ")"))
            {
                MessageBox.Show("[Save to:]" + Properties.Resources.NotConfigured, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(tbSaveDir.Text))
            {
                MessageBox.Show("[Save to:]" + Properties.Resources.FolderNotExist, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            Settings.imgDir = tbSaveDir.Text;
            Settings.useDB = cbUseSrv.Checked;
            Settings.DBSrvIP = tbDBSrv.Text;
            Settings.DBSrvPort = tbDBsrvPort.Text;
            Settings.DBconnectID = tbDbID.Text;
            if (tbDBpw.Visible)
            { Settings.DBconnectPw = tbDBpw.Text; }

            if (cbUseSrv.Checked)
            {
                if (testConnect())
                {
                    Settings.saveSettings();
                    this.Close();
                }
            }
            else
            {
                Settings.saveSettings();
                this.Close();
            }
        }

        private void btPwSet_Click(object sender, EventArgs e)
        {
            tbDBpw.Visible = true;
            tbDBpw.Focus();
        }
        #endregion

        private void cbUseSrv_CheckedChanged(object sender, EventArgs e)
        { setDbPropertyVisibleOrNot(); }

        private void setDbPropertyVisibleOrNot()
        {
            if (cbUseSrv.Checked)
            {
                lbDBSrv.Visible = true;
                tbDBSrv.Visible = true;
                lbSrvSample.Visible = true;
                lbDBsrvPort.Visible = true;
                tbDBsrvPort.Visible = true;
                lbDbID.Visible = true;
                tbDbID.Visible = true;
                pwState.Visible = true;
                btPwSet.Visible = true;
                btTestConnect.Visible = true;
            }
            else
            {
                lbDBSrv.Visible = false;
                tbDBSrv.Visible = false;
                lbSrvSample.Visible = false;
                lbDBsrvPort.Visible = false;
                tbDBsrvPort.Visible = false;
                lbDbID.Visible = false;
                tbDbID.Visible = false;
                pwState.Visible = false;
                btPwSet.Visible = false;
                tbDBpw.Visible = false;
                btTestConnect.Visible = false;
            }
        }

        private void btTestConnect_Click(object sender, EventArgs e)
        {
            if (testConnect())
            { MessageBox.Show(Properties.Resources.ConnectSuccess, "Successfully connected.", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }

        private Boolean testConnect()
        {
            if (this.tbDBSrv.Text.Length == 0)
            {
                MessageBox.Show(Properties.Resources.ServerIP, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (this.tbDBsrvPort.Text.Length == 0)
            {
                MessageBox.Show(Properties.Resources.portUnconfigured, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (this.tbDbID.Text.Length == 0)
            {
                MessageBox.Show(Properties.Resources.NoID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string temp_pw;

            if (this.tbDBpw.Visible)
            {
                if (this.tbDBpw.Text.Length == 0)
                {
                    MessageBox.Show(Properties.Resources.pwUnconfigured, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                temp_pw = this.tbDBpw.Text;
            }
            else
            {
                if (Settings.DBconnectPw == null)
                {
                    MessageBox.Show(Properties.Resources.pwUnconfigured, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                temp_pw = Settings.DBconnectPw;
            }

            #region Npgsql

            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection("Server=" + this.tbDBSrv.Text + ";Port=" + this.tbDBsrvPort.Text + ";User Id=" +
                    this.tbDbID.Text + ";Password=" + temp_pw + ";Database=endoDB;" + Settings.sslSetting);
            }
            catch (ArgumentException)
            {
                MessageBox.Show(Properties.Resources.WrongConnectingString, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            #endregion

            try
            { conn.Open(); }
            catch (NpgsqlException)
            {
                MessageBox.Show(Properties.Resources.CouldntOpenConn, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
                return false;
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show(Properties.Resources.ConnClosed, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
                return false;
            }

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                return true;
            }
            else
            {
                MessageBox.Show(Properties.Resources.CouldntOpenConn, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
                return false;
            }
        }
    }

    #region Settings
    public class Settings
    {
        public static string imgDir { get; set; }
        public static Boolean openFolderButtonVisible { get; set; }
        public static Boolean useDB { get; set; }
        public static string DBSrvIP { get; set; } //IP address of DB server
        public static string DBSrvPort { get; set; } //Port number of DB server
        public static string DBconnectID { get; set; } //ID of DB user
        public static string DBconnectPw { get; set; } //Pw of DB user
        public static string settingFile_location { get; set; } //Config file path
        public static string lang { get; set; } //language
        public static string sslSetting { get; set; } //SSL setting string
        public static string ptInfoPlugin { get; set; } //File location of the plug-in to get patient information

        public static void initiateSettings()
        {
            settingFile_location = Application.StartupPath + "\\settings.config";
            readSettings();
            lang = Application.CurrentCulture.TwoLetterISOLanguageName;
            //Settings.sslSetting = ""; //Use this when you want to connect without using SSL
            sslSetting = "SSL=true;SslMode=Require;"; //Use this when you want to connect using SSL
            ptInfoPlugin = checkPtInfoPlugin();

            if (Settings.useDB)
            {
                if (!testConnect())
                {
                    MessageBox.Show(Properties.Resources.CouldntOpenConn, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Settings.useDB = false;
                }
            }
        }

        public static void saveSettings()
        {
            Settings4file st = new Settings4file();
            st.imgDir = Settings.imgDir;
            st.openFolderButtonVisible = Settings.openFolderButtonVisible;
            st.useDB = Settings.useDB;
            if (Settings.useDB)
            {
                st.DBSrvIP = Settings.DBSrvIP;
                st.DBSrvPort = Settings.DBSrvPort;
                st.DBconnectID = Settings.DBconnectID;
                st.DBconnectPw = PasswordEncoder.Encrypt(Settings.DBconnectPw);
            }
            else
            {
                st.DBSrvIP = "";
                st.DBSrvPort = "";
                st.DBconnectID = "";
                st.DBconnectPw = "";
            }

            XmlSerializer xserializer = new XmlSerializer(typeof(Settings4file));
            //Open file
            System.IO.FileStream fs1 =
                new System.IO.FileStream(Settings.settingFile_location, System.IO.FileMode.Create);
            xserializer.Serialize(fs1, st);
            fs1.Close();
        }

        //Read from file
        public static void readSettings()
        {
            if (System.IO.File.Exists(Settings.settingFile_location))
            {
                Settings4file st = new Settings4file();

                XmlSerializer xserializer = new XmlSerializer(typeof(Settings4file));
                System.IO.FileStream fs2 =
                    new System.IO.FileStream(Settings.settingFile_location, System.IO.FileMode.Open);
                try
                {
                    st = (Settings4file)xserializer.Deserialize(fs2);
                    fs2.Close();
                }
                catch (InvalidOperationException)
                {
                    DialogResult ret;
                    ret = MessageBox.Show(Properties.Resources.SettingFileBroken, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    fs2.Close();
                    if (ret == DialogResult.Yes)
                    { file_control.delFile(Settings.settingFile_location); }
                }

                Settings.imgDir = st.imgDir;
                Settings.openFolderButtonVisible = st.openFolderButtonVisible;
                Settings.useDB = st.useDB;
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

        public static Boolean testConnect()
        {
            #region Npgsql
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection("Server=" + Settings.DBSrvIP + ";Port=" + Settings.DBSrvPort + ";User Id=" +
                    Settings.DBconnectID + ";Password=" + Settings.DBconnectPw + ";Database=endoDB;" + Settings.sslSetting);
            }
            catch (ArgumentException)
            {
                MessageBox.Show(Properties.Resources.WrongConnectingString, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            #endregion

            try
            { conn.Open(); }
            catch (NpgsqlException)
            {
                MessageBox.Show(Properties.Resources.CouldntOpenConn, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
                return false;
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show(Properties.Resources.ConnClosed, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
                return false;
            }

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                return true;
            }
            else
            {
                MessageBox.Show(Properties.Resources.CouldntOpenConn, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
                return false;
            }
        }
    }

    [Serializable()]
    public class Settings4file
    {
        public string imgDir { get; set; }
        public Boolean openFolderButtonVisible { get; set; } //Property for PtGraViewer
        public Boolean useDB { get; set; }
        public string DBSrvIP { get; set; } //IP address of DB server
        public string DBSrvPort { get; set; } //Port number of DB server
        public string DBconnectID { get; set; } //ID of DB user
        public string DBconnectPw { get; set; } //Pw of DB user
    }
    #endregion

    #region FileControl
    public class file_control
    {
        public static void delFile(string fileName)
        {
            if (System.IO.File.Exists(fileName) == true)
            {
                try
                { System.IO.File.Delete(fileName); }
                catch (System.IO.IOException)
                { MessageBox.Show(Properties.Resources.FileBeingUsed, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                catch (System.UnauthorizedAccessException)
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
                MessageBox.Show("[settings.config]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    #region password
    public class PasswordEncoder
    {
        private PasswordEncoder() { }

        private const string AesIV = @"&%jqiIurtmslLE58";
        private const string AesKey = @"3uJi<9!$kM0lkxme";

        public static string Encrypt(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            { return ""; }

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] src = Encoding.Unicode.GetBytes(text);

            using (ICryptoTransform encrypt = aes.CreateEncryptor())
            {
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
                return Convert.ToBase64String(dest);
            }
        }

        public static string Decrypt(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            { return ""; }

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] src = System.Convert.FromBase64String(text);
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                return Encoding.Unicode.GetString(dest);
            }
        }
    }
    #endregion
}