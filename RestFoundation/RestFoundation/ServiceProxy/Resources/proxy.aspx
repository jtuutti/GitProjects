﻿<%@ Page Language="C#" MasterPageFile="help.master" ClientIDMode="Static" ValidateRequest="false" EnableViewStateMac="false" EnableEventValidation="false" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Net.Security" %>
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

        if (!Request.IsLocal || String.Equals(Request.Url.Host, "localhost", StringComparison.OrdinalIgnoreCase) ||
            String.Equals(Request.Url.Host, "127.0.0.1"))
        {
            ConnectToProxy.Visible = false;
        }

        Tuple<string, string> urlParts = operation.GenerateSampleUrlParts();
        serviceUrl = urlParts.Item1;
        OperationUrl.Value = urlParts.Item2;

        if (operation.Credentials != null)
        {
            if (!String.IsNullOrEmpty(operation.Credentials.DefaultUserName))
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
    }

    protected void Page_Load(object sender, EventArgs e)
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
                string data, protocolVersion, responseCode;
                TimeSpan duration;

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

                if ("GET".Equals(HttpMethod.SelectedValue) || "HEAD".Equals(HttpMethod.SelectedValue) || "OPTIONS".Equals(HttpMethod.SelectedValue))
                {
                    data = PerformGetHeadOptionsRequest(url, client, out duration, out protocolVersion, out responseCode);
                }
                else if ("POST".Equals(HttpMethod.SelectedValue) || "PUT".Equals(HttpMethod.SelectedValue) || "PATCH".Equals(HttpMethod.SelectedValue))
                {
                    data = PerformPostPutPatchRequest(url, client, out duration, out protocolVersion, out responseCode);
                }
                else if ("DELETE".Equals(HttpMethod.SelectedValue))
                {
                    data = PerformDeleteRequest(url, client, out duration, out protocolVersion, out responseCode);
                }
                else
                {
                    throw new HttpException((int) HttpStatusCode.MethodNotAllowed, "Invalid HTTP method provided");
                }

                if (data == null)
                {
                    data = String.Empty;
                }

                bool hasData = data.Trim().Length > 0;

                if (hasData)
                {
                    ResponseData.Value = data;
                }
                
                if (client.ResponseHeaders != null)
                {
                    ContentType.Value = client.ResponseHeaders.Get("Content-Type");
                }

                if (hasData && !DoNotFormatBody.Checked)
                {
                    data = FormatBody(data, ContentType.Value);
                }

                if (!hasData || DisplayResponseHeaders.Checked)
                {
                    data = GetResponseHeaders(url, client.ResponseHeaders, duration, protocolVersion, responseCode, hasData) + data;
                }

                ResponseText.Value = data;

                if (hasData)
                {
                    ViewResponse.Visible = true;
                }
            }
        }
        catch (HttpException ex)
        {
            ResponseText.Value = String.Format(CultureInfo.InvariantCulture, "HTTP/1.1: {0} - {1}", ex.GetHttpCode(), ex.Message);
        }
        catch (Exception ex)
        {
            ResponseText.Value = String.Format(CultureInfo.InvariantCulture, "HTTP/1.1: 500 - {0}", ex.Message);
        }
        finally
        {
            if (validationCallback != null)
            {
                ServicePointManager.ServerCertificateValidationCallback = validationCallback;
            }
        }
    }

    protected void ExportSession(object sender, EventArgs args)
    {
        var session = new ProxySession
        {
            ServiceUrl = serviceUrl,
            OperationUrl = OperationUrl.Value,
            Format = ResourceFormat.SelectedValue,
            Method = HttpMethod.SelectedValue,
            Headers = HeaderText.Value.Trim(),
            Body = RequestText.Value.Trim(),
        };

        DateTime now = DateTime.Now;

        Response.Clear();        
        Response.ContentEncoding = Encoding.UTF8;
        Response.ContentType = "application/json";
        Response.AppendHeader("Content-Disposition", String.Format(
                                                            CultureInfo.InvariantCulture,
                                                            "attachment; filename=session.{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6:D3}.dat",
                                                            now.Year,
                                                            now.Month,
                                                            now.Day,
                                                            now.Hour,
                                                            now.Minute,
                                                            now.Second,
                                                            now.Millisecond));

        string serializedSession = ProxyJsonConvert.SerializeObject(session, false, false);

        Response.Output.Write(serializedSession);
        Response.Output.Flush();

        try
        {
            Response.End();
        }
        catch (Exception)
        {
        }
    }

    private static string GetProtocolVersion(ProxyWebClient client)
    {
        ProxyWebResponse response = client.WebResponse;
                
        return response != null ? response.ProtocolVersion.ToString() : null;
    }

    private static string GetStatusCode(ProxyWebClient client)
    {
        ProxyWebResponse response = client.WebResponse;

        return response != null ? String.Format(CultureInfo.InvariantCulture, "{0} - {1}", (int) response.StatusCode, DecodeHttpStatus(response.StatusCode, response.StatusDescription)) : null;
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

    private static string GetResponseHeaders(String url, NameValueCollection headerCollection, TimeSpan duration, string protocolVersion, string responseCode, bool hasOutput)
    {
        if (headerCollection == null || headerCollection.Count == 0)
        {
            return String.Empty;
        }

        var headerString = new StringBuilder();

        headerString.AppendLine("RESPONSE");
        CreateOutputSeparator(headerString);

        headerString.Append("URL: ").AppendLine(url.Replace("%20", "+"));
        headerString.Append("HTTP/").Append(protocolVersion);

        if (!String.IsNullOrEmpty(responseCode))
        {
            headerString.Append(": ").AppendLine(responseCode);
        }
        else
        {
            headerString.AppendLine();
        }

        headerString.AppendFormat("Duration: {0} ms", Convert.ToInt32(duration.TotalMilliseconds)).AppendLine().AppendLine();

        headerString.AppendLine("HEADERS");
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
        string charset = client.Headers.Get(AcceptCharsetHeader);

        if (String.IsNullOrWhiteSpace(charset) || "utf-8".Equals(charset, StringComparison.OrdinalIgnoreCase))
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
            client.Headers[AcceptHeader] = "text/xml";
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

        if ("XML".Equals(ResourceFormat.Text))
        {
            client.Headers[ContentTypeHeader] = "text/xml; charset=utf-8";
        }
        else if ("JSON".Equals(ResourceFormat.Text))
        {
            client.Headers[ContentTypeHeader] = "application/json; charset=utf-8";
        }
    }

    private string GetHttpError(WebException ex, TimeSpan duration)
    {
        ClientScript.RegisterStartupScript(GetType(), "HttpErrorScript", "$('#ResponseText').addClass('input-validation-error')", true);
        
        var webResponse = ex.Response as ProxyWebResponse;

        var errorString = new StringBuilder();
        errorString.AppendLine("RESPONSE");
        CreateOutputSeparator(errorString);
        errorString.Append("URL: ").AppendLine(String.Concat(serviceUrl, OperationUrl.Value.Replace("+", "%20")));

        if (webResponse != null)
        {
            errorString.AppendFormat("HTTP/{0}: {1} - {2}",
                                     webResponse.ProtocolVersion,
                                     (int) webResponse.StatusCode,
                                     DecodeHttpStatus(webResponse.StatusCode, webResponse.StatusDescription));
        }
        else
        {
            var httpResponse = ex.Response as HttpWebResponse;

            if (httpResponse != null)
            {
                errorString.AppendFormat("HTTP/{0}: {1} - {2}",
                                         httpResponse.ProtocolVersion,
                                         (int) httpResponse.StatusCode,
                                         DecodeHttpStatus(httpResponse.StatusCode, httpResponse.StatusDescription));
            }
        }
        
        errorString.AppendLine().AppendFormat("Duration: {0} ms", Convert.ToInt32(duration.TotalMilliseconds));
        errorString.Append(GetErrorResponseHeaders(ex));

        string responseData = GetErrorResponseData(ex);

        if (!String.IsNullOrWhiteSpace(responseData))
        {
            errorString.AppendLine().AppendLine().AppendLine("BODY");
            CreateOutputSeparator(errorString);
            errorString.Append(responseData);
        }

        return errorString.ToString();
    }

    private string GetErrorResponseHeaders(WebException ex)
    {
        if (!DisplayResponseHeaders.Checked)
        {
            return String.Empty;
        }

        var headerString = new StringBuilder();
               
        headerString.AppendLine().AppendLine().AppendLine("HEADERS");
        CreateOutputSeparator(headerString);
        
        var headerNames = ex.Response.Headers.AllKeys.OrderBy(k => k).ToList();

        for (int i = 0; i < headerNames.Count; i++)
        {
            string headerName = headerNames[i];

            if (String.IsNullOrWhiteSpace(headerName))
            {
                continue;
            }

            string headerValue = ex.Response.Headers[headerName];

            if (String.IsNullOrWhiteSpace(headerName) || headerName.Equals("set-cookie", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            headerString.AppendFormat("{0}: {1}", headerName, headerValue);

            if (i < headerNames.Count - 1)
            {
                headerString.AppendLine();
            }
        }

        return headerString.ToString();
    }

    private string GetErrorResponseData(WebException ex)
    {
        var responseStream = ex.Response.GetResponseStream();

        if (responseStream == null ||
            ex.Response.ContentType == null ||
            ex.Response.ContentType.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0 ||
            ex.Response.ContentType.IndexOf("application/xhtml+xml", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return String.Empty;
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
            return String.Empty;
        }
    }

    private string PerformGetHeadOptionsRequest(string url, ProxyWebClient client, out TimeSpan duration, out string protocolVersion, out string responseCode)
    {
        var timer = Stopwatch.StartNew();
        string data;

        try
        {
            if (String.Equals("OPTIONS", HttpMethod.SelectedValue))
            {
                client.Options = true;
            }
            else if (String.Equals("HEAD", HttpMethod.SelectedValue))
            {
                client.HeadOnly = true;
            }

            data = client.DownloadString(url);
            responseCode = GetStatusCode(client);
            protocolVersion = GetProtocolVersion(client);
        }
        catch (WebException ex)
        {
            data = GetHttpError(ex, timer.Elapsed);

            responseCode = null;
            protocolVersion = String.Empty;
        }
        finally
        {
            timer.Stop();
            duration = timer.Elapsed;
        }

        return data;
    }

    private string PerformPostPutPatchRequest(string url, ProxyWebClient client, out TimeSpan duration, out string protocolVersion, out string responseCode)
    {
        var timer = Stopwatch.StartNew();
        string data;

        try
        {
            data = client.UploadString(url, HttpMethod.SelectedValue, RequestText.Value);
            responseCode = GetStatusCode(client);
            protocolVersion = GetProtocolVersion(client);
        }
        catch (WebException ex)
        {
            data = GetHttpError(ex, timer.Elapsed);

            responseCode = null;
            protocolVersion = String.Empty;
        }
        finally
        {
            timer.Stop();
            duration = timer.Elapsed;
        }

        return data;
    }

    private string PerformDeleteRequest(string url, ProxyWebClient client, out TimeSpan duration, out string protocolVersion, out string responseCode)
    {
        var timer = Stopwatch.StartNew();
        string data;

        try
        {
            data = client.UploadString(url, HttpMethod.SelectedValue, String.Empty);
            responseCode = GetStatusCode(client);
            protocolVersion = GetProtocolVersion(client);
        }
        catch (WebException ex)
        {
            data = GetHttpError(ex, timer.Elapsed);

            responseCode = null;
            protocolVersion = String.Empty;
        }
        finally
        {
            timer.Stop();
            duration = timer.Elapsed;
        }

        return data;
    }
</script>

<asp:Content runat="server" ContentPlaceHolderID="bodyPlaceholder">
<form runat="server" id="bodyForm" autocomplete="off">
    <div id="Main">
        <div ><em><%: operation.Description %></em></div>
        <div>
            <h1>URL</h1>
            <% if (operation.Credentials != null) { %>
            <div id="Authentication">
                <strong>Authentication:</strong>
                <span><%: operation.Credentials.Type.ToString() %></span>
                <span class="spacer"></span>
                <strong>Username:</strong>
                <input type="text" id="UserName" runat="server" />
                <span class="spacer"></span>
                <strong>Password:</strong>
                <input type="password" id="Password" runat="server" />
            </div>
            <% } %>
            <span id="ServiceUrl"><%: serviceUrl %></span>
            <input id="OperationUrl" runat="server" type="text" />
            <asp:DropDownList id="ResourceFormat" runat="server" AutoPostBack="True">
                <asp:ListItem>JSON</asp:ListItem>
                <asp:ListItem>XML</asp:ListItem>
            </asp:DropDownList>
            <asp:DropDownList id="HttpMethod" runat="server">
                <asp:ListItem>GET</asp:ListItem>
                <asp:ListItem>POST</asp:ListItem>
                <asp:ListItem>PUT</asp:ListItem>
                <asp:ListItem>DELETE</asp:ListItem>
                <asp:ListItem>PATCH</asp:ListItem>
                <asp:ListItem>HEAD</asp:ListItem>
                <asp:ListItem>OPTIONS</asp:ListItem>
            </asp:DropDownList>
            <span id="ConnectToProxy" runat="server">
                <span>Proxy port:</span>
                <input type="text" id="ProxyPort" runat="server" />
            </span>
            <asp:Button runat="server" ID="Execute" Text="Execute" />
            <span id="PersistenceControls" class="button-separator">
                <input type="button" id="Import" value="Import" />
                <asp:Button runat="server" ID="Export" Text="Export" OnClick="ExportSession" />
                <input type="file" id="File" class="invisible" />
            </span>
            <span class="button-separator">
                <input type="button" runat="server" id="Reload" value="Reload" />
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
                <label for="DisplayResponseHeaders"><span class="underlined">A</span>lways display response headers</label>
            </div>
            <div id="FormatBodyToggle" class="toggle-option">
                <asp:CheckBox id="DoNotFormatBody" runat="server" />
                <label for="DoNotFormatBody"><span class="underlined">D</span>o not format body</label>
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
