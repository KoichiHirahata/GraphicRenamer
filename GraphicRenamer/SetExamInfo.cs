using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace GraphicRenamer
{
    public partial class SetExamInfo : Form
    {
        public string OkCancel;
        public string patientId = "";

        public SetExamInfo(string ptID, string examDate, string ptName, string startTime, string endTime)
        {
            InitializeComponent();
            tbPtId.Text = ptID;
            monthCalendar1.SetDate(DateTime.Parse(examDate));
            lbPtName.Text = ptName;
            lbStartTime.Text = startTime;
            lbEndTime.Text = endTime;

            if(Settings.useFeDB)
            {
                lbNameDbCaption.Visible = true;
                lbPtNameDB.Text = "";
                lbPtNameDB.Visible = true;
                readPtData(ptID);
            }
            else
            {
                lbNameDbCaption.Visible = false;
                lbPtNameDB.Visible = false;
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            OkCancel = "Cancel";
            this.Close();
        }

        private void btOK_Click(object sender, EventArgs e)
        { exeBtOK(); }

        private void tbPtId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            { exeBtOK(); }
        }

        private void exeBtOK()
        {
            if (tbPtId.Text.Length == 0)
            {
                MessageBox.Show(Properties.Resources.NoID, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OkCancel = "OK";
            patientId = tbPtId.Text;
            this.Close();
        }

        #region ReadPatientData
        private void tbPtId_KeyUp(object sender, KeyEventArgs e)
        {
            if (Settings.useFeDB)
            { readPtData(tbPtId.Text); }
        }

        public void readPtData(string patientID)
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
                return;
            }

            try
            { conn.Open(); }
            catch (NpgsqlException)
            {
                MessageBox.Show(Properties.Resources.CouldntOpenConn, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
                return;
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show(Properties.Resources.ConnClosed, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conn.Close();
                return;
            }
            #endregion

            string sql = "SELECT * FROM patient WHERE pt_id='" + patientID + "'";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                conn.Close();
                lbPtNameDB.Text = "No data";
                return;
            }
            else
            {
                DataRow row = dt.Rows[0];
                lbPtNameDB.Text = row["pt_name"].ToString();
                conn.Close();
            }
        }
        #endregion
    }
}
