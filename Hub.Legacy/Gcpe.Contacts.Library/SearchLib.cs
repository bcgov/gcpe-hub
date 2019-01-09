using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using MediaRelationsDatabase;
using Gcpe.Hub.Properties;

namespace MediaRelationsLibrary
{
    public class IntermediateContactSearchResult
    {
        public string ContactName;
        public Contact Contact;
        public ContactWebAddress WebAddress;
        public ContactPhoneNumber PhoneNumber;
        public ElectoralDistrict District;
        public Region Region;
        public Sector Sector;
        public ContactMediaJobTitle MediaJobTitle;
        public ContactBeat Beat;
    }
    public class IntermediateCompanySearchResult
    {
        public Company Company;
        public Company ParentCompany;
        public CompanyWebAddress WebAddress;
        public CompanyPhoneNumber PhoneNumber;
        public ElectoralDistrict District;
        public Region Region;
        public Sector Sector;
        public MediaDesk MediaDesk;
        public Distribution Distribution;
        public Language Language;
        public PublicationDay PublicationDays;
        public SpecialtyPublication SpecialtyPublication;
        public MediaType MediaType;
        public Ethnicity Ethnicity;
    }

    public class ContactSearchResult
    {
        public Contact Contact;
        public string FirstName;
        public string LastName;
        public string ContactName;
        public string City;
        public string Email;
        public string Twitter;
        public string MinisterialJobTitle;
        public string Ministry;
        public string PrimaryPhone;
        public string PrimaryPhoneExtension;
        public string MatchList;
    }

    public class CompanySearchResult
    {
        public Company Company;
        public string CompanyName;
        public string ParentCompanyName;
        public string City;
        public string Email;
        public string Twitter;
        public string PrimaryPhone;
        public string PrimaryPhoneExtension;
        public string MatchList;
        public string FirstMediaType;
    }

    public class SearchLib
    {
        public enum EntityType
        {
            Contact = 1,
            Company = 2,
            Outlet = 3
        };
        public enum ReportType
        {
            PressGallery = 1,
            EthnicMedia = 2,
            MajorMedia = 3,
            LiveTalkOpportunities = 4
        };
        public enum CriteriaType
        {
            Name = 1,
            EmailOrWebsite = 2,
            PhoneNumber = 3,
            StreetAddress = 4,
            City = 5,
            OtherCity = 6,
            Province = 7,
            OtherProvince = 8,
            Country = 9,
            PostalCode = 10,
            Region = 11,
            ElectoralDistrict = 12,
            Sector = 13,
            MediaJobTitle = 14,
            Ministry = 15,
            MinisterialJobTitle = 16,
            MLAAssignment = 17,
            Minister = 18,
            MediaDesk = 19,
            Distribution = 20,
            Language = 21,
            PublicationDays = 22,
            SpecialtyPublication = 23,
            MediaType = 24,
            PrintCategory = 25,
            Ethnicity = 26,
            PublicationFrequency = 27,
            ModifiedDateRange = 28,
            CreatedDateRange = 29,
            MediaCategory = 30,
            Beat = 31
        };
        public enum DateRangeType
        {
            Today = 1,
            Yesterday = 2,
            TwoDaysAgo = 3,
            Last7Days = 4,
            Last14Days = 5,
            Last30Days = 6,
            Older = 7
        };

        /*public static string[] noiseWords = { "in",
                                                "and",
                                                "or",
                                                "a",
                                                "the",
                                                "at",
                                                "on",
                                                "if",
                                                "of" };*/

        public List<string> GetTerms(string str)
        {
            str = Regex.Replace(str, "[\\,\\.]", "");

            List<string> terms = new List<string>();
            Regex re = new Regex("(?<match>\\w+)|\\\"(?<match>[^\\\"]+?)\\\"");

            MatchCollection matches = re.Matches(str);
            foreach (Match m in matches)
            {
                //if (Array.IndexOf(SearchLib.noiseWords, m.Groups["match"].Value) < 0)
                terms.Add(m.Groups["match"].Value);
            }

            return terms;
        }

