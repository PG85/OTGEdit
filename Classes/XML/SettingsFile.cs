using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TCEE.XML
{
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/TCEE")]
    public class SettingsFile
    {
        [DataMember]
        public WorldConfig WorldConfig;
        [DataMember]
        public List<Group> BiomeGroups = new List<Group>();
    }
}
