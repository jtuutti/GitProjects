<%@ Page Language="C#" ValidateRequest="false" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="RestFoundation.ServiceProxy" %>

<script runat="server" language="C#">
    protected void Page_Init(object sender, EventArgs e)
    {
        var session = new ProxySession
        {
            ServiceUrl = Request.Unvalidated.Form["ServiceUrl"],
            OperationUrl = Request.Unvalidated.Form["OperationUrl"],
            Format = Request.Unvalidated.Form["ResourceFormat"],
            Method = Request.Unvalidated.Form["HttpMethod"],
            Headers = Request.Unvalidated.Form["HeaderText"],
            Body = Request.Unvalidated.Form["RequestText"]
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
        Response.End();
    }
</script>
