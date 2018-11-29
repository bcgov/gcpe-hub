<%@ Page Title="LiveFeedManagement" Language="C#" MasterPageFile="~/News/Site.Master" AutoEventWireup="true" CodeBehind="LiveFeedManagement.aspx.cs" Inherits="Gcpe.Hub.News.LiveFeedManagement" %>
<%@ MasterType TypeName="Gcpe.Hub.News.Site" %>


<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="scriptsContentPlaceHolder" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="formContentPlaceHolder" runat="server">
    <h1>Live Feed Management</h1>
    <br />

    <div class="section">
        <table>
            <tr>
                <td style="width:30px;"></td>
                <td style="width:600px;">
                    <h2 id="enabled_Label" class="live-feed-label" runat="server" >Enable Live Feed</h2>
                    <asp:Button ID="save_Button" runat="server" Text="Enable Live Feed" CssClass="primary"  OnClick="btnToggleLiveFeed" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
