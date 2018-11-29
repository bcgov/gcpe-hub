/*
 * Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace CKEditor.NET
{
	#region Enum

	public enum contentsLangDirections
	{
		/// <summary>
		/// 'ui' - which indicate content direction will be the same with the user interface language direction;
		/// </summary>
		Ui,
		/// <summary>
		/// 'ltr' - for Left-To-Right language (like English);
		/// </summary>
		Ltr,
		/// <summary>
		/// 'rtl' - for Right-To-Left languages (like Arabic).
		/// </summary>
		Rtl
	}

	public enum DialogButtonsOrder
	{
		/// <summary>
		/// the buttons will be displayed in the default order of the user's OS
		/// </summary>
		OS,
		/// <summary>
		/// for Left-To-Right order
		/// </summary>
		Ltr,
		/// <summary>
		/// for Right-To-Left order
		/// </summary>
		Rtl
	}

	public enum EnterMode
	{
		/// <summary>
		/// new <p> paragraphs are created
		/// </summary>
		P = 1,
		/// <summary>
		/// lines are broken with <br> elements
		/// </summary>
		BR = 2,
		/// <summary>
		/// new <div> blocks are created
		/// </summary>
		DIV = 3
	}

	public enum StartupMode
	{
		Wysiwyg,
		Source
	}

	public enum ResizeDir
	{
		Both,
		Vertical,
		Horizontal
	}

	public enum ToolbarLocation
	{
		Top,
		Bottom
	}

	[Flags]
	public enum ScaytContextCommands
	{
		/// <summary>
		/// disables all options
		/// </summary>
		Off = 0,
		/// <summary>
		///  enables all options
		/// </summary>
		All = 1,
		/// <summary>
		/// enables the "Ignore" option
		/// </summary>
		Ignore = 2,
		/// <summary>
		/// enables the "Ignore All" option
		/// </summary>
		Ignoreall = 4,
		/// <summary>
		/// enables the "Add Word" option
		/// </summary>
		Add = 8
	}

	[Flags]
	public enum ScaytContextMenuItemsOrder
	{
		/// <summary>
		/// main suggestion word list
		/// </summary>
		Suggest = 1,
		/// <summary>
		/// moresuggest
		/// </summary>
		Moresuggest = 2,
		/// <summary>
		/// SCAYT commands, such as 'Ignore' and 'Add Word'
		/// </summary>
		Control = 4
	}

	public enum ScaytMoreSuggestions
	{
		On,
		Off
	}

	#endregion

	#region CKSerializable

	[System.AttributeUsage(System.AttributeTargets.Property)]
	public class CKSerializable : System.Attribute
	{
		public string Name = string.Empty;
		public bool RemoveEnters = false;
		public bool ForceAddToJSON = false;
		public bool IsObject = false;
	}

	public class JSONSerializer
	{
		public static string ToJavaScriptObjectNotation(object obj)
		{
			bool hasCommaAtEnd = false;
			System.Text.StringBuilder sbJSON = new System.Text.StringBuilder("{");
			Type objType = obj.GetType();
			System.Text.StringBuilder oc = new StringBuilder();
			#region Reflect Properties
			System.Reflection.PropertyInfo[] staticPropertyInfo = CKEditorConfig.GlobalConfig.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			Dictionary<string, object> staticPropertyInfoDictionary = new Dictionary<string, object>();
			foreach (System.Reflection.PropertyInfo propertyInfo in staticPropertyInfo)
				staticPropertyInfoDictionary.Add(propertyInfo.Name, propertyInfo.GetValue(CKEditorConfig.GlobalConfig, new object[0]));
			foreach (System.Reflection.PropertyInfo propertyInfo in
				objType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
			{
				bool canSerializable = false;
				string customName = string.Empty;
				bool removeEnters = false;
				bool forceAddToJSON = false;
				bool isObject = false;
				object[] attrs = propertyInfo.GetCustomAttributes(false);
				foreach (object attr in attrs)
					if (attr is CKSerializable)
					{
						canSerializable = true;
						removeEnters = ((CKSerializable)attr).RemoveEnters;
						forceAddToJSON = ((CKSerializable)attr).ForceAddToJSON;
						isObject = ((CKSerializable)attr).IsObject;
						if (!string.IsNullOrEmpty(((CKSerializable)attr).Name))
							customName = ((CKSerializable)attr).Name;
						break;
					}
				if (!canSerializable)
					continue;
				if (!propertyInfo.CanRead)
					continue;
				if (customName == "ExtraOptions")
					foreach (object[] item in (object[])propertyInfo.GetValue(obj, new object[0]))
						oc.Append(", ").Append("\"" + item[0].ToString() + "\" : ").Append(item[1].ToString());
				else if (customName == "stylesSet" && ((string[])propertyInfo.GetValue(obj, new object[0])).Length == 1 && !((string[])propertyInfo.GetValue(obj, new object[0]))[0].StartsWith("{"))
				{
					object staticValueToCompare = staticPropertyInfoDictionary[propertyInfo.Name];
					if (!campareArray(((string[]) propertyInfo.GetValue(obj, new object[0])), (string[]) staticValueToCompare))
					{
						string[] valueToJSON = (string[]) propertyInfo.GetValue(obj, new object[0]);
						sbJSON.Append("\"" + (string.IsNullOrEmpty(customName) ? propertyInfo.Name : customName) + "\" : ");
						if ((valueToJSON[0].StartsWith("[") || valueToJSON[0].StartsWith("{")) &&
							(valueToJSON[0].EndsWith("]") || valueToJSON[0].EndsWith("}")))
							sbJSON.Append(EscapeStringForJavaScript(valueToJSON[0]).TrimStart(new char[] { '\'' }).TrimEnd(new char[] { '\'' }));
						else
							sbJSON.Append("\"" + EscapeStringForJavaScript(valueToJSON[0]).TrimStart(new char[] {'\''}).TrimEnd(new char[] {'\''}) + "\"");
						sbJSON.Append(", ");
					}
					continue;
				}
				else if (customName == "on" && ((object[])propertyInfo.GetValue(obj, new object[0])).Length > 0)
				{
					object[] objArray = (object[])((object[])propertyInfo.GetValue(obj, new object[0]))[0];
					oc.Append(", \"on\" : {").Append(((object[])objArray)[0]).Append(" : ").Append(((object[])objArray)[1]).Append(" } ");
				}
				else if (propertyInfo.Name == "stylesheetParser_skipSelectors" || propertyInfo.Name == "stylesheetParser_validSelectors")
				{
					string valueToJSON = (string)propertyInfo.GetValue(obj, new object[0]);
					string staticValueToCompare = (string)staticPropertyInfoDictionary[propertyInfo.Name];
					if (valueToJSON != staticValueToCompare)
						oc.Append(", " + propertyInfo.Name + ": " + valueToJSON);
				}
				else if (propertyInfo.Name == "entities_processNumerical")
				{
					string valueToJSON = (string)propertyInfo.GetValue(obj, new object[0]);
					string staticValueToCompare = (string)staticPropertyInfoDictionary[propertyInfo.Name];
					if (valueToJSON != staticValueToCompare)
					{
						if (valueToJSON.ToLower() == false.ToString().ToLower() || valueToJSON.ToLower() == true.ToString().ToLower())
							oc.Append(", " + propertyInfo.Name + ": " + bool.Parse(valueToJSON));
						else
							oc.Append(", " + propertyInfo.Name + ": " + valueToJSON);
					}
				}
				else
				{
					object valueToJSON = propertyInfo.GetValue(obj, new object[0]);
					object staticValueToCompare = staticPropertyInfoDictionary[propertyInfo.Name];
					if (valueToJSON == null && staticValueToCompare == null)
						continue;
					else if (valueToJSON.GetType().IsArray && staticValueToCompare == null) { }
					else if (!forceAddToJSON && !valueToJSON.GetType().IsArray && !staticValueToCompare.GetType().IsArray && staticValueToCompare.ToString() == valueToJSON.ToString())
						continue;
					else if (!forceAddToJSON && valueToJSON.GetType().IsArray && staticValueToCompare.GetType().IsArray)
					{
						if (valueToJSON.GetType() == typeof(int[]) && campareArray((int[])valueToJSON, (int[])staticValueToCompare))
							continue;
						else if (valueToJSON.GetType() == typeof(string[]) && campareArray((string[])valueToJSON, (string[])staticValueToCompare))
							continue;
						else if (valueToJSON.GetType() == typeof(object[][]) && campareArray((object[][])valueToJSON, (object[][])staticValueToCompare))
							continue;
						else if (valueToJSON.GetType() == typeof(object[]) && campareArray((object[])valueToJSON, (object[])staticValueToCompare))
							continue;
					}
					sbJSON.Append("\"" + (string.IsNullOrEmpty(customName) ? propertyInfo.Name : customName) + "\" : ");
					if (propertyInfo.Name == "toolbar" || propertyInfo.Name == "toolbar_Basic" || propertyInfo.Name == "toolbar_Full")
					{
						object toolObj;
						if (propertyInfo.Name == "toolbar") toolObj = propertyInfo.GetValue(obj, new object[0]);
						else toolObj = ((object[])propertyInfo.GetValue(obj, new object[0]))[0];
						if (toolObj.GetType() == typeof(string) && ((string)toolObj).StartsWith("[") && ((string)toolObj).EndsWith("]"))
						{
							isObject = true;
							GetFieldOrPropertyValue(ref sbJSON, toolObj, removeEnters, isObject);
							sbJSON.Append(", ");
							continue;
						}
					}
					GetFieldOrPropertyValue(ref sbJSON, propertyInfo.GetValue(obj, new object[0]), removeEnters, isObject);
					sbJSON.Append(", ");
				}
				hasCommaAtEnd = true;
			}
			#endregion

			if (hasCommaAtEnd)
				sbJSON.Length -= 2;
			sbJSON.Append(oc.ToString()).Append("}");
			return sbJSON.ToString();
		}

		private static bool campareArray(string[] obj1, string[] obj2)
		{
			if (!obj1.GetType().IsArray || !obj2.GetType().IsArray)
				return false;
			if (obj1.Length != obj2.Length)
				return false;
			for (int i = 0; i < obj1.Length; i++)
				if (obj1[i].ToString() != obj2[i].ToString())
					return false;
			return true;
		}

		private static bool campareArray(int[] obj1, int[] obj2)
		{
			if (!obj1.GetType().IsArray || !obj2.GetType().IsArray)
				return false;
			if (obj1.Length != obj2.Length)
				return false;
			for (int i = 0; i < obj1.Length; i++)
				if (obj1[i].ToString() != obj2[i].ToString())
					return false;
			return true;
		}

		private static bool campareArray(object[][] obj1, object[][] obj2)
		{
			if (!obj1.GetType().IsArray || !obj2.GetType().IsArray)
				return false;
			if (obj1.Length != obj2.Length)
				return false;
			for (int i = 0; i < obj1.Length; i++)
				if (!campareArray(obj1[i], obj2[i]))
					return false;
			return true;
		}

		private static bool campareArray(object[] obj1, object[] obj2)
		{
			if (!obj1.GetType().IsArray || !obj2.GetType().IsArray)
				return false;
			if (obj1.Length != obj2.Length)
				return false;
			for (int i = 0; i < obj1.Length; i++)
				if ((obj1[i].GetType().IsArray) && !campareArray((object[])obj1[i], (object[])obj2[i]))
					return false;
				else if (obj1[i].ToString() != obj2[i].ToString())
					return false;
			return true;
		}

		private static void GetFieldOrPropertyValue(ref System.Text.StringBuilder sbJSON, object value, bool removeEnters, bool isObject)
		{
			Type type = value.GetType();
			if (type == typeof(System.DateTime))
				sbJSON.Append("new Date(" + ((DateTime)value - DateTime.Parse("1/1/1970")).TotalMilliseconds.ToString() + ")");
			else if (type == typeof(System.String))
			{
				string app = isObject ? string.Empty : "\"";
				sbJSON.Append(app + EscapeStringForJavaScript((removeEnters ? ((string)value).Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("\b", "") : (string)value)) + app);
			}
			else if (type == typeof(System.Int16) || type == typeof(System.Int32) || type == typeof(System.Int64))
				sbJSON.Append(value.ToString());
			else if (type == typeof(System.Decimal) || type == typeof(System.Double) || type == typeof(System.Single))
				sbJSON.Append(value.ToString().Replace(',', '.'));
			else if (type.IsArray)
			{
				bool hasCommaAtEnd = false;
				sbJSON.Append("[");
				foreach (object o in (Array)value)
				{
					GetFieldOrPropertyValue(ref sbJSON, o, removeEnters, isObject);
					sbJSON.Append(", ");
					hasCommaAtEnd = true;
				}
				if (hasCommaAtEnd)
					sbJSON.Length -= 2;
				sbJSON.Append("]");
			}
			else if (type == typeof(System.Boolean))
				sbJSON.Append(value.ToString().ToLower());
			else
				sbJSON.Append(EscapeStringForJavaScript(value.ToString()));
		}

		private static string EscapeStringForJavaScript(string input)
		{
			input = input.Replace("\\", @"\\");
			input = input.Replace("\b", @"\u0008");
			input = input.Replace("\t", @"\u0009");
			input = input.Replace("\n", @"\u000a");
			input = input.Replace("\f", @"\u000c");
			input = input.Replace("\r", @"\u000d");
			input = input.Replace("\"", @"""");
			input = input.Replace("\"", "\\\"");
			return input;
		}
	}

	#endregion

	[Serializable]
	public class CKEditorConfig
	{
		#region Consts

		public const int CKEDITOR_CTRL = 1000;
		public const int CKEDITOR_SHIFT = 2000;
		public const int CKEDITOR_ALT = 4000;

		#endregion

		#region GlobalConfig

		public static CKEditorConfig GlobalConfig;

		#endregion

		#region CKSerializable Property

		/// <summary>
		/// Extra height in pixel to leave between the bottom boundary of content with document size when auto resizing.
		/// Default Value: 0
		/// </summary>
		[CKSerializable]
		public int autoGrow_bottomSpace { get; set; }

		/// <summary>
		/// The maximum height to which the editor can reach using AutoGrow. Zero means unlimited.
		/// Default Value: 0
		/// </summary>
		[CKSerializable]
		public int autoGrow_maxHeight { get; set; }

		/// <summary>
		/// The minimum height to which the editor can reach using AutoGrow.
		/// Default Value: 200
		/// </summary>
		[CKSerializable]
		public int autoGrow_minHeight { get; set; }

		/// <summary>
		/// Whether to have the auto grow happen on editor creation.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool autoGrow_onStartup { get; set; }

		/// <summary>
		/// Whether automatically create wrapping blocks around inline contents inside document body, this helps to ensure the integrality of the block enter mode. Note: Changing the default value might introduce unpredictable usability issues.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool autoParagraph { get; set; }

		/// <summary>
		/// Whether the replaced element (usually a textarea) is to be updated automatically when posting the form containing the editor.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool autoUpdateElement { get; set; }

		/// <summary>
		/// The base Z-index for floating dialogs and popups.
		/// Default Value: 2000
		/// </summary>
		[CKSerializable]
		public int baseFloatZIndex { get; set; }

		/// <summary>
		/// The base href URL used to resolve relative and absolute URLs in the editor content.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string baseHref { get; set; }

		/// <summary>
		/// Whether to escape basic HTML entities in the document, including: nbsp, gt, lt, amp.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool basicEntities { get; set; }

		/// <summary>
		/// A list of keystrokes to be blocked if not defined in the CKEDITOR.config.keystrokes setting. In this way it is possible 
		/// to block the default browser behavior for those keystrokes.
		/// Default Value:
		/// new int[] 
		/// { 
		///		CKEDITOR_CTRL + 66 /*B*/, 
		///		CKEDITOR_CTRL + 73 /*I*/, 
		///		CKEDITOR_CTRL + 85 /*U*/
		/// };
		/// </summary>
		[CKSerializable]
		public int[] blockedKeystrokes { get; set; }

		/// <summary>
		/// Sets the "class" attribute to be used on the body element of the editing area. This can be useful when reusing the original CSS 
		/// file you're using on your live website and you want to assing to the editor the same class name you're using for the region 
		/// that'll hold the contents. In this way, class specific CSS rules will be enabled.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string bodyClass { get; set; }

		/// <summary>
		/// Sets the "id" attribute to be used on the body element of the editing area. This can be useful when reusing the original CSS 
		/// file you're using on your live website and you want to assing to the editor the same id you're using for the region 
		/// that'll hold the contents. In this way, id specific CSS rules will be enabled.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string bodyId { get; set; }

		/// <summary>
		/// Whether to show the browser native context menu when the CTRL or the META (Mac) key is pressed while opening the context menu.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool browserContextMenuOnCtrl { get; set; }

		/// <summary>
		/// Holds the style definition to be used to apply the text background color.
		/// Default Value: 
		/// "{
		/// 	element : 'span',
		/// 	styles : { 'background-color' : '#(color)' }
		/// }"
		///
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object colorButton_backStyle { get; set; }

		/// <summary>
		/// Defines the colors to be displayed in the color selectors. It's a string containing the hexadecimal notation for HTML colors, 
		/// without the "#" prefix. Since 3.3: A name may be optionally defined by prefixing the entries with the name and 
		/// the slash character. For example, "FontColor1/FF9900" will be displayed as the color #FF9900 in the selector, 
		/// but will be outputted as "FontColor1".
		/// Default Value: "000,800000,8B4513,2F4F4F,008080,000080,4B0082,696969,B22222,A52A2A,DAA520,006400,40E0D0,0000CD,800080,808080,F00,FF8C00,FFD700,008000,0FF,00F,EE82EE,A9A9A9,FFA07A,FFA500,FFFF00,00FF00,AFEEEE,ADD8E6,DDA0DD,D3D3D3,FFF0F5,FAEBD7,FFFFE0,F0FFF0,F0FFFF,F0F8FF,E6E6FA,FFF"
		/// </summary>
		[CKSerializable]
		public string colorButton_colors { get; set; }

		/// <summary>
		/// Whether to enable the More Colors button in the color selectors.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool colorButton_enableMore { get; set; }

		/// <summary>
		/// Holds the style definition to be used to apply the text foreground color.
		/// Default Value: 
		/// "{
		/// 	element		: 'span',
		/// 	styles		: { 'color' : '#(color)' },
		/// 	overrides	: [ { element : 'font', attributes : { 'color' : null } } ]
		/// }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object colorButton_foreStyle { get; set; }

		/// <summary>
		/// The CSS file(s) to be used to apply style to the contents. It should reflect the CSS used in the final pages where 
		/// the contents are to be used.
		/// Default Value: new string[] { "~/contents.css" };
		/// </summary>
		public string[] contentsCss { get; set; }
		[CKSerializable(Name = "contentsCss", IsObject = true)]
		private string[] contentsCssSer
		{
			get
			{
				List<string> retVal = new List<string>();
				ResolveParameters(contentsCss, retVal, true);
				return retVal.ToArray();
			}
		}

		/// <summary>
		/// The writting direction of the language used to write the editor contents. Allowed values are: 
		/// public enum contentsLangDirections
		/// {
		/// 	Ui - which indicate content direction will be the same with the user interface language direction;
		/// 	Ltr, - for Left-To-Right language (like English);
		/// 	Rtl - for Right-To-Left languages (like Arabic).
		/// }
		/// Default Value: contentsLangDirections.Ui
		/// </summary>
		public contentsLangDirections contentsLangDirection { get; set; }

		[CKSerializable(Name = "contentsLangDirection")]
		private string contentsLangDirectionSer { get { return contentsLangDirection.ToString().ToLower(); } }

		/// <summary>
		/// Language code of the writting language which is used to author the editor contents.
		/// Default Value: Same value with editor's UI language.
		/// </summary>
		[CKSerializable]
		public string contentsLanguage { get; set; }

		/// <summary>
		/// The style definition to be used to apply the bold style in the text.
		/// Default Value: "{ element : 'strong', overrides : 'b' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object coreStyles_bold { get; set; }

		/// <summary>
		/// The style definition to be used to apply the italic style in the text.
		/// Default Value: { element : 'em', overrides : 'i' }
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object coreStyles_italic { get; set; }

		/// <summary>
		/// The style definition to be used to apply the strike style in the text.
		/// Default Value: { element : 'strike' }
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object coreStyles_strike { get; set; }

		/// <summary>
		/// The style definition to be used to apply the subscript style in the text.
		/// Default Value: { element : 'sub' }
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object coreStyles_subscript { get; set; }

		/// <summary>
		/// The style definition to be used to apply the superscript style in the text.
		/// Default Value: { element : 'sup' }
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object coreStyles_superscript { get; set; }

		/// <summary>
		/// The style definition to be used to apply the underline style in the text.
		/// Default Value: { element : 'u' }
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object coreStyles_underline { get; set; }

		/// <summary>
		/// The URL path for the custom configuration file to be loaded. If not overloaded with inline configurations, it defaults 
		/// to the "config.js" file present in the root of the CKEditor installation directory.
		/// CKEditor will recursively load custom configuration files defined inside other custom configuration files.
		/// Default Value: "config.js"
		/// </summary>
		[CKSerializable]
		public string customConfig { get; set; }

		/// <summary>
		/// The language to be used if CKEDITOR.config.language is left empty and it's not possible to localize the editor to the user language.
		/// Default Value: "en"
		/// </summary>
		[CKSerializable]
		public string defaultLanguage { get; set; }

		/// <summary>
		/// A setting that stores CSS rules to be injected into the page with styles to be applied to the tooltip element.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string devtools_styles { get; set; }

		/// <summary>
		/// The color of the dialog background cover. It should be a valid CSS color string.
		/// Default Value: "white"
		/// </summary>
		[CKSerializable]
		public string dialog_backgroundCoverColor { get; set; }

		/// <summary>
		/// The opacity of the dialog background cover. It should be a number within the range [0.0, 1.0].
		/// Default Value: 0.5
		/// </summary>
		[CKSerializable]
		public double dialog_backgroundCoverOpacity { get; set; }

		/// <summary>
		/// The guideline to follow when generating the dialog buttons. There are 3 possible options: 
		/// public enum DialogButtonsOrder
		/// {
		/// 	OS - the buttons will be displayed in the default order of the user's OS
		/// 	Ltr - for Left-To-Right order
		/// 	Rtl - for Right-To-Left order
		/// }
		/// Default Value: DialogButtonsOrder.OS;
		/// </summary>
		public DialogButtonsOrder dialog_buttonsOrder { get; set; }
		[CKSerializable(Name = "dialog_buttonsOrder")]
		private string dialog_buttonsOrderSer { get { return dialog_buttonsOrder.ToString().ToLower(); } }

		/// <summary>
		/// The distance of magnetic borders used in moving and resizing dialogs, measured in pixels.
		/// Default Value: 20
		/// </summary>
		[CKSerializable]
		public int dialog_magnetDistance { get; set; }

		/// <summary>
		/// If the dialog has more than one tab, put focus into the first tab as soon as dialog is opened.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool dialog_startupFocusTab { get; set; }

		/// <summary>
		/// Disables the built-in spell checker while typing natively available in the browser (currently Firefox and Safari only).
		/// Even if word suggestions will not appear in the CKEditor context menu, this feature is useful to help quickly identifying misspelled words.
		/// This setting is currently compatible with Firefox only due to limitations in other browsers.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool disableNativeSpellChecker { get; set; }

		/// <summary>
		/// Disables the "table tools" offered natively by the browser (currently Firefox only) to make quick table editing operations, 
		/// like adding or deleting rows and columns.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool disableNativeTableHandles { get; set; }

		/// <summary>
		/// Disables the ability of resize objects (image and tables) in the editing area.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool disableObjectResizing { get; set; }

		/// <summary>
		/// Disables inline styling on read-only elements.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool disableReadonlyStyling { get; set; }

		/// <summary>
		/// Sets the doctype to be used when loading the editor content as HTML.
		/// Default Value: '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">'
		/// </summary>
		[CKSerializable]
		public string docType { get; set; }

		/// <summary>
		/// Whether to render or not the editing block area in the editor interface.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool editingBlock { get; set; }

		/// <summary>
		/// The e-mail address anti-spam protection option. The protection will be applied when creating or modifying e-mail links through the editor interface.
		/// Two methods of protection can be choosed: 
		/// The e-mail parts (name, domain and any other query string) are assembled into a function call pattern. Such function must be provided by 
		/// the developer in the pages that will use the contents. 
		/// Only the e-mail address is obfuscated into a special string that has no meaning for humans or spam bots, but which is properly rendered and accepted by the browser.
		/// Both approaches require JavaScript to be enabled.
		/// Default Value: "" (empty string = disabled)
		/// </summary>
		[CKSerializable]
		public string emailProtection { get; set; }

		/// <summary>
		/// Allow context-sensitive tab key behaviors, including the following scenarios: 
		/// When selection is anchored inside table cells:
		/// If TAB is pressed, select the contents of the "next" cell. If in the last cell in the table, add a new row to it and focus its first cell.
		/// If SHIFT+TAB is pressed, select the contents of the "previous" cell. Do nothing when it's in the first cell.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool enableTabKeyTools { get; set; }

		/// <summary>
		/// Sets the behavior for the ENTER key. It also dictates other behaviour rules in the editor, like whether the <br> element is to be used as a paragraph separator when indenting text. 
		/// The allowed values are the following constants, and their relative behavior: 
		/// public enum EnterMode
		/// {
		/// 	P = 1 - new <p> paragraphs are created
		/// 	BR = 2 - lines are broken with <br> elements
		/// 	DIV = 3 - new <div> blocks are created
		/// }
		/// Note: It's recommended to use the CKEDITOR.ENTER_P value because of its semantic value and correctness. The editor is optimized for this value.
		/// Default Value: EnterMode.P;
		/// </summary>
		public EnterMode enterMode { get; set; }
		[CKSerializable(Name = "enterMode", IsObject = true)]
		private int enterModeSer { get { return (int)this.enterMode; } }

		/// <summary>
		/// Whether to use HTML entities in the output.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool entities { get; set; }

		/// <summary>
		/// An additional list of entities to be used. It's a string containing each entry separated by a comma. 
		/// Entities names or number must be used, exclusing the "&" preffix and the ";" termination.
		/// Default Value: "#39" // The single quote (') character.
		/// </summary>
		[CKSerializable]
		public string entities_additional { get; set; }

		/// <summary>
		/// Whether to convert some symbols, mathematical symbols, and Greek letters to HTML entities. This may be more relevant for users typing text written in Greek. 
		/// The list of entities can be found at the W3C HTML 4.01 Specification, section 24.3.1.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool entities_greek { get; set; }

		/// <summary>
		/// Whether to convert some Latin characters (Latin alphabet No. 1, ISO 8859-1) to HTML entities. 
		/// The list of entities can be found at the W3C HTML 4.01 Specification, section 24.2.1.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool entities_latin { get; set; }

		/// <summary>
		/// Whether to convert all remaining characters not included in the ASCII character table to their relative decimal numeric representation of HTML entity. 
		/// When set to force, it will convert all entities into this format. For example the phrase "This is Chinese: 汉语." is output as "This is Chinese: &#27721;&#35821;."
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public string entities_processNumerical { get; set; }

		/// <summary>
		/// List of additional plugins to be loaded. 
		/// This is a tool setting which makes it easier to add new plugins, whithout having to touch and possibly breaking the CKEDITOR.config.plugins setting.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string extraPlugins { get; set; }

		/// <summary>
		/// The location of an external file browser, that should be launched when "Browse Server" button is pressed.
		/// If configured, the "Browse Server" button will appear in Link, Image and Flash dialogs.
		/// Default Value: "" (empty string = disabled)
		/// </summary>
		[CKSerializable]
		public string filebrowserBrowseUrl { get; set; }

		/// <summary>
		/// The location of an external file browser, that should be launched when "Browse Server" button is pressed in the Flash dialog.
		/// If not set, CKEditor will use CKEDITOR.config.filebrowserBrowseUrl.
		/// Default Value: "" (empty string = disabled)
		/// </summary>
		[CKSerializable]
		public string filebrowserFlashBrowseUrl { get; set; }

		/// <summary>
		/// The location of a script that handles file uploads in the Flash dialog. If not set, CKEditor will use CKEDITOR.config.filebrowserUploadUrl.
		/// Default Value: "" (empty string = disabled)
		/// </summary>
		[CKSerializable]
		public string filebrowserFlashUploadUrl { get; set; }

		/// <summary>
		/// The location of an external file browser, that should be launched when "Browse Server" button is pressed in the Link tab of Image dialog.
		/// If not set, CKEditor will use CKEDITOR.config.filebrowserBrowseUrl.
		/// Default Value: "" (empty string = disabled)
		/// </summary>
		[CKSerializable]
		public string filebrowserImageBrowseLinkUrl { get; set; }

		/// <summary>
		/// The location of an external file browser, that should be launched when "Browse Server" button is pressed in the Image dialog.
		/// If not set, CKEditor will use CKEDITOR.config.filebrowserBrowseUrl.
		/// Default Value: "" (empty string = disabled)
		/// </summary>
		[CKSerializable]
		public string filebrowserImageBrowseUrl { get; set; }

		/// <summary>
		/// The location of a script that handles file uploads in the Image dialog. If not set, CKEditor will use CKEDITOR.config.filebrowserUploadUrl.
		/// Default Value: "" (empty string = disabled)
		/// </summary>
		[CKSerializable]
		public string filebrowserImageUploadUrl { get; set; }

		/// <summary>
		/// The location of a script that handles file uploads. If set, the "Upload" tab will appear in "Link", "Image" and "Flash" dialogs.
		/// Default Value: "" (empty string = disabled)
		/// </summary>
		[CKSerializable]
		public string filebrowserUploadUrl { get; set; }

		/// <summary>
		/// The "features" to use in the file browser popup window.
		/// Default Value: "location=no,menubar=no,toolbar=no,dependent=yes,minimizable=no,modal=yes,alwaysRaised=yes,resizable=yes,scrollbars=yes"
		/// </summary>
		[CKSerializable]
		public string filebrowserWindowFeatures { get; set; }

		/// <summary>
		/// Whether a filler text (non-breaking space entity -  ) will be inserted into empty block elements in HTML output, 
		/// this is used to render block elements properly with line-height; When a function is instead specified, 
		/// it'll be passed a CKEDITOR.htmlParser.element to decide whether adding the filler text by expecting a boolean return value.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool fillEmptyBlocks { get; set; }

		/// <summary>
		/// Defines the style to be used to highlight results with the find dialog.
		/// Default Value: "{ element : 'span', styles : { 'background-color' : '#004', 'color' : '#fff' } }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object find_highlight { get; set; }

		/// <summary>
		/// The text to be displayed in the Font combo is none of the available values matches the current cursor position or text selection.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string font_defaultLabel { get; set; }

		/// <summary>
		/// The list of fonts names to be displayed in the Font combo in the toolbar. Entries are separated by semi-colons (;),
		/// while it's possible to have more than one font for each entry, in the HTML way (separated by comma).
		/// A display name may be optionally defined by prefixing the entries with the name and the slash character.
		/// For example, "Arial/Arial, Helvetica, sans-serif" will be displayed as "Arial" in the list, but will be outputted as "Arial, Helvetica, sans-serif".
		/// Default Value: 
		/// "Arial/Arial, Helvetica, sans-serif; Comic Sans MS/Comic Sans MS, cursive; Courier New/Courier New, Courier, monospace;
		/// Georgia/Georgia, serif;Lucida Sans Unicode/Lucida Sans Unicode, Lucida Grande, sans-serif;
		/// Tahoma/Tahoma, Geneva, sans-serif;Times New Roman/Times New Roman, Times, serif;
		/// Trebuchet MS/Trebuchet MS, Helvetica, sans-serif; Verdana/Verdana, Geneva, sans-serif"
		/// </summary>
		[CKSerializable(RemoveEnters = true)]
		public string font_names { get; set; }

		/// <summary>
		/// The style definition to be used to apply the font in the text.
		/// Default Value:
		/// "{
		/// 	element		: 'span',
		/// 	styles		: { 'font-family' : '#(family)' },
		/// 	overrides	: [ { element : 'font', attributes : { 'face' : null } } ] 
		/// }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object font_style { get; set; }

		/// <summary>
		/// The text to be displayed in the Font Size combo is none of the available values matches the current cursor position or text selection.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string fontSize_defaultLabel { get; set; }

		/// <summary>
		/// The list of fonts size to be displayed in the Font Size combo in the toolbar. Entries are separated by semi-colons (;).
		/// Any kind of "CSS like" size can be used, like "12px", "2.3em", "130%", "larger" or "x-small".
		/// A display name may be optionally defined by prefixing the entries with the name and the slash character. 
		/// For example, "Bigger Font/14px" will be displayed as "Bigger Font" in the list, but will be outputted as "14px".
		/// Default Value: "8/8px;9/9px;10/10px;11/11px;12/12px;14/14px;16/16px;18/18px;20/20px;22/22px;24/24px;26/26px;28/28px;36/36px;48/48px;72/72px"
		/// </summary>
		[CKSerializable]
		public string fontSize_sizes { get; set; }

		/// <summary>
		/// The style definition to be used to apply the font size in the text.
		/// Default Value: 
		/// "{
		/// 	element		: 'span',
		/// 	styles		: { 'font-size' : '#(size)' },
		/// 	overrides	: [ { element : 'font', attributes : { 'size' : null } } ]
		/// }"
		/// </summary>
		[CKSerializable]
		public object fontSize_style { get; set; }

		/// <summary>
		/// Force the respect of CKEDITOR.config.enterMode as line break regardless of the context, 
		/// E.g. If CKEDITOR.config.enterMode is set to CKEDITOR.ENTER_P, press enter key inside a 'div' will create a new paragraph with 'p' instead of 'div'.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool forceEnterMode { get; set; }

		/// <summary>
		/// Whether to force all pasting operations to insert on plain text into the editor, loosing any formatting information possibly available in the source text.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool forcePasteAsPlainText { get; set; }

		/// <summary>
		/// Whether to force using "&" instead of "&amp;" in elements attributes values,
		/// it's not recommended to change this setting for compliance with the W3C XHTML 1.0 standards (C.12, XHTML 1.0).
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool forceSimpleAmpersand { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Address" format.
		/// Default Value: "{ element : 'address' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_address { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Normal (DIV)" format.
		/// Default Value: "{ element : 'div' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_div { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Heading 1" format.
		/// Default Value: "{ element : 'h1' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_h1 { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Heading 2" format.
		/// Default Value: "{ element : 'h2' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_h2 { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Heading 3" format.
		/// Default Value: "{ element : 'h3' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_h3 { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Heading 4" format.
		/// Default Value: "{ element : 'h4' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_h4 { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Heading 5" format.
		/// Default Value: "{ element : 'h5' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_h5 { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Heading 6" format.
		/// Default Value: "{ element : 'h6' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_h6 { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Normal" format.
		/// Default Value: "{ element : 'p' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_p { get; set; }

		/// <summary>
		/// The style definition to be used to apply the "Formatted" format.
		/// Default Value: "{ element : 'pre' }"
		/// </summary>
		[CKSerializable(IsObject = true)]
		public object format_pre { get; set; }

		/// <summary>
		/// A list of semi colon separated style names (by default tags) representing the style definition for each entry to be displayed
		/// in the Format combo in the toolbar. Each entry must have its relative definition configuration in a setting named
		/// "format_(tagName)". For example, the "p" entry has its definition taken from config.format_p.
		/// Default Value: "p;h1;h2;h3;h4;h5;h6;pre;address;div"
		/// </summary>
		[CKSerializable]
		public string format_tags { get; set; }

		/// <summary>
		/// Indicates whether the contents to be edited are being inputted as a full HTML page. 
		/// A full page includes the <html>, <head> and <body> tags. 
		/// The final output will also reflect this setting, including the <body> contents only if this setting is disabled.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool fullPage { get; set; }

		/// <summary>
		/// The height of editing area( content ), in relative or absolute, e.g. 30px, 5em. Note: Percentage unit is not supported yet.e.g. 30%.
		/// Default Value: "200"
		/// </summary>
		[CKSerializable]
		public string height { get; set; }

		/// <summary>
		/// Whether escape HTML when editor update original input element.
		/// Default Value: true
		/// </summary>
		[CKSerializable(ForceAddToJSON = true)]
		public bool htmlEncodeOutput { get; set; }

		/// <summary>
		/// Whether the editor must output an empty value ("") if it's contents is made by an empty paragraph only.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool ignoreEmptyParagraph { get; set; }

		/// <summary>
		/// Padding text to set off the image in preview area.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string image_previewText { get; set; }

		/// <summary>
		/// Whether to remove links when emptying the link URL field in the image dialog.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool image_removeLinkByEmptyURL { get; set; }

		/// <summary>
		/// List of classes to use for indenting the contents. If it's null, no classes will be used and instead the #indentUnit and #indentOffset properties will be used.
		/// Default Value: []
		/// </summary>
		public string[] indentClasses { get; set; }
		[CKSerializable(Name = "indentClasses", IsObject = true)]
		private string[] indentClassesSer
		{
			get
			{
				List<string> retVal = new List<string>();
				ResolveParameters(indentClasses, retVal, true);
				return retVal.ToArray();
			}
		}

		/// <summary>
		/// Size of each indentation step
		/// Default Value: 40
		/// </summary>
		[CKSerializable]
		public int indentOffset { get; set; }

		/// <summary>
		/// Unit for the indentation style
		/// Default Value: "px"
		/// </summary>
		[CKSerializable]
		public string indentUnit { get; set; }

		/// <summary>
		/// Allows CKEditor to override jQuery.fn.val(), making it possible to use the val() function on textareas, as usual, having it synchronized with CKEditor.
		/// This configuration option is global and executed during the jQuery Adapter loading. It can't be customized across editor instances.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool jqueryOverrideVal { get; set; }

		/// <summary>
		/// A list associating keystrokes to editor commands. Each element in the list is an array where the first item is the keystroke,
		/// and the second is the name of the command to be executed.
		/// Default Value: null
		/// </summary>
		[CKSerializable]
		public object[] justifyClasses { get; set; }

		/// <summary>
		/// A list associating keystrokes to editor commands. Each element in the list is an array where the first item is the keystroke,
		/// and the second is the name of the command to be executed.
		/// Default Value: 
		/// GlobalConfigObj.keystrokes = new object[] 
		/// { 
		/// 	new object[] { CKEDITOR_ALT + 121 /*F10*/, "toolbarFocus" },
		/// 	new object[] { CKEDITOR_ALT + 122 /*F11*/, "elementsPathFocus" },
		/// 	new object[] { CKEDITOR_SHIFT + 121 /*F10*/, "contextMenu" },
		/// 	new object[] { CKEDITOR_CTRL + 90 /*Z*/, "undo" },
		/// 	new object[] { CKEDITOR_CTRL + 89 /*Y*/, "redo" },
		/// 	new object[] { CKEDITOR_CTRL + CKEDITOR_SHIFT + 90 /*Z*/, "redo" },
		/// 	new object[] { CKEDITOR_CTRL + 76 /*L*/, "link" },
		/// 	new object[] { CKEDITOR_CTRL + 66 /*B*/, "bold" },
		/// 	new object[] { CKEDITOR_CTRL + 73 /*I*/, "italic" },
		/// 	new object[] { CKEDITOR_CTRL + 85 /*U*/, "underline" },
		/// 	new object[] { CKEDITOR_ALT + 109 /*-*/, "toolbarCollapse" }
		/// };
		/// </summary>
		[CKSerializable]
		public object[] keystrokes { get; set; }

		/// <summary>
		/// The user interface language localization to use. If empty, the editor automatically localize the editor to the user language,
		/// if supported, otherwise the CKEDITOR.config.defaultLanguage language is used.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string language { get; set; }

		/// <summary>
		/// A comma separated list of items group names to be displayed in the context menu.
		/// The items order will reflect the order in this list if no priority has been definted in the groups.
		/// Default Value: "clipboard,form,tablecell,tablecellproperties,tablerow,tablecolumn,table,anchor,link,image,flash,checkbox,radio,textfield,hiddenfield,imagebutton,button,select,textarea"
		/// </summary>
		[CKSerializable]
		public string menu_groups { get; set; }

		/// <summary>
		/// The HTML to load in the editor when the "new page" command is executed.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string newpage_html { get; set; }

		/// <summary>
		/// The file that provides the MS Word cleanup function for pasting operations.
		/// Note: This is a global configuration shared by all editor instances present in the page.
		/// Default Value: "default"
		/// </summary>
		[CKSerializable]
		public string pasteFromWordCleanupFile { get; set; }

		/// <summary>
		/// Whether to transform MS Word outline numbered headings into lists.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool pasteFromWordNumberedHeadingToList { get; set; }

		/// <summary>
		/// Whether to prompt the user about the clean up of content being pasted from MS Word.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool pasteFromWordPromptCleanup { get; set; }

		/// <summary>
		/// Whether to ignore all font related formatting styles, including: 
		/// font size;
		/// font family;
		/// font foreground/background color.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool pasteFromWordRemoveFontStyles { get; set; }

		/// <summary>
		/// Whether to remove element styles that can't be managed with the editor. Note that this doesn't handle the font specific styles,
		/// which depends on the CKEDITOR.config.pasteFromWordRemoveFontStyles setting instead.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool pasteFromWordRemoveStyles { get; set; }

		//	todo
		// /// <summary>
		// /// Comma separated list of plugins to load and initialize for an editor instance. This should be rarely changed, using instead the CKEDITOR.config.extraPlugins and CKEDITOR.config.removePlugins for customizations.
		// /// </summary>
		// [CKSerializable]
		// public string plugins { get; set; }

		/// <summary>
		/// List of regular expressions to be executed over the input HTML, indicating HTML source code that matched must not present in WYSIWYG mode for editing.
		/// Default Value: []
		/// </summary>
		public string[] protectedSource { get; set; }

		/// <summary>
		/// If true, makes the editor start in read-only state. Otherwise, it will check if the linked <textarea> element has the disabled attribute.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool readOnly { get; set; }

		/// <summary>
		/// The dialog contents to removed. It's a string composed by dialog name and tab name with a colon between them. 
		/// Separate each pair with semicolon (see example). Note: All names are case-sensitive. 
		/// Note: Be cautious when specifying dialog tabs that are mandatory, like "info", 
		/// dialog functionality might be broken because of this!
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string removeDialogTabs { get; set; }

		/// <summary>
		/// A comma separated list of elements attributes to be removed when executing the "remove format" command.
		/// Default Value: "class,style,lang,width,height,align,hspace,valign"
		/// </summary>
		[CKSerializable]
		public string removeFormatAttributes { get; set; }

		/// <summary>
		/// A comma separated list of elements to be removed when executing the "remove " format" command. Note that only inline elements are allowed.
		/// Default Value: "b,big,code,del,dfn,em,font,i,ins,kbd,q,samp,small,span,strike,strong,sub,sup,tt,u,var"
		/// </summary>
		[CKSerializable]
		public string removeFormatTags { get; set; }

		/// <summary>
		/// List of plugins that must not be loaded. This is a tool setting which makes it easier to avoid loading plugins definied 
		/// in the CKEDITOR.config.plugins setting, whithout having to touch it and potentially breaking it.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string removePlugins { get; set; }

		/// <summary>
		/// The directions to which the editor resizing is enabled. Possible values are:
		/// public enum ResizeDir
		/// {
		/// 	Both,
		/// 	Vertical,
		/// 	Horizontal
		///}
		/// Default Value: ResizeDir.Both;
		/// </summary>
		public ResizeDir resize_dir { get; set; }
		[CKSerializable(Name = "resize_dir")]
		private string resize_dirSer { get { return resize_dir.ToString().ToLower(); } }

		/// <summary>
		/// Whether to enable the resizing feature. If disabled the resize handler will not be visible.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool resize_enabled { get; set; }

		/// <summary>
		/// The maximum editor height, in pixels, when resizing it with the resize handle.
		/// Default Value: 3000
		/// </summary>
		[CKSerializable]
		public int resize_maxHeight { get; set; }

		/// <summary>
		/// The maximum editor width, in pixels, when resizing it with the resize handle.
		/// Default Value: 3000
		/// </summary>
		[CKSerializable]
		public int resize_maxWidth { get; set; }

		/// <summary>
		/// The minimum editor height, in pixels, when resizing it with the resize handle. 
		/// Note: It fallbacks to editor's actual height if that's smaller than the default value.
		/// Default Value: 250
		/// </summary>
		[CKSerializable]
		public int resize_minHeight { get; set; }

		/// <summary>
		/// The minimum editor width, in pixels, when resizing it with the resize handle. 
		/// Note: It fallbacks to editor's actual width if that's smaller than the default value.
		/// Default Value: 750
		/// </summary>
		[CKSerializable]
		public int resize_minWidth { get; set; }

		/// <summary>
		/// If enabled (true), turns on SCAYT automatically after loading the editor.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool scayt_autoStartup { get; set; }

		/// <summary>
		/// Customizes the display of SCAYT context menu commands ("Add Word", "Ignore" and "Ignore All"). It must be a string with one or more of the following words separated by a pipe ("|"): 
		/// enum ScaytContextCommands
		/// {
		/// 	Off = 0 - disables all options
		/// 	All = 1 - enables all options
		/// 	Ignore = 2 - enables the "Ignore" option
		/// 	Ignoreall = 4 - enables the "Ignore All" option
		/// 	Add = 8 - enables the "Add Word" option
		/// }
		/// Default Value: ScaytContextCommands.All;
		/// </summary>
		public ScaytContextCommands scayt_contextCommands { get; set; }
		[CKSerializable(Name = "scayt_contextCommands")]
		private string scayt_contextCommandsSer { get { return scayt_contextCommands.ToString().ToLower().Replace(", ", "|").Replace(",", "|"); } }

		/// <summary>
		/// Define order of placing of SCAYT context menu items by groups. It must be a string with one or more of the following words separated by a pipe ("|"): 
		///  enum ScaytContextMenuItemsOrder
		/// {
		/// 	Suggest = 1 - main suggestion word list
		/// 	Moresuggest = 2 - moresuggest
		/// 	Control = 4 - SCAYT commands, such as 'Ignore' and 'Add Word'
		/// }
		/// Default Value: ScaytContextMenuItemsOrder.Suggest | ScaytContextMenuItemsOrder.Moresuggest | ScaytContextMenuItemsOrder.Control;
		/// </summary>
		public ScaytContextMenuItemsOrder scayt_contextMenuItemsOrder { get; set; }
		[CKSerializable(Name = "scayt_contextMenuItemsOrder")]
		private string scayt_contextMenuItemsOrderSer { get { return scayt_contextMenuItemsOrder.ToString().ToLower().Replace(", ", "|").Replace(",", "|"); } }

		/// <summary>
		/// Links SCAYT to custom dictionaries. It's a string containing dictionary ids separared by commas (","). 
		/// Available only for licensed version. Further details at http://wiki.spellchecker.net/doku.php?id=custom_dictionary_support .
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string scayt_customDictionaryIds { get; set; }

		/// <summary>
		/// Sets the customer ID for SCAYT. Required for migration from free version with banner to paid version.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string scayt_customerid { get; set; }

		/// <summary>
		/// Defines the number of SCAYT suggestions to show in the main context menu. The possible values are: 
		/// 0 (zero): All suggestions are displayed in the main context menu.
		/// Positive number: The maximum number of suggestions to shown in context menu. Other entries will be shown in "More Suggestions" sub-menu.
		/// Negative number: No suggestions are shown in the main context menu. All entries will be listed in the "Suggestions" sub-menu.
		/// Default Value: 5
		/// </summary>
		[CKSerializable]
		public int scayt_maxSuggestions { get; set; }

		/// <summary>
		/// Enables/disables the "More Suggestions" sub-menu in the context menu. The possible values are:
		/// public enum ScaytMoreSuggestions
		/// {
		/// 	On,
		/// 	Off
		/// }
		/// Default Value: ScaytMoreSuggestions.On;
		/// </summary>
		public ScaytMoreSuggestions scayt_moreSuggestions { get; set; }
		[CKSerializable(Name = "scayt_moreSuggestions")]
		private string scayt_moreSuggestionsSer { get { return scayt_moreSuggestions.ToString().ToLower(); } }

		/// <summary>
		/// Sets the default spellchecking language for SCAYT.
		/// Default Value: "en_US"
		/// </summary>
		[CKSerializable]
		public string scayt_sLang { get; set; }

		/// <summary>
		/// Set the URL to SCAYT core. Required to switch to licensed version of SCAYT application. 
		/// Further details at http://wiki.spellchecker.net/doku.php?id=3rd:wysiwyg:fckeditor:wscckf3l .
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string scayt_srcUrl { get; set; }

		/// <summary>
		/// Sets the visibility of the SCAYT tabs in the settings dialog and toolbar button.
		/// The value must contain a "1" (enabled) or "0" (disabled) number for each of the following entries, in this precise order,
		/// separated by a comma (","): "Options", "Languages" and "Dictionary".
		/// Default Value: "1,1,1"
		/// </summary>
		[CKSerializable]
		public string scayt_uiTabs { get; set; }

		/// <summary>
		/// Makes it possible to activate a custom dictionary on SCAYT. The user dictionary name must be used. 
		/// Available only for licensed version.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string scayt_userDictionaryName { get; set; }

		private string _sharedSpacesBottom;
		public string sharedSpacesBottom
		{
			get { return _sharedSpacesBottom; }
			set
			{
				_sharedSpacesBottom = value;
				if (!string.IsNullOrEmpty(_sharedSpacesBottom) && !removePlugins.Contains("resize"))
					removePlugins = string.IsNullOrEmpty(removePlugins) ? "resize" : (removePlugins + ",resize");
			}
		}
		private string _sharedSpacesTop;
		public string sharedSpacesTop
		{
			get { return _sharedSpacesTop; }
			set
			{
				_sharedSpacesTop = value;
				if (!string.IsNullOrEmpty(_sharedSpacesTop) && !removePlugins.Contains("maximize"))
					removePlugins = string.IsNullOrEmpty(removePlugins) ? "maximize" : (removePlugins + ",maximize");
			}
		}
		[CKSerializable(IsObject = true)]
		private string sharedSpaces
		{
			get
			{
				string retVal = string.Empty;
				if (!string.IsNullOrEmpty(sharedSpacesBottom) || !string.IsNullOrEmpty(sharedSpacesTop))
				{
					retVal += "{";
					if (!string.IsNullOrEmpty(sharedSpacesTop)) retVal += ("top:'" + sharedSpacesTop + "'");
					if (!string.IsNullOrEmpty(sharedSpacesBottom) && !string.IsNullOrEmpty(sharedSpacesTop)) retVal += ",";
					if (!string.IsNullOrEmpty(sharedSpacesBottom)) retVal += ("bottom:'" + sharedSpacesBottom + "'");
					retVal += "}";
				}
				return retVal;
			}
		}

		/// <summary>
		/// Just like the CKEDITOR.config.enterMode setting, it defines the behavior for the SHIFT+ENTER key.
		/// The allowed values are the following constants, and their relative behavior: 
		/// public enum EnterMode
		/// {
		/// 	P = 1 - new <p> paragraphs are created
		/// 	BR = 2 - lines are broken with <br> elements
		/// 	DIV = 3 - new <div> blocks are created
		/// }
		/// Default Value: EnterMode.P;
		/// </summary>
		public EnterMode shiftEnterMode { get; set; }
		[CKSerializable(Name = "shiftEnterMode")]
		private int shiftEnterModeSer { get { return (int)shiftEnterMode; } }

		/// <summary>
		/// The skin to load. It may be the name of the skin folder inside the editor installation path, or the name and the path separated by a comma.
		/// Default Value: "kama"
		/// </summary>
		[CKSerializable]
		public string skin { get; set; }

		/// <summary>
		/// The number of columns to be generated by the smilies matrix.
		/// Default Value: 8
		/// </summary>
		[CKSerializable]
		public int smiley_columns { get; set; }

		/// <summary>
		/// The description to be used for each of the smileys defined in the CKEDITOR.config.smiley_images setting.
		/// Each entry in this array list must match its relative pair in the CKEDITOR.config.smiley_images setting.
		/// Default Value: 
		/// new string[]
		/// {
		/// 	"smiley", "sad", "wink", "laugh", "frown", "cheeky", "blush", "surprise",
		/// 	"indecision", "angry", "angel", "cool", "devil", "crying", "enlightened", "no",
		/// 	"yes", "heart", "broken heart", "kiss", "mail"
		/// };
		/// </summary>
		[CKSerializable]
		public string[] smiley_descriptions { get; set; }

		/// <summary>
		/// The file names for the smileys to be displayed. These files must be contained inside the URL path defined with the CKEDITOR.config.smiley_path setting.
		/// Default Value: 
		/// new string[]
		/// {
		/// 	"regular_smile.gif", "sad_smile.gif", "wink_smile.gif", "teeth_smile.gif", "confused_smile.gif", "tounge_smile.gif",
		/// 	"embaressed_smile.gif", "omg_smile.gif", "whatchutalkingabout_smile.gif", "angry_smile.gif", "angel_smile.gif", "shades_smile.gif",
		/// 	"devil_smile.gif", "cry_smile.gif", "lightbulb.gif", "thumbs_down.gif", "thumbs_up.gif", "heart.gif",
		/// 	"broken_heart.gif", "kiss.gif", "envelope.gif"
		/// }
		/// </summary>
		[CKSerializable]
		public string[] smiley_images { get; set; }

		/// <summary>
		/// The base path used to build the URL for the smiley images. It must end with a slash.
		/// Default Value: "<CKEditor folder>/plugins/smiley/images/"
		/// </summary>
		[CKSerializable]
		public string smiley_path { get; set; }

		/// <summary>
		/// The list of special characters visible in Special Character dialog.
		/// Default Value: 
		/// new string[]
		/// {
		/// 	"!","\"","#","$","%","&","'","(",")","*","+","-",".","/",
		/// 	"0","1","2","3","4","5","6","7","8","9",":",";",
		/// 	"<","=",">","?","@",
		/// 	"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O",
		/// 	"P","Q","R","S","T","U","V","W","X","Y","Z",
		/// 	"[","]","^","_","`",
		/// 	"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p",
		/// 	"q","r","s","t","u","v","w","x","y","z",
		/// 	"{","|","}","~",
		/// 	"€", "‘", "’", "“", "”", "–", "—", "¡", "¢", "£", "¤", "¥", "¦", "§", "¨", "©", "ª", "«", "¬", "®", "¯", "°", "&", "²", "³", "´", "µ", "¶", "·", "¸", "¹", "º", "&", "¼", "½", "¾", "¿", "À", "Á", "Â", "Ã", "Ä", "Å", "Æ", "Ç", "È", "É", "Ê", "Ë", "Ì", "Í", "Î", "Ï", "Ð", "Ñ", "Ò", "Ó", "Ô", "Õ", "Ö", "×", "Ø", "Ù", "Ú", "Û", "Ü", "Ý", "Þ", "ß", "à", "á", "â", "ã", "ä", "å", "æ", "ç", "è", "é", "ê", "ë", "ì", "í", "î", "ï", "ð", "ñ", "ò", "ó", "ô", "õ", "ö", "÷", "ø", "ù", "ú", "û", "ü", "ü", "ý", "þ", "ÿ", "Œ", "œ", "Ŵ", "Ŷ", "ŵ", "ŷ", "‚", "‛", "„", "…", "™", "►", "•", "→", "⇒", "⇔", "♦", "≈"
		/// }
		/// </summary>
		[CKSerializable]
		public string[] specialChars { get; set; }

		/// <summary>
		/// Sets whether the editor should have the focus when the page loads.
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool startupFocus { get; set; }

		/// <summary>
		/// The mode to load at the editor startup. It depends on the plugins loaded. By default, the "wysiwyg" and "source" modes are available.
		/// public enum StartupMode
		/// {
		/// 	Wysiwyg,
		/// 	Source
		/// }
		/// Default Value: StartupMode.Wysiwyg;
		/// </summary>
		public StartupMode startupMode { get; set; }
		[CKSerializable(Name = "startupMode")]
		private string startupModeSer { get { return startupMode.ToString().ToLower(); } }

		/// <summary>
		/// Whether to automaticaly enable the "show block" command when the editor loads. (StartupShowBlocks in FCKeditor)
		/// Default Value: false
		/// </summary>
		[CKSerializable]
		public bool startupOutlineBlocks { get; set; }

		/// <summary>
		/// Whether to automatically enable the "show borders" command when the editor loads. (ShowBorders in FCKeditor)
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool startupShowBorders { get; set; }

		/// <summary>
		/// A regular expression that defines whether a CSS rule will be skipped by the Stylesheet Parser plugin. A CSS rule matching the regular expression will be ignored and will not be available in the Styles drop-down list.
		/// Default Value: /(^body\.|^\.)/i
		/// </summary>
		[CKSerializable]
		public string stylesheetParser_skipSelectors { get; set; }

		/// <summary>
		/// A regular expression that defines which CSS rules will be used by the Stylesheet Parser plugin. A CSS rule matching the regular expression will be available in the Styles drop-down list.
		/// Default Value: /\w+\.\w+/
		/// </summary>
		[CKSerializable]
		public string stylesheetParser_validSelectors { get; set; }

		/// <summary>
		/// The "styles definition set" to use in the editor. They will be used in the styles combo and the Style selector of the div container.
		/// The styles may be defined in the page containing the editor, or can be loaded on demand from an external file. In the second case, 
		/// if this setting contains only a name, the styles definition file will be loaded from the "styles" folder inside the styles plugin folder.
		/// Otherwise, this setting has the "name:url" syntax, making it possible to set the URL from which loading the styles file.
		/// Previously this setting was available as config.stylesCombo_stylesSet
		/// Default Value: ["'default'"]
		/// </summary>
		public string[] stylesSet { get; set; }
		[CKSerializable(Name = "stylesSet", IsObject = true)]
		private string[] stylesSetSer
		{
			get
			{
				if (stylesSet != null && stylesSet.Length == 1 && stylesSet[0].Contains("{") && stylesSet[0].Contains("}"))
				{
					stylesSet[0] = stylesSet[0].Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
					return stylesSet;
				}
				List<string> retVal = new List<string>();
				ResolveParameters(stylesSet, retVal, true);
				return retVal.ToArray();
			}
		}
		/// <summary>
		/// The editor tabindex value.
		/// Default Value: 0
		/// </summary>
		[CKSerializable]
		public int tabIndex { get; set; }

		/// <summary>
		/// Intructs the editor to add a number of spaces (&nbsp;) to the text when hitting the TAB key.
		/// If set to zero, the TAB key will be used to move the cursor focus to the next element in the page, out of the editor focus.
		/// Default Value: 0
		/// </summary>
		[CKSerializable]
		public int tabSpaces { get; set; }

		/// <summary>
		/// The templates definition set to use. It accepts a list of names separated by comma. It must match definitions loaded with the templates_files setting.
		/// Default Value: "default"
		/// </summary>
		[CKSerializable]
		public string templates { get; set; }

		/// <summary>
		/// The list of templates definition files to load.
		/// Default Value: new string[] { "~/plugins/templates/templates/default.js" };
		/// </summary>
		public string[] templates_files { get; set; }
		[CKSerializable(Name = "templates_files", IsObject = true)]
		private string[] templates_filesSer
		{
			get
			{
				List<string> retVal = new List<string>();
				ResolveParameters(templates_files, retVal, true);
				return retVal.ToArray();
			}
		}

		/// <summary>
		/// Whether the "Replace actual contents" checkbox is checked by default in the Templates dialog.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool templates_replaceContent { get; set; }

		/// <summary>
		/// The theme to be used to build the UI.
		/// Default Value: "default"
		/// </summary>
		[CKSerializable]
		public string theme { get; set; }

		/// <summary>
		/// The toolbox (alias toolbar) definition. It is a toolbar name or an array of toolbars (strips), each one being also an array, containing a list of UI items.
		/// Default Value: "Full"
		/// </summary>
		[CKSerializable]
		public object toolbar { get; set; }

		/// <summary>
		/// The toolbar definition. It is an array of toolbars (strips), each one being also an array, containing a list of UI items.
		/// Note that this setting is composed by "toolbar_" added by the toolbar name, which in this case is called "Basic".
		/// This second part of the setting name can be anything. You must use this name in the CKEDITOR.config.toolbar setting,
		/// so you instruct the editor which toolbar_(name) setting to you.
		/// Default Value: 
		/// new object[]
		/// {
		/// 	new object[] { "Bold", "Italic", "-", "NumberedList", "BulletedList", "-", "Link", "Unlink", "-", "About" },
		/// };
		/// </summary>
		[CKSerializable]
		public object[] toolbar_Basic { get; set; }

		/// <summary>
		/// This is the default toolbar definition used by the editor. It contains all editor features.
		/// Default Value: 
		/// new object[]
		/// {
		/// 	{'Source','-','Save','NewPage','Preview','-','Templates'},
		/// 	{'Cut','Copy','Paste','PasteText','PasteFromWord','-','Print', 'SpellChecker', 'Scayt'},
		/// 	{'Undo','Redo','-','Find','Replace','-','SelectAll','RemoveFormat'},
		/// 	{'Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton', 'HiddenField'},
		/// 	'/',
		/// 	{'Bold','Italic','Underline','Strike','-','Subscript','Superscript'},
		/// 	{'NumberedList','BulletedList','-','Outdent','Indent','Blockquote','CreateDiv'},
		/// 	{'JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock'},
		/// 	{'BidiLtr', 'BidiRtl' },
		/// 	{'Link','Unlink','Anchor'},
		/// 	{'Image','Flash','Table','HorizontalRule','Smiley','SpecialChar','PageBreak','Iframe'},
		/// 	'/',
		/// 	{'Styles','Format','Font','FontSize'},
		/// 	{'TextColor','BGColor'},
		/// 	{'Maximize', 'ShowBlocks','-','About'}
		/// };
		/// </summary>
		[CKSerializable]
		public object[] toolbar_Full { get; set; }

		/// <summary>
		/// This is the default toolbar definition used by the editor. It contains all editor features.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool toolbarCanCollapse { get; set; }

		/// <summary>
		/// When enabled, makes the arrow keys navigation cycle within the current toolbar group. Otherwise the arrows will move trought all items available in the toolbar. The TAB key will still be used to quickly jump among the toolbar groups.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool toolbarGroupCycling { get; set; }

		/// <summary>
		/// The "theme space" to which rendering the toolbar. For the default theme, the recommended options are{
		/// public enum ToolbarLocation
		/// {
		/// 	Top,
		/// 	Bottom
		/// }
		/// Default Value: ToolbarLocation.Top
		/// </summary>
		public ToolbarLocation toolbarLocation { get; set; }
		[CKSerializable(Name = "toolbarLocation")]
		private string toolbarLocationSer { get { return toolbarLocation.ToString().ToLower(); } }

		/// <summary>
		/// Whether the toolbar must start expanded when the editor is loaded.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool toolbarStartupExpanded { get; set; }

		/// <summary>
		/// Specifies the color of the user interface. Works only with the Kama skin.
		/// </summary>
		[CKSerializable]
		public string uiColor { get; set; }

		/// <summary>
		/// Indicates that some of the editor features, like alignment and text direction, should used the "computed value"
		/// of the featureto indicate it's on/off state, instead of using the "real value".
		/// If enabled, in a left to right written document, the "Left Justify" alignment button will show as active,
		/// even if the aligment style is not explicitly applied to the current paragraph in the editor.
		/// Default Value: true
		/// </summary>
		[CKSerializable]
		public bool useComputedState { get; set; }

		/// <summary>
		/// The editor width in CSS size format or pixel integer.
		/// Default Value: ""
		/// </summary>
		[CKSerializable]
		public string width { get; set; }

		/// <summary>
		/// new object[] { "eventName", "function body" }
		/// example: 
		/// CKEditorConfig.CKEditorEventHandler.Add(new object[] { "instanceReady", "function (evt) { alert('Loaded editor: ' + evt.editor.name);}" });
		/// CKEditorConfig.CKEditorEventHandler.Add(new object[] { "mode", "function (evt) { alert('Loaded editor: ' + evt.editor.name);}" });
		/// Default Value: []
		/// </summary>
		public List<object> CKEditorEventHandler { get; set; }
		// add becouse static fields aren't serialize
		private object[] CKEditorEventHandlerSer
		{
			get
			{
				object[] retVal = new object[0];
				if (this.CKEditorEventHandler != null)
				{
					retVal = this.CKEditorEventHandler.ToArray();
					foreach (object item in retVal)
					{
						object[] stringArray = (object[])item;
						if (!((string)stringArray[0]).StartsWith("'") && !((string)stringArray[0]).EndsWith("'"))
							stringArray[0] = "'" + stringArray[0] + "'";
					}
				}
				return retVal;
			}
		}

		/// <summary>
		/// new object[] { "eventName", "function body" }
		/// example:
		/// this.CKEditorInstanceEventHandler.Add(new object[] { "instanceReady", "function (evt) { alert('Loaded editor: ' + evt.editor.name);}" });
		/// this.CKEditorInstanceEventHandler.Add(new object[] { "mode", "function (evt) { alert('Loaded editor: ' + evt.editor.name);}" });
		/// Default Value: []
		/// </summary>
		public List<object> CKEditorInstanceEventHandler { get; set; }
		[CKSerializable(Name = "on", IsObject = true)]
		private object[] CKEditorInstanceEventHandlerSer
		{
			get
			{
				object[] retVal = new object[0];
				if (this.CKEditorInstanceEventHandler != null)
				{
					retVal = this.CKEditorInstanceEventHandler.ToArray();
					foreach (object item in retVal)
					{
						object[] stringArray = (object[])item;
						if (!((string)stringArray[0]).StartsWith("'") && !((string)stringArray[0]).EndsWith("'"))
							stringArray[0] = "'" + stringArray[0] + "'";
					}
				}
				return retVal;
			}
		}

		public System.Collections.Hashtable ExtraOptions { get; set; }
		[CKSerializable(Name = "ExtraOptions")]
		private object[] ExtraOptionsSer
		{
			get
			{
				object[] retVal = new object[this.ExtraOptions.Keys.Count];
				int i = 0;
				foreach (var item in this.ExtraOptions.Keys)
					retVal[i++] = new object[] { item, this.ExtraOptions[item] };
				return retVal;
			}
		}

		#endregion

		#region Default Constructor

		static CKEditorConfig()
		{
			CKEditorConfig GlobalConfigObj = new CKEditorConfig();
			GlobalConfigObj.autoGrow_bottomSpace = 0;
			GlobalConfigObj.autoGrow_maxHeight = 0;
			GlobalConfigObj.autoGrow_minHeight = 200;
			GlobalConfigObj.autoGrow_onStartup = false;
			GlobalConfigObj.autoParagraph = true;
			GlobalConfigObj.autoUpdateElement = true;
			GlobalConfigObj.baseFloatZIndex = 10000;
			GlobalConfigObj.baseHref = string.Empty;
			GlobalConfigObj.basicEntities = true;
			GlobalConfigObj.blockedKeystrokes = new int[] { CKEDITOR_CTRL + 66 /*B*/, CKEDITOR_CTRL + 73 /*I*/, CKEDITOR_CTRL + 85 /*U*/};
			GlobalConfigObj.bodyClass = string.Empty;
			GlobalConfigObj.bodyId = string.Empty;
			GlobalConfigObj.browserContextMenuOnCtrl = true;
			GlobalConfigObj.colorButton_backStyle = @"{ element : 'span', styles : { 'background-color' : '#(color)' } }";
			GlobalConfigObj.colorButton_colors = "000,800000,8B4513,2F4F4F,008080,000080,4B0082,696969,B22222,A52A2A,DAA520,006400,40E0D0,0000CD,800080,808080,F00,FF8C00,FFD700,008000,0FF,00F,EE82EE,A9A9A9,FFA07A,FFA500,FFFF00,00FF00,AFEEEE,ADD8E6,DDA0DD,D3D3D3,FFF0F5,FAEBD7,FFFFE0,F0FFF0,F0FFFF,F0F8FF,E6E6FA,FFF";
			GlobalConfigObj.colorButton_enableMore = true;
			GlobalConfigObj.colorButton_foreStyle = @"{ element : 'span',
styles : { 'color' : '#(color)' },
overrides : [ { element : 'font', attributes : { 'color' : null } } ] }";
			GlobalConfigObj.contentsCss = new string[] { "~/contents.css" };
			GlobalConfigObj.contentsLangDirection = contentsLangDirections.Ui;
			GlobalConfigObj.contentsLanguage = string.Empty;
			GlobalConfigObj.coreStyles_bold = @"{ element : 'strong', overrides : 'b' }";
			GlobalConfigObj.coreStyles_italic = @"{ element : 'em', overrides : 'i' }";
			GlobalConfigObj.coreStyles_strike = @"{ element : 'strike' }";
			GlobalConfigObj.coreStyles_subscript = @"{ element : 'sub' }";
			GlobalConfigObj.coreStyles_superscript = @"{ element : 'sup' }";
			GlobalConfigObj.coreStyles_underline = @"{ element : 'u' }";
			GlobalConfigObj.customConfig = "config.js";
			GlobalConfigObj.defaultLanguage = "en";
			GlobalConfigObj.devtools_styles = "";
			GlobalConfigObj.dialog_backgroundCoverColor = "white";
			GlobalConfigObj.dialog_backgroundCoverOpacity = 0.5;
			GlobalConfigObj.dialog_buttonsOrder = DialogButtonsOrder.OS;
			GlobalConfigObj.dialog_magnetDistance = 20;
			GlobalConfigObj.dialog_startupFocusTab = false;
			GlobalConfigObj.disableNativeSpellChecker = true;
			GlobalConfigObj.disableNativeTableHandles = true;
			GlobalConfigObj.disableObjectResizing = false;
			GlobalConfigObj.disableReadonlyStyling = false;
			GlobalConfigObj.docType = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">";
			GlobalConfigObj.editingBlock = true;
			GlobalConfigObj.emailProtection = string.Empty;
			GlobalConfigObj.enableTabKeyTools = true;
			GlobalConfigObj.enterMode = EnterMode.P;
			GlobalConfigObj.entities = true;
			GlobalConfigObj.entities_additional = "#39";
			GlobalConfigObj.entities_greek = true;
			GlobalConfigObj.entities_latin = true;
			GlobalConfigObj.entities_processNumerical = false.ToString();
			GlobalConfigObj.extraPlugins = string.Empty;
			GlobalConfigObj.filebrowserBrowseUrl = string.Empty;
			GlobalConfigObj.filebrowserFlashBrowseUrl = string.Empty;
			GlobalConfigObj.filebrowserFlashUploadUrl = string.Empty;
			GlobalConfigObj.filebrowserImageBrowseLinkUrl = string.Empty;
			GlobalConfigObj.filebrowserImageBrowseUrl = string.Empty;
			GlobalConfigObj.filebrowserImageUploadUrl = string.Empty;
			GlobalConfigObj.filebrowserUploadUrl = string.Empty;
			GlobalConfigObj.filebrowserWindowFeatures = "location=no,menubar=no,toolbar=no,dependent=yes,minimizable=no,modal=yes,alwaysRaised=yes,resizable=yes,scrollbars=yes";
			GlobalConfigObj.fillEmptyBlocks = true;
			GlobalConfigObj.find_highlight = @"{ element : 'span', styles : { 'background-color' : '#004', 'color' : '#fff' } }";
			GlobalConfigObj.font_defaultLabel = string.Empty;
			GlobalConfigObj.font_names = @"Arial/Arial, Helvetica, sans-serif;
Comic Sans MS/Comic Sans MS, cursive;
Courier New/Courier New, Courier, monospace;
Georgia/Georgia, serif;
Lucida Sans Unicode/Lucida Sans Unicode, Lucida Grande, sans-serif;
Tahoma/Tahoma, Geneva, sans-serif;
Times New Roman/Times New Roman, Times, serif;
Trebuchet MS/Trebuchet MS, Helvetica, sans-serif;
Verdana/Verdana, Geneva, sans-serif";
			GlobalConfigObj.font_style = @"{ element : 'span',
styles : { 'font-family' : '#(family)' },
overrides : [ { element : 'font', attributes : { 'face' : null } } ] }";
			GlobalConfigObj.fontSize_defaultLabel = string.Empty;
			GlobalConfigObj.fontSize_sizes = "8/8px;9/9px;10/10px;11/11px;12/12px;14/14px;16/16px;18/18px;20/20px;22/22px;24/24px;26/26px;28/28px;36/36px;48/48px;72/72px";
			GlobalConfigObj.fontSize_style = @"{ element : 'span',
