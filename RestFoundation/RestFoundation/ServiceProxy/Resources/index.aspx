<%@ Page Language="C#" MasterPageFile="help.master" ClientIDMode="Static" EnableViewStateMac="false" EnableEventValidation="false" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>

<script runat="server" language="C#">
    private IEnumerable<ProxyOperation> operations;
    private bool hasIPFilteredOperations;    

    public void Page_Init(object sender, EventArgs e)
    {
        EnableViewState = false;

        operations = ProxyOperationGenerator.Generate();
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
            <th>Relative Url</th>
            <th>Method</th>
            <th>Description</th>
            <th>Metadata</th>
            <th>Proxy</th>
        </tr>
        </thead>
        <tbody>
            <% foreach (ProxyOperation operation in operations) { %>
            <tr>
                <td class="<%: operation.IsIPFiltered ? "operation_uri ip-filtered" : "operation_uri" %>"><strong><%: operation.UrlTempate %></strong></td>
                <td><strong><%: operation.HttpMethod.ToString().ToUpperInvariant() %></strong></td>
                <td><%: operation.Description %></td>
                <td class="centered"><a href="<%: operation.MetadataUrl %>" title="View detailed service information">View</a></td>
                <td class="centered">
                    <a href="<%: operation.ProxyUrl + "&ct=json" %>" title="Profile or debug the service">JSON</a>&nbsp;|&nbsp;<a href="<%: operation.ProxyUrl + "&ct=xml" %>" title="Profile or debug the service">XML</a>
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
</asp:Content>
