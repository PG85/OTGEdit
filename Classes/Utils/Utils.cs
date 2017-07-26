using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Windows.Forms;

namespace OTGE.Utils
{
    public static class Misc
    {
        public static string FormatToolTipText(string toolTipText)
        {
            int maxCharactersPerLine = 150;

            string textWithLineBreaks = "";
            if (toolTipText != null && toolTipText.Length > maxCharactersPerLine)
            {
                for (int i = 0; i < toolTipText.Length; i += maxCharactersPerLine)
                {
                    while (i < toolTipText.Length && toolTipText[i].Equals(' '))
                    {
                        i++;
                    }

                    if (toolTipText.Length > i + maxCharactersPerLine)
                    {
                        string subStr = toolTipText.Substring(i, maxCharactersPerLine);
                        if (subStr.Contains("\r") || subStr.Contains("\n"))
                        {
                            // This line already has a break, find it and move to the next line.
                            int breakAtR = subStr.IndexOf("\r");
                            int breakAtN = subStr.IndexOf("\n");
                            int breakAt = i + Math.Min(breakAtR != -1 ? breakAtR : breakAtN, breakAtN != -1 ? breakAtN : breakAtR);
                            int nextLineAt = breakAt;
                            while (toolTipText[nextLineAt].Equals('\r') || toolTipText[nextLineAt].Equals('\n'))
                            {
                                nextLineAt++;
                            }

                            textWithLineBreaks += subStr.Substring(0, nextLineAt - i);
                            i = nextLineAt - maxCharactersPerLine;
                        } else {
                            // Cut this line at the nearest space before the max line length (if possible, otherwise cut at max line length)
                            int lastSpaceAt = i + maxCharactersPerLine;
                            while (!toolTipText[lastSpaceAt].Equals(' ') && lastSpaceAt > i)
                            {
                                lastSpaceAt--;
                            }

                            if(lastSpaceAt > i && lastSpaceAt != i + maxCharactersPerLine)
                            {
                                textWithLineBreaks += subStr.Substring(0, lastSpaceAt - i) + "\r\n";
                                i = lastSpaceAt - maxCharactersPerLine + 1; // + 1 to skip the space at the start of next line
                            } else {
                                textWithLineBreaks += subStr + "\r\n";
                            }
                        }
                    } else {
                        textWithLineBreaks += toolTipText.Substring(i);
                    }
                }
            } else {
                textWithLineBreaks = toolTipText;
            }

            return textWithLineBreaks;
        }
    }

