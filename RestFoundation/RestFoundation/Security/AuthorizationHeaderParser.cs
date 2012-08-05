// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace RestFoundation.Security
{
    internal static class AuthorizationHeaderParser
    {
        private const string AuthenticationTypeKey = "auth-type";
        private const string AuthenticationCredentialsKey = "auth-credentials";
        private const string UserNameKey = "username";
        private const string AuthorizationSeparator = " ";
        private const string AuthorizationItemSeparator = ",";
        private const string NameValueSeparator = "=";

        private static readonly Regex nameValueSeparatorRegex = new Regex(@"\s+=\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public static bool TryParse(string authorizationString, Encoding encoding, out AuthorizationHeader header)
        {
            if (String.IsNullOrWhiteSpace(authorizationString))
            {
                header = null;
                return false;
            }

            var items = ParseAuthorizationItems(authorizationString);

            string authenticationType;

            if (!items.TryGetValue(AuthenticationTypeKey, out authenticationType))
            {
                header = null;
                return false;
            }

            bool isBasic = "Basic".Equals(authenticationType, StringComparison.OrdinalIgnoreCase);

            if (!isBasic && !"Digest".Equals(authenticationType, StringComparison.OrdinalIgnoreCase))
            {
                header = null;
                return false;
            }

            string credentials, userName, password;

            if (isBasic && items.TryGetValue(AuthenticationCredentialsKey, out credentials))
            {
                Tuple<string, string> decodedCredentials = DecodeCredentials(credentials, encoding);

                if (decodedCredentials != null)
                {
                    userName = decodedCredentials.Item1;
                    password = decodedCredentials.Item2;
                }
                else
                {
                    userName = null;
                    password = null;
                }
            }
            else
            {
                if (!items.TryGetValue(UserNameKey, out userName))
                {
                    userName = null;
                }

                password = null;
            }

            if (String.IsNullOrWhiteSpace(userName))
            {
                header = null;
                return false;
            }

            header = new AuthorizationHeader(authenticationType, userName, GenerateParameters(items));

            if (password != null)
            {
                header.Password = password;
            }

            return true;
        }

        private static Tuple<string, string> DecodeCredentials(string credentials, Encoding encoding)
        {
            string decodedCredentials;

            try
            {
                decodedCredentials = encoding.GetString(Convert.FromBase64String(credentials));
            }
            catch (Exception)
            {
                decodedCredentials = null;
            }

            if (String.IsNullOrEmpty(decodedCredentials))
            {
                return null;
            }

            string[] credentialItems = decodedCredentials.Split(':');

            if (credentialItems.Length != 2 || credentialItems[0].Length == 0)
            {
                return null;
            }

            return Tuple.Create(credentialItems[0], credentialItems[1]);
        }

        private static IDictionary<string, string> ParseAuthorizationItems(string authorizationString)
        {
            string[] items = nameValueSeparatorRegex.Replace(authorizationString, NameValueSeparator)
                                                    .Split(new[]
                                                            {
                                                                AuthorizationSeparator,
                                                                AuthorizationItemSeparator
                                                            }, StringSplitOptions.RemoveEmptyEntries);

            var itemDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                string[] parts;

                if (!item.EndsWith(NameValueSeparator, StringComparison.Ordinal))
                {
                    parts = item.Split(new[] { NameValueSeparator }, 2, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    parts = new[] { item };
                }

                if (parts.Length == 1 && itemDictionary.Count == 0)
                {
                    itemDictionary[AuthenticationTypeKey] = parts[0];
                }
                else if (parts.Length == 1 && itemDictionary.Count > 0)
                {
                    itemDictionary[AuthenticationCredentialsKey] = parts[0];
                }
                else if (parts.Length == 2)
                {
                    itemDictionary[parts[0].Trim().ToLowerInvariant()] = parts[1].Trim().Trim('"');
                }
            }

            return itemDictionary;
        }

        private static NameValueCollection GenerateParameters(IEnumerable<KeyValuePair<string, string>> items)
        {
            var parameters = new NameValueCollection();

            foreach (KeyValuePair<string, string> item in items)
            {
                if (!String.Equals(item.Key, AuthenticationTypeKey) &&
                    !String.Equals(item.Key, AuthenticationCredentialsKey) &&
                    !String.Equals(item.Key, UserNameKey, StringComparison.OrdinalIgnoreCase))
                {
                    parameters.Add(item.Key, item.Value);
                }
            }

            return parameters;
        }
    }
}
