<%@ Page Title="" Language="C#" MasterPageFile="~/Test.Master" AutoEventWireup="true" CodeBehind="LinkEditorTest.aspx.cs" Inherits="LinqIt.Libraries.WebTest.Components.LinkEditorTest" %>
<%@ Register src="LinkEditor.ascx" tagname="LinkEditor" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="/assets/lib/linqit-components/js/treeview.js" type="text/javascript"></script>
    <script src="/assets/lib/linqit-components/js/LinqIt.LinkEditor.js" type="text/javascript"></script>
    
    <link href="/assets/lib/linqit-components/css/treeview.css" rel="stylesheet" type="text/css">
    <link href="/assets/lib/linqit-components/css/LinqIt.Components.css" rel="stylesheet" type="text/css">
    <link href="/assets/lib/linqit-components/css/LinqIt.LinkEditor.css" rel="stylesheet" type="text/css">

    <script type="text/javascript">
        function setTestValue() {
            var editor = $('.linqit-linkeditor');
            linqit.linkeditor.setValue(editor, $('#testvalue').val());
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:LinkEditor ID="LinkEditor1" runat="server" />
    <p>&nbsp;</p>
    <hr>
    <input type="text" id="testvalue">
    <a href="#" onclick="setTestValue(); return false;">Set Test Value</a>
    
</asp:Content>
