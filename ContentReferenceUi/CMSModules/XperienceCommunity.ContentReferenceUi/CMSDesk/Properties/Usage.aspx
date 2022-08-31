<%@ Page Language="C#" AutoEventWireup="true" Inherits="KenticoCommunity_ContentReferenceUi_CMSDesk_Properties_Usage"
    Theme="Default"  Codebehind="Usage.aspx.cs" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>


<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <h4>Content usage</h4>
    <asp:Label runat="server" Visible="false" ID="lblMessage"></asp:Label>

    <div style="max-width: 1000px;">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:UniGrid ID="gridDocuments" ShowActionsMenu="true" runat="server" ShortID="g" GridName="Usage.xml"
                    EnableViewState="true" DelayedReload="true" IsLiveSite="false" ExportFileName="cms_document" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>    
</asp:Content>
