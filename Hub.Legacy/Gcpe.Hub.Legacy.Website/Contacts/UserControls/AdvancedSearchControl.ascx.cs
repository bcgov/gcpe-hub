using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using MediaRelationsDatabase;
using MediaRelationsLibrary;
using System.Text;

public partial class UserControls_AdvancedSearchControl : System.Web.UI.UserControl
{
    public bool CancelButtonDisplayed
    {
        set
        {
            cancelButton.Visible = value;
        }
    }

    private string[] GetValuesForSearchType(SearchLib.CriteriaType type)
    {
        if (Request.QueryString.GetValues(type.ToString()) != null && Request.QueryString.GetValues(type.ToString()).Length > 0)
        {
            return Request.QueryString.GetValues(type.ToString());
        }
        return null;
    }

    /// <summary>
    /// gets the search criteria from the query string as an exact match
    /// </summary>
    public List<KeyValuePair<SearchLib.CriteriaType, string>> SearchCriteria
    {
        get
        {
            List<KeyValuePair<SearchLib.CriteriaType, string>> criteria = new List<KeyValuePair<SearchLib.CriteriaType, string>>();

            List<SearchLib.CriteriaType> criteriaTypes = new List<SearchLib.CriteriaType>((SearchLib.CriteriaType[])Enum.GetValues(typeof(SearchLib.CriteriaType)));

            criteriaTypes.Sort(delegate (SearchLib.CriteriaType a, SearchLib.CriteriaType b)
            {
                return a.ToString().CompareTo(b.ToString());
            });


            foreach (SearchLib.CriteriaType type in criteriaTypes)
            {
                string[] searchCriteria = GetValuesForSearchType(type);
                if (searchCriteria != null)
                {
                    foreach (string crit in searchCriteria)
                    {
                        if (!string.IsNullOrWhiteSpace(crit))
                        {
                            criteria.Add(new KeyValuePair<SearchLib.CriteriaType, string>(type, Server.UrlDecode(crit.Trim())));
                        }
                    }
                }
            }

            /*if (!string.IsNullOrWhiteSpace(searchByNameTb.Text)) {
                criteria.Add(new KeyValuePair<SearchLib.CriteriaType,string>(SearchLib.CriteriaType.Name, searchByNameTb.Text.Trim()));
            }*/

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<SearchLib.CriteriaType, string> pair in criteria)
            {
                sb.Append(pair.Key + " " + pair.Value + "<br/>\n");
            }

            return criteria;
        }
    }

    public string SearchCriteriaQueryUrl
    {
        get
        {
            string url = "";

            List<KeyValuePair<SearchLib.CriteriaType, string>> list = SearchCriteria;

            bool first = true;
            foreach (KeyValuePair<SearchLib.CriteriaType, string> pair in list)
            {
                if (!first) url += "&";
                url += pair.Key + "=" + Server.UrlEncode(pair.Value);
                first = false;
            }

            if (!MatchAll)
            {
                url += (first ? "" : "&") + "matchall=false";
            }

            return url;
        }
    }

    private string GetSearchCriteriaDatabaseValue(SearchLib.CriteriaType type, string value)
    {
        string criteriaValue = value;

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            Guid guid;
            switch (type)
            {
                case SearchLib.CriteriaType.Beat:
                    guid = Guid.Parse(value);
                    Beat beat = (from bt in ctx.Beats where bt.Id == guid select bt).FirstOrDefault();
                    if (beat != null) criteriaValue = beat.BeatName;
                    break;
                case SearchLib.CriteriaType.City:
                    guid = Guid.Parse(value);
                    City city = (from c in ctx.Cities where c.Id == guid select c).FirstOrDefault();
                    if (city != null) criteriaValue = city.CityName;
                    break;
                case SearchLib.CriteriaType.Province:
                    guid = Guid.Parse(value);
                    ProvState provState = (from ps in ctx.ProvStates where ps.Id == guid select ps).FirstOrDefault();
                    if (provState != null) criteriaValue = provState.ProvStateName;
                    break;
                case SearchLib.CriteriaType.Country:
                    guid = Guid.Parse(value);
                    Country country = (from c in ctx.Countries where c.Id == guid select c).FirstOrDefault();
                    if (country != null) criteriaValue = country.CountryName;
                    break;
                case SearchLib.CriteriaType.Region:
                    guid = Guid.Parse(value);
                    Region region = (from rg in ctx.Regions where rg.Id == guid select rg).FirstOrDefault();
                    if (region != null) criteriaValue = region.RegionName;
                    break;
                case SearchLib.CriteriaType.ElectoralDistrict:
                    guid = Guid.Parse(value);
                    ElectoralDistrict district = (from dist in ctx.ElectoralDistricts where dist.Id == guid select dist).FirstOrDefault();
                    if (district != null) criteriaValue = district.DistrictName;
                    break;
                case SearchLib.CriteriaType.Sector:
                    guid = Guid.Parse(value);
                    Sector sector = (from sec in ctx.Sectors where sec.Id == guid select sec).FirstOrDefault();
                    if (sector != null) criteriaValue = sector.DisplayName;
                    break;
                case SearchLib.CriteriaType.MediaJobTitle:
                    guid = Guid.Parse(value);
                    MediaJobTitle title = (from jt in ctx.MediaJobTitles where jt.Id == guid select jt).FirstOrDefault();
                    if (title != null) criteriaValue = title.MediaJobTitleName;
                    break;
                case SearchLib.CriteriaType.Ministry:
                case SearchLib.CriteriaType.Minister:
                    guid = Guid.Parse(value);
                    Ministry ministry = (from m in ctx.Ministries where m.Id == guid select m).FirstOrDefault();
                    if (ministry != null) criteriaValue = (type == SearchLib.CriteriaType.Ministry) ? ministry.DisplayName : ministry.MinisterName;
                    break;
                case SearchLib.CriteriaType.MinisterialJobTitle:
                    guid = Guid.Parse(value);
                    MinisterialJobTitle mTitle = (from jt in ctx.MinisterialJobTitles where jt.Id == guid select jt).FirstOrDefault();
                    if (mTitle != null) criteriaValue = mTitle.MinisterialJobTitleName;
                    break;
                case SearchLib.CriteriaType.MLAAssignment:
                    guid = Guid.Parse(value);
                    district = (from dist in ctx.ElectoralDistricts where dist.Id == guid select dist).FirstOrDefault();
                    if (district != null) criteriaValue = district.DistrictName;
                    break;
                case SearchLib.CriteriaType.MediaDesk:
                    guid = Guid.Parse(value);
                    MediaDesk mediaDesk = (from desk in ctx.MediaDesks where desk.Id == guid select desk).FirstOrDefault();
                    if (mediaDesk != null) criteriaValue = mediaDesk.MediaDeskName;
                    break;
                case SearchLib.CriteriaType.Distribution:
                    guid = Guid.Parse(value);
                    Distribution distribution = (from dist in ctx.Distributions where dist.Id == guid select dist).FirstOrDefault();
                    if (distribution != null) criteriaValue = distribution.DistributionName;
                    break;
                case SearchLib.CriteriaType.Language:
                    guid = Guid.Parse(value);
                    Language lang = (from l in ctx.Languages where l.Id == guid select l).FirstOrDefault();
                    if (lang != null) criteriaValue = lang.LanguageName;
                    break;
                case SearchLib.CriteriaType.PublicationDays:
                    guid = Guid.Parse(value);
                    PublicationDay day = (from pubday in ctx.PublicationDays where pubday.Id == guid select pubday).FirstOrDefault();
                    if (day != null) criteriaValue = day.PublicationDaysName;
                    break;
                case SearchLib.CriteriaType.SpecialtyPublication:
                    guid = Guid.Parse(value);
                    SpecialtyPublication special = (from spub in ctx.SpecialtyPublications where spub.Id == guid select spub).FirstOrDefault();
                    if (special != null) criteriaValue = special.SpecialtyPublicationName;
                    break;
                case SearchLib.CriteriaType.MediaType:
                    guid = Guid.Parse(value);
                    MediaType mediaType = (from mtype in ctx.MediaTypes where mtype.Id == guid select mtype).FirstOrDefault();
                    if (mediaType != null) criteriaValue = mediaType.MediaTypeName;
                    break;
                case SearchLib.CriteriaType.PrintCategory:
                    guid = Guid.Parse(value);
                    PrintCategory category = (from cat in ctx.PrintCategories where cat.Id == guid select cat).FirstOrDefault();
                    if (category != null) criteriaValue = category.PrintCategoryName;
                    break;
                case SearchLib.CriteriaType.Ethnicity:
                    guid = Guid.Parse(value);
                    Ethnicity ethninity = (from ethn in ctx.Ethnicities where ethn.Id == guid select ethn).FirstOrDefault();
                    if (ethninity != null) criteriaValue = ethninity.EthnicityName;
                    break;
                case SearchLib.CriteriaType.PublicationFrequency:
                    guid = Guid.Parse(value);
                    PublicationFrequency freq = (from pf in ctx.PublicationFrequencies where pf.Id == guid select pf).FirstOrDefault();
                    if (freq != null) criteriaValue = freq.PublicationFrequencyName;
                    break;
                case SearchLib.CriteriaType.MediaCategory:
                    SearchLib.ReportType rType;
                    if (Enum.TryParse(value, out rType))
                        criteriaValue = GetFriendlyDisplayName(rType);
                    break;
            }
        }

        return criteriaValue;
    }

    public string SearchCriteriaString
    {
        get
        {
            string criteria = "";
            List<KeyValuePair<SearchLib.CriteriaType, string>> list = SearchCriteria;

            bool first = true;
            foreach (KeyValuePair<SearchLib.CriteriaType, string> pair in list)
            {
                if (!first) criteria += ", ";
                criteria += GetFriendlyDisplayName(pair.Key) + ": " + GetSearchCriteriaDatabaseValue(pair.Key, pair.Value);
                first = false;
            }

            return criteria;
        }
    }

    public bool MatchAll
    {
        get
        {
            bool matchAll;
            bool.TryParse(Request.QueryString["matchall"], out matchAll);
            return matchAll;
        }
    }

    public string SearchText
    {
        get
        {
            return searchByNameTb.Text.Trim();
        }
    }

    private string GetFriendlyDisplayName(SearchLib.CriteriaType type)
    {
        if (type == SearchLib.CriteriaType.MLAAssignment) return "MLA Assignment";

        string currentString = type.ToString();
        return GetFriendlyDisplayName(currentString);
    }

    private string GetFriendlyDisplayName(SearchLib.DateRangeType type)
    {
        if (type == SearchLib.DateRangeType.Last7Days) return "Last 7 Days";
        if (type == SearchLib.DateRangeType.Last14Days) return "Last 14 Days";
        if (type == SearchLib.DateRangeType.Last30Days) return "Last 30 Days";

        string currentString = type.ToString();
        return GetFriendlyDisplayName(currentString);
    }

    private static string GetFriendlyDisplayName(string currentString)
    {
        string newString = "";

        for (int i = 0; i < currentString.Length; i++)
        {
            if (i == 0) newString += currentString[i];
            else
            {
                if (currentString[i].ToString().ToUpper() == currentString[i].ToString()) newString += " ";
                newString += currentString[i];
            }
        }

        return newString;
    }

    private string GetFriendlyDisplayName(SearchLib.ReportType type)
    {
        string currentString = type.ToString();
        return GetFriendlyDisplayName(currentString);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        searchByNameTb.Attributes.Add("placeholder", "Enter Name");

        saveReportButtonLink.ClientIDMode = System.Web.UI.ClientIDMode.Static;

        criteriaSelectedItems.ClientIDMode = ClientIDMode.Static;
        searchValueTextBox.ClientIDMode = ClientIDMode.Static;

        List<SearchLib.CriteriaType> criteriaTypes = new List<SearchLib.CriteriaType>((SearchLib.CriteriaType[])Enum.GetValues(typeof(SearchLib.CriteriaType)));

        criteriaTypes.Sort(delegate (SearchLib.CriteriaType a, SearchLib.CriteriaType b)
        {
            return a.ToString().CompareTo(b.ToString());
        });

        if (!IsPostBack)
        {
            searchTypeListBox.Items.Add(new ListItem("Search By", ""));
            foreach (SearchLib.CriteriaType val in criteriaTypes)
            {
                if (val != SearchLib.CriteriaType.Name) searchTypeListBox.Items.Add(new ListItem(GetFriendlyDisplayName(val), val.ToString()));
            }


            if (!string.IsNullOrWhiteSpace(Request.QueryString["name"]))
            {
                searchByNameTb.Text = Request.QueryString["name"].Trim();
            }

            bool matchAll;
            bool.TryParse(Request.QueryString["matchall"], out matchAll);

            if (matchAll)
            {
                matchAllRb.Checked = true;
                matchAnyRb.Checked = false;
            }
            else
            {
                matchAllRb.Checked = false;
                matchAnyRb.Checked = true;
            }

        }

        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type='text/javascript'>\n");

            sb.Append("var listOfItems = new Array();\n");
            sb.Append("var selectedItems = new Array();\n");

            foreach (SearchLib.CriteriaType criteriaType in criteriaTypes)
            {
                if (criteriaType == SearchLib.CriteriaType.Name) continue;

                sb.Append("listOfItems[\"" + criteriaType + "\"] = new Array();\n");
                sb.Append("listOfItems[\"" + criteriaType + "\"].push(['Select " + GetFriendlyDisplayName(criteriaType) + "', '']);\n");

                string[] submittedValues = null;
                submittedValues = Request.QueryString.GetValues(criteriaType.ToString());

                if (criteriaType == SearchLib.CriteriaType.EmailOrWebsite ||
                    // handle the ones that are specificly text boxes
                    criteriaType == SearchLib.CriteriaType.PhoneNumber ||
                    criteriaType == SearchLib.CriteriaType.StreetAddress ||
                    criteriaType == SearchLib.CriteriaType.OtherCity ||
                    criteriaType == SearchLib.CriteriaType.OtherProvince ||
                    criteriaType == SearchLib.CriteriaType.PostalCode
                    )
                {
                    if (submittedValues != null && submittedValues.Length > 0)
                    {
                        foreach (string val in submittedValues)
                        {
                            sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                        "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                        "\",\"" + Server.HtmlEncode(val) +
                                        "\",\"" + Server.HtmlEncode(val) + "\"]);\n");
                        }
                    }
                }
                else if (criteriaType == SearchLib.CriteriaType.CreatedDateRange
                  || criteriaType == SearchLib.CriteriaType.ModifiedDateRange)
                {
                    var dateTypes = Enum.GetValues(typeof(SearchLib.DateRangeType));

                    foreach (SearchLib.DateRangeType dateType in dateTypes)
                    {
                        sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + GetFriendlyDisplayName(dateType) + "\",\"" + dateType + "\"]);\n");

                        if (submittedValues != null)
                        {
                            if (submittedValues.Contains(dateType.ToString()))
                            {
                                sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                    "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                    "\",\"" + Server.HtmlEncode(GetFriendlyDisplayName(dateType)) +
                                    "\",\"" + Server.HtmlEncode(dateType.ToString()) + "\"]);\n");
                            }
                        }
                    }
                }
                else
                {
                    switch (criteriaType)
                    {
                        case SearchLib.CriteriaType.Beat:
                            List<Beat> beats = (from c in ctx.Beats orderby c.BeatName ascending select c).ToList();

                            foreach (Beat beat in beats)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + beat.BeatName + "\", \"" + beat.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(beat.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(beat.BeatName) +
                                            "\",\"" + Server.HtmlEncode(beat.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.City:
                            List<City> cities = (from c in ctx.Cities orderby c.CityName ascending select c).ToList();
                            foreach (City city in cities)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + city.CityName + "\", \"" + city.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(city.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(city.CityName) +
                                            "\",\"" + Server.HtmlEncode(city.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.Province:
                            List<ProvState> provs = (from c in ctx.ProvStates orderby c.ProvStateName ascending select c).ToList();
                            foreach (ProvState prov in provs)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + prov.ProvStateName + "\", \"" + prov.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(prov.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(prov.ProvStateName) +
                                            "\",\"" + Server.HtmlEncode(prov.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.Country:
                            List<Country> countries = (from c in ctx.Countries orderby c.CountryName ascending select c).ToList();
                            foreach (var country in countries)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + country.CountryName + "\", \"" + country.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(country.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(country.CountryName) +
                                            "\",\"" + Server.HtmlEncode(country.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.Region:
                            List<Region> regions = (from c in ctx.Regions orderby c.RegionName ascending select c).ToList();
                            foreach (var region in regions)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + region.RegionName + "\", \"" + region.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(region.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(region.RegionName) +
                                            "\",\"" + Server.HtmlEncode(region.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.ElectoralDistrict:
                            List<ElectoralDistrict> districts = (from c in ctx.ElectoralDistricts orderby c.DistrictName ascending select c).ToList();

                            foreach (ElectoralDistrict district in districts)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + district.DistrictName + "\", \"" + district.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(district.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(district.DistrictName) +
                                            "\",\"" + Server.HtmlEncode(district.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.Sector:
                            List<Sector> sectors = (from sec in ctx.Sectors where sec.IsActive orderby sec.DisplayName ascending select sec).ToList();
                            foreach (var sector in sectors)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + sector.DisplayName + "\", \"" + sector.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(sector.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(sector.DisplayName) +
                                            "\",\"" + Server.HtmlEncode(sector.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.MediaJobTitle:
                            List<MediaJobTitle> mediaJobTitles = (from c in ctx.MediaJobTitles orderby c.MediaJobTitleName ascending select c).ToList();
                            foreach (MediaJobTitle jobTitle in mediaJobTitles)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + jobTitle.MediaJobTitleName + "\", \"" + jobTitle.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(jobTitle.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(jobTitle.MediaJobTitleName) +
                                            "\",\"" + Server.HtmlEncode(jobTitle.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.Ministry:
                            List<Ministry> ministries = (from m in ctx.Ministries where m.IsActive && m.MinisterName != "" orderby m.DisplayName ascending select m).ToList();
                            foreach (Ministry item in ministries)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + item.DisplayName + "\", \"" + item.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(item.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(item.DisplayName) +
                                            "\",\"" + Server.HtmlEncode(item.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.MinisterialJobTitle:
                            List<MinisterialJobTitle> ministerialJobTitle = (from c in ctx.MinisterialJobTitles orderby c.MinisterialJobTitleName ascending select c).ToList();
                            foreach (MinisterialJobTitle jobTitle in ministerialJobTitle)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + jobTitle.MinisterialJobTitleName + "\", \"" + jobTitle.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(jobTitle.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(jobTitle.MinisterialJobTitleName) +
                                            "\",\"" + Server.HtmlEncode(jobTitle.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.MLAAssignment:
                            List<ElectoralDistrict> electoralDistricts = (from c in ctx.ElectoralDistricts orderby c.DistrictName ascending select c).ToList();

                            foreach (ElectoralDistrict district in electoralDistricts)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + district.DistrictName + "\", \"" + district.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(district.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(district.DistrictName) +
                                            "\",\"" + Server.HtmlEncode(district.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.Minister:
                            List<Ministry> ministers = (from m in ctx.Ministries where m.IsActive && m.MinisterName != "" orderby m.MinisterName ascending select m).ToList();
                            foreach (Ministry item in ministers)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + item.MinisterName + "\", \"" + item.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(item.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(item.MinisterName) +
                                            "\",\"" + Server.HtmlEncode(item.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.MediaDesk:
                            List<MediaDesk> mediaDesks = (from desk in ctx.MediaDesks orderby desk.Id ascending select desk).ToList();
                            foreach (var mediaDesk in mediaDesks)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + mediaDesk.MediaDeskName + "\", \"" + mediaDesk.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(mediaDesk.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(mediaDesk.MediaDeskName) +
                                            "\",\"" + Server.HtmlEncode(mediaDesk.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.Distribution:
                            List<Distribution> distributions = (from c in ctx.Distributions orderby c.DistributionName ascending select c).ToList();
                            foreach (var distrib in distributions)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + distrib.DistributionName + "\", \"" + distrib.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(distrib.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(distrib.DistributionName) +
                                            "\",\"" + Server.HtmlEncode(distrib.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.Language:
                            List<Language> languages = (from c in ctx.Languages orderby c.LanguageName ascending select c).ToList();
                            foreach (var lang in languages)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + lang.LanguageName + "\", \"" + lang.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(lang.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(lang.LanguageName) +
                                            "\",\"" + Server.HtmlEncode(lang.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.PublicationDays:
                            List<PublicationDay> pubDays = (from c in ctx.PublicationDays orderby c.SortOrder ascending select c).ToList();
                            foreach (var pubDay in pubDays)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + pubDay.PublicationDaysName + "\", \"" + pubDay.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(pubDay.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(pubDay.PublicationDaysName) +
                                            "\",\"" + Server.HtmlEncode(pubDay.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.SpecialtyPublication:
                            List<SpecialtyPublication> specialPublications = (from c in ctx.SpecialtyPublications orderby c.SpecialtyPublicationName ascending select c).ToList();
                            foreach (var specialtyPub in specialPublications)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + specialtyPub.SpecialtyPublicationName + "\", \"" + specialtyPub.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(specialtyPub.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(specialtyPub.SpecialtyPublicationName) +
                                            "\",\"" + Server.HtmlEncode(specialtyPub.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.MediaType:
                            List<MediaType> mediaTypes = (from c in ctx.MediaTypes orderby c.MediaTypeName ascending select c).ToList();
                            foreach (var mediaType in mediaTypes)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + mediaType.MediaTypeName + "\", \"" + mediaType.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(mediaType.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(mediaType.MediaTypeName) +
                                            "\",\"" + Server.HtmlEncode(mediaType.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.Ethnicity:
                            List<Ethnicity> ethnicities = (from c in ctx.Ethnicities orderby c.EthnicityName ascending select c).ToList();
                            foreach (var ethnicity in ethnicities)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + ethnicity.EthnicityName + "\", \"" + ethnicity.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(ethnicity.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(ethnicity.EthnicityName) +
                                            "\",\"" + Server.HtmlEncode(ethnicity.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.PublicationFrequency:
                            List<PublicationFrequency> frequencies = (from c in ctx.PublicationFrequencies orderby c.PublicationFrequencyName ascending select c).ToList();
                            foreach (PublicationFrequency frequency in frequencies)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + frequency.PublicationFrequencyName + "\", \"" + frequency.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(frequency.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(frequency.PublicationFrequencyName) +
                                            "\",\"" + Server.HtmlEncode(frequency.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.PrintCategory:
                            List<PrintCategory> categories = (from c in ctx.PrintCategories orderby c.PrintCategoryName ascending select c).ToList();
                            foreach (PrintCategory category in categories)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + category.PrintCategoryName + "\", \"" + category.Id + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(category.Id.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(category.PrintCategoryName) +
                                            "\",\"" + Server.HtmlEncode(category.Id.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                        case SearchLib.CriteriaType.MediaCategory:

                            var reportTypes = Enum.GetValues(typeof(SearchLib.ReportType));

                            foreach (SearchLib.ReportType reportType in reportTypes)
                            {
                                sb.Append("listOfItems[\"" + criteriaType + "\"].push([\"" + GetFriendlyDisplayName(reportType) + "\",\"" + reportType + "\"]);\n");

                                if (submittedValues != null)
                                {
                                    if (submittedValues.Contains(reportType.ToString()))
                                    {
                                        sb.Append("selectedItems.push([\"" + Server.HtmlEncode(GetFriendlyDisplayName(criteriaType)) +
                                            "\",\"" + Server.HtmlEncode(criteriaType.ToString()) +
                                            "\",\"" + Server.HtmlEncode(GetFriendlyDisplayName(reportType)) +
                                            "\",\"" + Server.HtmlEncode(reportType.ToString()) + "\"]);\n");
                                    }
                                }
                            }
                            break;
                    }
                }

            }

            sb.Append("var tbItems = new Array();\n");
            sb.Append("tbItems.push('" + SearchLib.CriteriaType.EmailOrWebsite + "');\n");
            sb.Append("tbItems.push('" + SearchLib.CriteriaType.PhoneNumber + "');\n");
            sb.Append("tbItems.push('" + SearchLib.CriteriaType.StreetAddress + "');\n");
            sb.Append("tbItems.push('" + SearchLib.CriteriaType.PostalCode + "');\n");
            sb.Append("tbItems.push('" + SearchLib.CriteriaType.OtherCity + "');\n");
            sb.Append("tbItems.push('" + SearchLib.CriteriaType.OtherProvince + "');\n");

            sb.Append("</script>\n");
            jsArrayLit.Text = sb.ToString();

        }

        if (SearchCriteria.Count == 0) saveReportButtonLink.Visible = false;

    }

    protected void SubmitSearchClick(object sender, EventArgs e)
    {
        StringBuilder sb = GetSearchqueryString();

        Response.Redirect("~/Contacts/Search.aspx" + sb);
    }

    private StringBuilder GetSearchqueryString()
    {
        string valueStr = advancedSearchDesktopHidden.Value.Trim();

        string[] split = valueStr.Split('|');

        StringBuilder sb = new StringBuilder();

        sb.Append("?matchall=" + (matchAllRb.Checked ? "true" : "false"));

        if (!string.IsNullOrWhiteSpace(searchByNameTb.Text)) sb.Append("&name=" + Server.UrlEncode(searchByNameTb.Text.Trim()));

        foreach (string item in split)
        {
            if (string.IsNullOrWhiteSpace(item)) continue;
            string[] record = item.Split('=');

            if (record.Length == 2 &&
                !string.IsNullOrWhiteSpace(record[0]) &&
                !string.IsNullOrWhiteSpace(record[1]))
            {
                sb.Append("&");
                sb.Append(record[0] + "=" + Server.UrlEncode(record[1]));
            }

        }
        return sb;
    }

    protected void CancelButtonClick(object sender, EventArgs e)
    {
        Response.Redirect("~/Contacts/");
    }

}