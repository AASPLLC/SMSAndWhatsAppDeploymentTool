using SMSAndWhatsAppDeploymentTool.JSONParsing;
using SMSAndWhatsAppDeploymentTool.StepByStep;

namespace SMSAndWhatsAppDeploymentTool
{
    public partial class SetupMethod : Form
    {
        readonly StepByStepValues sbs = new();
        public SetupMethod()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            ChooseDBType dbtype = new(this, 1, sbs);
            dbtype.ShowDialog();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            ChooseDBType dbtype = new(this, 0, sbs);
            dbtype.ShowDialog();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sbs.infoWebsites.OpenDeploymentRequirements();
        }

        private async void SetupMethod_Load(object sender, EventArgs e)
        {
            sbs.infoWebsites = await JSONDocuments.Load();
            sbs.secretNames = await JSONSecretNames.Load();
        }
    }
}
