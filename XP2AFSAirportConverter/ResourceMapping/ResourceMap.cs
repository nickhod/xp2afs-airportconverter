using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XP2AFSAirportConverter.ResourceMapping
{
    public class ResourceMapItem
    {
        [XmlAttribute]
        public string XPResource { get; set; }

        [XmlAttribute]
        public string AFSResource { get; set; }

        [XmlAttribute]
        public bool CultivationObject { get; set; }
    }

    public class ResourceMap
    {
        public List<ResourceMapItem> Resources { get; set; }
    }
}
