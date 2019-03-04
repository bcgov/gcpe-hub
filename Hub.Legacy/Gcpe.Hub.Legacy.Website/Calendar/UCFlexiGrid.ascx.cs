using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using CorporateCalendar.Security;

//using Newtonsoft.Json;

public partial class UCFlexiGrid : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private CustomPrincipal customPrincipal = new CustomPrincipal(HttpContext.Current.User.Identity);

    public bool DisplayReviewButton
    {
        get
        {
            return customPrincipal.IsGCPEHQ && customPrincipal.IsInRoleOrGreater(SecurityRole.Administrator);
        }
    }

    public string URL
    {
        get;
        set;
    }
    public string DataType { get; set; }
    public string[] HiddenColumns { get; set; }
    public string SortName { get; set; }
    public string SortOrder { get; set; }
    public bool UsePager { get; set; }
    public string Title { get; set; }
    public bool UseRP { get; set; }
    public int RecordsPerPage { get { return 30; } }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool Hide { get; set; }

    public string OnRowSelected { get; set; }

    public string AppendButton(string cssClass, string text, string onclick)
    {
        string paddingForImage = cssClass == "toggleView" ? "" : " style='padding-left:20px'";

        return string.Format("<div class='fbutton' onclick='{0}'><div><span class='{1}'{2}>{3}</span></div></div>",
            onclick, cssClass, paddingForImage, text);
    }

    public override void RenderControl(HtmlTextWriter writer)
    {
        base.RenderControl(writer);

        #region Render the FlexiGrid Javascript initialization code
        int totalWidthPercentage = 0;
        foreach (ColumnModel col in ColumnModel.ColumnInfos.Values)
        {
            if (!col.IsHidden(HiddenColumns))
                totalWidthPercentage += col.PercentWidth;
        }
        if (totalWidthPercentage == 0)
            totalWidthPercentage = 50;

        var startupScript = new StringBuilder();
        startupScript.AppendLine();
        startupScript.Append("<script language=\"javascript\" type=\"text/javascript\">");
        startupScript.AppendLine();
        startupScript.Append("var adjustedGridWidth = ($('#activityGridDiv').width()-20) / " + totalWidthPercentage + ";"); // 20 for the right scroll bar
        //startupScript.Append("alert($('#activityGridDiv').width() +' - ' +" + totalWidthPercentage + " );");
        startupScript.AppendLine();
        startupScript.Append("$('#" + this.ClientID + "_tbl').flexigrid({");
        startupScript.AppendLine();
        if (!string.IsNullOrEmpty(URL))
        {
            startupScript.Append("url: '");
            startupScript.Append(URL);
            startupScript.Append("',");
            startupScript.AppendLine();
        }

        if (!string.IsNullOrEmpty(DataType))
        {
            startupScript.Append("dataType: '");
            startupScript.Append(DataType);
            startupScript.Append("',");
            startupScript.AppendLine();
        }

        startupScript.AppendLine();

        startupScript.Append("colModel : [");

        foreach (var col in ColumnModel.ColumnInfos)
        {
            var colModel = col.Value;
            startupScript.AppendLine();

            startupScript.Append("{display: '");
            startupScript.Append(colModel.Display);

            startupScript.Append("', name: '");
            startupScript.Append(col.Key);
            startupScript.Append("', width: ");
            startupScript.Append(colModel.PercentWidth + " * adjustedGridWidth");
            startupScript.Append(", sortable: ");
            startupScript.Append(colModel.Sortable.ToString().ToLower());
            startupScript.Append(", align: '");
            startupScript.Append(colModel.Align.ToString().ToLower());
            startupScript.Append("', hide: ");
            startupScript.Append(colModel.IsHidden(HiddenColumns) ? "true" : "false");
            startupScript.Append("},");
        }
        startupScript.Remove(startupScript.Length - 1, 1);

        startupScript.Append("],");

        startupScript.AppendLine();

        if (!string.IsNullOrEmpty(SortName))
        {
            startupScript.Append("sortname: '");
            startupScript.Append(SortName); //string.Empty);
            startupScript.Append("',");
            startupScript.AppendLine();

        }

        if (!string.IsNullOrEmpty(SortOrder))
        {
            startupScript.Append("sortorder: '");
            startupScript.Append(SortOrder);
            startupScript.Append("',");
            startupScript.AppendLine();
        }

        startupScript.Append("usepager: ");
        startupScript.Append(UsePager.ToString().ToLower());
        startupScript.Append(",");
        startupScript.AppendLine();

        if (!string.IsNullOrEmpty(OnRowSelected))
        {
            startupScript.Append("onRowSelected: ");
            startupScript.Append(OnRowSelected);
            startupScript.Append(",");
            startupScript.AppendLine();
        }


        startupScript.Append("hide: ");
        startupScript.Append(Hide.ToString().ToLower());
        startupScript.Append(",");

        startupScript.Append("useRp: ");
        startupScript.Append(UseRP.ToString().ToLower());
        startupScript.Append(",");
        startupScript.AppendLine();

        startupScript.Append("rp : ");
        startupScript.Append(RecordsPerPage);
        startupScript.Append(",");
        startupScript.AppendLine();

        startupScript.Append("width: 'auto'");
        // startupScript.Append(Width);
        startupScript.Append(",");
        startupScript.AppendLine();


        startupScript.Append("nowrap: ");
        startupScript.Append("false");
        startupScript.Append(",");
        startupScript.AppendLine();

        startupScript.Append("height: 'auto'");
        //startupScript.Append(Height);
        startupScript.Append("");
        startupScript.AppendLine();


        startupScript.Append("}");
        startupScript.AppendLine();
        startupScript.Append(");");
        startupScript.AppendLine();/**/

        // Review: The script needs to be located here in the code-behind, because IE won't render
        // the grid if you try to call out to a script located on the page itself

        //startupScript.AppendLine("function review() {");
        //startupScript.AppendLine("  ReviewHandler();");
        //startupScript.AppendLine("}");


        //startupScript.AppendLine("function remove() {");
        //startupScript.AppendLine("if ($('#SelectedTextBox').val() == '') {");
        //startupScript.AppendLine("if (confirm('Are you sure you want to delete all the activities in the grid?')) {");
        //startupScript.AppendLine("var queryString = '?Op=DeleteGrid';");
        //startupScript.AppendLine("var filter = 'ActivityHandler.ashx' + queryString;");
        //startupScript.AppendLine("$.ajax({");
        //startupScript.AppendLine("type: 'POST',");
        //startupScript.AppendLine("contentType: 'application/json; charset=utf-8',");
        //startupScript.AppendLine("url: 'ActivityHandler.ashx' + queryString,");
        //startupScript.AppendLine("dataType: 'json',");
        //startupScript.AppendLine("success: function (data) {");
        //startupScript.AppendLine("$('#ActivityGrid_tbl').flexOptions().flexReload();");
        //startupScript.AppendLine("$('#SelectedTextBox').val('');");
        //startupScript.AppendLine("}");
        //startupScript.AppendLine("});");
        //startupScript.AppendLine("}");
        //startupScript.AppendLine("} else {");
        //startupScript.AppendLine("if (confirm('Are you sure you want to delete activities: ' + $('#SelectedTextBox').val().slice(0, -1) + '?')) {");
        //startupScript.AppendLine("var queryString = '?Op=DeleteSelected&ids=' + $('#SelectedTextBox').val().slice(0, -1);");
        //startupScript.AppendLine("var filter = 'ActivityHandler.ashx' + queryString;");
        //startupScript.AppendLine("$.ajax({");
        //startupScript.AppendLine("type: 'POST',");
        //startupScript.AppendLine("contentType: 'application/json; charset=utf-8',");
        //startupScript.AppendLine("url: 'ActivityHandler.ashx' + queryString,");
        //startupScript.AppendLine("dataType: 'json',");
        //startupScript.AppendLine("success: function (data) {");
        //startupScript.AppendLine("$('#ActivityGrid_tbl').flexOptions().flexReload();");
        //startupScript.AppendLine("$('#SelectedTextBox').val('');");
        //startupScript.AppendLine("}");
        //startupScript.AppendLine("});");
        //startupScript.AppendLine("};");
        //startupScript.AppendLine("}");
        //startupScript.AppendLine("}");

        startupScript.Append("</script>");

        writer.Write(startupScript.ToString());

        #endregion

    }

}

