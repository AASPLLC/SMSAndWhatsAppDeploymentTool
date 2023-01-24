namespace SMSAndWhatsAppDeploymentTool
{
    partial class ChooseDBType
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
            this.cosmosBTN = new System.Windows.Forms.Button();
            this.dataverseBTN = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // cosmosBTN
            // 
            this.cosmosBTN.Location = new System.Drawing.Point(289, 79);
            this.cosmosBTN.Name = "cosmosBTN";
            this.cosmosBTN.Size = new System.Drawing.Size(271, 40);
            this.cosmosBTN.TabIndex = 0;
            this.cosmosBTN.Text = "Cosmos DB";
            this.cosmosBTN.UseVisualStyleBackColor = true;
            this.cosmosBTN.Click += new System.EventHandler(this.CosmosBTN_Click);
            // 
            // dataverseBTN
            // 
            this.dataverseBTN.Location = new System.Drawing.Point(12, 79);
            this.dataverseBTN.Name = "dataverseBTN";
            this.dataverseBTN.Size = new System.Drawing.Size(271, 40);
            this.dataverseBTN.TabIndex = 1;
            this.dataverseBTN.Text = "Dataverse (default)";
            this.dataverseBTN.UseVisualStyleBackColor = true;
            this.dataverseBTN.Click += new System.EventHandler(this.DataverseBTN_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose  your database type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(542, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "The database will be used to store chat history and will need to have periodic ro" +
    "tated keys for security.";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(174, 43);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(221, 15);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Click here for Database Type Information";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1_LinkClicked);
            // 
            // ChooseDBType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 136);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataverseBTN);
            this.Controls.Add(this.cosmosBTN);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseDBType";
            this.Text = "Choose Database Type";
            this.Load += new System.EventHandler(this.ChooseDBType_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button cosmosBTN;
        private Button dataverseBTN;
        private Label label1;
        private Label label2;
        private LinkLabel linkLabel1;
    }
}