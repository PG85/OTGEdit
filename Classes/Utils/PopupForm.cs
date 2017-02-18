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
                        MessageBox.Show("Select at least one biome.");
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(textBox.Text) || listBox.SelectedIndices.Count == 1)
                        {
                            if (listBox.SelectedIndices.Count > 1)
                            {
                                //if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_+ -]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                {
                                    PopupFormSelectedItem = textBox.Text;
                                    form.DialogResult = DialogResult.OK;
                                    form.Close();
                                    form.Dispose();
                                }
                                else
                                {
                                    MessageBox.Show("Name contains illegal characters.", "Warning: Illegal input");
                                }
                            }
                            else
                            {
                                PopupFormSelectedItem = textBox.Text.Length > 0 ? textBox.Text : (string)listBox.SelectedItem;
                                form.DialogResult = DialogResult.OK;
                                form.Close();
                                form.Dispose();
                            }
                        }
                        else
                        {
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
            }
            else
            {
                groupName = null;
                return null;
            }
        }

        public static DialogResult InputBox(string title, string promptText, ref string value, bool allowBracesCommasDotsColons = false)
        {
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
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
                        if (!allowBracesCommasDotsColons ? System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_+ -]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase) : System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-z0-9_+(),.: -]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        {
                            PopupFormSelectedItem = textBox.Text;
                            form.DialogResult = DialogResult.OK;
                            form.Close();
                            form.Dispose();
                        }
                        else
                        {
                            MessageBox.Show("Name contains illegal characters.", "Warning: Illegal input");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Name cannot be empty.", "Warning: Illegal input");
                    }
                }
            );

            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                value = PopupFormSelectedItem;
            }
            else
            {
                value = null;
            }
            return dialogResult;
        }

        public static DialogResult CustomOkCancelBox(string title, string promptText, string okText, string cancelText)
        {
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
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
}
