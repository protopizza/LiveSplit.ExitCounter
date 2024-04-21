using LiveSplit.Options;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model.Input;
using System.Threading;

namespace LiveSplit.UI.Components
{
    public partial class ExitCounterComponentSettings : UserControl
    {
        public Color CounterTextColor { get; set; }
        public bool OverrideTextColor { get; set; }

        public string CounterFontString { get { return String.Format("{0} {1}", CounterFont.FontFamily.Name, CounterFont.Style); } }
        public Font CounterFont { get; set; }
        public bool OverrideCounterFont { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public String GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public string ExitCounterText { get; set; }
        public int TotalExitCount { get; set; }
        public bool AutoTotalCount { get; set; }

        public ExitCounterComponentSettings()
        {
            InitializeComponent();

            // Set default values.
            CounterFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);
            OverrideCounterFont = false;
            CounterTextColor = Color.FromArgb(255, 255, 255, 255);
            OverrideTextColor = false;
            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = GradientType.Plain;
            ExitCounterText = "Exits:";
            TotalExitCount = 0;
            AutoTotalCount = true;

            // Set bindings.
            txtExitCounterText.DataBindings.Add("Text", this, "ExitCounterText");
            numTotalExitCount.DataBindings.Add("Value", this, "TotalExitCount");
            chkOverrideFont.DataBindings.Add("Checked", this, "OverrideCounterFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblFontPicker.DataBindings.Add("Text", this, "CounterFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutoTotalCount.DataBindings.Add("Checked", this, "AutoTotalCount", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTxtColor.DataBindings.Add("BackColor", this, "CounterTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);

            // Assign event handlers.
            this.cmbGradientType.SelectedIndexChanged += cmbGradientType_SelectedIndexChanged;
            this.chkOverrideFont.Click += chkFontOverride_CheckedChanged;
            this.chkOverrideColor.Click += chkColorOverride_CheckedChanged;
            this.chkAutoTotalCount.Click += chkAutoTotalCount_CheckedChanged;

            Load += ExitCounterSettings_Load;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            CounterFont = SettingsHelper.GetFontFromElement(element["CounterFont"]); 
            CounterTextColor = SettingsHelper.ParseColor(element["CounterTextColor"]);
            OverrideCounterFont = SettingsHelper.ParseBool(element["OverrideCounterFont"]);
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);
            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"]);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"]);
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"]);
            ExitCounterText = SettingsHelper.ParseString(element["ExitCounterText"]);
            TotalExitCount = SettingsHelper.ParseInt(element["TotalExitCount"]);
            AutoTotalCount = SettingsHelper.ParseBool(element["AutoTotalCount"]);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.0") ^
            SettingsHelper.CreateSetting(document, parent, "CounterFont", CounterFont) ^
            SettingsHelper.CreateSetting(document, parent, "CounterTextColor", CounterTextColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideCounterFont", OverrideCounterFont) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
            SettingsHelper.CreateSetting(document, parent, "ExitCounterText", ExitCounterText) ^
            SettingsHelper.CreateSetting(document, parent, "TotalExitCount", TotalExitCount) ^
            SettingsHelper.CreateSetting(document, parent, "AutoTotalCount", AutoTotalCount);
        }

        private void ExitCounterSettings_Load(object sender, EventArgs e)
        {
            chkColorOverride_CheckedChanged(null, null);
            chkFontOverride_CheckedChanged(null, null);
            chkAutoTotalCount_CheckedChanged(null, null);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(CounterFont, 7, 20);
            dialog.FontChanged += (s, ev) => CounterFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblFontPicker.Text = CounterFontString;
        }

        private void chkColorOverride_CheckedChanged(object sender, EventArgs e)
        {
            lblTxtColor.Enabled = btnTxtColor.Enabled = chkOverrideColor.Checked;
        }

        private void chkFontOverride_CheckedChanged(object sender, EventArgs e)
        {
            lblFont.Enabled = lblFontPicker.Enabled = btnFont.Enabled = chkOverrideFont.Checked;
        }
        
        private void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }
        private void chkAutoTotalCount_CheckedChanged(object sender, EventArgs e)
        {
            numTotalExitCount.Enabled = lblTotalExitCount.Enabled = !chkAutoTotalCount.Checked;
        }

    }
}
