using Azure.Core;
using Azure.ResourceManager.Resources;
using SMSAndWhatsAppDeploymentTool.StepByStep;

namespace SMSAndWhatsAppDeploymentTool
{
    public partial class CosmosConfig : Form
    {
        readonly ChooseDBType dBType;
        readonly CosmosDeploy cosmosDeploy;
        readonly int setupType = 0;
        readonly StepByStepValues sbs;
        internal CosmosConfig(ChooseDBType dBType, int setupType, StepByStepValues sbs)
        {
            this.sbs = sbs;
            cosmosDeploy = new(dBType, sbs);
            this.dBType = dBType;
            this.setupType = setupType;
            InitializeComponent();

            //US added first
            /*foreach(AzureLocation item in Enum.GetValues(typeof(AzureLocation)))
            {
                comboBox3.Items.Add(item);
            }*/
            comboBox3.Items.Add(AzureLocation.CentralUS.Name);
            comboBox3.Items.Add(AzureLocation.NorthCentralUS.Name);
            comboBox3.Items.Add(AzureLocation.SouthCentralUS.Name);
            comboBox3.Items.Add(AzureLocation.EastUS.Name);
            comboBox3.Items.Add(AzureLocation.EastUS2.Name);
            comboBox3.Items.Add(AzureLocation.WestUS.Name);
            comboBox3.Items.Add(AzureLocation.WestUS2.Name);

            comboBox3.Items.Add(AzureLocation.AustraliaCentral.Name);
            comboBox3.Items.Add(AzureLocation.AustraliaCentral2.Name);
            comboBox3.Items.Add(AzureLocation.AustraliaEast.Name);
            comboBox3.Items.Add(AzureLocation.AustraliaSoutheast.Name);
            comboBox3.Items.Add(AzureLocation.BrazilSouth.Name);
            comboBox3.Items.Add(AzureLocation.BrazilSoutheast.Name);
            comboBox3.Items.Add(AzureLocation.CanadaCentral.Name);
            comboBox3.Items.Add(AzureLocation.CanadaEast.Name);
            comboBox3.Items.Add(AzureLocation.CentralIndia.Name);
            comboBox3.Items.Add(AzureLocation.ChinaEast.Name);
            comboBox3.Items.Add(AzureLocation.ChinaEast2.Name);
            comboBox3.Items.Add(AzureLocation.ChinaNorth.Name);
            comboBox3.Items.Add(AzureLocation.ChinaNorth2.Name);
            comboBox3.Items.Add(AzureLocation.EastAsia.Name);
            comboBox3.Items.Add(AzureLocation.FranceCentral.Name);
            comboBox3.Items.Add(AzureLocation.FranceSouth.Name);
            comboBox3.Items.Add(AzureLocation.GermanyCentral.Name);
            comboBox3.Items.Add(AzureLocation.GermanyNorth.Name);
            comboBox3.Items.Add(AzureLocation.GermanyNorthEast.Name);
            comboBox3.Items.Add(AzureLocation.GermanyWestCentral.Name);
            comboBox3.Items.Add(AzureLocation.JapanEast.Name);
            comboBox3.Items.Add(AzureLocation.JapanWest.Name);
            comboBox3.Items.Add(AzureLocation.KoreaCentral.Name);
            comboBox3.Items.Add(AzureLocation.KoreaSouth.Name);
            comboBox3.Items.Add(AzureLocation.NorthEurope.Name);
            comboBox3.Items.Add(AzureLocation.NorwayEast.Name);
            comboBox3.Items.Add(AzureLocation.NorwayWest.Name);
            comboBox3.Items.Add(AzureLocation.SouthAfricaNorth.Name);
            comboBox3.Items.Add(AzureLocation.SouthAfricaWest.Name);
            comboBox3.Items.Add(AzureLocation.SoutheastAsia.Name);
            comboBox3.Items.Add(AzureLocation.SouthIndia.Name);
            comboBox3.Items.Add(AzureLocation.SwitzerlandNorth.Name);
            comboBox3.Items.Add(AzureLocation.SwitzerlandWest.Name);
            comboBox3.Items.Add(AzureLocation.UAECentral.Name);
            comboBox3.Items.Add(AzureLocation.UAENorth.Name);
            comboBox3.Items.Add(AzureLocation.UKSouth.Name);
            comboBox3.Items.Add(AzureLocation.UKWest.Name);
            comboBox3.Items.Add(AzureLocation.WestCentralUS.Name);
            comboBox3.Items.Add(AzureLocation.WestEurope.Name);
            comboBox3.Items.Add(AzureLocation.WestIndia.Name);
            comboBox3.SelectedIndex = 0;
        }

        void DisableAll()
        {
            button1.Enabled = false;
            comboBox1.Enabled = false;
            button3.Enabled = false;
            comboBox3.Enabled = false;
        }

        void EnableAll()
        {
            button1.Enabled = true;
            comboBox1.Enabled = true;
            button3.Enabled = true;
            comboBox3.Enabled = true;
        }

        List<SubscriptionResource> subids = new();

        private void InstallConfig_Load(object sender, EventArgs e)
        {
            List<string> names;
            if (setupType == 1)
            {
                cosmosDeploy.Arm = new();
                (names, subids) = cosmosDeploy.Arm.SetupSubscriptionName();
            }
            else
            {
                sbs.Arm = new();
                (names, subids) = sbs.Arm.SetupSubscriptionName();
            }
            comboBox1.Items.AddRange(names.ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (setupType == 1)
                cosmosDeploy.SelectedSubscription = subids[comboBox1.SelectedIndex];
            else
                sbs.SelectedSubscription = subids[comboBox1.SelectedIndex];

            button1.Enabled = false;
            comboBox1.Enabled = false;
            subids.Clear();
            button3.Enabled = true;
            comboBox3.Enabled = true;
        }

        private void InstallConfig_Closed(object sender, FormClosedEventArgs e)
        {
            dBType.Close();
        }

        private async void Button3_Click(object sender, EventArgs e)
        {
            DisableAll();
            if (setupType == 1)
            {
                cosmosDeploy.SelectedRegion = comboBox3.Text;
                cosmosDeploy.AutoAPI = checkBox1.Checked;

                await cosmosDeploy.Init();
                this.Hide();
                cosmosDeploy.ShowDialog();
            }
            else
            {
                sbs.SelectedRegion = comboBox3.Text;
                sbs.DBType = 1;
                sbs.AutoAPI = checkBox1.Checked;

                ChooseKeyVaultNames1 s = new(sbs, this);
                await s.Init();
                this.Hide();
                s.ShowDialog();
            }
            EnableAll();
        }
    }
}
