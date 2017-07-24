using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace OTGE.XML
{
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/TCEE")]
    public class Group
    {
        [DataMember]
        public string Name;
        [DataMember]
        public List<string> Biomes = new List<string>();
        [DataMember]
        public BiomeConfig BiomeConfig;

        public bool showDefaults { get { return Biomes.Count == 1 && Name.Equals(Biomes[0]); } }

        public Group(string name, VersionConfig config)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be an empty or only spaces");
            Name = name;
            BiomeConfig = new BiomeConfig(config);
        }

        public Group(string name, List<string> biomes, BiomeConfig biomeConfig)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be an empty or only spaces");
            Name = name;
            Biomes = biomes;
            BiomeConfig = biomeConfig;
        }
    }
}
