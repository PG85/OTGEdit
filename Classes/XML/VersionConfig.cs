using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace OTGEdit.XML
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TCEE")]
    public class VersionConfig
    {
        [DataMember]
        public bool IsTerrainControl;

        [DataMember]
        public List<SettingsType> SettingsTypes;
        [DataMember]
        public List<ResourceQueueItem> ResourceQueueOptions;
        [DataMember]
        public List<TCProperty> WorldConfig = new List<TCProperty>();
        [DataMember]
        public List<TCProperty> BiomeConfig = new List<TCProperty>();

        private Dictionary<string, TCProperty> worldConfigDict = null;
        [XmlIgnore]
        public Dictionary<string, TCProperty> WorldConfigDict
        {
            get
            {
                if (worldConfigDict == null)
                {
                    worldConfigDict = new Dictionary<string, TCProperty>();
                    foreach (TCProperty property in WorldConfig)
                    {
                        worldConfigDict.Add(property.Name, property);
                    }
                }
                return worldConfigDict;
            }
        }

        private Dictionary<string, TCProperty> biomeConfigDict = null;
        [XmlIgnore]
        public Dictionary<string, TCProperty> BiomeConfigDict
        {
            get
            {
                if (biomeConfigDict == null)
                {
                    biomeConfigDict = new Dictionary<string, TCProperty>();
                    foreach (TCProperty property in BiomeConfig)
                    {
                        biomeConfigDict.Add(property.Name, property);
                    }
                }
                return biomeConfigDict;
            }
        }
    }
}
