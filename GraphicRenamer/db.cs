using System;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Npgsql;

namespace GraphicRenamer
{
    public static class db
    {
        public static string retConnStr()
        {
            return "Host=" + Settings.DBSrvIP + ";Port=" + Settings.DBSrvPort + ";Username="
                + Settings.DBconnectID + ";Password=" + Settings.DBconnectPw + ";Database="
                + Settings.DBname + ";" + Settings.sslSetting;
        }

        public static Boolean testConnect(string srvIP, string srvPort, string u_name, string u_pw)
        {
            try
            {
                using (var conn = new NpgsqlConnection("Host=" + srvIP + ";Port=" + srvPort
                    + ";Username=" + u_name + ";Password=" + u_pw
                    + ";Database=" + Settings.DBname + ";" + Settings.sslSetting))
                {
                    try
                    { conn.Open(); }
                    catch (NpgsqlException npe)
                    {
                        MessageBox.Show("接続できませんでした。[NpgsqlException]\r\n" + npe.Message, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                        return false;
                    }
                    catch (IOException ioe)
                    {
                        MessageBox.Show("接続が切断されました\r\n" + ioe.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                        return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                        return false;
                    }
                    finally
                    {
                        conn.Close();
                    }
                    return true;
                }
            }
            #region catch
            catch (ArgumentException ae)
            {
                MessageBox.Show(Properties.Resources.WrongConnectingString + "\r\n" + ae.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            #endregion
        }

        #region Get Patient Name
        public static string GetPtName(string _ptId)
        {
            #region Error check
            if (Settings.IdDigit != null)
            {
                if (_ptId.Length != Settings.IdDigit)
                {
                    return null;
                }
            }
            #endregion

            if (Settings.useFeDB)
            {
                return readPtDataUsingFe(_ptId);
            }
            else if (Settings.usePlugin)
            {
                return readPtDataUsingPlugin(_ptId);
            }
            else
            {
                return null;
            }
        }

        public static string readPtDataUsingFe(string patientID)
        {
            try
            {
                using (var conn = new NpgsqlConnection(db.retConnStr()))
                {
                    try
                    { conn.Open(); }
                    catch (NpgsqlException npe)
                    {
                        MessageBox.Show(Properties.Resources.CouldntOpenConn + "\r\n" + npe.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                        return null;
                    }
                    catch (IOException ioe)
                    {
                        MessageBox.Show(Properties.Resources.ConnClosed + "\r\n" + ioe.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                        return null;
                    }

                    if (conn.State == ConnectionState.Open)
                    {

                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "select * from get_pt_info_without_login(@p_id)";
                            cmd.Parameters.AddWithValue("p_id", patientID);

                            try
                            {
                                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                da.Fill(dt);

                                conn.Close();

                                if (dt.Rows.Count == 0)
                                {
                                    return "No data";
                                }
                                else
                                {
                                    DataRow row = dt.Rows[0];
                                    return (String.IsNullOrWhiteSpace(row["pt_name"].ToString())) ? "No data" : row["pt_name"].ToString();
                                }
                            }
                            catch (NpgsqlException nex)
                            {
                                MessageBox.Show("[NpgsqlException]\r\n" + nex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                conn.Close();
                                return null;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.CouldntOpenConn, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        conn.Close();
                        return null;
                    }
                }
            }
            #region catch
            catch (ArgumentException ae)
            {
                MessageBox.Show(Properties.Resources.WrongConnectingString + "\r\n" + ae.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            #endregion
        }

        public static string readPtDataUsingPlugin(string patienID)
        {
            string command = Settings.ptInfoPlugin;

            ProcessStartInfo psInfo = new ProcessStartInfo();

            psInfo.FileName = command;
            psInfo.Arguments = patienID;
            psInfo.CreateNoWindow = true; // Do not open console window
            psInfo.UseShellExecute = false; // Do not use shell

            psInfo.RedirectStandardOutput = true;

            Process p = Process.Start(psInfo);
            string output = p.StandardOutput.ReadToEnd();

            output = output.Replace("\r\r\n", "\n"); // Replace new line code

            if (String.IsNullOrWhiteSpace(output))
            { return "No data"; }
            else if (!Regex.IsMatch(output, "Patient Name:"))
            {
                return "No data";
                //lbPtName.Text = output;
            }
            else
            { return file_control.readItemSettingFromText(output, "Patient Name:"); }
        }
        #endregion
    }
}
