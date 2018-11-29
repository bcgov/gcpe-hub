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
    public List<ColumnModel> ColModel { get; set; }
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

    public string AppendButton(string cssClass,string text, string onclick)
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
        foreach (ColumnModel col in ColModel)
        {
            if (!col.Hide)
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

        foreach (ColumnModel col in ColModel)
        {
            startupScript.AppendLine();

            startupScript.Append("{display: '");
            startupScript.Append(col.Display);

            startupScript.Append("', name: '");
            startupScript.Append(col.Name);
            startupScript.Append("', width: ");
            startupScript.Append(col.PercentWidth + " * adjustedGridWidth");
            startupScript.Append(", sortable: ");
            startupScript.Append(col.Sortable.ToString().ToLower());
            startupScript.Append(", align: '");
            startupScript.Append(col.Align.ToString().ToLower());
            startupScript.Append("', hide: ");
            startupScript.Append(col.Hide ? "true" : "false");
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
    public ColumnModel(string displayName, string modelFieldName, int percentWidth, bool sortable, HorizontalAlign align, string[] hiddenColumns, int colIndex)
    {
        this.Display = displayName;
        this.Align = align;
        this.Name = modelFieldName;
        this.Sortable = sortable;
        this.PercentWidth = percentWidth;
        this.Hide = hiddenColumns.Contains(colIndex.ToString());
    }

    public string Display { get; set; }
    public string Name { get; set; }
    public int PercentWidth { get; set; }
    public bool Sortable { get; set; }
    public HorizontalAlign Align { get; set; }
    public bool Hide { get; set; }
}