<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserControls_AddEditContact" CodeBehind="AddEditContact.ascx.cs" %>
<%@ Register TagPrefix="mr" TagName="MultiSelector" Src="~/Contacts/UserControls/MultiSelector.ascx" %>
<%@ Register TagPrefix="mr" TagName="MultiSelectorDuplicate" Src="~/Contacts/UserControls/MultiSelectorDuplicate.ascx" %>
<%@ Register TagPrefix="mr" TagName="TabControl" Src="~/Contacts/UserControls/TabControl.ascx" %>

<div runat="server" id="ErrorNotice" class="ErrorNotice"><asp:Literal ID="ErrorLit" runat="server" /></div>
<asp:Panel ID="FormPanel" runat="server" CssClass="common-page-box-border">
    <script type="text/javascript">
        <asp:Literal ID="ScriptLiteral" runat="server"/>
    </script>

    <mr:TabControl ID="TabControl" runat="server" />
    <div class="common-page-tabs-inner">
        <asp:Panel ID="ContactPanel" runat="server" Visible="false">
            <asp:HiddenField runat="server" ID="timestamp" />
            <div class="edit-form-container" style="margin-bottom: 50px;">
                <div>
                    <div class="field">
                        <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="FirstNameChangedIcon" visible="false" />
                        First Name <span class="required">*</span>
                    </div>
                    <asp:TextBox ID="FirstName" runat="server" MaxLength="150" />
                    <div id="FirstNameError" runat="server" class="error" />
                </div>
                <div>
                    <div class="field">
                        <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="LastNameChangedIcon" visible="false" />
                        Last Name <span class="required">*</span>
                    </div>
                    <asp:TextBox ID="LastName" runat="server" MaxLength="150" />
                    <div id="LastNameError" runat="server" class="error" />
                </div>
            </div>

            <div class="multi-select-container">
                <div class="item even" id="multiSelectorContainer" runat="server">
                    <mr:MultiSelectorDuplicate ID="MediaJobTitleSelector" AllowDuplicates="True" ShowSelect="True" SelectLabel="Media Job Title" SecondaryLabel="Outlet" runat="server" Mode="Short" CountLabel="Media Job Title(s)" />
                </div>
                <div class="item odd">
                    <mr:MultiSelectorDuplicate ID="PhoneNumberSelector" ShowExt="true" AllowDuplicates="true" MaxLength="250" Mode="Short" SelectLabel="Phone Number <span class='required'>*</span>" SecondaryLabel="Enter Number" ShowSelect="false" ShowTextField="true" runat="server" CountLabel="Phone Number(s)" />
                </div>
                <div class="item odd">
                    <div class="field">
                        <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="ShowNotesChangedIcon" visible="false" />
                        Show Notes (100 characters max.)
                    </div>
                    <asp:TextBox ID="ShowNotes" TextMode="MultiLine" MaxLength="100" runat="server" onkeyup="if(this.value.length>100) { this.value=this.value.substring(0, 100) }" />
                    <div id="ShowNotesError" runat="server" class="error" />
                </div>

            </div>

            <script type="text/javascript">
            
                function ConfirmValidPage(isBackButton) {
                    //contacts tab 
                    //first name, last name, phone number required

                    //location tab
                    //city, province, country, region, electoral district required

                    //ministry tab
                    //ministry, mla assignment, minister assignment, minister assistant required

                    var errorFields = "";

                    document.getElementById('<%=FirstNameError.ClientID%>').innerHTML = "";
                    document.getElementById('<%=LastNameError.ClientID%>').innerHTML = "";
                    document.getElementById('<%=PhoneNumberSelector.ErrorDiv.ClientID%>').innerHTML = "";

                    if (/^[\\s]*$/.test(document.getElementById('<%=FirstName.ClientID%>').value)) {
                        errorFields += "First Name\n";
                        document.getElementById('<%=FirstNameError.ClientID%>').innerHTML = "First name must not be empty";
                    }
                    if (/^[\\s]*$/.test(document.getElementById('<%=LastName.ClientID%>').value)) {
                        errorFields += "Last Name\n";
                        document.getElementById('<%=LastNameError.ClientID%>').innerHTML = "Last name must not be empty";
                    }
                    var phoneError=false;
                    var phoneErrorText="";
                    if (multiselectorDuplicate_selectedCount['<%=PhoneNumberSelector.ClientID%>'] == null || multiselector_selectedCount['<%=PhoneNumberSelector.ClientID%>'] <= 0) {
                        phoneError=true;
                        phoneErrorText += "Please add at least one phone number<br/>\n";
                    }
                    var foundPrimary=false;
                    var formatError=false;
                    var hidPhone = document.getElementById("multiselector_selectedIds_<%=PhoneNumberSelector.ClientID%>").value.split("|");
                    for(var i=0; i<hidPhone.length; i++) {
                        var eqIndex = hidPhone[i].indexOf("=");
                        if(eqIndex>0) {
                            /*var hidPhoneSpl1 = hidPhone[i].substring(0, eqIndex);
                            var hidPhoneSpl2 = hidPhone[i].substring(eqIndex+1);

                            if(!phoneRegex.test(hidPhoneSpl2)) {
                                phoneError=true;
                                if(!formatError) phoneErrorText+="Phone Number format is invalid<br/>\n";
                                formatError=true;
                            } else {
                                if(hidPhoneSpl1.toLowerCase() == primaryPhoneType.toLowerCase()) foundPrimary = true;
                            }*/
                        } 
                    
                    }
                    //if(!foundPrimary) {
                    //    phoneError=true;
                    //    phoneErrorText+="Please add a primary phone number<br/>\n";
                    //}


                    if(phoneError) {
                        errorFields += "Phone Numbers\n";
                        document.getElementById('<%=PhoneNumberSelector.ErrorDiv.ClientID%>').innerHTML = phoneErrorText;
                    }

                    if (errorFields != "") {
                        alert(saveErrorText.replace("###errors###", errorFields));
                        document.getElementById("tabHiddenField").value = "";
                        return false;
                    }
                
                    return true;
                }
            </script>
        </asp:Panel>
        <asp:Panel ID="WebAddressPanel" runat="server" Visible="false">

            <asp:UpdatePanel ID="UpdatePanelWebAddress" runat="server" OnLoad="UpdatePanelWebAddressLoad">
                <ContentTemplate>
                    <table style="width: 100%;">
                        <tr style="vertical-align: top">
                            <td style="width: 60%;">
                                <asp:GridView ID="GridViewWebAddress" AutoGenerateColumns="false" runat="server" OnSelectedIndexChanging="GridViewWebAddress_SelectWebAddress" OnDataBound="GridViewWebAddress_DataBound"  OnRowDeleting="GridViewWebAddress_RowDeleting" Width="600px" CssClass="common-admin-table">
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
                                    <div style="padding-top: 5px;">
                                        <asp:Label ID="lblWebAddress" runat="server" Text="Label" Visible="false"></asp:Label>
                                        <asp:DropDownList ID="cboAddressTypes" runat="server" Visible="false" OnSelectedIndexChanged="cboAddressTypes_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                        <asp:TextBox ID="txtWebAddress" runat="server" OnTextChanged="txtWebAddress_TextChanged" AutoPostBack="true" Width="420px"></asp:TextBox>
                                    </div>
                                    <div style="padding-top: 5px;">
                                        <asp:Label ID="mediaDistributionListBoxLabel" runat="server" Text="Media Distribution Lists" Visible="true" Width="250px"></asp:Label>
                                        <asp:ListBox ID="mediaDistributionListBox" SelectionMode="Multiple" AutoPostBack="false" runat="server"></asp:ListBox>
                                    </div>
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
                            <td style="padding-left: 10px;">
                                <asp:Label ID="GridViewOtherInfoLabel" runat="server" Text="" Visible="true"></asp:Label>
                                <asp:GridView ID="GridViewOtherInfo" AutoGenerateColumns="false" runat="server" Width="100%" CssClass="common-admin-table">
                                    <Columns>
                                        <asp:BoundField DataField="Type" HeaderText="Used By" />
                                        <asp:BoundField DataField="Name" HeaderText="Usage" />
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



            <script type="text/javascript">
                function ConfirmValidPage(isBackButton) {
                    return true;
                }
            </script>
        </asp:Panel>
        <asp:Panel ID="LocationPanel" runat="server" Visible="false">
            <div class="edit-form-container" style="margin-bottom: 50px">
                <div class="field">
                    <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="StreetAddressChangedIcon" visible="false" />
                    Address (maximum 250 characters)
                </div>
                <asp:TextBox TextMode="MultiLine" ID="Address" runat="server" CssClass="textarea-250" />
                <div id="AddressError" runat="server" class="error" />


                <div>
                    <div style="float: left">
                        <div class="field">
                            <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="CityChangedIcon" visible="false" />
                            City <span class="required">*</span>
                        </div>
                        <asp:DropDownList ID="City" runat="server" OnChange="cityChanged(this);" />
                    </div>
                    <div style="display: none; float: left; margin-left: 20px;" id="CustomCityDiv" runat="server">
                        <div class="field">
                            <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="CustomCityChangedIcon" visible="false" />
                            Custom
                        </div>
                        <asp:TextBox ID="CustomCity" runat="server" />
                    </div>
                    <div style="clear: both"></div>
                </div>


                <div>
                    <div style="float: left;">
                        <div class="field">
                            <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="ProvinceChangedIcon" visible="false" />
                            Province <span class="required">*</span>
                        </div>
                        <asp:DropDownList ID="Province" runat="server" OnChange="provinceChanged(this)" />
                    </div>
                    <div style="float: left; margin-left: 20px; display: none;" id="CustomProvinceDiv" runat="server">
                        <div class="field">
                            <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="CustomProvinceChangedIcon" visible="false" />
                            Custom
                        </div>
                        <asp:TextBox ID="CustomProvince" runat="server" />
                    </div>
                    <div style="clear: both"></div>
                </div>
                <div class="field">
                    <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="CountryChangedIcon" visible="false" />
                    Country <span class="required">*</span>
                </div>
                <asp:DropDownList ID="Country" runat="server" />
                <div id="CountryError" runat="server" class="error" />
                <div class="field">
                    <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="PostalCodeChangedIcon" visible="false" />
                    Postal Code
                </div>
                <asp:TextBox ID="PostalCode" MaxLength="50" runat="server" />
                <div id="PostalError" runat="server" class="error" />
            </div>

            <div class="multi-select-container">

                <div class="item even">
                    <mr:MultiSelector ID="RegionSelector" SelectLabel="Region(s)" runat="server" CountLabel="Region(s)" Mode="Short" />
                </div>

                <div class="item odd">
                    <mr:MultiSelector ID="ElectoralDistrictSelector" SelectLabel="Electoral District(s)" runat="server" CountLabel="Electoral District(s)" Mode="Short" />
                </div>

                <div class="item even">
                    <mr:MultiSelector ID="SectorSelector" SelectLabel="Sector (add all that apply)" runat="server" CountLabel="Sector(s)" Mode="Short" />
                </div>

                <div class="clear"></div>

            </div>

            <script type="text/javascript">
                function cityChanged(sel) {
                    //var cityDiv = document.getElementById("<%=CustomCityDiv.ClientID%>");
                    //var cityTxt = document.getElementById("<%=CustomCity.ClientID%>");
                    //if(sel.options[sel.selectedIndex].value=="") {
                    //    cityDiv.style.display="";
                    //} else {
                    //    cityDiv.style.display="none";
                    //    cityTxt.value="";
                    //}
                }
                function provinceChanged(sel) {
                    //var provinceDiv = document.getElementById("<%=CustomProvinceDiv.ClientID%>");
                    //var provinceTxt = document.getElementById("<%=CustomProvince.ClientID%>");
                    //if(sel.options[sel.selectedIndex].value=="") {
                    //    provinceDiv.style.display="";
                    //} else {
                    //    provinceDiv.style.display="none";
                    //    provinceTxt.value="";
                    //}
                }
                function ConfirmValidPage(isBackButton) {
                    //contacts tab 
                    //first name, last name, phone number required

                    //location tab
                    //city, province, country, region, electoral district required

                    //ministry tab
                    //ministry, mla assignment, minister assignment, minister assistant required

                    var errorFields = "";
                    document.getElementById('<%=AddressError.ClientID%>').innerHTML = "";
                    document.getElementById('<%=CountryError.ClientID%>').innerHTML = "";
                    document.getElementById('<%=RegionSelector.ErrorDiv.ClientID%>').innerHTML = "";
                    document.getElementById('<%=ElectoralDistrictSelector.ErrorDiv.ClientID%>').innerHTML = "";

                    if(document.getElementById('<%=Address.ClientID%>').value.length>250) {
                        errorFields+="Street Address\n";
                        document.getElementById('<%=AddressError.ClientID%>').innerHTML="Street Address must be less than 250 characters";
                    }
                    if (!isBackButton&&document.getElementById('<%=Country.ClientID%>').selectedIndex <= 0) {
                        if (document.getElementById('<%=Province.ClientID%>').selectedIndex>0 || document.getElementById('<%=City.ClientID%>').selectedIndex>0) {
                            errorFields += "Country\n";
                            document.getElementById('<%=CountryError.ClientID%>').innerHTML = "Please select a country";
                        }
                    }

                    if (errorFields != "") {
                        alert(saveErrorText.replace("###errors###", errorFields));
                        document.getElementById("tabHiddenField").value = "";
                        return false;
                    }

                    return true;
                }
            </script>
        </asp:Panel>
        <asp:Panel ID="MediaPanel" runat="server" Visible="false">
            <div runat="server" id="beatSelectorContainer">
                <mr:MultiSelectorDuplicate ID="BeatSelector" AllowDuplicates="True" ShowSelect="true" SelectLabel="Beat" SecondaryLabel="Outlet" runat="server" Mode="Short" CountLabel="Beat(s)" />
            </div>

            <div class="field">
                <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="PressGalleryChangedIcon" visible="false" />
                Press Gallery <span class="required">*</span>
            </div>
            <asp:RadioButtonList ID="PressGallery" runat="server">
                <asp:ListItem Value="true" Text="Yes" />
                <asp:ListItem Value="false" Text="No" />
            </asp:RadioButtonList>
            <div id="PressGalleryError" runat="server" class="error" />

            <script type="text/javascript">
                function ConfirmValidPage(isBackButton) {
                    return true;
                }
            </script>
        </asp:Panel>
        <asp:Panel ID="MinistryPanel" runat="server" Visible="false" CssClass="edit-form-container">
            <div class="field">
                <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="MinistryNameChangedIcon" visible="false" />
                Ministry Name
            </div>
            <asp:DropDownList ID="Ministry" runat="server" onchange="setMinistrySelects(true)" />
            <div id="MinistryError" runat="server" class="error" />
            <div class="field">
                <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="MinisterialJobTitleChangedIcon" visible="false" />
                Ministerial Job Title <span id="ministerialJobTitleStar" class="required">*</span>
            </div>
            <asp:DropDownList ID="MinisterialJobTitle" runat="server" onchange="setMinistrySelects(false)" />
            <div id="MinisterialJobTitleError" runat="server" class="error" />
            <div class="field">
                <img runat="server" src="~/Contacts/images/AlertIcon.png" class="field-changed" id="MLAAssignmentChangedIcon" visible="false" />
                MLA Assignment <span id="mlaAssignmentStar" class="required">*</span>
            </div>
            <asp:DropDownList ID="MLAAssignment" runat="server" />
            <div id="MLAAssignmentError" runat="server" class="error" />


            <script type="text/javascript">
                var ministrySelect=document.getElementById("<%=Ministry.ClientID%>");
                var ministerialJobTitleSelect=document.getElementById("<%=MinisterialJobTitle.ClientID%>");
                var mlaAssignmentSelect=document.getElementById("<%=MLAAssignment.ClientID%>");

                var ministerialJobTitlsStar=document.getElementById("ministerialJobTitleStar");
                var mlaAssignmentStar=document.getElementById("mlaAssignmentStar");

                function setMinistrySelects() {
                    if(ministrySelect.selectedIndex<0||ministrySelect.options[ministrySelect.selectedIndex].value=="") {
                        //disable
                        ministerialJobTitleSelect.selectedIndex=0;
                        ministerialJobTitleSelect.disabled=true;
                        ministerialJobTitleStar.style.display="none";

                        mlaAssignmentSelect.selectedIndex=0;
                        mlaAssignmentSelect.disabled=true;
                        mlaAssignmentStar.style.display="none";
                    } else {
                        //enable ministerialjobtitle
                        ministerialJobTitleSelect.disabled=false;
                        ministerialJobTitleStar.style.display="";

                        if(ministerialJobTitleSelect.selectedIndex>0
                            &&ministerialJobTitleSelect.options[ministerialJobTitleSelect.selectedIndex].text.toLowerCase()=="minister") {
                            mlaAssignmentSelect.disabled=false;
                            mlaAssignmentStar.style.display="";
                        } else {
                            mlaAssignmentSelect.selectedIndex=0;
                            mlaAssignmentSelect.disabled=true;
                            mlaAssignmentStar.style.display="none";
                        }
                    }
                }

                setMinistrySelects(false);
                
                function ConfirmValidPage(isBackButton) {
                    var errorFields = "";
                    document.getElementById('<%=MinistryError.ClientID%>').innerHTML = "";
                    document.getElementById('<%=MinisterialJobTitleError.ClientID%>').innerHTML = "";
                    document.getElementById('<%=MLAAssignmentError.ClientID%>').innerHTML = "";

                    if(!isBackButton) {
                        if(ministrySelect.selectedIndex<0||ministrySelect.options[ministrySelect.selectedIndex].value=="") {
                            //no requirements
                        } else {
                            //enable ministerialjobtitle
                            if(ministerialJobTitleSelect.selectedIndex<=0) {
                                errorFields+="Ministerial Job Title\n";
                            } else {
                                if(ministerialJobTitleSelect.options[ministerialJobTitleSelect.selectedIndex].text.toLowerCase()=="minister") {
                                    if(mlaAssignmentSelect.selectedIndex<=0) {
                                        errorFields+="MLA Assignment\n";
                                    }
                                }
                            }
                        }
                    }

                    if (errorFields != "") {
                        alert(saveErrorText.replace("###errors###", errorFields));
                        document.getElementById("tabHiddenField").value = "";
                        return false;
                    }

                    return true;
                }
            </script>
        </asp:Panel>
    </div>

    <input type="hidden" id="tabHiddenField" name="tabHiddenField" />


    <script type="text/javascript">
        function DoSave(tab) {
            var okToContinue=true;
            if(doUnloadCheck) {
                okToContinue = confirm(tabConfirmText);
            }
            if(okToContinue) {
                document.getElementById("tabHiddenField").value = tab;
                document.getElementById('<%=SaveButton.ClientID%>').click();
            }
            return false;
        }

        <asp:Literal ID="AlertLiteral" runat="server"/>
    </script>
</asp:Panel>

<asp:Panel runat="server" ID="buttonContainerPanel">
    <asp:Button ID="CancelButton" CssClass="common-admin-button" Text="Cancel" runat="server" OnClientClick="doUnloadCheck=false;return confirm(cancelButtonText)" OnClick="CancelButton_Click" />
    <asp:Button ID="BackButton" CssClass="common-admin-button" Text="Prev" runat="server" OnClientClick="doUnloadCheck=false;return ConfirmValidPage(true);" OnClick="BackButton_Click" />
    <asp:Button ID="NextButton" CssClass="common-admin-button" Text="Next" runat="server" OnClientClick="doUnloadCheck=false;return ConfirmValidPage(false);" OnClick="NextButton_Click" />
    <asp:Button ID="SaveButton" CssClass="common-admin-button" Text="Save" runat="server" OnClientClick="doUnloadCheck=false;return ConfirmValidPage(false);" OnClick="SaveButton_Click" />
</asp:Panel>
