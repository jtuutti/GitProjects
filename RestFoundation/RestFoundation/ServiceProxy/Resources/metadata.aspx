<%@ Page Language="C#" MasterPageFile="help.master" ClientIDMode="Static" EnableViewStateMac="false" EnableEventValidation="false" %>
<%@ Import Namespace="System.Xml.Serialization" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>

<script runat="server" language="C#">
    private ProxyOperation operation;
    private string requestJsonExample, requestXmlExample, responseJsonExample, responseXmlExample, authentication;
    private IList<string> requestXmlSchemas, responseXmlSchemas;
    
    protected void Page_Init(object sender, EventArgs e)
    {
        EnableViewState = false;

        int operationId;
        
        if (!Int32.TryParse(Request.QueryString["oid"], out operationId) || operationId == 0)
        {
            Response.Redirect("index");
            return;
        }

        operation = ProxyOperationGenerator.Get(operationId);

        if (operation == null)
        {
            Response.Redirect("index?expired=1");
            return;
        }

        if (operation.HasResource && operation.RequestResourceExample != null)
        {
            object requestObj;

            try
            {
                requestObj = operation.RequestResourceExample.Instance;
            }
            catch (Exception)
            {
                requestObj = null;
            }

            if (requestObj != null)
            {
                if (!operation.DoesNotSupportJson)
                {
                    try
                    {
                        requestJsonExample = ProxyJsonConvert.SerializeObject(requestObj, true, false);
                    }
                    catch (Exception)
                    {
                        requestJsonExample = null;
                    }
                }

                if (!operation.DoesNotSupportXml)
                {
                    try
                    {
                        requestXmlExample = ProxyXmlConvert.SerializeObject(requestObj, true);
                    }
                    catch (Exception)
                    {
                        requestXmlExample = null;
                    }
                }
            }
                
            if (!operation.DoesNotSupportXml && requestXmlExample != null)
            {
                XmlSchemas schemas = operation.RequestResourceExample.XmlSchemas;

                if (schemas != null && schemas.Count > 0)
                {
                    requestXmlSchemas = schemas.Serialize();
                }
            }
        }

        if (operation.HasResponse && operation.ResponseResourceExample != null)
        {
            object responseObj;

            try
            {
                responseObj = operation.ResponseResourceExample.Instance;
            }
            catch (Exception)
            {
                responseObj = null;
            }

            if (responseObj != null)
            {
                if (!operation.DoesNotSupportJson)
                {
                    try
                    {
                        responseJsonExample = ProxyJsonConvert.SerializeObject(responseObj, true, true);
                    }
                    catch (Exception)
                    {
                        responseJsonExample = null;
                    }
                }

                if (!operation.DoesNotSupportXml)
                {
                    try
                    {
                        responseXmlExample = ProxyXmlConvert.SerializeObject(responseObj, true);
                    }
                    catch (Exception)
                    {
                        responseXmlExample = null;
                    }
                }
            }

            if (!operation.DoesNotSupportXml && responseXmlExample != null)
            {
                XmlSchemas schemas = operation.ResponseResourceExample.XmlSchemas;

                if (schemas != null && schemas.Count > 0)
                {
                    responseXmlSchemas = schemas.Serialize();
                }
            }
        }
    }
</script>

