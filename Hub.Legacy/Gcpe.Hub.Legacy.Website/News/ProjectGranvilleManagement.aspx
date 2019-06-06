<%@ Page Title="Project Granville Management" Language="C#" MasterPageFile="~/News/Site.Master" AutoEventWireup="true" CodeBehind="ProjectGranvilleManagement.aspx.cs" Inherits="Gcpe.Hub.News.ProjectGranvilleManagement" %>
<%@ MasterType TypeName="Gcpe.Hub.News.Site" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="scriptsContentPlaceHolder" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="formContentPlaceHolder" runat="server">
    <h1>Project Granville Management</h1>
    <br />

    <div class="section">
        <table>
            <tr>
                <td style="width:30px;"></td>
                <td style="width:600px;">
                    <h2 id="enabled_Label" class="live-feed-label" runat="server">Enable Project Granville</h2>
                    <asp:Button ID="save_Button" runat="server" Text="Enable Project Granville" CssClass="primary"  OnClick="btnToggleProjectGranville" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

