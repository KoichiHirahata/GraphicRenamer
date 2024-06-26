﻿using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using Npgsql;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GraphicRenamer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            move_label.Visible = false;
            AutoScaleMode = AutoScaleMode.None;
            verToolStripMenuItem.Text += Assembly.GetEntryAssembly().GetName().Version.Major.ToString()
                + "." + Assembly.GetEntryAssembly().GetName().Version.Minor.ToString()
                + "." + Assembly.GetEntryAssembly().GetName().Version.Build.ToString();
            //monthCalendar1.Font = new System.Drawing.Font(monthCalendar1.Font.Name, (float)11.25);

            pictureBox1.AllowDrop = true;
            lbPtName.Text = "";

            Settings.initiateSettings();

            if (Settings.lang == "ja" && File.Exists(Application.StartupPath + @"\jp.png"))
            { pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\jp.png"); }
            else if (File.Exists(Application.StartupPath + @"\eng.png"))
            { pictureBox1.Image = Image.FromFile(Application.StartupPath + @"\eng.png"); }

            #region Pluginが設定されていなかったらbtSearchを表示しない
            if (!Settings.usePlugin)
            {
                btSearch.Visible = false;
            }
            #endregion
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingForm sf = new SettingForm();
            sf.Owner = this;
            sf.ShowDialog(this);
            sf.Dispose();
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) //When draged objects are files or folders,
            { e.Effect = DragDropEffects.Copy; }　//indicate that they can be dragged.
            else
            { e.Effect = DragDropEffects.None; }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            try
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

                if (file_control.CheckID(tbID.Text) == false)
                {
                    MessageBox.Show(Properties.Resources.WrongText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!isSameType(gFiles))
                { 
                    return; 
                }
                #endregion

                //Convert gFiles from array to ArrayList, then sort it
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
                        Endoscopy.moveImagesAndFiles(gFilesArray[i].ToString(), this);
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

                #region If user use FindingsEditor or Plugins, check patient is blank or 'No data'
                if (Settings.useFeDB || Settings.useFeDB)
                {
                    if (lbPtName.Text == "" || lbPtName.Text == "No data" || lbPtName.Text == null)
                    {
                        MessageBox.Show("[ID] " + Properties.Resources.WrongText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                #endregion
                #endregion

                string imgPath = Settings.imgDir + file_control.MakeDirPath(tbID.Text) ;
                createFolder(imgPath);

                string serialNo = getSerialNo(imgPath, tbID.Text, monthCalendar1.SelectionStart.ToString("yyyyMMdd"));
                #region Error check of serialNo
                if (serialNo == "error")
                {
                    MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                #endregion

                // 1File
                if (gFilesArray.Count == 1)
                {
                    var extension = System.IO.Path.GetExtension(gFiles[0].ToString());

                    if ((string.Compare(extension, ".jpg", true) == 0) || (string.Compare(extension, ".jpeg", true) == 0) 
                        || (string.Compare(extension, ".JPG", true) == 0) || (string.Compare(extension, ".JPEG", true) == 0))
                    {
                        try
                        {
                            File.Move(gFilesArray[0].ToString(), imgPath + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + ".jpg");
                            logTitle(Path.GetDirectoryName(gFilesArray[0].ToString()), imgPath);
                            logFileName(Path.GetFileName(gFilesArray[0].ToString()), tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + ".jpg");
                        }
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
                    }
                    else if ((string.Compare(extension, ".heic", true) == 0) || (string.Compare(extension, ".heif", true) == 0))
                    {
                        try
                        {
                            var outputPath = @"../test.jpg";
                            HeicToJpeg.Convert(gFilesArray[0].ToString(), outputPath);
                            File.Move(outputPath, imgPath + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + ".jpg");
                            logTitle(Path.GetDirectoryName(outputPath), imgPath);
                            logFileName(Path.GetFileName(outputPath), tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + ".jpg");
                            File.Delete(gFilesArray[0].ToString());
                        }
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
                    }
                    else if (extension == ".pdf" || extension == ".PDF")
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
                    else if (extension == ".doc" || extension == ".docx")
                    {
                        try
                        {
                            File.Move(gFilesArray[0].ToString(), imgPath + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + extension);
                            logTitle(Path.GetDirectoryName(gFilesArray[0].ToString()), imgPath);
                            logFileName(Path.GetFileName(gFilesArray[0].ToString()), tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + extension);
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
                    else if (extension == ".xls" || extension == ".xlsx")
                    {
                        try
                        {
                            File.Move(gFilesArray[0].ToString(), imgPath + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + extension);
                            logTitle(Path.GetDirectoryName(gFilesArray[0].ToString()), imgPath);
                            logFileName(Path.GetFileName(gFilesArray[0].ToString()), tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + extension);
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
                // more file
                else
                {
                    string subFolderName = imgPath + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo;
                    createFolder(subFolderName);
                    logTitle(Path.GetDirectoryName(gFilesArray[0].ToString()), subFolderName);

                    var extension = System.IO.Path.GetExtension(gFiles[0].ToString());
                    //heic
                    if ((string.Compare(extension, ".heic", true) == 0) || (string.Compare(extension, ".heif", true) == 0))
                    {
                        for (int i = 0; i < gFilesArray.Count; i++)
                        {
                            if (plusZero((i + 1).ToString(), 3) == "Error")
                            {
                                MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            try
                            {
                                var outputPath = @"../test.jpg";
                                HeicToJpeg.Convert(gFilesArray[i].ToString(), outputPath);

                                File.Move(outputPath, subFolderName + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + "-" + plusZero((i + 1).ToString(), 3) + ".jpg");
                                logFileName(Path.GetFileName(outputPath), tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + "-" + plusZero((i + 1).ToString(), 3) + ".jpg");
                                File.Delete(gFilesArray[i].ToString());
                            }
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
                        }
                    }
                    //JPEG
                    else if ((string.Compare(extension, ".jpg", true) == 0) || (string.Compare(extension, ".jpeg", true) == 0)
                        || (string.Compare(extension, ".JPG", true) == 0) || (string.Compare(extension, ".JPEG", true) == 0))
                    {
                        for (int i = 0; i < gFilesArray.Count; i++)
                        {
                            if (plusZero((i + 1).ToString(), 3) == "Error")
                            {
                                MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            try
                            {
                                File.Move(gFilesArray[i].ToString(),
                                  subFolderName + @"\" + tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + "-" + plusZero((i + 1).ToString(), 3) + ".jpg");
                                logFileName(Path.GetFileName(gFilesArray[i].ToString()),
                                    tbID.Text + "_" + monthCalendar1.SelectionStart.ToString("yyyyMMdd") + "_" + serialNo + "-" + plusZero((i + 1).ToString(), 3) + ".jpg");
                            }
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
                        }
                    }
                    //pdf
                    else if (extension == ".pdf" || extension == ".PDF")
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("[pictureBox1_DragDrop] " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("[pictureBox1_DragDrop] " + ex.Message);
            }
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
            var firstxxtension = System.IO.Path.GetExtension(gFiles[0].ToString());
            if ((string.Compare(firstxxtension, ".jpg", true) == 0) || (string.Compare(firstxxtension, ".jpeg", true) == 0)
                || (string.Compare(firstxxtension, ".JPG", true) == 0) || (string.Compare(firstxxtension, ".JPEG", true) == 0))
            {
                for (int i = 1; i < gFiles.Length; i++)
                {
                    var extension = System.IO.Path.GetExtension(gFiles[i].ToString());
                    if (!((string.Compare(extension, ".jpg", true) == 0) || (string.Compare(extension, ".jpeg", true) == 0) 
                        || (string.Compare(firstxxtension, ".JPG", true) == 0) || (string.Compare(firstxxtension, ".JPEG", true) == 0)))
                    {
                        MessageBox.Show(Properties.Resources.DontDropJpgWithOther, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                }
                return true;
            }
            //word
            if ((string.Compare(firstxxtension, ".doc", true) == 0) || (string.Compare(firstxxtension, ".docx", true) == 0))
            {
                for (int i = 1; i < gFiles.Length; i++)
                {
                    var extension = System.IO.Path.GetExtension(gFiles[i].ToString());
                    if (!((string.Compare(extension, ".doc", true) == 0) || (string.Compare(extension, ".docx", true) == 0)))
                    {
                        MessageBox.Show(Properties.Resources.DontDropJpgWithOther, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                }
                return true;
            }
            //excel
            if ((string.Compare(firstxxtension, ".xls", true) == 0) || (string.Compare(firstxxtension, ".xlsx", true) == 0))
            {
                for (int i = 1; i < gFiles.Length; i++)
                {
                    var extension = System.IO.Path.GetExtension(gFiles[i].ToString());
                    if (!((string.Compare(extension, ".xls", true) == 0) || (string.Compare(extension, ".xlsx", true) == 0)))
                    {
                        MessageBox.Show(Properties.Resources.DontDropJpgWithOther, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                }
                return true;
            }
            //heic
            if ((gFiles[0].Substring(gFiles[0].Length - 5).ToLower() == ".heic") || (gFiles[0].Substring(gFiles[0].Length - 5).ToLower() == ".HEIC"))
            {
                for (int i = 1; i < gFiles.Length; i++)
                {
                    var extension = System.IO.Path.GetExtension(gFiles[i].ToString());
                    if (!((extension == ".heic") || (extension == ".HEIC")))
                    {
                        MessageBox.Show(Properties.Resources.DontDropJpgWithOther, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                }
                return true;
            }

            //最初のstringがpdfだった場合、他が全部pdfかどうか確認する。違ったらfalse返す。
            //pdf
            if (firstxxtension == ".pdf" || firstxxtension == ".PDF")
            {
                for (int i = 1; i < gFiles.Length; i++)
                {
                    var extension = System.IO.Path.GetExtension(gFiles[i].ToString());

                    if (!(extension == ".pdf" || extension == ".PDF"))
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
            string[] gFiles = Directory.GetFiles(imgPath, patientID + "_" + dateStr + "_*.*", SearchOption.TopDirectoryOnly);
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
        { logSomething("[" + System.DateTime.Now.ToString() + "] From:" + sourceDir + " To:" + moveTo); }

        public static void logFileName(string sourceFileName, string destFileName)
        { logSomething(sourceFileName + " -> " + destFileName); }

        public static void logSomething(string str)
        {
            #region Create directory
            if (!Directory.Exists(Application.StartupPath + @"\log"))
            { createFolder(Application.StartupPath + @"\log"); }

            if (!Directory.Exists(Application.StartupPath + @"\log\" + System.DateTime.Today.ToString("yyyy")))
            { createFolder(Application.StartupPath + @"\log\" + System.DateTime.Today.ToString("yyyy")); }
            #endregion

            StreamWriter sw
                = new StreamWriter(Application.StartupPath + @"\log\" + System.DateTime.Today.ToString("yyyy") + @"\" + System.DateTime.Today.ToString("yyyyMMdd") + ".log", true);
            sw.WriteLine(str);
            sw.Close();
        }
        #endregion

        #region ReadPatientData
        private void tbID_KeyUp(object sender, KeyEventArgs e)
        {
            lbPtName.Text = db.GetPtName(tbID.Text);
        }
        #endregion

        public static void createFolder(string folderName)
        {
            try
            {
                if (!Directory.Exists(folderName))
                { Directory.CreateDirectory(folderName); }
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            { clearTbPtId(); }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        { Process.Start("http://www.madeinclinic.jp/%E3%82%BD%E3%83%95%E3%83%88%E3%82%A6%E3%82%A7%E3%82%A2/pt_graphic/graphicrenamer/"); }

        public void changeMoveLabelText(string str)
        {
            move_label.Text = str;
            move_label.Visible = true;
            Update();
        }

        public void makeInvisibleMoveLable()
        { move_label.Visible = false; }

        private void clearTbPtId()
        { tbID.Text = ""; }

        private void button1_Click(object sender, EventArgs e)
        {
            clearTbPtId();
            lbPtName.Text = "";
            tbID.Focus();
        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            Search s = new Search();
            s.Owner = this;
            s.ShowDialog();

            if (s.ptId != null)
            {
                tbID.Text = s.ptId;
                lbPtName.Text = db.GetPtName(tbID.Text);
            }

            s.Dispose();
        }

        private void viewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Application.StartupPath + "\\PtGraViewer.exe"))
                {
                    if (!String.IsNullOrWhiteSpace(tbID.Text))
                    {
                        Process p = Process.Start(Application.StartupPath + "\\PtGraViewer.exe", "/pt:" + tbID.Text);
                    }
                    else
                    {
                        Process p = Process.Start(Application.StartupPath + "\\PtGraViewer.exe");
                    }
                }
                else
                {
                    MessageBox.Show("File not exist: PtGraViewer.exe", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            #region catch
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }
    }
}
