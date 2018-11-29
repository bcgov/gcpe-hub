using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Data.Entity.Validation;
using MediaRelationsDatabase;
using static MediaRelationsLibrary.CommonEventLogging;

namespace MediaRelationsLibrary
{
    public class CompanyAdministrationLib
    {
        #region validation methods

        /// <summary>
        /// Checks the values of the 1st tab against the required fields to ensure that the 
        /// submitted record is valid
        /// </summary>
        /// <param name="mediaName"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public int CheckCompanyFirstTabValidity(string mediaName, List<KeyValuePair<string, PhoneNumberInfo>> phoneNumbers, bool requiredCheck, string originalCompanyName, bool isOutlet)
        {
            int errors = 0;

            if (string.IsNullOrWhiteSpace(mediaName) && requiredCheck) errors |= 1;

            // check for primary phone number being filled out
            if (phoneNumbers.Count == 0 && requiredCheck) errors |= 2;

            // check for company name uniqueness
            if (originalCompanyName == null || (originalCompanyName != null && !originalCompanyName.ToLower().Equals(mediaName.ToLower())))
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    int count = (from c in ctx.Companies where c.CompanyName == mediaName where c.IsOutlet == isOutlet where c.IsActive select c).Count();
                    if (count > 0) errors |= 64;
                }
            }
            return errors;
        }

        /// <summary>
        /// checks the data from the "WebAddress" tab of the form for validity
        /// </summary>
        /// <param name="WebAddresses"></param>
        /// <returns>
        /// 0 if no errors are encountered, or a bitwise-or of the following:
        /// 1 - city is not selected/entered
        /// 2 - province is not selected/entered
        /// 4 - country is not selected/entered
        /// 8 - no regions have been selected
        /// 16 - no electoral districts have been selected
        /// </returns>
        private int IsValidTabWebAddress(IList<WebAddressDisplay> WebAddresses)
        {
            int errors = 0;

            return errors;
        }

        /// <summary>
        /// This is used to check validity of optional fields for the first tab
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="description"></param>
        /// <param name="webAddressTypes"></param>
        /// <param name="phoneNumbers"></param>
        /// <returns></returns>
        public int CheckCompanyFirstTabOptionalValuesValidity(int errors, string description, List<KeyValuePair<string, PhoneNumberInfo>> phoneNumberItems)
        {
            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                foreach (KeyValuePair<string, PhoneNumberInfo> item in phoneNumberItems)
                {
                    if (!ValidationMethods.CheckForValidPhoneNumber(item.Value.PhoneNumber)) errors |= 16;
                }

                if (description.Length > 500) errors |= 32;

            }
            return errors;
        }

        private static bool CheckAddressValidity(Guid cityId, Guid provStateId, Guid countryId)
        {
            if (cityId == Guid.Empty) return false;
            if (provStateId == Guid.Empty) return false;
            if (countryId == Guid.Empty) return false;
            return true;
        }

        public int CheckCompanyWebAddressTabValidity(IList<WebAddressDisplay> CWebAddr, bool requiredCheck)
        {
            //TODO Implement when migrating WebAddresses from first tab
            return 0;

        }

        /// <summary>
        /// this method is used to ensure that the values submitted in the 2nd tab meet the required fields
        /// and proper format
        /// </summary>
        /// <param name="address"></param>
        /// <param name="cityId"></param>
        /// <param name="provStateId"></param>
        /// <param name="countryId"></param>
        /// <param name="postalCode"></param>
        /// <param name="mailingAddress"></param>
        /// <param name="mailingCityId"></param>
        /// <param name="mailingProvStateId"></param>
        /// <param name="mailingCountryId"></param>
        /// <param name="mailingPostalCode"></param>
        /// <param name="regions"></param>
        /// <param name="electoralDistricts"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public int CheckCompanySecondTabValidity(string address, Guid cityId, Guid provStateId, Guid countryId, string postalCode,
                                                string mailingAddress, Guid mailingCityId, Guid mailingProvStateId, Guid mailingCountryId, string mailingPostalCode,
                                                Dictionary<string, string> regions, Dictionary<string, string> electoralDistricts, bool requiredCheck)
        {
            if (!requiredCheck) return 0;

            int errors = 0;

            //if (string.IsNullOrWhiteSpace(address)) errors |= 1;

            // check physical address
            bool validPhysicalAddress = CheckAddressValidity(cityId, provStateId, countryId);

            // check mailing address
            bool validMailingAddress = CheckAddressValidity(mailingCityId, mailingProvStateId, mailingCountryId);

            if (!validPhysicalAddress && !validMailingAddress) errors |= 64;
            else
            {
                bool physicalAddressPartial = false;

                if (!string.IsNullOrWhiteSpace(address)) physicalAddressPartial = true;
                if (cityId != Guid.Empty) physicalAddressPartial = true;
                if (provStateId != Guid.Empty) physicalAddressPartial = true;
                if (countryId != Guid.Empty) physicalAddressPartial = true;
                if (!string.IsNullOrWhiteSpace(postalCode)) physicalAddressPartial = true;

                if (physicalAddressPartial)
                {
                    if (cityId == Guid.Empty) errors |= 2;
                    if (provStateId == Guid.Empty) errors |= 4;
                    if (countryId == Guid.Empty) errors |= 8;


                    if (!string.IsNullOrWhiteSpace(address) && address.Length > 250) errors |= 1024;

                }

                bool mailingAddressPartial = false;

                if (!string.IsNullOrWhiteSpace(mailingAddress)) mailingAddressPartial = true;
                if (mailingCityId != Guid.Empty) mailingAddressPartial = true;
                if (mailingProvStateId != Guid.Empty) mailingAddressPartial = true;
                if (mailingCountryId != Guid.Empty) mailingAddressPartial = true;
                if (!string.IsNullOrWhiteSpace(mailingPostalCode)) mailingAddressPartial = true;

                if (mailingAddressPartial)
                {
                    if (mailingCityId == Guid.Empty) errors |= 128;
                    if (mailingProvStateId == Guid.Empty) errors |= 256;
                    if (mailingCountryId == Guid.Empty) errors |= 512;

                    if (!string.IsNullOrWhiteSpace(mailingAddress) && mailingAddress.Length > 250) errors |= 2048;
                }
            }

            if (regions.Count == 0) errors |= 16;
            if (electoralDistricts.Count == 0) errors |= 32;

            return errors;
        }

        /// <summary>
        /// this is used to check against the validity of data submitted in the 3rd tab. ensures required fields are set
        /// and proper data.
        /// </summary>
        /// <param name="distributions"></param>
        /// <param name="languages"></param>
        /// <param name="mediaTypes"></param>
        /// <param name="isEthnicMedia"></param>
        /// <param name="ethnicities"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public int CheckMediaOutletThirdTabValidity(Dictionary<string, string> distributions, Dictionary<string, string> languages, Dictionary<string, string> mediaTypes,
            bool isEthnicMedia, Dictionary<string, string> ethnicities, bool requiredCheck)
        {

            if (!requiredCheck) return 0;

            int errors = 0;

            if (distributions.Count == 0) errors |= 1;
            if (languages.Count == 0) errors |= 2;
            if (mediaTypes.Count == 0) errors |= 4;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                var mediaType = (from t in ctx.MediaTypes where t.MediaTypeName == "Print" select t).First();
                if (mediaTypes.ContainsKey(mediaType.Id.ToString()))
                {
                    if (string.IsNullOrWhiteSpace(mediaTypes[mediaType.Id.ToString()])) errors |= 8; // ensure print category is selected
                }
            }

            if (isEthnicMedia && ethnicities.Count == 0) errors |= 16;


            return errors;
        }

        /// <summary>
        /// checks the validity of the optional fields in this tab for proper input
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="keyPrograms"></param>
        /// <param name="circulation"></param>
        /// <param name="deadlines"></param>
        /// <returns></returns>
        public int CheckMediaOutletOptionalTabValidity(int errors, string keyPrograms, string circulation, string deadlines)
        {
            if (!string.IsNullOrWhiteSpace(keyPrograms) && keyPrograms.Length > 500) errors |= 32;
            if (!string.IsNullOrWhiteSpace(circulation) && keyPrograms.Length > 500) errors |= 64;
            if (!string.IsNullOrWhiteSpace(deadlines) && keyPrograms.Length > 500) errors |= 128;

            return errors;
        }

        #endregion

        #region common company modification steps

        /// <summary>
        /// This is used to set the company address objects information from the 2nd tab of the 
        /// edit screen
        /// </summary>
        /// <param name="company"></param>
        /// <param name="address"></param>
        /// <param name="cityId"></param>
        /// <param name="provStateId"></param>
        /// <param name="countryId"></param>
        /// <param name="postalCode"></param>
        /// <param name="mailingStreetAddress"></param>
        /// <param name="mailingCityId"></param>
        /// <param name="mailingProvStateId"></param>
        /// <param name="mailingCountryId"></param>
        /// <param name="mailingPostalCode"></param>
        /// <param name="regions"></param>
        /// <param name="electoralDistricts"></param>
        /// <param name="sectors"></param>
        /// <param name="ctx"></param>
        private static void CommonSecondStep(Company company, string address, Guid cityId, Guid provStateId, Guid countryId, string postalCode,
            string mailingStreetAddress, Guid mailingCityId, Guid mailingProvStateId, Guid mailingCountryId, string mailingPostalCode,
            Dictionary<string, string> regions, Dictionary<string, string> electoralDistricts, Dictionary<string, string> sectors, MediaRelationsEntities ctx, bool dbSave)
        {
            bool validPhysicalAddress = CheckAddressValidity(cityId, provStateId, countryId);
            bool validMailingAddress = CheckAddressValidity(mailingCityId, mailingProvStateId, mailingCountryId);

            #region physical mailing address
            int addressType = (int)CommonMethods.AddressType.Physical;

            CompanyAddress physicalAddress = null;

            physicalAddress = company.CompanyAddresses.Where(x => x.AddressType == (int)CommonMethods.AddressType.Physical).FirstOrDefault();
            if (validPhysicalAddress)
            {
                bool isNewAddress = false;

                if (physicalAddress == null)
                {
                    isNewAddress = true;
                    physicalAddress = new CompanyAddress()
                    {
                        Id = Guid.NewGuid(),
                        Company = company,
                        StreetAddress = address,
                        PostalZipCode = postalCode,
                        AddressType = addressType,
                        CreationDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    if (dbSave) WriteActivityLogEntry(company, "Added new Physical Address");
                }
                else
                {
                    physicalAddress.StreetAddress = Change(physicalAddress.StreetAddress, address, company, "Street Address", dbSave);
                    physicalAddress.PostalZipCode = Change(physicalAddress.PostalZipCode, postalCode, company, "Postal/Zip Code", dbSave);
                    physicalAddress.ModifiedDate = DateTime.Now;
                }

                if (cityId != Guid.Empty)
                {
                    physicalAddress.CityId = cityId;
                    physicalAddress.City = ctx.Cities.Find(cityId);
                    physicalAddress.CityName = Change(physicalAddress.CityName, physicalAddress.City.CityName, company, "City", dbSave);
                }

                if (provStateId != Guid.Empty)
                {
                    physicalAddress.ProvStateId = provStateId;
                    physicalAddress.ProvState = ctx.ProvStates.Find(provStateId);
                    physicalAddress.ProvStateName = Change(physicalAddress.ProvStateName, physicalAddress.ProvState.ProvStateName, company, "Prov/State", dbSave);
                }

                physicalAddress.CountryId = Change(physicalAddress.CountryId, countryId, company, "Country", dbSave);

                if (isNewAddress)
                {
                    company.CompanyAddresses.Add(physicalAddress);
                }
            }
            else
            {
                if (physicalAddress != null)
                {
                    ctx.CompanyAddresses.Remove(physicalAddress);
                    if (company.CompanyAddresses.Contains(physicalAddress)) company.CompanyAddresses.Remove(physicalAddress);
                }
            }

            #endregion

            #region mailing address
            addressType = (int)CommonMethods.AddressType.Mailing;

            CompanyAddress mailingAddress = null;

            mailingAddress = company.CompanyAddresses.Where(x => x.AddressType == (int)CommonMethods.AddressType.Mailing).FirstOrDefault();
            if (validMailingAddress)
            {
                bool isNewAddress = false;

                if (mailingAddress == null)
                {
                    isNewAddress = true;
                    mailingAddress = new CompanyAddress()
                    {
                        Id = Guid.NewGuid(),
                        Company = company,
                        StreetAddress = mailingStreetAddress,
                        PostalZipCode = mailingPostalCode,
                        AddressType = addressType,
                        CreationDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    if (dbSave) WriteActivityLogEntry(company, "Added new Mailing Address");
                }
                else
                {
                    mailingAddress.StreetAddress = Change(mailingAddress.StreetAddress, mailingStreetAddress, company, "Mailing Address", dbSave);
                    mailingAddress.PostalZipCode = Change(mailingAddress.PostalZipCode, mailingPostalCode, company, "Mailing PostalCode", dbSave);
                    mailingAddress.ModifiedDate = DateTime.Now;
                }

                if (mailingCityId != Guid.Empty)
                {
                    mailingAddress.CityId = mailingCityId;
                    mailingAddress.City = ctx.Cities.Find(mailingCityId);
                    mailingAddress.CityName = Change(mailingAddress.CityName, mailingAddress.City.CityName, company, "Mailing City", dbSave);
                }
                else
                {
                    mailingAddress.City = null;
                    mailingAddress.CityId = null;
                    mailingAddress.CityName = null;
                }

                if (mailingProvStateId != Guid.Empty)
                {
                    mailingAddress.ProvStateId = mailingProvStateId;
                    mailingAddress.ProvState = ctx.ProvStates.Find(mailingProvStateId);
                    mailingAddress.ProvStateName = Change(mailingAddress.ProvStateName, mailingAddress.ProvState.ProvStateName, company, "Mailing Prov/State", dbSave);
                }
                else
                {
                    mailingAddress.ProvState = null;
                    mailingAddress.ProvStateId = null;
                    mailingAddress.ProvStateName = null;
                }


                mailingAddress.CountryId = Change(mailingAddress.CountryId, mailingCountryId, company, "Mailing Country", dbSave);

                if (isNewAddress)
                {
                    company.CompanyAddresses.Add(mailingAddress);
                }
            }
            else
            {
                if (mailingAddress != null)
                {
                    ctx.CompanyAddresses.Remove(mailingAddress);
                    if (company.CompanyAddresses.Contains(mailingAddress)) company.CompanyAddresses.Remove(mailingAddress);
                }
            }

            #endregion

            for (int i = company.Regions.Count - 1; i >= 0; i--)
            {
                Region reg = company.Regions.ElementAt(i);
                if (!Remove(company.Regions, reg, !regions.Any(r => r.Key == reg.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Region: " + reg.RegionName);
            }
            foreach (KeyValuePair<string, string> r in regions)
            {
                Guid guid = Guid.Parse(r.Key);
                Region region = (from reg in ctx.Regions where reg.Id == guid select reg).First();
                if (!company.Regions.Contains(region))
                {
                    company.Regions.Add(region);
                    if (dbSave) WriteActivityLogEntry(company, "Added Region: " + region.RegionName);
                }
            }

            for (int i = company.ElectoralDistricts.Count - 1; i >= 0; i--)
            {
                ElectoralDistrict dst = company.ElectoralDistricts.ElementAt(i);
                if (!Remove(company.ElectoralDistricts, dst, !electoralDistricts.Any(r => r.Key == dst.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed District: " + dst.DistrictName);
            }
            foreach (KeyValuePair<string, string> e in electoralDistricts)
            {
                Guid guid = Guid.Parse(e.Key);
                ElectoralDistrict district = (from ed in ctx.ElectoralDistricts where ed.Id == guid select ed).First();
                if (!company.ElectoralDistricts.Contains(district))
                {
                    company.ElectoralDistricts.Add(district);
                    if (dbSave) WriteActivityLogEntry(company, "Added District: " + district.DistrictName);
                }
            }

            for (int i = company.Sectors.Count - 1; i >= 0; i--)
            {
                Sector sec = company.Sectors.ElementAt(i);
                if (!Remove(company.Sectors, sec, !sectors.Any(r => r.Key == sec.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Sector: " + sec.DisplayName);
            }
            foreach (KeyValuePair<string, string> s in sectors)
            {
                Guid guid = Guid.Parse(s.Key);
                Sector sector = (from sec in ctx.Sectors where sec.Id == guid select sec).First();
                if (!company.Sectors.Contains(sector))
                {
                    company.Sectors.Add(sector);
                    if (dbSave) WriteActivityLogEntry(company, "Added Sector: " + sector.DisplayName);
                }
            }
        }

        /// <summary>
        /// This is used to set the companies objects data from the 3rd tab of the edit screen
        /// </summary>
        /// <param name="company"></param>
        /// <param name="mediaDesks"></param>
        /// <param name="mediaPartners"></param>
        /// <param name="distributions"></param>
        /// <param name="languages"></param>
        /// <param name="publicationDays"></param>
        /// <param name="specialPublications"></param>
        /// <param name="mediaTypes"></param>
        /// <param name="ethnicities"></param>
        /// <param name="isEthnicMedia"></param>
        /// <param name="publicationFrequencyId"></param>
        /// <param name="isMajorMedia"></param>
        /// <param name="isLiveMedia"></param>
        /// <param name="keyPrograms"></param>
        /// <param name="circulation"></param>
        /// <param name="deadlines"></param>
        /// <param name="ctx"></param>
        private static void CommonMediaOutletThirdTab(Company company, Dictionary<string, string> mediaDesks, Dictionary<string, string> mediaPartners, Dictionary<string, string> distributions, Dictionary<string, string> languages, Dictionary<string, string> publicationDays, Dictionary<string, string> specialPublications, Dictionary<string, string> mediaTypes, Dictionary<string, string> ethnicities, bool isEthnicMedia, Guid? publicationFrequencyId, bool isMajorMedia, bool isLiveMedia, string keyPrograms, string circulation, string deadlines, MediaRelationsEntities ctx, bool dbSave)
        {
            for (int i = company.MediaDesks.Count - 1; i >= 0; i--)
            {
                MediaDesk desk = company.MediaDesks.ElementAt(i);
                if (!Remove(company.MediaDesks, desk, !mediaDesks.Any(r => r.Key == desk.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Media Desk: " + desk.MediaDeskName);
            }
            foreach (KeyValuePair<string, string> item in mediaDesks)
            {
                Guid guid = Guid.Parse(item.Key);
                MediaDesk desk = (from m in ctx.MediaDesks where m.Id == guid select m).First();
                if (!Add(company.MediaDesks, desk)) continue;
                if (dbSave) WriteActivityLogEntry(company, "Added Media Desk: " + desk.MediaDeskName);
            }

            for (int i = company.PartnerCompanies.Count - 1; i >= 0; i--)
            {
                Company partner = company.PartnerCompanies.ElementAt(i);
                if (!Remove(company.PartnerCompanies, partner, !mediaPartners.Any(r => r.Key == partner.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Partner: " + partner.CompanyName);
            }

            foreach (KeyValuePair<string, string> item in mediaPartners)
            {
                Guid guid = Guid.Parse(item.Key);
                Company partner = (from cmp in ctx.Companies where cmp.Id == guid select cmp).First();
                if (!Add(company.PartnerCompanies, partner)) continue;
                if (dbSave) WriteActivityLogEntry(company, "Added Partner: " + partner.CompanyName);
            }

            for (int i = company.Distributions.Count - 1; i >= 0; i--)
            {
                Distribution dist = company.Distributions.ElementAt(i);
                if (!Remove(company.Distributions, dist, !distributions.Any(r => r.Key == dist.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Distribution: " + dist.DistributionName);
            }

            foreach (KeyValuePair<string, string> item in distributions)
            {
                Guid guid = Guid.Parse(item.Key);
                Distribution dist = (from d in ctx.Distributions where d.Id == guid select d).First();
                if (!Add(company.Distributions, dist)) continue;
                if (dbSave) WriteActivityLogEntry(company, "Added Distribution: " + dist.DistributionName);
            }

            for (int i = company.Languages.Count - 1; i >= 0; i--)
            {
                Language lang = company.Languages.ElementAt(i);
                if (!Remove(company.Languages, lang, !languages.Any(r => r.Key == lang.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Language: " + lang.LanguageName);
            }
            foreach (KeyValuePair<string, string> item in languages)
            {
                Guid guid = Guid.Parse(item.Key);
                Language lang = (from l in ctx.Languages where l.Id == guid select l).First();
                if (!Add(company.Languages, lang)) continue;
                if (dbSave) WriteActivityLogEntry(company, "Added Language:" + lang.LanguageName);
            }

            for (int i = company.PublicationDays.Count - 1; i >= 0; i--)
            {
                PublicationDay day = company.PublicationDays.ElementAt(i);
                if (!Remove(company.PublicationDays, day, !publicationDays.Any(r => r.Key == day.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Publication Day: " + day.PublicationDaysName);
            }
            foreach (KeyValuePair<string, string> item in publicationDays)
            {
                Guid guid = Guid.Parse(item.Key);
                PublicationDay day = (from pday in ctx.PublicationDays where pday.Id == guid select pday).First();
                if (!Add(company.PublicationDays, day)) continue;
                if (dbSave) WriteActivityLogEntry(company, "Added Publication Day: " + day.PublicationDaysName);
            }

            for (int i = company.SpecialtyPublications.Count - 1; i >= 0; i--)
            {
                SpecialtyPublication sp = company.SpecialtyPublications.ElementAt(i);
                if (!Remove(company.SpecialtyPublications, sp, !specialPublications.Any(r => r.Key == sp.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Specialty Publication: " + sp.SpecialtyPublicationName);
            }
            foreach (KeyValuePair<string, string> item in specialPublications)
            {
                Guid guid = Guid.Parse(item.Key);
                SpecialtyPublication sp = (from spub in ctx.SpecialtyPublications where spub.Id == guid select spub).First();
                if (!Add(company.SpecialtyPublications, sp)) continue;
                if (dbSave) WriteActivityLogEntry(company, "Added Specialty Publication: " + sp.SpecialtyPublicationName);
            }

            MediaType printType = (from t in ctx.MediaTypes where t.MediaTypeName == "Print" select t).First();

            for (int i = company.MediaTypes.Count - 1; i >= 0; i--)
            {
                MediaType tp = company.MediaTypes.ElementAt(i);
                if (!Remove(company.MediaTypes, tp, !mediaTypes.Any(r => r.Key == tp.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Media Type: " + tp.MediaTypeName);
            }

            PrintCategory printCategory = null;
            foreach (KeyValuePair<string, string> item in mediaTypes)
            {
                Guid guid = Guid.Parse(item.Key);
                var mediaType = (from mtype in ctx.MediaTypes where mtype.Id == guid select mtype).First();
                if (mediaType == printType)
                {
                    guid = Guid.Parse(item.Value);
                    printCategory = (from cat in ctx.PrintCategories where cat.Id == guid select cat).First();
                }
                if (!Add(company.MediaTypes, mediaType)) continue;
                if (dbSave) WriteActivityLogEntry(company, "Added Media Type: " + mediaType.MediaTypeName);
            }
            if (printCategory != company.PrintCategory)
            {
                company.PrintCategory = printCategory;
                company.PrintCategoryId = printCategory?.Id;
                if (dbSave) WriteActivityLogEntry(company, "Changed Print Category: " + printCategory?.PrintCategoryName);
            }

            company.IsEthnicMedia = Change(company.IsEthnicMedia, isEthnicMedia, company, "Ethnic Media", dbSave);

            for (int i = company.Ethnicities.Count - 1; i >= 0; i--)
            {
                Ethnicity eth = company.Ethnicities.ElementAt(i);
                if (!Remove(company.Ethnicities, eth, !ethnicities.Any(r => r.Key == eth.Id.ToString()))) continue;
                if (dbSave) WriteActivityLogEntry(company, "Removed Ethnicity: " + eth.EthnicityName);
            }
            foreach (KeyValuePair<string, string> item in ethnicities)
            {
                Guid guid = Guid.Parse(item.Key);
                Ethnicity eth = (from e in ctx.Ethnicities where e.Id == guid select e).First();
                if (!Add(company.Ethnicities, eth)) continue;
                if (dbSave) WriteActivityLogEntry(company, "Added Ethnicity: " + eth.EthnicityName);
            }

            if (company.PublicationFrequencyId != publicationFrequencyId)
            {
                if (dbSave) WriteActivityLogEntry(company, "Changed Publication Frequency");
                company.PublicationFrequencyId = publicationFrequencyId;
                if (!publicationFrequencyId.HasValue)
                {
                    company.PublicationFrequency = null;
                }
            }

            company.IsMajorMedia = Change(company.IsMajorMedia, isMajorMedia, company, "Major Media", dbSave);
            company.IsLiveMedia = Change(company.IsLiveMedia, isLiveMedia, company, "Live Media", dbSave);
            company.KeyPrograms = Change(company.KeyPrograms, keyPrograms, company, "Key Programs", dbSave, false);
            company.CirculationDescription = Change(company.CirculationDescription, circulation, company, "Circulation Description", dbSave, false);
            company.Deadlines = Change(company.Deadlines, deadlines, company, "Deadlines", dbSave);
        }

        /// <summary>
        /// This method is used to add a list of outlets to a company, sets the parent company guid of the outlets to the passed in company
        /// </summary>
        /// <param name="company"></param>
        /// <param name="outletIds"></param>
        /// <param name="ctx"></param>
        private void AddParentCompanyToOutlets(Company company, List<Guid> outletIds, MediaRelationsEntities ctx)
        {
            foreach (Guid guid in outletIds)
            {
                Company outlet = (from cmp in ctx.Companies where cmp.Id == guid where cmp.IsOutlet == true where cmp.ParentCompanyId == null select cmp).First();
                if (outlet.ParentCompanyId != null)
                {
                    WriteActivityLogEntry(ctx.Companies.Find(outlet.ParentCompanyId), outlet.CompanyName, ActivityType.Outlet_Detached);
                }
                outlet.ParentCompanyId = company.Id;

                WriteActivityLogEntry(company, outlet.CompanyName, ActivityType.Outlet_Attached);
            }
        }

        #endregion

        #region add new company/outlet

        /// <summary>
        /// Creates a company object with the data from the first tab of the edit screen. This stores in session, calls the validation methods
        /// </summary>
        /// <param name="mediaName"></param>
        /// <param name="description"></param>
        /// <param name="webAddressTypes"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="isMediaOutlet"></param>
        /// <param name="parentCompanyId"></param>
        /// <param name="company"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public KeyValuePair<int, string> CreateCompanyFirstTab(string mediaName, string description,
                                                                List<KeyValuePair<string, PhoneNumberInfo>> phoneNumberItems, bool isMediaOutlet, Guid? parentCompanyId, Company company,
                                                                bool requiredCheck)
        {
            int errors = 0;
            string guidStr = null;

            errors = CheckCompanyFirstTabValidity(mediaName, phoneNumberItems, requiredCheck, null, isMediaOutlet);
            errors = CheckCompanyFirstTabOptionalValuesValidity(errors, description, phoneNumberItems);

            if (errors == 0)
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    if (company == null)
                    {
                        company = new Company()
                        {
                            Id = Guid.NewGuid(),
                            IsOutlet = isMediaOutlet,
                            IsActive = true,
                            CreationDate = DateTime.Now
                        };
                    }

                    company.CompanyName = mediaName;
                    company.CompanyDescription = description;
                    company.ModifiedDate = DateTime.Now;

                    if (isMediaOutlet && parentCompanyId != Guid.Empty)
                    {
                        company.ParentCompanyId = parentCompanyId;
                    }

                    company.CompanyPhoneNumbers.Clear();
                    foreach (KeyValuePair<string, PhoneNumberInfo> item in phoneNumberItems)
                    {
                        Guid guid = Guid.Parse(item.Key);
                        PhoneType phoneType = (from type in ctx.PhoneTypes where type.Id == guid select type).First();

                        string formattedPhoneNumber = item.Value.PhoneNumber;

                        CompanyPhoneNumber phoneNumber = new CompanyPhoneNumber()
                        {
                            Id = Guid.NewGuid(),
                            Company = company,
                            PhoneNumber = formattedPhoneNumber,
                            PhoneType = phoneType,
                            PhoneTypeId = phoneType.Id,
                            CreationDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        };

                        if (!string.IsNullOrWhiteSpace(item.Value.PhoneNumberExtension))
                            phoneNumber.PhoneNumberExtension = item.Value.PhoneNumberExtension;

                        company.CompanyPhoneNumbers.Add(phoneNumber);
                    }
                }

                HttpContext.Current.Session["CreateCompany_" + company.Id] = company;
                guidStr = company.Id.ToString();
            }

            return new KeyValuePair<int, string>(errors, guidStr);
        }

        /// <summary>
        /// This sets the company object with the data from the second tab of the edit screen. This stores in session and calls the validation methods
        /// </summary>
        /// <param name="company"></param>
        /// <param name="address"></param>
        /// <param name="cityId"></param>
        /// <param name="provStateId"></param>
        /// <param name="countryId"></param>
        /// <param name="postalCode"></param>
        /// <param name="mailingAddress"></param>
        /// <param name="mailingCityId"></param>
        /// <param name="mailingProvStateId"></param>
        /// <param name="mailingCountryId"></param>
        /// <param name="mailingPostalCode"></param>
        /// <param name="regions"></param>
        /// <param name="electoralDistricts"></param>
        /// <param name="sectors"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public KeyValuePair<int, string> CreateCompanyWebAddressTab(Company company, IList<WebAddressDisplay> CWebAddr, bool requiredCheck)
        {
            int errors = CheckCompanyWebAddressTabValidity(CWebAddr, requiredCheck);

            if (errors == 0)
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    EditCompanyTabWebAddress(company, ctx, CWebAddr, false);
                }
            }

            return new KeyValuePair<int, string>(errors, company.Id.ToString());
        }

        /// <summary>
        /// This sets the company object with the data from the second tab of the edit screen. This stores in session and calls the validation methods
        /// </summary>
        /// <param name="company"></param>
        /// <param name="address"></param>
        /// <param name="cityId"></param>
        /// <param name="provStateId"></param>
        /// <param name="countryId"></param>
        /// <param name="postalCode"></param>
        /// <param name="mailingAddress"></param>
        /// <param name="mailingCityId"></param>
        /// <param name="mailingProvStateId"></param>
        /// <param name="mailingCountryId"></param>
        /// <param name="mailingPostalCode"></param>
        /// <param name="regions"></param>
        /// <param name="electoralDistricts"></param>
        /// <param name="sectors"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public KeyValuePair<int, string> CreateCompanySecondTab(Company company, string address,
                                                                Guid cityId, Guid provStateId, Guid countryId, string postalCode,
                                                                string mailingAddress, Guid mailingCityId, Guid mailingProvStateId,
                                                                Guid mailingCountryId, string mailingPostalCode,
                                                                Dictionary<string, string> regions, Dictionary<string, string> electoralDistricts,
                                                                Dictionary<string, string> sectors, bool requiredCheck)
        {
            int errors = CheckCompanySecondTabValidity(address, cityId, provStateId, countryId, postalCode, mailingAddress,
                                        mailingCityId, mailingProvStateId, mailingCountryId, mailingPostalCode, regions, electoralDistricts, requiredCheck);

            if (errors == 0)
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    CommonSecondStep(company, address, cityId, provStateId, countryId, postalCode, mailingAddress, mailingCityId,
                        mailingProvStateId, mailingCountryId, mailingPostalCode, regions, electoralDistricts, sectors, ctx, false);
                }
            }

            return new KeyValuePair<int, string>(errors, company.Id.ToString());
        }

        /// <summary>
        /// This method is used to save the company outlets added to the company in storage for use
        /// in a later method. This is used for non outlet companies
        /// </summary>
        /// <param name="company"></param>
        /// <param name="strIds"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public KeyValuePair<int, string> CreateCompanyThirdTab(Company company, List<string> strIds, bool requiredCheck)
        {
            int errors = 0;
            string returnStr = null;

            List<Guid> guids = new List<Guid>();
            foreach (string str in strIds)
            {
                guids.Add(Guid.Parse(str));
            }

            HttpContext.Current.Session.Add("Company_MediaOutlets_" + company.Id, guids);
            returnStr = company.Id.ToString();

            return new KeyValuePair<int, string>(errors, returnStr);
        }

        /// <summary>
        /// this method is used to save an outlets 3rd tab of data from the edit screen. Calls the validation methods and common
        /// methods to save to the company object
        /// </summary>
        /// <param name="company"></param>
        /// <param name="mediaDesks"></param>
        /// <param name="mediaPartners"></param>
        /// <param name="distributions"></param>
        /// <param name="languages"></param>
        /// <param name="publicationDays"></param>
        /// <param name="specialPublications"></param>
        /// <param name="mediaTypes"></param>
        /// <param name="ethnicities"></param>
        /// <param name="isEthnicMedia"></param>
        /// <param name="publicationFrequencyId"></param>
        /// <param name="isMajorMedia"></param>
        /// <param name="isLiveMedia"></param>
        /// <param name="keyPrograms"></param>
        /// <param name="circulation"></param>
        /// <param name="deadlines"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public KeyValuePair<int, string> CreateMediaOutletThirdTab(Company company, Dictionary<string, string> mediaDesks, Dictionary<string, string> mediaPartners,
            Dictionary<string, string> distributions, Dictionary<string, string> languages, Dictionary<string, string> publicationDays,
            Dictionary<string, string> specialPublications, Dictionary<string, string> mediaTypes, Dictionary<string, string> ethnicities, bool isEthnicMedia,
            Guid? publicationFrequencyId, bool isMajorMedia, bool isLiveMedia, string keyPrograms, string circulation,
            string deadlines, bool requiredCheck)
        {
            int errors = 0;
            string returnVal = null;

            errors = CheckMediaOutletThirdTabValidity(distributions, languages, mediaTypes, isEthnicMedia, ethnicities, requiredCheck);
            errors = CheckMediaOutletOptionalTabValidity(errors, keyPrograms, circulation, deadlines);

            if (errors == 0)
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    CommonMediaOutletThirdTab(company, mediaDesks, mediaPartners, distributions, languages, publicationDays, specialPublications, mediaTypes, ethnicities, isEthnicMedia, publicationFrequencyId, isMajorMedia, isLiveMedia, keyPrograms, circulation, deadlines, ctx, false);
                }

                returnVal = company.Id.ToString();
            }
            return new KeyValuePair<int, string>(errors, returnVal);

        }

        /// <summary>
        /// This method takes in the company object that was stored in session and created through
        /// the tabs of the edit screen and save it to the database. 
        /// </summary>
        /// <param name="newCompany"></param>
        /// <param name="isContributor"></param>
        /// <returns></returns>
        public Guid CreateCompanyFinalize(Company newCompany, bool isContributor)
        {
            Guid returnId = Guid.Empty;
            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                string user = CommonMethods.GetLoggedInUser();

                // create the new company record with the data from what is in the database
                Company company = new Company()
                {
                    Id = Guid.NewGuid(),
                    ParentCompanyId = newCompany.ParentCompanyId,
                    CompanyName = newCompany.CompanyName,
                    CompanyDescription = newCompany.CompanyDescription,
                    CirculationDescription = newCompany.CirculationDescription,
                    Deadlines = newCompany.Deadlines,
                    KeyPrograms = newCompany.KeyPrograms,
                    PrintCategoryId = newCompany.PrintCategoryId,
                    PublicationFrequencyId = newCompany.PublicationFrequencyId,
                    IsMajorMedia = newCompany.IsMajorMedia,
                    IsLiveMedia = newCompany.IsLiveMedia,
                    IsEthnicMedia = newCompany.IsEthnicMedia,
                    IsOutlet = newCompany.IsOutlet,
                    CreationDate = newCompany.CreationDate,
                    ModifiedDate = newCompany.ModifiedDate,
                    IsActive = true,
                    RecordEditedBy = user
                };

                returnId = company.Id;

                // company address
                foreach (CompanyAddress addr in newCompany.CompanyAddresses.ToList())
                {
                    CompanyAddress companyAddress = new CompanyAddress()
                    {
                        Id = Guid.NewGuid(),
                        Company = company,
                        StreetAddress = addr.StreetAddress,
                        CityId = addr.CityId,
                        CityName = addr.CityName,
                        ProvStateId = addr.ProvStateId,
                        ProvStateName = addr.ProvStateName,
                        PostalZipCode = addr.PostalZipCode,
                        CountryId = addr.CountryId,
                        AddressType = addr.AddressType,
                        CreationDate = addr.CreationDate,
                        ModifiedDate = addr.ModifiedDate
                    };
                    company.CompanyAddresses.Add(companyAddress);
                }

                // company distribution
                foreach (Distribution dist in newCompany.Distributions)
                {
                    ctx.Distributions.Attach(dist);
                    company.Distributions.Add(dist);
                }

                // company ethnicity
                foreach (Ethnicity ethnicity in newCompany.Ethnicities)
                {
                    ctx.Ethnicities.Attach(ethnicity);
                    company.Ethnicities.Add(ethnicity);
                }

                // company language
                foreach (Language lang in newCompany.Languages)
                {
                    ctx.Languages.Attach(lang);
                    company.Languages.Add(lang);
                }

                // company media desk
                foreach (MediaDesk desk in newCompany.MediaDesks)
                {
                    ctx.MediaDesks.Attach(desk);
                    company.MediaDesks.Add(desk);
                }

                // company electoral district
                foreach (ElectoralDistrict district in newCompany.ElectoralDistricts)
                {
                    ctx.ElectoralDistricts.Attach(district);
                    company.ElectoralDistricts.Add(district);
                }

                // company media partner
                foreach (Company comp in newCompany.PartnerCompanies)
                {
                    ctx.Companies.Attach(comp);
                    company.PartnerCompanies.Add(company);
                }

                // media type
                foreach (MediaType type in newCompany.MediaTypes)
                {
                    ctx.MediaTypes.Attach(type);
                    company.MediaTypes.Add(type);
                }

                // phone numbers
                foreach (CompanyPhoneNumber number in newCompany.CompanyPhoneNumbers)
                {
                    CompanyPhoneNumber num = new CompanyPhoneNumber()
                    {
                        Id = Guid.NewGuid(),
                        Company = company,
                        PhoneNumber = number.PhoneNumber,
                        PhoneNumberExtension = number.PhoneNumberExtension,
                        PhoneTypeId = number.PhoneTypeId,
                        CreationDate = number.CreationDate,
                        ModifiedDate = number.ModifiedDate
                    };

                    company.CompanyPhoneNumbers.Add(num);
                }

                // publication days
                foreach (PublicationDay day in newCompany.PublicationDays)
                {
                    ctx.PublicationDays.Attach(day);
                    company.PublicationDays.Add(day);
                }

                // company region
                foreach (Region region in newCompany.Regions)
                {
                    ctx.Regions.Attach(region);
                    company.Regions.Add(region);
                }

                // company sector
                foreach (Sector sector in newCompany.Sectors)
                {
                    ctx.Sectors.Attach(sector);
                    company.Sectors.Add(sector);
                }

                // company specialty publication
                foreach (SpecialtyPublication sp in newCompany.SpecialtyPublications)
                {
                    ctx.SpecialtyPublications.Attach(sp);
                    company.SpecialtyPublications.Add(sp);
                }

                // web addresses
                foreach (CompanyWebAddress webAddr in newCompany.CompanyWebAddresses)
                {
                    CompanyWebAddress webAddrNew = new CompanyWebAddress()
                    {
                        Id = Guid.NewGuid(),
                        Company = company,
                        WebAddress = webAddr.WebAddress,
                        WebAddressTypeId = webAddr.WebAddressTypeId,
                        CreationDate = webAddr.CreationDate,
                        ModifiedDate = webAddr.ModifiedDate
                    };

                    company.CompanyWebAddresses.Add(webAddrNew);
                }

                ctx.Companies.Add(company);

                // company outlets
                bool isOutlet = company.IsOutlet;
                if (!isOutlet)
                {
                    if (HttpContext.Current.Session["Company_MediaOutlets_" + newCompany.Id] != null)
                    {
                        List<Guid> guids = (List<Guid>)HttpContext.Current.Session["Company_MediaOutlets_" + newCompany.Id];
                        AddParentCompanyToOutlets(company, guids, ctx);

                        HttpContext.Current.Session.Remove("Company_MediaOutlets_" + newCompany.Id);
                    }
                }

                ctx.SaveChanges();

                WriteActivityLogEntry(company, "", ActivityType.Record_Created);
            }
            return returnId;
        }

        #endregion

        static bool Change(bool? oldValue, bool newValue, Company company, string propertyName, bool dbSave)
        {
            if (dbSave && (oldValue ?? false) != newValue)
            {
                WriteActivityLogEntry(company, (newValue ? "Checked " : "Unchecked ") + propertyName);
            }
            return newValue;
        }

        static Guid Change(Guid oldValue, Guid newValue, Company company, string propertyName, bool dbSave)
        {
            if (dbSave && oldValue != newValue)
            {
                bool added = (oldValue == Guid.Empty);
                WriteActivityLogEntry(company, (added ? "Added " : "Changed ") + propertyName);
            }
            return newValue;
        }

        static string Change(string oldValue, string newValue, Company company, string propertyName, bool dbSave, bool showDetails = true)
        {
            bool added = string.IsNullOrEmpty(oldValue);
            if (oldValue == newValue || (string.IsNullOrEmpty(newValue) && added)) // null == string.Empty
                return oldValue;

            if (dbSave)
            {
                if (showDetails)
                {
                    propertyName += ": " + (added ? newValue : oldValue + " to " + newValue);
                }
                WriteActivityLogEntry(company, (added ? "Added " : "Changed ") + propertyName);
            }
            return newValue;
        }

        #region edit company/outlet

        /// <summary>
        /// This method is used by an admin to directly save the edited 1st tab data
        /// </summary>
        /// <param name="company"></param>
        /// <param name="mediaName"></param>
        /// <param name="description"></param>
        /// <param name="webAddressTypes"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="isMediaOutlet"></param>
        /// <param name="parentCompanyId"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public int EditCompanyFirstTab(Company company, MediaRelationsEntities ctx, string mediaName, string description, List<KeyValuePair<string, PhoneNumberInfo>> phoneNumbers,
            bool isMediaOutlet, Guid? parentCompanyId, bool requiredCheck)
        {
            int errors = 0;

            errors = CheckCompanyFirstTabValidity(mediaName, phoneNumbers, requiredCheck, company.CompanyName, isMediaOutlet);
            errors = CheckCompanyFirstTabOptionalValuesValidity(errors, description, phoneNumbers);

            if (errors == 0)
            {
                ctx.Companies.Attach(company); // re-attach the company instance

                company.CompanyName = Change(company.CompanyName, mediaName, company, "Company Name", true);
                company.CompanyDescription = Change(company.CompanyDescription, description, company, "Company Description", true, false);

                bool isOutlet = company.IsOutlet;

                if (isOutlet && company.ParentCompanyId != parentCompanyId)
                {
                    if (company.ParentCompanyId.HasValue)
                    {
                        WriteActivityLogEntry(ctx.Companies.Find(company.ParentCompanyId), company.CompanyName, ActivityType.Outlet_Detached);
                        company.ParentCompanyId = null;
                    }
                    if (parentCompanyId != Guid.Empty)
                    {
                        WriteActivityLogEntry(ctx.Companies.Find(parentCompanyId), company.CompanyName, ActivityType.Outlet_Attached);
                        company.ParentCompanyId = parentCompanyId;
                    }
                }

                for (int i = company.CompanyPhoneNumbers.Count() - 1; i >= 0; i--)
                {
                    CompanyPhoneNumber ccpn = company.CompanyPhoneNumbers.ElementAt(i);
                    if ((from s in phoneNumbers
                         where s.Key == ccpn.PhoneTypeId.ToString() && s.Value.PhoneNumber == ccpn.PhoneNumber && s.Value.PhoneNumberExtension == ccpn.PhoneNumberExtension
                         select s).Any()) continue;
                    WriteActivityLogEntry(company, "Removed " + ccpn.PhoneType.PhoneTypeName + ": " + ccpn.PhoneNumber);
                    ctx.CompanyPhoneNumbers.Remove(ccpn);
                }

                foreach (KeyValuePair<string, PhoneNumberInfo> kvp in phoneNumbers)
                {
                    Guid gd = Guid.Parse(kvp.Key);
                    string pn = kvp.Value.PhoneNumber;
                    string extension = kvp.Value.PhoneNumberExtension;

                    CompanyPhoneNumber oldCPN = (from s in company.CompanyPhoneNumbers
                                                 where s.PhoneTypeId == gd && s.PhoneNumber == pn && s.PhoneNumberExtension == extension
                                                 select s).FirstOrDefault();
                    if (oldCPN == null)
                    {
                        company.CompanyPhoneNumbers.Add(new CompanyPhoneNumber
                        {
                            Id = Guid.NewGuid(),
                            Company = company,
                            PhoneTypeId = gd,
                            PhoneNumber = pn,
                            PhoneNumberExtension = extension,
                            CreationDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        });
                        WriteActivityLogEntry(company, "Added " + ctx.PhoneTypes.Find(gd).PhoneTypeName + ": " + kvp.Value.PhoneNumberString);
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// Updates the contact with the data entered in the "Web Address" tab of the form
        /// </summary>
        /// <param name="con">the contact object to modify</param>
        /// <param name="ctx">the database contact to use</param>
        /// <param name="ContactWebAddresses"></param>
        /// <param name="dbSave">true to save to the database, false to save only to the session object</param>
        /// <returns>an integer representing validation errors encountered (see IsValidTabWebAddress method)</returns>
        public int EditCompanyTabWebAddress(Company company, MediaRelationsEntities ctx, IList<WebAddressDisplay> companyWebAddresses, bool dbSave)
        {
            var errors = IsValidTabWebAddress(companyWebAddresses);

            if (dbSave)
            {
                ctx.Companies.Attach(company); // re-attach the company instance
            }

            // email change if subscribed
            //NodSubscriptions.SaveSubscriberInfo(oldEmail, null, con.Email.WebAddress);

            //loop through the webaddresses and apply the changes to the contact
            foreach (WebAddressDisplay ca in companyWebAddresses)
            {
                if (ca.IsNew && !ca.IsDeleted)
                {
                    var newItem = new CompanyWebAddress();
                    newItem.Id = ca.Id;
                    newItem.WebAddressTypeId = ca.WebAddressTypeId;
                    newItem.WebAddressType = (from c in ctx.WebAddressTypes where c.Id == ca.WebAddressTypeId select c).FirstOrDefault();
                    newItem.WebAddress = ca.NewWebAddress;
                    newItem.CompanyId = company.Id;
                    newItem.ModifiedDate = DateTime.Now;
                    newItem.CreationDate = DateTime.Now;
                    company.CompanyWebAddresses.Add(newItem);
                    if (dbSave) WriteActivityLogEntry(company, "Added " + newItem.WebAddressType.WebAddressTypeName + ": " + ca.NewWebAddress);
                    if (ca.MediaDistributionLists != null && ca.MediaDistributionLists.Count() > 0)
                    {
                        NodSubscriptions.SaveSubscriberInfo(ca.NewWebAddress, ca.MediaDistributionLists);
                    }
                }

                if (!ca.IsNew && (ca.IsDeleted || ca.IsModified))
                {
                    //find the web address in the existing list and update it.
                    foreach (CompanyWebAddress existingItem in company.CompanyWebAddresses)
                    {
                        if (existingItem.Id == ca.Id)
                        {
                            if (ca.IsDeleted)
                            {
                                if (dbSave) WriteActivityLogEntry(company, "Removed " + existingItem.WebAddressType.WebAddressTypeName + ": " + existingItem.WebAddress);
                                ctx.CompanyWebAddresses.Remove(existingItem);
                                if (ca.WebAddressTypeName == WebAddressType.Email)
                                {
                                    //delete all the subscriptions if the email address is not used on another contact or outlet
                                    //there are always two entries, more than that tells you that the email address is still in use
                                    if (ca.EmailAddressInfo.Count <= 2 && ca.OriginalMediaSubscriptionCount > 0)
                                    {
                                        NodSubscriptions.SaveSubscriberInfo(ca.WebAddress, new List<string>());
                                    }
                                }
                            }
                            else
                            {
                                //modified
                                existingItem.WebAddress = Change(existingItem.WebAddress, ca.NewWebAddress, company, ctx.WebAddressTypes.Find(existingItem.WebAddressTypeId).WebAddressTypeName, dbSave);
                                if (existingItem.WebAddressType.WebAddressTypeName == WebAddressType.Email)
                                {
                                    if (ca.WebAddress == ca.NewWebAddress)
                                    {
                                        if (ca.OriginalMediaSubscriptionCount > 0 || (ca.MediaDistributionLists != null && ca.MediaDistributionLists.Count() > 0))
                                        {
                                            //just update the subscriptions
                                            NodSubscriptions.SaveSubscriberInfo(ca.WebAddress, ca.MediaDistributionLists, ca.NewWebAddress);
                                        }
                                    }
                                    else
                                    {
                                        //what to do with the subscriptions of the old email address?
                                        //if there is no other use of the email address in the contacts database, then just migrate it all
                                        if (ca.EmailAddressInfo.Count <= 2)
                                        {
                                            if (ca.OriginalMediaSubscriptionCount > 0 || (ca.MediaDistributionLists != null && ca.MediaDistributionLists.Count() > 0))
                                            {
                                                NodSubscriptions.SaveSubscriberInfo(ca.WebAddress, ca.MediaDistributionLists, ca.NewWebAddress);
                                            }
                                        }
                                        else
                                        {
                                            if (ca.MediaDistributionLists != null && ca.MediaDistributionLists.Count() > 0)
                                            {
                                                //create a new subscription for the new email address and leave the old one alone
                                                NodSubscriptions.SaveSubscriberInfo(ca.NewWebAddress, ca.MediaDistributionLists);
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// This method is called to edit a companies 2nd tab directly to the database. This is used by admins
        /// </summary>
        /// <param name="company"></param>
        /// <param name="address"></param>
        /// <param name="cityId"></param>
        /// <param name="provStateId"></param>
        /// <param name="countryId"></param>
        /// <param name="postalCode"></param>
        /// <param name="mailingAddress"></param>
        /// <param name="mailingCityId"></param>
        /// <param name="mailingProvStateId"></param>
        /// <param name="mailingCountryId"></param>
        /// <param name="mailingPostalCode"></param>
        /// <param name="regions"></param>
        /// <param name="electoralDistricts"></param>
        /// <param name="sectors"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public int EditCompanySecondTab(Company company, MediaRelationsEntities ctx, string address,
                                        Guid cityId, Guid provStateId, Guid countryId, string postalCode,
                                        string mailingAddress, Guid mailingCityId, Guid mailingProvStateId,
                                        Guid mailingCountryId, string mailingPostalCode,
                                        Dictionary<string, string> regions, Dictionary<string, string> electoralDistricts,
                                        Dictionary<string, string> sectors, bool requiredCheck)
        {
            int errors = CheckCompanySecondTabValidity(address, cityId, provStateId, countryId, postalCode, mailingAddress, mailingCityId, mailingProvStateId,
                mailingCountryId, mailingPostalCode, regions, electoralDistricts, requiredCheck);

            if (errors == 0)
            {
                ctx.Companies.Attach(company);

                CommonSecondStep(company, address, cityId, provStateId, countryId, postalCode, mailingAddress, mailingCityId,
                    mailingProvStateId, mailingCountryId, mailingPostalCode, regions, electoralDistricts, sectors, ctx, true);
            }

            return errors;
        }

        /// <summary>
        /// This method is used to save the companies outlets tab (3rd) changes to the database, used by admins.
        /// </summary>
        /// <param name="company"></param>
        /// <param name="strIds"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public int EditCompanyThirdTab(Company company, MediaRelationsEntities ctx, List<string> strIds, bool requiredCheck)
        {
            int errors = 0;

            List<Guid> guids = new List<Guid>();
            foreach (string gstr in strIds)
            {
                guids.Add(Guid.Parse(gstr));
            }

            ctx.Companies.Attach(company);

            // get the list of companies that are currently media outlets of the company
            List<Company> outlets = (from c in ctx.Companies where c.ParentCompanyId == company.Id select c).ToList();

            foreach (Company outlet in outlets)
            {
                if (!guids.Contains(outlet.Id))
                {
                    WriteActivityLogEntry(company, outlet.CompanyName, ActivityType.Outlet_Detached);
                    outlet.ParentCompanyId = null;
                }
                else
                {
                    guids.Remove(outlet.Id); // already an outlet in the system, don't need to make its parent later
                }
            }

            AddParentCompanyToOutlets(company, guids, ctx);

            return errors;
        }

        /// <summary>
        /// This method is used to directly save the media outlets 3rd (media) tab to the database. This is used by admins.
        /// </summary>
        /// <param name="company"></param>
        /// <param name="mediaDesks"></param>
        /// <param name="mediaPartners"></param>
        /// <param name="distributions"></param>
        /// <param name="languages"></param>
        /// <param name="publicationDays"></param>
        /// <param name="specialPublications"></param>
        /// <param name="mediaTypes"></param>
        /// <param name="ethnicities"></param>
        /// <param name="isEthnicMedia"></param>
        /// <param name="publicationFrequencyId"></param>
        /// <param name="isMajorMedia"></param>
        /// <param name="isLiveMedia"></param>
        /// <param name="keyPrograms"></param>
        /// <param name="circulation"></param>
        /// <param name="deadlines"></param>
        /// <param name="requiredCheck">whether the required fields are required</param>
        /// <returns></returns>
        public int EditMediaOutletThirdTab(Company company, MediaRelationsEntities ctx, Dictionary<string, string> mediaDesks, Dictionary<string, string> mediaPartners,
            Dictionary<string, string> distributions, Dictionary<string, string> languages, Dictionary<string, string> publicationDays,
            Dictionary<string, string> specialPublications, Dictionary<string, string> mediaTypes, Dictionary<string, string> ethnicities, bool isEthnicMedia,
            Guid? publicationFrequencyId, bool isMajorMedia, bool isLiveMedia, string keyPrograms, string circulation,
            string deadlines, bool requiredCheck)
        {
            int errors = 0;

            errors = CheckMediaOutletThirdTabValidity(distributions, languages, mediaTypes, isEthnicMedia, ethnicities, requiredCheck);
            errors = CheckMediaOutletOptionalTabValidity(errors, keyPrograms, circulation, deadlines);

            if (errors == 0)
            {
                ctx.Companies.Attach(company);

                CommonMediaOutletThirdTab(company, mediaDesks, mediaPartners, distributions, languages, publicationDays, specialPublications, mediaTypes, ethnicities, isEthnicMedia, publicationFrequencyId, isMajorMedia, isLiveMedia, keyPrograms, circulation, deadlines, ctx, true);
            }
            return errors;
        }

        #endregion


        #region delete company methods

        /// <summary>
        /// This method is used to delete a company from the system by company guid
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static string DeleteCompany(Guid companyId)
        {
            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Company company = (from c in ctx.Companies where c.Id == companyId && c.IsActive select c).FirstOrDefault();
                return DeleteCompany(company, ctx);
            }
        }

        /// <summary>
        /// This method is used to delete a company
        /// </summary>
        /// <param name="company"></param>
        /// <param name="ctx"></param>
        /// <param name="doLog">This is only to be set false on rejecting a pending company - that is logged in that method</param>
        /// <returns></returns>
        private static string DeleteCompany(Company company, MediaRelationsEntities ctx, bool doLog = true)
        {
            Permissions.SiteAction actions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsCompany);
            if ((actions & Permissions.SiteAction.Delete) == 0) return "Access denied";
            if (company.Outlets.Any(o => o.IsActive)) return "it has active outlets linked to it. Remove all associated outlet(s) first.";
            if (company.ContactMediaJobTitles.Any(j => j.Contact.IsActive)) return "it has active contacts linked to it. Remove all associated contact(s) first.";

            company.IsActive = false;

            RemoveCompanyObjects(company, ctx);

            ctx.SaveChanges();

            if (doLog)
            {
                WriteActivityLogEntry(company, "", ActivityType.Record_Deleted);
            }

            return null;
        }

        /// <summary>
        /// This method removes all the linking objects for companies in the system
        /// </summary>
        /// <param name="company"></param>
        /// <param name="ctx"></param>
        private static void RemoveCompanyObjects(Company company, MediaRelationsEntities ctx)
        {
            List<CompanyAddress> addresses = company.CompanyAddresses.ToList();
            for (int i = addresses.Count - 1; i >= 0; i--)
            {
                ctx.CompanyAddresses.Remove(addresses[i]);
            }

            company.Distributions.Clear();
            company.ElectoralDistricts.Clear();
            company.Ethnicities.Clear();
            company.Languages.Clear();
            company.MediaDesks.Clear();
            company.MediaTypes.Clear();

            company.PartnerCompanies.Clear();
            company.PartnerCompanies.Clear();
            company.Outlets.Clear();

            List<CompanyPhoneNumber> numbers = company.CompanyPhoneNumbers.ToList();
            for (int i = numbers.Count - 1; i >= 0; i--)
            {
                ctx.CompanyPhoneNumbers.Remove(numbers[i]);
            }

            company.PublicationDays.Clear();
            company.Regions.Clear();
            company.Sectors.Clear();
            company.SpecialtyPublications.Clear();

            List<CompanyWebAddress> companyWebAddresses = company.CompanyWebAddresses.ToList();
            for (int i = companyWebAddresses.Count - 1; i >= 0; i--)
            {
                ctx.CompanyWebAddresses.Remove(companyWebAddresses[i]);
            }

            if (!company.IsOutlet)
            {
                List<Company> childCompanies = (from c in ctx.Companies where c.ParentCompanyId == company.Id select c).ToList();
                foreach (Company child in childCompanies) child.ParentCompanyId = null;
            }

            List<ContactMediaJobTitle> jobTitles = company.ContactMediaJobTitles.ToList();
            for (int i = jobTitles.Count - 1; i >= 0; i--)
            {
                ctx.ContactMediaJobTitles.Remove(jobTitles[i]);
            }

            company.PrintCategoryId = null;
            company.PublicationFrequencyId = null;
        }

        /// <summary>
        /// This is used to delete a company from the system, removes all information from the database
        /// </summary>
        /// <param name="company"></param>
        /// <param name="ctx"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool PermanentlyDeleteCompany(Company company, MediaRelationsEntities ctx, string user)
        {
            string companyName = company.CompanyName;
            Guid companyId = company.Id;

            RemoveCompanyObjects(company, ctx);

            ctx.Companies.Remove(company);

            ctx.SaveChanges();

            WriteActivityLogEntry(ActivityType.Record_Purged, EntityType.Company,
                companyId, companyName, Guid.Empty, "", user);

            return true;
        }

        #endregion

        /// <summary>
        /// This method is called to change a company to an outlet in the system. This ensures that the company
        /// is not an outlet and does exist
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public int ConvertCompanyToOutlet(Guid guid)
        {
            int errors = 0;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Company company = (from c in ctx.Companies where c.Id == guid select c).FirstOrDefault();

                if (company == null) errors |= 1;
                else
                {
                    if (company.IsOutlet) errors |= 2;

                    int count = (from c in ctx.Companies where c.CompanyName == company.CompanyName where c.IsOutlet == true select c).Count();
                    if (count < 0) errors |= 4;

                    if (errors == 0)
                    {
                        List<Company> subCompanies = (from c in ctx.Companies where c.ParentCompanyId == company.Id select c).ToList();

                        foreach (Company sub in subCompanies)
                        {
                            sub.ParentCompanyId = null;
                        }

                        WriteActivityLogEntry(company, "Converted to Outlet");
                        company.IsOutlet = true;
                        ctx.SaveChanges();
                    }
                }
            }
            return errors;
        }
    }
}
