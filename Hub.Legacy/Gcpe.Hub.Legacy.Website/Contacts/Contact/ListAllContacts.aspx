<%@ Page Language="C#" AutoEventWireup="true" Inherits="Contact_ListAllContacts" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" Codebehind="ListAllContacts.aspx.cs" %>
<%@ Register TagPrefix="mr" TagName="Paginator" Src="~/Contacts/UserControls/Paginator.ascx" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <mrcl:PermissionContainer runat="server" Section="MediaRelationsContact" AccessLevel="Read">
        <mr:Paginator ID="PaginatorTop" Mode="Top" EntityType="Contacts" runat="server" />
        <table class="common-admin-table">
            <tr>
                <th>Name</th>
                <th>Action</th>
            </tr>
            <asp:Literal ID="ContactLit" runat="server" />
            <tr>
                <th>Name</th>
                <th>Action</th>
            </tr>
        </table>
        <mr:Paginator ID="PaginatorBottom" Mode="Bottom" runat="server" />
    </mrcl:PermissionContainer>
</asp:Content>