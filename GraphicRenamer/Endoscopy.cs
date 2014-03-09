using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace GraphicRenamer
{
    class Endoscopy
    {
        public static string determinFolderType(string folderPath)
        {
            if (File.Exists(folderPath + "\\patient.inf"))
            {
                string text;
                string ret = "unknown";
                #region Read from file
                try
                {
                    using (StreamReader sr = new StreamReader(folderPath + "\\patient.inf", Encoding.GetEncoding("Shift_JIS")))
                    { text = sr.ReadToEnd(); }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ret;
                }
                #endregion

                int index;
                #region Read P000:
                index = text.IndexOf("P000: ");
                if (index == -1)
                {
                    MessageBox.Show("[patient.inf]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ret;
                }
                else
                {
                    ret = getUntilNewLine(text, index + 6);
                    return ret;
                }
                #endregion
            }

            //●●●古いタイプの富士フイルム製かどうかチェック。TryParse使うなら、2014/02/08以前のコードに参考になるコードあるけどまぁググったら？

            return "Unknown";
        }

        /// <summary>patient.infから患者情報を取る関数。</summary>
        /// <param name="filePath">ファイルのパス</param>
        /// <returns>検査日、検査スタート時刻、検査終了時刻、患者ID、患者名の順の配列。</returns>
        public static string[] getPtInfo(string filePath) //
        {
            #region ret initiate
            string[] ret = new string[5];
            for (int i = 0; i < 5; i++)
            { ret[i] = ""; }
            #endregion

            if (Path.GetFileName(filePath) == "patient.inf")
            {
                string text = "";
                #region Read from file
                try
                {
                    using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("Shift_JIS")))
                    { text = sr.ReadToEnd(); }
                }
                catch (Exception e)
                { MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                #endregion

                int index;
                #region Read EXAM_DATE
                index = text.IndexOf("EXAM_DATE: ");
                if (index == -1)
                {
                    MessageBox.Show("[" + Path.GetFileName(filePath) + "]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ret;
                }
                else
                { ret[0] = getUntilNewLine(text, index + 11); }
                #endregion

                #region Read EXAM_START
                index = text.IndexOf("EXAM_START: ");
                if (index == -1)
                {
                    MessageBox.Show("[" + Path.GetFileName(filePath) + "]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ret;
                }
                else
                { ret[1] = getUntilNewLine(text, index + 12); }
                #endregion

                #region Read EXAM_END
                index = text.IndexOf("EXAM_END: ");
                if (index == -1)
                {
                    MessageBox.Show("[" + Path.GetFileName(filePath) + "]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ret;
                }
                else
                { ret[2] = getUntilNewLine(text, index + 10); }
                #endregion

                #region Read PID
                index = text.IndexOf("PID: ");
                if (index == -1)
                {
                    MessageBox.Show("[" + Path.GetFileName(filePath) + "]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ret;
                }
                else
                { ret[3] = getUntilNewLine(text, index + 5); }
                #endregion

                #region Read P(Patient name)
                index = text.IndexOf("P: ");
                if (index == -1)
                {
                    MessageBox.Show("[" + Path.GetFileName(filePath) + "]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ret;
                }
                else
                { ret[4] = getUntilNewLine(text, index + 3); }
                #endregion
            }
            else
            { MessageBox.Show("[" + Path.GetFileName(filePath) + "]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            return ret;
        }

        public enum EndoResult { success, failed, error }

        public static EndoResult moveFigures(string sourceDir)
        {
            string serialNo;
            string patientID;
            string dateStr;
            string destDir;//ファイルの移動先
            string[] textArray = new string[5];
            string[] jpgFiles;
            ArrayList jpgArray = new ArrayList();

            switch (Endoscopy.determinFolderType(sourceDir))
            {
                #region VP-4450HD
                case "VP-4450HD":
                    if (File.Exists(sourceDir + @"\patient.inf"))
                    {
                        textArray = Endoscopy.getPtInfo(sourceDir + @"\patient.inf");
                        patientID = textArray[3].ToString();
                        dateStr = textArray[0].ToString();

                        //ID、日付、名前、時間確認のフォーム表示。ID変更できるように。カレンダーも表示して、日付も変更できるよう配慮。なお、日付から[/]も消す。
                        #region set exam info
                        SetExamInfo sei = new SetExamInfo(patientID, dateStr, textArray[4].ToString(), textArray[1].ToString(), textArray[2].ToString());
                        sei.ShowDialog();
                        if (sei.OkCancel == "Cancel")
                        { return EndoResult.failed; }
                        patientID = sei.patientId;
                        dateStr = sei.examinationDate;//これで[/]はなくなってる。
                        sei.Dispose();
                        #endregion

                        MainForm.createFolder(Settings.imgDir + @"\" + patientID);

                        serialNo = MainForm.getSerialNo(Settings.imgDir + @"\" + patientID, patientID, dateStr);
                        if (serialNo == "error")
                        {
                            MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return EndoResult.failed;
                        }
                        MainForm.createFolder(Settings.imgDir + @"\" + patientID + @"\" + patientID + "_" + dateStr + "_" + serialNo);
                        destDir = Settings.imgDir + @"\" + patientID + @"\" + patientID + "_" + dateStr + "_" + serialNo;

                        jpgFiles = Directory.GetFiles(sourceDir, "*.JPG", SearchOption.TopDirectoryOnly);
                        jpgArray.AddRange(jpgFiles);
                        jpgArray.Sort();
                        string tempFilePath;//ファイル移動用

                        MainForm.logTitle(sourceDir, destDir);
                        for (int i = 0; i < jpgArray.Count; i++)
                        {
                            #region Error check
                            if (MainForm.plusZero((i + 1).ToString(), 3) == "Error")
                            {
                                MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return EndoResult.error;
                            }
                            #endregion

                            tempFilePath = destDir + @"\" + patientID + "_" + dateStr + "_" + serialNo + "-" + MainForm.plusZero((i + 1).ToString(), 3);

                            try
                            {
                                File.Move(jpgArray[i].ToString(), tempFilePath + ".jpg");
                                MainForm.logFileName(Path.GetFileName(jpgArray[i].ToString()), Path.GetFileName(tempFilePath + ".jpg"));
                            }
                            #region catch
                            catch (IOException)
                            {
                                MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return EndoResult.error;
                            }
                            catch (UnauthorizedAccessException)
                            {
                                MessageBox.Show("[Unauthorized Access Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return EndoResult.error;
                            }
                            catch (ArgumentException)
                            {
                                MessageBox.Show("[Argument Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return EndoResult.error;
                            }
                            #endregion

                            #region Move thu files
                            if (File.Exists(jpgArray[i].ToString().Substring(0, jpgArray[i].ToString().Length - 3) + "thu"))
                            {
                                try //Convert thu to tiff and move
                                {
                                    File.Move(jpgArray[i].ToString().Substring(0, jpgArray[i].ToString().Length - 3) + "thu", tempFilePath + ".tiff");
                                    MainForm.logFileName(Path.GetFileName(jpgArray[i].ToString().Substring(0, jpgArray[i].ToString().Length - 3) + "thu"),
                                        Path.GetFileName(tempFilePath + ".tiff"));
                                }
                                #region catch
                                catch (IOException)
                                {
                                    MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return EndoResult.error;
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    MessageBox.Show("[Unauthorized Access Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return EndoResult.error;
                                }
                                catch (ArgumentException)
                                {
                                    MessageBox.Show("[Argument Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return EndoResult.error;
                                }
                                #endregion
                            }
                            #endregion

                            #region Move tiff files
                            if (File.Exists(jpgArray[i].ToString().Substring(0, jpgArray[i].ToString().Length - 3) + "tiff"))
                            {
                                try
                                {
                                    File.Move(jpgArray[i].ToString().Substring(0, jpgArray[i].ToString().Length - 3) + "tiff", tempFilePath + ".tiff");
                                    MainForm.logFileName(Path.GetFileName(jpgArray[i].ToString().Substring(0, jpgArray[i].ToString().Length - 3) + "tiff"),
                                        Path.GetFileName(tempFilePath + ".tiff"));
                                }
                                #region catch
                                catch (IOException)
                                {
                                    MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return EndoResult.error;
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    MessageBox.Show("[Unauthorized Access Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return EndoResult.error;
                                }
                                catch (ArgumentException)
                                {
                                    MessageBox.Show("[Argument Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return EndoResult.error;
                                }
                                #endregion
                            }
                            #endregion
                        }

                        #region Move patient.inf
                        try //patient.infもID_日付_serialNo.infに変更して移動。
                        {
                            File.Move(sourceDir + @"\patient.inf", destDir + @"\patient.inf");
                            MainForm.logFileName(Path.GetFileName(sourceDir + @"\patient.inf"), Path.GetFileName(destDir + @"\patient.inf"));
                        }
                        #region catch
                        catch (IOException)
                        {
                            MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return EndoResult.error;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            MessageBox.Show("[Unauthorized Access Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return EndoResult.error;
                        }
                        catch (ArgumentException)
                        {
                            MessageBox.Show("[Argument Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return EndoResult.error;
                        }
                        #endregion
                        #endregion

                        #region Delete sourceDir
                        //元のフォルダの中身が空なのを確認してから削除する。
                        if (Directory.GetFiles(sourceDir, "*").Length == 0)
                        {
                            try
                            { Directory.Delete(sourceDir); }
                            catch (IOException)
                            {
                                MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return EndoResult.error;
                            }
                        }
                        #endregion

                        return EndoResult.success;
                    }
                    break;
                #endregion
            }
            return EndoResult.failed;
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
}
