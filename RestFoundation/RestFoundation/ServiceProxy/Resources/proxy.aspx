<%@ Page Language="C#" MasterPageFile="help.master" ClientIDMode="Static" ValidateRequest="false" EnableViewStateMac="false" EnableEventValidation="false" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Net.Security" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="RestFoundation.Runtime" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>

<script runat="server" language="C#">
    private const string AcceptHeader = "Accept";
    private const string AcceptCharsetHeader = "Accept-Charset";
    private const string ContentTypeHeader = "Content-Type";
    private const string UserAgentHeader = "User-Agent";
    private const string UserAgent = "Rest Foundation Proxy";

    private ProxyOperation operation;
    private Guid operationId;
    private string serviceUrl;

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!Guid.TryParse(Request.QueryString["oid"], out operationId) || operationId == Guid.Empty)
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

        Tuple<string, string> urlParts = operation.GenerateSampleUrlParts();
        serviceUrl = urlParts.Item1;
        OperationUrl.Value = urlParts.Item2;

        if (operation.Credentials != null)
        {
            if (!String.IsNullOrEmpty(operation.Credentials.Item2))
            {
                UserName.Value = operation.Credentials.Item2;
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

        ResourceFormat.SelectedIndexChanged += (o, args) =>
        {
            var resourceFormat = (DropDownList) o;
            Response.Redirect(String.Format(CultureInfo.InvariantCulture, "proxy?oid={0}&ct={1}", operationId, resourceFormat.Text.ToLowerInvariant()));
        };

        ResourceFormat.Text = format.ToUpperInvariant();
        HttpMethod.Value = operation.HttpMethod.ToString().ToUpperInvariant();

        foreach (Tuple<string, string> header in operation.AdditionalHeaders)
        {
            HeaderText.Value += String.Concat(header.Item1, ": ", header.Item2, Environment.NewLine);
        }

        if (operation.HasResource && operation.RequestExampleType != null)
        {
            IResourceExampleBuilder requestExampleBuilder;

            try
            {
                requestExampleBuilder = Activator.CreateInstance(operation.RequestExampleType) as IResourceExampleBuilder;
            }
            catch (Exception)
            {
                requestExampleBuilder = null;
            }

            if (requestExampleBuilder != null)
            {
                object requestObj;

                try
                {
                    requestObj = requestExampleBuilder.BuildInstance();
                }
                catch (Exception)
                {
                    requestObj = null;
                }

                if (requestObj != null)
                {
                    if (String.Equals("xml", format, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            RequestText.Value = XmlConvert.SerializeObject(requestObj, System.Xml.Formatting.Indented);
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
                            RequestText.Value = JsonConvert.SerializeObject(requestObj, Formatting.Indented);
                        }
                        catch (Exception)
                        {
                            RequestText.Value = String.Empty;
                        } 
                    }
                }
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
                        { new Uri(serviceUrl), operation.Credentials.Item1.ToString(), new NetworkCredential(UserName.Value.Trim(), Password.Value) }
                    };

                    client.Credentials = cachedCredentials;
                }

                AddHeaders(client);
                SetEncoding(client);

                if ("GET".Equals(HttpMethod.Value) || "HEAD".Equals(HttpMethod.Value) || "OPTIONS".Equals(HttpMethod.Value))
                {
                    data = PerformGetHeadOptionsRequest(url, client, out duration, out protocolVersion, out responseCode);
                }
                else if ("POST".Equals(HttpMethod.Value) || "PUT".Equals(HttpMethod.Value) || "PATCH".Equals(HttpMethod.Value))
                {
                    data = PerformPostPutPatchRequest(url, client, out duration, out protocolVersion, out responseCode);
                }
                else if ("DELETE".Equals(HttpMethod.Value))
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
                    data = GetResponseHeaders(client.ResponseHeaders, duration, protocolVersion, responseCode, hasData) + data;
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

    private static string GetHttpError(WebException ex)
    {
        var webResponse = ex.Response as ProxyWebResponse;

        if (webResponse != null)
        {
            return String.Format(CultureInfo.InvariantCulture,
                                 "HTTP/{0}: {1} - {2}",
                                 webResponse.ProtocolVersion,
                                 (int) webResponse.StatusCode,
                                 DecodeHttpStatus(webResponse.StatusCode, webResponse.StatusDescription));
        }

        var httpResponse = ex.Response as HttpWebResponse;

        if (httpResponse != null)
        {
            return String.Format(CultureInfo.InvariantCulture,
                                 "HTTP/{0}: {1} - {2}",
                                 httpResponse.ProtocolVersion,
                                 (int) httpResponse.StatusCode,
                                 DecodeHttpStatus(httpResponse.StatusCode, httpResponse.StatusDescription));
        }

        return ex.Message;
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

    private static string GetResponseHeaders(NameValueCollection headerCollection, TimeSpan duration, string protocolVersion, string responseCode, bool hasOutput)
    {
        if (headerCollection == null || headerCollection.Count == 0)
        {
            return String.Empty;
        }

        var headerString = new StringBuilder();

        headerString.AppendLine("RESPONSE");
        CreateOutputSeparator(headerString);

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

        if (String.IsNullOrWhiteSpace(client.Headers[AcceptHeader]))
        {
            SetMimeTypeForHeader(client, AcceptHeader);
        }

        if (String.IsNullOrWhiteSpace(client.Headers[ContentTypeHeader]))
        {
            if ("POST".Equals(HttpMethod.Value) || "PUT".Equals(HttpMethod.Value) || "PATCH".Equals(HttpMethod.Value))
            {
                SetMimeTypeForHeader(client, ContentTypeHeader);
            }
        }
    }

    private void SetMimeTypeForHeader(ProxyWebClient client, string headerName)
    {
        if ("XML".Equals(ResourceFormat.Text))
        {
            client.Headers[headerName] = "text/xml; charset=utf-8";
        }
        else if ("JSON".Equals(ResourceFormat.Text))
        {
            client.Headers[headerName] = "application/json; charset=utf-8";
        }
    }

    private string PerformGetHeadOptionsRequest(string url, ProxyWebClient client, out TimeSpan duration, out string protocolVersion, out string responseCode)
    {
        var timer = Stopwatch.StartNew();
        string data;

        try
        {
            if (String.Equals("OPTIONS", HttpMethod.Value))
            {
                client.Options = true;
            }
            else if (String.Equals("HEAD", HttpMethod.Value))
            {
                client.HeadOnly = true;
            }

            data = client.DownloadString(url);
            responseCode = GetStatusCode(client);
            protocolVersion = GetProtocolVersion(client);
        }
        catch (WebException ex)
        {
            data = GetHttpError(ex);

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
            data = client.UploadString(url, HttpMethod.Value, RequestText.Value);
            responseCode = GetStatusCode(client);
            protocolVersion = GetProtocolVersion(client);
        }
        catch (WebException ex)
        {
            data = GetHttpError(ex);

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
            data = client.UploadString(url, HttpMethod.Value, String.Empty);
            responseCode = GetStatusCode(client);
            protocolVersion = GetProtocolVersion(client);
        }
        catch (WebException ex)
        {
            data = GetHttpError(ex);

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
    <div id="main">
        <div ><em><%: operation.Description %></em></div>
        <div>
            <h1>URL</h1>
            <% if (operation.Credentials != null) { %>
            <div id="Authentication">
                <strong>Authentication:</strong>
                <span><%: operation.Credentials.Item1.ToString() %></span>
                <span class="spacer" />
                <strong>Username:</strong>
                <input type="text" id="UserName" runat="server" />
                <span class="spacer" />
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
            <select id="HttpMethod" runat="server">
                <option>GET</option>
                <option>POST</option>
                <option>PUT</option>
                <option>DELETE</option>
                <option>PATCH</option>
                <option>HEAD</option>
                <option>OPTIONS</option>
            </select>
            <asp:Button runat="server" ID="Execute" Text="Execute" />
            <input type="button" runat="server" id="Reload" value="Reload" />
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
            <div id="ResponseHeaderToggle" class="toggleOption">
                <asp:CheckBox id="DisplayResponseHeaders" runat="server" />
                <label for="DisplayResponseHeaders"><span style="text-decoration: underline">A</span>lways display response headers</label>
            </div>
            <div id="FormatBodyToggle" class="toggleOption">
                <asp:CheckBox id="DoNotFormatBody" runat="server" />
                <label for="DoNotFormatBody"><span style="text-decoration: underline">D</span>o not format body</label>
            </div>
            <div id="ResponseHeightControls">
                <a id="ViewResponse" runat="server" href="#" style="margin-right: 25px;">View in Browser</a>
                <a id="DecreaseResponseHeight" href="#">Decrease Height</a>
                <a id="IncreaseResponseHeight" href="#">Increase Height</a>
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
<script type="text/javascript">
    jQuery.cookie=function(d,b,a){if(arguments.length>1&&(b===null||typeof b!=="object")){a=jQuery.extend({},a);if(b===null)a.expires=-1;if(typeof a.expires==="number"){var g=a.expires,e=a.expires=new Date;e.setDate(e.getDate()+g)}return document.cookie=[encodeURIComponent(d),"=",a.raw?String(b):encodeURIComponent(String(b)),a.expires?"; expires="+a.expires.toUTCString():"",a.path?"; path="+a.path:"",a.domain?"; domain="+a.domain:"",a.secure?"; secure":""].join("")}a=b||{};var c,f=a.raw?function(a){return a}:decodeURIComponent;return(c=new RegExp("(?:^|; )"+encodeURIComponent(d)+"=([^;]*)").exec(document.cookie))?f(c[1]):null};
</script>
<script type="text/javascript">
    (function(a){function p(b){if(document.selection){var d=document.selection.createRange().getBookmark(),a=b.createTextRange();a.moveToBookmark(d);b=b.createTextRange();b.collapse(!0);b.setEndPoint("EndToStart",a);return b.text.length+a.text.length}return b.selectionEnd}function u(b){var d=p(b.ta),b=b.ta.value,b=b.substr(0,d),d=b.lastIndexOf("\n");0<=d&&(b=b.substr(d+1,b.length));return b}function v(b){var d=p(b.ta),a=b.ta.value,a=a.substr(0,d);if(" "==a.charAt(a.length-1)||":"==a.charAt(a.length-1)|| "\n"==a.charAt(a.length-1))return"";for(var d=[],c=0,e=a.length-1;c<b.wordCount&&0<=e&&"\n"!=a.charAt(e);)d.unshift(a.charAt(e)),e--,(" "==a.charAt(e)||":"==a.charAt(e)||0>e)&&c++;return d.join("")}function q(b,d,g){var c,e;if(!b.listVisible){b.listVisible=!0;if("outter"==b.mode)e=a(b.ta).offset(),c=e.top+a(b.ta).height()+8,e=e.left;else{a(b.clone).width(a(b.ta).width());c=b.ta;var h=p(b.ta),f=c.value;e=f.substr(0,h);for(var h=f.substr(h,f.length),f=[],i=a(b.clone).width(),l="",j=0,m=0,k="",n=b.chars, q=e.length,r=0;r<q;r++){var o=e.charAt(r),k=k+o,m=m+("undefined"==typeof n[o]?0:n[o]);if(" "==o||"-"==o||":"==o)j+m<i-1?(l+=k,j+=m):(f.push(l),l=k,j=m),k="",m=0;"\n"==o&&(j+m<i-1?f.push(l+k):(f.push(l),f.push(k)),k=l="",m=j=0)}j+m<i-1?f.push(l+k):(f.push(l),f.push(k));e=a(b.clone);e.html("");for(i=0;i<f.length-1;i++)e.append("<div style='height:"+parseInt(b.lineHeight)+"px;'>"+f[i]+"</div>");e.append("<span id='"+b.id+"' style='display:inline-block;'>"+f[f.length-1]+"</span>");e.append("<span id='rest' style='max-width:'"+ b.ta.clientWidth+"px'>"+h.replace(/\n/g,"<br/>")+"&nbsp;</span>");e.get(0).scrollTop=c.scrollTop;e=e.children("#"+b.id);h=e.offset();c=h.top+e.height();e=h.left+e.width()}a(b.list).css({left:e+"px",top:parseInt(c+3)+"px",display:"block"})}c="";g=RegExp("("+g.replace("*","\\*")+")");e=a(b.ta).width()-5;e="outter"==b.mode?"style='width:"+e+"px;'":"";for(h=0;h<d.length;h++)c+="<li data-value='"+d[h]+"' "+e+">"+d[h].replace(g,"$1")+"</li>";a(b.list).html(c)}function i(b){if(b.listVisible)a(b.list).css("display", "none"),b.listVisible=!1}function s(b,d){var g=a(d.list).find("[data-selected=true]");1!=g.length?0<b?a(d.list).find("li:first-child").attr("data-selected","true"):a(d.list).find("li:last-child").attr("data-selected","true"):(g.attr("data-selected","false"),0<b?g.next().attr("data-selected","true"):g.prev().attr("data-selected","true"))}function w(b){var d=a(b.list).find("[data-selected=true]");if(1==d.length)return d.get(0);b=a(b.list).find("li");return 1==b.length?b.get(0):null}function t(b,d){for(var g= a(b).attr("data-value"),c=p(d.ta),e=d.ta.value,e=e.substr(0,c),h=0,f=e.length-1;h<d.wordCount&&0<=f&&":"!=e.charAt(f)&&" "!=e.charAt(f)&&"\n"!=e.charAt(f);)f--,(" "==e.charAt(f)||":"==e.charAt(f)||0>f)&&h++;e=d.ta.value.substr(0,f+1);c=d.ta.value.substr(c,d.ta.value.length);h=d.ta.scrollTop;d.ta.value=e+g+c;d.ta.scrollTop=h;d.ta.selectionStart=f+1+g.length;d.ta.selectionEnd=f+1+g.length;i(d);a(d.ta).focus()}function x(b){a(b.list).delegate("li","click",function(a){t(this,b);a.stopPropagation();a.preventDefault(); return!1});a(b.ta).blur(function(){setTimeout(function(){i(b)},400)});a(b.ta).click(function(){i(b)});a(b.ta).keydown(function(a){if(b.listVisible)switch(a.keyCode){case 13:case 40:case 38:return a.stopPropagation(),a.preventDefault(),!1;case 27:i(b)}return a.ctrlKey&&32==a.keyCode?(a.stopPropagation(),a.preventDefault(),!1):!0});a(b.ta).keyup(function(d){if(b.listVisible){if(40==d.keyCode)return s(1,b),d.stopPropagation(),d.preventDefault(),!1;if(38==d.keyCode)return s(-1,b),d.stopPropagation(), d.preventDefault(),!1;if(13==d.keyCode){var g=w(b);if(g)return t(g,b),d.stopPropagation(),d.preventDefault(),!1;i(b)}if(27==d.keyCode)return d.stopPropagation(),d.preventDefault(),!1}switch(d.keyCode){case 17:case 27:case 37:case 38:case 39:case 40:return!0}var c=v(b),g=u(b);if(d.ctrlKey&&32==d.keyCode&&0==jQuery.trim(g).length)return b.on.query(c,g,function(a){a.length?q(b,a,c):i(b)}),!1;d=a.trim(g).lastIndexOf(":");""!=c||0<d&&d==a.trim(g).length-1?b.on.query(c,g,function(a){a.length?q(b,a,c):i(b)}): i(b);return!0});a(b.ta).scroll(function(d){d=d.target;a(b.clone).get(0).scrollTop=d.scrollTop})}a.fn.autocomplete=function(b){if("undefined"!=typeof a.browser.msie)b.mode="outter";this.each(function(d,g){if("TEXTAREA"==g.nodeName){j++;n[j]={id:"auto_"+j,ta:g,wordCount:b.wordCount,on:b.on,clone:null,lineHeight:0,list:null,charInLines:{},mode:b.mode,chars:{"`":0,"~":0,1:0,"!":0,2:0,"@":0,3:0,"#":0,4:0,$:0,5:0,"%":0,6:0,"^":0,7:0,"&":0,8:0,"*":0,9:0,"(":0,"0":0,")":0,"-":0,_:0,"=":0,"+":0,q:0,Q:0,w:0, W:0,e:0,E:0,r:0,R:0,t:0,T:0,y:0,Y:0,u:0,U:0,i:0,I:0,o:0,O:0,p:0,P:0,"[":0,"{":0,"]":0,"}":0,a:0,A:0,s:0,S:0,d:0,D:0,f:0,F:0,g:0,G:0,h:0,H:0,j:0,J:0,k:0,K:0,l:0,L:0,";":0,":":0,"'":0,'"':0,"\\":0,"|":0,z:0,Z:0,x:0,X:0,c:0,C:0,v:0,V:0,b:0,B:0,n:0,N:0,m:0,M:0,",":0,"<":0,".":0,">":0,"/":0,"?":0," ":0}};var c=n[j],e=document.createElement("div"),h=a(c.ta).offset();a(e).css({position:"absolute",top:h.top,left:h.left,"border-collapse":a(c.ta).css("border-collapse"),"border-bottom-style":a(c.ta).css("border-bottom-style"), "border-bottom-width":a(c.ta).css("border-bottom-width"),"border-left-style":a(c.ta).css("border-left-style"),"border-left-width":a(c.ta).css("border-left-width"),"border-right-style":a(c.ta).css("border-right-style"),"border-right-width":a(c.ta).css("border-right-width"),"border-spacing":a(c.ta).css("border-spacing"),"border-top-style":a(c.ta).css("border-top-style"),"border-top-width":a(c.ta).css("border-top-width"),direction:a(c.ta).css("direction"),"font-size-adjust":a(c.ta).css("font-size-adjust"), "font-size":a(c.ta).css("font-size"),"font-stretch":a(c.ta).css("font-stretch"),"font-style":a(c.ta).css("font-style"),"font-family":a(c.ta).css("font-family"),"font-variant":a(c.ta).css("font-variant"),"font-weight":a(c.ta).css("font-weight"),width:a(c.ta).css("width"),height:a(c.ta).css("height"),"letter-spacing":a(c.ta).css("letter-spacing"),"margin-bottom":a(c.ta).css("margin-bottom"),"margin-top":a(c.ta).css("margin-top"),"margin-right":a(c.ta).css("margin-right"),"margin-left":a(c.ta).css("margin-left"), "padding-bottom":a(c.ta).css("padding-bottom"),"padding-top":a(c.ta).css("padding-top"),"padding-right":a(c.ta).css("padding-right"),"padding-left":a(c.ta).css("padding-left"),"overflow-x":"hidden","line-height":a(c.ta).css("line-height"),"overflow-y":"hidden","z-index":-10});c.lineHeight=a(c.ta).css("line-height");if(isNaN(parseInt(c.lineHeight)))c.lineHeight=parseInt(a(c.ta).css("font-size"))+2;document.body.appendChild(e);n[j].clone=e;var c=n[j],f;for(f in c.chars)" "==f?a(c.clone).html("<span id='test-width_"+ c.id+"' style='display:inline-block'>&nbsp;</span>"):a(c.clone).html("<span id='test-width_"+c.id+"' style='display:inline-block'>"+f+"</span>"),e=a("#test-width_"+c.id).width(),c.chars[f]=e;f=n[j];c=document.createElement("ul");a(c).addClass("auto-list");document.body.appendChild(c);f.list=c;x(n[j])}})};var n={},j=0})(jQuery);
</script>
<script type="text/javascript">
    var ah="Accept Accept-Charset Accept-Encoding Accept-Language Authorization Content-Type If-Modified-Since If-None-Match".split(" "),headerValues={accept:"application/json,application/json; charset=utf-8,application/xml,application/xml; charset=utf-8,text/xml,text/xml; charset=utf-8,text/html,text/html; charset=utf-8,application/x-www-form-urlencoded,application/x-www-form-urlencoded; charset=utf-8,*/*".split(","),"accept-charset":["utf-8","*"],"accept-encoding":["gzip","deflate","*"],"accept-language":["en-US", "es-ES","*"],"content-type":"application/json,application/json; charset=utf-8,application/xml,application/xml; charset=utf-8,text/xml,text/xml; charset=utf-8,text/html,text/html; charset=utf-8,application/x-www-form-urlencoded,application/x-www-form-urlencoded; charset=utf-8".split(",")};function modifyMethodState(){"GET"==$("#HttpMethod").val()||"DELETE"==$("#HttpMethod").val()||"HEAD"==$("#HttpMethod").val()||"OPTIONS"==$("#HttpMethod").val()?$("#BodySection").hide():$("#BodySection").show()} function getResponseHeight(){var a=parseInt($.cookie("responseHeight"));if(!a||isNaN(a))a=parseInt($("#ResponseText").css("height"));return a}function setHeaderState(){"true"==$.cookie("displayHeaders")?$("#DisplayResponseHeaders").prop("checked",!0):$("#DisplayResponseHeaders").removeProp("checked")}function setBodyFormatState(){"true"==$.cookie("doNotFormatBody")?$("#DoNotFormatBody").prop("checked",!0):$("#DoNotFormatBody").removeProp("checked")} function setDecreaseHeightVisibility(a){200>=a?$("#DecreaseResponseHeight").hide():$("#DecreaseResponseHeight").show()}$(function(){setHeaderState();setBodyFormatState();var a=getResponseHeight();$("#ResponseText").css("height",a);setDecreaseHeightVisibility(a);$("#HttpMethod").change(function(){modifyMethodState()});modifyMethodState()}); $(function(){$("#Reload").click(function(){if(""!=$.trim($("#OperationUrl").val())){var a="proxy",b=$("#OperationId").val(),a=a+("?oid="+encodeURIComponent(b)),b=$("#ResourceFormat").val().toLowerCase(),a=a+("&ct="+encodeURIComponent(b));window.location.href=a}return!1})}); $(function(){$("#DisplayResponseHeaders").change(function(){$.cookie("displayHeaders",$(this).is(":checked").toString(),{path:"/"})});$("#DoNotFormatBody").change(function(){$.cookie("doNotFormatBody",$(this).is(":checked").toString(),{path:"/"})})}); $(function(){$("#IncreaseResponseHeight").click(function(){var a=getResponseHeight(),a=a+100;$("#ResponseText").css("height",a);setDecreaseHeightVisibility(a);$.cookie("responseHeight",a,{path:"/"});return!1});$("#DecreaseResponseHeight").click(function(){var a=getResponseHeight();300<=a&&(a-=100,$("#ResponseText").css("height",a),setDecreaseHeightVisibility(a),$.cookie("responseHeight",a,{path:"/"}));return!1})}); $(function(){$("#ViewResponse").click(function(){$("#ResponseForm").remove();var a=['<form id="ResponseForm" method="POST" action="output" target="_blank">'];a.push('<input type="hidden" name="ct" value="'+$("#ContentType").val()+'"/>');a.push('<input type="hidden" name="txt" value="'+encodeURIComponent($("#ResponseData").val())+'"/>');a.push("</form>");$(a.join("")).appendTo("body")[0].submit();return!1})}); $(function(){$(document).keydown(function(a){if(!a.ctrlKey&&!a.altKey&&!a.shiftKey){if(113==a.keyCode)return $("#Execute").click(),!1;if(116==a.keyCode)return $("#Reload").click(),!1}else if(!a.ctrlKey&&a.altKey&&!a.shiftKey){if(107==a.keyCode)return $("#IncreaseResponseHeight").click(),!1;if(109==a.keyCode)return $("#DecreaseResponseHeight").click(),!1;if(65==a.keyCode)return $("#DisplayResponseHeaders").is(":checked")?$("#DisplayResponseHeaders").removeProp("checked"):$("#DisplayResponseHeaders").prop("checked", !0),$.cookie("displayHeaders",$("#DisplayResponseHeaders").prop("checked").toString(),{path:"/"}),!1;if(66==a.keyCode)return $("#ViewOutput").click(),!1;if(68==a.keyCode)return $("#DoNotFormatBody").is(":checked")?$("#DoNotFormatBody").removeProp("checked"):$("#DoNotFormatBody").prop("checked","checked"),$.cookie("doNotFormatBody",$("#DoNotFormatBody").prop("checked").toString(),{path:"/"}),!1}return!0})}); $(function(){$("#HeaderText").autocomplete({wordCount:1,on:{query:function(a,b,f){var g=[],h=0,c=null,d=b.indexOf(":");if(0>d)c=ah;else if(0<d&&(c=$.trim(b.substr(0,d)),b=$.trim(b.substr(d-1,b.length)),c=headerValues[c.toLowerCase()],":"==b)){f(c);return}if(!(null==c||0==c.length)){for(b=0;b<c.length;b++){var d=c[b].toLowerCase(),e=$.trim(a).toLowerCase();if(0==d.indexOf(e)&&(1==e.length||d!=e))g[h++]=c[b]}f(g)}}}})});
</script>
<script type="text/javascript">
    function setAuthenticationState(a,b){b&&0==$.trim($("input#"+a).val()).length?$("input#"+a).addClass("empty"):!b&&0==$("input#"+a).val().length?$("input#"+a).addClass("empty"):$("input#"+a).removeClass("empty")}$("input#UserName")&&($("input#UserName").bind("change",function(){setAuthenticationState("UserName",!0)}),$("input#UserName").bind("keyup",function(){setAuthenticationState("UserName",!0)}),setAuthenticationState("UserName",!0)); $("input#Password")&&($("input#Password").bind("change",function(){setAuthenticationState("Password",false)}),$("input#Password").bind("keyup",function(){setAuthenticationState("Password",false)}),setAuthenticationState("Password",!1));
</script>
</asp:Content>
