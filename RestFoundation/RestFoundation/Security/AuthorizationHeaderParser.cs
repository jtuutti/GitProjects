// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace RestFoundation.Security
{
    /// <summary>
    /// Represents an authorization header parser.
    /// </summary>
    public static class AuthorizationHeaderParser
    {
        private const string ApiUserName = "key";
        private const string AuthenticationTypeKey = "auth-type";
        private const string AuthenticationCredentialsKey = "auth-credentials";
        private const string UserNameKey = "username";

        private static readonly Regex AuthorizationRegex = new Regex("^\\s*([^,^\\s]+)\\s*([^,^\\s]+)", RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex AuthorizationParameterRegex = new Regex("[^,^\\s]+\\s*=\\s*((\"[^\"=]+\")|([^,^=,^\\s]+))", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        /// <summary>
        /// Tries to parse an authorization header from an Authorization header style string.
        /// </summary>
        /// <param name="authorizationString">The authorization string.</param>
        /// <param name="header">The authorization header to output.</param>
        /// <returns>true if the authorization was valid; otherwise, false.</returns>
        public static bool TryParse(string authorizationString, out AuthorizationHeader header)
        {
            return TryParse(authorizationString, Encoding.UTF8, out header);
        }

        /// <summary>
        /// Tries to parse an authorization header from an Authorization header style string.
        /// </summary>
        /// <param name="authorizationString">The authorization string.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="header">The authorization header to output.</param>
        /// <returns>true if the authorization was valid; otherwise, false.</returns>
        public static bool TryParse(string authorizationString, Encoding encoding, out AuthorizationHeader header)
        {
            if (String.IsNullOrWhiteSpace(authorizationString))
            {
                header = null;
                return false;
            }

            var items = ParseAuthorizationItems(authorizationString);

            string authenticationType;

            if (!items.TryGetValue(AuthenticationTypeKey, out authenticationType) || String.IsNullOrWhiteSpace(authenticationType))
            {
                header = null;
                return false;
            }

            bool isBasic = String.Equals("Basic", authenticationType, StringComparison.OrdinalIgnoreCase);
            bool isDigest = String.Equals("Digest", authenticationType, StringComparison.OrdinalIgnoreCase);

            string credentials, userName, password;

            if (isBasic && items.TryGetValue(AuthenticationCredentialsKey, out credentials))
            {
                Tuple<string, string> basicCredentials = GetBasicCredentials(encoding, credentials);

                userName = basicCredentials.Item1;
                password = basicCredentials.Item2;
            }
            else if (isDigest)
            {
                if (!items.TryGetValue(UserNameKey, out userName))
                {
                    userName = null;
                }

                password = null;
            }
            else
            {
                userName = ApiUserName;
                password = DecodeApiKey(authorizationString, authenticationType);
            }

            if (String.IsNullOrWhiteSpace(userName))
            {
                header = null;
                return false;
            }

            header = new AuthorizationHeader(authenticationType, userName, GenerateParameters(items))
            {
                Password = password
            };

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

        private static Tuple<string, string> GetBasicCredentials(Encoding encoding, string credentials)
        {
            Tuple<string, string> decodedCredentials = DecodeCredentials(credentials, encoding);

            if (decodedCredentials == null)
            {
                return new Tuple<string, string>(null, null);
            }

            return Tuple.Create(decodedCredentials.Item1, decodedCredentials.Item2);
        }

        private static void SetAuthorizationValue(Match valueMatch, int index, Dictionary<string, string> itemDictionary)
        {
            Group match = valueMatch.Groups[index];

            if (!match.Success || String.IsNullOrWhiteSpace(match.Value))
            {
                return;
            }

            if (match.Value.TrimEnd('=').IndexOf('=') >= 0)
            {
                return;
            }

            switch (index)
            {
                case 1:
                    itemDictionary[AuthenticationTypeKey] = match.Value;
                    break;
                case 2:
                    if (!itemDictionary.ContainsKey(match.Value))
                    {
                        itemDictionary[AuthenticationCredentialsKey] = match.Value;
                    }
                    break;
            }
        }

        private static void SetAuthorizationParameter(Match parameterMatch, Dictionary<string, string> itemDictionary)
        {
            if (!parameterMatch.Success)
            {
                return;
            }

            int separatorIndex = parameterMatch.Value.IndexOf('=');

            if (separatorIndex <= 0 || separatorIndex == parameterMatch.Value.Length - 1)
            {
                return;
            }

            string key = parameterMatch.Value.Substring(0, separatorIndex);
            string value = parameterMatch.Value.Substring(separatorIndex + 1);

            itemDictionary[key.Trim().ToLowerInvariant()] = value.Trim('"', ' ');
        }

        private static IDictionary<string, string> ParseAuthorizationItems(string authorizationString)
        {
            var itemDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Match valueMatches = AuthorizationRegex.Match(authorizationString);

            for (int i = 0; i < valueMatches.Groups.Count; i++)
            {
                SetAuthorizationValue(valueMatches, i, itemDictionary);
            }

            MatchCollection parameterMatches = AuthorizationParameterRegex.Matches(authorizationString);

            foreach (Match match in parameterMatches)
            {
                SetAuthorizationParameter(match, itemDictionary);
            }

            return itemDictionary;
        }

        private static string DecodeApiKey(string authorizationString, string authenticationType)
        {
            return Regex.Replace(authorizationString.Split(',')[0].TrimEnd(),
                                 String.Concat("^", authenticationType, @"\s+"),
                                 String.Empty,
                                 RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
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
