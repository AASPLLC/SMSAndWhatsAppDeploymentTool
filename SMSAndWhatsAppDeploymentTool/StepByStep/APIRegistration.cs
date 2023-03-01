using AASPGlobalLibrary;
using System.Data;

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
            if (sbs.AutoAPI)
            {
                appIdTB.Enabled = false;
                objectTB.Enabled = false;
            }
            if (sbs.DBType == 1)
            {
                autoAppAccountCB.Checked = false;
                autoAppAccountCB.Enabled = false;
            }
        }
        public APIRegistration(StepByStepValues sbs, DataverseDeploy lastStep)
        {
            this.sbs = sbs;
            lastStep2 = lastStep;
            InitializeComponent();
            if (sbs.AutoAPI)
            {
                appIdTB.Enabled = false;
                objectTB.Enabled = false;
            }
            if (sbs.DBType == 1)
            {
                autoAppAccountCB.Checked = false;
                autoAppAccountCB.Enabled = false;
            }
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
            if (sbs.DBType == 0)
            {
                if (autoAppAccountCB.Checked)
                {
                    var results = MessageBox.Show("Make sure a Application User does not already exist." + Environment.NewLine + "Press OK to continue.", "System Account Creation", MessageBoxButtons.OKCancel);
                    if (results == DialogResult.OK)
                    {
                        await sbs.CreateAllDataverseResources(appIdTB.Text, objectTB.Text, autoAppAccountCB.Checked);

                        this.Hide();
                        ChooseCommunicationsName2 form = new(sbs, this);
                        form.ShowDialog();
                    }
                }
                else
                {
                    await sbs.CreateAllDataverseResources(appIdTB.Text, objectTB.Text, autoAppAccountCB.Checked);

                    this.Hide();
                    ChooseCommunicationsName2 form = new(sbs, this);
                    form.ShowDialog();
                }
            }
            else
            {
                await sbs.SetupCosmosEnvironment();

                this.Hide();
                ChooseCosmosAccountName form = new(sbs, this);
                form.ShowDialog();
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
            Globals.OpenLink(((LinkLabel)sender).Text);
        }
    }
}
