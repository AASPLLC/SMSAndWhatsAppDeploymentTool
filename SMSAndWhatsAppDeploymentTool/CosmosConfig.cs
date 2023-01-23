using Azure.Core;
using Azure.ResourceManager.Resources;

namespace SMSAndWhatsAppDeploymentTool
{
    public partial class CosmosConfig : Form
    {
        public CosmosConfig()
        {
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

        List<SubscriptionResource> subids = new();

        private void InstallConfig_Load(object sender, EventArgs e)
        {
            ChooseDBType.cosmosForm.Arm = new ArmClientHandler();
            //List<string> names = new();
            (List<string> names, subids) = ChooseDBType.cosmosForm.Arm.SetupSubscriptionName();
            comboBox1.Items.AddRange(names.ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ChooseDBType.cosmosForm.SelectedSubscription = subids[comboBox1.SelectedIndex];

            button1.Enabled = false;
            comboBox1.Enabled = false;
            subids.Clear();
            button3.Enabled = true;
            comboBox3.Enabled = true;
        }

        private async void InstallConfig_Closed(object sender, FormClosedEventArgs e)
        {
            await ChooseDBType.cosmosForm.Init();
            ChooseDBType.cosmosForm.ShowDialog();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ChooseDBType.cosmosForm.SelectedRegion = comboBox3.Text;

            button3.Enabled = false;
            comboBox3.Enabled = false;

            this.Close();
        }
    }
}
