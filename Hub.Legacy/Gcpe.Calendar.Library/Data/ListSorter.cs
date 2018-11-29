using System.Collections.Generic;
using System.Linq;
using System;

public enum ListSortDirection { Ascending, Descending }
/// <summary>
/// 
/// </summary>
public static class ListSorter
{   
    
    /// <summary>
    /// Sorts the entire specified list.
    /// </summary>
    /// <typeparam name="T">Type of the List Items.</typeparam>
    /// <param name="list">List to sort.</param>
    /// <param name="direction">The direction {Ascending, Descending}.</param>
    public static void Sort<T>(ref List<T> list, ListSortDirection direction)
    {
        list = Sort(list, direction == ListSortDirection.Ascending, string.Empty).ToList();
    }

    /// <summary>
    /// Sorts the specified list based on property Name.
    /// </summary>
    /// <typeparam name="T">Type of the List Items.</typeparam>
    /// <param name="list">List to sort.</param>
    /// <param name="propertyName">Name of the property on which the sorting to be done.</param>
    /// <param name="direction">The direction {Ascending, Descending}.</param>
    public static IEnumerable<T> Sort<T>(List<T> list, bool sortAscending, string propertyName, string property2Name = null)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            try
            {
                if (sortAscending)
                {
                    list.Sort();
                }
                else
                {
                    list.Sort();
                    list.Reverse();
                }
            }
            catch (Exception)
            {
                //throw new Exception("No property name specified");
            }
            return list;
        }
        else
        {
            try
            {
                Func<T, object> keySelector = x => GetObject(x, propertyName);
                IOrderedEnumerable<T> sortedList = sortAscending ? list.OrderBy(keySelector) : list.OrderByDescending(keySelector);
                if (property2Name != null)
                {
                    Func<T, object> key2Selector = x => GetObject(x, property2Name);
                    sortedList = sortAscending ? sortedList.ThenBy(key2Selector) : sortedList.ThenByDescending(key2Selector);
                }
                return sortedList;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Specified property '{0}' doesn't exist", propertyName));
            }
        }
    }

    /// <summary>
    /// Build the object for Lambda Expression
    /// </summary>
    /// <param name="obj">Object of the Parent Class</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Returns object for Lambda Expression</returns>
    private static object GetObject(object obj, string propertyName)
    {
        string currentPropertyName = "";
        string subPropertyName = "";
        if (propertyName.Contains('.'))
        {
            currentPropertyName = propertyName.Split('.')[0];
            subPropertyName = propertyName.Substring(currentPropertyName.Length + 1);
        }
        else
        {
            currentPropertyName = propertyName;
        }

        object result = obj.GetType().GetProperty(currentPropertyName).GetValue(obj, null);
        if (string.IsNullOrEmpty(subPropertyName))
        {
            return result;
        }
        return GetObject(result, subPropertyName);
    }
}
