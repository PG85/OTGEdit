using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using OTGE.XML;
using System.Xml;
using System.Runtime.Serialization;

namespace OTGE.Utils
{
    public static class PopUpForm
    {
        private static string PopupFormSelectedItem;

        public static string SingleSelectListBox(List<string> listItems, string title = "", string labelText = "")
        {
            PopupFormSelectedItem = null;

            int margin = 4;

            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
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
            form.StartPosition = FormStartPosition.CenterParent;
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

        public static List<string> BiomeListSelectionBox(ref string groupName, List<string> listItems)
        {
            PopupFormSelectedItem = null;
            int margin = 4;

            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            form.Text = "Create group";

            Label label = new Label();
            label.Text = "Enter a name for the group. Only a-z A-Z 0-9 space + - and _ are allowed.";
            label.SetBounds(12, 11, 372, 13);

            TextBox textBox = new TextBox();
            textBox.Text = groupName;
            textBox.SetBounds(12, 36, 372, 20);
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            textBox.TabIndex = 0;
            textBox.SelectionStart = Math.Max(0, textBox.Text.Length);
            textBox.SelectionLength = 0;

            Label labelListBox = new Label();
            labelListBox.Text = "Use SHIFT and CTRL to select biomes to add to the new group. If a single biome is selected and the name field is left empty then that biome's name wil automatically be used as the group name.";
            labelListBox.AutoEllipsis = true;
            labelListBox.SetBounds(12, textBox.Location.Y + textBox.Height + margin, 372, 45);

            ListBox listBox = new ListBox();
            listBox.SetBounds(12, labelListBox.Location.Y + labelListBox.Height + margin, 372, 200);
            listBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            listBox.SelectionMode = SelectionMode.MultiExtended;
            listBox.KeyDown += listBox_KeyDown;
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
                    if (listBox.SelectedIndices.Count < 1)
                    {
                        form.DialogResult = DialogResult.None;
                        PopUpForm.CustomMessageBox("Select at least one biome.");
                    } else {
                        if (!string.IsNullOrWhiteSpace(textBox.Text) || listBox.SelectedIndices.Count == 1)
                        {
                            if ((listBox.SelectedIndices.Count == 1 && textBox.Text.Length == 0) || System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_+ -]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                if (listBox.SelectedIndices.Count > 1)
                                {
                                    PopupFormSelectedItem = textBox.Text;
                                } else {                                        
                                    PopupFormSelectedItem = textBox.Text.Length > 0 ? textBox.Text : (string)listBox.SelectedItem;
                                }
                                form.DialogResult = DialogResult.OK;
                                form.Close();
                                form.Dispose();
                            } else {
                                form.DialogResult = DialogResult.None;
                                PopUpForm.CustomMessageBox("Name contains illegal characters.", "Illegal input");
                            }
                        } else {
                            form.DialogResult = DialogResult.None;
                            PopUpForm.CustomMessageBox("More than one biome has been selected, name cannot be empty.", "Illegal input");
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

            form.ClientSize = new Size(396, buttonCancel.Location.Y + buttonCancel.Height + (margin * 2));
            form.Controls.AddRange(new Control[] { label, textBox, labelListBox, listBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
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

        private static ComboBox ManageWorldsBoxFormLbVersion = null;
        private static ListBox ManageWorldsBoxFormLbWorlds = null;
        private static ListBox ManageWorldsBoxFormLbBiomes = null;

        public static void ManageWorldsBox()
        {
            PopupFormSelectedItem = null;

            int margin = 4;

            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            form.Text = "World templates and biomes";
            form.Width = 300;
            form.Height = 600;

            Label lbVersions = new Label();
            lbVersions.Text = "OTG / TerrainControl version";
            lbVersions.SetBounds(12, 12, 372, 20);
            lbVersions.Anchor = lbVersions.Anchor | AnchorStyles.Right;

            ComboBox cbVersion = new ComboBox();
            cbVersion.SetBounds(12, lbVersions.Top + lbVersions.Height + margin, 372, 200);
            cbVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            cbVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            cbVersion.Height = 50;
            cbVersion.MouseWheel += ManageWorldsBox_MouseWheel;
            ManageWorldsBoxFormLbVersion = cbVersion;

            Label lbWorlds = new Label();
            lbWorlds.Text = "Worlds";
            lbWorlds.SetBounds(12, cbVersion.Top + cbVersion.Height + (margin * 3), 372, 20);
            lbWorlds.Anchor = lbWorlds.Anchor | AnchorStyles.Right;

            ListBox listBoxWorlds = new ListBox();
            listBoxWorlds.SetBounds(12, lbWorlds.Top + lbWorlds.Height + margin, 372, 200);
            listBoxWorlds.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            listBoxWorlds.SelectionMode = SelectionMode.One;
            listBoxWorlds.Height = 100;
            listBoxWorlds.MouseWheel += ManageWorldsBox_MouseWheel;
            ManageWorldsBoxFormLbWorlds = listBoxWorlds;

            Button buttonCreateWorld = new Button();
            buttonCreateWorld.Width = 90;
            buttonCreateWorld.Text = "Create world";
            buttonCreateWorld.Location = new Point(12, listBoxWorlds.Location.Y + listBoxWorlds.Height + margin);
            buttonCreateWorld.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Button buttonRenameWorld = new Button();
            buttonRenameWorld.Width = 90;
            buttonRenameWorld.Text = "Rename world";
            buttonRenameWorld.Location = new Point(buttonCreateWorld.Left + buttonCreateWorld.Width + margin, listBoxWorlds.Location.Y + listBoxWorlds.Height + margin);
            buttonRenameWorld.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Button buttonDeleteWorld = new Button();
            buttonDeleteWorld.Width = 90;
            buttonDeleteWorld.Text = "Delete world";
            buttonDeleteWorld.Location = new Point(buttonRenameWorld.Left + buttonRenameWorld.Width + margin, listBoxWorlds.Location.Y + listBoxWorlds.Height + margin);
            buttonDeleteWorld.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Button buttonImportWorld = new Button();
            buttonImportWorld.Width = 90;
            buttonImportWorld.Text = "Import world";
            buttonImportWorld.Location = new Point(buttonDeleteWorld.Left + buttonDeleteWorld.Width + margin, listBoxWorlds.Location.Y + listBoxWorlds.Height + margin);
            buttonImportWorld.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonImportWorld.Click += btImportWorld_Click;

            Label lbBiomes = new Label();
            lbBiomes.Text = "Biomes";
            lbBiomes.SetBounds(12, buttonCreateWorld.Top + buttonCreateWorld.Height + (margin * 3), 372, 20);
            lbBiomes.Anchor = lbBiomes.Anchor | AnchorStyles.Right;

            ListBox listBoxBiomes = new ListBox();
            listBoxBiomes.SetBounds(12, lbBiomes.Top + lbBiomes.Height + margin, 372, 200);
            listBoxBiomes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            listBoxBiomes.SelectionMode = SelectionMode.One;
            listBoxBiomes.Height = 150;
            listBoxBiomes.MouseWheel += ManageWorldsBox_MouseWheel;
            ManageWorldsBoxFormLbBiomes = listBoxBiomes;

            Button buttonCreateBiome = new Button();
            buttonCreateBiome.Width = 121;
            buttonCreateBiome.Text = "Create biome";
            buttonCreateBiome.Location = new Point(12, listBoxBiomes.Location.Y + listBoxBiomes.Height + (int)(margin * 1.5));
            buttonCreateBiome.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Button buttonRenameBiome = new Button();
            buttonRenameBiome.Width = 121;
            buttonRenameBiome.Text = "Rename biome";
            buttonRenameBiome.Location = new Point(buttonCreateBiome.Left + buttonCreateBiome.Width + margin, listBoxBiomes.Location.Y + listBoxBiomes.Height + (int)(margin * 1.5));
            buttonRenameBiome.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Button buttonDeleteBiome = new Button();
            buttonDeleteBiome.Width = 121;
            buttonDeleteBiome.Text = "Delete biome";
            buttonDeleteBiome.Location = new Point(buttonRenameBiome.Left + buttonRenameBiome.Width + margin, listBoxBiomes.Location.Y + listBoxBiomes.Height + (int)(margin * 1.5));
            buttonDeleteBiome.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            buttonCreateWorld.Click += new EventHandler
            (
                delegate
                {
                    PopUpForm.CreateWorldBox(cbVersion.Text, (string)listBoxWorlds.SelectedItem);

                    listBoxWorlds.Items.Clear();
                    DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\");
                    if (versionDir3.Exists)
                    {
                        foreach (DirectoryInfo dir2 in versionDir3.GetDirectories())
                        {
                            listBoxWorlds.Items.Add(dir2.Name);
                        }
                        if (listBoxWorlds.Items.Count > 0)
                        {
                            listBoxWorlds.SelectedIndex = 0;
                        } else {
                            listBoxBiomes.Items.Clear();
                        }
                    }

                    if (listBoxWorlds.SelectedItem != null && ((string)(listBoxWorlds.SelectedItem)).Equals("Default"))
                    {
                        buttonRenameWorld.Enabled = false;
                        buttonDeleteWorld.Enabled = false;
                        buttonCreateBiome.Enabled = false;
                        buttonRenameBiome.Enabled = false;
                        buttonDeleteBiome.Enabled = false;
                    } else {
                        buttonRenameWorld.Enabled = true;
                        buttonDeleteWorld.Enabled = true;
                        buttonCreateBiome.Enabled = true;
                        buttonRenameBiome.Enabled = true;
                        buttonDeleteBiome.Enabled = true;
                    }

                    ManageWorldsBoxFormLbVersion.Focus();
                }
            );

            buttonRenameWorld.Click += new EventHandler
            (
                delegate
                {
                    string newWorldName = (string)listBoxWorlds.SelectedItem;
                    if (listBoxWorlds.SelectedItem != null && PopUpForm.InputBox("Rename world", "", ref newWorldName, false) == DialogResult.OK)
                    {
                        DirectoryInfo srcworldDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\");
                        DirectoryInfo worldDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + newWorldName + "\\");
                        if (srcworldDir3.Exists)
                        {
                            if (!worldDir3.Exists)
                            {
                                Session.ShowProgessBox();

                                System.Security.AccessControl.DirectorySecurity sec3 = srcworldDir3.GetAccessControl();
                                System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                                sec3.AddAccessRule(accRule3);

                                srcworldDir3.MoveTo(worldDir3.FullName);

                                listBoxWorlds.Items.Clear();
                                DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\");
                                if (versionDir3.Exists)
                                {
                                    foreach (DirectoryInfo dir2 in versionDir3.GetDirectories())
                                    {
                                        listBoxWorlds.Items.Add(dir2.Name);
                                    }
                                    if (listBoxWorlds.Items.Count > 0)
                                    {
                                        listBoxWorlds.SelectedIndex = 0;
                                    } else {
                                        listBoxBiomes.Items.Clear();
                                    }
                                }

                                if (listBoxWorlds.SelectedItem != null && ((string)(listBoxWorlds.SelectedItem)).Equals("Default"))
                                {
                                    buttonRenameWorld.Enabled = false;
                                    buttonDeleteWorld.Enabled = false;
                                    buttonCreateBiome.Enabled = false;
                                    buttonRenameBiome.Enabled = false;
                                    buttonDeleteBiome.Enabled = false;
                                } else {
                                    buttonRenameWorld.Enabled = true;
                                    buttonDeleteWorld.Enabled = true;
                                    buttonCreateBiome.Enabled = true;
                                    buttonRenameBiome.Enabled = true;
                                    buttonDeleteBiome.Enabled = true;
                                }

                                Session.HideProgessBox();

                            } else {
                                PopUpForm.CustomMessageBox("A world with that name already exists.");
                            }
                        }
                    }
                    ManageWorldsBoxFormLbVersion.Focus();
                }
            );

            buttonDeleteWorld.Click += new EventHandler
            (
                delegate
                {
                    DirectoryInfo srcworldDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\");
                    if (srcworldDir3.Exists)
                    {
                        if (PopUpForm.CustomYesNoBox("Delete world", "Are you sure you want to delete world \"" + listBoxWorlds.SelectedItem + "\"?", "Delete", "Cancel") == DialogResult.Yes)
                        {
                            Session.ShowProgessBox();

                            System.Security.AccessControl.DirectorySecurity sec3 = srcworldDir3.GetAccessControl();
                            System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                            sec3.AddAccessRule(accRule3);

                            srcworldDir3.Delete(true);
                            srcworldDir3.Refresh();

                            srcworldDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\");
                            srcworldDir3.Refresh();

                            listBoxWorlds.Items.Clear();
                            DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\");
                            if (versionDir3.Exists)
                            {
                                foreach (DirectoryInfo dir2 in versionDir3.GetDirectories())
                                {
                                    listBoxWorlds.Items.Add(dir2.Name);
                                }
                                if (listBoxWorlds.Items.Count > 0)
                                {
                                    listBoxWorlds.SelectedIndex = 0;
                                } else {
                                    listBoxBiomes.Items.Clear();
                                }
                            }

                            if (listBoxWorlds.SelectedItem != null && ((string)(listBoxWorlds.SelectedItem)).Equals("Default"))
                            {
                                buttonRenameWorld.Enabled = false;
                                buttonDeleteWorld.Enabled = false;
                                buttonCreateBiome.Enabled = false;
                                buttonRenameBiome.Enabled = false;
                                buttonDeleteBiome.Enabled = false;
                            } else {
                                buttonRenameWorld.Enabled = true;
                                buttonDeleteWorld.Enabled = true;
                                buttonCreateBiome.Enabled = true;
                                buttonRenameBiome.Enabled = true;
                                buttonDeleteBiome.Enabled = true;
                            }

                            Session.HideProgessBox();
                        }
                    }

                    ManageWorldsBoxFormLbVersion.Focus();
                }
            );

            buttonCreateBiome.Click += new EventHandler
            (
                delegate
                {
                    PopUpForm.CreateBiomeBox(cbVersion.Text, (string)listBoxWorlds.SelectedItem);

                    listBoxBiomes.Items.Clear();
                    DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\WorldBiomes\\");
                    if (versionDir3.Exists)
                    {
                        foreach (FileInfo file2 in versionDir3.GetFiles())
                        {
                            if (file2.Extension.ToLower().Equals(".bc"))
                            {
                                listBoxBiomes.Items.Add(file2.Name.Substring(0, file2.Name.Length - 3));
                            }
                        }
                        if (listBoxBiomes.Items.Count > 0)
                        {
                            listBoxBiomes.SelectedIndex = 0;
                        }
                    }

                    if (listBoxWorlds.SelectedItem != null && ((string)(listBoxWorlds.SelectedItem)).Equals("Default"))
                    {
                        buttonRenameWorld.Enabled = false;
                        buttonDeleteWorld.Enabled = false;
                        buttonCreateBiome.Enabled = false;
                        buttonRenameBiome.Enabled = false;
                        buttonDeleteBiome.Enabled = false;
                    } else {
                        buttonRenameWorld.Enabled = true;
                        buttonDeleteWorld.Enabled = true;
                        buttonCreateBiome.Enabled = true;
                        buttonRenameBiome.Enabled = true;
                        buttonDeleteBiome.Enabled = true;
                    }

                    ManageWorldsBoxFormLbVersion.Focus();
                }
            );

            buttonRenameBiome.Click += new EventHandler
            (
                delegate
                {
                    string newBiomeName = (string)listBoxBiomes.SelectedItem;
                    if (listBoxBiomes.SelectedItem != null && PopUpForm.InputBox("Rename biome", "", ref newBiomeName, false) == DialogResult.OK)
                    {
                        FileInfo srcbiomeFile3 = new FileInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\WorldBiomes\\" + listBoxBiomes.SelectedItem + ".bc");
                        FileInfo biomeFile3 = new FileInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\WorldBiomes\\" + newBiomeName + ".bc");
                        if (srcbiomeFile3.Exists)
                        {
                            if (!biomeFile3.Exists)
                            {
                                Session.ShowProgessBox();

                                System.Security.AccessControl.FileSecurity sec3 = srcbiomeFile3.GetAccessControl();
                                System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                                sec3.AddAccessRule(accRule3);

                                srcbiomeFile3.MoveTo(biomeFile3.FullName);

                                listBoxBiomes.Items.Clear();
                                DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\WorldBiomes\\");
                                if (versionDir3.Exists)
                                {
                                    foreach (FileInfo file2 in versionDir3.GetFiles())
                                    {
                                        if (file2.Extension.ToLower().Equals(".bc"))
                                        {
                                            listBoxBiomes.Items.Add(file2.Name.Substring(0, file2.Name.Length - 3));
                                        }
                                    }
                                    if (listBoxBiomes.Items.Count > 0)
                                    {
                                        listBoxBiomes.SelectedIndex = 0;
                                    }
                                }

                                if (listBoxWorlds.SelectedItem != null && ((string)(listBoxWorlds.SelectedItem)).Equals("Default"))
                                {
                                    buttonRenameWorld.Enabled = false;
                                    buttonDeleteWorld.Enabled = false;
                                    buttonCreateBiome.Enabled = false;
                                    buttonRenameBiome.Enabled = false;
                                    buttonDeleteBiome.Enabled = false;
                                } else {
                                    buttonRenameWorld.Enabled = true;
                                    buttonDeleteWorld.Enabled = true;
                                    buttonCreateBiome.Enabled = true;
                                    buttonRenameBiome.Enabled = true;
                                    buttonDeleteBiome.Enabled = true;
                                }

                                Session.HideProgessBox();
                                
                            } else {
                                PopUpForm.CustomMessageBox("A biome with that name already exists.");
                            }
                        }
                    }

                    ManageWorldsBoxFormLbVersion.Focus();
                }
            );

            buttonDeleteBiome.Click += new EventHandler
            (
                delegate
                {
                    FileInfo srcBiomedFile3 = new FileInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\WorldBiomes\\" + listBoxBiomes.SelectedItem + ".bc");
                    if (srcBiomedFile3.Exists)
                    {
                        if (PopUpForm.CustomYesNoBox("Delete biome", "Are you sure you want to delete biome \"" + listBoxBiomes.SelectedItem + "\"?", "Delete", "Cancel") == DialogResult.Yes)
                        {
                            Session.ShowProgessBox();

                            System.Security.AccessControl.FileSecurity sec3 = srcBiomedFile3.GetAccessControl();
                            System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                            sec3.AddAccessRule(accRule3);

                            srcBiomedFile3.Delete();
                            srcBiomedFile3.Refresh();

                            listBoxBiomes.Items.Clear();
                            DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\WorldBiomes\\");
                            if (versionDir3.Exists)
                            {
                                foreach (FileInfo file2 in versionDir3.GetFiles())
                                {
                                    if (file2.Extension.ToLower().Equals(".bc"))
                                    {
                                        listBoxBiomes.Items.Add(file2.Name.Substring(0, file2.Name.Length - 3));
                                    }
                                }
                                if (listBoxBiomes.Items.Count > 0)
                                {
                                    listBoxBiomes.SelectedIndex = 0;
                                }
                            }

                            if (listBoxWorlds.SelectedItem != null && ((string)(listBoxWorlds.SelectedItem)).Equals("Default"))
                            {
                                buttonRenameWorld.Enabled = false;
                                buttonDeleteWorld.Enabled = false;
                                buttonCreateBiome.Enabled = false;
                                buttonRenameBiome.Enabled = false;
                                buttonDeleteBiome.Enabled = false;
                            } else {
                                buttonRenameWorld.Enabled = true;
                                buttonDeleteWorld.Enabled = true;
                                buttonCreateBiome.Enabled = true;
                                buttonRenameBiome.Enabled = true;
                                buttonDeleteBiome.Enabled = true;
                            }

                            Session.HideProgessBox();
                        }
                    }

                    ManageWorldsBoxFormLbVersion.Focus();
                }
            );

            cbVersion.SelectedIndexChanged += new EventHandler(delegate(object s, EventArgs args)
            {
                listBoxWorlds.Items.Clear();
                DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\");
                if (versionDir3.Exists)
                {
                    foreach (DirectoryInfo dir2 in versionDir3.GetDirectories())
                    {
                        listBoxWorlds.Items.Add(dir2.Name);
                    }
                    if (listBoxWorlds.Items.Count > 0)
                    {
                        listBoxWorlds.SelectedIndex = 0;
                    } else {
                        listBoxBiomes.Items.Clear();
                    }

                    if (listBoxWorlds.SelectedItem != null && ((string)(listBoxWorlds.SelectedItem)).Equals("Default"))
                    {
                        buttonRenameWorld.Enabled = false;
                        buttonDeleteWorld.Enabled = false;
                        buttonCreateBiome.Enabled = false;
                        buttonRenameBiome.Enabled = false;
                        buttonDeleteBiome.Enabled = false;
                    } else {
                        buttonRenameWorld.Enabled = true;
                        buttonDeleteWorld.Enabled = true;
                        buttonCreateBiome.Enabled = true;
                        buttonRenameBiome.Enabled = true;
                        buttonDeleteBiome.Enabled = true;
                    }
                }
            });

            listBoxWorlds.SelectedIndexChanged += new EventHandler(delegate(object s, EventArgs args)
            {
                listBoxBiomes.Items.Clear();
                DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + cbVersion.SelectedItem + "\\Worlds\\" + listBoxWorlds.SelectedItem + "\\WorldBiomes\\");
                if (versionDir3.Exists)
                {
                    foreach (FileInfo file2 in versionDir3.GetFiles())
                    {
                        if (file2.Extension.ToLower().Equals(".bc"))
                        {
                            listBoxBiomes.Items.Add(file2.Name.Substring(0, file2.Name.Length - 3));
                        }                        
                    }
                    if (listBoxBiomes.Items.Count > 0)
                    {
                        listBoxBiomes.SelectedIndex = 0;
                    }
                }

                if (listBoxWorlds.SelectedItem != null && ((string)(listBoxWorlds.SelectedItem)).Equals("Default"))
                {
                    buttonRenameWorld.Enabled = false;
                    buttonDeleteWorld.Enabled = false;
                    buttonCreateBiome.Enabled = false;
                    buttonRenameBiome.Enabled = false;
                    buttonDeleteBiome.Enabled = false;
                } else {
                    buttonRenameWorld.Enabled = true;
                    buttonDeleteWorld.Enabled = true;
                    buttonCreateBiome.Enabled = true;
                    buttonRenameBiome.Enabled = true;
                    buttonDeleteBiome.Enabled = true;
                }
            });

            if (Session.VersionDir.Exists)
            {
                foreach (DirectoryInfo dir1 in Session.VersionDir.GetDirectories())
                {
                    cbVersion.Items.Add(dir1.Name);
                }
                if (cbVersion.Items.Count > 0)
                {
                    cbVersion.SelectedIndex = 0;
                }
            }

            Button buttonOk = new Button();
            buttonOk.Text = "OK";

            buttonOk.SetBounds(12, buttonCreateBiome.Location.Y + buttonCreateBiome.Height + (margin * 4), 75, 23);
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            Button buttonOpenWorldFolder = new Button();
            buttonOpenWorldFolder.Text = "Open world folder";

            buttonOpenWorldFolder.SetBounds(92, buttonCreateBiome.Location.Y + buttonCreateBiome.Height + (margin * 4), 110, 23);
            buttonOpenWorldFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonOpenWorldFolder.Click  += new EventHandler(delegate(object s, EventArgs args)                   
            {
                if(!String.IsNullOrWhiteSpace((string)cbVersion.SelectedItem) && !String.IsNullOrWhiteSpace((string)listBoxWorlds.SelectedItem))
                {
                    DirectoryInfo srcWorld = new DirectoryInfo(Session.VersionDir.FullName + "\\" + cbVersion.SelectedItem + "\\Worlds\\" + (string)listBoxWorlds.SelectedItem);
                    if(srcWorld.Exists)
                    {
                        System.Diagnostics.Process.Start(@"" + srcWorld.FullName);
                    }
                }
            });

            form.ClientSize = new Size(396, buttonOk.Location.Y + buttonOk.Height + (margin * 2));
            form.Controls.AddRange(new Control[] { lbVersions, cbVersion, lbWorlds, listBoxWorlds, buttonCreateWorld, buttonRenameWorld, buttonDeleteWorld, buttonImportWorld, lbBiomes, listBoxBiomes, buttonCreateBiome, buttonRenameBiome, buttonDeleteBiome, buttonOk, buttonOpenWorldFolder });

            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonOk;

            form.ShowDialog();
        }

        static void ManageWorldsBox_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            bool senderIsMouseTarget = false;

            if (ManageWorldsBoxFormLbVersion.ClientRectangle.Contains(ManageWorldsBoxFormLbVersion.PointToClient(Control.MousePosition)))
            {
                senderIsMouseTarget = sender == ManageWorldsBoxFormLbVersion;
                ManageWorldsBoxFormLbVersion.Focus();                
            }
            else if (ManageWorldsBoxFormLbWorlds.ClientRectangle.Contains(ManageWorldsBoxFormLbWorlds.PointToClient(Control.MousePosition)))
            {
                senderIsMouseTarget = sender == ManageWorldsBoxFormLbWorlds;
                ManageWorldsBoxFormLbWorlds.Focus();                
            }
            else if (ManageWorldsBoxFormLbBiomes.ClientRectangle.Contains(ManageWorldsBoxFormLbBiomes.PointToClient(Control.MousePosition)))
            {
                senderIsMouseTarget = sender == ManageWorldsBoxFormLbBiomes;
                ManageWorldsBoxFormLbBiomes.Focus();                
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

        private static ComboBox CreateWorldBoxFormcbWorlds = null;
        public static void CreateWorldBox(string versionName, string worldName)
        {
            string groupName = "";

            PopupFormSelectedItem = null;
            int margin = 4;

            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            form.Text = "Create world";

            Label label = new Label();
            label.Text = "Enter a name for the world. Only a-z A-Z 0-9 space and _ are allowed.";
            label.SetBounds(12, 11, 372, 13);

            TextBox textBox = new TextBox();
            textBox.Text = groupName;
            textBox.SetBounds(12, label.Location.Y + label.Height + (margin * 2), 372, 20);
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            textBox.TabIndex = 0;
            textBox.SelectionStart = Math.Max(0, textBox.Text.Length);
            textBox.SelectionLength = 0;
            textBox.MouseWheel += CreateBiomeBox_MouseWheel;

            Label lbWorlds = new Label();
            lbWorlds.Text = "Select a world to use as a base.";
            lbWorlds.SetBounds(12, textBox.Location.Y + textBox.Height + (margin * 2), 372, 20);
            lbWorlds.Anchor = lbWorlds.Anchor | AnchorStyles.Right;

            ComboBox cbWorlds = new ComboBox();
            cbWorlds.SetBounds(12, lbWorlds.Top + lbWorlds.Height + margin, 372, 200);
            cbWorlds.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            cbWorlds.DropDownStyle = ComboBoxStyle.DropDownList;
            cbWorlds.Height = 50;
            cbWorlds.MouseWheel += CreateBiomeBox_MouseWheel;
            CreateWorldBoxFormcbWorlds = cbWorlds;

            Button buttonOk = new Button();
            buttonOk.Text = "OK";
            buttonOk.Click += new EventHandler
            (
                delegate
                {
                    if (String.IsNullOrWhiteSpace((string)cbWorlds.SelectedItem))
                    {
                        form.DialogResult = DialogResult.None;
                        PopUpForm.CustomMessageBox("Select a source world.");
                    } else {
                        if (!string.IsNullOrWhiteSpace(textBox.Text))
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_ ]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                PopupFormSelectedItem = (string)cbWorlds.SelectedItem;

                                DirectoryInfo srcWorld = new DirectoryInfo(Session.VersionDir.FullName + "\\" + versionName + "\\Worlds\\" + (string)cbWorlds.SelectedItem);
                                if (srcWorld.Exists)
                                {
                                    DirectoryInfo destWorld = new DirectoryInfo(Session.VersionDir.FullName + "\\" + versionName + "\\Worlds\\" + textBox.Text);
                                    if (!destWorld.Exists || PopUpForm.CustomYesNoBox("Directory already exists", "World \"" + textBox.Text + "\" already exists, override it?", "Override", "Cancel") == DialogResult.Yes)
                                    {
                                        Session.ShowProgessBox();

                                        System.Security.AccessControl.DirectorySecurity sec3 = srcWorld.GetAccessControl();
                                        System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                                        sec3.AddAccessRule(accRule3);

                                        Utils.DirectoryUtils.DirectoryCopy(srcWorld.FullName, destWorld.FullName, true);

                                        Session.HideProgessBox();

                                        form.DialogResult = DialogResult.OK;
                                        form.Close();
                                        form.Dispose();
                                        return;
                                    } else {
                                        form.DialogResult = DialogResult.None;
                                    }
                                } else {
                                    form.DialogResult = DialogResult.None;
                                    PopUpForm.CustomMessageBox("Error: Could not find source world.", "Directory not found");
                                }
                            } else {
                                form.DialogResult = DialogResult.None;
                                PopUpForm.CustomMessageBox("Name contains illegal characters.", "Illegal input");
                            }
                        } else {
                            form.DialogResult = DialogResult.None;
                            PopUpForm.CustomMessageBox("Name cannot be empty.", "Illegal input");
                        }
                    }
                    CreateBiomeBoxFormcbWorlds.Focus();
                }
            );
            buttonOk.SetBounds(228, cbWorlds.Location.Y + cbWorlds.Height + (margin * 2), 75, 23);
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Button buttonCancel = new Button();
            buttonCancel.Text = "Cancel";
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.SetBounds(309, cbWorlds.Location.Y + cbWorlds.Height + (margin * 2), 75, 23);
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, buttonCancel.Location.Y + buttonCancel.Height + (margin * 2));
            form.Controls.AddRange(new Control[] { label, textBox, lbWorlds, lbWorlds, cbWorlds, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            if (Session.VersionDir.Exists)
            {
                DirectoryInfo worldsDir = new DirectoryInfo(Session.VersionDir.FullName + "\\" + versionName + "\\Worlds\\");
                if (worldsDir.Exists)
                {
                    foreach (DirectoryInfo dir1 in worldsDir.GetDirectories())
                    {
                        cbWorlds.Items.Add(dir1.Name);
                    }
                    if (cbWorlds.Items.Count > 0)
                    {
                        cbWorlds.SelectedIndex = 0;
                    }
                }
            }

            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                groupName = PopupFormSelectedItem;
            }
        }

        private static ComboBox CreateBiomeBoxFormcbWorlds = null;
        private static ListBox CreateBiomeBoxFormLbBiomes = null;
        public static void CreateBiomeBox(string versionName, string worldName)
        {
            string groupName = "";

            PopupFormSelectedItem = null;
            int margin = 4;

            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            form.Text = "Create biome";

            Label label = new Label();
            label.Text = "Enter a name for the biome. Only a-z A-Z 0-9 space and _ are allowed.";
            label.SetBounds(12, 11, 372, 13);

            TextBox textBox = new TextBox();
            textBox.Text = groupName;
            textBox.SetBounds(12, label.Location.Y + label.Height + (margin * 2), 372, 20);
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            textBox.TabIndex = 0;
            textBox.SelectionStart = Math.Max(0, textBox.Text.Length);
            textBox.SelectionLength = 0;
            textBox.MouseWheel += CreateBiomeBox_MouseWheel;

            Label lbWorlds = new Label();
            lbWorlds.Text = "Select a biome to use as a base.";
            lbWorlds.SetBounds(12, textBox.Location.Y + textBox.Height + (margin * 2), 372, 20);
            lbWorlds.Anchor = lbWorlds.Anchor | AnchorStyles.Right;

            ComboBox cbWorlds = new ComboBox();
            cbWorlds.SetBounds(12, lbWorlds.Top + lbWorlds.Height + margin, 372, 200);
            cbWorlds.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            cbWorlds.DropDownStyle = ComboBoxStyle.DropDownList;
            cbWorlds.Height = 50;
            cbWorlds.MouseWheel += CreateBiomeBox_MouseWheel;
            CreateBiomeBoxFormcbWorlds = cbWorlds;

            ListBox listBoxBiomes = new ListBox();
            listBoxBiomes.SetBounds(12, cbWorlds.Location.Y + cbWorlds.Height + (margin * 2), 372, 200);
            listBoxBiomes.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            listBoxBiomes.SelectionMode = SelectionMode.MultiExtended;
            listBoxBiomes.MouseWheel += CreateBiomeBox_MouseWheel;
            CreateBiomeBoxFormLbBiomes = listBoxBiomes;

            Button buttonOk = new Button();
            buttonOk.Text = "OK";
            buttonOk.Click += new EventHandler
            (
                delegate
                {
                    if (listBoxBiomes.SelectedIndices.Count != 1)
                    {
                        form.DialogResult = DialogResult.None;
                        PopUpForm.CustomMessageBox("Select at least one biome.");
                    } else {
                        if (!string.IsNullOrWhiteSpace(textBox.Text) && listBoxBiomes.SelectedIndices.Count == 1)
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_ ]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                PopupFormSelectedItem = (string)listBoxBiomes.SelectedItem;

                                FileInfo srcBiome = new FileInfo(Session.VersionDir.FullName + "\\" + versionName + "\\Worlds\\" + (string)cbWorlds.SelectedItem + "\\WorldBiomes\\" + PopupFormSelectedItem + ".bc");
                                if(srcBiome.Exists)
                                {
                                    FileInfo destBiome = new FileInfo(Session.VersionDir.FullName + "\\" + versionName + "\\Worlds\\" + worldName + "\\WorldBiomes\\" + textBox.Text + ".bc");
                                    if (!destBiome.Exists || PopUpForm.CustomYesNoBox("File already exists", "Biome \"" + textBox.Text + "\" already exists, override it?", "Override", "Cancel") == DialogResult.Yes)
                                    {
                                        Session.ShowProgessBox();

                                        System.Security.AccessControl.FileSecurity sec3 = srcBiome.GetAccessControl();
                                        System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                                        sec3.AddAccessRule(accRule3);

                                        srcBiome.CopyTo(destBiome.FullName);

                                        Session.HideProgessBox();

                                        form.DialogResult = DialogResult.OK;
                                        form.Close();
                                        form.Dispose();
                                        return;
                                    } else {
                                        form.DialogResult = DialogResult.None;
                                    }
                                } else {
                                    form.DialogResult = DialogResult.None;
                                    PopUpForm.CustomMessageBox("Error: Could not find source biome.", "File not found");
                                }
                            } else {
                                form.DialogResult = DialogResult.None;
                                PopUpForm.CustomMessageBox("Name contains illegal characters.", "Illegal input");
                            }
                        } else {
                            form.DialogResult = DialogResult.None;
                            PopUpForm.CustomMessageBox("Name cannot be empty.", "Illegal input");
                        }
                    }
                    CreateBiomeBoxFormcbWorlds.Focus();
                }
            );
            buttonOk.SetBounds(228, listBoxBiomes.Location.Y + listBoxBiomes.Height + (margin * 2), 75, 23);
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            Button buttonCancel = new Button();
            buttonCancel.Text = "Cancel";
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.SetBounds(309, listBoxBiomes.Location.Y + listBoxBiomes.Height + (margin * 2), 75, 23);
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, buttonCancel.Location.Y + buttonCancel.Height + (margin * 2));
            form.Controls.AddRange(new Control[] { label, textBox, lbWorlds, lbWorlds, cbWorlds, listBoxBiomes, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            cbWorlds.SelectedIndexChanged += new EventHandler(delegate(object s, EventArgs args)
            {
                listBoxBiomes.Items.Clear();
                DirectoryInfo biomesDir = new DirectoryInfo(Session.VersionDir.FullName + "\\" + versionName + "\\Worlds\\" + cbWorlds.SelectedItem + "\\WorldBiomes\\");
                if (biomesDir.Exists)
                {
                    foreach (FileInfo file2 in biomesDir.GetFiles())
                    {
                        if (file2.Extension.ToLower().Equals(".bc"))
                        {
                            listBoxBiomes.Items.Add(file2.Name.Substring(0, file2.Name.Length - 3));
                        }
                    }
                    if (listBoxBiomes.Items.Count > 0)
                    {
                        listBoxBiomes.SelectedIndex = 0;
                    }
                }
            });

            if (Session.VersionDir.Exists)
            {
                DirectoryInfo worldsDir = new DirectoryInfo(Session.VersionDir.FullName + "\\" + versionName + "\\Worlds\\");
                if (worldsDir.Exists)
                {
                    foreach (DirectoryInfo dir1 in worldsDir.GetDirectories())
                    {
                        cbWorlds.Items.Add(dir1.Name);
                    }
                    if (cbWorlds.Items.Count > 0)
                    {
                        cbWorlds.SelectedIndex = 0;
                    }
                }
            }

            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                groupName = PopupFormSelectedItem;
            }
        }

        static void CreateBiomeBox_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            bool senderIsMouseTarget = false;

            if (CreateBiomeBoxFormcbWorlds.ClientRectangle.Contains(CreateBiomeBoxFormcbWorlds.PointToClient(Control.MousePosition)))
            {
                senderIsMouseTarget = sender == CreateBiomeBoxFormcbWorlds;
                CreateBiomeBoxFormcbWorlds.Focus();
            }
            else if (CreateBiomeBoxFormLbBiomes.ClientRectangle.Contains(CreateBiomeBoxFormLbBiomes.PointToClient(Control.MousePosition)))
            {
                senderIsMouseTarget = sender == CreateBiomeBoxFormLbBiomes;
                CreateBiomeBoxFormLbBiomes.Focus();
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

        static FolderBrowserDialog fbdDestinationWorldDir = new FolderBrowserDialog();
        static SaveFileDialog sfd = null;
        // User clicks button, selects default biomes version, then selects world directory               
        // OTGE creates new world in VersionConfigs dir if there are custom biomes and/or BO3's
        // OTGE creates a save with a group for each biome in the world (unless all its values are defaults)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void btImportWorld_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\Saves\\");
            if (!dir.Exists)
            {
                dir.Create();
                dir.Refresh();
            }
            sfd = new SaveFileDialog() { DefaultExt = "xml", InitialDirectory = dir.FullName };

            // Show select menu and let user select version
            string versionName = SelectVersion();

            fbdDestinationWorldDir.Description = "Select a OTG/TerrainControl world folder. Any BO2's and BO3's that should be imported with this world must be placed in the world's WorldObjects folder (create if needed).";

            // Make user select world
            if (versionName != null && fbdDestinationWorldDir.ShowDialog() == DialogResult.OK)
            {
                // Get world's resource directories

                DirectoryInfo sourceWorldDir = new DirectoryInfo(fbdDestinationWorldDir.SelectedPath);
                DirectoryInfo worldBiomesDir = sourceWorldDir.GetDirectories().FirstOrDefault(a => a.Name.ToLower() == "worldbiomes");
                DirectoryInfo worldObjectsDir = sourceWorldDir.GetDirectories().FirstOrDefault(a => a.Name.ToLower() == "worldobjects");

                bool canContinue = false;
                DirectoryInfo destinationDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + versionName + "\\Worlds\\" + sourceWorldDir.Name + "\\");
                if (!destinationDir.Exists)
                {
                    destinationDir.Create();
                    destinationDir.Refresh();
                    canContinue = true;
                } else {
                    canContinue = PopUpForm.CustomYesNoBox("Delete existing world?", "A world with the same name already exists. Delete the existing world?", "Delete", "Cancel") == DialogResult.Yes;
                    if (canContinue)
                    {
                        // TODO: Is all this really necessary? <- Without this you can't overwrite an existing directory?

                        Session.ShowProgessBox();

                        System.Security.AccessControl.DirectorySecurity sec3 = destinationDir.GetAccessControl();
                        System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                        sec3.AddAccessRule(accRule3);

                        destinationDir.Delete(true);
                        destinationDir.Refresh();

                        destinationDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + versionName + "\\Worlds\\" + sourceWorldDir.Name + "\\");

                        destinationDir.Create();
                        destinationDir.Refresh();

                        Session.HideProgessBox();
                    }
                }

                if (canContinue)
                {
                    List<BiomeConfig> worldBiomes = new List<BiomeConfig>();

                    if (worldBiomesDir != null || worldObjectsDir != null)
                    {
                        DirectoryInfo versionDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\");
                        if (versionDir.Exists)
                        {
                            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(VersionConfig));

                            VersionConfig versionConfig = null;
                            string path = Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + versionName + "\\VersionConfig.xml";
                            if (File.Exists(path))
                            {
                                using (var reader = new XmlTextReader(path))
                                {
                                    versionConfig = (VersionConfig)xmlSerializer.Deserialize(reader);
                                }
                                if (versionConfig != null)
                                {
                                    Session.ShowProgessBox();

                                    List<string> msgs = new List<string>();

                                    // Load selected world's resources

                                    // Load worldconfig
                                    WorldConfig worldConfig = new WorldConfig(versionConfig);
                                    FileInfo worldConfigFile = new FileInfo(sourceWorldDir + "\\WorldConfig.ini");
                                    if (worldConfigFile.Exists)
                                    {
                                        try
                                        {
                                            worldConfig = World.LoadWorldConfigFromFile(worldConfigFile, versionConfig, false);
                                        }
                                        catch (InvalidDataException ex)
                                        {
                                            Session.HideProgessBox();
                                            return;
                                        }
                                        if (worldConfig != null)
                                        {
                                            msgs.Add("WorldConfig found");
                                        }
                                    }

                                    DirectoryInfo biomesDir = worldBiomesDir != null && worldBiomesDir.Exists ? worldBiomesDir : null;

                                    string txtErrorsWrongValue = "";
                                    string txtErrorsNoSetting = "";

                                    // Load biomeconfigs
                                    List<BiomeConfig> biomeConfigs = new List<BiomeConfig>();
                                    if (biomesDir != null)// || biomeConfigsDirPresent != null)
                                    {
                                        foreach (FileInfo biomeFile in biomesDir.GetFiles().ToList())
                                        {
                                            BiomeConfig biomeConfig = Biomes.LoadBiomeConfigFromFile(biomeFile, versionConfig, false, ref txtErrorsWrongValue, ref txtErrorsNoSetting);
                                            if (biomeConfig != null)
                                            {
                                                biomeConfigs.Add(biomeConfig);
                                            } else {
                                                Session.HideProgessBox();
                                                PopUpForm.CustomMessageBox("The file " + biomeFile.Name + " contains critical errors, it was probably not made for use with the selected version of TC/MCW/OTG/OTG+ and requires manual updating.", "Error reading configuration file");
                                                return;
                                            }
                                        }

                                        if (txtErrorsWrongValue.Length > 0)
                                        {
                                            PopUpForm.CustomMessageBox(txtErrorsWrongValue + "\r\n\r\nDefault values will be used instead.", "Version warnings");
                                        }
                                        if (txtErrorsNoSetting.Length > 0)
                                        {
                                            PopUpForm.CustomMessageBox(txtErrorsNoSetting + "\r\n\r\nDefault values will be used instead.", "Version warnings");
                                        }

                                        msgs.Add(biomeConfigs.Count + " biomes found");
                                        msgs.Add("\r\n");
                                    }

                                    // Copy world objects dir
                                    //if (worldObjectsDir != null)
                                    //{
                                    //
                                    //}

                                    // Load default resources for the selected version of TC

                                    // Load worldConfig
                                    WorldConfig defaultWorldConfig = new WorldConfig(versionConfig);
                                    FileInfo defaultWorldConfigFile = new FileInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + versionName + "\\Worlds\\Default\\WorldConfig.ini");
                                    if (defaultWorldConfigFile.Exists)
                                    {
                                        try
                                        {
                                            defaultWorldConfig = World.LoadWorldConfigFromFile(defaultWorldConfigFile, versionConfig, false);
                                        }
                                        catch (InvalidDataException ex)
                                        {
                                            Session.HideProgessBox();
                                            return;
                                        }
                                        if (defaultWorldConfig != null)
                                        {
                                            msgs.Add("Default WorldConfig found");
                                        }
                                    } else {
                                        Session.HideProgessBox();
                                        throw new Exception("WorldConfig.ini could not be loaded. Please make sure that WorldConfig.ini is present in the TCVersionConfig directory for the selected version. Exiting OTGE.");
                                    }

                                    // Load biomeconfigs
                                    List<BiomeConfig> defaultBiomeConfigs = new List<BiomeConfig>();
                                    DirectoryInfo defaultBiomesDir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + versionName + "\\Worlds\\Default\\" + "WorldBiomes");

                                    txtErrorsWrongValue = "";
                                    txtErrorsNoSetting = "";

                                    foreach (FileInfo biomeFile in defaultBiomesDir.GetFiles().ToList())
                                    {
                                        try
                                        {
                                            BiomeConfig biomeConfig = Biomes.LoadBiomeConfigFromFile(biomeFile, versionConfig, false, ref txtErrorsWrongValue, ref txtErrorsNoSetting);
                                            if (biomeConfig != null)
                                            {
                                                defaultBiomeConfigs.Add(biomeConfig);
                                            } else {
                                                Session.HideProgessBox();
                                                return;
                                            }
                                        }
                                        catch (InvalidDataException ex)
                                        {
                                            Session.HideProgessBox();
                                            return;
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

                                    Session.HideProgessBox();

                                    msgs.Add(defaultBiomeConfigs.Count + " default biomes found");

                                    if (msgs.Count > 0)
                                    {
                                        string msgboxmsg = "";
                                        foreach (string msg in msgs)
                                        {
                                            msgboxmsg += msg + "\r\n";
                                        }
                                        PopUpForm.CustomMessageBox(msgboxmsg);
                                    }

                                    bool useDefaults = PopUpForm.CustomYesNoBox("Use OTG/TerrainControl default values?", "Would you like to either:\r\n\r\n- Compare the world to default values and save any non-default values to a OTGE save file?\r\n- Use the imported world as default values for a new world.\r\n\r\n", "Save non-default values", "Use as default values") == DialogResult.Yes;

                                    Session.ShowProgessBox();

                                    //bool isDefaultWorld = worldObjectsDir == null;

                                    if (useDefaults)
                                    {
                                        // Check which settings in WorldConfig are different from default, save them as OTGE pre-set. If this world turns out to be non-compatible with default world then create new world folder and copy default worldconfig to it
                                        foreach (TCProperty property in versionConfig.WorldConfig)
                                        {
                                            string value = worldConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Value;
                                            if (
                                                !(
                                                    (
                                                        property.PropertyType.Equals("Bool") || 
                                                        property.PropertyType.Equals("Float") ||
                                                        property.PropertyType.Equals("Int") || 
                                                        property.PropertyType.Equals("Color")
                                                    ) &&
                                                    String.IsNullOrWhiteSpace(value)
                                                ) &&
                                                value != defaultWorldConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Value
                                            )
                                            {
                                                worldConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Override = true;
                                            } else {
                                                worldConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Override = false;
                                            }
                                        }
                                    }

                                    // Check which biomeconfigs are standard/nonstandard and for standard biomes check which values are not defaults, save those values as OTGE pre-set. 
                                    // If this world turns out to be non-compatible with default world then create new world folder and copy default biomeconfigs + non-standard biomeconfigs to it
                                    // For non-standard biomes get values from inherited biome?

                                    List<Group> groups = new List<Group>();
                                    Group customBiomes = new Group("Custom biomes", versionConfig) { BiomeConfig = new BiomeConfig(versionConfig) };
                                    if (useDefaults)
                                    {
                                        foreach (BiomeConfig biomeConfig in biomeConfigs)
                                        {
                                            if (!defaultBiomeConfigs.Any(a => a.BiomeName == biomeConfig.BiomeName))
                                            {
                                                //isDefaultWorld = false;
                                                customBiomes.Biomes.Add(biomeConfig.BiomeName);
                                            } else {
                                                BiomeConfig biomeDefaultConfig = defaultBiomeConfigs.FirstOrDefault(a => a.BiomeName == biomeConfig.BiomeName);
                                                bool hasDefaultValues = true;
                                                foreach (TCProperty property in versionConfig.BiomeConfig)
                                                {
                                                    string value = biomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Value;
                                                    if (
                                                        !(
                                                            (
                                                                property.PropertyType.Equals("Bool") || 
                                                                property.PropertyType.Equals("Float") ||
                                                                property.PropertyType.Equals("Int") || 
                                                                property.PropertyType.Equals("Color")
                                                            ) &&
                                                            String.IsNullOrWhiteSpace(value)
                                                        ) && 
                                                        value != biomeDefaultConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Value
                                                    )
                                                    {
                                                        biomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Override = true;
                                                        hasDefaultValues = false;
                                                    } else {
                                                        biomeConfig.Properties.FirstOrDefault(a => a.PropertyName == property.Name).Override = false;
                                                    }
                                                }
                                                if (!hasDefaultValues)
                                                {
                                                    groups.Add(new Group(biomeConfig.BiomeName, new List<string>() { biomeConfig.BiomeName }, biomeConfig));
                                                }
                                            }
                                        }
                                    }
                                    if (customBiomes.Biomes.Any())
                                    {
                                        List<Group> newGroups = new List<Group>();
                                        newGroups.Add(customBiomes);
                                        newGroups.AddRange(groups);
                                        groups = newGroups;
                                    }

                                    Session.HideProgessBox();

                                    bool canCelled = false;
                                    if (useDefaults)
                                    {
                                        sfd.CheckFileExists = false;
                                        sfd.Title = "Enter a world name and click save";
                                        if (sfd.ShowDialog() == DialogResult.OK)
                                        {
                                            DirectoryInfo destinationWorldDirectory = new DirectoryInfo(sfd.FileName.Substring(0, sfd.FileName.LastIndexOf("\\") + 1));
                                            if (destinationWorldDirectory.Exists)
                                            {
                                                Session.ShowProgessBox();

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

                                                Session.HideProgessBox();
                                                PopUpForm.CustomMessageBox("Settings saved as: " + sfd.FileName, "Settings saved");
                                            }
                                        } else {
                                            canCelled = true;
                                        }
                                    }

                                    if (!canCelled)
                                    {
                                        Session.ShowProgessBox();

                                        //sfd.Title = "File selection / creation";

                                        // Copy default WorldConfig, BiomeConfigs and WorldObjects to world output dir if the pre-set is not compatible with the default world
                                        //if ((!isDefaultWorld || !useDefaults))
                                        //{

                                        FileInfo presetFileName = new FileInfo(!String.IsNullOrWhiteSpace(sfd.FileName) ? sfd.FileName : sourceWorldDir.FullName);

                                        // Copy world config
                                        World.ConfigWorld(useDefaults ? defaultWorldConfig : worldConfig, defaultWorldConfig, versionConfig, Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + versionName + "\\Worlds\\Default\\", destinationDir.FullName, true);
                                        sfd.CheckFileExists = false;

                                        // Copy BO2's/BO3's
                                        if (worldObjectsDir != null)
                                        {
                                            //DirectoryInfo destinationWorldObjectsDir = new DirectoryInfo(destinationWorldDirectory.FullName + "/WorldObjects");
                                            DirectoryInfo destinationWorldObjectsDir = new DirectoryInfo(destinationDir.FullName + "\\WorldObjects");
                                            if (!destinationWorldObjectsDir.Exists)
                                            {
                                                destinationWorldObjectsDir.Create();
                                                destinationWorldObjectsDir.Refresh();
                                            }

                                            System.Security.AccessControl.DirectorySecurity sec = destinationWorldObjectsDir.GetAccessControl();
                                            System.Security.AccessControl.FileSystemAccessRule accRule = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                                            sec.AddAccessRule(accRule);

                                            Utils.CopyDir.CopyAll(worldObjectsDir, destinationWorldObjectsDir);
                                        }

                                        // Copy Biomes
                                        if (biomesDir != null)
                                        {

                                            DirectoryInfo destinationWorldBiomesDir = new DirectoryInfo(destinationDir.FullName + "\\WorldBiomes");

                                            if (!destinationWorldBiomesDir.Exists)
                                            {
                                                destinationWorldBiomesDir.Create();
                                                destinationWorldBiomesDir.Refresh();
                                            }

                                            System.Security.AccessControl.DirectorySecurity sec = destinationWorldBiomesDir.GetAccessControl();
                                            System.Security.AccessControl.FileSystemAccessRule accRule = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                                            sec.AddAccessRule(accRule);

                                            // Copy non-default biomes
                                            List<string> biomesToExport = new List<string>();
                                            foreach (BiomeConfig biomeConfig in biomeConfigs)
                                            {
                                                if (!defaultBiomeConfigs.Any(a => a.BiomeName == biomeConfig.BiomeName))
                                                {
                                                    Biomes.GenerateBiomeConfigs(biomesDir, new DirectoryInfo(destinationDir.FullName + "\\WorldBiomes"), new List<BiomeConfig>() { biomeConfigs.FirstOrDefault(a => a.BiomeName == biomeConfig.BiomeName) }, versionConfig, true);
                                                } else {
                                                    if(useDefaults)
                                                    {
                                                        Biomes.GenerateBiomeConfigs(defaultBiomesDir, new DirectoryInfo(destinationDir.FullName + "\\WorldBiomes"), new List<BiomeConfig>() { defaultBiomeConfigs.FirstOrDefault(a => a.BiomeName == biomeConfig.BiomeName) }, versionConfig, true);
                                                    } else {
                                                        Biomes.GenerateBiomeConfigs(biomesDir, new DirectoryInfo(destinationDir.FullName + "\\WorldBiomes"), new List<BiomeConfig>() { biomeConfigs.FirstOrDefault(a => a.BiomeName == biomeConfig.BiomeName) }, versionConfig, true);
                                                    }
                                                }
                                            }
                                        }

                                        Session.HideProgessBox();

                                        PopUpForm.CustomMessageBox("World import completed, you can now select the world and load your OTGE save (if any).", "Import completed");
                                        //}
                                    }
                                } else {
                                    PopUpForm.CustomMessageBox("Error: VersionConfig could not be deserialised.");
                                }
                            } else {
                                PopUpForm.CustomMessageBox("Could not find: " + path);
                            }
                        }
                    } else {
                        PopUpForm.CustomMessageBox("Could not find WorldBiomes or WorldObjects directory. At least one is required.");
                    }
                }
            }

            ManageWorldsBoxFormLbWorlds.Items.Clear();
            DirectoryInfo versionDir3 = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\TCVersionConfigs\\" + ManageWorldsBoxFormLbVersion.SelectedItem + "\\Worlds\\");
            if (versionDir3.Exists)
            {
                foreach (DirectoryInfo dir2 in versionDir3.GetDirectories())
                {
                    ManageWorldsBoxFormLbWorlds.Items.Add(dir2.Name);
                }
                if (ManageWorldsBoxFormLbWorlds.Items.Count > 0)
                {
                    ManageWorldsBoxFormLbWorlds.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// Shows select menu and let user select version
        /// </summary>
        private static string SelectVersion()
        {
            // Get available versions from TCVersionConfigs
            List<string> versions = Session.VersionDir.GetDirectories().Select(a => a.Name).ToList();
            return PopUpForm.SingleSelectListBox(versions, "Select a OTG/TerrainControl version", "Which version of OTG/TerrainControl will you use with this world?");
        }

        static void listBox_KeyDown(object sender, KeyEventArgs e)
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

        public static DialogResult InputBox(string title, string promptText, ref string value, bool allowBracesCommasDotsColons = false, bool numericOnly = false, bool allowEmpty = false)
        {
            PopupFormSelectedItem = null;

            int edgeMargin = 13;
            int labelAndButtonMargin = 5;
            int startWidth = 16;

            Form form = new Form();
            form.Text = title;
            form.Width = 600;

            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            Label measureTextWidthLabel = new Label();
            measureTextWidthLabel.Text = value;
            measureTextWidthLabel.Height = 23;
            measureTextWidthLabel.MaximumSize = new Size(form.Width - (edgeMargin * 2) - startWidth, int.MaxValue);
            measureTextWidthLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            measureTextWidthLabel.Location = new Point(0, 1000);
            form.Controls.Add(measureTextWidthLabel);

            bool showPromptText = !String.IsNullOrEmpty(promptText);

            Label label = null;
            if (showPromptText)
            {
                label = new Label();
                label.Text = promptText;
                label.Top = edgeMargin;
                label.Left = edgeMargin;
                label.Width = form.Width - (edgeMargin * 2) - startWidth;
                label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                label.AutoSize = true;
            }

            RichTextBox textBox = new RichTextBox();
            textBox.Multiline = true;
            textBox.WordWrap = true;
            textBox.Left = edgeMargin;
            textBox.Top = edgeMargin + (label != null ? label.Height : 0) + (label != null ? labelAndButtonMargin : 0);
            textBox.Height = measureTextWidthLabel.PreferredHeight + edgeMargin;
            textBox.Width = form.Width - (edgeMargin * 2) - startWidth;
            textBox.Text = value;
            textBox.TabIndex = 0;
            textBox.SelectionStart = Math.Max(0, textBox.Text.Length);
            textBox.SelectionLength = 0;

            Button buttonOk = new Button();
            buttonOk.Top = textBox.Top + textBox.Height + edgeMargin;
            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.Yes;
            buttonOk.AutoSize = true;
            buttonOk.Left = ((form.Width - startWidth) / 2) - buttonOk.Width - (labelAndButtonMargin / 2);

            Button buttonCancel = new Button();
            buttonCancel.Top = textBox.Top + textBox.Height + edgeMargin;
            buttonCancel.Text = "Cancel";            
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.AutoSize = true;
            buttonCancel.Left = ((form.Width - startWidth) / 2) + (labelAndButtonMargin / 2);

            if (showPromptText)
            {
                form.Controls.AddRange(new Control[] { buttonOk, buttonCancel, textBox, label });
            } else {
                form.Controls.AddRange(new Control[] { buttonOk, buttonCancel, textBox });
            }
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonOk;

            form.Height = buttonOk.Top + buttonOk.Height + edgeMargin + 36;

            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.Controls.Remove(measureTextWidthLabel);
            form.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            form.MinimumSize = new Size(194, 119);

            form.Resize += new EventHandler(delegate(object s, EventArgs args)
            {
                buttonOk.Left = ((form.Width - startWidth) / 2) - buttonOk.Width - (labelAndButtonMargin / 2);
                buttonCancel.Left = ((form.Width - startWidth) / 2) + (labelAndButtonMargin / 2);
            });

            string regExNumeric = "^[0-9]*$";
            string regExNumericAlphabetic = "^[a-z0-9_+ -]*$";
            string regExNumericAlphabeticExtended = "^[a-z0-9_+(),.: -]*$";

            buttonOk.Click += new EventHandler
            (
                delegate
                {
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, numericOnly ? regExNumeric : !allowBracesCommasDotsColons ? regExNumericAlphabetic : regExNumericAlphabeticExtended, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        {
                            PopupFormSelectedItem = textBox.Text;
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                            form.Dispose();
                        } else {
                            form.DialogResult = DialogResult.None;
                            PopUpForm.CustomMessageBox("Value contains illegal characters.", "Illegal input");
                        }
                    } else {
                        if (!allowEmpty)
                        {
                            form.DialogResult = DialogResult.None;
                            PopUpForm.CustomMessageBox("Value cannot be empty.", "Illegal input");
                        }
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

        public static DialogResult CustomYesNoBox(string title, string promptText, string yesText, string noText)
        {
            int edgeMargin = 13;
            int labelAndButtonMargin = 5;

            bool showPromptText = !String.IsNullOrEmpty(promptText);

            Label label = null;
            if (showPromptText)
            {
                label = new Label();
                label.Text = promptText;
                label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                label.Location = new Point(edgeMargin, edgeMargin);
                label.AutoSize = true;
            }

            Button buttonOk = new Button();
            buttonOk.Text = yesText;
            buttonOk.Anchor = AnchorStyles.Top | AnchorStyles.Left;      
            buttonOk.DialogResult = DialogResult.Yes;
            buttonOk.AutoSize = true;

            Button buttonCancel = new Button();
            buttonCancel.Text = noText;
            buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            buttonCancel.DialogResult = DialogResult.No;
            buttonCancel.AutoSize = true;

            Form form = new System.Windows.Forms.Form();
            form.Text = title;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            if (showPromptText)
            {
                form.Controls.AddRange(new Control[] { buttonOk, buttonCancel, label });
            } else {
                form.Controls.AddRange(new Control[] { buttonOk, buttonCancel });
            }            
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonOk;
            form.AutoSize = true;

            int formWidthBybutton = edgeMargin + (buttonOk.Width > buttonCancel.Width ? buttonOk.Width : buttonCancel.Width) + labelAndButtonMargin + (buttonOk.Width > buttonCancel.Width ? buttonOk.Width : buttonCancel.Width) + edgeMargin;
            int formWidthByLabel = 0;
            int formHeight = 0;
            if (showPromptText)
            {
                formWidthByLabel = edgeMargin + label.Width + edgeMargin;
                formHeight = label.Height + 65;
            } else {
                formHeight = 50;
            }

            form.ClientSize = new Size(showPromptText && formWidthByLabel > formWidthBybutton ? formWidthByLabel : formWidthBybutton, formHeight);

            buttonOk.Width = (buttonOk.Width > buttonCancel.Width ? buttonOk.Width : buttonCancel.Width);
            buttonOk.Location = new Point((form.ClientSize.Width / 2) - buttonOk.Width - (labelAndButtonMargin / 2), formHeight - buttonOk.Height - edgeMargin);

            buttonCancel.Width = (buttonOk.Width > buttonCancel.Width ? buttonOk.Width : buttonCancel.Width);
            buttonCancel.Location = new Point((form.ClientSize.Width / 2) + (labelAndButtonMargin / 2), formHeight - buttonOk.Height - edgeMargin);

            DialogResult dialogResult = form.ShowDialog();
            return dialogResult;
        }

        public static DialogResult CustomMessageBox(string promptText, string title = "")
        {
            int edgeMargin = 13;

            Label label = new Label();
            label.Text = promptText;
            label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            label.Location = new Point(edgeMargin, edgeMargin);
            label.AutoSize = true;

            Button buttonOk = new Button();
            buttonOk.Text = "Ok";
            buttonOk.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.AutoSize = true;

            Form form = new System.Windows.Forms.Form();
            form.Text = title;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.Controls.AddRange(new Control[] { buttonOk, label });
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonOk;
            form.AutoSize = true;

            int formWidthByLabel = label.Width + (edgeMargin * 2);
            if (formWidthByLabel < 150)
            {
                formWidthByLabel = 150;
            }
            int formHeight = label.Height + 65;

            form.ClientSize = new Size(formWidthByLabel, formHeight);

            buttonOk.Location = new Point((form.ClientSize.Width / 2) - (buttonOk.Width / 2), formHeight - buttonOk.Height - edgeMargin);

            DialogResult dialogResult = form.ShowDialog();
            return DialogResult.OK;
        }
    }
}