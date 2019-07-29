using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace OTGEdit.XML
{
    [DataContract(Namespace= "http://schemas.datacontract.org/2004/07/OTGEdit")]
    public class OTGProperty
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
        [DataMember]
        public double MinValue = Int32.MinValue;
        [DataMember]
        public double MaxValue = Int32.MaxValue;
        [DataMember]
        public List<string> AllowedValues;
    }
}