/// <summary>
/// Definition of the columns in the grid
/// </summary>
public class ColumnModel
{
    public static string[] HiddenByDefault = new[] { "1", "2", "3", "10" };
    public static Dictionary<string, ColumnModel> ColumnInfos = new Dictionary<string, ColumnModel>(){
        {"MinistryActivityId", new ColumnModel("0", HorizontalAlign.Right, 6, true, "Activity Id") },
        {"Keywords", new ColumnModel("1", HorizontalAlign.Left, 4, true, "Tags") },
        {"Ministry", new ColumnModel("2", HorizontalAlign.Left, 5, true, "Ministry") },
        {"Status", new ColumnModel("3", HorizontalAlign.Left, 4, true, "Status") },
        {"StartEndDateTime",  new ColumnModel("4", HorizontalAlign.Left, 6, true, "Date & Time") },
        {"TitleDetails",  new ColumnModel("5", HorizontalAlign.Left, 32, true, "Title & Summary") },
        {"Categories", new ColumnModel("6", HorizontalAlign.Left, 6, true, "Categories") },
        {"CommunicationsMaterials",  new ColumnModel("7", HorizontalAlign.Left, 7, true, "Comm. Materials") },
        {"PremierRequested",  new ColumnModel("8", HorizontalAlign.Left, 4, false,"Premier") },
        {"LeadOrganization",  new ColumnModel("9", HorizontalAlign.Left, 5, true, "Lead Org.") },
        {"Translations",  new ColumnModel("10", HorizontalAlign.Left, 6, false,"Translations") },
        {"City",  new ColumnModel("11", HorizontalAlign.Left, 6, true, "City") },
        {"NameAndNumber",  new ColumnModel("12", HorizontalAlign.Left, 7, true, "Comm. Contact") },
        {"GovernmentRepresentative",  new ColumnModel("13", HorizontalAlign.Left, 5, true, "Govt Rep.") }
        // Columns widths should total 100%
    };
    public ColumnModel(string colIndex, HorizontalAlign align, int percentWidth, bool sortable, string displayName)
    {
        Display = displayName;
        Align = align;
        Sortable = sortable;
        PercentWidth = percentWidth;
        ColIndex = colIndex;
    }

    public string Display { get; }
    public int PercentWidth { get; }
    public bool Sortable { get; }
    public HorizontalAlign Align { get; }
    public string ColIndex { get; }

    public bool IsHidden(IEnumerable<string> hiddenColumns)
    {
        return hiddenColumns.Contains(ColIndex);
    }
    public static bool IsHidden(string columnName, IEnumerable<string> hiddenColumns)
    {
        return ColumnInfos[columnName].IsHidden(hiddenColumns);
    }
}
