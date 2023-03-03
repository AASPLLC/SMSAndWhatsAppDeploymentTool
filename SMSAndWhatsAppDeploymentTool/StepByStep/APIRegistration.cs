using AASPGlobalLibrary;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public partial class APIRegistration : Form
    {
        readonly string message = "An API with a dynamics 365 connection is required to continue deployment." +
            Environment.NewLine + "Press OK if you want the API to be automatically created instead.";

        readonly StepByStepValues sbs;
        readonly ChooseKeyVaultNames1? lastStep;
        readonly DataverseDeploy? lastStep2;
        public APIRegistration(StepByStepValues sbs, ChooseKeyVaultNames1 lastStep)
        {
            this.sbs = sbs;
            this.lastStep = lastStep;
            InitializeComponent();
            if (sbs.DBType == 1)
            {
                objectTB.Enabled = false;
                autoAppAccountCB.Checked = false;
                autoAppAccountCB.Enabled = false;
            }
        }
        public APIRegistration(StepByStepValues sbs, DataverseDeploy lastStep)
        {
            this.sbs = sbs;
            lastStep2 = lastStep;
            InitializeComponent();
            if (sbs.DBType == 1)
            {
                objectTB.Enabled = false;
                autoAppAccountCB.Checked = false;
                autoAppAccountCB.Enabled = false;
            }
        }

        internal void DisableAll()
        {
            autoAppAccountCB.Enabled = false;
            appIdTB.Enabled = false;
            objectTB.Enabled = false;
            NextBTN.Enabled = false;
            BackBTN.Enabled = false;
        }
        internal void EnableAll()
        {
            if (sbs.DBType == 0)
            {
                autoAppAccountCB.Enabled = true;
                objectTB.Enabled = true;
            }
            appIdTB.Enabled = true;
            NextBTN.Enabled = true;
            BackBTN.Enabled = true;
        }

        public (string, string, bool) GetResponsePackage()
        {
            return (this.appIdTB.Text, this.objectTB.Text, this.autoAppAccountCB.Checked);
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            lastStep?.Close();
            lastStep2?.Close();
        }

        private async void NextBTN_Click(object sender, EventArgs e)
        {
            DisableAll();
            if (sbs.DBType == 0)
            {
                if (NextBTN.Text == "Next")
                {
                    Hide();
                    ChooseCommunicationsName2 form = new(sbs, this);
                    form.ShowDialog();
                }
                else
                {
                    OutputRT.Text = "";
                    if (autoAppAccountCB.Checked)
                    {
                        var results = MessageBox.Show("Make sure a Application User does not already exist." + Environment.NewLine + "Press OK to continue.", "System Account Creation", MessageBoxButtons.OKCancel);
                        if (results == DialogResult.OK)
                        {
                            if (await sbs.CreateAllDataverseResources(appIdTB.Text, objectTB.Text, autoAppAccountCB.Checked))
                                ((Control)sender).Text = "Next";
                        }
                    }
                    else
                    {
                        if (await sbs.CreateAllDataverseResources(appIdTB.Text, objectTB.Text, autoAppAccountCB.Checked))
                            ((Control)sender).Text = "Next";
                    }
                }
            }
            else
            {
                if (NextBTN.Text == "Next")
                {
                    Hide();
                    ChooseCommunicationsName2 form = new(sbs, this);
                    form.ShowDialog();
                }
                else
                {
                    OutputRT.Text = "";
                    if (sbs.AutoAPI)
                    {
                        if (await sbs.SetupCosmosEnvironment())
                            ((Control)sender).Text = "Next";
                    }
                    else
                    {
                        if (await sbs.SetupCosmosEnvironment(appIdTB.Text))
                            ((Control)sender).Text = "Next";
                    }
                }
            }
            if (!sbs.AutoAPI)
                EnableAll();
            else
            {
                NextBTN.Enabled = true;
                BackBTN.Enabled = true;
            }
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            if (appIdTB.Text != "" && objectTB.Text != "")
                this.Close();
            else
            {
                var results = MessageBox.Show(message, "Empty Fields Detected", MessageBoxButtons.OKCancel);
                if (results == DialogResult.OK)
                {
                    appIdTB.Text = "";
                    objectTB.Text = "";
                    autoAppAccountCB.Checked = true;
                    this.Close();
                }
            }
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sbs.infoWebsites.OpenManualAPIInstructions();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            _ = new SetConsoleOutput(OutputRT);

            if (sbs.AutoAPI)
            {
                var results = MessageBox.Show("Would you like to create an API automatically?" + Environment.NewLine + "Click no if you would like to enter an already existing API.", "Confirm", MessageBoxButtons.YesNo);
                if (results == DialogResult.Yes)
                {
                    appIdTB.Enabled = false;
                    objectTB.Enabled = false;
                    //sbs.AutoAPI = true;
                }
                else
                    sbs.AutoAPI = false;
            }
        }
    }
}