    public static class StringExtensionMethods
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }

    public class CopyDir
    {
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            target.Create();
            target.Refresh();

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

    public static class TCSettingsUtils
    {
        public static bool CompareBiomeLists(string biomeList, string biomeList2)
        {
            List<string> biomesListItemNames2 = biomeList.Split(',').ToList<string>();
            List<string> defaultBiomesListItemNames2 = biomeList2.Split(',').ToList<string>();
            if (defaultBiomesListItemNames2.Count != biomesListItemNames2.Count)
            {
                return false;
            }
            foreach (string newValue in biomesListItemNames2)
            {
                string defaultValue = defaultBiomesListItemNames2.FirstOrDefault(a => a.Trim().Equals(newValue.Trim()));
                if (defaultValue != null)
                {
                    defaultBiomesListItemNames2.Remove(defaultValue);
                } else {
                    return false;
                }
            }
            if (defaultBiomesListItemNames2.Count != 0)
            {
                return false;
            }
            return true;
        }

        public static bool CompareResourceQueues(string resourceQueue, string resourceQueue2)
        {
            List<string> resourceQueueItemNames2 = resourceQueue.Replace("\r", "").Split('\n').ToList<string>();
            List<string> defaultResourceQueueItemNames2 = resourceQueue2.Replace("\r", "").Split('\n').ToList<string>();
            if (defaultResourceQueueItemNames2.Count != resourceQueueItemNames2.Count)
            {
                return false;
            }
            foreach (string newValue in resourceQueueItemNames2)
            {
                string defaultValue = defaultResourceQueueItemNames2.FirstOrDefault(a => a.Trim().Equals(newValue.Trim()));
                if (defaultValue != null)
                {
                    defaultResourceQueueItemNames2.Remove(defaultValue);
                } else {
                    return false;
                }
            }
            if (defaultResourceQueueItemNames2.Count != 0)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Extends the NumericUpDown control by providing a property to 
    /// set the number of steps that are added to or subtracted from
    /// the value when the mouse wheel is scrolled.
    /// </summary>
    public class NumericUpDownExt : NumericUpDown
    {
        public bool DecimalsAllowed = true;

        public NumericUpDownExt(bool decimalsAllowed)
        {
            DecimalsAllowed = decimalsAllowed;
        }

        private double wheelDelta = 0;

        public override void UpButton()
        {
            int numberOfDecimals = !DecimalsAllowed ? 0 : this.Text.IndexOf(".") > 0 ? this.Text.Length - (this.Text.IndexOf(".") + 1) : 0;
            this.DecimalPlaces = numberOfDecimals;
            double increment = numberOfDecimals == 0 ? 1 : (1D / (Math.Pow(10D, numberOfDecimals)));

            decimal newValue = Value + Convert.ToDecimal(increment);
            if (newValue > this.Maximum)
            {
                newValue = this.Maximum;
            }
            if (newValue < this.Minimum)
            {
                newValue = this.Minimum;
            }
            this.Value = newValue;
        }

        public override void DownButton()
        {
            int numberOfDecimals = !DecimalsAllowed ? 0 : this.Text.IndexOf(".") > 0 ? this.Text.Length - (this.Text.IndexOf(".") + 1) : 0;
            this.DecimalPlaces = numberOfDecimals;
            double increment = numberOfDecimals == 0 ? 1 : (1D / (Math.Pow(10D, numberOfDecimals)));

            decimal newValue = Value - Convert.ToDecimal(increment);
            if (newValue > this.Maximum)
            {
                newValue = this.Maximum;
            }
            if (newValue < this.Minimum)
            {
                newValue = this.Minimum;
            }
            this.Value = newValue;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            int numberOfDecimals = !DecimalsAllowed ? 0 : this.Text.IndexOf(".") > 0 ? this.Text.Length - (this.Text.IndexOf(".") + 1) : 0;
            this.DecimalPlaces = numberOfDecimals;

            if (this.Value > this.Maximum)
            {
                this.Value = this.Maximum;
            }
            if (this.Value < this.Minimum)
            {
                this.Value = this.Minimum;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition)))
            {
                Session.Form1.FocusOnTab();
            } else {

                TextBox tb = GetPrivateFieldValue(this, "upDownEdit") as TextBox;
                int caretPosition = tb.SelectionStart;
                int distanceFromEnd = tb.Text.Length - caretPosition;

                int numberOfDecimals = !DecimalsAllowed ? 0 : this.Text.IndexOf(".") > 0 ? this.Text.Length - (this.Text.IndexOf(".") + 1) : 0;

                if (numberOfDecimals > 0 && distanceFromEnd > 1)
                {
                    numberOfDecimals -= distanceFromEnd;
                    if(numberOfDecimals < 0)
                    {
                        numberOfDecimals = 0;
                    }
                }

                this.DecimalPlaces = numberOfDecimals;
                double increment = numberOfDecimals == 0 ? 1 : (1D / (Math.Pow(10D, numberOfDecimals)));

                HandledMouseEventArgs args = e as HandledMouseEventArgs;
                if (args != null)
                {
                    if (args.Handled)
                    {
                        base.OnMouseWheel(e);
                        return;
                    }
                    args.Handled = true;
                }

                base.OnMouseWheel(e);

                if ((Control.ModifierKeys & (Keys.Alt | Keys.Shift)) == Keys.None && Control.MouseButtons == MouseButtons.None)
                {
                    this.wheelDelta += e.Delta;
                    double num2 = (double)this.wheelDelta / 120d;

                    double num3 = (increment * num2);
                    if (num3 > 0)
                    {
                        this.wheelDelta -= (num3 * (120d / (double)increment));

                        this.Value += Convert.ToDecimal(increment);

                    } else {
                        this.wheelDelta -= (num3 * (120d / (double)increment));

                        this.Value -= Convert.ToDecimal(increment);
                    }
                }

                tb.SelectionLength = 0;
                tb.SelectionStart = caretPosition > tb.Text.Length ? tb.Text.Length : caretPosition;
            }
        }

        private object GetPrivateFieldValue(object obj, string fieldName)
        {
            Type t = obj.GetType();
            System.Reflection.FieldInfo[] fiArr = t.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fiArr != null)
            {
                foreach (System.Reflection.FieldInfo fi in fiArr)
                {
                    if (fi.Name == fieldName)
                    {
                        return (fi).GetValue(obj);
                    }
                }
            }
            return (null);
        }
    }

    public static class DirectoryUtils
    {
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo sourcedir = new DirectoryInfo(sourceDirName);

            // If the source directory does not exist, throw an exception.
            if (!sourcedir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            DirectoryInfo destDir = new DirectoryInfo(destDirName);
            if (destDir.Exists)
            {
                System.Security.AccessControl.DirectorySecurity sec3 = destDir.GetAccessControl();
                System.Security.AccessControl.FileSystemAccessRule accRule3 = new System.Security.AccessControl.FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                sec3.AddAccessRule(accRule3);

                destDir.Delete(true);
                destDir.Refresh();
            }
            destDir.Create();
            destDir.Refresh();

            // Get the file contents of the directory to copy.
            FileInfo[] files = sourcedir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                DirectoryInfo[] dirs = sourcedir.GetDirectories();
                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
