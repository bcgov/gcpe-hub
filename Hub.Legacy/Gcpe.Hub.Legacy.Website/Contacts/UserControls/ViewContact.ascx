<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserControls_ViewContact" CodeBehind="ViewContact.ascx.cs" ClientIDMode="Static"%>
<%@ Register TagPrefix="mr" TagName="MultiSelector" Src="~/Contacts/UserControls/MultiSelector.ascx" %>
<%@ Register TagPrefix="mr" TagName="TabControl" Src="~/Contacts/UserControls/TabControl.ascx" %>


<asp:Panel ID="ViewContactControl" runat="server">
    <div id="ErrorNotice" class="ErrorNotice"><asp:Literal ID="ErrorLit" runat="server" /></div>

    <div class="common-page-box-border">
        <div class="common-admin-control-bar">

            <div><asp:HyperLink CssClass="noprint phone left-chevron" runat="server" ID="mobileBackBtn" Text="Back" Visible="true" NavigateUrl="javascript:history.back();" /></div>

            <span class="field labelbold" runat="server" id="contactNameLabel"><asp:Literal ID="ContactName" runat="server" /></span>

            <p class="mobile-view-name"><asp:Literal runat="server" ID="contactNameLitMobile" /></p>

            <div class="right desktop">
                <asp:Panel ID="TopButtonPanel" runat="server">
                    <a href="#" id="historyHref" data-toggle="modal" data-target="#recordHistory" runat="server" visible="false" class="iframe">View History</a>
                    <span id="historySeparator" runat="server" visible="false" class="link-separator">|</span>
                    <asp:LinkButton CssClass="common-admin-button" ID="EditButton" Text="Edit" OnClick="EditButton_Click" runat="server" />
                    <span id="editSeparator" runat="server" visible="false" class="link-separator">|</span>
                    <asp:LinkButton CssClass="common-admin-button" ID="CancelButton" Text="Cancel" OnClick="CancelButton_Click" runat="server" />
                    <span id="cancelSeparator" runat="server" visible="false" class="link-separator">|</span>
                    <asp:LinkButton ID="shareLink" runat="server" OnClick="ShareButton_Click">Share</asp:LinkButton>
                    <span id="shareSeparator" runat="server" visible="false" class="link-separator">|</span>
                    <span id="emailLink" runat="server"><asp:HyperLink ID="emailHref" runat="server" Text="Email" onclick="LogEmail();"/> </span>
                    <span id="emailSeparator" runat="server" visible="false" class="link-separator">|</span>
                    <span id="printLink" runat="server"><a href="javascript:doPrint();" onclick="LogPrint();">Print</a></span>
                    <span id="printSeparator" runat="server" visible="false" class="link-separator">|</span>

                    <asp:LinkButton runat="server" ID="DeleteButton" OnClick="DeleteButton_Click" Visible="false" Text="Delete" CssClass="common-admin-button" />
                    <input type="hidden" name="hidAltContact" id="hidAltContact"/>
                    <input type="hidden" name="hidTab" id="hidTab" />
                </asp:Panel>
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

        <mr:TabControl ID="TabControl" runat="server" />
        <div class="common-page-tabs-inner">
            <a name="contact"></a>
            <div class='phone mini-tab-bar mini-tab-bar4 mini-tab-bar-top gradient'>
                <!-- NTK in case we need
                    a class='mini-tab-on' href='#contact'>Contact</a>
                <a class='mini-tab' href='#location'>Location</a>
                <a class='mini-tab' href='#media'>Media</a>
                <a class='mini-tab' href='#media'>Ministry</a>-->
                CONTACT INFO
            </div>
            <asp:Panel ID="ContactPanel" runat="server" CssClass="contact-tab view-panel-hidden">
                <asp:Literal ID="ContactLiteral" runat="server" />
            </asp:Panel>
            <a name="Web Address"></a>
            <div class='phone mini-tab-bar mini-tab-bar4 gradient'>
                <!-- NTK in case we need
                    a class='mini-tab' href='#contact'>Contact</a>
                <a class='mini-tab-on' href='#location'>Location</a>
                <a class='mini-tab' href='#media'>Media</a>
                <a class='mini-tab' href='#media'>Ministry</a>-->
                WEB ADDRESS
            </div>
            <asp:Panel ID="WebAddressPanel" runat="server" CssClass="contact-tab view-panel-hidden">
                <asp:Literal ID="WebAddressLiteral" runat="server" />
            </asp:Panel>
            <a name="location"></a>
            <div class='phone mini-tab-bar mini-tab-bar4 gradient'>
                <!-- NTK in case we need
                    a class='mini-tab' href='#contact'>Contact</a>
                <a class='mini-tab-on' href='#location'>Location</a>
                <a class='mini-tab' href='#media'>Media</a>
                <a class='mini-tab' href='#media'>Ministry</a>-->
                LOCATION
            </div>
            <asp:Panel ID="LocationPanel" runat="server" CssClass="contact-tab view-panel-hidden">
                <asp:Literal ID="LocationLiteral" runat="server" />
            </asp:Panel>
            <a name="media"></a>
            <div class='phone mini-tab-bar mini-tab-bar4 gradient'>
                <!-- NTK in case we need 
                    a class='mini-tab' href='#contact'>Contact</a>
                <a class='mini-tab' href='#location'>Location</a>
                <a class='mini-tab-on' href='#media'>Media</a>
                <a class='mini-tab' href='#media'>Ministry</a>-->
                MEDIA
            </div>
            <asp:Panel ID="MediaPanel" runat="server" CssClass="contact-tab view-panel-hidden">
                <asp:Literal ID="MediaLiteral" runat="server" />
            </asp:Panel>
            <a name="ministry"></a>
            <div class='phone mini-tab-bar mini-tab-bar4 gradient'>
                <!-- NTK in case we need 
                    a class='mini-tab' href='#contact'>Contact</a>
                <a class='mini-tab' href='#location'>Location</a>
                <a class='mini-tab' href='#media'>Media</a>
                <a class='mini-tab-on' href='#media'>Ministry</a>-->
                MINISTRY
            </div>
            <asp:Panel ID="MinistryPanel" runat="server" CssClass="contact-tab view-panel-hidden">
                <asp:Literal ID="MinistryLiteral" runat="server" />
            </asp:Panel>
        </div>
    </div>

    <div class="last-updated-date">Last Updated: <asp:Literal ID="LastUpdatedDate" runat="server" /></div>

    <div class="phone bottom-save-bar noprint">
        <asp:LinkButton ID="shareMobileButton" runat="server" Text="SHARE" CssClass="common-admin-link-button common-admin-button2-left mobile-share-button" OnClick="ShareButton_Click" />
        <asp:HyperLink ID="emailMobileHref" CssClass="common-admin-link-button common-admin-button2-right mobile-email-button" Text="EMAIL" runat="server" onclick="LogEmail()" />
    </div>

    <asp:Panel ID="BottomButtonPanel" runat="server">

        <asp:Button CssClass="common-admin-button" ID="CancelButtonBottom" Text="Cancel" OnClick="CancelButton_Click" runat="server" />

    </asp:Panel>

