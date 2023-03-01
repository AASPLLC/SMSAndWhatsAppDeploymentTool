using AASPGlobalLibrary;
using SMSAndWhatsAppDeploymentTool.ResourceHandlers;

namespace SMSAndWhatsAppDeploymentTool.StepByStep
{
    public partial class ChooseCosmosAccountName : Form
    {
        readonly StepByStepValues sbs;
        readonly APIRegistration lastStep;
        public ChooseCosmosAccountName(StepByStepValues sbs, APIRegistration lastStep)
        {
            this.sbs = sbs;
            this.lastStep = lastStep;
            InitializeComponent();
        }

        internal void DisableAll()
        {
            desiredCosmosAccountNameFunctionNameTB.Enabled = false;
            BackBTN.Enabled = false;
            NextBTN.Enabled = false;
        }
        internal void EnableAll()
        {
            desiredCosmosAccountNameFunctionNameTB.Enabled = true;
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
            CosmosResourceHandler crh = new();
            await crh.InitialCreation(desiredCosmosAccountNameFunctionNameTB.Text, sbs);

            Hide();
            ChooseCommunicationsName2 form = new(sbs, this);
            form.ShowDialog();
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