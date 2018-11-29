<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserControls_ViewCompany" CodeBehind="ViewCompany.ascx.cs" %>
<%@ Register TagPrefix="mr" TagName="TabControl" Src="~/Contacts/UserControls/TabControl.ascx" %>
<%@ Register TagPrefix="mr" TagName="Paginator" Src="~/Contacts/UserControls/Paginator.ascx" %>
<%@ Register TagPrefix="mr" TagName="SortColumnHeader" Src="~/Contacts/UserControls/SortColumnHeader.ascx" %>
<%@ Import Namespace="MediaRelationsLibrary" %>

<asp:Panel runat="server" ID="errorPanel" Visible="false">
    <div class="error">Invalid page request</div>
</asp:Panel>

<div runat="server" id="ErrorNotice" class="ErrorNotice" visible="false">
</div>

<asp:Panel runat="server" ID="companyDisplayPanel" Visible="false">

    <div class="common-page-box-border">

        <div class="common-admin-control-bar">

            <div><asp:HyperLink CssClass="noprint phone left-chevron" runat="server" ID="mobileBackBtn" Text="Back" Visible="true" NavigateUrl="javascript:history.back();"/></div>

            <span class="field labelbold" runat="server" id="companyNameLabel"><asp:Literal runat="server" ID="companyNameLit" /></span>

            <p class="mobile-view-name"><asp:Literal runat="server" ID="companyNameLitMobile"/></p>

            <div class="right desktop">

                <a href="#" id="historyHref" data-toggle="modal" data-target="#recordHistory" runat="server" visible="false" class="iframe">View History</a>
                <span id="historySeparator" runat="server" visible="false" class="link-separator">|</span>
                <asp:LinkButton runat="server" ID="convertToOutletBtn" OnClick="ConvertToOutletClick" Visible="false" Text="Convert to Outlet" CssClass="common-admin-button" OnClientClick="return confirm(switchToOutletText);" />
                <span id="convertToOutletSeparator" runat="server" visible="false" class="link-separator">|</span>
                <asp:LinkButton runat="server" ID="editButton" OnClick="EditButtonClick" Visible="false" Text="Edit" CssClass="common-admin-button" />
                <span id="editSeparator" runat="server" visible="false" class="link-separator">|</span>

                <asp:Panel runat="server" ID="socialMediaHrefsPanel" Style="display: inline">
                    <asp:LinkButton runat="server" OnClick="ShareButton_Click">Share</asp:LinkButton>
                    |
                    <a href="#" runat="server" id="emailHref" onclick="LogEmail();">Email</a> <span id="emailSeparator" runat="server" class="link-separator">|</span>
                    <a href="javascript:doPrint();" onclick="LogPrint();">Print</a> <span id="printSeparator" runat="server" visible="false" class="link-separator">|</span>
                </asp:Panel>

                <asp:LinkButton runat="server" ID="deleteButton" OnClick="DeleteButtonClick" Visible="false" Text="Delete" CssClass="common-admin-button" OnClientClick="return confirm(deleteButtonText);" />

            </div>
        </div>

        <div id="recordHistory" class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <iframe runat="server" id="historyFrame" frameborder="0"></iframe>
                    <a data-toggle="modal" data-target="#recordHistory"></a>
                </div>
            </div>
        </div>

        <mr:TabControl runat="server" ID="tabControl" />

        <div class="common-page-tabs-inner">

            <a name="contact"></a>
            <div class='phone mini-tab-bar mini-tab-bar-top gradient'>
                <!-- NTK - in case we need this
                a class='mini-tab-on' href='#contact'>Contact</a>
                <a class='mini-tab' href='#location'>Location</a>
                <span ID="mediaTab1" runat="server"><a class='mini-tab' href='#media'>Media</a></span>
                <span ID="outletsTab1" runat="server"><a class='mini-tab' href='#outlets'>Outlets</a></span>
                -->

                CONTACT INFO
            </div>
            <asp:Panel runat="server" ID="contactTabDisplay" CssClass="company-tab view-panel-hidden">

                <div class="info-row">
                    <span id="descriptionLabel" class="view-label" runat="server">Description:</span>
                    <span runat="server" id="descriptionValue" class="view-value"></span>
                </div>

                <div>
                    <asp:Literal runat="server" ID="phoneNumberDisplayLit" />
                </div>

                <div class="info-row">
                    <span runat="server" id="parentCompanyLabel" class="view-label">Parent Company:</span>
                    <span runat="server" id="parentCompanyValue" class="view-value"></span>
                </div>

                <div class="info-row">
                    <span runat="server" id="contactsLabel" class='view-label'>Contacts:</span>
                    <span class="view-value"><asp:Literal runat="server" ID="contactsDisplayLit" /></span>
                </div>

            </asp:Panel>

            <a name="web-address"></a>
            <div class='phone mini-tab-bar mini-tab-bar-top gradient'>
                WEB ADDRESS
            </div>
            <asp:Panel runat="server" ID="webAddressTabDisplay" CssClass="company-tab view-panel-hidden">

                <div>
                    <asp:Literal runat="server" ID="webAddressDisplayTabLit" />
                </div>

            </asp:Panel>

            <a name="location"></a>
            <div class='phone mini-tab-bar gradient'>
                <!-- NTK - old if we need ths back
                    a class='mini-tab' href='#contact'>Contact</a>
                <a class='mini-tab-on' href='#location'>Location</a>
                <span ID="mediaTab2" runat="server"><a class='mini-tab' href='#media'>Media</a></span>
                <span ID="outletsTab2" runat="server"><a class='mini-tab' href='#outlets'>Outlets</a></span>
                -->

                LOCATION
            </div>
            <asp:Panel runat="server" ID="locationTabDisplay" CssClass="company-tab view-panel-hidden">

                <asp:Panel runat="server" ID="physicalAddressPanel">
                    <div class="mailing-address-header"><b>Physical Address</b></div>

                    <div>
                        <span class="view-label" id="addressLabel" runat="server">Address:</span>
                        <span runat="server" id="addressValue" class="view-value"></span>
                    </div>

                    <div>
                        <span class="view-label" runat="server" id="cityLabel">City:</span>
                        <span class="view-value" runat="server" id="cityValue"></span>
                    </div>

                    <div>
                        <span class="view-label" runat="server" id="provinceLabel">Province:</span>
                        <span class="view-value" runat="server" id="provinceValue"></span>
                    </div>

                    <div>
                        <span class="view-label" runat="server" id="countryLabel">Country:</span>
                        <span class="view-value" runat="server" id="countryValue"></span>
                    </div>

                    <div>
                        <span class="view-label" runat="server" id="postalCodeLabel">Postal/Zip Code:</span>
                        <span class="view-value" runat="server" id="postalCodeValue"></span>
                    </div>
                </asp:Panel>

                <asp:Panel runat="server" ID="mailingAddressPanel" Visible="false">
                    <div class="mailing-address-header"><b>Mailing Address</b></div>

                    <div>
                        <span class="view-label" id="mailingAddressLabel" runat="server">Address:</span>
                        <span runat="server" id="mailingAddressValue" class="view-value"></span>
                    </div>

                    <div>
                        <span class="view-label" runat="server" id="mailingCityLabel">City:</span>
                        <span class="view-value" runat="server" id="mailingCityValue"></span>
                    </div>

                    <div>
                        <span class="view-label" runat="server" id="mailingProvinceLabel">Province:</span>
                        <span class="view-value" runat="server" id="mailingProvinceValue"></span>
                    </div>

                    <div>
                        <span class="view-label" runat="server" id="mailingCountryLabel">Country:</span>
                        <span class="view-value" runat="server" id="mailingCountryValue"></span>
                    </div>

                    <div>
                        <span class="view-label" runat="server" id="mailingPostalCodeLabel">Postal/Zip Code:</span>
                        <span class="view-value" runat="server" id="mailingPostalCodeValue"></span>
                    </div>
                </asp:Panel>

                <div>
                    <span class="view-label" runat="server" id="regionLabel">Region(s):</span>
                    <span class="view-value" runat="server" id="regionValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="electoralDistrictLabel">Electoral District(s):</span>
                    <span class="view-value" runat="server" id="electoralDistrictValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="sectorsLabel">Sector(s):</span>
                    <span class="view-value" runat="server" id="sectorsValue"></span>
                </div>


            </asp:Panel>

            <a name="media"></a>
            <div id="mediaTabBar" runat="server" class='phone mini-tab-bar gradient'>
                <!-- NTK old if we need this back
                    a class='mini-tab' href='#contact'>Contact</a>
                <a class='mini-tab' href='#location'>Location</a>
                <span ID="mediaTab3" runat="server"><a class='mini-tab-on' href='#media'>Media</a></span>
                <span ID="outletsTab3" runat="server"><a class='mini-tab' href='#outlets'>Outlets</a></span>
                    -->
                MEDIA
            </div>
            <asp:Panel runat="server" ID="mediaTabDisplay" CssClass="company-tab view-panel-hidden" Visible="false">

                <div>
                    <span class="view-label" runat="server" id="mediaDeskLabel">Media Desk(s):</span>
                    <span class="view-value" runat="server" id="mediaDeskValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="mediaPartnersLabel">Media Partner(s):</span>
                    <span class="view-value" runat="server" id="mediaPartnersValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="distributionLabel">Distribution:</span>
                    <span class="view-value" runat="server" id="distributionValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="languageLabel">Language(s):</span>
                    <span class="view-value" runat="server" id="languageValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="publicationsDayLabel">Publication Day(s):</span>
                    <span class="view-value" runat="server" id="publicationsDayValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="specialtyPublicationsLabel">Specialty Publication(s):</span>
                    <span class="view-value" runat="server" id="specialtyPublicationsValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="mediaTypeLabel">Media Type(s):</span>
                    <span class="view-value" runat="server" id="mediaTypeValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="ethnicitiesLabel">Ethnicities:</span>
                    <span class="view-value" runat="server" id="ethnicitiesValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="ethnicMediaLabel">Ethnic Media:</span>
                    <span class="view-value" runat="server" id="ethnicMediaValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="majorMediaLabel">Major Media:</span>
                    <span class="view-value" runat="server" id="majorMediaValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="liveMediaOpportunityLabel">Live Media Opportunity:</span>
                    <span class="view-value" runat="server" id="liveMediaOpportunityValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="publicationFrequencyLabel">Publication Frequency:</span>
                    <span class="view-value" runat="server" id="publicationFrequencyValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="keyProgramsLabel">Key Programs:</span>
                    <span class="view-value" runat="server" id="keyProgramsValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="circulationLabel">Circulation Description:</span>
                    <span class="view-value" runat="server" id="circulationValue"></span>
                </div>

                <div>
                    <span class="view-label" runat="server" id="deadlinesLabel">Deadlines:</span>
                    <span class="view-value" runat="server" id="deadlinesValue"></span>
                </div>

            </asp:Panel>

            <a name="media"></a>
            <div id="outletsTabBar" runat="server" class='phone mini-tab-bar gradient'>
                <!--NTK - old if we need this
                    a class='mini-tab' href='#contact'>Contact</a>
                <a class='mini-tab' href='#location'>Location</a>
                <span ID="mediaTab4" runat="server"><a class='mini-tab' href='#media'>Media</a></span>
                <span ID="outletsTab4" runat="server"><a class='mini-tab-on' href='#outlets'>Outlets</a></span>
                    -->
                OUTLETS
            </div>
            <asp:Panel runat="server" ID="outletTabDisplay" CssClass="company-tab view-panel-hidden" Visible="false">
                <div class="desktop">
                    <mr:Paginator runat="server" ID="PaginatorTop" Mode="Bottom" BulkActions="False" />
                    <table class="common-admin-table">

                        <thead>
                            <tr class="top">
                                <th><mr:SortColumnHeader ID="SortColumnHeader1" runat="server" Text="Company"/></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader2" runat="server" Text="City"/></th>
                                <th>MEDIA TYPES</th>
                                <th>PRIMARY</th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader5" runat="server" Text="Email"/></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader6" runat="server" Text="Twitter"/></th>      
                                <th>FUNCTION</th>
                            </tr>
                        </thead>

                        <tbody>

                            <asp:Literal runat="server" ID="outletTableLit" />

                        </tbody>

                        <tfoot>
                            <tr class="bottom">
                                <th><mr:SortColumnHeader ID="SortColumnHeader7" runat="server" Text="Company"/></th>                    
                                <th><mr:SortColumnHeader ID="SortColumnHeader9" runat="server" Text="City"/></th>
                                <th>MEDIA TYPES</th>
                                <th>PRIMARY</th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader12" runat="server" Text="Email"/></th>
                                <th><mr:SortColumnHeader ID="SortColumnHeader13" runat="server" Text="Twitter"/></th>                    
                                <th>FUNCTION</th>
                            </tr>
                        </tfoot>

                    </table>
                    <mr:Paginator runat="server" ID="PaginatorBottom" Mode="Bottom" BulkActions="False" />
                </div>
                <div class="phone">
                    <asp:Literal ID="outletsPhoneDisplay" runat="server" />
                </div>
            </asp:Panel>

        </div>

    </div>

    <div class="last-updated-date">Last Updated: <asp:Literal runat="server" ID="lastUpdatedLit" /></div>

    <div class="phone bottom-save-bar noprint">

        <asp:LinkButton ID="shareMobileButton" runat="server" Text="SHARE" CssClass="common-admin-link-button common-admin-button2-left mobile-share-button" OnClick="ShareButton_Click" />
        <asp:HyperLink ID="emailMobileButton" runat="server" Text="EMAIL" CssClass="common-admin-link-button common-admin-button2-right mobile-email-button" onclick="LogEmail();" />

    </div>

    <div class="common-back-button desktop noprint" runat="server" id="backButtonContainer">
        <asp:HyperLink runat="server" ID="backButton" Text="BACK TO HOME" NavigateUrl="~/Contacts/" />
    </div>

    <script type="text/javascript">

        function LogPrint() {
            var requestUrl = "/Contacts/ajax/LogPrintCompany.ashx?guid=" + escape('<%= guid.ToString() %>');

            var request = GetHttpRequestObj();
            request.open("GET", requestUrl, false);
            request.send();
        }

        function LogEmail() {
            var requestUrl = "/Contacts/ajax/LogEmailCompany.ashx?guid=" + escape('<%= guid.ToString() %>');

            var request = GetHttpRequestObj();
            request.open("GET", requestUrl, false);
            request.send();
        }

    </script>

