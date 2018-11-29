using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data.Entity;
using MediaRelationsDatabase;
using static MediaRelationsLibrary.CommonEventLogging;

namespace MediaRelationsLibrary
{
    public static class DataListAdminLib
    {
        private static readonly object staticSync = new object();

        /// <summary>
        /// This method is used to delete a region from the database, this will affect
        /// subscribers and media outlets
        /// </summary>
        /// <param name="guid">The guid of the region to delete</param>
        /// <returns>a bool flag of success for the deletion as the key, string is the error message</returns>
        public static KeyValuePair<bool, string> DeleteRegion(Guid guid)
        {
            bool returnVal = false;
            string message = "";

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Region region = (from reg in ctx.Regions where reg.Id == guid select reg).FirstOrDefault();

                if (region != null)
                {
                    // todo implement the delete
                    bool canDelete = true;

                    if (region.Companies.Count != 0)
                    {
                        message = "Cannot delete a region attached to a company (" + region.RegionName + ")";
                        canDelete = false;
                    }

                    if (region.Contacts.Count != 0)
                    {
                        if (!canDelete) message += ", ";
                        message += "Cannot delete a region attached to a subscriber (" + region.RegionName + ")";
                        canDelete = false;
                    }

                    if (canDelete)
                    {
                        ctx.Regions.Remove(region);
                        ctx.SaveChanges();

                        WriteActivityLogEntry(ActivityType.Region_Deleted, EntityType.Region, region.Id, region.RegionName, Guid.Empty, "", CommonMethods.GetLoggedInUser());

                        returnVal = true;
                    }

                }
                else
                {
                    message = "Error deleting region: not found";
                }
            }

            return new KeyValuePair<bool, string>(returnVal, message);
        }

        /// <summary>
        /// This method is called to create or edit a region in the media advisory subscription service.
        /// Does validation to ensure that region name is not duplicated
        /// </summary>
        /// <param name="regionName">the name of the created/edited region</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditRegion(string regionName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(regionName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Region region;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Region_Modified;

