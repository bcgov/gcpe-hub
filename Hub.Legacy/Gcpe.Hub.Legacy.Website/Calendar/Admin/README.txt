1.  To hide columns for a table: add or enter an entry like the following to 
	Metadata.cs in the Gcpe.Calendar.Legacy.Data project:
		(this will show the Table, but hide the Id, and ActivityThemes columns)

    [MetadataType(typeof(Theme_MD))]
    [ScaffoldTable(true)]
    public partial class Theme
    {
        public class Theme_MD
        {
            [ScaffoldColumn(false)]
            public object Id { get; set; }

            [ScaffoldColumn(false)]
            public object ActivityThemes { get; set; }
       }
    }

2.  To not display a table in the list of tables, 
	add something like the following to Metadata.cs in the Gcpe.Calendar.Legacy.Data project:

    [ScaffoldTable(false)]
    public partial class ActivityTheme
    {
    }

3.  If you want to edit or delete in the lists, modify ListDetails.aspx.cs 
	the Gcpe.CorporateCalendar.Calendar.Admin project in section labelled as follows:

            // Disable various options if the table is readonly