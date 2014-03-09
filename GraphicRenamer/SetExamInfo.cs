using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicRenamer
{
    public partial class SetExamInfo : Form
    {
        public string OkCancel;
        public string patientId = "";
        public string examinationDate = "";

        public SetExamInfo(string ptID, string examDate, string ptName, string startTime, string endTime)
        {
            InitializeComponent();
            tbPtId.Text = ptID;
            monthCalendar1.SetDate(DateTime.Parse(examDate));
            lbPtName.Text = ptName;
            lbStartTime.Text = startTime;
            lbEndTime.Text = endTime;
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
            examinationDate = monthCalendar1.SelectionStart.ToString("yyyyMMdd");
            this.Close();
        }
    }
}
