<%@ Page Title="" Language="C#" MasterPageFile="~/News/Site.Master" AutoEventWireup="true" CodeBehind="EmergencySlideManagement.aspx.cs" Inherits="Gcpe.Hub.News.SlidePinningManagement" %>
<%@ MasterType TypeName="Gcpe.Hub.News.Site" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/bootstrap.min.js") %>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/News/ReleaseManagement/Scripts/ReleaseManagement.js") %>" type="text/javascript"></script>
    <link type="text/css" rel="stylesheet" href="<%= ResolveUrl("~/Content/bootstrap.min.css") %>" />
    <style>
        h1 {
            font-size: 1.8em;
            margin-bottom: 15px;
        }
        h1, h2 {
            font-family: Calibri;
            color: #444444;
            font-weight: bold;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="formContentPlaceHolder" runat="server" DataSource="<%# PinnedSlide %>" ItemType="Gcpe.Hub.Data.Entity.Slide">
    <div class="section">
        <h1 style="margin-top:0">
            <asp:Literal ID="ltrPageTitle" runat="server" Text="Emergency Pin Management"></asp:Literal>
        </h1>
    </div>
    <div class="section" style="margin-bottom: 20px;">

        <div class="carousel-column">
        <div>
            <div class="lbl" >Headline</div>
            <asp:TextBox id="txtHeadline" name="txtHeadline" runat="server" Width="400px" MaxLength="255" Text="<%# PinnedSlide!=null?PinnedSlide.Headline:string.Empty %>"></asp:TextBox>
        </div>
        <div>
            <div class="lbl">Summary</div>
            <asp:TextBox ID="txtSummary" runat="server" Width="400px" Height="61px" MaxLength="255" Text="<%#  PinnedSlide!=null ? PinnedSlide.Summary: string.Empty %>" TextMode="MultiLine"></asp:TextBox>
        </div>
    </div>
    <div class="carousel-column">
        <div class="lbl">Image</div>
        <div class="carousel-banner" style="background-image: url('CarouselImage.ashx?slideId=<%#PinnedSlide.Id!=Guid.Empty?PinnedSlide.Id.ToString():"" %>')">
            <div class='story <%# PinnedSlide.Justify == LeftJustify ? "left" : "right"%>'></div>
        </div>
        <div>
            <div class="lbl alignment">
                Slide Alignment
                <asp:RadioButton ID="RadioLeft" Text="Left" Checked="<%# PinnedSlide.Justify == LeftJustify %>" GroupName="RadioGroupAlignment" runat="server" />
                <asp:RadioButton ID="RadioRight" Text="Right" Checked="<%# PinnedSlide.Justify == RightJustify %>" GroupName="RadioGroupAlignment" runat="server" />
            </div>
        </div>
    </div>

    <div style="vertical-align: top; overflow: hidden">
        <input type="file" id="myFile" name="<%# PinnedSlide.Id %>" style="width: 70%" />
        <div class="pull-right">
            <asp:LinkButton id="UpdatePrimaryButton" CommandName="primayButton" onclick="btnUpdateEmergencySlide" CommandArgument='<%# PinnedSlide.Id %>' runat="server" style="margin-left:5px;margin-right:5px;"><i class="fa fa-floppy-o fa-lg" aria-hidden="true"></i></asp:LinkButton>
            <asp:LinkButton id="PinPrimaryButton" CommandName="primayButton" onclick="btnTogglePinnedSlide" CommandArgument='<%# PinnedSlide.Id %>' runat="server" style="margin-left:5px;margin-right:5px;"><i class="<%# IsPinnedSlide ? "fa fa-thumb-tack fa-lg pinned":"fa fa-thumb-tack fa-lg unpinned" %>" aria-hidden="true"></i></asp:LinkButton>
        </div>
        <div>
            <div class="lbl" style="margin-top: 10px">Action URL</div>
            <asp:TextBox ID="txtActionUrl" runat="server" Width="100%" MaxLength="255" Text="<%# PinnedSlide.ActionUrl %>"></asp:TextBox>
        </div>
        <div>
            <div class="lbl" >Facebook Post URL</div>
            <asp:TextBox ID="txtFacebookPostUrl" runat="server" Width="100%" MaxLength="255" Text="<%# PinnedSlide.FacebookPostUrl %>"></asp:TextBox>
        </div>
    </div>
    </div>
    <!--
    <div style="padding:10px; text-align:center;">
        <button onclick="btnSwap"><i class="fa fa-exchange fa-rotate-90 fa-4x" aria-hidden="true"></i></button>
    </div>
    -->
    <div class="section" style="margin-bottom: 20px;">
        <div class="carousel-column">
        <div>
            <div class="lbl" >Headline</div>
            <asp:TextBox id="txtSecondHeadline" runat="server" Width="400px" MaxLength="255" Text="<%# SecondarySlide!=null?SecondarySlide.Headline:string.Empty %>"></asp:TextBox>
        </div>
        <div>
            <div class="lbl">Summary</div>
            <asp:TextBox ID="txtSecondSummary" runat="server" Width="400px" Height="61px" MaxLength="255" Text="<%#  SecondarySlide!=null ? SecondarySlide.Summary: string.Empty %>" TextMode="MultiLine"></asp:TextBox>
        </div>
    </div>
    <div class="carousel-column">
        <div class="lbl">Image</div>
        <div class="carousel-banner" style="background-image: url('CarouselImage.ashx?slideId=<%#SecondarySlide.Id!=Guid.Empty?SecondarySlide.Id.ToString():"" %>')">
            <div class='story <%# SecondarySlide.Justify == LeftJustify ? "left" : "right"%>'></div>
        </div>
        <div>
            <div class="lbl alignment">
                Slide Alignment
                <asp:RadioButton ID="RadioButton1" Text="Left" Checked="<%# SecondarySlide.Justify == LeftJustify %>" GroupName="RadioGroupAlignment2" runat="server" />
                <asp:RadioButton ID="RadioButton2" Text="Right" Checked="<%# SecondarySlide.Justify == RightJustify %>" GroupName="RadioGroupAlignment2" runat="server" />
            </div>
        </div>
    </div>

    <div style="vertical-align: top; overflow: hidden">
        <input type="file" id="myFile2" name="<%# SecondarySlide.Id %>" style="width: 70%" />
        <div class="pull-right">
            <asp:LinkButton id="UpdateSecondaryButton" CommandName="secondaryButton" onclick="btnUpdateEmergencySlide" CommandArgument='<%# SecondarySlide.Id %>' runat="server" style="margin-left:5px;margin-right:5px;"><i class="fa fa-floppy-o fa-lg" aria-hidden="true"></i></asp:LinkButton>
            <asp:LinkButton id="PinSecondaryButton" CommandName="secondaryButton" onclick="btnTogglePinnedSlide" CommandArgument='<%# SecondarySlide.Id %>' runat="server" style="margin-left:5px;margin-right:5px;"><i class="<%# IsPinnedSecondarySlide ? "fa fa-thumb-tack fa-lg pinned":"fa fa-thumb-tack fa-lg unpinned" %>" aria-hidden="true"></i></asp:LinkButton>
        </div>
        <div>
            <div class="lbl" style="margin-top: 10px">Action URL</div>
            <asp:TextBox ID="txtSecondActionUrl" runat="server" Width="100%" MaxLength="255" Text="<%# SecondarySlide.ActionUrl %>"></asp:TextBox>
        </div>
        <div>
            <div class="lbl" >Facebook Post URL</div>
            <asp:TextBox ID="txtSecondFacebookPostUrl" runat="server" Width="100%" MaxLength="255" Text="<%# SecondarySlide.FacebookPostUrl %>"></asp:TextBox>
        </div>
    </div>
    </div>
    
</asp:Content>
