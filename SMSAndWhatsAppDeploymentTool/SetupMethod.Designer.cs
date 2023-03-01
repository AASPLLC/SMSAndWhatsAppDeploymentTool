namespace SMSAndWhatsAppDeploymentTool
{
    partial class SetupMethod
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
            Button1 = new Button();
            Button2 = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            LinkLabel1 = new LinkLabel();
            SuspendLayout();
            // 
            // Button1
            // 
            Button1.Location = new Point(12, 80);
            Button1.Name = "Button1";
            Button1.Size = new Size(231, 43);
            Button1.TabIndex = 0;
            Button1.Text = "Step-By-Step(Easier)";
            Button1.UseVisualStyleBackColor = true;
            Button1.Click += Button1_Click;
            // 
            // Button2
            // 
            Button2.Location = new Point(256, 80);
            Button2.Name = "Button2";
            Button2.Size = new Size(222, 43);
            Button2.TabIndex = 1;
            Button2.Text = "All At Once(Advanced)";
            Button2.UseVisualStyleBackColor = true;
            Button2.Click += Button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 37);
            label1.Name = "label1";
            label1.Size = new Size(135, 15);
            label1.TabIndex = 2;
            label1.Text = "Choose your setup type:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 62);
            label2.Name = "label2";
            label2.Size = new Size(231, 15);
            label2.TabIndex = 3;
            label2.Text = "Provides detailed instructions step by step:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(256, 62);
            label3.Name = "label3";
            label3.Size = new Size(222, 15);
            label3.TabIndex = 4;
            label3.Text = "Asks for all required information at once:";
            // 
            // LinkLabel1
            // 
            LinkLabel1.AutoSize = true;
            LinkLabel1.Location = new Point(12, 9);
            LinkLabel1.Name = "LinkLabel1";
            LinkLabel1.Size = new Size(274, 15);
            LinkLabel1.TabIndex = 5;
            LinkLabel1.TabStop = true;
            LinkLabel1.Text = "Click here to review the Deployment Requirements";
            LinkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // SetupMethod
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(503, 138);
            Controls.Add(LinkLabel1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(Button2);
            Controls.Add(Button1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SetupMethod";
            Text = "Setup Method";
            Load += SetupMethod_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Button1;
        private Button Button2;
        private Label label1;
        private Label label2;
        private Label label3;
        private LinkLabel LinkLabel1;
    }
}