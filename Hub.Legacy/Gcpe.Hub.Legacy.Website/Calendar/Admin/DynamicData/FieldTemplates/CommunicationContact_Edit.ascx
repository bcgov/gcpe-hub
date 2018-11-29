<%@ Control Language="C#" CodeBehind="CommunicationContact_Edit.ascx.cs" Inherits="CorporateCalendarAdmin.CommunicationContact_EditField" %>

<asp:DropDownList ID="DropDownList1" runat="server" CssClass="DDDropDown CC">
</asp:DropDownList>

<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" CssClass="DDControl DDValidator" ControlToValidate="DropDownList1" Display="Static" Enabled="false" />
<asp:DynamicValidator runat="server" ID="DynamicValidator1" CssClass="DDControl DDValidator" ControlToValidate="DropDownList1" Display="Static" />

