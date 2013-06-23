<%@ Page Language="C#" MasterPageFile="help.master" ClientIDMode="Static" Async="true" ValidateRequest="false" EnableViewStateMac="false" EnableEventValidation="false" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Net.Security" %>
<%@ Import Namespace="System.Threading.Tasks" %>
<%@ Import Namespace="RestFoundation.Collections.Specialized" %>
<%@ Import Namespace="RestFoundation.Runtime" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>
<%@ Import Namespace="RestFoundation.ServiceProxy.OperationMetadata" %>

<script runat="server" language="C#">
    private const string AcceptHeader = "Accept";
    private const string AcceptCharsetHeader = "Accept-Charset";
    private const string ContentTypeHeader = "Content-Type";
    private const string UserAgentHeader = "User-Agent";
    private const string UserAgent = "Rest Foundation Proxy";

    private ProxyOperation operation;
    private int operationId;
    private string serviceUrl;

    protected void Page_Init(object sender, EventArgs e)
    {
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

        if (!Request.IsLocal || String.Equals("localhost", Request.Url.Host, StringComparison.OrdinalIgnoreCase) ||
            String.Equals("127.0.0.1", Request.Url.Host) || String.Equals("::1", Request.Url.Host))
        {
            ConnectToProxy.Visible = false;
        }

        Tuple<string, string> urlParts = operation.GenerateSampleUrlParts();
        serviceUrl = urlParts.Item1;
        OperationUrl.Value = urlParts.Item2;

        if (operation.Credentials != null)
        {
            if (!String.IsNullOrEmpty(operation.Credentials.DefaultAuthorizationHeader))
            {
                HeaderText.InnerText = String.Concat("Authorization: ",
                                                     operation.Credentials.DefaultAuthorizationHeader.Trim(),
                                                     Environment.NewLine,
                                                     HeaderText.InnerText);
            }
            else if (!String.IsNullOrEmpty(operation.Credentials.DefaultUserName))
            {
                UserName.Value = operation.Credentials.DefaultUserName;
            }
        }

        if (String.IsNullOrWhiteSpace(serviceUrl))
        {
            Response.Redirect("index");
            return;
        }

        string format = Request.QueryString["ct"];

        if (format == null || (!String.Equals("json", format, StringComparison.OrdinalIgnoreCase) &&
                               !String.Equals("xml", format, StringComparison.OrdinalIgnoreCase)))
        {
            Response.Redirect("index");
            return;
        }

        if (!operation.DoesNotSupportJson && !operation.DoesNotSupportXml)
        {
            ResourceFormat.Enabled = true;
            ResourceFormat.SelectedIndexChanged += (o, args) =>
            {
                var resourceFormat = (DropDownList) o;
                Response.Redirect(String.Format(CultureInfo.InvariantCulture, "proxy?oid={0}&ct={1}", operationId, resourceFormat.Text.ToLowerInvariant()));
            };
        }
        else
        {
            ResourceFormat.Enabled = false;
        }

        ResourceFormat.Text = format.ToUpperInvariant();
        HttpMethod.SelectedValue = operation.HttpMethod.ToString().ToUpperInvariant();

        foreach (HeaderMetadata header in operation.AdditionalHeaders)
        {
            HeaderText.Value += String.Concat(header.Name, ": ", header.Value, Environment.NewLine);
        }

        if (operation.RequestResourceExample == null)
        {
            return;
        }

        object requestObj;

        try
        {
            requestObj = operation.RequestResourceExample.Instance;
        }
        catch (Exception)
        {
            requestObj = null;
        }

        if (requestObj == null)
        {
            return;
        }

        if (!operation.HasResource)
        {
            RequestText.Value = requestObj.ToString();
            return;
        }

        if (String.Equals("xml", format, StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RequestText.Value = ProxyXmlConvert.SerializeObject(requestObj, true);
            }
            catch (Exception)
            {
                RequestText.Value = String.Empty;
            }
        }
        else
        {
            try
            {
                RequestText.Value = ProxyJsonConvert.SerializeObject(requestObj, true, false);
            }
            catch (Exception)
            {
                RequestText.Value = String.Empty;
            } 
        }
        
        Server.ScriptTimeout = 125;
    }

    protected async void Page_Load(object sender, EventArgs e)
    {
        ViewResponse.Visible = false;

        if (!IsPostBack || Request.Form.Get("__EVENTTARGET").EndsWith("ResourceFormat"))
        {
            return;
        }

        RemoteCertificateValidationCallback validationCallback = null;

        if (serviceUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            validationCallback = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;
        }

        try
        {
            using (var client = new ProxyWebClient())
            {
                string url = String.Concat(serviceUrl, OperationUrl.Value.Replace("+", "%20"));

                if (operation.Credentials != null && !String.IsNullOrWhiteSpace(UserName.Value) && !String.IsNullOrEmpty(Password.Value))
                {
                    var cachedCredentials = new CredentialCache
                    {
                        { new Uri(serviceUrl), operation.Credentials.Type.ToString(), new NetworkCredential(UserName.Value.Trim(), Password.Value) }
                    };

                    client.Credentials = cachedCredentials;
                }
                
                int proxyPort;

                if (ConnectToProxy.Visible && Int32.TryParse(ProxyPort.Value, out proxyPort))
                {
                    client.Proxy = new WebProxy("127.0.0.1", proxyPort);
                }

                AddHeaders(client);
                SetEncoding(client);

                ProxyResponseData response = await PerformRequest(url, HttpMethod.SelectedValue, client);
                bool hasData = response.Data.Trim().Length > 0;

                if (hasData)
                {
                    ResponseData.Value = response.Data;
                }
                
                if (client.ResponseHeaders != null)
                {
                    ContentType.Value = client.ResponseHeaders.Get("Content-Type");
                }

                if (hasData && !DoNotFormatBody.Checked)
                {
                    response.Data = FormatBody(response.Data, ContentType.Value);
                }

                if (!hasData || DisplayResponseHeaders.Checked)
                {
                    response.Data = GetResponseHeaders(url, client.ResponseHeaders, response, hasData) + response.Data;
                }

                ResponseText.Value = response.Data;

                if (hasData)
                {
                    ViewResponse.Visible = true;
                }
            }
        }
        catch (HttpException ex)
        {
            ResponseText.Value = String.Format(CultureInfo.InvariantCulture, "HTTP/1.1: {0} - {1}", ex.GetHttpCode(), ex.Message);
            RegisterErrorHighlightScript();
        }
        catch (Exception ex)
        {
            ResponseText.Value = String.Format(CultureInfo.InvariantCulture, "HTTP/1.1: 500 - {0}", ex.Message);
            RegisterErrorHighlightScript();
        }
        finally
        {
            if (validationCallback != null)
            {
                ServicePointManager.ServerCertificateValidationCallback = validationCallback;
            }
        }
    }

    private static string GetProtocolVersion(WebResponse response)
    {
        var proxyResponse = response as ProxyWebResponse ?? new ProxyWebResponse(response);

        return proxyResponse.ProtocolVersion.ToString();
    }

    private static string GetStatusCode(WebResponse response)
    {
        var proxyResponse = response as ProxyWebResponse ?? new ProxyWebResponse(response);

        return String.Format(CultureInfo.InvariantCulture, "{0} - {1}", (int) proxyResponse.StatusCode, DecodeHttpStatus(proxyResponse.StatusCode, proxyResponse.StatusDescription));
    }

    private static string DecodeHttpStatus(HttpStatusCode statusCode, string statusDescription)
    {
        if (String.IsNullOrEmpty(statusDescription))
        {
            return Regex.Replace(statusCode.ToString(), "([a-z])([A-Z])", "$1 $2");
        }

        return HttpUtility.HtmlDecode(statusDescription);
    }

    private static string FormatBody(string data, string contentType)
    {
        if (contentType.IndexOf("application/json", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return ResourceOutputFormatter.FormatJson(data);
        }

        if (contentType.IndexOf("application/xml", StringComparison.OrdinalIgnoreCase) >= 0 ||
            contentType.IndexOf("text/xml", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return ResourceOutputFormatter.FormatXml(data);
        }

        return data;
    }

    private static string GetResponseHeaders(String url, NameValueCollection headerCollection, ProxyResponseData response, bool hasOutput)
    {
        var headerString = new StringBuilder();

        headerString.AppendLine("RESPONSE");
        CreateOutputSeparator(headerString);

        headerString.Append("URL: ").AppendLine(url.Replace("%20", "+"));
        headerString.Append("HTTP/").Append(response.ProtocolVersion);

        if (!String.IsNullOrEmpty(response.Code))
        {
            headerString.Append(": ").AppendLine(response.Code);
        }
        else
        {
            headerString.AppendLine();
        }

        headerString.AppendFormat("Duration: {0} ms", Convert.ToInt32(response.Duration.TotalMilliseconds));

        if (headerCollection == null || headerCollection.Count == 0)
        {
            return String.Empty;
        }

        headerString.AppendLine().AppendLine().AppendLine("HEADERS");
        CreateOutputSeparator(headerString);

        var headerNames = headerCollection.AllKeys.OrderBy(k => k).ToList();

        for (int i = 0; i < headerNames.Count; i++)
        {
            string headerName = headerNames[i];
            if (String.IsNullOrWhiteSpace(headerName)) continue;

            string headerValue = headerCollection[headerName];

            if (String.IsNullOrWhiteSpace(headerName) || headerName.Equals("set-cookie", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            headerString.AppendFormat("{0}: {1}", headerName, headerValue).AppendLine();
        }

        headerString.AppendLine();

        if (hasOutput)
        {
            headerString.AppendLine("BODY");
            CreateOutputSeparator(headerString);
        }

        return headerString.ToString();
    }

    private static void CreateOutputSeparator(StringBuilder headerString)
    {
        for (int i = 0; i < 60; i++)
        {
            headerString.Append('-');
        }

        headerString.AppendLine();
    }

    private static void SetEncoding(ProxyWebClient client)
    {
        string charset = new AcceptValueCollection(client.Headers.Get(AcceptCharsetHeader)).GetPreferredName();

        if (String.IsNullOrWhiteSpace(charset) || "utf-8".Equals(charset, StringComparison.OrdinalIgnoreCase) || charset.IndexOf('*') >= 0)
        {
            client.Encoding = Encoding.UTF8;
        }
        else
        {
            try
            {
                client.Encoding = Encoding.GetEncoding(charset);
            }
            catch (Exception)
            {
                throw new HttpException((int) HttpStatusCode.NotAcceptable, "The accepted charset provided is not supported");
            }
        }
    }

    private void AddHeaders(ProxyWebClient client)
    {
        if (!String.IsNullOrWhiteSpace(HeaderText.Value))
        {
            foreach (var header in HeaderText.Value.Replace("\r", String.Empty).Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    int separatorIndex = header.IndexOf(":", StringComparison.Ordinal);
                    if (separatorIndex <= 0 || header.Length <= separatorIndex + 1) continue;

                    string headerName = header.Substring(0, separatorIndex).Trim();
                    string headerValue = header.Substring(separatorIndex + 1).Trim();

                    if (!String.IsNullOrWhiteSpace(headerName) && !String.IsNullOrWhiteSpace(headerValue))
                    {
                        client.Headers.Add(headerName, headerValue);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        client.Headers[UserAgentHeader] = UserAgent;

        SetAcceptHeader(client);
        SetContentTypeHeader(client);
    }

    private void SetAcceptHeader(ProxyWebClient client)
    {
        if (!String.IsNullOrWhiteSpace(client.Headers[AcceptHeader]))
        {
            return;
        }

        if ("XML".Equals(ResourceFormat.Text))
        {
            client.Headers[AcceptHeader] = "application/xml";
        }
        else if ("JSON".Equals(ResourceFormat.Text))
        {
            client.Headers[AcceptHeader] = "application/json";
        }

        if (String.IsNullOrWhiteSpace(client.Headers[AcceptCharsetHeader]))
        {
            client.Headers[AcceptCharsetHeader] = "utf-8";
        }
    }

    private void SetContentTypeHeader(ProxyWebClient client)
    {
        if (!String.IsNullOrWhiteSpace(client.Headers[ContentTypeHeader]))
        {
            return;
        }

        if (!"POST".Equals(HttpMethod.SelectedValue) && !"PUT".Equals(HttpMethod.SelectedValue) && !"PATCH".Equals(HttpMethod.SelectedValue))
        {
            return;
        }

        var charset = new AcceptValueCollection(client.Headers[AcceptCharsetHeader]).GetPreferredName();

        if (String.IsNullOrWhiteSpace(charset) || charset.IndexOf('*') >= 0)
        {
            charset = "utf-8";
        }

        if ("XML".Equals(ResourceFormat.Text))
        {
            client.Headers[ContentTypeHeader] = "application/xml; charset=" + charset;
        }
        else if ("JSON".Equals(ResourceFormat.Text))
        {
            client.Headers[ContentTypeHeader] = "application/json; charset=" + charset;
        }
    }

    private string GetExceptionResponseData(WebException ex, out NameValueCollection headers)
    {
        headers = new NameValueCollection();

        if (ex.Response == null)
        {
            return ex.Message;
        }

        var responseStream = ex.Response.GetResponseStream();

        if (responseStream == null)
        {
            return ex.Message;
        }

        if (ex.Response.Headers != null)
        {
            headers.Add(ex.Response.Headers);
        }

        if (String.IsNullOrEmpty(ContentType.Value))
        {
            ContentType.Value = ex.Response.ContentType;
        }

        try
        {
            string data = new StreamReader(responseStream).ReadToEnd();

            if (!DoNotFormatBody.Checked)
            {
                return FormatBody(data, ContentType.Value);
            }

            return data;
        }
        catch (Exception)
        {
            return ex.Message;
        }
    }

    private async Task<ProxyResponseData> PerformRequest(string url, string httpMethod, ProxyWebClient client)
    {
        var response = new ProxyResponseData();
        var timer = Stopwatch.StartNew();

        try
        {
            Task<string> responseDataTask;
        
            switch (httpMethod.ToUpperInvariant())
            {
                case "DELETE":
                    responseDataTask = client.UploadStringTaskAsync(url, HttpMethod.SelectedValue, String.Empty);
                    break;
                case "GET":
                    responseDataTask = client.DownloadStringTaskAsync(url);
                    break;
                case "HEAD":
                    client.HeadOnly = true;
                    responseDataTask = client.DownloadStringTaskAsync(url);
                    break;
                case "OPTIONS":
                    client.Options = true;
                    responseDataTask = client.DownloadStringTaskAsync(url);
                    break;
                case "PATCH":
                case "POST":
                case "PUT":
                    responseDataTask = client.UploadStringTaskAsync(url, HttpMethod.SelectedValue, RequestText.Value);
                    break;
                default:
                    throw new HttpException((int) HttpStatusCode.MethodNotAllowed, "Invalid HTTP method provided");
            }

            response.Data = await responseDataTask;
            response.Code = GetStatusCode(client.WebResponse);
            response.ProtocolVersion = GetProtocolVersion(client.WebResponse);
        }
        catch (WebException ex)
        {
            NameValueCollection headers;
            client.ResponseHeaders.Clear();

            response.Data = GetExceptionResponseData(ex, out headers);

            if (ex.Response != null)
            {
                client.ResponseHeaders.Add(headers);

                response.Code = GetStatusCode(ex.Response);
                response.ProtocolVersion = GetProtocolVersion(ex.Response);
            }
            else
            {
                client.ResponseHeaders.Add(Context.Response.Headers);

                response.Code = "500 - Internal Server Error";
                response.ProtocolVersion = Context.Request.ServerVariables["SERVER_PROTOCOL"];

                int indexOfSeparator = response.ProtocolVersion.IndexOf('/');
                
                if (indexOfSeparator > 0 && indexOfSeparator < response.ProtocolVersion.Length - 1)
                {
                    response.ProtocolVersion = response.ProtocolVersion.Substring(indexOfSeparator + 1);
                }
            }

            RegisterErrorHighlightScript();
        }
        finally
        {
            timer.Stop();
            response.Duration = timer.Elapsed;
        }

        if (response.Data == null)
        {
            response.Data = String.Empty;
        }

        return response;
    }

    private void RegisterErrorHighlightScript()
    {
        ClientScript.RegisterStartupScript(GetType(), "RegisterErrorHighlightScript", "$('#ResponseText').addClass('input-validation-error')", true);
    }
</script>

<asp:Content runat="server" ContentPlaceHolderID="BodyPlaceholder">
<form runat="server" id="BodyForm" autocomplete="off">
    <div id="Main">
        <div ><em><%: operation.Description %></em></div>
        <div>
            <h1><a id="UrlPrompt" href="#">URL</a></h1>
            <% if (operation.Credentials != null && (operation.Credentials.Type == AuthenticationType.Basic || operation.Credentials.Type == AuthenticationType.Digest)) { %>
            <div id="Authentication">
                <span class="strong spacer-right">Authentication:</span>
                <span><%: operation.Credentials.Type.ToString() %></span>
                <span class="spacer"></span>
                <span class="strong spacer-right">Username:</span>
                <input type="text" id="UserName" runat="server" />
                <span class="spacer"></span>
                <span class="strong spacer-right">Password:</span>
                <input type="password" id="Password" runat="server" />
            </div>
            <% } %>
            <span id="ServiceUrl"><%: serviceUrl %></span>
            <input id="OperationUrl" runat="server" type="text" class="has-right-margin" />
            <asp:DropDownList id="ResourceFormat" runat="server" AutoPostBack="True" CssClass="has-right-margin">
                <asp:ListItem>JSON</asp:ListItem>
                <asp:ListItem>XML</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList id="HttpMethod" runat="server" CssClass="has-right-margin">
                <asp:ListItem>GET</asp:ListItem>
                <asp:ListItem>POST</asp:ListItem>
                <asp:ListItem>PUT</asp:ListItem>
                <asp:ListItem>DELETE</asp:ListItem>
                <asp:ListItem>PATCH</asp:ListItem>
                <asp:ListItem>HEAD</asp:ListItem>
                <asp:ListItem>OPTIONS</asp:ListItem>
            </asp:DropDownList>
            <span id="ConnectToProxy" runat="server" class="has-right-margin">
                <span>Proxy port:</span>
                <input type="text" id="ProxyPort" runat="server" />
            </span>
            <asp:Button runat="server" ID="Execute" Text="Execute" />
            <span class="button-separator">
                <input type="button" runat="server" id="Reload" value="Reload" />
            </span>
            <span id="PersistenceControls" class="button-separator">
                <input type="button" id="Import" value="Import" />
                <input type="button" id="Export" value="Export" />
                <input type="file" id="File" class="invisible" />
            </span>
        </div>
        <div>
            <h1>Headers</h1>
            <textarea runat="server" cols="20" id="HeaderText" rows="2" spellcheck="false"></textarea>
        </div>
        <div id="BodySection">
            <h1>Body</h1>
            <textarea runat="server" cols="20" id="RequestText" rows="2" spellcheck="false"></textarea>
        </div>
        <div>
            <div id="ResponseHeader">
                <h1>Response</h1>
            </div>
            <div id="ResponseHeaderToggle" class="toggle-option">
                <asp:CheckBox id="DisplayResponseHeaders" runat="server" />
                <label class="spacer-left" for="DisplayResponseHeaders"><span class="underlined">A</span>lways display response headers</label>
            </div>
            <div id="FormatBodyToggle" class="toggle-option">
                <asp:CheckBox id="DoNotFormatBody" runat="server" />
                <label class="spacer-left" for="DoNotFormatBody"><span class="underlined">D</span>o not format body</label>
            </div>
            <div id="ResponseHeightControls">
                <a id="ViewResponse" runat="server" href="#">View in Browser</a>
                <span class="button-separator">
                    <a id="DecreaseResponseHeight" href="#">Decrease Height</a>
                    <a id="IncreaseResponseHeight" href="#">Increase Height</a>
                </span>
            </div>
            <div class="clear"></div>
            <textarea id="ResponseText" runat="server" readonly="readonly" rows="80" cols="25" spellcheck="false"></textarea>
        </div>
        <div id="HotKeys">
            Hot keys: F2 = execute, F5 = reload, Alt-A = toggle response headers, Alt-B = view response in the browser, Alt-D = toggle body formatting, Alt-Plus = increase output window height, Alt-Minus = decrease output window height
        </div>
        <input type="hidden" id="OperationId" value="<%: operationId %>" />
        <input type="hidden" runat="server" id="ContentType" />
        <input type="hidden" runat="server" id="ResponseData" />
    </div>
</form>
<script type="text/javascript" src="<%= ProxyUrlHelper.GetByRouteName(Response, "ProxyProxyOp") %>"></script>
</asp:Content>
