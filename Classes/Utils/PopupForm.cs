using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TCEE.Utils
{
    public static class PopUpForm
    {
        public static string SingleSelectListBox(List<string> listItems, string title = "", string labelText = "")
        {
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

            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
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
                            //if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
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
                                PopUpForm.CustomMessageBox("Name contains illegal characters.", "Warning: Illegal input");
                            }
                        } else {
                            form.DialogResult = DialogResult.None;
                            PopUpForm.CustomMessageBox("More than one biome has been selected, name cannot be empty.", "Warning: Illegal input");
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

        public static DialogResult InputBox(string title, string promptText, ref string value, bool allowBracesCommasDotsColons = false, bool numericOnly = false, bool allowEmpty = false)
        {
            int edgeMargin = 13;
            int labelAndButtonMargin = 5;

            Label measureTextWidthLabel = new Label();
            measureTextWidthLabel.Text = value;
            measureTextWidthLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            measureTextWidthLabel.AutoSize = true;
            measureTextWidthLabel.Location = new Point(0, 1000);

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

            TextBox textBox = new TextBox();
            textBox.Text = value;
            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            textBox.AutoSize = true;

            Button buttonOk = new Button();
            buttonOk.Text = "OK";
            buttonOk.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            buttonOk.DialogResult = DialogResult.Yes;
            buttonOk.AutoSize = true;

            Button buttonCancel = new Button();
            buttonCancel.Text = "Cancel";
            buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.AutoSize = true;

            Form form = new Form();
            form.Text = title;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            if (showPromptText)
            {
                form.Controls.AddRange(new Control[] { buttonOk, buttonCancel, textBox, measureTextWidthLabel, label });
            } else {
                form.Controls.AddRange(new Control[] { buttonOk, buttonCancel, textBox, measureTextWidthLabel });
            }
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonOk;
            form.AutoSize = true;

            int minTextBoxWidth = 300;

            int formWidthByLabel = 0;
            int formWidthByTextBox = edgeMargin + (measureTextWidthLabel.Width > minTextBoxWidth ? measureTextWidthLabel.Width : minTextBoxWidth) + 4 + edgeMargin;
            int formHeight = 0;
            if (showPromptText)
            {
                formWidthByLabel = edgeMargin + label.Width + edgeMargin;
                formHeight = label.Height + 100;
            } else {
                formHeight = 85;
            }

            form.Controls.Remove(measureTextWidthLabel);

            form.ClientSize = new Size(showPromptText && formWidthByLabel > formWidthByTextBox ? formWidthByLabel : formWidthByTextBox, formHeight);

            textBox.Width = form.ClientSize.Width - (edgeMargin * 2);
            textBox.Location = new Point((form.ClientSize.Width - textBox.Width) / 2, form.ClientSize.Height - edgeMargin - buttonOk.Height - edgeMargin - textBox.Height);

            //buttonOk.Width = (form.ClientSize.Width - (edgeMargin * 2) - labelAndButtonMargin) / 2;
            buttonOk.Width = (buttonOk.Width > buttonCancel.Width ? buttonOk.Width : buttonCancel.Width);
            buttonOk.Location = new Point((form.ClientSize.Width / 2) - buttonOk.Width - (labelAndButtonMargin / 2), form.ClientSize.Height - edgeMargin - buttonOk.Height);

            //buttonCancel.Width = (form.ClientSize.Width - (edgeMargin * 2) - labelAndButtonMargin) / 2;
            buttonCancel.Width = (buttonOk.Width > buttonCancel.Width ? buttonOk.Width : buttonCancel.Width);
            buttonCancel.Location = new Point((form.ClientSize.Width / 2) + (labelAndButtonMargin / 2), form.ClientSize.Height - edgeMargin - buttonOk.Height);

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
                            PopUpForm.CustomMessageBox("Value contains illegal characters.", "Warning: Illegal input");
                        }
                    } else {
                        if (!allowEmpty)
                        {
                            form.DialogResult = DialogResult.None;
                            PopUpForm.CustomMessageBox("Value cannot be empty.", "Warning: Illegal input");
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
            form.StartPosition = FormStartPosition.CenterScreen;
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

            //buttonOk.Width = (form.ClientSize.Width - (edgeMargin * 2) - labelAndButtonMargin) / 2;
            buttonOk.Width = (buttonOk.Width > buttonCancel.Width ? buttonOk.Width : buttonCancel.Width);
            buttonOk.Location = new Point((form.ClientSize.Width / 2) - buttonOk.Width - (labelAndButtonMargin / 2), formHeight - buttonOk.Height - edgeMargin);

            //buttonCancel.Width = (form.ClientSize.Width - (edgeMargin * 2) - labelAndButtonMargin) / 2;
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
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.Controls.AddRange(new Control[] { buttonOk, label });
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonOk;
            form.AutoSize = true;

            int formWidthByLabel = label.Width + (edgeMargin * 2);
            int formHeight = label.Height + 65;

            form.ClientSize = new Size(formWidthByLabel, formHeight);

            buttonOk.Location = new Point((form.ClientSize.Width / 2) - (buttonOk.Width / 2), formHeight - buttonOk.Height - edgeMargin);

            DialogResult dialogResult = form.ShowDialog();
            return DialogResult.OK;
        }
    }
}