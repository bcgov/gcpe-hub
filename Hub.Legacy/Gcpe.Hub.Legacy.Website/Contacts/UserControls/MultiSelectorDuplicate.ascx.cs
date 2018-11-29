using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Text;
using MediaRelationsLibrary;

public partial class UserControls_MultiSelectorDuplicate : System.Web.UI.UserControl
{
    /// <summary>
    /// structure is: <guid, <name, isSelected, validationRegex, textValue>>
    /// </summary>
    public Dictionary<string, MultiSelectorItem> Items { get; set; }

    public string Mode { get; set; }
    
    public string SelectLabel { get; set; }

    public string SecondaryLabel { get; set; }

    public string CountLabel { get; set; }

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

    private bool showExt = false;
    public bool ShowExt
    {
        get { return showExt; }
        set { showExt = value; }
    }

    private bool allowDuplicates = false;
    public bool AllowDuplicates {
        get { return allowDuplicates; }
        set { allowDuplicates = value; }
    }

    public Dictionary<String, String> Items2 { get; set; }

    public bool DoSecondaryValue
    {
        get
        {
            return (ShowTextField||ShowSelect);
        }
    }

    List<KeyValuePair<String,String>> selectedItems=new List<KeyValuePair<String,String>>();
    public List<KeyValuePair<String, String>> SelectedItems
    {
        get
        {
            return selectedItems;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        
    }

    public void Refresh()
    {
        CountLabelLiteral.Text = CountLabel;

        StringBuilder sb = new StringBuilder();
        StringBuilder hsb = new StringBuilder();
        StringBuilder initialItemsSb = new StringBuilder();
        StringBuilder initialSelectedSb = new StringBuilder();
        StringBuilder initialItems2Sb = new StringBuilder();

        selectLabel.Text = SelectLabel;
        select2Label.Text = SecondaryLabel;
        

        if (ShowSelect)
        {
            SelectPanel.Visible = true;

            if ((ShowSelect && Items2 != null))
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

        if (ShowTextField)
        {
            TextPanel.Visible = true;
        }
        else
        {
            TextPanel.Visible = false;
        }

        if (ShowExt)
        {
            ExtPanel.Visible = true;
        }
        else
        {
            ExtPanel.Visible = false;
        }

        sb.Append("multiselectorDuplicate_doText['" + this.ClientID + "']=" + (ShowTextField ? "true" : "false") + ";\n");
        sb.Append("multiselectorDuplicate_doSelect['" + this.ClientID + "']=" + (ShowSelect ? "true" : "false") + ";\n");
        sb.Append("multiselectorDuplicate_doExt['" + this.ClientID + "']=" + (ShowExt ? "true" : "false") + ";\n");

        if (Items != null)
        {
            sb.Append("multiselectorDuplicate_items['" + this.ClientID + "'] = new Array();\n");
            int selectedCount = 0;
            initialItemsSb.Append("<option value=''>Select</option>\n");
            foreach (string key in Items.Keys)
            {
                sb.Append("multiselectorDuplicate_items['" + this.ClientID + "']['" + key + "']=['" + Items[key].Name.Replace("'", "\\'") + "', " + (Items[key].Selected ? "true" : "false") + ", " + (string.IsNullOrWhiteSpace(Items[key].ValidationRegex) ? "/[\\w]+/" : "/" + Items[key].ValidationRegex + "/") + ", '" + Items[key].TextValue.Replace("'", "\\'") + "', " + (Items[key].ShowSubItem ? "true" : "false") + "];\n");

                //put it in the dropdown
                initialItemsSb.Append("<option value='" + key + "'>" + Items[key].Name + "</option>\n");
            }

            int rowId = 0;
            if ((ShowSelect && Items2 != null) || ShowTextField)
            {
                
                foreach (KeyValuePair<string, string> key in this.SelectedItems)
                {                    
                    if (ShowSelect && Items2 != null)
                    {
                        if (Items2.ContainsKey(key.Value))
                        {
                            hsb.Append(key.Key + "=" + key.Value + "|");
                        }
                    }
                    else
                    {
                        hsb.Append(key.Key + "=" + key.Value + "|");
                    }
                }
            }

            sb.Append("multiselectorDuplicate_isShort['" + this.ClientID + "']=" + (Mode != null && Mode.Equals("Short") ? "true" : "false") + ";\n");
            sb.Append("multiselectorDuplicate_selectedCount['" + this.ClientID + "'] = " + selectedCount + ";\n");
            NumberAdded.Text = selectedCount.ToString();

            hiddenFieldValue.Text = hsb.ToString();

            initialItems.Text = initialItemsSb.ToString();
            //initialSelectedItems.Text = initialSelectedSb.ToString();
            select2Items.Text = initialItems2Sb.ToString();

            sb.Append("multiselectorDuplicate_allowDuplicates['" + this.ClientID + "'] = " + (allowDuplicates ? "true" : "false") + ";\n");
            sb.Append("multiselectorDuplicate_subitemCount = " + rowId + ";\n");

            MultiSelectorScript.Text = sb.ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        
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

                                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(sval1, sval2);

                                if(!selectedItems.Contains(kvp)) selectedItems.Add(kvp);
                                if (Items.ContainsKey(sval1))
                                {
                                    Items[sval1].Selected = true;
                                    Items[sval1].TextValue = sval2;
                                }

                                selectedIds.Add(sval1);
                            }
                            else
                            {
                                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(value, "");
                                if(!selectedItems.Contains(kvp)) selectedItems.Add(kvp);
                                if(Items.ContainsKey(value)) Items[value].Selected = true;

                                selectedIds.Add(value);
                            }
                        }
                        else
                        {
                            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(value, "");
                            if(!selectedItems.Contains(kvp)) selectedItems.Add(kvp);

                            if(Items.ContainsKey(value)) Items[value].Selected = true;

                            selectedIds.Add(value);
                        }
                    }
                }

                List<String> keys = new List<String>();
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

        Refresh();
    }
}