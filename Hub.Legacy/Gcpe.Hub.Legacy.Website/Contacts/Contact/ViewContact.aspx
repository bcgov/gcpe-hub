<%@ Page Language="C#" AutoEventWireup="true" Inherits="Contact_ViewContact" MasterPageFile="~/Contacts/MasterPage/MediaRelationsResponsive.master" Codebehind="ViewContact.aspx.cs" %>
<%@ Register TagPrefix="mr" TagName="ViewContact" Src="~/Contacts/UserControls/ViewContact.ascx" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server" >
    <mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsContact" AccessLevel="Read">
        <h1>View Contact <asp:Literal runat="server" ID="contactNameDisplayLabel" /></h1>

        <mr:ViewContact runat="server" />
    </mrcl:PermissionContainer>
</asp:Content>
