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
using System.Collections;

namespace GraphicRenamer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;//これで2診が大丈夫かチェック。
            //monthCalendar1.Font = new System.Drawing.Font(monthCalendar1.Font.Name, (float)11.25);

            pictureBox1.AllowDrop = true;

            Settings.readSettings();
            Settings.isJP = (Application.CurrentCulture.TwoLetterISOLanguageName == "ja");

            if (Settings.isJP)
            { pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\jp.png"); }
            else
            { pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\eng.png"); }

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingForm sf = new SettingForm();
            sf.ShowDialog(this);
            sf.Dispose();
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) //ドラッグされている内容がファイル、フォルダの場合
            { e.Effect = DragDropEffects.Copy; }　//コピーを許可するようにドラッグ元に通知する
            else
            { e.Effect = DragDropEffects.None; }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] gFiles = e.Data.GetData(DataFormats.FileDrop) as string[];

            #region Error check
            if (string.IsNullOrWhiteSpace(Settings.imgDir))
            {
                MessageBox.Show("[" + Properties.Resources.InitialSetting + "]" + Properties.Resources.NotConfigured, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(Settings.imgDir))
            {
                MessageBox.Show("[" + Properties.Resources.InitialSetting + "]" + Properties.Resources.FolderNotExist, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!isSameType(gFiles))
            { return; }
            #endregion

            //gFilesを配列からArrayListに変換してソート。
            ArrayList gFilesArray = new ArrayList();
            gFilesArray.AddRange(gFiles);
            gFilesArray.Sort();

            #region Endoscopy Folder
            if (Directory.Exists(gFilesArray[0].ToString()))
            {
                if (MessageBox.Show(Properties.Resources.IgnoreTextBox, "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                { return; }


                for (int i = 0; i < gFilesArray.Count; i++)
                {
                    Endoscopy.moveFigures(gFilesArray[i].ToString());
                    //●●●進行状況を別ウィンドウで表示する。
                }
                return;
            }
            #endregion

            #region Error check for tbID
            if (tbID.Text.Length == 0)
            {
                MessageBox.Show(Properties.Resources.NoID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (isWrongFolderName(tbID.Text))
            {
                MessageBox.Show("[ID] " + Properties.Resources.WrongText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            string imgPath = Settings.imgDir + @"\" + tbID.Text;
            createFolder(imgPath);

            string serialNo = getSerialNo(imgPath, tbID.Text, monthCalendar1.SelectionStart.ToString("yyyyMMdd"));
            #region Error check of serialNo
            if (serialNo == "error")
            {
                MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region 1File
            if (gFilesArray.Count == 1)
            {
                if ((gFiles[0].Substring(gFiles[0].Length - 4).ToLower() == ".jpg") || (gFiles[0].Substring(gFiles[0].Length - 5).ToLower() == ".jpeg"))
                {
                    try
                    { 
                        File.Move(gFilesArray[0].ToString(), imgPath + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + ".jpg");
                        logTitle(Path.GetDirectoryName(gFilesArray[0].ToString()), imgPath);
                        logFileName(Path.GetFileName(gFilesArray[0].ToString()), tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + ".jpg");
                    }
                    #region catch
                    catch (IOException)
                    {
                        MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("[Unauthorized Access Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show("[Argument Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    #endregion
                }
                else if (gFiles[0].Substring(gFiles[0].Length - 4).ToLower() == ".pdf")
                {
                    try
                    {
                        File.Move(gFilesArray[0].ToString(), imgPath + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + ".pdf");
                        logTitle(Path.GetDirectoryName(gFilesArray[0].ToString()), imgPath);
                        logFileName(Path.GetFileName(gFilesArray[0].ToString()), tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + ".pdf");
                    }
                    #region catch
                    catch (IOException)
                    {
                        MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("[Unauthorized Access Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show("[Argument Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    #endregion
                }
            }
            #endregion
            #region More files
            else
            {
                string subFolderName = imgPath + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo;
                createFolder(subFolderName);
                logTitle(Path.GetDirectoryName(gFilesArray[0].ToString()), subFolderName);

                #region JPEG
                if ((gFiles[0].Substring(gFiles[0].Length - 4).ToLower() == ".jpg") || (gFiles[0].Substring(gFiles[0].Length - 5).ToLower() == ".jpeg"))
                {
                    for (int i = 0; i < gFilesArray.Count; i++)
                    {
                        #region Error check
                        if (plusZero((i + 1).ToString(), 3) == "Error")
                        {
                            MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        #endregion
                        try
                        {
                            File.Move(gFilesArray[i].ToString(),
                              subFolderName + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + "-" + plusZero((i + 1).ToString(), 3) + ".jpg");
                            logFileName(Path.GetFileName(gFilesArray[i].ToString()), 
                                tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + "-" + plusZero((i + 1).ToString(), 3) + ".jpg");
                        }
                        #region catch
                        catch (IOException)
                        {
                            MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            MessageBox.Show("[Unauthorized Access Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        catch (ArgumentException)
                        {
                            MessageBox.Show("[Argument Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        #endregion
                    }
                }
                #endregion
                #region PDF
                else if (gFiles[0].Substring(gFiles[0].Length - 4).ToLower() == ".pdf")
                {
                    for (int i = 0; i < gFilesArray.Count; i++)
                    {
                        #region Error check
                        if (plusZero((i + 1).ToString(), 3) == "Error")
                        {
                            MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        #endregion
                        try
                        {
                            File.Move(gFilesArray[i].ToString(),
                                subFolderName + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + "-" + plusZero((i + 1).ToString(), 3) + ".pdf");
                            logFileName(Path.GetFileName(gFilesArray[i].ToString()),
                                tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + "-" + plusZero((i + 1).ToString(), 3) + ".pdf");
                        }
                        #region catch
                        catch (IOException)
                        {
                            MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            MessageBox.Show("[Unauthorized Access Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        catch (ArgumentException)
                        {
                            MessageBox.Show("[Argument Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        #endregion
                    }
                }
                #endregion
            }
            #endregion
        }

        #region Functions for error check
        private Boolean isSameType(string[] gFiles) //gFilesがフォルダ、Jpeg、PDFのいずれか単一の種類になっているのを確認する関数
        {
            //最初のstringがフォルダだった場合、他が全部フォルダかどうか確認する。違ったらfalse返す。
            if (Directory.Exists(gFiles[0]))
            {
                for (int i = 1; i < gFiles.Length; i++)
                {
                    if (!Directory.Exists(gFiles[i]))
                    {
                        MessageBox.Show(Properties.Resources.DontDropFolderFiles, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return true;
            }

            //最初のstringがjpgだった場合、他が全部jpgかどうか確認する。違ったらfalse返す。
            if ((gFiles[0].Substring(gFiles[0].Length - 4).ToLower() == ".jpg") || (gFiles[0].Substring(gFiles[0].Length - 5).ToLower() == ".jpeg"))
            {
                for (int i = 1; i < gFiles.Length; i++)
                {
                    if (!(gFiles[i].Substring(gFiles[i].Length - 4).ToLower() == ".jpg") || (gFiles[i].Substring(gFiles[i].Length - 5).ToLower() == ".jpeg"))
                    {
                        MessageBox.Show(Properties.Resources.DontDropJpgWithOther, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return true;
            }

            //最初のstringがpdfだった場合、他が全部pdfかどうか確認する。違ったらfalse返す。
            if (gFiles[0].Substring(gFiles[0].Length - 4).ToLower() == ".pdf")
            {
                for (int i = 1; i < gFiles.Length; i++)
                {
                    if (!(gFiles[0].Substring(gFiles[i].Length - 4).ToLower() == ".pdf"))
                    {
                        MessageBox.Show(Properties.Resources.DontDropJpgWithOther, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        private Boolean isWrongFolderName(string str)
        {
            string[] checkStr = { "/", @"\", ":", "*", "?", "\"", "<", ">", "|", "#", "{", "}", "%", "&", "~", ".." };
            for (int i = 0; i < checkStr.Count(); i++)
            {
                if (0 <= str.IndexOf(checkStr[i]))
                    return true;
            }

            if (str.Substring(0, 1) == ".")
            { return true; }

            if (str.Substring(str.Length - 1, 1) == ".")
            { return true; }

            if (str.Substring(0, 1) == "　")
            { return true; }

            if (str.Substring(str.Length - 1, 1) == "　")
            { return true; }

            return false;
        }
        #endregion

        #region Functions for numbering
        public static string getSerialNo(string imgPath, string patientID, string dateStr) //一番進んでいる通し番号をチェック調べて+1した数を返す。例）ID_日付_通し番号-サブ番号.jpg
        {
            int ret = 0;
            string[] gFiles = Directory.GetFiles(imgPath, patientID + "_" + dateStr + "_*.???", SearchOption.TopDirectoryOnly);
            string[] gFolders = Directory.GetDirectories(imgPath, patientID + "_" + dateStr + "_*", SearchOption.TopDirectoryOnly);
            string tempName;
            int tempNo;

            for (int i = 0; i < gFiles.Length; i++)
            {
                tempName = Path.GetFileName(gFiles[i]);
                tempNo = int.Parse(tempName.Substring(patientID.Length + dateStr.Length + 2, 3));
                if (tempNo > 998)
                { return "error"; }
                if (ret < tempNo)
                { ret = tempNo; }
            }

            for (int i = 0; i < gFolders.Length; i++)
            {
                tempName = gFolders[i].Substring(Path.GetDirectoryName(gFolders[i]).Length + 1);
                tempNo = int.Parse(tempName.Substring(patientID.Length + dateStr.Length + 2, 3));
                if (tempNo > 998)
                { return "error"; }
                if (ret < tempNo)
                { ret = tempNo; }
            }

            return plusZero((ret + 1).ToString(), 3);
        }

        public static string plusZero(string numberStr, int digits)
        {
            if (numberStr.Length > digits)
            { return "Error"; }
            for (int i = (digits - numberStr.Length); i > 0; i--)
            { numberStr = "0" + numberStr; }
            return numberStr;
        }
        #endregion

        #region Functions for logging
        public static void logTitle(string sourceDir, string moveTo)
        {
            #region Create directory
            if (!Directory.Exists(Application.StartupPath + @"\log"))
            { createFolder(Application.StartupPath + @"\log"); }

            if (!Directory.Exists(Application.StartupPath + @"\log\" + System.DateTime.Today.ToString("yyyy")))
            { createFolder(Application.StartupPath + @"\log\" + System.DateTime.Today.ToString("yyyy")); }
            #endregion

            StreamWriter sw
                = new StreamWriter(Application.StartupPath + @"\log\" + System.DateTime.Today.ToString("yyyy") + @"\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", true);
            sw.WriteLine("[" + System.DateTime.Now.ToString() + "] From:" + sourceDir + " To:" + moveTo);
            sw.Close();
        }

        public static void logFileName(string sourceFileName, string destFileName)
        {
            #region Create directory
            if (!Directory.Exists(Application.StartupPath + @"\log"))
            { createFolder(Application.StartupPath + @"\log"); }

            if (!Directory.Exists(Application.StartupPath + @"\log\" + System.DateTime.Today.ToString("yyyy")))
            { createFolder(Application.StartupPath + @"\log\" + System.DateTime.Today.ToString("yyyy")); }
            #endregion

            StreamWriter sw
                = new StreamWriter(Application.StartupPath + @"\log\" + System.DateTime.Today.ToString("yyyy") + @"\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", true);
            sw.WriteLine(sourceFileName + " -> " + destFileName);
            sw.Close();
        }
        #endregion

        public static void createFolder(string folderName)
        {
            try
            {
                if (!Directory.Exists(folderName))
                { System.IO.Directory.CreateDirectory(folderName); }
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            { tbID.Text = ""; }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        { System.Diagnostics.Process.Start("http://www.madeinclinic.jp/%E3%82%BD%E3%83%95%E3%83%88%E3%82%A6%E3%82%A7%E3%82%A2/pt_graphic/graphicrenamer/"); }
    }
}
