using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Tera.Libs.XML
{
    [Serializable]
    [XmlRoot("XMLCollection")]
    public class XMLCollection<EntityType> where EntityType : XMLEntity
    {
        [NonSerialized]
        private List<EntityType> memory = new List<EntityType>();

        [XmlArray("entities"), XmlArrayItem()]
        public List<EntityType> entities {
            get{return memory;}
            set{this.memory = value;}
        }

        public EntityType Find(Predicate<EntityType> match)
        {
            return entities.Find(match);
        }

        public List<EntityType> FindAll(Predicate<EntityType> match)
        {
            return entities.FindAll(match);
        }
   }
}