                    region = (from reg in ctx.Regions where reg.Id == guid.Value select reg).FirstOrDefault();
                    if (region == null) return errors | 1024;
                    oldName = region.RegionName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Region_Added;
                    region = new Region() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.Regions.Add(region);
                }

                if (errors == 0)
                {
                    if (oldName != regionName)
                    {
                        int count = (from c in ctx.Regions where c.RegionName == regionName select c).Count();
                        if (count > 0) return errors | 8;
                        region.RegionName = regionName;
                    }
                    region.ModifiedDate = DateTime.Now;
                    region.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.Region, region.Id, regionName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a city in the media advisory subscription service.
        /// This class also does validation of a valid city name.
        /// </summary>
        /// <param name="cityName">the name of the created/edited city</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditCity(string cityName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(cityName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                City city;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.City_Modified;

                    city = (from c in ctx.Cities where c.Id == guid.Value select c).FirstOrDefault();
                    if (city == null) return errors | 1024;
                    oldName = city.CityName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.City_Added;
                    city = new City() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.Cities.Add(city);
                }

                if (errors == 0)
                {
                    if (oldName != cityName)
                    {
                        int count = (from c in ctx.Cities where c.CityName == cityName select c).Count();
                        if (count > 0) return errors | 8;
                        city.CityName = cityName;
                    }
                    city.ModifiedDate = DateTime.Now;
                    city.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.City, city.Id, cityName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a ethnicity in the media advisory subscription service.
        /// Does validation to ensure that ethnicity name is not duplicated
        /// </summary>
        /// <param name="ethnicityName">the name of the created/edited ethnicity</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditEthnicity(string ethnicityName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(ethnicityName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Ethnicity ethnicity;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Ethnicity_Modified;

                    ethnicity = (from eth in ctx.Ethnicities where eth.Id == guid.Value select eth).FirstOrDefault();
                    if (ethnicity == null) return errors | 1024;
                    oldName = ethnicity.EthnicityName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Ethnicity_Added;
                    ethnicity = new Ethnicity() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.Ethnicities.Add(ethnicity);
                }

                if (errors == 0)
                {
                    if (oldName != ethnicityName)
                    {
                        int count = (from c in ctx.Ethnicities where c.EthnicityName == ethnicityName select c).Count();
                        if (count > 0) return errors | 8;
                        ethnicity.EthnicityName = ethnicityName;
                    }
                    ethnicity.ModifiedDate = DateTime.Now;
                    ethnicity.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.Ethnicity, ethnicity.Id, ethnicityName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a language in the media advisory subscription service.
        /// Does validation to ensure that language name is not duplicated
        /// </summary>
        /// <param name="languageName">the name of the created/edited language</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditLanguage(string languageName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(languageName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Language language;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Language_Modified;

                    language = (from lang in ctx.Languages where lang.Id == guid.Value select lang).FirstOrDefault();
                    if (language == null) return errors | 1024;
                    oldName = language.LanguageName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Language_Added;
                    language = new Language() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.Languages.Add(language);
                }

                if (errors == 0)
                {
                    if (oldName != languageName)
                    {
                        int count = (from c in ctx.Languages where c.LanguageName == languageName select c).Count();
                        if (count > 0) return errors | 8;
                        language.LanguageName = languageName;
                    }
                    language.ModifiedDate = DateTime.Now;
                    language.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.Language, language.Id, languageName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a mediaType in the media advisory subscription service.
        /// Does validation to ensure that mediaType name is not duplicated
        /// </summary>
        /// <param name="mediaTypeName">the name of the created/edited mediaType</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditMediaType(string mediaTypeName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(mediaTypeName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                MediaType mediaType;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.MediaType_Modified;

                    mediaType = (from mtype in ctx.MediaTypes where mtype.Id == guid.Value select mtype).FirstOrDefault();
                    if (mediaType == null) return errors | 1024;
                    oldName = mediaType.MediaTypeName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.MediaType_Added;
                    mediaType = new MediaType() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.MediaTypes.Add(mediaType);
                }

                if (errors == 0)
                {
                    if (oldName != mediaTypeName)
                    {
                        int count = (from c in ctx.MediaTypes where c.MediaTypeName == mediaTypeName select c).Count();
                        if (count > 0) return errors | 8;
                        mediaType.MediaTypeName = mediaTypeName;
                    }
                    mediaType.ModifiedDate = DateTime.Now;
                    mediaType.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.MediaType, mediaType.Id, mediaTypeName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }


        /// <summary>
        /// This method is used to delete an electoral district from the database, this will affect
        /// media outlets
        /// </summary>
        /// <param name="guid">The guid of the electoral district to delete</param>
        /// <returns>a bool flag of success for the deletion as key, value is an error message</returns>
        public static KeyValuePair<bool, string> DeleteElectoralDistrict(Guid guid)
        {
            bool returnVal = false;
            string message = "";

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                ElectoralDistrict district = (from dist in ctx.ElectoralDistricts where dist.Id == guid select dist).FirstOrDefault();

                if (district != null)
                {
                    // todo implement the delete
                    bool canDelete = true;

                    if (district.Companies.Where(t => t.IsActive).Count() != 0)
                    {
                        message = "You cannot delete an electoral district with a media outlet attached to it (" + district.DistrictName + ")";
                        canDelete = false;
                    }

                    if (district.Contacts.Where(t => t.IsActive).Count() != 0)
                    {
                        if (!canDelete) message += ", ";
                        message += "You cannot delete an electoral district with a contact subscribed to it (" + district.DistrictName + ")";
                        canDelete = false;
                    }

                    if (canDelete)
                    {
                        foreach (var cmp in district.Companies.Where(t => !t.IsActive))
                        {
                            cmp.ElectoralDistricts.Clear();
                        }

                        foreach (var con in district.Contacts.Where(t => !t.IsActive))
                        {
                            con.ElectoralDistricts.Clear();
                            con.MLAAssignmentId = null;
                        }

                        ctx.SaveChanges();

                        ctx.ElectoralDistricts.Remove(district);
                        ctx.SaveChanges();

                        WriteActivityLogEntry(ActivityType.Electoral_District_Deleted, EntityType.ElectoralDistrict, district.Id, district.DistrictName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                        returnVal = true;
                    }
                }
                else
                {
                    message = "Error deleting electoral district: not found";
                }
            }

            return new KeyValuePair<bool, string>(returnVal, message);
        }

        /// <summary>
        /// This method is called to create or edit an electoral district in the media advisory subscription service.
        /// Does validation to ensure that electoral district name is not duplicated
        /// </summary>
        /// <param name="districtName">the name of the created/edited electoral district</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditElectoralDistrict(string districtName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(districtName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                ElectoralDistrict district;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Electoral_District_Modified;

                    district = (from dist in ctx.ElectoralDistricts where dist.Id == guid.Value select dist).FirstOrDefault();
                    if (district == null) return errors | 1024;
                    oldName = district.DistrictName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Electoral_District_Added;
                    district = new ElectoralDistrict() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.ElectoralDistricts.Add(district);
                }

                if (errors == 0)
                {
                    if (oldName != districtName)
                    {
                        int count = (from c in ctx.ElectoralDistricts where c.DistrictName == districtName select c).Count();
                        if (count > 0) return errors | 8;
                        district.DistrictName = districtName;
                    }
                    district.ModifiedDate = DateTime.Now;
                    district.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.ElectoralDistrict, district.Id, districtName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }


        /// <summary>
        /// This method is called to create or edit a country in the media advisory subscription service.
        /// Does validation to ensure that country name or abbrev are not duplicated
        /// </summary>
        /// <param name="countryName">the name of the created/edited country</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditCountry(string countryName, string countryAbbrev, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(countryName)) errors |= 2;
            if (string.IsNullOrEmpty(countryAbbrev)) errors |= 4;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Country country;
                string oldName = null, oldAbbrev = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Country_Modified;

                    country = (from c in ctx.Countries where c.Id == guid.Value select c).FirstOrDefault();
                    if (country == null) return errors | 1024;
                    oldName = country.CountryName;
                    oldAbbrev = country.CountryAbbrev;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Country_Added;
                    country = new Country() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.Countries.Add(country);
                }

                if (errors == 0)
                {
                    if (oldName != countryName)
                    {
                        int count = (from c in ctx.Countries where c.CountryName == countryName select c).Count();
                        if (count > 0) return errors | 8;
                        country.CountryName = countryName;
                    }
                    if (oldAbbrev != countryAbbrev)
                    {
                        int count = (from c in ctx.Countries where c.CountryAbbrev == countryAbbrev select c).Count();
                        if (count > 0) return errors | 8;
                        country.CountryAbbrev = countryAbbrev;
                    }
                    country.ModifiedDate = DateTime.Now;
                    country.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.Country, country.Id, countryName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a provState in the media advisory subscription service.
        /// Does validation to ensure that provState name or abbrev are not duplicated
        /// </summary>
        /// <param name="provStateName">the name of the created/edited province or state</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditProvState(string provStateName, string provStateAbbrev, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(provStateName)) errors |= 2;
            if (string.IsNullOrEmpty(provStateAbbrev)) errors |= 4;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                ProvState provState;
                string oldName = null, oldAbbrev = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.ProvState_Modified;

                    provState = (from prov in ctx.ProvStates where prov.Id == guid.Value select prov).FirstOrDefault();
                    if (provState == null) return errors | 1024;
                    oldName = provState.ProvStateName;
                    oldAbbrev = provState.ProvStateAbbrev;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.ProvState_Added;
                    provState = new ProvState() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.ProvStates.Add(provState);
                }

                if (errors == 0)
                {
                    if (oldName != provStateName)
                    {
                        int count = (from c in ctx.ProvStates where c.ProvStateName == provStateName select c).Count();
                        if (count > 0) return errors | 8;
                        provState.ProvStateName = provStateName;
                    }
                    if (oldAbbrev != provStateAbbrev)
                    {
                        int count = (from c in ctx.ProvStates where c.ProvStateAbbrev == provStateAbbrev select c).Count();
                        if (count > 0) return errors | 8;
                        provState.ProvStateAbbrev = provStateAbbrev;
                    }
                    provState.ModifiedDate = DateTime.Now;
                    provState.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.ProvState, provState.Id, provStateName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a distribution in the media advisory subscription service.
        /// Does validation to ensure that distribution name is not duplicated
        /// </summary>
        /// <param name="distributionName">the name of the created/edited distribution</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditDistribution(string distributionName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(distributionName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Distribution distribution;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Distribution_Modified;

                    distribution = (from dist in ctx.Distributions where dist.Id == guid.Value select dist).FirstOrDefault();
                    if (distribution == null) return errors | 1024;
                    oldName = distribution.DistributionName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Distribution_Added;
                    distribution = new Distribution() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.Distributions.Add(distribution);
                }

                if (errors == 0)
                {
                    if (oldName != distributionName)
                    {
                        int count = (from c in ctx.Distributions where c.DistributionName == distributionName select c).Count();
                        if (count > 0) return errors | 8;
                        distribution.DistributionName = distributionName;
                    }
                    distribution.ModifiedDate = DateTime.Now;
                    distribution.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.Distribution, distribution.Id, distributionName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a mediaDesk in the media advisory subscription service.
        /// Does validation to ensure that mediaDesk name is not duplicated
        /// </summary>
        /// <param name="mediaDeskName">the name of the created/edited mediaDesk</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditMediaDesk(string mediaDeskName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(mediaDeskName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                MediaDesk mediaDesk;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.MediaDesk_Modified;

                    mediaDesk = (from mdesk in ctx.MediaDesks where mdesk.Id == guid.Value select mdesk).FirstOrDefault();
                    if (mediaDesk == null) return errors | 1024;
                    oldName = mediaDesk.MediaDeskName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.MediaDesk_Added;
                    mediaDesk = new MediaDesk() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.MediaDesks.Add(mediaDesk);
                }

                if (errors == 0)
                {
                    if (oldName != mediaDeskName)
                    {
                        int count = (from c in ctx.MediaDesks where c.MediaDeskName == mediaDeskName select c).Count();
                        if (count > 0) return errors | 8;
                        mediaDesk.MediaDeskName = mediaDeskName;
                    }
                    mediaDesk.ModifiedDate = DateTime.Now;
                    mediaDesk.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.MediaDesk, mediaDesk.Id, mediaDeskName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is used to delete a mediaJobTitle from the database, this will affect
        /// media outlets
        /// </summary>
        /// <param name="guid">The guid of the mediaJobTitle to delete</param>
        /// <returns>a bool flag of success for the deletion as the key, the value is a message of the error</returns>
        public static KeyValuePair<bool, string> DeleteMediaJobTitle(Guid guid)
        {
            bool returnVal = false;
            string message = "";

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                MediaJobTitle mediaJobTitle = (from jtitle in ctx.MediaJobTitles where jtitle.Id == guid select jtitle).FirstOrDefault();

                if (mediaJobTitle != null)
                {
                    // todo implement the delete
                    bool canDelete = true;

                    if (mediaJobTitle.ContactMediaJobTitles.Count() > 0)
                    {
                        canDelete = false;
                        message = "The Media Job Title is associated with contacts, and cannot be deleted.";
                    }

                    if (canDelete)
                    {
                        ctx.MediaJobTitles.Remove(mediaJobTitle);
                        ctx.SaveChanges();
                        WriteActivityLogEntry(ActivityType.Media_Job_Deleted, EntityType.MediaJobTitle, mediaJobTitle.Id, mediaJobTitle.MediaJobTitleName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                        returnVal = true;
                    }
                }
                else
                {
                    message = "Error deleting mediaJobTitle: not found";
                }
            }

            return new KeyValuePair<bool, string>(returnVal, message);
        }

        /// <summary>
        /// This method is called to create or edit a mediaJobTitle in the media advisory subscription service.
        /// Does validation to ensure that mediaJobTitle name is not duplicated
        /// </summary>
        /// <param name="mediaJobTitleName">the name of the created/edited mediaJobTitle</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditMediaJobTitle(string mediaJobTitleName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(mediaJobTitleName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                MediaJobTitle mediaJobTitle;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Media_Job_Modified;

                    mediaJobTitle = (from jtitle in ctx.MediaJobTitles where jtitle.Id == guid.Value select jtitle).FirstOrDefault();
                    if (mediaJobTitle == null) return errors | 1024;
                    oldName = mediaJobTitle.MediaJobTitleName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Media_Job_Added;
                    mediaJobTitle = new MediaJobTitle() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.MediaJobTitles.Add(mediaJobTitle);
                }

                if (errors == 0)
                {
                    if (oldName != mediaJobTitleName)
                    {
                        int count = (from c in ctx.MediaJobTitles where c.MediaJobTitleName == mediaJobTitleName select c).Count();
                        if (count > 0) return errors | 8;
                        mediaJobTitle.MediaJobTitleName = mediaJobTitleName;
                    }
                    mediaJobTitle.ModifiedDate = DateTime.Now;
                    mediaJobTitle.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.MediaJobTitle, mediaJobTitle.Id, mediaJobTitleName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is used to delete a ministerialJobTitle from the database, this will affect
        /// media outlets
        /// </summary>
        /// <param name="guid">The guid of the ministerialJobTitle to delete</param>
        /// <returns>a bool flag of success for the deletion as the key, the value is a message of the error</returns>
        public static KeyValuePair<bool, string> DeleteMinisterialJobTitle(Guid guid)
        {
            bool returnVal = false;
            string message = "";

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                MinisterialJobTitle ministerialJobTitle = (from jtitle in ctx.MinisterialJobTitles  where jtitle.Id == guid select jtitle).FirstOrDefault();

                if (ministerialJobTitle != null)
                {
                    // todo implement the delete
                    bool canDelete = true;

                    if (ministerialJobTitle.Contacts.Where(t => t.IsActive).Count() > 0)
                    {
                        canDelete = false;
                        message = "The Ministerial Job Title is associated with contacts, and cannot be deleted.";
                    }


                    if (canDelete)
                    {
                        foreach (var con in ministerialJobTitle.Contacts.Where(t => !t.IsActive))
                        {
                            con.MinisterialJobTitleId = null;
                        }

                        ctx.SaveChanges();

                        ctx.MinisterialJobTitles.Remove(ministerialJobTitle);
                        ctx.SaveChanges();

                        WriteActivityLogEntry(ActivityType.Ministerial_Job_Deleted, EntityType.MinisterialJobTitle, ministerialJobTitle.Id, ministerialJobTitle.MinisterialJobTitleName, Guid.Empty, "", CommonMethods.GetLoggedInUser());

                        returnVal = true;
                    }

                }
                else
                {
                    message = "Error deleting Ministerial Job Title: not found";
                }
            }

            return new KeyValuePair<bool, string>(returnVal, message);
        }

        /// <summary>
        /// This method is called to create or edit a ministerialJobTitle in the media advisory subscription service.
        /// Does validation to ensure that ministerialJobTitle name is not duplicated
        /// </summary>
        /// <param name="ministerialJobTitleName">the name of the created/edited ministerialJobTitle</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditMinisterialJobTitle(string ministerialJobTitleName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(ministerialJobTitleName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                MinisterialJobTitle ministerialJobTitle;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Ministerial_Job_Modified;

                    ministerialJobTitle = (from jtitle in ctx.MinisterialJobTitles  where jtitle.Id == guid.Value select jtitle).FirstOrDefault();
                    if (ministerialJobTitle == null) return errors | 1024;
                    oldName = ministerialJobTitle.MinisterialJobTitleName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Ministerial_Job_Added;
                    ministerialJobTitle = new MinisterialJobTitle() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.MinisterialJobTitles.Add(ministerialJobTitle);
                }

                if (errors == 0)
                {
                    if (oldName != ministerialJobTitleName)
                    {
                        int count = (from c in ctx.MinisterialJobTitles where c.MinisterialJobTitleName == ministerialJobTitleName select c).Count();
                        if (count > 0) return errors | 8;
                        ministerialJobTitle.MinisterialJobTitleName = ministerialJobTitleName;
                    }
                    ministerialJobTitle.ModifiedDate = DateTime.Now;
                    ministerialJobTitle.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.MinisterialJobTitle, ministerialJobTitle.Id, ministerialJobTitleName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a printCategory in the media advisory subscription service.
        /// Does validation to ensure that printCategory name is not duplicated
        /// </summary>
        /// <param name="printCategoryName">the name of the created/edited printCategory</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditPrintCategory(string printCategoryName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(printCategoryName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                PrintCategory printCategory;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Print_Category_Modified;

                    printCategory = (from cat in ctx.PrintCategories where cat.Id == guid.Value select cat).FirstOrDefault();
                    if (printCategory == null) return errors | 1024;
                    oldName = printCategory.PrintCategoryName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Print_Category_Added;
                    printCategory = new PrintCategory() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.PrintCategories.Add(printCategory);
                }

                if (errors == 0)
                {
                    if (oldName != printCategoryName)
                    {
                        int count = (from c in ctx.PrintCategories where c.PrintCategoryName == printCategoryName select c).Count();
                        if (count > 0) return errors | 8;
                        printCategory.PrintCategoryName = printCategoryName;
                    }
                    printCategory.ModifiedDate = DateTime.Now;
                    printCategory.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.PrintCategory, printCategory.Id, printCategoryName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a publicationDay in the media advisory subscription service.
        /// Does validation to ensure that publicationDay name is not duplicated
        /// </summary>
        /// <param name="publicationDayName">the name of the created/edited publicationDay</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditPublicationDay(string publicationDayName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(publicationDayName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                PublicationDay publicationDay;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Publication_Day_Modified;

                    publicationDay = (from pday in ctx.PublicationDays where pday.Id == guid.Value select pday).FirstOrDefault();
                    if (publicationDay == null) return errors | 1024;
                    oldName = publicationDay.PublicationDaysName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Publication_Day_Added;
                    publicationDay = new PublicationDay() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.PublicationDays.Add(publicationDay);
                }

                if (errors == 0)
                {
                    if (oldName != publicationDayName)
                    {
                        int count = (from c in ctx.PublicationDays where c.PublicationDaysName == publicationDayName select c).Count();
                        if (count > 0) return errors | 8;
                        publicationDay.PublicationDaysName = publicationDayName;
                    }
                    publicationDay.ModifiedDate = DateTime.Now;
                    publicationDay.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.PublicationDay, publicationDay.Id, publicationDayName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is used to delete a publicationFrequency from the database, this will affect
        /// media outlets
        /// </summary>
        /// <param name="guid">The guid of the publicationFrequency to delete</param>
        /// <returns>a bool flag of success for the deletion as the key, the value is a message of the error</returns>
        public static KeyValuePair<bool, string> DeletePublicationFrequency(Guid guid)
        {
            bool returnVal = false;
            string message = "";

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                PublicationFrequency publicationFrequency = (from pfreq in ctx.PublicationFrequencies where pfreq.Id == guid select pfreq).FirstOrDefault();

                if (publicationFrequency != null)
                {
                    // todo implement the delete
                    bool canDelete = true;
                    if (publicationFrequency.Companies.Where(x => x.IsActive).Count() > 0)
                    {
                        canDelete = false;
                        message = "Cannot delete a publication frequency that is used by outlets in the system";
                    }

                    if (canDelete)
                    {
                        List<Company> companiesToEdit = publicationFrequency.Companies.Where(x => !x.IsActive).ToList();
                        foreach (Company comp in companiesToEdit)
                        {
                            comp.PublicationFrequencyId = null;
                        }

                        ctx.SaveChanges();
                        ctx.PublicationFrequencies.Remove(publicationFrequency);
                        ctx.SaveChanges();
                        WriteActivityLogEntry(ActivityType.Publication_Frequency_Deleted, EntityType.PublicationFrequency, publicationFrequency.Id, publicationFrequency.PublicationFrequencyName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                        returnVal = true;
                    }
                }
                else
                {
                    message = "Error deleting publicationFrequency: not found";
                }
            }

            return new KeyValuePair<bool, string>(returnVal, message);
        }

        /// <summary>
        /// This method is called to create or edit a publicationFrequency in the media advisory subscription service.
        /// Does validation to ensure that publicationFrequency name is not duplicated
        /// </summary>
        /// <param name="publicationFrequencyName">the name of the created/edited publicationFrequency</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditPublicationFrequency(string publicationFrequencyName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(publicationFrequencyName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                PublicationFrequency publicationFrequency;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Publication_Frequency_Modified;

                    publicationFrequency = (from pfreq in ctx.PublicationFrequencies where pfreq.Id == guid.Value select pfreq).FirstOrDefault();
                    if (publicationFrequency == null) return errors | 1024;
                    oldName = publicationFrequency.PublicationFrequencyName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Publication_Frequency_Added;
                    publicationFrequency = new PublicationFrequency() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.PublicationFrequencies.Add(publicationFrequency);
                }

                if (errors == 0)
                {
                    if (oldName != publicationFrequencyName)
                    {
                        int count = (from c in ctx.PublicationFrequencies where c.PublicationFrequencyName == publicationFrequencyName select c).Count();
                        if (count > 0) return errors | 8;
                        publicationFrequency.PublicationFrequencyName = publicationFrequencyName;
                    }
                    publicationFrequency.ModifiedDate = DateTime.Now;
                    publicationFrequency.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.PublicationFrequency, publicationFrequency.Id, publicationFrequencyName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a sector in the media advisory subscription service.
        /// Does validation to ensure that sector name is not duplicated
        /// </summary>
        /// <param name="sectorName">the name of the created/edited sector</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditSector(string sectorName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(sectorName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Sector sector;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Sector_Modified;

                    sector = (from sect in ctx.Sectors where sect.Id == guid.Value select sect).FirstOrDefault();
                    if (sector == null) return errors | 1024;
                    oldName = sector.DisplayName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Sector_Added;
                    sector = new Sector() { Id = Guid.NewGuid(), Timestamp = DateTime.Now };
                    ctx.Sectors.Add(sector);
                }

                if (errors == 0)
                {
                    if (oldName != sectorName)
                    {
                        int count = (from c in ctx.Sectors where c.DisplayName == sectorName select c).Count();
                        if (count > 0) return errors | 8;
                        sector.DisplayName = sectorName;
                    }
                    sector.Timestamp = DateTime.Now;
                    sector.SortOrder = slotNumber;
                    ctx.SaveChanges();
                    WriteActivityLogEntry(activityType, EntityType.Sector, sector.Id, sectorName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to create or edit a specialtyPublication in the media advisory subscription service.
        /// Does validation to ensure that specialtyPublication name is not duplicated
        /// </summary>
        /// <param name="specialtyPublicationName">the name of the created/edited specialtyPublication</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditSpecialtyPublication(string specialtyPublicationName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(specialtyPublicationName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                SpecialtyPublication specialtyPublication;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Specialty_Publication_Modified;

                    specialtyPublication = (from spub in ctx.SpecialtyPublications where spub.Id == guid.Value select spub).FirstOrDefault();
                    if (specialtyPublication == null) return errors | 1024;
                    oldName = specialtyPublication.SpecialtyPublicationName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Specialty_Publication_Added;
                    specialtyPublication = new SpecialtyPublication() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.SpecialtyPublications.Add(specialtyPublication);
                }

                if (errors == 0)
                {
                    if (oldName != specialtyPublicationName)
                    {
                        int count = (from c in ctx.SpecialtyPublications where c.SpecialtyPublicationName == specialtyPublicationName select c).Count();
                        if (count > 0) return errors | 8;
                        specialtyPublication.SpecialtyPublicationName = specialtyPublicationName;
                    }
                    specialtyPublication.ModifiedDate = DateTime.Now;
                    specialtyPublication.SortOrder = slotNumber;
                    ctx.SaveChanges();

                    WriteActivityLogEntry(activityType, EntityType.SpecialtyPublication, specialtyPublication.Id, specialtyPublicationName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is used to delete a beat from the database, this will affect
        /// contacts
        /// </summary>
        /// <param name="guid">The guid of the beat to delete</param>
        /// <returns>a bool flag of success for the deletion as the key, string is the error message</returns>
        public static KeyValuePair<bool, string> DeleteBeat(Guid guid)
        {
            bool returnVal = false;
            string message = "";

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Beat beat = (from bt in ctx.Beats where bt.Id == guid select bt).FirstOrDefault();
                if (beat != null)
                {
                    // todo implement the delete
                    bool canDelete = true;
                    if (beat.ContactBeats.Count != 0)
                    {
                        if (!canDelete) message += ", ";
                        message += "Cannot delete a beat attached to a subscriber (" + beat.BeatName + ")";
                        canDelete = false;
                    }
                    if (canDelete)
                    {
                        ctx.Beats.Remove(beat);
                        ctx.SaveChanges();

                        WriteActivityLogEntry(ActivityType.Beat_Deleted, EntityType.Beat, beat.Id, beat.BeatName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                        returnVal = true;
                    }
                }
                else
                {
                    message = "Error deleting beat: not found";
                }
            }

            return new KeyValuePair<bool, string>(returnVal, message);
        }

        /// <summary>
        /// This method is called to create or edit a beat in the media advisory subscription service.
        /// Does validation to ensure that beat name is not duplicated
        /// </summary>
        /// <param name="beatName">the name of the created/edited beat</param>
        /// <returns>int status flag where 0 is a success, and not 0 represents errors</returns>
        public static int CreateEditBeat(string beatName, string slotNumberText, Guid? guid)
        {
            int errors = 0;

            int slotNumber;
            if (!int.TryParse(slotNumberText, out slotNumber)) errors |= 1;
            if (string.IsNullOrEmpty(beatName)) errors |= 2;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Beat beat;
                string oldName = null;
                ActivityType activityType;
                var permissions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsDataLists);
                if (guid.HasValue)
                {
                    if ((permissions & Permissions.SiteAction.Update) == 0) return -1;
                    activityType = ActivityType.Beat_Modified;

                    beat = (from bt in ctx.Beats where bt.Id == guid.Value select bt).FirstOrDefault();
                    if (beat == null) return errors | 1024;
                    oldName = beat.BeatName;
                }
                else
                {
                    if ((permissions & Permissions.SiteAction.Create) == 0) return -1;
                    activityType = ActivityType.Beat_Added;
                    beat = new Beat() { Id = Guid.NewGuid(), CreationDate = DateTime.Now };
                    ctx.Beats.Add(beat);
                }

                if (errors == 0)
                {
                    if (oldName != beatName)
                    {
                        int count = (from c in ctx.Beats where c.BeatName == beatName select c).Count();
                        if (count > 0) return errors | 8;
                        beat.BeatName = beatName;
                    }
                    beat.ModifiedDate = DateTime.Now;
                    beat.SortOrder = slotNumber;
                    ctx.SaveChanges();

                    WriteActivityLogEntry(activityType, EntityType.Beat, beat.Id, beatName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                }
            }

            return errors;
        }


        /// <summary>
        /// Increments or decrements the sort order of an ISortableEntity
        /// </summary>
        public static void MoveEntity<T>(string tableName, string keyColumnName, Guid entityKey, bool moveUp) where T : class, ISortableEntity
        {
            lock (staticSync)
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    var lowestSlotNum = ctx.Database.SqlQuery<T>("SELECT * FROM " + tableName + (typeof(IDeletableEntity).IsAssignableFrom(typeof(T)) ? " WHERE IsActive = 1" : ""), new object[0]).Min(e => e.SortOrder);
                    var highestSlotNum = ctx.Database.SqlQuery<T>("SELECT * FROM " + tableName + (typeof(IDeletableEntity).IsAssignableFrom(typeof(T)) ? " WHERE IsActive = 1" : ""), new object[0]).Max(e => e.SortOrder);

                    var entity = ctx.Database.SqlQuery<T>("SELECT * FROM " + tableName + (typeof(IDeletableEntity).IsAssignableFrom(typeof(T)) ? " WHERE IsActive = 1 AND " : " WHERE ") + keyColumnName + " = @Key", new object[] { new System.Data.SqlClient.SqlParameter("@Key", entityKey) }).Single();

                    if (moveUp)
                    {
                        if (entity.SortOrder > lowestSlotNum)
                        {
                            var previousEntity = ctx.Database.SqlQuery<T>("SELECT * FROM " + tableName + (typeof(IDeletableEntity).IsAssignableFrom(typeof(T)) ? " WHERE IsActive = 1 AND" : " WHERE ") + " SortOrder =  @SortOrder", new object[] { new System.Data.SqlClient.SqlParameter("@SortOrder", entity.SortOrder - 1) }).Single();

                            previousEntity.SortOrder = previousEntity.SortOrder + 1;
                            entity.SortOrder = entity.SortOrder - 1;

                            ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                            ctx.Entry(previousEntity).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    else
                    {
                        if (entity.SortOrder < highestSlotNum)
                        {
                            var nextEntity = ctx.Database.SqlQuery<T>("SELECT * FROM " + tableName + (typeof(IDeletableEntity).IsAssignableFrom(typeof(T)) ? " WHERE IsDeleted = 0 AND" : " WHERE ") + " SortOrder =  @SortOrder", new object[] { new System.Data.SqlClient.SqlParameter("@SortOrder", entity.SortOrder + 1) }).Single();

                            nextEntity.SortOrder = nextEntity.SortOrder - 1;
                            entity.SortOrder = entity.SortOrder + 1;

                            ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                            ctx.Entry(nextEntity).State = System.Data.Entity.EntityState.Modified;
                        }
                    }

                    ctx.SaveChanges();
                }
            }
        }

        /// <summary>
        /// This method is used to delete an entity from the database
        /// </summary>
        /// <param name="guid">The guid of the entity to delete</param>
        /// <returns>a bool flag of success for the deletion as the key, the value is a message of the error</returns>
        public static KeyValuePair<bool, string> DeleteEntity<TEntity>(Guid guid) where TEntity : class
        {
            bool returnVal = false;
            string message = "";

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                var type = typeof(TEntity);

                PropertyInfo tableProp = ctx.GetType().GetProperties().Where(x => x.GetMethod.ReturnParameter.ParameterType.FullName.Contains(type.FullName)).FirstOrDefault();

                var table = (DbSet<TEntity>)tableProp.GetValue(ctx);
                TEntity entity2Delete = table.Find(new object[] { guid });

                if (entity2Delete != null)
                {
                    bool canDelete = true;
                    string entityName = string.Empty;
                    foreach (var fn in entity2Delete.GetType().GetRuntimeMethods())
                    {
                        if (fn.Name.StartsWith("get_"))
                        {
                            if (fn.Name.EndsWith("Name"))
                            {
                                entityName = (string)fn.Invoke(entity2Delete, null);
                                break;
                            }
                            if (fn.Name.EndsWith("Companies"))
                            {
                                var companies = (ICollection<Company>)fn.Invoke(entity2Delete, null);
                                canDelete = (companies.Count == 0);
                            }
                        }
                    }

                    if (canDelete)
                    {
                        table.Remove(entity2Delete);

                        ctx.SaveChanges();
                        var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), type.Name + "Deleted");
                        var entityType = (EntityType)Enum.Parse(typeof(EntityType), type.Name);
                        WriteActivityLogEntry(activityType, entityType, guid, entityName, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                        returnVal = true;
                    }
                    else
                    {
                        message = "Cannot delete a " + type.Name + " attached to a company (" + entityName + ")";
                    }
                }
                else
                {
                    message = "Error deleting " + type.Name + ": not found";
                }
            }

            return new KeyValuePair<bool, string>(returnVal, message);
        }

        /// <summary>
        /// Returns the next SortOrder when creating a new row based on ISortableEntity
        /// </summary>
        public static int NextSortOrder<T>(IQueryable<T> table) where T : ISortableEntity
        {
            var entities = table.ToArray();

            var slots = entities.Select(row => row.SortOrder).Distinct();

            if (slots.Count() == 0)
            {
                return 0;
            }
            else if (slots.Count() == 1)
            {
                return slots.Single();
            }
            else
            {
                if (entities.Count() == slots.Count())
                    return entities.Max(row => row.SortOrder) + 1;
                else
                    return 0;
            }
        }
    }
}