</asp:Panel>

<div runat="server" class="common-back-button desktop noprint" id="backButtonContainer">
    <a href="~/Contacts/" runat="server" id="backButtonHref">BACK TO HOME</a>
</div>

<script type="text/javascript">

    function LogPrint() {
        var requestUrl = "/Contacts/ajax/LogPrintContact.ashx?guid=" + escape('<%= guid.ToString() %>');

        var request = GetHttpRequestObj();
        request.open("GET", requestUrl, false);
        request.send();
    }

    function LogEmail() {
        var requestUrl = "/Contacts/ajax/LogEmailContact.ashx?guid=" + escape('<%= guid.ToString() %>');

    var request = GetHttpRequestObj();
    request.open("GET", requestUrl, false);
    request.send();
}

</script>

<script type="text/javascript">

    function ChangeCurrentTab(tabName) {

        var mainContainer = document.getElementById('<%= ViewContactControl.ClientID %>');

        var elements = mainContainer.getElementsByTagName('div');

        var containerIdStr = '';
        if (tabName.toLowerCase() == "contactinfo") {
            containerIdStr = "<%= ContactPanel.ClientID %>";
        } else if (tabName.toLowerCase() == "webaddress") {
            containerIdStr = "<%= WebAddressPanel.ClientID %>";
        } else if (tabName.toLowerCase() == "location") {
            containerIdStr = "<%= LocationPanel.ClientID %>";
        } else if (tabName.toLowerCase() == "media") {
            containerIdStr = "<%= MediaPanel.ClientID %>";
        } else if (tabName.toLowerCase() == "ministry") {
            containerIdStr = "<%= MinistryPanel.ClientID %>";
        }

    document.getElementById("hidTab").value = tabName;

    for (var i = 0; i < elements.length; i++) {
        var element = elements[i];

        if (element.className.indexOf("contact-tab") > -1) {

            if (element.id == containerIdStr) {
                element.className = 'contact-tab view-panel-show';
            } else {
                element.className = 'contact-tab view-panel-hidden';
            }
        }
    }

    ChangeActiveTab(tabName);

    return false;
}

<asp:Literal ID="bottomScriptArea" runat="server"/>
</script>
