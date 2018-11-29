<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserControls_AddEditCompany" CodeBehind="AddEditCompany.ascx.cs" %>
<%@ Register TagPrefix="mr" TagName="Tabs" Src="~/Contacts/UserControls/TabControl.ascx" %>
<%@ Register TagPrefix="mr" TagName="Paginator" Src="~/Contacts/UserControls/Paginator.ascx" %>
<%@ Register TagPrefix="mr" TagName="SortColumnHeader" Src="~/Contacts/UserControls/SortColumnHeader.ascx" %>
<%@ Register TagPrefix="mr" TagName="ListBoxTransfer" Src="~/Contacts/UserControls/ListBoxTransferPopup.ascx" %>
<%@ Register TagPrefix="mr" TagName="MultiSelector" Src="~/Contacts/UserControls/MultiSelector.ascx" %>
<%@ Register TagPrefix="mr" TagName="MultiSelectorDuplicate" Src="~/Contacts/UserControls/MultiSelectorDuplicate.ascx" %>
<%@ Import Namespace="MediaRelationsLibrary" %>

<div runat="server" id="ErrorNotice" class="ErrorNotice"><asp:Literal ID="ErrorLit" runat="server" /></div>
<div class="common-page-box-border">

    <asp:Literal runat="server" ID="jsLit" />
    <asp:HiddenField runat="server" ID="tabHiddenField" />
    <asp:HiddenField runat="server" ID="addNewOutletHiddenField" />

    <script type="text/javascript">

        function DoTabChange(tabName, currentTab) {
            if (currentTab == "Outlets") return true;

            var okToContinue = true;
            if (doUnloadCheck) {
                okToContinue = confirm(tabConfirmText);
            }
            if (okToContinue) {
                document.getElementById('<%= tabHiddenField.ClientID %>').value = tabName;
                document.getElementById('<%= saveButton.ClientID %>').click();

            }

            return false;
        }

        <asp:Literal ID="AlertLit" runat="server"/>

    </script>

    <mr:Tabs runat="server" ID="tabControl" />

    <div class="common-page-tabs-inner">

        <asp:Panel runat="server" ID="contactPanel" Visible="false">
            <asp:HiddenField runat="server" ID="timestamp" />

            <div class="edit-form-container" style="margin-bottom: 50px">

                <div class="field">
                    <img runat="server" class="field-changed" id="companyNameChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Media Name (maximum 250 characters) <span class="required">*</span>
                </div>
                <asp:TextBox ID="mediaNameTb" MaxLength="250" runat="server" />
                <div class="error" runat="server" id="mediaNameError"></div>

                <div class="field">
                    <img runat="server" class="field-changed" id="companyDescriptionChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Description (maximum 500 characters)
                </div>
                <asp:TextBox TextMode="MultiLine" runat="server" MaxLength="500" ID="descriptionTb" />
                <div class="error" runat="server" id="descriptionError"></div>


                <asp:Panel runat="server" ID="parentCompanyPanel" Visible="false">
                    <div class="field">
                        <img runat="server" class="field-changed" id="parentCompanyChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Parent Company
                    </div>
                    <asp:DropDownList runat="server" ID="parentCompanyDD" />
                </asp:Panel>

            </div>

            <div class="multi-select-container">

                <div class="item even">

                    <mr:MultiSelectorDuplicate runat="server" ID="phoneNumberSelector"
                        ShowExt="true"
                        ShowTextField="True"
                        ShowSelect="false"
                        AllowDuplicates="true"
                        SelectLabel="Phone Numbers <span class='required'>*</span>"
                        SecondaryLabel="Enter Number"
                        CountLabel="Phone Number(s)"
                        MaxLength="50"
                        Mode="Short" />

                </div>

            </div>

            <script type="text/javascript">

                function CheckPageValidity() {
                    var valid = true;

                    document.getElementById('<%= mediaNameError.ClientID %>').innerHTML = "";
                    document.getElementById('<%=phoneNumberSelector.ErrorDiv.ClientID%>').innerHTML = "";
                    document.getElementById('<%= descriptionError.ClientID %>').innerHTML = "";

                    var errorFields = "";

                    if (document.getElementById('<%= mediaNameTb.ClientID %>').value.trim() == "") {
                        errorFields += "Media Name\n";
                        document.getElementById('<%= mediaNameError.ClientID %>').innerHTML = "Media name cannot be empty";
                        valid = false;
                    }

                    var phoneErrorText = "";
                    var phoneError = false;
                    if (multiselectorDuplicate_selectedCount['<%=phoneNumberSelector.ClientID%>'] == null || multiselectorDuplicate_selectedCount['<%=phoneNumberSelector.ClientID%>'] <= 0) {
                        phoneError = true;
                        phoneErrorText += "Please add at least one phone number<br/>\n";                    
                    }

                    if (phoneError) {
                        errorFields += "Phone Numbers\n";
                        document.getElementById('<%=phoneNumberSelector.ErrorDiv.ClientID%>').innerHTML = phoneErrorText;
                        valid = false;
                    }

                    if (document.getElementById('<%= descriptionTb.ClientID %>').value.trim() != "") {
                        if (document.getElementById('<%= descriptionTb.ClientID %>').value.trim().length > 500) {
                            errorFields += "Description\n";
                            document.getElementById('<%= descriptionError.ClientID %>').innerHTML = "Must not be longer than 500 characters";
                    }
                }


                if (!valid) {
                    alert(saveErrorText.replace("###errors###", errorFields));
                }


                return valid;
            }

            </script>

        </asp:Panel>

        <asp:Panel runat="server" ID="webAddressPanel" Visible="false">
            <asp:UpdatePanel ID="UpdatePanelWebAddress" runat="server" OnLoad="UpdatePanelWebAddressLoad">
                <ContentTemplate>
                    <table style="width: 100%;">
                        <tr style="vertical-align: top">
                            <td style="width: 60%;">
                                <asp:GridView ID="GridViewWebAddress" AutoGenerateColumns="false" runat="server" OnSelectedIndexChanging="GridViewWebAddress_SelectWebAddress" OnRowDeleting="GridViewWebAddress_RowDeleting" Width="100%" CssClass="common-admin-table">
                                    <Columns>
                                        <asp:ButtonField Text="Edit" CommandName="Select" ItemStyle-Width="30" />
                                        <asp:BoundField DataField="Id" ItemStyle-CssClass="hidden" HeaderStyle-CssClass="hidden" />
                                        <asp:BoundField DataField="WebAddressTypeName" HeaderText="Address&nbsp;Type" />
                                        <asp:BoundField DataField="NewWebAddress" HeaderText="Web Address" />
                                        <asp:TemplateField HeaderText="Media Distributions" ItemStyle-Width="200px" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="left">
                                            <ItemTemplate><%# Eval("MediaDistributions") %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Deleted">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="DeletedCheckBox" AutoPostBack="false" Enabled="false" Checked='<%# Eval("IsDeleted") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:ButtonField Text="Delete" CommandName="Delete" ItemStyle-Width="30" />
                                    </Columns>
                                </asp:GridView>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="WebAddressEditPanel" runat="server" Visible="False">
                                    <asp:Label ID="lblWebAddress" runat="server" Text="Label" Visible="false">
                                    </asp:Label>
                                    <asp:DropDownList ID="cboAddressTypes" runat="server" Visible="false" OnSelectedIndexChanged="cboAddressTypes_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>: 
                                    <asp:TextBox ID="txtWebAddress" runat="server" OnTextChanged="txtWebAddress_TextChanged" AutoPostBack="true" Width="420px"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="mediaDistributionListBoxLabel" runat="server" Text="Media Distribution Lists" Visible="true" Width="250px">
                                    </asp:Label><asp:ListBox ID="mediaDistributionListBox" SelectionMode="Multiple" AutoPostBack="false" runat="server"></asp:ListBox>

                                    <div style="float: right; padding-top: 5px;">
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" OnClientClick="setUnloadCheckTrue" />
                                        &nbsp;
                                        <asp:Button ID="btnApply" runat="server" Text="Apply" OnClick="btnApply_Click" OnClientClick="setUnloadCheckTrue" />
                                    </div>

                                </asp:Panel>
                                <asp:Button ID="btnAddWebAddress" runat="server" Text="Add Web Address" OnClick="btnAddWebAddress_Click" />

                                <script type="text/javascript">
                                    // ----------------------------------------------------------------------
                                    // MediaDistribution multiselect
                                    // ----------------------------------------------------------------------
                                    var prm = Sys.WebForms.PageRequestManager.getInstance();

                                    prm.add_endRequest(function() {
                                        // re-bind your jQuery events here
                                        $('#<%= mediaDistributionListBox.ClientID %>').kendoMultiSelect({
                                            autoClose: false,
                                            filter: "contains"
                                        });
                                    });
                        

                                    $('#mediaListPicker').show();
                                </script>

                                <div id="Div2" runat="server" class="error" />
                            </td>
                            <td style="padding-left:10px;">
                                <asp:GridView ID="GridViewOtherInfo" AutoGenerateColumns="false" runat="server" Width="100%" CssClass="common-admin-table">
                                    <Columns>
                                        <asp:BoundField DataField="Type" HeaderText="Used By"/>
                                        <asp:BoundField DataField="Name" HeaderText="Usage"/>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="GridViewWebAddress" EventName="selectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
            <%-- </div>--%>
            <%-- </div>--%>

            <script type="text/javascript">

                function CheckPageValidity() {
                    var valid = true;
                    var errorFields = "";

                    // TODO: validate any WebAddress changes...

                    if (!valid) {
                        alert(saveErrorText.replace("###errors###", errorFields));
                    }

                    return valid;
                }

            </script>
        </asp:Panel>

        <asp:Panel runat="server" ID="locationPanel" Visible="false">
            <div>
                <h3 style="float: left">Address <span class="required">*</span></h3>
                <div class='' style="margin-top: 14px; float: left; margin-left: 10px;">(At least one must be filled)</div>

                <div class="clear"></div>
            </div>
            <div class="error" runat="server" id="addressesError"></div>

            <div class="multi-select-container edit-form-container" style="margin-bottom: 50px;">
                <div class="item even">
                    <!-------------------- start physical address -------------------------------->
                    <div><b>Physical Address</b></div>

                    <div>

                        <div class="field">
                            <img runat="server" class="field-changed" id="addressChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />
                            Address (Maximum 250 characters)
                        </div>
                        <asp:TextBox runat="server" class="textarea-250" ID="streetAddressTb" TextMode="MultiLine" />
                        <div class="error" runat="server" id="addressError"></div>

                        <div>

                            <div class="field">
                                <img runat="server" class="field-changed" id="cityChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />City <span class="required">*</span>
                            </div>
                            <asp:DropDownList ID="cityDD" runat="server" />
                            <div class="error" runat="server" id="cityError"></div>

                        </div>

                        <div>

                            <div class="field">
                                <img runat="server" class="field-changed" id="provinceChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Province/State <span class="required">*</span>
                            </div>
                            <asp:DropDownList ID="provinceDD" runat="server" />
                            <div class="error" runat="server" id="provinceError"></div>

                        </div>

                        <div class="field">
                            <img runat="server" class="field-changed" id="countryChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Country <span class="required">*</span>
                        </div>
                        <asp:DropDownList runat="server" ID="countryDD" />
                        <div class="error" runat="server" id="countryError"></div>

                        <div class="field">
                            <img runat="server" class="field-changed" id="postalCodeChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Postal Code
                        </div>
                        <asp:TextBox runat="server" ID="postalCodeTb" MaxLength="50" />

                    </div>
                    <!-------------------- end physical address -------------------------------->
                </div>

                <div class="item odd">
                    <!-------------------- start mailing address -------------------------------->
                    <div><b>Mailing Address</b></div>

                    <div>

                        <div class="field">
                            <img runat="server" class="field-changed" id="mailingAddressChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />
                            Address (Maximum 250 characters)
                        </div>
                        <asp:TextBox runat="server" class="textarea-250" ID="mailingStreetAddressTb" TextMode="MultiLine" />
                        <div class="error" runat="server" id="mailingAddressError"></div>

                        <div>

                            <div class="field">
                                <img runat="server" class="field-changed" id="mailingCityChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />City <span class="required">*</span>
                            </div>
                            <asp:DropDownList ID="mailingCityDD" runat="server" />
                            <div class="error" runat="server" id="mailingCityError"></div>

                        </div>

                        <div>

                            <div class="field">
                                <img runat="server" class="field-changed" id="mailingProvinceChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Province/State <span class="required">*</span>
                            </div>
                            <asp:DropDownList ID="mailingProvinceDD" runat="server" />
                            <div class="error" runat="server" id="mailingProvinceError"></div>

                        </div>

                        <div class="field">
                            <img runat="server" class="field-changed" id="mailingCountryChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Country <span class="required">*</span>
                        </div>
                        <asp:DropDownList runat="server" ID="mailingCountryDD" />
                        <div class="error" runat="server" id="mailingCountryError"></div>

                        <div class="field">
                            <img runat="server" class="field-changed" id="mailingPostalCodeChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Postal Code
                        </div>
                        <asp:TextBox runat="server" ID="mailingPostalCodeTb" MaxLength="50" />

                    </div>
                    <!-------------------- end mailing address -------------------------------->
                </div>

                <div class="clear"></div>
            </div>

            <div class="multi-select-container">

                <div class="item even">
                    <mr:MultiSelector runat="server" ID="regionSelector"
                        SelectLabel="Region <span class='required'>*</span>"
                        CountLabel="Region(s)"
                        Mode="Short" />
                </div>

                <div class="item odd">
                    <mr:MultiSelector runat="server" ID="electoralDistrictSelector"
                        SelectLabel="Electoral District <span class='required'>*</span>"
                        CountLabel="Electoral District(s)"
                        Mode="Short" />
                </div>

                <div class="item even">
                    <mr:MultiSelector runat="server" ID="sectorSelector"
                        SelectLabel="Sector"
                        CountLabel="Sector(s)"
                        Mode="Short" />
                </div>

                <div class="clear"></div>

            </div>


            <script type="text/javascript">

                function CheckPageValidity() {
                    document.getElementById('<%= addressesError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= addressError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= cityError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= provinceError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= countryError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= mailingAddressError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= mailingCityError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= mailingProvinceError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= mailingCountryError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= regionSelector.ErrorDiv.ClientID %>').innerHTML = "";
                    document.getElementById('<%= electoralDistrictSelector.ErrorDiv.ClientID %>').innerHTML = "";

                    var errorFields = "";
                
                    // check addresses
                
                    // physical address
                    var streetAddressFilled = false;
                    var citySelected = false;
                    var provinceSelected = false;
                    var countrySelected = false;
                    var postalCodeFilled = false;

                    var addressFilledOut = false;

                    if (document.getElementById('<%= streetAddressTb.ClientID %>').value.trim() != "") {
                        streetAddressFilled = true;
                        addressFilledOut = true;

                        if (document.getElementById('<%= streetAddressTb.ClientID %>').value.trim().length > 250) {
                            errorFields += "Physical address: Street Address\n";
                            document.getElementById('<%= addressError.ClientID %>').innerHTML = "Address cannot be greater than 250 characters long";
                        }
                    }
                    if (document.getElementById('<%= cityDD.ClientID %>').options[document.getElementById('<%= cityDD.ClientID %>').selectedIndex].value != "<%= Guid.Empty.ToString() %>") {
                        citySelected = true;
                        addressFilledOut = true;
                    }
                    if (document.getElementById('<%= provinceDD.ClientID %>').options[document.getElementById('<%= provinceDD.ClientID %>').selectedIndex].value != "<%= Guid.Empty.ToString() %>") {
                        provinceSelected = true;
                        addressFilledOut = true;
                    }
                    if (document.getElementById('<%= countryDD.ClientID %>').options[document.getElementById('<%= countryDD.ClientID %>').selectedIndex].value != "<%= Guid.Empty.ToString() %>") {
                        countrySelected = true;
                        addressFilledOut = true;
                    }
                    if (document.getElementById('<%= postalCodeTb.ClientID %>').value.trim() != "") {
                        postalCodeFilled = true;
                        addressFilledOut = true;
                    }

                    if (addressFilledOut) {
                    
                        if (!citySelected) {
                            errorFields += "Physical address: City\n";
                            document.getElementById('<%= cityError.ClientID %>').innerHTML = "City has to be selected";
                        }
                        if (!provinceSelected) {
                            errorFields += "Physical address: Province\n";
                            document.getElementById('<%= provinceError.ClientID %>').innerHTML = "Province/State has to be selected";
                        }
                        if (!countrySelected) {
                            errorFields += "Physical address: Country\n";
                            document.getElementById('<%= countryError.ClientID %>').innerHTML = "Country has to be selected";
                        }

                    }

                    // mailing address
                    var streetAddressFilled = false;
                    var citySelected = false;
                    var provinceSelected = false;
                    var countrySelected = false;
                    var postalCodeFilled = false;

                    var mailingAddressFilledOut = false;

                    if (document.getElementById('<%= mailingStreetAddressTb.ClientID %>').value.trim() != "") {
                        streetAddressFilled = true;
                        mailingAddressFilledOut = true;

                        if (document.getElementById('<%= mailingStreetAddressTb.ClientID %>').value.trim().length > 250) {
                            errorFields += "Mailing address: Street Address\n";
                            document.getElementById('<%= mailingAddressError.ClientID %>').innerHTML = "Address cannot be greater than 250 characters long";
                    }

                }
                if (document.getElementById('<%= mailingCityDD.ClientID %>').options[document.getElementById('<%= mailingCityDD.ClientID %>').selectedIndex].value != "<%= Guid.Empty.ToString() %>") {
                        citySelected = true;
                        mailingAddressFilledOut = true;
                    }
                    if (document.getElementById('<%= mailingProvinceDD.ClientID %>').options[document.getElementById('<%= mailingProvinceDD.ClientID %>').selectedIndex].value != "<%= Guid.Empty.ToString() %>") {
                        provinceSelected = true;
                        mailingAddressFilledOut = true;
                    }
                    if (document.getElementById('<%= mailingCountryDD.ClientID %>').options[document.getElementById('<%= mailingCountryDD.ClientID %>').selectedIndex].value != "<%= Guid.Empty.ToString() %>") {
                        countrySelected = true;
                        mailingAddressFilledOut = true;
                    }
                    if (document.getElementById('<%= mailingPostalCodeTb.ClientID %>').value.trim() != "") {
                        postalCodeFilled = true;
                        mailingAddressFilledOut = true;
                    }

                    if (mailingAddressFilledOut) {

                        if (!citySelected) {
                            errorFields += "Mailing address: City\n";
                            document.getElementById('<%= mailingCityError.ClientID %>').innerHTML = "City has to be selected";
                        }
                        if (!provinceSelected) {
                            errorFields += "Mailing address: Province\n";
                            document.getElementById('<%= mailingProvinceError.ClientID %>').innerHTML = "Province/State has to be selected";
                        }
                        if (!countrySelected) {
                            errorFields += "Mailing address: Country\n";
                            document.getElementById('<%= mailingCountryError.ClientID %>').innerHTML = "Country has to be selected";
                        }

                    }

                    if (addressFilledOut == false && mailingAddressFilledOut == false) {
                        errorFields += "Address\n";
                        document.getElementById('<%= addressesError.ClientID %>').innerHTML = "At least one address must be filled out";
                    }


                    // bottom multiselects

                    if (multiselector_selectedCount['<%=regionSelector.ClientID%>'] == null || multiselector_selectedCount['<%=regionSelector.ClientID%>'] <= 0) {
                        errorFields += "Region\n";
                        document.getElementById('<%= regionSelector.ErrorDiv.ClientID %>').innerHTML = "At least one region is required";
                    }

                    if (multiselector_selectedCount['<%=electoralDistrictSelector.ClientID%>'] == null || multiselector_selectedCount['<%=electoralDistrictSelector.ClientID%>'] <= 0) {                    
                        errorFields += "Electoral District\n";
                        document.getElementById('<%= electoralDistrictSelector.ErrorDiv.ClientID %>').innerHTML = "At least one electoral district is required";                    
                    }

                    if (errorFields != "") {
                        alert(saveErrorText.replace("###errors###", errorFields));
                        return false;
                    }

                    return true;
                }

            </script>

        </asp:Panel>

        <asp:Panel runat="server" ID="outletsPanel" Visible="false">

            <div class="common-page-tabs-right-container edit-page">
                <a href="#listBoxTransferPopup" id="listBoxTransferPopupLink" class="outlet">Manage Outlets</a>
            </div>

            <mr:Paginator runat="server" ID="PaginatorTop" Mode="Bottom" BulkActions="False" />
            <table class="common-admin-table">

                <thead>
                    <tr class="top">
                        <th>
                            <mr:SortColumnHeader ID="SortColumnHeader1" runat="server" Text="Company" />
                        </th>
                        <th>
                            <mr:SortColumnHeader ID="SortColumnHeader2" runat="server" Text="City" />
                        </th>
                        <th>MEDIA TYPES</th>
                        <th>PRIMARY</th>
                        <th>
                            <mr:SortColumnHeader ID="SortColumnHeader5" runat="server" Text="Email" />
                        </th>
                        <th>
                            <mr:SortColumnHeader ID="SortColumnHeader6" runat="server" Text="Twitter" />
                        </th>
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
                    </tr>
                </tfoot>

            </table>
            <mr:Paginator runat="server" ID="PaginatorBottom" Mode="Bottom" BulkActions="False" />


            <mr:ListBoxTransfer runat="server"
                ID="outletPopup"
                AvailableListHeader="Available Outlets"
                SelectedListHeader="Selected Outlets"
                PopupHeader="Add Outlets"
                CallBackMode="PostBack"
                DoneButtonText="Done" />


            <script type="text/javascript">

           
                function CheckPageValidity() {
                    var valid = true;

                    return valid;
                }

            </script>

        </asp:Panel>

        <asp:Panel runat="server" ID="mediaPanel" Visible="false">

            <div class="multi-select-container" style="margin-top: 10px;">

                <div class="item even">
                    <mr:MultiSelector runat="server" ID="mediaDeskSelector"
                        SelectLabel="Media Desk"
                        CountLabel="Media Desk(s)"
                        Mode="Short" />
                </div>

                <div class="item odd">
                    <mr:MultiSelector runat="server" ID="mediaPartnersSelector"
                        SelectLabel="Media Partners"
                        CountLabel="Media Partner(s)"
                        Mode="Short" />
                </div>

                <div class="item even">
                    <mr:MultiSelector runat="server" ID="distributionSelector"
                        SelectLabel="Distribution <span class='required'>*</span>"
                        CountLabel="Distribution(s)"
                        Mode="Short" />
                </div>

                <div class="item odd">
                    <mr:MultiSelector runat="server" ID="languageSelector"
                        SelectLabel="Language <span class='required'>*</span>"
                        CountLabel="Language(s)"
                        Mode="Short" />
                </div>

                <div class="item even">
                    <mr:MultiSelector runat="server" ID="publicationDaysSelector"
                        SelectLabel="Publication Days"
                        CountLabel="Publication Day(s)"
                        Mode="Short" />
                </div>

                <div class="item odd">
                    <mr:MultiSelector runat="server" ID="specialtyPublicationsSelector"
                        SelectLabel="Specialty Publications"
                        CountLabel="Specialty Publication(s)"
                        Mode="Short" />
                </div>

                <div class="item even">
                    <mr:MultiSelector runat="server" ID="mediaTypeSelector"
                        SelectLabel="Media Type <span class='required'>*</span>"
                        ShowSelect="True"
                        SecondaryLabel="Print Category"
                        CountLabel="Media Type(s)"
                        Mode="Short" />
                </div>

                <div class="item odd">
                    <mr:MultiSelector runat="server" ID="ethnicitySelector"
                        SelectLabel="Ethnicity <span class='required' id='ethnicity_req'>*</span>"
                        CountLabel="Ethnicities"
                        Mode="Short" />
                </div>

                <div class="clear"></div>


                <script type="text/javascript">

                    function ToggleEthnicitySwitch() {
                        var element = document.getElementById('ethnicity_req');
                     
                        if (document.getElementById('<%= ethnicMediaRb.ClientID %>').checked) {
                            element.style.display = '';
                        } else {
                            element.style.display = 'none';
                        }

                    }

                    $(document).ready(function () {
                        ToggleEthnicitySwitch();
                    });

                </script>

            </div>

            <div class="media-outlet-bottom">

                <div class="item even">

                    <div style="float: left;">
                        <div class="field">
                            <img runat="server" class="field-changed" id="publicationFrequencyChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Publication Frequency
                        </div>
                        <span>
                            <asp:DropDownList ID="publicationFrequencyDD" runat="server" Style="width: 170px" /></span>
                    </div>


                    <div style="float: right;">
                        <div class="field">
                            <img runat="server" class="field-changed" id="liveMediaChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Live Media Opportunity
                        </div>
                        <span>
                            <asp:RadioButton ID="liveMediaYesRb" GroupName="liveMedia" runat="server" />
                            Yes 
                        <asp:RadioButton ID="liveMediaNoRb" GroupName="liveMedia" runat="server" Checked="true" />
                            No
                        </span>
                    </div>

                    <div style="float: right; margin-right: 30px;">
                        <div class="field">
                            <img runat="server" class="field-changed" id="majorMediaChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Major Media
                        </div>
                        <span>
                            <asp:RadioButton ID="majorMediaRb" GroupName="majorMediaGrp" runat="server" />
                            Yes 
                        <asp:RadioButton ID="notMjaorMediaRb" GroupName="majorMediaGrp" runat="server" Checked="true" />
                            No
                        </span>
                    </div>

                    <div class="clear"></div>

                </div>

                <div class="item odd">

                    <div>
                        <div class="field">
                            <img runat="server" class="field-changed" id="ethnicMediaChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Ethnic Media <span class='required'>*</span>
                        </div>
                        <span>
                            <asp:RadioButton GroupName="ethnicGrp" runat="server" ID="ethnicMediaRb" OnClick="ToggleEthnicitySwitch()" />
                            Yes
                        <asp:RadioButton GroupName="ethnicGrp" runat="server" ID="notEthnicMediaRb" Checked="true" OnClick="ToggleEthnicitySwitch()" />
                            No
                        </span>
                    </div>

                </div>

                <div class="clear"></div>

            </div>

            <div class="media-outlet-bottom">

                <div class="item even">
                    <div class="field">
                        <img runat="server" class="field-changed" id="keyProgramsChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Key Programs (Maximum 500 characters)
                    </div>
                    <asp:TextBox TextMode="MultiLine" runat="server" ID="keyProgramsTb" />
                    <div class="error" runat="server" id="keyProgramsError"></div>
                </div>

                <div class="item odd">
                    <div class="field">
                        <img runat="server" class="field-changed" id="circulationChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Circulation Description/Reach/Viewership (Maximum 500 characters)
                    </div>
                    <asp:TextBox TextMode="MultiLine" runat="server" ID="circulationTb" />
                    <div class="error" runat="server" id="circulationError"></div>
                </div>

                <div class="item even">
                    <div class="field">
                        <img runat="server" class="field-changed" id="deadlinesChanged" visible="false" runat="server" src="~/Contacts/images/AlertIcon.png" />Deadlines (Maximum 500 characters)
                    </div>
                    <asp:TextBox TextMode="MultiLine" runat="server" ID="deadlinesTb" />
                    <div class="error" runat="server" id="deadlinesError"></div>
                </div>

                <div class="clear"></div>
            </div>


            <script type="text/javascript">

                function CheckPageValidity() {
                    var valid = true;

                    document.getElementById('<%= distributionSelector.ErrorDiv.ClientID %>').innerHTML = "";
                    document.getElementById('<%= languageSelector.ErrorDiv.ClientID %>').innerHTML = "";
                    document.getElementById('<%= mediaTypeSelector.ErrorDiv.ClientID %>').innerHTML = "";
                    document.getElementById('<%= ethnicitySelector.ErrorDiv.ClientID %>').innerHTML = "";

                    document.getElementById('<%= keyProgramsError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= circulationError.ClientID %>').innerHTML = "";
                    document.getElementById('<%= deadlinesError.ClientID %>').innerHTML = "";

                    var errorFields = "";

                    if (multiselector_selectedCount['<%=distributionSelector.ClientID%>'] == null || multiselector_selectedCount['<%=distributionSelector.ClientID%>'] <= 0) {
                        valid = false;
                        errorFields += "Distribution\n";
                        document.getElementById('<%= distributionSelector.ErrorDiv.ClientID %>').innerHTML = "At least one distribution is required";
                    }

                    if (multiselector_selectedCount['<%=languageSelector.ClientID%>'] == null || multiselector_selectedCount['<%=languageSelector.ClientID%>'] <= 0) {
                        valid = false;
                        errorFields += "Language\n";
                        document.getElementById('<%= languageSelector.ErrorDiv.ClientID %>').innerHTML = "At least one language is required";
                    }

                    if (multiselector_selectedCount['<%=mediaTypeSelector.ClientID%>'] == null || multiselector_selectedCount['<%=mediaTypeSelector.ClientID%>'] <= 0) {
                        valid = false;
                        errorFields += "Media Type\n";
                        document.getElementById('<%= mediaTypeSelector.ErrorDiv.ClientID %>').innerHTML = "At least one media type is required";
                    }

                    if (document.getElementById('<%= ethnicMediaRb.ClientID %>').checked) {
                        if (multiselector_selectedCount['<%=ethnicitySelector.ClientID%>'] == null || multiselector_selectedCount['<%=ethnicitySelector.ClientID%>'] <= 0) {
                            valid = false;
                            errorFields += "Ethnicity\n";
                            document.getElementById('<%= ethnicitySelector.ErrorDiv.ClientID %>').innerHTML = "At least one ethnicity is required (ethnic media selected)";
                        }
                    }


                    if (document.getElementById('<%= keyProgramsTb.ClientID %>').value.trim().length > 500) {
                        errorFields += "Key Programs\n";
                        document.getElementById('<%= keyProgramsError.ClientID %>').innerHTML = "Must be less than 500 characters";
                        valid = false;
                    }

                    if (document.getElementById('<%= circulationTb.ClientID %>').value.trim().length > 500) {
                        errorFields += "Circulation Description\n";
                        document.getElementById('<%= circulationError.ClientID %>').innerHTML = "Must be less than 500 characters";
                        valid = false;
                    }

                    if (document.getElementById('<%= deadlinesTb.ClientID %>').value.trim().length > 500) {
                        errorFields += "Deadlines\n";
                        document.getElementById('<%= deadlinesError.ClientID %>').innerHTML = "Must be less than 500 characters";
                        valid = false;
                    }

                    if (!valid) {
                        alert(saveErrorText.replace("###errors###", errorFields));
                    }

                    return valid;
                }

            </script>

        </asp:Panel>

    </div>

