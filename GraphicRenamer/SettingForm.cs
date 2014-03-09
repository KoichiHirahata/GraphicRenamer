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
using System.Net;

namespace GraphicRenamer
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();

            if (System.IO.File.Exists(Application.StartupPath + "\\settings.ini"))
            {
                Settings.readSettings();
                if (Settings.imgDir.Length == 0)
                { this.tbSaveDir.Text = "(" + Properties.Resources.NotConfigured + ")"; }
                else
                { this.tbSaveDir.Text = Settings.imgDir; }
            }
            else
            { this.tbSaveDir.Text = "(" + Properties.Resources.InitialSetting + ")"; }

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
            if (e.KeyCode == Keys.Enter)
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
            Settings.saveSettings();
            this.Close();
        }
        #endregion
    }

    #region File_control
    //PDFデータの保存先などを記録するsetting.config用のクラス
    public class Settings
    {
        public static string imgDir { get; set; }
        public static Boolean isJP { get; set; } //Property for storing that machine's language is Japanese or not.

        //ファイルから読み込む
        public static void readSettings()
        {
            if (System.IO.File.Exists(Application.StartupPath + "\\settings.ini"))
            {
                string text = "";
                #region Read from file
                try
                {
                    using (StreamReader sr = new StreamReader(Application.StartupPath + "\\settings.ini"))
                    { text = sr.ReadToEnd(); }
                }
                catch (Exception e)
                { MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                #endregion

                int index;
                #region Read imgDir
                index = text.IndexOf("Image Directory:");
                if (index == -1)
                {
                    MessageBox.Show("[settings.ini]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                { Settings.imgDir = Endoscopy.getUntilNewLine(text, index + 16); }
                #endregion
            }
            else
            { Settings.imgDir = ""; }
        }

        //ファイルに書き込む
        public static void saveSettings()
        {
            string text = "";
            if (System.IO.File.Exists(Application.StartupPath + "\\settings.ini"))
            {
                #region Read from file
                try
                {
                    using (StreamReader sr = new StreamReader(Application.StartupPath + "\\settings.ini"))
                    { text = sr.ReadToEnd(); }
                }
                catch (Exception e)
                { MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                #endregion

                int index;
                #region write imgDir to text
                index = text.IndexOf("Image Directory:");
                if (index == -1)
                {
                    MessageBox.Show("[settings.ini]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    int temp_text_length = text.Length;
                    for (int i = 16; (index + i) <= temp_text_length; i++)
                    {
                        if ((index + i) == temp_text_length)
                        {
                            text = (text.Substring(0, index + 16) + Settings.imgDir);
                            break;
                        }
                        else if (text.Substring(index + 16, 1) != "\r")
                        { text = (text.Substring(0, index + 16) + text.Substring(index + 17)); }
                        else
                        {
                            text = (text.Substring(0, index + 16) + Settings.imgDir + text.Substring(index + 16));
                            break;
                        }
                    }
                }
                #endregion
            }
            else
            { text = "Image Directory:" + Settings.imgDir + "\r\nOpen folder button visible:False"; }

            #region Save to settings.ini
            StreamWriter sw = new StreamWriter(Application.StartupPath + @"\settings.ini", false);
            sw.Write(text);
            sw.Close();
            #endregion
        }
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
    }
    #endregion
}