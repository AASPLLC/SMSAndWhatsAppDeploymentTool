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
            cosmosBTN = new Button();
            dataverseBTN = new Button();
            label1 = new Label();
            label2 = new Label();
            linkLabel1 = new LinkLabel();
            SuspendLayout();
            // 
            // cosmosBTN
            // 
            cosmosBTN.Location = new Point(289, 68);
            cosmosBTN.Name = "cosmosBTN";
            cosmosBTN.Size = new Size(271, 40);
            cosmosBTN.TabIndex = 0;
            cosmosBTN.Text = "Cosmos DB";
            cosmosBTN.UseVisualStyleBackColor = true;
            cosmosBTN.Click += CosmosBTN_Click;
            // 
            // dataverseBTN
            // 
            dataverseBTN.Location = new Point(12, 68);
            dataverseBTN.Name = "dataverseBTN";
            dataverseBTN.Size = new Size(271, 40);
            dataverseBTN.TabIndex = 1;
            dataverseBTN.Text = "Dataverse (default)";
            dataverseBTN.UseVisualStyleBackColor = true;
            dataverseBTN.Click += DataverseBTN_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 50);
            label1.Name = "label1";
            label1.Size = new Size(153, 15);
            label1.TabIndex = 2;
            label1.Text = "Choose your database type:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 35);
            label2.Name = "label2";
            label2.Size = new Size(542, 15);
            label2.TabIndex = 3;
            label2.Text = "The database will be used to store chat history and will need to have periodic rotated keys for security.";
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(12, 9);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(221, 15);
            linkLabel1.TabIndex = 5;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Click here for Database Type Information";
            linkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // ChooseDBType
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(574, 118);
            Controls.Add(linkLabel1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dataverseBTN);
            Controls.Add(cosmosBTN);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseDBType";
            Text = "Choose Database Type";
            FormClosing += DBTypeForm_Closing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button cosmosBTN;
        private Button dataverseBTN;
        private Label label1;
        private Label label2;
        private LinkLabel linkLabel1;
    }
}