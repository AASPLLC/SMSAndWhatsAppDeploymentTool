using Azure.Core;
using Azure.ResourceManager.Resources;
using SMSAndWhatsAppDeploymentTool.JSONParsing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AASPGlobalLibrary;

namespace SMSAndWhatsAppDeploymentTool
{
    public partial class DataverseConfig : Form
    {
        public DataverseConfig()
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
        JSONGetDataverseEnvironments info = new();

        private async void InstallConfig_Load(object sender, EventArgs e)
        {
            ChooseDBType.dataverseForm.Arm = new ArmClientHandler();
            //List<string> names = new();
            (List<string> names, subids) = ChooseDBType.dataverseForm.Arm.SetupSubscriptionName();
            comboBox1.Items.AddRange(names.ToArray());
            comboBox1.SelectedIndex = 0;

            var disco = "https://globaldisco.crm.dynamics.com/api/discovery/v1.0/Instances";

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await TokenHandler.GetGlobalDynamicsImpersonationToken());
            HttpRequestMessage request = new(new HttpMethod("GET"), disco);
            var response = await httpClient.SendAsync(request);
#pragma warning disable CS8601 // Converting null literal or possible null value to non-nullable type.
            info = await response.Content.ReadFromJsonAsync<JSONGetDataverseEnvironments>();
#pragma warning restore CS8601 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8602 // Converting null literal or possible null value to non-nullable type.
            if (info.value.Length > 0)
            {
                for (int i = 0; i < info.value.Length; i++)
                {
                    comboBox2.Items.Add(info.value[i].FriendlyName);
                }
                comboBox2.SelectedIndex = 0;
            }
#pragma warning restore CS8602 // Converting null literal or possible null value to non-nullable type.
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ChooseDBType.dataverseForm.SelectedSubscription = subids[comboBox1.SelectedIndex];

            button1.Enabled = false;
            comboBox1.Enabled = false;
            subids.Clear();
            button3.Enabled = true;
            comboBox3.Enabled = true;
        }

        private async void InstallConfig_Closed(object sender, FormClosedEventArgs e)
        {
            await ChooseDBType.dataverseForm.Init();
            ChooseDBType.dataverseForm.ShowDialog();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ChooseDBType.dataverseForm.SelectedRegion = comboBox3.Text;

            button3.Enabled = false;
            comboBox3.Enabled = false;
            button2.Enabled = true;
            comboBox2.Enabled = true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
#pragma warning disable CS8602 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8601 // Converting null literal or possible null value to non-nullable type.
            ChooseDBType.dataverseForm.SelectedEnvironment =  info.value[comboBox2.SelectedIndex].UrlName;
            ChooseDBType.dataverseForm.SelectedOrgId = info.value[comboBox2.SelectedIndex].Id;
            ChooseDBType.dataverseForm.AutoAPI = checkBox1.Checked;
#pragma warning restore CS8601 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8602 // Converting null literal or possible null value to non-nullable type.

            this.Close();
        }
    }
}
