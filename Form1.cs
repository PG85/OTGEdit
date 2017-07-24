using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.Serialization;
using OTGE.XML;
using OTGE.Utils;

using System.Runtime.InteropServices;

namespace OTGE
{
    public partial class Form1 : Form
    {
        string WorldSaveDir = "";

        SaveFileDialog sfd;
        OpenFileDialog ofd;
        FolderBrowserDialog convertBO3fbd;
        FolderBrowserDialog copyBO3fbd;
        FolderBrowserDialog fbdDestinationWorldDir = new FolderBrowserDialog();
        OpenFileDialog convertBO3ofd;
        ColorDialog colorDlg = new ColorDialog() { AnyColor = true, SolidColorOnly = false };

        List<ListBox> BiomeListInputs = new List<ListBox>();
        Dictionary<object, TCProperty> ResourceQueueInputs = new Dictionary<object, TCProperty>();

        bool IgnorePropertyInputChangedBiome = false;
        bool IgnoreOverrideCheckChangedBiome = false;

        List<BiomeConfig> BiomeConfigsDefaultValues = new List<BiomeConfig>();

        DialogResult addingMultipleResourcesXToAll = DialogResult.Abort;
        DialogResult addingMultipleResourcesXToAll2 = DialogResult.Abort;
        DialogResult addingMultipleResourcesXToAll3 = DialogResult.Abort;
        List<string> resourcesToAdd = new List<string>();
        bool copyPasting = false;

        #region Startup

            public Form1()
            {
                InitializeComponent();

                // TODO: Pass these as method parameters instead of using static fields.
                Session.Form1 = this;
                Session.tabControl1 = tabControl1;
                Session.panel2 = panel2;
                Session.panel3 = panel3;
                Session.btSave = btSave;
                Session.btLoad = btLoad;
                Session.btGenerate = btGenerate;
                Session.rbSummerSkin = rbSummerSkin;
                Session.rbWinterSkin = rbWinterSkin;
                Session.rbNoSkin = rbNoSkin;
                Session.lbGroups = lbGroups;
                Session.btCopyBO3s = btCopyBO3s;
                Session.cbDeleteRegion = cbDeleteRegion;
                Session.cbDeleteRegion.Click += cbDeleteRegion_Click;
                Session.Init();

                this.richTextBox1.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox2.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox3.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox4.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox5.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox6.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox7.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox8.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox9.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox10.LinkClicked += richTextBox_LinkClicked;
                this.richTextBox11.LinkClicked += richTextBox_LinkClicked;

                this.AllowDrop = true;
                this.DragEnter += new DragEventHandler(Form1_DragEnter);
                this.DragDrop += new DragEventHandler(Form1_DragDrop);

                tbSearchWorldConfig.MouseWheel += tbSearchWorldConfig_MouseWheel;
                tbSearchBiomeConfig.MouseWheel += tbSearchBiomeConfig_MouseWheel;
                panel3.MouseWheel += lbBiomesTab_MouseWheel;
                lbGroups.MouseWheel += lbBiomesTab_MouseWheel;
                lbGroup.MouseWheel += lbBiomesTab_MouseWheel;
                lbBiomes.MouseWheel += lbBiomesTab_MouseWheel;

                this.Width = Session.ClosedWidth;
                this.Height = Session.ClosedHeight;

                btSave.Enabled = false;
                btLoad.Enabled = false;

                cbVersion.SelectedIndexChanged += new EventHandler(delegate(object s, EventArgs args)
                {
                    cbWorld.Items.Clear();
                    DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\");
                    if (versionDir3.Exists)
                    {
                        foreach (DirectoryInfo dir2 in versionDir3.GetDirectories())
                        {
                            cbWorld.Items.Add(dir2.Name);
                        }
                        if (cbWorld.Items.Count > 0)
                        {
                            cbWorld.SelectedIndex = 0;
                        }
                    }
                });

                if (Session.VersionDir.Exists)
                {
                    foreach (DirectoryInfo dir1 in Session.VersionDir.GetDirectories())
                    {
                        cbVersion.Items.Add(dir1.Name);
                    }
                    if(cbVersion.Items.Count > 0)
                    {
                        cbVersion.SelectedIndex = 0;
                    }
                }
                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\Saves\\");
                if (!dir.Exists)
                {
                    dir.Create();
                    dir.Refresh();
                }

                sfd = new SaveFileDialog() { DefaultExt = "xml", InitialDirectory = dir.FullName };
                sfd.Title = "Select an OTGE save file";
                ofd = new OpenFileDialog() { DefaultExt = "xml", InitialDirectory = dir.FullName };
                ofd.Title = "Select a save location";

                convertBO3fbd = new FolderBrowserDialog();
                convertBO3fbd.Description = "Select output directory";
                convertBO3ofd = new OpenFileDialog() { DefaultExt = "schematic" };
                convertBO3ofd.Title = "Select schematic(s) to convert";
                convertBO3ofd.Multiselect = true;

                copyBO3fbd = new FolderBrowserDialog();
                copyBO3fbd.Description = "Select a /GlobalObjects or /WorldObjects directory (create if needed).";
            }

            void SelectSourceWorld_Click(object sender, EventArgs e)
            {
                Session.ShowProgessBox();

                tlpBiomeSettingsContainer.Hide();
                tlpWorldSettingsContainer.Hide();

                Session.Form1.SuspendLayout();

                Session.BiomeNames.Clear();
                BiomeListInputs.Clear();
                Session.DestinationConfigsDir = "";
                WorldSaveDir = "";
                Session.SourceConfigsDir = "";
                Session.ToolTip1.RemoveAll();
                ResourceQueueInputs.Clear();
                tlpWorldSettings1.Controls.Clear();
                tlpWorldSettings1.RowStyles.Clear();
                tlpBiomeSettings1.Controls.Clear();
                tlpBiomeSettings1.RowStyles.Clear();

                Session.WorldConfigDefaultValues = null;
                Session.WorldConfig1 = null;
                Session.WorldSettingsInputs.Clear();

                Session.BiomeGroups.Clear();
                BiomeConfigsDefaultValues.Clear();
                Session.BiomeSettingsInputs.Clear();

                lbGroups.Items.Clear();
                lbGroup.Items.Clear();
                lbBiomes.Items.Clear();

                DirectoryInfo versionDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\");
                if (cbVersion.Items.Count > 0 && cbVersion.SelectedItem != null && versionDir.Exists)
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(VersionConfig));
                    using (var reader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\VersionConfig.xml"))
                    {
                        Session.VersionConfig = (VersionConfig)serializer.Deserialize(reader);
                    }
                    if (Session.VersionConfig != null)
                    {
                        Session.SettingsType = Session.VersionConfig.SettingsTypes.FirstOrDefault(a => a.Type == "Forge") ?? Session.VersionConfig.SettingsTypes.First();
                        Session.WorldConfig1 = new WorldConfig(Session.VersionConfig);
                        LoadUI();
                    } else {
                        PopUpForm.CustomMessageBox("Y u do dis? :(");
                    }

                    Session.SourceConfigsDir = Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + cbWorld.SelectedItem + "\\";

                    if (!String.IsNullOrEmpty(Session.SourceConfigsDir) && System.IO.Directory.Exists(Session.SourceConfigsDir + "\\" + "WorldBiomes" + "\\"))
                    {
                        System.IO.DirectoryInfo defaultWorldDirectory = new System.IO.DirectoryInfo(Session.SourceConfigsDir + "\\" + "WorldBiomes" + "\\");
                        foreach (System.IO.FileInfo file in defaultWorldDirectory.GetFiles())
                        {
                            if (file.Name.EndsWith(".bc"))
                            {
                                Session.BiomeNames.Add(file.Name.Replace(".bc", ""));
                            }
                        }
                        foreach (ListBox listBox in BiomeListInputs)
                        {
                            listBox.Items.Clear();
                            foreach (string biomeName in Session.BiomeNames)
                            {
                                listBox.Items.Add(biomeName.Trim());
                            }
                        }
                    }

                    Session.WorldConfigDefaultValues = World.LoadWorldConfigFromFile(new FileInfo(Session.SourceConfigsDir + "WorldConfig.ini"), Session.VersionConfig, true);
                    if (Session.WorldConfigDefaultValues == null)
                    {
                        PopUpForm.CustomMessageBox("WorldConfig.ini could not be loaded. Please make sure that WorldConfig.ini is present in the TCVersionConfig directory for the selected version.", "Incompatible WorldConfig.ini");
                        UnloadUI();
                    } else {
                        LoadBiomesList();
                        if (cbWorld.SelectedItem != null && cbWorld.SelectedItem.Equals("Default"))
                        {
                            LoadDefaultGroups();
                        }
                        if (lbBiomes.Items.Count > 0)
                        {
                            lbBiomes.SelectedIndex = 0;
                        }

                        foreach (Button bSetDefaults in Session.WorldSettingsInputs.Select(a => a.Value.Item3))
                        {
                            Session.ToolTip1.SetToolTip(bSetDefaults, "Set to default");
                            bSetDefaults.Text = "<";
                        }
                        btSetToDefault.Text = "Set to defaults";
                    }

                    Session.tabControl1.Visible = true;
                    Session.btSave.Enabled = true;
                    Session.btLoad.Enabled = true;
                    Session.btGenerate.Visible = true;
                    Session.btCopyBO3s.Visible = true;
                    Session.cbDeleteRegion.Visible = true;

                    Session.rbSummerSkin.Visible = true;
                    Session.rbWinterSkin.Visible = true;
                    Session.rbNoSkin.Visible = true;

                    //label4.Visible = true;
                    //label5.Visible = true;

                    Session.Form1.AutoSize = false;
                    Session.Form1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;

                    if (Session.Form1.Width < Session.OpenedWidth)
                    {
                        Session.Form1.Width = Session.OpenedWidth;
                    }
                    if (Session.Form1.Height < Session.OpenedHeight)
                    {
                        Session.Form1.Height = Session.OpenedHeight;
                    }

                    btClickBackGround(null, null);
                }

                Session.Form1.ResumeLayout();

                tlpBiomeSettingsContainer.Show();
                tlpWorldSettingsContainer.Show();

                Session.HideProgessBox();
            }

            public void UnloadUI()
            {
                tabControl1.Visible = false;
                btGenerate.Visible = false;
                btCopyBO3s.Visible = false;
                rbSummerSkin.Visible = false;
                rbWinterSkin.Visible = false;
                rbNoSkin.Visible = false;
                cbDeleteRegion.Visible = false;
                btSave.Enabled = false;
                btLoad.Enabled = false;
                tlpBiomeSettings1.Visible = false;
                label3.Visible = false;

                AutoSize = false;

                Width = Session.ClosedWidth;
                Height = Session.ClosedHeight;
            }

