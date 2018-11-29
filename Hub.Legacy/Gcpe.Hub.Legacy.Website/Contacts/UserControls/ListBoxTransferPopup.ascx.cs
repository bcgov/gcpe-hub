using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class UserControls_ListBoxTransferPopup : System.Web.UI.UserControl
{

    public string CallBackMode { get; set; }
    public string ContainerClientId { get; set; }

    public string AvailableListHeader { get; set; }
    public string SelectedListHeader { get; set; }

    public string PopupHeader { get; set; }

    public string SaveButtonId { get; set; }

    protected string hiddenFieldFlagId = "";
    public string HiddenFieldFlagId {
        get { return hiddenFieldFlagId; }
        set { hiddenFieldFlagId = value; }
    }

    protected string doneButtonText = "Done";
    public string DoneButtonText {
        get { return doneButtonText; }
        set { doneButtonText = value; }
    }

    //private List<string> selectedValues = new List<string>();

    /// <summary>
    /// returns <name, guid>
    /// </summary>
    public List<KeyValuePair<string, string>> SelectedValues {
        get {

            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            foreach (KeyValuePair<string, KeyValuePair<string, bool>> item in Items) {
                if (item.Value.Value == true) items.Add(new KeyValuePair<string, string>(item.Value.Key, item.Key));
            }

            return items;
        }
    }    

    /// <summary>
    /// structure is: <guid, <name, isSelected>>
    /// </summary>
    public Dictionary<string, KeyValuePair<string, bool>> Items { get; set; }

    protected void Page_Init(object sender, EventArgs e) { }

    protected void Page_Load(object sender, EventArgs e) {        

        StringBuilder availableItems = new StringBuilder();
        StringBuilder selectedItems = new StringBuilder();
               

        if (Request.Form.GetValues("availableDistributionLists") != null) {            
            string[] values = Request.Form.GetValues("availableDistributionLists");                       

            foreach (string val in values) {                
                if (Items.ContainsKey(val)) {                    
                    // set the value of isselected to false if it is in this list of available items
                    if (Items[val].Value != false) {                        
                        Items[val] = new KeyValuePair<string, bool>(Items[val].Key, false);                        
                    }
                }
            }
        }
        

        if (Request.Form.Get("selectedDistributionLists") != null) {         
            string[] values = Request.Form.GetValues("selectedDistributionLists");            
            foreach (string val in values) {
                if (Items.ContainsKey(val)) {
                    // set the value of isselected to false if it is in this list of available items
                    if (Items[val].Value != true) {                        
                        Items[val] = new KeyValuePair<string, bool>(Items[val].Key, true);
                    }
                }
            }
        }

        if (Items != null) {
            foreach (KeyValuePair<string, KeyValuePair<string, bool>> item in Items) {
                string option = "<option value='" + item.Key + "' selected>" + item.Value.Key + "</option>\n";
                if (item.Value.Value == false) {
                    availableItems.Append(option);
                } else {
                    selectedItems.Append(option);
                    //selectedValues.Add(item.Key);
                }
            }
        }

        availableDistributionListsLit.Text = availableItems.ToString();
        selectedDistributionListsLit.Text = selectedItems.ToString();        
    }
}