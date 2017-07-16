using System;
using System.Data;
using System.Windows.Forms;
using System.IO;
using Npgsql;

namespace GraphicRenamer
{
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();

            if (File.Exists(Application.StartupPath + "\\settings.config"))
            {
                Settings.readSettings();
                cbUseFeSrv.Checked = Settings.useFeDB;
                tbDBSrv.Text = Settings.DBSrvIP;
                tbDBsrvPort.Text = Settings.DBSrvPort;
                tbDbID.Text = Settings.DBconnectID;
                tbDBpw.Text = Settings.DBconnectPw;
                cbUsePlugin.Checked = Settings.usePlugin;
                tbPluginLocation.Text = Settings.ptInfoPlugin;

                if (Settings.imgDir.Length == 0)
                { tbSaveDir.Text = "(" + Properties.Resources.NotConfigured + ")"; }
                else
                { this.tbSaveDir.Text = Settings.imgDir; }
            }
            else
            { tbSaveDir.Text = "(" + Properties.Resources.InitialSetting + ")"; }

            setFeDbPropertyVisibleOrNot();
            setPluginPropertyVisibleOrNot();
            ActiveControl = btSave;
        }

        #region Buttons
        private void sansho_Bt_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog(); //余計なことしない方がよさそう。
            fbd.SelectedPath = Application.StartupPath;
            if (fbd.ShowDialog() == DialogResult.OK)
            { tbSaveDir.Text = fbd.SelectedPath; }
        }

        private void btCancel_Click(object sender, EventArgs e)
        { this.Close(); }

        private void btSave_Click(object sender, EventArgs e)
        { saveProcedure(); }

        private void tbSaveDir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && cbUseFeSrv.Checked == false)
            { saveProcedure(); }
        }

        private void saveProcedure()
        {
            #region ErrorCheck
            if ((tbSaveDir.Text == "(" + Properties.Resources.InitialSetting + ")") || (tbSaveDir.Text == "(" + Properties.Resources.NotConfigured + ")"))
            {
                MessageBox.Show("[Save to:]" + Properties.Resources.NotConfigured, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(tbSaveDir.Text))
            {
                MessageBox.Show("[Save to:]" + Properties.Resources.FolderNotExist, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cbUsePlugin.Checked && !File.Exists(tbPluginLocation.Text))
            {
                MessageBox.Show("[Plugin:]" + Properties.Resources.FileNotExist, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            if (cbUseFeSrv.Checked)
            {
                if (db.testConnect(tbDBSrv.Text, tbDBsrvPort.Text, tbDbID.Text, (tbDBpw.Visible) ? tbDBpw.Text : Settings.DBconnectPw))
                {
                    Settings.imgDir = tbSaveDir.Text;
                    Settings.useFeDB = cbUseFeSrv.Checked;
                    Settings.DBSrvIP = tbDBSrv.Text;
                    Settings.DBSrvPort = tbDBsrvPort.Text;
                    Settings.DBconnectID = tbDbID.Text;
                    if (tbDBpw.Visible)
                    { Settings.DBconnectPw = tbDBpw.Text; }
                    Settings.usePlugin = cbUsePlugin.Checked;

                    Settings.saveSettings();
                    Close();
                }
            }
            else
            {
                Settings.imgDir = tbSaveDir.Text;
                Settings.useFeDB = cbUseFeSrv.Checked;
                Settings.usePlugin = cbUsePlugin.Checked;
                Settings.ptInfoPlugin = tbPluginLocation.Text;

                Settings.saveSettings();
                Close();
            }
        }

        private void btPwSet_Click(object sender, EventArgs e)
        {
            tbDBpw.Visible = true;
            tbDBpw.Focus();
        }

        private void btBrowsePlugin_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (!Directory.Exists(Application.StartupPath + "\\plugin"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\plugin");
            }
            ofd.InitialDirectory = Application.StartupPath + "\\plugin";
            ofd.FilterIndex = 1;

            if (ofd.ShowDialog() == DialogResult.OK)
            { tbPluginLocation.Text = ofd.FileName; }
        }
        #endregion

        private void cbUseFeSrv_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUseFeSrv.Checked)
            { cbUsePlugin.Checked = false; }
            setFeDbPropertyVisibleOrNot();
        }

        private void setFeDbPropertyVisibleOrNot()
        {
            lbDBSrv.Visible = cbUseFeSrv.Checked;
            tbDBSrv.Visible = cbUseFeSrv.Checked;
            lbSrvSample.Visible = cbUseFeSrv.Checked;
            lbDBsrvPort.Visible = cbUseFeSrv.Checked;
            tbDBsrvPort.Visible = cbUseFeSrv.Checked;
            lbDbID.Visible = cbUseFeSrv.Checked;
            tbDbID.Visible = cbUseFeSrv.Checked;
            pwState.Visible = cbUseFeSrv.Checked;
            btPwSet.Visible = cbUseFeSrv.Checked;
            btTestConnect.Visible = cbUseFeSrv.Checked;
        }

        private void cbUsePlugin_CheckedChanged(object sender, EventArgs e)
        {
            if (cbUsePlugin.Checked)
            { cbUseFeSrv.Checked = false; }
            setPluginPropertyVisibleOrNot();
        }

        private void setPluginPropertyVisibleOrNot()
        {
            tbPluginLocation.Visible = cbUsePlugin.Checked;
            btBrowsePlugin.Visible = cbUsePlugin.Checked;
        }

        private void btTestConnect_Click(object sender, EventArgs e)
        {
            if (db.testConnect(tbDBSrv.Text, tbDBsrvPort.Text, tbDbID.Text, (tbDBpw.Visible) ? tbDBpw.Text : Settings.DBconnectPw))
            { MessageBox.Show(Properties.Resources.ConnectSuccess, "Successfully connected.", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }
    }
}