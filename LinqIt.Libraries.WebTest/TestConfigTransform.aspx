<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestConfigTransform.aspx.cs" Inherits="LinqIt.Libraries.WebTest.TestConfigTransform" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="txtOutput" runat="server" TextMode="MultiLine" Height="600" Width="800">
        </asp:TextBox>
        <asp:Literal ID="litOutput" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
