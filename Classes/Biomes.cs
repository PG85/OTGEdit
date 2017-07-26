using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OTGE.XML;
using System.Windows.Forms;
using OTGE.Utils;

namespace OTGE
{
    public static class Biomes
    {
        public static BiomeConfig LoadBiomeConfigFromFile(FileInfo file, VersionConfig versionConfig, bool loadComments, ref string txtErrorsWrongValue, ref string txtErrorsNoSetting)
        {
            BiomeConfig biomeConfig = new BiomeConfig(versionConfig)
            {
                FileName = file.Name
            };

            StringBuilder defaultText = new StringBuilder(System.IO.File.ReadAllText(file.FullName));
            string sDefaultText = defaultText.ToString();
            foreach (TCProperty property in versionConfig.BiomeConfig)
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

                            if (replaceStartIndex >= defaultText.Length - 1)
                            {
                                replaceStartIndex = replaceEndIndex;
                                break;
                            }
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
                        if (loadComments)
                        {
                            if(!String.IsNullOrEmpty(comment))
                            {
                                Session.ToolTip1.SetToolTip(Session.BiomeSettingsInputs[property].Item4, Utils.Misc.FormatToolTipText(comment + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue)));
                            } else {
                                Session.ToolTip1.SetToolTip(Session.BiomeSettingsInputs[property].Item4, "No description." + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue));
                            }
                        }

                        if (replaceLength > 0)
                        {
                            propertyValue = defaultText.ToString(replaceStartIndex, replaceLength).Trim();
                        }

