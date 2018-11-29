<%@ Page Language="C#" AutoEventWireup="true" Inherits="AdvancedSearch" MasterPageFile="~/Contacts/MasterPage/MediaRelationsResponsive.master" Codebehind="AdvancedSearch.aspx.cs" %>
<%@ Register TagPrefix="mr" TagName="AdvancedSearchControl" Src="~/Contacts/UserControls/AdvancedSearchControl.ascx" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>

<asp:Content ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
    
    div.bottom-filler-div {
        display: none;
    }

    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">

    <mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsSearch" AccessLevel="Read">

        <h1>Advanced Search</h1>
  
        <mr:AdvancedSearchControl runat="server" ID="advancedSearchControlDesktop" CancelButtonDisplayed="true"/>

    </mrcl:PermissionContainer>



</asp:Content>