<asp:Content runat="server" ContentPlaceHolderID="bodyPlaceholder">
    <p><em><%: operation.Description %></em></p>    
    <div>
        <div>
            <span class="strong">URL Template: </span>
            <span class="uri-template"><%: operation.UrlTempate %></span>
        </div>
        <div>
            <span class="strong">Sample URL: </span>
            <span class="uri-template"><%: operation.GenerateSampleUrlParts().Item1 + operation.GenerateSampleUrlParts().Item2 %></span>
        </div>
    </div>
    <% if (operation.HttpsPort > 0) { %>
    <p>
        <span class="strong">Requires HTTPS</span>
    </p>
    <% } %>
    <% if (operation.Credentials != null) { %>
    <p>
        <span class="strong">Authentication: </span><span><%: operation.Credentials.Type.ToString() %></span>
    </p>
    <% } %>
    <p>
        <span class="strong">HTTP Method: </span>
        <span class="method"><%: operation.HttpMethod.ToString().ToUpperInvariant() %></span>
        <% if (!String.IsNullOrEmpty(operation.SupportedHttpMethods)) { %>
            <br /><span class="strong">Supported Methods: </span>
            <span class="method"><%: operation.SupportedHttpMethods %></span>
        <% } %>
    </p>
    <% if (operation.SupportsOdata) { %>
    <p>
        This operation supports <a href="http://www.odata.org" target="_blank" title="OData">OData</a> URL conventions.
    </p>
    <% } %>
    <% if (operation.RouteParameters.Count > 0) { %>
    <table class="parameters">
    <tr>
        <th>URL parameter</th>
        <th>Query parameter?</th>
        <th>Type</th>
        <th>Optional?</th>
        <th>Constraint</th>
        <th>Allowed values</th>
        <th>Example</th>
    </tr>
    <% foreach (var routeParameter in operation.RouteParameters) { %>
    <tr>
        <td><%: "{" + routeParameter.Name + "}" %></td>
        <td><%: routeParameter.IsRouteParameter ? "N" : "Y" %></td>
        <td><%: routeParameter.GetTypeDescription() %></td>
        <td><%: routeParameter.IsOptionalParameter ? "Y" : "N" %></td>
        <td><%: routeParameter.RegexConstraint ?? String.Empty %></td>
        <td><%: routeParameter.AllowedValues ?? String.Empty %></td>
        <td><%: routeParameter.ExampleValue ?? String.Empty %></td>
    </tr>
    <% } %>
    </table>
    <% } %>
    <% if (operation.AdditionalHeaders.Count > 0) { %>
    <%
        var authorizationHeader = operation.AdditionalHeaders.FirstOrDefault(h => String.Equals("Authorization", h.Name, StringComparison.OrdinalIgnoreCase));

        if (authorizationHeader != null) {
            if (authorizationHeader.Value.StartsWith("Basic", StringComparison.OrdinalIgnoreCase)) {
                authentication = "basic";
            }
            else if (authorizationHeader.Value.StartsWith("Digest", StringComparison.OrdinalIgnoreCase)) {
                authentication = "digest";
            }
        }
    %>
    <% if (!String.IsNullOrWhiteSpace(authentication)) { %>
        <p><span class="strong">This operation requires <%: authentication %> authentication.</span></p>
    <% } %>
    <table class="parameters">
    <tr>
        <th>Header Name</th>
        <th>Example</th>
    </tr>
    <% foreach (var header in operation.AdditionalHeaders) { %>
    <tr>
        <td><%: header.Name %></td>
        <td><%: header.Value %></td>
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
    <% if (requestJsonExample != null) { %>
    <tr>
        <td>Request</td>
        <td>JSON</td>
        <td><a href="#request-json">Example</a></td>
    </tr>
    <% } %>
    <% if (requestXmlExample != null) { %>
    <tr>
        <td>Request</td>
        <td>XML</td>
        <td>
            <a href="#request-xml">Example</a><% if (requestXmlSchemas != null && requestXmlSchemas.Count > 0) { %><span>, </span><a href="#request-schema">Schema</a><% } %>
        </td>
    </tr>
    <% } %>
    <% if (requestJsonExample == null && requestXmlExample == null) { %>
    <tr>
        <td>Request</td>
        <td>N/A</td>
        <td>The request body is empty</td>
    </tr>
    <% } %>
    <% if (responseJsonExample != null) { %>
    <tr>
        <td>Response</td>
        <td>JSON</td>
        <td><a href="#response-json">Example</a></td>
    </tr>
    <% } %>
    <% if (responseXmlExample != null) { %>
    <tr>
        <td>Response</td>
        <td>XML</td>
        <td>
            <a href="#response-xml">Example</a><% if (responseXmlSchemas != null && responseXmlSchemas.Count > 0) { %><span>, </span><a href="#response-schema">Schema</a><% } %>
        </td>
    </tr>
    <% } %>
    <% if (responseJsonExample == null && responseXmlExample == null) { %>
    <tr>
        <td>Response</td>
        <td>N/A</td>
        <td>The response body is empty</td>
    </tr>
    <% } %>
    </table>
    <table id="ResponseCodes">
    <tr>
        <th>Response code</th>
        <th>Condition</th>
    </tr>
    <%  foreach (var statusCode in operation.StatusCodes) { %>
    <tr<%= statusCode.GetNumericStatusCode() >= 400 ? " class=\"error-response-code\"" : "" %>>
        <td><%: statusCode.GetNumericStatusCode() %></td>
        <td><%: statusCode.Condition %></td>
    </tr>
    <% } %>
    </table>
    <% if (operation.RequestResourceExample != null && operation.RequestResourceExample.Instance != null) { %>
    <% if (!String.IsNullOrEmpty(requestJsonExample)) { %>
    <div class="schema-section">
        <a name="request-json"></a>The following is an example JSON serialized request:
        <pre id="JsonRequest"><%: requestJsonExample %></pre>
        <a id="ViewJsonRequest" href="#">View Request</a>
    </div>
    <% } %>
    <% if (!String.IsNullOrEmpty(requestXmlExample)) { %>
    <div class="schema-section">
        <a name="request-xml"></a>The following is an example XML serialized request:
        <pre id="XmlRequest"><%: requestXmlExample %></pre>
        <a id="ViewXmlRequest" href="#">View Request</a>
    </div>
    <% } %>
    <% } %>
    <% if (operation.ResponseResourceExample != null && operation.ResponseResourceExample.Instance != null) { %>
    <% if (!String.IsNullOrEmpty(responseJsonExample)) { %>
    <div class="schema-section">
        <a name="response-json"></a>The following is an example JSON serialized response:
        <pre id="JsonResponse"><%: responseJsonExample %></pre>
        <a id="ViewJsonResponse" href="#">View Response</a>
    </div>
    <% } %>
    <% if (!String.IsNullOrEmpty(responseXmlExample)) { %>
    <div class="schema-section">
        <a name="response-xml"></a>The following is an example XML serialized response:
        <pre id="XmlResponse"><%: responseXmlExample %></pre>
        <a id="ViewXmlResponse" href="#">View Response</a>
    </div>
    <% } %>
    <% } %>
    <% if (requestXmlSchemas != null && requestXmlSchemas.Count > 0) { %>
    <div class="schema-section">
    <a name="request-schema"></a>The following is the request model XSD Schema:         
    <pre id="RequestSchema"><%: requestXmlSchemas[0] %></pre>
    <a id="ViewRequestSchema" href="#">View Schema</a>
    </div>
    <% if (requestXmlSchemas.Count > 1) { %>
    <div class="schema-section">
    <span>Additional request XSD Schemas:</span>
    <% for (int i = 1; i < requestXmlSchemas.Count; i++) { %>
        <pre id="RequestSchema<%: i %>"><%: requestXmlSchemas[i] %></pre>
        <a id="ViewRequestSchema<%: i %>" href="#">View Schema</a>
    <% } %>
    </div>
    <% } %>
    <% } %>
    <% if (responseXmlSchemas != null && responseXmlSchemas.Count > 0) { %>
    <div class="schema-section">
    <a name="response-schema"></a>The following is the response model XSD Schema:
    <pre id="ResponseSchema"><%: responseXmlSchemas[0] %></pre>
    <a id="ViewResponseSchema" href="#">Response Schema</a>
    </div>
    <% if (responseXmlSchemas.Count > 1) { %>
    <div class="schema-section">
    <span>Additional response XSD Schemas:</span>
    <% for (int i = 1; i < responseXmlSchemas.Count; i++) { %>
        <pre id="ResponseSchema<%: i %>"><%: responseXmlSchemas[i] %></pre>
        <a id="ViewResponseSchema<%: i %>" href="#">View Schema</a>
    <% } %>
    </div>
    <% } %>
    <% } %>
    <script type="text/javascript" src="<%= ProxyUrlHelper.GetByRouteName(Response, "ProxyMetadataOp") %>"></script>
</asp:Content>