                        biomeConfig.SetProperty(property, propertyValue, false, false);
                    } else {
                        txtErrorsNoSetting += "\r\nSetting \"" + property.Name + "\" could not be found in file " + file.Name;
                        if (!property.Optional)
                        {
                            PopUpForm.CustomMessageBox(txtErrorsNoSetting, "Version warnings");
                            PopUpForm.CustomMessageBox("The file " + file.Name + " contains critical errors, it was probably not made for use with the selected version of TC/MCW/OTG/OTG+ and requires manual updating.", "Error reading configuration file");
                            return null;
                        }
                        //throw new Exception("Setting \"" + property.Name + "\" could not be found in file " + file.Name);
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
                        if (loadComments)
                        {
                            if(!String.IsNullOrEmpty(comment))
                            {
                                Session.ToolTip1.SetToolTip(Session.BiomeSettingsInputs[property].Item4, Utils.Misc.FormatToolTipText(comment + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue)));
                            } else {
                                Session.ToolTip1.SetToolTip(Session.BiomeSettingsInputs[property].Item4, "No description." + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue));
                            }
                        }

                        int skipCharsLength = (property.ScriptHandle).Length;
                        int valueStringLength = 0;

                        if (valueStringStartIndex + skipCharsLength + valueStringLength < defaultText.Length)
                        {
                            while (defaultText[valueStringStartIndex + skipCharsLength + valueStringLength] != '\n')
                            {
                                valueStringLength += 1;

                                if (valueStringStartIndex + skipCharsLength + valueStringLength >= defaultText.Length)
                                {
                                    valueStringLength = 0;
                                    break;
                                }
                            }
                        }
                        if (valueStringLength > 0)
                        {
                            propertyValue = defaultText.ToString(valueStringStartIndex + skipCharsLength, valueStringLength).Trim();
                        }

                        try
                        {
                            biomeConfig.SetProperty(property, propertyValue, false, false);
                        }
                        catch (Exception ex)
                        {
                            if(ex is ArgumentNullException || ex is InvalidOperationException)
                            {
                                txtErrorsWrongValue += "\r\nValue for setting " + property.Name + " could not be read from file " + file.Name + ". There is either an error in the file or OTGE needs to be updated to support the value's formatting.";
                                //throw new Exception("Value for setting " + property.Name + " could not be read from file " + file.Name + ". There is either an error in the file or OTGE needs to be updated to support the value's formatting. Ex: " + ex.Message);
                            } else {
                                throw;
                            }
                        }
                    } else {

                        if (loadComments)
                        {
                            Session.ToolTip1.SetToolTip(Session.BiomeSettingsInputs[property].Item4, "No description." + (property.PropertyType != "Float" && property.PropertyType != "Int" ? "" : "\r\n\r\nMin value: " + property.MinValue + "\r\nMax value: " + property.MaxValue));
                        }
                        biomeConfig.SetProperty(property, "", false, false);

                        //TODO: Solve ReplaceToBiomeName problem: ReplaceToBiome is an optional setting but is required for virtual biomes
                        if (property.Name != "ReplaceToBiomeName")
                        {
                            txtErrorsNoSetting += "\r\nSetting \"" + property.Name + "\" could not be found in file " + file.Name + ".";
                            if (!property.Optional)// && property.Name != "BiomeSizeWhenIsle" && property.Name != "BiomeRarityWhenIsle" && property.Name != "BiomeSizeWhenBorder")
                            {
                                PopUpForm.CustomMessageBox(txtErrorsNoSetting, "Version warnings");
                                PopUpForm.CustomMessageBox("The files you are importing contain critical errors, they were probably not made for use with the selected version of TC/MCW/OTG/OTG+ and require manual updating.", "Error reading configuration files");
                                return null;

                                //throw new InvalidDataException("Setting \"" + property.Name + "\" could not be found in file " + file.Name);
                            }
                        }
                    }
                }
            }
            return biomeConfig;
        }

        public static void GenerateBiomeConfigs(System.IO.DirectoryInfo sourceDirectory, System.IO.DirectoryInfo destinationDirectory, List<BiomeConfig> biomeConfigsDefaultValues, VersionConfig versionConfig, bool ignoreOverrideCheck)
        {
            if (!destinationDirectory.Exists)
            {
                destinationDirectory.Create();
                destinationDirectory.Refresh();
            }
            if (sourceDirectory.Exists)
            {
                string errorsTxt = "";
                foreach (System.IO.FileInfo file in sourceDirectory.GetFiles())
                {
                    BiomeConfig defaultBiome = biomeConfigsDefaultValues.FirstOrDefault(a => a.FileName == file.Name);
                    if (defaultBiome != null)
                    {
                        StringBuilder defaultText = new StringBuilder(System.IO.File.ReadAllText(file.FullName));
                        foreach (TCProperty property in versionConfig.BiomeConfig)
                        {
                            string sDefaultText = defaultText.ToString();

                            List<string> valuesPerBiomeGroup = new List<string>();

                            string newValue = "";
                            string aggregateValue = "";
                            string defaultValue = "";

                            if (property.PropertyType == "BiomesList" || property.PropertyType == "ResourceQueue")
                            {
                                if (
                                    !Session.BiomeGroups.Any(a => 
                                        a.Biomes.Any(b => b == defaultBiome.BiomeName) && 
                                        (
                                            ignoreOverrideCheck ||
                                            a.BiomeConfig.Properties.First(c => c.PropertyName == property.Name).Override
                                        )
                                    )
                                )
                                {
                                    //Never overriden, set to default value
                                    defaultValue = defaultBiome.GetPropertyValueAsString(property);
                                }
                            } else {
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
                                    if (biomeGroup.Biomes.Any(a => a == defaultBiome.BiomeName))
                                    {
                                        if (
                                            biomeGroup.BiomeConfig.Properties.Any(a => a.PropertyName == property.Name) && 
                                            (
                                                ignoreOverrideCheck ||
                                                biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override
                                            )
                                        )
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
                                            if (
                                                biomeGroup.BiomeConfig.Properties.Any(a => a.PropertyName == property.Name) && 
                                                (
                                                    ignoreOverrideCheck ||
                                                    biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override
                                                )
                                            )
                                            {
                                                if (!biomeGroup.BiomeConfig.GetPropertyMerge(property))
                                                {
                                                    bOverride = true;
                                                }

                                                bMerge = biomeGroup.BiomeConfig.GetPropertyMerge(property);
                                                bOverrideParentvalues = biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).OverrideParentValues;

                                                if (biomeGroup.BiomeConfig.GetPropertyOverrideParentValues(property) || (property.PropertyType != "BiomesList" && property.PropertyType != "ResourceQueue"))
                                                {
                                                    if (!String.IsNullOrEmpty(biomeGroup.BiomeConfig.GetPropertyValueAsString(property)) || property.PropertyType == "String" || property.PropertyType == "BigString"  || property.PropertyType == "BiomesList" || property.PropertyType == "ResourceQueue")
                                                    {
                                                        groupvalue = biomeGroup.BiomeConfig.GetPropertyValueAsString(property);
                                                        aggregateValue = biomeGroup.BiomeConfig.GetPropertyValueAsString(property);
                                                    }
                                                }
                                                else if (property.PropertyType == "BiomesList" || property.PropertyType == "ResourceQueue")
                                                {
                                                    if (
                                                        property.PropertyType == "BiomesList" && 
                                                        (
                                                            ignoreOverrideCheck ||
                                                            biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override
                                                        )
                                                    )
                                                    {
                                                        if (groupvalue != null && !String.IsNullOrEmpty(groupvalue.Trim()))
                                                        {
                                                            groupvalue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? ", " + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                        } else {
                                                            groupvalue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                        }

                                                        if (aggregateValue != null && !String.IsNullOrEmpty(aggregateValue.Trim()))
                                                        {
                                                            aggregateValue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? ", " + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                        } else {
                                                            aggregateValue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                        }
                                                    }
                                                    else if (
                                                        property.PropertyType == "ResourceQueue" && 
                                                        (
                                                            ignoreOverrideCheck ||
                                                            biomeGroup.BiomeConfig.Properties.First(a => a.PropertyName == property.Name).Override
                                                        )
                                                    )
                                                    {
                                                        if (groupvalue != null && !String.IsNullOrEmpty(groupvalue.Trim()))
                                                        {
                                                            groupvalue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? "\r\n" + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                        } else {
                                                            groupvalue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                        }

                                                        if (aggregateValue != null && !String.IsNullOrEmpty(aggregateValue.Trim()))
                                                        {
                                                            aggregateValue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) != null ? "\r\n" + biomeGroup.BiomeConfig.GetPropertyValueAsString(property) : "";
                                                        } else {
                                                            aggregateValue += biomeGroup.BiomeConfig.GetPropertyValueAsString(property) ?? "";
                                                        }
                                                    } else {
                                                        PopUpForm.CustomMessageBox("One does not simply merge a non-BiomesList, non-ResourceQueue setting. Setting \"" + property.Name + "\" group \"" + biomeGroup.Name + "\". Biome generation aborted.", "Generation error");
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        if (!String.IsNullOrEmpty(groupvalue))
                                        {
                                            valuesPerBiomeGroup.Add(groupvalue);
                                        }
                                    } else {
                                        throw new Exception("Biome Group data could not be found. This is not supposed to happen! Please contact the developer with a bug report.");
                                    }
                                }
                            }

                            bMerge = (!bOverride || bOverrideParentvalues) && bMerge;

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

                                        if (replaceStartIndex >= defaultText.Length - 1)
                                        {
                                            replaceStartIndex = replaceEndIndex;
                                            break;
                                        }
                                    }
                                    if (replaceStartIndex > replaceEndIndex)
                                    {
                                        replaceStartIndex = replaceEndIndex;
                                    }

                                    int replaceLength = replaceEndIndex - replaceStartIndex;

                                    if (replaceLength > 0)
                                    {
                                        string[] defaultBiomesListItemNames = defaultBiome.GetPropertyValueAsString(property) != null ? defaultBiome.GetPropertyValueAsString(property).Replace("\r", "").Split('\n') : null;
                                        List<string> newPropertyValue = new List<string>();

                                        if (defaultBiomesListItemNames != null && bMerge)
                                        {
                                            foreach (string value1 in defaultBiomesListItemNames)
                                            {
                                                if (value1 != null && !string.IsNullOrEmpty(value1.Trim()))
                                                {
                                                    bool duplicatePermission = value1.StartsWith("CustomObject(") || !newPropertyValue.Any(a => (string)a.Replace("\r", "").Replace("\n", "") == (string)value1.Replace("\r", "").Replace("\n", ""));
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
                                                                                    } else {
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
                                                                                        } else {
                                                                                            if (parameters[selectedOption.UniqueParameterIndex].Trim() == parameters2[selectedOption.UniqueParameterIndex].Trim())
                                                                                            {
                                                                                                newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name + parameters2[selectedOption.UniqueParameterIndex]));
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                newGroupPropertyValue.Add(value1.Trim());
                                                                            } else {
                                                                                newPropertyValue.RemoveAll(a => a.StartsWith(selectedOption.Name));
                                                                                newGroupPropertyValue.Add(value1.Trim());
                                                                            }
                                                                        } else {
                                                                            newGroupPropertyValue.Add(value1.Trim());
                                                                        }
                                                                    } else {
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

                                    if (!String.IsNullOrWhiteSpace(newValue))
                                    {
                                        defaultText = defaultText.Insert(replaceStartIndex, newValue + "\r\n");
                                    }
                                } else {

                                    //if (!property.Optional)
                                    {
                                        defaultText.Append(property.ScriptHandle + " " + newValue + "\r\n");

                                        //errorsTxt += "\r\nVersion config error: Setting \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                    }
                                    //PopupForm.CustomMessageBox("Version config error: Setting \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                    //return;
                                }
                            } else {
                                newValue = aggregateValue;

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

                                    if (valueStringStartIndex + skipCharsLength + valueStringLength < defaultText.Length)
                                    {
                                        while (defaultText[valueStringStartIndex + skipCharsLength + valueStringLength] != '\n')
                                        {
                                            valueStringLength += 1;

                                            if (valueStringStartIndex + skipCharsLength + valueStringLength >= defaultText.Length)
                                            {
                                                valueStringLength = 0;
                                                break;
                                            }
                                        }
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
                                } else {
                                    //TODO: Solve ReplaceToBiomeName problem: ReplaceToBiome is an optional setting but is required for virtual biomes
                                    //if (property.Name != "ReplaceToBiomeName")// && property.Name != "BiomeSizeWhenIsle" && property.Name != "BiomeRarityWhenIsle" && property.Name != "BiomeSizeWhenBorder")
                                    //{
                                    //PopupForm.CustomMessageBox("Version config error: Handle for setting \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Biome generation aborted.", "Generation error");
                                    //return;
                                    //}

                                    //if (!property.Optional)
                                    {
                                        defaultText.Append(property.ScriptHandle + " " + newValue + "\r\n");

                                        //errorsTxt += "\r\nVersion config error: The value for setting \"" + property.Name + "\" could not be found in file \"" + file.FullName + "\". Added it to the end of the file.";
                                    }
                                }
                            }
                        }
                        string fName = destinationDirectory.FullName + "\\" + file.Name;
                        System.IO.File.WriteAllText(fName, defaultText.ToString());
                    }
                }
                if (errorsTxt.Length > 0)
                {
                    PopUpForm.CustomMessageBox(errorsTxt, "Version config warnings");
                }
            }
        }
    }
}
