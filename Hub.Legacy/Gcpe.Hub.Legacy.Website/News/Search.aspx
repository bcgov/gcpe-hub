<%@ Page Title="" Language="C#" MasterPageFile="~/News/Site.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Gcpe.Hub.News.Search" %>

<%@ MasterType TypeName="Gcpe.Hub.News.Site" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContentPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="formContentPlaceHolder" runat="server">

    <script>

        var warning_format = " * The date entered is invalid";
        var warning_required = " * This is required, enter a value.";
        var warning_range = " * Enter a date after the start date";




        function pageLoad() {

            // Performs the validation, cancels updatepanel postback if not valid
            Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(InitializeNewPageRequest);

            $(".filter-title").click(function () {
                if ($(this).next(".filter-options").is(":visible")) {
                    $(this).next(".filter-options").hide();
                    $(this).removeClass("filter-title-expanded");
                    $(this).addClass("filter-title-collapsed");
                } else {
                    $(this).next(".filter-options").show();
                    $(this).addClass("filter-title-expanded");
                    $(this).removeClass("filter-title-collapsed");
                }
            });

    <%--        $("#<% =chkCustomDateRange.ClientID %>").click(function () {
                $("#DateRange").toggle();
            });--%>

        }

        function InitializeNewPageRequest(sender, args) {

           <%-- var SubmitButtonID = "<% = btnAdd.ClientID %>";
            var currentButtonClickedID = args.get_postBackElement().id.toLowerCase();

            if (currentButtonClickedID == SubmitButtonID.toLowerCase()) {
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                if (!IsDatesValid()) {
                    prm.abortPostBack();
                    args.set_cancel(true);
                }
            }--%>
        }



        /* Validates the Date Format */
