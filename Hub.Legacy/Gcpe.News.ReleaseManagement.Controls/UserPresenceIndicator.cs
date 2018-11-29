using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace Gcpe.News.ReleaseManagement.Controls
{
    public class UserPresenceIndicator : System.Web.UI.WebControls.WebControl
    {
        public String Email { get; set; }
        public String DisplayName { get; set; }

        #region script literal
        /// <summary>
        /// constructs a client script to be included in the page only once (using Page.ClientScript)
        /// 
        /// this does contain some inline styles for some of the elements, which may be better pulled into a stylesheet
        /// on the other hand, maybe it is better to have them embedded here and save having to add an external resource?
        /// </summary>
        private String ScriptLiteral
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<script type='text/javascript'>");
                sb.AppendLine("if (typeof(gcpe)==\"undefined\") gcpe = {};");

                sb.AppendLine("gcpe.NameCtrlManager = {");
                sb.AppendLine("    nameCtrls: [],");
                sb.AppendLine("    addNameCtrl: function (name, displayName, displayDiv) {");
                sb.AppendLine("        var nameCtrl = new ActiveXObject('Name.NameCtrl.1');");
                sb.AppendLine("        if (nameCtrl.PresenceEnabled) {");
                sb.AppendLine("            nameCtrl.onStatusChange = gcpe.NameCtrlManager.onStatusChange;");
                sb.AppendLine("            nameCtrl.GetStatus(name, \"1\");");
                sb.AppendLine("        }");
                sb.AppendLine("        if (!this.nameCtrls[name]) this.nameCtrls[name] = nameCtrl;");

                sb.AppendLine("        {");
                sb.AppendLine("            var sipName = name;");

                sb.AppendLine("            var containerDiv = document.createElement(\"div\");");
                sb.AppendLine("            containerDiv.id = \"gcpe-namectrl-container_\" + name;");
                sb.AppendLine("            containerDiv.setAttribute(\"style\", \"cursor:default;font-family:Calibri;font-size:12px;overflow:hidden\");");

                sb.AppendLine("            containerDiv.style.cursor=\"default\";");
                sb.AppendLine("            containerDiv.style.fontFamily=\"Calibri\";");
                sb.AppendLine("            containerDiv.style.fontSize=\"12px\";");
                sb.AppendLine("            containerDiv.style.overflow=\"hidden\";");                    
                sb.AppendLine("            containerDiv.onmouseover = function () { gcpe.NameCtrlManager.showOOUI(sipName); }");
                sb.AppendLine("            containerDiv.onmouseout = function () { gcpe.NameCtrlManager.hideOOUI(sipName); }");

                sb.AppendLine("            var statusDiv = document.createElement(\"div\");");
                sb.AppendLine("            containerDiv.appendChild(statusDiv);");
                sb.AppendLine("            statusDiv.id = \"gcpe-namectrl-status_\" + name;");
                
                sb.AppendLine("            statusDiv.style.marginRight=\"5px\";");
                sb.AppendLine("            statusDiv.style.float=\"left\";");
                sb.AppendLine("            statusDiv.style.border=\"solid #999999 1px\";");
                sb.AppendLine("            statusDiv.style.borderRadius=\"2px\";");
                sb.AppendLine("            statusDiv.style.position=\"relative\";");
                sb.AppendLine("            statusDiv.style.width=\"10px\";");
                sb.AppendLine("            statusDiv.style.height=\"10px\";");

                sb.AppendLine("            var whitebarDiv = document.createElement(\"div\");");
                sb.AppendLine("            statusDiv.appendChild(whitebarDiv);");
                sb.AppendLine("            whitebarDiv.id = \"gcpe-namectrl-whitebar_\" + name;");
                sb.AppendLine("            whitebarDiv.style.display=\"none\";");
                sb.AppendLine("            whitebarDiv.style.position=\"absolute\";");
                sb.AppendLine("            whitebarDiv.style.width=\"6px\";");
                sb.AppendLine("            whitebarDiv.style.height=\"2px\";");
                sb.AppendLine("            whitebarDiv.style.background=\"#ffffff\";");
                sb.AppendLine("            whitebarDiv.style.top=\"4px\";");
                sb.AppendLine("            whitebarDiv.style.left=\"2px\";");

                sb.AppendLine("            var nameSpan = document.createElement(\"span\");");
                sb.AppendLine("            containerDiv.appendChild(nameSpan);");
                sb.AppendLine("            nameSpan.appendChild(document.createTextNode((displayName ? displayName : name)));");

                sb.AppendLine("            displayDiv.appendChild(containerDiv);");
                sb.AppendLine("        }");
                sb.AppendLine("    },");
                    //0   green       Online
                    //1   light red   Offline
                    //2   yellow      Away
                    //3   red       Busy
                    //4   yellow      Be right back
                    //5   red         On the phone
                    //6   red         Lunch
                    //7   red         Meeting
                    //8   red         Out of office
                    //9   STOP        Do not distrub
                    //15   STOP        Do not distrub but allowed (only urgent, communicator show red image)
                sb.AppendLine("    onStatusChange: function (name, status, id) {");
                sb.AppendLine("        var angle = \"150deg\"");
                
                sb.AppendLine("        var statusColors = {");
                sb.AppendLine("        0: \"#00ff00\","); //online
                sb.AppendLine("        1: \"#ffcccc\","); //offline
                sb.AppendLine("        2: \"#ffcc00\","); //away
                sb.AppendLine("        3: \"#ff0000\","); //busy
                sb.AppendLine("        4: \"#ffcc00\","); //brb
                sb.AppendLine("        5: \"#ff0000\","); //on phone
                sb.AppendLine("        6: \"#ff0000\","); //out to lunch
                sb.AppendLine("        7: \"#ff0000\","); //in meeting
                sb.AppendLine("        8: \"#ff0000\","); //out of office
                sb.AppendLine("        9: \"#990000\","); // do not disturb
                sb.AppendLine("        10: \"#999999\","); //???
                sb.AppendLine("        11: \"#00ff00\","); //online (out of office)
                sb.AppendLine("        12: \"#ffcccc\","); //offline (out of office)
                sb.AppendLine("        13: \"#ff0000\","); //busy (out of office)
                sb.AppendLine("        14: \"#999999\","); //???
                sb.AppendLine("        15: \"#990000\","); //do not disturb (out of office)
                sb.AppendLine("        16: \"#ffcc00\","); //idle
                sb.AppendLine("        17: \"#ffcc00\","); //idle (out of office)
                sb.AppendLine("        18: \"#000000\","); //blocked
                sb.AppendLine("        19: \"#ff9999\","); //idle-busy
                sb.AppendLine("        20: \"#ff9999\""); //idle-busy (out of office)
                sb.AppendLine("        }");

                sb.AppendLine("        var whitebarDisplays = {");
                sb.AppendLine("            0: \"none\",");
                sb.AppendLine("            1: \"none\",");
                sb.AppendLine("            2: \"none\",");
                sb.AppendLine("            3: \"none\",");
                sb.AppendLine("            4: \"none\",");
                sb.AppendLine("            5: \"none\",");
                sb.AppendLine("            6: \"none\",");
                sb.AppendLine("            7: \"none\",");
                sb.AppendLine("            8: \"none\",");
                sb.AppendLine("            9: \"\",");
                sb.AppendLine("            15: \"\"");
                sb.AppendLine("        }");

                sb.AppendLine("        var statusDiv = document.getElementById(\"gcpe-namectrl-status_\"+name);");
                sb.AppendLine("        try {");
                sb.AppendLine("            statusDiv.style.background = statusColors[status];");
                sb.AppendLine("            statusDiv.style.background = \"linear-gradient(\" + angle + \", #ffffff, \" + statusColors[status] + \", #ffffff)\";");
                sb.AppendLine("        } catch(e) {");
                sb.AppendLine("            statusDiv.style.background = statusColors[status];");
                sb.AppendLine("        }");

                sb.AppendLine("        var whitebarDiv = document.getElementById(\"gcpe-namectrl-whitebar_\"+name);");
                sb.AppendLine("        whitebarDiv.style.display = whitebarDisplays[status];");
                sb.AppendLine("    },");
                sb.AppendLine("    showOOUI: function (name) {");
                sb.AppendLine("        this.nameCtrls[name].ShowOOUI(name, 0, 15, 15);");
                sb.AppendLine("    },");
                sb.AppendLine("    hideOOUI: function (name) {");
                sb.AppendLine("        this.nameCtrls[name].HideOOUI();");
                sb.AppendLine("    }");
                sb.AppendLine("}");
                sb.AppendLine("</script>");

                return sb.ToString();
            }
        }
        #endregion

        /// <summary>
        /// override pre render event to register client script
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            String clientScriptKey = "Gcpe.News.ReleaseManagement.Controls.UserPresenceIndicator";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(Page.GetType(), clientScriptKey))
            {
                Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), clientScriptKey, ScriptLiteral);
            }
            base.OnPreRender(e);
        }

        /// <summary>
        /// render the control
        /// 
        /// This method writes a container div and then uses the script it registered to populate it with the activex control for user presence
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.WriteLine("<div id='UserPresence" + this.ClientID + "'></div>");
            writer.WriteLine("<script type='text/javascript'>");
            writer.WriteLine("gcpe.NameCtrlManager.addNameCtrl('" + Email + "', '" + DisplayName + "', document.getElementById('UserPresence" + this.ClientID + "'));");
            writer.WriteLine("</script>");
            base.Render(writer);
        }
    }
}
