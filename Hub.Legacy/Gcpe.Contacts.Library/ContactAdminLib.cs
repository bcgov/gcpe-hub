using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Data.Entity.Validation;
using MediaRelationsDatabase;
using static MediaRelationsLibrary.CommonEventLogging;

namespace MediaRelationsLibrary
{
    public class ContactAdminLib
    {
        #region Constructor
        /// <summary>
        /// constructor, initializes logging class
        /// </summary>
        public ContactAdminLib()
        {
        }
        #endregion

        #region Validation
        /// <summary>
        /// checks the data from the "Contact" tab of the form for validity
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="webAddresses"></param>
        /// <param name="phoneNumbers"></param>
        /// <returns>
        /// 0 if no errors are encountered, or a bitwise-or of the following:
        /// 1 - first name empty
        /// 2 - last name empty
        /// 4 - no phone numbers supplied
        /// 8 - one or more phone numbers has invalid format
        /// 16 - no primary phone number supplied
        /// </returns>
        public int IsValidTab1(string firstName, string lastName, List<KeyValuePair<string, PhoneNumberInfo>> phoneNumbers, string currentFirstName, string currentLastName, string showNotes)
        {
            int errors = 0;

            if (string.IsNullOrWhiteSpace(firstName)) errors |= 1;
            if (string.IsNullOrWhiteSpace(lastName)) errors |= 2;
            if (phoneNumbers.Count == 0)
            {
                errors |= 4;
            }
            else
            {
                //bool foundPrimary = false;
                foreach (KeyValuePair<string, PhoneNumberInfo> kvp in phoneNumbers)
                {
                    if (!Regex.IsMatch(kvp.Value.PhoneNumber, MultiSelectorItem.PHONE_REGEX)) errors |= 8;
                    //else if (kvp.Key == CommonMethods.PrimaryPhoneNumberPhoneType.PhoneTypeId.ToString()) foundPrimary = true;
                }
                //if (!foundPrimary) errors |= 16;
            }


            if ((currentFirstName == null && currentLastName == null) || !(currentFirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) && currentLastName.Equals(lastName, StringComparison.OrdinalIgnoreCase)))
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    int count = (from c in ctx.Contacts where c.FirstName == firstName where c.LastName == lastName where c.IsActive select c).Count();
                    if (count > 0) errors |= 32;
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
        /// checks the data from the "Location" tab of the form for validity
        /// </summary>
        /// <param name="address"></param>
        /// <param name="city"></param>
        /// <param name="province"></param>
        /// <param name="country"></param>
        /// <param name="regions"></param>
        /// <param name="electoralDistricts"></param>
        /// <returns>
        /// 0 if no errors are encountered, or a bitwise-or of the following:
        /// 1 - city is not selected/entered
        /// 2 - province is not selected/entered
        /// 4 - country is not selected/entered
        /// 8 - no regions have been selected
        /// 16 - no electoral districts have been selected
        /// </returns>
        private int IsValidTab2(string address, string city, string province, string country, Dictionary<string, string> regions, Dictionary<string, string> electoralDistricts)
        {
            int errors = 0;

            if (string.IsNullOrWhiteSpace(country))
            {
                if (!string.IsNullOrWhiteSpace(city) || !string.IsNullOrWhiteSpace(province)) errors |= 4;
            }
            //if (regions.Count == 0) errors |= 8;
            //if (electoralDistricts.Count == 0) errors |= 16;
            if (address.Length > 250) errors |= 32;

            return errors;
        }

        /// <summary>
        /// checks the data from the "Ministry" tab of the form for validity
        /// </summary>
        /// <param name="ministry"></param>
        /// <param name="ministerialJobTitle"></param>
        /// <param name="mlaAssignment"></param>
        /// <returns>
        /// 0 if no errors are encountered, or a bitwise-or of the following:
        /// 1 - ministry name not selected
        /// 2 - ministerial job title not selected
        /// 4 - mla assignment not selected
        /// </returns>
        public int IsValidTab4(string ministry, string ministerialJobTitle, string mlaAssignment, MediaRelationsEntities ctx)
        {
            int errors = 0;

            if (string.IsNullOrWhiteSpace(ministry))
            {
                //no requirements
            }
            else
            {
                //enable ministerial job title
                if (string.IsNullOrWhiteSpace(ministerialJobTitle))
                {
                    errors |= 1;
                }
                else
                {
                    Guid ministerialJobTitleId;
                    Guid.TryParse(ministerialJobTitle, out ministerialJobTitleId);

                    string ministerialJobTitleStr = (from jtitle in ctx.MinisterialJobTitles where jtitle.Id == ministerialJobTitleId select jtitle.MinisterialJobTitleName).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(ministerialJobTitleStr))
                    {
                        errors |= 1;
                    }
                    else if (ministerialJobTitleStr.ToLower() == "minister")
                    {
                        if (string.IsNullOrWhiteSpace(mlaAssignment))
                        {
                            errors |= 2;
                        }
                    }
                }
            }

