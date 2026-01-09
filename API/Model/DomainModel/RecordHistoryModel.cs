using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Model.DomainModel
{
    public class RecordHistoryModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Action { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string TableUniqueId { get; set; } = string.Empty;
        public string ColumnName { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }

    public class ObjectDifference
    {
        public ObjectDifference(object newObject, object oldObject)
        {
            NewObjectType = newObject.GetType();
            OldObjectType = oldObject.GetType();

            NewObject = newObject;
            OldObject = oldObject;
        }
        public PropertyInfo[] Properties { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;

        public object OldObject { get; set; } = null!;
        public Type OldObjectType { get; set; }
        public object NewObject { get; set; } = null!;
        public Type NewObjectType { get; set; }
        public string SavedBy { get; set; } = string.Empty;
        public string SavedByUserName { get; set; } = string.Empty;
        public DateTime SavedDate { get; set; }
    }
}
