namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    partial class APIRegistration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(APIRegistration));
            NextBTN = new Button();
            BackBTN = new Button();
            OutputRT = new RichTextBox();
            label1 = new Label();
            label2 = new Label();
            autoAppAccountCB = new CheckBox();
            objectTB = new TextBox();
            label3 = new Label();
            label4 = new Label();
            appIdTB = new TextBox();
            label5 = new Label();
            LinkLabel1 = new LinkLabel();
            SuspendLayout();
            // 
            // NextBTN
            // 
            NextBTN.Location = new Point(357, 477);
            NextBTN.Name = "NextBTN";
            NextBTN.Size = new Size(75, 23);
            NextBTN.TabIndex = 73;
            NextBTN.Text = "Create";
            NextBTN.UseVisualStyleBackColor = true;
            NextBTN.Click += NextBTN_Click;
            // 
            // BackBTN
            // 
            BackBTN.Location = new Point(12, 477);
            BackBTN.Name = "BackBTN";
            BackBTN.Size = new Size(75, 23);
            BackBTN.TabIndex = 72;
            BackBTN.Text = "Back";
            BackBTN.UseVisualStyleBackColor = true;
            BackBTN.Click += BackBTN_Click;
            // 
            // OutputRT
            // 
            OutputRT.Location = new Point(11, 202);
            OutputRT.Name = "OutputRT";
            OutputRT.ReadOnly = true;
            OutputRT.Size = new Size(421, 269);
            OutputRT.TabIndex = 70;
            OutputRT.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 184);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 71;
            label1.Text = "Output:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(0, 15);
            label2.TabIndex = 74;
            // 
            // autoAppAccountCB
            // 
            autoAppAccountCB.AutoSize = true;
            autoAppAccountCB.Checked = true;
            autoAppAccountCB.CheckState = CheckState.Checked;
            autoAppAccountCB.Location = new Point(11, 89);
            autoAppAccountCB.Name = "autoAppAccountCB";
            autoAppAccountCB.Size = new Size(303, 19);
            autoAppAccountCB.TabIndex = 80;
            autoAppAccountCB.Text = "Automatically Create Dataverse Application Account";
            autoAppAccountCB.UseVisualStyleBackColor = true;
            // 
            // objectTB
            // 
            objectTB.Location = new Point(77, 143);
            objectTB.Name = "objectTB";
            objectTB.PlaceholderText = "Enter an already used API Object ID here";
            objectTB.Size = new Size(355, 23);
            objectTB.TabIndex = 79;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 146);
            label3.Name = "label3";
            label3.Size = new Size(59, 15);
            label3.TabIndex = 78;
            label3.Text = "Object ID:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 117);
            label4.Name = "label4";
            label4.Size = new Size(55, 15);
            label4.TabIndex = 77;
            label4.Text = "Client ID:";
            // 
            // appIdTB
            // 
            appIdTB.Location = new Point(77, 114);
            appIdTB.Name = "appIdTB";
            appIdTB.PlaceholderText = "Enter an already used API Client ID here";
            appIdTB.Size = new Size(355, 23);
            appIdTB.TabIndex = 75;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(11, 9);
            label5.Name = "label5";
            label5.Size = new Size(398, 60);
            label5.TabIndex = 81;
            label5.Text = resources.GetString("label5.Text");
            // 
            // LinkLabel1
            // 
            LinkLabel1.AutoSize = true;
            LinkLabel1.Location = new Point(200, 54);
            LinkLabel1.Name = "LinkLabel1";
            LinkLabel1.Size = new Size(134, 15);
            LinkLabel1.TabIndex = 82;
            LinkLabel1.TabStop = true;
            LinkLabel1.Text = "Manual API Registration";
            LinkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // APIRegistration
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(447, 512);
            Controls.Add(LinkLabel1);
            Controls.Add(label5);
            Controls.Add(autoAppAccountCB);
            Controls.Add(objectTB);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(appIdTB);
            Controls.Add(label2);
            Controls.Add(NextBTN);
            Controls.Add(BackBTN);
            Controls.Add(OutputRT);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "APIRegistration";
            Text = "API Registration";
            FormClosing += Form_Closing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button NextBTN;
        private Button BackBTN;
        internal RichTextBox OutputRT;
        private Label label1;
        private Label label2;
        public CheckBox autoAppAccountCB;
        public TextBox objectTB;
        private Label label3;
        private Label label4;
        public TextBox appIdTB;
        private Label label5;
        private LinkLabel LinkLabel1;
    }
}