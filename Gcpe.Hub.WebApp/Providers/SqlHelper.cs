using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotSupportedException("Use LoadContactNavigationProperties(IEnumerable<Guid>, HubDbContext) instead.");
        }

        public static void LoadContactNavigationProperties(IEnumerable<Guid> contactIds, HubDbContext db)
        {
            var ids = contactIds?.Distinct().ToList();
            if (ids == null || ids.Count == 0)
            {
                return;
            }

            db.ContactPhoneNumber.Where(e => ids.Contains(e.ContactId)).Load();
            db.ContactWebAddress.Where(e => ids.Contains(e.ContactId)).Load();
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

