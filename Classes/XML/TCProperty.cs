using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TCEE.XML
{
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/TCEE")]
    public class TCProperty
    {
        [DataMember]
        public string Group;
        [DataMember]
        public string LabelText;
        [DataMember]
        public string Name;
        [DataMember]
        public string PropertyType;
        [DataMember]
        public string ScriptHandle;
        [DataMember]
        public bool Optional;
    }
}
