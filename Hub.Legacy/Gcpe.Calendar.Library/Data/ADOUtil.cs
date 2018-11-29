using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;

namespace Common
{
    public class ADOUtil
    {

        private ADOUtil() { }

        /// <summary>
        /// Converts to data table.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable(Object o)
        {
            PropertyInfo[] properties = o.GetType().GetProperties();

            DataTable dt = CreateDataTable(properties);

            FillData(properties, dt, o);

            return dt;

        }

        /// <summary>
        /// Converts to data table.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable(Object[] array)
        {

            PropertyInfo[] properties = array.GetType().GetElementType().GetProperties();
            DataTable dt = CreateDataTable(properties);
            if (array.Length != 0)
            {
                foreach (object o in array)
                {
                    FillData(properties, dt, o);
                }

            }
            return dt;
        }

        /// <summary>
        /// Creates the data table.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        private static DataTable CreateDataTable(PropertyInfo[] properties)
        {
            DataTable dt = new DataTable();
            DataColumn dc = null;
            foreach (PropertyInfo pi in properties)
            {
                dc = new DataColumn();
                dc.ColumnName = pi.Name;
                dc.DataType = pi.PropertyType;
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// Fills the data.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="o">The o.</param>
        private static void FillData(PropertyInfo[] properties, DataTable dt, Object o)
        {
            DataRow dr = dt.NewRow();

            foreach (PropertyInfo pi in properties)
            {
                dr[pi.Name] = pi.GetValue(o, null);
            }
            dt.Rows.Add(dr);
        }

        /// <summary>
        /// Execute "Select distinct" on the underlying datatable.
        /// </summary>
        /// <param name="sourceTable">The source table.</param>
        /// <param name="sourceColumn">The source column.</param>
        /// <returns></returns>
        public static DataTable SelectDistinct(DataTable sourceTable, string sourceColumn)
        {
            DataTable result = null;
            try
            {
                result = new DataTable();
                result.Columns.Add(sourceColumn, sourceTable.Columns[sourceColumn].DataType);
                Hashtable ht = new Hashtable();
                foreach (DataRow dr in sourceTable.Rows)
                {
                    if (!ht.ContainsKey(dr[sourceColumn].ToString()))
                    {
                        ht.Add(dr[sourceColumn].ToString(), null);
                        DataRow newRow = result.NewRow();
                        newRow[sourceColumn] = dr[sourceColumn];
                        result.Rows.Add(newRow);
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (result != null)
                {
                    result.Dispose();
                }
            }
        }
        /// <summary>
        /// Selects the query.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="whereClause">The where clause.</param>
        /// <param name="sortExpression">The sort expression.</param>
        /// <returns></returns>
        public static DataRow SelectQuery(DataTable table, string whereClause, string sortExpression)
        {
            DataRow[] results = table.Select(whereClause, sortExpression);
            if (results != null && results.Length > 0)
            {
                return results[0];
            }
            return null;
        }

        /// <summary>
        /// Copies the row.
        /// </summary>
        /// <param name="sourceRow">The source row.</param>
        /// <param name="destinationRow">The destination row.</param>
        public static void CopyRow(DataRow sourceRow, DataRow destinationRow)
        {
            for (int i = 0, l = sourceRow.ItemArray.Length; i < l; i++)
            {
                destinationRow[i] = sourceRow[i];
            }
        }

    }

}

