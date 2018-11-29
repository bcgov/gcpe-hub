<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReleaseImagePicker.ascx.cs" Inherits="Gcpe.Hub.News.ReleaseManagement.Controls.ReleaseImagePicker" %>

<asp:Panel runat="server" Visible="<%# IsReadOnly %>">

    <asp:Image ID="imgDocumentHeader" CssClass="page-header-image" runat="server" ImageUrl="<%# SelectedImageUrl %>" Height="<%# ReleaseImageHeight %>" />

</asp:Panel>
<asp:Panel runat="server" Visible="<%# !IsReadOnly %>">

    <asp:HiddenField runat="server" ID="hdPageImage" Value='<%# Value.HasValue ? Value.Value.ToString() : "" %>' />

    <div class="ImageSelected">
        <div class="txt">
            <asp:Image ID="imgSelected" runat="server" ImageUrl="<%# SelectedImageUrl %>" Height="<%# ReleaseImageHeight %>" />
        </div>
        <a class="ChangeImage" href="#">Change</a>
    </div>

    <div class="ImageChoice">
        <img class="image-option" id="" src="<%# GetReleaseImageClientUrl() %>" />

        <asp:Repeater runat="server" ID="rptImages" DataSource="<%# Images %>" ItemType="System.Collections.Generic.KeyValuePair<Guid, string>">
            <ItemTemplate>
                <img id="<%# Item.Key %>" src="<%# GetReleaseImageClientUrl(Item.Key) %>" title="<%# Item.Value %>" height="<%# ReleaseImageHeight.ToString() + "px" %>" />
            </ItemTemplate>
        </asp:Repeater>
    </div>

</asp:Panel>