        public string GetTypedownResults(string q)
        {
            if (q == null) return null;
            string query = q.Trim();
            if (string.IsNullOrWhiteSpace(query)) return null;

            StringBuilder sb = new StringBuilder();
            using (var ctx = new MediaRelationsEntities())
            {
                var contacts = (from con in ctx.Contacts
                                where con.IsActive && (con.FirstName.Contains(query) || con.LastName.Contains(query) || (con.FirstName + " " + con.LastName).Contains(query) || (con.LastName + ", " + con.FirstName).Contains(query))
                                select new
                                {
                                    Type = "Contact",
                                    Id = con.Id,
                                    Name = con.FirstName + " " + con.LastName
                                });
                var companies = (from cmp in ctx.Companies
                                 where cmp.IsActive && cmp.CompanyName.Contains(query) && !cmp.IsOutlet
                                 select new
                                 {
                                     Type = "Company",
                                     Id = cmp.Id,
                                     Name = cmp.CompanyName
                                 });
                var outlets = (from cmp in ctx.Companies
                               where cmp.IsActive && cmp.CompanyName.Contains(query) && cmp.IsOutlet
                               select new
                               {
                                   Type = "Outlet",
                                   Id = cmp.Id,
                                   Name = cmp.CompanyName
                               });

                int take = Settings.Default.TypedownItemLimit;

                var combined = contacts.Union(companies).Union(outlets).OrderBy(t => t.Name).Take(take);
                foreach (var result in combined)
                {
                    string resultUrl = null;

                    var path = System.Web.VirtualPathUtility.ToAbsolute("~/Contacts/");

                    if (result.Type == "Contact") resultUrl = path + "Contact/ViewContact.aspx?guid=" + result.Id;
                    else if (result.Type == "Outlet") resultUrl = path + "MediaOutlet/ViewOutlet.aspx?guid=" + result.Id;
                    else if (result.Type == "Company") resultUrl = path + "Company/ViewCompany.aspx?guid=" + result.Id;

                    sb.Append("<div class='typedown-result' onclick=\"%clk%\">");
                    sb.Append("<span class='result-original'>" + result.Name + "</span>");
                    sb.Append("<span class='result-url'>" + resultUrl + "</span>");
                    sb.Append(result.Type + ": " + Regex.Replace(result.Name, query, "<span style='font-weight:bold;'>$0</span>", RegexOptions.IgnoreCase));
                    sb.Append("</div>\n");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// gets a string formatted for consumption by the search date methods
        /// </summary>
        /// <param name="value">a string representing a DateRangeType enum entry</param>
        /// <returns>a string of the format yyyy/MM/dd|yyyy/MM/dd representing a from-to date range</returns>
        public string GetDateString(string value)
        {
            DateRangeType drType;
            if (Enum.TryParse(value, out drType))
                return GetDateString(drType);

            return value;
        }

        /// <summary>
        /// gets a string formatted for consumption by the search date methods
        /// </summary>
        /// <param name="drType">a DateRangeType</param>
        /// <returns>a string of the format yyyy/MM/dd|yyyy/MM/dd representing a from-to date range</returns>
        public string GetDateString(DateRangeType drType)
        {
            string result = null;

            switch (drType)
            {
                case DateRangeType.Today:
                    result = DateTime.Today.ToString("yyyy/MM/dd") + "|";
                    break;
                case DateRangeType.Yesterday:
                    result = DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd") + "|" + DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd");
                    break;
                case DateRangeType.TwoDaysAgo:
                    result = DateTime.Today.AddDays(-2).ToString("yyyy/MM/dd") + "|" + DateTime.Today.AddDays(-2).ToString("yyyy/MM/dd");
                    break;
                case DateRangeType.Last7Days:
                    result = DateTime.Today.AddDays(-7).ToString("yyyy/MM/dd") + "|";
                    break;
                case DateRangeType.Last14Days:
                    result = DateTime.Today.AddDays(-14).ToString("yyyy/MM/dd") + "|";
                    break;
                case DateRangeType.Last30Days:
                    result = DateTime.Today.AddDays(-30).ToString("yyyy/MM/dd") + "|";
                    break;
                case DateRangeType.Older:
                    result = "|" + DateTime.Today.AddDays(-30).ToString("yyyy/MM/dd");
                    break;
            }

            return result;
        }

        public bool IsContactOnlySearch(List<KeyValuePair<CriteriaType, string>> criteria)
        {
            bool returnVal = false;
            for (int i = 0; i < criteria.Count && !returnVal; i++)
            {
                switch (criteria[i].Key)
                {
                    case CriteriaType.Beat:
                        returnVal = true;
                        break;
                    case CriteriaType.MediaJobTitle:
                        returnVal = true;
                        break;
                    case CriteriaType.Minister:
                        returnVal = true;
                        break;
                    case CriteriaType.MinisterialJobTitle:
                        returnVal = true;
                        break;
                    case CriteriaType.Ministry:
                        returnVal = true;
                        break;
                    case CriteriaType.MLAAssignment:
                        returnVal = true;
                        break;
                    case CriteriaType.MediaCategory:
                        ReportType rtyp;
                        if (Enum.TryParse(criteria[i].Value, out rtyp))
                        {
                            switch (rtyp)
                            {
                                case ReportType.PressGallery:
                                    returnVal = true;
                                    break;
                            }
                        }
                        break;
                }
            }

            return returnVal;
        }

        public bool IsCompanyOnlySearch(List<KeyValuePair<CriteriaType, string>> criteria)
        {
            bool returnVal = false;
            for (int i = 0; i < criteria.Count && !returnVal; i++)
            {
                switch (criteria[i].Key)
                {
                    case CriteriaType.Distribution:
                        returnVal = true;
                        break;
                    case CriteriaType.Ethnicity:
                        returnVal = true;
                        break;
                    case CriteriaType.Language:
                        returnVal = true;
                        break;
                    case CriteriaType.MediaDesk:
                        returnVal = true;
                        break;
                    case CriteriaType.MediaType:
                        returnVal = true;
                        break;
                    case CriteriaType.PrintCategory:
                        returnVal = true;
                        break;
                    case CriteriaType.PublicationDays:
                        returnVal = true;
                        break;
                    case CriteriaType.PublicationFrequency:
                        returnVal = true;
                        break;
                    case CriteriaType.MediaCategory:
                        ReportType rtyp;
                        if (Enum.TryParse(criteria[i].Value, out rtyp))
                        {
                            switch (rtyp)
                            {
                                case ReportType.EthnicMedia:
                                    returnVal = true;
                                    break;
                                case ReportType.MajorMedia:
                                    returnVal = true;
                                    break;
                                case ReportType.LiveTalkOpportunities:
                                    returnVal = true;
                                    break;
                            }
                        }

                        break;

                    case CriteriaType.SpecialtyPublication:
                        returnVal = true;
                        break;
                }
            }
            return returnVal;
        }

        /// <summary>
        /// Used to check if this is the type of search where a company specific and contact specific field 
        /// are both specified, this is used so that it tells the search page to reset the outlet and company fields
        /// to contain only the outlets of the contact results
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="isMatchAll"></param>
        public bool IsNestedContactSearch(List<KeyValuePair<CriteriaType, string>> criteria, bool isMatchAll)
        {
            if (!isMatchAll) return false; // never a nested search
            if (criteria.Count == 0) return false;

            if (IsCompanyOnlySearch(criteria) && IsContactOnlySearch(criteria)) return true;
            return false;
        }

        public IQueryable<ContactSearchResult> GetContactsForCompanies(MediaRelationsEntities ctx, IList<CompanySearchResult> companies)
        {
            if (companies == null) return null;

            List<Guid> companyGuids = new List<Guid>();
            foreach (var cmp in companies)
            {
                companyGuids.Add(cmp.Company.Id);
            }

            var results = (from con in ctx.Contacts
                           from jb in con.ContactMediaJobTitles
                           where con.IsActive
                           select new
                           {
                               Contact = con,
                               Company = jb.Company
                           });

            results = results.Where(t => companyGuids.Contains(t.Company.Id));

            return results.Select(x => new ContactSearchResult
            {
                Contact = x.Contact,
                FirstName = x.Contact.FirstName,
                LastName = x.Contact.LastName,
                ContactName = x.Contact.FirstName + " " + x.Contact.LastName,
                City = (x.Contact.ContactAddresses.Count > 0 ? x.Contact.ContactAddresses.FirstOrDefault().CityName : null),
                Email = (x.Contact.ContactWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("email")) ? x.Contact.ContactWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("email")).FirstOrDefault().WebAddress : null),
                Twitter = (x.Contact.ContactWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")) ? x.Contact.ContactWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")).FirstOrDefault().WebAddress : null),
                PrimaryPhone = (x.Contact.ContactPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.Contact.ContactPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumber : null),
                PrimaryPhoneExtension = (x.Contact.ContactPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.Contact.ContactPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumberExtension : null),
                MinisterialJobTitle = x.Contact.MinisterialJobTitle.MinisterialJobTitleName,
                Ministry = (x.Contact.Ministry == null ? null : x.Contact.Ministry.DisplayName)
            }).Distinct();
        }

        public IQueryable<CompanySearchResult> GetOutletsForCompanies(MediaRelationsEntities ctx, IList<CompanySearchResult> companies)
        {
            if (companies == null) return null;

            List<Guid> companyGuids = new List<Guid>();
            foreach (var cmp in companies)
            {
                companyGuids.Add(cmp.Company.Id);
            }

            var results = (from cmp in ctx.Companies
                           where cmp.IsActive
                           where cmp.IsOutlet
                           where companyGuids.Contains((Guid)cmp.ParentCompanyId)
                           select cmp);

            return results.Select(x => new CompanySearchResult
            {
                Company = x,
                CompanyName = x.CompanyName,
                ParentCompanyName = (x.ParentCompanyId == null ? null : (from comp in ctx.Companies where comp.Id == x.ParentCompanyId select comp.CompanyName).FirstOrDefault()),
                //City = (x.Company.CompanyAddresses.Any(t => t.AddressType == physicalAddressType && t.City != null) ? x.Company.CompanyAddresses.Where(t => t.AddressType == physicalAddressType).FirstOrDefault().CityName : null),

                City = x.CompanyAddresses.Where(t => t.City != null).OrderBy(t => t.AddressType).Select(t => t.City.CityName).FirstOrDefault(),
                Email = (x.CompanyWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("email")) ? x.CompanyWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("email")).FirstOrDefault().WebAddress : null),
                Twitter = (x.CompanyWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")) ? x.CompanyWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")).FirstOrDefault().WebAddress : null),
                PrimaryPhone = (x.CompanyPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.CompanyPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumber : null),
                PrimaryPhoneExtension = (x.CompanyPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.CompanyPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumberExtension : null),
                FirstMediaType = (x.MediaTypes.Count > 0 ? x.MediaTypes.FirstOrDefault().MediaTypeName : null)
            }).Distinct();
        }

        /// <summary>
        /// Searches for contacts based on the supplied criteria
        /// </summary>
        /// <param name="criteria">a list of criteria types and values to search</param>
        /// <param name="ctx">database context to use</param>
        /// <param name="matchAll">if true, all criteria must be matched, if false, any one or more must be matched</param>
        /// <param name="companies"></param>
        /// <returns></returns>
        public IQueryable<ContactSearchResult> ContactSearch(List<KeyValuePair<CriteriaType, string>> criteria, MediaRelationsEntities ctx, bool matchAll, IList<CompanySearchResult> companies = null)
        {
            IQueryable<IntermediateContactSearchResult> results = null;
            IQueryable<ContactSearchResult> contacts = null;

            Expression<Func<IntermediateContactSearchResult, bool>> searchExpression = null;

            if (criteria.Count > 0)
            {
                if (!matchAll)
                {
                    searchExpression = (s => false);
                }
                else if (IsCompanyOnlySearch(criteria))
                {
                    //return only contacts from matched companies
                    contacts = GetContactsForCompanies(ctx, companies);
                    results = (from con in contacts
                               from cwa in con.Contact.ContactWebAddresses.DefaultIfEmpty()
                               from cpn in con.Contact.ContactPhoneNumbers.DefaultIfEmpty()
                               from ed in con.Contact.ElectoralDistricts.DefaultIfEmpty()
                               from reg in con.Contact.Regions.DefaultIfEmpty()
                               from sec in con.Contact.Sectors.DefaultIfEmpty()
                               from mjt in con.Contact.ContactMediaJobTitles.DefaultIfEmpty()
                               from bt in con.Contact.ContactBeats.DefaultIfEmpty()
                               where con.Contact.IsActive
                               select new IntermediateContactSearchResult
                               {
                                   ContactName = con.FirstName + " " + con.LastName,
                                   Contact = con.Contact,
                                   WebAddress = cwa,
                                   PhoneNumber = cpn,
                                   District = ed,
                                   Region = reg,
                                   Sector = sec,
                                   MediaJobTitle = mjt,
                                   Beat = bt,
                               });
                }
            }

            if (contacts == null)
            {
                results = (from con in ctx.Contacts
                           from cwa in con.ContactWebAddresses.DefaultIfEmpty()
                           from cpn in con.ContactPhoneNumbers.DefaultIfEmpty()
                           from ed in con.ElectoralDistricts.DefaultIfEmpty()
                           from reg in con.Regions.DefaultIfEmpty()
                           from sec in con.Sectors.DefaultIfEmpty()
                           from mjt in con.ContactMediaJobTitles.DefaultIfEmpty()
                           from bt in con.ContactBeats.DefaultIfEmpty()
                           where con.IsActive
                           select new IntermediateContactSearchResult
                           {
                               ContactName = con.FirstName + " " + con.LastName,
                               Contact = con,
                               WebAddress = cwa,
                               PhoneNumber = cpn,
                               District = ed,
                               Region = reg,
                               Sector = sec,
                               MediaJobTitle = mjt,
                               Beat = bt,
                           });
            }

            foreach (KeyValuePair<CriteriaType, string> cri in criteria)
            {
                string value = cri.Value;
                Expression<Func<IntermediateContactSearchResult, bool>> critExpression = null;
                switch (cri.Key)
                {
                    #region contact match any contact tab
                    //Contact Tab
                    case CriteriaType.Name:
                        List<string> terms = GetTerms(value);
                        foreach (string term in terms)
                        {
                            Expression<Func<IntermediateContactSearchResult, bool>> termOrExpression = s => s.ContactName.Contains(term);
                            critExpression = critExpression == null ? termOrExpression : critExpression.And(termOrExpression);
                        }
                        break;
                    case CriteriaType.EmailOrWebsite:
                        critExpression = s => s.WebAddress.WebAddress.Contains(value);
                        break;
                    case CriteriaType.PhoneNumber:
                        critExpression = s => s.PhoneNumber.PhoneNumber.Contains(value);
                        break;
                    case CriteriaType.Beat:
                        Guid bId = Guid.Parse(value);
                        critExpression = s => s.Beat.BeatId == bId;
                        break;
                    #endregion

                    #region contact match any location tab
                    //Location Tab
                    case CriteriaType.StreetAddress:
                        critExpression = s => s.Contact.ContactAddresses.Any(x => x.StreetAddress.Contains(value));
                        break;
                    case CriteriaType.City:
                        Guid cgd = Guid.Parse(value);
                        critExpression = s => s.Contact.ContactAddresses.Any(x => x.CityId == cgd);
                        break;
                    case CriteriaType.OtherCity:
                        critExpression = s => s.Contact.ContactAddresses.Any(x => x.CityName.Contains(value));
                        break;
                    case CriteriaType.Province:
                        Guid pgd = Guid.Parse(value);
                        critExpression = s => s.Contact.ContactAddresses.Any(x => x.ProvStateId == pgd);
                        break;
                    case CriteriaType.OtherProvince:
                        critExpression = s => s.Contact.ContactAddresses.Any(x => x.ProvStateName.Contains(value));
                        break;
                    case CriteriaType.Country:
                        Guid cygd = Guid.Parse(value);
                        critExpression = s => s.Contact.ContactAddresses.Any(x => x.CountryId == cygd);
                        break;
                    case CriteriaType.PostalCode:
                        critExpression = s => s.Contact.ContactAddresses.Any(x => x.PostalZipCode.Contains(value));
                        break;
                    case CriteriaType.ElectoralDistrict:
                        Guid edgd = Guid.Parse(value);
                        critExpression = s => s.District.Id == edgd;
                        break;
                    case CriteriaType.Region:
                        Guid rggd = Guid.Parse(value);
                        critExpression = s => s.Region.Id == rggd;
                        break;
                    case CriteriaType.Sector:
                        Guid scgd = Guid.Parse(value);
                        critExpression = s => s.Sector.Id == scgd;
                        break;
                    #endregion

                    #region contact match any media tab
                    //Media Tab
                    case CriteriaType.MediaJobTitle:
                        Guid mtgd = Guid.Parse(value);
                        critExpression = s => s.MediaJobTitle.MediaJobTitleId == mtgd;
                        break;
                    #endregion

                    #region contact match any ministry tab
                    //Ministry Tab
                    case CriteriaType.Ministry:
                    case CriteriaType.Minister:
                        Guid mngd = Guid.Parse(value);
                        critExpression = s => s.Contact.MinistryId == mngd;
                        break;
                    case CriteriaType.MinisterialJobTitle:
                        Guid mjgd = Guid.Parse(value);
                        critExpression = s => s.Contact.MinisterialJobTitleId == mjgd;
                        break;
                    case CriteriaType.MLAAssignment:
                        Guid mlgd = Guid.Parse(value);
                        critExpression = s => s.Contact.MLAAssignmentId == mlgd;
                        break;
                    #endregion

                    #region contact match any date and report type
                    //Dates and Report Types
                    case CriteriaType.ModifiedDateRange:
                        string dvalue = GetDateString(value);
                        string[] bothDates = dvalue.Split('|');
                        if (bothDates.Length == 2)
                        {
                            DateTime adt, bdt;
                            if (DateTime.TryParse(bothDates[1].Trim(), out bdt))
                                bdt = bdt.AddDays(1);
                            else
                                bdt = DateTime.MinValue;

                            if (!DateTime.TryParse(bothDates[0].Trim(), out adt))
                            {
                                if (!bdt.Equals(DateTime.MinValue))
                                {
                                    critExpression = (s => s.Contact.ModifiedDate < bdt);
                                }
                            }
                            else if (!bdt.Equals(DateTime.MinValue))
                            {
                                critExpression = (s => s.Contact.ModifiedDate >= adt && s.Contact.ModifiedDate < bdt);
                            }
                            else
                            {
                                critExpression = (s => s.Contact.ModifiedDate >= adt);
                            }
                        }
                        break;
                    case CriteriaType.CreatedDateRange:
                        string cdvalue = GetDateString(value);
                        string[] cbothDates = cdvalue.Split('|');
                        if (cbothDates.Length == 2)
                        {
                            DateTime adt, bdt;
                            if (DateTime.TryParse(cbothDates[1].Trim(), out bdt))
                                bdt = bdt.AddDays(1);
                            else
                                bdt = DateTime.MinValue;

                            if (!DateTime.TryParse(cbothDates[0].Trim(), out adt))
                            {
                                if (!bdt.Equals(DateTime.MinValue))
                                {
                                    critExpression = (s => s.Contact.CreationDate < bdt);
                                }
                            }
                            else if (!bdt.Equals(DateTime.MinValue))
                            {
                                critExpression = (s => s.Contact.CreationDate >= adt && s.Contact.CreationDate < bdt);
                            }
                            else
                            {
                                critExpression = (s => s.Contact.CreationDate >= adt);
                            }
                        }
                        break;

                    case CriteriaType.MediaCategory:
                        var rType = (ReportType)Enum.Parse(typeof(ReportType), value);
                        switch (rType)
                        {
                            case ReportType.PressGallery:
                                critExpression = (s => s.Contact.IsPressGallery);
                                break;
                        }
                        break;
                        #endregion
                }
                if (critExpression != null)
                {
                    if (searchExpression == null) searchExpression = critExpression;
                    else searchExpression = matchAll ? searchExpression.And(critExpression) : searchExpression.Or(critExpression);
                }
            }
            if (searchExpression != null)
            {
                results = results.Where(searchExpression);
            }

            List<string> cityCriteria = new List<string>();
            foreach (KeyValuePair<CriteriaType, string> crit in criteria.Where(t => t.Key == CriteriaType.City))
            {
                Guid gd = Guid.Parse(crit.Value);
                cityCriteria.Add((from c in ctx.Cities where c.Id == gd select c.CityName).FirstOrDefault());
            }

            List<string> regionCriteria = new List<string>();
            foreach (KeyValuePair<CriteriaType, string> crit in criteria.Where(t => t.Key == CriteriaType.Region))
            {
                Guid gd = Guid.Parse(crit.Value);
                regionCriteria.Add((from rg in ctx.Regions where rg.Id == gd select rg.RegionName).FirstOrDefault());
            }

            IQueryable<ContactSearchResult> finalResults = results.Select(x => new ContactSearchResult
            {
                Contact = x.Contact,
                FirstName = x.Contact.FirstName,
                LastName = x.Contact.LastName,
                ContactName = x.Contact.FirstName + " " + x.Contact.LastName,
                City = (x.Contact.ContactAddresses.Count > 0 ? x.Contact.ContactAddresses.FirstOrDefault().CityName : null),
                Email = (x.Contact.ContactWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("email")) ? x.Contact.ContactWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("email")).FirstOrDefault().WebAddress : null),
                Twitter = (x.Contact.ContactWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")) ? x.Contact.ContactWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")).FirstOrDefault().WebAddress : null),
                PrimaryPhone = (x.Contact.ContactPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.Contact.ContactPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumber : null),
                PrimaryPhoneExtension = (x.Contact.ContactPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.Contact.ContactPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumberExtension : null),
                MinisterialJobTitle = x.Contact.MinisterialJobTitle.MinisterialJobTitleName,
                Ministry = (x.Contact.Ministry != null ? x.Contact.Ministry.DisplayName : null)
            });
            if (criteria.Count > 0 && companies != null && companies.Count > 0 && contacts == null)
            {
                //DateTime startDt = DateTime.Now;
                //var cons = GetContactsForCompanies(ctx, companies).ToList();
                //HttpContext.Current.Response.Write(DateTime.Now.Subtract(startDt).TotalMilliseconds + " for contactForCompanies ("+cons.Count+")<br/>\n");
                finalResults = finalResults.Union(GetContactsForCompanies(ctx, companies));
            }

            return finalResults.Distinct();
        }

        /// <summary>
        /// gets a list of the criteria that the supplied contact matched; used to display to the user so they know why a particular contact appears for a given set of search criteria
        /// </summary>
        /// <param name="csr">a contact search result object</param>
        /// <param name="criteria">a list of KeyValuePair of CriteriaType and string values representing the search parameters</param>
        /// <returns>a comma-separated string list of which search criteria the contact matches </returns>
        public string GetMatchString(ContactSearchResult csr, List<KeyValuePair<CriteriaType, string>> criteria, List<CompanySearchResult> outlets = null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<CriteriaType, string> crit in criteria)
            {
                switch (crit.Key)
                {
                    #region contact match any contact tab
                    //Contact Tab
                    case CriteriaType.Name:
                        bool matchedAllTerms = true;
                        List<string> terms = GetTerms(crit.Value);
                        foreach (string term in terms)
                        {
                            if (!csr.ContactName.ToLower().Contains(term.ToLower()))
                            {
                                matchedAllTerms = false;
                                break;
                            }
                        }
                        if (matchedAllTerms) sb.Append("Text: " + crit.Value + ", ");
                        break;
                    case CriteriaType.EmailOrWebsite:
                        if (csr.Contact.ContactWebAddresses.Any(t => t.WebAddress.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Email or Website: " + crit.Value + ", ");
                        break;
                    case CriteriaType.PhoneNumber:
                        if (csr.Contact.ContactPhoneNumbers.Any(t => t.PhoneNumber.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Phone Number: " + crit.Value + ", ");
                        break;
                    #endregion

                    #region contact match any location tab
                    //Location Tab
                    case CriteriaType.StreetAddress:
                        if (csr.Contact.ContactAddresses.Any(t => t.StreetAddress != null && t.StreetAddress.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Street Address: " + crit.Value + ", ");
                        break;
                    case CriteriaType.City:
                        Guid cgd = Guid.Parse(crit.Value);
                        var city = csr.Contact.ContactAddresses.FirstOrDefault(x => x.CityId == cgd);
                        if (city != null) sb.Append("City: " + city.City.CityName + ", ");
                        break;
                    case CriteriaType.OtherCity:
                        if (csr.Contact.ContactAddresses.Any(x => x.CityName != null && x.CityName.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Other City: " + crit.Value + ", ");
                        break;
                    case CriteriaType.Province:
                        Guid pgd = Guid.Parse(crit.Value);
                        var provState = csr.Contact.ContactAddresses.FirstOrDefault(x => x.ProvStateId == pgd);
                        if (provState != null) sb.Append("Prov/State: " + provState.ProvStateName + ", ");
                        break;
                    case CriteriaType.OtherProvince:
                        if (csr.Contact.ContactAddresses.Any(x => x.ProvStateName != null && x.ProvStateName.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Other Province: " + crit.Value + ", ");
                        break;
                    case CriteriaType.Beat:
                        Guid cId = Guid.Parse(crit.Value);
                        var beat = csr.Contact.ContactBeats.FirstOrDefault(x => x.BeatId == cId);
                        if (beat != null) sb.Append("Beat: " + beat.Beat.BeatName + ", ");
                        break;
                    case CriteriaType.Country:
                        Guid cygd = Guid.Parse(crit.Value);
                        var country = csr.Contact.ContactAddresses.FirstOrDefault(x => x.CountryId == cygd);
                        if (country != null) sb.Append("Country: " + country.Country.CountryName + ", ");
                        break;
                    case CriteriaType.PostalCode:
                        if (csr.Contact.ContactAddresses.Any(x => x.PostalZipCode != null && x.PostalZipCode.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Postal Code: " + crit.Value + ", ");
                        break;
                    case CriteriaType.ElectoralDistrict:
                        Guid edgd = Guid.Parse(crit.Value);
                        var district = csr.Contact.ElectoralDistricts.FirstOrDefault(ed => ed.Id == edgd);
                        if (district != null) sb.Append("Electoral District: " + district.DistrictName + ", ");
                        break;
                    case CriteriaType.Region:
                        Guid rggd = Guid.Parse(crit.Value);
                        var region = csr.Contact.Regions.FirstOrDefault(rg => rg.Id == rggd);
                        if (region != null) sb.Append("Region: " + region.RegionName + ", ");
                        break;
                    case CriteriaType.Sector:
                        Guid scgd = Guid.Parse(crit.Value);
                        var sector = csr.Contact.Sectors.FirstOrDefault(sec => sec.Id == scgd);
                        if (sector != null) sb.Append("Sector: " + sector.DisplayName + ", ");
                        break;
                    #endregion

                    #region contact match any media tab
                    //Media Tab
                    case CriteriaType.MediaJobTitle:
                        Guid mtgd = Guid.Parse(crit.Value);
                        var jobTitle = csr.Contact.ContactMediaJobTitles.FirstOrDefault(x => x.MediaJobTitleId == mtgd);
                        if (jobTitle != null) sb.Append("Media Job Title: " + jobTitle.MediaJobTitle.MediaJobTitleName + ", ");
                        break;
                    #endregion

                    #region contact match any ministry tab
                    //Ministry Tab
                    case CriteriaType.Ministry:
                    case CriteriaType.Minister:
                        Guid mngd = Guid.Parse(crit.Value);
                        if (csr.Contact.MinistryId == mngd) sb.Append("Ministry: " + csr.Contact.Ministry.DisplayName + ", ");
                        break;
                    case CriteriaType.MinisterialJobTitle:
                        Guid mjgd = Guid.Parse(crit.Value);
                        if (csr.Contact.MinisterialJobTitleId == mjgd) sb.Append("Ministerial Job Title: " + csr.Contact.MinisterialJobTitle.MinisterialJobTitleName + ", ");
                        break;
                    case CriteriaType.MLAAssignment:
                        Guid mlgd = Guid.Parse(crit.Value);
                        if (csr.Contact.MLAAssignmentId == mlgd) sb.Append("MLA Assignment: " + csr.Contact.ElectoralDistrict.DistrictName + ", ");
                        break;
                    #endregion

                    #region contact match any date and report type
                    //Dates and Report Types
                    case CriteriaType.ModifiedDateRange:
                        string dvalue = GetDateString(crit.Value);
                        string[] bothDates = dvalue.Split('|');
                        if (bothDates.Length == 2)
                        {
                            DateTime adt, bdt;
                            if (DateTime.TryParse(bothDates[1].Trim(), out bdt))
                                bdt = bdt.AddDays(1);
                            else
                                bdt = DateTime.MinValue;

                            if (DateTime.TryParse(bothDates[0].Trim(), out adt))
                            {
                                if (!bdt.Equals(DateTime.MinValue))
                                {
                                    if (csr.Contact.ModifiedDate < bdt) sb.Append("Modified Date Range, ");
                                }
                            }
                            else if (!bdt.Equals(DateTime.MinValue))
                            {
                                if (csr.Contact.ModifiedDate >= adt && csr.Contact.ModifiedDate < bdt) sb.Append("Modified Date Range, ");
                            }
                            else
                            {
                                if (csr.Contact.ModifiedDate >= adt) sb.Append("Modified Date Range, ");
                            }
                        }
                        break;
                    case CriteriaType.CreatedDateRange:
                        string cdvalue = GetDateString(crit.Value);
                        string[] cbothDates = cdvalue.Split('|');
                        if (cbothDates.Length == 2)
                        {
                            DateTime adt, bdt;
                            if (DateTime.TryParse(cbothDates[1].Trim(), out bdt))
                                bdt = bdt.AddDays(1);
                            else
                                bdt = DateTime.MinValue;

                            if (DateTime.TryParse(cbothDates[0].Trim(), out adt))
                            {
                                if (!bdt.Equals(DateTime.MinValue))
                                {
                                    if (csr.Contact.CreationDate < bdt) sb.Append("Creation Date Range, ");
                                }
                            }
                            else if (!bdt.Equals(DateTime.MinValue))
                            {
                                if (csr.Contact.CreationDate >= adt && csr.Contact.CreationDate < bdt) sb.Append("Creation Date Range, ");
                            }
                            else
                            {
                                if (csr.Contact.CreationDate >= adt) sb.Append("Creation Date Range, ");
                            }
                        }
                        break;

                    case CriteriaType.MediaCategory:
                        var rType = (ReportType)Enum.Parse(typeof(ReportType), crit.Value);
                        switch (rType)
                        {
                            case ReportType.PressGallery:
                                if (csr.Contact.IsPressGallery) sb.Append("Press Gallery, ");
                                break;
                        }
                        break;
                        #endregion
                }
            }

            #region contact match returned outlet
            if (outlets != null)
            {
                foreach (CompanySearchResult cmp in outlets)
                {
                    if (csr.Contact.ContactMediaJobTitles.Any(j => j.CompanyId == cmp.Company.Id))
                    {
                        sb.Append("Outlet: " + cmp.CompanyName + ", ");
                    }
                }
            }
            #endregion

            if (sb.Length > 0) sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        /// <summary>
        /// gets a list of the criteria that the supplied company matched; used to display to the user so they know why a particular company appears for a given set of search criteria
        /// </summary>
        /// <param name="csr">a company search result object</param>
        /// <param name="criteria">a list of KeyValuePair of CriteriaType and string values representing the search parameters</param>
        /// <returns>a comma-separated string list of which search criteria the company matches </returns>
        public string GetMatchString(CompanySearchResult csr, List<KeyValuePair<CriteriaType, string>> criteria, List<CompanySearchResult> companies = null)
        {
            //return "";

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<CriteriaType, string> crit in criteria)
            {
                switch (crit.Key)
                {
                    #region contact match any contact tab
                    //Contact Tab
                    case CriteriaType.Name:
                        //if (csr.CompanyName.ToLower().Contains(crit.Value.ToLower())) sb.Append("Text: " + crit.Value + ", ");
                        bool matchedAllTerms = true;
                        List<string> terms = GetTerms(crit.Value);
                        foreach (string term in terms)
                        {
                            if (!csr.CompanyName.ToLower().Contains(term.ToLower())) matchedAllTerms = false;
                        }
                        if (matchedAllTerms) sb.Append("Text: " + crit.Value + ", ");
                        break;
                    case CriteriaType.EmailOrWebsite:
                        if (csr.Company.CompanyWebAddresses.Any(t => t.WebAddress.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Email or Website: " + crit.Value + ", ");
                        break;
                    case CriteriaType.PhoneNumber:
                        if (csr.Company.CompanyPhoneNumbers.Any(t => t.PhoneNumber.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Phone Number: " + crit.Value + ", ");
                        break;
                    #endregion

                    #region contact match any location tab
                    //Location Tab
                    case CriteriaType.StreetAddress:
                        if (csr.Company.CompanyAddresses.Any(t => t.StreetAddress != null && t.StreetAddress.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Street Address: " + crit.Value + ", ");
                        break;
                    case CriteriaType.City:
                        Guid cgd = Guid.Parse(crit.Value);
                        var city = csr.Company.CompanyAddresses.FirstOrDefault(x => x.CityId == cgd);
                        if (city != null) sb.Append("City: " + city.City.CityName + ", ");
                        break;
                    case CriteriaType.OtherCity:
                        if (csr.Company.CompanyAddresses.Any(x => x.CityName != null && x.CityName.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Other City: " + crit.Value + ", ");
                        break;
                    case CriteriaType.Province:
                        Guid pgd = Guid.Parse(crit.Value);
                        var provState = csr.Company.CompanyAddresses.FirstOrDefault(x => x.ProvStateId == pgd);
                        if (provState != null) sb.Append("Prov/State: " + provState.ProvStateName + ", ");
                        break;
                    case CriteriaType.OtherProvince:
                        if (csr.Company.CompanyAddresses.Any(x => x.ProvStateName != null && x.ProvStateName.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Other Province: " + crit.Value + ", ");
                        break;
                    case CriteriaType.Country:
                        Guid cygd = Guid.Parse(crit.Value);
                        var country = csr.Company.CompanyAddresses.FirstOrDefault(x => x.CountryId == cygd);
                        if (country != null) sb.Append("Country: " + country.Country.CountryName + ", ");
                        break;
                    case CriteriaType.PostalCode:
                        if (csr.Company.CompanyAddresses.Any(x => x.PostalZipCode != null && x.PostalZipCode.ToLower().Contains(crit.Value.ToLower()))) sb.Append("Postal Code: " + crit.Value + ", ");
                        break;
                    case CriteriaType.ElectoralDistrict:
                        Guid edgd = Guid.Parse(crit.Value);
                        var district = csr.Company.ElectoralDistricts.FirstOrDefault(ed => ed.Id == edgd);
                        if (district != null) sb.Append("Electoral District: " + district.DistrictName + ", ");
                        break;
                    case CriteriaType.Region:
                        Guid rggd = Guid.Parse(crit.Value);
                        var region = csr.Company.Regions.FirstOrDefault(rg => rg.Id == rggd);
                        if (region != null) sb.Append("Region: " + region.RegionName + ", ");
                        break;
                    case CriteriaType.Sector:
                        Guid scgd = Guid.Parse(crit.Value);
                        var sector = csr.Company.Sectors.FirstOrDefault(sec => sec.Id == scgd);
                        if (sector != null) sb.Append("Sector: " + sector.DisplayName + ", ");
                        break;
                    #endregion

                    #region contact match any media tab
                    //Media Tab
                    case CriteriaType.MediaDesk:
                        Guid mdgd = Guid.Parse(crit.Value);
                        var mediaDesk = csr.Company.MediaDesks.FirstOrDefault(desk => desk.Id == mdgd);
                        if (mediaDesk != null) sb.Append("Media Desk: " + mediaDesk.MediaDeskName + ", ");
                        break;
                    case CriteriaType.Distribution:
                        Guid dtgd = Guid.Parse(crit.Value);
                        var distribution = csr.Company.Distributions.FirstOrDefault(dist => dist.Id == dtgd);
                        if (distribution != null) sb.Append("Distribution: " + distribution.DistributionName + ", ");
                        break;
                    case CriteriaType.Language:
                        Guid lggd = Guid.Parse(crit.Value);
                        var language = csr.Company.Languages.FirstOrDefault(lang => lang.Id == lggd);
                        if (language != null) sb.Append("Language: " + language.LanguageName + ", ");
                        break;
                    case CriteriaType.PublicationDays:
                        Guid pdgd = Guid.Parse(crit.Value);
                        var publicationDays = csr.Company.PublicationDays.FirstOrDefault(pday => pday.Id == pdgd);
                        if (publicationDays != null) sb.Append("Publication Days: " + publicationDays.PublicationDaysName + ", ");
                        break;
                    case CriteriaType.SpecialtyPublication:
                        Guid spgd = Guid.Parse(crit.Value);
                        var specialtyPublication = csr.Company.SpecialtyPublications.FirstOrDefault(spub => spub.Id == spgd);
                        if (specialtyPublication != null) sb.Append("Specialty Publication: " + specialtyPublication.SpecialtyPublicationName + ", ");
                        break;
                    case CriteriaType.MediaType:
                        Guid mtgd = Guid.Parse(crit.Value);
                        var mediaType = csr.Company.MediaTypes.FirstOrDefault(mtype => mtype.Id == mtgd);
                        if (mediaType != null) sb.Append("Media Type: " + mediaType.MediaTypeName + ", ");
                        break;
                    case CriteriaType.PrintCategory:
                        Guid pcgd = Guid.Parse(crit.Value);
                        if (csr.Company.PrintCategoryId == pcgd) sb.Append("Print Category: " + csr.Company.PrintCategory.PrintCategoryName + ", ");
                        break;
                    case CriteriaType.Ethnicity:
                        Guid etgd = Guid.Parse(crit.Value);
                        var ethnicity = csr.Company.Ethnicities.FirstOrDefault(ethn => ethn.Id == etgd);
                        if (ethnicity != null) sb.Append("Ethnicity: " + ethnicity.EthnicityName + ", ");
                        break;
                    case CriteriaType.PublicationFrequency:
                        Guid pfgd = Guid.Parse(crit.Value);
                        if (csr.Company.PublicationFrequencyId == pfgd) sb.Append("Publication Frequency: " + csr.Company.PublicationFrequency.PublicationFrequencyName + ", ");
                        break;
                    #endregion

                    #region contact match any date and report type
                    //Dates and Report Types
                    case CriteriaType.ModifiedDateRange:
                        string dvalue = GetDateString(crit.Value);
                        string[] bothDates = dvalue.Split('|');
                        if (bothDates.Length == 2)
                        {
                            DateTime adt, bdt;
                            if (DateTime.TryParse(bothDates[1].Trim(), out bdt))
                                bdt = bdt.AddDays(1);
                            else
                                bdt = DateTime.MinValue;

                            if (!DateTime.TryParse(bothDates[0].Trim(), out adt))
                            {
                                if (!bdt.Equals(DateTime.MinValue))
                                {
                                    if (csr.Company.ModifiedDate < bdt) sb.Append("Modified Date Range, ");
                                }
                            }
                            else if (!bdt.Equals(DateTime.MinValue))
                            {
                                if (csr.Company.ModifiedDate >= adt && csr.Company.ModifiedDate < bdt) sb.Append("Modified Date Range, ");
                            }
                            else
                            {
                                if (csr.Company.ModifiedDate >= adt) sb.Append("Modified Date Range, ");
                            }
                        }
                        break;
                    case CriteriaType.CreatedDateRange:
                        string cdvalue = GetDateString(crit.Value);
                        string[] cbothDates = cdvalue.Split('|');
                        if (cbothDates.Length == 2)
                        {
                            DateTime adt, bdt;
                            if (DateTime.TryParse(cbothDates[1].Trim(), out bdt))
                                bdt = bdt.AddDays(1);
                            else
                                bdt = DateTime.MinValue;

                            if (!DateTime.TryParse(cbothDates[0].Trim(), out adt))
                            {
                                if (!bdt.Equals(DateTime.MinValue))
                                {
                                    if (csr.Company.CreationDate < bdt) sb.Append("Creation Date Range, ");
                                }
                            }
                            else if (!bdt.Equals(DateTime.MinValue))
                            {
                                if (csr.Company.CreationDate >= adt && csr.Company.CreationDate < bdt) sb.Append("Creation Date Range, ");
                            }
                            else
                            {
                                if (csr.Company.CreationDate >= adt) sb.Append("Creation Date Range, ");
                            }
                        }
                        break;

                    case CriteriaType.MediaCategory:
                        var rType = (ReportType)Enum.Parse(typeof(ReportType), crit.Value);
                        switch (rType)
                        {
                            case ReportType.EthnicMedia:
                                if (csr.Company.IsEthnicMedia == true) sb.Append("Ethnic Media, ");
                                break;
                            case ReportType.MajorMedia:
                                if (csr.Company.IsMajorMedia == true) sb.Append("Major Media, ");
                                break;
                        }
                        break;
                        #endregion
                }
            }

            #region outlet match returned company
            if (companies != null)
            {
                foreach (CompanySearchResult cmp in companies)
                {
                    if (csr.Company.ParentCompanyId == cmp.Company.Id)
                    {
                        sb.Append("Company: " + cmp.CompanyName + ", ");
                    }
                }
            }
            #endregion

            if (sb.Length > 0) sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        /// <summary>
        /// Searches for outlets based on the supplied criteria (this calls the below CompanySearch method)
        /// </summary>
        /// <param name="criteria">a list of criteria types and values to search</param>
        /// <param name="ctx">database context to use</param>
        /// <param name="matchAll">if true, all criteria must be matched, if false, any one or more must be matched</param>
        /// <param name="companies"></param>
        /// <returns>distinct IQueryable collection of Company objects matching the search criteria</returns>
        public IQueryable<CompanySearchResult> OutletSearch(List<KeyValuePair<CriteriaType, string>> criteria, MediaRelationsEntities ctx, bool matchAll, IList<CompanySearchResult> companies = null)
        {
            return CompanySearch(criteria, ctx, matchAll, companies);
        }

        /// <summary>
        /// Searches for companies (if companies is null otherwise it searches for outlets) based on the supplied criteria
        /// </summary>
        /// <param name="criteria">a list of criteria types and values to search</param>
        /// <param name="ctx">database context to use</param>
        /// <param name="matchAll">if true, all criteria must be matched, if false, any one or more must be matched</param>
        /// <param name="companies"></param>
        /// <returns>distinct IQueryable collection of Company objects matching the search criteria</returns>
        public IQueryable<CompanySearchResult> CompanySearch(List<KeyValuePair<CriteriaType, string>> criteria, MediaRelationsEntities ctx, bool matchAll, IList<CompanySearchResult> companies = null)
        {
            Expression<Func<IntermediateCompanySearchResult, bool>> searchExpression = null;
            if (criteria.Count > 0)
            {
                if (!matchAll)
                {
                    searchExpression = (s => false);
                }
                else if (IsContactOnlySearch(criteria) && !IsCompanyOnlySearch(criteria))
                {
                    //return empty result set
                    return Enumerable.Empty<CompanySearchResult>().AsQueryable();
                }
            }
            bool isOutlet = companies != null;

            var results = (from cmp in ctx.Companies
                           from cwa in cmp.CompanyWebAddresses.DefaultIfEmpty()
                           from cpn in cmp.CompanyPhoneNumbers.DefaultIfEmpty()
                           from ed in cmp.ElectoralDistricts.DefaultIfEmpty()
                           from reg in cmp.Regions.DefaultIfEmpty()
                           from sec in cmp.Sectors.DefaultIfEmpty()
                           from mdk in cmp.MediaDesks.DefaultIfEmpty()
                           from dtr in cmp.Distributions.DefaultIfEmpty()
                           from lng in cmp.Languages.DefaultIfEmpty()
                           from pdy in cmp.PublicationDays.DefaultIfEmpty()
                           from spb in cmp.SpecialtyPublications.DefaultIfEmpty()
                           from mtp in cmp.MediaTypes.DefaultIfEmpty()
                           from eth in cmp.Ethnicities.DefaultIfEmpty()
                           from pmp in ctx.Companies.Where(c => c.Id == cmp.ParentCompanyId).DefaultIfEmpty()
                           where cmp.IsOutlet == isOutlet && cmp.IsActive
                           select new IntermediateCompanySearchResult
                           {
                               Company = cmp,
                               ParentCompany = pmp,
                               WebAddress = cwa,
                               PhoneNumber = cpn,
                               District = ed,
                               Region = reg,
                               Sector = sec,
                               MediaDesk = mdk,
                               Distribution = dtr,
                               Language = lng,
                               PublicationDays = pdy,
                               SpecialtyPublication = spb,
                               MediaType = mtp,
                               Ethnicity = eth
                           });

            foreach (KeyValuePair<CriteriaType, string> cri in criteria)
            {
                string value = cri.Value;
                Expression<Func<IntermediateCompanySearchResult, bool>> critExpression = null;
                switch (cri.Key)
                {
                    #region company match any contact
                    case CriteriaType.Name:
                        List<string> terms = GetTerms(value);
                        foreach (string term in terms)
                        {
                            Expression<Func<IntermediateCompanySearchResult, bool>> termOrExpression = s => s.Company.CompanyName.Contains(term);
                            critExpression = critExpression == null ? termOrExpression : critExpression.And(termOrExpression);
                        }
                        break;
                    case CriteriaType.EmailOrWebsite:
                        critExpression = s => s.WebAddress.WebAddress.Contains(value);
                        break;
                    case CriteriaType.PhoneNumber:
                        critExpression = s => s.PhoneNumber.PhoneNumber.Contains(value);
                        break;
                    #endregion

                    #region company match any location

                    case CriteriaType.City:
                        Guid cgd = Guid.Parse(value);
                        critExpression = s => s.Company.CompanyAddresses.Any(x => x.CityId == cgd);
                        break;
                    case CriteriaType.OtherCity:
                        critExpression = s => s.Company.CompanyAddresses.Any(x => x.CityName.Contains(value));
                        break;
                    case CriteriaType.Province:
                        Guid pgd = Guid.Parse(value);
                        critExpression = s => s.Company.CompanyAddresses.Any(x => x.ProvStateId == pgd);
                        break;
                    case CriteriaType.OtherProvince:
                        critExpression = s => s.Company.CompanyAddresses.Any(x => x.ProvStateName.Contains(value));
                        break;
                    case CriteriaType.Country:
                        Guid cygd = Guid.Parse(value);
                        critExpression = s => s.Company.CompanyAddresses.Any(x => x.CountryId == cygd);
                        break;
                    case CriteriaType.PostalCode:
                        critExpression = s => s.Company.CompanyAddresses.Any(x => x.PostalZipCode.Contains(value));
                        break;
                    case CriteriaType.ElectoralDistrict:
                        Guid edgd = Guid.Parse(value);
                        critExpression = s => s.District.Id == edgd;
                        break;
                    case CriteriaType.Region:
                        Guid rggd = Guid.Parse(value);
                        critExpression = s => s.Region.Id == rggd;
                        break;
                    case CriteriaType.Sector:
                        Guid scgd = Guid.Parse(value);
                        critExpression = s => s.Sector.Id == scgd;
                        break;
                    #endregion

                    #region company match any media
                    case CriteriaType.MediaDesk:
                        Guid mdgd = Guid.Parse(value);
                        critExpression = s => s.MediaDesk.Id == mdgd;
                        break;
                    case CriteriaType.Distribution:
                        Guid dtgd = Guid.Parse(value);
                        critExpression = s => s.Distribution.Id == dtgd;
                        break;
                    case CriteriaType.Language:
                        Guid lggd = Guid.Parse(value);
                        critExpression = s => s.Language.Id == lggd;
                        break;
                    case CriteriaType.PublicationDays:
                        Guid pdgd = Guid.Parse(value);
                        critExpression = s => s.PublicationDays.Id == pdgd;
                        break;
                    case CriteriaType.SpecialtyPublication:
                        Guid spgd = Guid.Parse(value);
                        critExpression = s => s.SpecialtyPublication.Id == spgd;
                        break;
                    case CriteriaType.MediaType:
                        Guid mtgd = Guid.Parse(value);
                        critExpression = s => s.MediaType.Id == mtgd;
                        break;
                    case CriteriaType.PrintCategory:
                        Guid pcgd = Guid.Parse(value);
                        critExpression = s => s.Company.PrintCategoryId == pcgd;
                        break;
                    case CriteriaType.Ethnicity:
                        Guid etgd = Guid.Parse(value);
                        critExpression = s => s.Ethnicity.Id == etgd;
                        break;
                    case CriteriaType.PublicationFrequency:
                        Guid pfgd = Guid.Parse(value);
                        critExpression = s => s.Company.PublicationFrequencyId == pfgd;
                        break;
                    #endregion

                    #region company match any dates and report types
                    case CriteriaType.ModifiedDateRange:
                        string dvalue = GetDateString(value);
                        string[] bothDates = dvalue.Split('|');
                        if (bothDates.Length == 2)
                        {
                            DateTime adt, bdt;
                            if (DateTime.TryParse(bothDates[1].Trim(), out bdt))
                                bdt = bdt.AddDays(1);
                            else
                                bdt = DateTime.MinValue;

                            if (DateTime.TryParse(bothDates[0].Trim(), out adt))
                            {
                                if (!bdt.Equals(DateTime.MinValue))
                                {
                                    critExpression = (s => s.Company.ModifiedDate < bdt);
                                }
                            }
                            else if (!bdt.Equals(DateTime.MinValue))
                            {
                                critExpression = (s => s.Company.ModifiedDate >= adt && s.Company.ModifiedDate < bdt);
                            }
                            else
                            {
                                critExpression = (s => s.Company.ModifiedDate >= adt);
                            }
                        }
                        break;
                    case CriteriaType.CreatedDateRange:
                        string cdvalue = GetDateString(value);
                        string[] cbothDates = cdvalue.Split('|');
                        if (cbothDates.Length == 2)
                        {
                            DateTime adt, bdt;
                            if (DateTime.TryParse(cbothDates[1].Trim(), out bdt))
                                bdt = bdt.AddDays(1);
                            else
                                bdt = DateTime.MinValue;

                            if (DateTime.TryParse(cbothDates[0].Trim(), out adt))
                            {
                                if (!bdt.Equals(DateTime.MinValue))
                                {
                                    critExpression = (s => s.Company.CreationDate < bdt);
                                }
                            }
                            else if (!bdt.Equals(DateTime.MinValue))
                            {
                                critExpression = (s => s.Company.CreationDate >= adt && s.Company.CreationDate < bdt);
                            }
                            else
                            {
                                critExpression = (s => s.Company.CreationDate >= adt);
                            }
                        }
                        break;

                    case CriteriaType.MediaCategory:
                        var rType = (ReportType)Enum.Parse(typeof(ReportType), value);
                        switch (rType)
                        {
                            case ReportType.EthnicMedia:
                                critExpression = (s => s.Company.IsEthnicMedia == true);
                                break;
                            case ReportType.MajorMedia:
                                critExpression = (s => s.Company.IsMajorMedia == true);
                                break;
                            case ReportType.LiveTalkOpportunities:
                                critExpression = (s => s.Company.IsLiveMedia == true);
                                break;
                        }
                        break;
                        #endregion
                }
                if (critExpression != null)
                {
                    if (searchExpression == null) searchExpression = critExpression;
                    else searchExpression = matchAll ? searchExpression.And(critExpression) : searchExpression.Or(critExpression);
                }
            }
            if (searchExpression != null)
            {
                results = results.Where(searchExpression);
            }

            IQueryable<CompanySearchResult> finalResults = results.Select(x => new CompanySearchResult
            {
                Company = x.Company,
                CompanyName = x.Company.CompanyName,
                ParentCompanyName = (x.ParentCompany == null ? null : x.ParentCompany.CompanyName),

                City = x.Company.CompanyAddresses.Where(t => t.City != null).OrderBy(t => t.AddressType).Select(t => t.City.CityName).FirstOrDefault(),
                Email = (x.Company.CompanyWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("email")) ? x.Company.CompanyWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("email")).FirstOrDefault().WebAddress : null),
                Twitter = (x.Company.CompanyWebAddresses.Any(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")) ? x.Company.CompanyWebAddresses.Where(t => t.WebAddressType.WebAddressTypeName.Contains("twitter")).FirstOrDefault().WebAddress : null),
                PrimaryPhone = (x.Company.CompanyPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.Company.CompanyPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumber : null),
                PrimaryPhoneExtension = (x.Company.CompanyPhoneNumbers.Any(t => t.PhoneType.PhoneTypeName.Contains("primary")) ? x.Company.CompanyPhoneNumbers.Where(t => t.PhoneType.PhoneTypeName.Contains("primary")).FirstOrDefault().PhoneNumberExtension : null),
                FirstMediaType = (x.Company.MediaTypes.Count > 0 ? x.Company.MediaTypes.FirstOrDefault().MediaTypeName : null)
            });

            if (criteria.Count > 0 && companies != null && companies.Count > 0)
            {
                finalResults = finalResults.Union(GetOutletsForCompanies(ctx, companies));
            }
            return finalResults.Distinct();
        }
    }
}
