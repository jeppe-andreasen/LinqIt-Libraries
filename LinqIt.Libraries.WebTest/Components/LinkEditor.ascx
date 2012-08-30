<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkEditor.ascx.cs" Inherits="LinqIt.Libraries.WebTest.Components.LinkEditor" %>
<%@ Register assembly="LinqIt.Components" namespace="LinqIt.Components" tagprefix="cc1" %>
<div class="linqit-linkeditor">
    <div class="editor-output">
    </div>
    <div class="editor-input">
	    <ul class="tabs">
            <li><a href="#" data-type="internal" onclick="return linqit.linkeditor.selectTab(this);">Internal</a></li>
            <li><a href="#" data-type="external" onclick="return linqit.linkeditor.selectTab(this);">External</a></li>
            <li><a href="#" data-type="media" onclick="return linqit.linkeditor.selectTab(this);">Media</a></li>
            <li><a href="#" data-type="mailto" onclick="return linqit.linkeditor.selectTab(this);">Mail To</a></li>
            <li><a href="#" data-type="javascript" onclick="return linqit.linkeditor.selectTab(this);">Javascript</a></li>
            <li><a href="#" data-type="anchor" onclick="return linqit.linkeditor.selectTab(this);">Anchor</a></li>
        </ul>
        <div class="editor-form">
        <table>
            <tr>
                <td>
                <fieldset class="internal active">
                    <legend>Internal Link</legend>
                    <table>
                        <tr>
                            <td class="tv">
                                <cc1:LinqItTreeView ID="internalTree" runat="server"></cc1:LinqItTreeView>
                            </td>
                            <td class="labels">
                                <div class="field">
                                    <asp:Label ID="lblInternalText" runat="server" AssociatedControlID="txtInternalText" Text="Link Text:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblInternalTarget" runat="server" AssociatedControlID="ddlInternalTarget" Text="Target:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblInternalTooltip" runat="server" AssociatedControlID="txtInternalTooltip" Text="Tooltip:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblInternalClass" runat="server" AssociatedControlID="txtInternalClass" Text="Css class:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblInternalQueryString" runat="server" AssociatedControlID="txtInternalQueryString" Text="Query string:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblInternalAnchor" runat="server" AssociatedControlID="txtInternalAnchor" Text="Anchor:"></asp:Label>
                                </div>
                            </td>
                            <td>
                                <div class="field">
                                    <asp:TextBox ID="txtInternalText" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:DropDownList ID="ddlInternalTarget" runat="server"></asp:DropDownList>
                                </div>
                                 <div class="field">
                                    <asp:TextBox ID="txtInternalTooltip" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtInternalClass" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtInternalQueryString" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtInternalAnchor" runat="server"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <fieldset class="external">
                    <legend>External Link</legend>
                    <table>
                        <tr>
                            <td class="labels" style="padding-left:0;">
                                 <div class="field">
                                    <asp:Label ID="lblExternalText" runat="server" AssociatedControlID="txtExternalText" Text="Link Text:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblExternalPath" runat="server" AssociatedControlID="txtExternalPath" Text="Url:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblExternalTarget" runat="server" AssociatedControlID="ddlExternalTarget" Text="Target:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblExternalTooltip" runat="server" AssociatedControlID="txtExternalTooltip" Text="Tooltip:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblExternalClass" runat="server" AssociatedControlID="txtExternalClass" Text="Css class:"></asp:Label>
                                </div>
                            </td>
                            <td>
                                <div class="field">
                                    <asp:TextBox ID="txtExternalText" runat="server"></asp:TextBox>
                                </div>
                                <div class="field url">
                                    <asp:TextBox ID="txtExternalPath" runat="server" Text="http://"></asp:TextBox>
                                    <asp:Button ID="btnExternalTestUrl" runat="server" UseSubmitBehavior="false" OnClientClick="return linqit.linkeditor.testUrl(this);" Text="Test" />
                                </div>
                                <div class="field">
                                    <asp:DropDownList ID="ddlExternalTarget" runat="server"></asp:DropDownList>
                                </div>
                                 <div class="field">
                                    <asp:TextBox ID="txtExternalTooltip" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtExternalClass" runat="server"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                    </table>
            
                </fieldset>
                <fieldset class="media">
                    <legend>Media Link</legend>
                     <table>
                        <tr>
                            <td class="tv">
                                <cc1:LinqItTreeView ID="mediaTree" runat="server"></cc1:LinqItTreeView>
                            </td>
                            <td style="padding-left:10px;">
                                <div class="media-preview">
                                </div>
                                <table>
                                    <tr>
                                        <td class="labels" style="padding-left:0;">
                                            <div class="field">
                                                <asp:Label ID="lblMediaText" runat="server" AssociatedControlID="txtMediaText" Text="Link Text:"></asp:Label>
                                            </div>
                                            <div class="field">
                                                <asp:Label ID="lblMediaTarget" runat="server" AssociatedControlID="ddlMediaTarget" Text="Target:"></asp:Label>
                                            </div>
                                            <div class="field">
                                                <asp:Label ID="lblMediaTooltip" runat="server" AssociatedControlID="txtMediaTooltip" Text="Tooltip:"></asp:Label>
                                            </div>
                                            <div class="field">
                                                <asp:Label ID="lblMediaClass" runat="server" AssociatedControlID="txtMediaClass" Text="Css class"></asp:Label>
                                            </div>
                                        </td>
                                        <td>
                                            <div class="field">
                                                <asp:TextBox ID="txtMediaText" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="field">
                                                <asp:DropDownList ID="ddlMediaTarget" runat="server"></asp:DropDownList>
                                            </div>
                                             <div class="field">
                                                <asp:TextBox ID="txtMediaTooltip" runat="server"></asp:TextBox>
                                            </div>
                                            <div class="field">
                                                <asp:TextBox ID="txtMediaClass" runat="server"></asp:TextBox>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <fieldset class="mailto">
                    <legend>MailTo Link</legend>
                    <table>
                        <tr>
                            <td class="labels" style="padding-left:0;">
                                 <div class="field">
                                    <asp:Label ID="lblMailToText" runat="server" AssociatedControlID="txtMailToText" Text="Link Text:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblMailToPath" runat="server" AssociatedControlID="txtMailToPath" Text="Email address:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblMailToSubject" runat="server" AssociatedControlID="txtMailToSubject" Text="Subject:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblMailToTooltip" runat="server" AssociatedControlID="txtMailToTooltip" Text="Tooltip:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblMailToClass" runat="server" AssociatedControlID="txtMailToClass" Text="Css class"></asp:Label>
                                </div>
                            </td>
                            <td>
                                <div class="field">
                                    <asp:TextBox ID="txtMailToText" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtMailToPath" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtMailToSubject" runat="server"></asp:TextBox>
                                </div>
                                 <div class="field">
                                    <asp:TextBox ID="txtMailToTooltip" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtMailToClass" runat="server"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <fieldset class="javascript">
                    <legend>Javascript</legend>
                    <table>
                        <tr>
                            <td class="labels" style="padding-left:0;">
                                <div class="field">
                                    <asp:Label ID="lblJavascriptText" runat="server" AssociatedControlID="txtJavascriptText" Text="Link Text:"></asp:Label>
                                </div>
                                <div class="field multiline">
                                    <asp:Label ID="lblJavscriptCode" runat="server" AssociatedControlID="txtJavascriptCode" Text="Javascript:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblJavascriptTooltip" runat="server" AssociatedControlID="txtJavascriptTooltip" Text="Tooltip:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblJavascriptClass" runat="server" AssociatedControlID="txtJavascriptClass" Text="Css class"></asp:Label>
                                </div>
                            </td>
                            <td>
                                <div class="field">
                                    <asp:TextBox ID="txtJavascriptText" runat="server"></asp:TextBox>
                                </div>
                                <div class="field multiline">
                                    <asp:TextBox ID="txtJavascriptCode" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                 <div class="field">
                                    <asp:TextBox ID="txtJavascriptTooltip" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtJavascriptClass" runat="server"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <fieldset class="anchor">
                    <legend>Insert a link to an Anchor</legend>
                    <table>
                        <tr>
                            <td class="labels" style="padding-left:0;">
                                <div class="field">
                                    <asp:Label ID="lblAnchorText" runat="server" AssociatedControlID="txtAnchorText" Text="Link Text:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblAnchorTarget" runat="server" AssociatedControlID="txtAnchorTarget" Text="Anchor:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblAnchorTooltip" runat="server" AssociatedControlID="txtAnchorTooltip" Text="Tooltip:"></asp:Label>
                                </div>
                                <div class="field">
                                    <asp:Label ID="lblAnchorClass" runat="server" AssociatedControlID="txtAnchorClass" Text="Css class"></asp:Label>
                                </div>
                            </td>
                            <td>
                                <div class="field">
                                    <asp:TextBox ID="txtAnchorText" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtAnchorTarget" runat="server"></asp:TextBox>
                                </div>
                                 <div class="field">
                                    <asp:TextBox ID="txtAnchorTooltip" runat="server"></asp:TextBox>
                                </div>
                                <div class="field">
                                    <asp:TextBox ID="txtAnchorClass" runat="server"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                    </table>
                </fieldset>
                </td>
            </tr>
            <tr>
                <td style="text-align:right;">
                    <asp:Button ID="btnOk" runat="server" Text="Ok" UseSubmitBehavior="false" OnClientClick="linqit.linkeditor.updateValue(this); return false;" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" UseSubmitBehavior="false" OnClientClick="linqit.linkeditor.cancelUpdate(this); return false;" />
                </td>
            </tr>
        </table>
        
    </div>
    </div>
    <asp:HiddenField ID="hiddenValue" runat="server" />
</div>
