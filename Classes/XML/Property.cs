using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace OTGEdit.XML
{
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/TCEE")]
    public class Property
    {
        [DataMember]
        public string Value;
        [DataMember]
        public bool Override;
        [DataMember]
        public string PropertyName;
        [DataMember]
        public bool Merge;
        [DataMember]
        public bool OverrideParentValues;

        public Property(string value, bool bOverride, string propertyName, bool merge, bool overrideParentValues)
        {
            Value = value;
            Override = bOverride;
            PropertyName = propertyName;
            Merge = merge;
            OverrideParentValues = overrideParentValues;
        }
    }
}
