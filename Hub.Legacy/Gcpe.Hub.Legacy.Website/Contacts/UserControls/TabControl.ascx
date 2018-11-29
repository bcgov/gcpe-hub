<%@ Control Language="C#" AutoEventWireup="true" Inherits="UserControls_TabControl" Codebehind="TabControl.ascx.cs" %>

<div ID="CssClassDiv" runat="server">
    <div class="common-page-tabs desktop" runat="server" id="tabsContainerDiv">
        <asp:Literal runat="server" ID="tabLit"/>
        <span style="display:block;clear:both"></span>
    </div>

    <asp:Literal runat="server" ID="tabsScriptLit" />

    <script type="text/javascript">

        function ChangeActiveTab(tabName) {

            tabControlCurrentTab = tabName;

            var container = document.getElementById('<%= tabsContainerDiv.ClientID %>');
            container.innerHTML = "";

            for (var i = 0; i < tabControlTabs.length; i++) {

                var item = tabControlTabs[i];

                if (item[0].toLowerCase() == tabName.toLowerCase()) {

                    var el = document.createElement("div");
                    el.innerHTML = AddSpacesCamelCase(tabControlCurrentTab);
                    el.className = "selected bluegradient num" + tabControlTabs.length + (i == 0 ? " first" : "") + (i == tabControlTabs.length - 1 ? " last" : "");
                    container.appendChild(el);

                } else {
                    var el = document.createElement('a');                    
                    el.innerHTML = AddSpacesCamelCase(item[0]);
                    el.href = item[1];

                    el.onclick = function () {
                        var str = onClickMethod + "('" + RemoveSpaces(this.innerHTML) + "')";
                        return eval(str);
                    };

                    el.className = "gradient num" + tabControlTabs.length + (i == 0 ? " first" : "") + (i == tabControlTabs.length - 1 ? " last" : "");
                    container.appendChild(el);
                }

            }

            var span = document.createElement("span");
            span.style.display = "block";
            span.style.clear = "both";
            container.appendChild(span);

        }

    </script>
</div>
