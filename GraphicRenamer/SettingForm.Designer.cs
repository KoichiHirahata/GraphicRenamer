namespace GraphicRenamer
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.tbSaveDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.btSave = new System.Windows.Forms.Button();
            this.sansho_Bt = new System.Windows.Forms.Button();
            this.tbDBsrvPort = new System.Windows.Forms.TextBox();
            this.lbDBsrvPort = new System.Windows.Forms.Label();
            this.tbDbID = new System.Windows.Forms.TextBox();
            this.lbDbID = new System.Windows.Forms.Label();
            this.btTestConnect = new System.Windows.Forms.Button();
            this.btPwSet = new System.Windows.Forms.Button();
            this.tbDBpw = new System.Windows.Forms.TextBox();
            this.pwState = new System.Windows.Forms.Label();
            this.lbSrvSample = new System.Windows.Forms.Label();
            this.lbDBSrv = new System.Windows.Forms.Label();
            this.tbDBSrv = new System.Windows.Forms.TextBox();
            this.cbUseFeSrv = new System.Windows.Forms.CheckBox();
            this.cbUsePlugin = new System.Windows.Forms.CheckBox();
            this.tbPluginLocation = new System.Windows.Forms.TextBox();
            this.btBrowsePlugin = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbSaveDir
            // 
            resources.ApplyResources(this.tbSaveDir, "tbSaveDir");
            this.tbSaveDir.Name = "tbSaveDir";
            this.tbSaveDir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSaveDir_KeyDown);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btCancel
            // 
            resources.ApplyResources(this.btCancel, "btCancel");
            this.btCancel.Name = "btCancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btSave
            // 
            resources.ApplyResources(this.btSave, "btSave");
            this.btSave.Name = "btSave";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // sansho_Bt
            // 
            resources.ApplyResources(this.sansho_Bt, "sansho_Bt");
            this.sansho_Bt.Name = "sansho_Bt";
            this.sansho_Bt.UseVisualStyleBackColor = true;
            this.sansho_Bt.Click += new System.EventHandler(this.sansho_Bt_Click);
            // 
            // tbDBsrvPort
            // 
            resources.ApplyResources(this.tbDBsrvPort, "tbDBsrvPort");
            this.tbDBsrvPort.Name = "tbDBsrvPort";
            // 
            // lbDBsrvPort
            // 
            resources.ApplyResources(this.lbDBsrvPort, "lbDBsrvPort");
            this.lbDBsrvPort.Name = "lbDBsrvPort";
            // 
            // tbDbID
            // 
            resources.ApplyResources(this.tbDbID, "tbDbID");
            this.tbDbID.Name = "tbDbID";
            // 
            // lbDbID
            // 
            resources.ApplyResources(this.lbDbID, "lbDbID");
            this.lbDbID.Name = "lbDbID";
            // 
            // btTestConnect
            // 
            resources.ApplyResources(this.btTestConnect, "btTestConnect");
            this.btTestConnect.Name = "btTestConnect";
            this.btTestConnect.UseVisualStyleBackColor = true;
            this.btTestConnect.Click += new System.EventHandler(this.btTestConnect_Click);
            // 
            // btPwSet
            // 
            resources.ApplyResources(this.btPwSet, "btPwSet");
            this.btPwSet.Name = "btPwSet";
            this.btPwSet.UseVisualStyleBackColor = true;
            this.btPwSet.Click += new System.EventHandler(this.btPwSet_Click);
            // 
            // tbDBpw
            // 
            resources.ApplyResources(this.tbDBpw, "tbDBpw");
            this.tbDBpw.Name = "tbDBpw";
            // 
            // pwState
            // 
            resources.ApplyResources(this.pwState, "pwState");
            this.pwState.Name = "pwState";
            // 
            // lbSrvSample
            // 
            resources.ApplyResources(this.lbSrvSample, "lbSrvSample");
            this.lbSrvSample.Name = "lbSrvSample";
            // 
            // lbDBSrv
            // 
            resources.ApplyResources(this.lbDBSrv, "lbDBSrv");
            this.lbDBSrv.Name = "lbDBSrv";
            // 
            // tbDBSrv
            // 
            resources.ApplyResources(this.tbDBSrv, "tbDBSrv");
            this.tbDBSrv.Name = "tbDBSrv";
            // 
            // cbUseFeSrv
            // 
            resources.ApplyResources(this.cbUseFeSrv, "cbUseFeSrv");
            this.cbUseFeSrv.Name = "cbUseFeSrv";
            this.cbUseFeSrv.UseVisualStyleBackColor = true;
            this.cbUseFeSrv.CheckedChanged += new System.EventHandler(this.cbUseFeSrv_CheckedChanged);
            // 
            // cbUsePlugin
            // 
            resources.ApplyResources(this.cbUsePlugin, "cbUsePlugin");
            this.cbUsePlugin.Name = "cbUsePlugin";
            this.cbUsePlugin.UseVisualStyleBackColor = true;
            this.cbUsePlugin.CheckedChanged += new System.EventHandler(this.cbUsePlugin_CheckedChanged);
            // 
            // tbPluginLocation
            // 
            resources.ApplyResources(this.tbPluginLocation, "tbPluginLocation");
            this.tbPluginLocation.Name = "tbPluginLocation";
            // 
            // btBrowsePlugin
            // 
            resources.ApplyResources(this.btBrowsePlugin, "btBrowsePlugin");
            this.btBrowsePlugin.Name = "btBrowsePlugin";
            this.btBrowsePlugin.UseVisualStyleBackColor = true;
            this.btBrowsePlugin.Click += new System.EventHandler(this.btBrowsePlugin_Click);
            // 
            // SettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbPluginLocation);
            this.Controls.Add(this.btBrowsePlugin);
            this.Controls.Add(this.cbUsePlugin);
            this.Controls.Add(this.cbUseFeSrv);
            this.Controls.Add(this.tbDBsrvPort);
            this.Controls.Add(this.lbDBsrvPort);
            this.Controls.Add(this.tbDbID);
            this.Controls.Add(this.lbDbID);
            this.Controls.Add(this.btTestConnect);
            this.Controls.Add(this.btPwSet);
            this.Controls.Add(this.tbDBpw);
            this.Controls.Add(this.pwState);
            this.Controls.Add(this.lbSrvSample);
            this.Controls.Add(this.lbDBSrv);
            this.Controls.Add(this.tbDBSrv);
            this.Controls.Add(this.tbSaveDir);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.sansho_Bt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSaveDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button sansho_Bt;
        private System.Windows.Forms.TextBox tbDBsrvPort;
        private System.Windows.Forms.Label lbDBsrvPort;
        private System.Windows.Forms.TextBox tbDbID;
        private System.Windows.Forms.Label lbDbID;
        private System.Windows.Forms.Button btTestConnect;
        private System.Windows.Forms.Button btPwSet;
        private System.Windows.Forms.TextBox tbDBpw;
        private System.Windows.Forms.Label pwState;
        private System.Windows.Forms.Label lbSrvSample;
        private System.Windows.Forms.Label lbDBSrv;
        private System.Windows.Forms.TextBox tbDBSrv;
        private System.Windows.Forms.CheckBox cbUseFeSrv;
        private System.Windows.Forms.CheckBox cbUsePlugin;
        private System.Windows.Forms.TextBox tbPluginLocation;
        private System.Windows.Forms.Button btBrowsePlugin;
    }
}