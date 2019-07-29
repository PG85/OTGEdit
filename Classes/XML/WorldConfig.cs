using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace OTGEdit.XML
{
    [DataContract(Namespace= "http://schemas.datacontract.org/2004/07/OTGEdit")]
    public class WorldConfig
    {
        [DataMember]
        public List<Property> Properties = new List<Property>();

        [XmlIgnore]
        private Dictionary<string, Property> propertiesDict = null;
        [XmlIgnore]
        public Dictionary<string, Property> PropertiesDict // TODO: Fix this properly and don't expose this
        {
            get
            {
                if (propertiesDict == null)
                {
                    propertiesDict = new Dictionary<string, Property>();
                    foreach (Property property in Properties)
                    {
                        propertiesDict.Add(property.PropertyName, property);
                    }
                }
                return propertiesDict;
            }
        }

        public WorldConfig(VersionConfig config)
        {
            propertiesDict = new Dictionary<string, Property>();
            foreach (OTGProperty otgProperty in config.WorldConfigDict.Values)
            {
                AddProperty(new Property(null, false, otgProperty.Name, false, otgProperty.PropertyType != "BiomesList" && otgProperty.PropertyType != "BiomesListSingle" && otgProperty.PropertyType != "ResourceQueue"));
            }
        }

        public void AddProperty(Property property)
        {
            PropertiesDict.Add(property.PropertyName, property);
            Properties.Add(property);
        }

        public void RemoveProperty(string propertyName)
        {
            Property property = PropertiesDict[propertyName];
            PropertiesDict.Remove(propertyName);
            Properties.Remove(property);
        }

        public void SetProperty(OTGProperty tcProperty, string value, bool merge, bool overrideParentValues)
        {
            if (tcProperty.PropertyType != "String" && tcProperty.PropertyType != "BigString" && tcProperty.PropertyType != "BiomesList" && tcProperty.PropertyType != "BiomesListSingle" && tcProperty.PropertyType != "ResourceQueue" && String.IsNullOrWhiteSpace(value))
            {
                value = null;
            }
            if (value != null)
            {
                value = value.Trim();
            }
            Property property = PropertiesDict[tcProperty.Name];
            property.Value = value;
            property.Merge = merge;
            property.OverrideParentValues = overrideParentValues;
        }

        public string GetPropertyValueAsString(OTGProperty property)
        {
            Property prop = PropertiesDict.ContainsKey(property.Name) ? PropertiesDict[property.Name] : null;
            return prop != null ? prop.Value : null;
        }

        public bool GetPropertyMerge(OTGProperty property)
        {
            Property prop = PropertiesDict.ContainsKey(property.Name) ? PropertiesDict[property.Name] : null;
            return prop != null ? prop.Merge : false;
        }

        public bool GetPropertyOverrideParentValues(OTGProperty property)
        {
            Property prop = PropertiesDict.ContainsKey(property.Name) ? PropertiesDict[property.Name] : null;
            return prop != null ? prop.OverrideParentValues : false;
        }
    }
}
