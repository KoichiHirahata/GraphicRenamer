using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace GraphicRenamer
{
    public partial class Search : Form
    {
        public string ptId { get; set; } = null;
        private List<SimplePtInfo> patients { get; set; }

        public Search()
        {
            InitializeComponent();

            dgvPatients.RowHeadersVisible = false;
            dgvPatients.MultiSelect = false;
            dgvPatients.Font = new Font(dgvPatients.Font.Name, 12);
            dgvPatients.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            try
            {
                #region Error check
                if (String.IsNullOrWhiteSpace(tbSearchStr.Text))
                {
                    return;
                }
                #endregion

                string command = Settings.ptInfoPlugin;

                ProcessStartInfo psInfo = new ProcessStartInfo();

                psInfo.FileName = command;
                psInfo.Arguments = "/k \"" + tbSearchStr.Text + "\"";
                psInfo.CreateNoWindow = true; // Do not open console window
                psInfo.UseShellExecute = false; // Do not use shell

                psInfo.RedirectStandardOutput = true;

                Process p = Process.Start(psInfo);
                string output = p.StandardOutput.ReadToEnd();

                output = output.Replace("\r\r\n", "\n"); // Replace new line code

                if (String.IsNullOrWhiteSpace(output))
                {
                    dgvPatients.DataSource = null;
                }
                else
                {
                    patients = FunctionsForSearch.GetListOfSimplePtInfo(output);
                    dgvPatients.DataSource = patients;

                    #region Change header text
                    dgvPatients.Columns["ptId"].HeaderText = "ID";
                    dgvPatients.Columns["ptName"].HeaderText = "Name";
                    #endregion

                    DataGridViewButtonColumn btSelect = new DataGridViewButtonColumn(); //Create DataGridViewButtonColumn
                    btSelect.Name = "btSelect";  //Set column name
                    btSelect.UseColumnTextForButtonValue = true; //Display button text
                    btSelect.Text = "Select";  //Set button text
                    dgvPatients.Columns.Add(btSelect); //Add button
                }
            }
            #region catch
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }

        public class SimplePtInfo
        {
            public string ptId { get; set; }
            public string ptName { get; set; }
        }

        private void dgvPatients_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dgv = (DataGridView)sender;

                if (dgv.Columns[e.ColumnIndex].Name == "btSelect")
                {
                    ptId = dgv.Rows[e.RowIndex].Cells["ptId"].Value.ToString();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public static class FunctionsForSearch
    {
        public static List<Search.SimplePtInfo> GetListOfSimplePtInfo(string _str)
        {
            try
            {
                List<Search.SimplePtInfo> ret = new List<Search.SimplePtInfo>();

                StringReader sr = new StringReader(_str);

                string tempStr = null;
                string tempId = null;
                string tempName = null;

                //ストリームの末端まで繰り返す
                while (sr.Peek() > -1)
                {
                    tempStr = sr.ReadLine();

                    if (tempStr.IndexOf("Patient ID:") != -1)
                    {
                        tempId = tempStr.Substring(("Patient ID:").Length);
                        //MessageBox.Show(tempStr + "\r\n" + tempStr.IndexOf("Patient ID:").ToString() + "\r\n" + tempId);
                    }
                    else if (tempStr.IndexOf("Patient Name:") != -1)
                    {
                        tempName = tempStr.Substring(("Patient Name:").Length);

                        ret.Add(new Search.SimplePtInfo
                        {
                            ptId = tempId,
                            ptName = tempName
                        });

                        tempId = null;
                        tempName = null;
                    }
                }

                sr.Close();

                if (ret.Count == 0)
                {
                    return null;
                }
                else
                {
                    return ret;
                }
            }
            #region catch
            catch (Exception ex)
            {
                MessageBox.Show("[GetListOfSimplePtInfo] " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            #endregion
        }
    }
}
