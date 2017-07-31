namespace OTGEdit
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btGenerate = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxWithBorder7 = new System.Windows.Forms.TextBox();
            this.textBoxWithBorder6 = new System.Windows.Forms.TextBox();
            this.textBoxWithBorder4 = new System.Windows.Forms.TextBox();
            this.textBoxWithBorder5 = new System.Windows.Forms.TextBox();
            this.rtbHelpTabLink1 = new System.Windows.Forms.RichTextBox();
            this.rtbHelpTabLink3 = new System.Windows.Forms.RichTextBox();
            this.rtbHelpTabLink2 = new System.Windows.Forms.RichTextBox();
            this.rtbHelpTabLink5 = new System.Windows.Forms.RichTextBox();
            this.rtbHelpTabLink4 = new System.Windows.Forms.RichTextBox();
            this.textBoxWithBorder1 = new System.Windows.Forms.TextBox();
            this.textBoxWithBorder2 = new System.Windows.Forms.TextBox();
            this.textBoxWithBorder3 = new System.Windows.Forms.TextBox();
            this.groupBoxWorldTab = new System.Windows.Forms.GroupBox();
            this.btWorldTabHelp = new System.Windows.Forms.Button();
            this.btSearchWorldConfigNext = new System.Windows.Forms.Button();
            this.btSearchWorldConfigPrev = new System.Windows.Forms.Button();
            this.tbSearchWorldConfig = new OTGEdit.Utils.TextBoxWithBorder();
            this.pnlWorldTabInputs = new OTGEdit.Utils.PanelWithScrollExposePanelWithScrollExposed();
            this.tlpWorldSettingsContainer = new System.Windows.Forms.TableLayoutPanel();
            this.tlpWorldSettings1 = new System.Windows.Forms.TableLayoutPanel();
            this.btSetToDefault = new System.Windows.Forms.Button();
            this.groupBoxBiomesTab = new System.Windows.Forms.GroupBox();
            this.btBiomeTabHelp = new System.Windows.Forms.Button();
            this.btSearchBiomeConfigNext = new System.Windows.Forms.Button();
            this.btSearchBiomeConfigPrev = new System.Windows.Forms.Button();
            this.tbSearchBiomeConfig = new OTGEdit.Utils.TextBoxWithBorder();
            this.pnlBiomesTabInputs = new OTGEdit.Utils.PanelWithScrollExposePanelWithScrollExposed();
            this.tlpBiomeSettingsContainer = new System.Windows.Forms.TableLayoutPanel();
            this.tlpBiomeSettings1 = new System.Windows.Forms.TableLayoutPanel();
            this.btCloneGroup = new System.Windows.Forms.Button();
            this.btGroupMoveDown = new System.Windows.Forms.Button();
            this.btGroupMoveUp = new System.Windows.Forms.Button();
            this.btEditGroup = new System.Windows.Forms.Button();
            this.btRemoveFromGroup = new System.Windows.Forms.Button();
            this.btAddToGroup = new System.Windows.Forms.Button();
            this.btDeleteGroup = new System.Windows.Forms.Button();
            this.btNewGroup = new System.Windows.Forms.Button();
            this.lblGroups = new System.Windows.Forms.Label();
            this.lbGroups = new System.Windows.Forms.ListBox();
            this.lblBiomesInGroup = new System.Windows.Forms.Label();
            this.lblAvailableBioms = new System.Windows.Forms.Label();
            this.lbGroup = new System.Windows.Forms.ListBox();
            this.lbBiomes = new System.Windows.Forms.ListBox();
            this.btSave = new System.Windows.Forms.Button();
            this.btLoad = new System.Windows.Forms.Button();
            this.cbDeleteRegion = new System.Windows.Forms.CheckBox();
            this.btCopyBO3s = new System.Windows.Forms.Button();
            this.btImportWorld = new System.Windows.Forms.Button();
            this.btSelectSourceWorld = new System.Windows.Forms.Button();
            this.cbVersion = new System.Windows.Forms.ComboBox();
            this.cbWorld = new System.Windows.Forms.ComboBox();
            this.pnlVersionWorldSelect = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnConvertSchematicToBO3 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBoxWorldTab.SuspendLayout();
            this.pnlWorldTabInputs.SuspendLayout();
            this.tlpWorldSettingsContainer.SuspendLayout();
            this.groupBoxBiomesTab.SuspendLayout();
            this.pnlBiomesTabInputs.SuspendLayout();
            this.tlpBiomeSettingsContainer.SuspendLayout();
            this.pnlVersionWorldSelect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btGenerate
            // 
            this.btGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btGenerate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btGenerate.ForeColor = System.Drawing.Color.White;
            this.btGenerate.Location = new System.Drawing.Point(11, 685);
            this.btGenerate.Name = "btGenerate";
            this.btGenerate.Size = new System.Drawing.Size(105, 25);
            this.btGenerate.TabIndex = 13;
            this.btGenerate.TabStop = false;
            this.btGenerate.Text = "Generate";
            this.btGenerate.UseVisualStyleBackColor = false;
            this.btGenerate.Visible = false;
            this.btGenerate.Click += new System.EventHandler(this.btGenerate_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 120);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1015, 555);
            this.tabControl1.TabIndex = 25;
            this.tabControl1.TabStop = false;
            this.tabControl1.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1007, 529);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "World Settings";
            this.tabPage1.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1007, 529);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Biome Settings";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1007, 529);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Help";
            this.tabPage2.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox3.Controls.Add(this.textBoxWithBorder7);
            this.groupBox3.Controls.Add(this.textBoxWithBorder6);
            this.groupBox3.Controls.Add(this.textBoxWithBorder4);
            this.groupBox3.Controls.Add(this.textBoxWithBorder5);
            this.groupBox3.Controls.Add(this.rtbHelpTabLink1);
            this.groupBox3.Controls.Add(this.rtbHelpTabLink3);
            this.groupBox3.Controls.Add(this.rtbHelpTabLink2);
            this.groupBox3.Controls.Add(this.rtbHelpTabLink5);
            this.groupBox3.Controls.Add(this.rtbHelpTabLink4);
            this.groupBox3.Controls.Add(this.textBoxWithBorder1);
            this.groupBox3.Controls.Add(this.textBoxWithBorder2);
            this.groupBox3.Controls.Add(this.textBoxWithBorder3);
            this.groupBox3.Location = new System.Drawing.Point(5, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(995, 521);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Help";
            this.groupBox3.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // textBoxWithBorder7
            // 
            this.textBoxWithBorder7.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxWithBorder7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxWithBorder7.Location = new System.Drawing.Point(26, 355);
            this.textBoxWithBorder7.Name = "textBoxWithBorder7";
            this.textBoxWithBorder7.ReadOnly = true;
            this.textBoxWithBorder7.Size = new System.Drawing.Size(899, 13);
            this.textBoxWithBorder7.TabIndex = 30;
            this.textBoxWithBorder7.TabStop = false;
            this.textBoxWithBorder7.Text = "MCW: PeeGee85";
            // 
            // textBoxWithBorder6
            // 
            this.textBoxWithBorder6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxWithBorder6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxWithBorder6.Location = new System.Drawing.Point(26, 329);
            this.textBoxWithBorder6.Name = "textBoxWithBorder6";
            this.textBoxWithBorder6.ReadOnly = true;
            this.textBoxWithBorder6.Size = new System.Drawing.Size(899, 13);
            this.textBoxWithBorder6.TabIndex = 29;
            this.textBoxWithBorder6.TabStop = false;
            this.textBoxWithBorder6.Text = "TerrainControl: RutgerKok, TimeThor, Khorn/Wickth, Olof Larsson. And lots of comm" +
    "unity contributors, special thanks to BloodMC and PeeGee85.";
            // 
            // textBoxWithBorder4
            // 
            this.textBoxWithBorder4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxWithBorder4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxWithBorder4.Location = new System.Drawing.Point(26, 277);
            this.textBoxWithBorder4.Name = "textBoxWithBorder4";
            this.textBoxWithBorder4.ReadOnly = true;
            this.textBoxWithBorder4.Size = new System.Drawing.Size(899, 13);
            this.textBoxWithBorder4.TabIndex = 28;
            this.textBoxWithBorder4.TabStop = false;
            this.textBoxWithBorder4.Text = "OTG / OTG+: PeeGee85 (code), MCPitman (wiki/reddit/websites, art, community)";
            // 
            // textBoxWithBorder5
            // 
            this.textBoxWithBorder5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxWithBorder5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxWithBorder5.Location = new System.Drawing.Point(26, 303);
            this.textBoxWithBorder5.Name = "textBoxWithBorder5";
            this.textBoxWithBorder5.ReadOnly = true;
            this.textBoxWithBorder5.Size = new System.Drawing.Size(899, 13);
            this.textBoxWithBorder5.TabIndex = 27;
            this.textBoxWithBorder5.TabStop = false;
            this.textBoxWithBorder5.Text = "Biome Bundle: MCPitman and LordSmellyPants";
            // 
            // rtbHelpTabLink1
            // 
            this.rtbHelpTabLink1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbHelpTabLink1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbHelpTabLink1.Location = new System.Drawing.Point(26, 64);
            this.rtbHelpTabLink1.Multiline = false;
            this.rtbHelpTabLink1.Name = "rtbHelpTabLink1";
            this.rtbHelpTabLink1.ReadOnly = true;
            this.rtbHelpTabLink1.Size = new System.Drawing.Size(899, 13);
            this.rtbHelpTabLink1.TabIndex = 26;
            this.rtbHelpTabLink1.TabStop = false;
            this.rtbHelpTabLink1.Text = "OTGEdit wiki: http://openterraingen.wikia.com/wiki/OTGEdit";
            // 
            // rtbHelpTabLink3
            // 
            this.rtbHelpTabLink3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbHelpTabLink3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbHelpTabLink3.Location = new System.Drawing.Point(26, 116);
            this.rtbHelpTabLink3.Multiline = false;
            this.rtbHelpTabLink3.Name = "rtbHelpTabLink3";
            this.rtbHelpTabLink3.ReadOnly = true;
            this.rtbHelpTabLink3.Size = new System.Drawing.Size(899, 13);
            this.rtbHelpTabLink3.TabIndex = 25;
            this.rtbHelpTabLink3.TabStop = false;
            this.rtbHelpTabLink3.Text = "OTG reddit: https://www.reddit.com/r/openterraingen/";
            // 
            // rtbHelpTabLink2
            // 
            this.rtbHelpTabLink2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbHelpTabLink2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbHelpTabLink2.Location = new System.Drawing.Point(26, 90);
            this.rtbHelpTabLink2.Multiline = false;
            this.rtbHelpTabLink2.Name = "rtbHelpTabLink2";
            this.rtbHelpTabLink2.ReadOnly = true;
            this.rtbHelpTabLink2.Size = new System.Drawing.Size(899, 13);
            this.rtbHelpTabLink2.TabIndex = 24;
            this.rtbHelpTabLink2.TabStop = false;
            this.rtbHelpTabLink2.Text = "OTG wiki: http://openterraingen.wikia.com/wiki/Open_Terrain_Generator_Wiki";
            // 
            // rtbHelpTabLink5
            // 
            this.rtbHelpTabLink5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbHelpTabLink5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbHelpTabLink5.Location = new System.Drawing.Point(26, 168);
            this.rtbHelpTabLink5.Multiline = false;
            this.rtbHelpTabLink5.Name = "rtbHelpTabLink5";
            this.rtbHelpTabLink5.ReadOnly = true;
            this.rtbHelpTabLink5.Size = new System.Drawing.Size(899, 13);
            this.rtbHelpTabLink5.TabIndex = 23;
            this.rtbHelpTabLink5.TabStop = false;
            this.rtbHelpTabLink5.Text = "MCW/OTGEdit video tutorials: https://www.youtube.com/user/PeeGee85";
            // 
            // rtbHelpTabLink4
            // 
            this.rtbHelpTabLink4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbHelpTabLink4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbHelpTabLink4.Location = new System.Drawing.Point(26, 142);
            this.rtbHelpTabLink4.Multiline = false;
            this.rtbHelpTabLink4.Name = "rtbHelpTabLink4";
            this.rtbHelpTabLink4.ReadOnly = true;
            this.rtbHelpTabLink4.Size = new System.Drawing.Size(899, 13);
            this.rtbHelpTabLink4.TabIndex = 16;
            this.rtbHelpTabLink4.TabStop = false;
            this.rtbHelpTabLink4.Text = "TerrainControl Wiki: https://github.com/MCTCP/TerrainControl/wiki";
            // 
            // textBoxWithBorder1
            // 
            this.textBoxWithBorder1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxWithBorder1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxWithBorder1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxWithBorder1.Location = new System.Drawing.Point(26, 32);
            this.textBoxWithBorder1.Name = "textBoxWithBorder1";
            this.textBoxWithBorder1.ReadOnly = true;
            this.textBoxWithBorder1.Size = new System.Drawing.Size(899, 13);
            this.textBoxWithBorder1.TabIndex = 13;
            this.textBoxWithBorder1.TabStop = false;
            this.textBoxWithBorder1.Text = "Resources:";
            // 
            // textBoxWithBorder2
            // 
            this.textBoxWithBorder2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxWithBorder2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxWithBorder2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxWithBorder2.Location = new System.Drawing.Point(26, 218);
            this.textBoxWithBorder2.Name = "textBoxWithBorder2";
            this.textBoxWithBorder2.ReadOnly = true;
            this.textBoxWithBorder2.Size = new System.Drawing.Size(899, 13);
            this.textBoxWithBorder2.TabIndex = 12;
            this.textBoxWithBorder2.TabStop = false;
            this.textBoxWithBorder2.Text = "Credits:";
            // 
            // textBoxWithBorder3
            // 
            this.textBoxWithBorder3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxWithBorder3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxWithBorder3.Location = new System.Drawing.Point(26, 251);
            this.textBoxWithBorder3.Name = "textBoxWithBorder3";
            this.textBoxWithBorder3.ReadOnly = true;
            this.textBoxWithBorder3.Size = new System.Drawing.Size(899, 13);
            this.textBoxWithBorder3.TabIndex = 10;
            this.textBoxWithBorder3.TabStop = false;
            this.textBoxWithBorder3.Text = "OTGEdit: PeeGee85 (code), MCPitman (pre-sets)";
            // 
            // groupBoxWorldTab
            // 
            this.groupBoxWorldTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxWorldTab.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBoxWorldTab.Controls.Add(this.btWorldTabHelp);
            this.groupBoxWorldTab.Controls.Add(this.btSearchWorldConfigNext);
            this.groupBoxWorldTab.Controls.Add(this.btSearchWorldConfigPrev);
            this.groupBoxWorldTab.Controls.Add(this.tbSearchWorldConfig);
            this.groupBoxWorldTab.Controls.Add(this.pnlWorldTabInputs);
            this.groupBoxWorldTab.Controls.Add(this.btSetToDefault);
            this.groupBoxWorldTab.Location = new System.Drawing.Point(21, 144);
            this.groupBoxWorldTab.Name = "groupBoxWorldTab";
            this.groupBoxWorldTab.Size = new System.Drawing.Size(995, 521);
            this.groupBoxWorldTab.TabIndex = 26;
            this.groupBoxWorldTab.TabStop = false;
            this.groupBoxWorldTab.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // btWorldTabHelp
            // 
            this.btWorldTabHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btWorldTabHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btWorldTabHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btWorldTabHelp.ForeColor = System.Drawing.Color.White;
            this.btWorldTabHelp.Location = new System.Drawing.Point(905, 13);
            this.btWorldTabHelp.Name = "btWorldTabHelp";
            this.btWorldTabHelp.Size = new System.Drawing.Size(85, 25);
            this.btWorldTabHelp.TabIndex = 33;
            this.btWorldTabHelp.TabStop = false;
            this.btWorldTabHelp.Text = "Help";
            this.btWorldTabHelp.UseVisualStyleBackColor = false;
            this.btWorldTabHelp.Click += new System.EventHandler(this.btWorldTabHelp_Click);
            // 
            // btSearchWorldConfigNext
            // 
            this.btSearchWorldConfigNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSearchWorldConfigNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btSearchWorldConfigNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSearchWorldConfigNext.ForeColor = System.Drawing.Color.White;
            this.btSearchWorldConfigNext.Location = new System.Drawing.Point(875, 13);
            this.btSearchWorldConfigNext.Name = "btSearchWorldConfigNext";
            this.btSearchWorldConfigNext.Size = new System.Drawing.Size(26, 25);
            this.btSearchWorldConfigNext.TabIndex = 31;
            this.btSearchWorldConfigNext.TabStop = false;
            this.btSearchWorldConfigNext.Text = ">";
            this.btSearchWorldConfigNext.UseVisualStyleBackColor = false;
            this.btSearchWorldConfigNext.Click += new System.EventHandler(this.btSearchWorldConfigNext_Click);
            // 
            // btSearchWorldConfigPrev
            // 
            this.btSearchWorldConfigPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSearchWorldConfigPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btSearchWorldConfigPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSearchWorldConfigPrev.ForeColor = System.Drawing.Color.White;
            this.btSearchWorldConfigPrev.Location = new System.Drawing.Point(845, 13);
            this.btSearchWorldConfigPrev.Name = "btSearchWorldConfigPrev";
            this.btSearchWorldConfigPrev.Size = new System.Drawing.Size(26, 25);
            this.btSearchWorldConfigPrev.TabIndex = 30;
            this.btSearchWorldConfigPrev.TabStop = false;
            this.btSearchWorldConfigPrev.Text = "<";
            this.btSearchWorldConfigPrev.UseVisualStyleBackColor = false;
            this.btSearchWorldConfigPrev.Click += new System.EventHandler(this.btSearchWorldConfigPrev_Click);
            // 
            // tbSearchWorldConfig
            // 
            this.tbSearchWorldConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchWorldConfig.Location = new System.Drawing.Point(685, 16);
            this.tbSearchWorldConfig.Name = "tbSearchWorldConfig";
            this.tbSearchWorldConfig.Size = new System.Drawing.Size(154, 20);
            this.tbSearchWorldConfig.TabIndex = 29;
            this.tbSearchWorldConfig.TextChanged += new System.EventHandler(this.tbSearchWorldConfig_TextChanged);
            // 
            // pnlWorldTabInputs
            // 
            this.pnlWorldTabInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlWorldTabInputs.AutoScroll = true;
            this.pnlWorldTabInputs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlWorldTabInputs.Controls.Add(this.tlpWorldSettingsContainer);
            this.pnlWorldTabInputs.Location = new System.Drawing.Point(6, 45);
            this.pnlWorldTabInputs.Name = "pnlWorldTabInputs";
            this.pnlWorldTabInputs.Size = new System.Drawing.Size(983, 465);
            this.pnlWorldTabInputs.TabIndex = 28;
            // 
            // tlpWorldSettingsContainer
            // 
            this.tlpWorldSettingsContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpWorldSettingsContainer.AutoSize = true;
            this.tlpWorldSettingsContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpWorldSettingsContainer.ColumnCount = 1;
            this.tlpWorldSettingsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpWorldSettingsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpWorldSettingsContainer.Controls.Add(this.tlpWorldSettings1, 0, 0);
            this.tlpWorldSettingsContainer.Location = new System.Drawing.Point(0, 0);
            this.tlpWorldSettingsContainer.Name = "tlpWorldSettingsContainer";
            this.tlpWorldSettingsContainer.RowCount = 1;
            this.tlpWorldSettingsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpWorldSettingsContainer.Size = new System.Drawing.Size(983, 6);
            this.tlpWorldSettingsContainer.TabIndex = 27;
            this.tlpWorldSettingsContainer.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // tlpWorldSettings1
            // 
            this.tlpWorldSettings1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpWorldSettings1.AutoSize = true;
            this.tlpWorldSettings1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpWorldSettings1.ColumnCount = 4;
            this.tlpWorldSettings1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpWorldSettings1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpWorldSettings1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpWorldSettings1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpWorldSettings1.Location = new System.Drawing.Point(3, 3);
            this.tlpWorldSettings1.Name = "tlpWorldSettings1";
            this.tlpWorldSettings1.RowCount = 1;
            this.tlpWorldSettings1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpWorldSettings1.Size = new System.Drawing.Size(977, 0);
            this.tlpWorldSettings1.TabIndex = 19;
            this.tlpWorldSettings1.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // btSetToDefault
            // 
            this.btSetToDefault.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btSetToDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSetToDefault.ForeColor = System.Drawing.Color.White;
            this.btSetToDefault.Location = new System.Drawing.Point(7, 13);
            this.btSetToDefault.Name = "btSetToDefault";
            this.btSetToDefault.Size = new System.Drawing.Size(85, 25);
            this.btSetToDefault.TabIndex = 26;
            this.btSetToDefault.TabStop = false;
            this.btSetToDefault.Text = "Clear all";
            this.btSetToDefault.UseVisualStyleBackColor = false;
            this.btSetToDefault.Click += new System.EventHandler(this.btWorldSettingsSetToDefault_Click);
            // 
            // groupBoxBiomesTab
            // 
            this.groupBoxBiomesTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxBiomesTab.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBoxBiomesTab.Controls.Add(this.btBiomeTabHelp);
            this.groupBoxBiomesTab.Controls.Add(this.btSearchBiomeConfigNext);
            this.groupBoxBiomesTab.Controls.Add(this.btSearchBiomeConfigPrev);
            this.groupBoxBiomesTab.Controls.Add(this.tbSearchBiomeConfig);
            this.groupBoxBiomesTab.Controls.Add(this.pnlBiomesTabInputs);
            this.groupBoxBiomesTab.Controls.Add(this.btCloneGroup);
            this.groupBoxBiomesTab.Controls.Add(this.btGroupMoveDown);
            this.groupBoxBiomesTab.Controls.Add(this.btGroupMoveUp);
            this.groupBoxBiomesTab.Controls.Add(this.btEditGroup);
            this.groupBoxBiomesTab.Controls.Add(this.btRemoveFromGroup);
            this.groupBoxBiomesTab.Controls.Add(this.btAddToGroup);
            this.groupBoxBiomesTab.Controls.Add(this.btDeleteGroup);
            this.groupBoxBiomesTab.Controls.Add(this.btNewGroup);
            this.groupBoxBiomesTab.Controls.Add(this.lblGroups);
            this.groupBoxBiomesTab.Controls.Add(this.lbGroups);
            this.groupBoxBiomesTab.Controls.Add(this.lblBiomesInGroup);
            this.groupBoxBiomesTab.Controls.Add(this.lblAvailableBioms);
            this.groupBoxBiomesTab.Controls.Add(this.lbGroup);
            this.groupBoxBiomesTab.Controls.Add(this.lbBiomes);
            this.groupBoxBiomesTab.Location = new System.Drawing.Point(21, 144);
            this.groupBoxBiomesTab.Name = "groupBoxBiomesTab";
            this.groupBoxBiomesTab.Size = new System.Drawing.Size(995, 521);
            this.groupBoxBiomesTab.TabIndex = 0;
            this.groupBoxBiomesTab.TabStop = false;
            this.groupBoxBiomesTab.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // btBiomeTabHelp
            // 
            this.btBiomeTabHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btBiomeTabHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btBiomeTabHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btBiomeTabHelp.ForeColor = System.Drawing.Color.White;
            this.btBiomeTabHelp.Location = new System.Drawing.Point(905, 13);
            this.btBiomeTabHelp.Name = "btBiomeTabHelp";
            this.btBiomeTabHelp.Size = new System.Drawing.Size(85, 25);
            this.btBiomeTabHelp.TabIndex = 42;
            this.btBiomeTabHelp.TabStop = false;
            this.btBiomeTabHelp.Text = "Help";
            this.btBiomeTabHelp.UseVisualStyleBackColor = false;
            this.btBiomeTabHelp.Click += new System.EventHandler(this.btBiomeTabHelp_Click);
            // 
            // btSearchBiomeConfigNext
            // 
            this.btSearchBiomeConfigNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSearchBiomeConfigNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btSearchBiomeConfigNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSearchBiomeConfigNext.ForeColor = System.Drawing.Color.White;
            this.btSearchBiomeConfigNext.Location = new System.Drawing.Point(875, 13);
            this.btSearchBiomeConfigNext.Name = "btSearchBiomeConfigNext";
            this.btSearchBiomeConfigNext.Size = new System.Drawing.Size(26, 25);
            this.btSearchBiomeConfigNext.TabIndex = 40;
            this.btSearchBiomeConfigNext.TabStop = false;
            this.btSearchBiomeConfigNext.Text = ">";
            this.btSearchBiomeConfigNext.UseVisualStyleBackColor = false;
            this.btSearchBiomeConfigNext.Click += new System.EventHandler(this.btSearchBiomeConfigNext_Click);
            // 
            // btSearchBiomeConfigPrev
            // 
            this.btSearchBiomeConfigPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSearchBiomeConfigPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btSearchBiomeConfigPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSearchBiomeConfigPrev.ForeColor = System.Drawing.Color.White;
            this.btSearchBiomeConfigPrev.Location = new System.Drawing.Point(845, 13);
            this.btSearchBiomeConfigPrev.Name = "btSearchBiomeConfigPrev";
            this.btSearchBiomeConfigPrev.Size = new System.Drawing.Size(26, 25);
            this.btSearchBiomeConfigPrev.TabIndex = 39;
            this.btSearchBiomeConfigPrev.TabStop = false;
            this.btSearchBiomeConfigPrev.Text = "<";
            this.btSearchBiomeConfigPrev.UseVisualStyleBackColor = false;
            this.btSearchBiomeConfigPrev.Click += new System.EventHandler(this.btSearchBiomeConfigPrev_Click);
            // 
            // tbSearchBiomeConfig
            // 
            this.tbSearchBiomeConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchBiomeConfig.Location = new System.Drawing.Point(685, 16);
            this.tbSearchBiomeConfig.Name = "tbSearchBiomeConfig";
            this.tbSearchBiomeConfig.Size = new System.Drawing.Size(154, 20);
            this.tbSearchBiomeConfig.TabIndex = 38;
            this.tbSearchBiomeConfig.TextChanged += new System.EventHandler(this.tbSearchBiomeConfig_TextChanged);
            // 
            // pnlBiomesTabInputs
            // 
            this.pnlBiomesTabInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlBiomesTabInputs.AutoScroll = true;
            this.pnlBiomesTabInputs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlBiomesTabInputs.Controls.Add(this.tlpBiomeSettingsContainer);
            this.pnlBiomesTabInputs.Location = new System.Drawing.Point(220, 43);
            this.pnlBiomesTabInputs.Name = "pnlBiomesTabInputs";
            this.pnlBiomesTabInputs.Size = new System.Drawing.Size(769, 467);
            this.pnlBiomesTabInputs.TabIndex = 37;
            // 
            // tlpBiomeSettingsContainer
            // 
            this.tlpBiomeSettingsContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpBiomeSettingsContainer.AutoSize = true;
            this.tlpBiomeSettingsContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpBiomeSettingsContainer.ColumnCount = 1;
            this.tlpBiomeSettingsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBiomeSettingsContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpBiomeSettingsContainer.Controls.Add(this.tlpBiomeSettings1, 0, 0);
            this.tlpBiomeSettingsContainer.Location = new System.Drawing.Point(0, 0);
            this.tlpBiomeSettingsContainer.Name = "tlpBiomeSettingsContainer";
            this.tlpBiomeSettingsContainer.RowCount = 1;
            this.tlpBiomeSettingsContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBiomeSettingsContainer.Size = new System.Drawing.Size(769, 6);
            this.tlpBiomeSettingsContainer.TabIndex = 27;
            this.tlpBiomeSettingsContainer.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // tlpBiomeSettings1
            // 
            this.tlpBiomeSettings1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpBiomeSettings1.AutoSize = true;
            this.tlpBiomeSettings1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpBiomeSettings1.ColumnCount = 4;
            this.tlpBiomeSettings1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpBiomeSettings1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpBiomeSettings1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpBiomeSettings1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpBiomeSettings1.Location = new System.Drawing.Point(3, 3);
            this.tlpBiomeSettings1.Name = "tlpBiomeSettings1";
            this.tlpBiomeSettings1.RowCount = 1;
            this.tlpBiomeSettings1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBiomeSettings1.Size = new System.Drawing.Size(763, 0);
            this.tlpBiomeSettings1.TabIndex = 19;
            this.tlpBiomeSettings1.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // btCloneGroup
            // 
            this.btCloneGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btCloneGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btCloneGroup.ForeColor = System.Drawing.Color.White;
            this.btCloneGroup.Location = new System.Drawing.Point(155, 196);
            this.btCloneGroup.Name = "btCloneGroup";
            this.btCloneGroup.Size = new System.Drawing.Size(50, 23);
            this.btCloneGroup.TabIndex = 35;
            this.btCloneGroup.TabStop = false;
            this.btCloneGroup.Text = "Clone";
            this.btCloneGroup.UseVisualStyleBackColor = false;
            this.btCloneGroup.Click += new System.EventHandler(this.btCloneGroup_Click);
            // 
            // btGroupMoveDown
            // 
            this.btGroupMoveDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btGroupMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btGroupMoveDown.ForeColor = System.Drawing.Color.White;
            this.btGroupMoveDown.Location = new System.Drawing.Point(111, 167);
            this.btGroupMoveDown.Name = "btGroupMoveDown";
            this.btGroupMoveDown.Size = new System.Drawing.Size(94, 23);
            this.btGroupMoveDown.TabIndex = 34;
            this.btGroupMoveDown.TabStop = false;
            this.btGroupMoveDown.Text = "Move down";
            this.btGroupMoveDown.UseVisualStyleBackColor = false;
            this.btGroupMoveDown.Click += new System.EventHandler(this.btGroupMoveDown_Click);
            // 
            // btGroupMoveUp
            // 
            this.btGroupMoveUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btGroupMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btGroupMoveUp.ForeColor = System.Drawing.Color.White;
            this.btGroupMoveUp.Location = new System.Drawing.Point(8, 167);
            this.btGroupMoveUp.Name = "btGroupMoveUp";
            this.btGroupMoveUp.Size = new System.Drawing.Size(97, 23);
            this.btGroupMoveUp.TabIndex = 33;
            this.btGroupMoveUp.TabStop = false;
            this.btGroupMoveUp.Text = "Move up";
            this.btGroupMoveUp.UseVisualStyleBackColor = false;
            this.btGroupMoveUp.Click += new System.EventHandler(this.btGroupMoveUp_Click);
            // 
            // btEditGroup
            // 
            this.btEditGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btEditGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btEditGroup.ForeColor = System.Drawing.Color.White;
            this.btEditGroup.Location = new System.Drawing.Point(53, 196);
            this.btEditGroup.Name = "btEditGroup";
            this.btEditGroup.Size = new System.Drawing.Size(40, 23);
            this.btEditGroup.TabIndex = 32;
            this.btEditGroup.TabStop = false;
            this.btEditGroup.Text = "Edit";
            this.btEditGroup.UseVisualStyleBackColor = false;
            this.btEditGroup.Click += new System.EventHandler(this.btEditGroup_Click);
            // 
            // btRemoveFromGroup
            // 
            this.btRemoveFromGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btRemoveFromGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btRemoveFromGroup.ForeColor = System.Drawing.Color.White;
            this.btRemoveFromGroup.Location = new System.Drawing.Point(111, 357);
            this.btRemoveFromGroup.Name = "btRemoveFromGroup";
            this.btRemoveFromGroup.Size = new System.Drawing.Size(95, 23);
            this.btRemoveFromGroup.TabIndex = 31;
            this.btRemoveFromGroup.TabStop = false;
            this.btRemoveFromGroup.Text = "Remove";
            this.btRemoveFromGroup.UseVisualStyleBackColor = false;
            this.btRemoveFromGroup.Click += new System.EventHandler(this.btRemoveFromGroup_Click);
            // 
            // btAddToGroup
            // 
            this.btAddToGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btAddToGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btAddToGroup.ForeColor = System.Drawing.Color.White;
            this.btAddToGroup.Location = new System.Drawing.Point(9, 357);
            this.btAddToGroup.Name = "btAddToGroup";
            this.btAddToGroup.Size = new System.Drawing.Size(96, 23);
            this.btAddToGroup.TabIndex = 30;
            this.btAddToGroup.TabStop = false;
            this.btAddToGroup.Text = "Add";
            this.btAddToGroup.UseVisualStyleBackColor = false;
            this.btAddToGroup.Click += new System.EventHandler(this.btAddToGroup_Click);
            // 
            // btDeleteGroup
            // 
            this.btDeleteGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btDeleteGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btDeleteGroup.ForeColor = System.Drawing.Color.White;
            this.btDeleteGroup.Location = new System.Drawing.Point(99, 196);
            this.btDeleteGroup.Name = "btDeleteGroup";
            this.btDeleteGroup.Size = new System.Drawing.Size(50, 23);
            this.btDeleteGroup.TabIndex = 29;
            this.btDeleteGroup.TabStop = false;
            this.btDeleteGroup.Text = "Delete";
            this.btDeleteGroup.UseVisualStyleBackColor = false;
            this.btDeleteGroup.Click += new System.EventHandler(this.btDeleteGroup_Click);
            // 
            // btNewGroup
            // 
            this.btNewGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btNewGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btNewGroup.ForeColor = System.Drawing.Color.White;
            this.btNewGroup.Location = new System.Drawing.Point(8, 196);
            this.btNewGroup.Name = "btNewGroup";
            this.btNewGroup.Size = new System.Drawing.Size(39, 23);
            this.btNewGroup.TabIndex = 28;
            this.btNewGroup.TabStop = false;
            this.btNewGroup.Text = "New";
            this.btNewGroup.UseVisualStyleBackColor = false;
            this.btNewGroup.Click += new System.EventHandler(this.btNewGroup_Click);
            // 
            // lblGroups
            // 
            this.lblGroups.AutoSize = true;
            this.lblGroups.Location = new System.Drawing.Point(6, 19);
            this.lblGroups.Name = "lblGroups";
            this.lblGroups.Size = new System.Drawing.Size(41, 13);
            this.lblGroups.TabIndex = 27;
            this.lblGroups.Text = "Groups";
            // 
            // lbGroups
            // 
            this.lbGroups.FormattingEnabled = true;
            this.lbGroups.Location = new System.Drawing.Point(9, 36);
            this.lbGroups.Name = "lbGroups";
            this.lbGroups.Size = new System.Drawing.Size(195, 121);
            this.lbGroups.TabIndex = 26;
            this.lbGroups.TabStop = false;
            this.lbGroups.SelectedIndexChanged += new System.EventHandler(this.lbGroups_SelectedIndexChanged);
            // 
            // lblBiomesInGroup
            // 
            this.lblBiomesInGroup.AutoSize = true;
            this.lblBiomesInGroup.Location = new System.Drawing.Point(5, 224);
            this.lblBiomesInGroup.Name = "lblBiomesInGroup";
            this.lblBiomesInGroup.Size = new System.Drawing.Size(82, 13);
            this.lblBiomesInGroup.TabIndex = 25;
            this.lblBiomesInGroup.Text = "Biomes in group";
            // 
            // lblAvailableBioms
            // 
            this.lblAvailableBioms.AutoSize = true;
            this.lblAvailableBioms.Location = new System.Drawing.Point(6, 385);
            this.lblAvailableBioms.Name = "lblAvailableBioms";
            this.lblAvailableBioms.Size = new System.Drawing.Size(88, 13);
            this.lblAvailableBioms.TabIndex = 24;
            this.lblAvailableBioms.Text = "Avalilable biomes";
            // 
            // lbGroup
            // 
            this.lbGroup.FormattingEnabled = true;
            this.lbGroup.Location = new System.Drawing.Point(9, 243);
            this.lbGroup.Name = "lbGroup";
            this.lbGroup.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbGroup.Size = new System.Drawing.Size(195, 108);
            this.lbGroup.Sorted = true;
            this.lbGroup.TabIndex = 23;
            this.lbGroup.TabStop = false;
            this.lbGroup.SelectedIndexChanged += new System.EventHandler(this.lbGroup_SelectedIndexChanged);
            // 
            // lbBiomes
            // 
            this.lbBiomes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbBiomes.FormattingEnabled = true;
            this.lbBiomes.Location = new System.Drawing.Point(9, 402);
            this.lbBiomes.Name = "lbBiomes";
            this.lbBiomes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbBiomes.Size = new System.Drawing.Size(195, 108);
            this.lbBiomes.Sorted = true;
            this.lbBiomes.TabIndex = 0;
            this.lbBiomes.TabStop = false;
            // 
            // btSave
            // 
            this.btSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSave.ForeColor = System.Drawing.Color.White;
            this.btSave.Location = new System.Drawing.Point(17, 65);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(105, 25);
            this.btSave.TabIndex = 26;
            this.btSave.TabStop = false;
            this.btSave.Text = "Save settings";
            this.btSave.UseVisualStyleBackColor = false;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btLoad
            // 
            this.btLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btLoad.ForeColor = System.Drawing.Color.White;
            this.btLoad.Location = new System.Drawing.Point(128, 65);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(109, 25);
            this.btLoad.TabIndex = 28;
            this.btLoad.TabStop = false;
            this.btLoad.Text = "Load settings";
            this.btLoad.UseVisualStyleBackColor = false;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // cbDeleteRegion
            // 
            this.cbDeleteRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbDeleteRegion.AutoSize = true;
            this.cbDeleteRegion.Checked = true;
            this.cbDeleteRegion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDeleteRegion.Location = new System.Drawing.Point(125, 690);
            this.cbDeleteRegion.Name = "cbDeleteRegion";
            this.cbDeleteRegion.Size = new System.Drawing.Size(137, 17);
            this.cbDeleteRegion.TabIndex = 33;
            this.cbDeleteRegion.TabStop = false;
            this.cbDeleteRegion.Text = "Delete /region directory";
            this.cbDeleteRegion.UseVisualStyleBackColor = true;
            this.cbDeleteRegion.Visible = false;
            // 
            // btCopyBO3s
            // 
            this.btCopyBO3s.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btCopyBO3s.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btCopyBO3s.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btCopyBO3s.ForeColor = System.Drawing.Color.White;
            this.btCopyBO3s.Location = new System.Drawing.Point(273, 685);
            this.btCopyBO3s.Name = "btCopyBO3s";
            this.btCopyBO3s.Size = new System.Drawing.Size(162, 25);
            this.btCopyBO3s.TabIndex = 36;
            this.btCopyBO3s.TabStop = false;
            this.btCopyBO3s.Text = "Copy structure files (BO3s)";
            this.btCopyBO3s.UseVisualStyleBackColor = false;
            this.btCopyBO3s.Visible = false;
            this.btCopyBO3s.Click += new System.EventHandler(this.btCopyBO3s_Click);
            // 
            // btImportWorld
            // 
            this.btImportWorld.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btImportWorld.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btImportWorld.ForeColor = System.Drawing.Color.White;
            this.btImportWorld.Location = new System.Drawing.Point(243, 65);
            this.btImportWorld.Name = "btImportWorld";
            this.btImportWorld.Size = new System.Drawing.Size(126, 25);
            this.btImportWorld.TabIndex = 37;
            this.btImportWorld.TabStop = false;
            this.btImportWorld.Text = "Manage worlds";
            this.btImportWorld.UseVisualStyleBackColor = false;
            this.btImportWorld.Click += new System.EventHandler(this.btmanageWorlds_Click);
            // 
            // btSelectSourceWorld
            // 
            this.btSelectSourceWorld.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btSelectSourceWorld.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSelectSourceWorld.ForeColor = System.Drawing.Color.White;
            this.btSelectSourceWorld.Location = new System.Drawing.Point(418, 36);
            this.btSelectSourceWorld.Name = "btSelectSourceWorld";
            this.btSelectSourceWorld.Size = new System.Drawing.Size(125, 23);
            this.btSelectSourceWorld.TabIndex = 8;
            this.btSelectSourceWorld.TabStop = false;
            this.btSelectSourceWorld.Text = "Select";
            this.btSelectSourceWorld.UseVisualStyleBackColor = false;
            this.btSelectSourceWorld.Click += new System.EventHandler(this.SelectSourceWorld_Click);
            // 
            // cbVersion
            // 
            this.cbVersion.BackColor = System.Drawing.Color.White;
            this.cbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbVersion.ForeColor = System.Drawing.Color.Black;
            this.cbVersion.FormattingEnabled = true;
            this.cbVersion.Location = new System.Drawing.Point(17, 36);
            this.cbVersion.Name = "cbVersion";
            this.cbVersion.Size = new System.Drawing.Size(191, 21);
            this.cbVersion.TabIndex = 31;
            this.cbVersion.TabStop = false;
            // 
            // cbWorld
            // 
            this.cbWorld.BackColor = System.Drawing.Color.White;
            this.cbWorld.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorld.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbWorld.ForeColor = System.Drawing.Color.Black;
            this.cbWorld.FormattingEnabled = true;
            this.cbWorld.Location = new System.Drawing.Point(214, 36);
            this.cbWorld.Name = "cbWorld";
            this.cbWorld.Size = new System.Drawing.Size(198, 21);
            this.cbWorld.TabIndex = 33;
            this.cbWorld.TabStop = false;
            // 
            // pnlVersionWorldSelect
            // 
            this.pnlVersionWorldSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlVersionWorldSelect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.pnlVersionWorldSelect.Controls.Add(this.pictureBox1);
            this.pnlVersionWorldSelect.Controls.Add(this.label4);
            this.pnlVersionWorldSelect.Controls.Add(this.label1);
            this.pnlVersionWorldSelect.Controls.Add(this.cbWorld);
            this.pnlVersionWorldSelect.Controls.Add(this.btImportWorld);
            this.pnlVersionWorldSelect.Controls.Add(this.btSave);
            this.pnlVersionWorldSelect.Controls.Add(this.cbVersion);
            this.pnlVersionWorldSelect.Controls.Add(this.btSelectSourceWorld);
            this.pnlVersionWorldSelect.Controls.Add(this.btnConvertSchematicToBO3);
            this.pnlVersionWorldSelect.Controls.Add(this.btLoad);
            this.pnlVersionWorldSelect.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pnlVersionWorldSelect.Location = new System.Drawing.Point(0, -3);
            this.pnlVersionWorldSelect.Name = "pnlVersionWorldSelect";
            this.pnlVersionWorldSelect.Size = new System.Drawing.Size(1042, 107);
            this.pnlVersionWorldSelect.TabIndex = 21;
            this.pnlVersionWorldSelect.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::OTGEdit.Properties.Resources.OTGEdit;
            this.pictureBox1.Location = new System.Drawing.Point(593, 27);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(304, 63);
            this.pictureBox1.TabIndex = 38;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(212, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "World template";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(16, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "OTG / TerrainControl version";
            // 
            // btnConvertSchematicToBO3
            // 
            this.btnConvertSchematicToBO3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(150)))), ((int)(((byte)(134)))));
            this.btnConvertSchematicToBO3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConvertSchematicToBO3.ForeColor = System.Drawing.Color.White;
            this.btnConvertSchematicToBO3.Location = new System.Drawing.Point(375, 65);
            this.btnConvertSchematicToBO3.Name = "btnConvertSchematicToBO3";
            this.btnConvertSchematicToBO3.Size = new System.Drawing.Size(168, 25);
            this.btnConvertSchematicToBO3.TabIndex = 35;
            this.btnConvertSchematicToBO3.TabStop = false;
            this.btnConvertSchematicToBO3.Text = "Convert .schematic to BO3";
            this.btnConvertSchematicToBO3.UseVisualStyleBackColor = false;
            this.btnConvertSchematicToBO3.Click += new System.EventHandler(this.btnConvertSchematicToBO3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(224)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1039, 718);
            this.Controls.Add(this.groupBoxBiomesTab);
            this.Controls.Add(this.btCopyBO3s);
            this.Controls.Add(this.groupBoxWorldTab);
            this.Controls.Add(this.cbDeleteRegion);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pnlVersionWorldSelect);
            this.Controls.Add(this.btGenerate);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "OTGEdit v1.0.12";
            this.Click += new System.EventHandler(this.btClickBackGround);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBoxWorldTab.ResumeLayout(false);
            this.groupBoxWorldTab.PerformLayout();
            this.pnlWorldTabInputs.ResumeLayout(false);
            this.pnlWorldTabInputs.PerformLayout();
            this.tlpWorldSettingsContainer.ResumeLayout(false);
            this.tlpWorldSettingsContainer.PerformLayout();
            this.groupBoxBiomesTab.ResumeLayout(false);
            this.groupBoxBiomesTab.PerformLayout();
            this.pnlBiomesTabInputs.ResumeLayout(false);
            this.pnlBiomesTabInputs.PerformLayout();
            this.tlpBiomeSettingsContainer.ResumeLayout(false);
            this.tlpBiomeSettingsContainer.PerformLayout();
            this.pnlVersionWorldSelect.ResumeLayout(false);
            this.pnlVersionWorldSelect.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btGenerate;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBoxWorldTab;
        private System.Windows.Forms.Button btSetToDefault;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBoxBiomesTab;
        private System.Windows.Forms.ListBox lbBiomes;
        private System.Windows.Forms.Label lblGroups;
        private System.Windows.Forms.ListBox lbGroups;
        private System.Windows.Forms.Label lblBiomesInGroup;
        private System.Windows.Forms.Label lblAvailableBioms;
        private System.Windows.Forms.ListBox lbGroup;
        private System.Windows.Forms.Button btRemoveFromGroup;
        private System.Windows.Forms.Button btAddToGroup;
        private System.Windows.Forms.Button btDeleteGroup;
        private System.Windows.Forms.Button btNewGroup;
        private System.Windows.Forms.Button btEditGroup;
        private System.Windows.Forms.Button btGroupMoveDown;
        private System.Windows.Forms.Button btGroupMoveUp;
        private System.Windows.Forms.CheckBox cbDeleteRegion;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxWithBorder1;
        private System.Windows.Forms.TextBox textBoxWithBorder2;
        private System.Windows.Forms.TextBox textBoxWithBorder3;
        private System.Windows.Forms.RichTextBox rtbHelpTabLink5;
        private System.Windows.Forms.Button btCopyBO3s;
        private System.Windows.Forms.Button btImportWorld;
        private System.Windows.Forms.Button btCloneGroup;
        private System.Windows.Forms.RichTextBox rtbHelpTabLink3;
        private System.Windows.Forms.RichTextBox rtbHelpTabLink2;
        private System.Windows.Forms.RichTextBox rtbHelpTabLink4;
        private System.Windows.Forms.Button btSelectSourceWorld;
        private System.Windows.Forms.ComboBox cbVersion;
        private System.Windows.Forms.ComboBox cbWorld;
        private System.Windows.Forms.Panel pnlVersionWorldSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tlpWorldSettings1;
        private System.Windows.Forms.TableLayoutPanel tlpWorldSettingsContainer;
        private Utils.PanelWithScrollExposePanelWithScrollExposed pnlWorldTabInputs;
        private System.Windows.Forms.TableLayoutPanel tlpBiomeSettings1;
        private System.Windows.Forms.TableLayoutPanel tlpBiomeSettingsContainer;
        private Utils.PanelWithScrollExposePanelWithScrollExposed pnlBiomesTabInputs;
        private Utils.TextBoxWithBorder tbSearchWorldConfig;
        private System.Windows.Forms.Button btSearchWorldConfigNext;
        private System.Windows.Forms.Button btSearchWorldConfigPrev;
        private System.Windows.Forms.Button btSearchBiomeConfigNext;
        private System.Windows.Forms.Button btSearchBiomeConfigPrev;
        private Utils.TextBoxWithBorder tbSearchBiomeConfig;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnConvertSchematicToBO3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBoxWithBorder6;
        private System.Windows.Forms.TextBox textBoxWithBorder4;
        private System.Windows.Forms.TextBox textBoxWithBorder5;
        private System.Windows.Forms.RichTextBox rtbHelpTabLink1;
        private System.Windows.Forms.TextBox textBoxWithBorder7;
        private System.Windows.Forms.Button btWorldTabHelp;
        private System.Windows.Forms.Button btBiomeTabHelp;
    }
}

