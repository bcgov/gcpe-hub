<%@ Page Language="C#" AutoEventWireup="true" Inherits="Search" MasterPageFile="~/Contacts/MasterPage/MediaRelationsResponsive.master" CodeBehind="Search.aspx.cs" ClientIDMode="Static" %>

<%@ Register TagPrefix="mr" TagName="AdvancedSearchControl" Src="~/Contacts/UserControls/AdvancedSearchControl.ascx" %>
<%@ Register TagPrefix="mr" TagName="TabControl" Src="~/Contacts/UserControls/TabControl.ascx" %>
<%@ Register TagPrefix="mr" TagName="Paginator" Src="~/Contacts/UserControls/Paginator.ascx" %>
<%@ Register TagPrefix="mr" TagName="SortColumnHeader" Src="~/Contacts/UserControls/SortColumnHeader.ascx" %>
<%@ Register TagPrefix="mrcl" Namespace="MediaRelationsLibrary" Assembly="MediaRelationsLibrary" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder" runat="server">
    <mrcl:PermissionContainer ID="PermissionContainer1" runat="server" Section="MediaRelationsSearch" AccessLevel="Read">

        <h1>Search Results</h1>

        <div class="desktop">
            <div class="noprint">
                <mr:AdvancedSearchControl runat="server" ID="advancedSearchControlDesktop" />
            </div>
            <div class="search-search-criteria">
                <span runat="server" id="searchCount"></span><span runat="server" id="searchCriteria"></span>
            </div>
            
        </div>

        <div class="phone phone-search-top">
            <a href="#" class="save-report-button noprint" data-toggle="modal" data-target="#saveReportDialogBox">Save As Report</a>

            <a href="" runat="server" id="advancedSearchHref" class="refine-results-link right-chevron">Refine Results</a>
            <div class="clear"></div>

            <p><asp:Literal runat="server" ID="searchCriteriaLit" /></p>
        </div>

        <mr:TabControl CssClass="search-page-tabs" ID="tabControl" runat="server" IsAddScreen="false" ForPhone="True" />

        <div class="desktop noprint" style="position: relative">
            <div class="common-page-tabs-right-container">
                <asp:LinkButton title="E-mail search results to me" runat="server" OnClick="ShareButton_Click" class="share-button">SHARE</asp:LinkButton>
                <asp:HyperLink title="E-mail all of the media contacts listed below" CssClass="email-button" runat="server" ID="emailHrefLink" Text="EMAIL" />
                <a href="javascript:doPrint()" title="Print this report" onclick='LogPrint();' class="print-button">PRINT</a>
                <asp:LinkButton runat="server" title="Export this info to Microsoft Excel" CssClass="export-button" Text="EXPORT" OnClick="ExportButton_Click" />
                <div class="clear"></div>
            </div>
        </div>

        <div id="saveReportDialogBox" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content" style="padding: 10px"">
                    <div class="modal-header">SAVE AS REPORT</div>
                    <asp:TextBox runat="server" ID="reportTitleNameTb" CssClass="ignore-unload" />

                    <div class="report-type-container" id="reportTypeContainer" runat="server" visible="false">
                        <asp:RadioButton runat="server" ID="privateReportRb" Checked="true" GroupName="publicRbGrp" />Private
                        <asp:RadioButton runat="server" ID="publicReportRb" GroupName="publicRbGrp" />Public
                    </div>

                    <script type="text/javascript">
                        var element = document.getElementById('reportTypeContainer');
                        if (element !== null) {
                            var elements = element.getElementsByTagName("input");
                            for (var i = 0; i < elements.length; i++) {
                                elements[i].className = "ignore-unload";
                            }
                        }
                    </script>

                    <div class="buttonContainer">
                        <asp:LinkButton ID="cancelButtonFb" runat="server" data-toggle="modal" data-target="#saveReportDialogBox" Text="CANCEL" CssClass="common-admin-link-button first" />
                        <asp:LinkButton ID="SaveReportButton" OnClick="SaveReport_Click" runat="server" Text="SAVE" CssClass="common-admin-link-button" OnClientClick="return ConfirmSaveReport();" />
                    </div>

                </div>
            </div>
        </div>

        <script type="text/javascript">
            //<asp:Literal ID="ShareCountLit" runat="server"/>
            var shareLinkHref = "";
            var emailLinkHref = "";

            function doShare() {
                window.location = shareLinkHref;
            }
            function doEmail() {
                window.location = emailLinkHref;
            }
            function emailLinkClick() {

                if (confirm(emailButtonText)) {
                    LogEmail();
                    return true;
                } return false;
            }
            function ConfirmSaveReport() {

                var element = document.getElementById('reportTitleNameTb');
                if (element.value.trim() === "") {
                    alert(reportTitleTextEmpty);
                    return false;
                }

                var isPublic = false;
                if (document.getElementById('reportTypeContainer') != null) {
                    if (document.getElementById('publicReportRb').checked) isPublic = true;
                }

                var requestUrl = "ajax/ReportTitles.ashx?reportTitle=" + escape(element.value) + "&reportType=" + (isPublic ? "public" : "private");

                var d = new Date();
                requestUrl += "&time=" + d.getTime();

                var request = GetHttpRequestObj();
                request.open("GET", requestUrl, false);
                request.send();

                if (request.responseText === "0") {
                    // title in use
                    alert(reportTitleUsedText);
                    return false;
                } else if (request.responseText !== "1") {
                    // unknown error
                    alert(errorOccurredText);
                    return false;
                }
                // valid title
                return true;
            }
        </script>
        <div id="debugdiv"></div>
        <div class="common-page-tabs-inner-border">

            <asp:Panel runat="server" ID="contactsDisplayPanel" Visible="false">

                <div class="desktop">

                    <div class="noprint">
                        <mr:Paginator ID="topContactsPaginator" runat="server" BulkActions="true" Mode="Bottom" />
                    </div>

                    <table class="common-admin-table">

                        <thead>
                            <tr class="top">
                                <th runat="server" id="contactDisplayCbTop"><input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader1" runat="server" Text="Name"/></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader20" runat="server" Text="Outlet&nbsp;/&nbsp;Ministry" OverrideSort="Ministry"/></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader2" runat="server" Text="City"/></nobr></th>
                                <th>Primary/ Cell</th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader3" runat="server" Text="Email"/></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader6" runat="server" Text="Ministerial Job Title"/></nobr></th>
                                <th>Function</th>
                            </tr>
                        </thead>

                        <tbody id="tableContentTB">

                            <asp:Literal runat="server" ID="contactsTableLit" />

                        </tbody>

                        <tfoot>
                            <tr class="bottom">
                                <th runat="server" id="contactDisplayCbBottom"><input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader4" runat="server" Text="Name"/></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader24" runat="server" Text="Outlet&nbsp;/&nbsp;Ministry" OverrideSort="Ministry"/></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader5" runat="server" Text="City"/></nobr></th>
                                <th>Primary/ Cell</th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader7" runat="server" Text="Email"/></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader8" runat="server" Text="Ministerial Job Title"/></nobr></th>
                                <th><nobr>Function</nobr></th>
                            </tr>
                        </tfoot>

                    </table>

                    <div class="noprint">
                        <mr:Paginator ID="bottomContactsPaginator" runat="server" BulkActions="true" Mode="Bottom" />
                    </div>

                </div>

                <div class="phone">
                    <a name="contacts"></a>
                    <!--div class="phone view-panel-header"></div-->
                    <asp:Literal runat="server" ID="contactCountLit" />

                    <asp:Literal runat="server" ID="contactsPhoneDisplayLit" />

                </div>

            </asp:Panel>

            <asp:Panel runat="server" ID="outletDisplayPanel" Visible="false">

                <div class="desktop">

                    <div class="noprint">
                        <mr:Paginator ID="TopOutletsPaginator" runat="server" BulkActions="true" Mode="Bottom" />
                    </div>

                    <table class="common-admin-table">

                        <thead>
                            <tr class="top">
                                <th runat="server" id="outletDisplayCbTop"><input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader17" runat="server" Text="Name"/></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="OutletCompanySortHeader1" runat="server" Text="Company" /></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader18" runat="server" Text="City"/></nobr></th>
                                <th>News Desk</th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader19" runat="server" Text="Email"/></nobr></th>
                                <th><nobr><mr:SortColumnHeader ID="SortColumnHeader25" runat="server" Text="Media Type"/></th>
                                <th><nobr>Function</nobr></th>
                            </tr>
                        </thead>

                        <tbody id="outletContentTB">

                            <asp:Literal runat="server" ID="outletsTableLit" />

                        </tbody>

                        <tfoot>
                            <tr class="bottom">
                                <th runat="server" id="outletDisplayCbBottom"><input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></nobr></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader21" runat="server" Text="Name" /></th>
                                <th><mr:SortColumnHeader ID="OutletCompanySortHeader2" runat="server" Text="Company" /></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader22" runat="server" Text="City" /></th>
                                <th>News Desk</th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader23" runat="server" Text="Email" /></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader26" runat="server" Text="Media Type" /></th>
                                <th>Function</th>
                            </tr>
                        </tfoot>

                    </table>

                    <div class="noprint">
                        <mr:Paginator ID="BottomOutletsPaginator" runat="server" BulkActions="true" Mode="Bottom" />
                    </div>

                </div>

                <div class="phone">
                    <a name="outlets"></a>
                    <!--div class="phone view-panel-header"></div-->
                    <asp:Literal runat="server" ID="outletCountLit" />

                    <asp:Literal runat="server" ID="outletsPhoneDisplayLit" />

                </div>

            </asp:Panel>

            <asp:Panel runat="server" ID="companyDisplayPanel" Visible="false">

                <div class="desktop">
                    <div class="noprint">
                        <mr:Paginator ID="topCompanyPaginator" runat="server" BulkActions="true" Mode="Bottom" />
                    </div>

                    <table class="common-admin-table">

                        <thead>
                            <tr class="top">
                                <th runat="server" id="companyDisplayCbTop"><input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></nobr></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader9" runat="server" Text="Name" /></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader10" runat="server" Text="City" /></th>
                                <th>Primary/ Cell</th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader11" runat="server" Text="Email" /></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader12" runat="server" Text="Twitter" /></th>
                                <th>Function</th>
                            </tr>
                        </thead>

                        <tbody id="companyContentTB">

                            <asp:Literal runat="server" ID="companiesTableLit" />

                        </tbody>

                        <tfoot>
                            <tr class="bottom">
                                <th runat="server" id="companyDisplayCbBottom"><input type="checkbox" name="selectAll" onclick="toggleSelectAll(this, 'tableContentTB');"></nobr></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader13" runat="server" Text="Name" /></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader14" runat="server" Text="City" /></th>
                                <th>Primary/ Cell</th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader15" runat="server" Text="Email" /></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader16" runat="server" Text="Twitter" /></th>
                                <th>Function</th>
                            </tr>
                        </tfoot>

                    </table>

                    <div class="noprint">
                        <mr:Paginator ID="bottomCompanyPaginator" runat="server" BulkActions="true" Mode="Bottom" />
                    </div>

                </div>

                <div class="phone">
                    <a name="companies"></a>
                    <!--div class="phone view-panel-header"></div-->
                    <asp:Literal runat="server" ID="companyCountLit" />
                    <asp:Literal ID="companiesPhoneDisplayLit" runat="server" />
                </div>

            </asp:Panel>

            <div class="phone search-back-to-top">
                <a href="javascript:void(0);" onclick="window.scrollTo(0, 0);">
                    <img runat="server" src="~/Contacts/images/Top@2x.png" border="0" /></a>
            </div>

            <div class="phone bottom-save-bar">
                <asp:LinkButton ID="mobileShareBtn" title="E-mail search results to me" NavigateUrl="test.asmx" runat="server" Text="SHARE" CssClass="common-admin-link-button common-admin-button2-left mobile-share-button noprint" OnClick="ShareButton_Click" />
                <asp:HyperLink ID="mobileEmailHref" title="E-mail all of the media contacts listed below" NavigateUrl="test.asmx" runat="server" Text="EMAIL" CssClass="common-admin-link-button common-admin-button2-right mobile-email-button noprint" />
            </div>

            <asp:Literal runat="server" ID="shareSuccessLit">

            </asp:Literal>

        </div>
        <asp:Literal runat="server" ID="onLoadJsLit" />
    </mrcl:PermissionContainer>
</asp:Content>

