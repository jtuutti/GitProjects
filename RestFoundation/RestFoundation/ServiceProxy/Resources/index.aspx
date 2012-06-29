<%@ Page Language="C#" MasterPageFile="proxy.master" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>

<script runat="server" language="C#">
    public void Page_Init(object sender, EventArgs e)
    {
        EnableViewState = false;
    }
</script>

<asp:Content runat="server" ContentPlaceHolderID="bodyPlaceholder">
<div id="content">
    <p><em>This page describes the service operations at this endpoint.</em></p>
    <table>
        <thead>
        <tr>
            <th>Relative Url</th>
            <th>Method</th>
            <th>Description</th>
            <th>&nbsp;</th>
            <th>&nbsp;</th>
        </tr>
        </thead>
        <tbody>
            <% foreach (ProxyOperation operation in ProxyOperationGenerator.Generate()) { %>
            <tr>
                <td class="operation_uri"><strong><%: operation.UrlTempate %></strong></td>
                <td><strong><%: operation.HttpMethod.ToString().ToUpperInvariant() %></strong></td>
                <td><%: operation.Description %></td>
                <td><a href="<%: operation.MetadataUrl %>" title="View detailed service information">Metadata</a></td>
                <td><a href="<%: operation.ProxyUrl %>" title="Profile or debug the service">Proxy</a></td>
            </tr>
            <% } %>
        </tbody>
    </table>
</div>
</asp:Content>
