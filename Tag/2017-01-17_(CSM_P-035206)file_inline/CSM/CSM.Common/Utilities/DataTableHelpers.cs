using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSM.Common.Utilities
{
    public static class DataTableHelpers
    {
        public static DataTable ConvertTo<T>(T item)
        {
            DataTable table = null;

            try
            {
                table = CreateTable<T>();
                Type entityType = typeof (T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                table.Rows.Add(row);
                return table;
            }
            finally
            {
                if (table != null)
                {
                    table.Dispose();
                    table = null;
                }
            }
        }

        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = null;

            try
            {
                table = CreateTable<T>();
                Type entityType = typeof (T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
                foreach (T item in list)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    table.Rows.Add(row);
                }
                return table;
            }
            finally
            {
                if (table != null)
                {
                    table.Dispose();
                    table = null;
                }
            }
        }

        public static DataTable ConvertTo<T>(IEnumerable<T> list)
        {
            DataTable table = null;

            try
            {
                table = CreateTable<T>();
                Type entityType = typeof (T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
                foreach (T item in list)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor prop in properties)
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    table.Rows.Add(row);
                }
                return table;
            }
            finally
            {
                if (table != null)
                {
                    table.Dispose();
                    table = null;
                }
            }
        }

        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;
            if (rows != null)
            {
                list = new List<T>();
                foreach (DataRow row in rows)
                {
                    var item = CreateItem<T>(row);
                    list.Add(item);
                }
            }
            return list;
        }

        public static IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
                return null;

            var rows = new List<DataRow>();
            foreach (DataRow row in table.Rows)
                rows.Add(row);

            return ConvertTo<T>(rows);
        }

        // Convert DataRow into T Object
        public static T CreateItem<T>(DataRow row)
        {
            string columnName;
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in row.Table.Columns)
                {
                    columnName = column.ColumnName;
                    //Get property with same columnName
                    PropertyInfo prop = obj.GetType().GetProperty(columnName);
                    try
                    {
                        //Get value for the column
                        object value = (row[columnName].GetType() == typeof (DBNull)) ? null : row[columnName];
                        //Set property value
                        prop.SetValue(obj, value, null);
                    }
                    catch
                    {
                        throw;
                        //Catch whatever here
                    }
                }
            }
            return obj;
        }

        public static DataTable CreateTable<T>()
        {
            DataTable table = null;

            try
            {
                Type entityType = typeof (T);
                table = new DataTable(entityType.Name);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

                foreach (PropertyDescriptor prop in properties)
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

                return table;
            }
            finally
            {
                if (table != null)
                {
                    table.Dispose();
                    table = null;
                }
            }
        }

        public static DataTable ConvertToEx<T>() { return ConvertToEx<T>(null); }

        public static DataTable ConvertToEx<T>(IList<T> list)
        {
            DataTable table = null;
            try
            {
                Type entityType = typeof(T);
                table = new DataTable(entityType.Name);
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(entityType);
                foreach (PropertyDescriptor prop in props)
                {
                    var exAttr = prop.Attributes.OfType<ExportAttribute>().FirstOrDefault();
                    if (exAttr != null)
                    {
                        table.Columns.Add(exAttr.DisplayName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }
                }
                if (list != null)
                {
                    foreach (T item in list)
                    {
                        DataRow row = table.NewRow();
                        foreach (PropertyDescriptor prop in props)
                        {
                            var exAttr = prop.Attributes.OfType<ExportAttribute>().FirstOrDefault();
                            if (exAttr != null)
                            {
                                row[exAttr.DisplayName] = prop.GetValue(item) ?? DBNull.Value;
                            }
                        }
                        table.Rows.Add(row);
                    }
                }
                return table;
            }
            finally
            {
                if (table != null)
                    table.Dispose();
            }
        }

        public static string ToCSV(this DataTable dt, string[] numberAsStringCols = null)
        {
            StringBuilder sb = new StringBuilder();

            List<int> numStrCols = new List<int>();
            if (numberAsStringCols != null)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    if (numberAsStringCols.Contains(col.ColumnName))
                    {
                        numStrCols.Add(col.Ordinal);
                    }
                }
            }

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => $"\"{column.ColumnName.Replace("\"", "\"\"")}\"");
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => "\"" + field.ToString()
                                                                                    .Replace("\"", "\"\"")
                                                                                    .Replace(",", ", ")
                                                                                    .Replace(",  ", ", ")
                                                                                    + "\"");
                List<string> fieldList = fields.ToList();
                numStrCols.ForEach(idx =>
                {
                    if (fieldList[idx].Contains(",") == false)
                    {
                        fieldList[idx] = "=" + fieldList[idx];
                    }
                });
                sb.AppendLine(string.Join(",", fieldList));
            }

            return sb.ToString();
        }
    }
}