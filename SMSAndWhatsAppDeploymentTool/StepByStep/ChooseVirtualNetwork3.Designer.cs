namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    partial class ChooseVirtualNetwork3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseVirtualNetwork3));
            LinkLabel1 = new LinkLabel();
            label4 = new Label();
            label2 = new Label();
            OutputRT = new RichTextBox();
            label1 = new Label();
            BackBTN = new Button();
            NextBTN = new Button();
            LinkLabel2 = new LinkLabel();
            appsSubnetTB = new TextBox();
            label10 = new Label();
            defaultSubnetTB = new TextBox();
            label5 = new Label();
            SuspendLayout();
            // 
            // LinkLabel1
            // 
            LinkLabel1.AutoSize = true;
            LinkLabel1.Location = new Point(222, 256);
            LinkLabel1.Name = "LinkLabel1";
            LinkLabel1.Size = new Size(140, 15);
            LinkLabel1.TabIndex = 51;
            LinkLabel1.TabStop = true;
            LinkLabel1.Text = "Networking Setup Details";
            LinkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(261, 241);
            label4.Name = "label4";
            label4.Size = new Size(67, 15);
            label4.TabIndex = 50;
            label4.Text = "References:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 9);
            label2.Name = "label2";
            label2.Size = new Size(550, 150);
            label2.TabIndex = 48;
            label2.Text = resources.GetString("label2.Text");
            // 
            // OutputRT
            // 
            OutputRT.Location = new Point(9, 322);
            OutputRT.Name = "OutputRT";
            OutputRT.ReadOnly = true;
            OutputRT.Size = new Size(566, 177);
            OutputRT.TabIndex = 52;
            OutputRT.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 304);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 53;
            label1.Text = "Output:";
            // 
            // BackBTN
            // 
            BackBTN.Location = new Point(9, 505);
            BackBTN.Name = "BackBTN";
            BackBTN.Size = new Size(75, 23);
            BackBTN.TabIndex = 54;
            BackBTN.Text = "Back";
            BackBTN.UseVisualStyleBackColor = true;
            BackBTN.Click += BackBTN_Click;
            // 
            // NextBTN
            // 
            NextBTN.Location = new Point(500, 505);
            NextBTN.Name = "NextBTN";
            NextBTN.Size = new Size(75, 23);
            NextBTN.TabIndex = 55;
            NextBTN.Text = "Create";
            NextBTN.UseVisualStyleBackColor = true;
            NextBTN.Click += NextBTN_Click;
            // 
            // LinkLabel2
            // 
            LinkLabel2.AutoSize = true;
            LinkLabel2.Location = new Point(162, 271);
            LinkLabel2.Name = "LinkLabel2";
            LinkLabel2.Size = new Size(279, 15);
            LinkLabel2.TabIndex = 58;
            LinkLabel2.TabStop = true;
            LinkLabel2.Text = "Microsoft Azure Virtual Networking Documentation";
            LinkLabel2.LinkClicked += LinkLabel2_LinkClicked;
            // 
            // appsSubnetTB
            // 
            appsSubnetTB.Location = new Point(118, 207);
            appsSubnetTB.Name = "appsSubnetTB";
            appsSubnetTB.PlaceholderText = "Keep blank to use 10.1.0.32/27, ends in /27";
            appsSubnetTB.Size = new Size(457, 23);
            appsSubnetTB.TabIndex = 62;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(9, 210);
            label10.Name = "label10";
            label10.Size = new Size(77, 15);
            label10.TabIndex = 61;
            label10.Text = "Apps Subnet:";
            // 
            // defaultSubnetTB
            // 
            defaultSubnetTB.Location = new Point(118, 178);
            defaultSubnetTB.Name = "defaultSubnetTB";
            defaultSubnetTB.PlaceholderText = "Keep blank to use 10.1.0.0/29, ends in /29";
            defaultSubnetTB.Size = new Size(457, 23);
            defaultSubnetTB.TabIndex = 60;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(9, 181);
            label5.Name = "label5";
            label5.Size = new Size(88, 15);
            label5.TabIndex = 59;
            label5.Text = "Default Subnet:";
            // 
            // ChooseVirtualNetwork2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(588, 533);
            Controls.Add(appsSubnetTB);
            Controls.Add(label10);
            Controls.Add(defaultSubnetTB);
            Controls.Add(label5);
            Controls.Add(LinkLabel2);
            Controls.Add(NextBTN);
            Controls.Add(BackBTN);
            Controls.Add(OutputRT);
            Controls.Add(label1);
            Controls.Add(LinkLabel1);
            Controls.Add(label4);
            Controls.Add(label2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseVirtualNetwork2";
            Text = "Virtual Networking";
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
        private LinkLabel LinkLabel2;
        private TextBox appsSubnetTB;
        private Label label10;
        private TextBox defaultSubnetTB;
        private Label label5;
    }
}