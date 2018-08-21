using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace XP2AFSAirportConverter.ResourceMapping
{
    public class ResourceMapper
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        private ResourceMap resourceMap;
        private Dictionary<string, ResourceMapItem> resourceLookup;

        public void ReadResourceMapping(string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ResourceMap));

                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    this.resourceMap = (ResourceMap)serializer.Deserialize(streamReader);

                    this.resourceLookup = new Dictionary<string, ResourceMapItem>();

                    foreach(var resourceItem in this.resourceMap.Resources)
                    {
                        this.resourceLookup.Add(resourceItem.XPResource, resourceItem);
                    }
                }
            }
            catch (Exception)
            {
                log.Error("Error reading resource map XML");
            }

        }

        public ResourceMapItem GetAFSResource(string xpResource)
        {
            ResourceMapItem resourceMapItem = null;

            if (this.resourceLookup.ContainsKey(xpResource))
            {
                resourceMapItem = this.resourceLookup[xpResource];
            }

            return resourceMapItem;
        }
    }
}
