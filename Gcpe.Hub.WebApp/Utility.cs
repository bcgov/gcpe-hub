using System;
using System.Collections.Generic;

namespace Gcpe.Hub
{
    public static class Utility
    {
        public static string FormatPhoneNumberAndExtension(string numberAndExtension)
        {
            string numberWithoutExtension;
            string extension = GetPhoneExtension(numberAndExtension, out numberWithoutExtension);

            return FormatPhoneNumber(numberWithoutExtension) + (string.IsNullOrEmpty(extension) ? "" : " x " + extension);
        }

        /// <summary>
        /// This method takes in a phone number from the system and formats it
        /// to a friendly string
        /// </summary>
        /// <param name="phoneNumber">a phone number that may or may not already be formatted</param>
        /// <returns>a formatted phone number with dashes</returns>
        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return string.Empty;

            if (phoneNumber.Length == 7)
            {
                return $"250-{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3)}";
            }

            if (phoneNumber.Length == 10)
            {
                return $"{phoneNumber.Substring(0, 3)}-{phoneNumber.Substring(3, 3)}-{phoneNumber.Substring(6)}";
            }

            if (phoneNumber.Length == 11 && phoneNumber[0] == '1')
            {
                return $"({phoneNumber.Substring(1, 3)}) {phoneNumber.Substring(4, 3)}-{phoneNumber.Substring(7)}";
            }

            return phoneNumber;
        }

        public static string GetPhoneExtension(string numberAndExtension, out string numberWithoutExtension)
        {
            string result = string.Empty;

            if (numberAndExtension != null)
            {
                int separator = numberAndExtension.IndexOf('x');
                if (separator == -1)
                {
                    numberWithoutExtension = numberAndExtension;
                }
                else
                {
                    numberWithoutExtension = numberAndExtension.Substring(0, separator).TrimEnd();
                    result = numberAndExtension.Substring(separator + 1).Trim();
                }
            }
            else
            {
                numberWithoutExtension = numberAndExtension;
            }
            return result;
        }

        public static IEnumerable<T> Yield<T>(this T item)
        {
            if (item == null)
                return new T[0];

            return new T[] { item };
        }
    }
}