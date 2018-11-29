<%@ Page Language="C#" AutoEventWireup="true" Inherits="EditCompany" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" Codebehind="EditCompany.aspx.cs" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<%@ Register TagPrefix="mr" TagName="EditCompany" Src="~/Contacts/UserControls/AddEditCompany.ascx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server" ID="Content1">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    
        <h1>Edit Company <asp:Literal runat="server" ID="companyNameDisplayLabel" /></h1>

        <mr:EditCompany Mode="Edit" IsMediaOutlet="false" runat="server" ID="editCompanyControl" />

        <asp:Literal runat="server" ID="ErrorLit" />
        

</asp:Content>
