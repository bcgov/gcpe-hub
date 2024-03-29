﻿using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.Expressions;
using CorporateCalendar.Security;
using Gcpe.Hub.Properties;

namespace CorporateCalendarAdmin
{
    public partial class ListDetails : System.Web.UI.Page
    {
        protected MetaTable table;

        protected void Page_Init(object sender, EventArgs e)
        {
            table = DynamicDataRouteHandler.GetRequestMetaTable(Context);
            GridView1.SetMetaTable(table, table.GetColumnValuesFromRoute(Context));
            FormView1.SetMetaTable(table);
            GridDataSource.EntityTypeName = table.EntityType.AssemblyQualifiedName;
            DetailsDataSource.EntityTypeName = table.EntityType.AssemblyQualifiedName;
            if (table.EntityType != table.RootEntityType)
            {
                GridQueryExtender.Expressions.Add(new OfTypeExpression(table.EntityType));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Title = table.DisplayName;

            bool DisableEditTable = Settings.Default.DisableEditTable;

            // Selection from url
            if (!Page.IsPostBack && table.HasPrimaryKey)
            {
                GridView1.SelectedPersistedDataKey = table.GetDataKeyFromRoute();
                if (GridView1.SelectedPersistedDataKey == null)
                {
                    GridView1.SelectedIndex = 0;
                }
            }
            CustomPrincipal customPrincipal = new CustomPrincipal(System.Web.HttpContext.Current.User.Identity);
            // Disable various options if the table is readonly
            if (table.IsReadOnly || (DisableEditTable
                    && ((string.Equals(Title, "CommunicationMaterials", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "Cities", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "Categories", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "GovernmentRepresentatives", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "Ministries", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "News Subscribe", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "NRDistributions", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "NROrigins", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "PremierRequested", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "Priorities", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "Roles", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "Sectors", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "Status", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "Themes", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Title, "Services", StringComparison.OrdinalIgnoreCase)) 
                    && !customPrincipal.IsInRole(CorporateCalendar.Security.SecurityRole.SysAdmin))))
            {
                DetailsPanel.Visible = false;
                GridView1.AutoGenerateSelectButton = false;
                GridView1.AutoGenerateEditButton = false;
                GridView1.AutoGenerateDeleteButton = false;
                GridView1.EnablePersistedSelection = false;
            }

            // Enable tags options if the table is readonly
            //else if (table.IsReadOnly || (DisableEditTable
            //        && (string.Equals(Title, "News Subscribe", StringComparison.OrdinalIgnoreCase)
            //        && !customPrincipal.IsInRole(CorporateCalendar.Security.SecurityRole.SysAdmin))))
            //{
            //    DetailsPanel.Visible = true;
            //    GridView1.AutoGenerateSelectButton = true;
            //    GridView1.AutoGenerateEditButton = true;
            //    GridView1.AutoGenerateDeleteButton = true;
            //    GridView1.EnablePersistedSelection = true;
            //}
        }

        protected void GridView1_DataBound(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count == 0)
            {
                FormView1.ChangeMode(FormViewMode.Insert);
            }
        }


        protected void Label_PreRender(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            DynamicFilter dynamicFilter = (DynamicFilter)label.FindControl("DynamicFilter");
            QueryableFilterUserControl fuc = dynamicFilter.FilterTemplate as QueryableFilterUserControl;
            if (fuc != null && fuc.FilterControl != null)
            {
                label.AssociatedControlID = fuc.FilterControl.GetUniqueIDRelativeTo(label);
            }
        }

        protected void DynamicFilter_FilterChanged(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            GridView1.PageIndex = 0;
            FormView1.ChangeMode(FormViewMode.ReadOnly);
        }

        protected void GridView1_RowEditing(object sender, EventArgs e)
        {
            FormView1.ChangeMode(FormViewMode.ReadOnly);
        }

        protected void GridView1_SelectedIndexChanging(object sender, EventArgs e)
        {
            GridView1.EditIndex = -1;
            FormView1.ChangeMode(FormViewMode.ReadOnly);
        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            SetDeleteConfirmation(e.Row);
        }

        protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {
            FormView1.DataBind();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            UpdateTimestamp(e.NewValues);
        }

        protected void GridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            FormView1.DataBind();
        }

        protected void FormView1_ItemDeleted(object sender, FormViewDeletedEventArgs e)
        {
            GridView1.DataBind();
        }

        protected void FormView1_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            UpdateTimestamp(e.NewValues);
        }

        protected void FormView1_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            GridView1.DataBind();
        }

        protected void FormView1_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            GridView1.DataBind();
        }

        protected void FormView1_ModeChanging(object sender, FormViewModeEventArgs e)
        {
            if (e.NewMode != FormViewMode.ReadOnly)
            {
                GridView1.EditIndex = -1;
            }
        }

        protected void FormView1_PreRender(object sender, EventArgs e)
        {
            if (FormView1.Row != null)
            {
                SetDeleteConfirmation(FormView1.Row);
            }
        }

        private void SetDeleteConfirmation(TableRow row)
        {
            foreach (Control c in row.Cells[0].Controls)
            {
                LinkButton button = c as LinkButton;
                if (button != null && button.CommandName == DataControlCommands.DeleteCommandName)
                {
                    button.OnClientClick = "return confirm('Are you sure you want to delete this item?');";
                }
            }
        }
        private static void UpdateTimestamp(IOrderedDictionary newValues)
        {
            string key = IsKey(newValues, "Timestamp") ?? IsKey(newValues, "LastUpdatedDateTime");
            if (key != null)
            {
                // update so it reflects on the Gov News Website
                newValues[key] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static string IsKey(IOrderedDictionary newValues, string key)
        {
            return newValues.Contains(key) ? key : null;
        }
    }
}
