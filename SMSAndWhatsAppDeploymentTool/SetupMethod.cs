namespace SMSAndWhatsAppDeploymentTool
{
    public partial class SetupMethod : Form
    {
        public SetupMethod()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            ChooseDBType dbtype = new(this);
            dbtype.ShowDialog();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
