using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace OTGEdit.XML
{
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/OTGEdit")]
    public class VersionConfig
    {
        [DataMember]
        public bool IsTerrainControl;

        [DataMember]
        public List<SettingsType> SettingsTypes;
        [DataMember]
        public List<ResourceQueueItem> ResourceQueueOptions;
        [DataMember]
        public List<OTGProperty> WorldConfig = new List<OTGProperty>();
        [DataMember]
        public List<OTGProperty> BiomeConfig = new List<OTGProperty>();

        [XmlIgnore]
        private Dictionary<string, OTGProperty> worldConfigDict = null;
        [XmlIgnore]
        public Dictionary<string, OTGProperty> WorldConfigDict
        {
            get
            {
                if (worldConfigDict == null)
                {
                    worldConfigDict = new Dictionary<string, OTGProperty>();
                    foreach (OTGProperty property in WorldConfig)
                    {
                        worldConfigDict.Add(property.Name, property);
                    }
                }
                return worldConfigDict;
            }
        }

        [XmlIgnore]
        private Dictionary<string, OTGProperty> biomeConfigDict = null;
        [XmlIgnore]
        public Dictionary<string, OTGProperty> BiomeConfigDict
        {
            get
            {
                if (biomeConfigDict == null)
                {
                    biomeConfigDict = new Dictionary<string, OTGProperty>();
                    foreach (OTGProperty property in BiomeConfig)
                    {
                        biomeConfigDict.Add(property.Name, property);
                    }
                }
                return biomeConfigDict;
            }
        }
    }
}
