<%@ Page Language="C#" AutoEventWireup="true" Inherits="ViewOutlet" MasterPageFile="~/Contacts/MasterPage/MediaRelationsResponsive.master" Codebehind="ViewOutlet.aspx.cs" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<%@ Register TagPrefix="mr" TagName="ViewOutlet" Src="~/Contacts/UserControls/ViewCompany.ascx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <mrcl:PermissionContainer runat="server" AccessLevel="Read" Section="MediaRelationsCompany">
        <h1>View Outlet <asp:Literal runat="server" ID="companyNameDisplayLabel" /></h1>

        <mr:ViewOutlet ID="ViewOutlet1" runat="server" Mode="View" IsMediaOutlet="true"/>
    </mrcl:PermissionContainer>
</asp:Content>