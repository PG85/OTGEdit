using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace OTGEdit.XML
{
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/TCEE")]
    public class ResourceQueueItem
    {
        [DataMember]
        public string Name;
        [DataMember]
        public bool HasUniqueParameter;
        [DataMember]
        public int UniqueParameterIndex;
        [DataMember]
        public bool IsUnique;
        [DataMember]
        public List<string> UniqueParameterValues;

        [DataMember]
        public bool PopupAskForOverride = false;
    }
}
