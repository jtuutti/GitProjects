﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Faq.aspx.cs" Inherits="RestTest.Views.Faq" %>
<%@ Register TagPrefix="faq" TagName="header" Src="FaqHeader.ascx"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>FAQ</title>
</head>
<body>
    <faq:header runat="server" />
    <h3>
        This is an example of a web page routed by the REST foundation.
    </h3>       
    <div id="divQueryParameters" runat="server" />
</body>
</html>
