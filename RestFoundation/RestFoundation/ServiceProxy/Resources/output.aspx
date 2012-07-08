<%@ Page Language="C#" %>

<script runat="server" language="C#">
    public void Page_Init(object sender, EventArgs e)
    {
        Response.Clear();

        string contentType = Request.Params["ct"];

        if (!String.IsNullOrWhiteSpace(contentType))
        {
            Response.ContentType = contentType;
        }
        else
        {
            Response.ContentType = "text/plain";
        }

        string responseText = Request.Params["txt"];

        if (responseText != null)
        {
            Response.Write(Server.UrlDecode(responseText));
        }

        Response.End();
    }
</script>
