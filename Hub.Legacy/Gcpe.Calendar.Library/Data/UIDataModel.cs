using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Collections;

/// <summary>
/// Summary description for UIDataModel
/// </summary>
public class UIDataModel
{
	public UIDataModel()
	{
		
	}

    public int page;
    
    public int total;
    public List<UIDataRow> rows;
    public string loadedTime;
}

public class UIDataRow
{
    public int id;
    public string color;
    public Hashtable cell;
}