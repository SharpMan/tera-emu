using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace Tera.ORM
{
    public class TeraRecord
    {
        public void Update()
        {
            switch (TeraProvider.DbType)
            {
                case TeraDatabase.MYSQL:
                    var mysql_query = new StringBuilder("UPDATE " + tableName + " SET ");
                    var values = new List<string>();
                    foreach (var f in fields)
                    {
                        values.Add(f.Field + "='" + this.GetType().GetField(f.Field).GetValue(this) + "'");
                    }
                    mysql_query.Append(string.Join(",", values));
                    mysql_query.Append(" WHERE " + getPrimary().Field + "='" + this.GetType().GetField(getPrimary().Field).GetValue(this) + "'");
                    var cmd = new MySqlCommand(mysql_query.ToString(), TeraProvider.MYSQL_CONNECTION).ExecuteNonQuery();
                    break;
            }
        }

        public void Update(String[] records)
        {
            switch (TeraProvider.DbType)
            {
                case TeraDatabase.MYSQL:
                    var mysql_query = new StringBuilder("UPDATE " + tableName + " SET ");
                    var values = new List<string>();
                    foreach (String record in records)
                    {
                        values.Add(record + "='" + this.GetType().GetField(record).GetValue(this) + "'");
                    }
                    mysql_query.Append(string.Join(",", values));
                    mysql_query.Append(" WHERE " + getPrimary().Field + "='" + this.GetType().GetField(getPrimary().Field).GetValue(this) + "'");
                    var cmd = new MySqlCommand(mysql_query.ToString(), TeraProvider.MYSQL_CONNECTION).ExecuteNonQuery();
                    break;
            }
        }

        public void Insert()
        {
            switch (TeraProvider.DbType)
            {
                case TeraDatabase.MYSQL:
                    var mysql_query = new StringBuilder("INSERT INTO " + tableName + " (" + string.Join(",", this.fields) + ") VALUES ");
                    var values = new List<string>();
                    foreach (var f in fields)
                    {
                        var v = this.GetType().GetField(f.Field).GetValue(this);
                        values.Add("'" + v + "'");
                    }
                    mysql_query.Append("(" + string.Join(",", values) + ")");
                    var cmd = new MySqlCommand(mysql_query.ToString(), TeraProvider.MYSQL_CONNECTION).ExecuteNonQuery();
                    break;
            }
        }

        public void Delete()
        {
            switch (TeraProvider.DbType)
            {
                case TeraDatabase.MYSQL:
                    var mysql_query = new StringBuilder("DELETE FROM " + tableName + " WHERE " + getPrimary().Field + " = '" +
                        this.GetType().GetField(getPrimary().Field).GetValue(this) + "'");
                    var cmd = new MySqlCommand(mysql_query.ToString(), TeraProvider.MYSQL_CONNECTION).ExecuteNonQuery();
                    break;
            }

        }

        public static T FindFirst<T>(string criterion)
        {
            var objs = FindAll<T>(criterion);
            if (objs.Count > 0)
            {
                return objs[0];
            }
            else
            {
                return default(T);
            }
        }

        public static List<T> Find<T>(string criterion)
        {
            return FindAll<T>(criterion);
        }

        public static List<T> FindAll<T>(string criterion = "")
        {
            List<T> objs = new List<T>();
            List<TeraField> fields = getFields(typeof(T));
            var query = new StringBuilder("SELECT * FROM " + getTableName(typeof(T)));
            if (criterion != "")
            {
                query.Append(" WHERE " + criterion);
            }

            switch (TeraProvider.DbType)
            {
                case TeraDatabase.MYSQL:
                    var mysql_cmd = new MySqlCommand(query.ToString(), TeraProvider.MYSQL_CONNECTION);
                    var mysql_reader = mysql_cmd.ExecuteReader();
                    while (mysql_reader.Read())
                    {
                        T obj = (T)Activator.CreateInstance(typeof(T));
                        if (obj != null)
                        {
                            foreach (var field in fields)
                            {
                                switch (field.Infos)
                                {
                                    case TeraFieldType.INT:
                                        obj.GetType().GetField(field.Field).SetValue(obj, (int)mysql_reader[field.Field]);
                                        break;

                                    case TeraFieldType.STRING:
                                        obj.GetType().GetField(field.Field).SetValue(obj, (string)mysql_reader[field.Field]);
                                        break;
                                }
                            }
                        }
                        objs.Add(obj);
                    }
                    mysql_reader.Close();
                    break;
            }
            return objs;
        }

         private static string getTableName(Type type)
        {
            return ((TeraTable)type.GetCustomAttributes(typeof(TeraTable), true)[0]).Table;
        }

        private static List<TeraField> getFields(Type type)
        {
            var fields = new List<TeraField>();
            foreach (var fc in type.GetFields())
            {
                if (fc.GetCustomAttributes(typeof(TeraField), true).Length > 0)
                {
                    var attr = (TeraField)fc.GetCustomAttributes(typeof(TeraField), true)[0];
                    attr.Infos = getTypeOfField(fc);
                    fields.Add(attr);
                }
            }
            return fields;
        }

        private string tableName
        {
            get
            {
                return ((TeraTable)this.GetType().GetCustomAttributes(typeof(TeraTable), true)[0]).Table;
            }
        }

        private List<TeraField> fields
        {
            get
            {
                var fields = new List<TeraField>();
                foreach (var fc in this.GetType().GetFields())
                {
                    if (fc.GetCustomAttributes(typeof(TeraField), true).Length > 0)
                    {
                        var attr = (TeraField)fc.GetCustomAttributes(typeof(TeraField), true)[0];
                        attr.Infos = getTypeOfField(fc);
                        fields.Add(attr);
                    }
                }
                return fields;
            }
        }

        private static TeraFieldType getTypeOfField(FieldInfo f)
        {
            switch (f.FieldType.Name.ToLower())
            {
                case "int32":
                    return TeraFieldType.INT;

                case "string":
                    return TeraFieldType.STRING;

                default:
                    Console.WriteLine(f.FieldType.Name.ToLower());
                    return TeraFieldType.INT;
            }
        }

        private TeraField getPrimary()
        {
            var p = this.fields.FirstOrDefault(x => x.IsPrimary);
            return p;
        }

        public enum TeraFieldType
        {
            INT = 1,
            STRING = 2,
            LONG = 3,
        }

        public class TeraTable : Attribute
        {
            public string Table { get; set; }

            public TeraTable(string table)
            {
                this.Table = table;
            }
        }

        public class TeraField : Attribute
        {
            public string Field { get; set; }
            public TeraFieldType Infos = TeraFieldType.STRING;
            public bool IsPrimary { get; set; }

            public TeraField(string field, bool isPrimary = false)
            {
                this.Field = field;
                this.IsPrimary = isPrimary;
            }

            public override string ToString()
            {
                return this.Field;
            }
        }
    }
}
