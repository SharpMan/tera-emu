using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Tera.Libs.XML
{
    public class XMLTable<EntityType> : IEnumerable<EntityType> where EntityType : XMLEntity
    {

        public static XMLTable<EntityType> operator -(XMLTable<EntityType> table, EntityType value)
        {
            if (table.memory != null)
            {
                table.memory.entities.Remove(value);
            }
            return table;
        }

        public static XMLTable<EntityType> operator +(XMLTable<EntityType> table, EntityType value)
        {
            if (table.memory != null && !table.memory.entities.Contains(value))
            {
                table.memory.entities.Add(value);
            }
            return table;
        }

        private string file
        {
            get;
            set;
        }

        private XMLCollection<EntityType> memory = new XMLCollection<EntityType>();
        public XMLCollection<EntityType> collection
        {
            get
            { return memory; }
            set
            {
                this.memory = value;
            }
        }

        public XMLTable(string file)
        {
            this.file = file;
        }

        public void Save()
        {
            Type[] SubClass = { typeof(EntityType) };
            XmlSerializer xs = new XmlSerializer(collection.GetType(), SubClass);
            using (StreamWriter writer = File.CreateText(file))
            {
                xs.Serialize(writer, collection);
                writer.Flush();
            }
        }

        public void Load()
        {
            Type[] SubClass = { typeof(EntityType) };
            XmlSerializer xs = new XmlSerializer(typeof(XMLCollection<EntityType>), SubClass);
            using (StreamReader reader = File.OpenText(file))
            {
                this.collection = (XMLCollection<EntityType>)xs.Deserialize(reader);
            }
        }

        public IEnumerator<EntityType> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (this.memory != null)
            {
                return this.memory.entities.GetEnumerator();
            }
            else
            {
                return null;
            }
        }
    }
}
