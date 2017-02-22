using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCEE.XML;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace TCEE
{
    public static class World
    {
        public static WorldConfig LoadWorldConfigFromFile(FileInfo file, VersionConfig versionConfig, bool loadUI) // TODO: remove loadui param and load UI a different method! (this method used to be "loadWorldConfigDefaults")
        {
            WorldConfig worldConfig = new WorldConfig(versionConfig);

            if (file.Exists)
            {
                if (loadUI)
                {
                    Session.tabControl1.Visible = true;
                    Session.btSave.Enabled = true;
                    Session.btLoad.Enabled = true;
                    Session.btGenerate.Visible = true;
                    Session.btCopyBO3s.Visible = true;
                    Session.cbDeleteRegion.Visible = true;

                    Session.rbSummerSkin.Visible = true;
                    Session.rbWinterSkin.Visible = true;

                    //label4.Visible = true;
                    //label5.Visible = true;

                    Session.Form1.AutoSize = false;
                    Session.Form1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;

                    //Width = 1110;
                    Session.Form1.Height = 743;
                }

                string txtErrors = "";

                string sDefaultText = System.IO.File.ReadAllText(file.FullName);
                foreach (TCProperty property in versionConfig.WorldConfig)
                {
                    if (!loadUI || Session.WorldSettingsInputs.ContainsKey(property))
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
                                    }
                                    else
                                    {
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
                                                    }
                                                    else
                                                    {
                                                        lastNoHashLineBeforeComment = arg;
                                                    }
                                                }
                                                else
                                                {
                                                    lastNoHashLineBeforeComment = -1;
                                                    break;
                                                }
                                            }
                                            else
                                            {
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
                                        Session.ToolTip1.SetToolTip(Session.WorldSettingsInputs[property].Item4, comment);
                                    }

                                    propertyValue = sDefaultText.Substring(replaceStartIndex, replaceLength).Trim();

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
                                    }
                                    else
                                    {
                                        worldConfig.SetProperty(property, propertyValue, false, false);
                                    }
                                }
                                else
                                {
                                    txtErrors += "\r\nProperty value for property " + property.Name + " could not be read from world configuration file. There is either an error in the file or TCEE needs to be updated to support the value's formatting.";
                                    //throw new Exception("Property value for property " + property.Name + " could not be read from world configuration file. There is either an error in the file or TCEE needs to be updated to support the value's formatting.");
                                }
                            }
                            else
                            {
                                txtErrors += "\r\nScriptHandle for property \"" + property.Name + "\" could not be found in world configuration file.";
                                if (!property.Optional)
                                {
                                    MessageBox.Show(txtErrors, "Version warnings");
                                    MessageBox.Show(
                                        "The files you are trying to import have caused an error. They were probably not generated by the selected version of TC or MCW. Try importing using a different version. MCW and TC are backwards compatible, so for instance you can use TC2.6.3 settings (MCW 1.0.5. and lower) in TC2.7.2 (MCW 1.0.6. or higher). When TC or MCW detects old world and biomeconfigs it should update the files automatically. You can then import the generated world into TCEE using the new version.",
                                        "Error reading configuration files"
                                    );
                                    return null;

                                }
                                //throw new Exception();
                            }
                        }
                        else
                        {
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
                                    }
                                    else
                                    {
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
                                                    }
                                                    else
                                                    {
                                                        lastNoHashLineBeforeComment = arg;
                                                    }
                                                }
                                                else
                                                {
                                                    lastNoHashLineBeforeComment = -1;
                                                    break;
                                                }
                                            }
                                            else
                                            {
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
                                        Session.ToolTip1.SetToolTip(Session.WorldSettingsInputs[property].Item4, comment);
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
                                                    ((ComboBox)Session.WorldSettingsInputs[property].Item1).SelectedIndex = 1;
                                                    Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                }
                                                else if (propertyValue.ToLower() == "false")
                                                {
                                                    ((ComboBox)Session.WorldSettingsInputs[property].Item1).SelectedIndex = 2;
                                                    Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                }
                                                else
                                                {
                                                    ((ComboBox)Session.WorldSettingsInputs[property].Item1).SelectedIndex = 0;
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
                                                    }
                                                    else
                                                    {
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
                                                Session.WorldSettingsInputs[property].Item1.Text = propertyValue;
                                                Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                break;
                                            case "String":
                                                Session.WorldSettingsInputs[property].Item1.Text = propertyValue;
                                                Session.WorldSettingsInputs[property].Item2.Checked = false;
                                                break;
                                        }
                                        worldConfig.SetProperty(property, propertyValue, false, false);
                                        Session.IgnoreOverrideCheckChangedWorld = false;
                                        Session.IgnorePropertyInputChangedWorld = false;
                                    }
                                    else
                                    {
                                        worldConfig.SetProperty(property, propertyValue, false, false);
                                    }
                                }
                                else
                                {
                                    txtErrors += "\r\nProperty value for property " + property.Name + " could not be read from world configuration file. There is either an error in the file or TCEE needs to be updated to support the value's formatting.";
                                    //throw new Exception("Property value for property " + property.Name + " could not be read from world configuration file. There is either an error in the file or TCEE needs to be updated to support the value's formatting.");
                                }
                            }
                            else
                            {
                                txtErrors += "\r\nScriptHandle for property \"" + property.Name + "\" could not be found in world configuration file.";
                                if (!property.Optional)
                                {
                                    MessageBox.Show(txtErrors, "Version warnings");                                    
                                    MessageBox.Show(
                                        "The files you are trying to import have caused an error. They were probably not generated by the selected version of TC or MCW. Try importing using a different version. MCW and TC are backwards compatible, so for instance you can use TC2.6.3 settings (MCW 1.0.5. and lower) in TC2.7.2 (MCW 1.0.6. or higher). When TC or MCW detects old world and biomeconfigs it should update the files automatically. You can then import the generated world into TCEE using the new version.", 
                                        "Error reading configuration files"                                    
                                    );
                                    return null;
                                }

                                //throw new Exception("ScriptHandle for property \"" + property.Name + "\" could not be found in world configuration file");
                            }
                        }
                    }
                }
                if (txtErrors.Length > 0)
                {
                    MessageBox.Show(txtErrors, "Version warnings");
                }
            }
            return worldConfig;
        }

        public static void ConfigWorld()
        {
            System.IO.DirectoryInfo DefaultWorldDirectory = new System.IO.DirectoryInfo(Session.SourceConfigsDir);
            System.IO.FileInfo defaultWorldConfig = new System.IO.FileInfo(DefaultWorldDirectory + "/WorldConfig.ini");

            if (defaultWorldConfig.Exists)
            {
                StringBuilder defaultText = new StringBuilder(System.IO.File.ReadAllText(defaultWorldConfig.FullName));
                string sDefaultText = defaultText.ToString();
                string errorsTxt = "";
                foreach (TCProperty property in Session.VersionConfig.WorldConfig)
                {
                    string propertyValue = Session.WorldConfig1.GetPropertyValueAsString(property);
                    if (propertyValue != null && Session.WorldConfigDefaultValues != null && (propertyValue == Session.WorldConfigDefaultValues.GetPropertyValueAsString(property) || (property.PropertyType.Equals("BiomesList") && Utils.TCSettingsUtils.CompareBiomeLists(propertyValue, Session.WorldConfigDefaultValues.GetPropertyValueAsString(property))) || (property.PropertyType.Equals("ResourceQueue") && Utils.TCSettingsUtils.CompareResourceQueues(propertyValue, Session.WorldConfigDefaultValues.GetPropertyValueAsString(property)))))
                    {
                        propertyValue = null;
                    }
                    if (propertyValue != null && Session.WorldConfig1.Properties.First(a => a.PropertyName == property.Name).Override)
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
                                    }
                                    else
                                    {
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
                                        string[] defaultBiomesListItemNames = Session.WorldConfig1.GetPropertyValueAsString(property) != null ? Session.WorldConfig1.GetPropertyValueAsString(property).Replace("\r", "").Split('\n') : null;
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
                                                            foreach (ResourceQueueItem option in Session.VersionConfig.ResourceQueueOptions)
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
                                                                        }
                                                                        else
                                                                        {
                                                                            newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name));
                                                                            newPropertyValue.Add(value1.Trim());
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        newPropertyValue.Add(value1.Trim());
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    newPropertyValue.Add(value1.Trim());
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
                                }
                                else
                                {
                                    //if (!property.Optional)
                                    {
                                        //defaultText.Append("\r\n# This property was added by TCEE and is most probably used by the Minecraft Worlds mod #\r\n");
                                        defaultText.Append(property.ScriptHandle + " " + propertyValue + "\r\n");

                                        //errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                    }
                                }
                            }
                            else
                            {
                                //if (!property.Optional)
                                {
                                    //defaultText.Append("\r\n# This property was added by TCEE and is most probably used by the Minecraft Worlds mod #\r\n");
                                    defaultText.Append(property.ScriptHandle + " " + propertyValue + "\r\n");

                                    //errorsTxt += "\r\nVersion config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                }
                            }
                        }
                        else
                        {
                            bool bFound = false;
                            int replaceStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                            if (replaceStartIndex > -1)
                            {
                                while (!bFound)
                                {
                                    if (sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("\n") > sDefaultText.Substring(0, replaceStartIndex).LastIndexOf("#"))
                                    {
                                        bFound = true;
                                    }
                                    else
                                    {
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

                                    if (property.PropertyType == "BiomesList" && Session.WorldConfig1.GetPropertyMerge(property))
                                    {
                                        if (replaceLength > 0)
                                        {
                                            string[] biomesListItemNames = propertyValue != null ? propertyValue.Split(',') : null;
                                            string originalValue = defaultText.ToString().Substring(replaceStartIndex + skipCharsLength, replaceLength);
                                            string[] originalbiomesListItemNames = originalValue != null ? originalValue.Split(',') : null;
                                            List<string> newPropertyValue = new List<string>();
                                            foreach (string value in biomesListItemNames)
                                            {
                                                if (newPropertyValue.Any(a => (string)a == (string)value))
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
                                }
                                else
                                {
                                    //if (!property.Optional)
                                    {
                                        defaultText.Append(property.ScriptHandle + " " + propertyValue + "\r\n");

                                        //errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                    }
                                }
                            }
                            else
                            {
                                //if (!property.Optional)
                                {
                                    defaultText.Append(property.ScriptHandle + " " + propertyValue + "\r\n");

                                    //errorsTxt += "\r\nVersion config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + defaultWorldConfig.FullName + "\". Added it to the end of the file.";
                                }
                            }
                        }
                    }
                }

                if (errorsTxt.Length > 0)
                {
                    MessageBox.Show(errorsTxt, "Version config warnings");
                }

                System.IO.FileInfo newWorldConfig = new System.IO.FileInfo(Session.DestinationConfigsDir + "/WorldConfig.ini");
                if (newWorldConfig.Exists)
                {
                    newWorldConfig.Delete();
                }
                string fName = newWorldConfig.FullName;
                System.IO.File.WriteAllText(fName, defaultText.ToString());
            }
        }
    }
}
