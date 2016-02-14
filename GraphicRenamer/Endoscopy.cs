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
    public class Endoscopy
    {
        public static string determinFolderType(string folderPath)
        {
            string text;
            string ret = "Unknown";

            #region Check empty folder
            if (Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).Length == 0)
            { return "Empty"; }
            #endregion

            #region Only Thumbs.db
            if (Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).Length == 1 && File.Exists(folderPath + "\\Thumbs.db"))
            { return "Only Thumbs.db"; }
            #endregion

            if (File.Exists(folderPath + "\\patient.inf"))
            {
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
            else if (Directory.GetFiles(folderPath, "*.inf", SearchOption.TopDirectoryOnly).Length > 0)
            {
                string[] infs = Directory.GetFiles(folderPath, "*.inf");

                if (infs.Length != 0)
                {
                    #region Read from file
                    try
                    {
                        using (StreamReader sr = new StreamReader(infs[0], Encoding.GetEncoding("Shift_JIS")))
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
                        MessageBox.Show("[inf file]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return ret;
                    }
                    else
                    {
                        ret = getUntilNewLine(text, index + 6).Trim();
                        return ret;
                    }
                    #endregion
                }
            }
            else if (Directory.Exists(folderPath + "\\DCIM") && Directory.Exists(folderPath + "\\CV\\STUDY"))
            {
                if (Directory.GetDirectories(folderPath + "\\DCIM", "*", SearchOption.TopDirectoryOnly).Length > 0)
                { return "OLYMPUS"; }
            }
            else if (File.Exists(folderPath + "\\ExamInfo.xml") && Directory.GetFiles(folderPath, "*.jpg", SearchOption.TopDirectoryOnly).Length > 0)
            { return "MovedOLYMPUS"; }

            return ret;
        }

        /// <summary>Function to get patient information from patient.inf (For FUJIFILM system)</summary>
        /// <param name="filePath">Path of file</param>
        /// <returns>Array of examination date, start time, end time, patient ID, patient name.</returns>
        public static string[] getPtInfoFujifilm(string filePath) //
        {
            #region ret initiate
            string[] ret = new string[5];
            for (int i = 0; i < 5; i++)
            { ret[i] = ""; }
            #endregion

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
            { ret[3] = getUntilNewLine(text, index + 5).Trim(); }
            #endregion

            #region Read P(Patient name)
            index = text.IndexOf("P: ");
            if (index == -1)
            {
                MessageBox.Show("[" + Path.GetFileName(filePath) + "]" + Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ret;
            }
            else
            { ret[4] = getUntilNewLine(text, index + 3).Trim(); }
            #endregion

            return ret;
        }

        /// <summary>Function to get information from ExamInfo.xml (For OLYMPUS system)</summary>
        /// <param name="filePath">Path of file</param>
        /// <returns>Array of examination date, start time, end time, patient ID, patient name.</returns>
        public static string[] getPtInfoOlympus(string filePath) //
        {
            #region ret initiate
            string[] ret = new string[5];
            for (int i = 0; i < 5; i++)
            { ret[i] = ""; }
            #endregion

            string text = "";
            #region Read from file
            try
            {
                using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("utf-8")))
                { text = sr.ReadToEnd(); }
            }
            catch (Exception e)
            { MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            #endregion

            ret[0] = getValueOfTag(text, "date");
            ret[1] = ""; //start time
            ret[2] = ""; //end time
            ret[3] = getValueOfTag(text, "idno");
            ret[4] = getValueOfTag(text, "name");
            return ret;
        }


        public enum EndoResult { success, failed, error, fileExist, skipped }

        public static EndoResult moveImagesAndFiles(string sourceDir, MainForm ownerForm)
        {
            string serialNo;
            string patientID;
            string dateStr;
            string destDir;//ファイルの移動先
            string[] testInfoArray = new string[5]; //examination date, start time, end time, patient ID, patient name
            string[] graphicFiles;
            ArrayList graphicArray = new ArrayList();

            string[] tempArray;

            switch (determinFolderType(sourceDir))
            {
                #region VP-4450HD, VP-4400
                case "VP-4450HD":
                case "VP-4400":
                    string[] infs = Directory.GetFiles(sourceDir, "*.inf");

                    if (File.Exists(infs[0]))
                    {
                        testInfoArray = Endoscopy.getPtInfoFujifilm(infs[0]);
                        if (isTimeEmpty(testInfoArray[2]))
                        { return EndoResult.skipped; }
                        patientID = testInfoArray[3].ToString();
                        dateStr = testInfoArray[0].ToString().Replace("/", "");
                        tempArray = prepareToMove(testInfoArray, ownerForm);
                        if (tempArray[0] == "")
                        { return EndoResult.failed; }

                        destDir = tempArray[0];
                        serialNo = tempArray[1];
                        patientID = tempArray[2];

                        graphicFiles = Directory.GetFiles(sourceDir, "*.JPG", SearchOption.TopDirectoryOnly);
                        graphicArray.AddRange(graphicFiles);
                        graphicArray.Sort();
                        string tempFilePath; //For moving files

                        MainForm.logTitle(sourceDir, destDir);
                        for (int i = 0; i < graphicArray.Count; i++)
                        {
                            #region Error check
                            if (MainForm.plusZero((i + 1).ToString(), 3) == "Error")
                            {
                                MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return EndoResult.error;
                            }
                            #endregion

                            ownerForm.changeMoveLabelText("Now moving files [" + (i + 1).ToString() + "/" + graphicArray.Count.ToString() + "]");

                            tempFilePath = destDir + @"\" + patientID + "_" + dateStr + "_" + serialNo + "-" + MainForm.plusZero((i + 1).ToString(), 3);

                            try
                            {
                                File.Move(graphicArray[i].ToString(), tempFilePath + ".jpg");
                                MainForm.logFileName(Path.GetFileName(graphicArray[i].ToString()), Path.GetFileName(tempFilePath + ".jpg"));
                            }
                            #region catch
                            catch (IOException)
                            {
                                MainForm.logSomething("[IO Exception]" + graphicArray[i].ToString() + " to " + tempFilePath + ".jpg");
                                MessageBox.Show("[" + graphicArray[i].ToString() + " to " + tempFilePath + ".jpg]\r\n"
                                    + "[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            if (File.Exists(graphicArray[i].ToString().Substring(0, graphicArray[i].ToString().Length - 3) + "thu"))
                            {
                                try //Convert thu to tiff and move
                                {
                                    File.Move(graphicArray[i].ToString().Substring(0, graphicArray[i].ToString().Length - 3) + "thu", tempFilePath + ".tiff");
                                    MainForm.logFileName(Path.GetFileName(graphicArray[i].ToString().Substring(0, graphicArray[i].ToString().Length - 3) + "thu"),
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
                            if (File.Exists(graphicArray[i].ToString().Substring(0, graphicArray[i].ToString().Length - 3) + "tiff"))
                            {
                                try
                                {
                                    File.Move(graphicArray[i].ToString().Substring(0, graphicArray[i].ToString().Length - 3) + "tiff", tempFilePath + ".tiff");
                                    MainForm.logFileName(Path.GetFileName(graphicArray[i].ToString().Substring(0, graphicArray[i].ToString().Length - 3) + "tiff"),
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

                            #region Move TIF files(For VP-4400)
                            string TIF_filename = "s" + Path.GetFileName(graphicArray[i].ToString()).Substring(0, Path.GetFileName(graphicArray[i].ToString()).Length - 3) + "TIF";
                            if (File.Exists(Path.GetDirectoryName(graphicArray[i].ToString()) + "\\" + TIF_filename))
                            {
                                try
                                {
                                    File.Move(Path.GetDirectoryName(graphicArray[i].ToString()) + "\\" + TIF_filename, tempFilePath + ".tiff");
                                    MainForm.logFileName(TIF_filename, Path.GetFileName(tempFilePath + ".tiff"));
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

                        ownerForm.makeInvisibleMoveLable();

                        #region Move inf file(s)
                        for (int i = 0; i < infs.Length; i++)
                        {
                            try
                            {
                                File.Move(infs[i], destDir + @"\" + Path.GetFileName(infs[i]));
                                string infFileName = Path.GetFileName(infs[i]);
                                MainForm.logFileName(infFileName, infFileName);
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

                        if (deleteDir(sourceDir) == EndoResult.error)
                        { return EndoResult.error; }

                        return EndoResult.success;
                    }
                    break;
                #endregion

                #region OLYMPUS
                case "OLYMPUS":
                    string[] dirs = Directory.GetDirectories(sourceDir + @"\CV\STUDY", "*", SearchOption.TopDirectoryOnly);
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        #region Error check(Check whether ExamInfo.xml and JPEG files exist or not)
                        //Check whether ExamInfo.xml exist or not
                        if (!File.Exists(dirs[i] + "\\ExamInfo.xml"))
                        {
                            MessageBox.Show("[" + dirs[i] + "]ExamInfo.xml has not found.");
                            continue;
                        }

                        //Check whether JPEG files exist or not
                        if (Directory.GetFiles(sourceDir + @"\DCIM\" + Path.GetFileName(dirs[i]), "*.jpg", SearchOption.TopDirectoryOnly).Length == 0)
                        {
                            MessageBox.Show("[" + sourceDir + @"\DCIM\" + dirs[i] + "]JPEG files have not found.");
                            continue;
                        }
                        #endregion

                        testInfoArray = getPtInfoOlympus(dirs[i] + "\\ExamInfo.xml");
                        patientID = testInfoArray[3].ToString();
                        dateStr = testInfoArray[0].ToString().Replace("/", "");
                        tempArray = prepareToMove(testInfoArray, ownerForm);

                        if (tempArray[0] != "")
                        {
                            destDir = tempArray[0];
                            serialNo = tempArray[1];
                            patientID = tempArray[2];

                            #region Move jpg files
                            try { graphicFiles = Directory.GetFiles(sourceDir + @"\DCIM\" + Path.GetFileName(dirs[i]), "*.jpg", SearchOption.TopDirectoryOnly); }
                            #region catch
                            catch (DirectoryNotFoundException)
                            {
                                MessageBox.Show("[" + sourceDir + @"\DCIM\" + Path.GetFileName(dirs[i]) + "]" + Properties.Resources.FolderNotExist, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return EndoResult.error;
                            }
                            #endregion

                            graphicArray.Clear();
                            graphicArray.AddRange(graphicFiles);
                            graphicArray.Sort();
                            string tempFilePath;//ファイル移動用

                            MainForm.logTitle(sourceDir + @"\DCIM\" + Path.GetFileName(dirs[i]), destDir);
                            for (int j = 0; j < graphicArray.Count; j++)
                            {
                                #region Error check
                                if (MainForm.plusZero((j + 1).ToString(), 3) == "Error")
                                {
                                    MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return EndoResult.error;
                                }
                                #endregion

                                ownerForm.changeMoveLabelText("Now moving files [" + (j + 1).ToString() + "/" + graphicArray.Count.ToString() + "]");

                                tempFilePath = destDir + @"\" + patientID + "_" + dateStr + "_" + serialNo + "-" + MainForm.plusZero((j + 1).ToString(), 3);

                                try
                                {
                                    File.Move(graphicArray[j].ToString(), tempFilePath + ".jpg");
                                    MainForm.logFileName(Path.GetFileName(graphicArray[j].ToString()), Path.GetFileName(tempFilePath + ".jpg"));
                                }
                                #region catch
                                catch (IOException)
                                {
                                    MainForm.logSomething("[IO Exception]" + graphicArray[j].ToString() + " to " + tempFilePath + ".jpg");
                                    MessageBox.Show("[" + graphicArray[j].ToString() + " to " + tempFilePath + ".jpg]\r\n"
                                        + "[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                            ownerForm.makeInvisibleMoveLable();

                            #endregion

                            #region Move tiff files
                            graphicFiles = Directory.GetFiles(sourceDir + @"\DCIM\" + Path.GetFileName(dirs[i]), "*.tif", SearchOption.TopDirectoryOnly);
                            graphicArray.Clear();
                            graphicArray.AddRange(graphicFiles);
                            graphicArray.Sort();

                            for (int j = 0; j < graphicArray.Count; j++)
                            {
                                #region Error check
                                if (MainForm.plusZero((j + 1).ToString(), 3) == "Error")
                                {
                                    MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return EndoResult.error;
                                }
                                #endregion

                                ownerForm.changeMoveLabelText("Now moving files [" + (j + 1).ToString() + "/" + graphicArray.Count.ToString() + "]");

                                tempFilePath = destDir + @"\" + patientID + "_" + dateStr + "_" + serialNo + "-" + MainForm.plusZero((j + 1).ToString(), 3);

                                try
                                {
                                    File.Move(graphicArray[j].ToString(), tempFilePath + ".tif");
                                    MainForm.logFileName(Path.GetFileName(graphicArray[j].ToString()), Path.GetFileName(tempFilePath + ".tif"));
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

                            ownerForm.makeInvisibleMoveLable();
                            #endregion

                            if (deleteDir(sourceDir + @"\DCIM\" + Path.GetFileName(dirs[i])) == EndoResult.error)
                            {
                                MessageBox.Show("[" + sourceDir + @"\DCIM\" + Path.GetFileName(dirs[i]) + "]" + Properties.Resources.FailedToRemove, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            #region Move xml file
                            try
                            {
                                File.Move(dirs[i] + @"\ExamInfo.xml", destDir + @"\ExamInfo.xml");
                                MainForm.logFileName(dirs[i] + @"\ExamInfo.xml", destDir + @"\ExamInfo.xml");
                            }
                            #region catch
                            catch (IOException)
                            {
                                MainForm.logSomething("[IO Exception]Move: " + dirs[i] + @"\ExamInfo.xml to " + destDir + @"\ExamInfo.xml");
                                MessageBox.Show("[" + dirs[i] + @"\ExamInfo.xml to " + destDir + @"\ExamInfo.xml]\r\n" + "[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                            if (File.Exists(destDir + @"\ExamInfo.xml"))
                            { deleteDir(dirs[i], true); }
                            #endregion
                        }
                    }

                    #region Delete sourceDir
                    //Delete source folder after checking that empty.
                    if (Directory.GetFiles(sourceDir + @"\DCIM", "*", SearchOption.AllDirectories).Length == 0)
                    {
                        try
                        { Directory.Delete(sourceDir, true); }
                        catch (IOException)
                        {
                            MainForm.logSomething("[IO Exception]DELETE: " + sourceDir);
                            MessageBox.Show("DELETE: " + sourceDir + "\r\n" +
                                "[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return EndoResult.error;
                        }
                    }
                    #endregion

                    return EndoResult.success;
                #endregion

                #region MovedOLYMPUS
                case "MovedOLYMPUS":
                    #region Error check(Check whether ExamInfo.xml and JPEG files exist or not)
                    //Check whether ExamInfo.xml exist or not
                    if (!File.Exists(sourceDir + "\\ExamInfo.xml"))
                    {
                        MessageBox.Show("[" + sourceDir + "]ExamInfo.xml has not found.");
                        return EndoResult.skipped;
                    }

                    //Check whether JPEG files exist or not
                    if (Directory.GetFiles(sourceDir, "*.jpg", SearchOption.TopDirectoryOnly).Length == 0)
                    {
                        MessageBox.Show("["+sourceDir+"]JPEG files have not found.");
                        return EndoResult.skipped;
                    }
                    #endregion

                    testInfoArray = getPtInfoOlympus(sourceDir + "\\ExamInfo.xml");
                    patientID = testInfoArray[3].ToString();
                    dateStr = testInfoArray[0].ToString().Replace("/", "");
                    tempArray = prepareToMove(testInfoArray, ownerForm);

                    if (tempArray[0] != "")
                    {
                        destDir = tempArray[0];
                        serialNo = tempArray[1];
                        patientID = tempArray[2];

                        #region Move jpg files
                        try { graphicFiles = Directory.GetFiles(sourceDir, "*.jpg", SearchOption.TopDirectoryOnly); }
                        #region catch
                        catch (DirectoryNotFoundException)
                        {
                            MessageBox.Show("[" + sourceDir + "]" + Properties.Resources.FolderNotExist, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return EndoResult.error;
                        }
                        #endregion

                        graphicArray.Clear();
                        graphicArray.AddRange(graphicFiles);
                        graphicArray.Sort();
                        string tempFilePath;//ファイル移動用

                        MainForm.logTitle(sourceDir, destDir);
                        for (int j = 0; j < graphicArray.Count; j++)
                        {
                            #region Error check
                            if (MainForm.plusZero((j + 1).ToString(), 3) == "Error")
                            {
                                MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return EndoResult.error;
                            }
                            #endregion

                            ownerForm.changeMoveLabelText("Now moving files [" + (j + 1).ToString() + "/" + graphicArray.Count.ToString() + "]");

                            tempFilePath = destDir + @"\" + patientID + "_" + dateStr + "_" + serialNo + "-" + MainForm.plusZero((j + 1).ToString(), 3);

                            try
                            {
                                File.Move(graphicArray[j].ToString(), tempFilePath + ".jpg");
                                MainForm.logFileName(Path.GetFileName(graphicArray[j].ToString()), Path.GetFileName(tempFilePath + ".jpg"));
                            }
                            #region catch
                            catch (IOException)
                            {
                                MainForm.logSomething("[IO Exception]" + graphicArray[j].ToString() + " to " + tempFilePath + ".jpg");
                                MessageBox.Show("[" + graphicArray[j].ToString() + " to " + tempFilePath + ".jpg]\r\n"
                                    + "[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                        ownerForm.makeInvisibleMoveLable();
                        #endregion

                        #region Move tiff files
                        graphicFiles = Directory.GetFiles(sourceDir, "*.tif", SearchOption.TopDirectoryOnly);
                        graphicArray.Clear();
                        graphicArray.AddRange(graphicFiles);
                        graphicArray.Sort();

                        for (int j = 0; j < graphicArray.Count; j++)
                        {
                            #region Error check
                            if (MainForm.plusZero((j + 1).ToString(), 3) == "Error")
                            {
                                MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return EndoResult.error;
                            }
                            #endregion

                            ownerForm.changeMoveLabelText("Now moving files [" + (j + 1).ToString() + "/" + graphicArray.Count.ToString() + "]");

                            tempFilePath = destDir + @"\" + patientID + "_" + dateStr + "_" + serialNo + "-" + MainForm.plusZero((j + 1).ToString(), 3);

                            try
                            {
                                File.Move(graphicArray[j].ToString(), tempFilePath + ".tif");
                                MainForm.logFileName(Path.GetFileName(graphicArray[j].ToString()), Path.GetFileName(tempFilePath + ".tif"));
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

                        ownerForm.makeInvisibleMoveLable();
                        #endregion

                        #region Move xml file
                        try
                        {
                            File.Move(sourceDir + @"\ExamInfo.xml", destDir + @"\ExamInfo.xml");
                            MainForm.logFileName("ExamInfo.xml", "ExamInfo.xml");
                        }
                        #region catch
                        catch (IOException)
                        {
                            MainForm.logSomething("[IO Exception]Move: " + sourceDir + @"\ExamInfo.xml to " + destDir + @"\ExamInfo.xml");
                            MessageBox.Show("[" + sourceDir + @"\ExamInfo.xml to " + destDir + @"\ExamInfo.xml]\r\n" + "[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


                        if (deleteDir(sourceDir) == EndoResult.error)
                        {
                            MessageBox.Show("[" + sourceDir + "]" + Properties.Resources.FailedToRemove, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("[" + sourceDir + "]" + Properties.Resources.UnsupportedFolderType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return EndoResult.failed;
                    }
                    return EndoResult.success;
                #endregion

                #region Empty
                case "Empty":
                    deleteDir(sourceDir);
                    return EndoResult.success;
                #endregion

                #region Only Thumbs.db
                case "Only Thumbs.db":
                    deleteDir(sourceDir, true);
                    return EndoResult.success;
                #endregion

                #region Unknown
                case "Unknown":
                    MessageBox.Show(Properties.Resources.UnsupportedFolderType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return EndoResult.failed;
                    #endregion
            }
            return EndoResult.failed;
        }

        /// <summary>
        /// Prepare to file move and return destination of file moving, serial number
        /// </summary>
        /// <param name="testInfoArray"></param>
        /// <param name="ownerForm"></param>
        /// <returns>Destination of file moving, serial number</returns>
        public static string[] prepareToMove(string[] testInfoArray, Form ownerForm)
        {
            string[] ret = { "", "", "" }; //destDir, serialNo, patientID

            string serialNo = "";
            string patientID = testInfoArray[3].ToString();
            string dateStr = testInfoArray[0].ToString();

            //Show dialog and display ID, date, name, time. User can change ID and examination date with the dialog.
            #region set exam info
            SetExamInfo sei = new SetExamInfo(patientID, dateStr, testInfoArray[4].ToString(), testInfoArray[1].ToString(), testInfoArray[2].ToString());
            sei.Owner = ownerForm;
            sei.ShowDialog();
            if (sei.OkCancel == "Cancel")
            { return ret; }
            patientID = sei.patientId;
            sei.Dispose();
            #endregion

            dateStr = dateStr.Replace("/", "");
            MainForm.createFolder(Settings.imgDir + @"\" + patientID);

            serialNo = MainForm.getSerialNo(Settings.imgDir + @"\" + patientID, patientID, dateStr);
            if (serialNo == "error")
            {
                MessageBox.Show(Properties.Resources.SerialNoOver999, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ret;
            }
            MainForm.createFolder(Settings.imgDir + @"\" + patientID + @"\" + patientID + "_" + dateStr + "_" + serialNo);
            ret[0] = Settings.imgDir + @"\" + patientID + @"\" + patientID + "_" + dateStr + "_" + serialNo; //destDir
            ret[1] = serialNo;
            ret[2] = patientID;
            return ret;
        }

        /// <summary>
        /// Delete source folder after checking that empty.
        /// </summary>
        /// <param name="dir">Directory to delete</param>
        /// <param name="forceToDelete">Set true when you need to delete regardless of file existence</param>
        /// <returns>EndoResult</returns>
        public static EndoResult deleteDir(string dir, Boolean forceToDelete = false)
        {
            if (Directory.GetFiles(dir, "*").Length == 0 || forceToDelete)
            {
                try
                { Directory.Delete(dir, true); }
                catch (IOException)
                {
                    MessageBox.Show("[IO Exception]" + Properties.Resources.HasOccurred, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return EndoResult.error;
                }
                return EndoResult.success;
            }
            else
            {
                return EndoResult.fileExist;
            }
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

        public static string getValueOfTag(string text, string tag)
        {
            int index_from;
            int index_to;
            index_from = text.IndexOf("<" + tag + ">");
            index_to = text.IndexOf("</" + tag + ">");
            if (index_from == -1)
            {
                MessageBox.Show(Properties.Resources.UnsupportedFileType, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            else
            { return text.Substring(index_from + 2 + tag.Length, index_to - index_from - 2 - tag.Length); }
        }

        public static bool isTimeEmpty(string str)
        {
            str = str.Replace(':', ' ');
            return String.IsNullOrWhiteSpace(str);
        }
    }
}
