<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkListEditor.ascx.cs" Inherits="LinqIt.Libraries.WebTest.Components.LinkListEditor" %>
<%@ Register src="LinkEditor.ascx" tagname="LinkEditor" tagprefix="uc1" %>
<div class="linqit-linklisteditor">
    <table style="width:100%">
        <tr>
            <td class="links">
                <ul class="sortable">
                </ul>
            </td>
            <td class="editor">
                <div class="editor">
                    <uc1:LinkEditor ID="LinkEditor1" runat="server" />
                </div>
            </td>
        </tr>
    </table>
    <div class="buttons">
        <asp:Button ID="btnAddLink" runat="server" UseSubmitBehavior="false" OnClientClick="linqit.linklisteditor.addLink(this); return false;" Text="Add Link" />
        <asp:Button ID="btnRemoveLink" runat="server" UseSubmitBehavior="false" OnClientClick="linqit.linklisteditor.removeLink(this); return false;" Text="Remove Link" />
    </div>
    <asp:HiddenField ID="hiddenValue" runat="server"></asp:HiddenField>
</div>