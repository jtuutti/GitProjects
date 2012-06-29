<%@ Page Language="C#" MasterPageFile="proxy.master" %>
<%@ Import Namespace="System.Xml.Serialization" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="RestFoundation.Runtime" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>
<%@ Import Namespace="RestFoundation.ServiceProxy.Helpers" %>

<script runat="server" language="C#">
    public ProxyOperation Operation;
    public string RequestJsonExample, RequestXmlExample, ResponseJsonExample, ResponseXmlExample;
    public IList<string> XmlSchemas;
    
    public void Page_Init(object sender, EventArgs e)
    {
        EnableViewState = false;
        Guid operationId;
        
        if (!Guid.TryParse(Request.QueryString["oid"], out operationId) || operationId == Guid.Empty)
        {
            Response.Redirect("index.aspx");
            return;
        }

        Operation = ProxyOperationGenerator.Get(operationId);

        if (Operation == null)
        {
            Response.Redirect("index.aspx");
            return;
        }

        if (Operation.HasResource && Operation.RequestExampleType != null)
        {
            IResourceExample requestExample;

            try
            {
                requestExample = Activator.CreateInstance(Operation.RequestExampleType) as IResourceExample;
            }
            catch (Exception)
            {
                requestExample = null;
            }

            if (requestExample != null)
            {
                object requestObj;

                try
                {
                    requestObj = requestExample.Create();
                }
                catch (Exception)
                {
                    requestObj = null;
                }

                if (requestObj != null)
                {
                    try
                    {
                        RequestJsonExample = JsonConvert.SerializeObject(requestObj, Formatting.Indented);
                    }
                    catch (Exception)
                    {
                        RequestJsonExample = null;
                    }

                    try
                    {
                        RequestXmlExample = XmlConvert.SerializeObject(requestObj, System.Xml.Formatting.Indented);
                    }
                    catch (Exception)
                    {
                        RequestXmlExample = null;
                    }
                }
            }
        }

        if (Operation.HasResponse && Operation.ResponseExampleType != null)
        {
            IResourceExample responseExample;

            try
            {
                responseExample = Activator.CreateInstance(Operation.ResponseExampleType) as IResourceExample;
            }
            catch (Exception)
            {
                responseExample = null;
            }

            if (responseExample != null)
            {
                object responseObj;

                try
                {
                    responseObj = responseExample.Create();
                }
                catch (Exception)
                {
                    responseObj = null;
                }

                if (responseObj != null)
                {
                    try
                    {
                        ResponseJsonExample = JsonConvert.SerializeObject(responseObj, Formatting.Indented);
                    }
                    catch (Exception)
                    {
                        ResponseJsonExample = null;
                    }

                    try
                    {
                        ResponseXmlExample = XmlConvert.SerializeObject(responseObj, System.Xml.Formatting.Indented);
                    }
                    catch (Exception)
                    {
                        ResponseXmlExample = null;
                    }
                }

                if (ResponseXmlExample != null)
                {
                    XmlSchemas schemas = responseExample.GetSchemas();

                    if (schemas != null && schemas.Count > 0)
                    {
                        XmlSchemas = schemas.Serialize();
                    }
                }
            }
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
        <th>Constraint</th>
        <th>Allowed values</th>
        <th>Example</th>
    </tr>
    <% foreach (var routeParameter in Operation.RouteParameters) { %>
    <tr>
        <td><%: "{" + routeParameter.Name + "}" %></td>
        <td><%: routeParameter.Type %></td>
        <td><%: routeParameter.Constraint ?? String.Empty %></td>
        <td><%: routeParameter.AllowedValues ?? String.Empty %></td>
        <td><%: routeParameter.ExampleValue ?? String.Empty %></td>
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
    <% if (RequestJsonExample != null) { %>
    <tr>
        <td>Request</td>
        <td>JSON</td>
        <td><a href="#request-json">Example</a></td>
    </tr>
    <% } %>
    <% if (RequestXmlExample != null) { %>
    <tr>
        <td>Request</td>
        <td>XML</td>
        <td><a href="#request-xml">Example</a></td>
    </tr>
    <% } %>
    <% if (RequestJsonExample == null && RequestXmlExample == null) { %>
    <tr>
        <td>Request</td>
        <td>N/A</td>
        <td>The request body is empty</td>
    </tr>
    <% } %>
    <% if (ResponseJsonExample != null) { %>
    <tr>
        <td>Response</td>
        <td>JSON</td>
        <td><a href="#response-json">Example</a></td>
    </tr>
    <% } %>
    <% if (ResponseXmlExample != null) { %>
    <tr>
        <td>Response</td>
        <td>XML</td>
        <td><a href="#response-xml">Example</a></td>
    </tr>
    <% } %>
    <% if (ResponseJsonExample == null && ResponseXmlExample == null) { %>
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
    <% if (Operation.ResponseExampleType != null) { %>
    <% if (!String.IsNullOrEmpty(RequestJsonExample)) { %>
    <div class="schemaSection">
        <a name="request-json">The following is an example JSON serialized request:</a>           
        <pre><%: RequestJsonExample %></pre>
    </div>
    <% } %>
    <% if (!String.IsNullOrEmpty(RequestXmlExample)) { %>
    <div class="schemaSection">
        <a name="request-xml">The following is an example XML serialized request:</a>           
        <pre><%: RequestXmlExample %></pre>
    </div>
    <% } %>
    <% } %>
    <% if (Operation.ResponseExampleType != null) { %>
    <% if (!String.IsNullOrEmpty(ResponseJsonExample)) { %>
    <div class="schemaSection">
        <a name="response-json">The following is an example JSON serialized response:</a>
        <pre><%: ResponseJsonExample %></pre>
    </div>
    <% } %>
    <% if (!String.IsNullOrEmpty(ResponseXmlExample)) { %>
    <div class="schemaSection">
        <a name="response-xml">The following is an example XML serialized response:</a>
        <pre><%: ResponseXmlExample %></pre>
    </div>
    <% } %>
    <% } %>
    <% if (XmlSchemas != null && XmlSchemas.Count > 0) { %>
    <div class="schemaSection">
    <a name="response-schema">The following is the response model XSD Schema:</a>           
    <pre><%: XmlSchemas[0] %></pre>
    </div>
    <% if (XmlSchemas.Count > 1) { %>
    <div class="schemaSection">
    <span>Additional response XSD Schemas:</span>
    <% for (int i = 1; i < XmlSchemas.Count; i++) { %>
        <pre><%: XmlSchemas[i] %></pre>
    <% } %>
    </div>
    <% } %>
    <% } %>
</div>

</asp:Content>
