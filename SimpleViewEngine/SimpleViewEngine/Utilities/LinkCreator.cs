using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Newtonsoft.Json;
using SimpleViewEngine.Properties;

namespace SimpleViewEngine.Utilities
{
    internal static class LinkCreator
    {
        public static string CreatorActionLink(HtmlView view, ViewContext context, Match linkMatch)
        {
            var linkAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            MatchCollection linkAttributeMatches = RegularExpressions.AttributeNameValue.Matches(linkMatch.Value);

            foreach (Match linkAttributeMatch in linkAttributeMatches)
            {
                string attributeName = linkAttributeMatch.Groups[1].Value.Trim();

                if (String.Equals("routeValues", attributeName, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals("htmlAttributes", attributeName, StringComparison.OrdinalIgnoreCase))
                {
                    linkAttributes[attributeName] = DeserializeObjectAsDictionary(linkAttributeMatch.Groups[2].Value);
                }
                else
                {
                    linkAttributes[attributeName] = linkAttributeMatch.Groups[2].Value.Trim();
                }
            }

            object action, controller, routeValues, htmlAttributes;

            linkAttributes.TryGetValue("action", out action);
            linkAttributes.TryGetValue("controller", out controller);

            if (linkAttributes.TryGetValue("routeValues", out routeValues) && routeValues is IDictionary<string, object>)
            {
                routeValues = new RouteValueDictionary((IDictionary<string, object>) routeValues);
            }
            else
            {
                routeValues = null;
            }

            if (!linkAttributes.TryGetValue("htmlAttributes", out htmlAttributes) || !(htmlAttributes is IDictionary<string, object>))
            {
                htmlAttributes = null;
            }

            string value = linkMatch.Groups[2].Value;

            if (String.IsNullOrEmpty(value))
            {
                value = "action link";
            }

            var helper = new HtmlHelper(context, view);
            return helper.ActionLink(value,
                                     action as string,
                                     controller as string,
                                     routeValues as RouteValueDictionary,
                                     htmlAttributes as IDictionary<string, object>)
                         .ToHtmlString();
        }

        public static string CreatorRouteLink(HtmlView view, ViewContext context, Match linkMatch)
        {
            var linkAttributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            MatchCollection linkAttributeMatches = RegularExpressions.AttributeNameValue.Matches(linkMatch.Value);

            foreach (Match linkAttributeMatch in linkAttributeMatches)
            {
                string attributeName = linkAttributeMatch.Groups[1].Value.Trim();

                if (String.Equals("routeValues", attributeName, StringComparison.OrdinalIgnoreCase) ||
                    String.Equals("htmlAttributes", attributeName, StringComparison.OrdinalIgnoreCase))
                {
                    linkAttributes[attributeName] = DeserializeObjectAsDictionary(linkAttributeMatch.Groups[2].Value);
                }
                else
                {
                    linkAttributes[attributeName] = linkAttributeMatch.Groups[2].Value.Trim();
                }
            }

            object route, routeValues, htmlAttributes;

            linkAttributes.TryGetValue("route", out route);

            if (linkAttributes.TryGetValue("routeValues", out routeValues) && routeValues is IDictionary<string, object>)
            {
                routeValues = new RouteValueDictionary((IDictionary<string, object>) routeValues);
            }
            else
            {
                routeValues = null;
            }

            if (!linkAttributes.TryGetValue("htmlAttributes", out htmlAttributes) || !(htmlAttributes is IDictionary<string, object>))
            {
                htmlAttributes = null;
            }

            string value = linkMatch.Groups[2].Value;

            if (String.IsNullOrEmpty(value))
            {
                value = "route link";
            }

            var helper = new HtmlHelper(context, view);
            return helper.RouteLink(value,
                                    route as string,
                                    routeValues as RouteValueDictionary,
                                    htmlAttributes as IDictionary<string, object>)
                         .ToHtmlString();
        }

        private static IDictionary<string, object> DeserializeObjectAsDictionary(string serializedObject)
        {
            try
            {
                return JsonConvert.DeserializeObject<IDictionary<string, object>>(serializedObject);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Concat(Resources.InvalidAttribute, ": ", serializedObject), ex);
            }
        }
    }
}
