<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserControls_MultiSelector" Codebehind="MultiSelector.ascx.cs" %>
<div class="multiselector-wrapper">
<div class="multiselector-container<%=(this.Mode!=null&&this.Mode=="Short"?"-short":"")%>">
    <div class="multiselector-left-column<%=(this.Mode!=null&&this.Mode=="Short"?"-short":"")%> gradient">
        <div class="search-label"><asp:Literal ID="selectLabel" runat="server" /></div>
        <select name="multiselector_srcSelect_<%=this.ClientID%>" id="multiselector_srcSelect_<%=this.ClientID%>" onchange="multiselector_checkSubItem(this);">
            <asp:Literal ID="initialItems" runat="server" />
        </select><br />

  
        <div class="multiselector-sub-item" id="multiselector_subitem_<%=this.ClientID%>" style="display:none;">
            <asp:Panel ID="TextPanel" runat="server">
                <div class="search-label"><asp:Literal ID="textLabel" runat="server" /></div>
                <input type="text" class="textinput" name="multiselector_srcText_<%=this.ClientID%>" id="multiselector_srcText_<%=this.ClientID%>" <%=(MaxLength==null||MaxLength<=0?"":"maxlength='"+MaxLength+"'")%> />
            </asp:Panel>

            <asp:Panel ID="SelectPanel" runat="server">
                <div class="search-label"><asp:Literal ID="select2Label" runat="server" /></div>
                <select name="multiselector_srcSelect2_<%=this.ClientID%>" id="multiselector_srcSelect2_<%=this.ClientID%>" onchange="multiselector_setButtonDisabled('<%=this.ClientID%>')">
                    <asp:Literal ID="select2Items" runat="server" />
                </select>
            </asp:Panel>

        </div>
      

        <div class="multiselector-add-link"><input type="button" id="multiselector_addButton_<%=this.ClientID%>" class="gradient" onclick="multiselector_addItem('<%=this.ClientID%>', <%=(this.Mode!=null&&this.Mode=="Short"?"true":"false")%>);return false;" value="Add"/></div> 
    </div>
    <div class="multiselector-right-column<%=(this.Mode!=null&&this.Mode=="Short"?"-short":"")%>">
        <div class="selected-items-label search-label"><span id="numberAddedLabel_<%=this.ClientID%>"><asp:Literal ID="NumberAdded" runat="server" /></span> <asp:Literal ID="CountLabelLiteral" runat="server"/> added</div>
        <div class="multiselector-selected-items<%=(this.Mode!=null&&this.Mode=="Short"?"-short":"")%>" ID="multiselector_selectedItems_<%=this.ClientID%>">
            <asp:Literal ID="initialSelectedItems" runat="server" />
        </div>
        <input type="hidden" id="multiselector_selectedIds_<%=this.ClientID%>" name="multiselector_selectedIds_<%=this.ClientID%>" value="<asp:Literal ID="hiddenFieldValue" runat="server" />" />
    </div>
</div>
<div ID="ErrorMessageDiv" runat="server" class="error" />
</div>

<script type="text/javascript">
    window.onunload=function() {}
    
    <asp:Literal ID="MultiSelectorScript" runat="server"/>

    

    $(document).ready(function() {
        multiselector_initItems("<%=this.ClientID%>");
    });
</script>
