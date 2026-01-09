using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.CustomeAttributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableInfoAttribute : Attribute
    {
        private string _tableName;
        private string _keyFieldName;
        public TableInfoAttribute(string tableName, string keyFieldName) 
        {
            _tableName = tableName;
            _keyFieldName = keyFieldName;
        }
        public virtual string TableName
        {
            get { return _tableName; }
        }
        public virtual string KeyFieldName
        {
            get { return _keyFieldName; }
        }
    }
}
