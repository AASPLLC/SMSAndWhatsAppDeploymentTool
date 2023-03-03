using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public partial class ChooseVirtualNetwork3 : Form
    {
        readonly StepByStepValues sbs;
        readonly ChooseCommunicationsName2 lastStep;
        public ChooseVirtualNetwork3(StepByStepValues sbs, ChooseCommunicationsName2 lastStep)
        {
            this.sbs = sbs;
            this.lastStep = lastStep;
            InitializeComponent();
        }

        internal void DisableAll()
        {
            defaultSubnetTB.Enabled = false;
            appsSubnetTB.Enabled = false;
            BackBTN.Enabled = false;
            NextBTN.Enabled = false;
        }
        internal void EnableAll()
        {
            defaultSubnetTB.Enabled = true;
            appsSubnetTB.Enabled = true;
            BackBTN.Enabled = true;
            NextBTN.Enabled = true;
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void NextBTN_Click(object sender, EventArgs e)
        {
            DisableAll();
            if (NextBTN.Text == "Next")
            {
                Hide();
                if (sbs.DBType == 0)
                {
                    ChooseStorageName4 form = new(sbs, this);
                    form.ShowDialog();
                }
                else
                {
                    ChooseCosmosAccountName form = new(sbs, this);
                    form.ShowDialog();
                }
            }
            else
            {
                OutputRT.Text = "";
                VirtualNetworkResourceHandler vnrh = new();
                await vnrh.InitialCreation(defaultSubnetTB.Text, appsSubnetTB.Text, sbs);

                ((Control)sender).Text = "Next";
            }
            EnableAll();
        }

        private void Form_Closing(object sender, FormClosedEventArgs e)
        {
            lastStep.Close();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            _ = new SetConsoleOutput(OutputRT);
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://learn.microsoft.com/en-us/azure/virtual-network/virtual-networks-overview");
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sbs.infoWebsites.OpenNetworkingDetails();
        }
    }
}
