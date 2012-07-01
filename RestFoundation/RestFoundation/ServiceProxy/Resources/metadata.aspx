<%@ Page Language="C#" MasterPageFile="help.master" ClientIDMode="Static" EnableViewStateMac="false" EnableEventValidation="false" %>
<%@ Import Namespace="System.Xml.Serialization" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="RestFoundation.Runtime" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>

<script runat="server" language="C#">
    private ProxyOperation operation;
    private string requestJsonExample, requestXmlExample, responseJsonExample, responseXmlExample;
    private IList<string> requestXmlSchemas, responseXmlSchemas;
    
    public void Page_Init(object sender, EventArgs e)
    {
        EnableViewState = false;

        Guid operationId;
        
        if (!Guid.TryParse(Request.QueryString["oid"], out operationId) || operationId == Guid.Empty)
        {
            Response.Redirect("index");
            return;
        }

        operation = ProxyOperationGenerator.Get(operationId);

        if (operation == null)
        {
            Response.Redirect("index");
            return;
        }

        if (operation.HasResource && operation.RequestExampleType != null)
        {
            IResourceExample requestExample;

            try
            {
                requestExample = Activator.CreateInstance(operation.RequestExampleType) as IResourceExample;
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
                        requestJsonExample = JsonConvert.SerializeObject(requestObj, Formatting.Indented);
                    }
                    catch (Exception)
                    {
                        requestJsonExample = null;
                    }

                    try
                    {
                        requestXmlExample = XmlConvert.SerializeObject(requestObj, System.Xml.Formatting.Indented);
                    }
                    catch (Exception)
                    {
                        requestXmlExample = null;
                    }
                }
                
                if (requestXmlExample != null)
                {
                    XmlSchemas schemas = requestExample.GetSchemas();

                    if (schemas != null && schemas.Count > 0)
                    {
                        requestXmlSchemas = schemas.Serialize();
                    }
                }
            }
        }

        if (operation.HasResponse && operation.ResponseExampleType != null)
        {
            IResourceExample responseExample;

            try
            {
                responseExample = Activator.CreateInstance(operation.ResponseExampleType) as IResourceExample;
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
                        responseJsonExample = JsonConvert.SerializeObject(responseObj, Formatting.Indented);
                    }
                    catch (Exception)
                    {
                        responseJsonExample = null;
                    }

                    try
                    {
                        responseXmlExample = XmlConvert.SerializeObject(responseObj, System.Xml.Formatting.Indented);
                    }
                    catch (Exception)
                    {
                        responseXmlExample = null;
                    }
                }

                if (responseXmlExample != null)
                {
                    XmlSchemas schemas = responseExample.GetSchemas();

                    if (schemas != null && schemas.Count > 0)
                    {
                        responseXmlSchemas = schemas.Serialize();
                    }
                }
            }
        }
    }
</script>

