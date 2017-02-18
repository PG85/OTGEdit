using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TCEE.XML;
using System.Windows.Forms;

namespace TCEE
{
    public static class Biomes
    {
        public static BiomeConfig LoadBiomeConfigFromFile(FileInfo file, VersionConfig versionConfig, bool loadComments)
        {
            BiomeConfig bDefaultConfig = new BiomeConfig(versionConfig)
            {
                FileName = file.Name
            };

            string sDefaultText = System.IO.File.ReadAllText(file.FullName);
            foreach (TCProperty property in versionConfig.BiomeConfig)
            {
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
                            if (property.PropertyType == "BiomesList")
                            {
                                comment += "\r\n\r\nUse CTRL or SHIFT + Click to select one or multiple biomes from the list.";
                            }
                            if (loadComments && !String.IsNullOrEmpty(comment))
                            {
                                Session.ToolTip1.SetToolTip(Session.BiomeSettingsInputs[property].Item4, comment);
                            }

                            propertyValue = sDefaultText.Substring(replaceStartIndex, replaceLength).Trim();
                            bDefaultConfig.SetProperty(property, propertyValue, false, false);
                        }
                        else
                        {
                            throw new Exception("Property value for property " + property.Name + " could not be read from file " + file.Name + ". There is either an error in the file or TCEE needs to be updated to support the value's formatting.");
                        }
                    }
                    else
                    {
                        throw new Exception("ScriptHandle for property \"" + property.Name + "\" could not be found in biome file " + file.Name);
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
                            if (loadComments && !String.IsNullOrEmpty(comment))
                            {
                                Session.ToolTip1.SetToolTip(Session.BiomeSettingsInputs[property].Item4, comment);
                            }

                            int skipCharsLength = (property.ScriptHandle).Length;
                            int valueStringLength = 0;
                            //float originalValue = 0;

                            try
                            {
                                while (sDefaultText.Substring(valueStringStartIndex + skipCharsLength + valueStringLength, 1) != "\n")
                                {
                                    valueStringLength += 1;
                                }
                                propertyValue = sDefaultText.Substring(valueStringStartIndex + skipCharsLength, valueStringLength).Trim();
                                bDefaultConfig.SetProperty(property, propertyValue, false, false);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Property value for property " + property.Name + " could not be read from file " + file.Name + ". There is either an error in the file or TCEE needs to be updated to support the value's formatting. Ex: " + ex.Message);
                            }
                        }
                        else
                        {
                            throw new Exception("Property value for property " + property.Name + " could not be read from file " + file.Name + ". There is either an error in the file or TCEE needs to be updated to support the value's formatting.");
                        }
                    }
                    else
                    {

                        if (loadComments)
                        {
                            Session.ToolTip1.SetToolTip(Session.BiomeSettingsInputs[property].Item4, "No tooltip available, comments for this setting are missing in source files.");
                        }
                        bDefaultConfig.SetProperty(property, "", false, false);

                        //TODO: solve ReplaceToBiomeName problem!
                        if (property.Name != "ReplaceToBiomeName" && !property.Optional)// && property.Name != "BiomeSizeWhenIsle" && property.Name != "BiomeRarityWhenIsle" && property.Name != "BiomeSizeWhenBorder")
                        {
                            MessageBox.Show("ScriptHandle for property \"" + property.Name + "\" could not be found in biome file " + file.Name, "Error reading configuration files");
                            MessageBox.Show("The files you are trying to import have caused an error. They were probably not generated by the selected version of TC or MCW. MCW and TC are backwards compatible though, so for instance you can use TC2.6.3 settings (MCW 1.0.5. and lower) in TC2.7.2 (MCW 1.0.6. or higher). When TC or MCW detects old world and biomeconfigs it should update the files automatically. You can then import the generated world into TCEE using the new version.", "Error reading configuration files");
                            //throw new InvalidDataException("ScriptHandle for property \"" + property.Name + "\" could not be found in biome file " + file.Name);
                        }
                    }
                }
            }
            return bDefaultConfig;
        }

        public static void GenerateBiomeConfigs(System.IO.DirectoryInfo sourceDirectory, System.IO.DirectoryInfo destinationDirectory, List<BiomeConfig> biomeConfigsDefaultValues, VersionConfig versionConfig)
        {
            if (!destinationDirectory.Exists) { destinationDirectory.Create(); }
            if (sourceDirectory.Exists)
            {
                foreach (FileInfo file in destinationDirectory.GetFiles())
                {
                    file.Delete();
                }
                string errorsTxt = "";
                foreach (System.IO.FileInfo file in sourceDirectory.GetFiles())
                {
                    BiomeConfig defaultBiome = biomeConfigsDefaultValues.FirstOrDefault(a => a.FileName == file.Name);
                    if (defaultBiome != null)
                    {
                        StringBuilder defaultText = new StringBuilder(System.IO.File.ReadAllText(file.FullName));
                        string sDefaultText = defaultText.ToString();
                        foreach (TCProperty property in versionConfig.BiomeConfig)
                        {
                            List<string> valuesPerBiomeGroup = new List<string>();

                            string newValue = "";
                            string aggregateValue = "";
                            string defaultValue = "";

                            if (property.PropertyType == "BiomesList" || property.PropertyType == "ResourceQueue")
                            {
                                if (!Session.BiomeGroups.Any(a => a.Biomes.Any(b => b == defaultBiome.BiomeName) && a.BiomeConfig.Properties.First(c => c.PropertyName == property.Name).Override))
                                {
                                    //Never overriden, set to default value
                                    defaultValue = defaultBiome.GetPropertyValueAsString(property);
                                }
                            }
                            else
                            {
                                defaultValue = defaultBiome.GetPropertyValueAsString(property);
                            }

                            newValue += defaultValue;
                            aggregateValue += defaultValue;
                            if (!String.IsNullOrEmpty(defaultValue))
                            {
                                valuesPerBiomeGroup.Add(defaultValue);
                            }

                            bool bOverride = false;
                            bool bOverrideParentvalues = false;
                            bool bMerge = false;

                            string lastBiomeGroupNameToOverrideParentValues = null;

                            foreach (string biomeGroupName in Session.lbGroups.Items)
                            {
                                Group biomeGroup = Session.BiomeGroups.FirstOrDefault(a => a.Name.Equals(biomeGroupName));
                                if (biomeGroup != null)
                                {
                                    string groupvalue = "";
                                    if (biomeGroup.Biomes.Any(a => a == defaultBiome.BiomeName))
                                    {
                                        if (biomeGroup.BiomeConfig.Properties.Any(a => a.PropertyName == property.Name) && biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override)
                                        {
                                            if (biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).OverrideParentValues)
                                            {
                                                lastBiomeGroupNameToOverrideParentValues = biomeGroupName;
                                            }
                                        }
                                    }
                                }
                            }

                            // Make sure that the biome groups are processed in the correct order
                            foreach (string biomeGroupName in Session.lbGroups.Items)
                            {
                                if (lastBiomeGroupNameToOverrideParentValues == null || lastBiomeGroupNameToOverrideParentValues.Equals(biomeGroupName))
                                {
                                    Group biomeGroup = Session.BiomeGroups.FirstOrDefault(a => a.Name.Equals(biomeGroupName));
                                    if (biomeGroup != null)
                                    {
                                        string groupvalue = "";
                                        if (biomeGroup.Biomes.Any(a => a == defaultBiome.BiomeName))
                                        {
                                            if (biomeGroup.BiomeConfig.Properties.Any(a => a.PropertyName == property.Name) && biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override)
                                            {
                                                if (!biomeGroup.BiomeConfig.GetPropertyMerge(property))
                                                {
                                                    bOverride = true;
                                                }

                                                bMerge = biomeGroup.BiomeConfig.GetPropertyMerge(property);
                                                bOverrideParentvalues = biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).OverrideParentValues;

                                                if (biomeGroup.BiomeConfig.GetPropertyOverrideParentValues(property) || (property.PropertyType != "BiomesList" && property.PropertyType != "ResourceQueue"))
                                                {
                                                    if (!String.IsNullOrEmpty(biomeGroup.BiomeConfig.GetPropertyValueAsString(property)) || property.PropertyType == "String" || property.PropertyType == "BiomesList" || property.PropertyType == "ResourceQueue")
                                                    {
                                                        groupvalue = biomeGroup.BiomeConfig.GetPropertyValueAsString(property);
                                                        aggregateValue = biomeGroup.BiomeConfig.GetPropertyValueAsString(property);
                                                    }
                                                }
                                                else if (property.PropertyType == "BiomesList" || property.PropertyType == "ResourceQueue")
                                                {
                                                    if (property.PropertyType == "BiomesList" && biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override)
                                                    {
                                                        if (groupvalue != null && !String.IsNullOrEmpty(groupvalue.Trim()))
                                                        {
                                                            groupvalue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? ", " + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                        }
                                                        else
                                                        {
                                                            groupvalue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                        }

                                                        if (aggregateValue != null && !String.IsNullOrEmpty(aggregateValue.Trim()))
                                                        {
                                                            aggregateValue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? ", " + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                        }
                                                        else
                                                        {
                                                            aggregateValue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                        }
                                                    }
                                                    else if (property.PropertyType == "ResourceQueue" && biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override)
                                                    {
                                                        if (groupvalue != null && !String.IsNullOrEmpty(groupvalue.Trim()))
                                                        {
                                                            groupvalue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? "\r\n" + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                        }
                                                        else
                                                        {
                                                            groupvalue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                        }

                                                        if (aggregateValue != null && !String.IsNullOrEmpty(aggregateValue.Trim()))
                                                        {
                                                            aggregateValue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? "\r\n" + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                        }
                                                        else
                                                        {
                                                            aggregateValue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                        }

                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("One does not simply merge a non-BiomesList, non-ResourceQueue property. Property \"" + property.Name + "\" group \"" + biomeGroup.Name + "\". Biome generation aborted.", "Generation error");
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        if (!String.IsNullOrEmpty(groupvalue))
                                        {
                                            valuesPerBiomeGroup.Add(groupvalue);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Biome Group data could not be found. This is not supposed to happen! Please contact the developer with a bug report.");
                                    }
                                }
                            }

                            bMerge = (!bOverride || bOverrideParentvalues) && bMerge;

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
                                            string[] defaultBiomesListItemNames = defaultBiome.GetPropertyValueAsString(property) != null ? defaultBiome.GetPropertyValueAsString(property).Replace("\r", "").Split('\n') : null;
                                            List<string> newPropertyValue = new List<string>();
                                            //List<string> defaultPropertyValue = new List<string>();

                                            //string finalBiomeGroupValue = newValue;                                                    
                                            if (defaultBiomesListItemNames != null && bMerge)
                                            {
                                                foreach (string value1 in defaultBiomesListItemNames)
                                                {
                                                    if (value1 != null && !string.IsNullOrEmpty(value1.Trim()))
                                                    {
                                                        //bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)value1) || MessageBox.Show("An item with the same value already exists, are you sure you want to add another?", "Allow duplicate?", MessageBoxButtons.OKCancel) == DialogResult.OK;
                                                        bool duplicatePermission = value1.StartsWith("CustomObject(") || !newPropertyValue.Any(a => (string)a.Replace("\r", "").Replace("\n", "") == (string)value1.Replace("\r", "").Replace("\n", ""));
                                                        //bool duplicatePermission = true;
                                                        if (duplicatePermission)
                                                        {
                                                            if (value1.Contains('(') && value1.Contains(')'))
                                                            {
                                                                bool bFound3 = false;
                                                                ResourceQueueItem selectedOption = null;
                                                                foreach (ResourceQueueItem option in versionConfig.ResourceQueueOptions)
                                                                {
                                                                    if (value1.StartsWith(option.Name))
                                                                    {
                                                                        bFound3 = true;
                                                                        selectedOption = option;
                                                                        break;
                                                                    }
                                                                }
                                                                if (bFound3)
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
                                                                                bool bFound4 = false;
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
                                                                                                    bFound4 = true;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (parameters[selectedOption.UniqueParameterIndex].Trim() == parameters2[selectedOption.UniqueParameterIndex].Trim())
                                                                                            {
                                                                                                bFound4 = true;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                if (!bFound4)
                                                                                {
                                                                                    newPropertyValue.Add(value1.Trim());
                                                                                }
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

                                            foreach (string biomeGroupValue in valuesPerBiomeGroup)
                                            {
                                                string[] biomesListItemNames = biomeGroupValue != null ? biomeGroupValue.Replace("\r", "").Split('\n') : null;
                                                List<string> newGroupPropertyValue = new List<string>();

                                                if (biomesListItemNames != null)
                                                {
                                                    foreach (string value1 in biomesListItemNames)
                                                    {
                                                        if (value1 != null && !string.IsNullOrEmpty(value1.Trim()))
                                                        {
                                                            //bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)value1) || MessageBox.Show("An item with the same value already exists, are you sure you want to add another?", "Allow duplicate?", MessageBoxButtons.OKCancel) == DialogResult.OK;
                                                            //bool duplicatePermission = value1.StartsWith("CustomObject(") || !newPropertyValue.Any(a => (string)a == (string)value1);
                                                            //bool duplicatePermission = !newPropertyValue.Any(a => (string)a == (string)value1);
                                                            bool duplicatePermission = true;
                                                            if (duplicatePermission)
                                                            {
                                                                if (value1.Contains('(') && value1.Contains(')'))
                                                                {
                                                                    bool bFound3 = false;
                                                                    ResourceQueueItem selectedOption = null;
                                                                    foreach (ResourceQueueItem option in versionConfig.ResourceQueueOptions)
                                                                    {
                                                                        if (value1.StartsWith(option.Name))
                                                                        {
                                                                            bFound3 = true;
                                                                            selectedOption = option;
                                                                            break;
                                                                        }
                                                                    }
                                                                    if (bFound3)
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
                                                                                    newGroupPropertyValue.Add(value1.Trim());
                                                                                }
                                                                                else
                                                                                {
                                                                                    newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name));
                                                                                    newGroupPropertyValue.Add(value1.Trim());
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                newGroupPropertyValue.Add(value1.Trim());
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            newGroupPropertyValue.Add(value1.Trim());
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                newPropertyValue.AddRange(newGroupPropertyValue);
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
                                        defaultText = defaultText.Insert(replaceStartIndex, newValue + "\r\n");
                                        sDefaultText = defaultText.ToString();
                                    }
                                    else
                                    {

                                        //if (!property.Optional)
                                        {
                                            defaultText.Append(property.ScriptHandle + " " + newValue + "\r\n");

                                            //errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                        }

                                        //MessageBox.Show("Version config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                        //return;
                                    }
                                }
                                else
                                {

                                    //if (!property.Optional)
                                    {
                                        defaultText.Append(property.ScriptHandle + " " + newValue + "\r\n");

                                        //errorsTxt += "\r\nVersion config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                    }
                                    //MessageBox.Show("Version config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                    //return;
                                }
                            }
                            else
                            {

                                newValue = aggregateValue;

                                bool bFound = false;
                                int valueStringStartIndex = sDefaultText.ToLower().IndexOf(property.ScriptHandle.ToLower());
                                if (valueStringStartIndex > -1)
                                {
                                    while (!bFound)
                                    {
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
                                        int skipCharsLength = (property.ScriptHandle).Length;
                                        int valueStringLength = 0;

                                        while (sDefaultText.Substring(valueStringStartIndex + skipCharsLength + valueStringLength, 1) != "\n")
                                        {
                                            valueStringLength += 1;
                                        }

                                        if (property.PropertyType == "BiomesList" && bMerge && valueStringLength > 0)
                                        {
                                            string[] biomesListItemNames = newValue != null ? newValue.Split(',') : null;
                                            string[] defaultBiomesListItemNames = defaultBiome.GetPropertyValueAsString(property) != null ? defaultBiome.GetPropertyValueAsString(property).Split(',') : null;
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
                                        sDefaultText = defaultText.ToString();
                                    }
                                    else
                                    {

                                        //if(!property.Optional)
                                        {
                                            defaultText.Append(property.ScriptHandle + " " + newValue + "\r\n");

                                            //errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                        }

                                        //MessageBox.Show("Version config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                        //return;
                                    }
                                }
                                else
                                {
                                    //TODO: solve ReplaceToBiomeName problem!
                                    //if (property.Name != "ReplaceToBiomeName")// && property.Name != "BiomeSizeWhenIsle" && property.Name != "BiomeRarityWhenIsle" && property.Name != "BiomeSizeWhenBorder")
                                    //{
                                    //MessageBox.Show("Version config error: Handle for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                    //return;
                                    //}

                                    //if (!property.Optional)
                                    {
                                        defaultText.Append(property.ScriptHandle + " " + newValue + "\r\n");

                                        //errorsTxt += "\r\nVersion config error: The value for property \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                    }
                                }
                            }
                        }

                        string fName = destinationDirectory.FullName + "/" + file.Name;
                        System.IO.File.WriteAllText(fName, defaultText.ToString());
                    }
                }
                if (errorsTxt.Length > 0)
                {
                    MessageBox.Show(errorsTxt, "Version config warnings");
                }
            }
        }
    }
}
