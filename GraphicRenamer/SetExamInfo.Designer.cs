namespace GraphicRenamer
{
    partial class SetExamInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetExamInfo));
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.tbPtId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbPtName = new System.Windows.Forms.Label();
            this.lbStartTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbEndTime = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.lbPtNameDB = new System.Windows.Forms.Label();
            this.lbNameDbCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // monthCalendar1
            // 
            resources.ApplyResources(this.monthCalendar1, "monthCalendar1");
            this.monthCalendar1.Name = "monthCalendar1";
            // 
            // tbPtId
            // 
            resources.ApplyResources(this.tbPtId, "tbPtId");
            this.tbPtId.Name = "tbPtId";
            this.tbPtId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPtId_KeyDown);
            this.tbPtId.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbPtId_KeyUp);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lbPtName
            // 
            resources.ApplyResources(this.lbPtName, "lbPtName");
            this.lbPtName.Name = "lbPtName";
            // 
            // lbStartTime
            // 
            resources.ApplyResources(this.lbStartTime, "lbStartTime");
            this.lbStartTime.Name = "lbStartTime";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lbEndTime
            // 
            resources.ApplyResources(this.lbEndTime, "lbEndTime");
            this.lbEndTime.Name = "lbEndTime";
            // 
            // btCancel
            // 
            resources.ApplyResources(this.btCancel, "btCancel");
            this.btCancel.Name = "btCancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btOK
            // 
            resources.ApplyResources(this.btOK, "btOK");
            this.btOK.Name = "btOK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // lbPtNameDB
            // 
            resources.ApplyResources(this.lbPtNameDB, "lbPtNameDB");
            this.lbPtNameDB.Name = "lbPtNameDB";
            // 
            // lbNameDbCaption
            // 
            resources.ApplyResources(this.lbNameDbCaption, "lbNameDbCaption");
            this.lbNameDbCaption.Name = "lbNameDbCaption";
            // 
            // SetExamInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbPtNameDB);
            this.Controls.Add(this.lbNameDbCaption);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.lbEndTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbStartTime);
            this.Controls.Add(this.lbPtName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPtId);
            this.Controls.Add(this.monthCalendar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetExamInfo";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.TextBox tbPtId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbPtName;
        private System.Windows.Forms.Label lbStartTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbEndTime;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Label lbPtNameDB;
        private System.Windows.Forms.Label lbNameDbCaption;
    }
}