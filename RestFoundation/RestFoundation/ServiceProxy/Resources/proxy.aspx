<%@ Page Language="C#" MasterPageFile="proxy.master" %>
<%@ Import Namespace="RestFoundation.ServiceProxy.Helpers" %>

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
            <th>Uri</th>
            <th>Method</th>
            <th>Description</th>
        </tr>
        </thead>
        <tbody>
            <% foreach (EndPoint endPoint in EndPointGenerator.Generate()) { %>
            <tr>
                <td class="operation_uri"><a href="<%: endPoint.RelativeUrl %>" title="Profile or debug the service"><%: endPoint.UrlTempate %></a></td>
                <td><a href="#" title="View detailed service information"><%: endPoint.HttpMethod.ToString().ToUpperInvariant() %></a></td>
                <td><%: endPoint.Description %></td>
            </tr>
            <% } %>
        </tbody>
    </table>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("a").click(function () {
            return $(this).attr("href") != "#";
        });
    });
</script>
</asp:Content>