﻿<%@ Page Language="C#" %>

<script runat="server" language="C#">
    protected void Page_Init(object sender, EventArgs e)
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

        string responseText = Server.UrlDecode(Request.Params["txt"]);

        if (responseText != null)
        {
            Response.Write(responseText);
        }

        Response.End();
    }
</script>
