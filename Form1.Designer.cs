namespace TCEE
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
            this.btSelectSourceWorld = new System.Windows.Forms.Button();
            this.btGenerate = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbWorld = new System.Windows.Forms.ComboBox();
            this.cbVersion = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btSetToDefault = new System.Windows.Forms.Button();
            this.tlpWorldSettings = new System.Windows.Forms.TableLayoutPanel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
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
            this.btBiomeSettingsResetToDefaults = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tlpBiomeSettings = new System.Windows.Forms.TableLayoutPanel();
            this.lbBiomes = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.richTextBox11 = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.richTextBox10 = new System.Windows.Forms.RichTextBox();
            this.richTextBox9 = new System.Windows.Forms.RichTextBox();
            this.richTextBox8 = new System.Windows.Forms.RichTextBox();
            this.richTextBox7 = new System.Windows.Forms.RichTextBox();
            this.richTextBox6 = new System.Windows.Forms.RichTextBox();
            this.richTextBox5 = new System.Windows.Forms.RichTextBox();
            this.richTextBox4 = new System.Windows.Forms.RichTextBox();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.btSave = new System.Windows.Forms.Button();
            this.btLoad = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbDeleteRegion = new System.Windows.Forms.CheckBox();
            this.btnConvertSchematicToBO3 = new System.Windows.Forms.Button();
            this.btCopyBO3s = new System.Windows.Forms.Button();
            this.btImportWorld = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btSelectSourceWorld
            // 
            this.btSelectSourceWorld.Location = new System.Drawing.Point(412, 22);
            this.btSelectSourceWorld.Name = "btSelectSourceWorld";
            this.btSelectSourceWorld.Size = new System.Drawing.Size(95, 23);
            this.btSelectSourceWorld.TabIndex = 8;
            this.btSelectSourceWorld.Text = "Select";
            this.btSelectSourceWorld.UseVisualStyleBackColor = true;
            this.btSelectSourceWorld.Click += new System.EventHandler(this.SelectSourceWorld_Click);
            // 
            // btGenerate
            // 
            this.btGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btGenerate.Location = new System.Drawing.Point(11, 685);
            this.btGenerate.Name = "btGenerate";
            this.btGenerate.Size = new System.Drawing.Size(215, 25);
            this.btGenerate.TabIndex = 13;
            this.btGenerate.Text = "Generate";
            this.btGenerate.UseVisualStyleBackColor = true;
            this.btGenerate.Visible = false;
            this.btGenerate.Click += new System.EventHandler(this.btGenerate_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Wheat;
            this.groupBox2.Controls.Add(this.cbWorld);
            this.groupBox2.Controls.Add(this.cbVersion);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.btSelectSourceWorld);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(524, 61);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General settings";
            // 
            // cbWorld
            // 
            this.cbWorld.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorld.FormattingEnabled = true;
            this.cbWorld.Location = new System.Drawing.Point(271, 23);
            this.cbWorld.Name = "cbWorld";
            this.cbWorld.Size = new System.Drawing.Size(135, 21);
            this.cbWorld.TabIndex = 33;
            // 
            // cbVersion
            // 
            this.cbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersion.FormattingEnabled = true;
            this.cbVersion.Location = new System.Drawing.Point(130, 23);
            this.cbVersion.Name = "cbVersion";
            this.cbVersion.Size = new System.Drawing.Size(135, 21);
            this.cbVersion.TabIndex = 31;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 13);
            this.label8.TabIndex = 30;
            this.label8.Text = "TerrainControl version";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 110);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1139, 565);
            this.tabControl1.TabIndex = 25;
            this.tabControl1.TabStop = false;
            this.tabControl1.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Wheat;
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1131, 539);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "World Settings";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btSetToDefault);
            this.groupBox1.Controls.Add(this.tlpWorldSettings);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1119, 527);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "World Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(344, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(488, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Tick a checkbox to apply the value. Only applied values are used by \"Generate\" an" +
    "d \"Save settings\".";
            // 
            // btSetToDefault
            // 
            this.btSetToDefault.Location = new System.Drawing.Point(9, 18);
            this.btSetToDefault.Name = "btSetToDefault";
            this.btSetToDefault.Size = new System.Drawing.Size(215, 25);
            this.btSetToDefault.TabIndex = 26;
            this.btSetToDefault.Text = "Clear all";
            this.btSetToDefault.UseVisualStyleBackColor = true;
            this.btSetToDefault.Click += new System.EventHandler(this.btWorldSettingsSetToDefault_Click);
            // 
            // tlpWorldSettings
            // 
            this.tlpWorldSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpWorldSettings.AutoScroll = true;
            this.tlpWorldSettings.ColumnCount = 8;
            this.tlpWorldSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpWorldSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tlpWorldSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpWorldSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tlpWorldSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpWorldSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tlpWorldSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpWorldSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tlpWorldSettings.Location = new System.Drawing.Point(10, 50);
            this.tlpWorldSettings.Name = "tlpWorldSettings";
            this.tlpWorldSettings.RowCount = 1;
            this.tlpWorldSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpWorldSettings.Size = new System.Drawing.Size(1103, 471);
            this.tlpWorldSettings.TabIndex = 19;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Wheat;
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1131, 539);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Biome Settings";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.btCloneGroup);
            this.groupBox4.Controls.Add(this.btGroupMoveDown);
            this.groupBox4.Controls.Add(this.btGroupMoveUp);
            this.groupBox4.Controls.Add(this.btEditGroup);
            this.groupBox4.Controls.Add(this.btRemoveFromGroup);
            this.groupBox4.Controls.Add(this.btAddToGroup);
            this.groupBox4.Controls.Add(this.btDeleteGroup);
            this.groupBox4.Controls.Add(this.btNewGroup);
            this.groupBox4.Controls.Add(this.lblGroups);
            this.groupBox4.Controls.Add(this.lbGroups);
            this.groupBox4.Controls.Add(this.lblBiomesInGroup);
            this.groupBox4.Controls.Add(this.lblAvailableBioms);
            this.groupBox4.Controls.Add(this.lbGroup);
            this.groupBox4.Controls.Add(this.btBiomeSettingsResetToDefaults);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.tlpBiomeSettings);
            this.groupBox4.Controls.Add(this.lbBiomes);
            this.groupBox4.Location = new System.Drawing.Point(7, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1117, 526);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Biome Settings";
            // 
            // btCloneGroup
            // 
            this.btCloneGroup.Location = new System.Drawing.Point(155, 163);
            this.btCloneGroup.Name = "btCloneGroup";
            this.btCloneGroup.Size = new System.Drawing.Size(50, 23);
            this.btCloneGroup.TabIndex = 35;
            this.btCloneGroup.TabStop = false;
            this.btCloneGroup.Text = "Clone";
            this.btCloneGroup.UseVisualStyleBackColor = true;
            this.btCloneGroup.Click += new System.EventHandler(this.btCloneGroup_Click);
            // 
            // btGroupMoveDown
            // 
            this.btGroupMoveDown.Location = new System.Drawing.Point(111, 134);
            this.btGroupMoveDown.Name = "btGroupMoveDown";
            this.btGroupMoveDown.Size = new System.Drawing.Size(94, 23);
            this.btGroupMoveDown.TabIndex = 34;
            this.btGroupMoveDown.TabStop = false;
            this.btGroupMoveDown.Text = "Move down";
            this.btGroupMoveDown.UseVisualStyleBackColor = true;
            this.btGroupMoveDown.Click += new System.EventHandler(this.btGroupMoveDown_Click);
            // 
            // btGroupMoveUp
            // 
            this.btGroupMoveUp.Location = new System.Drawing.Point(8, 134);
            this.btGroupMoveUp.Name = "btGroupMoveUp";
            this.btGroupMoveUp.Size = new System.Drawing.Size(94, 23);
            this.btGroupMoveUp.TabIndex = 33;
            this.btGroupMoveUp.TabStop = false;
            this.btGroupMoveUp.Text = "Move up";
            this.btGroupMoveUp.UseVisualStyleBackColor = true;
            this.btGroupMoveUp.Click += new System.EventHandler(this.btGroupMoveUp_Click);
            // 
            // btEditGroup
            // 
            this.btEditGroup.Location = new System.Drawing.Point(53, 163);
            this.btEditGroup.Name = "btEditGroup";
            this.btEditGroup.Size = new System.Drawing.Size(40, 23);
            this.btEditGroup.TabIndex = 32;
            this.btEditGroup.TabStop = false;
            this.btEditGroup.Text = "Edit";
            this.btEditGroup.UseVisualStyleBackColor = true;
            this.btEditGroup.Click += new System.EventHandler(this.btEditGroup_Click);
            // 
            // btRemoveFromGroup
            // 
            this.btRemoveFromGroup.Location = new System.Drawing.Point(111, 310);
            this.btRemoveFromGroup.Name = "btRemoveFromGroup";
            this.btRemoveFromGroup.Size = new System.Drawing.Size(95, 23);
            this.btRemoveFromGroup.TabIndex = 31;
            this.btRemoveFromGroup.TabStop = false;
            this.btRemoveFromGroup.Text = "Remove";
            this.btRemoveFromGroup.UseVisualStyleBackColor = true;
            this.btRemoveFromGroup.Click += new System.EventHandler(this.btRemoveFromGroup_Click);
            // 
            // btAddToGroup
            // 
            this.btAddToGroup.Location = new System.Drawing.Point(9, 310);
            this.btAddToGroup.Name = "btAddToGroup";
            this.btAddToGroup.Size = new System.Drawing.Size(95, 23);
            this.btAddToGroup.TabIndex = 30;
            this.btAddToGroup.TabStop = false;
            this.btAddToGroup.Text = "Add";
            this.btAddToGroup.UseVisualStyleBackColor = true;
            this.btAddToGroup.Click += new System.EventHandler(this.btAddToGroup_Click);
            // 
            // btDeleteGroup
            // 
            this.btDeleteGroup.Location = new System.Drawing.Point(99, 163);
            this.btDeleteGroup.Name = "btDeleteGroup";
            this.btDeleteGroup.Size = new System.Drawing.Size(50, 23);
            this.btDeleteGroup.TabIndex = 29;
            this.btDeleteGroup.TabStop = false;
            this.btDeleteGroup.Text = "Delete";
            this.btDeleteGroup.UseVisualStyleBackColor = true;
            this.btDeleteGroup.Click += new System.EventHandler(this.btDeleteGroup_Click);
            // 
            // btNewGroup
            // 
            this.btNewGroup.Location = new System.Drawing.Point(8, 163);
            this.btNewGroup.Name = "btNewGroup";
            this.btNewGroup.Size = new System.Drawing.Size(39, 23);
            this.btNewGroup.TabIndex = 28;
            this.btNewGroup.TabStop = false;
            this.btNewGroup.Text = "New";
            this.btNewGroup.UseVisualStyleBackColor = true;
            this.btNewGroup.Click += new System.EventHandler(this.btNewGroup_Click);
            // 
            // lblGroups
            // 
            this.lblGroups.AutoSize = true;
            this.lblGroups.Location = new System.Drawing.Point(6, 17);
            this.lblGroups.Name = "lblGroups";
            this.lblGroups.Size = new System.Drawing.Size(41, 13);
            this.lblGroups.TabIndex = 27;
            this.lblGroups.Text = "Groups";
            // 
            // lbGroups
            // 
            this.lbGroups.FormattingEnabled = true;
            this.lbGroups.Location = new System.Drawing.Point(9, 33);
            this.lbGroups.Name = "lbGroups";
            this.lbGroups.Size = new System.Drawing.Size(195, 95);
            this.lbGroups.TabIndex = 26;
            this.lbGroups.TabStop = false;
            this.lbGroups.SelectedIndexChanged += new System.EventHandler(this.lbGroups_SelectedIndexChanged);
            // 
            // lblBiomesInGroup
            // 
            this.lblBiomesInGroup.AutoSize = true;
            this.lblBiomesInGroup.Location = new System.Drawing.Point(5, 193);
            this.lblBiomesInGroup.Name = "lblBiomesInGroup";
            this.lblBiomesInGroup.Size = new System.Drawing.Size(82, 13);
            this.lblBiomesInGroup.TabIndex = 25;
            this.lblBiomesInGroup.Text = "Biomes in group";
            // 
            // lblAvailableBioms
            // 
            this.lblAvailableBioms.AutoSize = true;
            this.lblAvailableBioms.Location = new System.Drawing.Point(6, 338);
            this.lblAvailableBioms.Name = "lblAvailableBioms";
            this.lblAvailableBioms.Size = new System.Drawing.Size(88, 13);
            this.lblAvailableBioms.TabIndex = 24;
            this.lblAvailableBioms.Text = "Avalilable biomes";
            // 
            // lbGroup
            // 
            this.lbGroup.FormattingEnabled = true;
            this.lbGroup.Location = new System.Drawing.Point(9, 209);
            this.lbGroup.Name = "lbGroup";
            this.lbGroup.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbGroup.Size = new System.Drawing.Size(195, 95);
            this.lbGroup.Sorted = true;
            this.lbGroup.TabIndex = 23;
            this.lbGroup.TabStop = false;
            this.lbGroup.SelectedIndexChanged += new System.EventHandler(this.lbGroup_SelectedIndexChanged);
            // 
            // btBiomeSettingsResetToDefaults
            // 
            this.btBiomeSettingsResetToDefaults.Location = new System.Drawing.Point(217, 33);
            this.btBiomeSettingsResetToDefaults.Name = "btBiomeSettingsResetToDefaults";
            this.btBiomeSettingsResetToDefaults.Size = new System.Drawing.Size(126, 23);
            this.btBiomeSettingsResetToDefaults.TabIndex = 22;
            this.btBiomeSettingsResetToDefaults.Text = "Set to defaults";
            this.btBiomeSettingsResetToDefaults.UseVisualStyleBackColor = true;
            this.btBiomeSettingsResetToDefaults.Click += new System.EventHandler(this.btBiomeSettingsResetToDefaults_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(370, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(488, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Tick a checkbox to apply the value. Only applied values are used by \"Generate\" an" +
    "d \"Save settings\".";
            // 
            // tlpBiomeSettings
            // 
            this.tlpBiomeSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpBiomeSettings.AutoScroll = true;
            this.tlpBiomeSettings.ColumnCount = 8;
            this.tlpBiomeSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpBiomeSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tlpBiomeSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpBiomeSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tlpBiomeSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpBiomeSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tlpBiomeSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpBiomeSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tlpBiomeSettings.Location = new System.Drawing.Point(217, 64);
            this.tlpBiomeSettings.Name = "tlpBiomeSettings";
            this.tlpBiomeSettings.RowCount = 1;
            this.tlpBiomeSettings.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBiomeSettings.Size = new System.Drawing.Size(894, 458);
            this.tlpBiomeSettings.TabIndex = 20;
            // 
            // lbBiomes
            // 
            this.lbBiomes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbBiomes.FormattingEnabled = true;
            this.lbBiomes.Location = new System.Drawing.Point(9, 354);
            this.lbBiomes.Name = "lbBiomes";
            this.lbBiomes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbBiomes.Size = new System.Drawing.Size(195, 160);
            this.lbBiomes.Sorted = true;
            this.lbBiomes.TabIndex = 0;
            this.lbBiomes.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Wheat;
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1131, 539);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Help";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.richTextBox11);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.richTextBox10);
            this.groupBox3.Controls.Add(this.richTextBox9);
            this.groupBox3.Controls.Add(this.richTextBox8);
            this.groupBox3.Controls.Add(this.richTextBox7);
            this.groupBox3.Controls.Add(this.richTextBox6);
            this.groupBox3.Controls.Add(this.richTextBox5);
            this.groupBox3.Controls.Add(this.richTextBox4);
            this.groupBox3.Controls.Add(this.richTextBox3);
            this.groupBox3.Controls.Add(this.richTextBox2);
            this.groupBox3.Controls.Add(this.richTextBox1);
            this.groupBox3.Controls.Add(this.textBox10);
            this.groupBox3.Controls.Add(this.textBox9);
            this.groupBox3.Controls.Add(this.textBox7);
            this.groupBox3.Controls.Add(this.textBox6);
            this.groupBox3.Location = new System.Drawing.Point(4, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1121, 526);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Help";
            // 
            // richTextBox11
            // 
            this.richTextBox11.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox11.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox11.Location = new System.Drawing.Point(20, 323);
            this.richTextBox11.Multiline = false;
            this.richTextBox11.Name = "richTextBox11";
            this.richTextBox11.ReadOnly = true;
            this.richTextBox11.Size = new System.Drawing.Size(560, 25);
            this.richTextBox11.TabIndex = 26;
            this.richTextBox11.TabStop = false;
            this.richTextBox11.Text = "JFromHollands\'s Terra Incognita:  http://forum.mctcp.com/t/download-first-beta-re" +
    "lease-terra-incognita/839/23";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Wheat;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(20, 270);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(701, 13);
            this.textBox1.TabIndex = 25;
            this.textBox1.TabStop = false;
            this.textBox1.Text = "MCW 1.0.6 has been tested with:";
            // 
            // richTextBox10
            // 
            this.richTextBox10.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox10.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox10.Location = new System.Drawing.Point(20, 299);
            this.richTextBox10.Multiline = false;
            this.richTextBox10.Name = "richTextBox10";
            this.richTextBox10.ReadOnly = true;
            this.richTextBox10.Size = new System.Drawing.Size(560, 25);
            this.richTextBox10.TabIndex = 24;
            this.richTextBox10.TabStop = false;
            this.richTextBox10.Text = "MCPitman\'s Biome Bundle:  https://sites.google.com/site/biomebundle/";
            // 
            // richTextBox9
            // 
            this.richTextBox9.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox9.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox9.Location = new System.Drawing.Point(20, 223);
            this.richTextBox9.Multiline = false;
            this.richTextBox9.Name = "richTextBox9";
            this.richTextBox9.ReadOnly = true;
            this.richTextBox9.Size = new System.Drawing.Size(793, 23);
            this.richTextBox9.TabIndex = 23;
            this.richTextBox9.TabStop = false;
            this.richTextBox9.Text = "MCW/TCEE video tutorials: https://www.youtube.com/user/PeeGee85";
            // 
            // richTextBox8
            // 
            this.richTextBox8.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox8.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox8.Location = new System.Drawing.Point(20, 198);
            this.richTextBox8.Multiline = false;
            this.richTextBox8.Name = "richTextBox8";
            this.richTextBox8.ReadOnly = true;
            this.richTextBox8.Size = new System.Drawing.Size(793, 23);
            this.richTextBox8.TabIndex = 22;
            this.richTextBox8.TabStop = false;
            this.richTextBox8.Text = "MCW forum thread:  http://forum.mctcp.com/t/minecraft-worlds-mod-beta-download/66" +
    "0/9";
            // 
            // richTextBox7
            // 
            this.richTextBox7.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox7.Location = new System.Drawing.Point(20, 174);
            this.richTextBox7.Multiline = false;
            this.richTextBox7.Name = "richTextBox7";
            this.richTextBox7.ReadOnly = true;
            this.richTextBox7.Size = new System.Drawing.Size(793, 23);
            this.richTextBox7.TabIndex = 21;
            this.richTextBox7.TabStop = false;
            this.richTextBox7.Text = "MCW forum thread:  http://www.minecraftforum.net/forums/mapping-and-modding/minec" +
    "raft-tools/2592678-minecraft-worlds-ultimate-world-generator-tons-of";
            // 
            // richTextBox6
            // 
            this.richTextBox6.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox6.Location = new System.Drawing.Point(20, 444);
            this.richTextBox6.Multiline = false;
            this.richTextBox6.Name = "richTextBox6";
            this.richTextBox6.ReadOnly = true;
            this.richTextBox6.Size = new System.Drawing.Size(701, 23);
            this.richTextBox6.TabIndex = 20;
            this.richTextBox6.TabStop = false;
            this.richTextBox6.Text = "Most BO3s in the demonstration worlds were made using schematics from:  http://ww" +
    "w.minecraft-schematics.com/";
            // 
            // richTextBox5
            // 
            this.richTextBox5.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox5.Location = new System.Drawing.Point(20, 150);
            this.richTextBox5.Multiline = false;
            this.richTextBox5.Name = "richTextBox5";
            this.richTextBox5.ReadOnly = true;
            this.richTextBox5.Size = new System.Drawing.Size(701, 23);
            this.richTextBox5.TabIndex = 19;
            this.richTextBox5.TabStop = false;
            this.richTextBox5.Text = "TCEE download & bugs thread: http://forum.mctcp.com/t/terraincontrol-editor-tcee-" +
    "beta-download-bugs-thread/561";
            // 
            // richTextBox4
            // 
            this.richTextBox4.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox4.Location = new System.Drawing.Point(20, 126);
            this.richTextBox4.Multiline = false;
            this.richTextBox4.Name = "richTextBox4";
            this.richTextBox4.ReadOnly = true;
            this.richTextBox4.Size = new System.Drawing.Size(701, 23);
            this.richTextBox4.TabIndex = 18;
            this.richTextBox4.TabStop = false;
            this.richTextBox4.Text = "How to make TCEE work for any version of TC:  http://forum.mctcp.com/t/terraincon" +
    "trol-editor-tcee-how-to-make-it-work-for-any-version-of-tc/563/3";
            // 
            // richTextBox3
            // 
            this.richTextBox3.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox3.Location = new System.Drawing.Point(20, 103);
            this.richTextBox3.Multiline = false;
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.ReadOnly = true;
            this.richTextBox3.Size = new System.Drawing.Size(701, 23);
            this.richTextBox3.TabIndex = 17;
            this.richTextBox3.TabStop = false;
            this.richTextBox3.Text = "TerrainControl forums: http://forum.mctcp.com/";
            // 
            // richTextBox2
            // 
            this.richTextBox2.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox2.Location = new System.Drawing.Point(20, 80);
            this.richTextBox2.Multiline = false;
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new System.Drawing.Size(701, 23);
            this.richTextBox2.TabIndex = 16;
            this.richTextBox2.TabStop = false;
            this.richTextBox2.Text = "TerrainControl Wiki: https://github.com/MCTCP/TerrainControl/wiki";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.Wheat;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(20, 57);
            this.richTextBox1.Multiline = false;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(701, 23);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = "TCEE tutorial: http://forum.mctcp.com/t/terraincontrol-editor-tcee-tutorials-thre" +
    "ad/562";
            // 
            // textBox10
            // 
            this.textBox10.BackColor = System.Drawing.Color.Wheat;
            this.textBox10.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox10.Location = new System.Drawing.Point(20, 27);
            this.textBox10.Name = "textBox10";
            this.textBox10.ReadOnly = true;
            this.textBox10.Size = new System.Drawing.Size(701, 13);
            this.textBox10.TabIndex = 13;
            this.textBox10.TabStop = false;
            this.textBox10.Text = "Resources:";
            // 
            // textBox9
            // 
            this.textBox9.BackColor = System.Drawing.Color.Wheat;
            this.textBox9.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox9.Location = new System.Drawing.Point(20, 369);
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.Size = new System.Drawing.Size(701, 13);
            this.textBox9.TabIndex = 12;
            this.textBox9.TabStop = false;
            this.textBox9.Text = "Credits:";
            // 
            // textBox7
            // 
            this.textBox7.BackColor = System.Drawing.Color.Wheat;
            this.textBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox7.Location = new System.Drawing.Point(20, 421);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(701, 13);
            this.textBox7.TabIndex = 10;
            this.textBox7.TabStop = false;
            this.textBox7.Text = "TCEE and MCW by:  PeeGee85";
            // 
            // textBox6
            // 
            this.textBox6.BackColor = System.Drawing.Color.Wheat;
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox6.Location = new System.Drawing.Point(20, 398);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(701, 13);
            this.textBox6.TabIndex = 9;
            this.textBox6.TabStop = false;
            this.textBox6.Text = "TerrainControl by:  Khoorn/Wickth, RutgerKok, TimeThor";
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(11, 79);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(105, 25);
            this.btSave.TabIndex = 26;
            this.btSave.Text = "Save settings";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btLoad
            // 
            this.btLoad.Location = new System.Drawing.Point(122, 79);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(109, 25);
            this.btLoad.TabIndex = 28;
            this.btLoad.Text = "Load settings";
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Location = new System.Drawing.Point(544, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(626, 123);
            this.pictureBox1.TabIndex = 29;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1083, 694);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "v1.0.8 2016";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.DimGray;
            this.label4.Location = new System.Drawing.Point(905, 694);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(147, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Athmospheric enhancements:";
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.Maroon;
            this.label5.Location = new System.Drawing.Point(1052, 694);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "off";
            this.label5.Visible = false;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // cbDeleteRegion
            // 
            this.cbDeleteRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbDeleteRegion.AutoSize = true;
            this.cbDeleteRegion.Checked = true;
            this.cbDeleteRegion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDeleteRegion.Location = new System.Drawing.Point(237, 690);
            this.cbDeleteRegion.Name = "cbDeleteRegion";
            this.cbDeleteRegion.Size = new System.Drawing.Size(137, 17);
            this.cbDeleteRegion.TabIndex = 33;
            this.cbDeleteRegion.Text = "Delete /region directory";
            this.cbDeleteRegion.UseVisualStyleBackColor = true;
            this.cbDeleteRegion.Visible = false;
            // 
            // btnConvertSchematicToBO3
            // 
            this.btnConvertSchematicToBO3.Location = new System.Drawing.Point(367, 79);
            this.btnConvertSchematicToBO3.Name = "btnConvertSchematicToBO3";
            this.btnConvertSchematicToBO3.Size = new System.Drawing.Size(169, 25);
            this.btnConvertSchematicToBO3.TabIndex = 35;
            this.btnConvertSchematicToBO3.Text = "Convert .schematic to BO3";
            this.btnConvertSchematicToBO3.UseVisualStyleBackColor = true;
            this.btnConvertSchematicToBO3.Click += new System.EventHandler(this.btnConvertSchematicToBO3_Click);
            // 
            // btCopyBO3s
            // 
            this.btCopyBO3s.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btCopyBO3s.Location = new System.Drawing.Point(385, 685);
            this.btCopyBO3s.Name = "btCopyBO3s";
            this.btCopyBO3s.Size = new System.Drawing.Size(215, 25);
            this.btCopyBO3s.TabIndex = 36;
            this.btCopyBO3s.Text = "Copy structure files (BO3s)";
            this.btCopyBO3s.UseVisualStyleBackColor = true;
            this.btCopyBO3s.Visible = false;
            this.btCopyBO3s.Click += new System.EventHandler(this.btCopyBO3s_Click);
            // 
            // btImportWorld
            // 
            this.btImportWorld.Location = new System.Drawing.Point(237, 79);
            this.btImportWorld.Name = "btImportWorld";
            this.btImportWorld.Size = new System.Drawing.Size(126, 25);
            this.btImportWorld.TabIndex = 37;
            this.btImportWorld.Text = "Import world";
            this.btImportWorld.UseVisualStyleBackColor = true;
            this.btImportWorld.Click += new System.EventHandler(this.btImportWorld_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1163, 718);
            this.Controls.Add(this.btImportWorld);
            this.Controls.Add(this.btCopyBO3s);
            this.Controls.Add(this.btnConvertSchematicToBO3);
            this.Controls.Add(this.cbDeleteRegion);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btLoad);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.btGenerate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "TCEE : TerrainControl\'s Excellent Editor (TM) by PeeGee85";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btSelectSourceWorld;
        private System.Windows.Forms.Button btGenerate;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tlpWorldSettings;
        private System.Windows.Forms.Button btSetToDefault;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox lbBiomes;
        private System.Windows.Forms.TableLayoutPanel tlpBiomeSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btBiomeSettingsResetToDefaults;
        private System.Windows.Forms.Label label2;
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
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbVersion;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbDeleteRegion;
        private System.Windows.Forms.ComboBox cbWorld;
        private System.Windows.Forms.Button btnConvertSchematicToBO3;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.RichTextBox richTextBox6;
        private System.Windows.Forms.RichTextBox richTextBox5;
        private System.Windows.Forms.RichTextBox richTextBox4;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox8;
        private System.Windows.Forms.RichTextBox richTextBox7;
        private System.Windows.Forms.RichTextBox richTextBox9;
        private System.Windows.Forms.Button btCopyBO3s;
        private System.Windows.Forms.Button btImportWorld;
        private System.Windows.Forms.RichTextBox richTextBox11;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox richTextBox10;
        private System.Windows.Forms.Button btCloneGroup;

    }
}

