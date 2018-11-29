<%@ Page Language="C#" AutoEventWireup="true" Inherits="EditOutlet" MasterPageFile="~/Contacts/MasterPage/MediaRelations.master" Codebehind="EditOutlet.aspx.cs" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>
<%@ Register TagPrefix="mr" TagName="EditOutlet" Src="~/Contacts/UserControls/AddEditCompany.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        
        <h1>Edit Outlet <asp:Literal runat="server" ID="companyNameDisplayLabel" /></h1>

        
        <mr:EditOutlet ID="editOutletControl" Mode="Edit" IsMediaOutlet="True" runat="server" />
        

            <asp:Literal runat="server" ID="ErrorLit" />
        

</asp:Content>