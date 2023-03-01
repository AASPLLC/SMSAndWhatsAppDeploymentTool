namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    partial class ChooseCommunicationsName2
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
            LinkLabel1 = new LinkLabel();
            label4 = new Label();
            label2 = new Label();
            OutputRT = new RichTextBox();
            label1 = new Label();
            BackBTN = new Button();
            NextBTN = new Button();
            UniqueStringBTN = new Button();
            AutoGenerateNamesBTN = new Button();
            LinkLabel2 = new LinkLabel();
            desiredCommunicationsNameTB = new TextBox();
            label3 = new Label();
            SuspendLayout();
            // 
            // LinkLabel1
            // 
            LinkLabel1.AutoSize = true;
            LinkLabel1.Location = new Point(197, 151);
            LinkLabel1.Name = "LinkLabel1";
            LinkLabel1.Size = new Size(287, 15);
            LinkLabel1.TabIndex = 51;
            LinkLabel1.TabStop = true;
            LinkLabel1.Text = "How To Manage Phone Numbers (After Deployment)";
            LinkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(303, 136);
            label4.Name = "label4";
            label4.Size = new Size(67, 15);
            label4.TabIndex = 50;
            label4.Text = "References:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(131, 9);
            label2.Name = "label2";
            label2.Size = new Size(398, 30);
            label2.TabIndex = 48;
            label2.Text = "This is the primary link to establish phone numbers for SMS.\r\nIf WhatsApp is used, the phone numbers will also be required for it as well.";
            // 
            // OutputRT
            // 
            OutputRT.Location = new Point(9, 215);
            OutputRT.Name = "OutputRT";
            OutputRT.ReadOnly = true;
            OutputRT.Size = new Size(687, 190);
            OutputRT.TabIndex = 52;
            OutputRT.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 197);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 53;
            label1.Text = "Output:";
            // 
            // BackBTN
            // 
            BackBTN.Location = new Point(9, 411);
            BackBTN.Name = "BackBTN";
            BackBTN.Size = new Size(75, 23);
            BackBTN.TabIndex = 54;
            BackBTN.Text = "Back";
            BackBTN.UseVisualStyleBackColor = true;
            BackBTN.Click += BackBTN_Click;
            // 
            // NextBTN
            // 
            NextBTN.Location = new Point(621, 411);
            NextBTN.Name = "NextBTN";
            NextBTN.Size = new Size(75, 23);
            NextBTN.TabIndex = 55;
            NextBTN.Text = "Create";
            NextBTN.UseVisualStyleBackColor = true;
            NextBTN.Click += NextBTN_Click;
            // 
            // UniqueStringBTN
            // 
            UniqueStringBTN.Location = new Point(563, 97);
            UniqueStringBTN.Margin = new Padding(4, 5, 4, 5);
            UniqueStringBTN.Name = "UniqueStringBTN";
            UniqueStringBTN.Size = new Size(133, 23);
            UniqueStringBTN.TabIndex = 57;
            UniqueStringBTN.Text = "Make Unique";
            UniqueStringBTN.UseVisualStyleBackColor = true;
            UniqueStringBTN.Click += UniqueStringBTN_Click;
            // 
            // AutoGenerateNamesBTN
            // 
            AutoGenerateNamesBTN.Location = new Point(563, 64);
            AutoGenerateNamesBTN.Margin = new Padding(4, 5, 4, 5);
            AutoGenerateNamesBTN.Name = "AutoGenerateNamesBTN";
            AutoGenerateNamesBTN.Size = new Size(133, 23);
            AutoGenerateNamesBTN.TabIndex = 56;
            AutoGenerateNamesBTN.Text = "Auto Generate Name";
            AutoGenerateNamesBTN.UseVisualStyleBackColor = true;
            AutoGenerateNamesBTN.Click += AutoGenerateNamesBTN_Click;
            // 
            // LinkLabel2
            // 
            LinkLabel2.AutoSize = true;
            LinkLabel2.Location = new Point(184, 166);
            LinkLabel2.Name = "LinkLabel2";
            LinkLabel2.Size = new Size(312, 15);
            LinkLabel2.TabIndex = 58;
            LinkLabel2.TabStop = true;
            LinkLabel2.Text = "Microsoft Azure Communication Services Documentation";
            LinkLabel2.LinkClicked += LinkLabel2_LinkClicked;
            // 
            // desiredCommunicationsNameTB
            // 
            desiredCommunicationsNameTB.Location = new Point(197, 80);
            desiredCommunicationsNameTB.Name = "desiredCommunicationsNameTB";
            desiredCommunicationsNameTB.PlaceholderText = "Can contain dashes (-)";
            desiredCommunicationsNameTB.Size = new Size(359, 23);
            desiredCommunicationsNameTB.TabIndex = 60;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 83);
            label3.Name = "label3";
            label3.Size = new Size(179, 15);
            label3.TabIndex = 59;
            label3.Text = "Desired Communications Name:";
            // 
            // ChooseCommunicationsName1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(707, 446);
            Controls.Add(desiredCommunicationsNameTB);
            Controls.Add(label3);
            Controls.Add(LinkLabel2);
            Controls.Add(UniqueStringBTN);
            Controls.Add(AutoGenerateNamesBTN);
            Controls.Add(NextBTN);
            Controls.Add(BackBTN);
            Controls.Add(OutputRT);
            Controls.Add(label1);
            Controls.Add(LinkLabel1);
            Controls.Add(label4);
            Controls.Add(label2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseCommunicationsName1";
            Text = "Communication Service";
            FormClosed += Form_Closing;
            Load += Form_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private LinkLabel LinkLabel1;
        private Label label4;
        private Label label2;
        internal RichTextBox OutputRT;
        private Label label1;
        private Button BackBTN;
        private Button NextBTN;
        private Button UniqueStringBTN;
        private Button AutoGenerateNamesBTN;
        private LinkLabel LinkLabel2;
        private TextBox desiredCommunicationsNameTB;
        private Label label3;
    }
}