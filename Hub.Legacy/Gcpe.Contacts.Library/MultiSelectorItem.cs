using MediaRelationsDatabase;

namespace MediaRelationsLibrary
{
    public class MultiSelectorItem
    {
        public string Name;
        public bool Selected;
        public string ValidationRegex;
        public string TextValue;
        public bool ShowSubItem;

        public MultiSelectorItem(string name, bool selected, string validationRegex, string textValue, bool showSubItem = false)
        {
            Name = name;
            Selected = selected;
            ValidationRegex = validationRegex;
            TextValue = textValue;
            ShowSubItem = showSubItem;
        }

        public const string EMAIL_REGEX = "^[\\w\\.\\-]+\\@[\\w\\.\\-]+$";
        public const string WEBSITE_REGEX = "^http[s]?\\:\\/\\/[\\w\\-\\.\\/]+$";
        public const string NOT_EMPTY_REGEX = "[^\\s]+";
        public const string PHONE_REGEX = @"^(?:\+?1[- ]?)?\(?([0-9]{3})\)?[- ]?([0-9]{3})[-]?([0-9]{4})$|^\+(?:[0-9] ?){6,14}[0-9]$|^\+[0-9]{1,3}\.[0-9]{4,14}(?:x.+)?$"; // 10 digit or international number

        public static string GetValidationRegex(string fieldName)
        {
            string result = NOT_EMPTY_REGEX;

            if (fieldName.ToLower().Contains("email") || fieldName.ToLower().Contains("e-mail"))
            {
                result = EMAIL_REGEX;
            }
            else if (fieldName.ToLower().Contains("website") || fieldName.ToLower().Contains("url"))
            {
                result = NOT_EMPTY_REGEX;
            }

            return result;
        }

        public static string GetLinkPrefix(WebAddressType waType)
        {
            string urlPrefix = null;
            if (waType.WebAddressTypeName.ToLower().Contains("twitter")) urlPrefix = "http://www.twitter.com/";
            else if (waType.WebAddressTypeName.ToLower().Contains("email")) urlPrefix = "mailto:";

            return urlPrefix;
        }

        public static string GetLink(WebAddressType webAddressType, string webAddressValue)
        {
            if (webAddressType.WebAddressTypeName == WebAddressType.GooglePlus ||
                webAddressType.WebAddressTypeName == WebAddressType.Facebook ||
                webAddressType.WebAddressTypeName == WebAddressType.Website)
            {

                string returnVal = "<a target='_blank' href='";

                if (!webAddressValue.ToLower().StartsWith("http://") && !webAddressValue.ToLower().StartsWith("https://"))
                {
                    returnVal += "http://" + webAddressValue;
                }
                else
                {
                    returnVal += webAddressValue;
                }

                returnVal += "'>" + webAddressValue + "</a>";

                return returnVal;
            }

            if (webAddressType.WebAddressTypeName == WebAddressType.Email) return GetEmailLink(webAddressValue);
            if (webAddressType.WebAddressTypeName == WebAddressType.Twitter) return GetTwitterLink(webAddressValue);

            string linkPrefix = GetLinkPrefix(webAddressType);
            return "<a href='" + linkPrefix + webAddressValue + "'>" + webAddressValue + "</a>";
        }

        #region get link types

        public static string GetPhoneNumberLink(string phoneNumber)
        {
            return "<a href='tel:" + phoneNumber + "'>" + phoneNumber + "</a>";
        }

        public static string GetTwitterLink(string twitterHandle)
        {
            return "<a href='http://www.twitter.com/" + twitterHandle + "' target='_blank'>@" + twitterHandle + "</a>";
        }

        public static string GetEmailLink(string emailAddress)
        {
            return "<a href='mailto:" + emailAddress + "'>" + emailAddress + "</a>";
        }

        #endregion
    }
}
