using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.Serialization;
using TCEE.XML;

namespace TCEE
{
    public partial class Form1 : Form
    {
        DirectoryInfo VersionDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/");
        string WorldSaveDir = "";

        SaveFileDialog sfd;
        OpenFileDialog ofd;
        FolderBrowserDialog convertBO3fbd;
        FolderBrowserDialog copyBO3fbd;
        FolderBrowserDialog fbdDestinationWorldDir = new FolderBrowserDialog();
        OpenFileDialog convertBO3ofd;

        bool playerStopped = true;
        WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();

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

            public Form1()
            {
                InitializeComponent();

                // TODO: Pass these as method parameters instead of using static fields.
                Session.Form1 = this;
                Session.tabControl1 = tabControl1;
                Session.btSave = btSave;
                Session.btLoad = btLoad;
                Session.btGenerate = btGenerate;
                Session.lbGroups = lbGroups;
                Session.btCopyBO3s = btCopyBO3s;
                Session.cbDeleteRegion = cbDeleteRegion;

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

                this.Height = 202;

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

                if (VersionDir.Exists)
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
                Session.BiomeNames.Clear();
                BiomeListInputs.Clear();
                Session.DestinationConfigsDir = "";
                WorldSaveDir = "";
                Session.SourceConfigsDir = "";
                Session.ToolTip1.RemoveAll();
                ResourceQueueInputs.Clear();
                tlpWorldSettings.Controls.Clear();
                tlpWorldSettings.RowStyles.Clear();
                tlpBiomeSettings.Controls.Clear();
                tlpBiomeSettings.RowStyles.Clear();

                Session.WorldConfigDefaultValues = null;
                Session.WorldConfig1 = null;
                Session.WorldSettingsInputs.Clear();

                Session.BiomeGroups.Clear();
                BiomeConfigsDefaultValues.Clear();
                Session.BiomeSettingsInputs.Clear();

                lbGroups.Items.Clear();
                lbGroup.Items.Clear();
                lbBiomes.Items.Clear();

                DirectoryInfo versionDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/");
                if (cbVersion.Items.Count > 0 && cbVersion.SelectedItem != null && versionDir.Exists)
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(VersionConfig));
                    using (var reader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + cbVersion.SelectedItem + "/VersionConfig.xml"))
                    {
                        Session.VersionConfig = (VersionConfig)serializer.Deserialize(reader);
                    }
                    if (Session.VersionConfig != null)
                    {
                        Session.SettingsType = Session.VersionConfig.SettingsTypes.FirstOrDefault(a => a.Type == "Forge") ?? Session.VersionConfig.SettingsTypes.First();
                        Session.WorldConfig1 = new WorldConfig(Session.VersionConfig);
                        LoadUI();
                    } else {
                        MessageBox.Show("Y u do dis? :(");
                    }

                    Session.SourceConfigsDir = Path.GetDirectoryName(Application.ExecutablePath) + "/TCVersionConfigs/" + cbVersion.SelectedItem + "/Worlds/" + cbWorld.SelectedItem + "/";

