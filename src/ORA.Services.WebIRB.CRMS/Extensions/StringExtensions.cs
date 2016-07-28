using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;

namespace ORA.Services.WebIRBCRMS.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Attempts to convert an object to an enumeration of strings by calling
        /// a comma separated ToString() overload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToStrings<T>(this T source)
        {
            return source.ToString().Split(',').Select(x => x.Trim());
        }

        public static string RemoveInvalidXmlChars( this string text )
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            var validXmlChars = text.Replace("\r\n", " ")
                                .Replace("\n", " ")
                                .Replace("\r", " ")
                                .Replace("\t", " ")
                                .Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            return new string(validXmlChars);
        }

        public static string TrimString( this string text)
        {
            if (string.IsNullOrEmpty(text))
            { 
                return null;
            }
            return text.Trim();
        }
    }
}