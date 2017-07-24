using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace OTGE.XML
{
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/TCEE")]
    public class SettingsType
    {
        [DataMember]
        public string Type;
        [DataMember]
        public string ColorType;
    }
}