                    if (!String.IsNullOrEmpty(Session.SourceConfigsDir) && System.IO.Directory.Exists(Session.SourceConfigsDir + "/" + "WorldBiomes" + "/"))
                    {
                        System.IO.DirectoryInfo defaultWorldDirectory = new System.IO.DirectoryInfo(Session.SourceConfigsDir + "/" + "WorldBiomes" + "/");
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
                        throw new Exception("WorldConfig.ini could not be loaded. Please make sure that WorldConfig.ini is present in the TCVersionConfig directory for the selected version. Exiting TCEE.");
                    } else {

                        LoadBiomesList();
                        LoadDefaultGroups();
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
                }
            }

            public void LoadUI()
            {
                Session.ToolTip1 = new ToolTip();
                Session.ToolTip1.AutoPopDelay = 32000;
                Session.ToolTip1.InitialDelay = 500;
                Session.ToolTip1.ReshowDelay = 0;
                Session.ToolTip1.ShowAlways = true;

                Session.ToolTip1.SetToolTip(lblGroups, 
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

                int i = 0;
                foreach (TCProperty property in Session.VersionConfig.WorldConfig)
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
                    Session.ToolTip1.SetToolTip(cbOverride, "Apply this value");
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
                            Session.ToolTip1.SetToolTip(btOverrideAll, "Removes all default values and replaces them with selected values.");

                            RadioButton btMergeWithDefaults = new RadioButton();
                            btMergeWithDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btMergeWithDefaults.Dock = DockStyle.Top;
                            btMergeWithDefaults.Text = "Merge with defaults";
                            btMergeWithDefaults.Enabled = false;
                            btMergeWithDefaults.Visible = false;
                            btMergeWithDefaults.Name = "Merge";
                            btMergeWithDefaults.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                            pCheckBoxes.Controls.Add(btMergeWithDefaults);
                            Session.ToolTip1.SetToolTip(btMergeWithDefaults, "Instead of overriding previously defined values this setting makes the resourcequeue add its values to the default values\r\nand the values defined in biome groups that are higher in the biome groups list.\r\n\r\nSome property and parameter combinations are configured as \"must be unique\" in the VersionConfig.xml and will always be \r\noverridden, for instance Ore(GOLD_ORE, which means the values configured in this list will replace \r\nany existing Ore(GOLD_ORE settings. Unique properties are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nProperty name and * must be unique.\r\n\r\nProperties that have a block as a unique parameter (such as ORE(Block,...)) can be configured to\r\nbe unique only when used with specific blocks (like GOLD_ORE, IRON_ORE etc).\r\n\r\nUnique properties, parameters and lists of blocks can be configured in the VersionConfig.xml.\r\n\r\nUpdate: It is now allowed to add multiple resources of the same type to this list even if they are configured\r\nas \"must be unique\", a popup will ask if you want to override or keep the existing items. This does not\r\naffect the merging behaviours between biome groups / default values.");

                            CheckBox btIgnoreParentMerge = new CheckBox();
                            btIgnoreParentMerge.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btIgnoreParentMerge.Dock = DockStyle.Top;
                            btIgnoreParentMerge.Text = "Override parent values";
                            btIgnoreParentMerge.Name = "OverrideParent";
                            btIgnoreParentMerge.Enabled = false;
                            btIgnoreParentMerge.Visible = false;
                            btIgnoreParentMerge.CheckedChanged += btOverrideParentValuesWorld_CheckedChanged;
                            pCheckBoxes.Controls.Add(btIgnoreParentMerge);
                            Session.ToolTip1.SetToolTip(btIgnoreParentMerge, "Ignore any values defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add values to the same property.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore to ResourceQueue.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");

                            pnl2.Controls.Add(pCheckBoxes);
                            pnl.Controls.Add(pnl2);
                            tlpWorldSettings.Controls.Add(pnl, column3, row);

                            if (tlpWorldSettings.RowStyles.Count - 1 < row)
                            {
                                tlpWorldSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                            }
                            tlpWorldSettings.RowStyles[tlpWorldSettings.RowStyles.Count - 1].SizeType = SizeType.Absolute;
                            tlpWorldSettings.RowStyles[tlpWorldSettings.RowStyles.Count - 1].Height = 120;

                            Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes));
                        break;
                        case "String":
                        case "Float":
                            TextBox txPropertyInput = new TextBox();
                            txPropertyInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                            tlpWorldSettings.Controls.Add(txPropertyInput, column3, row);
                            txPropertyInput.TextChanged += PropertyInputChangedWorld;
                            txPropertyInput.LostFocus += PropertyInputLostFocusWorld;
                            Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
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

                            Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput2, cbOverride, bSetDefaults, txPropertyLabel, lbPropertyInput2, null));
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
                            Session.ToolTip1.SetToolTip(btOverrideAll2, "Removes all default values and replaces them with selected values.");

                            RadioButton btMergeWithDefaults2 = new RadioButton();
                            btMergeWithDefaults2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btMergeWithDefaults2.Dock = DockStyle.Top;
                            btMergeWithDefaults2.Text = "Merge with defaults";
                            btMergeWithDefaults2.Enabled = false;
                            btMergeWithDefaults2.Visible = false;
                            btMergeWithDefaults2.Name = "Merge";
                            btMergeWithDefaults2.CheckedChanged += btOverrideAllWorld_CheckedChanged;
                            pCheckBoxes2.Controls.Add(btMergeWithDefaults2);
                            Session.ToolTip1.SetToolTip(btMergeWithDefaults2, "Instead of overriding previously defined values this setting makes the resourcequeue add its values to the default values\r\nand the values defined in biome groups that are higher in the biome groups list.\r\n\r\nSome property and parameter combinations are configured as \"must be unique\" in the VersionConfig.xml and will always be \r\noverridden, for instance Ore(GOLD_ORE, which means the values configured in this list will replace \r\nany existing Ore(GOLD_ORE settings. Unique properties are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nProperty name and * must be unique.\r\n\r\nProperties that have a block as a unique parameter (such as ORE(Block,...)) can be configured to\r\nbe unique only when used with specific blocks (like GOLD_ORE, IRON_ORE etc).\r\n\r\nUnique properties, parameters and lists of blocks can be configured in the VersionConfig.xml.\r\n\r\nUpdate: It is now allowed to add multiple resources of the same type to this list even if they are configured\r\nas \"must be unique\", a popup will ask if you want to override or keep the existing items. This does not\r\naffect the merging behaviours between biome groups / default values.");

                            CheckBox btIgnoreParentMerge2 = new CheckBox();
                            btIgnoreParentMerge2.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btIgnoreParentMerge2.Dock = DockStyle.Top;
                            btIgnoreParentMerge2.Text = "Override parent values";
                            btIgnoreParentMerge2.Name = "OverrideParent";
                            btIgnoreParentMerge2.Enabled = false;
                            btIgnoreParentMerge2.Visible = false;
                            btIgnoreParentMerge2.CheckedChanged += btOverrideParentValuesWorld_CheckedChanged;
                            pCheckBoxes2.Controls.Add(btIgnoreParentMerge2);
                            Session.ToolTip1.SetToolTip(btIgnoreParentMerge2, "Ignore any values defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add values to the same property.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore to ResourceQueue.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");

                            pnl5.Controls.Add(pCheckBoxes2);
                            pnl4.Controls.Add(pnl5);
                            tlpWorldSettings.Controls.Add(pnl4, column3, row);

                            if (tlpWorldSettings.RowStyles.Count - 1 < row)
                            {
                                tlpWorldSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                            }
                            tlpWorldSettings.RowStyles[tlpWorldSettings.RowStyles.Count - 1].SizeType = SizeType.Absolute;
                            tlpWorldSettings.RowStyles[tlpWorldSettings.RowStyles.Count - 1].Height = 120;

                            Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput3, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes2));
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
                            Session.WorldSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(cbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                            break;
                    }
                    if (tlpWorldSettings.RowStyles.Count - 1 < row)
                    {
                        tlpWorldSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }

                    Session.ToolTip1.SetToolTip(bSetDefaults, "Clear");

                    i += 1;
                }

                i = 0;
                foreach (TCProperty property in Session.VersionConfig.BiomeConfig)
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
                    Session.ToolTip1.SetToolTip(cbOverride, "Apply this value");
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
                        Session.ToolTip1.SetToolTip(btOverrideAll, "Removes all default values and replaces them with selected values.");

                        RadioButton btMergeWithDefaults = new RadioButton();
                        btMergeWithDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        btMergeWithDefaults.Dock = DockStyle.Top;
                        btMergeWithDefaults.Text = "Merge with defaults";
                        btMergeWithDefaults.Checked = true;
                        btMergeWithDefaults.Name = "Merge";
                        btMergeWithDefaults.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                        pCheckBoxes.Controls.Add(btMergeWithDefaults);
                        Session.ToolTip1.SetToolTip(btMergeWithDefaults, "Instead of overriding previously defined values this setting makes the resourcequeue add its values to the default values\r\nand the values defined in biome groups that are higher in the biome groups list.\r\n\r\nSome property and parameter combinations are configured as \"must be unique\" in the VersionConfig.xml and will always be \r\noverridden, for instance Ore(GOLD_ORE, which means the values configured in this list will replace \r\nany existing Ore(GOLD_ORE settings. Unique properties are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nProperty name and * must be unique.\r\n\r\nProperties that have a block as a unique parameter (such as ORE(Block,...)) can be configured to\r\nbe unique only when used with specific blocks (like GOLD_ORE, IRON_ORE etc).\r\n\r\nUnique properties, parameters and lists of blocks can be configured in the VersionConfig.xml.\r\n\r\nUpdate: It is now allowed to add multiple resources of the same type to this list even if they are configured\r\nas \"must be unique\", a popup will ask if you want to override or keep the existing items. This does not\r\naffect the merging behaviours between biome groups / default values.");

                        CheckBox btIgnoreParentMerge = new CheckBox();
                        btIgnoreParentMerge.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        btIgnoreParentMerge.Dock = DockStyle.Top;
                        btIgnoreParentMerge.Text = "Override parent values";
                        btIgnoreParentMerge.Name = "OverrideParent";
                        btIgnoreParentMerge.CheckedChanged += btOverrideParentValuesBiome_CheckedChanged;
                        pCheckBoxes.Controls.Add(btIgnoreParentMerge);
                        Session.ToolTip1.SetToolTip(btIgnoreParentMerge, "Ignore any values defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add values to the same property.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");
                    
                        pnl2.Controls.Add(pCheckBoxes);

                        pnl.Controls.Add(pnl2);
                        tlpBiomeSettings.Controls.Add(pnl, column3, row);

                        if (tlpBiomeSettings.RowStyles.Count - 1 < row)
                        {
                            tlpBiomeSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        }
                        tlpBiomeSettings.RowStyles[tlpBiomeSettings.RowStyles.Count - 1].SizeType = SizeType.Absolute;
                        tlpBiomeSettings.RowStyles[tlpBiomeSettings.RowStyles.Count - 1].Height = 200;
                        Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes));
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
                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
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

                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(txPropertyInput2, cbOverride, bSetDefaults, txPropertyLabel, lbPropertyInput, null));
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
                                Session.ToolTip1.SetToolTip(btOverrideAll, "Removes all default values and replaces them with selected values.");

                                RadioButton btMergeWithDefaults = new RadioButton();
                                btMergeWithDefaults.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btMergeWithDefaults.Dock = DockStyle.Top;
                                btMergeWithDefaults.Text = "Merge with defaults";
                                btMergeWithDefaults.Name = "Merge";
                                btMergeWithDefaults.Checked = true;
                                btMergeWithDefaults.CheckedChanged += btOverrideAllBiome_CheckedChanged;
                                pCheckBoxes.Controls.Add(btMergeWithDefaults);
                                Session.ToolTip1.SetToolTip(btMergeWithDefaults, "Instead of overriding previously defined values this setting makes the resourcequeue add its values to the default values\r\nand the values defined in biome groups that are higher in the biome groups list.\r\n\r\nSome property and parameter combinations are configured as \"must be unique\" in the VersionConfig.xml and will always be \r\noverridden, for instance Ore(GOLD_ORE, which means the values configured in this list will replace \r\nany existing Ore(GOLD_ORE settings. Unique properties are:\r\n\r\n" + uniqueResourceQueueItems + "\r\n\r\nProperty name and * must be unique.\r\n\r\nProperties that have a block as a unique parameter (such as ORE(Block,...)) can be configured to\r\nbe unique only when used with specific blocks (like GOLD_ORE, IRON_ORE etc).\r\n\r\nUnique properties, parameters and lists of blocks can be configured in the VersionConfig.xml.\r\n\r\nUpdate: It is now allowed to add multiple resources of the same type to this list even if they are configured\r\nas \"must be unique\", a popup will ask if you want to override or keep the existing items. This does not\r\naffect the merging behaviours between biome groups / default values.");

                                CheckBox btIgnoreParentMerge = new CheckBox();
                                btIgnoreParentMerge.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btIgnoreParentMerge.Dock = DockStyle.Top;
                                btIgnoreParentMerge.Text = "Override parent values";
                                btIgnoreParentMerge.Name = "OverrideParent";
                                btIgnoreParentMerge.CheckedChanged += btOverrideParentValuesBiome_CheckedChanged;
                                pCheckBoxes.Controls.Add(btIgnoreParentMerge);
                                Session.ToolTip1.SetToolTip(btIgnoreParentMerge, "Ignore any values defined in a group listed higher in the Groups menu than the current group.\r\n\r\n If this is disabled then multiple groups can add values to the same property.\r\n For instance Group 1 can add gold ore and Group 2 can add diamond ore.\r\n\r\nIf this is enabled then only the current group will add its values.\r\nFor instance if this option is enabled in Group 2 then if Group 1 adds gold ore and Group 2 adds diamond ore only the diamond ore is added.");

                                pnl2.Controls.Add(pCheckBoxes);
                                pnl.Controls.Add(pnl2);
                                tlpBiomeSettings.Controls.Add(pnl, column3, row);

                                if (tlpBiomeSettings.RowStyles.Count - 1 < row)
                                {
                                    tlpBiomeSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                                }
                                tlpBiomeSettings.RowStyles[tlpBiomeSettings.RowStyles.Count - 1].SizeType = SizeType.Absolute;
                                tlpBiomeSettings.RowStyles[tlpBiomeSettings.RowStyles.Count - 1].Height = 180;

                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(lbPropertyInput2, cbOverride, bSetDefaults, txPropertyLabel, null, pCheckBoxes));
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
                                Session.BiomeSettingsInputs.Add(property, new Tuple<Control, CheckBox, Button, Label, ListBox, Panel>(cbPropertyInput, cbOverride, bSetDefaults, txPropertyLabel, null, null));
                                break;
                        }
                    }
                    if (tlpBiomeSettings.RowStyles.Count - 1 < row)
                    {
                        tlpBiomeSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }

                    Session.ToolTip1.SetToolTip(bSetDefaults, "Clear");

                    i += 1;
                }
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
                    bIsDefault = Utils.TCSettingsUtils.CompareBiomeLists(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property), Session.WorldConfig1.GetPropertyValueAsString(property));

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
                        ColorDialog colorDlg = new ColorDialog();
                        colorDlg.AllowFullOpen = false;
                        colorDlg.AnyColor = true;
                        colorDlg.SolidColorOnly = false;

                        if (colorDlg.ShowDialog() == DialogResult.OK)
                        {
                            KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item5 == sender);
                            TCProperty property = kvp.Key;
                            kvp.Value.Item5.BackColor = colorDlg.Color;
                            Session.IgnorePropertyInputChangedWorld = true;
                            Session.IgnoreOverrideCheckChangedWorld = true;
                            if (Session.SettingsType.ColorType == "0x")
                            {
                                kvp.Value.Item1.Text = "0x" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            else if (Session.SettingsType.ColorType == "#")
                            {
                                kvp.Value.Item1.Text = "#" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            Session.WorldConfig1.SetProperty(property, kvp.Value.Item1.Text, false, false);

                            if (Session.WorldConfigDefaultValues == null || kvp.Value.Item1.Text != Session.WorldConfigDefaultValues.GetPropertyValueAsString(property))
                            {
                                kvp.Value.Item2.Checked = true;
                            } else {
                                kvp.Value.Item2.Checked = false;
                            }
                            Session.IgnorePropertyInputChangedWorld = false;
                            Session.IgnoreOverrideCheckChangedWorld = false;
                        }
                    }
                    else if(sender is TextBox)
                    {
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item1 == sender);
                        TCProperty property = kvp.Key;
                        try
                        {
                            if (kvp.Value.Item1.Text.Length == 8 || (kvp.Value.Item1.Text.Length == 7 && kvp.Value.Item1.Text.StartsWith("#")))
                            {
                                Session.WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(kvp.Value.Item1.Text);
                                if (Session.SettingsType.ColorType == "0x")
                                {
                                    kvp.Value.Item1.Text = "0x" + Session.WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                else if (Session.SettingsType.ColorType == "#")
                                {
                                    kvp.Value.Item1.Text = "#" + Session.WorldSettingsInputs[property].Item5.BackColor.R.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.G.ToString("X2") + Session.WorldSettingsInputs[property].Item5.BackColor.B.ToString("X2");
                                }
                                Session.WorldConfig1.SetProperty(property, kvp.Value.Item1.Text, false, false);

                                if (Session.WorldConfigDefaultValues == null || kvp.Value.Item1.Text != Session.WorldConfigDefaultValues.GetPropertyValueAsString(property))
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

            void PropertyInputChangedWorld(object sender, EventArgs e)
            {
                if (!Session.IgnorePropertyInputChangedWorld)
                {
                    KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.WorldSettingsInputs.First(a => a.Value.Item1 == sender);
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
                        Session.WorldConfig1.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);

                        if (!(property.PropertyType == "Bool" && String.IsNullOrEmpty(tb.Text)) && (Session.WorldConfigDefaultValues == null || (property.PropertyType != "Bool" && tb.Text != Session.WorldConfigDefaultValues.GetPropertyValueAsString(property)) || (property.PropertyType == "Bool" && !String.IsNullOrEmpty(tb.Text) && tb.Text != Session.WorldConfigDefaultValues.GetPropertyValueAsString(property))))
                        {
                            cb.Checked = true;
                        } else {
                            if (property.PropertyType == "Bool" && String.IsNullOrEmpty(tb.Text) && Session.WorldConfigDefaultValues != null)
                            {
                                Session.IgnoreOverrideCheckChangedWorld = true;
                                Session.IgnorePropertyInputChangedWorld = true;
                                tb.Text = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
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
                    if (((TextBox)sender).Text.Length == 8 || (((TextBox)sender).Text.Length == 7 && ((TextBox)sender).Text.StartsWith("#")))
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
                            if (Session.WorldConfigDefaultValues == null || value.ToUpper() != Session.WorldConfigDefaultValues.GetPropertyValueAsString(property).ToUpper())
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
                        if (defaultValue != null && (defaultValue.Length == 8 || (defaultValue.Length == 7 && defaultValue.StartsWith("#"))))
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
                else if (property.PropertyType == "Float")
                {
                    float result = 0;
                    if (!float.TryParse(((TextBox)sender).Text, out result))
                    {
                        if (Session.WorldConfigDefaultValues != null)
                        {
                            ((TextBox)sender).Text = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                        }
                        Session.WorldSettingsInputs[property].Item2.Checked = false;
                    }
                }
                else if (String.IsNullOrWhiteSpace(((TextBox)sender).Text) && property.PropertyType != "String")
                {
                    if (Session.WorldConfigDefaultValues != null)
                    {
                        ((TextBox)sender).Text = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
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
                        if (
                            property.PropertyType == "String" ||
                            property.PropertyType == "Bool" ||
                            (property.PropertyType == "Float" && float.TryParse(tb.Text, out result))
                        )
                        {
                            Session.WorldConfig1.SetProperty(property, tb.Text, kvp.Value.Item6 != null && ((RadioButton)kvp.Value.Item6.Controls.Find("Merge", true)[0]).Checked, kvp.Value.Item6 != null && ((CheckBox)kvp.Value.Item6.Controls.Find("OverrideParent", true)[0]).Checked);
                        }
                        else if (property.PropertyType == "Float")
                        {
                            Session.IgnoreOverrideCheckChangedWorld = true;
                            Session.IgnorePropertyInputChangedWorld = true;
                            if (Session.WorldConfigDefaultValues != null && Session.WorldConfigDefaultValues.GetPropertyValueAsString(property) != null)
                            {
                                tb.Text = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
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
                                if (kvp.Value.Item1.Text.Length == 8 || (kvp.Value.Item1.Text.Length == 7 && kvp.Value.Item1.Text.StartsWith("#")))
                                {
                                    if ((Session.SettingsType.ColorType == "0x" && kvp.Value.Item1.Text.StartsWith("0x")) || (Session.SettingsType.ColorType == "#" && kvp.Value.Item1.Text.StartsWith("#")))
                                    {
                                        Color derp = System.Drawing.ColorTranslator.FromHtml(Session.WorldConfigDefaultValues.GetPropertyValueAsString(property));
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
                        if (propertyValue != null && ((Session.SettingsType.ColorType == "0x" && propertyValue.Length == 8) || (propertyValue != null && Session.SettingsType.ColorType == "#" && propertyValue.Length == 7 && propertyValue.StartsWith("#"))))
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
                                addingMultipleResourcesXToAll = MessageBox.Show("One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep existing items?", MessageBoxButtons.YesNoCancel);
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
                                    addingMultipleResourcesXToAll2 = MessageBox.Show("Resource \"" + propertyValue + "\" already exists, do you still want to add it?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Add exact duplicate?", MessageBoxButtons.YesNo);
                                }
                                permissionGiven = addingMultipleResourcesXToAll2;
                            } else {
                                if (addingMultipleResourcesXToAll3 == DialogResult.Abort)
                                {
                                    addingMultipleResourcesXToAll3 = MessageBox.Show("One or more \"" + selectedOption.Name + "\" resources already exist but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep same resource with different parameters?", MessageBoxButtons.YesNoCancel);
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
                                                        addingMultipleResourcesXToAll = MessageBox.Show("One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep existing items?", MessageBoxButtons.YesNoCancel);
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
                                        MessageBox.Show("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
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
                            MessageBox.Show("Cannot add item. property name was not recognized. Legal property names are: \r\n" + sPropertyNames, "Error: Illegal input");
                        }
                    }
                } else {
                    if (newPropertyValue == null)
                    {
                        bAllowed = true;
                    }
                    else if (selectedOption == null)
                    {
                        MessageBox.Show("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is PropertyName(Parameters)", "Error: Illegal input");
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
                                        addingMultipleResourcesXToAll = MessageBox.Show("One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep existing items?", MessageBoxButtons.YesNoCancel);
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
                                            addingMultipleResourcesXToAll2 = MessageBox.Show("Resource \"" + propertyValue + "\" already exists, do you still want to add it?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Add exact duplicate?", MessageBoxButtons.YesNo);
                                        }
                                        permissionGiven = addingMultipleResourcesXToAll2;
                                    } else {
                                        if (addingMultipleResourcesXToAll3 == DialogResult.Abort)
                                        {
                                            addingMultipleResourcesXToAll3 = MessageBox.Show("One or more \"" + selectedOption.Name + "\" resources already exist but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep same resource with different parameters?", MessageBoxButtons.YesNoCancel);
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
                                                                addingMultipleResourcesXToAll = MessageBox.Show("One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep existing items?", MessageBoxButtons.YesNoCancel);
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
                                                MessageBox.Show("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
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
                                    MessageBox.Show("Cannot add item. property name was not recognized. Legal property names are: \r\n" + sPropertyNames, "Error: Illegal input");
                                }
                            }
                        } else {
                            if (newPropertyValue == null)
                            {
                                bAllowed = true;
                            }
                            else if (selectedOption == null)
                            {
                                MessageBox.Show("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is PropertyName(Parameters)", "Error: Illegal input");
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
                                if ((propertyValue != null && Session.SettingsType.ColorType == "0x" && propertyValue.Length == 8) || (propertyValue != null && Session.SettingsType.ColorType == "#" && propertyValue.Length == 7 && propertyValue.StartsWith("#")))
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
                                Session.WorldConfig1.SetProperty(property, null, false, false);
                                break;
                        }
                        boxes.Item2.Checked = false;

                        Session.IgnorePropertyInputChangedWorld = false;
                        Session.IgnoreOverrideCheckChangedWorld = false;
                    }
                }
            }

        #endregion

        #region Biomes

            void LoadBiomesList()
            {
                if (!String.IsNullOrEmpty(Session.SourceConfigsDir) && System.IO.Directory.Exists(Session.SourceConfigsDir + "/" + "WorldBiomes" + "/"))
                {
                    System.IO.DirectoryInfo defaultWorldDirectory = new System.IO.DirectoryInfo(Session.SourceConfigsDir + "/" + "WorldBiomes" + "/");
                    lbBiomes.Items.Clear();
                    BiomeConfigsDefaultValues.Clear();

                    foreach (System.IO.FileInfo file in defaultWorldDirectory.GetFiles())
                    {
                        if (file.Name.EndsWith(".bc"))
                        {
                            lbBiomes.Items.Add(file.Name.Replace(".bc", ""));
                            BiomeConfig bDefaultConfig = Biomes.LoadBiomeConfigFromFile(file, Session.VersionConfig, true);
                            BiomeConfigsDefaultValues.Add(bDefaultConfig);
                        }
                    }
                }
            }

            void LoadDefaultGroups()
            {
                if (BiomeConfigsDefaultValues.Any())
                {
                    lbGroups.Items.Add("Land");
                    Group overworldLand = new Group("Land", Session.VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName != "Hell" && biomeConfig.BiomeName != "Sky" && !biomeConfig.BiomeName.ToLower().Contains("ocean"))
                        {
                            overworldLand.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    Session.BiomeGroups.Add(overworldLand);

                    lbGroups.Items.Add("Oceans");
                    Group overworldOceans = new Group("Oceans", Session.VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName.ToLower().Contains("ocean"))
                        {
                            overworldOceans.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    Session.BiomeGroups.Add(overworldOceans);

                    lbGroups.Items.Add("Sky");
                    Group overworldSky = new Group("Sky", Session.VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName == "Sky")
                        {
                            overworldSky.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    Session.BiomeGroups.Add(overworldSky);

                    lbGroups.Items.Add("Nether");
                    Group hell = new Group("Nether", Session.VersionConfig);
                    foreach (BiomeConfig biomeConfig in BiomeConfigsDefaultValues)
                    {
                        if (biomeConfig.BiomeName == "Hell")
                        {
                            hell.Biomes.Add(biomeConfig.BiomeName);
                        }
                    }
                    Session.BiomeGroups.Add(hell);
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
                        ColorDialog colorDlg = new ColorDialog();
                        colorDlg.AllowFullOpen = false;
                        colorDlg.AnyColor = true;
                        colorDlg.SolidColorOnly = false;

                        if (colorDlg.ShowDialog() == DialogResult.OK)
                        {
                            KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.First(a => a.Value.Item5 == sender);
                            TCProperty property = kvp.Key;
                            kvp.Value.Item5.BackColor = colorDlg.Color;
                            IgnorePropertyInputChangedBiome = true;
                            IgnoreOverrideCheckChangedBiome = true;
                            if (Session.SettingsType.ColorType == "0x")
                            {
                                kvp.Value.Item1.Text = "0x" + colorDlg.Color.R.ToString("X2") + colorDlg.Color.G.ToString("X2") + colorDlg.Color.B.ToString("X2");
                            }
                            else if (Session.SettingsType.ColorType == "#")
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
                        KeyValuePair<TCProperty, Tuple<Control, CheckBox, Button, Label, ListBox, Panel>> kvp = Session.BiomeSettingsInputs.First(a => a.Value.Item1 == sender);
                        TCProperty property = kvp.Key;
                        try
                        {
                            if (kvp.Value.Item1.Text.Length == 8 || (kvp.Value.Item1.Text.Length == 7 && kvp.Value.Item1.Text.StartsWith("#")))
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

                                if (biomeDefaultConfig == null || kvp.Value.Item1.Text != biomeDefaultConfig.GetPropertyValueAsString(property))
                                {
                                    kvp.Value.Item2.Checked = true;
                                } else {
                                    kvp.Value.Item2.Checked = false;
                                }
                                string breakpoint = "";
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
                    if (((TextBox)sender).Text.Length == 8 || (((TextBox)sender).Text.Length == 7 && ((TextBox)sender).Text.StartsWith("#")))
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
                            if (biomeDefaultConfig == null || value.ToUpper() != biomeDefaultConfig.GetPropertyValueAsString(property).ToUpper())
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
                            if (defaultValue != null && (defaultValue.Length == 8 || (defaultValue.Length == 7 && defaultValue.StartsWith("#"))))
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
                        Session.BiomeSettingsInputs[property].Item2.Checked = false;
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
                                    if ((Session.SettingsType.ColorType == "0x" && kvp.Value.Item1.Text.StartsWith("0x")) || (Session.SettingsType.ColorType == "#" && kvp.Value.Item1.Text.StartsWith("#")))
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

            void bSetDefaultsBiomesProperty(object sender, EventArgs e)
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
                                addingMultipleResourcesXToAll = MessageBox.Show("One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep existing items?", MessageBoxButtons.YesNoCancel);
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
                                    addingMultipleResourcesXToAll2 = MessageBox.Show("Resource \"" + propertyValue + "\" already exists, do you still want to add it?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Add exact duplicate?", MessageBoxButtons.YesNo);
                                }
                                permissionGiven = addingMultipleResourcesXToAll2;
                            } else {
                                if (addingMultipleResourcesXToAll3 == DialogResult.Abort)
                                {
                                    addingMultipleResourcesXToAll3 = MessageBox.Show("One or more \"" + selectedOption.Name + "\" resources already exist but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep same resource with different parameters?", MessageBoxButtons.YesNoCancel);
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
                                                        addingMultipleResourcesXToAll = MessageBox.Show("One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep existing items?", MessageBoxButtons.YesNoCancel);
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
                                        MessageBox.Show("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
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
                            MessageBox.Show("Cannot add item. property name was not recognized. Legal property names are: \r\n" + sPropertyNames, "Error: Illegal input");
                        }
                    }
                } else {
                    if (newPropertyValue == null)
                    {
                        bAllowed = true;
                    }
                    else if (selectedOption == null)
                    {
                        MessageBox.Show("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is PropertyName(Parameters)", "Error: Illegal input");
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
                                        addingMultipleResourcesXToAll = MessageBox.Show("One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep existing items?", MessageBoxButtons.YesNoCancel);
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
                                            addingMultipleResourcesXToAll2 = MessageBox.Show("Resource \"" + propertyValue + "\" already exists, do you still want to add it?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Add exact duplicate?", MessageBoxButtons.YesNo);
                                        }
                                        permissionGiven = addingMultipleResourcesXToAll2;
                                    } else {
                                        if (addingMultipleResourcesXToAll3 == DialogResult.Abort)
                                        {
                                            addingMultipleResourcesXToAll3 = MessageBox.Show("One or more \"" + selectedOption.Name + "\" resources already exist but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep same resource with different parameters?", MessageBoxButtons.YesNoCancel);
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
                                                                addingMultipleResourcesXToAll = MessageBox.Show("One ore more resources with parameter \"" + newParameters[selectedOption.UniqueParameterIndex].Trim() + "\" already exists but with different parameters, do you want to keep existing resources (yes) or override them (no)?" + (copyPasting ? "\r\n\r\nThis action will be applied to all items currently being pasted." : ""), "Keep existing items?", MessageBoxButtons.YesNoCancel);
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
                                                MessageBox.Show("Cannot add item. Illegal format, could not find parameter " + selectedOption.UniqueParameterIndex + ".", "Error: Illegal input");
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
                                    MessageBox.Show("Cannot add item. property name was not recognized. Legal property names are: \r\n" + sPropertyNames, "Error: Illegal input");
                                }
                            }
                        } else {
                            if(newPropertyValue == null)
                            {
                                bAllowed = true;
                            }
                            else if(selectedOption == null)
                            {
                                MessageBox.Show("Cannot add item. Illegal format, opening and/or closing brace could not be found. Correct format is PropertyName(Parameters)", "Error: Illegal input");
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

            private void btBiomeSettingsResetToDefaults_Click(object sender, EventArgs e)
            {
                Group g = Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.SelectedItem);
                if (BiomeConfigsDefaultValues.Count > 0 && g.showDefaults)
                {
                    BiomeConfig biomeDefaultConfig = BiomeConfigsDefaultValues.FirstOrDefault(a => a.BiomeName == g.Biomes[0]);
                    if (biomeDefaultConfig != null)
                    {
                        foreach (TCProperty property in Session.VersionConfig.BiomeConfig)
                        {
                            Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = Session.BiomeSettingsInputs[property];
                            IgnorePropertyInputChangedBiome = true;
                            IgnoreOverrideCheckChangedBiome = true;

                            string propertyValue = biomeDefaultConfig.GetPropertyValueAsString(property);
                            switch (property.PropertyType)
                            {
                                case "BiomesList":
                                    ((ListBox)Session.BiomeSettingsInputs[property].Item1).SelectedItems.Clear();
                                    string[] biomeNames = propertyValue.Split(',');
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
                                        if (Session.SettingsType.ColorType == "0x")
                                        {
                                            value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                        }
                                        else if (Session.SettingsType.ColorType == "#")
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
                        foreach (TCProperty property in Session.VersionConfig.BiomeConfig)
                        {
                            Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = Session.BiomeSettingsInputs[property];
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
                    foreach (TCProperty property in Session.VersionConfig.BiomeConfig)
                    {
                        Tuple<Control, CheckBox, Button, Label, ListBox, Panel> boxes = Session.BiomeSettingsInputs[property];
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
                        btBiomeSettingsResetToDefaults.Text = "Clear all";
                        foreach (Button bSetDefaults in Session.BiomeSettingsInputs.Select(a => a.Value.Item3))
                        {
                            Session.ToolTip1.SetToolTip(bSetDefaults, "Clear");
                            bSetDefaults.Text = "C";
                        }
                    } else {
                        if (biomeConfigDefaultValues != null)
                        {
                            btBiomeSettingsResetToDefaults.Text = "Set to defaults";
                            foreach (Button bSetDefaults in Session.BiomeSettingsInputs.Select(a => a.Value.Item3))
                            {
                                Session.ToolTip1.SetToolTip(bSetDefaults, "Set to default");
                                bSetDefaults.Text = "<";
                            }
                        } else {
                            btBiomeSettingsResetToDefaults.Text = "Clear all";
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
                            ((ComboBox)Session.BiomeSettingsInputs[property].Item1).SelectedIndex = 0;
                            break;
                            case "Color":
                            Session.BiomeSettingsInputs[property].Item5.BackColor = Color.White;
                            Session.BiomeSettingsInputs[property].Item1.Text = "";
                            break;
                            case "Float":
                            Session.BiomeSettingsInputs[property].Item1.Text = "";
                            break;
                            case "String":
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
                                            if (Session.SettingsType.ColorType == "0x")
                                            {
                                                value = "0x" + boxes.Item5.BackColor.R.ToString("X2") + boxes.Item5.BackColor.G.ToString("X2") + boxes.Item5.BackColor.B.ToString("X2");
                                            }
                                            else if (Session.SettingsType.ColorType == "#")
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
                    if (!Session.BiomeGroups.Any(a => a.Name == groupName))
                    {
                        lbGroups.Items.Add(groupName);
                        Group g = new Group(groupName, Session.VersionConfig);
                        g.Biomes.AddRange(biomesToAdd);
                        Session.BiomeGroups.Add(g);
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
                            if (!Session.BiomeGroups.Any(a => a.Name == groupName))
                            {
                                Session.BiomeGroups.FirstOrDefault(a => a.Name == (string)lbGroups.Items[lbGroups.SelectedIndex]).Name = groupName;
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
                    Session.BiomeGroups.RemoveAll(a => a.Name == (string)lbGroups.SelectedItem);
                    lbGroups.Items.Remove(lbGroups.SelectedItem);
                    lbGroups.SelectedIndex = lbGroups.Items.Count > selectedIndex ? selectedIndex : (lbGroups.Items.Count > selectedIndex - 1 ? selectedIndex - 1 : (lbGroups.Items.Count > 0 ? 0 : -1));
                    if(lbGroups.Items.Count < 1)
                    {
                        lbGroup.Items.Clear();
                        lbBiomes.Items.Clear();
                    }
                }
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
                                    MessageBox.Show("A group with this name already exists", "Error: Illegal input");
                                }
                            }
                        }
                    } else {
                        MessageBox.Show("Cannot clone a biome group with a single biome (yet, sorry).");
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
                }
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
                } else {
                    if (lbGroup.Items.Count == 1 || lbGroup.SelectedItems.Count >= lbGroup.Items.Count)
                    {
                        MessageBox.Show("Cannot delete the selected biome(s). A group must contain at least one biome.", "Error: Illegal input");
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

                    MessageBox.Show("Settings saved as: " + sfd.FileName, "Settings saved");
                }
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
                                Session.WorldConfig1 = settingsFile.WorldConfig;
                                foreach (Property prop in Session.WorldConfig1.Properties.Where(c => c.Override && !Session.VersionConfig.WorldConfig.Any(d => (string)d.Name == (string)c.PropertyName)))
                                {
                                    sErrorMessage += "Could not load value \"" + prop.Value + "\" for property \"" + prop.PropertyName + "\" in WorldConfig.\r\n";
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
                                            ((ComboBox)Session.WorldSettingsInputs[property].Item1).SelectedIndex = 0;
                                            break;
                                        case "Color":
                                            Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                            Session.WorldSettingsInputs[property].Item1.Text = "";
                                            break;
                                        case "Float":
                                            Session.WorldSettingsInputs[property].Item1.Text = "";
                                            break;
                                        case "String":
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
                                                        sErrorMessage2 += "Could not select biome \"" + biomeNames[k].Trim() + "\" for property \"" + property.Name + "\" in world config.\r\n";
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
                                                    ((ComboBox)boxes.Item1).SelectedIndex = 1;
                                                }
                                                else if (propertyValue.ToLower() == "false")
                                                {
                                                    ((ComboBox)boxes.Item1).SelectedIndex = 2;
                                                } else {
                                                    ((ComboBox)boxes.Item1).SelectedIndex = 0;
                                                }
                                                boxes.Item2.Checked = Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                            case "Color":
                                                if (propertyValue.Length == 8 || (propertyValue.Length == 7 && propertyValue.StartsWith("#")))
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
                                                        if (Session.WorldConfigDefaultValues == null || boxes.Item2.Checked || value.ToUpper() != Session.WorldConfigDefaultValues.GetPropertyValueAsString(property).ToUpper())
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
                                                        string asss = Session.WorldConfigDefaultValues.GetPropertyValueAsString(property);
                                                        if (asss.Length == 8 || (asss.Length == 7 && asss.StartsWith("#")))
                                                        {
                                                            try
                                                            {
                                                                boxes.Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(asss);
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
                                                boxes.Item1.Text = propertyValue;
                                                boxes.Item2.Checked = Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override;
                                                break;
                                            case "String":
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
                                            sErrorMessage += "Could not load value \"" + prop.Value + "\" for property \"" + prop.PropertyName + "\" in biome group \"" + biomeGroup.Name + "\".\r\n";
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
                                        biomeGroup.BiomeConfig.Properties.RemoveAll(a => !Session.VersionConfig.BiomeConfig.Any(b => (string)b.Name == (string)a.PropertyName));
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

                        Session.DestinationConfigsDir = fbdDestinationWorldDir.SelectedPath;
                        WorldSaveDir = Session.DestinationConfigsDir.Replace("mods\\TerrainControl\\worlds\\", "saves\\");

                        World.ConfigWorld();
                        Biomes.GenerateBiomeConfigs(new System.IO.DirectoryInfo(Session.SourceConfigsDir + "/" + "WorldBiomes"), new System.IO.DirectoryInfo(Session.DestinationConfigsDir + "/" + "WorldBiomes"), BiomeConfigsDefaultValues, Session.VersionConfig);

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

                            System.IO.DirectoryInfo StructureDataDirectory = new System.IO.DirectoryInfo(Session.DestinationConfigsDir + "/StructureData");
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

                            System.IO.FileInfo structureDataFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "/StructureData.txt");
                            if (structureDataFile.Exists)
                            {
                                structureDataFile.Delete();
                            }
                            System.IO.FileInfo nullChunksFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "/NullChunks.txt");
                            if (nullChunksFile.Exists)
                            {
                                nullChunksFile.Delete();
                            }
                            System.IO.FileInfo spawnedStructuresFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "/SpawnedStructures.txt");
                            if (spawnedStructuresFile.Exists)
                            {
                                spawnedStructuresFile.Delete();
                            }

                            System.IO.FileInfo chunkProviderPopulatedChunksFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "/ChunkProviderPopulatedChunks.txt");
                            if (chunkProviderPopulatedChunksFile.Exists)
                            {
                                chunkProviderPopulatedChunksFile.Delete();
                            }

                            System.IO.FileInfo pregeneratedChunksFile = new System.IO.FileInfo(Session.DestinationConfigsDir + "/PregeneratedChunks.txt");
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
                                            worldConfig = World.LoadWorldConfigFromFile(worldConfigFile, versionConfig, false);
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
                                                BiomeConfig biomeConfig = Biomes.LoadBiomeConfigFromFile(biomeFile, versionConfig, false);
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
                                            defaultWorldConfig = World.LoadWorldConfigFromFile(defaultWorldConfigFile, versionConfig, false);
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
                                            BiomeConfig biomeConfig = Biomes.LoadBiomeConfigFromFile(biomeFile, versionConfig, false);
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

                                                    Utils.CopyDir.CopyAll(worldObjectsDir, destinationWorldObjectsDir);
                                                }

                                                // Copy Biomes
                                                if (biomesDir != null)
                                                {
                                                    // Copy default biomes
                                                    //GenerateBiomeConfigs(defaultBiomesDir, new DirectoryInfo(destinationWorldDirectory.FullName + settingsType.BiomesDirectory), defaultBiomeConfigs, versionConfig);
                                                    if (!useDefaults)
                                                    {
                                                        Biomes.GenerateBiomeConfigs(defaultBiomesDir, new DirectoryInfo(destinationDir.FullName + "/" + "WorldBiomes"), defaultBiomeConfigs, versionConfig);
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
                            Utils.SchematicToBO3.doSchematicToBO3(new FileInfo(fileName), new DirectoryInfo(convertBO3fbd.SelectedPath), exportForTC);
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
                    System.IO.DirectoryInfo SourceWorldDirectory = new System.IO.DirectoryInfo(Session.SourceConfigsDir + "/WorldObjects");
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

                        Utils.CopyDir.CopyAll(SourceWorldDirectory, DestinationWorldDirectory);
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
            TCProperty property = Session.WorldSettingsInputs.FirstOrDefault(a => a.Value.Item1 == sender).Key;

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
            if(e.KeyCode == Keys.Delete)
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

        void lbPropertyInput_KeyDown(object sender, KeyEventArgs e)
        {
            TCProperty property = Session.BiomeSettingsInputs.FirstOrDefault(a => a.Value.Item1 == sender).Key;

            if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = true;

                ListBox lb = (ListBox)sender;
                String clipBoardString = "";
                foreach (String selectedItem in lb.SelectedItems)
                {
                    clipBoardString += selectedItem + "\r\n";
                }
                if (clipBoardString.Length > 0)
                {
                    clipBoardString = clipBoardString.Substring(0, clipBoardString.Length - 1); // Remove trailing "/"
                    Clipboard.SetText(clipBoardString);
                }
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true;

                ListBox lb = (ListBox)sender;
                String[] clipBoardStrings = Clipboard.GetText().Replace("\r\n","/").Split('/');
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
            if (e.KeyCode == Keys.Delete)
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
}