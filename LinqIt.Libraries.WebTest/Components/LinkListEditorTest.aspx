<%@ Page Title="" Language="C#" MasterPageFile="~/Test.Master" AutoEventWireup="true" CodeBehind="LinkListEditorTest.aspx.cs" Inherits="LinqIt.Libraries.WebTest.Components.LinkListEditorTest" %>
<%@ Register src="LinkListEditor.ascx" tagname="LinkListEditor" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="/assets/lib/linqit-components/js/treeview.js" type="text/javascript"></script>
    <script src="/assets/lib/linqit-components/js/LinqIt.LinkEditor.js" type="text/javascript"></script>
    <script src="/assets/lib/linqit-components/js/LinqIt.LinkListEditor.js" type="text/javascript"></script>
    
    <link href="/assets/lib/linqit-components/css/treeview.css" rel="stylesheet" type="text/css">
    <link href="/assets/lib/linqit-components/css/LinqIt.Components.css" rel="stylesheet" type="text/css">
    <link href="/assets/lib/linqit-components/css/LinqIt.LinkEditor.css" rel="stylesheet" type="text/css">
    <link href="/assets/lib/linqit-components/css/LinqIt.LinkListEditor.css" rel="stylesheet" type="text/css">


    <style type="text/css">
	    #sortable { list-style-type: none; margin: 0; padding: 0; }
	    #sortable li { margin: 0 3px 3px 3px; padding:0; height: 18px; }
	    #sortable li span { position: absolute; margin-left: -1.3em; }
	</style>
	
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:LinkListEditor ID="LinkListEditor1" runat="server" />
</asp:Content>
