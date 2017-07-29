using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace OTGEdit.XML
{
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/TCEE")]
    public class VersionConfig
    {
        [DataMember]
        public List<SettingsType> SettingsTypes;
        [DataMember]
        public List<ResourceQueueItem> ResourceQueueOptions;
        [DataMember]
        public List<TCProperty> WorldConfig = new List<TCProperty>();
        [DataMember]
        public List<TCProperty> BiomeConfig = new List<TCProperty>();
    }
}
