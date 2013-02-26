<%@ Page Language="C#" ValidateRequest="false" %>

<script runat="server" language="C#">
    protected void Page_Init(object sender, EventArgs e)
    {
        Response.Clear();

        string contentType = Request.Unvalidated.Form["ct"];

        if (!String.IsNullOrWhiteSpace(contentType))
        {
            Response.ContentType = contentType;
        }
        else
        {
            Response.ContentType = "text/plain";
        }

        string responseText = Server.UrlDecode(Request.Unvalidated.Form["txt"]);

        if (responseText != null)
        {
            Response.Write(responseText);
        }

        Response.End();
    }
</script>
