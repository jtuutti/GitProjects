<%@ Page Language="C#" MasterPageFile="help.master" ClientIDMode="Static" EnableViewStateMac="false" EnableEventValidation="false" %>
<%@ Import Namespace="System.Xml.Serialization" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="RestFoundation.Runtime" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>

<script runat="server" language="C#">
    private ProxyOperation operation;
    private string requestJsonExample, requestXmlExample, responseJsonExample, responseXmlExample;
    private IList<string> xmlSchemas;
    
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
                        xmlSchemas = schemas.Serialize();
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
    <% if (operation.RouteParameters.Count > 0) { %>
    <table id="queryParameters">
    <tr>
        <th>URL parameter</th>
        <th>Type</th>
        <th>Constraint</th>
        <th>Allowed values</th>
        <th>Example</th>
    </tr>
    <% foreach (var routeParameter in operation.RouteParameters) { %>
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
        <td><a href="#request-xml">Example</a></td>
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
        <td><a href="#response-xml">Example</a></td>
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
        <a name="request-json">The following is an example JSON serialized request:</a>           
        <pre><%: requestJsonExample %></pre>
    </div>
    <% } %>
    <% if (!String.IsNullOrEmpty(requestXmlExample)) { %>
    <div class="schemaSection">
        <a name="request-xml">The following is an example XML serialized request:</a>           
        <pre><%: requestXmlExample %></pre>
    </div>
    <% } %>
    <% } %>
    <% if (operation.ResponseExampleType != null) { %>
    <% if (!String.IsNullOrEmpty(responseJsonExample)) { %>
    <div class="schemaSection">
        <a name="response-json">The following is an example JSON serialized response:</a>
        <pre><%: responseJsonExample %></pre>
    </div>
    <% } %>
    <% if (!String.IsNullOrEmpty(responseXmlExample)) { %>
    <div class="schemaSection">
        <a name="response-xml">The following is an example XML serialized response:</a>
        <pre><%: responseXmlExample %></pre>
    </div>
    <% } %>
    <% } %>
    <% if (xmlSchemas != null && xmlSchemas.Count > 0) { %>
    <div class="schemaSection">
    <a name="response-schema">The following is the response model XSD Schema:</a>           
    <pre><%: xmlSchemas[0] %></pre>
    </div>
    <% if (xmlSchemas.Count > 1) { %>
    <div class="schemaSection">
    <span>Additional response XSD Schemas:</span>
    <% for (int i = 1; i < xmlSchemas.Count; i++) { %>
        <pre><%: xmlSchemas[i] %></pre>
    <% } %>
    </div>
    <% } %>
    <% } %>
</asp:Content>
