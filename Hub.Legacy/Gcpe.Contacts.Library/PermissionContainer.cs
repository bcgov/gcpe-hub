using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaRelationsLibrary
{

    [ToolboxData("<{0}:SimpleContainer runat=server></{0}:SimpleContainer>")]
    [ParseChildren(true, "Content")]
    public class PermissionContainer : WebControl, INamingContainer
    {
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateContainer(typeof(PermissionContainer))]
        [TemplateInstance(TemplateInstance.Single)]
        public virtual ITemplate Content { get; set; }

        public CommonEventLogging Logger = new CommonEventLogging();

        public Permissions.SiteSection Section = Permissions.SiteSection.None;
        private Permissions.SiteAction accessLevel = Permissions.SiteAction.Read;
        public string AccessLevel
        {
            get
            {
                return accessLevel.ToString();
            }
            set
            {
                string[] acs = value.Split('|');
                accessLevel = Permissions.SiteAction.None;
                foreach (string ac in acs)
                {
                    Permissions.SiteAction act;
                    if (Enum.TryParse(ac, out act))
                        accessLevel |= act;
                }
                //HttpContext.Current.Response.Write("access level = "+accessLevel);
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            // Do not render anything.
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            // Do not render anything.
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if (Section == Permissions.SiteSection.None)
            {
                this.RenderChildren(output);
            }
            else
            {
                Permissions.SiteAction permission = Permissions.GetUserPermissions(Section);

                //HttpContext.Current.Response.Write("permission = " + permission);

                //HttpContext.Current.Response.Write("permission = "+permission+", and-y thing = "+(permission & accessLevel));

                if (accessLevel > Permissions.SiteAction.None && (permission & accessLevel) == 0)
                {
                    //show error message
                    this.Controls.Clear();
                    this.Controls.Add(new LiteralControl("You do not have permission to access this resource. "));
                    this.RenderChildren(output);
                }
                else
                {
                    this.RenderChildren(output);
                }
            }
        }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            // Initialize all child controls.
            this.CreateChildControls();
            this.ChildControlsCreated = true;
        }

        protected override void CreateChildControls()
        {
            // Remove any controls
            this.Controls.Clear();

            // Add all content to a container.
            var container = new Control();
            this.Content.InstantiateIn(container);

            // Add container to the control collection.
            this.Controls.Add(container);
        }
    }
}