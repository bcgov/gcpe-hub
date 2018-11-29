using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Text;
using MediaRelationsLibrary;

public partial class UserControls_MultiSelector : System.Web.UI.UserControl
{
    /// <summary>
    /// structure is: <guid, <name, isSelected, validationRegex, textValue>>
    /// </summary>
    public Dictionary<string, MultiSelectorItem> Items { get; set; }

    public string Mode { get; set; }
    
    public string SelectLabel { get; set; }

    public string SecondaryLabel { get; set; }

    public string CountLabel { get; set; }

    public int MaxLength { get; set; }

    public string ErrorMessage
    {
        get
        {
            return ErrorMessageDiv.InnerHtml;
        }
        set
        {
            ErrorMessageDiv.InnerHtml = value;
        }
    }

    public HtmlGenericControl ErrorDiv
    {
        get
        {
            return ErrorMessageDiv;
        }
    }

    private bool showTextField = false;
    public bool ShowTextField {
        get { return showTextField; }
        set { showTextField = value; }
    }

    private bool showSelect = false;
    public bool ShowSelect {
        get { return showSelect; }
        set { showSelect = value; }
    }

    public Dictionary<String, String> Items2 { get; set; }

    public bool DoSecondaryValue
    {
        get
        {
            return (ShowTextField||ShowSelect);
        }
    }

    Dictionary<String,String> selectedItems=new Dictionary<String,String>();
    public Dictionary<String, String> SelectedItems
    {
        get
        {
            return selectedItems;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        CountLabelLiteral.Text = CountLabel;
        if (IsPostBack)
        {
            selectedItems.Clear();
            if (Request.Form["multiselector_selectedIds_" + this.ClientID] != null)
            {
                //Response.Write("multiselector_selectedIds_" + this.ClientID + "<br/>");
                String[] values = Request.Form["multiselector_selectedIds_" + this.ClientID].Split('|');

                List<String> selectedIds = new List<String>();

                foreach (string value in values)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (DoSecondaryValue)
                        {
                            int eqIndex = value.IndexOf("=");
                            
                            if (eqIndex >= 0)
                            {
                                string sval1 = value.Substring(0, eqIndex);
                                string sval2 = value.Substring(eqIndex + 1);

                                if(selectedItems.ContainsKey(sval1)) selectedItems[sval1]=sval2;
                                else selectedItems.Add(sval1, sval2);

                                if (Items.ContainsKey(sval1))
                                {
                                    Items[sval1].Selected = true;
                                    Items[sval1].TextValue = sval2;
                                }

                                selectedIds.Add(sval1);
                            }
                            else
                            {
                                if (selectedItems.ContainsKey(value)) selectedItems[value] = "";
                                else selectedItems.Add(value, "");

                                if(Items.ContainsKey(value)) Items[value].Selected = true;

                                selectedIds.Add(value);
                            }
                        }
                        else
                        {
                            if (selectedItems.ContainsKey(value)) selectedItems[value] = "";
                            else selectedItems.Add(value, "");

                            if(Items.ContainsKey(value)) Items[value].Selected = true;

                            selectedIds.Add(value);
                        }
                    }
                }

                List<String> keys = new List<String>();
                if (Items != null)
                {
                    foreach (string key in Items.Keys)
                    {
                        keys.Add(key);
                    }
                    foreach (string key in keys)
                    {
                        Items[key].Selected = (selectedIds.Contains(key));
                        if (!Items[key].Selected) Items[key].TextValue = "";
                    }
                }
            }
        }

        StringBuilder sb = new StringBuilder();
        StringBuilder hsb=new StringBuilder();
        StringBuilder initialItemsSb = new StringBuilder();
        StringBuilder initialSelectedSb = new StringBuilder();
        StringBuilder initialItems2Sb = new StringBuilder();

        selectLabel.Text = SelectLabel;
        textLabel.Text = SecondaryLabel;
        select2Label.Text = SecondaryLabel;
        if (ShowTextField)
        {
            TextPanel.Visible = true;
        }
        else
        {
            TextPanel.Visible = false;
        }

        if (ShowSelect)
        {
            SelectPanel.Visible = true;

            if (Items2 != null)
            {
                initialItems2Sb.Append("<option value=''>Select</option>\n");
                foreach (string key in Items2.Keys)
                {
                    initialItems2Sb.Append("<option value='" + key + "'>" + Items2[key] + "</option>\n");
                }
            }
        }
        else
        {
            SelectPanel.Visible = false;
        }

        sb.Append("multiselector_doText['"+this.ClientID+"']=" + (ShowTextField ? "true" : "false") + ";\n");
        sb.Append("multiselector_doSelect['" + this.ClientID + "']=" + (ShowSelect ? "true" : "false") + ";\n");

        if (Items != null)
        {
            sb.Append("multiselector_items['" + this.ClientID + "'] = new Array();\n");
            int selectedCount = 0;
            initialItemsSb.Append("<option value=''>Select</option>\n");
            foreach (string key in Items.Keys)
            {
                sb.Append("multiselector_items['" + this.ClientID + "']['" + key + "']=['" + Items[key].Name.Replace("'", "\\'") + "', "+ (Items[key].Selected?"true":"false") + ", "+(string.IsNullOrWhiteSpace(Items[key].ValidationRegex)?"/[\\w]+/":"/"+Items[key].ValidationRegex+"/")+", '"+Items[key].TextValue.Replace("'", "\\'")+"', "+(Items[key].ShowSubItem?"true":"false")+"];\n");
                if (Items[key].Selected)
                {
                    hsb.Append(key + (ShowTextField?"=" + Items[key].TextValue:"") + (ShowSelect&&Items[key].ShowSubItem?"="+Items[key].TextValue:"") + "|");
                }

                initialItemsSb.Append("<option value='" + key + "'>" + Items[key].Name + "</option>\n");
            }

            sb.Append("multiselector_selectedCount['" + this.ClientID + "'] = "+selectedCount+";\n");
            NumberAdded.Text = selectedCount.ToString();

            sb.Append("multiselector_checkSubItem(document.getElementById('multiselector_srcSelect_" + this.ClientID+"'));\n");
            sb.Append("multiselector_isShort['" + this.ClientID + "']=" + (Mode != null && Mode.Equals("Short") ? "true" : "false") + ";\n");
            sb.Append("multiselector_setPlaceholder('" + this.ClientID + "');\n");

            hiddenFieldValue.Text = hsb.ToString();

            initialItems.Text = initialItemsSb.ToString();
            //initialSelectedItems.Text = initialSelectedSb.ToString();
            select2Items.Text = initialItems2Sb.ToString();

            MultiSelectorScript.Text = sb.ToString();
        }
    }
}