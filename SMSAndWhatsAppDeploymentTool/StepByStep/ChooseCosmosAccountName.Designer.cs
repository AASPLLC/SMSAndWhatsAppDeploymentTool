namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    partial class ChooseCosmosAccountName
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseCosmosAccountName));
            LinkLabel3 = new LinkLabel();
            UniqueStringBTN = new Button();
            AutoGenerateNamesBTN = new Button();
            NextBTN = new Button();
            BackBTN = new Button();
            OutputRT = new RichTextBox();
            label1 = new Label();
            label4 = new Label();
            label2 = new Label();
            label3 = new Label();
            desiredCosmosAccountNameFunctionNameTB = new TextBox();
            LinkLabel5 = new LinkLabel();
            SuspendLayout();
            // 
            // LinkLabel3
            // 
            LinkLabel3.AutoSize = true;
            LinkLabel3.Location = new Point(222, 183);
            LinkLabel3.Name = "LinkLabel3";
            LinkLabel3.Size = new Size(223, 15);
            LinkLabel3.TabIndex = 73;
            LinkLabel3.TabStop = true;
            LinkLabel3.Text = "Microsoft Azure Cosmos Documentation";
            LinkLabel3.LinkClicked += LinkLabel3_LinkClicked;
            // 
            // UniqueStringBTN
            // 
            UniqueStringBTN.Location = new Point(563, 135);
            UniqueStringBTN.Margin = new Padding(4, 5, 4, 5);
            UniqueStringBTN.Name = "UniqueStringBTN";
            UniqueStringBTN.Size = new Size(133, 23);
            UniqueStringBTN.TabIndex = 71;
            UniqueStringBTN.Text = "Make Unique";
            UniqueStringBTN.UseVisualStyleBackColor = true;
            UniqueStringBTN.Click += UniqueStringBTN_Click;
            // 
            // AutoGenerateNamesBTN
            // 
            AutoGenerateNamesBTN.Location = new Point(563, 102);
            AutoGenerateNamesBTN.Margin = new Padding(4, 5, 4, 5);
            AutoGenerateNamesBTN.Name = "AutoGenerateNamesBTN";
            AutoGenerateNamesBTN.Size = new Size(133, 23);
            AutoGenerateNamesBTN.TabIndex = 70;
            AutoGenerateNamesBTN.Text = "Auto Generate Name";
            AutoGenerateNamesBTN.UseVisualStyleBackColor = true;
            AutoGenerateNamesBTN.Click += AutoGenerateNamesBTN_Click;
            // 
            // NextBTN
            // 
            NextBTN.Location = new Point(622, 409);
            NextBTN.Name = "NextBTN";
            NextBTN.Size = new Size(75, 23);
            NextBTN.TabIndex = 69;
            NextBTN.Text = "Create";
            NextBTN.UseVisualStyleBackColor = true;
            NextBTN.Click += NextBTN_Click;
            // 
            // BackBTN
            // 
            BackBTN.Location = new Point(12, 409);
            BackBTN.Name = "BackBTN";
            BackBTN.Size = new Size(75, 23);
            BackBTN.TabIndex = 68;
            BackBTN.Text = "Back";
            BackBTN.UseVisualStyleBackColor = true;
            BackBTN.Click += BackBTN_Click;
            // 
            // OutputRT
            // 
            OutputRT.Location = new Point(12, 225);
            OutputRT.Name = "OutputRT";
            OutputRT.ReadOnly = true;
            OutputRT.Size = new Size(687, 178);
            OutputRT.TabIndex = 66;
            OutputRT.Text = "";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 207);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 67;
            label1.Text = "Output:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(306, 168);
            label4.Name = "label4";
            label4.Size = new Size(67, 15);
            label4.TabIndex = 64;
            label4.Text = "References:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(43, 9);
            label2.Name = "label2";
            label2.Size = new Size(616, 90);
            label2.TabIndex = 63;
            label2.Text = resources.GetString("label2.Text");
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 122);
            label3.Name = "label3";
            label3.Size = new Size(178, 15);
            label3.TabIndex = 76;
            label3.Text = "Desired Cosmos Account Name:";
            // 
            // desiredCosmosAccountNameFunctionNameTB
            // 
            desiredCosmosAccountNameFunctionNameTB.Location = new Point(195, 119);
            desiredCosmosAccountNameFunctionNameTB.Name = "desiredCosmosAccountNameFunctionNameTB";
            desiredCosmosAccountNameFunctionNameTB.PlaceholderText = "The cosmos database account name";
            desiredCosmosAccountNameFunctionNameTB.Size = new Size(362, 23);
            desiredCosmosAccountNameFunctionNameTB.TabIndex = 75;
            // 
            // LinkLabel5
            // 
            LinkLabel5.AutoSize = true;
            LinkLabel5.Location = new Point(401, 39);
            LinkLabel5.Name = "LinkLabel5";
            LinkLabel5.Size = new Size(182, 15);
            LinkLabel5.TabIndex = 77;
            LinkLabel5.TabStop = true;
            LinkLabel5.Text = "Manual Requirements Document";
            LinkLabel5.LinkClicked += LinkLabel5_LinkClicked;
            // 
            // ChooseCosmosAccountName
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(709, 440);
            Controls.Add(LinkLabel5);
            Controls.Add(label3);
            Controls.Add(desiredCosmosAccountNameFunctionNameTB);
            Controls.Add(LinkLabel3);
            Controls.Add(UniqueStringBTN);
            Controls.Add(AutoGenerateNamesBTN);
            Controls.Add(NextBTN);
            Controls.Add(BackBTN);
            Controls.Add(OutputRT);
            Controls.Add(label1);
            Controls.Add(label4);
            Controls.Add(label2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseCosmosAccountName";
            Text = "Cosmos";
            FormClosing += Form_Closing;
            Load += Form_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private LinkLabel LinkLabel3;
        private Button UniqueStringBTN;
        private Button AutoGenerateNamesBTN;
        private Button NextBTN;
        private Button BackBTN;
        internal RichTextBox OutputRT;
        private Label label1;
        private Label label4;
        private Label label2;
        private Label label3;
        private TextBox desiredCosmosAccountNameFunctionNameTB;
        private LinkLabel LinkLabel5;
    }
}