            return errors;
        }

        #endregion

        #region Add Edit Contact
        /// <summary>
        /// Updates the contact with the data entered in the "Contact" tab of the form
        /// </summary>
        /// <param name="con">the contact to modify</param>
        /// <param name="ctx">the database context to use</param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="mediaJobTitles"></param>
        /// <param name="webAddresses"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="dbSave">true to save to the database, false to save only to the session object</param>
        /// <returns>an integer representing validation errors encountered (see IsValidTab1 method)</returns>
        public int UpdateContactTab1(Contact con, MediaRelationsEntities ctx, string firstName, string lastName, List<KeyValuePair<string, string>> mediaJobTitles, List<KeyValuePair<string, PhoneNumberInfo>> phoneNumbers, string showNotes, bool dbSave)
        {
            int result = IsValidTab1(firstName, lastName, phoneNumbers, con.FirstName, con.LastName, showNotes);

            if (result == 0)
            {
                con.FirstName = Change(con.FirstName, firstName, con, "First Name", dbSave);
                con.LastName = Change(con.LastName, lastName, con, "Last Name", dbSave);
                con.ShowNotes = Change(con.ShowNotes, showNotes, con, "Show Notes", dbSave, false);

                for (int i = con.ContactMediaJobTitles.Count() - 1; i >= 0; i--)
                {
                    ContactMediaJobTitle cmjt = con.ContactMediaJobTitles.ElementAt(i);
                    if ((from s in mediaJobTitles where s.Key == cmjt.MediaJobTitleId.ToString() && s.Value == cmjt.CompanyId.ToString() select s).Any()) continue;
                    if (dbSave)
                    {
                        WriteActivityLogEntry(con, "Removed Job: " + cmjt.MediaJobTitle.MediaJobTitleName + " @ " + cmjt.Company.CompanyName);
                        ctx.ContactMediaJobTitles.Remove(cmjt);
                    }
                    else con.ContactMediaJobTitles.Remove(cmjt);
                }

                foreach (KeyValuePair<string, string> kvp in mediaJobTitles)
                {
                    Guid gd = Guid.Parse(kvp.Key);
                    Guid cgd = Guid.Parse(kvp.Value);
                    ContactMediaJobTitle oldCMJT = (from s in con.ContactMediaJobTitles where s.MediaJobTitleId == gd && s.CompanyId == cgd select s).FirstOrDefault();
                    if (oldCMJT == null)
                    {
                        Company cmp = (from comp in ctx.Companies where comp.Id == cgd select comp).FirstOrDefault();
                        con.ContactMediaJobTitles.Add(new ContactMediaJobTitle
                        {
                            Id = Guid.NewGuid(),
                            Contact = con,
                            Company = cmp,
                            CompanyId = cgd,
                            MediaJobTitleId = gd
                        });
                        if (dbSave)
                        {
                            WriteActivityLogEntry(con, "Added Job: " + ctx.MediaJobTitles.Find(gd).MediaJobTitleName + " @ " + ctx.Companies.Find(cgd).CompanyName);
                        }
                    }
                }


                for (int i = con.ContactPhoneNumbers.Count() - 1; i >= 0; i--)
                {
                    ContactPhoneNumber ccpn = con.ContactPhoneNumbers.ElementAt(i);
                    string phoneNumber = ccpn.PhoneNumber.Replace("-", "");
                    var existingPhone = phoneNumbers.FirstOrDefault(p => p.Key == ccpn.PhoneTypeId.ToString() && p.Value.PhoneNumber.Replace("-", "") == phoneNumber && p.Value.PhoneNumberExtension == ccpn.PhoneNumberExtension).Value;
                    if (existingPhone != null)
                    {
                        ccpn.PhoneNumber = existingPhone.PhoneNumber;// in case it wasn't formatted
                        continue;
                    }

                    if (dbSave)
                    {
                        WriteActivityLogEntry(con, "Removed " + ccpn.PhoneType.PhoneTypeName + ": " + ccpn.PhoneNumber);
                        ctx.ContactPhoneNumbers.Remove(ccpn); // remove AFTER having accessed ccpn.PhoneType or it gets null
                    }
                    else con.ContactPhoneNumbers.Remove(ccpn);
                }

                foreach (KeyValuePair<string, PhoneNumberInfo> kvp in phoneNumbers)
                {
                    Guid gd = Guid.Parse(kvp.Key);
                    string pn = kvp.Value.PhoneNumber;
                    string extension = kvp.Value.PhoneNumberExtension;
                    ContactPhoneNumber oldCPN = (from s in con.ContactPhoneNumbers where s.PhoneTypeId == gd && s.PhoneNumber == pn && s.PhoneNumberExtension == extension select s).FirstOrDefault();
                    if (oldCPN == null)
                    {
                        //PhoneType ptp = (from s in ctx.PhoneTypes where s.PhoneTypeId == gd select s).FirstOrDefault();
                        con.ContactPhoneNumbers.Add(new ContactPhoneNumber
                        {
                            Id = Guid.NewGuid(),
                            Contact = con,
                            PhoneTypeId = gd,
                            PhoneNumber = pn,
                            PhoneNumberExtension = extension,
                            CreationDate = DateTime.Now,
                            ModifiedDate = DateTime.Now
                        });
                        if (dbSave) WriteActivityLogEntry(con, "Added " + ctx.PhoneTypes.Find(gd).PhoneTypeName + ": " + kvp.Value.PhoneNumberString);
                    }
                }

                if (!dbSave)
                {
                    HttpContext.Current.Session["mru_edit_contact_" + con.Id] = con;
                }

            }
            return result;
        }