<asp:Content runat="server" ContentPlaceHolderID="bodyPlaceholder">
    <p><em><%: operation.Description %></em></p>    
    <p>
        <div>
            <strong>URL Template: </strong>
            <span class="uri-template"><%: operation.UrlTempate %></span>
        </div>
        <div>
            <strong>Sample URL: </strong>
            <span class="uri-template"><%: operation.GenerateSampleUrlParts().Item1 + operation.GenerateSampleUrlParts().Item2 %></span>
        </div>
    </p>
    <p>
        <strong>HTTP Method: </strong>
        <span class="method"><%: operation.HttpMethod.ToString().ToUpperInvariant() %></span>
        <% if (!String.IsNullOrEmpty(operation.SupportedHttpMethods)) { %>
            <br /><b>Supported Methods: </b>
            <span class="method"><%: operation.SupportedHttpMethods %></span>
        <% } %>
    </p>
    <% if (operation.SupportOdata) { %>
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
        <th>Constraint</th>
        <th>Allowed values</th>
        <th>Example</th>
    </tr>
    <% foreach (var routeParameter in operation.RouteParameters) { %>
    <tr>
        <td><%: "{" + routeParameter.Name + "}" %></td>
        <td><%: routeParameter.IsRouteParameter ? "N" : "Y" %></td>
        <td><%: routeParameter.Type %></td>
        <td><%: routeParameter.Constraint ?? String.Empty %></td>
        <td><%: routeParameter.AllowedValues ?? String.Empty %></td>
        <td><%: routeParameter.ExampleValue ?? String.Empty %></td>
    </tr>
    <% } %>
    </table>
    <% } %>
    <% if (operation.AdditionalHeaders.Count > 0) { %>
    <table class="parameters">
    <tr>
        <th>Header Name</th>
        <th>Example</th>
    </tr>
    <% foreach (var header in operation.AdditionalHeaders) { %>
    <tr>
        <td><%: header.Item1 %></td>
        <td><%: header.Item2 %></td>
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
    <table id="responseCodes">
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
    <% if (operation.ResponseExampleType != null) { %>
    <% if (!String.IsNullOrEmpty(requestJsonExample)) { %>
    <div class="schemaSection">
        <a name="request-json"></a>The following is an example JSON serialized request:
        <pre id="JsonRequest"><%: requestJsonExample %></pre>
        <a id="ViewJsonRequest" href="#">View Request</a>
    </div>
    <% } %>
    <% if (!String.IsNullOrEmpty(requestXmlExample)) { %>
    <div class="schemaSection">
        <a name="request-xml"></a>The following is an example XML serialized request:
        <pre id="XmlRequest"><%: requestXmlExample %></pre>
        <a id="ViewXmlRequest" href="#">View Request</a>
    </div>
    <% } %>
    <% } %>
    <% if (operation.ResponseExampleType != null) { %>
    <% if (!String.IsNullOrEmpty(responseJsonExample)) { %>
    <div class="schemaSection">
        <a name="response-json"></a>The following is an example JSON serialized response:
        <pre id="JsonResponse"><%: responseJsonExample %></pre>
        <a id="ViewJsonResponse" href="#">View Response</a>
    </div>
    <% } %>
    <% if (!String.IsNullOrEmpty(responseXmlExample)) { %>
    <div class="schemaSection">
        <a name="response-xml"></a>The following is an example XML serialized response:
        <pre id="XmlResponse"><%: responseXmlExample %></pre>
        <a id="ViewXmlResponse" href="#">View Response</a>
    </div>
    <% } %>
    <% } %>
    <% if (requestXmlSchemas != null && requestXmlSchemas.Count > 0) { %>
    <div class="schemaSection">
    <a name="request-schema"></a>The following is the request model XSD Schema:         
    <pre id="RequestSchema"><%: requestXmlSchemas[0] %></pre>
    <a id="ViewRequestSchema" href="#">View Schema</a>
    </div>
    <% if (requestXmlSchemas.Count > 1) { %>
    <div class="schemaSection">
    <span>Additional request XSD Schemas:</span>
    <% for (int i = 1; i < requestXmlSchemas.Count; i++) { %>
        <pre id="RequestSchema<%: i %>"><%: requestXmlSchemas[i] %></pre>
        <a id="ViewRequestSchema<%: i %>" href="#">View Schema</a>
    <% } %>
    </div>
    <% } %>
    <% } %>
    <% if (responseXmlSchemas != null && responseXmlSchemas.Count > 0) { %>
    <div class="schemaSection">
    <a name="response-schema"></a>The following is the response model XSD Schema:
    <pre id="ResponseSchema"><%: responseXmlSchemas[0] %></pre>
    <a id="ViewResponseSchema" href="#">Response Schema</a>
    </div>
    <% if (responseXmlSchemas.Count > 1) { %>
    <div class="schemaSection">
    <span>Additional response XSD Schemas:</span>
    <% for (int i = 1; i < responseXmlSchemas.Count; i++) { %>
        <pre id="ResponseSchema<%: i %>"><%: responseXmlSchemas[i] %></pre>
        <a id="ViewResponseSchema<%: i %>" href="#">View Schema</a>
    <% } %>
    </div>
    <% } %>
    <% } %>
    <script type="text/javascript">
        $(function(){$("#ViewJsonRequest").click(function(){displayCode($("#JsonRequest").text());return!1});$("#ViewXmlRequest").click(function(){displayCode($("#XmlRequest").text());return!1});$("#ViewJsonResponse").click(function(){displayCode($("#JsonResponse").text());return!1});$("#ViewXmlResponse").click(function(){displayCode($("#XmlResponse").text());return!1});$("#ViewRequestSchema").click(function(){displayCode($("#RequestSchema").text());return!1});$("#ViewResponseSchema").click(function(){displayCode($("#ResponseSchema").text()); return!1});for(var a=1;10>a&&0!=$("#RequestSchema"+a).length;a++)$("#ViewRequestSchema"+a).click(function(){var a=$(this).attr("id").replace(/^View/,"");displayCode($("#"+a).text());return!1});for(a=1;10>a&&0!=$("#ResponseSchema"+a).length;a++)$("#ViewResponseSchema"+a).click(function(){var a=$(this).attr("id").replace(/^View/,"");displayCode($("#"+a).text());return!1})}); function displayCode(a){$("#OutputForm").remove();var b=['<form id="OutputForm" method="POST" action="output" target="_blank">'];b.push('<input type="hidden" name="format" value="text/plain"/>');b.push('<input type="hidden" name="responseText" value="'+encodeURIComponent(a)+'"/>');b.push("</form>");$(b.join("")).appendTo("body")[0].submit()};
    </script>
</asp:Content>
