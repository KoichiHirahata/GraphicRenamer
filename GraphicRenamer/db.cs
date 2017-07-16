using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Npgsql;
using System.Windows.Forms;

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
        }
    }
}
