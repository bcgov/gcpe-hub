<%@ Page Language="C#" AutoEventWireup="true" Inherits="AddContact" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" Codebehind="AddContact.aspx.cs" %>
<%@ Register TagPrefix="mr" TagName="AddEditContact" src="~/Contacts/UserControls/AddEditContact.ascx" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <%--<mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsContact" AccessLevel="Create">--%>
        <h1>Add New Contact <asp:Literal runat="server" ID="contactNameDisplayLabel" /></h1>
        <mr:AddEditContact runat="server" />
    <%--</mrcl:PermissionContainer>--%>
</asp:Content>