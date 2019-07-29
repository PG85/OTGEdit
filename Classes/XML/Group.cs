using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace OTGEdit.XML
{
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/OTGEdit")]
    public class Group
    {
        [DataMember]
        public string Name;
        [DataMember]
        private List<string> Biomes = new List<string>();

        [XmlIgnore]
        private HashSet<string> biomesHash = null;
        [XmlIgnore]
        public HashSet<string> BiomesHash // TODO: Fix this properly and don't expose this
        {
            get
            {
                if (biomesHash == null)
                {
                    biomesHash = new HashSet<string>();
                    foreach (string property in Biomes)
                    {
                        biomesHash.Add(property);
                    }
                }
                return biomesHash;
            }
            set
            {
                biomesHash = value;
            }
        }

        [DataMember]
        public BiomeConfig BiomeConfig;

        public bool showDefaults { get { return BiomesHash.Count == 1 && Name.Equals(BiomesHash.First()); } }

        public void AddBiome(string biomeName)
        {
            BiomesHash.Add(biomeName);
            Biomes.Add(biomeName);
        }

        public void RemoveBiome(string biomeName)
        {
            BiomesHash.Remove(biomeName);
            Biomes.Remove(biomeName);
        }

        public void ClearBiomes()
        {
            BiomesHash.Clear();
            Biomes.Clear();
        }

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
