<%@ Page Language="C#" MasterPageFile="help.master" ClientIDMode="Static" EnableViewStateMac="false" EnableEventValidation="false" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>

<script runat="server" language="C#">
    public void Page_Init(object sender, EventArgs e)
    {
        EnableViewState = false;
    }
</script>

<asp:Content runat="server" ContentPlaceHolderID="bodyPlaceholder">
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
            <% foreach (ProxyOperation operation in ProxyOperationGenerator.Generate()) { %>
            <tr>
                <td class="operation_uri"><strong><%: operation.UrlTempate %></strong></td>
                <td><strong><%: operation.HttpMethod.ToString().ToUpperInvariant() %></strong></td>
                <td><%: operation.Description %></td>
                <td class="centered"><a href="<%: operation.MetadataUrl %>" title="View detailed service information">View</a></td>
                <td class="centered">
                    <a href="<%: operation.ProxyUrl + "&ct=json" %>" title="Profile or debug the service">JSON</a>&nbsp;|&nbsp;<a href="<%: operation.ProxyUrl + "&ct=xml" %>" title="Profile or debug the service">XML</a>
                </td>
            </tr>
            <% } %>
        </tbody>
    </table>
</asp:Content>
