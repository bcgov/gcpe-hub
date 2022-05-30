<%@ Page Title="Carousel" Language="C#" MasterPageFile="~/News/Site.Master" AutoEventWireup="true" CodeBehind="Carousel.aspx.cs" Inherits="Gcpe.Hub.News.Carousel" %>

<%@ MasterType TypeName="Gcpe.Hub.News.Site" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/bootstrap.min.js") %>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/News/ReleaseManagement/Scripts/ReleaseManagement.js") %>" type="text/javascript"></script>
    <link type="text/css" rel="stylesheet" href="<%= ResolveUrl("~/Content/bootstrap.min.css") %>" />
    <style>
        h1 {
            margin-top: 15px;
            margin-left:15px;
            font-size: 1.8em;
            margin-bottom: 25px;
        }
        h1, h2 {
            font-family: Calibri;
            color: #444444;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="formContentPlaceHolder" runat="server">

    <div style="display: inline-block">
        <h1 style="margin-top:0">
            <asp:Literal ID="ltrPageTitle" runat="server" Text="Carousel Management"></asp:Literal>
        </h1>
    </div>
    <div style="display:inline-block;float:right">
        <span class="lbl" draggable="false" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)" style="line-height:30px">Publish Date</span>
        <asp:TextBox ID="publishDateTimePicker" runat="server" Text='<%#CarouselPublishDateTime.ToString("yyyy-MM-dd hh:mm tt") %>'></asp:TextBox>
        <button type="button" class="btn btn-default" runat="server" onserverclick="btnPrevious_Click" style="margin-left:10px">
          <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>
        </button>
        <button id="nextButton" ClientIDMode="Static" type="button" class="btn btn-default" runat="server" OnClick="if (!OnNextSlide())return false;" onserverclick="btnNext_Click">
          <span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>
        </button>
        <asp:Button ID="ButtonSave" runat="server" Text="Save" CssClass="primary" style="margin-left:20px" OnClick="btnSave_Click" />
    </div>
    <br />

    <!--set draggable to all labels to work around: https://connect.microsoft.com/IE/feedbackdetail/view/927470/ie-11-input-field-of-type-text-does-not-respond-to-mouse-clicks-when-ancestor-node-has-draggable-true -->
    <asp:Repeater ID="rptSlides" DataSource="<%# SlidesItems %>" runat="server" ItemType="Gcpe.Hub.Data.Entity.CarouselSlide" >
        <ItemTemplate>
            <asp:Panel runat="server" ID="drag" class="section" style='<%# Item.SortIndex<0 ? "border:2px solid rgb(216, 41, 47);margin-bottom:20px;padding:10px;padding-left:15px;":"margin-bottom:20px;" %>' ondrop="OnSlideDrop(event)" ondragover="OnDragOver(event)">
                <div style="<%# Item.SortIndex<0 ? "display:none; margin-bottom:20px;float:left;margin-top:50px":"margin-bottom:20px;float:left;margin-top:50px" %>" draggable="<%# Item.SortIndex>=0 %>" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)">
                    <span class='ui-icon ui-icon-arrowthick-2-n-s'></span>
                </div>
                <div class="carousel-column">
                    <div>
                        <div class="lbl" draggable="<%# Item.SortIndex>=0 %>" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)">Headline</div>
                        <asp:TextBox ID="txtHeadline" ReadOnly="<%# Item.SortIndex<0 %>" runat="server" Width="400px" MaxLength="255" Text="<%#Item.Slide.Headline %>"></asp:TextBox>
                    </div>
                    <div>
                        <div class="lbl" draggable="<%# Item.SortIndex>=0 %>" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)">Summary</div>
                        <asp:TextBox ReadOnly="<%# Item.SortIndex<0 %>" ID="txtSummary" runat="server"  Width="400px" Height="61px" MaxLength="255" Text='<%# Item.Slide.Summary %>' TextMode="MultiLine" ></asp:TextBox>
                    </div>
                </div>

                <div class="carousel-column">
                    <div class="lbl" draggable="<%# Item.SortIndex>=0 %>" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)">Image</div>
                    <div class="carousel-banner" draggable="<%# Item.SortIndex>=0 %>" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)" style="background-image: url('CarouselImage.ashx?slideId=<%#Item.Slide.Id %>')">
                        <div class='story <%# Item.Slide.Justify == LeftJustify ? "left" : "right"%>' ></div>
                    </div>
                    <div>
                        <div class="lbl alignment" draggable="<%# Item.SortIndex>=0 %>" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)">Slide Alignment
                        <asp:RadioButton Enabled="<%# Item.SortIndex>=0 %>" id="RadioLeft" Text="Left" Checked="<%# Item.Slide.Justify == LeftJustify %>" GroupName="RadioGroupAlignment" runat="server" />
                        <asp:RadioButton Enabled="<%# Item.SortIndex>=0 %>" id="RadioRight" Text="Right" Checked="<%# Item.Slide.Justify == RightJustify %>" GroupName="RadioGroupAlignment" runat="server" />
                        </div>
                    </div>
                </div>

                <div style="vertical-align:top;overflow:hidden">
                    <input type="file" id="myFile" name="<%# Item.Slide.Id %>" style="<%# Item.SortIndex>=0 ? "width:70%;" : "visibility:hidden"%>" />
           
                    <div>
                        <div class="lbl" style="margin-top:10px" draggable="<%# Item.SortIndex>=0 %>" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)">Action URL</div>
                        <asp:TextBox ReadOnly="<%# Item.SortIndex<0 %>" ID="txtActionUrl" runat="server" Width="100%" MaxLength="255" Text="<%# Item.Slide.ActionUrl %>" ></asp:TextBox>
                    </div>
                    <div>
                        <div class="lbl" draggable="<%# Item.SortIndex>=0 %>" ondragstart="OnDragStart(event)" ondragend="OnDragEnd(event)">Facebook Post URL</div>
                        <asp:TextBox ReadOnly="<%# Item.SortIndex<0 %>" ID="txtFacebookPostUrl" runat="server" Width="100%" MaxLength="255" Text="<%# Item.Slide.FacebookPostUrl %>" ></asp:TextBox>
                    </div>
                </div>

                <asp:HiddenField ID="sortIndex" runat="server" Value= "<%# Item.SortIndex %>"/>
            </asp:Panel>

        </ItemTemplate>
    </asp:Repeater>


    <div class="modal fade" id="confirm-add" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    Would you like to add a new Carousel ?
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
                    <button type="button" class="btn btn-ok" data-dismiss="modal">Add</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function OnSlideDrop(ev) {
            if (!draggedSource) return;
            ev.preventDefault();
            if (draggedSource.find("[id*='sortIndex']").val() > 0)
            {
                var target = $(ev.target).closest("[ondrop]");
                draggedSource.insertBefore(target);
            }
            else {
                console.log('false');
                return false;
            }
            
            //console.log(draggedSource.find("[id*='sortIndex']").val());

            // renumber the sortIndexs
            target.parent().find("[id*='sortIndex']").each(function (key, control) {
                if (control.value >= 0) {
                    control.value = key;
                }
                
               //console.log(key);
            });
        }
        $('#<%= publishDateTimePicker.ClientID %>').kendoDateTimePicker({
            value: '',
            format: "yyyy-MM-dd hh:mm tt",
            change: function(e){
            }
        });

        var confirmed = false;
        function OnNextSlide() {
            <% if (isLastCarousel)
            {%>
            if (!confirmed) {
                var confirmAdd = $("#confirm-add");
                confirmAdd.modal('show');
                $(".btn-ok").click(function () {
                    confirmAdd.modal('hide');
                    confirmed = true;
                    $("#nextButton").click();
                });
                return false;
            }
            <%} %>
            return true;
        }
    </script>

</asp:Content>