styles : { 'font-size' : '#(size)' },
overrides : [ { element : 'font', attributes : { 'size' : null } } ] }";
			GlobalConfigObj.forceEnterMode = false;
			GlobalConfigObj.forcePasteAsPlainText = false;
			GlobalConfigObj.forceSimpleAmpersand = false;
			GlobalConfigObj.format_address = @"{ element : 'address' }";
			GlobalConfigObj.format_div = @"{ element : 'div' }";
			GlobalConfigObj.format_h1 = @"{ element : 'h1' }";
			GlobalConfigObj.format_h2 = @"{ element : 'h2' }";
			GlobalConfigObj.format_h3 = @"{ element : 'h3' }";
			GlobalConfigObj.format_h4 = @"{ element : 'h4' }";
			GlobalConfigObj.format_h5 = @"{ element : 'h5' }";
			GlobalConfigObj.format_h6 = @"{ element : 'h6' }";
			GlobalConfigObj.format_p = @"{ element : 'p' }";
			GlobalConfigObj.format_pre = @"{ element : 'pre' }";
			GlobalConfigObj.format_tags = "p;h1;h2;h3;h4;h5;h6;pre;address;div";
			GlobalConfigObj.fullPage = false;
			GlobalConfigObj.height = "200";
			GlobalConfigObj.htmlEncodeOutput = true;
			GlobalConfigObj.ignoreEmptyParagraph = true;
			GlobalConfigObj.image_previewText = string.Empty;
			GlobalConfigObj.image_removeLinkByEmptyURL = true;
			GlobalConfigObj.indentClasses = new string[0];
			GlobalConfigObj.indentOffset = 40;
			GlobalConfigObj.indentUnit = "px";
			GlobalConfigObj.jqueryOverrideVal = true;
			GlobalConfigObj.justifyClasses = null;
			GlobalConfigObj.keystrokes = new object[] 
			{ 
				new object[] { CKEDITOR_ALT + 121 /*F10*/, "toolbarFocus" },
				new object[] { CKEDITOR_ALT + 122 /*F11*/, "elementsPathFocus" },
				new object[] { CKEDITOR_SHIFT + 121 /*F10*/, "contextMenu" },
				new object[] { CKEDITOR_CTRL + 90 /*Z*/, "undo" },
				new object[] { CKEDITOR_CTRL + 89 /*Y*/, "redo" },
				new object[] { CKEDITOR_CTRL + CKEDITOR_SHIFT + 90 /*Z*/, "redo" },
				new object[] { CKEDITOR_CTRL + 76 /*L*/, "link" },
				new object[] { CKEDITOR_CTRL + 66 /*B*/, "bold" },
				new object[] { CKEDITOR_CTRL + 73 /*I*/, "italic" },
				new object[] { CKEDITOR_CTRL + 85 /*U*/, "underline" },
				new object[] { CKEDITOR_ALT + 109 /*-*/, "toolbarCollapse" }
			};
			GlobalConfigObj.language = string.Empty;
			GlobalConfigObj.menu_groups = "clipboard,form,tablecell,tablecellproperties,tablerow,tablecolumn,table,anchor,link,image,flash,checkbox,radio,textfield,hiddenfield,imagebutton,button,select,textarea";
			GlobalConfigObj.newpage_html = string.Empty;
			GlobalConfigObj.pasteFromWordCleanupFile = "default";
			GlobalConfigObj.pasteFromWordNumberedHeadingToList = false;
			GlobalConfigObj.pasteFromWordPromptCleanup = false;
			GlobalConfigObj.pasteFromWordRemoveFontStyles = true;
			GlobalConfigObj.pasteFromWordRemoveStyles = false;
			//	todo
			//this.plugins = string.Empty;
			GlobalConfigObj.protectedSource = new string[0];
			GlobalConfigObj.readOnly = false;
			GlobalConfigObj.removeDialogTabs = string.Empty;
			GlobalConfigObj.removeFormatAttributes = "class,style,lang,width,height,align,hspace,valign";
			GlobalConfigObj.removeFormatTags = "b,big,code,del,dfn,em,font,i,ins,kbd,q,samp,small,span,strike,strong,sub,sup,tt,u,var";
			GlobalConfigObj.removePlugins = string.Empty;
			GlobalConfigObj.resize_dir = ResizeDir.Both;
			GlobalConfigObj.resize_enabled = true;
			GlobalConfigObj.resize_maxHeight = 3000;
			GlobalConfigObj.resize_maxWidth = 3000;
			GlobalConfigObj.resize_minHeight = 250;
			GlobalConfigObj.resize_minWidth = 750;
			GlobalConfigObj.scayt_autoStartup = false;
			GlobalConfigObj.scayt_contextCommands = ScaytContextCommands.All;
			GlobalConfigObj.scayt_contextMenuItemsOrder = ScaytContextMenuItemsOrder.Suggest | ScaytContextMenuItemsOrder.Moresuggest | ScaytContextMenuItemsOrder.Control;
			GlobalConfigObj.scayt_customDictionaryIds = string.Empty;
			GlobalConfigObj.scayt_customerid = string.Empty;
			GlobalConfigObj.scayt_maxSuggestions = 5;
			GlobalConfigObj.scayt_moreSuggestions = ScaytMoreSuggestions.On;
			GlobalConfigObj.scayt_sLang = "en_US";
			GlobalConfigObj.scayt_srcUrl = string.Empty;
			GlobalConfigObj.scayt_uiTabs = "1,1,1";
			GlobalConfigObj.scayt_userDictionaryName = string.Empty;
			GlobalConfigObj.sharedSpacesBottom = string.Empty;
			GlobalConfigObj.sharedSpacesTop = string.Empty;
			GlobalConfigObj.shiftEnterMode = EnterMode.P;
			GlobalConfigObj.skin = "kama";
			GlobalConfigObj.smiley_columns = 8;
			GlobalConfigObj.smiley_descriptions = new string[]
			{
				"smiley", "sad", "wink", "laugh", "frown", "cheeky", "blush", "surprise",
				"indecision", "angry", "angel", "cool", "devil", "crying", "enlightened", "no",
				"yes", "heart", "broken heart", "kiss", "mail"
			};
			GlobalConfigObj.smiley_images = new string[]
			{
				"regular_smile.gif", "sad_smile.gif", "wink_smile.gif", "teeth_smile.gif", "confused_smile.gif", "tounge_smile.gif",
				"embaressed_smile.gif", "omg_smile.gif", "whatchutalkingabout_smile.gif", "angry_smile.gif", "angel_smile.gif", "shades_smile.gif",
				"devil_smile.gif", "cry_smile.gif", "lightbulb.gif", "thumbs_down.gif", "thumbs_up.gif", "heart.gif",
				"broken_heart.gif", "kiss.gif", "envelope.gif"
			};
			GlobalConfigObj.smiley_path = string.Empty;
			GlobalConfigObj.specialChars = new string[]
			{
				"!","\"","#","$","%","&","'","(",")","*","+","-",".","/",
				"0","1","2","3","4","5","6","7","8","9",":",";",
				"<","=",">","?","@",
				"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O",
				"P","Q","R","S","T","U","V","W","X","Y","Z",
				"[","]","^","_","`",
				"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p",
				"q","r","s","t","u","v","w","x","y","z",
				"{","|","}","~",
				"€", "‘", "’", "“", "”", "–", "—", "¡", "¢", "£", "¤", "¥", "¦", "§", "¨", "©", "ª", "«", "¬", "®", "¯", "°", "&", "²", "³", "´", "µ", "¶", "·", "¸", "¹", "º", "&", "¼", "½", "¾", "¿", "À", "Á", "Â", "Ã", "Ä", "Å", "Æ", "Ç", "È", "É", "Ê", "Ë", "Ì", "Í", "Î", "Ï", "Ð", "Ñ", "Ò", "Ó", "Ô", "Õ", "Ö", "×", "Ø", "Ù", "Ú", "Û", "Ü", "Ý", "Þ", "ß", "à", "á", "â", "ã", "ä", "å", "æ", "ç", "è", "é", "ê", "ë", "ì", "í", "î", "ï", "ð", "ñ", "ò", "ó", "ô", "õ", "ö", "÷", "ø", "ù", "ú", "û", "ü", "ü", "ý", "þ", "ÿ", "Œ", "œ", "Ŵ", "Ŷ", "ŵ", "ŷ", "‚", "‛", "„", "…", "™", "►", "•", "→", "⇒", "⇔", "♦", "≈"
			};
			GlobalConfigObj.startupFocus = false;
			GlobalConfigObj.startupMode = StartupMode.Wysiwyg;
			GlobalConfigObj.startupOutlineBlocks = false;
			GlobalConfigObj.startupShowBorders = true;
			GlobalConfigObj.stylesheetParser_skipSelectors = @"/(^body\.|^\.)/i";
			GlobalConfigObj.stylesheetParser_validSelectors = @"/\w+\.\w+/";
			GlobalConfigObj.stylesSet = new string[] { "default" };
			GlobalConfigObj.tabIndex = 0;
			GlobalConfigObj.tabSpaces = 0;
			GlobalConfigObj.templates = "default";
			GlobalConfigObj.templates_files = new string[] { "~/plugins/templates/templates/default.js" };
			GlobalConfigObj.templates_replaceContent = true;
			GlobalConfigObj.theme = "default";
			GlobalConfigObj.toolbar = "Full";
			GlobalConfigObj.toolbar_Basic = new object[]
			{
				new object[] { "Bold", "Italic", "-", "NumberedList", "BulletedList", "-", "Link", "Unlink", "-", "About" },
			};
			GlobalConfigObj.toolbar_Full = new object[]
			{
				new object[] { "Source", "-", "Save", "NewPage", "Preview", "-", "Templates" },
				new object[] { "Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Print", "SpellChecker", "Scayt" },
				new object[] { "Undo", "Redo", "-", "Find", "Replace", "-", "SelectAll", "RemoveFormat" },
				new object[] { "Form", "Checkbox", "Radio", "TextField", "Textarea", "Select", "Button", "ImageButton", "HiddenField" },
				"/",
				new object[] { "Bold", "Italic", "Underline", "Strike", "-", "Subscript", "Superscript" },
				new object[] { "NumberedList", "BulletedList", "-", "Outdent", "Indent", "Blockquote", "CreateDiv" },
				new object[] { "JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock" },
				new object[] { "BidiLtr", "BidiRtl" },
				new object[] { "Link", "Unlink", "Anchor" },
				new object[] { "Image", "Flash", "Table", "HorizontalRule", "Smiley", "SpecialChar", "PageBreak", "Iframe" },
				"/",
				new object[] { "Styles", "Format", "Font", "FontSize" },
				new object[] { "TextColor", "BGColor" },
				new object[] { "Maximize", "ShowBlocks", "-", "About" }
			};
			GlobalConfigObj.toolbarCanCollapse = true;
			GlobalConfigObj.toolbarGroupCycling = true;
			GlobalConfigObj.toolbarLocation = ToolbarLocation.Top;
			GlobalConfigObj.toolbarStartupExpanded = true;
			GlobalConfigObj.uiColor = "#D3D3D3";
			GlobalConfigObj.useComputedState = true;
			GlobalConfigObj.width = string.Empty;
			GlobalConfigObj.CKEditorInstanceEventHandler = null;
			GlobalConfigObj.CKEditorEventHandler = null;
			GlobalConfigObj.ExtraOptions = new System.Collections.Hashtable();
			CKEditorConfig.GlobalConfig = GlobalConfigObj;
		}
		private CKEditorConfig() { }
		public CKEditorConfig(string editorPath)
		{
			this.autoGrow_bottomSpace = CKEditorConfig.GlobalConfig.autoGrow_bottomSpace;
			this.autoGrow_maxHeight = CKEditorConfig.GlobalConfig.autoGrow_maxHeight;
			this.autoGrow_minHeight = CKEditorConfig.GlobalConfig.autoGrow_minHeight;
			this.autoGrow_onStartup = CKEditorConfig.GlobalConfig.autoGrow_onStartup;
			this.autoParagraph = CKEditorConfig.GlobalConfig.autoParagraph;
			this.autoUpdateElement = CKEditorConfig.GlobalConfig.autoUpdateElement;
			this.baseFloatZIndex = CKEditorConfig.GlobalConfig.baseFloatZIndex;
			this.baseHref = CKEditorConfig.GlobalConfig.baseHref;
			this.basicEntities = CKEditorConfig.GlobalConfig.basicEntities;
			this.blockedKeystrokes = CKEditorConfig.GlobalConfig.blockedKeystrokes;
			this.bodyClass = CKEditorConfig.GlobalConfig.bodyClass;
			this.bodyId = CKEditorConfig.GlobalConfig.bodyId;
			this.browserContextMenuOnCtrl = CKEditorConfig.GlobalConfig.browserContextMenuOnCtrl;
			this.colorButton_backStyle = CKEditorConfig.GlobalConfig.colorButton_backStyle;
			this.colorButton_colors = CKEditorConfig.GlobalConfig.colorButton_colors;
			this.colorButton_enableMore = CKEditorConfig.GlobalConfig.colorButton_enableMore;
			this.colorButton_foreStyle = CKEditorConfig.GlobalConfig.colorButton_foreStyle;
			this.contentsCss = ResolveUrl(CKEditorConfig.GlobalConfig.contentsCss, editorPath);
			this.contentsLangDirection = CKEditorConfig.GlobalConfig.contentsLangDirection;
			this.contentsLanguage = CKEditorConfig.GlobalConfig.contentsLanguage;
			this.coreStyles_bold = CKEditorConfig.GlobalConfig.coreStyles_bold;
			this.coreStyles_italic = CKEditorConfig.GlobalConfig.coreStyles_italic;
			this.coreStyles_strike = CKEditorConfig.GlobalConfig.coreStyles_strike;
			this.coreStyles_subscript = CKEditorConfig.GlobalConfig.coreStyles_subscript;
			this.coreStyles_superscript = CKEditorConfig.GlobalConfig.coreStyles_superscript;
			this.coreStyles_underline = CKEditorConfig.GlobalConfig.coreStyles_underline;
			this.customConfig = CKEditorConfig.GlobalConfig.customConfig;
			this.defaultLanguage = CKEditorConfig.GlobalConfig.defaultLanguage;
			this.devtools_styles = CKEditorConfig.GlobalConfig.devtools_styles;
			this.dialog_backgroundCoverColor = CKEditorConfig.GlobalConfig.dialog_backgroundCoverColor;
			this.dialog_backgroundCoverOpacity = CKEditorConfig.GlobalConfig.dialog_backgroundCoverOpacity;
			this.dialog_buttonsOrder = CKEditorConfig.GlobalConfig.dialog_buttonsOrder;
			this.dialog_magnetDistance = CKEditorConfig.GlobalConfig.dialog_magnetDistance;
			this.dialog_startupFocusTab = CKEditorConfig.GlobalConfig.dialog_startupFocusTab;
			this.disableNativeSpellChecker = CKEditorConfig.GlobalConfig.disableNativeSpellChecker;
			this.disableNativeTableHandles = CKEditorConfig.GlobalConfig.disableNativeTableHandles;
			this.disableObjectResizing = CKEditorConfig.GlobalConfig.disableObjectResizing;
			this.disableReadonlyStyling = CKEditorConfig.GlobalConfig.disableReadonlyStyling;
			this.docType = CKEditorConfig.GlobalConfig.docType;
			this.editingBlock = CKEditorConfig.GlobalConfig.editingBlock;
			this.emailProtection = CKEditorConfig.GlobalConfig.emailProtection;
			this.enableTabKeyTools = CKEditorConfig.GlobalConfig.enableTabKeyTools;
			this.enterMode = CKEditorConfig.GlobalConfig.enterMode;
			this.entities = CKEditorConfig.GlobalConfig.entities;
			this.entities_additional = CKEditorConfig.GlobalConfig.entities_additional;
			this.entities_greek = CKEditorConfig.GlobalConfig.entities_greek;
			this.entities_latin = CKEditorConfig.GlobalConfig.entities_latin;
			this.entities_processNumerical = CKEditorConfig.GlobalConfig.entities_processNumerical;
			this.extraPlugins = CKEditorConfig.GlobalConfig.extraPlugins;
			this.filebrowserBrowseUrl = CKEditorConfig.GlobalConfig.filebrowserBrowseUrl;
			this.filebrowserFlashBrowseUrl = CKEditorConfig.GlobalConfig.filebrowserFlashBrowseUrl;
			this.filebrowserFlashUploadUrl = CKEditorConfig.GlobalConfig.filebrowserFlashUploadUrl;
			this.filebrowserImageBrowseLinkUrl = CKEditorConfig.GlobalConfig.filebrowserImageBrowseLinkUrl;
			this.filebrowserImageBrowseUrl = CKEditorConfig.GlobalConfig.filebrowserImageBrowseUrl;
			this.filebrowserImageUploadUrl = CKEditorConfig.GlobalConfig.filebrowserImageUploadUrl;
			this.filebrowserUploadUrl = CKEditorConfig.GlobalConfig.filebrowserUploadUrl;
			this.filebrowserWindowFeatures = CKEditorConfig.GlobalConfig.filebrowserWindowFeatures;
			this.fillEmptyBlocks = CKEditorConfig.GlobalConfig.fillEmptyBlocks;
			this.find_highlight = CKEditorConfig.GlobalConfig.find_highlight;
			this.font_defaultLabel = CKEditorConfig.GlobalConfig.font_defaultLabel;
			this.font_names = CKEditorConfig.GlobalConfig.font_names;
			this.font_style = CKEditorConfig.GlobalConfig.font_style;
			this.fontSize_defaultLabel = CKEditorConfig.GlobalConfig.fontSize_defaultLabel;
			this.fontSize_sizes = CKEditorConfig.GlobalConfig.fontSize_sizes;
			this.fontSize_style = CKEditorConfig.GlobalConfig.fontSize_style;
			this.forceEnterMode = CKEditorConfig.GlobalConfig.forceEnterMode;
			this.forcePasteAsPlainText = CKEditorConfig.GlobalConfig.forcePasteAsPlainText;
			this.forceSimpleAmpersand = CKEditorConfig.GlobalConfig.forceSimpleAmpersand;
			this.format_address = CKEditorConfig.GlobalConfig.format_address;
			this.format_div = CKEditorConfig.GlobalConfig.format_div;
			this.format_h1 = CKEditorConfig.GlobalConfig.format_h1;
			this.format_h2 = CKEditorConfig.GlobalConfig.format_h2;
			this.format_h3 = CKEditorConfig.GlobalConfig.format_h3;
			this.format_h4 = CKEditorConfig.GlobalConfig.format_h4;
			this.format_h5 = CKEditorConfig.GlobalConfig.format_h5;
			this.format_h6 = CKEditorConfig.GlobalConfig.format_h6;
			this.format_p = CKEditorConfig.GlobalConfig.format_p;
			this.format_pre = CKEditorConfig.GlobalConfig.format_pre;
			this.format_tags = CKEditorConfig.GlobalConfig.format_tags;
			this.fullPage = CKEditorConfig.GlobalConfig.fullPage;
			this.height = CKEditorConfig.GlobalConfig.height;
			this.htmlEncodeOutput = CKEditorConfig.GlobalConfig.htmlEncodeOutput;
			this.ignoreEmptyParagraph = CKEditorConfig.GlobalConfig.ignoreEmptyParagraph;
			this.image_previewText = CKEditorConfig.GlobalConfig.image_previewText;
			this.image_removeLinkByEmptyURL = CKEditorConfig.GlobalConfig.image_removeLinkByEmptyURL;
			this.indentClasses = CKEditorConfig.GlobalConfig.indentClasses;
			this.indentOffset = CKEditorConfig.GlobalConfig.indentOffset;
			this.jqueryOverrideVal = CKEditorConfig.GlobalConfig.jqueryOverrideVal;
			this.indentUnit = CKEditorConfig.GlobalConfig.indentUnit;
			this.justifyClasses = CKEditorConfig.GlobalConfig.justifyClasses;
			this.keystrokes = CKEditorConfig.GlobalConfig.keystrokes;
			this.language = CKEditorConfig.GlobalConfig.language;
			this.menu_groups = CKEditorConfig.GlobalConfig.menu_groups;
			this.newpage_html = CKEditorConfig.GlobalConfig.newpage_html;
			this.pasteFromWordCleanupFile = CKEditorConfig.GlobalConfig.pasteFromWordCleanupFile;
			this.pasteFromWordNumberedHeadingToList = CKEditorConfig.GlobalConfig.pasteFromWordNumberedHeadingToList;
			this.pasteFromWordPromptCleanup = CKEditorConfig.GlobalConfig.pasteFromWordPromptCleanup;
			this.pasteFromWordRemoveFontStyles = CKEditorConfig.GlobalConfig.pasteFromWordRemoveFontStyles;
			this.pasteFromWordRemoveStyles = CKEditorConfig.GlobalConfig.pasteFromWordRemoveStyles;
			this.protectedSource = CKEditorConfig.GlobalConfig.protectedSource;
			this.readOnly = CKEditorConfig.GlobalConfig.readOnly;
			this.removeDialogTabs = CKEditorConfig.GlobalConfig.removeDialogTabs;
			this.removeFormatAttributes = CKEditorConfig.GlobalConfig.removeFormatAttributes;
			this.removeFormatTags = CKEditorConfig.GlobalConfig.removeFormatTags;
			this.removePlugins = CKEditorConfig.GlobalConfig.removePlugins;
			this.resize_dir = CKEditorConfig.GlobalConfig.resize_dir;
			this.resize_enabled = CKEditorConfig.GlobalConfig.resize_enabled;
			this.resize_maxHeight = CKEditorConfig.GlobalConfig.resize_maxHeight;
			this.resize_maxWidth = CKEditorConfig.GlobalConfig.resize_maxWidth;
			this.resize_minHeight = CKEditorConfig.GlobalConfig.resize_minHeight;
			this.resize_minWidth = CKEditorConfig.GlobalConfig.resize_minWidth;
			this.scayt_autoStartup = CKEditorConfig.GlobalConfig.scayt_autoStartup;
			this.scayt_contextCommands = CKEditorConfig.GlobalConfig.scayt_contextCommands;
			this.scayt_contextMenuItemsOrder = CKEditorConfig.GlobalConfig.scayt_contextMenuItemsOrder;
			this.scayt_customDictionaryIds = CKEditorConfig.GlobalConfig.scayt_customDictionaryIds;
			this.scayt_customerid = CKEditorConfig.GlobalConfig.scayt_customerid;
			this.scayt_maxSuggestions = CKEditorConfig.GlobalConfig.scayt_maxSuggestions;
			this.scayt_moreSuggestions = CKEditorConfig.GlobalConfig.scayt_moreSuggestions;
			this.scayt_sLang = CKEditorConfig.GlobalConfig.scayt_sLang;
			this.scayt_srcUrl = CKEditorConfig.GlobalConfig.scayt_srcUrl;
			this.scayt_uiTabs = CKEditorConfig.GlobalConfig.scayt_uiTabs;
			this.scayt_userDictionaryName = CKEditorConfig.GlobalConfig.scayt_userDictionaryName;
			this.sharedSpacesBottom = CKEditorConfig.GlobalConfig.sharedSpacesBottom;
			this.sharedSpacesTop = CKEditorConfig.GlobalConfig.sharedSpacesTop;
			this.shiftEnterMode = CKEditorConfig.GlobalConfig.shiftEnterMode;
			this.skin = CKEditorConfig.GlobalConfig.skin;
			this.smiley_columns = CKEditorConfig.GlobalConfig.smiley_columns;
			this.smiley_descriptions = CKEditorConfig.GlobalConfig.smiley_descriptions;
			this.smiley_images = CKEditorConfig.GlobalConfig.smiley_images;
			this.smiley_path = CKEditorConfig.GlobalConfig.smiley_path;
			this.specialChars = CKEditorConfig.GlobalConfig.specialChars;
			this.startupFocus = CKEditorConfig.GlobalConfig.startupFocus;
			this.startupMode = CKEditorConfig.GlobalConfig.startupMode;
			this.startupOutlineBlocks = CKEditorConfig.GlobalConfig.startupOutlineBlocks;
			this.startupShowBorders = CKEditorConfig.GlobalConfig.startupShowBorders;
			this.stylesheetParser_skipSelectors = CKEditorConfig.GlobalConfig.stylesheetParser_skipSelectors;
			this.stylesheetParser_validSelectors = CKEditorConfig.GlobalConfig.stylesheetParser_validSelectors;
			this.stylesSet = CKEditorConfig.GlobalConfig.stylesSet;
			this.tabIndex = CKEditorConfig.GlobalConfig.tabIndex;
			this.tabSpaces = CKEditorConfig.GlobalConfig.tabSpaces;
			this.templates = CKEditorConfig.GlobalConfig.templates;
			this.templates_files = ResolveUrl(CKEditorConfig.GlobalConfig.templates_files, editorPath);
			this.templates_replaceContent = CKEditorConfig.GlobalConfig.templates_replaceContent;
			this.theme = CKEditorConfig.GlobalConfig.theme;
			this.toolbar = CKEditorConfig.GlobalConfig.toolbar;
			this.toolbar_Basic = CKEditorConfig.GlobalConfig.toolbar_Basic;
			this.toolbar_Full = CKEditorConfig.GlobalConfig.toolbar_Full;
			this.toolbarCanCollapse = CKEditorConfig.GlobalConfig.toolbarCanCollapse;
			this.toolbarGroupCycling = CKEditorConfig.GlobalConfig.toolbarGroupCycling;
			this.toolbarLocation = CKEditorConfig.GlobalConfig.toolbarLocation;
			this.uiColor = CKEditorConfig.GlobalConfig.uiColor;
			this.toolbarStartupExpanded = CKEditorConfig.GlobalConfig.toolbarStartupExpanded;
			this.useComputedState = CKEditorConfig.GlobalConfig.useComputedState;
			this.width = CKEditorConfig.GlobalConfig.width;
			this.ExtraOptions = new System.Collections.Hashtable(CKEditorConfig.GlobalConfig.ExtraOptions);
			this.CKEditorEventHandler = null;
			if (CKEditorInstanceEventHandler != null)
				this.CKEditorInstanceEventHandler = new List<object>(CKEditorConfig.GlobalConfig.CKEditorInstanceEventHandler);
		}
		private string[] ResolveUrl(string[] value, string resolvedStr)
		{
			for (int i = 0; i < value.Length; i++)
				if (value[i].StartsWith("~") && !value[i].StartsWith(resolvedStr))
					value[i] = resolvedStr + value[i].Replace("~", "");
			return value;
		}
		private void ResolveParameters(string[] parIn, List<string> retVal, bool addAp)
		{
			foreach (string item in parIn)
			{
				if (item.StartsWith("[") || item.EndsWith("]"))
					ResolveParameters(item.Trim().Replace("[", string.Empty).Replace("]", string.Empty).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), retVal, addAp);
				else if (addAp && !item.StartsWith("'") && !item.EndsWith("'"))
					retVal.Add("'" + item.Trim().Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty) + "'");
				else
					retVal.Add(item.Trim().Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty));
			}
		}
		private string ResolveParameters(string parIn, bool addAp)
		{
			if (parIn.StartsWith("[") && parIn.EndsWith("]"))
				return parIn;
			else if (addAp && !parIn.StartsWith("'") && !parIn.EndsWith("'"))
				return "'" + parIn + "'";
			else
				return parIn;
		}
		#endregion
	}
}