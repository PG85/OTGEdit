using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OTGE.XML;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using OTGE.Utils;

namespace OTGE
{
    public static class World
    {
        public static WorldConfig LoadWorldConfigFromFile(FileInfo file, VersionConfig versionConfig, bool loadUI) // TODO: remove loadui param and load UI a different method! (this method used to be "loadWorldConfigDefaults")
        {
            WorldConfig worldConfig = new WorldConfig(versionConfig);

            if (file.Exists)
            {
                string txtErrorsWrongValue = "";
                string txtErrorsNoSetting = "";

                StringBuilder defaultText = new StringBuilder(System.IO.File.ReadAllText(file.FullName));
                string sDefaultText = defaultText.ToString();
                foreach (TCProperty property in versionConfig.WorldConfig)
                {
                    if (!loadUI || Session.WorldSettingsInputs.ContainsKey(property))
                    {
                        string propertyValue = "";
                        if (property.PropertyType == "ResourceQueue")
                        {
                            int replaceStartIndex = sDefaultText.IndexOf(property.ScriptHandle);
                            if (replaceStartIndex > -1)
                            {
                                // Find end of resource list
                                string seperator = "###############";
                                replaceStartIndex = replaceStartIndex + property.ScriptHandle.Length;

                                char currentChar = defaultText[replaceStartIndex];
                                bool firstEmptLineFound = false;
                                int replaceEndIndex = defaultText.Length - 1;

                                // Find next new line with characters that does not start with a #
                                while (currentChar < replaceEndIndex)
                                {
                                    if (currentChar == '\n' && currentChar + 1 < defaultText.Length)
                                    {
                                        char nextChar = defaultText[replaceStartIndex + 1];
                                        if (!firstEmptLineFound && nextChar != '#')
                                        {
                                            firstEmptLineFound = true;
                                            // Skipped the opening seperator, now find the closing seperator
                                            string searchString = defaultText.ToString(replaceStartIndex, defaultText.Length - replaceStartIndex);
                                            replaceEndIndex = searchString.IndexOf(seperator) - 1 + replaceStartIndex;
                                        }
                                        if (nextChar != '#' && nextChar != '\r' && nextChar != '\n')
                                        {
                                            // Resource found
                                            break;
                                        }

                                        if (nextChar != '\n')
                                        {
                                            // Check the next character and skip it if necessary.
                                            replaceStartIndex++;
                                        }
                                    }
                                    replaceStartIndex++;
                                    currentChar = defaultText[replaceStartIndex];
                                }
                                if (replaceStartIndex > replaceEndIndex)
                                {
                                    replaceStartIndex = replaceEndIndex;
                                }

                                int replaceLength = replaceEndIndex - replaceStartIndex;

                                // Get comments above setting for tooltips
                                // Get all lines above propertyValue before encountering a line starting with multiple '#'s or another propertyvalue
                                int commentStartIndex = -1;
                                int commentEndIndex = replaceStartIndex - 1;
                                int lastNoHashLineBeforeComment = defaultText.ToString(0, replaceStartIndex).LastIndexOf("\n");
                                if (lastNoHashLineBeforeComment > -1)
                                {
                                    while (true)
                                    {
                                        if (lastNoHashLineBeforeComment > 0)
                                        {
                                            int arg = defaultText.ToString(0, lastNoHashLineBeforeComment - 1).LastIndexOf("\n");
                                            if (arg != -1)
                                            {
                                                string s = defaultText.ToString(arg, lastNoHashLineBeforeComment - arg);
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
                                int lastDoubleHashLineBeforeComment = defaultText.ToString(0, replaceStartIndex).LastIndexOf("##\n") > defaultText.ToString(0, replaceStartIndex).LastIndexOf("##\r\n") ? defaultText.ToString(0, replaceStartIndex).LastIndexOf("##\n") + 3 : defaultText.ToString(0, replaceStartIndex).LastIndexOf("##\r\n") + 4;
                                if (lastDoubleHashLineBeforeComment > -1 || lastNoHashLineBeforeComment > -1)
                                {
                                    commentStartIndex = lastDoubleHashLineBeforeComment > lastNoHashLineBeforeComment ? lastDoubleHashLineBeforeComment : lastNoHashLineBeforeComment;
                                }
                                string comment = "";
                                if (commentStartIndex > -1)
                                {
                                    comment = defaultText.ToString(commentStartIndex, commentEndIndex - commentStartIndex).Trim();
                                }
                                if (property.PropertyType == "BiomesList")
                                {
                                    comment += "\r\n\r\nUse CTRL or SHIFT + Click to select one or multiple biomes from the list.";
                                }
                                if (loadUI)
                                {
                                    if(!String.IsNullOrEmpty(comment))
                                    {
                                        Session.ToolTip1.SetToolTip(Session.WorldSettingsInputs[property].Item4, Utils.Misc.FormatToolTipText(comment + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue)));
                                    } else {
                                        Session.ToolTip1.SetToolTip(Session.WorldSettingsInputs[property].Item4, "No description." + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue));
                                    }
                                }

                                if (replaceLength > 0)
                                {
                                    propertyValue = defaultText.ToString(replaceStartIndex, replaceLength).Trim();
                                }

                                if (loadUI)
                                {
                                    Session.IgnoreOverrideCheckChangedWorld = true;
                                    Session.IgnorePropertyInputChangedWorld = true;

                                    ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Clear();
                                    ((ListBox)Session.WorldSettingsInputs[property].Item1).SelectedIndex = -1;
                                    ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                    ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                    ((CheckBox)Session.WorldSettingsInputs[property].Item6.Controls.Find("OverrideParent", true)[0]).Checked = false;

                                    if (propertyValue != null)
                                    {
                                        string[] resourceQueueItemNames = propertyValue.Replace("\r", "").Split('\n');
                                        foreach (string resourceQueueItemName in resourceQueueItemNames)
                                        {
                                            if (!String.IsNullOrEmpty(resourceQueueItemName))
                                            {
                                                ((ListBox)Session.WorldSettingsInputs[property].Item1).Items.Add(resourceQueueItemName.Trim());
                                            }
                                        }
                                    }

                                    worldConfig.SetProperty(property, propertyValue, false, false);

                                    Session.IgnoreOverrideCheckChangedWorld = false;
                                    Session.IgnorePropertyInputChangedWorld = false;
                                } else {
                                    worldConfig.SetProperty(property, propertyValue, false, false);
                                }
                            } else {
                                txtErrorsNoSetting += "\r\nSetting \"" + property.Name + "\" could not be found in WorldConfig.ini file.";
                                if (!property.Optional)
                                {
                                    PopUpForm.CustomMessageBox(txtErrorsNoSetting, "Version warnings");
                                    PopUpForm.CustomMessageBox("The files you are importing contain critical errors, they were probably not made for use with the selected version of TC/MCW/OTG/OTG+ and require manual updating.", "Error reading configuration files");
                                    return null;
                                }
                                //throw new Exception();
                            }
                        } else {
                            int valueStringStartIndex = sDefaultText.IndexOf(property.ScriptHandle);
                            while (valueStringStartIndex != -1 && !(valueStringStartIndex == 0 || defaultText.ToString(valueStringStartIndex - 1, defaultText.Length - (valueStringStartIndex - 1)).IndexOf("\n") == 0))
                            {
                                int valueStringStartIndex2 = defaultText.ToString(valueStringStartIndex + property.ScriptHandle.Length, defaultText.Length - (valueStringStartIndex + property.ScriptHandle.Length)).IndexOf(property.ScriptHandle);
                                if (valueStringStartIndex2 == -1)
                                {
                                    // No next
                                    valueStringStartIndex = -1;
                                    break;
                                }

                                // Try next
                                valueStringStartIndex += property.ScriptHandle.Length + valueStringStartIndex2;
                            }
                            if (valueStringStartIndex > -1)
                            {
                                // Get comments above setting for tooltips
                                // Get all lines above propertyValue before encountering a line starting with multiple '#'s or another propertyvalue
                                int commentStartIndex = -1;
                                int commentEndIndex = valueStringStartIndex - 1;
                                int lastNoHashLineBeforeComment = defaultText.ToString(0, valueStringStartIndex).LastIndexOf("\n");
                                if (lastNoHashLineBeforeComment > -1)
                                {
                                    while (true)
                                    {
                                        if (lastNoHashLineBeforeComment > 0)
                                        {
                                            int arg = defaultText.ToString(0, lastNoHashLineBeforeComment - 1).LastIndexOf("\n");
                                            if (arg != -1)
                                            {
                                                string s = defaultText.ToString(arg, lastNoHashLineBeforeComment - arg);
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
                                int lastDoubleHashLineBeforeComment = defaultText.ToString(0, valueStringStartIndex).LastIndexOf("##\n") > defaultText.ToString(0, valueStringStartIndex).LastIndexOf("##\r\n") ? defaultText.ToString(0, valueStringStartIndex).LastIndexOf("##\n") + 3 : defaultText.ToString(0, valueStringStartIndex).LastIndexOf("##\r\n") + 4;
                                if (lastDoubleHashLineBeforeComment > -1 || lastNoHashLineBeforeComment > -1)
                                {
                                    commentStartIndex = lastDoubleHashLineBeforeComment > lastNoHashLineBeforeComment ? lastDoubleHashLineBeforeComment : lastNoHashLineBeforeComment;
                                }
                                string comment = "";
                                if (commentStartIndex > -1)
                                {
                                    comment = defaultText.ToString(commentStartIndex, commentEndIndex - commentStartIndex).Trim();
                                }
                                if (property.PropertyType == "BiomesList")
                                {
                                    comment += "\r\n\r\nUse CTRL or SHIFT + Click to select one or multiple biomes from the list.";
                                }
                                if (loadUI)
                                {
                                    if(!String.IsNullOrEmpty(comment))
                                    {
                                        Session.ToolTip1.SetToolTip(Session.WorldSettingsInputs[property].Item4, Utils.Misc.FormatToolTipText(comment + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue)));
                                    } else {
                                        Session.ToolTip1.SetToolTip(Session.WorldSettingsInputs[property].Item4, "No description." + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue));
                                    }
                                }

                                int skipCharsLength = (property.ScriptHandle).Length;
                                int valueStringLength = 0;

                                while (defaultText[valueStringStartIndex + skipCharsLength + valueStringLength] != '\n')
                                {
                                    valueStringLength += 1;
                                }
                                if (valueStringLength > 0)
                                {
                                    propertyValue = defaultText.ToString(valueStringStartIndex + skipCharsLength, valueStringLength).Trim();
                                }

                                try
                                {
                                    if (loadUI)
                                    {
                                        Session.IgnoreOverrideCheckChangedWorld = true;
                                        Session.IgnorePropertyInputChangedWorld = true;
                                        switch (property.PropertyType)
                                        {
                                            case "BiomesList":
                                                ((ListBox)Session.WorldSettingsInputs[property].Item1).SelectedItems.Clear();
                                                string[] biomeNames = propertyValue.Split(',');
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
                                                ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Override", true)[0]).Checked = true;
                                                ((RadioButton)Session.WorldSettingsInputs[property].Item6.Controls.Find("Merge", true)[0]).Checked = false;
                                                break;
                                            case "Bool":
                                                if (propertyValue.ToLower() == "true")
                                                {
                                                    ((Button)Session.WorldSettingsInputs[property].Item1).Text = "true";
                                                    ((Button)Session.WorldSettingsInputs[property].Item1).ForeColor = Color.Green;
                                                    Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                }
                                                else if (propertyValue.ToLower() == "false")
                                                {
                                                    ((Button)Session.WorldSettingsInputs[property].Item1).Text = "false";
                                                    ((Button)Session.WorldSettingsInputs[property].Item1).ForeColor = Color.Red;
                                                    Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                } else {
                                                    ((Button)Session.WorldSettingsInputs[property].Item1).Text = "";
                                                    ((Button)Session.WorldSettingsInputs[property].Item1).ForeColor = Color.Empty;
                                                    Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                }
                                                break;
                                            case "Color":
                                                try
                                                {
                                                    if (propertyValue.Length == 8 || (propertyValue.Length == 7 && propertyValue.StartsWith("#")))
                                                    {
                                                        Session.WorldSettingsInputs[property].Item5.BackColor = System.Drawing.ColorTranslator.FromHtml(propertyValue);
                                                        Session.WorldSettingsInputs[property].Item1.Text = propertyValue;
                                                    } else {
                                                        Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                                        Session.WorldSettingsInputs[property].Item1.Text = "";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Session.WorldSettingsInputs[property].Item5.BackColor = Color.White;
                                                    Session.WorldSettingsInputs[property].Item1.Text = "";
                                                }
                                                Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                break;
                                            case "Float":
                                            case "Int":
                                                int numberOfDecimals = property.PropertyType == "Int" ? 0 : propertyValue.IndexOf(".") > 0 ? propertyValue.Length - (propertyValue.IndexOf(".") + 1) : 0;
                                                ((NumericUpDownExt)Session.WorldSettingsInputs[property].Item1).DecimalPlaces = numberOfDecimals;
                                                Session.WorldSettingsInputs[property].Item1.Text = propertyValue;
                                                Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                break;
                                            case "String":
                                            case "BigString":
                                                Session.WorldSettingsInputs[property].Item1.Text = propertyValue;
                                                Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                break;
                                        }
                                        worldConfig.SetProperty(property, propertyValue, false, false);
                                        Session.IgnoreOverrideCheckChangedWorld = false;
                                        Session.IgnorePropertyInputChangedWorld = false;
                                    } else {
                                        worldConfig.SetProperty(property, propertyValue, false, false);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if(ex is ArgumentNullException || ex is InvalidOperationException)
                                    {
                                        txtErrorsWrongValue += "\r\nValue for setting " + property.Name + " could not be read from WorldConfig.ini file. There is either an error in the file or OTGE needs to be updated to support the value's formatting.";
                                        //throw new Exception("Value for setting " + property.Name + " could not be read from file " + file.Name + ". There is either an error in the file or OTGE needs to be updated to support the value's formatting. Ex: " + ex.Message);
                                    } else {
                                        throw;
                                    }
                                }
                            } else {

                                if (loadUI)
                                {
                                    Session.ToolTip1.SetToolTip(Session.WorldSettingsInputs[property].Item4, "No description." + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue));
                                }
                                worldConfig.SetProperty(property, "", false, false);

                                txtErrorsNoSetting += "\r\nSetting \"" + property.Name + "\" could not be found in WorldConfig.ini file.";
                                if (!property.Optional)
                                {
                                    PopUpForm.CustomMessageBox(txtErrorsNoSetting, "Version warnings");
                                    PopUpForm.CustomMessageBox("The files you are importing contain critical errors, they were probably not made for use with the selected version of TC/MCW/OTG/OTG+ and require manual updating.", "Error reading configuration files");
                                    return null;
                                }

                                //throw new Exception("Setting \"" + property.Name + "\" could not be found in WorldConfig.ini file.");
                            }
                        }
                    }
                }
                if (txtErrorsWrongValue.Length > 0)
                {
                    PopUpForm.CustomMessageBox(txtErrorsWrongValue, "Version warnings");
                }
                if (txtErrorsNoSetting.Length > 0)
                {
                    PopUpForm.CustomMessageBox(txtErrorsNoSetting + "\r\n\r\nDefault values will be used instead.", "Version warnings");
                }
            }
            return worldConfig;
        }

        public static void ConfigWorld(WorldConfig worldConfig, WorldConfig worldConfigDefaultValues, VersionConfig versionConfig, string sourceConfigsDir, string destinationConfigsDir, bool ignoreOverrideCheck)
        {
            System.IO.DirectoryInfo DefaultWorldDirectory = new System.IO.DirectoryInfo(sourceConfigsDir);
            System.IO.FileInfo defaultWorldConfig = new System.IO.FileInfo(DefaultWorldDirectory + "\\WorldConfig.ini");

            if (defaultWorldConfig.Exists)
            {
                StringBuilder defaultText = new StringBuilder(System.IO.File.ReadAllText(defaultWorldConfig.FullName));
                string errorsTxt = "";
                foreach (TCProperty property in versionConfig.WorldConfig)
                {
                    string sDefaultText = defaultText.ToString();

                    string newValue = worldConfig.GetPropertyValueAsString(property);
                    if (
                        newValue != null && 
                        worldConfigDefaultValues != null && 
                        (
                            newValue == worldConfigDefaultValues.GetPropertyValueAsString(property) || 
                            (
                                property.PropertyType.Equals("BiomesList") && 
                                Utils.TCSettingsUtils.CompareBiomeLists(newValue, worldConfigDefaultValues.GetPropertyValueAsString(property))
                            ) || 
                            (
                                property.PropertyType.Equals("ResourceQueue") && 
                                Utils.TCSettingsUtils.CompareResourceQueues(newValue, worldConfigDefaultValues.GetPropertyValueAsString(property))
                            )
                        )
                    )
                    {
                        newValue = null;
                    }
                    if (
                        newValue != null && 
                        (
                            ignoreOverrideCheck ||
                            worldConfig.Properties.First(a => a.PropertyName == property.Name).Override
                        )
                    )
                    {
                        if (property.PropertyType == "ResourceQueue")
                        {
                            int replaceStartIndex = sDefaultText.IndexOf(property.ScriptHandle);

                            if (replaceStartIndex > -1)
                            {
                                // Find end of resource list
                                string seperator = "###############";
                                replaceStartIndex = replaceStartIndex + property.ScriptHandle.Length;
                                       
                                char currentChar = defaultText[replaceStartIndex];
                                bool firstEmptLineFound = false;
                                int replaceEndIndex = defaultText.Length - 1;

                                // Find next new line with characters that does not start with a #
                                while (currentChar < replaceEndIndex)
                                {
                                    if (currentChar == '\n' && currentChar + 1 < defaultText.Length)
                                    {
                                        char nextChar = defaultText[replaceStartIndex + 1];
                                        if (!firstEmptLineFound && nextChar != '#')
                                        {
                                            firstEmptLineFound = true;
                                            // Skipped the opening seperator, now find the closing seperator
                                            string searchString = defaultText.ToString(replaceStartIndex, defaultText.Length - replaceStartIndex);
                                            replaceEndIndex = searchString.IndexOf(seperator) - 1 + replaceStartIndex;
                                        }
                                        if (nextChar != '#' && nextChar != '\r' && nextChar != '\n')
                                        {
                                            // Resource found
                                            break;
                                        }

                                        if (nextChar != '\n')
                                        {
                                            // Check the next character and skip it if necessary.
                                            replaceStartIndex++;
                                        }
                                    }
                                    replaceStartIndex++;
                                    currentChar = defaultText[replaceStartIndex];
                                }
                                if (replaceStartIndex > replaceEndIndex)
                                {
                                    replaceStartIndex = replaceEndIndex;
                                }

                                int replaceLength = replaceEndIndex - replaceStartIndex;

                                if (replaceLength > 0)
                                {
                                    string[] biomesListItemNames = newValue != null ? newValue.Replace("\r", "").Split('\n') : null;
                                    string[] defaultBiomesListItemNames = worldConfig.GetPropertyValueAsString(property) != null ? worldConfig.GetPropertyValueAsString(property).Replace("\r", "").Split('\n') : null;
                                    List<string> newPropertyValue = new List<string>();

                                    if (biomesListItemNames != null)
                                    {
                                        foreach (string value1 in biomesListItemNames)
                                        {
                                            if (value1 != null && !string.IsNullOrEmpty(value1.Trim()))
                                            {
                                                bool duplicatePermission = value1.StartsWith("CustomObject(") || !newPropertyValue.Any(a => (string)a.Replace("\r", "").Replace("\n", "") == (string)value1.Replace("\r", "").Replace("\n", ""));
                                                if (duplicatePermission)
                                                {
                                                    if (value1.Contains('(') && value1.Contains(')'))
                                                    {
                                                        ResourceQueueItem selectedOption = null;
                                                        foreach (ResourceQueueItem option in versionConfig.ResourceQueueOptions)
                                                        {
                                                            if (value1.StartsWith(option.Name))
                                                            {
                                                                selectedOption = option;
                                                                break;
                                                            }
                                                        }
                                                        if (selectedOption != null)
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
                                    newValue = "";
                                    int i = 0;
                                    foreach (string value1 in newPropertyValue)
                                    {
                                        newValue += (i != newPropertyValue.Count() - 1 ? value1 + "\r\n" : value1);
                                        i++;
                                    }
                                }
                                if (replaceLength > 0)
                                {
                                    defaultText = defaultText.Remove(replaceStartIndex, replaceLength);
                                }

                                if (!String.IsNullOrWhiteSpace(newValue))
                                {
                                    defaultText = defaultText.Insert(replaceStartIndex, newValue + "\r\n");
                                }
                            } else {
                                //if (!property.Optional)
                                {
                                    //defaultText.Append("\r\n# This setting was added by OTGE and is most probably used by the Minecraft Worlds mod #\r\n");
                                    defaultText.Append(property.ScriptHandle + " " + newValue + "\r\n");

                                    //errorsTxt += "\r\nVersion config error: Setting \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                }
                            }
                        } else {
                            int valueStringStartIndex = sDefaultText.IndexOf(property.ScriptHandle);
                            while (valueStringStartIndex != -1 && !(valueStringStartIndex == 0 || defaultText.ToString(valueStringStartIndex - 1, defaultText.Length - (valueStringStartIndex - 1)).IndexOf("\n") == 0))
                            {
                                int valueStringStartIndex2 = defaultText.ToString(valueStringStartIndex + property.ScriptHandle.Length, defaultText.Length - (valueStringStartIndex + property.ScriptHandle.Length)).IndexOf(property.ScriptHandle);
                                if (valueStringStartIndex2 == -1)
                                {
                                    // No next
                                    valueStringStartIndex = -1;
                                    break;
                                }

                                // Try next
                                valueStringStartIndex += property.ScriptHandle.Length + valueStringStartIndex2;
                            }
                            if (valueStringStartIndex > -1)
                            {
                                int skipCharsLength = (property.ScriptHandle).Length;
                                int valueStringLength = 0;

                                while (defaultText[valueStringStartIndex + skipCharsLength + valueStringLength] != '\n')
                                {
                                    valueStringLength += 1;
                                }

                                if (property.PropertyType == "BiomesList" && worldConfig.GetPropertyMerge(property) && valueStringLength > 0)
                                {
                                    string[] biomesListItemNames = newValue != null ? newValue.Split(',') : null;
                                    string[] defaultBiomesListItemNames = worldConfigDefaultValues.GetPropertyValueAsString(property) != null ? worldConfigDefaultValues.GetPropertyValueAsString(property).Split(',') : null;
                                    List<string> newPropertyValue = new List<string>();
                                    if (biomesListItemNames != null)
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

                                    newValue = "";
                                    foreach (string value1 in newPropertyValue)
                                    {
                                        newValue += (value1 != newPropertyValue[newPropertyValue.Count() - 1] ? value1 + ", " : value1);
                                    }
                                }
                                if (valueStringLength > 0)
                                {
                                    defaultText = defaultText.Remove(valueStringStartIndex + skipCharsLength, valueStringLength);
                                }
                                defaultText = defaultText.Insert(valueStringStartIndex + skipCharsLength, " " + newValue);
                            } else {
                                //if (!property.Optional)
                                {
                                    defaultText.Append(property.ScriptHandle + " " + newValue + "\r\n");

                                    //errorsTxt += "\r\nVersion config error: Setting \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                }
                            }
                        }
                    }
                }

                if (errorsTxt.Length > 0)
                {
                    PopUpForm.CustomMessageBox(errorsTxt, "Version config warnings");
                }

                System.IO.FileInfo newWorldConfig = new System.IO.FileInfo(destinationConfigsDir + "\\WorldConfig.ini");
                if (newWorldConfig.Exists)
                {
                    newWorldConfig.Delete();
                    newWorldConfig.Refresh();
                }
                string fName = newWorldConfig.FullName;
                System.IO.File.WriteAllText(fName, defaultText.ToString());
            }
        }
    }
}
