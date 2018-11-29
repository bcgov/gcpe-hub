<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Site.master" CodeBehind="UserList.aspx.cs" Inherits="CorporateCalendarAdmin.UserList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
    <a id="HyperLink1"   style="color: #666;font: .7em Tahoma, Arial, Sans-Serif;"  href="User.aspx?user=new">New User</a>
    </div>
    <br />
    <div class="DDSubHeader">
        Update Users
    </div>
    <div style="color: #666;font: .7em Tahoma, Arial, Sans-Serif;">
        <asp:PlaceHolder ID="SystemUserTablePlaceHolder" runat="server"></asp:PlaceHolder>
    </div>
    </asp:Content>