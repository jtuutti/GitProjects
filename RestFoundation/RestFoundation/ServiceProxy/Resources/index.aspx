<%@ Page Language="C#" MasterPageFile="help.master" ClientIDMode="Static" EnableViewStateMac="false" EnableEventValidation="false" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>

<script runat="server" language="C#">
    private List<ProxyOperation> operations;
    private bool hasIPFilteredOperations;

    protected void Page_Init(object sender, EventArgs e)
    {
        EnableViewState = false;

        operations = ProxyOperationGenerator.GetAll().ToList();

        string previousUrlTemplate = null;
        int originalIndex = -1;
        
        for (int i = 0; i < operations.Count; i++)
        {
            if (originalIndex < 0 && i > 0)
            {
                previousUrlTemplate = operations[i - 1].UrlTempate;
            }

            if (previousUrlTemplate == operations[i].UrlTempate)
            {
                if (originalIndex < 0)
                {
                    originalIndex = i - 1;
                }

                operations[i].UrlTempate = String.Empty;
                operations[originalIndex].RepeatedTemplateCount++;
            }
            else
            {
                originalIndex = -1;
            }
        }

        hasIPFilteredOperations = operations.Any(o => o.IsIPFiltered);
    }
</script>

<asp:Content runat="server" ContentPlaceHolderID="bodyPlaceholder">
    <% if (Request.QueryString["expired"] == "1") { %>
    <p class="field-validation-error">
        <em>Session Expired</em>
    </p>
    <% } %>
    <p><em>This page describes the service operations at this endpoint.</em></p>
    <table>
        <thead>
        <tr>
            <th>Relative URL</th>
            <th>Method</th>
            <th>Description</th>
            <th>Details</th>
            <th>Proxy</th>
        </tr>
        </thead>
        <tbody>
            <% foreach (ProxyOperation operation in operations) { %>
            <tr>
                <% if (!String.IsNullOrEmpty(operation.UrlTempate)) { %>
                    <td class="<%: operation.IsIPFiltered ? "operation-uri ip-filtered" : "operation-uri" %>" rowspan="<%: operation.RepeatedTemplateCount + 1 %>">
                        <span class="strong"><%: operation.UrlTempate %></span>
                    </td>
                <% } %>
                <td><span class="strong"><%: operation.HttpMethod.ToString().ToUpperInvariant() %></span></td>
                <td><%: operation.Description %></td>
                <td class="centered"><a href="<%: operation.MetadataUrl %>" class="operation-link" title="View detailed service information">View</a></td>
                <td class="centered">
                    <% if (!operation.DoesNotSupportJson) { %>
                        <a href="<%: operation.ProxyUrl + "&ct=json" %>" class="operation-link" title="Profile or debug the service">JSON</a>
                    <% } %>
                    <% if (!operation.DoesNotSupportXml) { %>
                        <% if (!operation.DoesNotSupportJson) { %>|<% } %>
                        <a href="<%: operation.ProxyUrl + "&ct=xml" %>" class="operation-link" title="Profile or debug the service">XML</a>
                    <% } %>
                </td>
            </tr>
            <% } %>
        </tbody>
        <% if (hasIPFilteredOperations) { %>
        <tfoot>
        <tr>
            <td colspan="5">
                <span class="ip-filtered">Red URL</span> = an IP filtered operation
            </td>
        </tr>
        </tfoot>
        <% } %>
    </table>
    <script type="text/javascript" src="<%= ProxyUrlHelper.GetByRouteName(Response, "ProxyIndexOp") %>"></script>
</asp:Content>