</div>

<asp:Panel runat="server" ID="buttonContainerPanel">
    <asp:Button ID="cancelButton" runat="server" OnClick="CancelButtonClick" Text="Cancel" CssClass="common-admin-button" OnClientClick="doUnloadCheck=false;return confirm(cancelButtonText);" Visible="false" />
    <asp:Button ID="previousButton" runat="server" OnClick="PreviousButtonClick" Text="Prev" CssClass="common-admin-button" OnClientClick="doUnloadCheck=false;" Visible="false" />
    <asp:Button ID="nextButton" runat="server" OnClick="NextButtonClick" CssClass="common-admin-button" Text="Next" Visible="true" OnClientClick="doUnloadCheck=false;return CheckPageValidity();" />
    <asp:Button ID="saveButton" runat="server" OnClick="SaveButtonClick" CssClass="common-admin-button" Text="Save" Visible="false" OnClientClick="doUnloadCheck=false;return CheckPageValidity();" />


    <asp:Button ID="outletsButton" runat="server" OnClick="SaveOutletsClick" CssClass="common-admin-button" OnClientClick="doUnloadCheck=false;" Style="display: none" />

    <div class="common-back-button" runat="server" id="backButton" visible="false">
        <a href="/Search.aspx" class="common-back-button">Back To Search</a>
    </div>
</asp:Panel>
