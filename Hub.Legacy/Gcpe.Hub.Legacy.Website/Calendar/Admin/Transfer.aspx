<%@ Page Title="" Language="C#" MasterPageFile="~/Calendar/Admin/Site.master" AutoEventWireup="true"
    CodeBehind="Transfer.aspx.cs" Inherits="CorporateCalendarAdmin.Transfer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="DDSubHeader">
        Activity Transfer
    </div>
   <div style="color: #666;font: .8em Tahoma, Arial, Sans-Serif;">
        <p>To transfer all the activities from one communication contact to another:<br /><br />
            1. Choose the "from" contact on the left<br />
            2. Choose the "to" contact on the right<br />
            3. Click transfer. <br /><br />
        You will be prompted to confirm.</p>
        <p>
            <asp:Label ID="Label1" runat="server" Text="From:"></asp:Label>
            <asp:DropDownList ID="CommunicationContactFromDropDownList" CssClass="DDDropDown"
                runat="server">
            </asp:DropDownList>
            <asp:Label ID="Label2" runat="server" Text="To:"></asp:Label>
            <asp:DropDownList ID="CommunicationContactToDropDownList" CssClass="DDDropDown" runat="server">
            </asp:DropDownList>
            <asp:Button ID="Button1" runat="server" Text="Transfer" OnClientClick="javascript:return confirm('Are you sure that you want to transfer these activities?');"
                OnClick="Button1_Click" />
        </p>
    </div>
    <p>
        <asp:Label ID="Label3" runat="server"></asp:Label>
    </p>
  
</asp:Content>