</asp:Panel>

<asp:HiddenField runat="server" ID="changedTabHiddenField" ClientIDMode="Static" />

<script type="text/javascript">

    function ChangeCurrentTab(tabName) {
        var mainContainer = document.getElementById('<%= companyDisplayPanel.ClientID %>');

        var elements = mainContainer.getElementsByTagName('div');

        var containerIdStr = '';
        if (tabName.toLowerCase() == "contactinfo") {
            containerIdStr = "<%= contactTabDisplay.ClientID %>";
        } else if (tabName.toLowerCase() == "webaddress") {
            containerIdStr = "<%= webAddressTabDisplay.ClientID %>";
        } else if (tabName.toLowerCase() == "location") {
            containerIdStr = "<%= locationTabDisplay.ClientID %>";
        } else if (tabName.toLowerCase() == "outlets") {
            containerIdStr = "<%= outletTabDisplay.ClientID %>";
        } else if (tabName.toLowerCase() == "media") {
            containerIdStr = "<%= mediaTabDisplay.ClientID %>";
        }



    for (var i = 0; i < elements.length; i++) {
        var element = elements[i];

        if (element.className.indexOf("company-tab") > -1) {

            if (element.id == containerIdStr) {
                element.className = 'company-tab view-panel-show';
            } else {
                element.className = 'company-tab view-panel-hidden';
            }
        }
    }

    document.getElementById('changedTabHiddenField').value = tabName;

    ChangeActiveTab(tabName);

    return false;
}

</script>

<asp:Literal ID="bottomScriptArea" runat="server" />
