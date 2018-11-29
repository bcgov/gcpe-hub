using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.WebApp.Providers
{
    public class SqlHelper
    {
        public static string ToInClause(IEnumerable<Guid> guids)
        {
            return "IN ('" + string.Join("','", guids) + "')";
        }

        public static string ProtectAgainstSqlInjection(string elt)
        {
            return elt.Replace("'", "''");
        }

        public static void AddDateToWhereClause(ref string whereClause, string elt, DateTimeOffset? date)
        {
            AddToWhereClause(ref whereClause, elt + "'" + date.Value.UtcDateTime.ToString("O") + "'");
        }

        public static void AddToWhereClause(ref string whereClause, string elt)
        {
            whereClause = string.IsNullOrEmpty(whereClause) ? " WHERE (" : whereClause + " AND (";
            whereClause += elt + ")";
        }
        public static void LoadContactNavigationProperties(string inClause, HubDbContext db)
        {
            db.ContactPhoneNumber.FromSql("SELECT * FROM media.ContactPhoneNumber WHERE ContactId " + inClause).Load();
            db.ContactWebAddress.FromSql("SELECT * FROM media.ContactWebAddress WHERE ContactId " + inClause).Load();
        }

        public static string CreateSearchClause(string search, string pattern)
        {
            string whereClause = null;
            foreach (string term in SqlHelper.ProtectAgainstSqlInjection(search).Split(' '))
            {
                // restore first name search ..  but only first 2 characters for fuzzy matching
                AddToWhereClause(ref whereClause, string.Format(pattern, term, term.Substring(0, Math.Min(term.Length, 2))));
            }
            return whereClause;
        }
    }
}