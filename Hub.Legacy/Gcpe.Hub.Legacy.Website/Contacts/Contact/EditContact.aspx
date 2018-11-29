<%@ Page Language="C#" AutoEventWireup="true" Inherits="EditContact" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" Codebehind="EditContact.aspx.cs" %>
<%@ Register TagPrefix="mr" TagName="AddEditContact" src="~/Contacts/UserControls/AddEditContact.ascx" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
   <%-- <mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsContact" AccessLevel="Update">--%>
        <h1>Edit Contact <asp:Literal runat="server" ID="contactNameDisplayLabel" /></h1>
        <asp:Literal ID="ErrorLit" runat="server" />
        
        <mr:AddEditContact ID="AddEditContactControl" Mode="Edit" runat="server" />
   <%-- </mrcl:PermissionContainer>--%>
</asp:Content>