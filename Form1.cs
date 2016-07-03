using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.Serialization;

namespace TCEE
{
    public partial class Form1 : Form
    {
        #region Startup

            public SettingsType SettingsType = null;
            public VersionConfig VersionConfig;

            string DestinationConfigsDir = "";
            string WorldSaveDir = "";
            string SourceConfigsDir = "";
            FolderBrowserDialog fbdDestinationWorldDir = new FolderBrowserDialog();
            ToolTip ToolTip1 = new ToolTip();

            List<string> BiomeNames = new List<string>();
            List<ListBox> BiomeListInputs = new List<ListBox>();
            Dictionary<object, TCProperty> ResourceQueueInputs = new Dictionary<object, TCProperty>();

            DirectoryInfo VersionDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/");

            void Form1_DragEnter(object sender, DragEventArgs e)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            }

            void Form1_DragDrop(object sender, DragEventArgs e)
            {
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
                                    TCProperty property = BiomeSettingsInputs.First(a => a.Key.Name == "ResourceQueue").Key;
                                    ListBox box = (ListBox)BiomeSettingsInputs.First(a => a.Key.Name == "ResourceQueue").Value.Item1;
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

            public Form1()
            {
                InitializeComponent();

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

                player.settings.autoStart = false;
                player.settings.playCount = -1;
                player.settings.volume = 50;
                player.URL = Path.GetDirectoryName(Application.ExecutablePath) + "/Resources/Kenny Burrell - Birk's Works (TCEE Edit).mp3";
                player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(delegate(int arse) { if (player.playState == WMPLib.WMPPlayState.wmppsStopped && !playerStopped) { player.controls.play(); } });

                Height = 202;

                btSave.Enabled = false;
                btLoad.Enabled = false;

                cbVersion.SelectedIndexChanged += new EventHandler(delegate(object s, EventArgs args)
                {
                    cbWorld.Items.Clear();
                    DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + cbVersion.SelectedItem + "/Worlds/");
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

                if(VersionDir.Exists)
                {
                    foreach (DirectoryInfo dir1 in VersionDir.GetDirectories())
                    {
                        cbVersion.Items.Add(dir1.Name);
                    }
                    if(cbVersion.Items.Count > 0)
                    {
                        cbVersion.SelectedIndex = 0;
                    }
                }
                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/Saves/");
                if (!dir.Exists)
                {
                    dir.Create();
                }
                sfd = new SaveFileDialog() { DefaultExt = "xml", InitialDirectory = dir.FullName };
                sfd.Title = "Select a TCEE save file";
                ofd = new OpenFileDialog() { DefaultExt = "xml", InitialDirectory = dir.FullName };
                ofd.Title = "Select a save location";

                convertBO3fbd = new FolderBrowserDialog();
                convertBO3fbd.Description = "Select output directory";                
                convertBO3ofd = new OpenFileDialog() { DefaultExt = "schematic", InitialDirectory = dir.FullName };
                convertBO3ofd.Title = "Select schematic(s) to convert";
                convertBO3ofd.Multiselect = true;

                copyBO3fbd = new FolderBrowserDialog();
                copyBO3fbd.Description = "Select a /GlobalObjects or /WorldObjects directory (create if needed).";                
            }

            void SelectSourceWorld_Click(object sender, EventArgs e)
            {
                BiomeNames.Clear();
                BiomeListInputs.Clear();
                DestinationConfigsDir = "";
                WorldSaveDir = "";
                SourceConfigsDir = "";
                ToolTip1.RemoveAll();
                ResourceQueueInputs.Clear();
                tlpWorldSettings.Controls.Clear();
                tlpWorldSettings.RowStyles.Clear();
                tlpBiomeSettings.Controls.Clear();
                tlpBiomeSettings.RowStyles.Clear();

                WorldConfigDefaultValues = null;
                WorldConfig1 = null;
                WorldSettingsInputs.Clear();

                BiomeGroups.Clear();
                BiomeConfigsDefaultValues.Clear();
                BiomeSettingsInputs.Clear();

                lbGroups.Items.Clear();
                lbGroup.Items.Clear();
                lbBiomes.Items.Clear();

                DirectoryInfo versionDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/");
                if (cbVersion.Items.Count > 0 && cbVersion.SelectedItem != null && versionDir.Exists)
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(VersionConfig));
                    using (var reader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + cbVersion.SelectedItem + "/VersionConfig.xml"))
                    {
                        VersionConfig = (VersionConfig)serializer.Deserialize(reader);
                    }
                    if (VersionConfig != null)
                    {
                        SettingsType = VersionConfig.SettingsTypes.FirstOrDefault(a => a.Type == "Forge") ?? VersionConfig.SettingsTypes.First();
                        WorldConfig1 = new WorldConfig(VersionConfig);
                        LoadUI();
                    } else {
                        MessageBox.Show("Y u do dis? :(");
                    }

                    SourceConfigsDir = Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + cbVersion.SelectedItem + "/Worlds/" + cbWorld.SelectedItem + "/";

                    if (!String.IsNullOrEmpty(SourceConfigsDir) && System.IO.Directory.Exists(SourceConfigsDir + "/" + "WorldBiomes" + "/"))
                    {
                        System.IO.DirectoryInfo defaultWorldDirectory = new System.IO.DirectoryInfo(SourceConfigsDir + "/" + "WorldBiomes" + "/");
                        foreach (System.IO.FileInfo file in defaultWorldDirectory.GetFiles())
                        {
                            if (file.Name.EndsWith(".bc"))
                            {
                                BiomeNames.Add(file.Name.Replace(".bc", ""));
                            }
                        }
                        foreach (ListBox listBox in BiomeListInputs)
                        {
                            listBox.Items.Clear();
                            foreach (string biomeName in BiomeNames)
                            {
                                listBox.Items.Add(biomeName.Trim());
                            }
                        }
                    }

                    WorldConfigDefaultValues = LoadWorldConfigFromFile(new FileInfo(SourceConfigsDir + "WorldConfig.ini"), VersionConfig, true);
                    if(WorldConfigDefaultValues == null)
                    {
                        throw new Exception("WorldConfig.ini could not be loaded. Please make sure that WorldConfig.ini is present in the TCVersionConfig directory for the selected version. Exiting TCEE.");
                    } else {

                        LoadBiomesList();
                        LoadDefaultGroups();
                        if (lbBiomes.Items.Count > 0)
                        {
                            lbBiomes.SelectedIndex = 0;
                        }

                        foreach(Button bSetDefaults in WorldSettingsInputs.Select(a => a.Value.Item3))
                        {
                            ToolTip1.SetToolTip(bSetDefaults, "Set to default");
                            bSetDefaults.Text = "<";
                        }
                        btSetToDefault.Text = "Set to defaults";
                    }
                }
            }

            public void LoadUI()
            { 
                ToolTip1 = new ToolTip();
                ToolTip1.AutoPopDelay = 32000;
                ToolTip1.InitialDelay = 500;
                ToolTip1.ReshowDelay = 0;
                ToolTip1.ShowAlways = true;

                ToolTip1.SetToolTip(lblGroups, 
                    "Create biome groups containing one or more biome(s) and configure settings for each group.\r\n" +
                    "If a biome is listed in multiple groups then the order of the groups determines the eventual value.\r\n" +
                    "For instance you can set a value in the first group and override it in the second.\r\n" +
                    "Properties that accept a list of biomes or resources as a value can even be merged\r\n" +
                    "so you can add gold ore in the first group and diamond ore in the second."
                );

                tabControl1.Visible = false;
                btGenerate.Visible = false;
                btCopyBO3s.Visible = false;
                cbDeleteRegion.Visible = false;
                btSave.Enabled = false;
                btLoad.Enabled = false;
                tlpBiomeSettings.Visible = false;
                btBiomeSettingsResetToDefaults.Visible = false;

                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;

                string uniqueResourceQueueItems = "";
                foreach (ResourceQueueItem resourceQueueItem in VersionConfig.ResourceQueueOptions)
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

                int i = 0;
                foreach (TCProperty property in VersionConfig.WorldConfig)
                {
                    int row = i == 0 ? 0 : Convert.ToInt32(Math.Floor((float)i / 2));
                    int column1 = i % 2 == 0 ? 0 : 4;
                    Label txPropertyLabel = new Label();
                    txPropertyLabel.Text = property.LabelText != null && property.LabelText.Length > 0 ? property.LabelText : property.Name;
                    txPropertyLabel.AutoSize = true;
                    txPropertyLabel.Margin = new Padding(0, 5, 0, 0);
                    tlpWorldSettings.Controls.Add(txPropertyLabel, column1, row);

                    int column2 = i % 2 == 0 ? 1 : 5;
                    CheckBox cbOverride = new CheckBox();
                    cbOverride.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    cbOverride.TabStop = false;
                    ToolTip1.SetToolTip(cbOverride, "Apply this value");
                    tlpWorldSettings.Controls.Add(cbOverride, column2, row);
                    cbOverride.CheckedChanged += PropertyInputOverrideCheckChangedWorld;

                    int column4 = i % 2 == 0 ? 3 : 7;
                    Button bSetDefaults = new Button();
                    bSetDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    bSetDefaults.BackColor = Color.FromKnownColor(KnownColor.Control);
                    bSetDefaults.Text = "C";
                    bSetDefaults.Width = 23;
                    bSetDefaults.Height = 23;
                    bSetDefaults.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
                    bSetDefaults.Click += bSetDefaultsWorldProperty;
                    bSetDefaults.TabStop = false;
                    tlpWorldSettings.Controls.Add(bSetDefaults, column4, row);

                    int column3 = i % 2 == 0 ? 2 : 6;
                    switch (property.PropertyType)
                    {
                        case "ResourceQueue":
                            Panel pnl = new Panel();
                            pnl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

                            ListBox lbPropertyInput = new ListBox();
                            lbPropertyInput.Sorted = true;
                            lbPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                            lbPropertyInput.Dock = DockStyle.Fill;
                            lbPropertyInput.AutoSize = true;
                            lbPropertyInput.SelectionMode = SelectionMode.MultiExtended;
                            lbPropertyInput.KeyDown += lbPropertyInput_KeyDown_World;
                            pnl.Controls.Add(lbPropertyInput);

                            Panel pnl2 = new Panel();
                            pnl2.Dock = DockStyle.Bottom;
                            pnl2.AutoSize = true;

                            Panel pnl3 = new Panel();
                            pnl3.Dock = DockStyle.Bottom;
                            pnl3.AutoSize = true;
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
                            btAddResourceQueueItem.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btAddResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                            btAddResourceQueueItem.Width = 45;
                            btAddResourceQueueItem.Text = "Add";
                            btAddResourceQueueItem.Click += btAddResourceQueueItemWorld_Click;
                            ResourceQueueInputs.Add(btAddResourceQueueItem, property);
                            pnl3.Controls.Add(btAddResourceQueueItem);

                            Button btEditResourceQueueItem = new Button();
                            btEditResourceQueueItem.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btEditResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                            btEditResourceQueueItem.Left = 50;
                            btEditResourceQueueItem.Width = 48;
                            btEditResourceQueueItem.Text = "Edit";
                            btEditResourceQueueItem.Click += btEditResourceQueueItemWorld_Click;
                            ResourceQueueInputs.Add(btEditResourceQueueItem, property);
                            pnl3.Controls.Add(btEditResourceQueueItem);

                            Button btDeleteResourceQueueItem = new Button();
                            btDeleteResourceQueueItem.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btDeleteResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                            btDeleteResourceQueueItem.Left = 103;
                            btDeleteResourceQueueItem.Width = 50;
                            btDeleteResourceQueueItem.Text = "Delete";
                            btDeleteResourceQueueItem.Click += btDeleteResourceQueueItemWorld_Click;
                            ResourceQueueInputs.Add(btDeleteResourceQueueItem, property);
                            pnl3.Controls.Add(btDeleteResourceQueueItem);

                            pnl2.Controls.Add(pnl3);

                            Panel pCheckBoxes = new Panel();
                            pCheckBoxes.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            pCheckBoxes.Dock = DockStyle.Bottom;
                            pCheckBoxes.AutoSize = true;

                            RadioButton btOverrideAll = new RadioButton();
                            btOverrideAll.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btOverrideAll.Dock = DockStyle.Top;
                            btOverrideAll.Text = "Override defaults";
                            btOverrideAll.Checked = true;
                            btOverrideAll.Enabled = false;
                            btOverrideAll.Visible = false;
                            btOverrideAll.Name = "Override";
                            btOverrideAll.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                            pCheckBoxes.Controls.Add(btOverrideAll);
                            ToolTip1.SetToolTip(btOverrideAll, "Removes all default values and replaces them with selected values.");

                            RadioButton btMergeWithDefaults = new RadioButton();
                            btMergeWithDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btMergeWithDefaults.Dock = DockStyle.Top;
                            btMergeWithDefaults.Text = "Merge with defaults";
                            btMergeWithDefaults.Enabled = false;
                            btMergeWithDefaults.Visible = false;
                            btMergeWithDefaults.Name = "Merge";
                            btMergeWithDefaults.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                            pCheckBoxes.Controls.Add(btMergeWithDefaults);
                            ToolTip1.SetToolTip(btMergeWithDefaults, "Adds selected values to the default values. Some properties must be unique such as Ore(GOLD_ORE, Vein(GOLD_ORE and UnderWaterOre(GOLD_ORE and will replace existing values. Unique properties are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nProperty name and * must be unique.\r\n\r\nUpdate: An exception is made for Vein(), Ore() and UnderWaterOre(), they only have to be unique when used with ores (IRON_ORE, GOLD_ORE etc), if used with non-ore materials they can appear multiple times and will not override other settings.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.");

                            CheckBox btIgnoreParentMerge = new CheckBox();
                            btIgnoreParentMerge.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btIgnoreParentMerge.Dock = DockStyle.Top;
                            btIgnoreParentMerge.Text = "Override parent values";
                            btIgnoreParentMerge.Name = "OverrideParent";
                            btIgnoreParentMerge.Enabled = false;
                            btIgnoreParentMerge.Visible = false;
                            btIgnoreParentMerge.CheckedChanged += btOverrideParentValuesWorld_CheckedChanged;
                            pCheckBoxes.Controls.Add(btIgnoreParentMerge);
                            ToolTip1.SetToolTip(btIgnoreParentMerge, "Ignore any values defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add values to the same property.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore to ResourceQueue.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");

                            pnl2.Controls.Add(pCheckBoxes);
                            pnl.Controls.Add(pnl2);
                            tlpWorldSettings.Controls.Add(pnl, column3, row);

                            if (tlpWorldSettings.RowStyles.Count - 1 < row)
                            {
                                tlpWorldSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                            }
                            tlpWorldSettings.RowStyles[tlpWorldSettings.RowStyles.Count - 1].SizeType = SizeType.Absolute;
                            tlpWorldSettings.RowStyles[tlpWorldSettings.RowStyles.Count - 1].Height = 120;

                            WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes));
                        break;
                        case "String":
                        case "Float":
                            TextBox txPropertyInput = new TextBox();
                            txPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                            tlpWorldSettings.Controls.Add(txPropertyInput, column3, row);
                            txPropertyInput.TextChanged += PropertyInputChangedWorld;
                            txPropertyInput.LostFocus += PropertyInputLostFocusWorld;
                            WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                            break;
                        case "Color":
                            Panel colorPickerPanel = new Panel();
                            colorPickerPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                            colorPickerPanel.Height = 24;

                            ListBox lbPropertyInput2 = new ListBox();
                            lbPropertyInput2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            lbPropertyInput2.Width = 20;
                            lbPropertyInput2.Height = 24;
                            lbPropertyInput2.BackColor = Color.White;
                            lbPropertyInput2.Margin = new Padding(3, 0, 0, 0);
                            lbPropertyInput2.TabStop = false;
                            colorPickerPanel.Controls.Add(lbPropertyInput2);
                            lbPropertyInput2.Click += PropertyInputColorChangedWorld;

                            TextBox txPropertyInput2 = new TextBox();
                            txPropertyInput2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                            colorPickerPanel.Controls.Add(txPropertyInput2);
                            txPropertyInput2.TextChanged += PropertyInputColorChangedWorld;
                            txPropertyInput2.LostFocus += PropertyInputLostFocusWorld;
                            txPropertyInput2.Left = 26;                     
                            tlpWorldSettings.Controls.Add(colorPickerPanel, column3, row);

                            WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput2, cbOverride, bSetDefaults, txPropertyLabel, lbPropertyInput2, null));
                            break;
                        case "BiomesList":
                            Panel pnl4 = new Panel();
                            pnl4.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

                            ListBox lbPropertyInput3 = new ListBox();
                            lbPropertyInput3.Sorted = true;
                            lbPropertyInput3.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                            lbPropertyInput3.Dock = DockStyle.Fill;
                            lbPropertyInput3.AutoSize = true;
                            lbPropertyInput3.SelectionMode = SelectionMode.MultiExtended;
                            lbPropertyInput3.SelectedIndexChanged += lbPropertyInputWorld_SelectedIndexChanged;
                            pnl4.Controls.Add(lbPropertyInput3);

                            Panel pnl5 = new Panel();
                            pnl5.Dock = DockStyle.Bottom;
                            pnl5.AutoSize = true;

                            Panel pCheckBoxes2 = new Panel();
                            pCheckBoxes2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            pCheckBoxes2.Dock = DockStyle.Bottom;
                            pCheckBoxes2.AutoSize = true;

                            RadioButton btOverrideAll2 = new RadioButton();
                            btOverrideAll2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btOverrideAll2.Dock = DockStyle.Top;
                            btOverrideAll2.Text = "Override defaults";
                            btOverrideAll2.Checked = true;
                            btOverrideAll2.Enabled = false;
                            btOverrideAll2.Visible = false;
                            btOverrideAll2.Name = "Override";
                            btOverrideAll2.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                            pCheckBoxes2.Controls.Add(btOverrideAll2);
                            ToolTip1.SetToolTip(btOverrideAll2, "Removes all default values and replaces them with selected values.");

                            RadioButton btMergeWithDefaults2 = new RadioButton();
                            btMergeWithDefaults2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btMergeWithDefaults2.Dock = DockStyle.Top;
                            btMergeWithDefaults2.Text = "Merge with defaults";
                            btMergeWithDefaults2.Enabled = false;
                            btMergeWithDefaults2.Visible = false;
                            btMergeWithDefaults2.Name = "Merge";
                            btMergeWithDefaults2.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                            pCheckBoxes2.Controls.Add(btMergeWithDefaults2);
                            ToolTip1.SetToolTip(btMergeWithDefaults2, "Adds selected values to the default values. Some properties must be unique such as Ore(GOLD_ORE, Vein(GOLD_ORE and UnderWaterOre(GOLD_ORE and will replace existing values. Unique properties are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nProperty name and * must be unique.\r\n\r\nUpdate: An exception is made for Vein(), Ore() and UnderWaterOre(), they only have to be unique when used with ores (IRON_ORE, GOLD_ORE etc), if used with non-ore materials they can appear multiple times and will not override other settings.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.");


                            CheckBox btIgnoreParentMerge2 = new CheckBox();
                            btIgnoreParentMerge2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btIgnoreParentMerge2.Dock = DockStyle.Top;
                            btIgnoreParentMerge2.Text = "Override parent values";
                            btIgnoreParentMerge2.Name = "OverrideParent";
                            btIgnoreParentMerge2.Enabled = false;
                            btIgnoreParentMerge2.Visible = false;
                            btIgnoreParentMerge2.CheckedChanged += btOverrideParentValuesWorld_CheckedChanged;
                            pCheckBoxes2.Controls.Add(btIgnoreParentMerge2);
                            ToolTip1.SetToolTip(btIgnoreParentMerge2, "Ignore any values defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add values to the same property.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore to ResourceQueue.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");

                            pnl5.Controls.Add(pCheckBoxes2);
                            pnl4.Controls.Add(pnl5);
                            tlpWorldSettings.Controls.Add(pnl4, column3, row);

                            if (tlpWorldSettings.RowStyles.Count - 1 < row)
                            {
                                tlpWorldSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                            }
                            tlpWorldSettings.RowStyles[tlpWorldSettings.RowStyles.Count - 1].SizeType = SizeType.Absolute;
                            tlpWorldSettings.RowStyles[tlpWorldSettings.RowStyles.Count - 1].Height = 120;

                            WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput3, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes2));
                            BiomeListInputs.Add(lbPropertyInput3);

                            break;
                        case "Bool":
                            ComboBox cbPropertyInput = new ComboBox();
                            cbPropertyInput.Items.Add("");
                            cbPropertyInput.Items.Add("true");
                            cbPropertyInput.Items.Add("false");
                            cbPropertyInput.SelectedIndex = 0;
                            cbPropertyInput.DropDownStyle = ComboBoxStyle.DropDownList;
                            cbPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                            tlpWorldSettings.Controls.Add(cbPropertyInput, column3, row);
                            cbPropertyInput.SelectedIndexChanged += PropertyInputChangedWorld;
                            //cbPropertyInput.LostFocus += PropertyInputLostFocusWorld;
                            WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(cbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                            break;
                    }
                    if (tlpWorldSettings.RowStyles.Count - 1 < row)
                    {
                        tlpWorldSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }

                    ToolTip1.SetToolTip(bSetDefaults, "Clear");

                    i += 1;
                }

                i = 0;
                foreach (TCProperty property in VersionConfig.BiomeConfig)
                {
                    int row = i == 0 ? 0 : Convert.ToInt32(Math.Floor((float)i / 2));
                    int column1 = i % 2 == 0 ? 0 : 4;
                    Label txPropertyLabel = new Label();
                    txPropertyLabel.Text = property.LabelText != null && property.LabelText.Length > 0 ? property.LabelText : property.Name;
                    txPropertyLabel.AutoSize = true;
                    txPropertyLabel.Margin = new Padding(0, 5, 0, 0);
                    tlpBiomeSettings.Controls.Add(txPropertyLabel, column1, row);

                    int column2 = i % 2 == 0 ? 1 : 5;
                    CheckBox cbOverride = new CheckBox();
                    cbOverride.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    cbOverride.TabStop = false;
                    ToolTip1.SetToolTip(cbOverride, "Apply this value");
                    tlpBiomeSettings.Controls.Add(cbOverride, column2, row);
                    cbOverride.CheckedChanged += PropertyInputOverrideCheckChangedBiome;

                    int column4 = i % 2 == 0 ? 3 : 7;
                    Button bSetDefaults = new Button();
                    bSetDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    bSetDefaults.Text = "C";
                    bSetDefaults.BackColor = Color.FromKnownColor(KnownColor.Control);
                    bSetDefaults.Width = 23;
                    bSetDefaults.Height = 23;
                    bSetDefaults.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
                    bSetDefaults.Click += bSetDefaultsBiomesProperty;
                    bSetDefaults.TabStop = false;
                    tlpBiomeSettings.Controls.Add(bSetDefaults, column4, row);

                    int column3 = i % 2 == 0 ? 2 : 6;
                    if (property.PropertyType == "ResourceQueue")
                    {
                        Panel pnl = new Panel();
                        pnl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

                        ListBox lbPropertyInput = new ListBox();
                        lbPropertyInput.Sorted = true;
                        lbPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                        lbPropertyInput.Dock = DockStyle.Fill;
                        lbPropertyInput.AutoSize = true;
                        lbPropertyInput.SelectionMode = SelectionMode.MultiExtended;
                        lbPropertyInput.KeyDown += lbPropertyInput_KeyDown;
                        pnl.Controls.Add(lbPropertyInput);

                        Panel pnl2 = new Panel();
                        pnl2.Dock = DockStyle.Bottom;
                        pnl2.AutoSize = true;

                        Panel pnl3 = new Panel();
                        pnl3.Dock = DockStyle.Bottom;
                        pnl3.AutoSize = true;
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
                        btAddResourceQueueItem.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        btAddResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                        btAddResourceQueueItem.Width = 45;
                        btAddResourceQueueItem.Text = "Add";
                        btAddResourceQueueItem.Click += btAddResourceQueueItem_Click;
                        ResourceQueueInputs.Add(btAddResourceQueueItem, property);
                        pnl3.Controls.Add(btAddResourceQueueItem);

                        Button btEditResourceQueueItem = new Button();
                        btEditResourceQueueItem.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        btEditResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                        btEditResourceQueueItem.Left = 50;
                        btEditResourceQueueItem.Width = 48;
                        btEditResourceQueueItem.Text = "Edit";
                        btEditResourceQueueItem.Click += btEditResourceQueueItem_Click;
                        ResourceQueueInputs.Add(btEditResourceQueueItem, property);
                        pnl3.Controls.Add(btEditResourceQueueItem);

                        Button btDeleteResourceQueueItem = new Button();
                        btDeleteResourceQueueItem.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        btDeleteResourceQueueItem.BackColor = Color.FromKnownColor(KnownColor.Control);
                        btDeleteResourceQueueItem.Left = 103;
                        btDeleteResourceQueueItem.Width = 50;
                        btDeleteResourceQueueItem.Text = "Delete";
                        btDeleteResourceQueueItem.Click += btDeleteResourceQueueItem_Click;
                        ResourceQueueInputs.Add(btDeleteResourceQueueItem, property);
                        pnl3.Controls.Add(btDeleteResourceQueueItem);

                        pnl2.Controls.Add(pnl3);

                        Panel pCheckBoxes = new Panel();
                        pCheckBoxes.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        pCheckBoxes.Dock = DockStyle.Bottom;
                        pCheckBoxes.AutoSize = true;

                        RadioButton btOverrideAll = new RadioButton();
                        btOverrideAll.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        btOverrideAll.Dock = DockStyle.Top;
                        btOverrideAll.Text = "Override defaults";
                        btOverrideAll.Name = "Override";
                        btOverrideAll.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                        pCheckBoxes.Controls.Add(btOverrideAll);
                        ToolTip1.SetToolTip(btOverrideAll, "Removes all default values and replaces them with selected values.");

                        RadioButton btMergeWithDefaults = new RadioButton();
                        btMergeWithDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        btMergeWithDefaults.Dock = DockStyle.Top;
                        btMergeWithDefaults.Text = "Merge with defaults";
                        btMergeWithDefaults.Checked = true;
                        btMergeWithDefaults.Name = "Merge";
                        btMergeWithDefaults.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                        pCheckBoxes.Controls.Add(btMergeWithDefaults);
                        ToolTip1.SetToolTip(btMergeWithDefaults, "Adds selected values to the default values. Some properties must be unique such as Ore(GOLD_ORE, Vein(GOLD_ORE and UnderWaterOre(GOLD_ORE and will replace existing values. Unique properties are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nProperty name and * must be unique.\r\n\r\nUpdate: An exception is made for Vein(), Ore() and UnderWaterOre(), they only have to be unique when used with ores (IRON_ORE, GOLD_ORE etc), if used with non-ore materials they can appear multiple times and will not override other settings.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.");

                        CheckBox btIgnoreParentMerge = new CheckBox();
                        btIgnoreParentMerge.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        btIgnoreParentMerge.Dock = DockStyle.Top;
                        btIgnoreParentMerge.Text = "Override parent values";
                        btIgnoreParentMerge.Name = "OverrideParent";
                        btIgnoreParentMerge.CheckedChanged += btOverrideParentValuesBiome_CheckedChanged;
                        pCheckBoxes.Controls.Add(btIgnoreParentMerge);
                        ToolTip1.SetToolTip(btIgnoreParentMerge, "Ignore any values defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add values to the same property.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");
                    
                        pnl2.Controls.Add(pCheckBoxes);

                        pnl.Controls.Add(pnl2);
                        tlpBiomeSettings.Controls.Add(pnl, column3, row);

                        if (tlpBiomeSettings.RowStyles.Count - 1 < row)
                        {
                            tlpBiomeSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        }
                        tlpBiomeSettings.RowStyles[tlpBiomeSettings.RowStyles.Count - 1].SizeType = SizeType.Absolute;
                        tlpBiomeSettings.RowStyles[tlpBiomeSettings.RowStyles.Count - 1].Height = 200;
                        BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes));
                    } else {
                        switch (property.PropertyType)
                        {
                            case "String":
                            case "Float":
                                TextBox txPropertyInput = new TextBox();
                                txPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                tlpBiomeSettings.Controls.Add(txPropertyInput, column3, row);
                                txPropertyInput.TextChanged += PropertyInputChangedBiome;
                                txPropertyInput.LostFocus += PropertyInputLostFocusBiome;
                                BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                                break;
                            case "Color":
                                Panel colorPickerPanel = new Panel();
                                colorPickerPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                                colorPickerPanel.Height = 24;

                                ListBox lbPropertyInput = new ListBox();
                                lbPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                lbPropertyInput.Width = 20;
                                lbPropertyInput.Height = 24;
                                lbPropertyInput.BackColor = Color.White;
                                lbPropertyInput.Margin = new Padding(3, 0, 0, 0);
                                lbPropertyInput.TabStop = false;
                                colorPickerPanel.Controls.Add(lbPropertyInput);
                                lbPropertyInput.Click += PropertyInputColorChangedBiome;
                                lbPropertyInput.LostFocus += PropertyInputLostFocusBiome;

                                TextBox txPropertyInput2 = new TextBox();
                                txPropertyInput2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                colorPickerPanel.Controls.Add(txPropertyInput2);
                                txPropertyInput2.TextChanged += PropertyInputColorChangedBiome;
                                txPropertyInput2.LostFocus += PropertyInputLostFocusBiome;
                                txPropertyInput2.Left = 26;

                                tlpBiomeSettings.Controls.Add(colorPickerPanel, column3, row);

                                BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput2, cbOverride, bSetDefaults, txPropertyLabel, lbPropertyInput, null));
                                break;
                            case "BiomesList":
                                Panel pnl = new Panel();
                                pnl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

                                ListBox lbPropertyInput2 = new ListBox();
                                lbPropertyInput2.Sorted = true;
                                lbPropertyInput2.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                                lbPropertyInput2.Dock = DockStyle.Fill;
                                lbPropertyInput2.AutoSize = true;
                                lbPropertyInput2.SelectionMode = SelectionMode.MultiExtended;
                                lbPropertyInput2.SelectedIndexChanged += lbPropertyInputBiome_SelectedIndexChanged;
                                pnl.Controls.Add(lbPropertyInput2);

                                Panel pnl2 = new Panel();
                                pnl2.Dock = DockStyle.Bottom;
                                pnl2.AutoSize = true;

                                Panel pCheckBoxes = new Panel();
                                pCheckBoxes.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                pCheckBoxes.Dock = DockStyle.Bottom;
                                pCheckBoxes.AutoSize = true;

                                RadioButton btOverrideAll = new RadioButton();
                                btOverrideAll.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btOverrideAll.Dock = DockStyle.Top;
                                btOverrideAll.Text = "Override defaults";
                                btOverrideAll.Name = "Override";
                                btOverrideAll.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                                pCheckBoxes.Controls.Add(btOverrideAll);
                                ToolTip1.SetToolTip(btOverrideAll, "Removes all default values and replaces them with selected values.");

                                RadioButton btMergeWithDefaults = new RadioButton();
                                btMergeWithDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btMergeWithDefaults.Dock = DockStyle.Top;
                                btMergeWithDefaults.Text = "Merge with defaults";
                                btMergeWithDefaults.Name = "Merge";
                                btMergeWithDefaults.Checked = true;
                                btMergeWithDefaults.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                                pCheckBoxes.Controls.Add(btMergeWithDefaults);
                                ToolTip1.SetToolTip(btMergeWithDefaults, "Adds selected values to the default values. Some properties must be unique such as Ore(GOLD_ORE, Vein(GOLD_ORE and UnderWaterOre(GOLD_ORE and will replace existing values. Unique properties are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nProperty name and * must be unique.\r\n\r\nUpdate: An exception is made for Vein(), Ore() and UnderWaterOre(), they only have to be unique when used with ores (IRON_ORE, GOLD_ORE etc), if used with non-ore materials they can appear multiple times and will not override other settings.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.");

                                CheckBox btIgnoreParentMerge = new CheckBox();
                                btIgnoreParentMerge.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btIgnoreParentMerge.Dock = DockStyle.Top;
                                btIgnoreParentMerge.Text = "Override parent values";
                                btIgnoreParentMerge.Name = "OverrideParent";
                                btIgnoreParentMerge.CheckedChanged += btOverrideParentValuesBiome_CheckedChanged;
                                pCheckBoxes.Controls.Add(btIgnoreParentMerge);
                                ToolTip1.SetToolTip(btIgnoreParentMerge, "Ignore any values defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add values to the same property.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");

                                pnl2.Controls.Add(pCheckBoxes);
                                pnl.Controls.Add(pnl2);
                                tlpBiomeSettings.Controls.Add(pnl, column3, row);

                                if (tlpBiomeSettings.RowStyles.Count - 1 < row)
                                {
                                    tlpBiomeSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                                }
                                tlpBiomeSettings.RowStyles[tlpBiomeSettings.RowStyles.Count - 1].SizeType = SizeType.Absolute;
                                tlpBiomeSettings.RowStyles[tlpBiomeSettings.RowStyles.Count - 1].Height = 180;

                                BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput2, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes));
                                BiomeListInputs.Add(lbPropertyInput2);
                                break;
                            case "Bool":
                                ComboBox cbPropertyInput = new ComboBox();
                                cbPropertyInput.Items.Add("");
                                cbPropertyInput.Items.Add("true");
                                cbPropertyInput.Items.Add("false");
                                cbPropertyInput.SelectedIndex = 0;
                                cbPropertyInput.DropDownStyle = ComboBoxStyle.DropDownList;
                                cbPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                                tlpBiomeSettings.Controls.Add(cbPropertyInput, column3, row);
                                cbPropertyInput.SelectedIndexChanged += PropertyInputChangedBiome;
                                //cbPropertyInput.LostFocus += PropertyInputLostFocusBiome;
                                BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(cbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                                break;
                        }
                    }
                    if (tlpBiomeSettings.RowStyles.Count - 1 < row)
                    {
                        tlpBiomeSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }

                    ToolTip1.SetToolTip(bSetDefaults, "Clear");

                    i += 1;
                }
            }

        #endregion

        #region World

            WorldConfig WorldConfigDefaultValues;
            WorldConfig WorldConfig1;
            Dictionary<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> WorldSettingsInputs = new Dictionary<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>>();

            WorldConfig LoadWorldConfigFromFile(FileInfo file, VersionConfig versionConfig, bool loadUI) // TODO: remove loadui param and load UI a different method! (this method used to be "loadWorldConfigDefaults")
            {
                WorldConfig worldConfig = new WorldConfig(versionConfig);

                if (file.Exists)
                {
                    if(loadUI)
                    {
                        tabControl1.Visible = true;
                        btSave.Enabled = true;
                        btLoad.Enabled = true;
                        btGenerate.Visible = true;
                        btCopyBO3s.Visible = true;
                        cbDeleteRegion.Visible = true;
                        //label4.Visible = true;
                        //label5.Visible = true;

                        AutoSize = false;
                        AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;

                        //Width = 1110;
                        Height = 743;
                    }

                    string txtErrors = "";

                    string sDefaultText = System.IO.File.ReadAllText(file.FullName);
                    foreach (TCProperty property in versionConfig.WorldConfig)
                    {
                        if(!loadUI || WorldSettingsInputs.ContainsKey(property))
                        {
                            //WorldSettingsInputs[property].Item1.Text = "";

                            string propertyValue = "";
                            if (property.PropertyType == "ResourceQueue")
                            {
                                bool bFound = false;
                                int replaceStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                                if (replaceStartIndex > -1)
                                {
                                    while (!bFound)
                                    {
                                        int lineStart = sDefaultText.Substring(replaceStartIndex).IndexOf("\n") + 1 + replaceStartIndex;
                                        int lineLength = sDefaultText.Substring(lineStart).IndexOf("\n") + 1;

                                        if (
                                            /* line doesnt contain # */
                                            !sDefaultText.Substring(lineStart, lineLength).Contains("#") && 
                                            (
                                                /* and line isnt empty or is empty and is last line or next line starts with ## */
                                                sDefaultText.Substring(lineStart, lineLength).Replace("\r", "").Replace("\n", "").Trim().Length > 0 || 
                                                (
                                                    //if this is the last line
                                                    sDefaultText.Substring(lineStart + lineLength).IndexOf("\n") < 0 ||
                                                    //or next line starts with ##
                                                    (sDefaultText.Substring(lineStart + lineLength).IndexOf("##") > -1 && sDefaultText.Substring(lineStart + lineLength).IndexOf("##") < sDefaultText.Substring(lineStart + lineLength).IndexOf("\n"))
                                                )
                                            )
                                        )
                                        {
                                            replaceStartIndex = lineStart;
                                            bFound = true;
                                        } else {
                                            replaceStartIndex = sDefaultText.Substring(replaceStartIndex).IndexOf("\n") + 1 + replaceStartIndex;
                                            if (replaceStartIndex < 0 || replaceStartIndex == sDefaultText.Length)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (replaceStartIndex > -1)
                                    {
                                        int replaceLength = 0;

                                        while (replaceStartIndex + replaceLength + 2 <= sDefaultText.Length && sDefaultText.Substring(replaceStartIndex + replaceLength, 2) != "\n#")
                                        {
                                            replaceLength += 1;
                                        }
                                        if (replaceStartIndex + replaceLength + 1 == sDefaultText.Length)
                                        {
                                            replaceLength += 1;
                                        }

                                        // Get comments above property for tooltips
                                        // Get all lines above propertyValue before encountering a line starting with multiple '#'s or another propertyvalue
                                        int commentStartIndex = -1;
                                        int commentEndIndex = replaceStartIndex - 1;
                                        int lastNoHashLineBeforeComment = sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("\n");
                                        if (lastNoHashLineBeforeComment > -1)
                                        {
                                            while (true)
                                            {
                                                if (lastNoHashLineBeforeComment > 0)
                                                {
                                                    int arg = sDefaultText.Substring(0, lastNoHashLineBeforeComment - 1).LastIndexOf("\n");
                                                    if (arg != -1)
                                                    {
                                                        string s = sDefaultText.Substring(arg, lastNoHashLineBeforeComment - arg);
                                                        if (!String.IsNullOrWhiteSpace(s) && !s.Replace("\n", "").Replace("\r", "").StartsWith("#"))
                                                        {
                                                            break;
                                                        } else {
                                                            lastNoHashLineBeforeComment = arg;
                                                        }
                                                    } else {
                                                        lastNoHashLineBeforeComment = -1;
                                                        break;
                                                    }
                                                } else {
                                                    break;
                                                }
                                            }
                                        }
                                        int lastDoubleHashLineBeforeComment = sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("##\n") > sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("##\r\n") ? sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("##\n") + 3 : sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("##\r\n") + 4;
                                        if (lastDoubleHashLineBeforeComment > -1 || lastNoHashLineBeforeComment > -1)
                                        {
                                            commentStartIndex = lastDoubleHashLineBeforeComment > lastNoHashLineBeforeComment ? lastDoubleHashLineBeforeComment : lastNoHashLineBeforeComment;
                                        }
                                        string comment = "";
                                        if (commentStartIndex > -1)
                                        {
                                            comment = sDefaultText.Substring(commentStartIndex, commentEndIndex - commentStartIndex).Trim();
                                        }
                                        if (loadUI && !String.IsNullOrEmpty(comment))
                                        {
                                            ToolTip1.SetToolTip(WorldSettingsInputs[property].Item4, comment);
                                        }

                                        propertyValue = sDefaultText.Substring(replaceStartIndex, replaceLength).Trim();

                                        if (loadUI)
                                        {
                                            IgnoreOverrideCheckChangedWorld = true;
                                            IgnorePropertyInputChangedWorld = true;

                                            ((ListBox)WorldSettingsInputs[property].Item1).Items.Clear();
                                            ((ListBox)WorldSettingsInputs[property].Item1).SelectedIndex = -1;
                                            ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                            ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                            ((CheckBox)WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;

                                            if (propertyValue != null)
                                            {
                                                string[] resourceQueueItemNames = propertyValue.Replace("\r", "").Split('\n');
                                                foreach (string resourceQueueItemName in resourceQueueItemNames)
                                                {
                                                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                                                    {
                                                        ((ListBox)WorldSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                                                    }
                                                }
                                            }

                                            worldConfig.SetProperty(property, propertyValue, false, false);

                                            IgnoreOverrideCheckChangedWorld = false;
                                            IgnorePropertyInputChangedWorld = false;
                                        } else {
                                            worldConfig.SetProperty(property, propertyValue, false, false);
                                        }
                                    } else {
                                        txtErrors += "\r\nProperty value for property " + property.Name + " could not be read from world configuration file. There is either an error in the file or TCEE needs to be updated to support the value's formatting.";
                                        //throw new Exception("Property value for property " + property.Name + " could not be read from world configuration file. There is either an error in the file or TCEE needs to be updated to support the value's formatting.");
                                    }
                                } else {
                                    txtErrors += "\r\nScriptHandle for property \"" + property.Name + "\" could not be found in world configuration file.";
                                    if (!property.Optional)
                                    {
                                        MessageBox.Show(txtErrors, "Version warnings");
                                        MessageBox.Show("The files you are trying to import have caused an error. They were probably not generated by the selected version of TC or MCW. Try using a lower version. MCW and TC are backwards compatible, so for instance you can use TC2.6.3 settings (MCW 1.0.5. and lower) in TC2.7.2 (MCW 1.0.6. or higher). When TC or MCW detects old world and biomeconfigs it should update the files automatically. You can then import the generated world into TCEE using the new version.", "Error reading configuration files");
                                        throw new InvalidDataException("ScriptHandle for property \"" + property.Name + "\" could not be found in world configuration file.");
                                    }
                                    //throw new Exception();
                                }
                            } else {
                                bool bFound = false;
                                int valueStringStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                                if (valueStringStartIndex > -1)
                                {
                                    while (!bFound)
                                    {
                                        //If this line is not a line starting with #
                                        if (sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("\n") > sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("#"))
                                        {
                                            bFound = true;
                                        } else {
                                            valueStringStartIndex = sDefaultText.Substring(valueStringStartIndex + property.ScriptHandle.Length).ToLower().IndexOf(property.ScriptHandle.ToLower()) + valueStringStartIndex + property.ScriptHandle.Length;
                                            if (valueStringStartIndex < 0)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (valueStringStartIndex > -1)
                                    {
                                        // Get comments above property for tooltips
                                        // Get all lines above propertyValue before encountering a line starting with multiple '#'s or another propertyvalue
                                        int commentStartIndex = -1;
                                        int commentEndIndex = valueStringStartIndex - 1;
                                        int lastNoHashLineBeforeComment = sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("\n");
                                        if (lastNoHashLineBeforeComment > -1)
                                        {
                                            while (true)
                                            {
                                                if (lastNoHashLineBeforeComment > 0)
                                                {
                                                    int arg = sDefaultText.Substring(0, lastNoHashLineBeforeComment - 1).LastIndexOf("\n");
                                                    if (arg != -1)
                                                    {
                                                        string s = sDefaultText.Substring(arg, lastNoHashLineBeforeComment - arg);
                                                        if (!String.IsNullOrWhiteSpace(s) && !s.Replace("\n", "").Replace("\r", "").StartsWith("#"))
                                                        {
                                                            break;
                                                        } else {
                                                            lastNoHashLineBeforeComment = arg;
                                                        }
                                                    } else {
                                                        lastNoHashLineBeforeComment = -1;
                                                        break;
                                                    }
                                                } else {
                                                    break;
                                                }
                                            }
                                        }
                                        int lastDoubleHashLineBeforeComment = sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("##\n") > sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("##\r\n") ? sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("##\n") + 3 : sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("##\r\n") + 4;
                                        if (lastDoubleHashLineBeforeComment > -1 || lastNoHashLineBeforeComment > -1)
                                        {
                                            commentStartIndex = lastDoubleHashLineBeforeComment > lastNoHashLineBeforeComment ? lastDoubleHashLineBeforeComment : lastNoHashLineBeforeComment;
                                        }
                                        string comment = "";
                                        if (commentStartIndex > -1)
                                        {
                                            comment = sDefaultText.Substring(commentStartIndex, commentEndIndex - commentStartIndex).Trim();
                                        }
                                        if (property.PropertyType == "BiomesList")
                                        {
                                            comment += "\r\n\r\nUse CTRL or SHIFT + Click to select one or multiple biomes from the list.";
                                        }
                                        if (loadUI && !String.IsNullOrEmpty(comment))
                                        {
                                            ToolTip1.SetToolTip(WorldSettingsInputs[property].Item4, comment);
                                        }

                                        int skipCharsLength = (property.ScriptHandle).Length;
                                        int valueStringLength = 0;
                                        //float originalValue = 0;

                                        while (valueStringStartIndex + skipCharsLength + valueStringLength < sDefaultText.Length && sDefaultText.Substring(valueStringStartIndex + skipCharsLength + valueStringLength, 1) != "\n")
                                        {
                                            valueStringLength += 1;
                                        }
                                        propertyValue = sDefaultText.Substring(valueStringStartIndex + skipCharsLength, valueStringLength).Trim();

                                        if (loadUI)
                                        {
                                            IgnoreOverrideCheckChangedWorld = true;
                                            IgnorePropertyInputChangedWorld = true;
                                            switch (property.PropertyType)
                                            {
                                                case "BiomesList":
                                                    ((ListBox)WorldSettingsInputs[property].Item1).SelectedItems.Clear();
                                                    string[] biomeNames = propertyValue.Split(',');
                                                    for (int k = 0; k < biomeNames.Length; k++)
                                                    {
                                                        if (BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                                        {
                                                            for (int l = 0; l < ((ListBox)WorldSettingsInputs[property].Item1).Items.Count; l++)
                                                            {
                                                                if (((string)((ListBox)WorldSettingsInputs[property].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                                                {
                                                                    ((ListBox)WorldSettingsInputs[property].Item1).SelectedItems.Add(((ListBox)WorldSettingsInputs[property].Item1).Items[l]);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                                    ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                                break;
                                                case "Bool":
                                                    if(propertyValue.ToLower() == "true")
                                                    {
                                                        ((ComboBox)WorldSettingsInputs[property].Item1).SelectedIndex = 1;
                                                        WorldSettingsInputs[property].Item2.Checked = false;
                                                    }
                                                    else if (propertyValue.ToLower() == "false")
                                                    {
                                                        ((ComboBox)WorldSettingsInputs[property].Item1).SelectedIndex = 2;
                                                        WorldSettingsInputs[property].Item2.Checked = false;
                                                    } else {
                                                        ((ComboBox)WorldSettingsInputs[property].Item1).SelectedIndex = 0;
                                                        WorldSettingsInputs[property].Item2.Checked = false;
                                                    }
                                                break;
                                                case "Color":
                                                    try
                                                    {
                                                        if (propertyValue.Length == 8 || (propertyValue.Length == 7 && propertyValue.StartsWith("#")))
                                                        {
                                                            WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                                            WorldSettingsInputs[property].Item1.Text = propertyValue;
                                                        } else {
                                                            WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                                            WorldSettingsInputs[property].Item1.Text = "";
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                                        WorldSettingsInputs[property].Item1.Text = "";
                                                    }
                                                    WorldSettingsInputs[property].Item2.Checked = false;
                                                break;
                                                case "Float":
                                                    WorldSettingsInputs[property].Item1.Text = propertyValue;
                                                    WorldSettingsInputs[property].Item2.Checked = false;
                                                break;
                                                case "String":
                                                    WorldSettingsInputs[property].Item1.Text = propertyValue;
                                                    WorldSettingsInputs[property].Item2.Checked = false;
                                                break;
                                            }
                                            worldConfig.SetProperty(property, propertyValue, false, false);
                                            IgnoreOverrideCheckChangedWorld = false;
                                            IgnorePropertyInputChangedWorld = false;
                                        } else {
                                            worldConfig.SetProperty(property, propertyValue, false, false);
                                        }
                                    } else {
                                        txtErrors += "\r\nProperty value for property " + property.Name + " could not be read from world configuration file. There is either an error in the file or TCEE needs to be updated to support the value's formatting.";
                                        //throw new Exception("Property value for property " + property.Name + " could not be read from world configuration file. There is either an error in the file or TCEE needs to be updated to support the value's formatting.");
                                    }
                                } else {
                                    txtErrors += "\r\nScriptHandle for property \"" + property.Name + "\" could not be found in world configuration file.";
                                    if (!property.Optional)
                                    {
                                        MessageBox.Show(txtErrors, "Version warnings");
                                        MessageBox.Show("The files you are trying to import have caused an error. They were probably not generated by the selected version of TC or MCW. Try importing using a different version. MCW and TC are backwards compatible, so for instance you can use TC2.6.3 settings (MCW 1.0.5. and lower) in TC2.7.2 (MCW 1.0.6. or higher). When TC or MCW detects old world and biomeconfigs it should update the files automatically. You can then import the generated world into TCEE using the new version.", "Error reading configuration files");
                                        throw new InvalidDataException("ScriptHandle for property \"" + property.Name + "\" could not be found in world configuration file.");
                                    }

                                    //throw new Exception("ScriptHandle for property \"" + property.Name + "\" could not be found in world configuration file");
                                }
                            }
                        }
                    }
                    if(txtErrors.Length > 0)
                    {
                        MessageBox.Show(txtErrors, "Version warnings");
                    }
                }
                return worldConfig;
            }

            private void btOverrideParentValuesWorld_CheckedChanged(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedWorld)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = WorldSettingsInputs.First(a => a.Value.Item6 == ((Control)sender).Parent);
                    TCProperty property = kvp.Key;

                    WorldConfig1.SetProperty(property, WorldConfig1.GetPropertyValueAsString(property), WorldConfig1.GetPropertyMerge(property), ((CheckBox)sender).Checked);
                }
            }

            private void btOverrideAllWorld_CheckedChanged(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedWorld && ((RadioButton)sender).Checked)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = WorldSettingsInputs.First(a => a.Value.Item6 == ((Control)sender).Parent);
                    Control tb = kvp.Value.Item1;
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    bool merge = false;

                    ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                    ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;

                    if (sender == kvp.Value.Item6.Controls.Find("Override", true)[0])
                    {
                        merge = !((RadioButton)sender).Checked;
                    }
                    else if (sender == kvp.Value.Item6.Controls.Find("Merge", true)[0])
                    {
                        merge = ((RadioButton)sender).Checked;
                    }
                    bool defaultValue = WorldConfigDefaultValues.GetPropertyMerge(property);
                    WorldConfig1.SetProperty(property, WorldConfig1.GetPropertyValueAsString(property), merge, ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    if(property.PropertyType == "BiomesList")
                    {
                        if ((!String.IsNullOrEmpty(WorldConfig1.GetPropertyValueAsString(property)) && !CompareBiomeLists(WorldConfig1.GetPropertyValueAsString(property), WorldConfigDefaultValues.GetPropertyValueAsString(property))) || (WorldConfig1.GetPropertyValueAsString(property) != null && !CompareBiomeLists(WorldConfig1.GetPropertyValueAsString(property), WorldConfigDefaultValues.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            cb.Checked = false;
                        }
                    }
                    else if (property.PropertyType == "ResourceQueue")
                    {
                        if ((!String.IsNullOrEmpty(WorldConfig1.GetPropertyValueAsString(property)) && !CompareResourceQueues(WorldConfig1.GetPropertyValueAsString(property), WorldConfigDefaultValues.GetPropertyValueAsString(property))) || (WorldConfig1.GetPropertyValueAsString(property) != null && !CompareResourceQueues(WorldConfig1.GetPropertyValueAsString(property), WorldConfigDefaultValues.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            cb.Checked = false;
                        }
                    }
                }
            }

            private void lbPropertyInputWorld_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedWorld)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = WorldSettingsInputs.First(a => a.Value.Item1 == sender);
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
                    WorldConfig1.SetProperty(property, sBiomeNames, merge, ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    bool bIsDefault = true;
                    bIsDefault = CompareBiomeLists(WorldConfigDefaultValues.GetPropertyValueAsString(property), WorldConfig1.GetPropertyValueAsString(property));

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
                if (!IgnorePropertyInputChangedWorld)
                {                
                    if(sender is ListBox)
                    {
                        ColorDialog colorDlg = new ColorDialog();
                        colorDlg.AllowFullOpen = false;
                        colorDlg.AnyColor = true;
                        colorDlg.SolidColorOnly = false;

                        if (colorDlg.ShowDialog() == DialogResult.OK)
                        {
                            KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = WorldSettingsInputs.First(a => a.Value.Item5 == sender);
                            TCProperty property = kvp.Key;
                            kvp.Value.Item5.BackColor = colorDlg.Color;
                            IgnorePropertyInputChangedWorld = true;
                            IgnoreOverrideCheckChangedWorld = true;
                            if (SettingsType.ColorType == "0x")
                            {
                                kvp.Value.Item1.Text = "0x" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            else if (SettingsType.ColorType == "#")
                            {
                                kvp.Value.Item1.Text = "#" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            WorldConfig1.SetProperty(property, kvp.Value.Item1.Text, false, false);

                            if (WorldConfigDefaultValues == null || kvp.Value.Item1.Text != WorldConfigDefaultValues.GetPropertyValueAsString(property))
                            {
                                kvp.Value.Item2.Checked = true;
                            } else {
                                kvp.Value.Item2.Checked = false;
                            }
                            IgnorePropertyInputChangedWorld = false;
                            IgnoreOverrideCheckChangedWorld = false;
                        }
                    }
                    else if(sender is TextBox)
                    {
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = WorldSettingsInputs.First(a => a.Value.Item1 == sender);
                        TCProperty property = kvp.Key;
                        try
                        {
                            if (kvp.Value.Item1.Text.Length == 8 || (kvp.Value.Item1.Text.Length == 7 && kvp.Value.Item1.Text.StartsWith("#")))
                            {
                                WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(kvp.Value.Item1.Text);
                                if (SettingsType.ColorType == "0x")
                                {
                                    kvp.Value.Item1.Text = "0x" + WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                else if (SettingsType.ColorType == "#")
                                {
                                    kvp.Value.Item1.Text = "#" + WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                WorldConfig1.SetProperty(property, kvp.Value.Item1.Text, false, false);

                                if (WorldConfigDefaultValues == null || kvp.Value.Item1.Text != WorldConfigDefaultValues.GetPropertyValueAsString(property))
                                {
                                    kvp.Value.Item2.Checked = true;
                                } else {
                                    kvp.Value.Item2.Checked = false;
                                }
                            } else {
                                WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                kvp.Value.Item2.Checked = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            WorldSettingsInputs[property].Item5.BackColor = Color.White;
                            kvp.Value.Item2.Checked = false;
                        }
                    }
                }
            }

            bool IgnorePropertyInputChangedWorld = false;
            void PropertyInputChangedWorld(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedWorld)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = WorldSettingsInputs.First(a => a.Value.Item1 == sender);
                    Control tb = kvp.Value.Item1;
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    float result;
                    if (
                        property.PropertyType == "String" ||
                        property.PropertyType == "Bool" ||
                        (property.PropertyType == "Float" && float.TryParse(tb.Text, out result))
                    )
                    {
                        WorldConfig1.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                        if (!(property.PropertyType == "Bool" && String.IsNullOrEmpty(tb.Text)) && (WorldConfigDefaultValues == null || (property.PropertyType != "Bool" && tb.Text != WorldConfigDefaultValues.GetPropertyValueAsString(property)) || (property.PropertyType == "Bool" && !String.IsNullOrEmpty(tb.Text) && tb.Text != WorldConfigDefaultValues.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            if (property.PropertyType == "Bool" && String.IsNullOrEmpty(tb.Text) && WorldConfigDefaultValues != null)
                            {
                                IgnoreOverrideCheckChangedWorld = true;
                                IgnorePropertyInputChangedWorld = true;
                                tb.Text = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                IgnorePropertyInputChangedWorld = false;
                                IgnoreOverrideCheckChangedWorld = false;
                            }
                            cb.Checked = false;
                        }
                    }
                }
            }

            void PropertyInputLostFocusWorld(object sender, EventArgs e)
            {
                // If color select box was sender
                if (WorldSettingsInputs.Any(a => a.Value.Item5 == sender))
                {
                    sender = WorldSettingsInputs.First(a => a.Value.Item5 == sender).Value.Item1;
                }

                TCProperty property = WorldSettingsInputs.First(a => a.Value.Item1 == sender).Key;
                if (property.PropertyType == "Color")
                {
                    IgnorePropertyInputChangedWorld = true;
                    IgnoreOverrideCheckChangedWorld = true;
                    bool bSetToDefaults = false;
                    if (((TextBox)sender).Text.Length == 8 || (((TextBox)sender).Text.Length == 7 && ((TextBox)sender).Text.StartsWith("#")))
                    {
                        try
                        {
                            WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(((TextBox)sender).Text);
                            string value = "";
                            if (SettingsType.ColorType == "0x")
                            {
                                value = "0x" + WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                            }
                            else if (SettingsType.ColorType == "#")
                            {
                                value = "#" + WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                            }
                            WorldSettingsInputs[property].Item1.Text = value;
                            WorldConfig1.SetProperty(property, value, false, false);
                            if (WorldConfigDefaultValues == null || value.ToUpper() != WorldConfigDefaultValues.GetPropertyValueAsString(property).ToUpper())
                            {
                                WorldSettingsInputs[property].Item2.Checked = true;
                            } else {
                                WorldSettingsInputs[property].Item2.Checked = false;
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
                        string defaultValue = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                        WorldConfig1.SetProperty(property, null, false, false);
                        if (defaultValue != null && (defaultValue.Length == 8 || (defaultValue.Length == 7 && defaultValue.StartsWith("#"))))
                        {
                            try
                            {
                                WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(defaultValue);
                                string value = "";
                                if (SettingsType.ColorType == "0x")
                                {
                                    value = "0x" + WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                else if (SettingsType.ColorType == "#")
                                {
                                    value = "#" + WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                WorldSettingsInputs[property].Item1.Text = value;
                            }
                            catch (Exception ex2)
                            {
                                WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                WorldSettingsInputs[property].Item1.Text = "";
                            }
                            WorldSettingsInputs[property].Item2.Checked = false;
                        } else {
                            WorldSettingsInputs[property].Item5.BackColor = Color.White;
                            WorldSettingsInputs[property].Item1.Text = "";
                            WorldSettingsInputs[property].Item2.Checked = false;
                        }
                    }
                    IgnorePropertyInputChangedWorld = false;
                    IgnoreOverrideCheckChangedWorld = false;
                }
                else if (property.PropertyType == "Float")
                {
                    float result = 0;
                    if (!float.TryParse(((TextBox)sender).Text, out result))
                    {
                        if (WorldConfigDefaultValues != null)
                        {
                            ((TextBox)sender).Text = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                        }
                        WorldSettingsInputs[property].Item2.Checked = false;
                    }
                }
                else if (String.IsNullOrWhiteSpace(((TextBox)sender).Text) && property.PropertyType != "String")
                {
                    if(WorldConfigDefaultValues != null)
                    {
                        ((TextBox)sender).Text = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                    }
                    WorldSettingsInputs[property].Item2.Checked = false;
                }
            }

            bool IgnoreOverrideCheckChangedWorld = false;
            void PropertyInputOverrideCheckChangedWorld(object sender, EventArgs e)
            {
                if (!IgnoreOverrideCheckChangedWorld)
                {
                    TCProperty property = WorldSettingsInputs.First(a => a.Value.Item2 == sender).Key;
                    WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = ((CheckBox)sender).Checked;

                    if (((CheckBox)sender).Checked)
                    {
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = WorldSettingsInputs.First(a => a.Value.Item2 == sender);
                        Control tb = kvp.Value.Item1;
                        CheckBox cb = kvp.Value.Item2;

                        float result;
                        if (
                            property.PropertyType == "String" ||
                            property.PropertyType == "Bool" ||
                            (property.PropertyType == "Float" && float.TryParse(tb.Text, out result))
                        )
                        {
                            WorldConfig1.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                        else if (property.PropertyType == "Float")
                        {
                            IgnoreOverrideCheckChangedWorld = true;
                            IgnorePropertyInputChangedWorld = true;
                            if (WorldConfigDefaultValues != null && WorldConfigDefaultValues.GetPropertyValueAsString(property) != null)
                            {
                                tb.Text = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                            } else {
                                tb.Text = "";
                            }
                            cb.Checked = false;
                            WorldConfig1.SetProperty(property, null, false, false);
                            IgnoreOverrideCheckChangedWorld = false;
                            IgnorePropertyInputChangedWorld = false;
                        }
                        else if (property.PropertyType == "Color")
                        {
                            try
                            {
                                if (kvp.Value.Item1.Text.Length == 8 || (kvp.Value.Item1.Text.Length == 7 && kvp.Value.Item1.Text.StartsWith("#")))
                                {
                                    if ((SettingsType.ColorType == "0x" && kvp.Value.Item1.Text.StartsWith("0x")) || (SettingsType.ColorType == "#" && kvp.Value.Item1.Text.StartsWith("#")))
                                    {
                                        Color derp = System.Drawing.ColorTranslator.FromHtml(WorldConfigDefaultValues.GetPropertyValueAsString(property));
                                        WorldConfig1.SetProperty(property, kvp.Value.Item1.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                                    } else {
                                        IgnoreOverrideCheckChangedWorld = true;
                                        IgnorePropertyInputChangedWorld = true;
                                        if (WorldConfigDefaultValues != null && !String.IsNullOrEmpty(WorldConfigDefaultValues.GetPropertyValueAsString(property)))
                                        {
                                            tb.Text = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                            WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(WorldConfigDefaultValues.GetPropertyValueAsString(property));
                                        } else {
                                            tb.Text = "";
                                            WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                        }
                                        cb.Checked = false;
                                        WorldConfig1.SetProperty(property, null, false, false);
                                        IgnoreOverrideCheckChangedWorld = false;
                                        IgnorePropertyInputChangedWorld = false;
                                    }
                                } else {
                                    IgnoreOverrideCheckChangedWorld = true;
                                    IgnorePropertyInputChangedWorld = true;
                                    if (WorldConfigDefaultValues != null && !String.IsNullOrEmpty(WorldConfigDefaultValues.GetPropertyValueAsString(property)))
                                    {
                                        tb.Text = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                        WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(WorldConfigDefaultValues.GetPropertyValueAsString(property));
                                    } else {
                                        tb.Text = "";
                                        WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                    }
                                    cb.Checked = false;
                                    WorldConfig1.SetProperty(property, null, false, false);
                                    IgnoreOverrideCheckChangedWorld = false;
                                    IgnorePropertyInputChangedWorld = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                IgnoreOverrideCheckChangedWorld = true;
                                IgnorePropertyInputChangedWorld = true;
                                if (WorldConfigDefaultValues != null && !String.IsNullOrEmpty(WorldConfigDefaultValues.GetPropertyValueAsString(property)))
                                {
                                    tb.Text = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                    WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(WorldConfigDefaultValues.GetPropertyValueAsString(property));
                                } else {
                                    tb.Text = "";
                                    WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                }
                                cb.Checked = false;
                                WorldConfig1.SetProperty(property, null, false, false);
                                IgnoreOverrideCheckChangedWorld = false;
                                IgnorePropertyInputChangedWorld = false;
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
                            WorldConfig1.SetProperty(property, sBiomeNames, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
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
                            WorldConfig1.SetProperty(property, sBiomeNames, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                    }
                }
            }

            void bSetDefaultsWorldProperty(object sender, EventArgs e)
            {
                KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = WorldSettingsInputs.FirstOrDefault(a => a.Value.Item3 == sender);
                if (WorldConfigDefaultValues != null)
                {
                    string propertyValue = WorldConfigDefaultValues.GetPropertyValueAsString(kvp.Key);
                    switch(kvp.Key.PropertyType)
                    {
                        case "BiomesList":
                            if(propertyValue != null)
                            {
                                ((ListBox)WorldSettingsInputs[kvp.Key].Item1).SelectedItems.Clear();
                                string[] biomeNames = propertyValue.Split(',');
                                ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                for (int k = 0; k < biomeNames.Length; k++)
                                {
                                    if (BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                    {
                                        for (int l = 0; l < ((ListBox)WorldSettingsInputs[kvp.Key].Item1).Items.Count; l++)
                                        {
                                            if (((string)((ListBox)WorldSettingsInputs[kvp.Key].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                            {
                                                ((ListBox)WorldSettingsInputs[kvp.Key].Item1).SelectedItems.Add(((ListBox)WorldSettingsInputs[kvp.Key].Item1).Items[l]);
                                            }
                                        }
                                    }
                                }
                            }
                        break;
                        case "Bool":
                            if (propertyValue != null && propertyValue.ToLower() == "true")
                            {
                                ((ComboBox)kvp.Value.Item1).SelectedIndex = 1;
                            }
                            else if (propertyValue != null && propertyValue.ToLower() == "false")
                            {
                                ((ComboBox)kvp.Value.Item1).SelectedIndex = 2;
                            } else {
                                ((ComboBox)kvp.Value.Item1).SelectedIndex = 0;
                            }
                        break;
                        case "Color":
                            if (propertyValue != null && ((SettingsType.ColorType == "0x" && propertyValue.Length == 8) || (propertyValue != null && SettingsType.ColorType == "#" && propertyValue.Length == 7 && propertyValue.StartsWith("#"))))
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
                            kvp.Value.Item1.Text = propertyValue;
                        break;
                        case "String":
                            kvp.Value.Item1.Text = propertyValue;
                        break;
                        case "ResourceQueue":
                            ((ListBox)kvp.Value.Item1).Items.Clear();
                            ((ListBox)kvp.Value.Item1).SelectedIndex = -1;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                            ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                            WorldConfig1.SetProperty(kvp.Key, null, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
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
                    kvp.Value.Item2.Checked = WorldConfigDefaultValues.Properties.First(a => a.PropertyName == kvp.Key.Name).Override;
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
                            ((ComboBox)kvp.Value.Item1).SelectedIndex = 0;
                        break;
                        case "Color":
                            kvp.Value.Item5.BackColor = Color.White;
                            kvp.Value.Item1.Text = "";
                        break;
                        case "Float":
                            kvp.Value.Item1.Text = "";
                        break;
                        case "String":
                            kvp.Value.Item1.Text = "";
                        break;
                        case "ResourceQueue":
                            ((ListBox)kvp.Value.Item1).Items.Clear();
                            ((ListBox)kvp.Value.Item1).SelectedIndex = -1;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                            ((RadioButton)kvp.Value.Item6.Controls.Find("Override", true)[0]).Checked = true;
                            WorldConfig1.SetProperty(kvp.Key, null, false, false);
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
                if (propertyValue == null || string.IsNullOrEmpty(propertyValue.Trim()))
                {
                    return;
                }

                ListBox lb = ((ListBox)WorldSettingsInputs[property].Item1);
                List<string> newPropertyValue = new List<string>();
                foreach (string item in lb.Items)
                {
                    newPropertyValue.Add(item.Trim());
                }

                bool bAllowed = false;
                if (newPropertyValue != null)
                {
                    bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)propertyValue) || (propertyValue.StartsWith("CustomObject(") && MessageBox.Show("An item with the value \"" + propertyValue + "\" already exists, are you sure you want to add another?", "Allow duplicate?", MessageBoxButtons.OKCancel) == DialogResult.OK);
                    if (duplicatePermission)
                    {
                        if (property.Name == "Mob spawning")
                        {
                            bAllowed = true;
                        } else {
                            if (propertyValue.Contains('(') && propertyValue.Contains(')'))
                            {
                                bool bFound3 = false;
                                ResourceQueueItem selectedOption = null;
                                foreach (ResourceQueueItem option in VersionConfig.ResourceQueueOptions)
                                {
                                    if (propertyValue.StartsWith(option.Name))
                                    {
                                        bFound3 = true;
                                        selectedOption = option;
                                        break;
                                    }
                                }
                                if (bFound3)
                                {
                                    if (selectedOption.IsUnique)
                                    {
                                        List<string> possibleDuplicates = new List<string>();
                                        foreach (string value2 in newPropertyValue)
                                        {
                                            if (value2.StartsWith(selectedOption.Name))
                                            {
                                                if (!selectedOption.HasUniqueParameter) // Is duplicate tag, but not necessarily same params
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
                                            //if (selectedOption.HasUniqueParameter)
                                            //{
                                                bool bFound4 = false;
                                                string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                foreach (string existingValue in possibleDuplicates)
                                                {
                                                    string[] existingParameters = existingValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                    if (existingParameters.Length > selectedOption.UniqueParameterIndex && newParameters.Length > selectedOption.UniqueParameterIndex)
                                                    {
                                                        if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                                        {
                                                            if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(newParameters[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                            {
                                                                if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                                {
                                                                    //bFound4 = true;
                                                                    //Delete old add new
                                                                    DeleteResourceQueueItemWorld(property, existingValue);
                                                                }
                                                            }
                                                        } else {
                                                            if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                            {
                                                                //bFound4 = true;
                                                                //Delete old add new
                                                                DeleteResourceQueueItemWorld(property, existingValue);
                                                            }
                                                        }
                                                    }
                                                }
                                                //if (!bFound4)
                                                //{
                                                    //bAllowed = true;
                                                //} else {
                                                    if (newParameters.Length > selectedOption.UniqueParameterIndex)
                                                    {
                                                        bAllowed = true;
                                                        //if (showDuplicateWarnings)
                                                        //{
                                                            //MessageBox.Show("Cannot add item. An item for property \"" + selectedOption.Name + "\" with parameter " + parameters2[selectedOption.UniqueParameterIndex] + " already exists and \"" + selectedOption.Name + parameters2[selectedOption.UniqueParameterIndex] + "\" must be unique.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.", "Error: Illegal input");
                                                        //}
                                                    } else {
                                                        MessageBox.Show("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
                                                    }
                                                //}
                                            //} else {
                                                //if (showDuplicateWarnings)
                                                //{
                                                    //MessageBox.Show("Cannot add item. An item for property \"" + selectedOption.Name + "\" already exists and \"" + selectedOption.Name + "\" must be unique.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.", "Error: Illegal input");
                                                //}
                                            //}
                                        } else {
                                            bAllowed = true;
                                        }
                                    } else {
                                        bAllowed = true;
                                    }
                                } else {
                                    string[] propertyNames = VersionConfig.ResourceQueueOptions.Select(a => a.Name).ToArray();
                                    string sPropertyNames = "";
                                    foreach (string a in propertyNames)
                                    {
                                        sPropertyNames += a + "\r\n";
                                    }
                                    MessageBox.Show("Cannot add item. property name was not recognized. Legal property names are: \r\n" + sPropertyNames, "Error: Illegal input");
                                }
                            } else {
                                MessageBox.Show("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is PropertyName(Parameters)", "Error: Illegal input");
                            }
                        }
                    } else {
                        if (!propertyValue.StartsWith("CustomObject("))
                        {
                            if (showDuplicateWarnings)
                            {
                                MessageBox.Show("Cannot add item. An item with the value \"" + propertyValue + "\" already exists.", "Error: Illegal input");
                            }
                        }
                    }
                } else {
                    bAllowed = true;
                }
                if (bAllowed)
                {
                    IgnoreOverrideCheckChangedWorld = true;

                    string s = !String.IsNullOrEmpty(WorldConfig1.GetPropertyValueAsString(property)) ? WorldConfig1.GetPropertyValueAsString(property) + "\r\n" + propertyValue.Trim() : propertyValue.Trim();
                    if (WorldConfigDefaultValues != null)
                    {
                        bool bIsDefault = true;
                        if (WorldConfig1.GetPropertyValueAsString(property) == null)
                        {
                            s = WorldConfigDefaultValues.GetPropertyValueAsString(property) + "\r\n" + propertyValue.Trim();
                            bIsDefault = false;
                        } else {
                            bIsDefault = CompareResourceQueues(s, WorldConfigDefaultValues.GetPropertyValueAsString(property));
                        }

                        if (!bIsDefault)
                        {
                            WorldConfig1.SetProperty(property, s, WorldSettingsInputs[property].Item6 != null && ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, WorldSettingsInputs[property].Item6 != null && ((CheckBox)WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                            WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = true;
                            WorldSettingsInputs[property].Item2.Checked = true;
                        } else {
                            WorldConfig1.SetProperty(property, null, WorldSettingsInputs[property].Item6 != null && ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, WorldSettingsInputs[property].Item6 != null && ((CheckBox)WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                            WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = false;
                            WorldSettingsInputs[property].Item2.Checked = false;
                        }
                    } else {
                        WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = true;
                        WorldSettingsInputs[property].Item2.Checked = true;
                        WorldConfig1.SetProperty(property, s, WorldSettingsInputs[property].Item6 != null && ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, WorldSettingsInputs[property].Item6 != null && ((CheckBox)WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    }
                    WorldConfig1.SetProperty(property, s.Trim(), WorldSettingsInputs[property].Item6 != null && ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, WorldSettingsInputs[property].Item6 != null && ((CheckBox)WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    ((ListBox)WorldSettingsInputs[property].Item1).Items.Clear();
                    string[] resourceQueueItemNames = WorldConfig1.GetPropertyValueAsString(property).Replace("\r", "").Split('\n');
                    foreach (string resourceQueueItemName in resourceQueueItemNames)
                    {
                        if (!String.IsNullOrEmpty(resourceQueueItemName))
                        {
                            ((ListBox)WorldSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                        }
                    }
                    IgnoreOverrideCheckChangedWorld = false;
                }
            }

            void btEditResourceQueueItemWorld_Click(object sender, EventArgs e)
            {
                bool showDuplicateWarnings = true;

                TCProperty property = ResourceQueueInputs[sender];
                ListBox lb = ((ListBox)WorldSettingsInputs[property].Item1);
                string propertyValue = (string)lb.SelectedItem;
                if (PopUpForm.InputBox("Edit item", "", ref propertyValue, true) == DialogResult.OK)
                {
                    if (propertyValue != null && !String.IsNullOrEmpty(propertyValue.Trim()))
                    {
                        if (lb.SelectedItem != null)
                        {
                            List<string> newPropertyValue = new List<string>();
                            foreach (string item in lb.Items)
                            {
                                if (item != (string)lb.SelectedItem)
                                {
                                    newPropertyValue.Add(item.Trim());
                                }
                            }

                            bool bAllowed = false;
                            if (newPropertyValue != null)
                            {
                                bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)propertyValue) || (propertyValue.StartsWith("CustomObject(") && MessageBox.Show("An item with the value \"" + propertyValue + "\" already exists, are you sure you want to add another?", "Allow duplicate?", MessageBoxButtons.OKCancel) == DialogResult.OK);
                                if (duplicatePermission)
                                {
                                    if (property.Name == "Mob spawning")
                                    {
                                        bAllowed = true;
                                    } else {
                                        if (propertyValue.Contains('(') && propertyValue.Contains(')'))
                                        {
                                            bool bFound3 = false;
                                            ResourceQueueItem selectedOption = null;
                                            foreach (ResourceQueueItem option in VersionConfig.ResourceQueueOptions)
                                            {
                                                if (propertyValue.StartsWith(option.Name))
                                                {
                                                    bFound3 = true;
                                                    selectedOption = option;
                                                    break;
                                                }
                                            }
                                            if (bFound3)
                                            {
                                                if (selectedOption.IsUnique)
                                                {
                                                    List<string> possibleDuplicates = new List<string>();
                                                    foreach (string value2 in newPropertyValue)
                                                    {
                                                        if (value2.StartsWith(selectedOption.Name))
                                                        {                                                            
                                                            if (!selectedOption.HasUniqueParameter) // Is duplicate tag, but not necessarily same params
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
                                                        //if (selectedOption.HasUniqueParameter)
                                                        //{
                                                            bool bFound4 = false;
                                                            string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                            foreach (string existingValue in possibleDuplicates)
                                                            {
                                                                string[] existingParameters = existingValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                                if (existingParameters.Length > selectedOption.UniqueParameterIndex && newParameters.Length > selectedOption.UniqueParameterIndex)
                                                                {
                                                                    if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                                                    {
                                                                        if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(newParameters[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                                        {
                                                                            if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                                            {
                                                                                //bFound4 = true;
                                                                                //Delete old add new
                                                                                //lb.Items.Remove(existingValue);
                                                                                DeleteResourceQueueItemWorld(property, existingValue);
                                                                            }
                                                                        }
                                                                    } else {
                                                                        if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                                        {
                                                                            //bFound4 = true;
                                                                            //Delete old add new
                                                                            DeleteResourceQueueItemWorld(property, existingValue);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            //if (!bFound4)
                                                            //{
                                                                //bAllowed = true;
                                                            //} //else {
                                                                if (newParameters.Length > selectedOption.UniqueParameterIndex)
                                                                {
                                                                    bAllowed = true;
                                                                    //if (showDuplicateWarnings)
                                                                    //{
                                                                        //MessageBox.Show("Cannot add item. An item for property \"" + selectedOption.Name + "\" with parameter " + parameters2[selectedOption.UniqueParameterIndex] + " already exists and \"" + selectedOption.Name + parameters2[selectedOption.UniqueParameterIndex] + "\" must be unique.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.", "Error: Illegal input");
                                                                    //}
                                                                } else {
                                                                    MessageBox.Show("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
                                                                }
                                                            //}
                                                        //}// else {
                                                            //if (showDuplicateWarnings)
                                                            //{
                                                                //MessageBox.Show("Cannot add item. An item for property \"" + selectedOption.Name + "\" already exists and \"" + selectedOption.Name + "\" must be unique.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.", "Error: Illegal input");
                                                            //}
                                                        //}
                                                    } else {
                                                        bAllowed = true;
                                                    }
                                                } else {
                                                    bAllowed = true;
                                                }
                                            } else {
                                                string[] propertyNames = VersionConfig.ResourceQueueOptions.Select(a => a.Name).ToArray();
                                                string sPropertyNames = "";
                                                foreach (string a in propertyNames)
                                                {
                                                    sPropertyNames += a + "\r\n";
                                                }
                                                MessageBox.Show("Cannot add item. property name was not recognized. Legal property names are: \r\n" + sPropertyNames, "Error: Illegal input");
                                            }
                                        } else {
                                            MessageBox.Show("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is PropertyName(Parameters)", "Error: Illegal input");
                                        }
                                    }
                                } else {
                                    if (!propertyValue.StartsWith("CustomObject("))
                                    {
                                        if (showDuplicateWarnings)
                                        {
                                            MessageBox.Show("Cannot add item. An item with the value \"" + propertyValue + "\" already exists.", "Error: Illegal input");
                                        }
                                    }
                                }
                            } else {
                                bAllowed = true;
                            }
                            if (bAllowed)
                            {
                                IgnoreOverrideCheckChangedWorld = true;
                                string s = WorldConfig1.GetPropertyValueAsString(property) ?? (WorldConfigDefaultValues != null && WorldConfigDefaultValues.GetPropertyValueAsString(property) != null ? WorldConfigDefaultValues.GetPropertyValueAsString(property) : "");
                                if (((string)lb.SelectedItem).Length > 0)
                                {
                                    if (s.IndexOf("\r\n" + (string)lb.SelectedItem + "\r\n") > -1) { s = (s.Replace("\r\n" + (string)lb.SelectedItem + "\r\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\r\n" + (string)lb.SelectedItem + "\n") > -1) { s = (s.Replace("\r\n" + (string)lb.SelectedItem + "\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\n" + (string)lb.SelectedItem + "\r\n") > -1) { s = (s.Replace("\n" + (string)lb.SelectedItem + "\r\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\n" + (string)lb.SelectedItem + "\n") > -1) { s = (s.Replace("\n" + (string)lb.SelectedItem + "\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf((string)lb.SelectedItem + "\r\n") > -1) { s = (s.Replace((string)lb.SelectedItem + "\r\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf((string)lb.SelectedItem + "\r\n") > -1) { s = (s.Replace((string)lb.SelectedItem + "\r\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf((string)lb.SelectedItem + "\n") > -1) { s = (s.Replace((string)lb.SelectedItem + "\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\r\n" + (string)lb.SelectedItem) > -1) { s = (s.Replace("\r\n" + (string)lb.SelectedItem, "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\n" + (string)lb.SelectedItem) > -1) { s = (s.Replace("\n" + (string)lb.SelectedItem, "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf((string)lb.SelectedItem) > -1) { s = (s.Replace((string)lb.SelectedItem, "\r\n" + propertyValue + "\r\n")).Trim(); }
                                }

                                bool bIsDefault = CompareResourceQueues(s, WorldConfigDefaultValues != null && WorldConfigDefaultValues.GetPropertyValueAsString(property) != null ? WorldConfigDefaultValues.GetPropertyValueAsString(property) : "");

                                if (!bIsDefault)
                                {
                                    WorldConfig1.SetProperty(property, s, WorldSettingsInputs[property].Item6 != null && ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, WorldSettingsInputs[property].Item6 != null && ((CheckBox)WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                                    WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = true;
                                    WorldSettingsInputs[property].Item2.Checked = true;
                                } else {
                                    WorldConfig1.SetProperty(property, null, WorldSettingsInputs[property].Item6 != null && ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, WorldSettingsInputs[property].Item6 != null && ((CheckBox)WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                                    WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = false;
                                    WorldSettingsInputs[property].Item2.Checked = false;
                                }

                                ((ListBox)WorldSettingsInputs[property].Item1).Items.Clear();
                                string[] resourceQueueItemNames = s.Replace("\r", "").Split('\n');
                                foreach (string resourceQueueItemName in resourceQueueItemNames)
                                {
                                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                                    {
                                        ((ListBox)WorldSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                                    }
                                }
                                IgnoreOverrideCheckChangedWorld = false;
                            }
                        }
                    }
                }
            }

            void btDeleteResourceQueueItemWorld_Click(object sender, EventArgs e)
            {
                TCProperty property = ResourceQueueInputs[sender];
                ListBox lb = ((ListBox)WorldSettingsInputs[property].Item1);
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
                IgnoreOverrideCheckChangedWorld = true;

                string s = WorldConfig1.GetPropertyValueAsString(property) ?? (WorldConfigDefaultValues != null && WorldConfigDefaultValues.GetPropertyValueAsString(property) != null ? WorldConfigDefaultValues.GetPropertyValueAsString(property) : "");
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

                bool bIsDefault = CompareResourceQueues(s, WorldConfigDefaultValues != null && WorldConfigDefaultValues.GetPropertyValueAsString(property) != null ? WorldConfigDefaultValues.GetPropertyValueAsString(property) : "");
                if (!bIsDefault)
                {
                    WorldConfig1.SetProperty(property, s, WorldSettingsInputs[property].Item6 != null && ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, WorldSettingsInputs[property].Item6 != null && ((CheckBox)WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = true;
                    WorldSettingsInputs[property].Item2.Checked = true;
                } else {
                    WorldConfig1.SetProperty(property, null, WorldSettingsInputs[property].Item6 != null && ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, WorldSettingsInputs[property].Item6 != null && ((CheckBox)WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override = false;
                    WorldSettingsInputs[property].Item2.Checked = false;
                }

                ((ListBox)WorldSettingsInputs[property].Item1).Items.Clear();
                string[] resourceQueueItemNames3 = s.Replace("\r", "").Split('\n');
                foreach (string resourceQueueItemName in resourceQueueItemNames3)
                {
                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                    {
                        ((ListBox)WorldSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                    }
                }
                IgnoreOverrideCheckChangedWorld = false;
            }

            void btWorldSettingsSetToDefault_Click(object sender, EventArgs e)
            {
                if (WorldConfigDefaultValues != null)
                {
                    foreach (TCProperty property in VersionConfig.WorldConfig)
                    {
                        Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = WorldSettingsInputs[property];
                        IgnorePropertyInputChangedWorld = true;
                        IgnoreOverrideCheckChangedWorld = true;

                        string propertyValue = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                        switch (property.PropertyType)
                        {
                            case "BiomesList":
                                ((ListBox)WorldSettingsInputs[property].Item1).SelectedItems.Clear();
                                string[] biomeNames = propertyValue.Split(',');
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                for (int k = 0; k < biomeNames.Length; k++)
                                {
                                    if (BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                    {
                                        for (int l = 0; l < ((ListBox)WorldSettingsInputs[property].Item1).Items.Count; l++)
                                        {
                                            if (((string)((ListBox)WorldSettingsInputs[property].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                            {
                                                ((ListBox)WorldSettingsInputs[property].Item1).SelectedItems.Add(((ListBox)WorldSettingsInputs[property].Item1).Items[l]);
                                            }
                                        }
                                    }
                                }
                                break;
                            case "Bool":
                                if (propertyValue != null && propertyValue.ToLower() == "true")
                                {
                                    ((ComboBox)boxes.Item1).SelectedIndex = 1;
                                }
                                else if (propertyValue != null && propertyValue.ToLower() == "false")
                                {
                                    ((ComboBox)boxes.Item1).SelectedIndex = 2;
                                } else {
                                    ((ComboBox)boxes.Item1).SelectedIndex = 0;
                                }
                                break;
                            case "Color":
                                if ((propertyValue != null && SettingsType.ColorType == "0x" && propertyValue.Length == 8) || (propertyValue != null && SettingsType.ColorType == "#" && propertyValue.Length == 7 && propertyValue.StartsWith("#")))
                                {
                                    boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                    boxes.Item1.Text = propertyValue;
                                } else {
                                    boxes.Item5.BackColor = Color.White;
                                    boxes.Item1.Text = "";
                                }
                                break;
                            case "Float":
                                boxes.Item1.Text = propertyValue;
                                break;
                            case "String":
                                boxes.Item1.Text = propertyValue;
                                break;
                            case "ResourceQueue":
                                ((ListBox)boxes.Item1).Items.Clear();
                                ((ListBox)boxes.Item1).SelectedIndex = -1;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((CheckBox)boxes.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                                WorldConfig1.SetProperty(property, null, false, false);
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

                        IgnorePropertyInputChangedWorld = false;
                        IgnoreOverrideCheckChangedWorld = false;
                    }
                    WorldConfig1 = new WorldConfig(VersionConfig);
                } else {
                    foreach (TCProperty property in VersionConfig.WorldConfig)
                    {
                        Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = WorldSettingsInputs[property];
                        IgnorePropertyInputChangedWorld = true;
                        IgnoreOverrideCheckChangedWorld = true;
                   
                        switch (property.PropertyType)
                        {
                            case "BiomesList":
                                ((ListBox)boxes.Item1).SelectedIndices.Clear();
                                ((ListBox)boxes.Item1).SelectedIndex = -1;
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                break;
                            case "Bool":
                                ((ComboBox)boxes.Item1).SelectedIndex = 0;
                                break;
                            case "Color":
                                boxes.Item5.BackColor = Color.White;
                                boxes.Item1.Text = "";
                                break;
                            case "Float":
                                boxes.Item1.Text = "";
                                break;
                            case "String":
                                boxes.Item1.Text = "";
                                break;
                            case "ResourceQueue":
                                ((ListBox)boxes.Item1).Items.Clear();
                                ((ListBox)boxes.Item1).SelectedIndex = -1;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((CheckBox)boxes.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                                WorldConfig1.SetProperty(property, null, false, false);
                                break;
                        }
                        boxes.Item2.Checked = false;

                        IgnorePropertyInputChangedWorld = false;
                        IgnoreOverrideCheckChangedWorld = false;
                    }
                }
            }

        #endregion

        #region Biomes

            List<Group> BiomeGroups = new List<Group>();
            List<BiomeConfig> BiomeConfigsDefaultValues = new List<BiomeConfig>();
            Dictionary<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> BiomeSettingsInputs = new Dictionary<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>>();

            void LoadBiomesList()
            {
                if (!String.IsNullOrEmpty(SourceConfigsDir) && System.IO.Directory.Exists(SourceConfigsDir + "/" + "WorldBiomes" + "/"))
                {
                    System.IO.DirectoryInfo defaultWorldDirectory = new System.IO.DirectoryInfo(SourceConfigsDir + "/" + "WorldBiomes" + "/");
                    lbBiomes.Items.Clear();
                    BiomeConfigsDefaultValues.Clear();

                    foreach (System.IO.FileInfo file in defaultWorldDirectory.GetFiles())
                    {
                        if (file.Name.EndsWith(".bc"))
                        {
                            lbBiomes.Items.Add(file.Name.Replace(".bc", ""));                            
                            BiomeConfig bDefaultConfig = LoadBiomeConfigFromFile(file, VersionConfig, true);
                            BiomeConfigsDefaultValues.Add(bDefaultConfig);
                        }
                    }
                }
            }

            BiomeConfig LoadBiomeConfigFromFile(FileInfo file, VersionConfig versionConfig, bool loadComments)
            {
                BiomeConfig bDefaultConfig = new BiomeConfig(versionConfig)
                {
                    FileName = file.Name
                };

                string sDefaultText = System.IO.File.ReadAllText(file.FullName);
                foreach (TCProperty property in versionConfig.BiomeConfig)
                {
                    string propertyValue = "";
                    if (property.PropertyType == "ResourceQueue")
                    {
                        bool bFound = false;
                        int replaceStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                        if (replaceStartIndex > -1)
                        {
                            while (!bFound)
                            {
                                int lineStart = sDefaultText.Substring(replaceStartIndex).IndexOf("\n") + 1 + replaceStartIndex;
                                int lineLength = sDefaultText.Substring(lineStart).IndexOf("\n") + 1;

                                if (
                                    /* line doesnt contain # */
                                    !sDefaultText.Substring(lineStart, lineLength).Contains("#") &&
                                    (
                                        /* and line isnt empty or is empty and is last line or next line starts with ## */
                                        sDefaultText.Substring(lineStart, lineLength).Replace("\r", "").Replace("\n", "").Trim().Length > 0 ||
                                        (
                                            //if this is the last line
                                            sDefaultText.Substring(lineStart + lineLength).IndexOf("\n") < 0 ||
                                            //or next line starts with ##
                                            (sDefaultText.Substring(lineStart + lineLength).IndexOf("##") > -1 && sDefaultText.Substring(lineStart + lineLength).IndexOf("##") < sDefaultText.Substring(lineStart + lineLength).IndexOf("\n"))
                                        )
                                    )
                                )
                                {
                                    replaceStartIndex = lineStart;
                                    bFound = true;
                                } else {
                                    replaceStartIndex = sDefaultText.Substring(replaceStartIndex).IndexOf("\n") + 1 + replaceStartIndex;
                                    if (replaceStartIndex < 0 || replaceStartIndex == sDefaultText.Length)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (replaceStartIndex > -1)
                            {
                                int replaceLength = 0;

                                while (replaceStartIndex + replaceLength + 2 <= sDefaultText.Length && sDefaultText.Substring(replaceStartIndex + replaceLength, 2) != "\n#")
                                {
                                    replaceLength += 1;
                                }
                                if (replaceStartIndex + replaceLength + 1 == sDefaultText.Length)
                                {
                                    replaceLength += 1;
                                }

                                // Get comments above property for tooltips
                                // Get all lines above propertyValue before encountering a line starting with multiple '#'s or another propertyvalue
                                int commentStartIndex = -1;
                                int commentEndIndex = replaceStartIndex - 1;
                                int lastNoHashLineBeforeComment = sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("\n");
                                if (lastNoHashLineBeforeComment > -1)
                                {
                                    while (true)
                                    {
                                        if (lastNoHashLineBeforeComment > 0)
                                        {
                                            int arg = sDefaultText.Substring(0, lastNoHashLineBeforeComment - 1).LastIndexOf("\n");
                                            if (arg != -1)
                                            {
                                                string s = sDefaultText.Substring(arg, lastNoHashLineBeforeComment - arg);
                                                if (!String.IsNullOrWhiteSpace(s) && !s.Replace("\n", "").Replace("\r", "").StartsWith("#"))
                                                {
                                                    break;
                                                } else {
                                                    lastNoHashLineBeforeComment = arg;
                                                }
                                            } else {
                                                lastNoHashLineBeforeComment = -1;
                                                break;
                                            }
                                        } else {
                                            break;
                                        }
                                    }
                                }
                                int lastDoubleHashLineBeforeComment = sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("##\n") > sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("##\r\n") ? sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("##\n") + 3 : sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("##\r\n") + 4;
                                if (lastDoubleHashLineBeforeComment > -1 || lastNoHashLineBeforeComment > -1)
                                {
                                    commentStartIndex = lastDoubleHashLineBeforeComment > lastNoHashLineBeforeComment ? lastDoubleHashLineBeforeComment : lastNoHashLineBeforeComment;
                                }
                                string comment = "";
                                if (commentStartIndex > -1)
                                {
                                    comment = sDefaultText.Substring(commentStartIndex, commentEndIndex - commentStartIndex).Trim();
                                }
                                if (property.PropertyType == "BiomesList")
                                {
                                    comment += "\r\n\r\nUse CTRL or SHIFT + Click to select one or multiple biomes from the list.";
                                }
                                if (loadComments && !String.IsNullOrEmpty(comment))
                                {
                                    ToolTip1.SetToolTip(BiomeSettingsInputs[property].Item4, comment);
                                }

                                propertyValue = sDefaultText.Substring(replaceStartIndex, replaceLength).Trim();
                                bDefaultConfig.SetProperty(property, propertyValue, false, false);
                            } else {
                                throw new Exception("Property value for property " + property.Name + " could not be read from file " + file.Name + ". There is either an error in the file or TCEE needs to be updated to support the value's formatting.");
                            }
                        } else {
                            throw new Exception("ScriptHandle for property \"" + property.Name + "\" could not be found in biome file " + file.Name);
                        }
                    } else {
                        bool bFound = false;
                        int valueStringStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                        if (valueStringStartIndex > -1)
                        {
                            while (!bFound)
                            {
                                if (sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("\n") > sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("#"))
                                {
                                    bFound = true;
                                } else {
                                    valueStringStartIndex = sDefaultText.Substring(valueStringStartIndex + property.ScriptHandle.Length).ToLower().IndexOf(property.ScriptHandle.ToLower()) + valueStringStartIndex + property.ScriptHandle.Length;
                                    if (valueStringStartIndex < 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            if (valueStringStartIndex > -1)
                            {
                                // Get comments above property for tooltips
                                // Get all lines above propertyValue before encountering a line starting with multiple '#'s or another propertyvalue
                                int commentStartIndex = -1;
                                int commentEndIndex = valueStringStartIndex - 1;
                                int lastNoHashLineBeforeComment = sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("\n");
                                if (lastNoHashLineBeforeComment > -1)
                                {
                                    while (true)
                                    {
                                        if (lastNoHashLineBeforeComment > 0)
                                        {
                                            int arg = sDefaultText.Substring(0, lastNoHashLineBeforeComment - 1).LastIndexOf("\n");
                                            if (arg != -1)
                                            {
                                                string s = sDefaultText.Substring(arg, lastNoHashLineBeforeComment - arg);
                                                if (!String.IsNullOrWhiteSpace(s) && !s.Replace("\n", "").Replace("\r", "").StartsWith("#"))
                                                {
                                                    break;
                                                } else {
                                                    lastNoHashLineBeforeComment = arg;
                                                }
                                            } else {
                                                lastNoHashLineBeforeComment = -1;
                                                break;
                                            }
                                        } else {
                                            break;
                                        }
                                    }
                                }
                                int lastDoubleHashLineBeforeComment = sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("##\n") > sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("##\r\n") ? sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("##\n") + 3 : sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("##\r\n") + 4;
                                if (lastDoubleHashLineBeforeComment > -1 || lastNoHashLineBeforeComment > -1)
                                {
                                    commentStartIndex = lastDoubleHashLineBeforeComment > lastNoHashLineBeforeComment ? lastDoubleHashLineBeforeComment : lastNoHashLineBeforeComment;
                                }
                                string comment = "";
                                if (commentStartIndex > -1)
                                {
                                    comment = sDefaultText.Substring(commentStartIndex, commentEndIndex - commentStartIndex).Trim();
                                }
                                if (property.PropertyType == "BiomesList")
                                {
                                    comment += "\r\n\r\nUse CTRL or SHIFT + Click to select one or multiple biomes from the list.";
                                }
                                if (loadComments && !String.IsNullOrEmpty(comment))
                                {
                                    ToolTip1.SetToolTip(BiomeSettingsInputs[property].Item4, comment);
                                }

                                int skipCharsLength = (property.ScriptHandle).Length;
                                int valueStringLength = 0;
                                //float originalValue = 0;

                                while (sDefaultText.Substring(valueStringStartIndex + skipCharsLength + valueStringLength, 1) != "\n")
                                {
                                    valueStringLength += 1;
                                }
                                propertyValue = sDefaultText.Substring(valueStringStartIndex + skipCharsLength, valueStringLength).Trim();
                                bDefaultConfig.SetProperty(property, propertyValue, false, false);
                            } else {
                                throw new Exception("Property value for property " + property.Name + " could not be read from file " + file.Name + ". There is either an error in the file or TCEE needs to be updated to support the value's formatting.");
                            }
                        } else {

                            if (loadComments)
                            {
                                ToolTip1.SetToolTip(BiomeSettingsInputs[property].Item4, "No tooltip available, comments for this setting are missing in source files.");
                            }
                            bDefaultConfig.SetProperty(property, "", false, false);

                            //TODO: solve ReplaceToBiomeName problem!
                            if (property.Name != "ReplaceToBiomeName" && !property.Optional)// && property.Name != "BiomeSizeWhenIsle" && property.Name != "BiomeRarityWhenIsle" && property.Name != "BiomeSizeWhenBorder")
                            {
                                MessageBox.Show("ScriptHandle for property \"" + property.Name + "\" could not be found in biome file " + file.Name, "Error reading configuration files");
                                MessageBox.Show("The files you are trying to import have caused an error. They were probably not generated by the selected version of TC or MCW. MCW and TC are backwards compatible though, so for instance you can use TC2.6.3 settings (MCW 1.0.5. and lower) in TC2.7.2 (MCW 1.0.6. or higher). When TC or MCW detects old world and biomeconfigs it should update the files automatically. You can then import the generated world into TCEE using the new version.", "Error reading configuration files");
                                throw new InvalidDataException("ScriptHandle for property \"" + property.Name + "\" could not be found in biome file " + file.Name);
                            }
                        }
                    }
                }
                return bDefaultConfig;
            }

            void LoadDefaultGroups()
            {
                if (BiomeConfigsDefaultValues.Any())
                {
                    lbGroups.Items.Add("Land");
                    Group overworldLand = new Group("Land", VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName != "Hell" && biomeConfig.BiomeName != "Sky" && !biomeConfig.BiomeName.ToLower().Contains("ocean"))
                        {
                            overworldLand.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    BiomeGroups.Add(overworldLand);

                    lbGroups.Items.Add("Oceans");
                    Group overworldOceans = new Group("Oceans", VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName.ToLower().Contains("ocean"))
                        {
                            overworldOceans.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    BiomeGroups.Add(overworldOceans);

                    lbGroups.Items.Add("Sky");
                    Group overworldSky = new Group("Sky", VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName == "Sky")
                        {
                            overworldSky.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    BiomeGroups.Add(overworldSky);

                    lbGroups.Items.Add("Nether");
                    Group hell = new Group("Nether", VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName == "Hell")
                        {
                            hell.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    BiomeGroups.Add(hell);
                }
            }

            private void btOverrideParentValuesBiome_CheckedChanged(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedBiome)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item6 == ((Control)sender).Parent);
                    TCProperty property = kvp.Key;

                    Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;

                    biomeConfig.SetProperty(property, biomeConfig.GetPropertyValueAsString(property), biomeConfig.GetPropertyMerge(property), ((CheckBox)sender).Checked);
                }
            }

            private void btOverrideAllBiome_CheckedChanged(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedBiome && ((RadioButton)sender).Checked)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item6 == ((Control)sender).Parent);
                    Control tb = kvp.Value.Item1;
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
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
                            (!String.IsNullOrEmpty(biomeConfig.GetPropertyValueAsString(property)) && !CompareBiomeLists(biomeConfig.GetPropertyValueAsString(property), biomeDefaultConfig.GetPropertyValueAsString(property))) || 
                            (biomeConfig.GetPropertyValueAsString(property) != null && !CompareBiomeLists(biomeConfig.GetPropertyValueAsString(property), biomeDefaultConfig.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            cb.Checked = false;
                        }
                    }
                    else if (property.PropertyType == "ResourceQueue")
                    {
                        if ((biomeDefaultConfig == null && !String.IsNullOrEmpty(biomeConfig.GetPropertyValueAsString(property))) || (!String.IsNullOrEmpty(biomeConfig.GetPropertyValueAsString(property)) && !CompareResourceQueues(biomeConfig.GetPropertyValueAsString(property), biomeDefaultConfig.GetPropertyValueAsString(property))) || (biomeConfig.GetPropertyValueAsString(property) != null && !CompareResourceQueues(biomeConfig.GetPropertyValueAsString(property), biomeDefaultConfig.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            cb.Checked = false;
                        }
                    }
                }
            }

            private void lbPropertyInputBiome_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (!IgnorePropertyInputChangedBiome)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = BiomeSettingsInputs.First(a => a.Value.Item1 == sender);
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
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
                        bIsDefault = CompareBiomeLists(biomeDefaultConfig.GetPropertyValueAsString(property), biomeConfig.GetPropertyValueAsString(property));
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
                    Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;

                    BiomeConfig biomeDefaultConfig = null;
                    if (g.showDefaults)
                    {
                        biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    }

                    if (sender is ListBox)
                    {
                        ColorDialog colorDlg = new ColorDialog();
                        colorDlg.AllowFullOpen = false;
                        colorDlg.AnyColor = true;
                        colorDlg.SolidColorOnly = false;

                        if (colorDlg.ShowDialog() == DialogResult.OK)
                        {
                            KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = BiomeSettingsInputs.First(a => a.Value.Item5 == sender);
                            TCProperty property = kvp.Key;
                            kvp.Value.Item5.BackColor = colorDlg.Color;
                            IgnorePropertyInputChangedBiome = true;
                            IgnoreOverrideCheckChangedBiome = true;
                            if (SettingsType.ColorType == "0x")
                            {
                                kvp.Value.Item1.Text = "0x" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            else if (SettingsType.ColorType == "#")
                            {
                                kvp.Value.Item1.Text = "#" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            biomeConfig.SetProperty(property, kvp.Value.Item1.Text, false, false);

                            if (biomeDefaultConfig == null || kvp.Value.Item1.Text != biomeDefaultConfig.GetPropertyValueAsString(property))
                            {
                                kvp.Value.Item2.Checked = true;
                            } else {
                                kvp.Value.Item2.Checked = false;
                            }
                            IgnorePropertyInputChangedBiome = false;
                            IgnoreOverrideCheckChangedBiome = false;
                        }
                    }
                    else if (sender is TextBox)
                    {
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = BiomeSettingsInputs.First(a => a.Value.Item1 == sender);
                        TCProperty property = kvp.Key;
                        try
                        {
                            if (kvp.Value.Item1.Text.Length == 8 || (kvp.Value.Item1.Text.Length == 7 && kvp.Value.Item1.Text.StartsWith("#")))
                            {
                                BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(kvp.Value.Item1.Text);
                                if (SettingsType.ColorType == "0x")
                                {
                                    kvp.Value.Item1.Text = "0x" + BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                else if (SettingsType.ColorType == "#")
                                {
                                    kvp.Value.Item1.Text = "#" + BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                biomeConfig.SetProperty(property, kvp.Value.Item1.Text, false, false);

                                if (biomeDefaultConfig == null || kvp.Value.Item1.Text != biomeDefaultConfig.GetPropertyValueAsString(property))
                                {
                                    kvp.Value.Item2.Checked = true;
                                } else {
                                    kvp.Value.Item2.Checked = false;
                                }
                                string breakpoint = "";
                            } else {
                                BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                kvp.Value.Item2.Checked = false;
                            }                            
                        }
                        catch (Exception ex)
                        {
                            BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                            kvp.Value.Item2.Checked = false;
                        }
                    }
                }
            }

            bool IgnorePropertyInputChangedBiome = false;
            void PropertyInputChangedBiome(object sender, EventArgs e)
            {
                if(!IgnorePropertyInputChangedBiome)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item1 == sender);
                    Control tb = kvp.Value.Item1;
                    CheckBox cb = kvp.Value.Item2;
                    TCProperty property = kvp.Key;

                    Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;
                    biomeConfig.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    BiomeConfig biomeDefaultConfig = null;
                    if (g.showDefaults)
                    {
                        biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    }

                    float result;
                    if (
                        property.PropertyType == "String" ||
                        property.PropertyType == "Bool" ||
                        (property.PropertyType == "Float" && float.TryParse(tb.Text, out result))
                    )
                    {
                        biomeConfig.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        if (!(property.PropertyType == "Bool" && String.IsNullOrEmpty(tb.Text)) && (biomeDefaultConfig == null || (property.PropertyType != "Bool" && tb.Text != biomeDefaultConfig.GetPropertyValueAsString(property)) || (property.PropertyType == "Bool" && !String.IsNullOrEmpty(tb.Text) && tb.Text != biomeDefaultConfig.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            if (property.PropertyType == "Bool" && String.IsNullOrEmpty(tb.Text) && biomeDefaultConfig != null)
                            {
                                IgnoreOverrideCheckChangedBiome = true;
                                IgnorePropertyInputChangedBiome = true;
                                tb.Text = biomeDefaultConfig.GetPropertyValueAsString(property);
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
                if (BiomeSettingsInputs.Any(a => a.Value.Item5 == sender))
                {
                    sender = BiomeSettingsInputs.First(a => a.Value.Item5 == sender).Value.Item1;
                }

                KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = BiomeSettingsInputs.First(a => a.Value.Item1 == sender);
                TCProperty property = kvp.Key;
                Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
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
                    if (((TextBox)sender).Text.Length == 8 || (((TextBox)sender).Text.Length == 7 && ((TextBox)sender).Text.StartsWith("#")))
                    {
                        try
                        {
                            BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(((TextBox)sender).Text);
                            string value = "";
                            if (SettingsType.ColorType == "0x")
                            {
                                value = "0x" + BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                            }
                            else if (SettingsType.ColorType == "#")
                            {
                                value = "#" + BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                            }
                            BiomeSettingsInputs[property].Item1.Text = value;
                            biomeConfig.SetProperty(property, value, false, false);
                            if (biomeDefaultConfig == null || value.ToUpper() != biomeDefaultConfig.GetPropertyValueAsString(property).ToUpper())
                            {
                                BiomeSettingsInputs[property].Item2.Checked = true;
                            } else {
                                BiomeSettingsInputs[property].Item2.Checked = false;
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
                            if (defaultValue != null && (defaultValue.Length == 8 || (defaultValue.Length == 7 && defaultValue.StartsWith("#"))))
                            {
                                try
                                {
                                    BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(defaultValue);
                                    string value = "";
                                    if (SettingsType.ColorType == "0x")
                                    {
                                        value = "0x" + BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                    }
                                    else if (SettingsType.ColorType == "#")
                                    {
                                        value = "#" + BiomeSettingsInputs[property].Item5.BackColor.R.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.G.ToString("X2") + BiomeSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                    }
                                    BiomeSettingsInputs[property].Item1.Text = value;
                                }
                                catch (Exception ex2)
                                {
                                    BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                    BiomeSettingsInputs[property].Item1.Text = "";
                                }
                                BiomeSettingsInputs[property].Item2.Checked = false;
                            } else {
                                BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                BiomeSettingsInputs[property].Item1.Text = "";
                                BiomeSettingsInputs[property].Item2.Checked = false;
                            }
                        } else {
                            BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                            BiomeSettingsInputs[property].Item1.Text = "";
                            BiomeSettingsInputs[property].Item2.Checked = false;
                        }
                    }
                    IgnorePropertyInputChangedBiome = false;
                    IgnoreOverrideCheckChangedBiome = false;
                }
                else if (property.PropertyType == "Float")
                {
                    float result = 0;
                    if (!float.TryParse(((TextBox)sender).Text, out result))
                    {
                        if (biomeDefaultConfig != null)
                        {
                            ((TextBox)sender).Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                        } else {
                            ((TextBox)sender).Text = "";
                        }
                        BiomeSettingsInputs[property].Item2.Checked = false;
                    }
                }
                else if (String.IsNullOrWhiteSpace(((TextBox)sender).Text) && property.PropertyType != "String")
                {
                    if (biomeDefaultConfig != null)
                    {
                        ((TextBox)sender).Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                    } else {
                        ((TextBox)sender).Text = "";
                    }
                    BiomeSettingsInputs[property].Item2.Checked = false;
                }
            }

            bool IgnoreOverrideCheckChangedBiome = false;
            void PropertyInputOverrideCheckChangedBiome(object sender, EventArgs e)
            {
                if (!IgnoreOverrideCheckChangedBiome)
                {
                    TCProperty property = BiomeSettingsInputs.First(a => a.Value.Item2 == sender).Key;
                    Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                    BiomeConfig biomeConfig = g.BiomeConfig;
                    biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = ((CheckBox)sender).Checked;

                    BiomeConfig biomeDefaultConfig = null;
                    if (g.showDefaults)
                    {
                        biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    }

                    if (((CheckBox)sender).Checked)
                    {
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = BiomeSettingsInputs.First(a => a.Value.Item2 == sender);
                        Control tb = kvp.Value.Item1;
                        CheckBox cb = kvp.Value.Item2;

                        float result;
                        if (
                            property.PropertyType == "String" ||
                            (property.PropertyType == "Bool" && !String.IsNullOrEmpty(tb.Text)) ||
                            (property.PropertyType == "Float" && float.TryParse(tb.Text, out result))
                        )
                        {
                            biomeConfig.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                        else if (property.PropertyType == "Float" || property.PropertyType == "Bool")
                        {
                            IgnoreOverrideCheckChangedBiome = true;
                            IgnorePropertyInputChangedBiome = true;
                            if (biomeDefaultConfig != null && biomeDefaultConfig.GetPropertyValueAsString(property) != null)
                            {
                                tb.Text = biomeDefaultConfig.GetPropertyValueAsString(property);
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
                                if (kvp.Value.Item1.Text.Length == 8 || (kvp.Value.Item1.Text.Length == 7 && kvp.Value.Item1.Text.StartsWith("#")))
                                {
                                    if ((SettingsType.ColorType == "0x" && kvp.Value.Item1.Text.StartsWith("0x")) || (SettingsType.ColorType == "#" && kvp.Value.Item1.Text.StartsWith("#")))
                                    {
                                        biomeConfig.SetProperty(property, kvp.Value.Item1.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                                    } else {
                                        IgnoreOverrideCheckChangedBiome = true;
                                        IgnorePropertyInputChangedBiome = true;
                                        if (biomeDefaultConfig != null && !String.IsNullOrEmpty(biomeDefaultConfig.GetPropertyValueAsString(property)))
                                        {
                                            tb.Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                                            BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(biomeDefaultConfig.GetPropertyValueAsString(property));
                                        } else {
                                            tb.Text = "";
                                            BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                        }
                                        cb.Checked = false;
                                        biomeConfig.SetProperty(property, null, false, false);
                                        IgnoreOverrideCheckChangedBiome = false;
                                        IgnorePropertyInputChangedBiome = false;
                                    }
                                } else {
                                    IgnoreOverrideCheckChangedBiome = true;
                                    IgnorePropertyInputChangedBiome = true;
                                    if (biomeDefaultConfig != null && !String.IsNullOrEmpty(biomeDefaultConfig.GetPropertyValueAsString(property)))
                                    {
                                        tb.Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                                        BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(biomeDefaultConfig.GetPropertyValueAsString(property));
                                    } else {
                                        tb.Text = "";
                                        BiomeSettingsInputs[property].Item5.BackColor = Color.White;
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
                                    BiomeSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(biomeDefaultConfig.GetPropertyValueAsString(property));
                                } else {
                                    tb.Text = "";
                                    BiomeSettingsInputs[property].Item5.BackColor = Color.White;
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

            void bSetDefaultsBiomesProperty(object sender, EventArgs e)
            {
                KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item3 == sender);
                Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
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
                            ((ListBox)BiomeSettingsInputs[kvp.Key].Item1).SelectedItems.Clear();
                            string[] biomeNames = propertyValue.Split(',');
                            for (int k = 0; k < biomeNames.Length; k++)
                            {
                                if (BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                {
                                    for (int l = 0; l < ((ListBox)BiomeSettingsInputs[kvp.Key].Item1).Items.Count; l++)
                                    {
                                        if (((string)((ListBox)BiomeSettingsInputs[kvp.Key].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                        {
                                            ((ListBox)BiomeSettingsInputs[kvp.Key].Item1).SelectedItems.Add(((ListBox)BiomeSettingsInputs[kvp.Key].Item1).Items[l]);
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
                                ((ComboBox)kvp.Value.Item1).SelectedIndex = 1;
                            }
                            else if (propertyValue.ToLower() == "false")
                            {
                                ((ComboBox)kvp.Value.Item1).SelectedIndex = 2;
                            } else {
                                ((ComboBox)kvp.Value.Item1).SelectedIndex = 0;
                            }
                            break;
                        case "Color":
                            if ((propertyValue != null && propertyValue.Length == 8) || (propertyValue != null && propertyValue.Length == 7 && propertyValue.StartsWith("#")))
                            {
                                try
                                {
                                    kvp.Value.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                    string value = "";
                                    if (SettingsType.ColorType == "0x")
                                    {
                                        value = "0x" + kvp.Value.Item5.BackColor.R.ToString("X2") + kvp.Value.Item5.BackColor.G.ToString("X2") + kvp.Value.Item5.BackColor.B.ToString("X2");
                                    }
                                    else if (SettingsType.ColorType == "#")
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
                            kvp.Value.Item1.Text = propertyValue;
                        break;
                        case "String":
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
                            ((ComboBox)kvp.Value.Item1).SelectedIndex = 0;
                            break;
                        case "Color":
                            kvp.Value.Item5.BackColor = Color.White;
                            kvp.Value.Item1.Text = "";
                            break;
                        case "Float":
                            kvp.Value.Item1.Text = "";
                            break;
                        case "String":
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
                if(propertyValue == null || string.IsNullOrEmpty(propertyValue.Trim()))
                {
                    return;
                }

                ListBox lb = ((ListBox)BiomeSettingsInputs[property].Item1);
                List<string> newPropertyValue = new List<string>();
                foreach(string item in lb.Items)
                {
                    newPropertyValue.Add(item.Trim());
                }

                bool bAllowed = false;
                if (newPropertyValue != null)
                {
                    bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)propertyValue) || (propertyValue.StartsWith("CustomObject(") && MessageBox.Show("An item with the value \"" + propertyValue + "\" already exists, are you sure you want to add another?", "Allow duplicate?", MessageBoxButtons.OKCancel) == DialogResult.OK);
                    if (duplicatePermission)
                    {
                        if (property.Name == "Mob spawning")
                        {
                            bAllowed = true;
                        } else {
                            if (propertyValue.Contains('(') && propertyValue.Contains(')'))
                            {
                                bool bFound3 = false;
                                ResourceQueueItem selectedOption = null;
                                foreach (ResourceQueueItem option in VersionConfig.ResourceQueueOptions)
                                {
                                    if (propertyValue.StartsWith(option.Name))
                                    {
                                        bFound3 = true;
                                        selectedOption = option;
                                        break;
                                    }
                                }
                                if (bFound3)
                                {
                                    if (selectedOption.IsUnique)
                                    {
                                        List<string> possibleDuplicates = new List<string>();
                                        foreach (string value2 in newPropertyValue)
                                        {
                                            if (value2.StartsWith(selectedOption.Name))
                                            {
                                                if (!selectedOption.HasUniqueParameter) // Is duplicate tag, but not necessarily same params
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
                                            //if (selectedOption.HasUniqueParameter)
                                            //{
                                                bool bFound4 = false;
                                                string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                foreach (string existingValue in possibleDuplicates)
                                                {
                                                    string[] existingParameters = existingValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                    if (existingParameters.Length > selectedOption.UniqueParameterIndex && newParameters.Length > selectedOption.UniqueParameterIndex)
                                                    {
                                                        if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                                        {
                                                            if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(newParameters[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                            {
                                                                if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                                {
                                                                    //bFound4 = true;
                                                                    //Delete old add new
                                                                    DeleteResourceQueueItem(property, existingValue);
                                                                }
                                                            }
                                                        } else {
                                                            if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                            {
                                                                //bFound4 = true;
                                                                //Delete old add new
                                                                DeleteResourceQueueItem(property, existingValue);
                                                            }
                                                        }
                                                    }
                                                }
                                                //if (!bFound4)
                                                //{
                                                    //bAllowed = true;
                                                //} else {
                                                    if (newParameters.Length > selectedOption.UniqueParameterIndex)
                                                    {
                                                        bAllowed = true;
                                                        //if (showDuplicateWarnings)
                                                        //{
                                                            //MessageBox.Show("Cannot add item. An item for property \"" + selectedOption.Name + "\" with parameter " + parameters2[selectedOption.UniqueParameterIndex] + " already exists and \"" + selectedOption.Name + parameters2[selectedOption.UniqueParameterIndex] + "\" must be unique.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.", "Error: Illegal input");
                                                        //}
                                                    } else {
                                                        MessageBox.Show("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
                                                    }
                                                //}
                                            //} else {
                                                //if (showDuplicateWarnings)
                                                //{
                                                    //MessageBox.Show("Cannot add item. An item for property \"" + selectedOption.Name + "\" already exists and \"" + selectedOption.Name + "\" must be unique.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.", "Error: Illegal input");
                                                //}
                                            //}
                                        } else {
                                            bAllowed = true;
                                        }
                                    } else {
                                        bAllowed = true;
                                    }
                                } else {
                                    string[] propertyNames = VersionConfig.ResourceQueueOptions.Select(a => a.Name).ToArray();
                                    string sPropertyNames = "";
                                    foreach(string a in propertyNames)
                                    {
                                        sPropertyNames += a + "\r\n";
                                    }
                                    MessageBox.Show("Cannot add item. property name was not recognized. Legal property names are: \r\n" + sPropertyNames, "Error: Illegal input");
                                }
                            } else {
                                MessageBox.Show("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is PropertyName(Parameters)", "Error: Illegal input");
                            }
                        }
                    } else {
                        if (!propertyValue.StartsWith("CustomObject("))
                        {
                            if (showDuplicateWarnings)
                            {
                                MessageBox.Show("Cannot add item. An item with the value \"" + propertyValue + "\" already exists.", "Error: Illegal input");
                            }
                        }
                    }
                } else {
                    bAllowed = true;
                }
                if(bAllowed)
                {
                    IgnoreOverrideCheckChangedBiome = true;
                    Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
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
                            bIsDefault = CompareResourceQueues(s, biomeDefaultConfig.GetPropertyValueAsString(property));
                        }

                        if (!bIsDefault)
                        {
                            biomeConfig.SetProperty(property, s, BiomeSettingsInputs[property].Item6 != null && ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, BiomeSettingsInputs[property].Item6 != null && ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                            biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = true;
                            BiomeSettingsInputs[property].Item2.Checked = true;
                        } else {
                            biomeConfig.SetProperty(property, null, BiomeSettingsInputs[property].Item6 != null && ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, BiomeSettingsInputs[property].Item6 != null && ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                            biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = false;
                            BiomeSettingsInputs[property].Item2.Checked = false;
                        }
                    } else {
                        biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = true;
                        BiomeSettingsInputs[property].Item2.Checked = true;
                        biomeConfig.SetProperty(property, s, BiomeSettingsInputs[property].Item6 != null && ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, BiomeSettingsInputs[property].Item6 != null && ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    }
                    biomeConfig.SetProperty(property, s.Trim(), BiomeSettingsInputs[property].Item6 != null && ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, BiomeSettingsInputs[property].Item6 != null && ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                    ((ListBox)BiomeSettingsInputs[property].Item1).Items.Clear();
                    string[] resourceQueueItemNames = biomeConfig.GetPropertyValueAsString(property).Replace("\r", "").Split('\n');
                    foreach (string resourceQueueItemName in resourceQueueItemNames)
                    {
                        if (!String.IsNullOrEmpty(resourceQueueItemName))
                        {
                            ((ListBox)BiomeSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                        }
                    }
                    IgnoreOverrideCheckChangedBiome = false;
                }
            }

            void btEditResourceQueueItem_Click(object sender, EventArgs e)
            {
                bool showDuplicateWarnings = true;
                TCProperty property = ResourceQueueInputs[sender];
                ListBox lb = ((ListBox)BiomeSettingsInputs[property].Item1);
                string propertyValue = (string)lb.SelectedItem;
                if (PopUpForm.InputBox("Edit item", "", ref propertyValue, true) == DialogResult.OK)
                {
                    if(propertyValue != null && !String.IsNullOrEmpty(propertyValue.Trim()))
                    {
                        if (lb.SelectedItem != null)
                        {
                            List<string> newPropertyValue = new List<string>();
                            foreach (string item in lb.Items)
                            {
                                if(item != (string)lb.SelectedItem)
                                {
                                    newPropertyValue.Add(item.Trim());
                                }
                            }

                            bool bAllowed = false;
                            if (newPropertyValue != null)
                            {
                                bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)propertyValue) || (propertyValue.StartsWith("CustomObject(") && MessageBox.Show("An item with the value \"" + propertyValue + "\" already exists, are you sure you want to add another?", "Allow duplicate?", MessageBoxButtons.OKCancel) == DialogResult.OK);
                                if (duplicatePermission)
                                {
                                    if (property.Name == "Mob spawning")
                                    {
                                        bAllowed = true;
                                    } else {
                                        if (propertyValue.Contains('(') && propertyValue.Contains(')'))
                                        {
                                            bool bFound3 = false;
                                            ResourceQueueItem selectedOption = null;
                                            foreach (ResourceQueueItem option in VersionConfig.ResourceQueueOptions)
                                            {
                                                if (propertyValue.StartsWith(option.Name))
                                                {
                                                    bFound3 = true;
                                                    selectedOption = option;
                                                    break;
                                                }
                                            }
                                            if (bFound3)
                                            {
                                                if (selectedOption.IsUnique)
                                                {
                                                    List<string> possibleDuplicates = new List<string>();
                                                    foreach (string value2 in newPropertyValue)
                                                    {
                                                        if (value2.StartsWith(selectedOption.Name))
                                                        {
                                                            if (!selectedOption.HasUniqueParameter) // Is duplicate tag, but not necessarily same params
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
                                                        //if (selectedOption.HasUniqueParameter)
                                                        //{
                                                            bool bFound4 = false;
                                                            string[] newParameters = propertyValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                            foreach (string existingValue in possibleDuplicates)
                                                            {
                                                                string[] existingParameters = existingValue.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                                if (existingParameters.Length > selectedOption.UniqueParameterIndex && newParameters.Length > selectedOption.UniqueParameterIndex)
                                                                {
                                                                    if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                                                    {
                                                                        if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(newParameters[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                                        {
                                                                            if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                                            {
                                                                                //bFound4 = true;
                                                                                //Delete old add new
                                                                                DeleteResourceQueueItem(property, existingValue);
                                                                            }
                                                                        }
                                                                    } else {
                                                                        if (existingParameters[selectedOption.UniqueParameterIndex].Trim() == newParameters[selectedOption.UniqueParameterIndex].Trim())
                                                                        {
                                                                            //bFound4 = true;
                                                                            //Delete old add new
                                                                            DeleteResourceQueueItem(property, existingValue);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            //if (!bFound4)
                                                            //{
                                                                //bAllowed = true;
                                                            //} else {
                                                                if (newParameters.Length > selectedOption.UniqueParameterIndex)
                                                                {
                                                                    bAllowed = true;
                                                                    //if (showDuplicateWarnings)
                                                                    //{
                                                                        //MessageBox.Show("Cannot add item. An item for property \"" + selectedOption.Name + "\" with parameter " + parameters2[selectedOption.UniqueParameterIndex] + " already exists and \"" + selectedOption.Name + parameters2[selectedOption.UniqueParameterIndex] + "\" must be unique.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.", "Error: Illegal input");
                                                                    //}
                                                                } else {
                                                                    MessageBox.Show("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
                                                                }
                                                            //}
                                                        //} else {
                                                            //if (showDuplicateWarnings)
                                                            //{
                                                                //MessageBox.Show("Cannot add item. An item for property \"" + selectedOption.Name + "\" already exists and \"" + selectedOption.Name + "\" must be unique.\r\n\r\nUnique properties and parameters can be configured in the VersionConfig.xml.", "Error: Illegal input");
                                                            //}
                                                        //}
                                                    } else {
                                                        bAllowed = true;
                                                    }
                                                } else {
                                                    bAllowed = true;
                                                }
                                            } else {
                                                string[] propertyNames = VersionConfig.ResourceQueueOptions.Select(a => a.Name).ToArray();
                                                string sPropertyNames = "";
                                                foreach (string a in propertyNames)
                                                {
                                                    sPropertyNames += a + "\r\n";
                                                }
                                                MessageBox.Show("Cannot add item. property name was not recognized. Legal property names are: \r\n" + sPropertyNames, "Error: Illegal input");
                                            }
                                        } else {
                                            MessageBox.Show("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is PropertyName(Parameters)", "Error: Illegal input");
                                        }
                                    }
                                } else {
                                    if (!propertyValue.StartsWith("CustomObject("))
                                    {
                                        if (showDuplicateWarnings)
                                        {
                                            MessageBox.Show("Cannot add item. An item with the value \"" + propertyValue + "\" already exists.", "Error: Illegal input");
                                        }
                                    }
                                }
                            } else {
                                bAllowed = true;
                            }
                            if(bAllowed)
                            {
                                IgnoreOverrideCheckChangedBiome = true;
                                Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                                BiomeConfig biomeConfig = g.BiomeConfig;

                                BiomeConfig biomeDefaultConfig = null;
                                if (g.showDefaults)
                                {
                                    biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                                }

                                string s = biomeConfig.GetPropertyValueAsString(property) ?? (biomeDefaultConfig != null && biomeDefaultConfig.GetPropertyValueAsString(property) != null ? biomeDefaultConfig.GetPropertyValueAsString(property) : "");
                                if (((string)lb.SelectedItem).Length > 0)
                                {
                                    if (s.IndexOf("\r\n" + (string)lb.SelectedItem + "\r\n") > -1) { s = (s.Replace("\r\n" + (string)lb.SelectedItem + "\r\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\r\n" + (string)lb.SelectedItem + "\n") > -1) { s = (s.Replace("\r\n" + (string)lb.SelectedItem + "\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\n" + (string)lb.SelectedItem + "\r\n") > -1) { s = (s.Replace("\n" + (string)lb.SelectedItem + "\r\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\n" + (string)lb.SelectedItem + "\n") > -1) { s = (s.Replace("\n" + (string)lb.SelectedItem + "\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf((string)lb.SelectedItem + "\r\n") > -1) { s = (s.Replace((string)lb.SelectedItem + "\r\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf((string)lb.SelectedItem + "\r\n") > -1) { s = (s.Replace((string)lb.SelectedItem + "\r\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf((string)lb.SelectedItem + "\n") > -1) { s = (s.Replace((string)lb.SelectedItem + "\n", "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\r\n" + (string)lb.SelectedItem) > -1) { s = (s.Replace("\r\n" + (string)lb.SelectedItem, "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf("\n" + (string)lb.SelectedItem) > -1) { s = (s.Replace("\n" + (string)lb.SelectedItem, "\r\n" + propertyValue + "\r\n")).Trim(); }
                                    else if (s.IndexOf((string)lb.SelectedItem) > -1) { s = (s.Replace((string)lb.SelectedItem, "\r\n" + propertyValue + "\r\n")).Trim(); }
                                }

                                bool bIsDefault = CompareResourceQueues(s, biomeDefaultConfig != null && biomeDefaultConfig.GetPropertyValueAsString(property) != null ? biomeDefaultConfig.GetPropertyValueAsString(property) : "");

                                if (!bIsDefault)
                                {
                                    biomeConfig.SetProperty(property, s, BiomeSettingsInputs[property].Item6 != null && ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, BiomeSettingsInputs[property].Item6 != null && ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                                    biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = true;
                                    BiomeSettingsInputs[property].Item2.Checked = true;
                                } else {
                                    biomeConfig.SetProperty(property, null, BiomeSettingsInputs[property].Item6 != null && ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, BiomeSettingsInputs[property].Item6 != null && ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                                    biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = false;
                                    BiomeSettingsInputs[property].Item2.Checked = false;
                                }

                                ((ListBox)BiomeSettingsInputs[property].Item1).Items.Clear();
                                string[] resourceQueueItemNames = s.Replace("\r", "").Split('\n');
                                foreach (string resourceQueueItemName in resourceQueueItemNames)
                                {
                                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                                    {
                                        ((ListBox)BiomeSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                                    }
                                }
                                IgnoreOverrideCheckChangedBiome = false;
                            }
                        }
                    }
                }
            }

            void btDeleteResourceQueueItem_Click(object sender, EventArgs e)
            {
                TCProperty property = ResourceQueueInputs[sender];
                ListBox lb = ((ListBox)BiomeSettingsInputs[property].Item1);
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
                Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
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

                bool bIsDefault = CompareResourceQueues(s, biomeDefaultConfig != null && biomeDefaultConfig.GetPropertyValueAsString(property) != null ? biomeDefaultConfig.GetPropertyValueAsString(property) : "");
                if (!bIsDefault)
                {
                    biomeConfig.SetProperty(property, s, BiomeSettingsInputs[property].Item6 != null && ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, BiomeSettingsInputs[property].Item6 != null && ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = true;
                    BiomeSettingsInputs[property].Item2.Checked = true;
                } else {
                    biomeConfig.SetProperty(property, null, BiomeSettingsInputs[property].Item6 != null && ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked, BiomeSettingsInputs[property].Item6 != null && ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                    biomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = false;
                    BiomeSettingsInputs[property].Item2.Checked = false;
                }

                ((ListBox)BiomeSettingsInputs[property].Item1).Items.Clear();
                string[] resourceQueueItemNames3 = s.Replace("\r", "").Split('\n');
                foreach (string resourceQueueItemName in resourceQueueItemNames3)
                {
                    if (!String.IsNullOrEmpty(resourceQueueItemName))
                    {
                        ((ListBox)BiomeSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                    }
                }
                IgnoreOverrideCheckChangedBiome = false;
            }

            private void btBiomeSettingsResetToDefaults_Click(object sender, EventArgs e)
            {
                Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                if (BiomeConfigsDefaultValues.Count > 0 && g.showDefaults)
                {
                    BiomeConfig biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    if (biomeDefaultConfig != null)
                    {
                        foreach (TCProperty property in VersionConfig.BiomeConfig)
                        {
                            Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = BiomeSettingsInputs[property];
                            IgnorePropertyInputChangedBiome = true;
                            IgnoreOverrideCheckChangedBiome = true;

                            string propertyValue = biomeDefaultConfig.GetPropertyValueAsString(property);
                            switch (property.PropertyType)
                            {
                                case "BiomesList":
                                    ((ListBox)BiomeSettingsInputs[property].Item1).SelectedItems.Clear();
                                    string[] biomeNames = propertyValue.Split(',');
                                    for (int k = 0; k < biomeNames.Length; k++)
                                    {
                                        if (BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                        {
                                            for (int l = 0; l < ((ListBox)BiomeSettingsInputs[property].Item1).Items.Count; l++)
                                            {
                                                if (((string)((ListBox)BiomeSettingsInputs[property].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                                {
                                                    ((ListBox)BiomeSettingsInputs[property].Item1).SelectedItems.Add(((ListBox)BiomeSettingsInputs[property].Item1).Items[l]);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case "Bool":
                                    if (propertyValue != null && propertyValue.ToLower() == "true")
                                    {
                                        ((ComboBox)boxes.Item1).SelectedIndex = 1;
                                    }
                                    else if (propertyValue != null && propertyValue.ToLower() == "false")
                                    {
                                        ((ComboBox)boxes.Item1).SelectedIndex = 2;
                                    } else {
                                        ((ComboBox)boxes.Item1).SelectedIndex = 0;
                                    }
                                    break;
                                case "Color":
                                    if ((propertyValue != null && propertyValue.Length == 8) || (propertyValue != null && propertyValue.Length == 7 && propertyValue.StartsWith("#")))
                                    {
                                        boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                        string value = "";
                                        if (SettingsType.ColorType == "0x")
                                        {
                                            value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                        }
                                        else if (SettingsType.ColorType == "#")
                                        {
                                            value = "#" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                        }
                                        boxes.Item1.Text = value;
                                    } else {
                                        boxes.Item5.BackColor = Color.White;
                                        boxes.Item1.Text = "";
                                    }
                                    break;
                                case "Float":
                                    boxes.Item1.Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                                    break;
                                case "String":
                                    boxes.Item1.Text = biomeDefaultConfig.GetPropertyValueAsString(property);
                                    break;
                                case "ResourceQueue":
                                    ((ListBox)boxes.Item1).Items.Clear();
                                    ((ListBox)boxes.Item1).SelectedIndex = -1;
                                    ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                    ((CheckBox)boxes.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                                    g.BiomeConfig.SetProperty(property, null, false, false);
                                    if(propertyValue != null)
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

                            IgnorePropertyInputChangedBiome = false;
                            IgnoreOverrideCheckChangedBiome = false;
                            g.BiomeConfig.SetProperty(property, null, false, false);
                            g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = false;
                        }
                    } else {
                        foreach (TCProperty property in VersionConfig.BiomeConfig)
                        {
                            Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = BiomeSettingsInputs[property];
                            IgnorePropertyInputChangedBiome = true;
                            IgnoreOverrideCheckChangedBiome = true;

                            switch (property.PropertyType)
                            {
                                case "BiomesList":
                                    ((ListBox)boxes.Item1).SelectedIndices.Clear();
                                    ((ListBox)boxes.Item1).SelectedIndex = -1;
                                    ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                    ((CheckBox)boxes.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                                    break;
                                case "Bool":
                                    ((ComboBox)boxes.Item1).SelectedIndex = 0;
                                    break;
                                case "Color":
                                    boxes.Item5.BackColor = Color.White;
                                    boxes.Item1.Text = "";
                                    break;
                                case "Float":
                                    boxes.Item1.Text = "";
                                    break;
                                case "String":
                                    boxes.Item1.Text = "";
                                    break;
                                case "ResourceQueue":
                                    ((ListBox)boxes.Item1).Items.Clear();
                                    ((ListBox)boxes.Item1).SelectedIndex = -1;
                                    ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                    ((CheckBox)boxes.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                                    g.BiomeConfig.SetProperty(property, null, false, false);
                                    break;
                            }
                            boxes.Item2.Checked = false;

                            IgnorePropertyInputChangedBiome = false;
                            IgnoreOverrideCheckChangedBiome = false;
                            g.BiomeConfig.SetProperty(property, null, false, false);
                            g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = false;
                        }
                    }
                } else {
                    foreach (TCProperty property in VersionConfig.BiomeConfig)
                    {
                        Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = BiomeSettingsInputs[property];
                        IgnorePropertyInputChangedBiome = true;
                        IgnoreOverrideCheckChangedBiome = true;

                        switch (property.PropertyType)
                        {
                            case "BiomesList":
                                ((ListBox)boxes.Item1).SelectedIndices.Clear();
                                ((ListBox)boxes.Item1).SelectedIndex = -1;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((CheckBox)boxes.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                                break;
                            case "Bool":
                                ((ComboBox)boxes.Item1).SelectedIndex = 0;
                                break;
                            case "Color":
                                boxes.Item5.BackColor = Color.White;
                                boxes.Item1.Text = "";
                                break;
                            case "Float":
                                boxes.Item1.Text = "";
                                break;
                            case "String":
                                boxes.Item1.Text = "";
                                break;
                            case "ResourceQueue":
                                ((ListBox)boxes.Item1).Items.Clear();
                                ((ListBox)boxes.Item1).SelectedIndex = -1;
                                ((RadioButton)boxes.Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                ((RadioButton)boxes.Item6.Controls.Find("Override", true)[0]).Checked = true;
                                ((CheckBox)boxes.Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                                break;
                        }
                        boxes.Item2.Checked = false;

                        IgnorePropertyInputChangedBiome = false;
                        IgnoreOverrideCheckChangedBiome = false;
                        g.BiomeConfig.SetProperty(property, null, false, false);
                        g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override = false;
                    }
                }
            }

            private void lbGroups_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (lbGroups.SelectedIndex > -1)
                {
                    tlpBiomeSettings.Visible = true;
                    btBiomeSettingsResetToDefaults.Visible = true;
                    label3.Visible = true;

                    lbGroup.Items.Clear();
                    lbBiomes.Items.Clear();
                    Group g = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
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
                        btBiomeSettingsResetToDefaults.Text = "Clear all";
                        foreach (Button bSetDefaults in BiomeSettingsInputs.Select(a => a.Value.Item3))
                        {
                            ToolTip1.SetToolTip(bSetDefaults, "Clear");
                            bSetDefaults.Text = "C";
                        }
                    } else {
                        if (biomeConfigDefaultValues != null)
                        {
                            btBiomeSettingsResetToDefaults.Text = "Set to defaults";
                            foreach (Button bSetDefaults in BiomeSettingsInputs.Select(a => a.Value.Item3))
                            {
                                ToolTip1.SetToolTip(bSetDefaults, "Set to default");
                                bSetDefaults.Text = "<";
                            }
                        } else {
                            btBiomeSettingsResetToDefaults.Text = "Clear all";
                            foreach (Button bSetDefaults in BiomeSettingsInputs.Select(a => a.Value.Item3))
                            {
                                ToolTip1.SetToolTip(bSetDefaults, "Clear");
                                bSetDefaults.Text = "C";
                            }
                        }
                    }

                    IgnorePropertyInputChangedBiome = true;
                    IgnoreOverrideCheckChangedBiome = true;

                    foreach (TCProperty property in VersionConfig.BiomeConfig)
                    {
                        switch (property.PropertyType)
                        {
                            case "BiomesList":
                                ((ListBox)BiomeSettingsInputs[property].Item1).SelectedIndices.Clear();
                                ((ListBox)BiomeSettingsInputs[property].Item1).SelectedIndex = -1;
                                if (g.showDefaults)
                                {
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Visible = false;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Visible = false;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                } else {
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Visible = true;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Visible = true;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                }
                                ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                            break;
                            case "Bool":
                                ((ComboBox)BiomeSettingsInputs[property].Item1).SelectedIndex = 0;
                            break;
                            case "Color":
                                BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                                BiomeSettingsInputs[property].Item1.Text = "";
                            break;
                            case "Float":
                                BiomeSettingsInputs[property].Item1.Text = "";
                            break;
                            case "String":
                                BiomeSettingsInputs[property].Item1.Text = "";
                            break;
                            case "ResourceQueue":
                                ((ListBox)BiomeSettingsInputs[property].Item1).Items.Clear();
                                ((ListBox)BiomeSettingsInputs[property].Item1).SelectedIndex = -1;
                                if (g.showDefaults)
                                {
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Visible = false;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Visible = false;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                } else {
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Visible = true;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Visible = true;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                }
                                ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;
                            break;
                        }
                        BiomeSettingsInputs[property].Item2.Checked = false;

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
                            Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = BiomeSettingsInputs[property];

                            string propertyValue = s;
                            switch (property.PropertyType)
                            {
                                case "BiomesList":
                                    ((ListBox)BiomeSettingsInputs[property].Item1).SelectedItems.Clear();
                                    string[] biomeNames = propertyValue != null ? propertyValue.Split(',') : new string[0];
                                    for (int k = 0; k < biomeNames.Length; k++)
                                    {
                                        if (BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                        {
                                            for (int l = 0; l < ((ListBox)BiomeSettingsInputs[property].Item1).Items.Count; l++)
                                            {
                                                if (((string)((ListBox)BiomeSettingsInputs[property].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                                {
                                                    ((ListBox)BiomeSettingsInputs[property].Item1).SelectedItems.Add(((ListBox)BiomeSettingsInputs[property].Item1).Items[l]);
                                                }
                                            }
                                        }
                                    }
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = !g.BiomeConfig.GetPropertyMerge(property);
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = g.BiomeConfig.GetPropertyMerge(property);
                                    ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = g.BiomeConfig.GetPropertyOverrideParentValues(property);
                                    boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                break;
                                case "Bool":
                                    if (propertyValue.ToLower() == "true")
                                    {
                                        ((ComboBox)boxes.Item1).SelectedIndex = 1;
                                    }
                                    else if (propertyValue.ToLower() == "false")
                                    {
                                        ((ComboBox)boxes.Item1).SelectedIndex = 2;
                                    } else {
                                        ((ComboBox)boxes.Item1).SelectedIndex = 0;
                                    }
                                    boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                break;
                                case "Color":
                                    if (propertyValue.Length == 8 || (propertyValue.Length == 7 && propertyValue.StartsWith("#")))
                                    {
                                        boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                        bool bException = false;
                                        try
                                        {
                                            boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                            string value = "";
                                            if (SettingsType.ColorType == "0x")
                                            {
                                                value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                            }
                                            else if (SettingsType.ColorType == "#")
                                            {
                                                value = "#" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                            }
                                            boxes.Item1.Text = value;
                                            if (biomeConfigDefaultValues == null || boxes.Item2.Checked || value.ToUpper() != biomeConfigDefaultValues.GetPropertyValueAsString(property).ToUpper())
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
                                            string asss = biomeConfigDefaultValues.GetPropertyValueAsString(property);
                                            if (asss.Length == 8 || (asss.Length == 7 && asss.StartsWith("#")))
                                            {
                                                try
                                                {
                                                    boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(asss);
                                                    string value = "";
                                                    if (SettingsType.ColorType == "0x")
                                                    {
                                                        value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                    }
                                                    else if (SettingsType.ColorType == "#")
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
                                    boxes.Item1.Text = propertyValue;
                                    boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                break;
                                case "String":
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
                                            ((ListBox)BiomeSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                                        }
                                    }
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = !g.BiomeConfig.GetPropertyMerge(property);
                                    ((RadioButton)BiomeSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = g.BiomeConfig.GetPropertyMerge(property);
                                    ((CheckBox)BiomeSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = g.BiomeConfig.GetPropertyOverrideParentValues(property);
                                    boxes.Item2.Checked = g.BiomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name) != null && g.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override;
                                break;
                            }                        
                        }
                    }

                    IgnoreOverrideCheckChangedBiome = false;
                    IgnorePropertyInputChangedBiome = false;
                } else {
                    tlpBiomeSettings.Visible = false;
                    btBiomeSettingsResetToDefaults.Visible = false;
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
            }

            private void btNewGroup_Click(object sender, EventArgs e)
            {
                string groupName = "";
                List<string> biomesToAdd = PopUpForm.BiomeListSelectionBox(ref groupName, BiomeConfigsDefaultValues.Select(a => a.BiomeName).ToList());

                if (!String.IsNullOrWhiteSpace(groupName))
                {
                    if (!BiomeGroups.Any(a => a.Name == groupName))
                    {
                        lbGroups.Items.Add(groupName);
                        Group g = new Group(groupName, VersionConfig);
                        g.Biomes.AddRange(biomesToAdd);
                        BiomeGroups.Add(g);
                        lbGroups.SelectedIndex = -1;
                        lbGroups.SelectedIndex = lbGroups.Items.Count - 1;
                    } else {
                        MessageBox.Show("A group with this name already exists", "Error: Illegal input");
                    }
                }
            }

            private void btEditGroup_Click(object sender, EventArgs e)
            {
                if(lbGroups.SelectedIndex > -1)
                {
                    string groupName = "";
                    if(PopUpForm.InputBox("Rename group", "Enter a name for the group. Only a-z A-Z 0-9 space + - and _ are allowed.", ref groupName) == DialogResult.OK)
                    {
                        if(!string.IsNullOrWhiteSpace(groupName))
                        {
                            if (!BiomeGroups.Any(a => a.Name == groupName))
                            {
                                BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.Items[lbGroups.SelectedIndex]).Name = groupName;
                                lbGroups.Items[lbGroups.SelectedIndex] = groupName;                        
                            } else {
                                MessageBox.Show("A group with this name already exists", "Error: Illegal input");
                            }
                        }
                    }
                }
            }

            private void btDeleteGroup_Click(object sender, EventArgs e)
            {
                if (lbGroups.SelectedIndex > -1)
                {
                    int selectedIndex = lbGroups.SelectedIndex;
                    BiomeGroups.RemoveAll(a => a.Name == (string)lbGroups.SelectedItem);
                    lbGroups.Items.Remove(lbGroups.SelectedItem);
                    lbGroups.SelectedIndex = lbGroups.Items.Count > selectedIndex ? selectedIndex : (lbGroups.Items.Count > selectedIndex - 1 ? selectedIndex - 1 : (lbGroups.Items.Count > 0 ? 0 : -1));
                    if(lbGroups.Items.Count < 1)
                    {
                        lbGroup.Items.Clear();
                        lbBiomes.Items.Clear();
                    }
                }
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
                        Group biomeGroup = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
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
                }
            }

            private void btRemoveFromGroup_Click(object sender, EventArgs e)
            {
                if (lbGroup.Items.Count > 1 && lbGroup.SelectedItems.Count > 0 && lbGroup.SelectedItems.Count < lbGroup.Items.Count)
                {
                    List<string> itemsToDelete = new List<string>();
                    Group biomeGroup = BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
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
                } else {
                    if (lbGroup.Items.Count == 1 || lbGroup.SelectedItems.Count >= lbGroup.Items.Count)
                    {
                        MessageBox.Show("Cannot delete the selected biome(s). A group must contain at least one biome.", "Error: Illegal input");
                    }
                }
            }

        #endregion

        #region Load / Save / Generate

            SaveFileDialog sfd;
            void btSave_Click(object sender, EventArgs e)
            {
                sfd.CheckFileExists = false;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    List<Group> groups = new List<Group>();
                    foreach(string groupName in lbGroups.Items)
                    {
                        groups.Add(BiomeGroups.FirstOrDefault(a => a.Name == groupName));
                    }

                    SettingsFile settingsFile = new SettingsFile()
                    {
                        WorldConfig = WorldConfig1,
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

                    MessageBox.Show("Settings saved as: " + sfd.FileName, "Settings saved");
                }
            }

            OpenFileDialog ofd;
            void btLoad_Click(object sender, EventArgs e)
            {
                string sErrorMessage = "";
                string sErrorMessage2 = "";
                if(BiomeConfigsDefaultValues.Count > 0)
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        if(!ofd.FileName.ToLower().EndsWith("xml"))
                        {
                            MessageBox.Show("Error: The selected file was not a valid TCEE save file");
                        } else {
                            SettingsFile settingsFile = null;
                            var serializer = new DataContractSerializer(typeof(SettingsFile));
                            using (var reader = new XmlTextReader(ofd.FileName))
                            {
                                settingsFile = (SettingsFile)serializer.ReadObject(reader);
                            }

                            if (settingsFile != null)
                            {
                                WorldConfig1 = settingsFile.WorldConfig;                        
                                foreach(Property prop in WorldConfig1.Properties.Where(c => c.Override && !VersionConfig.WorldConfig.Any(d => (string)d.Name == (string)c.PropertyName)))
                                {
                                    sErrorMessage += "Could not load value \"" + prop.Value + "\" for property \"" + prop.PropertyName + "\" in WorldConfig.\r\n";
                                }
                                WorldConfig1.Properties.RemoveAll(a => !VersionConfig.WorldConfig.Any(b => (string)b.Name == (string)a.PropertyName));

                                IgnorePropertyInputChangedWorld = true;
                                IgnoreOverrideCheckChangedWorld = true;

                                foreach (TCProperty property in VersionConfig.WorldConfig)
                                {
                                    switch (property.PropertyType)
                                    {
                                        case "BiomesList":
                                            ((ListBox)WorldSettingsInputs[property].Item1).SelectedIndices.Clear();
                                            ((ListBox)WorldSettingsInputs[property].Item1).SelectedIndex = -1;
                                            ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                            ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                            break;
                                        case "Bool":
                                            ((ComboBox)WorldSettingsInputs[property].Item1).SelectedIndex = 0;
                                            break;
                                        case "Color":
                                            WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                            WorldSettingsInputs[property].Item1.Text = "";
                                            break;
                                        case "Float":
                                            WorldSettingsInputs[property].Item1.Text = "";
                                            break;
                                        case "String":
                                            WorldSettingsInputs[property].Item1.Text = "";
                                            break;
                                        case "ResourceQueue":
                                            ((ListBox)WorldSettingsInputs[property].Item1).Items.Clear();
                                            ((ListBox)WorldSettingsInputs[property].Item1).SelectedIndex = -1;
                                            ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                            ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                            break;
                                    }
                                    WorldSettingsInputs[property].Item2.Checked = false;
                           
                                    if (WorldConfig1.Properties.FirstOrDefault(a => a.PropertyName == property.Name) == null)
                                    {
                                        WorldConfig1.Properties.Add(new Property(null, false, property.Name, false, false, property.Optional));
                                    }

                                    string s = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                    if (WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override)
                                    {
                                        if (property.PropertyType != "ResourceQueue" || WorldConfig1.GetPropertyValueAsString(property) != null)
                                        {
                                            s = WorldConfig1.GetPropertyValueAsString(property);
                                        }
                                    }

                                    if (s != null || WorldConfig1.GetPropertyMerge(property))
                                    {
                                        Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = WorldSettingsInputs[property];
                                        string propertyValue = s;
                                        switch (property.PropertyType)
                                        {
                                            case "BiomesList":
                                                ((ListBox)WorldSettingsInputs[property].Item1).SelectedItems.Clear();
                                                string[] biomeNames = propertyValue.Split(',');
                                                string newpropertyValue = "";
                                                for (int k = 0; k < biomeNames.Length; k++)
                                                {
                                                    if (BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                                    {
                                                        for (int l = 0; l < ((ListBox)WorldSettingsInputs[property].Item1).Items.Count; l++)
                                                        {
                                                            if (((string)((ListBox)WorldSettingsInputs[property].Item1).Items[l]).Trim() == (string)biomeNames[k].Trim())
                                                            {
                                                                ((ListBox)WorldSettingsInputs[property].Item1).SelectedItems.Add(((ListBox)WorldSettingsInputs[property].Item1).Items[l]);
                                                            }
                                                        }
                                                        newpropertyValue += k == biomeNames.Length - 1 ? (string)biomeNames[k].Trim() : (string)biomeNames[k].Trim() + ", ";
                                                    }
                                                    else if (!String.IsNullOrEmpty(biomeNames[k].Trim()))
                                                    {
                                                        sErrorMessage2 += "Could not select biome \"" + biomeNames[k].Trim() + "\" for property \"" + property.Name + "\" in world config.\r\n";
                                                    }
                                                }
                                                if(!String.IsNullOrEmpty(newpropertyValue))
                                                {
                                                    WorldConfig1.Properties.First(a => (string)a.PropertyName == (string)property.Name).Value = newpropertyValue;
                                                }
                                                ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = !WorldConfig1.GetPropertyMerge(property);
                                                ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = WorldConfig1.GetPropertyMerge(property);
                                                boxes.Item2.Checked = WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                            case "Bool":
                                                if (propertyValue.ToLower() == "true")
                                                {
                                                    ((ComboBox)boxes.Item1).SelectedIndex = 1;
                                                }
                                                else if (propertyValue.ToLower() == "false")
                                                {
                                                    ((ComboBox)boxes.Item1).SelectedIndex = 2;
                                                } else {
                                                    ((ComboBox)boxes.Item1).SelectedIndex = 0;
                                                }
                                                boxes.Item2.Checked = WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                            case "Color":
                                                if (propertyValue.Length == 8 || (propertyValue.Length == 7 && propertyValue.StartsWith("#")))
                                                {
                                                    boxes.Item2.Checked = WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                    bool bException = false;
                                                    try
                                                    {
                                                        boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                                        string value = "";
                                                        if (SettingsType.ColorType == "0x")
                                                        {
                                                            value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                        }
                                                        else if (SettingsType.ColorType == "#")
                                                        {
                                                            value = "#" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                        }
                                                        boxes.Item1.Text = value;
                                                        if (WorldConfigDefaultValues == null || boxes.Item2.Checked || value.ToUpper() != WorldConfigDefaultValues.GetPropertyValueAsString(property).ToUpper())
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
                                                        string asss = WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                                        if (asss.Length == 8 || (asss.Length == 7 && asss.StartsWith("#")))
                                                        {
                                                            try
                                                            {
                                                                boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(asss);
                                                                string value = "";
                                                                if (SettingsType.ColorType == "0x")
                                                                {
                                                                    value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                                                }
                                                                else if (SettingsType.ColorType == "#")
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
                                                boxes.Item1.Text = propertyValue;
                                                boxes.Item2.Checked = WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                            case "String":
                                                boxes.Item1.Text = propertyValue;
                                                boxes.Item2.Checked = WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
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
                                                        ((ListBox)WorldSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                                                    }
                                                }
                                                ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = !WorldConfig1.GetPropertyMerge(property);
                                                ((RadioButton)WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = WorldConfig1.GetPropertyMerge(property);
                                                boxes.Item2.Checked = WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                        }
                                    }
                                }

                                IgnoreOverrideCheckChangedWorld = false;
                                IgnorePropertyInputChangedWorld = false;

                                BiomeGroups.Clear();
                                lbBiomes.Items.Clear();
                                lbGroup.Items.Clear();
                                lbGroups.Items.Clear();

                                //tlpBiomeSettings.Controls.Clear();
                                tlpBiomeSettings.Visible = false;
                                btBiomeSettingsResetToDefaults.Visible = false;
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
                                            if (BiomeNames.Any(a => a.Equals(biomeName)))
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
                                            BiomeGroups.Add(biomeGroup);
                                        }
                                        if(!String.IsNullOrEmpty(sErrorMessage3))
                                        {
                                            sErrorMessage2 += sErrorMessage3;// +"These biomes do not exist.\r\n";
                                        }
                                    } else {
                                        if (BiomeNames.Any(a => a.Equals(biomeGroup.Biomes[0])))
                                        {
                                            lbGroups.Items.Add(biomeGroup.Name);
                                            BiomeGroups.Add(biomeGroup);
                                        } else {
                                            sErrorMessage2 += "Could not load biome group \"" + biomeGroup.Name + "\" because biome \"" + biomeGroup.Biomes[0] + "\" does not exist.\r\n";
                                        }
                                    }
                                }
                                settingsFile.BiomeGroups = newBiomeGroups;

                                if(lbGroups.Items.Count > 0)
                                {
                                    lbGroups.SelectedIndex = 0;
                                    foreach (Group biomeGroup in BiomeGroups)
                                    {
                                        foreach(TCProperty tcProp in VersionConfig.BiomeConfig)
                                        {
                                            if (!biomeGroup.BiomeConfig.Properties.Any(a => (string)a.PropertyName == (string)tcProp.Name))
                                            {
                                                biomeGroup.BiomeConfig.Properties.Add(new Property(null, false, tcProp.Name, false, false, tcProp.Optional));
                                            }
                                        }
                                        foreach (Property prop in biomeGroup.BiomeConfig.Properties.Where(c => c.Override && !VersionConfig.BiomeConfig.Any(d => (string)d.Name == (string)c.PropertyName)))
                                        {
                                            sErrorMessage += "Could not load value \"" + prop.Value + "\" for property \"" + prop.PropertyName + "\" in biome group \"" + biomeGroup.Name + "\".\r\n";
                                        }
                                        foreach (Property prop in biomeGroup.BiomeConfig.Properties.Where(c => c.Override && VersionConfig.BiomeConfig.Any(d => (string)d.Name == (string)c.PropertyName && d.PropertyType == "BiomesList")))
                                        {
                                            string[] biomeNames = prop.Value.Split(',');
                                            string newBiomeNames = "";
                                            for (int k = 0; k < biomeNames.Length; k++)
                                            {
                                                if (!BiomeNames.Any(a => (string)a.Trim() == (string)biomeNames[k].Trim()))
                                                {
                                                    if(!String.IsNullOrEmpty(biomeNames[k].Trim()))
                                                    {
                                                        sErrorMessage2 += "Could not select biome \"" + biomeNames[k].Trim() + "\" for property \"" + prop.PropertyName + "\" in biome group \"" + biomeGroup.Name + "\".\r\n";
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
                                        biomeGroup.BiomeConfig.Properties.RemoveAll(a => !VersionConfig.BiomeConfig.Any(b => (string)b.Name == (string)a.PropertyName));
                                    }
                                }
                            }
                        }
                    }
                } else {
                    MessageBox.Show("No source world found. Load a source world before loading a settings file.", "Version warning");
                }
                string sErrorMessageFinal = "";
                if(!String.IsNullOrEmpty(sErrorMessage))
                {
                    sErrorMessageFinal = sErrorMessage + "These properties do not exist in the selected version of TerrainControl.";
                }
                if (!String.IsNullOrEmpty(sErrorMessage2))
                {
                    sErrorMessageFinal = sErrorMessageFinal + "\r\n\r\n" + sErrorMessage2 + "These biomes do not exist in the selected version of TerrainControl.";
                }
                if(!String.IsNullOrEmpty(sErrorMessageFinal))
                {
                    MessageBox.Show(sErrorMessageFinal, "Version warnings");
                }
            }        

            void btGenerate_Click(object sender, EventArgs e)
            {
                fbdDestinationWorldDir.Description = "Select a TerrainControl world folder.";

                if (fbdDestinationWorldDir.ShowDialog() == DialogResult.OK)
                {
                    bool go = true;
                    String ares = fbdDestinationWorldDir.SelectedPath.Substring(0, fbdDestinationWorldDir.SelectedPath.LastIndexOf("\\"));
                    if (!fbdDestinationWorldDir.SelectedPath.Substring(0, fbdDestinationWorldDir.SelectedPath.LastIndexOf("\\")).EndsWith("TerrainControl\\worlds"))
                    {
                        go = MessageBox.Show("The selected directory was not a TerrainControl/MCW world directory, are you sure you want to generate files here?", "Generate files here?", MessageBoxButtons.OKCancel) == DialogResult.OK;
                    }
                    if (go)
                    {

                        DestinationConfigsDir = fbdDestinationWorldDir.SelectedPath;
                        WorldSaveDir = DestinationConfigsDir.Replace("mods\\TerrainControl\\worlds\\", "saves\\");

                        ConfigWorld();
                        GenerateBiomeConfigs(new System.IO.DirectoryInfo(SourceConfigsDir + "/" + "WorldBiomes"), new System.IO.DirectoryInfo(DestinationConfigsDir + "/" + "WorldBiomes"), BiomeConfigsDefaultValues, VersionConfig);

                        bool bDone = true;
                        if (cbDeleteRegion.Checked)
                        {
                            System.IO.DirectoryInfo WorldDirectory = new System.IO.DirectoryInfo(WorldSaveDir + "/region");
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
                                    MessageBox.Show("Could not delete /regions directory because it is currently in use. Please make sure that no one is playing the selected world and try again.", "Generation error");
                                }
                            }

                            System.IO.DirectoryInfo worldDirectory2 = new System.IO.DirectoryInfo(WorldSaveDir + "/data");
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
                                    MessageBox.Show("Could not delete /data directory because it is currently in use. Please make sure that no one is playing the selected world and try again.", "Generation error");
                                }
                            }

                            System.IO.DirectoryInfo StructureDataDirectory = new System.IO.DirectoryInfo(DestinationConfigsDir + "/StructureData");
                            if (StructureDataDirectory.Exists)
                            {
                                bDone = false;
                                try
                                {
                                    StructureDataDirectory.Delete(true);
                                    bDone = true;
                                }
                                catch (System.IO.IOException ex)
                                {
                                    MessageBox.Show("Could not delete /StructureData directory because it is currently in use. Please make sure that no one is playing the selected world and try again.", "Generation error");
                                }
                            }

                            System.IO.FileInfo structureDataFile = new System.IO.FileInfo(DestinationConfigsDir + "/StructureData.txt");
                            if (structureDataFile.Exists)
                            {
                                structureDataFile.Delete();
                            }
                            System.IO.FileInfo nullChunksFile = new System.IO.FileInfo(DestinationConfigsDir + "/NullChunks.txt");
                            if (nullChunksFile.Exists)
                            {
                                nullChunksFile.Delete();
                            }
                            System.IO.FileInfo spawnedStructuresFile = new System.IO.FileInfo(DestinationConfigsDir + "/SpawnedStructures.txt");
                            if (spawnedStructuresFile.Exists)
                            {
                                spawnedStructuresFile.Delete();
                            }

                            System.IO.FileInfo chunkProviderPopulatedChunksFile = new System.IO.FileInfo(DestinationConfigsDir + "/ChunkProviderPopulatedChunks.txt");
                            if (chunkProviderPopulatedChunksFile.Exists)
                            {
                                chunkProviderPopulatedChunksFile.Delete();
                            }

                            System.IO.FileInfo pregeneratedChunksFile = new System.IO.FileInfo(DestinationConfigsDir + "/PregeneratedChunks.txt");
                            if (pregeneratedChunksFile.Exists)
                            {
                                pregeneratedChunksFile.Delete();
                            }
                        }

                        if (bDone)
                        {
                            MessageBox.Show("Done", "Generating");
                        }
                    }
                }
            }

            void ConfigWorld()
            {
                System.IO.DirectoryInfo DefaultWorldDirectory = new System.IO.DirectoryInfo(SourceConfigsDir);
                System.IO.FileInfo defaultWorldConfig = new System.IO.FileInfo(DefaultWorldDirectory + "/WorldConfig.ini");

                if (defaultWorldConfig.Exists)
                {
                    StringBuilder defaultText = new StringBuilder(System.IO.File.ReadAllText(defaultWorldConfig.FullName));
                    string sDefaultText = defaultText.ToString();
                    string errorsTxt = "";
                    foreach (TCProperty property in VersionConfig.WorldConfig)
                    {
                        string propertyValue = WorldConfig1.GetPropertyValueAsString(property);
                        if (propertyValue != null && WorldConfigDefaultValues != null && (propertyValue == WorldConfigDefaultValues.GetPropertyValueAsString(property) || (property.PropertyType.Equals("BiomesList") && CompareBiomeLists(propertyValue, WorldConfigDefaultValues.GetPropertyValueAsString(property))) || (property.PropertyType.Equals("ResourceQueue") && CompareResourceQueues(propertyValue, WorldConfigDefaultValues.GetPropertyValueAsString(property)))))
                        {
                            propertyValue = null;
                        }
                        if(propertyValue != null && WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override)
                        {
                            if (property.PropertyType == "ResourceQueue")
                            {
                                bool bFound = false;
                                int replaceStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                                if (replaceStartIndex > -1)
                                {
                                    while (!bFound)
                                    {
                                        int lineStart = sDefaultText.Substring(replaceStartIndex).IndexOf("\n") + 1 + replaceStartIndex;
                                        int lineLength = sDefaultText.Substring(lineStart).IndexOf("\n") + 1;

                                        if (
                                            /* line doesnt contain # */
                                            !sDefaultText.Substring(lineStart, lineLength).Contains("#") &&
                                            (
                                            /* and line isnt empty or is empty and is last line or next line starts with ## */
                                                sDefaultText.Substring(lineStart, lineLength).Replace("\r", "").Replace("\n", "").Trim().Length > 0 ||
                                                (
                                            //if this is the last line
                                                    sDefaultText.Substring(lineStart + lineLength).IndexOf("\n") < 0 ||
                                            //or next line starts with ##
                                                    (sDefaultText.Substring(lineStart + lineLength).IndexOf("##") > -1 && sDefaultText.Substring(lineStart + lineLength).IndexOf("##") < sDefaultText.Substring(lineStart + lineLength).IndexOf("\n"))
                                                )
                                            )
                                        )
                                        {
                                            replaceStartIndex = lineStart;
                                            bFound = true;
                                        } else {
                                            replaceStartIndex = sDefaultText.Substring(replaceStartIndex).IndexOf("\n") + 1 + replaceStartIndex;
                                            if (replaceStartIndex < 0 || replaceStartIndex == sDefaultText.Length)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (replaceStartIndex > -1)
                                    {
                                        int replaceLength = 0;

                                        while (replaceStartIndex + replaceLength + 2 <= sDefaultText.Length && sDefaultText.Substring(replaceStartIndex + replaceLength, 2) != "\n#")
                                        {
                                            replaceLength += 1;
                                        }
                                        if (replaceStartIndex + replaceLength + 1 == sDefaultText.Length)
                                        {
                                            replaceLength += 1;
                                        }
                                        if (replaceLength > 0)
                                        {
                                            string[] biomesListItemNames = propertyValue != null ? propertyValue.Replace("\r", "").Split('\n') : null;
                                            string[] defaultBiomesListItemNames = WorldConfig1.GetPropertyValueAsString(property) != null ? WorldConfig1.GetPropertyValueAsString(property).Replace("\r", "").Split('\n') : null;
                                            List<string> newPropertyValue = new List<string>();

                                            if (biomesListItemNames != null)
                                            {
                                                foreach (string value1 in biomesListItemNames)
                                                {
                                                    if (value1 != null && !string.IsNullOrEmpty(value1.Trim()))
                                                    {
                                                        //bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)value1) || MessageBox.Show("An item with the same value already exists, are you sure you want to add another?", "Allow duplicate?", MessageBoxButtons.OKCancel) == DialogResult.OK;
                                                        bool duplicatePermission = value1.StartsWith("CustomObject(") || !newPropertyValue.Any(a => (string)a == (string)value1);
                                                        //bool duplicatePermission = true;
                                                        if (duplicatePermission)
                                                        {
                                                            if (property.Name == "Mob spawning")
                                                            {
                                                                newPropertyValue.Add(value1.Trim());
                                                            } else {
                                                                if (value1.Contains('(') && value1.Contains(')'))
                                                                {
                                                                    bool bFound3 = false;
                                                                    ResourceQueueItem selectedOption = null;
                                                                    foreach (ResourceQueueItem option in VersionConfig.ResourceQueueOptions)
                                                                    {
                                                                        if (value1.StartsWith(option.Name))
                                                                        {
                                                                            bFound3 = true;
                                                                            selectedOption = option;
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (bFound3)
                                                                    {
                                                                        if (selectedOption.IsUnique)
                                                                        {
                                                                            List<string> possibleDuplicates = new List<string>();
                                                                            foreach (string value2 in newPropertyValue)
                                                                            {
                                                                                if (value2.StartsWith(selectedOption.Name))
                                                                                {
                                                                                    possibleDuplicates.Add(value2);
                                                                                }
                                                                            }
                                                                            if (possibleDuplicates.Any())
                                                                            {
                                                                                if (selectedOption.HasUniqueParameter)
                                                                                {
                                                                                    foreach (string value3 in possibleDuplicates)
                                                                                    {
                                                                                        string[] parameters = value3.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                                                        string[] parameters2 = value1.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                                                        if (parameters.Length > selectedOption.UniqueParameterIndex && parameters2.Length > selectedOption.UniqueParameterIndex)
                                                                                        {
                                                                                            if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                                                                            {
                                                                                                if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(parameters2[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                                                                {
                                                                                                    if (parameters[selectedOption.UniqueParameterIndex].Trim() == parameters2[selectedOption.UniqueParameterIndex].Trim())
                                                                                                    {
                                                                                                        newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name + parameters2[selectedOption.UniqueParameterIndex]));
                                                                                                    }
                                                                                                }
                                                                                            } else {
                                                                                                if (parameters[selectedOption.UniqueParameterIndex].Trim() == parameters2[selectedOption.UniqueParameterIndex].Trim())
                                                                                                {
                                                                                                    newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name + parameters2[selectedOption.UniqueParameterIndex]));
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    newPropertyValue.Add(value1.Trim());
                                                                                } else {
                                                                                    newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name));
                                                                                    newPropertyValue.Add(value1.Trim());
                                                                                }
                                                                            } else {
                                                                                newPropertyValue.Add(value1.Trim());
                                                                            }
                                                                        } else {
                                                                            newPropertyValue.Add(value1.Trim());
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            propertyValue = "";
                                            int i = 0;
                                            foreach (string value1 in newPropertyValue)
                                            {
                                                propertyValue += (i != newPropertyValue.Count() - 1 ? value1 + "\r\n" : value1);
                                                i++;
                                            }
                                        }
                                        if (replaceLength > 0)
                                        {
                                            defaultText = defaultText.Remove(replaceStartIndex, replaceLength);
                                        }
                                        defaultText = defaultText.Insert(replaceStartIndex, propertyValue + "\r\n");
                                        sDefaultText = defaultText.ToString();
                                    } else {
                                        if (!property.Optional)
                                        {
                                            defaultText.Append("\r\n# This property was added by TCEE and is most probably used by the Minecraft Worlds mod #\r\n");
                                            defaultText.Append(property.ScriptHandle + " " + propertyValue + "\r\n");

                                            errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                        }
                                    }
                                } else {
                                    if (!property.Optional)
                                    {
                                        defaultText.Append("\r\n# This property was added by TCEE and is most probably used by the Minecraft Worlds mod #\r\n");
                                        defaultText.Append(property.ScriptHandle + " " + propertyValue + "\r\n");

                                        errorsTxt += "\r\nVersion config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                    }
                                }
                            } else {
                                bool bFound = false;
                                int replaceStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                                if (replaceStartIndex > -1)
                                {
                                    while (!bFound)
                                    {
                                        if (sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("\n") > sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("#"))
                                        {
                                            bFound = true;
                                        } else {
                                            replaceStartIndex = sDefaultText.Substring(replaceStartIndex + property.ScriptHandle.Length).ToLower().IndexOf(property.ScriptHandle.ToLower()) + replaceStartIndex + property.ScriptHandle.Length;
                                            if (replaceStartIndex < 0 || replaceStartIndex == sDefaultText.Length)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (replaceStartIndex > -1)
                                    {
                                        int skipCharsLength = (property.ScriptHandle).Length;
                                        int replaceLength = 0;

                                        while (replaceStartIndex + skipCharsLength + replaceLength < sDefaultText.Length && sDefaultText.Substring(replaceStartIndex + skipCharsLength + replaceLength, 1) != "\n")
                                        {
                                            replaceLength += 1;
                                        }

                                        if (property.PropertyType == "BiomesList" && WorldConfig1.GetPropertyMerge(property))
                                        {
                                            if (replaceLength > 0)
                                            {                                        
                                                string[] biomesListItemNames = propertyValue != null ? propertyValue.Split(',') : null;
                                                string originalValue = defaultText.ToString().Substring(replaceStartIndex + skipCharsLength, replaceLength);
                                                string[] originalbiomesListItemNames = originalValue != null ? originalValue.Split(',') : null;
                                                List<string> newPropertyValue = new List<string>();
                                                foreach(string value in biomesListItemNames)
                                                {
                                                    if(newPropertyValue.Any(a => (string)a == (string)value))
                                                    {
                                                        newPropertyValue.Add(value);
                                                    }
                                                }
                                                foreach (string value in originalbiomesListItemNames)
                                                {
                                                    if (newPropertyValue.Any(a => (string)a == (string)value))
                                                    {
                                                        newPropertyValue.Add(value);
                                                    }
                                                }
                                                propertyValue = "";
                                                foreach (string value in newPropertyValue)
                                                {
                                                    propertyValue += (value != newPropertyValue[newPropertyValue.Count() - 1] ? value + ", " : value);
                                                }
                                            }
                                        }
                                        if (replaceLength > 0)
                                        {
                                            defaultText = defaultText.Remove(replaceStartIndex + skipCharsLength, replaceLength);
                                        }
                                        defaultText = defaultText.Insert(replaceStartIndex + skipCharsLength, " " + propertyValue);
                                        sDefaultText = defaultText.ToString();
                                    } else {
                                        if (!property.Optional)
                                        {
                                            defaultText.Append(property.ScriptHandle + " " + propertyValue + "\r\n");

                                            errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                        }
                                    }
                                } else {
                                    if (!property.Optional)
                                    {
                                        defaultText.Append(property.ScriptHandle + " " + propertyValue + "\r\n");

                                        errorsTxt += "\r\nVersion config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                    }
                                }
                            }
                        }
                    }

                    if (errorsTxt.Length > 0)
                    {
                        MessageBox.Show(errorsTxt, "Version config warnings");
                    }

                    System.IO.FileInfo newWorldConfig = new System.IO.FileInfo(DestinationConfigsDir + "/WorldConfig.ini");
                    if(newWorldConfig.Exists)
                    {
                        newWorldConfig.Delete();
                    }
                    string fName = newWorldConfig.FullName;                
                    System.IO.File.WriteAllText(fName, defaultText.ToString());
                }
            }

            void GenerateBiomeConfigs(System.IO.DirectoryInfo sourceDirectory, System.IO.DirectoryInfo destinationDirectory, List<BiomeConfig> biomeConfigsDefaultValues, VersionConfig versionConfig)
            {
                if (!destinationDirectory.Exists) { destinationDirectory.Create(); }
                if (sourceDirectory.Exists)
                {
                    foreach (FileInfo file in destinationDirectory.GetFiles())
                    {
                        file.Delete();
                    }
                    string errorsTxt = "";
                    foreach (System.IO.FileInfo file in sourceDirectory.GetFiles())
                    {
                        BiomeConfig defaultBiome = biomeConfigsDefaultValues.FirstOrDefault(a => a.FileName == file.Name);
                        if (defaultBiome != null)
                        {
                            StringBuilder defaultText = new StringBuilder(System.IO.File.ReadAllText(file.FullName));
                            string sDefaultText = defaultText.ToString();

                            foreach (TCProperty property in versionConfig.BiomeConfig)
                            {
                                string value = "";
                                if (property.PropertyType == "BiomesList" || property.PropertyType == "ResourceQueue")
                                {
                                    if (!BiomeGroups.Any(a => a.Biomes.Any(b => b == defaultBiome.BiomeName) && a.BiomeConfig.Properties.First(c => c.PropertyName == property.Name).Override))
                                    {
                                        //Never overriden, set to default value
                                        value = defaultBiome.GetPropertyValueAsString(property);
                                    }
                                } else {
                                    value = defaultBiome.GetPropertyValueAsString(property);
                                }
                                bool bOverride = false;
                                bool bOverrideParentvalues = false;
                                bool bMerge = false;
                                foreach (Group biomeGroup in BiomeGroups)
                                {
                                    if (biomeGroup.Biomes.Any(a => a == defaultBiome.BiomeName))
                                    {
                                        if (biomeGroup.BiomeConfig.Properties.Any(a => a.PropertyName == property.Name) && biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override)
                                        {
                                            if (!biomeGroup.BiomeConfig.GetPropertyMerge(property))
                                            {
                                                bOverride = true;
                                            }
                                            bMerge = biomeGroup.BiomeConfig.GetPropertyMerge(property);
                                            bOverrideParentvalues = biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).OverrideParentValues;

                                            if (biomeGroup.BiomeConfig.GetPropertyOverrideParentValues(property) || (property.PropertyType != "BiomesList" && property.PropertyType != "ResourceQueue"))
                                            {
                                                if (!String.IsNullOrEmpty(biomeGroup.BiomeConfig.GetPropertyValueAsString(property)) || property.PropertyType == "String" || property.PropertyType == "BiomesList" || property.PropertyType == "ResourceQueue")
                                                {
                                                    value = biomeGroup.BiomeConfig.GetPropertyValueAsString(property);
                                                }
                                            }
                                            else if (property.PropertyType == "BiomesList" || property.PropertyType == "ResourceQueue")
                                            {
                                                if (property.PropertyType == "BiomesList" && biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override)
                                                {
                                                    if (value != null && !String.IsNullOrEmpty(value.Trim()))
                                                    {
                                                        value += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? ", " + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                    } else {
                                                        value += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                    }
                                                }
                                                else if (property.PropertyType == "ResourceQueue" && biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override)
                                                {
                                                    if (value != null && !String.IsNullOrEmpty(value.Trim()))
                                                    {
                                                        value += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? "\r\n" + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                    } else {
                                                        value += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                    }
                                                } else {
                                                    MessageBox.Show("One does not simply merge a non-BiomesList, non-ResourceQueue property. Property \"" + property.Name + "\" group \"" + biomeGroup.Name + "\". Biome generation aborted.", "Generation error");
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                                bMerge = (!bOverride || (bMerge && bOverrideParentvalues)) && bMerge;

                                if (property.PropertyType == "ResourceQueue")
                                {
                                    bool bFound = false;
                                    int replaceStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                                    if (replaceStartIndex > -1)
                                    {
                                        while (!bFound)
                                        {
                                            int lineStart = sDefaultText.Substring(replaceStartIndex).IndexOf("\n") + 1 + replaceStartIndex;
                                            int lineLength = sDefaultText.Substring(lineStart).IndexOf("\n") + 1;

                                            if (
                                                /* line doesnt contain # */
                                                !sDefaultText.Substring(lineStart, lineLength).Contains("#") &&
                                                (
                                                    /* and line isnt empty or is empty and is last line or next line starts with ## */
                                                    sDefaultText.Substring(lineStart, lineLength).Replace("\r", "").Replace("\n", "").Trim().Length > 0 ||
                                                    (
                                                        //if this is the last line
                                                        sDefaultText.Substring(lineStart + lineLength).IndexOf("\n") < 0 ||
                                                        //or next line starts with ##
                                                        (sDefaultText.Substring(lineStart + lineLength).IndexOf("##") > -1 && sDefaultText.Substring(lineStart + lineLength).IndexOf("##") < sDefaultText.Substring(lineStart + lineLength).IndexOf("\n"))
                                                    )
                                                )
                                            )
                                            {
                                                replaceStartIndex = lineStart;
                                                bFound = true;
                                            } else {
                                                replaceStartIndex = sDefaultText.Substring(replaceStartIndex).IndexOf("\n") + 1 + replaceStartIndex;
                                                if (replaceStartIndex < 0 || replaceStartIndex == sDefaultText.Length)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        if (replaceStartIndex > -1)
                                        {
                                            int replaceLength = 0;

                                            while (replaceStartIndex + replaceLength + 2 <= sDefaultText.Length && sDefaultText.Substring(replaceStartIndex + replaceLength, 2) != "\n#")
                                            {
                                                replaceLength += 1;
                                            }
                                            if (replaceStartIndex + replaceLength + 1 == sDefaultText.Length)
                                            {
                                                replaceLength += 1;
                                            }
                                            if (replaceLength > 0)
                                            {
                                                string[] biomesListItemNames = value != null ? value.Replace("\r", "").Split('\n') : null;
                                                string[] defaultBiomesListItemNames = defaultBiome.GetPropertyValueAsString(property) != null ? defaultBiome.GetPropertyValueAsString(property).Replace("\r", "").Split('\n') : null;
                                                List<string> newPropertyValue = new List<string>();

                                                if (defaultBiomesListItemNames != null && bMerge)
                                                {
                                                    foreach (string value1 in defaultBiomesListItemNames)
                                                    {
                                                        if(value1 != null && !string.IsNullOrEmpty(value1.Trim()))
                                                        {
                                                            //bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)value1) || MessageBox.Show("An item with the same value already exists, are you sure you want to add another?", "Allow duplicate?", MessageBoxButtons.OKCancel) == DialogResult.OK;
                                                            bool duplicatePermission = value1.StartsWith("CustomObject(") || !newPropertyValue.Any(a => (string)a == (string)value1);
                                                            //bool duplicatePermission = true;
                                                            if (duplicatePermission)
                                                            {
                                                                if (property.Name == "Mob spawning")
                                                                {
                                                                    newPropertyValue.Add(value1.Trim());
                                                                } else {
                                                                    if (value1.Contains('(') && value1.Contains(')'))
                                                                    {
                                                                        bool bFound3 = false;
                                                                        ResourceQueueItem selectedOption = null;
                                                                        foreach (ResourceQueueItem option in versionConfig.ResourceQueueOptions)
                                                                        {
                                                                            if (value1.StartsWith(option.Name))
                                                                            {
                                                                                bFound3 = true;
                                                                                selectedOption = option;
                                                                                break;
                                                                            }
                                                                        }
                                                                        if (bFound3)
                                                                        {
                                                                            if (selectedOption.IsUnique)
                                                                            {
                                                                                List<string> possibleDuplicates = new List<string>();
                                                                                foreach (string value2 in newPropertyValue)
                                                                                {
                                                                                    if (value2.StartsWith(selectedOption.Name))
                                                                                    {
                                                                                        possibleDuplicates.Add(value2);
                                                                                    }
                                                                                }
                                                                                if (possibleDuplicates.Any())
                                                                                {
                                                                                    if (selectedOption.HasUniqueParameter)
                                                                                    {
                                                                                        bool bFound4 = false;
                                                                                        foreach (string value3 in possibleDuplicates)
                                                                                        {
                                                                                            string[] parameters = value3.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                                                            string[] parameters2 = value1.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                                                            if (parameters.Length > selectedOption.UniqueParameterIndex && parameters2.Length > selectedOption.UniqueParameterIndex)
                                                                                            {
                                                                                                if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                                                                                {
                                                                                                    if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(parameters2[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                                                                    {
                                                                                                        if (parameters[selectedOption.UniqueParameterIndex].Trim() == parameters2[selectedOption.UniqueParameterIndex].Trim())
                                                                                                        {
                                                                                                            bFound4 = true;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (parameters[selectedOption.UniqueParameterIndex].Trim() == parameters2[selectedOption.UniqueParameterIndex].Trim())
                                                                                                    {
                                                                                                        bFound4 = true;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        if (!bFound4)
                                                                                        {
                                                                                            newPropertyValue.Add(value1.Trim());
                                                                                        }
                                                                                    }
                                                                                } else {
                                                                                    newPropertyValue.Add(value1.Trim());
                                                                                }
                                                                            } else {
                                                                                newPropertyValue.Add(value1.Trim());
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if(biomesListItemNames != null)
                                                {
                                                    foreach (string value1 in biomesListItemNames)
                                                    {
                                                        if (value1 != null && !string.IsNullOrEmpty(value1.Trim()))
                                                        {
                                                            //bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)value1) || MessageBox.Show("An item with the same value already exists, are you sure you want to add another?", "Allow duplicate?", MessageBoxButtons.OKCancel) == DialogResult.OK;
                                                            bool duplicatePermission = value1.StartsWith("CustomObject(") || !newPropertyValue.Any(a => (string)a == (string)value1);
                                                            //bool duplicatePermission = true;
                                                            if (duplicatePermission)
                                                            {
                                                                if(property.Name == "Mob spawning")
                                                                {
                                                                    newPropertyValue.Add(value1.Trim());
                                                                } else {
                                                                    if (value1.Contains('(') && value1.Contains(')'))
                                                                    {
                                                                        bool bFound3 = false;
                                                                        ResourceQueueItem selectedOption = null;
                                                                        foreach (ResourceQueueItem option in versionConfig.ResourceQueueOptions)
                                                                        {
                                                                            if(value1.StartsWith(option.Name))
                                                                            {
                                                                                bFound3 = true;
                                                                                selectedOption = option;
                                                                                break;
                                                                            }
                                                                        }
                                                                        if(bFound3)
                                                                        {
                                                                            if(selectedOption.IsUnique)
                                                                            {
                                                                                List<string> possibleDuplicates = new List<string>();
                                                                                foreach(string value2 in newPropertyValue)
                                                                                {
                                                                                    if(value2.StartsWith(selectedOption.Name))
                                                                                    {
                                                                                        possibleDuplicates.Add(value2);
                                                                                    }
                                                                                }
                                                                                if(possibleDuplicates.Any())
                                                                                {
                                                                                    if (selectedOption.HasUniqueParameter)
                                                                                    {
                                                                                        foreach (string value3 in possibleDuplicates)
                                                                                        {
                                                                                            string[] parameters = value3.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                                                            string[] parameters2 = value1.Replace(selectedOption.Name, "").Replace(")", "").Replace("\r", "").Replace("\n", "").Split(',');
                                                                                            if (parameters.Length > selectedOption.UniqueParameterIndex && parameters2.Length > selectedOption.UniqueParameterIndex)
                                                                                            {
                                                                                                if (selectedOption.UniqueParameterValues != null && selectedOption.UniqueParameterValues.Count > 0)
                                                                                                {
                                                                                                    if (selectedOption.UniqueParameterValues.Any(a => a.ToLower().Trim().Equals(parameters2[selectedOption.UniqueParameterIndex].ToLower().Trim())))
                                                                                                    {
                                                                                                        if (parameters[selectedOption.UniqueParameterIndex].Trim() == parameters2[selectedOption.UniqueParameterIndex].Trim())
                                                                                                        {
                                                                                                            newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name + parameters2[selectedOption.UniqueParameterIndex]));
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (parameters[selectedOption.UniqueParameterIndex].Trim() == parameters2[selectedOption.UniqueParameterIndex].Trim())
                                                                                                    {
                                                                                                        newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name + parameters2[selectedOption.UniqueParameterIndex]));
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        newPropertyValue.Add(value1.Trim());
                                                                                    } else {
                                                                                        newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name));
                                                                                        newPropertyValue.Add(value1.Trim());
                                                                                    }
                                                                                } else {
                                                                                    newPropertyValue.Add(value1.Trim());
                                                                                }
                                                                            } else {
                                                                                newPropertyValue.Add(value1.Trim());
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                value = "";
                                                int i = 0;
                                                foreach (string value1 in newPropertyValue)
                                                {
                                                    value += (i != newPropertyValue.Count() - 1 ? value1 + "\r\n" : value1);
                                                    i++;
                                                }
                                            }
                                            if(replaceLength > 0)
                                            {
                                                defaultText = defaultText.Remove(replaceStartIndex, replaceLength);
                                            }
                                            defaultText = defaultText.Insert(replaceStartIndex, value + "\r\n");
                                            sDefaultText = defaultText.ToString();
                                        } else {

                                            if (!property.Optional)
                                            {
                                                defaultText.Append(property.ScriptHandle + " " + value + "\r\n");

                                                errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                            }

                                            //MessageBox.Show("Version config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                            //return;
                                        }
                                    } else {

                                        if (!property.Optional)
                                        {
                                            defaultText.Append(property.ScriptHandle + " " + value + "\r\n");

                                            errorsTxt += "\r\nVersion config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                        }
                                        //MessageBox.Show("Version config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                        //return;
                                    }
                                } else {
                                    bool bFound = false;
                                    int valueStringStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                                    if (valueStringStartIndex > -1)
                                    {
                                        while (!bFound)
                                        {
                                            if (sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("\n") > sDefaultText.Substring(0, valueStringStartIndex).LastIndexOf("#"))
                                            {
                                                bFound = true;
                                            } else {
                                                valueStringStartIndex = sDefaultText.Substring(valueStringStartIndex + property.ScriptHandle.Length).ToLower().IndexOf(property.ScriptHandle.ToLower()) + valueStringStartIndex + property.ScriptHandle.Length;
                                                if (valueStringStartIndex < 0)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        if (valueStringStartIndex > -1)
                                        {
                                            int skipCharsLength = (property.ScriptHandle).Length;
                                            int valueStringLength = 0;

                                            while (sDefaultText.Substring(valueStringStartIndex + skipCharsLength + valueStringLength, 1) != "\n")
                                            {
                                                valueStringLength += 1;
                                            }

                                            if (property.PropertyType == "BiomesList" && bMerge && valueStringLength > 0)
                                            {
                                                string[] biomesListItemNames = value != null ? value.Split(',') : null;
                                                string[] defaultBiomesListItemNames = defaultBiome.GetPropertyValueAsString(property) != null ? defaultBiome.GetPropertyValueAsString(property).Split(',') : null;
                                                List<string> newPropertyValue = new List<string>();
                                                if(biomesListItemNames != null)
                                                {
                                                    foreach (string value1 in biomesListItemNames)
                                                    {
                                                        if (!newPropertyValue.Any(a => (string)a == (string)value1) && value1 != null && !string.IsNullOrEmpty(value1.Trim()))
                                                        {
                                                            newPropertyValue.Add(value1.Trim());
                                                        }
                                                    }
                                                }
                                                if (defaultBiomesListItemNames != null)
                                                {
                                                    foreach (string value1 in defaultBiomesListItemNames)
                                                    {
                                                        if (!newPropertyValue.Any(a => (string)a == (string)value1) && value1 != null && !string.IsNullOrEmpty(value1.Trim()))
                                                        {
                                                            newPropertyValue.Add(value1.Trim());
                                                        }
                                                    }
                                                }

                                                value = "";
                                                foreach (string value1 in newPropertyValue)
                                                {
                                                    value += (value1 != newPropertyValue[newPropertyValue.Count() - 1] ? value1 + ", " : value1);
                                                }
                                            }
                                            if (valueStringLength > 0)
                                            {
                                                defaultText = defaultText.Remove(valueStringStartIndex + skipCharsLength, valueStringLength);
                                            }
                                            defaultText = defaultText.Insert(valueStringStartIndex + skipCharsLength, " " + value);
                                            sDefaultText = defaultText.ToString();
                                        } else {

                                            if(!property.Optional)
                                            {
                                                defaultText.Append(property.ScriptHandle + " " + value + "\r\n");

                                                errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                            }

                                            //MessageBox.Show("Version config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                            //return;
                                        }
                                    } else {
                                        //TODO: solve ReplaceToBiomeName problem!
                                        //if (property.Name != "ReplaceToBiomeName")// && property.Name != "BiomeSizeWhenIsle" && property.Name != "BiomeRarityWhenIsle" && property.Name != "BiomeSizeWhenBorder")
                                        //{
                                            //MessageBox.Show("Version config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                            //return;
                                        //}

                                        if (!property.Optional)
                                        {
                                            defaultText.Append(property.ScriptHandle + " " + value + "\r\n");

                                            errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                        }
                                    }
                                }
                            }

                            string fName = destinationDirectory.FullName + "/" + file.Name;
                            System.IO.File.WriteAllText(fName, defaultText.ToString());
                        }
                    }
                    if (errorsTxt.Length > 0)
                    {
                        MessageBox.Show(errorsTxt, "Version config warnings");
                    }
                }
            }

            public static class PopUpForm
            {
                public static string SingleSelectListBox(List<string> listItems, string title = "", string labelText = "")
                {
                    int margin = 4;

                    Form form = new Form();
                    form.Text = title;

                    Label labelListBox = new Label();
                    labelListBox.Text = labelText;
                    labelListBox.AutoEllipsis = true;
                    labelListBox.SetBounds(9, 8, 372, 20);

                    ListBox listBox = new ListBox();
                    listBox.SetBounds(12, labelListBox.Location.Y + labelListBox.Height + margin, 372, 70);
                    listBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                    listBox.SelectionMode = SelectionMode.One;
                    foreach (string item in listItems)
                    {
                        listBox.Items.Add(item);
                    }

                    Button buttonOk = new Button();
                    buttonOk.Text = "OK";
                    buttonOk.Click += new EventHandler
                    (
                        delegate
                        {
                            PopupFormSelectedItem = (string)listBox.SelectedItem;
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                            form.Dispose();
                        }
                    );
                    buttonOk.SetBounds(228, listBox.Location.Y + listBox.Height + margin, 75, 23);
                    buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                    Button buttonCancel = new Button();
                    buttonCancel.Text = "Cancel";
                    buttonCancel.DialogResult = DialogResult.Cancel;
                    buttonCancel.SetBounds(309, listBox.Location.Y + listBox.Height + margin, 75, 23);
                    buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                    form.ClientSize = new Size(396, buttonCancel.Location.Y + buttonCancel.Height + margin);
                    form.Controls.AddRange(new Control[] { labelListBox, listBox, buttonOk, buttonCancel });
                    form.ClientSize = new Size(Math.Max(300, labelListBox.Right + 10), form.ClientSize.Height);
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.AcceptButton = buttonOk;
                    form.CancelButton = buttonCancel;

                    string selection = null;
                    DialogResult dialogResult = form.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        selection = PopupFormSelectedItem;
                    }
                    return selection;
                }

                private static string PopupFormSelectedItem;

                public static List<string> BiomeListSelectionBox(ref string groupName, List<string> listItems)
                {
                    int margin = 4;

                    Form form = new Form();
                    form.Text = "Create group";

                    Label label = new Label();
                    label.Text = "Enter a name for the group. Only a-z A-Z 0-9 space + - and _ are allowed.";
                    label.SetBounds(9, 20, 372, 13);

                    TextBox textBox = new TextBox();
                    textBox.Text = groupName;
                    textBox.SetBounds(12, 36, 372, 20);
                    textBox.Anchor = textBox.Anchor | AnchorStyles.Right;

                    Label labelListBox = new Label();
                    labelListBox.Text = "Use SHIFT and CTRL to select biomes to add to the new group. If a single biome is selected and the name field is left empty then that biome's name wil automatically be used as the group name.";
                    labelListBox.AutoEllipsis = true;
                    labelListBox.SetBounds(9, textBox.Location.Y + textBox.Height + margin, 372, 45);

                    ListBox listBox = new ListBox();
                    listBox.SetBounds(12, labelListBox.Location.Y + labelListBox.Height + margin, 372, 200);
                    listBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                    listBox.SelectionMode = SelectionMode.MultiExtended;
                    foreach(string item in listItems)
                    {
                        listBox.Items.Add(item);
                    }

                    Button buttonOk = new Button();
                    buttonOk.Text = "OK";
                    buttonOk.Click += new EventHandler
                    (
                        delegate
                        {
                            if(listBox.SelectedIndices.Count < 1)
                            {
                                MessageBox.Show("Select at least one biome.");
                            } else {
                                if (!string.IsNullOrWhiteSpace(textBox.Text) || listBox.SelectedIndices.Count == 1)
                                {
                                    if(listBox.SelectedIndices.Count > 1)
                                    {
                                        //if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                        if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_ -+]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                        {
                                            PopupFormSelectedItem = textBox.Text;
                                            form.DialogResult = DialogResult.OK;
                                            form.Close();
                                            form.Dispose();
                                        } else {
                                            MessageBox.Show("Name contains illegal characters.", "Warning: Illegal input");
                                        }
                                    } else {
                                        PopupFormSelectedItem = textBox.Text.Length > 0 ? textBox.Text : (string)listBox.SelectedItem;
                                        form.DialogResult = DialogResult.OK;
                                        form.Close();
                                        form.Dispose();
                                    }
                                } else {
                                    MessageBox.Show("More than one biome has been selected, name cannot be empty.", "Warning: Illegal input");
                                }
                            }
                        }
                    );
                    buttonOk.SetBounds(228, listBox.Location.Y + listBox.Height + margin, 75, 23);
                    buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                    Button buttonCancel = new Button();
                    buttonCancel.Text = "Cancel";
                    buttonCancel.DialogResult = DialogResult.Cancel;
                    buttonCancel.SetBounds(309, listBox.Location.Y + listBox.Height + margin, 75, 23);
                    buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                    form.ClientSize = new Size(396, buttonCancel.Location.Y + buttonCancel.Height + margin);
                    form.Controls.AddRange(new Control[] { label, textBox, labelListBox, listBox, buttonOk, buttonCancel });
                    form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.AcceptButton = buttonOk;
                    form.CancelButton = buttonCancel;

                    DialogResult dialogResult = form.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        groupName = PopupFormSelectedItem;
                        List<string> selectedBiomes = new List<string>();
                        foreach (int selectedIndex in listBox.SelectedIndices)
                        {
                            selectedBiomes.Add((string)listBox.Items[selectedIndex]);
                        }
                        return selectedBiomes;
                    } else {
                        groupName = null;
                        return null;
                    }
                }

                public static DialogResult InputBox(string title, string promptText, ref string value, bool allowBracesCommasDotsColons = false)
                {
                    Form form = new Form();
                    Label label = new Label();
                    TextBox textBox = new TextBox();
                    Button buttonOk = new Button();
                    Button buttonCancel = new Button();

                    form.Text = title;
                    label.Text = promptText;
                    textBox.Text = value;

                    buttonOk.Text = "OK";
                    buttonCancel.Text = "Cancel";
                    buttonOk.DialogResult = DialogResult.OK;
                    buttonCancel.DialogResult = DialogResult.Cancel;

                    label.SetBounds(9, 20, 372, 13);
                    textBox.SetBounds(12, 36, 372, 20);
                    buttonOk.SetBounds(228, 72, 75, 23);
                    buttonCancel.SetBounds(309, 72, 75, 23);

                    label.AutoSize = true;
                    textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
                    buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                    buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                    form.ClientSize = new Size(396, 107);
                    form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
                    form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.AcceptButton = buttonOk;
                    form.CancelButton = buttonCancel;

                    buttonOk.Click += new EventHandler
                    (
                        delegate
                        {
                            if (!string.IsNullOrWhiteSpace(textBox.Text))
                            {
                                if (!allowBracesCommasDotsColons ? System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_ -+]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase) : System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_(),.: -]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                {
                                    PopupFormSelectedItem = textBox.Text;
                                    form.DialogResult = DialogResult.OK;
                                    form.Close();
                                    form.Dispose();
                                } else {
                                    MessageBox.Show("Name contains illegal characters.", "Warning: Illegal input");
                                }
                            } else {
                                MessageBox.Show("Name cannot be empty.", "Warning: Illegal input");
                            }
                        }
                    );

                    DialogResult dialogResult = form.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        value = PopupFormSelectedItem;
                    } else {
                        value = null;
                    }
                    return dialogResult;
                }

                public static DialogResult CustomOkCancelBox(string title, string promptText, string okText, string cancelText)
                {
                    Form form = new Form();
                    Label label = new Label();
                    Button buttonOk = new Button();
                    Button buttonCancel = new Button();

                    form.Text = title;
                    label.Text = promptText;

                    buttonOk.Text = okText;
                    buttonCancel.Text = cancelText;
                    buttonOk.DialogResult = DialogResult.OK;
                    buttonCancel.DialogResult = DialogResult.Cancel;

                    label.SetBounds(9, 20, 372, 13);
                    buttonOk.SetBounds(228, 72, 75, 23);
                    buttonCancel.SetBounds(309, 72, 75, 23);

                    label.AutoSize = true;
                    buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                    buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                    form.ClientSize = new Size(396, 107);
                    form.Controls.AddRange(new Control[] { label, buttonOk, buttonCancel });
                    form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.AcceptButton = buttonOk;
                    form.CancelButton = buttonCancel;

                    DialogResult dialogResult = form.ShowDialog();
                    return dialogResult;
                }
            }

            public bool CompareBiomeLists(string biomeList, string biomeList2)
            {
                string[] biomesListItemNames2 = biomeList != null ? biomeList.Split(',') : null;
                string[] defaultBiomesListItemNames2 = biomeList2 != null ? biomeList2.Split(',') : null;
                if (biomesListItemNames2 != null && defaultBiomesListItemNames2 != null && defaultBiomesListItemNames2.Length == biomesListItemNames2.Length)
                {
                    for (int i = 0; i < biomesListItemNames2.Length; i++)
                    {
                        bool bFound2 = false;
                        for (int i2 = 0; i2 < defaultBiomesListItemNames2.Length; i2++)
                        {
                            if (defaultBiomesListItemNames2[i2].Trim().Equals(biomesListItemNames2[i].Trim()))
                            {
                                bFound2 = true;
                                break;
                            }
                        }
                        if (!bFound2)
                        {
                            return false;
                        }
                    }
                } else {
                    if (biomesListItemNames2 != null || defaultBiomesListItemNames2 != null)
                    {
                        return false;
                    }
                }
                return true;
            }

            public bool CompareResourceQueues(string resourceQueue, string resourceQueue2)
            {
                string[] resourceQueueItemNames2 = resourceQueue.Replace("\r", "").Split('\n');
                string[] defaultResourceQueueItemNames2 = resourceQueue2.Replace("\r", "").Split('\n');
                if (defaultResourceQueueItemNames2.Length == resourceQueueItemNames2.Length)
                {
                    for (int i = 0; i < resourceQueueItemNames2.Length; i++)
                    {
                        bool bFound2 = false;
                        for (int i2 = 0; i2 < defaultResourceQueueItemNames2.Length; i2++)
                        {
                            if (defaultResourceQueueItemNames2[i2].Trim().Equals(resourceQueueItemNames2[i].Trim()))
                            {
                                bFound2 = true;
                                break;
                            }
                        }
                        if (!bFound2)
                        {
                            return false;
                        }
                    }
                } else {
                    return false;
                }
                return true;
            }

        #endregion

        # region Import world

            // User clicks button, selects default biomes version, then selects world directory               
            // TCEE creates new world in VersionConfigs dir if there are custom biomes and/or BO3's
            // TCEE creates a save with a group for each biome in the world (unless all its values are defaults)

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void btImportWorld_Click(object sender, EventArgs e)
            {
                // Show select menu and let user select version
                string versionName = SelectVersion();

                fbdDestinationWorldDir.Description = "Select a TerrainControl world folder. Any BO2's and BO3's that should be imported with this world must be placed in the world's WorldObjects folder (create if needed).";

                // Make user select world
                if (versionName != null && fbdDestinationWorldDir.ShowDialog() == DialogResult.OK)
                {
                    // Get world's resource directories

                    DirectoryInfo sourceWorldDir = new DirectoryInfo(fbdDestinationWorldDir.SelectedPath);
                    DirectoryInfo worldBiomesDir = sourceWorldDir.GetDirectories().FirstOrDefault(a => a.Name.ToLower() == "worldbiomes");
                    DirectoryInfo worldObjectsDir = sourceWorldDir.GetDirectories().FirstOrDefault(a => a.Name.ToLower() == "worldobjects");

                    List<BiomeConfig> worldBiomes = new List<BiomeConfig>();

                    if (worldBiomesDir != null || worldObjectsDir != null)
                    {
                        DirectoryInfo versionDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/");
                        if (versionDir.Exists)
                        {
                            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(VersionConfig));
                            
                            VersionConfig versionConfig = null;
                            string path = Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + versionName + "/VersionConfig.xml";
                            if(File.Exists(path))
                            {
                                using (var reader = new XmlTextReader(path))
                                {
                                    versionConfig = (VersionConfig)xmlSerializer.Deserialize(reader);
                                }
                                if (versionConfig != null)
                                {
                                    List<string> msgs = new List<string>();

                                    // Load selected world's resources

                                    // Load worldconfig
                                    WorldConfig worldConfig = new WorldConfig(versionConfig);
                                    FileInfo worldConfigFile = new FileInfo(sourceWorldDir + "/WorldConfig.ini");
                                    if (worldConfigFile.Exists)
                                    {
                                        try
                                        {
                                            worldConfig = LoadWorldConfigFromFile(worldConfigFile, versionConfig, false);
                                        }
                                        catch(InvalidDataException ex)
                                        {
                                            return;
                                        }
                                        if(worldConfig != null)
                                        {
                                            msgs.Add("WorldConfig found");
                                        }
                                    }

                                    DirectoryInfo biomesDir = worldBiomesDir != null && worldBiomesDir.Exists ? worldBiomesDir : null;

                                    // Load biomeconfigs
                                    List<BiomeConfig> biomeConfigs = new List<BiomeConfig>();
                                    if (biomesDir != null)// || biomeConfigsDirPresent != null)
                                    {
                                        foreach (FileInfo biomeFile in biomesDir.GetFiles().ToList())
                                        {
                                            try
                                            {
                                                BiomeConfig biomeConfig = LoadBiomeConfigFromFile(biomeFile, versionConfig, false);
                                                if (biomeConfig != null)
                                                {
                                                    biomeConfigs.Add(biomeConfig);
                                                }
                                            }
                                            catch(InvalidDataException ex)
                                            {
                                                return;
                                            }
                                        }
                                        msgs.Add(biomeConfigs.Count + " biomes found");
                                    }

                                    // Copy world objects dir
                                    //if (worldObjectsDir != null)
                                    //{
                                    //
                                    //}

                                    // Load default resources for the selected version of TC

                                    // Load worldConfig
                                    WorldConfig defaultWorldConfig = new WorldConfig(versionConfig);
                                    FileInfo defaultWorldConfigFile = new FileInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + versionName + "/Worlds/Default/WorldConfig.ini");
                                    if (defaultWorldConfigFile.Exists)
                                    {
                                        try
                                        {
                                            defaultWorldConfig = LoadWorldConfigFromFile(defaultWorldConfigFile, versionConfig, false);
                                        }
                                        catch(InvalidDataException ex)
                                        {
                                            return;
                                        }
                                        if (defaultWorldConfig != null)
                                        {
                                            msgs.Add("Default WorldConfig found");
                                        }
                                    } else {
                                        throw new Exception("WorldConfig.ini could not be loaded. Please make sure that WorldConfig.ini is present in the TCVersionConfig directory for the selected version. Exiting TCEE.");
                                    }

                                    // Load biomeconfigs
                                    List<BiomeConfig> defaultBiomeConfigs = new List<BiomeConfig>();
                                    DirectoryInfo defaultBiomesDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + versionName + "/Worlds/Default/" + "WorldBiomes");
                                    foreach (FileInfo biomeFile in defaultBiomesDir.GetFiles().ToList())
                                    {
                                        try
                                        {
                                            BiomeConfig biomeConfig = LoadBiomeConfigFromFile(biomeFile, versionConfig, false);
                                            if (biomeConfig != null)
                                            {
                                                defaultBiomeConfigs.Add(biomeConfig);
                                            }
                                        }
                                        catch(InvalidDataException ex)
                                        {
                                            return;
                                        }
                                    }
                                    msgs.Add(defaultBiomeConfigs.Count + " default biomes found");

                                    if (msgs.Count > 0)
                                    {
                                        string msgboxmsg = "";
                                        foreach (string msg in msgs)
                                        {
                                            msgboxmsg += msg + "\r\n";
                                        }
                                        MessageBox.Show(msgboxmsg);
                                    }

                                    bool useDefaults = MessageBox.Show("Use TC biome default values for this world and save any non-default values to a TCEE save file? Select No to use the imported world's values as default values instead. If you are unsure, just click No.", "Use TC default values?", MessageBoxButtons.YesNo) == DialogResult.Yes;

                                    bool isDefaultWorld = worldObjectsDir == null;

                                    // Check which settings in WorldConfig are different from default, save them as TCEE pre-set. If this world turns out to be non-compatible with default world then create new world folder and copy default worldconfig to it
                                    foreach(TCProperty property in versionConfig.WorldConfig)
                                    {
                                        if (
                                            worldConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Value != defaultWorldConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Value
                                        )
                                        {
                                            worldConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Override = true;
                                        } else {
                                            worldConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Override = false;
                                        }
                                    }
                                
                                    // Check which biomeconfigs are standard/nonstandard and for standard biomes check which values are not defaults, save those values as TCEE pre-set. 
                                    // If this world turns out to be non-compatible with default world then create new world folder and copy default biomeconfigs + non-standard biomeconfigs to it
                                    // For non-standard biomes get values from inherited biome?

                                    List<Group> groups = new List<Group>();
                                    if (useDefaults)
                                    {
                                        foreach(BiomeConfig biomeConfig in biomeConfigs)
                                        {
                                            if(!defaultBiomeConfigs.Any(a => a.BiomeName == biomeConfig.BiomeName))
                                            {
                                                isDefaultWorld = false;
                                            } else {
                                                BiomeConfig biomeDefaultConfig = defaultBiomeConfigs.FirstOrDefault(a => a.BiomeName == biomeConfig.BiomeName);
                                                bool hasDefaultValues = true;
                                                foreach (TCProperty property in versionConfig.BiomeConfig)
                                                {
                                                    if (
                                                        biomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Value != biomeDefaultConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Value
                                                    )
                                                    {
                                                        biomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Override = true;
                                                        hasDefaultValues = false;
                                                    } else {
                                                        biomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Override = false;
                                                    }
                                                }
                                                if(!hasDefaultValues)
                                                {
                                                    groups.Add(new Group(biomeConfig.BiomeName, new List<string>() { biomeConfig.BiomeName }, biomeConfig));
                                                }
                                            }
                                        }
                                    }

                                    sfd.Title = "Enter a world name and click save";

                                    // 
                                    sfd.CheckFileExists = false;
                                    if (sfd.ShowDialog() == DialogResult.OK)
                                    {
                                        sfd.Title = "File selection / creation";

                                        DirectoryInfo destinationWorldDirectory = new DirectoryInfo(sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("\\") + 1));
                                        if(destinationWorldDirectory.Exists)
                                        {
                                            SettingsFile settingsFile = new SettingsFile()
                                            {
                                                WorldConfig = worldConfig,
                                                BiomeGroups = groups
                                            };

                                            var dataContractXMLSerializer = new DataContractSerializer(typeof(SettingsFile));
                                            string xmlString;
                                            using (var sw = new StringWriter())
                                            {
                                                using (var writer = new XmlTextWriter(sw))
                                                {
                                                    writer.Formatting = Formatting.Indented; // indent the Xml so it's human readable
                                                    dataContractXMLSerializer.WriteObject(writer, settingsFile);
                                                    writer.Flush();
                                                    xmlString = sw.ToString();
                                                }
                                            }
                                            System.IO.File.WriteAllText(sfd.FileName, xmlString);
                                            MessageBox.Show("Settings saved as: " + sfd.FileName, "Settings saved");

                                            // Copy default WorldConfig, BiomeConfigs and WorldObjects to world output dir if the pre-set is not compatible with the default world
                                            if (!isDefaultWorld || !useDefaults)
                                            {
                                                FileInfo presetFileName = new FileInfo(sfd.FileName);

                                                DirectoryInfo destinationDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + versionName + "/Worlds/" + presetFileName.Name.Replace(".xml","") + "/");
                                                if (!destinationDir.Exists)
                                                {
                                                    destinationDir.Create();
                                                }

                                                // Copy world config
                                                defaultWorldConfigFile.CopyTo(destinationDir.FullName + "/WorldConfig.ini", true);

                                                // Copy BO2's/BO3's
                                                if (worldObjectsDir != null)
                                                {
                                                    //DirectoryInfo destinationWorldObjectsDir = new DirectoryInfo(destinationWorldDirectory.FullName + "/WorldObjects");
                                                    DirectoryInfo destinationWorldObjectsDir = new DirectoryInfo(destinationDir.FullName + "/WorldObjects");
                                                    if (!destinationWorldObjectsDir.Exists)
                                                    {
                                                        destinationWorldObjectsDir.Create();
                                                    }

                                                    System.Windows.Forms.MessageBox.Show("Copying BO3s to output directory, this can take a while depending on the number and size of the BO3s!", "Copying BO3s");
                                                    System.Security.AccessControl.DirectorySecurity sec = destinationWorldObjectsDir.GetAccessControl();
                                                    System.Security.AccessControl.FileSystemAccessRule accRule = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                                                    sec.AddAccessRule(accRule);

                                                    CopyDir.CopyAll(worldObjectsDir, destinationWorldObjectsDir);
                                                }

                                                // Copy Biomes
                                                if (biomesDir != null)
                                                {
                                                    // Copy default biomes
                                                    //GenerateBiomeConfigs(defaultBiomesDir, new DirectoryInfo(destinationWorldDirectory.FullName + settingsType.BiomesDirectory), defaultBiomeConfigs, versionConfig);
                                                    if (!useDefaults)
                                                    {
                                                        GenerateBiomeConfigs(defaultBiomesDir, new DirectoryInfo(destinationDir.FullName + "/" + "WorldBiomes"), defaultBiomeConfigs, versionConfig);
                                                    }

                                                    // Copy non-default biomes
                                                    List<string> biomesToExport = new List<string>();
                                                    foreach(BiomeConfig biomeConfig in biomeConfigs)
                                                    {
                                                        if(!useDefaults || !defaultBiomeConfigs.Any(a => a.BiomeName == biomeConfig.BiomeName))
                                                        {

                                                            FileInfo biomeConfigFile = new FileInfo(biomesDir.FullName + "/" + biomeConfig.FileName);
                                                            biomeConfigFile.CopyTo(destinationDir.FullName + "/" + "WorldBiomes" + "/" + biomeConfig.FileName, true);
                                                        }
                                                    }
                                                }

                                                // Force refresh of available worlds list
                                                cbWorld.Items.Clear();
                                                DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + cbVersion.SelectedItem + "/Worlds/");
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

                                                System.Windows.Forms.MessageBox.Show("World import completed, you can now select the world and load your TCEE save.", "Import completed");
                                            }
                                        } else {
                                            MessageBox.Show("Derp! 1");
                                        }
                                    }
                                } else {
                                    MessageBox.Show("Y u do dis? :(");
                                }
                            } else {
                                MessageBox.Show("Could not find ");
                            }
                        }
                    } else {
                        MessageBox.Show("Could not find WorldBiomes or WorldObjects directory. At least one is required.");
                    }
                }

                // Create versionconfig and save files for world
                ImportWorld(versionName);

                // Load world with save using existing methods
            }

            /// <summary>
            /// Shows select menu and let user select version
            /// </summary>
            private string SelectVersion()
            {                 
                // Get available versions from TCVersionConfigs
                List<string> versions = VersionDir.GetDirectories().Select(a => a.Name).ToList();
                return PopUpForm.SingleSelectListBox(versions, "Select a TC version", "Which version of TC will you use with this world?");
            }

            /// <summary>
            /// Create versionconfig and save files for world
            /// </summary>
            /// <param name="versionName"></param>
            private void ImportWorld(string versionName)
            {
                // Get biomes and bo2's/bo3's for the world and create TCVersionConfig directory if necessary
                DirectoryInfo versionDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + versionName);

                // Create a save with a group for each biome in the world (unless all its values are defaults)

            }

        # endregion

        #region Music

            bool playerStopped = true;
            WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();

            private void label5_Click(object sender, EventArgs e)
            {
                if(((Label)sender).ForeColor == Color.FromKnownColor(KnownColor.Maroon))
                {
                    ((Label)sender).ForeColor = Color.FromKnownColor(KnownColor.DarkGreen);
                    ((Label)sender).Text = "on";
                    playerStopped = false;
                    player.controls.play();
                }
                else if (((Label)sender).ForeColor == Color.FromKnownColor(KnownColor.DarkGreen))
                {
                    ((Label)sender).ForeColor = Color.FromKnownColor(KnownColor.Maroon);
                    ((Label)sender).Text = "off";
                    playerStopped = true;
                    player.controls.stop();
                }
            }

        #endregion

        OpenFileDialog convertBO3ofd;
        FolderBrowserDialog convertBO3fbd;
        private void btnConvertSchematicToBO3_Click(object sender, EventArgs e)
        {
            if (convertBO3ofd.ShowDialog() == DialogResult.OK)
            {
                if (convertBO3ofd.FileNames.Length > 0 && convertBO3fbd.ShowDialog() == DialogResult.OK)
                {
                    bool exportForTC = PopUpForm.CustomOkCancelBox("Export for TC/MCW", "Do you want to export BO3s for TerrainControl (TC) or Minecraft Worlds (MCW)?", "TC", "MCW") == DialogResult.OK;

                    System.Windows.Forms.MessageBox.Show("Converting schematics to BO3s, this can take a while depending on the number and size of the schematics!", "Converting schematics to BO3s");
                    int converted = 0;
                    foreach (String fileName in convertBO3ofd.FileNames)
                    {
                        if (fileName.ToLower().EndsWith(".schematic"))
                        {
                            converted += 1;
                            SchematicToBO3.doSchematicToBO3(new FileInfo(fileName), new DirectoryInfo(convertBO3fbd.SelectedPath), exportForTC);
                        }
                    }
                    System.Windows.Forms.MessageBox.Show(converted + " schematics were converted to BO3s and saved at " + convertBO3fbd.SelectedPath, "Converting schematics to BO3s");
                }
            }
        }

        private void Link_Clicked(object sender, System.Windows.Forms.LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        FolderBrowserDialog copyBO3fbd;
        private void btCopyBO3s_Click(object sender, EventArgs e)
        {
            if (copyBO3fbd.ShowDialog() == DialogResult.OK)
            {
                bool go = true;
                if (!copyBO3fbd.SelectedPath.EndsWith("GlobalObjects") && !copyBO3fbd.SelectedPath.EndsWith("WorldObjects"))
                {
                    go = MessageBox.Show("The selected directory was not a GlobalObjects or WorldObjects directory, are you sure you want to copy here?", "Copy BO3s here?", MessageBoxButtons.OKCancel) == DialogResult.OK;
                }
                if (go)
                {
                    System.Windows.Forms.MessageBox.Show("Copying BO3s to output directory, this can take a while depending on the number and size of the BO3s!", "Copying BO3s");
                    System.IO.DirectoryInfo SourceWorldDirectory = new System.IO.DirectoryInfo(SourceConfigsDir + "/WorldObjects");
                    System.IO.DirectoryInfo DestinationWorldDirectory = new System.IO.DirectoryInfo(copyBO3fbd.SelectedPath);
                    int copied = 0;
                    if (SourceWorldDirectory.Exists)
                    {
                        if(!DestinationWorldDirectory.Exists)
                        {
                            DestinationWorldDirectory.Create();
                        }

                        System.Security.AccessControl.DirectorySecurity sec = DestinationWorldDirectory.GetAccessControl();
                        System.Security.AccessControl.FileSystemAccessRule accRule = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                        sec.AddAccessRule(accRule);

                        CopyDir.CopyAll(SourceWorldDirectory, DestinationWorldDirectory);
                    }
                    System.Windows.Forms.MessageBox.Show("All BO3s were copied to " + copyBO3fbd.SelectedPath, "Copying BO3s");
                }
            }
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        void lbPropertyInput_KeyDown_World(object sender, KeyEventArgs e)
        {
            TCProperty property = WorldSettingsInputs.FirstOrDefault(a => a.Value.Item1 == sender).Key;

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
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true; 

                ListBox lb = (ListBox)sender;
                String[] clipBoardStrings = Clipboard.GetText().Replace("\r\n", "/").Split('/');
                foreach (String selectedItem in clipBoardStrings)
                {
                    AddToResourceQueueWorld(property, selectedItem, false);
                }
            }
            if(e.KeyCode == Keys.Delete)
            {
                ListBox lb = ((ListBox)WorldSettingsInputs[property].Item1);
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

        void lbPropertyInput_KeyDown(object sender, KeyEventArgs e)
        {
            TCProperty property = BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item1 == sender).Key;

            if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = true;

                ListBox lb = (ListBox)sender;
                String clipBoardString = "";
                foreach(String selectedItem in lb.SelectedItems)
                {
                    clipBoardString += selectedItem + "\r\n";
                }
                clipBoardString = clipBoardString.Substring(0, clipBoardString.Length - 1); // Remove trailing "/"
                Clipboard.SetText(clipBoardString);
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true;

                ListBox lb = (ListBox)sender;
                String[] clipBoardStrings = Clipboard.GetText().Replace("\r\n","/").Split('/');
                foreach (String selectedItem in clipBoardStrings)
                {
                    AddToResourceQueue(property, selectedItem, false);
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                ListBox lb = ((ListBox)BiomeSettingsInputs[property].Item1);
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

    class CopyDir
    {
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }

    #region XML Classes

        [DataContract]
        public class SettingsFile
        {
            [DataMember]
            public WorldConfig WorldConfig;
            [DataMember]
            public List<Group> BiomeGroups = new List<Group>();
        }

        [DataContract]
        public class Group
        {
            [DataMember]
            public string Name;
            [DataMember]
            public List<string> Biomes = new List<string>();
            [DataMember]
            public BiomeConfig BiomeConfig;

            public bool showDefaults { get { return Biomes.Count == 1 && Name.Equals(Biomes[0]); } }

            public Group(string name, VersionConfig config)
            {
                if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be an empty or only spaces");
                Name = name;
                BiomeConfig = new BiomeConfig(config);
            }

            public Group(string name, List<string> biomes, BiomeConfig biomeConfig)
            {
                if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be an empty or only spaces");
                Name = name;
                Biomes = biomes;
                BiomeConfig = biomeConfig;
            }
        }

        [DataContract]
        public class WorldConfig
        {
            [DataMember]
            public List<Property> Properties;

            public WorldConfig(VersionConfig config)
            {
                Properties = new List<Property>();
                foreach (TCProperty property in config.WorldConfig)
                {
                    Properties.Add(new Property(null, false, property.Name, false, property.PropertyType != "BiomesList" && property.PropertyType != "ResourceQueue", property.Optional));
                }
            }

            public void SetProperty(TCProperty property, string value, bool merge, bool overrideParentValues)
            {
                if (property.PropertyType != "String" && property.PropertyType != "BiomesList" && property.PropertyType != "ResourceQueue" && String.IsNullOrWhiteSpace(value))
                {
                    value = null;
                }
                if (value != null)
                {
                    value = value.Trim();
                }
                Properties.First(a => a.PropertyName == property.Name).Value = value;
                Properties.First(a => a.PropertyName == property.Name).Merge = merge;
                Properties.First(a => a.PropertyName == property.Name).OverrideParentValues = overrideParentValues;
            }

            public string GetPropertyValueAsString(TCProperty property)
            {
                Property prop = Properties.FirstOrDefault(a => a.PropertyName == property.Name);
                return prop != null ? prop.Value : null;
            }

            public bool GetPropertyMerge(TCProperty property)
            {
                Property prop = Properties.FirstOrDefault(a => a.PropertyName == property.Name);
                return prop != null ? prop.Merge : false;
            }

            public bool GetPropertyOverrideParentValues(TCProperty property)
            {
                Property prop = Properties.FirstOrDefault(a => a.PropertyName == property.Name);
                return prop != null ? prop.OverrideParentValues : false;
            }
        }

        [DataContract]
        public class BiomeConfig
        {
            [DataMember]
            public string FileName = "";
            public string BiomeName
            {
                get
                {
                    FileInfo f = new FileInfo(FileName);
                    if (f != null)
                    {
                        return f.Name.Replace("BiomeConfig.ini", "").Replace(".bc", "");
                    }
                    return null;
                }
            }

            [DataMember]
            public List<Property> Properties;

            public BiomeConfig(VersionConfig config)
            {
                Properties = new List<Property>();
                foreach (TCProperty property in config.BiomeConfig)
                {
                    Properties.Add(new Property(null, false, property.Name, false, property.PropertyType != "BiomesList" && property.PropertyType != "ResourceQueue", property.Optional));
                }
            }

            public void SetProperty(TCProperty property, string value, bool merge, bool overrideParentValues)
            {
                if (property.PropertyType != "String" && property.PropertyType != "BiomesList" && property.PropertyType != "ResourceQueue" && String.IsNullOrWhiteSpace(value))
                {
                    value = null;
                }
                if (value != null)
                {
                    value = value.Trim();
                }
                Properties.First(a => a.PropertyName == property.Name).Value = value;
                Properties.First(a => a.PropertyName == property.Name).Merge = merge;
                Properties.First(a => a.PropertyName == property.Name).OverrideParentValues = overrideParentValues;
            }

            public string GetPropertyValueAsString(TCProperty property)
            {
                Property prop = Properties.FirstOrDefault(a => a.PropertyName == property.Name);
                return prop != null ? prop.Value : null;
            }

            public bool GetPropertyMerge(TCProperty property)
            {
                Property prop = Properties.FirstOrDefault(a => a.PropertyName == property.Name);
                return prop != null ? prop.Merge : false;
            }

            public bool GetPropertyOverrideParentValues(TCProperty property)
            {
                Property prop = Properties.FirstOrDefault(a => a.PropertyName == property.Name);
                return prop != null ? prop.OverrideParentValues : false;
            }
        }

        [DataContract]
        public class Property
        {
            [DataMember]
            public string Value;
            [DataMember]
            public bool Override;
            [DataMember]
            public string PropertyName;
            [DataMember]
            public bool Merge;
            [DataMember]
            public bool OverrideParentValues;

            public Property(string value, bool bOverride, string propertyName, bool merge, bool overrideParentValues, bool optional)
            {
                Value = value;
                Override = bOverride;
                PropertyName = propertyName;
                Merge = merge;
                OverrideParentValues = overrideParentValues;
            }
        }

        [DataContract]
        public class VersionConfig
        {
            [DataMember]
            public List<SettingsType> SettingsTypes;
            [DataMember]
            public List<ResourceQueueItem> ResourceQueueOptions;
            [DataMember]
            public List<TCProperty> WorldConfig = new List<TCProperty>();
            [DataMember]
            public List<TCProperty> BiomeConfig = new List<TCProperty>();
        }

        [DataContract]
        public class SettingsType
        {
            [DataMember]
            public string Type;
            [DataMember]
            public string ColorType;
        }

        [DataContract]
        public class ResourceQueueItem
        {
            [DataMember]
            public string Name;
            [DataMember]
            public bool HasUniqueParameter;
            [DataMember]
            public int UniqueParameterIndex;
            [DataMember]
            public bool IsUnique;
            [DataMember]
            public List<string> UniqueParameterValues;
        }

        [DataContract]
        public class TCProperty
        {
            [DataMember]
            public string Group;
            [DataMember]
            public string LabelText;
            [DataMember]
            public string Name;
            [DataMember]
            public string PropertyType;
            [DataMember]
            public string ScriptHandle;
            [DataMember]
            public bool Optional;
        }

    #endregion
}