<%--        var startDateIsValid = true;
        function StartDatePicker_onDateSelected(sender, args) {
            $('#<%= startDatePicker.ClientID %>').parents('div .field-group').removeClass('warning');
            $("#<% = lblFromDateError.ClientID %>").text("");
            startDateIsValid = true;
        }
        function StartDatePicker_onBlur(sender, args) {
            $('#<%= startDatePicker.ClientID %>').parents('div .field-group').removeClass('warning');
            $("#<% = lblFromDateError.ClientID %>").text("");
            if (sender.get_textBoxValue().trim() == "") {
                $('#<%= startDatePicker.ClientID %>').parents('div .field-group').addClass('warning');
                $("#<% = lblFromDateError.ClientID %>").text(warning_required);
            }              
        }
        function StartDatePicker_onFocus(sender, args) {
            startDateIsValid = true; // resets to true, this fires before onDataError
        }
        function StartDatePicker_onDataError(sender, args) {
            $('#<%= startDatePicker.ClientID %>').parents('div .field-group').addClass('warning');
            $("#<% = lblFromDateError.ClientID %>").text(warning_format);
            startDateIsValid = false;
        }


        var endDateIsValid = true;
        function EndDatePicker_onDateSelected(sender, args) {
            $('#<%= endDatePicker.ClientID %>').parents('div .field-group').removeClass('warning');
            $("#<% = lblToDateError.ClientID %>").text("");
            endDateIsValid = true;
        }
        function EndDatePicker_onBlur(sender, args) {
            $('#<%= endDatePicker.ClientID %>').parents('div .field-group').removeClass('warning');
            $("#<% = lblToDateError.ClientID %>").text("");
            if (sender.get_textBoxValue().trim() == "") {
                $('#<%= endDatePicker.ClientID %>').parents('div .field-group').addClass('warning');
                $("#<% = lblToDateError.ClientID %>").text(warning_required);
            }
                
        }
        function EndDatePicker_onFocus(sender, args) {
            endDateIsValid = true; // resets to true, this fires before onDataError
        }
        function EndDatePicker_onDataError(sender, args) {
            $('#<%= endDatePicker.ClientID %>').parents('div .field-group').addClass('warning');
            $("#<% = lblToDateError.ClientID %>").text(warning_format);
            endDateIsValid = false;
        }--%>




        function IsDatesValid() {

     <%--       var firstError;
            var startDatePicker = $find("<%= startDatePicker.ClientID %>");

            if (startDatePicker.isEmpty()) {
                $('#<%= startDatePicker.ClientID %>').parents('div .field-group').addClass('warning');
                $("#<% = lblFromDateError.ClientID %>").text(warning_required);
                $("#<% = lblFromDateError.ClientID %>").show();
                if (firstError == null) {
                    firstError = $("#<%= startDatePicker.ClientID %>");
                }
            } else if (!startDateIsValid) {
                $('#<%= startDatePicker.ClientID %>').parents('div .field-group').addClass('warning');
                $("#<% = lblFromDateError.ClientID %>").text(warning_format);
                $("#<% = lblFromDateError.ClientID %>").show();
                if (firstError == null) {
                    firstError = $("#<%= startDatePicker.ClientID %>");
                }
            }

            var endDatePicker = $find("<%= endDatePicker.ClientID %>");

            if (endDatePicker.isEmpty()) {
                $('#<%= endDatePicker.ClientID %>').parents('div .field-group').addClass('warning');
                $("#<% = lblToDateError.ClientID %>").text(warning_required);
                $("#<% = lblToDateError.ClientID %>").show();
                if (firstError == null) {
                    firstError = $("#<%= endDatePicker.ClientID %>");
                }
            } else if (!endDateIsValid) {
                $('#<%= endDatePicker.ClientID %>').parents('div .field-group').addClass('warning');
                $("#<% = lblToDateError.ClientID %>").text(warning_format);
                $("#<% = lblToDateError.ClientID %>").show();
                if (firstError == null) {
                    firstError = $("#<%= endDatePicker.ClientID %>");
                }
            }


            if (firstError == null) {
                // No error so far, compare dates
                if (startDatePicker.get_selectedDate() > endDatePicker.get_selectedDate()) {

                    $('#<%= endDatePicker.ClientID %>').parents('div .field-group').addClass('warning');
                    $("#<% = lblToDateError.ClientID %>").text(warning_range);
                    $("#<% = lblToDateError.ClientID %>").show();

                    if (firstError == null) {
                        firstError = $("#<%= endDatePicker.ClientID %>");
                    }
                }
            }--%>

            //var date = datePicker.get_selectedDate();

            //var isEmpty = datePicker.isEmpty();


            if (firstError != null) {
                //$('html, body').animate({ scrollTop: firstError.offset().top - 50 }, 500);// scroll to the position
                firstError.find(":input").focus(); // focus on field               
                return false;
            } else {
                return true;
            }

        }



    </script>



    <div style="background-color: white; padding: 20px 0px 20px 15px; margin-bottom: 20px;">

        <table>
            <tr>
                <td style="width: 190px;">
                    <h1 style="padding-bottom: 0px; margin-bottom: 0px;" class="search-icon">Search the Hub</h1>
                </td>
                <td style="padding-right: 5px">
                    <asp:TextBox ID="txtKeyword" runat="server" CssClass="big-input" Width="450px"></asp:TextBox></td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="search-button primary" OnClick="btnSearch_Click" /></td>
            </tr>
        </table>

    </div>

    <asp:Panel runat="server" ID="pnlErrors" CssClass="section-error" Visible="false">
        <h2>Sorry, but there was an error</h2>
        <asp:Repeater runat="server" ID="rptErrors">
            <HeaderTemplate>
                <ul>
            </HeaderTemplate>
            <ItemTemplate>
                <li><%# Container.DataItem %></li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlResults" Visible="<%# Model.CountResults() > 0 || Model.FilterByMinistries.Any() || Model.FilterBySectors.Any() || Model.FilterByStatus.HasValue || Model.FilterByEndDate.HasValue || Model.FilterByEndDate.HasValue || Model.FilterByDateRange.HasValue %>">

        <div id="two-column">

            <div id="left-menu">


                <asp:Panel runat="server" ID="pnlFiltersSelected" Visible="<%# Model.FilterByMinistries.Any() || Model.FilterBySectors.Any() || Model.FilterByStatus.HasValue || Model.FilterByEndDate.HasValue || Model.FilterByEndDate.HasValue || Model.FilterByDateRange.HasValue %>">


                    <div style="background-color: #eee; padding: 8px 5px; margin-bottom: 10px;">

                        <div style="padding-bottom: 4px; font-weight: bold; border-bottom: 1px solid #d6d6d6;">Filtering by</div>



                        <asp:Panel runat="server" ID="pnlStatuses" DataSource="<%# Model.FilterByStatus %>" Visible="<%# Model.FilterByStatus.HasValue %>">
                            <div style="padding-bottom: 2px; font-size: 0.9em; margin-top: 5px;">Status</div>
                            <div style="padding: 3px 0px; display: inline-block;">
                                <asp:Button ID="btnRemoveStatus" runat="server" ToolTip="Remove filter" CssClass="search-filter-remove" OnClick="btnRemoveStatus_Click" /></div>
                            <div style="display: inline-block;">
                                <asp:Literal runat="server" ID="Literal1" Mode="Encode" Text="<%# GetStatusText(Model.FilterByStatus) %>"></asp:Literal></div>
                        </asp:Panel>


                        <asp:Repeater runat="server" ID="rptMinistryFilters" DataSource="<%# Model.FilterByMinistries %>" OnItemCommand="rptMinistryFilters_ItemCommand" Visible="<%# Model.FilterByMinistries.Any() %>">
                            <HeaderTemplate>
                                <div style="padding-bottom: 2px; font-size: 0.9em; margin-top: 5px;">Ministry</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="display: block; padding: 3px 0px;">
                                    <div style="display: inline-block; width: 15px; vertical-align: top;">
                                        <asp:Button ID="btnRemove" runat="server" CommandArgument="<%# ((KeyValuePair<Guid,string>)Container.DataItem).Key %>" ToolTip="Remove filter" CssClass="search-filter-remove" /></div>
                                    <div style="display: inline-block; width: 200px;">
                                        <asp:Literal runat="server" ID="ltrName" Mode="Encode" Text="<%# ((KeyValuePair<Guid,string>)Container.DataItem).Value %>"></asp:Literal></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <asp:Panel runat="server" ID="pnlDatefilters" Visible="<%# Model.FilterByStartDate.HasValue || Model.FilterByEndDate.HasValue %>">
                            <div style="padding-bottom: 2px; font-size: 0.9em; margin-top: 5px;">Date</div>
                            <div style="display: block; padding: 3px 0px;">
                                <div style="display: inline-block; width: 15px; vertical-align: top;">
                                    <asp:Button ID="btnRemoveDateFilter" runat="server" ToolTip="Remove filter" CssClass="search-filter-remove" OnClick="btnRemoveDateFilter_Click" /></div>
                                <div style="display: inline-block; width: 200px;"><%# (Model.FilterByEndDate.HasValue && Model.FilterByEndDate.HasValue) ? Model.FilterByStartDate.Value.ToString("MMM d, yyyy") + " to " + Model.FilterByEndDate.Value.ToString("MMM d, yyyy") : "" %></div>
                            </div>
                        </asp:Panel>


                        <asp:Repeater runat="server" ID="rptSectorFilters" DataSource="<%# Model.FilterBySectors %>" OnItemCommand="rptSectorFilters_ItemCommand" Visible="<%# Model.FilterBySectors.Any() %>">
                            <HeaderTemplate>
                                <div style="padding-bottom: 2px; font-size: 0.9em; margin-top: 5px;">Sector</div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="display: block; padding: 3px 0px;">
                                    <div style="display: inline-block; width: 15px; vertical-align: top;">
                                        <asp:Button ID="btnRemove" runat="server" CommandArgument="<%# ((KeyValuePair<Guid,string>)Container.DataItem).Key %>" ToolTip="Remove filter" CssClass="search-filter-remove" /></div>
                                    <div style="display: inline-block; width: 200px;">
                                        <asp:Literal runat="server" ID="ltrName" Mode="Encode" Text="<%# ((KeyValuePair<Guid,string>)Container.DataItem).Value %>"></asp:Literal></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <asp:Panel runat="server" ID="pnlDateRangeFilters" DataSource="<%# Model.FilterByDateRange %>" Visible="<%# Model.FilterByDateRange.HasValue %>">
                            <div style="padding-bottom: 2px; font-size: 0.9em; margin-top: 5px;">Date</div>
                            <div style="display: block; padding: 3px 0px;">
                                <div style="display: inline-block; width: 15px; vertical-align: top;">
                                    <asp:Button ID="btnRemoveDateRangeFilter" runat="server" ToolTip="Remove filter" CssClass="search-filter-remove" OnClick="btnRemoveDateRangeFilter_Click" /></div>
                                <div style="display: inline-block; width: 200px;">
                                    <asp:Literal runat="server" ID="ltrName" Mode="Encode" Text="<%# GetDateRangeText(Model.FilterByDateRange) %>"></asp:Literal></div>
                            </div>
                        </asp:Panel>

                    </div>



                </asp:Panel>

                <asp:Panel runat="server" ID="pnlStatusFilter" CssClass="filter" Visible="<%# false && !Model.FilterByStatus.HasValue && Model.CountResults() > 0 %>">
                    <div class="filter-title filter-title-expanded">Status</div>
                    <div class="filter-options">
                        <asp:CheckBoxList ID="chklstStatus" runat="server" DataSource="<%# Model.Statuses %>" DataValueField="Key" DataTextField="Value" CssClass="search-chklst" AutoPostBack="true" OnSelectedIndexChanged="chklstStatus_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </div>
                </asp:Panel>


                <asp:Panel runat="server" ID="pnlMinistryFilter" CssClass="filter" Visible="<%# Model.Ministries.Any() %>">
                    <div class="filter-title filter-title-expanded">Ministry</div>
                    <div class="filter-options">
                        <asp:CheckBoxList ID="chklstMinistries" runat="server" DataSource="<%# Model.Ministries %>" CssClass="search-chklst" DataValueField="Value" DataTextField="Text" AutoPostBack="true" OnSelectedIndexChanged="chklstMinistries_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </div>
                </asp:Panel>


                <asp:Panel runat="server" ID="pnlSectorFilter" CssClass="filter" Visible="<%# Model.Sectors.Any() %>">
                    <div class="filter-title filter-title-expanded">Sectors</div>
                    <div class="filter-options">
                        <asp:CheckBoxList ID="chklstSectors" runat="server" DataSource="<%# Model.Sectors %>" CssClass="search-chklst" DataValueField="Value" DataTextField="Text" AutoPostBack="true" OnSelectedIndexChanged="chklstSectors_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </div>
                </asp:Panel>

                <%--  <asp:Panel runat="server" ID="pnlDateFilter" CssClass="filter" Visible="<%# false && !Model.FilterByStartDate.HasValue && !Model.FilterByEndDate.HasValue && !Model.FilterByDateRange.HasValue && Model.CountResults() > 0 %>">
                <div class="filter-title filter-title-expanded">Date</div>
                <div class="filter-options">

                    <asp:CheckBoxList runat="server" ID="chklstDatePresets" DataSource="<%# Model.DatePresets %>" DataValueField="Key" DataTextField="Value" CssClass="search-chklst" AutoPostBack="true" OnSelectedIndexChanged="chklstDatePresets_SelectedIndexChanged">
                    </asp:CheckBoxList>
                    <asp:CheckBox runat="server" ID="chkCustomDateRange" Text="Custom range" CssClass="search-chklst" />

                    <div id="DateRange" style="display:none;">


                        <div class="field-group" style="width:100%;">
                            <div style="padding-top:5px;">From <asp:Label runat="server" ID="lblFromDateError" CssClass="info hidden"></asp:Label></div>
                            <telerik:RadDatePicker ID="startDatePicker" Width="150" Height="25"  runat="server" SelectedDate="<%# Model.FilterByStartDate %>" >
                                <DateInput runat="server" DisplayDateFormat="<%# ShortDateEnterFormat %>" DateFormat="<%# ShortDateEnterFormat %>">
                                    <ClientEvents OnBlur="StartDatePicker_onBlur" OnError="StartDatePicker_onDataError" OnFocus="StartDatePicker_onFocus" />
                                </DateInput>
                                <ClientEvents OnDateSelected="StartDatePicker_onDateSelected" />
                                <Calendar runat="server" ShowRowHeaders="false"></Calendar>
                                <Calendar runat="server">  
                                    <SpecialDays>
                                        <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="#cde6f7"/>   
                                    </SpecialDays>  
                                </Calendar>
                            </telerik:RadDatePicker>        
                        </div>

                        <div class="field-group" style="width:100%;">
                            <div style="padding-top:5px;">To <asp:Label runat="server" ID="lblToDateError" CssClass="info hidden"></asp:Label></div>
                            <telerik:RadDatePicker ID="endDatePicker" Width="150" Height="25"  runat="server" SelectedDate="<%# Model.FilterByEndDate %>">
                                <DateInput runat="server" DisplayDateFormat="<%# ShortDateEnterFormat %>" DateFormat="<%# ShortDateEnterFormat %>">
                                    <ClientEvents OnBlur="EndDatePicker_onBlur" OnError="EndDatePicker_onDataError" OnFocus="EndDatePicker_onFocus" />
                                </DateInput>
                                <ClientEvents OnDateSelected="EndDatePicker_onDateSelected" /> 
                                <Calendar runat="server" ShowRowHeaders="false"></Calendar>
                                <Calendar runat="server">  
                                    <SpecialDays>
                                        <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="#cde6f7"/>   
                                    </SpecialDays>  
                                </Calendar>
                            </telerik:RadDatePicker>
                        </div>



                    <div style="padding-top:10px;margin-left:40px;">
                        <asp:Button runat="server" ID="btnAdd" Text="Add Filter" CssClass="search-button secondary" OnClick="btnAdd_Click"   />
                    </div>

                    </div>

                        
                </div>  
            </asp:Panel>--%>
            </div>
            <div id="main-content">


                <div style="margin-left: 30px;">

                    <div class="search-nbr-records"><%# Model.CountResults() %> matches found</div>

                    <asp:ObjectDataSource ID="listViewDataSource" EnablePaging="true" runat="server" TypeName="Gcpe.Hub.News.SearchDataSource" SelectMethod="GetResults" SelectCountMethod="GetResultsCount" OnObjectCreated="listViewDataSource_ObjectCreated" />

                    <asp:ListView ID="ListView1" runat="server" DataSourceID="listViewDataSource" ItemType="Gcpe.Hub.News.SearchModel+SearchResult">

                        <LayoutTemplate>
                            <asp:Panel runat="server" ID="itemPlaceholder" />
                            <br />
                            <div style="text-align: center">
                                <asp:DataPager runat="server" PageSize="20">
                                    <Fields>
                                        <asp:NumericPagerField ButtonCount="25" PreviousPageText=" < " NextPageText=" > " NumericButtonCssClass="numeric_button" CurrentPageLabelCssClass="current_page" />
                                    </Fields>
                                </asp:DataPager>
                            </div>
                        </LayoutTemplate>

                        <ItemTemplate>
                            <div class="search-result">
                                <a href="<%# ((Gcpe.Hub.News.SearchModel.SearchResult)Container.DataItem).Url %>"><%# ((Gcpe.Hub.News.SearchModel.SearchResult)Container.DataItem).Title %></a>
                                <div class="search-result-info">
                                    <div style="display: inline-block"><%# ((Gcpe.Hub.News.SearchModel.SearchResult)Container.DataItem).Source %>  | </div>
                                    <div style="display: inline-block">
                                        <%# ((Gcpe.Hub.News.SearchModel.SearchResult)Container.DataItem).Date.HasValue ? ((Gcpe.Hub.News.SearchModel.SearchResult)Container.DataItem).Date.Value.ToString("MMMM dd, yyyy") : (((Gcpe.Hub.News.SearchModel.SearchResult)Container.DataItem).IsCommitted ? "Published" : "Draft") %>
                                    </div>
                                </div>
                                <div class="search-result-desc"><%# ((Gcpe.Hub.News.SearchModel.SearchResult)Container.DataItem).Description %></div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>


                </div>

            </div>


        </div>


    </asp:Panel>







</asp:Content>
