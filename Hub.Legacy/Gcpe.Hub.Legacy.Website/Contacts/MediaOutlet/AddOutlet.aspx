<%@ Page Language="C#" AutoEventWireup="true" Inherits="AddOutlet" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" Codebehind="AddOutlet.aspx.cs" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<%@ Register TagPrefix="mr" TagName="AddOutlet" Src="~/Contacts/UserControls/AddEditCompany.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <%--<mrcl:PermissionContainer runat="server" AccessLevel="Create" Section="MediaRelationsCompany">--%>
        
        <h1>Add Outlet <asp:Literal runat="server" ID="companyNameDisplayLabel" /></h1>

        <mr:AddOutlet Mode="Add" IsMediaOutlet="True" runat="server" id="addOutletControl"/>
        

    <%--</mrcl:PermissionContainer>--%>
</asp:Content>