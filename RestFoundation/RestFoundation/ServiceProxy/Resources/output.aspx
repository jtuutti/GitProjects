<%@ Page Language="C#" %>

<script runat="server" language="C#">
    public void Page_Init(object sender, EventArgs e)
    {
        Response.Clear();

        string format = Request.Params["format"];

        if ("JSON".Equals(format, StringComparison.OrdinalIgnoreCase))
        {
            Response.ContentType = "application/json";
        }
        else if ("XML".Equals(format, StringComparison.OrdinalIgnoreCase))
        {
            Response.ContentType = "text/xml";
        }
        else
        {
            Response.ContentType = "text/plain";
        }

        string responseText = Request.Params["responseText"];

        if (responseText != null)
        {
            Response.Write(Server.UrlDecode(responseText));
        }
    }
</script>
