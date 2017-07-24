namespace OTGE
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btSearchWorldConfigNext = new System.Windows.Forms.Button();
            this.btSearchWorldConfigPrev = new System.Windows.Forms.Button();
            this.tbSearchWorldConfig = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tlpWorldSettingsContainer = new System.Windows.Forms.TableLayoutPanel();
            this.tlpWorldSettings1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.btSetToDefault = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btSearchBiomeConfigNext = new System.Windows.Forms.Button();
            this.btSearchBiomeConfigPrev = new System.Windows.Forms.Button();
            this.tbSearchBiomeConfig = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tlpBiomeSettingsContainer = new System.Windows.Forms.TableLayoutPanel();
            this.tlpBiomeSettings1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.richTextBox11 = new System.Windows.Forms.RichTextBox();
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
            this.btSave = new System.Windows.Forms.Button();
            this.btLoad = new System.Windows.Forms.Button();
            this.cbDeleteRegion = new System.Windows.Forms.CheckBox();
            this.btCopyBO3s = new System.Windows.Forms.Button();
            this.btImportWorld = new System.Windows.Forms.Button();
            this.rbSummerSkin = new System.Windows.Forms.RadioButton();
            this.rbWinterSkin = new System.Windows.Forms.RadioButton();
            this.btSelectSourceWorld = new System.Windows.Forms.Button();
            this.cbVersion = new System.Windows.Forms.ComboBox();
            this.cbWorld = new System.Windows.Forms.ComboBox();
            this.pnlVersionWorldSelect = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbNoSkin = new System.Windows.Forms.RadioButton();
            this.btnConvertSchematicToBO3 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tlpWorldSettingsContainer.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tlpBiomeSettingsContainer.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.pnlVersionWorldSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // btGenerate
            // 
            this.btGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btGenerate.Location = new System.Drawing.Point(11, 685);
            this.btGenerate.Name = "btGenerate";
            this.btGenerate.Size = new System.Drawing.Size(105, 25);
            this.btGenerate.TabIndex = 13;
            this.btGenerate.TabStop = false;
            this.btGenerate.Text = "Generate";
            this.btGenerate.UseVisualStyleBackColor = true;
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
            this.tabControl1.Location = new System.Drawing.Point(12, 110);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1015, 565);
            this.tabControl1.TabIndex = 25;
            this.tabControl1.TabStop = false;
            this.tabControl1.Visible = false;
            this.tabControl1.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.GhostWhite;
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1007, 539);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "World Settings";
            this.tabPage1.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.GhostWhite;
            this.groupBox1.Controls.Add(this.btSearchWorldConfigNext);
            this.groupBox1.Controls.Add(this.btSearchWorldConfigPrev);
            this.groupBox1.Controls.Add(this.tbSearchWorldConfig);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btSetToDefault);
            this.groupBox1.Location = new System.Drawing.Point(7, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(995, 527);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // btSearchWorldConfigNext
            // 
            this.btSearchWorldConfigNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSearchWorldConfigNext.Location = new System.Drawing.Point(963, 13);
            this.btSearchWorldConfigNext.Name = "btSearchWorldConfigNext";
            this.btSearchWorldConfigNext.Size = new System.Drawing.Size(26, 25);
            this.btSearchWorldConfigNext.TabIndex = 31;
            this.btSearchWorldConfigNext.TabStop = false;
            this.btSearchWorldConfigNext.Text = ">";
            this.btSearchWorldConfigNext.UseVisualStyleBackColor = true;
            this.btSearchWorldConfigNext.Click += new System.EventHandler(this.btSearchWorldConfigNext_Click);
            // 
            // btSearchWorldConfigPrev
            // 
            this.btSearchWorldConfigPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSearchWorldConfigPrev.Location = new System.Drawing.Point(931, 13);
            this.btSearchWorldConfigPrev.Name = "btSearchWorldConfigPrev";
            this.btSearchWorldConfigPrev.Size = new System.Drawing.Size(26, 25);
            this.btSearchWorldConfigPrev.TabIndex = 30;
            this.btSearchWorldConfigPrev.TabStop = false;
            this.btSearchWorldConfigPrev.Text = "<";
            this.btSearchWorldConfigPrev.UseVisualStyleBackColor = true;
            this.btSearchWorldConfigPrev.Click += new System.EventHandler(this.btSearchWorldConfigPrev_Click);
            // 
            // tbSearchWorldConfig
            // 
            this.tbSearchWorldConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchWorldConfig.Location = new System.Drawing.Point(771, 15);
            this.tbSearchWorldConfig.Name = "tbSearchWorldConfig";
            this.tbSearchWorldConfig.Size = new System.Drawing.Size(154, 20);
            this.tbSearchWorldConfig.TabIndex = 29;
            this.tbSearchWorldConfig.TextChanged += new System.EventHandler(this.tbSearchWorldConfig_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoScroll = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.tlpWorldSettingsContainer);
            this.panel2.Location = new System.Drawing.Point(6, 45);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(983, 476);
            this.panel2.TabIndex = 28;
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
            this.tlpWorldSettingsContainer.Size = new System.Drawing.Size(982, 6);
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
            this.tlpWorldSettings1.Size = new System.Drawing.Size(976, 0);
            this.tlpWorldSettings1.TabIndex = 19;
            this.tlpWorldSettings1.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(100, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(440, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Tick a checkbox to override the default value. Only checked values are saved / ge" +
    "nerated.";
            // 
            // btSetToDefault
            // 
            this.btSetToDefault.Location = new System.Drawing.Point(9, 13);
            this.btSetToDefault.Name = "btSetToDefault";
            this.btSetToDefault.Size = new System.Drawing.Size(85, 25);
            this.btSetToDefault.TabIndex = 26;
            this.btSetToDefault.TabStop = false;
            this.btSetToDefault.Text = "Clear all";
            this.btSetToDefault.UseVisualStyleBackColor = true;
            this.btSetToDefault.Click += new System.EventHandler(this.btWorldSettingsSetToDefault_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.GhostWhite;
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1007, 539);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Biome Settings";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.BackColor = System.Drawing.Color.GhostWhite;
            this.groupBox4.Controls.Add(this.btSearchBiomeConfigNext);
            this.groupBox4.Controls.Add(this.btSearchBiomeConfigPrev);
            this.groupBox4.Controls.Add(this.tbSearchBiomeConfig);
            this.groupBox4.Controls.Add(this.panel3);
            this.groupBox4.Controls.Add(this.label3);
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
            this.groupBox4.Controls.Add(this.lbBiomes);
            this.groupBox4.Location = new System.Drawing.Point(7, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(995, 527);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // btSearchBiomeConfigNext
            // 
            this.btSearchBiomeConfigNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSearchBiomeConfigNext.Location = new System.Drawing.Point(963, 13);
            this.btSearchBiomeConfigNext.Name = "btSearchBiomeConfigNext";
            this.btSearchBiomeConfigNext.Size = new System.Drawing.Size(26, 25);
            this.btSearchBiomeConfigNext.TabIndex = 40;
            this.btSearchBiomeConfigNext.TabStop = false;
            this.btSearchBiomeConfigNext.Text = ">";
            this.btSearchBiomeConfigNext.UseVisualStyleBackColor = true;
            this.btSearchBiomeConfigNext.Click += new System.EventHandler(this.btSearchBiomeConfigNext_Click);
            // 
            // btSearchBiomeConfigPrev
            // 
            this.btSearchBiomeConfigPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSearchBiomeConfigPrev.Location = new System.Drawing.Point(931, 13);
            this.btSearchBiomeConfigPrev.Name = "btSearchBiomeConfigPrev";
            this.btSearchBiomeConfigPrev.Size = new System.Drawing.Size(26, 25);
            this.btSearchBiomeConfigPrev.TabIndex = 39;
            this.btSearchBiomeConfigPrev.TabStop = false;
            this.btSearchBiomeConfigPrev.Text = "<";
            this.btSearchBiomeConfigPrev.UseVisualStyleBackColor = true;
            this.btSearchBiomeConfigPrev.Click += new System.EventHandler(this.btSearchBiomeConfigPrev_Click);
            // 
            // tbSearchBiomeConfig
            // 
            this.tbSearchBiomeConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchBiomeConfig.Location = new System.Drawing.Point(771, 15);
            this.tbSearchBiomeConfig.Name = "tbSearchBiomeConfig";
            this.tbSearchBiomeConfig.Size = new System.Drawing.Size(154, 20);
            this.tbSearchBiomeConfig.TabIndex = 38;
            this.tbSearchBiomeConfig.TextChanged += new System.EventHandler(this.tbSearchBiomeConfig_TextChanged);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.AutoScroll = true;
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.Controls.Add(this.tlpBiomeSettingsContainer);
            this.panel3.Location = new System.Drawing.Point(220, 43);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(768, 473);
            this.panel3.TabIndex = 37;
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
            this.tlpBiomeSettingsContainer.Size = new System.Drawing.Size(760, 6);
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
            this.tlpBiomeSettings1.Size = new System.Drawing.Size(754, 0);
            this.tlpBiomeSettings1.TabIndex = 19;
            this.tlpBiomeSettings1.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(217, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(440, 13);
            this.label3.TabIndex = 36;
            this.label3.Text = "Tick a checkbox to override the default value. Only checked values are saved / ge" +
    "nerated.";
            // 
            // btCloneGroup
            // 
            this.btCloneGroup.Location = new System.Drawing.Point(155, 165);
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
            this.btGroupMoveDown.Location = new System.Drawing.Point(111, 136);
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
            this.btGroupMoveUp.Location = new System.Drawing.Point(8, 136);
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
            this.btEditGroup.Location = new System.Drawing.Point(53, 165);
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
            this.btRemoveFromGroup.Location = new System.Drawing.Point(111, 312);
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
            this.btAddToGroup.Location = new System.Drawing.Point(9, 312);
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
            this.btDeleteGroup.Location = new System.Drawing.Point(99, 165);
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
            this.btNewGroup.Location = new System.Drawing.Point(8, 165);
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
            this.lblGroups.Location = new System.Drawing.Point(6, 19);
            this.lblGroups.Name = "lblGroups";
            this.lblGroups.Size = new System.Drawing.Size(41, 13);
            this.lblGroups.TabIndex = 27;
            this.lblGroups.Text = "Groups";
            // 
            // lbGroups
            // 
            this.lbGroups.FormattingEnabled = true;
            this.lbGroups.Location = new System.Drawing.Point(9, 35);
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
            this.lblAvailableBioms.Location = new System.Drawing.Point(6, 340);
            this.lblAvailableBioms.Name = "lblAvailableBioms";
            this.lblAvailableBioms.Size = new System.Drawing.Size(88, 13);
            this.lblAvailableBioms.TabIndex = 24;
            this.lblAvailableBioms.Text = "Avalilable biomes";
            // 
            // lbGroup
            // 
            this.lbGroup.FormattingEnabled = true;
            this.lbGroup.Location = new System.Drawing.Point(9, 211);
            this.lbGroup.Name = "lbGroup";
            this.lbGroup.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbGroup.Size = new System.Drawing.Size(195, 95);
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
            this.lbBiomes.Location = new System.Drawing.Point(9, 356);
            this.lbBiomes.Name = "lbBiomes";
            this.lbBiomes.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbBiomes.Size = new System.Drawing.Size(195, 160);
            this.lbBiomes.Sorted = true;
            this.lbBiomes.TabIndex = 0;
            this.lbBiomes.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.GhostWhite;
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1007, 539);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Help";
            this.tabPage2.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.BackColor = System.Drawing.Color.GhostWhite;
            this.groupBox3.Controls.Add(this.richTextBox11);
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
            this.groupBox3.Location = new System.Drawing.Point(4, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(997, 526);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Help";
            this.groupBox3.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // richTextBox11
            // 
            this.richTextBox11.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox11.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox11.Location = new System.Drawing.Point(20, 78);
            this.richTextBox11.Multiline = false;
            this.richTextBox11.Name = "richTextBox11";
            this.richTextBox11.ReadOnly = true;
            this.richTextBox11.Size = new System.Drawing.Size(701, 23);
            this.richTextBox11.TabIndex = 25;
            this.richTextBox11.TabStop = false;
            this.richTextBox11.Text = "OTG reddit: https://www.reddit.com/r/openterraingen/";
            // 
            // richTextBox10
            // 
            this.richTextBox10.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox10.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox10.Location = new System.Drawing.Point(20, 55);
            this.richTextBox10.Multiline = false;
            this.richTextBox10.Name = "richTextBox10";
            this.richTextBox10.ReadOnly = true;
            this.richTextBox10.Size = new System.Drawing.Size(701, 23);
            this.richTextBox10.TabIndex = 24;
            this.richTextBox10.TabStop = false;
            this.richTextBox10.Text = "OTG wiki: http://openterraingen.wikia.com/wiki/Open_Terrain_Generator_Wiki";
            // 
            // richTextBox9
            // 
            this.richTextBox9.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox9.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox9.Location = new System.Drawing.Point(20, 262);
            this.richTextBox9.Multiline = false;
            this.richTextBox9.Name = "richTextBox9";
            this.richTextBox9.ReadOnly = true;
            this.richTextBox9.Size = new System.Drawing.Size(793, 23);
            this.richTextBox9.TabIndex = 23;
            this.richTextBox9.TabStop = false;
            this.richTextBox9.Text = "MCW/OTGE video tutorials: https://www.youtube.com/user/PeeGee85";
            // 
            // richTextBox8
            // 
            this.richTextBox8.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox8.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox8.Location = new System.Drawing.Point(20, 239);
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
            this.richTextBox7.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox7.Location = new System.Drawing.Point(20, 216);
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
            this.richTextBox6.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox6.Location = new System.Drawing.Point(20, 358);
            this.richTextBox6.Multiline = false;
            this.richTextBox6.Name = "richTextBox6";
            this.richTextBox6.ReadOnly = true;
            this.richTextBox6.Size = new System.Drawing.Size(701, 22);
            this.richTextBox6.TabIndex = 20;
            this.richTextBox6.TabStop = false;
            this.richTextBox6.Text = "Most BO3s in the demonstration worlds were made using schematics from:  http://ww" +
    "w.minecraft-schematics.com/";
            // 
            // richTextBox5
            // 
            this.richTextBox5.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox5.Location = new System.Drawing.Point(20, 193);
            this.richTextBox5.Multiline = false;
            this.richTextBox5.Name = "richTextBox5";
            this.richTextBox5.ReadOnly = true;
            this.richTextBox5.Size = new System.Drawing.Size(793, 23);
            this.richTextBox5.TabIndex = 19;
            this.richTextBox5.TabStop = false;
            this.richTextBox5.Text = "OTGE download & bugs thread (old): http://forum.mctcp.com/t/terraincontrol-editor" +
    "-tcee-beta-download-bugs-thread/561";
            // 
            // richTextBox4
            // 
            this.richTextBox4.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox4.Location = new System.Drawing.Point(20, 170);
            this.richTextBox4.Multiline = false;
            this.richTextBox4.Name = "richTextBox4";
            this.richTextBox4.ReadOnly = true;
            this.richTextBox4.Size = new System.Drawing.Size(793, 23);
            this.richTextBox4.TabIndex = 18;
            this.richTextBox4.TabStop = false;
            this.richTextBox4.Text = "How to make OTGE work for any version of OTG/TC:  http://forum.mctcp.com/t/terrai" +
    "ncontrol-editor-tcee-how-to-make-it-work-for-any-version-of-tc/563/3";
            // 
            // richTextBox3
            // 
            this.richTextBox3.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox3.Location = new System.Drawing.Point(20, 147);
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
            this.richTextBox2.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox2.Location = new System.Drawing.Point(20, 124);
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
            this.richTextBox1.BackColor = System.Drawing.Color.GhostWhite;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(20, 101);
            this.richTextBox1.Multiline = false;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(701, 23);
            this.richTextBox1.TabIndex = 15;
            this.richTextBox1.TabStop = false;
            this.richTextBox1.Text = "OTGE tutorial: http://forum.mctcp.com/t/terraincontrol-editor-tcee-tutorials-thre" +
    "ad/562";
            // 
            // textBox10
            // 
            this.textBox10.BackColor = System.Drawing.Color.GhostWhite;
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
            this.textBox9.BackColor = System.Drawing.Color.GhostWhite;
            this.textBox9.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox9.Location = new System.Drawing.Point(20, 304);
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.Size = new System.Drawing.Size(701, 13);
            this.textBox9.TabIndex = 12;
            this.textBox9.TabStop = false;
            this.textBox9.Text = "Credits:";
            // 
            // textBox7
            // 
            this.textBox7.BackColor = System.Drawing.Color.GhostWhite;
            this.textBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox7.Location = new System.Drawing.Point(20, 335);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(701, 13);
            this.textBox7.TabIndex = 10;
            this.textBox7.TabStop = false;
            this.textBox7.Text = "OTGE:  PeeGee85";
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(11, 80);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(105, 25);
            this.btSave.TabIndex = 26;
            this.btSave.TabStop = false;
            this.btSave.Text = "Save settings";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // btLoad
            // 
            this.btLoad.Location = new System.Drawing.Point(122, 80);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(109, 25);
            this.btLoad.TabIndex = 28;
            this.btLoad.TabStop = false;
            this.btLoad.Text = "Load settings";
            this.btLoad.UseVisualStyleBackColor = true;
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
            this.btCopyBO3s.Location = new System.Drawing.Point(273, 685);
            this.btCopyBO3s.Name = "btCopyBO3s";
            this.btCopyBO3s.Size = new System.Drawing.Size(162, 25);
            this.btCopyBO3s.TabIndex = 36;
            this.btCopyBO3s.TabStop = false;
            this.btCopyBO3s.Text = "Copy structure files (BO3s)";
            this.btCopyBO3s.UseVisualStyleBackColor = true;
            this.btCopyBO3s.Visible = false;
            this.btCopyBO3s.Click += new System.EventHandler(this.btCopyBO3s_Click);
            // 
            // btImportWorld
            // 
            this.btImportWorld.Location = new System.Drawing.Point(237, 80);
            this.btImportWorld.Name = "btImportWorld";
            this.btImportWorld.Size = new System.Drawing.Size(126, 25);
            this.btImportWorld.TabIndex = 37;
            this.btImportWorld.TabStop = false;
            this.btImportWorld.Text = "Manage worlds";
            this.btImportWorld.UseVisualStyleBackColor = true;
            this.btImportWorld.Click += new System.EventHandler(this.btmanageWorlds_Click);
            // 
            // rbSummerSkin
            // 
            this.rbSummerSkin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbSummerSkin.AutoSize = true;
            this.rbSummerSkin.Location = new System.Drawing.Point(445, 689);
            this.rbSummerSkin.Name = "rbSummerSkin";
            this.rbSummerSkin.Size = new System.Drawing.Size(63, 17);
            this.rbSummerSkin.TabIndex = 38;
            this.rbSummerSkin.Text = "Summer";
            this.rbSummerSkin.UseVisualStyleBackColor = true;
            this.rbSummerSkin.Visible = false;
            this.rbSummerSkin.CheckedChanged += new System.EventHandler(this.rbSummerSkin_CheckedChanged);
            // 
            // rbWinterSkin
            // 
            this.rbWinterSkin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbWinterSkin.AutoSize = true;
            this.rbWinterSkin.Checked = true;
            this.rbWinterSkin.Location = new System.Drawing.Point(514, 690);
            this.rbWinterSkin.Name = "rbWinterSkin";
            this.rbWinterSkin.Size = new System.Drawing.Size(56, 17);
            this.rbWinterSkin.TabIndex = 39;
            this.rbWinterSkin.TabStop = true;
            this.rbWinterSkin.Text = "Winter";
            this.rbWinterSkin.UseVisualStyleBackColor = true;
            this.rbWinterSkin.Visible = false;
            this.rbWinterSkin.CheckedChanged += new System.EventHandler(this.rbWinterSkin_CheckedChanged);
            // 
            // btSelectSourceWorld
            // 
            this.btSelectSourceWorld.Location = new System.Drawing.Point(413, 28);
            this.btSelectSourceWorld.Name = "btSelectSourceWorld";
            this.btSelectSourceWorld.Size = new System.Drawing.Size(100, 23);
            this.btSelectSourceWorld.TabIndex = 8;
            this.btSelectSourceWorld.TabStop = false;
            this.btSelectSourceWorld.Text = "Select";
            this.btSelectSourceWorld.UseVisualStyleBackColor = true;
            this.btSelectSourceWorld.Click += new System.EventHandler(this.SelectSourceWorld_Click);
            // 
            // cbVersion
            // 
            this.cbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersion.FormattingEnabled = true;
            this.cbVersion.Location = new System.Drawing.Point(12, 28);
            this.cbVersion.Name = "cbVersion";
            this.cbVersion.Size = new System.Drawing.Size(191, 21);
            this.cbVersion.TabIndex = 31;
            this.cbVersion.TabStop = false;
            // 
            // cbWorld
            // 
            this.cbWorld.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWorld.FormattingEnabled = true;
            this.cbWorld.Location = new System.Drawing.Point(209, 28);
            this.cbWorld.Name = "cbWorld";
            this.cbWorld.Size = new System.Drawing.Size(198, 21);
            this.cbWorld.TabIndex = 33;
            this.cbWorld.TabStop = false;
            // 
            // pnlVersionWorldSelect
            // 
            this.pnlVersionWorldSelect.BackColor = System.Drawing.Color.GhostWhite;
            this.pnlVersionWorldSelect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlVersionWorldSelect.Controls.Add(this.label4);
            this.pnlVersionWorldSelect.Controls.Add(this.label1);
            this.pnlVersionWorldSelect.Controls.Add(this.cbWorld);
            this.pnlVersionWorldSelect.Controls.Add(this.cbVersion);
            this.pnlVersionWorldSelect.Controls.Add(this.btSelectSourceWorld);
            this.pnlVersionWorldSelect.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pnlVersionWorldSelect.Location = new System.Drawing.Point(12, 12);
            this.pnlVersionWorldSelect.Name = "pnlVersionWorldSelect";
            this.pnlVersionWorldSelect.Size = new System.Drawing.Size(524, 61);
            this.pnlVersionWorldSelect.TabIndex = 21;
            this.pnlVersionWorldSelect.Click += new System.EventHandler(this.btClickBackGround);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(207, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "World template";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "OTG / TerrainControl version";
            // 
            // rbNoSkin
            // 
            this.rbNoSkin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbNoSkin.AutoSize = true;
            this.rbNoSkin.Location = new System.Drawing.Point(576, 690);
            this.rbNoSkin.Name = "rbNoSkin";
            this.rbNoSkin.Size = new System.Drawing.Size(59, 17);
            this.rbNoSkin.TabIndex = 40;
            this.rbNoSkin.Text = "Default";
            this.rbNoSkin.UseVisualStyleBackColor = true;
            this.rbNoSkin.Visible = false;
            this.rbNoSkin.CheckedChanged += new System.EventHandler(this.rbVanillaSkin_CheckedChanged);
            // 
            // btnConvertSchematicToBO3
            // 
            this.btnConvertSchematicToBO3.Location = new System.Drawing.Point(369, 80);
            this.btnConvertSchematicToBO3.Name = "btnConvertSchematicToBO3";
            this.btnConvertSchematicToBO3.Size = new System.Drawing.Size(168, 25);
            this.btnConvertSchematicToBO3.TabIndex = 35;
            this.btnConvertSchematicToBO3.TabStop = false;
            this.btnConvertSchematicToBO3.Text = "Convert .schematic to BO3";
            this.btnConvertSchematicToBO3.UseVisualStyleBackColor = true;
            this.btnConvertSchematicToBO3.Click += new System.EventHandler(this.btnConvertSchematicToBO3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.BackgroundImage = global::OTGE.Properties.Resources.BGWinter;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1039, 718);
            this.Controls.Add(this.rbNoSkin);
            this.Controls.Add(this.rbWinterSkin);
            this.Controls.Add(this.rbSummerSkin);
            this.Controls.Add(this.btImportWorld);
            this.Controls.Add(this.btCopyBO3s);
            this.Controls.Add(this.btnConvertSchematicToBO3);
            this.Controls.Add(this.cbDeleteRegion);
            this.Controls.Add(this.btLoad);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pnlVersionWorldSelect);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.btGenerate);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "OTGE: OpenTerrainGenerator\'s Editor v1.0.11 2017";
            this.Click += new System.EventHandler(this.btClickBackGround);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tlpWorldSettingsContainer.ResumeLayout(false);
            this.tlpWorldSettingsContainer.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tlpBiomeSettingsContainer.ResumeLayout(false);
            this.tlpBiomeSettingsContainer.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.pnlVersionWorldSelect.ResumeLayout(false);
            this.pnlVersionWorldSelect.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btGenerate;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btSetToDefault;
        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox4;
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
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.RichTextBox richTextBox5;
        private System.Windows.Forms.RichTextBox richTextBox4;
        private System.Windows.Forms.RichTextBox richTextBox8;
        private System.Windows.Forms.RichTextBox richTextBox7;
        private System.Windows.Forms.RichTextBox richTextBox9;
        private System.Windows.Forms.Button btCopyBO3s;
        private System.Windows.Forms.Button btImportWorld;
        private System.Windows.Forms.Button btCloneGroup;
        private System.Windows.Forms.RadioButton rbSummerSkin;
        private System.Windows.Forms.RadioButton rbWinterSkin;
        private System.Windows.Forms.RichTextBox richTextBox11;
        private System.Windows.Forms.RichTextBox richTextBox10;
        private System.Windows.Forms.RichTextBox richTextBox6;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btSelectSourceWorld;
        private System.Windows.Forms.ComboBox cbVersion;
        private System.Windows.Forms.ComboBox cbWorld;
        private System.Windows.Forms.Panel pnlVersionWorldSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tlpWorldSettings1;
        private System.Windows.Forms.TableLayoutPanel tlpWorldSettingsContainer;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tlpBiomeSettings1;
        private System.Windows.Forms.TableLayoutPanel tlpBiomeSettingsContainer;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox tbSearchWorldConfig;
        private System.Windows.Forms.Button btSearchWorldConfigNext;
        private System.Windows.Forms.Button btSearchWorldConfigPrev;
        private System.Windows.Forms.RadioButton rbNoSkin;
        private System.Windows.Forms.Button btSearchBiomeConfigNext;
        private System.Windows.Forms.Button btSearchBiomeConfigPrev;
        private System.Windows.Forms.TextBox tbSearchBiomeConfig;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnConvertSchematicToBO3;

    }
}