            public void LoadUI()
            {
                this.Resize += Form1_Resize;
                this.ResizeEnd += Form1_ResizeEnd;

                Session.ToolTip1.RemoveAll();

                Session.ToolTip1.SetToolTip(lblGroups, 
                    "Create biome groups containing one or more biome(s) and configure settings for each group.\r\n" +
                    "If a biome is listed in multiple groups then the order of the groups determines the final value.\r\n" +
                    "For instance you can set a value in the first group and override it in the second.\r\n" +
                    "Settings that accept a list of biomes or resources as a value can even be merged\r\n" +
                    "so you can add gold ore in the first group and diamond ore in the second."
                );

                tabControl1.Visible = false;
                btGenerate.Visible = false;
                btCopyBO3s.Visible = false;
                rbSummerSkin.Visible = false;
                rbWinterSkin.Visible = false;
                rbNoSkin.Visible = false;
                cbDeleteRegion.Visible = false;
                btSave.Enabled = false;
                btLoad.Enabled = false;
                tlpBiomeSettings1.Visible = false;
                label3.Visible = false;

                string uniqueResourceQueueItems = "";
                foreach (ResourceQueueItem resourceQueueItem in Session.VersionConfig.ResourceQueueOptions)
                {
                    if (resourceQueueItem.IsUnique)
                    {
                        if (!resourceQueueItem.HasUniqueParameter)
                        {
                            uniqueResourceQueueItems += resourceQueueItem.Name + "\r\n";
                        }
                        else if (resourceQueueItem.UniqueParameterIndex == 0)
                        {
                            uniqueResourceQueueItems += resourceQueueItem.Name + "*,\r\n";
                        }
                        else if (resourceQueueItem.UniqueParameterIndex == 1)
                        {
                            uniqueResourceQueueItems += resourceQueueItem.Name + "X,*,\r\n";
                        }
                    }
                }

                Dictionary<String, List<TCProperty>> settingsByGroup = new Dictionary<string,List<TCProperty>>();

                // Worlds

                foreach (TCProperty property in Session.VersionConfig.WorldConfig)
                {
                    if (settingsByGroup.ContainsKey(property.Group))
                    {
                        settingsByGroup[property.Group].Add(property);
                    } else {
                        settingsByGroup[property.Group] = new List<TCProperty>() { property };
                    }
                }

                this.SuspendLayout();

                panel2.SuspendLayout();
                panel3.SuspendLayout();

                tlpWorldSettingsContainer.SuspendLayout();
                tlpWorldSettings1.SuspendLayout();

                tlpBiomeSettingsContainer.SuspendLayout();
                tlpBiomeSettings1.SuspendLayout();

                tlpWorldSettings1.Controls.Clear();
                tlpWorldSettings1.RowStyles.Clear();

                tlpBiomeSettings1.Controls.Clear();
                tlpBiomeSettings1.RowStyles.Clear();

                string lastGroupTitle = null;
                int row = 0;

                foreach (KeyValuePair<String, List<TCProperty>> settingsGroup in settingsByGroup)
                {
                    if (!String.IsNullOrEmpty(settingsGroup.Key) && (lastGroupTitle == null || !lastGroupTitle.Equals(settingsGroup.Key)))
                    {
                        lastGroupTitle = settingsGroup.Key;

                        Label txPropertyLabel1 = new Label();
                        txPropertyLabel1.Text = settingsGroup.Key;
                        txPropertyLabel1.Font = new System.Drawing.Font(txPropertyLabel1.Font, FontStyle.Bold);
                        txPropertyLabel1.AutoSize = true;
                        txPropertyLabel1.Margin = new Padding(0, row == 0 ? 10 : 35, 0, 15);
                        txPropertyLabel1.Click += new System.EventHandler(this.btClickBackGround);
                        tlpWorldSettings1.Controls.Add(txPropertyLabel1, 0, row);

                        row++;

                        tlpWorldSettings1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }

                    foreach (TCProperty property in settingsGroup.Value)
                    {
                        Label txPropertyLabel = new Label();
                        txPropertyLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                        txPropertyLabel.Text = property.LabelText != null && property.LabelText.Length > 0 ? property.LabelText : property.Name;
                        txPropertyLabel.AutoSize = true;
                        txPropertyLabel.TabStop = false;
                        txPropertyLabel.Margin = new Padding(0, 5, 0, 0);
                        txPropertyLabel.Click += new System.EventHandler(this.btClickBackGround);
                        tlpWorldSettings1.Controls.Add(txPropertyLabel, 0, row);

                        CheckBox cbOverride = new CheckBox();
                        cbOverride.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        cbOverride.TabStop = false;
                        cbOverride.AutoSize = true;
                        cbOverride.Padding = new Padding(cbOverride.Padding.Left, cbOverride.Padding.Top + 4, cbOverride.Padding.Right, cbOverride.Padding.Bottom);
                        Session.ToolTip1.SetToolTip(cbOverride, "Apply this value");
                        tlpWorldSettings1.Controls.Add(cbOverride, 1, row);
                        cbOverride.CheckedChanged += PropertyInputOverrideCheckChangedWorld;

                        Button bSetDefaults = new Button();
                        bSetDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        bSetDefaults.BackColor = Color.FromKnownColor(KnownColor.Control);
                        bSetDefaults.Text = "C";
                        bSetDefaults.Width = 23;
                        bSetDefaults.Height = 23;
                        bSetDefaults.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
                        bSetDefaults.Click += bSetDefaultsWorldProperty;
                        bSetDefaults.TabStop = false;
                        tlpWorldSettings1.Controls.Add(bSetDefaults, 3, row);

                        switch (property.PropertyType)
                        {
                            case "ResourceQueue":

                                Panel pnl = new Panel();
                                pnl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                pnl.AutoSize = false;
                                pnl.Width = 160;

                                ListBox lbPropertyInput = new ListBox();
                                lbPropertyInput.Sorted = true;
                                lbPropertyInput.Width = pnl.Width;
                                lbPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                lbPropertyInput.SelectionMode = SelectionMode.MultiExtended;
                                lbPropertyInput.KeyDown += lbPropertyInput_KeyDown_World;
                                lbPropertyInput.Height = 140;
                                lbPropertyInput.MouseWheel += lbWorldTabSetting_MouseWheel;
                                lbPropertyInput.MouseHover += lbPropertyInput_MouseHover;
                                pnl.Controls.Add(lbPropertyInput);

                                Panel pnl3 = new Panel();
                                pnl3.Top = 140;
                                pnl3.Width = 160;
                                pnl3.Height = 25;
                                pnl3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                                pnl3.Resize += new EventHandler(delegate(object s, EventArgs args)
                                {
                                    int left = 0;
                                    foreach (Control ctl in ((Panel)s).Controls)
                                    {
                                        ctl.Width = (((Panel)s).Width - 10) / 3;
                                        ctl.Left = left;
                                        left += ((((Panel)s).Width - 10) / 3) + 5;
                                    }                        
                                });

                                Button btAddResourceQueueItem = new Button();
                                btAddResourceQueueItem.Width = 50;
                                btAddResourceQueueItem.Anchor = AnchorStyles.Top;
                                btAddResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                                btAddResourceQueueItem.Text = "Add";
                                btAddResourceQueueItem.Click += btAddResourceQueueItemWorld_Click;
                                btAddResourceQueueItem.TabStop = false;
                                ResourceQueueInputs.Add(btAddResourceQueueItem, property);
                                pnl3.Controls.Add(btAddResourceQueueItem);

                                Button btEditResourceQueueItem = new Button();
                                btEditResourceQueueItem.Left = 55;
                                btEditResourceQueueItem.Width = 50;
                                btEditResourceQueueItem.Anchor = AnchorStyles.Top;
                                btEditResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                                btEditResourceQueueItem.Text = "Edit";
                                btEditResourceQueueItem.Click += btEditResourceQueueItemWorld_Click;
                                btEditResourceQueueItem.TabStop = false;
                                ResourceQueueInputs.Add(btEditResourceQueueItem, property);
                                pnl3.Controls.Add(btEditResourceQueueItem);

                                Button btDeleteResourceQueueItem = new Button();
                                btDeleteResourceQueueItem.Left = 110;
                                btDeleteResourceQueueItem.Width = 50;
                                btDeleteResourceQueueItem.Anchor = AnchorStyles.Top;
                                btDeleteResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);                               
                                btDeleteResourceQueueItem.Text = "Delete";
                                btDeleteResourceQueueItem.Click += btDeleteResourceQueueItemWorld_Click;
                                btDeleteResourceQueueItem.TabStop = false;
                                ResourceQueueInputs.Add(btDeleteResourceQueueItem, property);
                                pnl3.Controls.Add(btDeleteResourceQueueItem);
                           
                                Panel pCheckBoxes = new Panel();
                                pCheckBoxes.Top = 172;
                                pCheckBoxes.Width = 160;
                                pCheckBoxes.Height = 70;
                                pCheckBoxes.Visible = false;
                                pCheckBoxes.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

                                CheckBox btIgnoreParentMerge = new CheckBox();
                                btIgnoreParentMerge.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btIgnoreParentMerge.AutoSize = true;
                                btIgnoreParentMerge.Text = "Override parent values";
                                btIgnoreParentMerge.Name = "OverrideParent";
                                btIgnoreParentMerge.Enabled = false;
                                btIgnoreParentMerge.Visible = false;
                                btIgnoreParentMerge.CheckedChanged += btOverrideParentValuesWorld_CheckedChanged;
                                pCheckBoxes.Controls.Add(btIgnoreParentMerge);
                                Session.ToolTip1.SetToolTip(btIgnoreParentMerge, "Ignore any resources defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add resources to the same setting.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore to ResourceQueue.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");

                                RadioButton btMergeWithDefaults = new RadioButton();
                                btMergeWithDefaults.Top = 25;
                                btMergeWithDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btMergeWithDefaults.AutoSize = true;
                                btMergeWithDefaults.Text = "Merge with defaults";
                                btMergeWithDefaults.Enabled = false;
                                btMergeWithDefaults.Visible = false;
                                btMergeWithDefaults.Name = "Merge";
                                btMergeWithDefaults.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                                pCheckBoxes.Controls.Add(btMergeWithDefaults);
                                Session.ToolTip1.SetToolTip(btMergeWithDefaults, "Adds the selected resources to the default resources.\r\n\r\nSome resource and parameter combinations are configured as \"must be unique\" in the VersionConfig.xml and will always be \r\noverridden, for instance Ore(GOLD_ORE, which means the values configured in this list will replace \r\nany existing Ore(GOLD_ORE resources. Unique resources are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nResource name and * must be unique.\r\n\r\nResources that have a block as a unique parameter (such as ORE(Block,...)) can be configured to\r\nbe unique only when used with specific blocks (like GOLD_ORE, IRON_ORE etc).\r\n\r\nUnique resources, parameters and lists of blocks can be configured in the VersionConfig.xml.\r\n\r\nUpdate: Unique resources can now be added multiple times, duplicates are allowed but only within a single list.\r\nWhen merging resource lists with other biome groups and default values merging behaviours are applied and\r\nduplicates between lists are removed.");

                                RadioButton btOverrideAll = new RadioButton();
                                btOverrideAll.Top = 50;
                                btOverrideAll.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btOverrideAll.AutoSize = true;
                                btOverrideAll.Text = "Override defaults";
                                btOverrideAll.Checked = true;
                                btOverrideAll.Enabled = false;
                                btOverrideAll.Visible = false;
                                btOverrideAll.Name = "Override";
                                btOverrideAll.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                                pCheckBoxes.Controls.Add(btOverrideAll);
                                Session.ToolTip1.SetToolTip(btOverrideAll, "Removes all default resources and replaces them with selected resources.");

                                pCheckBoxes.AutoSize = true;
                                pCheckBoxes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

                                pnl.Controls.Add(pnl3);
                                pnl.Controls.Add(pCheckBoxes);

                                pnl.AutoSize = true;

                                tlpWorldSettings1.Controls.Add(pnl, 2, row);
                                Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes));

                                break;
                            case "String":
                                if(property.AllowedValues != null && property.AllowedValues.Count > 0)
                                {
                                    ComboBox txPropertyInput = new ComboBox();
                                    txPropertyInput.DropDownStyle = ComboBoxStyle.DropDownList;
                                    txPropertyInput.Items.Add("");
                                    foreach(string allowedValue in property.AllowedValues)
                                    {
                                        txPropertyInput.Items.Add(allowedValue);
                                    }
                                    txPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                    txPropertyInput.SelectedIndexChanged += PropertyInputChangedWorld;
                                    txPropertyInput.LostFocus += PropertyInputLostFocusWorld;
                                    txPropertyInput.MouseHover += lbPropertyInput_MouseHover;
                                    txPropertyInput.MouseWheel += lbWorldTabSetting_MouseWheel;

                                    tlpWorldSettings1.Controls.Add(txPropertyInput, 2, row);
                                    Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                                } else {
                                    TextBox txPropertyInput = new TextBox();                                
                                    txPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                    txPropertyInput.TextChanged += PropertyInputChangedWorld;
                                    txPropertyInput.LostFocus += PropertyInputLostFocusWorld;
                                    txPropertyInput.MouseHover += lbPropertyInput_MouseHover;

                                    tlpWorldSettings1.Controls.Add(txPropertyInput, 2, row);
                                    Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                                }
                                break;
                            case "BigString":
                                RichTextBox txPropertyInput2 = new RichTextBox();
                                txPropertyInput2.Multiline = true;
                                txPropertyInput2.Height = 58;
                                txPropertyInput2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                txPropertyInput2.TextChanged += PropertyInputChangedWorld;
                                txPropertyInput2.LostFocus += PropertyInputLostFocusWorld;
                                txPropertyInput2.MouseHover += lbPropertyInput_MouseHover;
                                txPropertyInput2.MouseWheel += lbWorldTabSetting_MouseWheel;

                                tlpWorldSettings1.Controls.Add(txPropertyInput2, 2, row);
                                Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput2, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                                break;
                            case "Float":
                            case "Int":
                                NumericUpDownExt txPropertyInput3 = new NumericUpDownExt(property.PropertyType == "Float");
                                txPropertyInput3.Minimum = Convert.ToInt32(Math.Ceiling(property.MinValue));
                                txPropertyInput3.Maximum = Convert.ToInt32(Math.Floor(property.MaxValue));
                                txPropertyInput3.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                txPropertyInput3.TextChanged += PropertyInputChangedWorld;
                                txPropertyInput3.LostFocus += PropertyInputLostFocusWorld;
                                // This is needed to make the table cell/column/row resize correctly
                                // This doesn't actually seem to affect the textbox's height/width.
                                // TODO: Is this still needed, using a hack on window resize now?
                                txPropertyInput3.AutoSize = false;
                                txPropertyInput3.Width = 0;
                                txPropertyInput3.Height = 0;

                                tlpWorldSettings1.Controls.Add(txPropertyInput3, 2, row);
                                Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput3, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                                break;
                            case "Color":
                                Panel colorPickerPanel = new Panel();
                                colorPickerPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                colorPickerPanel.AutoSize = false;
                                colorPickerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                                colorPickerPanel.Width = 0;
                                colorPickerPanel.Height = 0;

                                ListBox lbPropertyInput2 = new ListBox();
                                lbPropertyInput2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                lbPropertyInput2.IntegralHeight = false;
                                lbPropertyInput2.Width = 20;
                                lbPropertyInput2.Height = 20;
                                lbPropertyInput2.BackColor = Color.White;
                                lbPropertyInput2.Margin = new Padding(3, 0, 0, 0);
                                lbPropertyInput2.TabStop = false;
                                lbPropertyInput2.Click += PropertyInputColorChangedWorld;
                                colorPickerPanel.Controls.Add(lbPropertyInput2);

                                TextBox txPropertyInput4 = new TextBox();
                                txPropertyInput4.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                txPropertyInput4.TextChanged += PropertyInputColorChangedWorld;
                                txPropertyInput4.LostFocus += PropertyInputLostFocusWorld;
                                txPropertyInput4.Left = 26;
                                colorPickerPanel.Controls.Add(txPropertyInput4);

                                colorPickerPanel.AutoSize = true;

                                tlpWorldSettings1.Controls.Add(colorPickerPanel, 2, row);
                                Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput4, cbOverride, bSetDefaults, txPropertyLabel, lbPropertyInput2, null));
                                break;
                            case "BiomesList":

                                Panel pnl4 = new Panel();
                                pnl4.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                pnl4.AutoSize = false;
                                pnl4.Width = 160;

                                ListBox lbPropertyInput3 = new ListBox();
                                lbPropertyInput3.KeyDown += lbPropertyInput_KeyDown_World;
                                lbPropertyInput3.Sorted = true;
                                lbPropertyInput3.Width = pnl4.Width;
                                lbPropertyInput3.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                lbPropertyInput3.SelectionMode = SelectionMode.MultiExtended;
                                lbPropertyInput3.SelectedIndexChanged += lbPropertyInputWorld_SelectedIndexChanged;
                                lbPropertyInput3.Height = 140;
                                lbPropertyInput3.MouseWheel += lbWorldTabSetting_MouseWheel;
                                lbPropertyInput3.MouseHover += lbPropertyInput_MouseHover;
                                pnl4.Controls.Add(lbPropertyInput3);
                           
                                Panel pCheckBoxes2 = new Panel();
                                pCheckBoxes2.Top = 144;
                                pCheckBoxes2.Width = 160;
                                pCheckBoxes2.Height = 70;
                                pCheckBoxes2.Visible = false;
                                pCheckBoxes2.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

                                CheckBox btIgnoreParentMerge2 = new CheckBox();
                                btIgnoreParentMerge2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btIgnoreParentMerge2.AutoSize = true;
                                btIgnoreParentMerge2.Text = "Override parent values";
                                btIgnoreParentMerge2.Name = "OverrideParent";
                                btIgnoreParentMerge2.Enabled = false;
                                btIgnoreParentMerge2.Visible = false;
                                btIgnoreParentMerge2.CheckedChanged += btOverrideParentValuesWorld_CheckedChanged;
                                pCheckBoxes2.Controls.Add(btIgnoreParentMerge2);
                                Session.ToolTip1.SetToolTip(btIgnoreParentMerge2, "Ignore any biomes defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add biomes to the same setting.\r\n For instance Group 1 can add biome A and Group 2 can add biome B.\r\n\r\nIf this is enabled then only the current group will add its biomes.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds biome A and Group 2 adds biome B only biome B is added.");

                                RadioButton btMergeWithDefaults2 = new RadioButton();
                                btMergeWithDefaults2.Top = 25;
                                btMergeWithDefaults2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btMergeWithDefaults2.AutoSize = true;
                                btMergeWithDefaults2.Text = "Merge with defaults";
                                btMergeWithDefaults2.Enabled = false;
                                btMergeWithDefaults2.Visible = false;
                                btMergeWithDefaults2.Name = "Merge";
                                btMergeWithDefaults2.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                                pCheckBoxes2.Controls.Add(btMergeWithDefaults2);
                                Session.ToolTip1.SetToolTip(btMergeWithDefaults2, "Adds the selected biomes to the default biomes.");

                                RadioButton btOverrideAll2 = new RadioButton();
                                btOverrideAll2.Top = 50;
                                btOverrideAll2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btOverrideAll2.AutoSize = true;
                                btOverrideAll2.Text = "Override defaults";
                                btOverrideAll2.Checked = true;
                                btOverrideAll2.Enabled = false;
                                btOverrideAll2.Visible = false;
                                btOverrideAll2.Name = "Override";
                                btOverrideAll2.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                                pCheckBoxes2.Controls.Add(btOverrideAll2);
                                Session.ToolTip1.SetToolTip(btOverrideAll2, "Removes all default biomes and replaces them with selected biomes.");

                                pCheckBoxes2.AutoSize = true;
                                pCheckBoxes2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

                                pnl4.Controls.Add(pCheckBoxes2);
                                pnl4.AutoSize = true;

                                tlpWorldSettings1.Controls.Add(pnl4, 2, row);
                                Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput3, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes2));
                                BiomeListInputs.Add(lbPropertyInput3);

                                break;
                            case "Bool":
                                Button btnTrueFalse = new Button();
                                btnTrueFalse.Text = "";
                                btnTrueFalse.BackColor = Color.FromKnownColor(KnownColor.Control);
                                btnTrueFalse.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btnTrueFalse.ForeColor = Color.Empty;
                                btnTrueFalse.Click += btnTrueFalseWorld_Click;

                                tlpWorldSettings1.Controls.Add(btnTrueFalse, 2, row);
                                Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(btnTrueFalse, cbOverride, bSetDefaults, txPropertyLabel, null, null));

                                break;
                        }

                        Session.ToolTip1.SetToolTip(bSetDefaults, "Clear");

                        row++;
                    }
                }

                // Biomes

                settingsByGroup = new Dictionary<string, List<TCProperty>>();

                foreach (TCProperty property in Session.VersionConfig.BiomeConfig)
                {
                    if (settingsByGroup.ContainsKey(property.Group))
                    {
                        settingsByGroup[property.Group].Add(property);
                    } else {
                        settingsByGroup[property.Group] = new List<TCProperty>() { property };
                    }
                }

                lastGroupTitle = null;
                row = 0;

                foreach (KeyValuePair<String, List<TCProperty>> settingsGroup in settingsByGroup)
                {
                    if (!String.IsNullOrEmpty(settingsGroup.Key) && (lastGroupTitle == null || !lastGroupTitle.Equals(settingsGroup.Key)))
                    {
                        lastGroupTitle = settingsGroup.Key;

                        Label txPropertyLabel1 = new Label();
                        txPropertyLabel1.Text = settingsGroup.Key;
                        txPropertyLabel1.Font = new System.Drawing.Font(txPropertyLabel1.Font, FontStyle.Bold);
                        txPropertyLabel1.AutoSize = true;
                        txPropertyLabel1.Margin = new Padding(0, row == 0 ? 10 : 35, 0, 15);
                        txPropertyLabel1.Click += new System.EventHandler(this.btClickBackGround);
                        tlpBiomeSettings1.Controls.Add(txPropertyLabel1, 0, row);

                        row++;

                        tlpBiomeSettings1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }

                    foreach (TCProperty property in settingsGroup.Value)
                    {
                        Label txPropertyLabel = new Label();
                        txPropertyLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                        txPropertyLabel.Text = property.LabelText != null && property.LabelText.Length > 0 ? property.LabelText : property.Name;
                        txPropertyLabel.AutoSize = true;
                        txPropertyLabel.TabStop = false;
                        txPropertyLabel.Margin = new Padding(0, 5, 0, 0);
                        txPropertyLabel.Click += new System.EventHandler(this.btClickBackGround);
                        tlpBiomeSettings1.Controls.Add(txPropertyLabel, 0, row);

                        CheckBox cbOverride = new CheckBox();
                        cbOverride.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        cbOverride.TabStop = false;
                        cbOverride.AutoSize = true;
                        cbOverride.Padding = new Padding(cbOverride.Padding.Left, cbOverride.Padding.Top + 4, cbOverride.Padding.Right, cbOverride.Padding.Bottom);
                        Session.ToolTip1.SetToolTip(cbOverride, "Apply this value");
                        tlpBiomeSettings1.Controls.Add(cbOverride, 1, row);
                        cbOverride.CheckedChanged += PropertyInputOverrideCheckChangedBiome;

                        Button bSetDefaults = new Button();
                        bSetDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        bSetDefaults.BackColor = Color.FromKnownColor(KnownColor.Control);
                        bSetDefaults.Text = "C";
                        bSetDefaults.Width = 23;
                        bSetDefaults.Height = 23;
                        bSetDefaults.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
                        bSetDefaults.Click += bSetDefaultsBiomeProperty;
                        bSetDefaults.TabStop = false;
                        tlpBiomeSettings1.Controls.Add(bSetDefaults, 3, row);

                        switch (property.PropertyType)
                        {
                            case "ResourceQueue":

                                Panel pnl = new Panel();
                                pnl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                pnl.AutoSize = false;
                                pnl.Width = 160;

                                ListBox lbPropertyInput = new ListBox();
                                lbPropertyInput.Sorted = true;
                                lbPropertyInput.Width = pnl.Width;
                                lbPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                lbPropertyInput.SelectionMode = SelectionMode.MultiExtended;
                                lbPropertyInput.KeyDown += lbPropertyInput_KeyDown;
                                lbPropertyInput.Height = 140;
                                lbPropertyInput.MouseWheel += lbBiomesTabSetting_MouseWheel;
                                lbPropertyInput.MouseHover += lbPropertyInput_MouseHover;
                                pnl.Controls.Add(lbPropertyInput);

                                Panel pnl3 = new Panel();
                                pnl3.Top = 140;
                                pnl3.Width = 160;
                                pnl3.Height = 25;
                                pnl3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                                pnl3.Resize += new EventHandler(delegate(object s, EventArgs args)
                                {
                                    int left = 0;
                                    foreach (Control ctl in ((Panel)s).Controls)
                                    {
                                        ctl.Width = (((Panel)s).Width - 10) / 3;
                                        ctl.Left = left;
                                        left += ((((Panel)s).Width - 10) / 3) + 5;
                                    }                        
                                });

                                Button btAddResourceQueueItem = new Button();
                                btAddResourceQueueItem.Width = 50;
                                btAddResourceQueueItem.Anchor = AnchorStyles.Top;
                                btAddResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                                btAddResourceQueueItem.Text = "Add";
                                btAddResourceQueueItem.Click += btAddResourceQueueItem_Click;
                                btAddResourceQueueItem.TabStop = false;
                                ResourceQueueInputs.Add(btAddResourceQueueItem, property);
                                pnl3.Controls.Add(btAddResourceQueueItem);

                                Button btEditResourceQueueItem = new Button();
                                btEditResourceQueueItem.Left = 55;
                                btEditResourceQueueItem.Width = 50;
                                btEditResourceQueueItem.Anchor = AnchorStyles.Top;
                                btEditResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                                btEditResourceQueueItem.Text = "Edit";
                                btEditResourceQueueItem.Click += btEditResourceQueueItem_Click;
                                btEditResourceQueueItem.TabStop = false;
                                ResourceQueueInputs.Add(btEditResourceQueueItem, property);
                                pnl3.Controls.Add(btEditResourceQueueItem);

                                Button btDeleteResourceQueueItem = new Button();
                                btDeleteResourceQueueItem.Left = 110;
                                btDeleteResourceQueueItem.Width = 50;
                                btDeleteResourceQueueItem.Anchor = AnchorStyles.Top;
                                btDeleteResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);                               
                                btDeleteResourceQueueItem.Text = "Delete";
                                btDeleteResourceQueueItem.Click += btDeleteResourceQueueItem_Click;
                                btDeleteResourceQueueItem.TabStop = false;
                                ResourceQueueInputs.Add(btDeleteResourceQueueItem, property);
                                pnl3.Controls.Add(btDeleteResourceQueueItem);
                           
                                Panel pCheckBoxes = new Panel();
                                pCheckBoxes.Top = 172;
                                pCheckBoxes.Width = 160;
                                pCheckBoxes.Height = 70;
                                pCheckBoxes.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

                                CheckBox btIgnoreParentMerge = new CheckBox();
                                btIgnoreParentMerge.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btIgnoreParentMerge.AutoSize = true;
                                btIgnoreParentMerge.Text = "Override parent values";
                                btIgnoreParentMerge.Name = "OverrideParent";
                                btIgnoreParentMerge.CheckedChanged += btOverrideParentValuesBiome_CheckedChanged;
                                pCheckBoxes.Controls.Add(btIgnoreParentMerge);
                                Session.ToolTip1.SetToolTip(btIgnoreParentMerge, "Ignore any resources defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add resources to the same setting.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore to ResourceQueue.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");

                                RadioButton btMergeWithDefaults = new RadioButton();
                                btMergeWithDefaults.Top = 25;
                                btMergeWithDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btMergeWithDefaults.AutoSize = true;
                                btMergeWithDefaults.Text = "Merge with defaults";
                                btMergeWithDefaults.Name = "Merge";
                                btMergeWithDefaults.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                                pCheckBoxes.Controls.Add(btMergeWithDefaults);
                                Session.ToolTip1.SetToolTip(btMergeWithDefaults, "Adds the selected resources to the default resources.\r\n\r\nSome resource and parameter combinations are configured as \"must be unique\" in the VersionConfig.xml and will always be \r\noverridden, for instance Ore(GOLD_ORE, which means the values configured in this list will replace \r\nany existing Ore(GOLD_ORE resources. Unique resources are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nResource name and * must be unique.\r\n\r\nResources that have a block as a unique parameter (such as ORE(Block,...)) can be configured to\r\nbe unique only when used with specific blocks (like GOLD_ORE, IRON_ORE etc).\r\n\r\nUnique resources, parameters and lists of blocks can be configured in the VersionConfig.xml.\r\n\r\nUpdate: Unique resources can now be added multiple times, duplicates are allowed but only within a single list.\r\nWhen merging resource lists with other biome groups and default values merging behaviours are applied and\r\nduplicates between lists are removed.");

                                RadioButton btOverrideAll = new RadioButton();
                                btOverrideAll.Top = 50;
                                btOverrideAll.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btOverrideAll.AutoSize = true;
                                btOverrideAll.Text = "Override defaults";
                                btOverrideAll.Checked = true;
                                btOverrideAll.Name = "Override";
                                btOverrideAll.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                                pCheckBoxes.Controls.Add(btOverrideAll);
                                Session.ToolTip1.SetToolTip(btOverrideAll, "Removes all default resources and replaces them with selected resources.");

                                pCheckBoxes.AutoSize = true;
                                pCheckBoxes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

                                pnl.Controls.Add(pnl3);
                                pnl.Controls.Add(pCheckBoxes);

                                pnl.AutoSize = true;

                                tlpBiomeSettings1.Controls.Add(pnl, 2, row);
                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes));

                                break;
                            case "String":
                                if(property.AllowedValues != null && property.AllowedValues.Count > 0)
                                {
                                    ComboBox txPropertyInput = new ComboBox();
                                    txPropertyInput.DropDownStyle = ComboBoxStyle.DropDownList;
                                    txPropertyInput.Items.Add("");
                                    foreach(string allowedValue in property.AllowedValues)
                                    {
                                        txPropertyInput.Items.Add(allowedValue);
                                    }
                                    txPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                    txPropertyInput.SelectedIndexChanged += PropertyInputChangedBiome;
                                    txPropertyInput.LostFocus += PropertyInputLostFocusBiome;
                                    txPropertyInput.MouseHover += lbPropertyInput_MouseHover;
                                    txPropertyInput.MouseWheel += lbBiomesTabSetting_MouseWheel;

                                    tlpBiomeSettings1.Controls.Add(txPropertyInput, 2, row);
                                    Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                                } else {
                                    TextBox txPropertyInput = new TextBox();                                
                                    txPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;                                
                                    txPropertyInput.TextChanged += PropertyInputChangedBiome;
                                    txPropertyInput.LostFocus += PropertyInputLostFocusBiome;
                                    txPropertyInput.MouseHover += lbPropertyInput_MouseHover;

                                    tlpBiomeSettings1.Controls.Add(txPropertyInput, 2, row);
                                    Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                                }
                                break;
                            case "BigString":
                                RichTextBox txPropertyInput2 = new RichTextBox();
                                txPropertyInput2.Height = 58;
                                txPropertyInput2.Multiline = true;
                                txPropertyInput2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;                                
                                txPropertyInput2.TextChanged += PropertyInputChangedBiome;
                                txPropertyInput2.LostFocus += PropertyInputLostFocusBiome;
                                txPropertyInput2.MouseHover += lbPropertyInput_MouseHover;
                                txPropertyInput2.MouseWheel += lbBiomesTabSetting_MouseWheel;

                                tlpBiomeSettings1.Controls.Add(txPropertyInput2, 2, row);
                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput2, cbOverride, bSetDefaults, txPropertyLabel, null, null));                                
                                break;
                            case "Float":
                            case "Int":
                                NumericUpDownExt txPropertyInput3 = new NumericUpDownExt(property.PropertyType == "Float");
                                txPropertyInput3.Minimum = Convert.ToInt32(Math.Ceiling(property.MinValue));
                                txPropertyInput3.Maximum = Convert.ToInt32(Math.Floor(property.MaxValue));
                                txPropertyInput3.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                txPropertyInput3.TextChanged += PropertyInputChangedBiome;
                                txPropertyInput3.LostFocus += PropertyInputLostFocusBiome;
                                // This is needed to make the table cell/column/row resize correctly
                                // This doesn't actually seem to affect the textbox's height/width.
                                // TODO: Is this still needed, using a hack on window resize now?
                                txPropertyInput3.AutoSize = false;
                                txPropertyInput3.Width = 0;
                                txPropertyInput3.Height = 0;

                                tlpBiomeSettings1.Controls.Add(txPropertyInput3, 2, row);
                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput3, cbOverride, bSetDefaults, txPropertyLabel, null, null));                                
                                break;
                            case "Color":
                                Panel colorPickerPanel = new Panel();
                                colorPickerPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                colorPickerPanel.AutoSize = false;
                                colorPickerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                                colorPickerPanel.Width = 0;
                                colorPickerPanel.Height = 0;

                                ListBox lbPropertyInput2 = new ListBox();
                                lbPropertyInput2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                lbPropertyInput2.IntegralHeight = false;
                                lbPropertyInput2.Width = 20;
                                lbPropertyInput2.Height = 20;
                                lbPropertyInput2.BackColor = Color.White;
                                lbPropertyInput2.Margin = new Padding(3, 0, 0, 0);
                                lbPropertyInput2.TabStop = false;
                                lbPropertyInput2.Click += PropertyInputColorChangedBiome;
                                colorPickerPanel.Controls.Add(lbPropertyInput2);

                                TextBox txPropertyInput4 = new TextBox();
                                txPropertyInput4.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                txPropertyInput4.TextChanged += PropertyInputColorChangedBiome;
                                txPropertyInput4.LostFocus += PropertyInputLostFocusBiome;
                                txPropertyInput4.Left = 26;
                                colorPickerPanel.Controls.Add(txPropertyInput4);

                                colorPickerPanel.AutoSize = true;

                                tlpBiomeSettings1.Controls.Add(colorPickerPanel, 2, row);
                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput4, cbOverride, bSetDefaults, txPropertyLabel, lbPropertyInput2, null));
                                break;
                            case "BiomesList":

                                Panel pnl4 = new Panel();
                                pnl4.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                pnl4.AutoSize = false;
                                pnl4.Width = 160;

                                ListBox lbPropertyInput3 = new ListBox();
                                lbPropertyInput3.KeyDown += lbPropertyInput_KeyDown;
                                lbPropertyInput3.Sorted = true;
                                lbPropertyInput3.Width = pnl4.Width;
                                lbPropertyInput3.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                lbPropertyInput3.SelectionMode = SelectionMode.MultiExtended;
                                lbPropertyInput3.SelectedIndexChanged += lbPropertyInputBiome_SelectedIndexChanged;
                                lbPropertyInput3.Height = 140;
                                lbPropertyInput3.MouseWheel += lbBiomesTabSetting_MouseWheel;
                                lbPropertyInput3.MouseHover += lbPropertyInput_MouseHover;
                                pnl4.Controls.Add(lbPropertyInput3);
                           
                                Panel pCheckBoxes2 = new Panel();
                                pCheckBoxes2.Top = 144;
                                pCheckBoxes2.Width = 160;
                                pCheckBoxes2.Height = 70;
                                pCheckBoxes2.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

                                CheckBox btIgnoreParentMerge2 = new CheckBox();
                                btIgnoreParentMerge2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btIgnoreParentMerge2.AutoSize = true;
                                btIgnoreParentMerge2.Text = "Override parent values";
                                btIgnoreParentMerge2.Name = "OverrideParent";
                                btIgnoreParentMerge2.CheckedChanged += btOverrideParentValuesBiome_CheckedChanged;
                                pCheckBoxes2.Controls.Add(btIgnoreParentMerge2);
                                Session.ToolTip1.SetToolTip(btIgnoreParentMerge2, "Ignore any biomes defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add biomes to the same setting.\r\n For instance Group 1 can add biome A and Group 2 can add biome B.\r\n\r\nIf this is enabled then only the current group will add its biomes.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds biome A and Group 2 adds biome B only biome B is added.");

                                RadioButton btMergeWithDefaults2 = new RadioButton();
                                btMergeWithDefaults2.Top = 25;
                                btMergeWithDefaults2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btMergeWithDefaults2.AutoSize = true;
                                btMergeWithDefaults2.Text = "Merge with defaults";
                                btMergeWithDefaults2.Name = "Merge";
                                btMergeWithDefaults2.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                                pCheckBoxes2.Controls.Add(btMergeWithDefaults2);
                                Session.ToolTip1.SetToolTip(btMergeWithDefaults2, "Adds the selected biomes to the default biomes.");

                                RadioButton btOverrideAll2 = new RadioButton();
                                btOverrideAll2.Top = 50;
                                btOverrideAll2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btOverrideAll2.AutoSize = true;
                                btOverrideAll2.Text = "Override defaults";
                                btOverrideAll2.Checked = true;
                                btOverrideAll2.Name = "Override";
                                btOverrideAll2.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                                pCheckBoxes2.Controls.Add(btOverrideAll2);
                                Session.ToolTip1.SetToolTip(btOverrideAll2, "Removes all default biomes and replaces them with selected biomes.");

                                pCheckBoxes2.AutoSize = true;
                                pCheckBoxes2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

                                pnl4.Controls.Add(pCheckBoxes2);

                                pnl4.AutoSize = true;

                                tlpBiomeSettings1.Controls.Add(pnl4, 2, row);
                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput3, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes2));
                                BiomeListInputs.Add(lbPropertyInput3);

                                break;
                            case "Bool":
                                Button btnTrueFalse = new Button();
                                btnTrueFalse.Text = "";
                                btnTrueFalse.BackColor = Color.FromKnownColor(KnownColor.Control);
                                btnTrueFalse.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btnTrueFalse.ForeColor = Color.Empty;
                                btnTrueFalse.Click += btnTrueFalseBiome_Click;

                                tlpBiomeSettings1.Controls.Add(btnTrueFalse, 2, row);
                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(btnTrueFalse, cbOverride, bSetDefaults, txPropertyLabel, null, null));

                                break;
                        }

                        Session.ToolTip1.SetToolTip(bSetDefaults, "Clear");

                        row++;
                    }
                }

                tlpWorldSettings1.ResumeLayout();

                tlpWorldSettingsContainer.ResumeLayout();

                tlpBiomeSettings1.ResumeLayout();

                tlpBiomeSettingsContainer.ResumeLayout();

                panel3.ResumeLayout();
                panel2.ResumeLayout();

                this.ResumeLayout();
            }

        #endregion

        #region World

            private void btOverrideParentValuesWorld_CheckedChanged(object sender, EventArgs e)
            {
                if (!Session.IgnorePropertyInputChangedWorld)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item6 == ((Control)sender).Parent);
                    TCProperty property = kvp.Key;

                    Session.WorldConfig1.SetProperty(property, Session.WorldConfig1.GetPropertyValueAsString(property), Session.WorldConfig1.GetPropertyMerge(property), ((CheckBox)sender).Checked);
                }

                this.panel2.Focus();
            }

            private void btOverrideAllWorld_CheckedChanged(object sender, EventArgs e)
            {
                if (!Session.IgnorePropertyInputChangedWorld && ((RadioButton)sender).Checked)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item6 == ((Control)sender).Parent);
                    Control tb = kvp.Value.Item1;
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    bool merge = false;

                    ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                    ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;

                    if (sender == kvp.Value.Item6.Controls.Find("Override", true)[0])
                    {
                        merge = !((RadioButton)sender).Checked;
                    }
                    else if (sender == kvp.Value.Item6.Controls.Find("Merge", true)[0])
                    {
                        merge = ((RadioButton)sender).Checked;
                    }
                    bool defaultValue = Session.WorldConfigDefaultValues.GetPropertyMerge(property);
                    Session.WorldConfig1.SetProperty(property, Session.WorldConfig1.GetPropertyValueAsString(property), merge, ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    if(property.PropertyType == "BiomesList")
                    {
                        if ((!String.IsNullOrEmpty(Session.WorldConfig1.GetPropertyValueAsString(property)) && !Utils.TCSettingsUtils.CompareBiomeLists(Session.WorldConfig1.GetPropertyValueAsString(property), Session.WorldConfigDefaultValues.GetPropertyValueAsString(property))) || (Session.WorldConfig1.GetPropertyValueAsString(property) != null && !Utils.TCSettingsUtils.CompareBiomeLists(Session.WorldConfig1.GetPropertyValueAsString(property), Session.WorldConfigDefaultValues.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            cb.Checked = false;
                        }
                    }
                    else if (property.PropertyType == "ResourceQueue")
                    {
                        if ((!String.IsNullOrEmpty(Session.WorldConfig1.GetPropertyValueAsString(property)) && !Utils.TCSettingsUtils.CompareResourceQueues(Session.WorldConfig1.GetPropertyValueAsString(property), Session.WorldConfigDefaultValues.GetPropertyValueAsString(property))) || (Session.WorldConfig1.GetPropertyValueAsString(property) != null && !Utils.TCSettingsUtils.CompareResourceQueues(Session.WorldConfig1.GetPropertyValueAsString(property), Session.WorldConfigDefaultValues.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            cb.Checked = false;
                        }
                    }
                }

                this.panel2.Focus();
            }

            private void lbPropertyInputWorld_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (!Session.IgnorePropertyInputChangedWorld)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item1 == sender);
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    List<string> biomeNames = new List<string>();
                    string sBiomeNames = "";
                    foreach(string s in ((ListBox)kvp.Value.Item1).SelectedItems)
                    {
                        biomeNames.Add(s);
                        sBiomeNames += sBiomeNames.Length == 0 ? s : ", " + s;
                    }
                    bool merge = false;
                    if (sender == kvp.Value.Item6.Controls.Find("Override", true)[0])
                    {
                        merge = !((RadioButton)sender).Checked;
                    }
                    else if (sender == kvp.Value.Item6.Controls.Find("Merge", true)[0])
                    {
                        merge = ((RadioButton)sender).Checked;
                    }
                    Session.WorldConfig1.SetProperty(property, sBiomeNames, merge, ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    bool bIsDefault = true;
                    string defaultValue = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                    string newValue = Session.WorldConfig1.GetPropertyValueAsString(property);
                    bIsDefault = Utils.TCSettingsUtils.CompareBiomeLists(defaultValue, newValue);

                    if (!bIsDefault)
                    {
                        cb.Checked = true;
                    } else {
                        cb.Checked = false;
                    }
                }
            }

            void PropertyInputColorChangedWorld(object sender, EventArgs e)
            {
                if (!Session.IgnorePropertyInputChangedWorld)
                {                
                    if(sender is ListBox)
                    {
                        if (colorDlg.ShowDialog() == DialogResult.OK)
                        {
                            KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item5 == sender);
                            TCProperty property = kvp.Key;
                            kvp.Value.Item5.BackColor = colorDlg.Color;
                            if (Session.SettingsType.ColorType == "0x")
                            {
                                kvp.Value.Item1.Text = "0x" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            else if (Session.SettingsType.ColorType == "#")
                            {
                                kvp.Value.Item1.Text = "#" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            Session.WorldConfig1.SetProperty(property, kvp.Value.Item1.Text, false, false);

                            if (Session.WorldConfigDefaultValues == null || ColorTranslator.FromHtml(kvp.Value.Item1.Text).ToArgb() != ColorTranslator.FromHtml(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property)).ToArgb())
                            {
                                kvp.Value.Item2.Checked = true;
                            } else {
                                kvp.Value.Item2.Checked = false;
                            }
                        }
                        this.panel2.Focus();
                    }
                    else if(sender is TextBox)
                    {
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item1 == sender);
                        TCProperty property = kvp.Key;
                        try
                        {
                            if ((kvp.Value.Item1.Text.StartsWith("0x") && kvp.Value.Item1.Text.Length == 8) || (kvp.Value.Item1.Text.StartsWith("#") && kvp.Value.Item1.Text.Length == 7))
                            {
                                Session.WorldSettingsInputs[property].Item5.BackColor = ColorTranslator.FromHtml(kvp.Value.Item1.Text);
                                if (Session.SettingsType.ColorType == "0x")
                                {
                                    kvp.Value.Item1.Text = "0x" + Session.WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                else if (Session.SettingsType.ColorType == "#")
                                {
                                    kvp.Value.Item1.Text = "#" + Session.WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                Session.WorldConfig1.SetProperty(property, kvp.Value.Item1.Text, false, false);

                                if (Session.WorldConfigDefaultValues == null || ColorTranslator.FromHtml(kvp.Value.Item1.Text).ToArgb() != ColorTranslator.FromHtml(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property)).ToArgb())
                                {
                                    kvp.Value.Item2.Checked = true;
                                } else {
                                    kvp.Value.Item2.Checked = false;
                                }
                            } else {
                                Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                kvp.Value.Item2.Checked = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                            kvp.Value.Item2.Checked = false;
                        }
                    }
                }
            }

            void btnTrueFalseWorld_Click(object sender, EventArgs e)
            {
                Button btnSender = (Button)sender;

                if(btnSender.Text.Equals(""))
                {
                    btnSender.Text = "true";
                    btnSender.ForeColor = Color.Green;
                }
                else if (btnSender.Text.Equals("true"))
                {
                    btnSender.Text = "false";
                    btnSender.ForeColor = Color.Red;
                }
                else if (btnSender.Text.Equals("false"))
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item1 == sender);
                    TCProperty property = kvp.Key;
                    string value = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                    if (value != null && value.Equals("false"))
                    {
                        btnSender.Text = "true";
                        btnSender.ForeColor = Color.Green;
                    } else {
                        btnSender.Text = "";
                        btnSender.ForeColor = Color.Empty;
                    }
                }

                PropertyInputChangedWorld(sender, e);

                this.panel2.Focus();
            }

            void btnTrueFalseBiome_Click(object sender, EventArgs e)
            {
                Button btnSender = (Button)sender;

                if (btnSender.Text.Equals(""))
                {
                    btnSender.Text = "true";
                    btnSender.ForeColor = Color.Green;
                }
                else if (btnSender.Text.Equals("true"))
                {
                    btnSender.Text = "false";
                    btnSender.ForeColor = Color.Red;
                }
                else if (btnSender.Text.Equals("false"))
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item1 == sender);
                    TCProperty property = kvp.Key;

                    Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);

                    BiomeConfig biomeDefaultConfig = null;
                    if (g.showDefaults)
                    {
                        biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    }

                    string value = biomeDefaultConfig != null ? biomeDefaultConfig.GetPropertyValueAsString(property) : null;
                    if (biomeDefaultConfig != null && value != null && value.Equals("false"))
                    {
                        btnSender.Text = "true";
                        btnSender.ForeColor = Color.Green;
                    } else {
                        btnSender.Text = "";
                        btnSender.ForeColor = Color.Empty;
                    }
                }

                PropertyInputChangedBiome(sender, e);

                this.panel3.Focus();
            }

            void PropertyInputChangedWorld(object sender, EventArgs e)
            {
                if (!Session.IgnorePropertyInputChangedWorld)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item1 == sender);
                    Control tb = kvp.Value.Item1;
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    float result;
                    int result2;
                    if (
                        property.PropertyType == "String" ||
                        property.PropertyType == "BigString" ||
                        property.PropertyType == "Bool" ||
                        (
                            (property.PropertyType == "Float" && float.TryParse(tb.Text, out result)) ||
                            (property.PropertyType == "Int" && int.TryParse(tb.Text, out result2))
                        )
                    )
                    {
                        Session.WorldConfig1.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                        if (
                            !(
                                property.PropertyType == "Bool" && 
                                String.IsNullOrEmpty(tb.Text)
                            ) && 
                            (                                
                                Session.WorldConfigDefaultValues == null || 
                                (
                                    property.PropertyType != "Bool" && 
                                    tb.Text != Session.WorldConfigDefaultValues.GetPropertyValueAsString(property)
                                ) || 
                                (
                                    property.PropertyType == "Bool" && 
                                    !String.IsNullOrEmpty(tb.Text) && 
                                    tb.Text != Session.WorldConfigDefaultValues.GetPropertyValueAsString(property)
                                )
                            )
                        )
                        {
                            cb.Checked = true;
                        } else {
                            if (property.PropertyType == "Bool" && String.IsNullOrEmpty(tb.Text) && Session.WorldConfigDefaultValues != null)
                            {
                                Session.IgnoreOverrideCheckChangedWorld = true;
                                Session.IgnorePropertyInputChangedWorld = true;
                                tb.Text = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                if (tb.Text.Equals("true"))
                                {
                                    tb.ForeColor = Color.Green;
                                }
                                else if (tb.Text.Equals("true"))
                                {
                                    tb.ForeColor = Color.Red;
                                } else {
                                    tb.ForeColor = Color.Empty;
                                }
                                Session.IgnorePropertyInputChangedWorld = false;
                                Session.IgnoreOverrideCheckChangedWorld = false;
                            }
                            cb.Checked = false;
                        }
                    }
                }
            }

            void PropertyInputLostFocusWorld(object sender, EventArgs e)
            {
                // If color select box was sender
                if (Session.WorldSettingsInputs.Any(a => a.Value.Item5 == sender))
                {
                    sender = Session.WorldSettingsInputs.First(a => a.Value.Item5 == sender).Value.Item1;
                }

                TCProperty property = Session.WorldSettingsInputs.First(a => a.Value.Item1 == sender).Key;
                if (property.PropertyType == "Color")
                {
                    Session.IgnorePropertyInputChangedWorld = true;
                    Session.IgnoreOverrideCheckChangedWorld = true;
                    bool bSetToDefaults = false;
                    if ((((TextBox)sender).Text.StartsWith("0x") && ((TextBox)sender).Text.Length == 8) || (((TextBox)sender).Text.StartsWith("#") && ((TextBox)sender).Text.Length == 7))
                    {
                        try
                        {
                            Session.WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(((TextBox)sender).Text);
                            string value = "";
                            if (Session.SettingsType.ColorType == "0x")
                            {
                                value = "0x" + Session.WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                            }
                            else if (Session.SettingsType.ColorType == "#")
                            {
                                value = "#" + Session.WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                            }
                            Session.WorldSettingsInputs[property].Item1.Text = value;
                            Session.WorldConfig1.SetProperty(property, value, false, false);
                            if (Session.WorldConfigDefaultValues == null || ColorTranslator.FromHtml(value.ToUpper()).ToArgb() != ColorTranslator.FromHtml(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property).ToUpper()).ToArgb())
                            {
                                Session.WorldSettingsInputs[property].Item2.Checked = true;
                            } else {
                                Session.WorldSettingsInputs[property].Item2.Checked = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            bSetToDefaults = true;
                        }
                    } else {
                        bSetToDefaults = true;
                    }
                    if(bSetToDefaults)
                    {
                        string defaultValue = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                        Session.WorldConfig1.SetProperty(property, null, false, false);
                        if (defaultValue != null && ((defaultValue.StartsWith("0x") && defaultValue.Length == 8) || (defaultValue.StartsWith("#") && defaultValue.Length == 7)))
                        {
                            try
                            {
                                Session.WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(defaultValue);
                                string value = "";
                                if (Session.SettingsType.ColorType == "0x")
                                {
                                    value = "0x" + Session.WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                else if (Session.SettingsType.ColorType == "#")
                                {
                                    value = "#" + Session.WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                Session.WorldSettingsInputs[property].Item1.Text = value;
                            }
                            catch (Exception ex2)
                            {
                                Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                Session.WorldSettingsInputs[property].Item1.Text = "";
                            }
                            Session.WorldSettingsInputs[property].Item2.Checked = false;
                        } else {
                            Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                            Session.WorldSettingsInputs[property].Item1.Text = "";
                            Session.WorldSettingsInputs[property].Item2.Checked = false;
                        }
                    }
                    Session.IgnorePropertyInputChangedWorld = false;
                    Session.IgnoreOverrideCheckChangedWorld = false;
                }
                else if (property.PropertyType == "Float" || property.PropertyType == "Int")
                {
                    float result = 0;
                    int result2 = 0;
                    if (
                        (
                            property.PropertyType == "Float" &&
                            !float.TryParse(((TextBox)sender).Text, out result)
                        ) ||
                        (
                            property.PropertyType == "Int" &&
                            !int.TryParse(((TextBox)sender).Text, out result2)
                        )
                    )
                    {
                        if (Session.WorldConfigDefaultValues != null)
                        {
                            string newText = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                            int numberOfDecimals = property.PropertyType == "Int" ? 0 : newText.IndexOf(".") > 0 ? newText.Length - (newText.IndexOf(".") + 1) : 0;
                            ((NumericUpDownExt)sender).DecimalPlaces = numberOfDecimals;
                            ((TextBox)sender).Text = newText;
                        }
                        Session.WorldSettingsInputs[property].Item2.Checked = false;
                    }
                }
                else if (String.IsNullOrWhiteSpace(((Control)sender).Text) && property.PropertyType != "String" && property.PropertyType != "BigString")
                {
                    if (Session.WorldConfigDefaultValues != null)
                    {
                        ((Control)sender).Text = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                    }
                    Session.WorldSettingsInputs[property].Item2.Checked = false;
                }
            }

            void PropertyInputOverrideCheckChangedWorld(object sender, EventArgs e)
            {
                if (!Session.IgnoreOverrideCheckChangedWorld)
                {
                    TCProperty property = Session.WorldSettingsInputs.First(a => a.Value.Item2 == sender).Key;
                    Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = ((CheckBox)sender).Checked;

                    if (((CheckBox)sender).Checked)
                    {
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item2 == sender);
                        Control tb = kvp.Value.Item1;
                        CheckBox cb = kvp.Value.Item2;

                        float result;
                        int result2;
                        if (
                            property.PropertyType == "String" ||
                            property.PropertyType == "BigString" ||
                            property.PropertyType == "Bool" ||
                            (
                                (property.PropertyType == "Float" && float.TryParse(tb.Text, out result)) ||
                                (property.PropertyType == "Int" && int.TryParse(tb.Text, out result2))
                            )
                        )
                        {
                            Session.WorldConfig1.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                        else if (property.PropertyType == "Float" || property.PropertyType == "Int")
                        {
                            Session.IgnoreOverrideCheckChangedWorld = true;
                            Session.IgnorePropertyInputChangedWorld = true;
                            if (Session.WorldConfigDefaultValues != null && Session.WorldConfigDefaultValues.GetPropertyValueAsString(property) != null)
                            {
                                string newText = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                int numberOfDecimals = property.PropertyType == "Int" ? 0 : newText.IndexOf(".") > 0 ? newText.Length - (newText.IndexOf(".") + 1) : 0;
                                ((NumericUpDownExt)tb).DecimalPlaces = numberOfDecimals;
                                tb.Text = newText;
                            } else {
                                tb.Text = "";
                            }
                            cb.Checked = false;
                            Session.WorldConfig1.SetProperty(property, null, false, false);
                            Session.IgnoreOverrideCheckChangedWorld = false;
                            Session.IgnorePropertyInputChangedWorld = false;
                        }
                        else if (property.PropertyType == "Color")
                        {
                            try
                            {
                                if ((kvp.Value.Item1.Text.StartsWith("0x") && kvp.Value.Item1.Text.Length == 8) || (kvp.Value.Item1.Text.StartsWith("#") && kvp.Value.Item1.Text.Length == 7))
                                {
                                    Session.WorldConfig1.SetProperty(property, kvp.Value.Item1.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                                } else {
                                    Session.IgnoreOverrideCheckChangedWorld = true;
                                    Session.IgnorePropertyInputChangedWorld = true;
                                    if (Session.WorldConfigDefaultValues != null && !String.IsNullOrEmpty(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property)))
                                    {
                                        tb.Text = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                        Session.WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property));
                                    } else {
                                        tb.Text = "";
                                        Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                    }
                                    cb.Checked = false;
                                    Session.WorldConfig1.SetProperty(property, null, false, false);
                                    Session.IgnoreOverrideCheckChangedWorld = false;
                                    Session.IgnorePropertyInputChangedWorld = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                Session.IgnoreOverrideCheckChangedWorld = true;
                                Session.IgnorePropertyInputChangedWorld = true;
                                if (Session.WorldConfigDefaultValues != null && !String.IsNullOrEmpty(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property)))
                                {
                                    tb.Text = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                    Session.WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property));
                                } else {
                                    tb.Text = "";
                                    Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                }
                                cb.Checked = false;
                                Session.WorldConfig1.SetProperty(property, null, false, false);
                                Session.IgnoreOverrideCheckChangedWorld = false;
                                Session.IgnorePropertyInputChangedWorld = false;
                            }
                        }
                        else if (property.PropertyType == "BiomesList")
                        {
                            List<string> biomeNames = new List<string>();
                            string sBiomeNames = "";
                            foreach (string s in ((ListBox)kvp.Value.Item1).SelectedItems)
                            {
                                biomeNames.Add(s);
                                sBiomeNames += sBiomeNames.Length == 0 ? s : ", " + s;
                            }
                            Session.WorldConfig1.SetProperty(property, sBiomeNames, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                        else if (property.PropertyType == "ResourceQueue")
                        {
                            List<string> biomeNames = new List<string>();
                            string sBiomeNames = "";
                            foreach (string s in ((ListBox)kvp.Value.Item1).Items)
                            {
                                biomeNames.Add(s);
                                sBiomeNames += sBiomeNames.Length == 0 ? s : "\r\n" + s;
                            }
                            Session.WorldConfig1.SetProperty(property, sBiomeNames, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                    }
                }
            }

            void bSetDefaultsWorldProperty(object sender, EventArgs e)
            {
                KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.FirstOrDefault(a => a.Value.Item3 == sender);
                if (Session.WorldConfigDefaultValues != null)
                {
                    string propertyValue = Session.WorldConfigDefaultValues.GetPropertyValueAsString(kvp.Key);
                    switch(kvp.Key.PropertyType)
                    {
                        case "BiomesList":
                            if(propertyValue != null)
                            {
                                ((ListBox)Session.WorldSettingsInputs[kvp.Key].Item1).SelectedItems.Clear();
                                string[] biomeNames = propertyValue.Split(',');
                                ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                for (int k = 0; k < biomeNames.Length; k++)
                                {
                                    if (Session.BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                    {
                                        for (int l = 0; l < ((ListBox)Session.WorldSettingsInputs[kvp.Key].Item1).Items.Count; l++)
                                        {
                                            if (((string)((ListBox)Session.WorldSettingsInputs[kvp.Key].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                            {
                                                ((ListBox)Session.WorldSettingsInputs[kvp.Key].Item1).SelectedItems.Add(((ListBox)Session.WorldSettingsInputs[kvp.Key].Item1).Items[l]);
                                            }
                                        }
                                    }
                                }
                            }
                        break;
                        case "Bool":
                            if (propertyValue != null && propertyValue.ToLower() == "true")
                            {
                                ((Button)kvp.Value.Item1).Text = "true";
                                ((Button)kvp.Value.Item1).ForeColor = Color.Green;
                            }
                            else if (propertyValue != null && propertyValue.ToLower() == "false")
                            {
                                ((Button)kvp.Value.Item1).Text = "false";
                                ((Button)kvp.Value.Item1).ForeColor = Color.Red;
                            } else {
                                ((Button)kvp.Value.Item1).Text = "";
                                ((Button)kvp.Value.Item1).ForeColor = Color.Empty;
                            }
                        break;
                        case "Color":
                        if (propertyValue != null && ((propertyValue.StartsWith("0x") && propertyValue.Length == 8) || (propertyValue.StartsWith("#") && propertyValue.Length == 7)))
                            {
                                try
                                {
                                    kvp.Value.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                    kvp.Value.Item1.Text = propertyValue;
                                }
                                catch(Exception ex)
                                {
                                    kvp.Value.Item5.BackColor = Color.White;
                                    kvp.Value.Item1.Text = "";
                                }
                            } else {
                                kvp.Value.Item5.BackColor = Color.White;
                                kvp.Value.Item1.Text = "";
                            }
                        break;
                        case "Float":
                        case "Int":
                            string newText = propertyValue;
                            int numberOfDecimals = kvp.Key.PropertyType == "Int" ? 0 : newText.IndexOf(".") > 0 ? newText.Length - (newText.IndexOf(".") + 1) : 0;
                            ((NumericUpDownExt)kvp.Value.Item1).DecimalPlaces = numberOfDecimals;
                            kvp.Value.Item1.Text = newText;
                        break;
                        case "String":
                        case "BigString":
                            kvp.Value.Item1.Text = propertyValue;
                        break;
                        case "ResourceQueue":
                            ((ListBox)kvp.Value.Item1).Items.Clear();
                            ((ListBox)kvp.Value.Item1).SelectedIndex = -1;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                            ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                            Session.WorldConfig1.SetProperty(kvp.Key, null, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                            if (propertyValue != null)
                            {
                                string[] resourceQueueItemNames = propertyValue.Replace("\r", "").Split('\n');
                                foreach (string resourceQueueItemName in resourceQueueItemNames)
                                {
                                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                                    {
                                        ((ListBox)kvp.Value.Item1).Items.Add(resourceQueueItemName.Trim());
                                    }
                                }
                            }
                        break;

                    }
                    kvp.Value.Item2.Checked = Session.WorldConfigDefaultValues.Properties.First(a => a.PropertyName == kvp.Key.Name).Override;
                } else {
                    switch(kvp.Key.PropertyType)
                    {
                        case "BiomesList":
                            ((ListBox)kvp.Value.Item1).SelectedIndices.Clear();
                            ((ListBox)kvp.Value.Item1).SelectedIndex = -1;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                        break;
                        case "Bool":
                            ((Button)kvp.Value.Item1).Text = "";
                            ((Button)kvp.Value.Item1).ForeColor = Color.Empty;
                        break;
                        case "Color":
                            kvp.Value.Item5.BackColor = Color.White;
                            kvp.Value.Item1.Text = "";
                        break;
                        case "Float":
                        case "Int":
                            kvp.Value.Item1.Text = "";
                        break;
                        case "String":
                        case "BigString":
                            kvp.Value.Item1.Text = "";
                        break;
                        case "ResourceQueue":
                            ((ListBox)kvp.Value.Item1).Items.Clear();
                            ((ListBox)kvp.Value.Item1).SelectedIndex = -1;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                            Session.WorldConfig1.SetProperty(kvp.Key, null, false, false);
                        break;
                    }
                    kvp.Value.Item2.Checked = false;
                }
            }

            void btAddResourceQueueItemWorld_Click(object sender, EventArgs e)
            {
                string propertyValue = "";
                if (PopUpForm.InputBox("Add item", "", ref propertyValue, true) == DialogResult.OK)
                {
                    if (propertyValue != null && !String.IsNullOrEmpty(propertyValue.Trim()))
                    {
                        TCProperty property = ResourceQueueInputs[sender];
                        AddToResourceQueueWorld(property, propertyValue);
                    }
                }
            }

            private void AddToResourceQueueWorld(TCProperty property, string propertyValue, bool showDuplicateWarnings = true)
            {
                if (propertyValue == null || string.IsNullOrEmpty(propertyValue.Trim().Replace("\r", "").Replace("\n", "")))
                {
                    return;
                }

                if (!copyPasting)
                {
                    addingMultipleResourcesXToAll = DialogResult.Abort;
                    addingMultipleResourcesXToAll2 = DialogResult.Abort;
                    addingMultipleResourcesXToAll3 = DialogResult.Abort;
                }

                propertyValue = propertyValue.Replace("\r", "").Replace("\n", "");

                ListBox lb = ((ListBox)Session.WorldSettingsInputs[property].Item1);
                List<string> newPropertyValue = new List<string>();
                foreach (string item in lb.Items)
                {
                    newPropertyValue.Add(item.Trim());
                }

                bool bAllowed = false;
                bool bOverrideExisting = false;
                ResourceQueueItem selectedOption = null;
                foreach (ResourceQueueItem option in Session.VersionConfig.ResourceQueueOptions)
                {
                    if (propertyValue.StartsWith(option.Name))
                    {
                        selectedOption = option;
                        break;
                    }
                }

                if (newPropertyValue != null && selectedOption != null)
                {
                    DialogResult permissionGiven = DialogResult.Abort;
                    bool bFound = false;
                    if ((!selectedOption.HasUniqueParameter && newPropertyValue.Any(a => a.StartsWith(selectedOption.Name)) || (selectedOption.HasUniqueParameter && newPropertyValue.Any(a => (string)a == (string)propertyValue))))
                    {
                        if (selectedOption.HasUniqueParameter && !newPropertyValue.Any(a => (string)a == (string)propertyValue))
                        {
                            if (addingMultipleResourcesXToAll == DialogResult.Abort)
                            {
                                string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                //addingMultipleResourcesXToAll = PopUpForm.CustomYesNoBox("Keep existing items?", "One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                addingMultipleResourcesXToAll = System.Windows.Forms.DialogResult.Yes;
                            }
                            permissionGiven = addingMultipleResourcesXToAll;
                            if (permissionGiven == DialogResult.No)
                            {
                                bOverrideExisting = true;
                            }
                            if (permissionGiven == DialogResult.Cancel)
                            {
                                return;
                            }
                        } else {
                            if (newPropertyValue.Any(a => (string)a == (string)propertyValue))
                            {
                                if (addingMultipleResourcesXToAll2 == DialogResult.Abort)
                                {
                                    //addingMultipleResourcesXToAll2 = PopUpForm.CustomYesNoBox("Add exact duplicate?", "Resource \"" + propertyValue + "\" already exists, do you still want to add it?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Yes", "No");
                                    addingMultipleResourcesXToAll2 = System.Windows.Forms.DialogResult.Yes;
                                }
                                permissionGiven = addingMultipleResourcesXToAll2;
                            } else {
                                if (addingMultipleResourcesXToAll3 == DialogResult.Abort)
                                {
                                    //addingMultipleResourcesXToAll3 = PopUpForm.CustomYesNoBox("Keep same resource with different parameters?", "One or more \"" + selectedOption.Name + "\" resources already exist but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                    addingMultipleResourcesXToAll3 = System.Windows.Forms.DialogResult.Yes;
                                }
                                permissionGiven = addingMultipleResourcesXToAll3;
                                if (permissionGiven == DialogResult.No)
                                {
                                    bOverrideExisting = true;
                                }
                                if (permissionGiven == DialogResult.Cancel)
                                {
                                    return;
                                }
                            }
                        }
                        if (permissionGiven == DialogResult.Yes)
                        {
                            bAllowed = true;
                        }
                        bFound = true;
                    }
                    if (!bFound || bOverrideExisting)
                    {
                        if (propertyValue.Contains('(') && propertyValue.Contains(')'))
                        {
                            if (selectedOption.IsUnique)
                            {
                                List<string> possibleDuplicates = new List<string>();
                                foreach (string value2 in newPropertyValue)
                                {
                                    if (value2.StartsWith(selectedOption.Name))
                                    {
                                        if (!selectedOption.HasUniqueParameter && bOverrideExisting) // Is duplicate tag, but not necessarily same params
                                        {
                                            //Delete old add new
                                            DeleteResourceQueueItemWorld(property, value2);
                                        } else {
                                            possibleDuplicates.Add(value2);
                                        }
                                    }
                                }
                                if (possibleDuplicates.Any())
                                {
                                    string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                    foreach (string existingValue in possibleDuplicates)
                                    {
                                        string[] existingParameters = existingValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                        if (existingParameters.Length > selectedOption.UniqueParameterIndex && newParameters.Length > selectedOption.UniqueParameterIndex)
                                        {
                                            bool bFound4 = false;
                                            if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                            {
                                                if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(newParameters[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                {
                                                    if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                    {
                                                        bFound4 = true;
                                                    }
                                                }
                                            } else {
                                                if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                {
                                                    bFound4 = true;
                                                }
                                            }
                                            if (bFound4)
                                            {
                                                //Delete old add new
                                                if (permissionGiven == DialogResult.Abort)
                                                {
                                                    if (addingMultipleResourcesXToAll == DialogResult.Abort)
                                                    {
                                                        addingMultipleResourcesXToAll = PopUpForm.CustomYesNoBox("Keep existing items?", "One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                                    }
                                                    permissionGiven = addingMultipleResourcesXToAll;
                                                }
                                                if (permissionGiven == DialogResult.No)
                                                {
                                                    DeleteResourceQueueItemWorld(property, existingValue);
                                                }
                                                if (permissionGiven == DialogResult.Cancel)
                                                {
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    if (newParameters.Length > selectedOption.UniqueParameterIndex)
                                    {
                                        bAllowed = true;
                                    } else {
                                        PopUpForm.CustomMessageBox("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
                                    }
                                } else {
                                    bAllowed = true;
                                }
                            } else {
                                bAllowed = true;
                            }
                        } else {
                            string[] propertyNames = Session.VersionConfig.ResourceQueueOptions.Select(a => a.Name).ToArray();
                            string sPropertyNames = "";
                            foreach (string a in propertyNames)
                            {
                                sPropertyNames += a + "\r\n";
                            }
                            PopUpForm.CustomMessageBox("Cannot add item. Setting name was not recognized. Legal setting names are: \r\n" + sPropertyNames, "Error: Illegal input");
                        }
                    }
                } else {
                    if (newPropertyValue == null)
                    {
                        bAllowed = true;
                    }
                    else if (selectedOption == null)
                    {
                        PopUpForm.CustomMessageBox("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is SettingName(Parameters)", "Error: Illegal input");
                    }
                }
                if (bAllowed)
                {
                    if (!copyPasting)
                    {
                        AddResourceToWorld(property, propertyValue);
                    } else {
                        resourcesToAdd.Add(propertyValue);
                    }
                }
            }

            public void AddResourceToWorld(TCProperty property, string propertyValue)
            {
                Session.IgnoreOverrideCheckChangedWorld = true;

                string s = !String.IsNullOrEmpty(Session.WorldConfig1.GetPropertyValueAsString(property)) ? Session.WorldConfig1.GetPropertyValueAsString(property) + "\r\n" + propertyValue.Trim() : propertyValue.Trim();
                if (Session.WorldConfigDefaultValues != null)
                {
                    bool bIsDefault = true;
                    if (Session.WorldConfig1.GetPropertyValueAsString(property) == null)
                    {
                        s = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property) + "\r\n" + propertyValue.Trim();
                        bIsDefault = false;
                    } else {
                        bIsDefault = Utils.TCSettingsUtils.CompareResourceQueues(s, Session.WorldConfigDefaultValues.GetPropertyValueAsString(property));
                    }

                    if (!bIsDefault)
                    {
                        Session.WorldConfig1.SetProperty(property, s, Session.WorldSettingsInputs[property].Item6 != null && ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.WorldSettingsInputs[property].Item6 != null && ((CheckBox)Session.WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = true;
                        Session.WorldSettingsInputs[property].Item2.Checked = true;
                    } else {
                        Session.WorldConfig1.SetProperty(property, null, Session.WorldSettingsInputs[property].Item6 != null && ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.WorldSettingsInputs[property].Item6 != null && ((CheckBox)Session.WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = false;
                        Session.WorldSettingsInputs[property].Item2.Checked = false;
                    }
                } else {
                    Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = true;
                    Session.WorldSettingsInputs[property].Item2.Checked = true;
                    Session.WorldConfig1.SetProperty(property, s, Session.WorldSettingsInputs[property].Item6 != null && ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.WorldSettingsInputs[property].Item6 != null && ((CheckBox)Session.WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                }
                Session.WorldConfig1.SetProperty(property, s.Trim(), Session.WorldSettingsInputs[property].Item6 != null && ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.WorldSettingsInputs[property].Item6 != null && ((CheckBox)Session.WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Clear();
                string[] resourceQueueItemNames = Session.WorldConfig1.GetPropertyValueAsString(property).Replace("\r", "").Split('\n');
                foreach (string resourceQueueItemName in resourceQueueItemNames)
                {
                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                    {
                        ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                    }
                }
                Session.IgnoreOverrideCheckChangedWorld = false;
            }

            void btEditResourceQueueItemWorld_Click(object sender, EventArgs e)
            {
                addingMultipleResourcesXToAll = DialogResult.Abort;
                addingMultipleResourcesXToAll2 = DialogResult.Abort;
                addingMultipleResourcesXToAll3 = DialogResult.Abort;

                TCProperty property = ResourceQueueInputs[sender];
                ListBox lb = ((ListBox)Session.WorldSettingsInputs[property].Item1);
                if (lb.SelectedItem != null)
                {
                    string originalPropertyValue = (string)lb.SelectedItem.ToString();
                    string propertyValue = (string)lb.SelectedItem.ToString();

                    if (PopUpForm.InputBox("Edit item", "", ref propertyValue, true) == DialogResult.OK && !propertyValue.Equals(originalPropertyValue))
                    {
                        if (propertyValue != null && !String.IsNullOrEmpty(propertyValue.Trim()))
                        {
                            List<string> newPropertyValue = new List<string>();
                            bool bFound1 = false;
                            foreach (string item in lb.Items)
                            {
                                if (!item.Equals(originalPropertyValue) || bFound1)
                                {
                                    newPropertyValue.Add(item.Trim());
                                } else {
                                    bFound1 = true;
                                }
                            }

                            bool bAllowed = false;
                            bool bOverrideExisting = false;
                            ResourceQueueItem selectedOption = null;
                            foreach (ResourceQueueItem option in Session.VersionConfig.ResourceQueueOptions)
                            {
                                if (propertyValue.StartsWith(option.Name))
                                {
                                    selectedOption = option;
                                    break;
                                }
                            }

                            if (newPropertyValue != null && selectedOption != null)
                            {
                                DialogResult permissionGiven = DialogResult.Abort;
                                bool bFound = false;
                                if ((!selectedOption.HasUniqueParameter && newPropertyValue.Any(a => a.StartsWith(selectedOption.Name)) || (selectedOption.HasUniqueParameter && newPropertyValue.Any(a => (string)a == (string)propertyValue))))
                                {
                                    if (selectedOption.HasUniqueParameter && !newPropertyValue.Any(a => (string)a == (string)propertyValue))
                                    {
                                        if (addingMultipleResourcesXToAll == DialogResult.Abort)
                                        {
                                            string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                            //addingMultipleResourcesXToAll = PopUpForm.CustomYesNoBox("Keep existing items?", "One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                            addingMultipleResourcesXToAll = System.Windows.Forms.DialogResult.Yes;
                                        }
                                        permissionGiven = addingMultipleResourcesXToAll;
                                        if (permissionGiven == DialogResult.No)
                                        {
                                            bOverrideExisting = true;
                                        }
                                        if (permissionGiven == DialogResult.Cancel)
                                        {
                                            return;
                                        }
                                    } else {
                                        if (newPropertyValue.Any(a => (string)a == (string)propertyValue))
                                        {
                                            if (addingMultipleResourcesXToAll2 == DialogResult.Abort)
                                            {
                                                //addingMultipleResourcesXToAll2 = PopUpForm.CustomYesNoBox("Add exact duplicate?", "Resource \"" + propertyValue + "\" already exists, do you still want to add it?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Yes", "No");
                                                addingMultipleResourcesXToAll2 = System.Windows.Forms.DialogResult.Yes;
                                            }
                                            permissionGiven = addingMultipleResourcesXToAll2;
                                        } else {
                                            if (addingMultipleResourcesXToAll3 == DialogResult.Abort)
                                            {
                                                //addingMultipleResourcesXToAll3 = PopUpForm.CustomYesNoBox("Keep same resource with different parameters?", "One or more \"" + selectedOption.Name + "\" resources already exist but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                                addingMultipleResourcesXToAll3 = System.Windows.Forms.DialogResult.Yes;
                                            }
                                            permissionGiven = addingMultipleResourcesXToAll3;
                                            if (permissionGiven == DialogResult.No)
                                            {
                                                bOverrideExisting = true;
                                            }
                                            if (permissionGiven == DialogResult.Cancel)
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    if (permissionGiven == DialogResult.Yes)
                                    {
                                        bAllowed = true;
                                    }
                                    bFound = true;
                                }
                                if (!bFound || bOverrideExisting)
                                {
                                    if (propertyValue.Contains('(') && propertyValue.Contains(')'))
                                    {
                                        if (selectedOption.IsUnique)
                                        {
                                            List<string> possibleDuplicates = new List<string>();
                                            foreach (string value2 in newPropertyValue)
                                            {
                                                if (value2.StartsWith(selectedOption.Name))
                                                {
                                                    if (!selectedOption.HasUniqueParameter && bOverrideExisting) // Is duplicate tag, but not necessarily same params
                                                    {
                                                        //Delete old add new
                                                        DeleteResourceQueueItemWorld(property, value2);
                                                    } else {
                                                        possibleDuplicates.Add(value2);
                                                    }
                                                }
                                            }
                                            if (possibleDuplicates.Any())
                                            {
                                                string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                foreach (string existingValue in possibleDuplicates)
                                                {
                                                    string[] existingParameters = existingValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                    if (existingParameters.Length > selectedOption.UniqueParameterIndex && newParameters.Length > selectedOption.UniqueParameterIndex)
                                                    {
                                                        bool bFound4 = false;
                                                        if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                                        {
                                                            if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(newParameters[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                            {
                                                                if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                                {
                                                                    bFound4 = true;
                                                                }
                                                            }
                                                        } else {
                                                            if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                            {
                                                                bFound4 = true;
                                                            }
                                                        }
                                                        if (bFound4)
                                                        {
                                                            //Delete old add new
                                                            if (permissionGiven == DialogResult.Abort)
                                                            {
                                                                if (addingMultipleResourcesXToAll == DialogResult.Abort)
                                                                {
                                                                    //addingMultipleResourcesXToAll = PopUpForm.CustomYesNoBox("Keep existing items?", "One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                                                    addingMultipleResourcesXToAll = System.Windows.Forms.DialogResult.Yes;
                                                                }
                                                                permissionGiven = addingMultipleResourcesXToAll;
                                                            }
                                                            if (permissionGiven == DialogResult.No)
                                                            {
                                                                DeleteResourceQueueItemWorld(property, existingValue);
                                                            }
                                                            if (permissionGiven == DialogResult.Cancel)
                                                            {
                                                                return;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (newParameters.Length > selectedOption.UniqueParameterIndex)
                                                {
                                                    bAllowed = true;
                                                } else {
                                                    PopUpForm.CustomMessageBox("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
                                                }
                                            } else {
                                                bAllowed = true;
                                            }
                                        } else {
                                            bAllowed = true;
                                        }
                                    } else {
                                        string[] propertyNames = Session.VersionConfig.ResourceQueueOptions.Select(a => a.Name).ToArray();
                                        string sPropertyNames = "";
                                        foreach (string a in propertyNames)
                                        {
                                            sPropertyNames += a + "\r\n";
                                        }
                                        PopUpForm.CustomMessageBox("Cannot add item. Setting name was not recognized. Legal setting names are: \r\n" + sPropertyNames, "Error: Illegal input");
                                    }
                                }
                            } else {
                                if (newPropertyValue == null)
                                {
                                    bAllowed = true;
                                }
                                else if (selectedOption == null)
                                {
                                    PopUpForm.CustomMessageBox("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is SettingName(Parameters)", "Error: Illegal input");
                                }
                            }
                            if (bAllowed)
                            {
                                DeleteResourceQueueItemWorld(property, originalPropertyValue);
                                AddResourceToWorld(property, propertyValue);
                            }
                        }
                    }
                }
            }

            void btDeleteResourceQueueItemWorld_Click(object sender, EventArgs e)
            {
                TCProperty property = ResourceQueueInputs[sender];
                ListBox lb = ((ListBox)Session.WorldSettingsInputs[property].Item1);
                if (lb.SelectedItem != null)
                {
                    List<string> itemsToRemove = new List<string>();
                    foreach(string selectedItem in lb.SelectedItems)
                    {
                        itemsToRemove.Add(selectedItem);
                    }
                    foreach(string selectedItem in itemsToRemove)
                    {
                        DeleteResourceQueueItemWorld(property, selectedItem);
                    }
                }
            }

            void DeleteResourceQueueItemWorld(TCProperty property, string selectedItem)
            {
                Session.IgnoreOverrideCheckChangedWorld = true;

                string s = Session.WorldConfig1.GetPropertyValueAsString(property) ?? (Session.WorldConfigDefaultValues != null && Session.WorldConfigDefaultValues.GetPropertyValueAsString(property) != null ? Session.WorldConfigDefaultValues.GetPropertyValueAsString(property) : "");
                string[] resourceQueueItemNames = s.Replace("\r", "").Split('\n');
                foreach (string resourceQueueItemName in resourceQueueItemNames)
                {
                    if (resourceQueueItemName == selectedItem)
                    {
                        int replaceStartIndex = 0;
                        foreach (string resourceQueueItemName2 in resourceQueueItemNames)
                        {
                            if (resourceQueueItemName == resourceQueueItemName2)
                            {
                                break;
                            }
                            replaceStartIndex += resourceQueueItemName2.Length;
                            while (s.Substring(replaceStartIndex, 1) == "\r" || s.Substring(replaceStartIndex, 1) == "\n")
                            {
                                replaceStartIndex += 1;
                            }
                        }
                        int length = resourceQueueItemName.Length;
                        while (replaceStartIndex + length + 1 < s.Length && (s.Substring(replaceStartIndex + length, 1) == "\r" || s.Substring(replaceStartIndex + length, 1) == "\n"))
                        {
                            length += 1;
                        }
                        s = s.Remove(replaceStartIndex, length).Trim();
                        break;
                    }
                }

                bool bIsDefault = Utils.TCSettingsUtils.CompareResourceQueues(s, Session.WorldConfigDefaultValues != null && Session.WorldConfigDefaultValues.GetPropertyValueAsString(property) != null ? Session.WorldConfigDefaultValues.GetPropertyValueAsString(property) : "");
                if (!bIsDefault)
                {
                    Session.WorldConfig1.SetProperty(property, s, Session.WorldSettingsInputs[property].Item6 != null && ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.WorldSettingsInputs[property].Item6 != null && ((CheckBox)Session.WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = true;
                    Session.WorldSettingsInputs[property].Item2.Checked = true;
                } else {
                    Session.WorldConfig1.SetProperty(property, null, Session.WorldSettingsInputs[property].Item6 != null && ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.WorldSettingsInputs[property].Item6 != null && ((CheckBox)Session.WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = false;
                    Session.WorldSettingsInputs[property].Item2.Checked = false;
                }

                ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Clear();
                string[] resourceQueueItemNames3 = s.Replace("\r", "").Split('\n');
                foreach (string resourceQueueItemName in resourceQueueItemNames3)
                {
                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                    {
                        ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                    }
                }
                Session.IgnoreOverrideCheckChangedWorld = false;
            }

            void btWorldSettingsSetToDefault_Click(object sender, EventArgs e)
            {
                if (Session.WorldConfigDefaultValues != null)
                {
                    foreach (TCProperty property in Session.VersionConfig.WorldConfig)
                    {
                        Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = Session.WorldSettingsInputs[property];
                        Session.IgnorePropertyInputChangedWorld = true;
                        Session.IgnoreOverrideCheckChangedWorld = true;

                        string propertyValue = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                        switch (property.PropertyType)
                        {
                            case "BiomesList":
                                ((ListBox)Session.WorldSettingsInputs[property].Item1).SelectedItems.Clear();
                                string[] biomeNames = propertyValue.Split(',');
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                for (int k = 0; k < biomeNames.Length; k++)
                                {
                                    if (Session.BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                    {
                                        for (int l = 0; l < ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Count; l++)
                                        {
                                            if (((string)((ListBox)Session.WorldSettingsInputs[property].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                            {
                                                ((ListBox)Session.WorldSettingsInputs[property].Item1).SelectedItems.Add(((ListBox)Session.WorldSettingsInputs[property].Item1).Items[l]);
                                            }
                                        }
                                    }
                                }
                                break;
                            case "Bool":
                                if (propertyValue != null && propertyValue.ToLower() == "true")
                                {
                                    ((Button)boxes.Item1).Text = "true";
                                    ((Button)boxes.Item1).ForeColor = Color.Green;
                                }
                                else if (propertyValue != null && propertyValue.ToLower() == "false")
                                {
                                    ((Button)boxes.Item1).Text = "false";
                                    ((Button)boxes.Item1).ForeColor = Color.Red;
                                } else {
                                    ((Button)boxes.Item1).Text = "";
                                    ((Button)boxes.Item1).ForeColor = Color.Empty;
                                }
                                break;
                            case "Color":
                                if (propertyValue != null && ((propertyValue.StartsWith("0x") && propertyValue.Length == 8) || (propertyValue.StartsWith("#") && propertyValue.Length == 7)))
                                {
                                    boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                    boxes.Item1.Text = propertyValue;
                                } else {
                                    boxes.Item5.BackColor = Color.White;
                                    boxes.Item1.Text = "";
                                }
                                break;
                            case "Float":
                            case "Int":
                                string newText = propertyValue;
                                int numberOfDecimals = property.PropertyType == "Int" ? 0 : newText.IndexOf(".") > 0 ? newText.Length - (newText.IndexOf(".") + 1) : 0;
                                ((NumericUpDownExt)boxes.Item1).DecimalPlaces = numberOfDecimals;
                                boxes.Item1.Text = newText;
                                break;
                            case "String":
                            case "BigString":
                                boxes.Item1.Text = propertyValue;
                                break;
                            case "ResourceQueue":
                                ((ListBox)boxes.Item1).Items.Clear();
                                ((ListBox)boxes.Item1).SelectedIndex = -1;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((CheckBox)boxes.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                                Session.WorldConfig1.SetProperty(property, null, false, false);
                                if (propertyValue != null)
                                {
                                    string[] resourceQueueItemNames = propertyValue.Replace("\r", "").Split('\n');
                                    foreach (string resourceQueueItemName in resourceQueueItemNames)
                                    {
                                        if (!String.IsNullOrEmpty(resourceQueueItemName))
                                        {
                                            ((ListBox)boxes.Item1).Items.Add(resourceQueueItemName.Trim());
                                        }
                                    }
                                }
                                break;
                        }
                        boxes.Item2.Checked = false;

                        Session.IgnorePropertyInputChangedWorld = false;
                        Session.IgnoreOverrideCheckChangedWorld = false;
                    }
                    Session.WorldConfig1 = new WorldConfig(Session.VersionConfig);
                } else {
                    foreach (TCProperty property in Session.VersionConfig.WorldConfig)
                    {
                        Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = Session.WorldSettingsInputs[property];
                        Session.IgnorePropertyInputChangedWorld = true;
                        Session.IgnoreOverrideCheckChangedWorld = true;
                   
                        switch (property.PropertyType)
                        {
                            case "BiomesList":
                                ((ListBox)boxes.Item1).SelectedIndices.Clear();
                                ((ListBox)boxes.Item1).SelectedIndex = -1;
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                break;
                            case "Bool":
                                ((Button)boxes.Item1).Text = "";
                                ((Button)boxes.Item1).ForeColor = Color.Empty;
                                break;
                            case "Color":
                                boxes.Item5.BackColor = Color.White;
                                boxes.Item1.Text = "";
                                break;
                            case "Float":
                            case "Int":
                                boxes.Item1.Text = "";
                                break;
                            case "String":
                            case "BigString":
                                boxes.Item1.Text = "";
                                break;
                            case "ResourceQueue":
                                ((ListBox)boxes.Item1).Items.Clear();
                                ((ListBox)boxes.Item1).SelectedIndex = -1;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((CheckBox)boxes.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                                Session.WorldConfig1.SetProperty(property, null, false, false);
                                break;
                        }
                        boxes.Item2.Checked = false;

                        Session.IgnorePropertyInputChangedWorld = false;
                        Session.IgnoreOverrideCheckChangedWorld = false;
                    }
                }

                this.panel2.Focus();
            }

            Control lastWorldSetting = null;
            private void tbSearchWorldConfig_TextChanged(object sender, EventArgs e)
            {
                lastWorldSetting = null;

                bool textIsNullOrEmpty = String.IsNullOrWhiteSpace(this.tbSearchWorldConfig.Text);

                Control worldSetting = null;

                foreach (Control ctl in this.tlpWorldSettings1.Controls)
                {
                    if(ctl is Label)
                    {
                        if (
                            !textIsNullOrEmpty && 
                            ctl.Text.ToLower().Trim().Contains(this.tbSearchWorldConfig.Text.ToLower().Trim())
                        )
                        {
                            if (worldSetting == null)
                            {
                                worldSetting = ctl;
                                ctl.BackColor = Color.DodgerBlue;
                                ctl.ForeColor = Color.White;
                            } else {
                                ctl.BackColor = Color.LightBlue;
                                ctl.ForeColor = Color.Black;
                            }                    
                        } else {
                            ctl.BackColor = Color.Empty;
                            ctl.ForeColor = Color.Black;
                        }
                    }
                }

                if (worldSetting != null && !textIsNullOrEmpty)
                {
                    lastWorldSetting = worldSetting;
                    this.panel2.ScrollControlIntoView(worldSetting);
                }
            }

            void tbSearchWorldConfig_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
            {
                this.panel2.Focus();
            }

            private void btSearchWorldConfigPrev_Click(object sender, EventArgs e)
            {
                if (lastWorldSetting != null)
                {
                    bool textIsNullOrEmpty = String.IsNullOrWhiteSpace(this.tbSearchWorldConfig.Text);
                    Control previousSetting = null;
                    bool lastWorldSettingFound = false;
                    bool newWorldSettingFound = false;

                    foreach (Control ctl in this.tlpWorldSettings1.Controls)
                    {
                        if (ctl is Label)
                        {
                            if (ctl == lastWorldSetting)
                            {
                                lastWorldSettingFound = true;
                                if (previousSetting != null)
                                {
                                    newWorldSettingFound = true;
                                }
                            } else {
                                if (ctl.Text.ToLower().Trim().Contains(this.tbSearchWorldConfig.Text.ToLower().Trim()))
                                {
                                    if (!lastWorldSettingFound || !newWorldSettingFound)
                                    {
                                        previousSetting = ctl;
                                        previousSetting.BackColor = Color.LightBlue;
                                        previousSetting.ForeColor = Color.Black;
                                    }
                                } else {
                                    ctl.BackColor = Color.Empty;
                                    ctl.ForeColor = Color.Black;
                                }
                            }
                        }
                    }

                    if (previousSetting != null)
                    {
                        if (lastWorldSetting != previousSetting)
                        {
                            lastWorldSetting.BackColor = Color.LightBlue;
                            lastWorldSetting.ForeColor = Color.Black;

                            lastWorldSetting = previousSetting;

                            previousSetting.BackColor = Color.DodgerBlue;
                            previousSetting.ForeColor = Color.White;
                        }

                        this.panel2.ScrollControlIntoView(previousSetting);
                    }
                }

                this.panel2.Focus();
            }

            private void btSearchWorldConfigNext_Click(object sender, EventArgs e)
            {
                if (lastWorldSetting != null)
                {
                    bool textIsNullOrEmpty = String.IsNullOrWhiteSpace(this.tbSearchWorldConfig.Text);
                    Control worldSetting = null;
                    Control firstWorldSetting = null;
                    bool lastWorldSettingFound = false;

                    foreach (Control ctl in this.tlpWorldSettings1.Controls)
                    {
                        if(ctl is Label)
                        {
                            if (ctl == lastWorldSetting)
                            {
                                lastWorldSettingFound = true;
                            }

                            if(
                                firstWorldSetting == null &&
                                ctl.Text.ToLower().Trim().Contains(this.tbSearchWorldConfig.Text.ToLower().Trim())
                            )
                            {
                                firstWorldSetting = ctl;
                            }

                            if (
                                !textIsNullOrEmpty &&
                                ctl != lastWorldSetting && 
                                ctl.Text.ToLower().Trim().Contains(this.tbSearchWorldConfig.Text.ToLower().Trim())
                            )
                            {
                                if (worldSetting == null && lastWorldSettingFound)
                                {
                                    worldSetting = ctl;
                                    ctl.BackColor = Color.DodgerBlue;
                                    ctl.ForeColor = Color.White;
                                } else {
                                    ctl.BackColor = Color.LightBlue;
                                    ctl.ForeColor = Color.Black;
                                }                    
                            } else {
                                ctl.BackColor = Color.Empty;
                                ctl.ForeColor = Color.Black;
                            }
                        }
                    }

                    if (worldSetting == null)
                    {
                        worldSetting = firstWorldSetting;
                        if (worldSetting != null)
                        {
                            worldSetting.BackColor = Color.DodgerBlue;
                            worldSetting.ForeColor = Color.White;
                        }
                    }

                    if (worldSetting != null)
                    {
                        if (worldSetting != lastWorldSetting)
                        {
                            lastWorldSetting.BackColor = Color.LightBlue;
                            lastWorldSetting.ForeColor = Color.Black;
                            lastWorldSetting = worldSetting;
                        }

                        this.panel2.ScrollControlIntoView(worldSetting);
                    }
                }

                this.panel2.Focus();
            }

            void lbWorldTabSetting_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
            {
                bool senderIsMouseTarget = true;

                if (!((Control)sender).ClientRectangle.Contains(((Control)sender).PointToClient(Control.MousePosition)))
                {
                    senderIsMouseTarget = false;
                    this.panel2.Focus();
                }

                if (!senderIsMouseTarget)
                {
                    HandledMouseEventArgs args = e as HandledMouseEventArgs;
                    if (args != null)
                    {
                        args.Handled = true;
                    }
                }
            }


        #endregion

        #region Biomes

            void LoadBiomesList()
            {
                if (!String.IsNullOrEmpty(Session.SourceConfigsDir) && System.IO.Directory.Exists(Session.SourceConfigsDir + "\\" + "WorldBiomes" + "\\"))
                {
                    System.IO.DirectoryInfo defaultWorldDirectory = new System.IO.DirectoryInfo(Session.SourceConfigsDir + "\\" + "WorldBiomes" + "\\");
                    lbBiomes.Items.Clear();
                    BiomeConfigsDefaultValues.Clear();

                    string txtErrorsWrongValue = "";
                    string txtErrorsNoSetting = "";

                    foreach (System.IO.FileInfo file in defaultWorldDirectory.GetFiles())
                    {
                        if (file.Name.EndsWith(".bc"))
                        {
                            lbBiomes.Items.Add(file.Name.Replace(".bc", ""));
                            BiomeConfig bDefaultConfig = Biomes.LoadBiomeConfigFromFile(file, Session.VersionConfig, true, ref txtErrorsWrongValue, ref txtErrorsNoSetting);
                            if (bDefaultConfig == null)
                            {
                                UnloadUI();
                                return;
                            } else {
                                BiomeConfigsDefaultValues.Add(bDefaultConfig);
                            }
                        }
                    }

                    if (txtErrorsWrongValue.Length > 0)
                    {
                        PopUpForm.CustomMessageBox(txtErrorsWrongValue + "\r\n\r\nThe biome config files for this world contain errors, they were probably not generated with the selected version of TC/MCW/OTG/OTG+ and require manual updating.", "Version warnings");
                    }
                    if (txtErrorsNoSetting.Length > 0)
                    {
                        PopUpForm.CustomMessageBox(txtErrorsNoSetting + "\r\n\r\nThe biome config files for this world contain errors, they were probably not generated with the selected version of TC/MCW/OTG/OTG+ and require manual updating.", "Version warnings");
                    }
                }
            }

            void LoadDefaultGroups()
            {
                if (BiomeConfigsDefaultValues.Any())
                {
                    Group overworldLand = new Group("Land", Session.VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName != "Hell" && biomeConfig.BiomeName != "Sky" && !biomeConfig.BiomeName.ToLower().Contains("ocean"))
                        {
                            overworldLand.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    if (overworldLand.Biomes.Any())
                    {
                        lbGroups.Items.Add("Land");
                        Session.BiomeGroups.Add(overworldLand);
                    }
                    
                    Group overworldOceans = new Group("Oceans", Session.VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName.ToLower().Contains("ocean"))
                        {
                            overworldOceans.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    if (overworldOceans.Biomes.Any())
                    {
                        lbGroups.Items.Add("Oceans");
                        Session.BiomeGroups.Add(overworldOceans);
                    }

                    Group overworldSky = new Group("Sky", Session.VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName == "Sky")
                        {
                            overworldSky.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    if (overworldSky.Biomes.Any())
                    {
                        lbGroups.Items.Add("Sky");
                        Session.BiomeGroups.Add(overworldSky);
                    }
                    
                    Group hell = new Group("Nether", Session.VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName == "Hell")
                        {
                            hell.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    if (hell.Biomes.Any())
                    {
                        lbGroups.Items.Add("Nether");
                        Session.BiomeGroups.Add(hell);
                    }
                }
            }

            private void btOverrideParentValuesBiome_CheckedChanged(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedBiome)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item6 == ((Control)sender).Parent);
                    TCProperty property = kvp.Key;

                    Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;

                    biomeConfig.SetProperty(property, biomeConfig.GetPropertyValueAsString(property), biomeConfig.GetPropertyMerge(property), ((CheckBox)sender).Checked);
                }

                this.panel3.Focus();
            }

            private void btOverrideAllBiome_CheckedChanged(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedBiome && ((RadioButton)sender).Checked)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item6 == ((Control)sender).Parent);
                    Control tb = kvp.Value.Item1;
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;

                    BiomeConfig biomeDefaultConfig = null;
                    if (g.showDefaults)
                    {
                        biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    }

                    bool merge = false;
                    if (sender == kvp.Value.Item6.Controls.Find("Override", true)[0])
                    {
                        merge = !((RadioButton)sender).Checked;
                    }
                    else if (sender == kvp.Value.Item6.Controls.Find("Merge", true)[0])
                    {
                        merge = ((RadioButton)sender).Checked;
                    }
                    bool defaultValue = biomeDefaultConfig != null ? biomeDefaultConfig.GetPropertyMerge(property) : false;
                    biomeConfig.SetProperty(property, biomeConfig.GetPropertyValueAsString(property), merge, ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    if(property.PropertyType == "BiomesList")
                    {
                        if (
                            (biomeDefaultConfig == null && biomeConfig.GetPropertyValueAsString(property) != null) ||
                            (!String.IsNullOrEmpty(biomeConfig.GetPropertyValueAsString(property)) && !Utils.TCSettingsUtils.CompareBiomeLists(biomeConfig.GetPropertyValueAsString(property), biomeDefaultConfig.GetPropertyValueAsString(property))) ||
                            (biomeConfig.GetPropertyValueAsString(property) != null && !Utils.TCSettingsUtils.CompareBiomeLists(biomeConfig.GetPropertyValueAsString(property), biomeDefaultConfig.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            cb.Checked = false;
                        }
                    }
                    else if (property.PropertyType == "ResourceQueue")
                    {
                        if ((biomeDefaultConfig == null && !String.IsNullOrEmpty(biomeConfig.GetPropertyValueAsString(property))) || (!String.IsNullOrEmpty(biomeConfig.GetPropertyValueAsString(property)) && !Utils.TCSettingsUtils.CompareResourceQueues(biomeConfig.GetPropertyValueAsString(property), biomeDefaultConfig.GetPropertyValueAsString(property))) || (biomeConfig.GetPropertyValueAsString(property) != null && (biomeDefaultConfig == null || !Utils.TCSettingsUtils.CompareResourceQueues(biomeConfig.GetPropertyValueAsString(property), biomeDefaultConfig.GetPropertyValueAsString(property)))))
                        {
                            cb.Checked = true;
                        } else {
                            cb.Checked = false;
                        }
                    }
                }

                this.panel3.Focus();
            }

            private void lbPropertyInputBiome_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedBiome)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.First(a => a.Value.Item1 == sender);
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;

                    BiomeConfig biomeDefaultConfig = null;
                    if (g.showDefaults)
                    {
                        biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    }

                    List<string> biomeNames = new List<string>();
                    string sBiomeNames = "";
                    foreach (string s in ((ListBox)kvp.Value.Item1).SelectedItems)
                    {
                        biomeNames.Add(s);
                        sBiomeNames += sBiomeNames.Length == 0 ? s : ", " + s;
                    }
                    biomeConfig.SetProperty(property, sBiomeNames, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    bool bIsDefault = true;
                    if (biomeDefaultConfig != null)
                    {
                        bIsDefault = Utils.TCSettingsUtils.CompareBiomeLists(biomeDefaultConfig.GetPropertyValueAsString(property), biomeConfig.GetPropertyValueAsString(property));
                    } else {
                        if (((ListBox)kvp.Value.Item1).SelectedItems.Count == 0)
                        {
                            bIsDefault = true;
                        } else {
                            bIsDefault = false;
                        }
                    }

                    if (!bIsDefault)
                    {
                        cb.Checked = true;
                    } else {
                        cb.Checked = false;
                    }
                }
            }

            void PropertyInputColorChangedBiome(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedBiome)
                {
                    Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;

                    BiomeConfig biomeDefaultConfig = null;
                    if (g.showDefaults)
                    {
                        biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    }

                    if (sender is ListBox)
                    {                        
                        if (colorDlg.ShowDialog() == DialogResult.OK)
                        {
                            KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.First(a => a.Value.Item5 == sender);
                            TCProperty property = kvp.Key;
                            kvp.Value.Item5.BackColor = colorDlg.Color;
                            if (Session.SettingsType.ColorType == "0x")
                            {
                                kvp.Value.Item1.Text = "0x" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            else if (Session.SettingsType.ColorType == "#")
                            {
                                kvp.Value.Item1.Text = "#" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            biomeConfig.SetProperty(property, kvp.Value.Item1.Text, false, false);

                            if (biomeDefaultConfig == null || ColorTranslator.FromHtml(kvp.Value.Item1.Text).ToArgb() != ColorTranslator.FromHtml(biomeDefaultConfig.GetPropertyValueAsString(property)).ToArgb())
                            {
                                kvp.Value.Item2.Checked = true;
                            } else {
                                kvp.Value.Item2.Checked = false;
                            }
                        }
                        this.panel3.Focus();
                    }
                    else if (sender is TextBox)
                    {
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.First(a => a.Value.Item1 == sender);
                        TCProperty property = kvp.Key;
                        try
                        {
                            if ((kvp.Value.Item1.Text.StartsWith("0x") && kvp.Value.Item1.Text.Length == 8) || (kvp.Value.Item1.Text.StartsWith("#") && kvp.Value.Item1.Text.Length == 7))
                            {
                                Session.BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(kvp.Value.Item1.Text);
                                if (Session.SettingsType.ColorType == "0x")
                                {
                                    kvp.Value.Item1.Text = "0x" + Session.BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                else if (Session.SettingsType.ColorType == "#")
                                {
                                    kvp.Value.Item1.Text = "#" + Session.BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                biomeConfig.SetProperty(property, kvp.Value.Item1.Text, false, false);

                                if (biomeDefaultConfig == null || ColorTranslator.FromHtml(kvp.Value.Item1.Text).ToArgb() != ColorTranslator.FromHtml(biomeDefaultConfig.GetPropertyValueAsString(property)).ToArgb())
                                {
                                    kvp.Value.Item2.Checked = true;
                                } else {
                                    kvp.Value.Item2.Checked = false;
                                }
                            } else {
                                Session.BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                kvp.Value.Item2.Checked = false;
                            }                            
                        }
                        catch (Exception ex)
                        {
                            Session.BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                            kvp.Value.Item2.Checked = false;
                        }
                    }
                }
            }

            void PropertyInputChangedBiome(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedBiome)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item1 == sender);
                    Control tb = kvp.Value.Item1;
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;
                    biomeConfig.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    BiomeConfig biomeDefaultConfig = null;
                    if (g.showDefaults)
                    {
                        biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    }

                    float result;
                    int result2;
                    if (
                        property.PropertyType == "String" ||
                        property.PropertyType == "BigString" ||
                        property.PropertyType == "Bool" ||
                        (
                            (
                                property.PropertyType == "Float" &&
                                (
                                    String.IsNullOrEmpty(tb.Text) ||
                                    float.TryParse(tb.Text, out result)
                                )
                            ) ||
                            (
                                property.PropertyType == "Int" &&
                                (
                                    String.IsNullOrEmpty(tb.Text) ||
                                    int.TryParse(tb.Text, out result2)
                                )
                            )
                        )
                    )
                    {
                        biomeConfig.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        if (
                            !(
                                property.PropertyType == "Bool" && 
                                String.IsNullOrEmpty(tb.Text)
                            ) && 
                            (
                                (
                                    biomeDefaultConfig == null &&
                                    !String.IsNullOrEmpty(tb.Text)
                                ) || 
                                (
                                    biomeDefaultConfig != null &&
                                    property.PropertyType != "Bool" && 
                                    tb.Text != biomeDefaultConfig.GetPropertyValueAsString(property)
                                ) || 
                                (
                                    biomeDefaultConfig != null &&
                                    property.PropertyType == "Bool" && 
                                    !String.IsNullOrEmpty(tb.Text) && 
                                    tb.Text != biomeDefaultConfig.GetPropertyValueAsString(property)
                                )
                            )
                        )
                        {
                            cb.Checked = true;
                        } else {
                            if (property.PropertyType == "Bool" && String.IsNullOrEmpty(tb.Text) && biomeDefaultConfig != null)
                            {
                                IgnoreOverrideCheckChangedBiome = true;
                                IgnorePropertyInputChangedBiome = true;
                                tb.Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                                if(tb.Text.Equals("true"))
                                {
                                    tb.ForeColor = Color.Green;
                                }
                                else if (tb.Text.Equals("false"))
                                {
                                    tb.ForeColor = Color.Red;
                                } else {
                                    tb.ForeColor = Color.Empty;
                                }
                                IgnoreOverrideCheckChangedBiome = false;
                                IgnorePropertyInputChangedBiome = false;
                            }
                            cb.Checked = false;
                        }
                    }
                }
            }

            void PropertyInputLostFocusBiome(object sender, EventArgs e)
            {
                // If color select box was sender
                if (Session.BiomeSettingsInputs.Any(a => a.Value.Item5 == sender))
                {
                    sender = Session.BiomeSettingsInputs.First(a => a.Value.Item5 == sender).Value.Item1;
                }

                KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.First(a => a.Value.Item1 == sender);
                TCProperty property = kvp.Key;
                Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                BiomeConfig biomeConfig = g.BiomeConfig;
                BiomeConfig biomeDefaultConfig = null;
                if (g.showDefaults)
                {
                    biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                }
                Control tb = kvp.Value.Item1;
                CheckBox cb = kvp.Value.Item2;

                if (property.PropertyType == "Color")
                {
                    IgnorePropertyInputChangedBiome = true;
                    IgnoreOverrideCheckChangedBiome = true;
                    bool bSetToDefaults = false;
                    if ((((TextBox)sender).Text.StartsWith("0x") && ((TextBox)sender).Text.Length == 8) || (((TextBox)sender).Text.StartsWith("#") && ((TextBox)sender).Text.Length == 7))
                    {
                        try
                        {
                            Session.BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(((TextBox)sender).Text);
                            string value = "";
                            if (Session.SettingsType.ColorType == "0x")
                            {
                                value = "0x" + Session.BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                            }
                            else if (Session.SettingsType.ColorType == "#")
                            {
                                value = "#" + Session.BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                            }
                            Session.BiomeSettingsInputs[property].Item1.Text = value;
                            biomeConfig.SetProperty(property, value, false, false);
                            if (biomeDefaultConfig == null || ColorTranslator.FromHtml(value.ToUpper()).ToArgb() != ColorTranslator.FromHtml(biomeDefaultConfig.GetPropertyValueAsString(property).ToUpper()).ToArgb())
                            {
                                Session.BiomeSettingsInputs[property].Item2.Checked = true;
                            } else {
                                Session.BiomeSettingsInputs[property].Item2.Checked = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            bSetToDefaults = true;
                        }
                    } else {
                        bSetToDefaults = true;
                    }
                    if (bSetToDefaults)
                    {
                        if (biomeDefaultConfig != null)
                        {
                            string defaultValue = biomeDefaultConfig.GetPropertyValueAsString(property);
                            biomeConfig.SetProperty(property, null, false, false);
                            if (defaultValue != null && ((defaultValue.StartsWith("0x") && defaultValue.Length == 8) || (defaultValue.StartsWith("#") && defaultValue.Length == 7)))
                            {
                                try
                                {
                                    Session.BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(defaultValue);
                                    string value = "";
                                    if (Session.SettingsType.ColorType == "0x")
                                    {
                                        value = "0x" + Session.BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                    }
                                    else if (Session.SettingsType.ColorType == "#")
                                    {
                                        value = "#" + Session.BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                    }
                                    Session.BiomeSettingsInputs[property].Item1.Text = value;
                                }
                                catch (Exception ex2)
                                {
                                    Session.BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                    Session.BiomeSettingsInputs[property].Item1.Text = "";
                                }
                                Session.BiomeSettingsInputs[property].Item2.Checked = false;
                            } else {
                                Session.BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                Session.BiomeSettingsInputs[property].Item1.Text = "";
                                Session.BiomeSettingsInputs[property].Item2.Checked = false;
                            }
                        } else {
                            Session.BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                            Session.BiomeSettingsInputs[property].Item1.Text = "";
                            Session.BiomeSettingsInputs[property].Item2.Checked = false;
                        }
                    }
                    IgnorePropertyInputChangedBiome = false;
                    IgnoreOverrideCheckChangedBiome = false;
                }
                else if (property.PropertyType == "Float" || property.PropertyType == "Int")
                {
                    float result = 0;
                    int result2 = 0;
                    if (
                        (
                            property.PropertyType == "Float" &&
                            !float.TryParse(((TextBox)sender).Text, out result)
                        ) ||
                        (
                            property.PropertyType == "Int" &&
                            !int.TryParse(((TextBox)sender).Text, out result2)
                        )
                    )
                    {
                        if (biomeDefaultConfig != null)
                        {
                            string newText = biomeDefaultConfig.GetPropertyValueAsString(property);
                            int numberOfDecimals = property.PropertyType == "Int" ? 0 : newText.IndexOf(".") > 0 ? newText.Length - (newText.IndexOf(".") + 1) : 0;
                            ((NumericUpDownExt)sender).DecimalPlaces = numberOfDecimals;

                            ((TextBox)sender).Text = newText;
                        } else {
                            ((TextBox)sender).Text = "";
                        }
                        Session.BiomeSettingsInputs[property].Item2.Checked = false;
                    }
                }
                else if (String.IsNullOrWhiteSpace(((Control)sender).Text) && property.PropertyType != "String" && property.PropertyType != "BigString")
                {
                    if (biomeDefaultConfig != null)
                    {
                        ((Control)sender).Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                    } else {
                        ((Control)sender).Text = "";
                    }
                    Session.BiomeSettingsInputs[property].Item2.Checked = false;
                }
            }

            void PropertyInputOverrideCheckChangedBiome(object sender, EventArgs e)
            {
                if (!IgnoreOverrideCheckChangedBiome)
                {
                    TCProperty property = Session.BiomeSettingsInputs.First(a => a.Value.Item2 == sender).Key;
                    Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;
                    biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = ((CheckBox)sender).Checked;

                    BiomeConfig biomeDefaultConfig = null;
                    if (g.showDefaults)
                    {
                        biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    }

                    if (((CheckBox)sender).Checked)
                    {
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.First(a => a.Value.Item2 == sender);
                        Control tb = kvp.Value.Item1;
                        CheckBox cb = kvp.Value.Item2;

                        float result;
                        int result2;
                        if (
                            property.PropertyType == "String" ||
                            property.PropertyType == "BigString" ||
                            (property.PropertyType == "Bool" && !String.IsNullOrEmpty(tb.Text)) ||
                            (property.PropertyType == "Float" && float.TryParse(tb.Text, out result)) ||
                            (property.PropertyType == "Int" && int.TryParse(tb.Text, out result2))
                        )
                        {
                            biomeConfig.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                        else if (property.PropertyType == "Float" || property.PropertyType == "Int" || property.PropertyType == "Bool")
                        {
                            IgnoreOverrideCheckChangedBiome = true;
                            IgnorePropertyInputChangedBiome = true;
                            if (biomeDefaultConfig != null && biomeDefaultConfig.GetPropertyValueAsString(property) != null)
                            {
                                string newText = biomeDefaultConfig.GetPropertyValueAsString(property);
                                if (property.PropertyType == "Float" || property.PropertyType == "Int")
                                {                                    
                                    int numberOfDecimals = property.PropertyType == "Int" ? 0 : newText.IndexOf(".") > 0 ? newText.Length - (newText.IndexOf(".") + 1) : 0;
                                    ((NumericUpDownExt)tb).DecimalPlaces = numberOfDecimals;
                                }

                                tb.Text = newText;
                                if (property.PropertyType == "Bool")
                                {
                                    if (tb.Text.Equals("true"))
                                    {
                                        tb.ForeColor = Color.Green;
                                    }
                                    else if (tb.Text.Equals("false"))
                                    {
                                        tb.ForeColor = Color.Red;
                                    } else {
                                        tb.ForeColor = Color.Empty;
                                    }
                                }
                            } else {
                                tb.Text = "";
                            }
                            cb.Checked = false;
                            biomeConfig.SetProperty(property, null, false, false);
                            IgnoreOverrideCheckChangedBiome = false;
                            IgnorePropertyInputChangedBiome = false;
                        }
                        else if (property.PropertyType == "Color")
                        {
                            try
                            {
                                if ((kvp.Value.Item1.Text.StartsWith("0x") && kvp.Value.Item1.Text.Length == 8) || (kvp.Value.Item1.Text.StartsWith("#") && kvp.Value.Item1.Text.Length == 7))
                                {
                                    biomeConfig.SetProperty(property, kvp.Value.Item1.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                                } else {
                                    IgnoreOverrideCheckChangedBiome = true;
                                    IgnorePropertyInputChangedBiome = true;
                                    if (biomeDefaultConfig != null && !String.IsNullOrEmpty(biomeDefaultConfig.GetPropertyValueAsString(property)))
                                    {
                                        tb.Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                                        Session.BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(biomeDefaultConfig.GetPropertyValueAsString(property));
                                    } else {
                                        tb.Text = "";
                                        Session.BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                    }
                                    cb.Checked = false;
                                    biomeConfig.SetProperty(property, null, false, false);
                                    IgnoreOverrideCheckChangedBiome = false;
                                    IgnorePropertyInputChangedBiome = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                IgnoreOverrideCheckChangedBiome = true;
                                IgnorePropertyInputChangedBiome = true;
                                if (biomeDefaultConfig != null && !String.IsNullOrEmpty(biomeDefaultConfig.GetPropertyValueAsString(property)))
                                {
                                    tb.Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                                    Session.BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(biomeDefaultConfig.GetPropertyValueAsString(property));
                                } else {
                                    tb.Text = "";
                                    Session.BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                }
                                cb.Checked = false;
                                biomeConfig.SetProperty(property, null, false, false);
                                IgnoreOverrideCheckChangedBiome = false;
                                IgnorePropertyInputChangedBiome = false;
                            }
                        }
                        else if (property.PropertyType == "BiomesList")
                        {
                            List<string> biomeNames = new List<string>();
                            string sBiomeNames = "";
                            foreach (string s in ((ListBox)kvp.Value.Item1).SelectedItems)
                            {
                                biomeNames.Add(s);
                                sBiomeNames += sBiomeNames.Length == 0 ? s : ", " + s;
                            }
                            biomeConfig.SetProperty(property, sBiomeNames, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                        else if (property.PropertyType == "ResourceQueue")
                        {
                            List<string> biomeNames = new List<string>();
                            string sBiomeNames = "";
                            foreach (string s in ((ListBox)kvp.Value.Item1).Items)
                            {
                                biomeNames.Add(s);
                                sBiomeNames += sBiomeNames.Length == 0 ? s : "\r\n" + s;
                            }
                            biomeConfig.SetProperty(property, sBiomeNames, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                    }
                }
            }

            void bSetDefaultsBiomeProperty(object sender, EventArgs e)
            {
                KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item3 == sender);
                Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                BiomeConfig biomeConfig = g.BiomeConfig;
                BiomeConfig biomeDefaultConfig = null;
                if (g.showDefaults)
                {
                    biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                }
                if (biomeDefaultConfig != null)
                {
                    string propertyValue = biomeDefaultConfig.GetPropertyValueAsString(kvp.Key);
                    switch (kvp.Key.PropertyType)
                    {
                        case "BiomesList":
                            ((ListBox)Session.BiomeSettingsInputs[kvp.Key].Item1).SelectedItems.Clear();
                            string[] biomeNames = propertyValue.Split(',');
                            for (int k = 0; k < biomeNames.Length; k++)
                            {
                                if (Session.BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                {
                                    for (int l = 0; l < ((ListBox)Session.BiomeSettingsInputs[kvp.Key].Item1).Items.Count; l++)
                                    {
                                        if (((string)((ListBox)Session.BiomeSettingsInputs[kvp.Key].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                        {
                                            ((ListBox)Session.BiomeSettingsInputs[kvp.Key].Item1).SelectedItems.Add(((ListBox)Session.BiomeSettingsInputs[kvp.Key].Item1).Items[l]);
                                        }
                                    }
                                }
                            }
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                            ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                        break;
                        case "Bool":
                            if (propertyValue.ToLower() == "true")
                            {
                                ((Button)kvp.Value.Item1).Text = "true";
                                ((Button)kvp.Value.Item1).ForeColor = Color.Green;
                            }
                            else if (propertyValue.ToLower() == "false")
                            {
                                ((Button)kvp.Value.Item1).Text = "false";
                                ((Button)kvp.Value.Item1).ForeColor = Color.Red;
                            } else {
                                ((Button)kvp.Value.Item1).Text = "";
                                ((Button)kvp.Value.Item1).ForeColor = Color.Empty;
                            }
                            break;
                        case "Color":
                            if (propertyValue != null && ((propertyValue.StartsWith("0x") && propertyValue.Length == 8) || (propertyValue.StartsWith("#") && propertyValue.Length == 7)))
                            {
                                try
                                {
                                    kvp.Value.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                    string value = "";
                                    if (Session.SettingsType.ColorType == "0x")
                                    {
                                        value = "0x" + kvp.Value.Item5.BackColor.R.ToString("X2") + kvp.Value.Item5.BackColor.G.ToString("X2") + kvp.Value.Item5.BackColor.B.ToString("X2");
                                    }
                                    else if (Session.SettingsType.ColorType == "#")
                                    {
                                        value = "#" + kvp.Value.Item5.BackColor.R.ToString("X2") + kvp.Value.Item5.BackColor.G.ToString("X2") + kvp.Value.Item5.BackColor.B.ToString("X2");
                                    }
                                    kvp.Value.Item1.Text = value;
                                    kvp.Value.Item1.Text = propertyValue;
                                }
                                catch (Exception ex)
                                {
                                    kvp.Value.Item5.BackColor = Color.White;
                                    kvp.Value.Item1.Text = "";
                                }
                            } else {
                                kvp.Value.Item5.BackColor = Color.White;
                                kvp.Value.Item1.Text = "";
                            }
                        break;
                        case "Float":
                        case "Int":
                            string newText = propertyValue;
                            int numberOfDecimals = kvp.Key.PropertyType == "Int" ? 0 : newText.IndexOf(".") > 0 ? newText.Length - (newText.IndexOf(".") + 1) : 0;
                            ((NumericUpDownExt)kvp.Value.Item1).DecimalPlaces = numberOfDecimals;
                            kvp.Value.Item1.Text = newText;
                        break;
                        case "String":
                        case "BigString":
                            kvp.Value.Item1.Text = propertyValue;
                        break;
                        case "ResourceQueue":
                            ((ListBox)kvp.Value.Item1).Items.Clear();
                            ((ListBox)kvp.Value.Item1).SelectedIndex = -1;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                            ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                            biomeConfig.SetProperty(kvp.Key, null, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                            if(propertyValue != null)
                            {
                                string[] resourceQueueItemNames = propertyValue.Replace("\r", "").Split('\n');
                                foreach (string resourceQueueItemName in resourceQueueItemNames)
                                {
                                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                                    {
                                        ((ListBox)kvp.Value.Item1).Items.Add(resourceQueueItemName.Trim());
                                    }
                                }
                            }
                        break;
                    }
                    kvp.Value.Item2.Checked = biomeDefaultConfig.Properties.First(a => a.PropertyName == kvp.Key.Name).Override;
                } else {
                    switch (kvp.Key.PropertyType)
                    {
                        case "BiomesList":
                            ((ListBox)kvp.Value.Item1).SelectedIndices.Clear();
                            ((ListBox)kvp.Value.Item1).SelectedIndex = -1;
                            break;
                        case "Bool":
                            ((Button)kvp.Value.Item1).Text = "";
                            ((Button)kvp.Value.Item1).ForeColor = Color.Empty;
                            break;
                        case "Color":
                            kvp.Value.Item5.BackColor = Color.White;
                            kvp.Value.Item1.Text = "";
                            break;
                        case "Float":
                        case "Int":
                            kvp.Value.Item1.Text = "";
                            break;
                        case "String":
                        case "BigString":
                            kvp.Value.Item1.Text = "";
                            break;
                        case "ResourceQueue":
                            ((ListBox)kvp.Value.Item1).Items.Clear();
                            ((ListBox)kvp.Value.Item1).SelectedIndex = -1;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                            biomeConfig.SetProperty(kvp.Key, null, false, false);
                            break;
                    }
                    kvp.Value.Item2.Checked = false;
                }

                this.panel3.Focus();
            }

            void btAddResourceQueueItem_Click(object sender, EventArgs e)
            {
                string propertyValue = "";
                if (PopUpForm.InputBox("Add item", "", ref propertyValue, true) == DialogResult.OK)
                {
                    if(propertyValue != null && !String.IsNullOrEmpty(propertyValue.Trim()))
                    {
                        TCProperty property = ResourceQueueInputs[sender];
                        AddToResourceQueue(property, propertyValue);
                    }
                }
            }

            private void AddToResourceQueue(TCProperty property, string propertyValue, bool showDuplicateWarnings = true)
            {
                if (propertyValue == null || string.IsNullOrEmpty(propertyValue.Trim().Replace("\r", "").Replace("\n", "")))
                {
                    return;
                }

                if (!copyPasting)
                {
                    addingMultipleResourcesXToAll = DialogResult.Abort;
                    addingMultipleResourcesXToAll2 = DialogResult.Abort;
                    addingMultipleResourcesXToAll3 = DialogResult.Abort;
                }

                propertyValue = propertyValue.Replace("\r", "").Replace("\n", "");

                ListBox lb = ((ListBox)Session.BiomeSettingsInputs[property].Item1);
                List<string> newPropertyValue = new List<string>();
                foreach(string item in lb.Items)
                {
                    newPropertyValue.Add(item.Trim());
                }

                bool bAllowed = false;
                bool bOverrideExisting = false;
                ResourceQueueItem selectedOption = null;
                foreach (ResourceQueueItem option in Session.VersionConfig.ResourceQueueOptions)
                {
                    if (propertyValue.StartsWith(option.Name))
                    {
                        selectedOption = option;
                        break;
                    }
                }

                if (newPropertyValue != null && selectedOption != null)
                {
                    DialogResult permissionGiven = DialogResult.Abort;
                    bool bFound = false;
                    if ((!selectedOption.HasUniqueParameter && newPropertyValue.Any(a => a.StartsWith(selectedOption.Name)) || (selectedOption.HasUniqueParameter && newPropertyValue.Any(a => (string)a == (string)propertyValue))))
                    {
                        if (selectedOption.HasUniqueParameter && !newPropertyValue.Any(a => (string)a == (string)propertyValue))
                        {
                            if (addingMultipleResourcesXToAll == DialogResult.Abort)
                            {
                                string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                //addingMultipleResourcesXToAll = PopUpForm.CustomYesNoBox("Keep existing items?", "One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                addingMultipleResourcesXToAll = System.Windows.Forms.DialogResult.Yes;
                            }
                            permissionGiven = addingMultipleResourcesXToAll;
                            if (permissionGiven == DialogResult.No)
                            {
                                bOverrideExisting = true;
                            }
                            if (permissionGiven == DialogResult.Cancel)
                            {
                                return;
                            }
                        } else {
                            if (newPropertyValue.Any(a => (string)a == (string)propertyValue))
                            {
                                if (addingMultipleResourcesXToAll2 == DialogResult.Abort)
                                {
                                    //addingMultipleResourcesXToAll2 = PopUpForm.CustomYesNoBox("Add exact duplicate?", "Resource \"" + propertyValue + "\" already exists, do you still want to add it?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Yes", "No");
                                    addingMultipleResourcesXToAll2 = System.Windows.Forms.DialogResult.Yes;
                                }
                                permissionGiven = addingMultipleResourcesXToAll2;
                            } else {
                                if (addingMultipleResourcesXToAll3 == DialogResult.Abort)
                                {
                                    //addingMultipleResourcesXToAll3 = PopUpForm.CustomYesNoBox("Keep same resource with different parameters?", "One or more \"" + selectedOption.Name + "\" resources already exist but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                    addingMultipleResourcesXToAll3 = System.Windows.Forms.DialogResult.Yes;
                                }
                                permissionGiven = addingMultipleResourcesXToAll3;
                                if (permissionGiven == DialogResult.No)
                                {
                                    bOverrideExisting = true;
                                }
                                if (permissionGiven == DialogResult.Cancel)
                                {
                                    return;
                                }
                            }
                        }
                        if (permissionGiven == DialogResult.Yes)
                        {
                            bAllowed = true;
                        }
                        bFound = true;
                    }
                    if (!bFound || bOverrideExisting)
                    {
                        if (propertyValue.Contains('(') && propertyValue.Contains(')'))
                        {
                            if (selectedOption.IsUnique)
                            {
                                List<string> possibleDuplicates = new List<string>();
                                foreach (string value2 in newPropertyValue)
                                {
                                    if (value2.StartsWith(selectedOption.Name))
                                    {
                                        if (!selectedOption.HasUniqueParameter && bOverrideExisting) // Is duplicate tag, but not necessarily same params
                                        {
                                            //Delete old add new
                                            DeleteResourceQueueItem(property, value2);
                                        } else {
                                            possibleDuplicates.Add(value2);
                                        }
                                    }
                                }
                                if (possibleDuplicates.Any())
                                {
                                    string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                    foreach (string existingValue in possibleDuplicates)
                                    {
                                        string[] existingParameters = existingValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                        if (existingParameters.Length > selectedOption.UniqueParameterIndex && newParameters.Length > selectedOption.UniqueParameterIndex)
                                        {
                                            bool bFound4 = false;
                                            if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                            {
                                                if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(newParameters[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                {
                                                    if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                    {
                                                        bFound4 = true;
                                                    }
                                                }
                                            } else {
                                                if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                {
                                                    bFound4 = true;
                                                }
                                            }
                                            if (bFound4)
                                            {
                                                //Delete old add new
                                                if (permissionGiven == DialogResult.Abort)
                                                {
                                                    if (addingMultipleResourcesXToAll == DialogResult.Abort)
                                                    {
                                                        //addingMultipleResourcesXToAll = PopUpForm.CustomYesNoBox("Keep existing items?", "One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                                        addingMultipleResourcesXToAll = System.Windows.Forms.DialogResult.Yes;
                                                    }
                                                    permissionGiven = addingMultipleResourcesXToAll;
                                                }
                                                if (permissionGiven == DialogResult.No)
                                                {
                                                    DeleteResourceQueueItem(property, existingValue);
                                                }
                                                if (permissionGiven == DialogResult.Cancel)
                                                {
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    if (newParameters.Length > selectedOption.UniqueParameterIndex)
                                    {
                                        bAllowed = true;
                                    } else {
                                        PopUpForm.CustomMessageBox("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
                                    }
                                } else {
                                    bAllowed = true;
                                }
                            } else {
                                bAllowed = true;
                            }
                        } else {
                            string[] propertyNames = Session.VersionConfig.ResourceQueueOptions.Select(a => a.Name).ToArray();
                            string sPropertyNames = "";
                            foreach (string a in propertyNames)
                            {
                                sPropertyNames += a + "\r\n";
                            }
                            PopUpForm.CustomMessageBox("Cannot add item. Setting name was not recognized. Legal setting names are: \r\n" + sPropertyNames, "Error: Illegal input");
                        }
                    }
                } else {
                    if (newPropertyValue == null)
                    {
                        bAllowed = true;
                    }
                    else if (selectedOption == null)
                    {
                        PopUpForm.CustomMessageBox("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is SettingName(Parameters)", "Error: Illegal input");
                    }
                }
                if(bAllowed)
                {
                    if (!copyPasting)
                    {
                        AddResourceToBiome(property, propertyValue);
                    } else {
                        resourcesToAdd.Add(propertyValue);
                    }
                }
            }

            public void AddResourceToBiome(TCProperty property, string propertyValue)
            {
                IgnoreOverrideCheckChangedBiome = true;
                Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                BiomeConfig biomeConfig = g.BiomeConfig;

                BiomeConfig biomeDefaultConfig = null;
                if (g.showDefaults)
                {
                    biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                }

                string s = !String.IsNullOrEmpty(biomeConfig.GetPropertyValueAsString(property)) ? biomeConfig.GetPropertyValueAsString(property) + "\r\n" + propertyValue.Trim() : propertyValue.Trim();
                if (biomeDefaultConfig != null)
                {
                    bool bIsDefault = true;
                    if (biomeConfig.GetPropertyValueAsString(property) == null)
                    {
                        s = biomeDefaultConfig.GetPropertyValueAsString(property) + "\r\n" + propertyValue.Trim();
                        bIsDefault = false;
                    } else {
                        bIsDefault = Utils.TCSettingsUtils.CompareResourceQueues(s, biomeDefaultConfig.GetPropertyValueAsString(property));
                    }

                    if (!bIsDefault)
                    {
                        biomeConfig.SetProperty(property, s, Session.BiomeSettingsInputs[property].Item6 != null && ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.BiomeSettingsInputs[property].Item6 != null && ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = true;
                        Session.BiomeSettingsInputs[property].Item2.Checked = true;
                    } else {
                        biomeConfig.SetProperty(property, null, Session.BiomeSettingsInputs[property].Item6 != null && ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.BiomeSettingsInputs[property].Item6 != null && ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = false;
                        Session.BiomeSettingsInputs[property].Item2.Checked = false;
                    }
                } else {
                    biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = true;
                    Session.BiomeSettingsInputs[property].Item2.Checked = true;
                    biomeConfig.SetProperty(property, s, Session.BiomeSettingsInputs[property].Item6 != null && ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.BiomeSettingsInputs[property].Item6 != null && ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);                    
                }
                biomeConfig.SetProperty(property, s.Trim(), Session.BiomeSettingsInputs[property].Item6 != null && ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.BiomeSettingsInputs[property].Item6 != null && ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                ((ListBox)Session.BiomeSettingsInputs[property].Item1).Items.Clear();
                string[] resourceQueueItemNames = biomeConfig.GetPropertyValueAsString(property).Replace("\r", "").Split('\n');
                foreach (string resourceQueueItemName in resourceQueueItemNames)
                {
                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                    {
                        ((ListBox)Session.BiomeSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                    }
                }
                IgnoreOverrideCheckChangedBiome = false;
            }

            void btEditResourceQueueItem_Click(object sender, EventArgs e)
            {
                addingMultipleResourcesXToAll = DialogResult.Abort;
                addingMultipleResourcesXToAll2 = DialogResult.Abort;
                addingMultipleResourcesXToAll3 = DialogResult.Abort;

                TCProperty property = ResourceQueueInputs[sender];
                ListBox lb = ((ListBox)Session.BiomeSettingsInputs[property].Item1);
                if (lb.SelectedItem != null)
                {
                    string originalPropertyValue = (string)lb.SelectedItem.ToString();
                    string propertyValue = (string)lb.SelectedItem.ToString();

                    if (PopUpForm.InputBox("Edit item", "", ref propertyValue, true) == DialogResult.OK && !propertyValue.Equals(originalPropertyValue))
                    {
                        if (propertyValue != null && !String.IsNullOrEmpty(propertyValue.Trim()))
                        {
                            List<string> newPropertyValue = new List<string>();
                            bool bFound1 = false;
                            foreach (string item in lb.Items)
                            {
                                if (!item.Equals(originalPropertyValue) || bFound1)
                                {
                                    newPropertyValue.Add(item.Trim());
                                } else {
                                    bFound1 = true;
                                }
                            }

                            bool bAllowed = false;
                            bool bOverrideExisting = false;
                            ResourceQueueItem selectedOption = null;
                            foreach (ResourceQueueItem option in Session.VersionConfig.ResourceQueueOptions)
                            {
                                if (propertyValue.StartsWith(option.Name))
                                {
                                    selectedOption = option;
                                    break;
                                }
                            }

                            if (newPropertyValue != null && selectedOption != null)
                            {
                                DialogResult permissionGiven = DialogResult.Abort;
                                bool bFound = false;
                                if ((!selectedOption.HasUniqueParameter && newPropertyValue.Any(a => a.StartsWith(selectedOption.Name)) || (selectedOption.HasUniqueParameter && newPropertyValue.Any(a => (string)a == (string)propertyValue))))
                                {
                                    if (selectedOption.HasUniqueParameter && !newPropertyValue.Any(a => (string)a == (string)propertyValue))
                                    {
                                        if (addingMultipleResourcesXToAll == DialogResult.Abort)
                                        {
                                            string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                            //addingMultipleResourcesXToAll = PopUpForm.CustomYesNoBox("Keep existing items?", "One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                            addingMultipleResourcesXToAll = System.Windows.Forms.DialogResult.Yes;
                                        }
                                        permissionGiven = addingMultipleResourcesXToAll;
                                        if (permissionGiven == DialogResult.No)
                                        {
                                            bOverrideExisting = true;
                                        }
                                        if (permissionGiven == DialogResult.Cancel)
                                        {
                                            return;
                                        }
                                    } else {
                                        if (newPropertyValue.Any(a => (string)a == (string)propertyValue))
                                        {
                                            if (addingMultipleResourcesXToAll2 == DialogResult.Abort)
                                            {
                                                //addingMultipleResourcesXToAll2 = PopUpForm.CustomYesNoBox("Add exact duplicate?", "Resource \"" + propertyValue + "\" already exists, do you still want to add it?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Yes", "No");
                                                addingMultipleResourcesXToAll2 = System.Windows.Forms.DialogResult.Yes;
                                            }
                                            permissionGiven = addingMultipleResourcesXToAll2;
                                        } else {
                                            if (addingMultipleResourcesXToAll3 == DialogResult.Abort)
                                            {
                                                //addingMultipleResourcesXToAll3 = PopUpForm.CustomYesNoBox("Keep same resource with different parameters?", "One or more \"" + selectedOption.Name + "\" resources already exist but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                                addingMultipleResourcesXToAll3 = System.Windows.Forms.DialogResult.Yes;
                                            }
                                            permissionGiven = addingMultipleResourcesXToAll3;
                                            if (permissionGiven == DialogResult.No)
                                            {
                                                bOverrideExisting = true;
                                            }
                                            if (permissionGiven == DialogResult.Cancel)
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    if (permissionGiven == DialogResult.Yes)
                                    {
                                        bAllowed = true;
                                    }
                                    bFound = true;
                                }
                                if (!bFound || bOverrideExisting)
                                {
                                    if (propertyValue.Contains('(') && propertyValue.Contains(')'))
                                    {
                                        if (selectedOption.IsUnique)
                                        {
                                            List<string> possibleDuplicates = new List<string>();
                                            foreach (string value2 in newPropertyValue)
                                            {
                                                if (value2.StartsWith(selectedOption.Name))
                                                {
                                                    if (!selectedOption.HasUniqueParameter && bOverrideExisting) // Is duplicate tag, but not necessarily same params
                                                    {
                                                        //Delete old add new
                                                        DeleteResourceQueueItem(property, value2);
                                                    } else {
                                                        possibleDuplicates.Add(value2);
                                                    }
                                                }
                                            }
                                            if (possibleDuplicates.Any())
                                            {
                                                string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                foreach (string existingValue in possibleDuplicates)
                                                {
                                                    string[] existingParameters = existingValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                    if (existingParameters.Length > selectedOption.UniqueParameterIndex && newParameters.Length > selectedOption.UniqueParameterIndex)
                                                    {
                                                        bool bFound4 = false;
                                                        if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                                        {
                                                            if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(newParameters[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                            {
                                                                if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                                {
                                                                    bFound4 = true;
                                                                }
                                                            }
                                                        } else {
                                                            if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                            {
                                                                bFound4 = true;
                                                            }
                                                        }
                                                        if (bFound4)
                                                        {
                                                            //Delete old add new
                                                            if (permissionGiven == DialogResult.Abort)
                                                            {
                                                                if (addingMultipleResourcesXToAll == DialogResult.Abort)
                                                                {
                                                                    addingMultipleResourcesXToAll = PopUpForm.CustomYesNoBox("Keep existing items?", "One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources or override them?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep", "Override");
                                                                }
                                                                permissionGiven = addingMultipleResourcesXToAll;
                                                            }
                                                            if (permissionGiven == DialogResult.No)
                                                            {
                                                                DeleteResourceQueueItem(property, existingValue);
                                                            }
                                                            if (permissionGiven == DialogResult.Cancel)
                                                            {
                                                                return;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (newParameters.Length > selectedOption.UniqueParameterIndex)
                                                {
                                                    bAllowed = true;
                                                } else {
                                                    PopUpForm.CustomMessageBox("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
                                                }
                                            } else {
                                                bAllowed = true;
                                            }
                                        } else {
                                            bAllowed = true;
                                        }
                                    } else {
                                        string[] propertyNames = Session.VersionConfig.ResourceQueueOptions.Select(a => a.Name).ToArray();
                                        string sPropertyNames = "";
                                        foreach (string a in propertyNames)
                                        {
                                            sPropertyNames += a + "\r\n";
                                        }
                                        PopUpForm.CustomMessageBox("Cannot add item. Setting name was not recognized. Legal setting names are: \r\n" + sPropertyNames, "Error: Illegal input");
                                    }
                                }
                            } else {
                                if(newPropertyValue == null)
                                {
                                    bAllowed = true;
                                }
                                else if(selectedOption == null)
                                {
                                    PopUpForm.CustomMessageBox("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is SettingName(Parameters)", "Error: Illegal input");
                                }
                            }
                            if(bAllowed)
                            {
                                DeleteResourceQueueItem(property, originalPropertyValue);
                                AddResourceToBiome(property, propertyValue);
                            }
                        }
                    }
                }
            }

            void btDeleteResourceQueueItem_Click(object sender, EventArgs e)
            {
                TCProperty property = ResourceQueueInputs[sender];
                ListBox lb = ((ListBox)Session.BiomeSettingsInputs[property].Item1);
                if(lb.SelectedItem != null)
                {
                    List<string> itemsToRemove = new List<string>();
                    foreach(string selectedItem in lb.SelectedItems)
                    {
                        itemsToRemove.Add(selectedItem);
                    }
                    foreach(string selectedItem in itemsToRemove)
                    {
                        DeleteResourceQueueItem(property, selectedItem);
                    }
                }
            }

            private void DeleteResourceQueueItem(TCProperty property, string selectedItem)
            {
                IgnoreOverrideCheckChangedBiome = true;
                Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                BiomeConfig biomeConfig = g.BiomeConfig;

                BiomeConfig biomeDefaultConfig = null;
                if (g.showDefaults)
                {
                    biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                }

                string s = biomeConfig.GetPropertyValueAsString(property) ?? (biomeDefaultConfig != null && biomeDefaultConfig.GetPropertyValueAsString(property) != null ? biomeDefaultConfig.GetPropertyValueAsString(property) : "");
                string[] resourceQueueItemNames = s.Replace("\r", "").Split('\n');
                foreach (string resourceQueueItemName in resourceQueueItemNames)
                {
                    if (resourceQueueItemName == selectedItem)
                    {
                        int replaceStartIndex = 0;
                        foreach (string resourceQueueItemName2 in resourceQueueItemNames)
                        {
                            if (resourceQueueItemName == resourceQueueItemName2)
                            {
                                break;
                            }
                            replaceStartIndex += resourceQueueItemName2.Length;
                            while (s.Substring(replaceStartIndex, 1) == "\r" || s.Substring(replaceStartIndex, 1) == "\n")
                            {
                                replaceStartIndex += 1;
                            }
                        }
                        int length = resourceQueueItemName.Length;
                        while (replaceStartIndex + length + 1 < s.Length && (s.Substring(replaceStartIndex + length, 1) == "\r" || s.Substring(replaceStartIndex + length, 1) == "\n"))
                        {
                            length += 1;
                        }
                        s = s.Remove(replaceStartIndex, length).Trim();
                        break;
                    }
                }

                bool bIsDefault = Utils.TCSettingsUtils.CompareResourceQueues(s, biomeDefaultConfig != null && biomeDefaultConfig.GetPropertyValueAsString(property) != null ? biomeDefaultConfig.GetPropertyValueAsString(property) : "");
                if (!bIsDefault)
                {
                    biomeConfig.SetProperty(property, s, Session.BiomeSettingsInputs[property].Item6 != null && ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.BiomeSettingsInputs[property].Item6 != null && ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = true;
                    Session.BiomeSettingsInputs[property].Item2.Checked = true;
                } else {
                    biomeConfig.SetProperty(property, null, Session.BiomeSettingsInputs[property].Item6 != null && ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, Session.BiomeSettingsInputs[property].Item6 != null && ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = false;
                    Session.BiomeSettingsInputs[property].Item2.Checked = false;
                }

                ((ListBox)Session.BiomeSettingsInputs[property].Item1).Items.Clear();
                string[] resourceQueueItemNames3 = s.Replace("\r", "").Split('\n');
                foreach (string resourceQueueItemName in resourceQueueItemNames3)
                {
                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                    {
                        ((ListBox)Session.BiomeSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                    }
                }
                IgnoreOverrideCheckChangedBiome = false;
            }

            private void lbGroups_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (lbGroups.SelectedIndex > -1)
                {
                    tlpBiomeSettings1.Visible = true;

                    panel3.SuspendLayout();
                    tlpBiomeSettingsContainer.SuspendLayout();
                    tlpBiomeSettings1.SuspendLayout();

                    label3.Visible = true;

                    lbGroup.Items.Clear();
                    lbBiomes.Items.Clear();
                    Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    foreach (string biomeName in g.Biomes)
                    {
                        lbGroup.Items.Add(biomeName);
                    }
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (!g.Biomes.Any(a => a == biomeConfig.BiomeName))
                        {
                            lbBiomes.Items.Add(biomeConfig.BiomeName);
                        }
                    }

                    BiomeConfig biomeConfigDefaultValues = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);

                    if (!g.showDefaults)
                    {
                        foreach (Button bSetDefaults in Session.BiomeSettingsInputs.Select(a => a.Value.Item3))
                        {
                            Session.ToolTip1.SetToolTip(bSetDefaults, "Clear");
                            bSetDefaults.Text = "C";
                        }
                    } else {
                        if (biomeConfigDefaultValues != null)
                        {
                            foreach (Button bSetDefaults in Session.BiomeSettingsInputs.Select(a => a.Value.Item3))
                            {
                                Session.ToolTip1.SetToolTip(bSetDefaults, "Set to default");
                                bSetDefaults.Text = "<";
                            }
                        } else {
                            foreach (Button bSetDefaults in Session.BiomeSettingsInputs.Select(a => a.Value.Item3))
                            {
                                Session.ToolTip1.SetToolTip(bSetDefaults, "Clear");
                                bSetDefaults.Text = "C";
                            }
                        }
                    }

                    IgnorePropertyInputChangedBiome = true;
                    IgnoreOverrideCheckChangedBiome = true;

                    foreach (TCProperty property in Session.VersionConfig.BiomeConfig)
                    {
                        switch (property.PropertyType)
                        {
                            case "BiomesList":
                                ((ListBox)Session.BiomeSettingsInputs[property].Item1).SelectedIndices.Clear();
                                ((ListBox)Session.BiomeSettingsInputs[property].Item1).SelectedIndex = -1;
                                if (g.showDefaults)
                                {
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Visible = false;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Visible = false;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                } else {
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Visible = true;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Visible = true;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                }
                                ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                            break;
                            case "Bool":
                                ((Button)Session.BiomeSettingsInputs[property].Item1).Text = "";
                                ((Button)Session.BiomeSettingsInputs[property].Item1).ForeColor = Color.Empty;
                            break;
                            case "Color":
                                Session.BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                Session.BiomeSettingsInputs[property].Item1.Text = "";
                            break;
                            case "Float":
                            case "Int":
                                Session.BiomeSettingsInputs[property].Item1.Text = "";
                            break;
                            case "String":
                            case "BigString":
                                Session.BiomeSettingsInputs[property].Item1.Text = "";
                            break;
                            case "ResourceQueue":
                                ((ListBox)Session.BiomeSettingsInputs[property].Item1).Items.Clear();
                                ((ListBox)Session.BiomeSettingsInputs[property].Item1).SelectedIndex = -1;
                                if (g.showDefaults)
                                {
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Visible = false;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Visible = false;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                } else {
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Visible = true;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Visible = true;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                }
                                ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                            break;
                        }
                        Session.BiomeSettingsInputs[property].Item2.Checked = false;

                        string s = "";
                        if (!g.showDefaults)
                        {
                            s = g.BiomeConfig.GetPropertyValueAsString(property);
                        } else {
                            BiomeConfig biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                            s = biomeDefaultConfig.GetPropertyValueAsString(property);
                            if (g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override)
                            {
                                if(property.PropertyType != "ResourceQueue" || g.BiomeConfig.GetPropertyValueAsString(property) != null)
                                {
                                    s = g.BiomeConfig.GetPropertyValueAsString(property);
                                }
                            }
                        }

                        if (s != null || g.BiomeConfig.GetPropertyMerge(property) || (g.BiomeConfig.GetPropertyOverrideParentValues(property) && (property.PropertyType == "ResourceQueue" || property.PropertyType == "BiomesList")))
                        {
                            Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = Session.BiomeSettingsInputs[property];

                            string propertyValue = s;
                            switch (property.PropertyType)
                            {
                                case "BiomesList":
                                    ((ListBox)Session.BiomeSettingsInputs[property].Item1).SelectedItems.Clear();
                                    string[] biomeNames = propertyValue != null ? propertyValue.Split(',') : new string[0];
                                    for (int k = 0; k < biomeNames.Length; k++)
                                    {
                                        if (Session.BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                        {
                                            for (int l = 0; l < ((ListBox)Session.BiomeSettingsInputs[property].Item1).Items.Count; l++)
                                            {
                                                if (((string)((ListBox)Session.BiomeSettingsInputs[property].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                                {
                                                    ((ListBox)Session.BiomeSettingsInputs[property].Item1).SelectedItems.Add(((ListBox)Session.BiomeSettingsInputs[property].Item1).Items[l]);
                                                }
                                            }
                                        }
                                    }
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = !g.BiomeConfig.GetPropertyMerge(property);
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = g.BiomeConfig.GetPropertyMerge(property);
                                    ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = g.BiomeConfig.GetPropertyOverrideParentValues(property);
                                    boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                break;
                                case "Bool":
                                    if (propertyValue.ToLower() == "true")
                                    {
                                        ((Button)boxes.Item1).Text = "true";
                                        ((Button)boxes.Item1).ForeColor = Color.Green;
                                    }
                                    else if (propertyValue.ToLower() == "false")
                                    {
                                        ((Button)boxes.Item1).Text = "false";
                                        ((Button)boxes.Item1).ForeColor = Color.Red;
                                    } else {
                                        ((Button)boxes.Item1).Text = "";
                                        ((Button)boxes.Item1).ForeColor = Color.Empty;
                                    }
                                    boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                break;
                                case "Color":
                                    if ((propertyValue.StartsWith("0x") && propertyValue.Length == 8) || (propertyValue.StartsWith("#") && propertyValue.Length == 7))
                                    {
                                        boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                        bool bException = false;
                                        try
                                        {
                                            boxes.Item5.BackColor = ColorTranslator.FromHtml(propertyValue);
                                            string value = "";
                                            if (Session.SettingsType.ColorType == "0x")
                                            {
                                                value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                            }
                                            else if (Session.SettingsType.ColorType == "#")
                                            {
                                                value = "#" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                            }
                                            boxes.Item1.Text = value;
                                            if (biomeConfigDefaultValues == null || boxes.Item2.Checked || ColorTranslator.FromHtml(value.ToUpper()).ToArgb() != ColorTranslator.FromHtml(biomeConfigDefaultValues.GetPropertyValueAsString(property).ToUpper()).ToArgb())
                                            {
                                                boxes.Item2.Checked = true;
                                            } else {
                                                boxes.Item2.Checked = false;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            bException = true;
                                        }
                                        if (bException)
                                        {
                                            string propertyString = biomeConfigDefaultValues.GetPropertyValueAsString(property);
                                            if ((propertyString.StartsWith("0x") && propertyString.Length == 8) || (propertyString.StartsWith("#") && propertyString.Length == 7))
                                            {
                                                try
                                                {
                                                    boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyString);
                                                    string value = "";
                                                    if (Session.SettingsType.ColorType == "0x")
                                                    {
                                                        value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                    }
                                                    else if (Session.SettingsType.ColorType == "#")
                                                    {
                                                        value = "#" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                    }
                                                    boxes.Item1.Text = value;
                                                }
                                                catch (Exception ex2)
                                                {
                                                    boxes.Item5.BackColor = Color.White;
                                                    boxes.Item1.Text = "";
                                                }
                                                boxes.Item2.Checked = false;
                                            } else {
                                                boxes.Item5.BackColor = Color.White;
                                                boxes.Item1.Text = "";
                                                boxes.Item2.Checked = false;
                                            }
                                        }
                                    } else {
                                        boxes.Item5.BackColor = Color.White;
                                        boxes.Item1.Text = "";
                                        boxes.Item2.Checked = false;
                                    }
                                break;
                                case "Float":
                                case "Int":
                                    string newText = propertyValue;
                                    int numberOfDecimals = property.PropertyType == "Int" ? 0 : newText.IndexOf(".") > 0 ? newText.Length - (newText.IndexOf(".") + 1) : 0;
                                    ((NumericUpDownExt)boxes.Item1).DecimalPlaces = numberOfDecimals;
                                    boxes.Item1.Text = newText;
                                    boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                break;
                                case "String":
                                case "BigString":
                                    boxes.Item1.Text = propertyValue;
                                    boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Override;
                                break;
                                case "ResourceQueue":
                                    string[] resourceQueueItemNames = {};
                                    if(propertyValue != null)
                                    {
                                        resourceQueueItemNames = propertyValue.Replace("\r", "").Split('\n');
                                    }                                
                                
                                    foreach (string resourceQueueItemName in resourceQueueItemNames)
                                    {
                                        if (!String.IsNullOrEmpty(resourceQueueItemName))
                                        {
                                            ((ListBox)Session.BiomeSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                                        }
                                    }
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = !g.BiomeConfig.GetPropertyMerge(property);
                                    ((RadioButton)Session.BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = g.BiomeConfig.GetPropertyMerge(property);
                                    ((CheckBox)Session.BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = g.BiomeConfig.GetPropertyOverrideParentValues(property);
                                    boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                break;
                            }                        
                        }
                    }

                    panel3.ResumeLayout();
                    tlpBiomeSettingsContainer.ResumeLayout();
                    tlpBiomeSettings1.ResumeLayout();

                    IgnoreOverrideCheckChangedBiome = false;
                    IgnorePropertyInputChangedBiome = false;
                } else {
                    tlpBiomeSettings1.Visible = false;
                    label3.Visible = false;
                }
            }

            private void btGroupMoveUp_Click(object sender, EventArgs e)
            {
                if (lbGroups.SelectedIndex > -1 && lbGroups.Items.Count > 1 && lbGroups.SelectedIndex - 1 > -1)
                {
                    int selectedIndex = lbGroups.SelectedIndex;
                    object a = lbGroups.Items[selectedIndex];
                    object b = lbGroups.Items[selectedIndex - 1];
                    lbGroups.Items[selectedIndex - 1] = a;
                    lbGroups.Items[selectedIndex] = b;
                    lbGroups.SelectedIndex = selectedIndex - 1;
                }

                this.panel3.Focus();
            }

            private void btGroupMoveDown_Click(object sender, EventArgs e)
            {
                if (lbGroups.SelectedIndex > -1 && lbGroups.Items.Count > 1 && lbGroups.SelectedIndex + 1 < lbGroups.Items.Count)
                {
                    int selectedIndex = lbGroups.SelectedIndex;
                    object a = lbGroups.Items[selectedIndex];
                    object b = lbGroups.Items[selectedIndex + 1];
                    lbGroups.Items[selectedIndex + 1] = a;
                    lbGroups.Items[selectedIndex] = b;
                    lbGroups.SelectedIndex = selectedIndex + 1;
                }

                this.panel3.Focus();
            }

            private void btNewGroup_Click(object sender, EventArgs e)
            {
                string groupName = "";
                List<string> biomesToAdd = PopUpForm.BiomeListSelectionBox(ref groupName, BiomeConfigsDefaultValues.Select(a => a.BiomeName).ToList());

                if (!String.IsNullOrWhiteSpace(groupName))
                {
                    if (!Session.BiomeGroups.Any(a => a.Name == groupName))
                    {
                        lbGroups.Items.Add(groupName);
                        Group g = new Group(groupName, Session.VersionConfig);
                        g.Biomes.AddRange(biomesToAdd);
                        Session.BiomeGroups.Add(g);
                        lbGroups.SelectedIndex = -1;
                        lbGroups.SelectedIndex = lbGroups.Items.Count - 1;
                    } else {
                        PopUpForm.CustomMessageBox("A group with this name already exists", "Error: Illegal input");
                    }
                }

                this.panel3.Focus();
            }

            private void btEditGroup_Click(object sender, EventArgs e)
            {
                if(lbGroups.SelectedIndex > -1)
                {
                    string groupName = (string)lbGroups.SelectedItem;
                    if(PopUpForm.InputBox("Rename group", "Enter a name for the group. Only a-z A-Z 0-9 space + - and _ are allowed.", ref groupName) == DialogResult.OK)
                    {
                        if(!string.IsNullOrWhiteSpace(groupName))
                        {
                            if (!Session.BiomeGroups.Any(a => a.Name == groupName))
                            {
                                Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.Items[lbGroups.SelectedIndex]).Name = groupName;
                                lbGroups.Items[lbGroups.SelectedIndex] = groupName;                        
                            } else {
                                PopUpForm.CustomMessageBox("A group with this name already exists", "Error: Illegal input");
                            }
                        }
                    }
                }
                this.panel3.Focus();
            }

            private void btDeleteGroup_Click(object sender, EventArgs e)
            {
                if (lbGroups.SelectedIndex > -1)
                {
                    int selectedIndex = lbGroups.SelectedIndex;
                    Session.BiomeGroups.RemoveAll(a => a.Name == (string)lbGroups.SelectedItem);
                    lbGroups.Items.Remove(lbGroups.SelectedItem);
                    lbGroups.SelectedIndex = lbGroups.Items.Count > selectedIndex ? selectedIndex : (lbGroups.Items.Count > selectedIndex - 1 ? selectedIndex - 1 : (lbGroups.Items.Count > 0 ? 0 : -1));
                    if(lbGroups.Items.Count < 1)
                    {
                        lbGroup.Items.Clear();
                        lbBiomes.Items.Clear();
                    }
                }
                this.panel3.Focus();
            }

            private void btCloneGroup_Click(object sender, EventArgs e)
            {
                if (lbGroups.SelectedIndex > -1)
                {
                    Group groupToClone = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.Items[lbGroups.SelectedIndex]);
                    if(groupToClone != null && groupToClone.Biomes.Count > 1)
                    {
                        string groupName = "";
                        if (PopUpForm.InputBox("Name new group", "Enter a name for the group. Only a-z A-Z 0-9 space + - and _ are allowed.", ref groupName) == DialogResult.OK)
                        {
                            if (!string.IsNullOrWhiteSpace(groupName))
                            {
                                if (!Session.BiomeGroups.Any(a => a.Name == groupName))
                                {
                                    Group g = new Group(groupName, Session.VersionConfig);
                                    g.Biomes.AddRange(groupToClone.Biomes);                                                                
                                    foreach(Property prop in groupToClone.BiomeConfig.Properties)
                                    {
                                        TCProperty tcProp = Session.VersionConfig.BiomeConfig.FirstOrDefault(a => a.Name == prop.PropertyName);
                                        if(tcProp != null)
                                        {
                                            g.BiomeConfig.SetProperty(tcProp, groupToClone.BiomeConfig.GetPropertyValueAsString(tcProp), groupToClone.BiomeConfig.GetPropertyMerge(tcProp), groupToClone.BiomeConfig.GetPropertyOverrideParentValues(tcProp));
                                            g.BiomeConfig.Properties.First(a => a.PropertyName == tcProp.Name).Override = true;
                                        }
                                    }
                                    Session.BiomeGroups.Add(g);

                                    lbGroups.Items.Add(groupName);
                                    lbGroups.SelectedIndex = -1;
                                    lbGroups.SelectedIndex = lbGroups.Items.Count - 1;

                                } else {
                                    PopUpForm.CustomMessageBox("A group with this name already exists", "Error: Illegal input");
                                }
                            }
                        }
                    } else {
                        PopUpForm.CustomMessageBox("Cannot clone a biome group with a single biome (yet, sorry).");
                    }
                }

                this.panel3.Focus();
            }

            private void lbGroup_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (lbGroup.SelectedIndex > -1)
                {
                    lbBiomes.SelectedIndex = -1;
                }
            }

            private void btAddToGroup_Click(object sender, EventArgs e)
            {
                if (lbBiomes.SelectedItems.Count > 0)
                {
                    List<string> itemsToDelete = new List<string>();
                    foreach(string selectedItem in lbBiomes.SelectedItems)
                    {
                        Group biomeGroup = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                        if (biomeGroup != null)
                        {
                            if (!biomeGroup.Biomes.Any(a => a == selectedItem))
                            {
                                int selectedIndex = lbBiomes.SelectedIndex;
                                BiomeConfig defaultBiomeConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == selectedItem);
                                if (defaultBiomeConfig != null)
                                {
                                    itemsToDelete.Add(selectedItem);
                                    lbGroup.Items.Add(selectedItem);
                                    biomeGroup.Biomes.Add(defaultBiomeConfig.BiomeName);
                                }
                            }
                        }
                    }
                    lbBiomes.SelectedIndex = -1;
                    foreach (string selectedItem in itemsToDelete)
                    {
                        lbBiomes.Items.Remove(selectedItem);
                    }
                    lbGroups_SelectedIndexChanged(null, null);
                }

                this.panel3.Focus();
            }

            private void btRemoveFromGroup_Click(object sender, EventArgs e)
            {
                if (lbGroup.Items.Count > 1 && lbGroup.SelectedItems.Count > 0 && lbGroup.SelectedItems.Count < lbGroup.Items.Count)
                {
                    List<string> itemsToDelete = new List<string>();
                    Group biomeGroup = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    if (biomeGroup != null)
                    {
                        foreach (string selectedItem in lbGroup.SelectedItems)
                        {
                            if (biomeGroup.Biomes.Any(a => a == selectedItem))
                            {
                                BiomeConfig defaultBiomeConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == selectedItem);
                                if (defaultBiomeConfig != null)
                                {
                                    lbBiomes.Items.Add(selectedItem);
                                }
                                biomeGroup.Biomes.RemoveAll(a => a == selectedItem);
                            }
                            itemsToDelete.Add(selectedItem);
                        }
                    }
                    lbGroup.SelectedIndex = -1;
                    foreach (string selectedItem in itemsToDelete)
                    {
                        lbGroup.Items.Remove(selectedItem);
                    }
                    lbGroups_SelectedIndexChanged(null, null);
                } else {
                    if (lbGroup.Items.Count == 1 || lbGroup.SelectedItems.Count >= lbGroup.Items.Count)
                    {
                        PopUpForm.CustomMessageBox("Cannot delete the selected biome(s). A group must contain at least one biome.", "Error: Illegal input");
                    }
                }

                this.panel3.Focus();
            }

            Control lastBiomeSetting = null;
            private void tbSearchBiomeConfig_TextChanged(object sender, EventArgs e)
            {
                lastBiomeSetting = null;

                bool textIsNullOrEmpty = String.IsNullOrWhiteSpace(this.tbSearchBiomeConfig.Text);

                Control biomeSetting = null;

                foreach (Control ctl in this.tlpBiomeSettings1.Controls)
                {
                    if(ctl is Label)
                    {
                        if (
                            !textIsNullOrEmpty &&
                            ctl.Text.ToLower().Trim().Contains(this.tbSearchBiomeConfig.Text.ToLower().Trim())
                        )
                        {
                            if (biomeSetting == null)
                            {
                                biomeSetting = ctl;
                                ctl.BackColor = Color.DodgerBlue;
                                ctl.ForeColor = Color.White;
                            } else {
                                ctl.BackColor = Color.LightBlue;
                                ctl.ForeColor = Color.Black;
                            }
                        } else {
                            ctl.BackColor = Color.Empty;
                            ctl.ForeColor = Color.Black;
                        }
                    }
                }

                if (biomeSetting != null && !textIsNullOrEmpty)
                {
                    lastBiomeSetting = biomeSetting;
                    this.panel3.ScrollControlIntoView(biomeSetting);
                }
            }

            void tbSearchBiomeConfig_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
            {           
                this.panel3.Focus();
            }

            private void btSearchBiomeConfigPrev_Click(object sender, EventArgs e)
            {
                if (lastBiomeSetting != null)
                {
                    bool textIsNullOrEmpty = String.IsNullOrWhiteSpace(this.tbSearchBiomeConfig.Text);
                    Control previousSetting = null;
                    bool lastBiomeSettingFound = false;
                    bool newBiomeSettingFound = false;

                    foreach (Control ctl in this.tlpBiomeSettings1.Controls)
                    {
                        if (ctl is Label)
                        {
                            if (ctl == lastBiomeSetting)
                            {
                                lastBiomeSettingFound = true;
                                if (previousSetting != null)
                                {
                                    newBiomeSettingFound = true;
                                }
                            } else {
                                if (ctl.Text.ToLower().Trim().Contains(this.tbSearchBiomeConfig.Text.ToLower().Trim()))
                                {
                                    if (!lastBiomeSettingFound || !newBiomeSettingFound)
                                    {
                                        previousSetting = ctl;
                                        previousSetting.BackColor = Color.LightBlue;
                                        previousSetting.ForeColor = Color.Black;
                                    }
                                } else {
                                    ctl.BackColor = Color.Empty;
                                    ctl.ForeColor = Color.Black;
                                }
                            }
                        }
                    }

                    if (previousSetting != null)
                    {
                        if (lastBiomeSetting != previousSetting)
                        {
                            lastBiomeSetting.BackColor = Color.LightBlue;
                            lastBiomeSetting.ForeColor = Color.Black;

                            lastBiomeSetting = previousSetting;

                            previousSetting.BackColor = Color.DodgerBlue;
                            previousSetting.ForeColor = Color.White;
                        }

                        this.panel3.ScrollControlIntoView(previousSetting);
                    }
                }

                this.panel3.Focus();
            }

            private void btSearchBiomeConfigNext_Click(object sender, EventArgs e)
            {
                if (lastBiomeSetting != null)
                {
                    bool textIsNullOrEmpty = String.IsNullOrWhiteSpace(this.tbSearchBiomeConfig.Text);
                    Control biomeSetting = null;
                    Control firstBiomeSetting = null;
                    bool lastBiomeSettingFound = false;

                    foreach (Control ctl in this.tlpBiomeSettings1.Controls)
                    {
                        if(ctl is Label)
                        {
                            if (ctl == lastBiomeSetting)
                            {
                                lastBiomeSettingFound = true;
                            }

                            if (
                                firstBiomeSetting == null &&
                                ctl.Text.ToLower().Trim().Contains(this.tbSearchBiomeConfig.Text.ToLower().Trim())
                            )
                            {
                                firstBiomeSetting = ctl;
                            }

                            if (
                                !textIsNullOrEmpty &&
                                ctl != lastBiomeSetting &&
                                ctl.Text.ToLower().Trim().Contains(this.tbSearchBiomeConfig.Text.ToLower().Trim())
                            )
                            {
                                if (biomeSetting == null && lastBiomeSettingFound)
                                {
                                    biomeSetting = ctl;
                                    ctl.BackColor = Color.DodgerBlue;
                                    ctl.ForeColor = Color.White;
                                } else {
                                    ctl.BackColor = Color.LightBlue;
                                    ctl.ForeColor = Color.Black;
                                }
                            } else {
                                ctl.BackColor = Color.Empty;
                                ctl.ForeColor = Color.Black;
                            }
                        }
                    }

                    if (biomeSetting == null)
                    {
                        biomeSetting = firstBiomeSetting;
                        if (biomeSetting != null)
                        {
                            biomeSetting.BackColor = Color.DodgerBlue;
                            biomeSetting.ForeColor = Color.White;
                        }
                    }

                    if (biomeSetting != null)
                    {
                        if (lastBiomeSetting != biomeSetting)
                        {
                            lastBiomeSetting.BackColor = Color.LightBlue;
                            lastBiomeSetting.ForeColor = Color.Black;

                            lastBiomeSetting = biomeSetting;
                        }
                        this.panel3.ScrollControlIntoView(biomeSetting);
                    }
                }

                this.panel3.Focus();
            }

            void lbBiomesTabSetting_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
            {
                bool senderIsMouseTarget = true;

                if (!((Control)sender).ClientRectangle.Contains(((Control)sender).PointToClient(Control.MousePosition)))
                {
                    senderIsMouseTarget = false;
                    this.panel3.Focus();
                }

                if (!senderIsMouseTarget)
                {
                    HandledMouseEventArgs args = e as HandledMouseEventArgs;
                    if (args != null)
                    {
                        args.Handled = true;
                    }
                }
            }

            void lbBiomesTab_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
            {
                bool senderIsMouseTarget = true;

                if (this.panel3.ClientRectangle.Contains(this.panel3.PointToClient(Control.MousePosition)))
                {
                    senderIsMouseTarget = sender == this.panel3;
                    this.panel3.Focus();
                }
                else if (this.lbGroups.ClientRectangle.Contains(this.lbGroups.PointToClient(Control.MousePosition)))
                {
                    senderIsMouseTarget = sender == this.lbGroups;
                    this.lbGroups.Focus();
                }
                else if (this.lbGroup.ClientRectangle.Contains(this.lbGroup.PointToClient(Control.MousePosition)))
                {
                    senderIsMouseTarget = sender == this.lbGroup;
                    this.lbGroup.Focus();
                }
                else if (this.lbBiomes.ClientRectangle.Contains(this.lbBiomes.PointToClient(Control.MousePosition)))
                {
                    senderIsMouseTarget = sender == this.lbBiomes;
                    this.lbBiomes.Focus();
                }

                if (!senderIsMouseTarget)
                {
                    HandledMouseEventArgs args = e as HandledMouseEventArgs;
                    if (args != null)
                    {
                        args.Handled = true;
                    }
                }
            }        
        
        #endregion

        #region Load / Save / Generate

            void btSave_Click(object sender, EventArgs e)
            {
                sfd.CheckFileExists = false;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    List<Group> groups = new List<Group>();
                    foreach(string groupName in lbGroups.Items)
                    {
                        groups.Add(Session.BiomeGroups.FirstOrDefault(a => a.Name == groupName));
                    }

                    SettingsFile settingsFile = new SettingsFile()
                    {
                        WorldConfig = Session.WorldConfig1,
                        BiomeGroups = groups
                    };

                    var serializer = new DataContractSerializer(typeof(SettingsFile));
                    string xmlString;
                    using (var sw = new StringWriter())
                    {
                        using (var writer = new XmlTextWriter(sw))
                        {
                            writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                            serializer.WriteObject(writer, settingsFile);
                            writer.Flush();
                            xmlString = sw.ToString();
                        }
                    }
                    System.IO.File.WriteAllText(sfd.FileName, xmlString);

                    PopUpForm.CustomMessageBox("Settings saved as: " + sfd.FileName, "Settings saved");
                }

                FocusOnTab();
            }

            void btLoad_Click(object sender, EventArgs e)
            {
                string sErrorMessage = "";
                string sErrorMessage2 = "";
                if (BiomeConfigsDefaultValues.Count > 0)
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        if (!ofd.FileName.ToLower().EndsWith("xml"))
                        {
                            PopUpForm.CustomMessageBox("Error: The selected file was not a valid OTGE save file");
                        } else {
                            SettingsFile settingsFile = null;
                            var serializer = new DataContractSerializer(typeof(SettingsFile));
                            using (var reader = new XmlTextReader(ofd.FileName))
                            {
                                settingsFile = (SettingsFile)serializer.ReadObject(reader);
                            }

                            if (settingsFile != null)
                            {
                                Session.WorldConfig1 = settingsFile.WorldConfig;
                                foreach (Property prop in Session.WorldConfig1.Properties.Where(c => c.Override && !Session.VersionConfig.WorldConfig.Any(d => (string)d.Name == (string)c.PropertyName)))
                                {
                                    sErrorMessage += "Could not load value \"" + prop.Value + "\" for setting \"" + prop.PropertyName + "\" in WorldConfig.\r\n";
                                }
                                Session.WorldConfig1.Properties.RemoveAll(a => !Session.VersionConfig.WorldConfig.Any(b => (string)b.Name == (string)a.PropertyName));

                                Session.IgnorePropertyInputChangedWorld = true;
                                Session.IgnoreOverrideCheckChangedWorld = true;

                                foreach (TCProperty property in Session.VersionConfig.WorldConfig)
                                {
                                    switch (property.PropertyType)
                                    {
                                        case "BiomesList":
                                            ((ListBox)Session.WorldSettingsInputs[property].Item1).SelectedIndices.Clear();
                                            ((ListBox)Session.WorldSettingsInputs[property].Item1).SelectedIndex = -1;
                                            ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                            ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                            break;
                                        case "Bool":
                                            ((Button)Session.WorldSettingsInputs[property].Item1).Text = "";
                                            ((Button)Session.WorldSettingsInputs[property].Item1).ForeColor = Color.Empty;
                                            break;
                                        case "Color":
                                            Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                            Session.WorldSettingsInputs[property].Item1.Text = "";
                                            break;
                                        case "Float":
                                        case "Int":
                                            Session.WorldSettingsInputs[property].Item1.Text = "";
                                            break;
                                        case "String":
                                        case "BigString":
                                            Session.WorldSettingsInputs[property].Item1.Text = "";
                                            break;
                                        case "ResourceQueue":
                                            ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Clear();
                                            ((ListBox)Session.WorldSettingsInputs[property].Item1).SelectedIndex = -1;
                                            ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                            ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                            break;
                                    }
                                    Session.WorldSettingsInputs[property].Item2.Checked = false;
                           
                                    if (Session.WorldConfig1.Properties.FirstOrDefault(a => a.PropertyName == property.Name) == null)
                                    {
                                        Session.WorldConfig1.Properties.Add(new Property(null, false, property.Name, false, false));
                                    }

                                    string s = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                    if (Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override)
                                    {
                                        if (property.PropertyType != "ResourceQueue" || Session.WorldConfig1.GetPropertyValueAsString(property) != null)
                                        {
                                            s = Session.WorldConfig1.GetPropertyValueAsString(property);
                                        }
                                    }

                                    if (s != null || Session.WorldConfig1.GetPropertyMerge(property))
                                    {
                                        Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = Session.WorldSettingsInputs[property];
                                        string propertyValue = s;
                                        switch (property.PropertyType)
                                        {
                                            case "BiomesList":
                                                ((ListBox)Session.WorldSettingsInputs[property].Item1).SelectedItems.Clear();
                                                string[] biomeNames = propertyValue.Split(',');
                                                string newpropertyValue = "";
                                                for (int k = 0; k < biomeNames.Length; k++)
                                                {
                                                    if (Session.BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                                    {
                                                        for (int l = 0; l < ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Count; l++)
                                                        {
                                                            if (((string)((ListBox)Session.WorldSettingsInputs[property].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                                            {
                                                                ((ListBox)Session.WorldSettingsInputs[property].Item1).SelectedItems.Add(((ListBox)Session.WorldSettingsInputs[property].Item1).Items[l]);
                                                            }
                                                        }
                                                        newpropertyValue += k == biomeNames.Length - 1 ? (string)biomeNames[k].Trim() : (string)biomeNames[k].Trim() + ", ";
                                                    }
                                                    else if (!String.IsNullOrEmpty(biomeNames[k].Trim()))
                                                    {
                                                        sErrorMessage2 += "Could not select biome \"" + biomeNames[k].Trim() + "\" for setting \"" + property.Name + "\" in world config.\r\n";
                                                    }
                                                }
                                                if(!String.IsNullOrEmpty(newpropertyValue))
                                                {
                                                    Session.WorldConfig1.Properties.First(a => (string)a.PropertyName == (string)property.Name).Value = newpropertyValue;
                                                }
                                                ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = !Session.WorldConfig1.GetPropertyMerge(property);
                                                ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = Session.WorldConfig1.GetPropertyMerge(property);
                                                boxes.Item2.Checked = Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                            case "Bool":
                                                if (propertyValue.ToLower() == "true")
                                                {
                                                    ((Button)boxes.Item1).Text = "true";
                                                    ((Button)boxes.Item1).ForeColor = Color.Green;
                                                }
                                                else if (propertyValue.ToLower() == "false")
                                                {
                                                    ((Button)boxes.Item1).Text = "false";
                                                    ((Button)boxes.Item1).ForeColor = Color.Red;
                                                } else {
                                                    ((Button)boxes.Item1).Text = "";
                                                    ((Button)boxes.Item1).ForeColor = Color.Empty;
                                                }
                                                boxes.Item2.Checked = Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                            case "Color":
                                                if ((propertyValue.StartsWith("0x") && propertyValue.Length == 8) || (propertyValue.StartsWith("#") && propertyValue.Length == 7))
                                                {
                                                    boxes.Item2.Checked = Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                    bool bException = false;
                                                    try
                                                    {
                                                        boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                                        string value = "";
                                                        if (Session.SettingsType.ColorType == "0x")
                                                        {
                                                            value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                        }
                                                        else if (Session.SettingsType.ColorType == "#")
                                                        {
                                                            value = "#" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                        }
                                                        boxes.Item1.Text = value;
                                                        if (Session.WorldConfigDefaultValues == null || boxes.Item2.Checked || ColorTranslator.FromHtml(value.ToUpper()).ToArgb() != ColorTranslator.FromHtml(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property).ToUpper()).ToArgb())
                                                        {
                                                            boxes.Item2.Checked = true;
                                                        } else {
                                                            boxes.Item2.Checked = false;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        bException = true;
                                                    }
                                                    if (bException)
                                                    {
                                                        string propertyString = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                                        if ((propertyString.StartsWith("0x") && propertyString.Length == 8) || (propertyString.StartsWith("#") && propertyString.Length == 7))
                                                        {
                                                            try
                                                            {
                                                                boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyString);
                                                                string value = "";
                                                                if (Session.SettingsType.ColorType == "0x")
                                                                {
                                                                    value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                                }
                                                                else if (Session.SettingsType.ColorType == "#")
                                                                {
                                                                    value = "#" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                                }
                                                                boxes.Item1.Text = value;
                                                            }
                                                            catch (Exception ex2)
                                                            {
                                                                boxes.Item5.BackColor = Color.White;
                                                                boxes.Item1.Text = "";
                                                            }
                                                            boxes.Item2.Checked = false;
                                                        } else {
                                                            boxes.Item5.BackColor = Color.White;
                                                            boxes.Item1.Text = "";
                                                            boxes.Item2.Checked = false;
                                                        }
                                                    }
                                                } else {
                                                    boxes.Item5.BackColor = Color.White;
                                                    boxes.Item1.Text = "";
                                                    boxes.Item2.Checked = false;
                                                }
                                                break;
                                            case "Float":
                                            case "Int":
                                                string newText = propertyValue;
                                                int numberOfDecimals = property.PropertyType == "Int" ? 0 : newText.IndexOf(".") > 0 ? newText.Length - (newText.IndexOf(".") + 1) : 0;
                                                ((NumericUpDownExt)boxes.Item1).DecimalPlaces = numberOfDecimals;
                                                boxes.Item1.Text = newText;
                                                boxes.Item2.Checked = Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                            case "String":
                                            case "BigString":
                                                boxes.Item1.Text = propertyValue;
                                                boxes.Item2.Checked = Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                            case "ResourceQueue":
                                                string[] resourceQueueItemNames = { };
                                                if (propertyValue != null)
                                                {
                                                    resourceQueueItemNames = propertyValue.Replace("\r", "").Split('\n');
                                                }

                                                foreach (string resourceQueueItemName in resourceQueueItemNames)
                                                {
                                                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                                                    {
                                                        ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                                                    }
                                                }
                                                ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = !Session.WorldConfig1.GetPropertyMerge(property);
                                                ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = Session.WorldConfig1.GetPropertyMerge(property);
                                                boxes.Item2.Checked = Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                        }
                                    }
                                }

                                Session.IgnoreOverrideCheckChangedWorld = false;
                                Session.IgnorePropertyInputChangedWorld = false;

                                Session.BiomeGroups.Clear();
                                lbBiomes.Items.Clear();
                                lbGroup.Items.Clear();
                                lbGroups.Items.Clear();

                                tlpBiomeSettings1.Visible = false;
                                label3.Visible = false;

                                List<Group> newBiomeGroups = new List<Group>();

                                foreach (Group biomeGroup in settingsFile.BiomeGroups)
                                {
                                    foreach(Property p in biomeGroup.BiomeConfig.Properties)
                                    {
                                        if(!p.Override)
                                        {
                                            p.Value = null;
                                        }
                                    }

                                    if (biomeGroup.Biomes.Count > 1)
                                    {
                                        int count = 0;
                                        string sErrorMessage3 = "";
                                        List<string> newBiomeGroupBiomes = new List<string>();
                                        foreach(string biomeName in biomeGroup.Biomes)
                                        {
                                            if (Session.BiomeNames.Any(a => a.Equals(biomeName)))
                                            {
                                                if (!newBiomeGroupBiomes.Any(a => a.Equals(biomeName)))
                                                {
                                                    newBiomeGroupBiomes.Add(biomeName);
                                                    count += 1;
                                                }
                                            } else {
                                                sErrorMessage3 += "Could not load biome \"" + biomeName + "\" in biome group \"" + biomeGroup.Name + "\".\r\n";
                                            }
                                        }
                                        biomeGroup.Biomes = newBiomeGroupBiomes;
                                        if(count > 0)
                                        {
                                            newBiomeGroups.Add(biomeGroup);
                                            lbGroups.Items.Add(biomeGroup.Name);
                                            Session.BiomeGroups.Add(biomeGroup);
                                        }
                                        if(!String.IsNullOrEmpty(sErrorMessage3))
                                        {
                                            sErrorMessage2 += sErrorMessage3;// +"These biomes do not exist.\r\n";
                                        }
                                    } else {
                                        if (Session.BiomeNames.Any(a => a.Equals(biomeGroup.Biomes[0])))
                                        {
                                            lbGroups.Items.Add(biomeGroup.Name);
                                            Session.BiomeGroups.Add(biomeGroup);
                                        } else {
                                            sErrorMessage2 += "Could not load biome group \"" + biomeGroup.Name + "\" because biome \"" + biomeGroup.Biomes[0] + "\" does not exist.\r\n";
                                        }
                                    }
                                }
                                settingsFile.BiomeGroups = newBiomeGroups;

                                if(lbGroups.Items.Count > 0)
                                {
                                    lbGroups.SelectedIndex = 0;
                                    foreach (Group biomeGroup in Session.BiomeGroups)
                                    {
                                        foreach(TCProperty tcProp in Session.VersionConfig.BiomeConfig)
                                        {
                                            if (!biomeGroup.BiomeConfig.Properties.Any(a => (string)a.PropertyName == (string)tcProp.Name))
                                            {
                                                biomeGroup.BiomeConfig.Properties.Add(new Property(null, false, tcProp.Name, false, false));
                                            }
                                        }
                                        foreach (Property prop in biomeGroup.BiomeConfig.Properties.Where(c => c.Override && !Session.VersionConfig.BiomeConfig.Any(d => (string)d.Name == (string)c.PropertyName)))
                                        {
                                            sErrorMessage += "Could not load value \"" + prop.Value + "\" for setting \"" + prop.PropertyName + "\" in biome group \"" + biomeGroup.Name + "\".\r\n";
                                        }
                                        foreach (Property prop in biomeGroup.BiomeConfig.Properties.Where(c => c.Override && Session.VersionConfig.BiomeConfig.Any(d => (string)d.Name == (string)c.PropertyName && d.PropertyType == "BiomesList")))
                                        {
                                            string[] biomeNames = prop.Value.Split(',');
                                            string newBiomeNames = "";
                                            for (int k = 0; k < biomeNames.Length; k++)
                                            {
                                                if (!Session.BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                                {
                                                    if(!String.IsNullOrEmpty(biomeNames[k].Trim()))
                                                    {
                                                        sErrorMessage2 += "Could not select biome \"" + biomeNames[k].Trim() + "\" for setting \"" + prop.PropertyName + "\" in biome group \"" + biomeGroup.Name + "\".\r\n";
                                                    }
                                                } else {
                                                    if (newBiomeNames != (string)biomeNames[k].Trim() + "," && !newBiomeNames.Contains("," + (string)biomeNames[k].Trim() + ","))
                                                    {
                                                        newBiomeNames += (string)biomeNames[k].Trim() + ",";
                                                    }
                                                }
                                            }
                                            prop.Value = newBiomeNames.Length == 0 ? newBiomeNames : newBiomeNames.Substring(0, newBiomeNames.Length - 1);
                                        }
                                        biomeGroup.BiomeConfig.Properties.RemoveAll(a => !Session.VersionConfig.BiomeConfig.Any(b => (string)b.Name == (string)a.PropertyName));
                                    }
                                }
                            }
                        }
                    }
                } else {
                    PopUpForm.CustomMessageBox("No source world found. Load a source world before loading a settings file.", "Version warning");
                }
                string sErrorMessageFinal = "";
                if(!String.IsNullOrEmpty(sErrorMessage))
                {
                    sErrorMessageFinal = sErrorMessage + "\r\nThese settings do not exist in the selected version of OTG/TerrainControl.";
                }
                if (!String.IsNullOrEmpty(sErrorMessage2))
                {
                    sErrorMessageFinal = sErrorMessageFinal + "\r\n\r\n" + sErrorMessage2 + "\r\nThese biomes do not exist in the selected version of OTG/TerrainControl.";
                }
                if(!String.IsNullOrEmpty(sErrorMessageFinal))
                {
                    PopUpForm.CustomMessageBox(sErrorMessageFinal, "Version warnings");
                }

                FocusOnTab();
            }        

            void btGenerate_Click(object sender, EventArgs e)
            {
                fbdDestinationWorldDir.Description = "Select a OTG/TerrainControl world folder (create if needed).";

                if (fbdDestinationWorldDir.ShowDialog() == DialogResult.OK)
                {
                    bool go = true;
                    String ares = fbdDestinationWorldDir.SelectedPath.Substring(0, fbdDestinationWorldDir.SelectedPath.LastIndexOf("\\"));
                    if (!fbdDestinationWorldDir.SelectedPath.Substring(0, fbdDestinationWorldDir.SelectedPath.LastIndexOf("\\")).EndsWith("TerrainControl\\worlds") && !fbdDestinationWorldDir.SelectedPath.Substring(0, fbdDestinationWorldDir.SelectedPath.LastIndexOf("\\")).EndsWith("OpenTerrainGenerator\\worlds"))
                    {
                        go = PopUpForm.CustomYesNoBox("Generate files here?", "The selected directory was not a OTG/TerrainControl world directory, are you sure you want to generate files here?", "Yes", "No") == DialogResult.OK;
                    }
                    if (go)
                    {
                        Session.ShowProgessBox();

                        DateTime startTime = DateTime.Now;

                        Session.DestinationConfigsDir = fbdDestinationWorldDir.SelectedPath;
                        WorldSaveDir = Session.DestinationConfigsDir.Replace("mods\\TerrainControl\\worlds\\", "saves\\").Replace("mods\\OpenTerrainGenerator\\worlds\\", "saves\\");

                        World.ConfigWorld(Session.WorldConfig1, Session.WorldConfigDefaultValues, Session.VersionConfig, Session.SourceConfigsDir, Session.DestinationConfigsDir, false);

                        DirectoryInfo destBiomesDir = new System.IO.DirectoryInfo(Session.DestinationConfigsDir + "\\WorldBiomes");
                        DirectoryInfo sourceDirectory = new System.IO.DirectoryInfo(Session.SourceConfigsDir + "\\WorldBiomes");
                        if (destBiomesDir.Exists)
                        {
                            // TODO: Is all this really necessary? <- Without this you can't overwrite an existing directory?

                            List<string> filesToOverride = new List<string>();

                            foreach (System.IO.FileInfo file in sourceDirectory.GetFiles())
                            {
                                filesToOverride.Add(Session.DestinationConfigsDir + "\\WorldBiomes\\" + file.Name);
                            }

                            System.Security.AccessControl.DirectorySecurity sec3 = destBiomesDir.GetAccessControl();
                            System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                            sec3.AddAccessRule(accRule3);

                            destBiomesDir.Delete(true);
                            destBiomesDir.Refresh();

                            destBiomesDir = new System.IO.DirectoryInfo(Session.DestinationConfigsDir + "\\WorldBiomes");

                            if(!destBiomesDir.Exists)
                            {
                                destBiomesDir.Create();
                                destBiomesDir.Refresh();
                            }
                        }

                        Biomes.GenerateBiomeConfigs(sourceDirectory, destBiomesDir, BiomeConfigsDefaultValues, Session.VersionConfig, false);

                        bool bDone = true;
                        if (cbDeleteRegion.Checked)
                        {
                            System.IO.DirectoryInfo WorldDirectory = new System.IO.DirectoryInfo(WorldSaveDir + "\\region");
                            if (WorldDirectory.Exists)
                            {
                                bDone = false;
                                try
                                {
                                    WorldDirectory.Delete(true);
                                    bDone = true;
                                }
                                catch (System.IO.IOException ex)
                                {
                                    PopUpForm.CustomMessageBox("Could not delete /regions directory because it is currently in use. Please make sure that no one is playing the selected world and try again.", "Generation error");
                                }
                            }

                            System.IO.DirectoryInfo worldDirectory2 = new System.IO.DirectoryInfo(WorldSaveDir + "\\data");
                            if (worldDirectory2.Exists)
                            {
                                bDone = false;
                                try
                                {
                                    worldDirectory2.Delete(true);
                                    bDone = true;
                                }
                                catch (System.IO.IOException ex)
                                {
                                    PopUpForm.CustomMessageBox("Could not delete /data directory because it is currently in use. Please make sure that no one is playing the selected world and try again.", "Generation error");
                                }
                            }

                            System.IO.DirectoryInfo StructureDataDirectory = new System.IO.DirectoryInfo(Session.DestinationConfigsDir + "\\StructureData");
                            if (StructureDataDirectory.Exists)
                            {
                                bDone = false;
                                try
                                {
                                    StructureDataDirectory.Delete(true);
                                    StructureDataDirectory.Refresh();
                                    bDone = true;
                                }
                                catch (System.IO.IOException ex)
                                {
                                    PopUpForm.CustomMessageBox("Could not delete /StructureData directory because it is currently in use. Please make sure that no one is playing the selected world and try again.", "Generation error");
                                }
                            }

                            System.IO.FileInfo structureDataFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "\\StructureData.txt");
                            if (structureDataFile.Exists)
                            {
                                structureDataFile.Delete();
                            }
                            System.IO.FileInfo nullChunksFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "\\NullChunks.txt");
                            if (nullChunksFile.Exists)
                            {
                                nullChunksFile.Delete();
                            }
                            System.IO.FileInfo spawnedStructuresFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "\\SpawnedStructures.txt");
                            if (spawnedStructuresFile.Exists)
                            {
                                spawnedStructuresFile.Delete();
                            }

                            System.IO.FileInfo chunkProviderPopulatedChunksFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "\\ChunkProviderPopulatedChunks.txt");
                            if (chunkProviderPopulatedChunksFile.Exists)
                            {
                                chunkProviderPopulatedChunksFile.Delete();
                            }

                            System.IO.FileInfo pregeneratedChunksFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "\\PregeneratedChunks.txt");
                            if (pregeneratedChunksFile.Exists)
                            {
                                pregeneratedChunksFile.Delete();
                            }
                        }

                        Session.HideProgessBox();

                        if (bDone)
                        {
                            //PopUpForm.CustomMessageBox("Done in " + (DateTime.Now - startTime).TotalSeconds + " seconds.", "Generating");
                            PopUpForm.CustomMessageBox("Done.");
                        }
                    }
                }

                FocusOnTab();
            }

        #endregion

        # region Manage worlds

            private void btmanageWorlds_Click(object sender, EventArgs e)
            {
                PopUpForm.ManageWorldsBox();

                cbWorld.Items.Clear();
                DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\");
                if (versionDir3.Exists)
                {
                    foreach (DirectoryInfo dir2 in versionDir3.GetDirectories())
                    {
                        cbWorld.Items.Add(dir2.Name);
                    }
                    if (cbWorld.Items.Count > 0)
                    {
                        cbWorld.SelectedIndex = 0;
                    }
                }

                if(!System.IO.Directory.Exists(Session.SourceConfigsDir))
                {
                    UnloadUI();
                }

                Session.Form1.FocusOnTab();
            }

        # endregion

        private void btnConvertSchematicToBO3_Click(object sender, EventArgs e)
        {
            if (convertBO3ofd.ShowDialog() == DialogResult.OK)
            {
                if (convertBO3ofd.FileNames.Length > 0 && convertBO3fbd.ShowDialog() == DialogResult.OK)
                {
                    DialogResult useBranchesResult = PopUpForm.CustomYesNoBox("CustomObject or CustomStructure?", "Export schematic as:\r\n\r\n- CustomObject: A single BO3 containing all blocks.\r\n- CustomStructure: Slice the schematic into 16x16 BO3's connected via branches.\r\n\r\nFor small schematics (<= 32x32) such as trees and rocks use CustomObject.\r\nFor large schematics (> 32x32) such as structures use CustomStructure.", "CustomObject", "CustomStructure");
                    if (useBranchesResult != System.Windows.Forms.DialogResult.Cancel)
                    {
                        bool useBranches = useBranchesResult == DialogResult.No;

                        DialogResult exportForTCResult = !useBranches ? DialogResult.Yes : PopUpForm.CustomYesNoBox("OTG/TerrainControl or MCW/OTG+?", "Export BO3's for TC/OTG or MCW/OTG+?", "OTG/TerrainControl", "MCW/OTG+");
                        if (exportForTCResult != System.Windows.Forms.DialogResult.Cancel)
                        {
                            bool exportForTC = useBranchesResult == DialogResult.Yes;

                            DialogResult removeAirResult = PopUpForm.CustomYesNoBox("Remove air blocks?", "", "Remove air", "Keep air");
                            if(removeAirResult != System.Windows.Forms.DialogResult.Cancel)
                            {
                                bool removeAir = removeAirResult == DialogResult.Yes;

                                string centerBlockIdString = null;
                                if (!useBranches)
                                {
                                    DialogResult useCenterBlockResult = PopUpForm.InputBox("Use a center block?", "Center the BO3 around a specific block? If so, enter the block id (number).", ref centerBlockIdString, false, true, true);
                                    if (useCenterBlockResult == System.Windows.Forms.DialogResult.Cancel)
                                    {
                                        return;
                                    }
                                }

                                int centerBlockId = -1;
                                if (!String.IsNullOrEmpty(centerBlockIdString))
                                {
                                    int.TryParse(centerBlockIdString, out centerBlockId);
                                }

                                Session.ShowProgessBox();

                                int converted = 0;
                                foreach (String fileName in convertBO3ofd.FileNames)
                                {
                                    convertBO3ofd.InitialDirectory = fileName.Substring(0, fileName.LastIndexOf("\\"));

                                    if (fileName.ToLower().EndsWith(".schematic"))
                                    {
                                        converted += 1;
                                        Utils.SchematicToBO3.doSchematicToBO3(new FileInfo(fileName), new DirectoryInfo(convertBO3fbd.SelectedPath), exportForTC, useBranches, centerBlockId, removeAir);
                                    }
                                }

                                Session.HideProgessBox();

                                if (converted > 1)
                                {
                                    PopUpForm.CustomMessageBox(converted + " schematics were converted to BO3's and saved at " + convertBO3fbd.SelectedPath, "Converting schematics to BO3's");
                                } else {
                                    PopUpForm.CustomMessageBox(converted + " schematic was converted to BO3's and saved at " + convertBO3fbd.SelectedPath, "Converting schematic to BO3's");
                                }
                            }
                        }
                    }
                }
            }

            Session.Form1.FocusOnTab();
        }

        private void Link_Clicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void btCopyBO3s_Click(object sender, EventArgs e)
        {
            if (copyBO3fbd.ShowDialog() == DialogResult.OK)
            {
                bool go = true;
                if (!copyBO3fbd.SelectedPath.EndsWith("GlobalObjects") && !copyBO3fbd.SelectedPath.EndsWith("WorldObjects"))
                {
                    go = PopUpForm.CustomYesNoBox("Copy BO3's here?", "The selected directory was not a GlobalObjects or WorldObjects directory, are you sure you want to copy here?", "Yes", "No") == DialogResult.OK;
                }
                if (go)
                {
                    PopUpForm.CustomMessageBox("Copying BO3's to output directory, this can take a while depending on the number and size of the BO3's.", "Copying BO3's");
                    System.IO.DirectoryInfo SourceWorldDirectory = new System.IO.DirectoryInfo(Session.SourceConfigsDir + "\\WorldObjects");
                    System.IO.DirectoryInfo DestinationWorldDirectory = new System.IO.DirectoryInfo(copyBO3fbd.SelectedPath);
                    if (SourceWorldDirectory.Exists)
                    {
                        if(!DestinationWorldDirectory.Exists)
                        {
                            DestinationWorldDirectory.Create();
                            DestinationWorldDirectory.Refresh();
                        }

                        System.Security.AccessControl.DirectorySecurity sec = DestinationWorldDirectory.GetAccessControl();
                        System.Security.AccessControl.FileSystemAccessRule accRule = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                        sec.AddAccessRule(accRule);

                        Utils.CopyDir.CopyAll(SourceWorldDirectory, DestinationWorldDirectory);
                    }
                    PopUpForm.CustomMessageBox("All BO3's were copied to " + copyBO3fbd.SelectedPath, "Copying BO3's");
                }
            }

            Session.Form1.FocusOnTab();
        }

        private void btClickBackGround(object sender, EventArgs e)
        {
            FocusOnTab();
        }

        bool isMaximised = false;
        private void Form1_Resize(object sender, EventArgs e)
        {
            if(isMaximised)
            {
                isMaximised = false;
                this.Width += 1;
            } else {
                isMaximised = WindowState == FormWindowState.Maximized;
            }
        }

        void Form1_ResizeEnd(object sender, EventArgs e)
        {
            isMaximised = false;
            this.Width += 1;
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            // For adding BO3/schematic files to the resource queue by drag/drop

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            // If the biomes tab has been selected
            if (tabControl1.SelectedIndex == 1 && lbGroups.SelectedIndex > -1) 
            {
                bool bFound = false;

                foreach (string file in files)
                {
                    if (file.ToLower().Contains(".bo3") || file.ToLower().Contains(".schematic"))
                    {
                        bFound = true;
                    }
                }

                if(bFound)
                {
                    string propertyValue = "100";
                    if (PopUpForm.InputBox("Add BO3", "Rarity", ref propertyValue, true) == DialogResult.OK)
                    {
                        if (propertyValue != null && !String.IsNullOrEmpty(propertyValue.Trim()))
                        {
                            double rarity = -1;
                            bool isDouble = double.TryParse(propertyValue, out rarity);

                            if (isDouble && rarity >= 0 && rarity <= 100)
                            {
                                TCProperty property = Session.BiomeSettingsInputs.First(a => a.Key.Name == "ResourceQueue").Key;
                                ListBox box = (ListBox)Session.BiomeSettingsInputs.First(a => a.Key.Name == "ResourceQueue").Value.Item1;
                                foreach (string file in files)
                                {
                                    string BO3Name = file.Substring(file.LastIndexOf("\\") + 1).Replace(".BO3", "").Replace(".bo3", "").Replace(".schematic", "").Replace("(", "").Replace(")", "");
                                    bFound = false;
                                    foreach (string item in box.Items)
                                    {
                                        if (item.Trim().ToLower().StartsWith("customstructure(" + BO3Name.Trim().ToLower()))
                                        {
                                            bFound = true;
                                        }
                                    }
                                    if (!bFound)
                                    {
                                        AddToResourceQueue(property, "CustomStructure(" + BO3Name + "," + rarity + ")");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        void lbPropertyInput_KeyDown_World(object sender, KeyEventArgs e)
        {
            TCProperty property = Session.WorldSettingsInputs.FirstOrDefault(a => a.Value.Item1 == sender).Key;

            if (e.Control && e.KeyCode == Keys.A)
            {
                e.SuppressKeyPress = true;

                ListBox lb = (ListBox)sender;
                lb.SuspendLayout();
                for (int i = 0; i < lb.Items.Count; i++)
                {
                    lb.SetSelected(i, true);
                }
                lb.ResumeLayout();
            }
            else if (property.PropertyType == "BiomesList")
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    e.SuppressKeyPress = true;

                    ListBox lb = (ListBox)sender;
                    String clipBoardString = "";
                    foreach (String selectedItem in lb.SelectedItems)
                    {
                        clipBoardString += selectedItem + ", ";
                    }
                    clipBoardString = clipBoardString.Substring(0, clipBoardString.Length - 2); // Remove trailing ","
                    Clipboard.SetText(clipBoardString);
                }
                else if (e.Control && e.KeyCode == Keys.V)
                {
                    e.SuppressKeyPress = true;

                    ListBox lb = (ListBox)sender;
                    String[] clipBoardStrings = Clipboard.GetText().Replace(")", "").Split(',');

                    lb.SuspendLayout();
                    lb.ClearSelected();
                    for (int i = 0; i < lb.Items.Count; i++)
                    {
                        if (clipBoardStrings.Any(a => a.Trim().Equals(((string)lb.Items[i]).Trim())))
                        {
                            lb.SetSelected(i, true);
                        }
                    }
                    lb.ResumeLayout();
                }
            }
            else if (property.PropertyType == "ResourceQueue")
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    e.SuppressKeyPress = true;

                    ListBox lb = (ListBox)sender;
                    String clipBoardString = "";
                    foreach (String selectedItem in lb.SelectedItems)
                    {
                        clipBoardString += selectedItem + "\r\n";
                    }
                    clipBoardString = clipBoardString.Substring(0, clipBoardString.Length - 1); // Remove trailing "/"
                    Clipboard.SetText(clipBoardString);
                }
                else if (e.Control && e.KeyCode == Keys.V)
                {
                    e.SuppressKeyPress = true;

                    ListBox lb = (ListBox)sender;
                    String[] clipBoardStrings = Clipboard.GetText().Replace("\r\n", "/").Split('/');
                    addingMultipleResourcesXToAll = DialogResult.Abort;
                    addingMultipleResourcesXToAll2 = DialogResult.Abort;
                    addingMultipleResourcesXToAll3 = DialogResult.Abort;
                    resourcesToAdd = new List<string>();
                    copyPasting = true;
                    foreach (String selectedItem in clipBoardStrings)
                    {
                        AddToResourceQueueWorld(property, selectedItem, false);
                    }
                    foreach (string itemToAdd in resourcesToAdd)
                    {
                        AddResourceToWorld(property, itemToAdd);
                    }
                    copyPasting = false;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    ListBox lb = ((ListBox)Session.WorldSettingsInputs[property].Item1);
                    if (lb.SelectedItem != null)
                    {
                        List<string> itemsToRemove = new List<string>();
                        foreach (string selectedItem in lb.SelectedItems)
                        {
                            itemsToRemove.Add(selectedItem);
                        }
                        foreach (string selectedItem in itemsToRemove)
                        {
                            DeleteResourceQueueItemWorld(property, selectedItem);
                        }
                    }
                }
            }
        }

        void lbPropertyInput_KeyDown(object sender, KeyEventArgs e)
        {
            TCProperty property = Session.BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item1 == sender).Key;

            if (e.Control && e.KeyCode == Keys.A)
            {
                e.SuppressKeyPress = true;

                ListBox lb = (ListBox)sender;
                lb.SuspendLayout();
                for (int i = 0; i < lb.Items.Count; i++)
                {
                    lb.SetSelected(i, true);
                }
                lb.ResumeLayout();
            }
            else if (property.PropertyType == "BiomesList")
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    e.SuppressKeyPress = true;

                    ListBox lb = (ListBox)sender;
                    String clipBoardString = "";
                    foreach (String selectedItem in lb.SelectedItems)
                    {
                        clipBoardString += selectedItem + ", ";
                    }
                    clipBoardString = clipBoardString.Substring(0, clipBoardString.Length - 2); // Remove trailing ","
                    Clipboard.SetText(clipBoardString);
                }
                if (e.Control && e.KeyCode == Keys.V)
                {
                    e.SuppressKeyPress = true;

                    ListBox lb = (ListBox)sender;
                    String[] clipBoardStrings = Clipboard.GetText().Replace(")", "").Split(',');

                    lb.SuspendLayout();
                    lb.ClearSelected();
                    for (int i = 0; i < lb.Items.Count; i++)
                    {
                        if (clipBoardStrings.Any(a => a.Trim().Equals(((string)lb.Items[i]).Trim())))
                        {
                            lb.SetSelected(i, true);
                        }
                    }
                    lb.ResumeLayout();
                }
            }
            else if (property.PropertyType == "ResourceQueue")
            {
                if (e.Control && e.KeyCode == Keys.C)
                {
                    e.SuppressKeyPress = true;

                    ListBox lb = (ListBox)sender;
                    String clipBoardString = "";
                    foreach (String selectedItem in lb.SelectedItems)
                    {
                        clipBoardString += selectedItem + "\r\n";
                    }
                    clipBoardString = clipBoardString.Substring(0, clipBoardString.Length - 1); // Remove trailing "/"
                    Clipboard.SetText(clipBoardString);
                }
                else if (e.Control && e.KeyCode == Keys.V)
                {
                    e.SuppressKeyPress = true;

                    ListBox lb = (ListBox)sender;
                    String[] clipBoardStrings = Clipboard.GetText().Replace("\r\n", "/").Split('/');
                    addingMultipleResourcesXToAll = DialogResult.Abort;
                    addingMultipleResourcesXToAll2 = DialogResult.Abort;
                    addingMultipleResourcesXToAll3 = DialogResult.Abort;
                    resourcesToAdd = new List<string>();
                    copyPasting = true;
                    foreach (String selectedItem in clipBoardStrings)
                    {
                        AddToResourceQueue(property, selectedItem, false);
                    }
                    foreach (string itemToAdd in resourcesToAdd)
                    {
                        AddResourceToBiome(property, itemToAdd);
                    }
                    copyPasting = false;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    ListBox lb = ((ListBox)Session.BiomeSettingsInputs[property].Item1);
                    if (lb.SelectedItem != null)
                    {
                        List<string> itemsToRemove = new List<string>();
                        foreach (string selectedItem in lb.SelectedItems)
                        {
                            itemsToRemove.Add(selectedItem);
                        }
                        foreach (string selectedItem in itemsToRemove)
                        {
                            DeleteResourceQueueItem(property, selectedItem);
                        }
                    }
                }
            }
        }

        private void rbSummerSkin_CheckedChanged(object sender, EventArgs e)
        {
            this.BackgroundImage = global::OTGE.Properties.Resources.BGSummer;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;

            this.tabPage1.BackColor = System.Drawing.Color.Wheat;
            this.tabPage2.BackColor = System.Drawing.Color.Wheat;
            this.tabPage3.BackColor = System.Drawing.Color.Wheat;

            this.groupBox1.BackColor = System.Drawing.Color.Wheat;
            this.pnlVersionWorldSelect.BackColor = System.Drawing.Color.Wheat;
            this.groupBox3.BackColor = System.Drawing.Color.Wheat;
            this.groupBox4.BackColor = System.Drawing.Color.Wheat;

            this.richTextBox1.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox2.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox3.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox4.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox5.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox6.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox7.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox8.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox9.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox10.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox11.BackColor = System.Drawing.Color.Wheat;

            this.textBox10.BackColor = System.Drawing.Color.Wheat;

            FocusOnTab();
        }

        private void rbWinterSkin_CheckedChanged(object sender, EventArgs e)
        {
            this.BackgroundImage = global::OTGE.Properties.Resources.BGWinter;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            this.tabPage1.BackColor = System.Drawing.Color.GhostWhite;
            this.tabPage2.BackColor = System.Drawing.Color.GhostWhite;
            this.tabPage3.BackColor = System.Drawing.Color.GhostWhite;

            this.groupBox1.BackColor = System.Drawing.Color.GhostWhite;
            this.pnlVersionWorldSelect.BackColor = System.Drawing.Color.GhostWhite;
            this.groupBox3.BackColor = System.Drawing.Color.GhostWhite;
            this.groupBox4.BackColor = System.Drawing.Color.GhostWhite;

            this.richTextBox1.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox2.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox3.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox4.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox5.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox6.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox7.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox8.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox9.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox10.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox11.BackColor = System.Drawing.Color.GhostWhite;

            this.textBox7.BackColor = System.Drawing.Color.GhostWhite;
            this.textBox9.BackColor = System.Drawing.Color.GhostWhite;
            this.textBox10.BackColor = System.Drawing.Color.GhostWhite;

            FocusOnTab();
        }

        private void rbVanillaSkin_CheckedChanged(object sender, EventArgs e)
        {
            this.BackgroundImage = null;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BackColor = Color.FromKnownColor(KnownColor.Control);

            this.tabPage1.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.tabPage2.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.tabPage3.BackColor = Color.FromKnownColor(KnownColor.Control);

            this.groupBox1.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.pnlVersionWorldSelect.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.groupBox3.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.groupBox4.BackColor = Color.FromKnownColor(KnownColor.Control);

            this.richTextBox1.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox2.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox3.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox4.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox5.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox6.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox7.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox8.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox9.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox10.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.richTextBox11.BackColor = Color.FromKnownColor(KnownColor.Control);

            this.textBox7.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.textBox9.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.textBox10.BackColor = Color.FromKnownColor(KnownColor.Control);

            FocusOnTab();
        }
        
        private void cbDeleteRegion_Click(object sender, EventArgs e)
        {
            FocusOnTab();
        }

        object lastSender = null;
        void lbPropertyInput_MouseHover(object sender, EventArgs e)
        {
            int scrollBarWidth = 20;
            if (sender is ListBox)
            {
                ListBox listBox = (ListBox)sender;
                string hoveredItem = DetermineHoveredItem((ListBox)sender);
                if (hoveredItem != null)
                {
                    string textWithLineBreaks = Utils.Misc.FormatToolTipText(hoveredItem);

                    if (!Session.ToolTip2.GetToolTip((ListBox)sender).Equals(textWithLineBreaks))
                    {
                        Size stringSize = TextRenderer.MeasureText(hoveredItem, listBox.Font);
                        if (listBox.Width - (listBox.PreferredHeight > listBox.Height ? scrollBarWidth : 0) >= stringSize.Width)
                        {
                            Session.ToolTip2.RemoveAll();
                        } else {
                            Session.ToolTip2.RemoveAll();
                            Session.ToolTip2.SetToolTip((ListBox)sender, textWithLineBreaks);
                        }
                    }
                }
                MouseInput.ResetMouseHover(((ListBox)sender).Handle);
            }
            if (sender is TextBox || sender is RichTextBox)
            {
                if(
                    (
                        sender is TextBox || 
                        sender is RichTextBox
                    ) &&
                    sender != this.ActiveControl
                )
                {
                    Control textBox = (Control)sender;

                    string textWithLineBreaks = Utils.Misc.FormatToolTipText(textBox.Text);

                    if (!Session.ToolTip2.GetToolTip((Control)sender).Equals(textWithLineBreaks))
                    {
                        Size stringSize = TextRenderer.MeasureText(textBox.Text, textBox.Font);
                        if (textBox.Width >= stringSize.Width)
                        {
                            Session.ToolTip2.RemoveAll();
                        } else {
                            Session.ToolTip2.RemoveAll();
                            Session.ToolTip2.SetToolTip((Control)sender, textWithLineBreaks);
                            lastSender = sender;
                        }
                    }
                } else {
                    Session.ToolTip2.RemoveAll();
                }
            }            
        }

        private string DetermineHoveredItem(ListBox listBox)
        {
            Point screenPosition = ListBox.MousePosition;
            Point listBoxClientAreaPosition = listBox.PointToClient(screenPosition);

            int hoveredIndex = listBox.IndexFromPoint(listBoxClientAreaPosition);
            if (hoveredIndex != -1)
            {
                return listBox.Items[hoveredIndex] as string;
            } else {
                return null;
            }
        }

        public void FocusOnTab()
        {
            if (Session.tabControl1.SelectedIndex == 0)
            {
                panel2.Focus();
            }
            else if (Session.tabControl1.SelectedIndex == 1)
            {
                panel3.Focus();
            }
        }
    }

    public static class MouseInput
    {
        // TME_HOVER
        // The caller wants hover notification. Notification is delivered as a 
        // WM_MOUSEHOVER message.  If the caller requests hover tracking while 
        // hover tracking is already active, the hover timer will be reset.

        private const int TME_HOVER = 0x1;

        private struct TRACKMOUSEEVENT
        {
            // Size of the structure - calculated in the constructor
            public int cbSize;

            // value that we'll set to specify we want to start over Mouse Hover and get
            // notification when the hover has happened
            public int dwFlags;

            // Handle to what's interested in the event
            public IntPtr hwndTrack;

            // How long it takes for a hover to occur
            public int dwHoverTime;

            // Setting things up specifically for a simple reset
            public TRACKMOUSEEVENT(IntPtr hWnd)
            {
                this.cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
                this.hwndTrack = hWnd;
                this.dwHoverTime = SystemInformation.MouseHoverTime;
                this.dwFlags = TME_HOVER;
            }

        }

        // Declaration of the Win32API function
        [DllImport("user32")]
        private static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        public static void ResetMouseHover(IntPtr windowTrackingMouseHandle)
        {
            // Set up the parameter collection for the API call so that the appropriate
            // control fires the event
            TRACKMOUSEEVENT parameterBag = new TRACKMOUSEEVENT(windowTrackingMouseHandle);

            // The actual API call
            TrackMouseEvent(ref parameterBag);
        }

    }
}