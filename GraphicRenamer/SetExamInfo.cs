using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace GraphicRenamer
{
    public partial class SetExamInfo : Form
    {
        public string OkCancel;
        public string patientId = "";
        public string dateStr = "";

        public SetExamInfo(string ptID, string examDate, string ptName, string startTime, string endTime)
        {
            InitializeComponent();
            tbPtId.Text = ptID;
            monthCalendar1.SetDate(DateTime.Parse(examDate));
            lbPtName.Text = ptName;
            lbStartTime.Text = startTime;
            lbEndTime.Text = endTime;

            if (Settings.useFeDB||Settings.usePlugin)
            {
                lbNameDbCaption.Visible = true;
                lbPtNameDB.Text = "";
                lbPtNameDB.Visible = true;
                lbPtNameDB.Text = db.GetPtName(ptID);
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
            if (String.IsNullOrWhiteSpace(tbPtId.Text))
            {
                MessageBox.Show(Properties.Resources.NoID, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OkCancel = "OK";
            patientId = tbPtId.Text;
            dateStr = monthCalendar1.SelectionStart.ToString("yyyyMMdd");
            Close();
        }

        #region ReadPatientData
        private void tbPtId_KeyUp(object sender, KeyEventArgs e)
        {
            if (Settings.useFeDB||Settings.usePlugin)
            {
                lbPtNameDB.Text = db.GetPtName(tbPtId.Text);
            }
        }
        #endregion
    }
}
