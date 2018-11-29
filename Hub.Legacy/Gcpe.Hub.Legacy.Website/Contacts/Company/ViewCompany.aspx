<%@ Page Language="C#" AutoEventWireup="true" Inherits="ViewCompany" MasterPageFile="~/Contacts/MasterPage/MediaRelationsResponsive.master" Codebehind="ViewCompany.aspx.cs" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<%@ Register TagPrefix="mr" TagName="ViewCompany" Src="~/Contacts/UserControls/ViewCompany.ascx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">

    <mrcl:PermissionContainer runat="server" AccessLevel="Read" Section="MediaRelationsCompany">
         <h1>View Company <asp:Literal runat="server" ID="companyNameDisplayLabel" /></h1>

        <mr:ViewCompany ID="ViewOutlet1" runat="server" Mode="View" IsMediaOutlet="false"/>
    </mrcl:PermissionContainer>


</asp:Content>
