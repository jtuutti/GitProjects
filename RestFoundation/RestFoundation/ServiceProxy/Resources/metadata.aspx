<%@ Page Language="C#" MasterPageFile="proxy.master" %>
<%@ Import Namespace="RestFoundation.ServiceProxy.Helpers" %>

<script runat="server" language="C#">
    public ProxyOperation Operation;

    public void Page_Init(object sender, EventArgs e)
    {
        EnableViewState = false;
        Guid operationId;
        
        if (!Guid.TryParse(Request.QueryString["oid"], out operationId) || operationId == Guid.Empty)
        {
            Response.Redirect("index.aspx");
        }

        Operation = ProxyOperationGenerator.Get(operationId);

        if (Operation == null)
        {
            Response.Redirect("index.aspx");
        }
    }
</script>

<asp:Content runat="server" ContentPlaceHolderID="bodyPlaceholder">

<div id="content">
    <p><em><%: Operation.Description %></em></p>    
    <p>
        <div>
            <strong>URL Template: </strong>
            <span class="uri-template"><%: Operation.UrlTempate %></span>
        </div>
        <div>
            <strong>Sample URL: </strong>
            <span class="uri-template"><%: Operation.GenerateSampleUrl(true) %></span>
        </div>
    </p>
    <p>
        <strong>HTTP Method: </strong>
        <span class="method"><%: Operation.HttpMethod.ToString().ToUpperInvariant() %></span>
        <% if (!String.IsNullOrEmpty(Operation.SupportedHttpMethods)) { %>
            <br /><b>Supported Methods: </b>
            <span class="method"><%: Operation.SupportedHttpMethods %></span>
        <% } %>
    </p>
    <% if (Operation.RouteParameters.Count > 0) { %>
    <table id="queryParameters">
    <tr>
        <th>URL parameter</th>
        <th>Type</th>
        <th>Example</th>
        <th>Allowed values</th>
    </tr>
    <% foreach (var routeParameter in Operation.RouteParameters) { %>
    <tr>
        <td><%: "{" + routeParameter.Name + "}" %></td>
        <td><%: routeParameter.Type %></td>
        <td><%: routeParameter.ExampleValue ?? String.Empty %></td>
        <td><%: routeParameter.AllowedValues ?? String.Empty %></td>
    </tr>
    <% } %>
    </table>
    <% } %>
    <table>
    <tr>
        <th>Message direction</th>
        <th>Format</th>
        <th>Body</th>
    </tr>
    <% if (Operation.HasResource) { %>
    <tr>
        <td>Request</td>
        <td>JSON</td>
        <td><a href="#request-json">Example</a></td>
    </tr>
    <tr>
        <td>Request</td>
        <td>XML</td>
        <td><a href="#request-xml">Example</a></td>
    </tr>
    <% } else { %>
    <tr>
        <td>Request</td>
        <td>N/A</td>
        <td>The request body is empty</td>
    </tr>
    <% } %>
    <% if (Operation.HasResponse) { %>
    <tr>
        <td>Response</td>
        <td>JSON</td>
        <td><a href="#response-json">Example</a></td>
    </tr>
    <tr>
        <td>Response</td>
        <td>XML</td>
        <td><a href="#response-xml">Example</a></td>
    </tr>
    <% } else { %>
    <tr>
        <td>Response</td>
        <td>N/A</td>
        <td>The response body is empty</td>
    </tr>
    <% } %>
    </table>
    <table id="responseCodes">
    <tr>
        <th>Response code</th>
        <th>Condition</th>
    </tr>
    <%  foreach (var statusCode in Operation.StatusCodes) { %>
    <tr<%= statusCode.GetNumericStatusCode() >= 400 ? " class=\"error-response-code\"" : "" %>>
        <td><%: statusCode.GetNumericStatusCode() %></td>
        <td><%: statusCode.Condition %></td>
    </tr>
    <% } %>
    </table>
</div>

</asp:Content>
