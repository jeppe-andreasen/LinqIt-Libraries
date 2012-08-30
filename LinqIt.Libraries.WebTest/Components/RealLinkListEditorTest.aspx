<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RealLinkListEditorTest.aspx.cs" Inherits="LinqIt.Libraries.WebTest.Components.RealLinkListEditorTest" %>

<%@ Register assembly="LinqIt.Components" namespace="LinqIt.Components" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="/assets/lib/jquery-ui-1.8.23/css/ui-darkness/jquery-ui-1.8.23.css" rel="stylesheet" type="text/css">
    <link href="/assets/lib/jquery.alerts/css/jquery.alerts.css" rel="stylesheet" type="text/css">
    <script src="/assets/lib/jquery-ui-1.8.23/js/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="/assets/lib/jquery-ui-1.8.23/js/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <script src="/assets/lib/jquery.alerts/js/jquery.alerts.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <cc1:LinqItLinkListEditor ID="LinqItLinkListEditor1" runat="server">
        </cc1:LinqItLinkListEditor>
    
    </div>
    </form>
</body>
</html>
