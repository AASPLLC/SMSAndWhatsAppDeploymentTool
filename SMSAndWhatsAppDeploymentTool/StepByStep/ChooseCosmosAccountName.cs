using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public partial class ChooseCosmosAccountName : Form
    {
        readonly StepByStepValues sbs;
        readonly ChooseVirtualNetwork3 lastStep;
        public ChooseCosmosAccountName(StepByStepValues sbs, ChooseVirtualNetwork3 lastStep)
        {
            this.sbs = sbs;
            this.lastStep = lastStep;
            InitializeComponent();
        }

        internal void DisableAll()
        {
            desiredCosmosAccountNameFunctionNameTB.Enabled = false;
            AutoGenerateNamesBTN.Enabled = false;
            UniqueStringBTN.Enabled = false;
            BackBTN.Enabled = false;
            NextBTN.Enabled = false;
        }
        internal void EnableAll()
        {
            desiredCosmosAccountNameFunctionNameTB.Enabled = true;
            AutoGenerateNamesBTN.Enabled = true;
            UniqueStringBTN.Enabled = true;
            BackBTN.Enabled = true;
            NextBTN.Enabled = true;
        }

        private void AutoGenerateNamesBTN_Click(object sender, EventArgs e)
        {
            desiredCosmosAccountNameFunctionNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
        }

        private void UniqueStringBTN_Click(object sender, EventArgs e)
        {
            if (desiredCosmosAccountNameFunctionNameTB.Text == "")
                desiredCosmosAccountNameFunctionNameTB.Text = WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord() + WordGenerator.GetRandomWord();
            desiredCosmosAccountNameFunctionNameTB.Text = ChooseDBType.GenerateUniqueString(desiredCosmosAccountNameFunctionNameTB.Text);
        }

        private async void NextBTN_Click(object sender, EventArgs e)
        {
            DisableAll();
            if (NextBTN.Text == "Next")
            {
                Hide();
                ChooseStorageName4 form = new(sbs, this);
                form.ShowDialog();
            }
            else
            {
                OutputRT.Text = "";
                CosmosResourceHandler crh = new();
                if (await crh.InitialCreation(desiredCosmosAccountNameFunctionNameTB.Text, sbs))
                    ((Control)sender).Text = "Next";
            }
            EnableAll();
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            _ = new SetConsoleOutput(OutputRT);
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            lastStep.Close();
        }

        private void LinkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sbs.infoWebsites.OpenManualRequirementsAfterDeployment();
        }

        private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Globals.OpenLink("https://learn.microsoft.com/en-us/azure/cosmos-db/");
        }
    }
}