        private void ApplyWebAddressesToContact(Contact con, MediaRelationsEntities ctx, IList<WebAddressDisplay> contactWebAddresses, bool dbSave)
        {
            //loop through the webaddresses and apply the changes to the contact
            foreach (WebAddressDisplay ca in contactWebAddresses)
            {
                if (ca.IsNew && !ca.IsDeleted)
                {
                    var newItem = new ContactWebAddress();
                    newItem.Id = ca.Id;
                    newItem.WebAddressTypeId = ca.WebAddressTypeId;
                    newItem.WebAddressType = ctx.WebAddressTypes.Find(ca.WebAddressTypeId);
                    newItem.WebAddress = ca.NewWebAddress;
                    newItem.ContactId = con.Id;
                    newItem.ModifiedDate = DateTime.Now;
                    newItem.CreationDate = DateTime.Now;
                    con.ContactWebAddresses.Add(newItem);
                    if (dbSave) WriteActivityLogEntry(con, "Added " + newItem.WebAddressType.WebAddressTypeName + ": " + ca.NewWebAddress);
                    if (ca.MediaDistributionLists != null && ca.MediaDistributionLists.Count() > 0)
                    {
                        NodSubscriptions.SaveSubscriberInfo(ca.NewWebAddress, ca.MediaDistributionLists);
                    }
                }

                if (!ca.IsNew && (ca.IsDeleted || ca.IsModified))
                {
                    //find the web address in the existing list and update it.
                    foreach (ContactWebAddress existingItem in con.ContactWebAddresses)
                    {
                        if (existingItem.Id == ca.Id)
                        {
                            if (ca.IsDeleted)
                            {
                                if (dbSave)
                                {
                                    WriteActivityLogEntry(con, "Removed " + existingItem.WebAddressType.WebAddressTypeName + ": " + existingItem.WebAddress);
                                    ctx.ContactWebAddresses.Remove(existingItem);
                                }
                                else
                                {
                                    con.ContactWebAddresses.Remove(existingItem);
                                }

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
                                existingItem.WebAddress = Change(existingItem.WebAddress, ca.NewWebAddress, con, ctx.WebAddressTypes.Find(existingItem.WebAddressTypeId).WebAddressTypeName, dbSave);

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

        }

        /// <summary>
        /// Updates the contact with the data entered in the "Web Address" tab of the form
        /// </summary>
        /// <param name="con">the contact object to modify</param>
        /// <param name="ctx">the database contact to use</param>
        /// <param name="ContactWebAddresses"></param>
        /// <param name="dbSave">true to save to the database, false to save only to the session object</param>
        /// <returns>an integer representing validation errors encountered (see IsValidTabWebAddress method)</returns>
        public int UpdateContactTabWebAddress(Contact con, MediaRelationsEntities ctx, IList<WebAddressDisplay> contactWebAddresses, bool dbSave)
        {
            var errors = IsValidTabWebAddress(contactWebAddresses);


            ApplyWebAddressesToContact(con, ctx, contactWebAddresses, dbSave);

            if (!dbSave)
            {
                HttpContext.Current.Session["mru_edit_contact_" + con.Id] = con;
            }

            return errors;
        }

        static Guid? Change(Guid? oldValue, string newValueStr, Contact con, string propertyName, bool dbSave)
        {
            Guid newValue;
            if (!Guid.TryParse(newValueStr, out newValue) || oldValue == newValue) return oldValue;
            if (dbSave)
            {
                bool added = !oldValue.HasValue || oldValue.Value == Guid.Empty;
                WriteActivityLogEntry(con, (added ? "Added " : "Changed ") + propertyName);
            }
            return newValue;
        }

        static string Change(string oldValue, string newValue, Contact con, string propertyName, bool dbSave, bool showDetails = true)
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
                WriteActivityLogEntry(con, (added ? "Added " : "Changed ") + propertyName);
            }
            return newValue;
        }

        /// <summary>
        /// Updates the contact with the data entered in the "Location" tab of the form
        /// </summary>
        /// <param name="con">the contact object to modify</param>
        /// <param name="ctx">the database contact to use</param>
        /// <param name="address"></param>
        /// <param name="city"></param>
        /// <param name="customCity"></param>
        /// <param name="province"></param>
        /// <param name="customProvince"></param>
        /// <param name="country"></param>
        /// <param name="postalCode"></param>
        /// <param name="regions"></param>
        /// <param name="electoralDistricts"></param>
        /// <param name="sectors"></param>
        /// <param name="dbSave">true to save to the database, false to save only to the session object</param>
        /// <returns>an integer representing validation errors encountered (see IsValidTab2 method)</returns>
        public int UpdateContactTab2(Contact con, MediaRelationsEntities ctx, string address, string city, string customCity, string province, string customProvince, string countryId, string postalCode, Dictionary<string, string> regions, Dictionary<string, string> electoralDistricts, Dictionary<string, string> sectors, bool dbSave)
        {
            int result = IsValidTab2(address, city, province, countryId, regions, electoralDistricts);
            if (result == 0)
            {
                Guid temp;
                if (Guid.TryParse(countryId, out temp))
                {
                    ContactAddress contactAddress = con.ContactAddresses.FirstOrDefault();
                    if (contactAddress == null)
                    {
                        contactAddress = new ContactAddress();
                        contactAddress.Id = Guid.NewGuid();
                        contactAddress.CreationDate = DateTime.Now;
                        contactAddress.ModifiedDate = DateTime.Now;
                    }

                    con.ContactAddresses.Clear();
                    contactAddress.CountryId = (Guid)Change(contactAddress.CountryId, countryId, con, "Country", dbSave);
                    contactAddress.PostalZipCode = Change(contactAddress.PostalZipCode ?? "", postalCode, con, "Postal/Zip Code", dbSave);
                    contactAddress.StreetAddress = Change(contactAddress.StreetAddress ?? "", address, con, "Street Address", dbSave);

                    if (Guid.TryParse(city, out temp) && contactAddress.CityId != temp)
                    {
                        contactAddress.CityId = temp;
                        contactAddress.CityName = Change(contactAddress.CityName, ctx.Cities.Find(contactAddress.CityId)?.CityName ?? customCity, con, "City", dbSave);
                    }

                    if (Guid.TryParse(province, out temp) && contactAddress.ProvStateId != temp)
                    {
                        contactAddress.ProvStateId = temp;
                        contactAddress.ProvStateName = Change(contactAddress.ProvStateName, ctx.ProvStates.Find(contactAddress.ProvStateId)?.ProvStateName ?? customProvince, con, "Prov/State", dbSave);
                    }

                    con.ContactAddresses.Add(contactAddress);
                }


                for (int i = con.Regions.Count - 1; i >= 0; i--)
                {
                    Region reg = con.Regions.ElementAt(i);
                    if (!Remove(con.Regions, reg, !regions.Any(r => r.Key == reg.Id.ToString()))) continue;
                    if (dbSave) WriteActivityLogEntry(con, "Removed Region: " + reg.RegionName);
                }

                foreach (string key in regions.Keys)
                {
                    Guid gd = Guid.Parse(key);
                    Region reg = (from rg in ctx.Regions where rg.Id == gd select rg).FirstOrDefault();
                    if (reg == null || !Add(con.Regions, reg)) continue;
                    if (dbSave) WriteActivityLogEntry(con, "Added Region: " + reg.RegionName);
                }

                for (int i = con.ElectoralDistricts.Count - 1; i >= 0; i--)
                {
                    ElectoralDistrict dst = con.ElectoralDistricts.ElementAt(i);
                    if (!Remove(con.ElectoralDistricts, dst, !electoralDistricts.Any(r => r.Key == dst.Id.ToString()))) continue;
                    if (dbSave) WriteActivityLogEntry(con, "Removed District: " + dst.DistrictName);
                }

                foreach (string key in electoralDistricts.Keys)
                {
                    Guid gd = Guid.Parse(key);
                    ElectoralDistrict dst = (from dist in ctx.ElectoralDistricts where dist.Id == gd select dist).FirstOrDefault();
                    if (dst == null || !Add(con.ElectoralDistricts, dst)) continue;
                    if (dbSave) WriteActivityLogEntry(con, "Added District: " + dst.DistrictName);
                }

                for (int i = con.Sectors.Count - 1; i >= 0; i--)
                {
                    Sector sec = con.Sectors.ElementAt(i);
                    if (!Remove(con.Sectors, sec, !sectors.Any(r => r.Key == sec.Id.ToString()))) continue;
                    if (dbSave) WriteActivityLogEntry(con, "Removed Sector: " + sec.DisplayName);
                }

                foreach (string key in sectors.Keys)
                {
                    Guid gd = Guid.Parse(key);
                    Sector sec = (from s in ctx.Sectors where s.Id == gd select s).FirstOrDefault();
                    if (sec == null || !Add(con.Sectors, sec)) continue;
                    if (dbSave) WriteActivityLogEntry(con, "Added Sector: " + sec.DisplayName);
                }
            }
            return result;
        }

        /// <summary>
        /// Updates the contact with the data entered in the "Media" tab of the form
        /// </summary>
        /// <param name="con">the contact to modify</param>
        /// <param name="ctx">the database context to use</param>
        /// <param name="pressGallery"></param>
        /// <param name="dbSave">true to save to the database, false to save only to the session object</param>
        /// <returns>an integer representing validation errors encountered (see IsValidTab3 method)</returns>
        public int UpdateContactTab3(Contact con, MediaRelationsEntities ctx, bool pressGallery, List<KeyValuePair<string, string>> beats, bool dbSave)
        {
            for (int i = con.ContactBeats.Count - 1; i >= 0; i--)
            {
                ContactBeat cbet = con.ContactBeats.ElementAt(i);
                if (beats.Any(r => r.Key == cbet.BeatId.ToString() && r.Value == cbet.CompanyId.ToString())) continue;
                if (dbSave)
                {
                    string cpyName = cbet.Company.CompanyName;
                    WriteActivityLogEntry(con, "Removed Beat: " + cbet.Beat.BeatName + " for Outlet " + cpyName);
                    ctx.ContactBeats.Remove(cbet);
                }
                else con.ContactBeats.Remove(cbet);
            }

            foreach (KeyValuePair<string, string> kvp in beats)
            {
                Guid gd = Guid.Parse(kvp.Key);
                Guid cgd = Guid.Parse(kvp.Value);
                ContactBeat oldCBT = (from s in con.ContactBeats where s.BeatId == gd && s.CompanyId == cgd select s).FirstOrDefault();
                if (oldCBT == null)
                {
                    con.ContactBeats.Add(new ContactBeat
                    {
                        Id = Guid.NewGuid(),
                        Contact = con,
                        CompanyId = cgd,
                        BeatId = gd
                    });
                    if (dbSave)
                    {
                        string cpyName = ctx.Companies.Find(cgd).CompanyName;
                        WriteActivityLogEntry(con, "Added Beat: " + ctx.Beats.Find(gd).BeatName + " for Outlet " + cpyName);
                    }
                }
            }

            if (con.IsPressGallery != pressGallery)
            {
                con.IsPressGallery = pressGallery;
                if (dbSave) WriteActivityLogEntry(con, (pressGallery ? "Checked: " : "Unchecked: ") + "Press Gallery");
            }
            return 0;
        }

        /// <summary>
        /// Updates the contact with the data entered in the "Ministry" tab of the form
        /// </summary>
        /// <param name="con">the contact object to modify</param>
        /// <param name="ctx">the database context to use</param>
        /// <param name="ministry"></param>
        /// <param name="ministerialJobTitle"></param>
        /// <param name="mlaAssignment"></param>
        /// <param name="dbSave"></param>true to save to the database, false to save only to the session object</param>
        /// <returns>an integer representing validation errors encountered (see IsValidTab4 method)</returns>
        public int UpdateContactTab4(Contact con, MediaRelationsEntities ctx, string ministryId, string ministerialJobTitle, string mlaAssignment, bool dbSave)
        {
            int result = IsValidTab4(ministryId, ministerialJobTitle, mlaAssignment, ctx);
            if (result == 0)
            {
                con.MinistryId = Change(con.MinistryId, ministryId, con, "Ministry", dbSave);
                con.MLAAssignmentId = Change(con.MLAAssignmentId, mlaAssignment, con, "MLA assignment", dbSave);

                Guid temp;
                if (Guid.TryParse(ministerialJobTitle, out temp) && con.MinisterialJobTitleId != temp)
                {
                    bool added = !con.MinisterialJobTitleId.HasValue;
                    con.MinisterialJobTitleId = temp;
                    var ministerialJobTitleName = ctx.MinisterialJobTitles.Find(con.MinisterialJobTitleId)?.MinisterialJobTitleName;
                    if (dbSave) WriteActivityLogEntry(con, (added ? "Added Ministerial Job Title: " : "Changed Ministerial Job Title to: " + ministerialJobTitleName));
                    if (ministerialJobTitleName != null)
                    {
                        ministerialJobTitleName = ministerialJobTitleName.ToLower();
                        if (ministerialJobTitleName.Contains("assistant")) con.HasMinisterAssignment = true;
                        else con.HasMinisterAssignment = false;
                        if (ministerialJobTitleName.Contains("primary")) con.IsPrimaryMediaContact = true;
                        else con.IsPrimaryMediaContact = false;
                        if (ministerialJobTitleName.Contains("secondary")) con.IsSecondaryMediaContact = true;
                        else con.IsSecondaryMediaContact = false;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// for adding contacts only - inserts the contact into the database
        /// </summary>
        /// <param name="sessionCon">the contact object that has been stored in session</param>
        /// <returns>the guid of the contact inserted into the database</returns>
        public Guid FinalizeContact(Contact sessionCon)
        {
            bool isAdmin = Permissions.IsAdmin();

            using (var ctx = new MediaRelationsEntities())
            {
                Contact con = new Contact
                {
                    IsActive = true,

                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,

                    FirstName = sessionCon.FirstName,
                    LastName = sessionCon.LastName,
                    ShowNotes = sessionCon.ShowNotes,

                    IsPressGallery = sessionCon.IsPressGallery,

                    MinistryId = sessionCon.MinistryId,
                    MinisterialJobTitleId = sessionCon.MinisterialJobTitleId,
                    MLAAssignmentId = sessionCon.MLAAssignmentId,
                    HasMinisterAssignment = sessionCon.HasMinisterAssignment,
                    IsPrimaryMediaContact = sessionCon.IsPrimaryMediaContact,
                    IsSecondaryMediaContact = sessionCon.IsSecondaryMediaContact
                };

                ContactAddress oldAddress = sessionCon.ContactAddresses.FirstOrDefault();
                if (oldAddress != null)
                {
                    ContactAddress contactAddress = new ContactAddress
                    {
                        Id = Guid.NewGuid(),
                        CreationDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        StreetAddress = oldAddress.StreetAddress,
                        CityId = oldAddress.CityId,
                        CityName = oldAddress.CityName,
                        ProvStateId = oldAddress.ProvStateId,
                        ProvStateName = oldAddress.ProvStateName,
                        CountryId = oldAddress.CountryId,
                        PostalZipCode = oldAddress.PostalZipCode
                    };
                    con.ContactAddresses.Add(contactAddress);
                }

                foreach (ContactMediaJobTitle job in sessionCon.ContactMediaJobTitles)
                {
                    ctx.Companies.Attach(job.Company);
                    //con.Companies.Add(cmp);
                }

                foreach (ContactWebAddress cwa in sessionCon.ContactWebAddresses)
                {
                    var newCwa = new ContactWebAddress
                    {
                        Id = Guid.NewGuid(),
                        CreationDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        WebAddressTypeId = cwa.WebAddressTypeId,
                        WebAddress = cwa.WebAddress
                    };
                    con.ContactWebAddresses.Add(newCwa);
                }
                foreach (ContactPhoneNumber cpn in sessionCon.ContactPhoneNumbers)
                {
                    var newCpn = new ContactPhoneNumber
                    {
                        Id = Guid.NewGuid(),
                        CreationDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        PhoneTypeId = cpn.PhoneTypeId,
                        PhoneNumber = cpn.PhoneNumber,
                        PhoneNumberExtension = cpn.PhoneNumberExtension
                    };
                    con.ContactPhoneNumbers.Add(newCpn);
                }
                foreach (Region reg in sessionCon.Regions)
                {
                    ctx.Regions.Attach(reg);
                    con.Regions.Add(reg);
                }
                foreach (ElectoralDistrict dst in sessionCon.ElectoralDistricts)
                {
                    ctx.ElectoralDistricts.Attach(dst);
                    con.ElectoralDistricts.Add(dst);
                }
                foreach (Sector sec in sessionCon.Sectors)
                {
                    ctx.Sectors.Attach(sec);
                    con.Sectors.Add(sec);
                }
                foreach (ContactMediaJobTitle mjt in sessionCon.ContactMediaJobTitles)
                {
                    //ctx.ContactMediaJobTitles.Attach(mjt);
                    var newMJT = new ContactMediaJobTitle
                    {
                        ContactId = con.Id,
                        MediaJobTitleId = mjt.MediaJobTitleId,
                        CompanyId = mjt.CompanyId,
                        Id = Guid.NewGuid()
                    };
                    con.ContactMediaJobTitles.Add(newMJT);
                }
                foreach (ContactBeat bet in sessionCon.ContactBeats)
                {
                    var newBET = new ContactBeat
                    {
                        ContactId = con.Id,
                        BeatId = bet.BeatId,
                        CompanyId = bet.CompanyId,
                        Id = Guid.NewGuid()
                    };
                    con.ContactBeats.Add(newBET);
                }

                con.RecordEditedBy = CommonMethods.GetLoggedInUser();
                ctx.Contacts.Add(con);
                ctx.SaveChanges();

                WriteActivityLogEntry(con, "", ActivityType.Record_Created);

                return con.Id;
            }
        }

        #endregion

        #region Get Contact Info External
        /// <summary>
        /// check if the contact identified by the guid exists
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>true if the contact exists, false otherwise</returns>
        public bool ContactExists(Guid guid)
        {
            using (var ctx = new MediaRelationsEntities())
            {
                var con = (from contact in ctx.Contacts where contact.Id == guid select contact).FirstOrDefault();

                if (con != null) return true;
            }
            return false;
        }

        /// <summary>
        /// checks if the record was last edited by the currently logged in user
        /// </summary>
        /// <param name="guid">guid of the contact to check</param>
        /// <returns>true if the last edit was performed by the logged in user, false otherwise</returns>
        public bool WasEditedByLoggedInUser(Guid guid)
        {
            using (var ctx = new MediaRelationsEntities())
            {
                var con = (from contact in ctx.Contacts where contact.Id == guid select contact).FirstOrDefault();
                if (con != null)
                {
                    return WasEditedByLoggedInUser(con);
                }
            }
            return false;
        }

        /// <summary>
        /// checks if the record was last edited by the currently logged in user
        /// </summary>
        /// <param name="con">the contact to check</param>
        /// <returns>true if the last edit was performed by the logged in user, false otherwise</returns>
        public bool WasEditedByLoggedInUser(Contact con)
        {
            string recordEditedBy = con.RecordEditedBy;
            string loggedInUser = CommonMethods.GetLoggedInUser();
            if (recordEditedBy != null && loggedInUser != null && recordEditedBy.ToLower().Equals(loggedInUser.ToLower())) return true;
            return false;
        }

        #endregion

        #region delete contact

        /// <summary>
        /// This method is used to delete (de-activate) a contact from database 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string DeleteContact(Guid guid, string altContact = null)
        {
            Permissions.SiteAction perm = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsContact);
            if ((perm & Permissions.SiteAction.Delete) == 0) return "An error occurred deleting the contact";

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                Contact contact = (from con in ctx.Contacts where con.Id == guid where con.IsActive select con).FirstOrDefault();
                if (contact == null) return "The contact does not exist anymore";
                if (contact.MediaRequestContacts.Count != 0)
                {
                    Contact alt = null;
                    int posSpace = altContact != null ? altContact.IndexOf(' ') : -1;
                    if (posSpace == -1)
                    {
                        return "This contact is linked to media requests and can only be deleted when viewing it";
                    }
                    string firstName = altContact.Substring(0, posSpace);
                    alt = (from con in ctx.Contacts where con.FirstName == firstName && con.LastName == altContact.Substring(posSpace + 1) && con.IsActive select con).FirstOrDefault();
                    if (alt == null) return "An alternate contact cannot be found for '" + altContact + "'";
                    if (alt.Id != contact.Id)
                    {
                        for (int i = contact.MediaRequestContacts.Count - 1; i >= 0; i--)
                        {
                            var mediaRequestContact = contact.MediaRequestContacts.ElementAt(i);
                            ContactMediaJobTitle newOutlet = alt.ContactMediaJobTitles.FirstOrDefault(j => j.CompanyId == mediaRequestContact.CompanyId);
                            if (newOutlet == null) return "The alternate contact does not belong to '" + mediaRequestContact.Company.CompanyName + "'";

                            contact.MediaRequestContacts.Remove(mediaRequestContact);
                            alt.MediaRequestContacts.Add(new MediaRequestContact { ContactId = alt.Id, CompanyId = mediaRequestContact.CompanyId, MediaRequestId = mediaRequestContact.MediaRequestId });
                            mediaRequestContact.MediaRequest.ModifiedAt = DateTime.Now; // to trigger an Azure reindex
                        }
                    }
                }
                RemoveContactObjects(contact, ctx);

                contact.IsActive = false;
                contact.ModifiedDate = DateTime.Now;
                ctx.SaveChanges();

                WriteActivityLogEntry(contact, "", ActivityType.Record_Deleted);
            }

            return null;
        }

        /// <summary>
        /// This method permanently deletes a contact with the 90 day purge rule
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="ctx"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool PermanentlyDeleteContact(Contact contact, MediaRelationsEntities ctx, string user)
        {
            RemoveContactObjects(contact, ctx);

            ctx.Contacts.Remove(contact);
            ctx.SaveChanges();

            WriteActivityLogEntry(contact, "", ActivityType.Record_Purged);

            return true;
        }

        /// <summary>
        /// This method removes the contact linking objects from the contact object
        /// </summary>
        /// <param name="con"></param>
        /// <param name="ctx"></param>
        private static void RemoveContactObjects(Contact con, MediaRelationsEntities ctx)
        {
            //con.Companies.Clear();

            List<ContactWebAddress> cwas = con.ContactWebAddresses.ToList();
            for (int i = cwas.Count - 1; i >= 0; i--)
            {
                ctx.ContactWebAddresses.Remove(cwas[i]);
            }

            List<ContactPhoneNumber> cpns = con.ContactPhoneNumbers.ToList();
            for (int i = cpns.Count - 1; i >= 0; i--)
            {
                ctx.ContactPhoneNumbers.Remove(cpns[i]);
            }

            List<ContactAddress> cdrs = con.ContactAddresses.ToList();
            for (int i = cdrs.Count - 1; i >= 0; i--)
            {
                ctx.ContactAddresses.Remove(cdrs[i]);
            }
            List<ContactMediaJobTitle> cmts = con.ContactMediaJobTitles.ToList();
            for (int i = cmts.Count - 1; i >= 0; i--)
            {
                ctx.ContactMediaJobTitles.Remove(cmts[i]);
            }
            List<ContactBeat> bets = con.ContactBeats.ToList();
            for (int i = bets.Count - 1; i >= 0; i--)
            {
                ctx.ContactBeats.Remove(bets[i]);
            }

            con.Regions.Clear();
            con.ElectoralDistricts.Clear();
            con.Sectors.Clear();

            con.MinistryId = null;
            con.MinisterialJobTitleId = null;
            con.MLAAssignmentId = null;
        }

        #endregion
    }
}
