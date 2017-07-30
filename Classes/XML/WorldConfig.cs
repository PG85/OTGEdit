using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace OTGEdit.XML
{
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/TCEE")]
    public class WorldConfig
    {
        [DataMember]
        public List<Property> Properties = new List<Property>();

        private Dictionary<string, Property> propertiesDict = null;
        [XmlIgnore]
        public Dictionary<string, Property> PropertiesDict
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
            foreach (TCProperty tcProperty in config.WorldConfigDict.Values)
            {
                Property property = new Property(null, false, tcProperty.Name, false, tcProperty.PropertyType != "BiomesList" && tcProperty.PropertyType != "ResourceQueue");
                propertiesDict.Add(property.PropertyName, property);
            }
        }

        public void SetProperty(TCProperty tcProperty, string value, bool merge, bool overrideParentValues)
        {
            if (tcProperty.PropertyType != "String" && tcProperty.PropertyType != "BigString" && tcProperty.PropertyType != "BiomesList" && tcProperty.PropertyType != "ResourceQueue" && String.IsNullOrWhiteSpace(value))
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

        public string GetPropertyValueAsString(TCProperty property)
        {
            Property prop = PropertiesDict.ContainsKey(property.Name) ? PropertiesDict[property.Name] : null;
            return prop != null ? prop.Value : null;
        }

        public bool GetPropertyMerge(TCProperty property)
        {
            Property prop = PropertiesDict.ContainsKey(property.Name) ? PropertiesDict[property.Name] : null;
            return prop != null ? prop.Merge : false;
        }

        public bool GetPropertyOverrideParentValues(TCProperty property)
        {
            Property prop = PropertiesDict.ContainsKey(property.Name) ? PropertiesDict[property.Name] : null;
            return prop != null ? prop.OverrideParentValues : false;
        }
    }
}
