<%@ Page Language="C#" AutoEventWireup="true" Inherits="AddCompany" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" Codebehind="AddCompany.aspx.cs" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<%@ Register TagPrefix="mr" TagName="AddCompany" Src="~/Contacts/UserControls/AddEditCompany.ascx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <%--<mrcl:PermissionContainer ID="PermissionContainer1" runat="server" AccessLevel="Create" Section="MediaRelationsCompany">--%>

        <h1>Add Company <asp:Literal runat="server" ID="companyNameDisplayLabel" /></h1>
        
        
        <mr:AddCompany Mode="Add" IsMediaOutlet="False" runat="server" ID="addCompanyControl" />
        

    <%--</mrcl:PermissionContainer>--%>


</asp:Content>
