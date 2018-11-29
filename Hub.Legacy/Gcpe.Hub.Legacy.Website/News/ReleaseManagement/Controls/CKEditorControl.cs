/*
 * Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Collections.Specialized;

namespace CKEditor.NET
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:CKEditorControl runat=server></{0}:CKEditorControl>")]
    [ParseChildren(false)]
    [Designer("CKEditor.NET.CKEditorControlDesigner")]
    public class CKEditorControl : TextBox, IPostBackDataHandler
    {
        #region Changed TextBox Property

        [Bindable(true)]
        [Category("CKEditor Basic Settings")]
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        public override TextBoxMode TextMode { get { return base.TextMode; } }

        [Description("If true, makes the editor start in read-only state. Otherwise, it will check if the linked <textarea> element has the disabled attribute.")]
        [DefaultValue(false)]
        public override bool ReadOnly
        {
            get
            {
                return config.readOnly;
            }
            set
            {
                config.readOnly = base.ReadOnly = value;
            }
        }

        #endregion

        #region Private Property

        private string CKEditorJSFile
        {
            get
            {
                return this.BasePath + "/ckeditor.js";
            }
        }

        private bool isChanged = false;

        #endregion

        #region CKEditor Other Settings Property

        [Category("CKEditor Basic Settings")]
        [DefaultValue("~/Scripts/ckeditor")]
        public string BasePath
        {
            get
            {
                object obj = ViewState["BasePath"];
                if (obj == null)
                {
                    obj = System.Configuration.ConfigurationManager.AppSettings["CKeditor:BasePath"];
                    ViewState["BasePath"] = (obj == null ? "~/Scripts/ckeditor" : (string)obj);
                }
                return (string)ViewState["BasePath"];
            }
            set
            {
                if (value.EndsWith("/"))
                    value = value.Remove(value.Length - 1);
                ViewState["BasePath"] = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CKEditorConfig config
        {
            get
            {
                if (ViewState["CKEditorConfig"] == null)
                    ViewState["CKEditorConfig"] = new CKEditorConfig(this.BasePath.StartsWith("~") ? this.ResolveUrl(this.BasePath) : this.BasePath);
                return (CKEditorConfig)ViewState["CKEditorConfig"];
            }
            set { ViewState["CKEditorConfig"] = value; }
        }

        [Category("CKEditor Basic Settings")]
        [Description("The height of editing area( content ), in relative or absolute, e.g. 30px, 5em. Note: Percentage unit is not supported yet.e.g. 30%.")]
        [DefaultValue(typeof(System.Web.UI.WebControls.Unit), "200px")]
        public override Unit Height
        {
            get
            {
                System.Web.UI.WebControls.Unit result = new Unit(string.Empty);
                try
                {
                    result = System.Web.UI.WebControls.Unit.Parse(config.height);
                    base.Height = result;
                }
                catch { }
                return result;
            }
            set
            {
                System.Web.UI.WebControls.Unit result = new Unit(string.Empty);
                try
                {
                    result = value;
                    base.Height = result;
                }
                catch { }
                config.height = value.ToString().Replace("px", string.Empty);
            }
        }

        [Category("CKEditor Basic Settings")]
        [Description("The editor width in CSS size format or pixel integer.")]
        [DefaultValue(typeof(System.Web.UI.WebControls.Unit), "100%")]
        public override Unit Width
        {
            get
            {
                System.Web.UI.WebControls.Unit result = new Unit(string.Empty);
                try
                {
                    result = System.Web.UI.WebControls.Unit.Parse(config.width);
                    base.Width = result;
                }
                catch { }
                return result;
            }
            set
            {
                System.Web.UI.WebControls.Unit result = new Unit(string.Empty);
                try
                {
                    result = value;
                    base.Width = result;
                }
                catch { }
                config.width = value.ToString().Replace("px", string.Empty);
            }
        }

        [Category("CKEditor Other Settings")]
        [Description("The editor tabindex value.")]
        [DefaultValue(typeof(short), "0")]
        public override short TabIndex { get { return (short)config.tabIndex; } set { config.tabIndex = value; } }

        #region Encaspulate config

        [Category("CKEditor Other Settings")]
        [Description("Extra height in pixel to leave between the bottom boundary of content with document size when auto resizing.")]
        [DefaultValue(0)]
        public int AutoGrowBottomSpace { get { return config.autoGrow_bottomSpace; } set { config.autoGrow_bottomSpace = value; } }

        [Category( "CKEditor Other Settings" )]
        [Description( "Whether to have the auto grow happen on editor creation." )]
        [DefaultValue(false)]
        public bool AutoGrowOnStartup { get { return config.autoGrow_onStartup; } set { config.autoGrow_onStartup = value; } }

        [Category("CKEditor Other Settings")]
        [Description("The maximum height to which the editor can reach using AutoGrow. Zero means unlimited.")]
        [DefaultValue(0)]
        public int AutoGrowMaxHeight { get { return config.autoGrow_maxHeight; } set { config.autoGrow_maxHeight = value; } }

        [Category("CKEditor Other Settings")]
        [Description("The minimum height to which the editor can reach using AutoGrow.")]
        [DefaultValue(200)]
        public int AutoGrowMinHeight { get { return config.autoGrow_minHeight; } set { config.autoGrow_minHeight = value; } }

        [Category("CKEditor Other Settings")]
        [Description( "Whether automatically create wrapping blocks around inline contents inside document body, this helps to ensure the integrality of the block enter mode. Note: Changing the default value might introduce unpredictable usability issues." )]
        [DefaultValue(true)]
        public bool AutoParagraph { get { return config.autoParagraph; } set { config.autoParagraph = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether the replaced element (usually a textarea) is to be updated automatically when posting the form containing the editor.")]
        [DefaultValue(true)]
        public bool AutoUpdateElement { get { return config.autoUpdateElement; } set { config.autoUpdateElement = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether to escape basic HTML entities in the document, including: nbsp, gt, lt, amp.")]
        [DefaultValue(true)]
        public bool BasicEntities { get { return config.basicEntities; } set { config.basicEntities = value; } }

        [Category("CKEditor Basic Settings")]
        [Description("The base href URL used to resolve relative and absolute URLs in the editor content.")]
        [DefaultValue("")]
        public string BaseHref { get { return config.baseHref; } set { config.baseHref = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Sets the ""class"" attribute to be used on the body element of the editing area. This can be useful when reusing the original CSS
file you're using on your live website and you want to assign to the editor the same class name you're using for the region 
that'll hold the contents. In this way, class specific CSS rules will be enabled.")]
        [DefaultValue("")]
        public string BodyClass { get { return config.bodyClass; } set { config.bodyClass = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Sets the ""id"" attribute to be used on the body element of the editing area. This can be useful when reusing the original CSS 
file you're using on your live website and you want to assign to the editor the same id you're using for the region 
that'll hold the contents. In this way, id specific CSS rules will be enabled.")]
        [DefaultValue("")]
        public string BodyId { get { return config.bodyId; } set { config.bodyId = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether to show the browser native context menu when the CTRL or the META (Mac) key is pressed while opening the context menu.")]
        [DefaultValue(true)]
        public bool BrowserContextMenuOnCtrl { get { return config.browserContextMenuOnCtrl; } set { config.browserContextMenuOnCtrl = value; } }

        [PersistenceMode(PersistenceMode.Attribute)]
        [Category("CKEditor Basic Settings")]
        [Description("The CSS file(s) to be used to apply style to the contents. It should reflect the CSS used in the final pages where the contents are to be used.")]
        [DefaultValue("~/Scripts/ckeditor/contents.css")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ContentsCss
        {
            get
            {
                string retVal = string.Empty;
                foreach (string item in config.contentsCss) retVal += item + ",";
                if (retVal.EndsWith(",")) retVal = retVal.Remove(retVal.Length - 1);
                return retVal;
            }
            set { config.contentsCss = value.Replace("\t", string.Empty).Split(new char[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries); }
        }

        [Category("CKEditor Other Settings")]
        [Description(@"The writing direction of the language used to write the editor contents. Allowed values are: 
'ui' - which indicate content direction will be the same with the user interface language direction;
'ltr' - for Left-To-Right language (like English);
'rtl' - for Right-To-Left languages (like Arabic).")]
        [DefaultValue(typeof(contentsLangDirections), "Ui")]
        public contentsLangDirections ContentsLangDirection { get { return config.contentsLangDirection; } set { config.contentsLangDirection = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Language code of the writing language which is used to author the editor contents.")]
        [DefaultValue("")]
        public string ContentsLanguage { get { return config.contentsLanguage; } set { config.contentsLanguage = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"The URL path for the custom configuration file to be loaded. If not overloaded with inline configurations, it defaults 
to the ""config.js"" file present in the root of the CKEditor installation directory.
CKEditor will recursively load custom configuration files defined inside other custom configuration files.")]
        [DefaultValue("config.js")]
        public string CustomConfig { get { return config.customConfig; } set { config.customConfig = value; } }

        [Category("CKEditor Basic Settings")]
        [Description("The language to be used if CKEDITOR.config.language is left empty and it's not possible to localize the editor to the user language.")]
        [DefaultValue("en")]
        public string DefaultLanguage { get { return config.defaultLanguage; } set { config.defaultLanguage = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"The guideline to follow when generating the dialog buttons. There are 3 possible options:
'OS' - the buttons will be displayed in the default order of the user's OS;
'ltr' - for Left-To-Right order;
'rtl' - for Right-To-Left order.")]
        [DefaultValue(typeof(DialogButtonsOrder), "OS")]
        public DialogButtonsOrder DialogButtonsOrder { get { return config.dialog_buttonsOrder; } set { config.dialog_buttonsOrder = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Disables the built-in spell checker while typing natively available in the browser (currently Firefox and Safari only).
Even if word suggestions will not appear in the CKEditor context menu, this feature is useful to help quickly identifying misspelled words.
This setting is currently compatible with Firefox only due to limitations in other browsers.")]
        [DefaultValue(true)]
        public bool DisableNativeSpellChecker { get { return config.disableNativeSpellChecker; } set { config.disableNativeSpellChecker = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Disables the ""table tools"" offered natively by the browser (currently Firefox only) to make quick table editing operations, 
like adding or deleting rows and columns.")]
        [DefaultValue(true)]
        public bool DisableNativeTableHandles { get { return config.disableNativeTableHandles; } set { config.disableNativeTableHandles = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Disables the ability of resize objects (image and tables) in the editing area.")]
        [DefaultValue(false)]
        public bool DisableObjectResizing { get { return config.disableObjectResizing; } set { config.disableObjectResizing = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Disables inline styling on read-only elements.")]
        [DefaultValue(false)]
        public bool DisableReadonlyStyling { get { return config.disableReadonlyStyling; } set { config.disableReadonlyStyling = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Sets the doctype to be used when loading the editor content as HTML.")]
        [DefaultValue(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">")]
        public string DocType { get { return config.docType; } set { config.docType = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether to render or not the editing block area in the editor interface.")]
        [DefaultValue(true)]
        public bool EditingBlock { get { return config.editingBlock; } set { config.editingBlock = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"The e-mail address anti-spam protection option. The protection will be applied when creating or modifying e-mail links through the editor interface.
Two methods of protection can be chosen:
The e-mail parts (name, domain and any other query string) are assembled into a function call pattern. Such function must be provided by 
the developer in the pages that will use the contents. 
Only the e-mail address is obfuscated into a special string that has no meaning for humans or spam bots, but which is properly rendered and accepted by the browser.
Both approaches require JavaScript to be enabled.")]
        [DefaultValue("")]
        public string EmailProtection { get { return config.emailProtection; } set { config.emailProtection = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Allow context-sensitive tab key behaviors, including the following scenarios: 
When selection is anchored inside table cells:
If TAB is pressed, select the contents of the ""next"" cell. If in the last cell in the table, add a new row to it and focus its first cell.
If SHIFT+TAB is pressed, select the contents of the ""previous"" cell. Do nothing when it's in the first cell.")]
        [DefaultValue(true)]
        public bool EnableTabKeyTools { get { return config.enableTabKeyTools; } set { config.enableTabKeyTools = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Sets the behavior for the ENTER key. It also dictates other behaviour rules in the editor, like whether the <br> element is to be used as a paragraph separator when indenting text.
The allowed values are the following constants, and their relative behavior: 
CKEDITOR.ENTER_P (1): new <p> paragraphs are created;
CKEDITOR.ENTER_BR (2): lines are broken with <br> elements;
CKEDITOR.ENTER_DIV (3): new <div> blocks are created.
Note: It's recommended to use the CKEDITOR.ENTER_P value because of its semantic value and correctness. The editor is optimized for this value.")]
        [DefaultValue(typeof(EnterMode), "P")]
        public EnterMode EnterMode { get { return config.enterMode; } set { config.enterMode = value; } }

        [Category("CKEditor Basic Settings")]
        [Description("Whether to use HTML entities in the output.")]
        [DefaultValue(true)]
        public bool Entities { get { return config.entities; } set { config.entities = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"An additional list of entities to be used. It's a string containing each entry separated by a comma.
Entities names or number must be used, exclusing the ""&"" preffix and the "";"" termination.")]
        [DefaultValue("#39")]
        public string EntitiesAdditional { get { return config.entities_additional; } set { config.entities_additional = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"Whether to convert some symbols, mathematical symbols, and Greek letters to HTML entities. This may be more relevant for users typing text written in Greek.
The list of entities can be found at the W3C HTML 4.01 Specification, section 24.3.1.")]
        [DefaultValue(true)]
        public bool EntitiesGreek { get { return config.entities_greek; } set { config.entities_greek = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"Whether to convert some Latin characters (Latin alphabet No. 1, ISO 8859-1) to HTML entities.
The list of entities can be found at the W3C HTML 4.01 Specification, section 24.2.1.")]
        [DefaultValue(true)]
        public bool EntitiesLatin { get { return config.entities_latin; } set { config.entities_latin = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"List of additional plugins to be loaded. 
This is a tool setting which makes it easier to add new plugins, whithout having to touch and possibly breaking the CKEDITOR.config.plugins setting.")]
        [DefaultValue("")]
        public string ExtraPlugins { get { return config.extraPlugins; } set { config.extraPlugins = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"The location of an external file browser, that should be launched when ""Browse Server"" button is pressed.
If configured, the ""Browse Server"" button will appear in Link, Image and Flash dialogs.")]
        [DefaultValue("")]
        public string FilebrowserBrowseUrl { get { return config.filebrowserBrowseUrl; } set { config.filebrowserBrowseUrl = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"The location of an external file browser, that should be launched when ""Browse Server"" button is pressed in the Flash dialog.
If not set, CKEditor will use CKEDITOR.config.filebrowserBrowseUrl.")]
        [DefaultValue("")]
        public string FilebrowserFlashBrowseUrl { get { return config.filebrowserFlashBrowseUrl; } set { config.filebrowserFlashBrowseUrl = value; } }

        [Category("CKEditor Basic Settings")]
        [Description("The location of a script that handles file uploads in the Flash dialog. If not set, CKEditor will use CKEDITOR.config.filebrowserUploadUrl.")]
        [DefaultValue("")]
        public string FilebrowserFlashUploadUrl { get { return config.filebrowserFlashUploadUrl; } set { config.filebrowserFlashUploadUrl = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"The location of an external file browser, that should be launched when ""Browse Server"" button is pressed in the Link tab of Image dialog.
If not set, CKEditor will use CKEDITOR.config.filebrowserBrowseUrl.")]
        [DefaultValue("")]
        public string FilebrowserImageBrowseLinkUrl { get { return config.filebrowserImageBrowseLinkUrl; } set { config.filebrowserImageBrowseLinkUrl = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"The location of an external file browser, that should be launched when ""Browse Server"" button is pressed in the Image dialog.
If not set, CKEditor will use CKEDITOR.config.filebrowserBrowseUrl.")]
        [DefaultValue("")]
        public string FilebrowserImageBrowseUrl { get { return config.filebrowserImageBrowseUrl; } set { config.filebrowserImageBrowseUrl = value; } }

        [Category("CKEditor Basic Settings")]
        [Description("The location of a script that handles file uploads in the Image dialog. If not set, CKEditor will use CKEDITOR.config.filebrowserUploadUrl.")]
        [DefaultValue("")]
        public string FilebrowserImageUploadUrl { get { return config.filebrowserImageUploadUrl; } set { config.filebrowserImageUploadUrl = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"The location of a script that handles file uploads. If set, the ""Upload"" tab will appear in ""Link"", ""Image"" and ""Flash"" dialogs.")]
        [DefaultValue("")]
        public string FilebrowserUploadUrl { get { return config.filebrowserUploadUrl; } set { config.filebrowserUploadUrl = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"The ""features"" to use in the file browser popup window.")]
        [DefaultValue("location=no,menubar=no,toolbar=no,dependent=yes,minimizable=no,modal=yes,alwaysRaised=yes,resizable=yes,scrollbars=yes")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FilebrowserWindowFeatures { get { return config.filebrowserWindowFeatures; } set { config.filebrowserWindowFeatures = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Whether a filler text (non-breaking space entity -  ) will be inserted into empty block elements in HTML output, 
this is used to render block elements properly with line-height; When a function is instead specified, 
it'll be passed a CKEDITOR.htmlParser.element to decide whether adding the filler text by expecting a boolean return value.")]
        [DefaultValue(true)]
        public bool FillEmptyBlocks { get { return config.fillEmptyBlocks; } set { config.fillEmptyBlocks = value; } }

        [Category("CKEditor Other Settings")]
        [Description("The text to be displayed in the Font combo is none of the available values matches the current cursor position or text selection.")]
        [DefaultValue("")]
        public string FontDefaultLabel { get { return config.font_defaultLabel; } set { config.font_defaultLabel = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"The list of fonts names to be displayed in the Font combo in the toolbar. Entries are separated by semi-colons (;),
while it's possible to have more than one font for each entry, in the HTML way (separated by comma).
A display name may be optionally defined by prefixing the entries with the name and the slash character.")]
        [DefaultValue(@"Arial/Arial, Helvetica, sans-serif;
Comic Sans MS/Comic Sans MS, cursive;
Courier New/Courier New, Courier, monospace;
Georgia/Georgia, serif;
Lucida Sans Unicode/Lucida Sans Unicode, Lucida Grande, sans-serif;
Tahoma/Tahoma, Geneva, sans-serif;
Times New Roman/Times New Roman, Times, serif;
Trebuchet MS/Trebuchet MS, Helvetica, sans-serif;
Verdana/Verdana, Geneva, sans-serif")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FontNames { get { return config.font_names; } set { config.font_names = value; } }

        [Category("CKEditor Other Settings")]
        [Description("The text to be displayed in the Font Size combo is none of the available values matches the current cursor position or text selection.")]
        [DefaultValue("")]
        public string FontSizeDefaultLabel { get { return config.fontSize_defaultLabel; } set { config.fontSize_defaultLabel = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"The list of fonts size to be displayed in the Font Size combo in the toolbar. Entries are separated by semi-colons (;).
Any kind of ""CSS like"" size can be used, like ""12px"", ""2.3em"", ""130%"", ""larger"" or ""x-small"".
A display name may be optionally defined by prefixing the entries with the name and the slash character.")]
        [DefaultValue("8/8px;9/9px;10/10px;11/11px;12/12px;14/14px;16/16px;18/18px;20/20px;22/22px;24/24px;26/26px;28/28px;36/36px;48/48px;72/72px")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FontSizeSizes { get { return config.fontSize_sizes; } set { config.fontSize_sizes = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Force the respect of CKEDITOR.config.enterMode as line break regardless of the context, 
E.g. If CKEDITOR.config.enterMode is set to CKEDITOR.ENTER_P, press enter key inside a 'div' will create a new paragraph with 'p' instead of 'div'.")]
        [DefaultValue(false)]
        public bool ForceEnterMode { get { return config.forceEnterMode; } set { config.forceEnterMode = value; } }

        [Category("CKEditor Basic Settings")]
        [Description("Whether to force all pasting operations to insert on plain text into the editor, loosing any formatting information possibly available in the source text.")]
        [DefaultValue(false)]
        public bool ForcePasteAsPlainText { get { return config.forcePasteAsPlainText; } set { config.forcePasteAsPlainText = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@" Whether to force using ""&"" instead of ""&amp;"" in elements attributes values,
it's not recommended to change this setting for compliance with the W3C XHTML 1.0 standards (C.12, XHTML 1.0).")]
        [DefaultValue(false)]
        public bool ForceSimpleAmpersand { get { return config.forceSimpleAmpersand; } set { config.forceSimpleAmpersand = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@" A list of semi colon separated style names (by default tags) representing the style definition for each entry to be displayed
in the Format combo in the toolbar. Each entry must have its relative definition configuration in a setting named
""format_(tagName)"". For example, the ""p"" entry has its definition taken from config.format_p.
")]
        [DefaultValue("p;h1;h2;h3;h4;h5;h6;pre;address;div")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FormatTags { get { return config.format_tags; } set { config.format_tags = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"Indicates whether the contents to be edited are being inputted as a full HTML page. 
A full page includes the <html>, <head> and <body> tags. 
The final output will also reflect this setting, including the <body> contents only if this setting is disabled.")]
        [DefaultValue(false)]
        public bool FullPage { get { return config.fullPage; } set { config.fullPage = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether escape HTML when editor update original input element.")]
        [DefaultValue(true)]
        public bool HtmlEncodeOutput { get { return config.htmlEncodeOutput; } set { config.htmlEncodeOutput = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Whether the editor must output an empty value ("""") if it's contents is made by an empty paragraph only.")]
        [DefaultValue(true)]
        public bool IgnoreEmptyParagraph { get { return config.ignoreEmptyParagraph; } set { config.ignoreEmptyParagraph = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Padding text to set off the image in preview area.")]
        [DefaultValue("")]
        public string ImagePreviewText { get { return config.image_previewText; } set { config.image_previewText = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether to remove links when emptying the link URL field in the image dialog.")]
        [DefaultValue(true)]
        public bool ImageRemoveLinkByEmptyURL { get { return config.image_removeLinkByEmptyURL; } set { config.image_removeLinkByEmptyURL = value; } }

        [TypeConverter(typeof(StringArrayConverter))]
        [PersistenceMode(PersistenceMode.Attribute)]
        [Category("CKEditor Other Settings")]
        [Description("List of classes to use for indenting the contents. If it's null, no classes will be used and instead the #indentUnit and #indentOffset properties will be used.")]
        [DefaultValue(typeof(Array), null)]
        public string[] IndentClasses { get { return config.indentClasses; } set { config.indentClasses = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Size of each indentation step")]
        [DefaultValue(40)]
        public int IndentOffset { get { return config.indentOffset; } set { config.indentOffset = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Unit for the indentation style")]
        [DefaultValue("px")]
        public string IndentUnit { get { return config.indentUnit; } set { config.indentUnit = value; } }

        [Category("CKEditor Basic Settings")]
        [Description(@"The user interface language localization to use. If empty, the editor automatically localize the editor to the user language,
if supported, otherwise the CKEDITOR.config.defaultLanguage language is used.")]
        [DefaultValue("")]
        public string Language { get { return config.language; } set { config.language = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether to transform MS Word outline numbered headings into lists.")]
        [DefaultValue(false)]
        public bool PasteFromWordNumberedHeadingToList { get { return config.pasteFromWordNumberedHeadingToList; } set { config.pasteFromWordNumberedHeadingToList = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether to prompt the user about the clean up of content being pasted from MS Word.")]
        [DefaultValue(false)]
        public bool PasteFromWordPromptCleanup { get { return config.pasteFromWordPromptCleanup; } set { config.pasteFromWordPromptCleanup = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Whether to ignore all font related formatting styles, including: 
font size;
font family;
font foreground/background color.")]
        [DefaultValue(true)]
        public bool PasteFromWordRemoveFontStyles { get { return config.pasteFromWordRemoveFontStyles; } set { config.pasteFromWordRemoveFontStyles = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Whether to remove element styles that can't be managed with the editor. Note that this doesn't handle the font specific styles,
which depends on the CKEDITOR.config.pasteFromWordRemoveFontStyles setting instead.")]
        [DefaultValue(false)]
        public bool PasteFromWordRemoveStyles { get { return config.pasteFromWordRemoveStyles; } set { config.pasteFromWordRemoveStyles = value; } }

        [TypeConverter(typeof(StringArrayConverter))]
        [PersistenceMode(PersistenceMode.Attribute)]
        [Category("CKEditor Other Settings")]
        [Description("List of regular expressions to be executed over the input HTML, indicating HTML source code that matched must not present in WYSIWYG mode for editing.")]
        [DefaultValue(typeof(Array), null)]
        public string[] ProtectedSource { get { return config.protectedSource; } set { config.protectedSource = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"The dialog contents to removed. It's a string composed by dialog name and tab name with a colon between them. 
Separate each pair with semicolon (see example). Note: All names are case-sensitive. 
Note: Be cautious when specifying dialog tabs that are mandatory, like ""info"", 
dialog functionality might be broken because of this!")]
        [DefaultValue("")]
        public string RemoveDialogTabs { get { return config.removeDialogTabs; } set { config.removeDialogTabs = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"List of plugins that must not be loaded. This is a tool setting which makes it easier to avoid loading plugins definied 
in the CKEDITOR.config.plugins setting, whithout having to touch it and potentially breaking it.")]
        [DefaultValue("")]
        public string RemovePlugins { get { return config.removePlugins; } set { config.removePlugins = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"The directions to which the editor resizing is enabled. Possible values are ""both"", ""vertical"" and ""horizontal"".")]
        [DefaultValue(typeof(ResizeDir), "Both")]
        public ResizeDir ResizeDir { get { return config.resize_dir; } set { config.resize_dir = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether to enable the resizing feature. If disabled the resize handler will not be visible.")]
        [DefaultValue(true)]
        public bool ResizeEnabled { get { return config.resize_enabled; } set { config.resize_enabled = value; } }

        [Category("CKEditor Other Settings")]
        [Description("The maximum editor height, in pixels, when resizing it with the resize handle.")]
        [DefaultValue(3000)]
        public int ResizeMaxHeight { get { return config.resize_maxHeight; } set { config.resize_maxHeight = value; } }

        [Category("CKEditor Other Settings")]
        [Description("The maximum editor width, in pixels, when resizing it with the resize handle.")]
        [DefaultValue(3000)]
        public int ResizeMaxWidth { get { return config.resize_maxWidth; } set { config.resize_maxWidth = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"The minimum editor height, in pixels, when resizing it with the resize handle. 
Note: It fallbacks to editor's actual height if that's smaller than the default value.")]
        [DefaultValue(250)]
        public int ResizeMinHeight { get { return config.resize_minHeight; } set { config.resize_minHeight = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"The minimum editor width, in pixels, when resizing it with the resize handle. 
Note: It fallbacks to editor's actual width if that's smaller than the default value.")]
        [DefaultValue(750)]
        public int ResizeMinWidth { get { return config.resize_minWidth; } set { config.resize_minWidth = value; } }

        [Category("CKEditor Other Settings")]
        [Description("If enabled (true), turns on SCAYT automatically after loading the editor.")]
        [DefaultValue(false)]
        public bool ScaytAutoStartup { get { return config.scayt_autoStartup; } set { config.scayt_autoStartup = value; } }

        [Category("CKEditor Other Settings")]
        [Description("ID of bottom cntrol's shared")]
        [DefaultValue("")]
        public string SharedSpacesBottom { get { return config.sharedSpacesBottom; } set { config.sharedSpacesBottom = value; } }

        [Category("CKEditor Other Settings")]
        [Description("ID of top cntrol's shared")]
        [DefaultValue("")]
        public string SharedSpacesTop { get { return config.sharedSpacesTop; } set { config.sharedSpacesTop = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Just like the CKEDITOR.config.enterMode setting, it defines the behavior for the SHIFT+ENTER key.
The allowed values are the following constants, and their relative behavior: 
CKEDITOR.ENTER_P (1): new <p> paragraphs are created;
CKEDITOR.ENTER_BR (2): lines are broken with <br> elements;
CKEDITOR.ENTER_DIV (3): new <div> blocks are created.")]
        [DefaultValue(typeof(EnterMode), "P")]
        public EnterMode ShiftEnterMode { get { return config.shiftEnterMode; } set { config.shiftEnterMode = value; } }

        [Category("CKEditor Basic Settings")]
        [Description("The skin to load. It may be the name of the skin folder inside the editor installation path, or the name and the path separated by a comma.")]
        [DefaultValue("kama")]
        public string Skin { get { return config.skin; } set { config.skin = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"The mode to load at the editor startup. It depends on the plugins loaded. By default, the ""wysiwyg"" and ""source"" modes are available.")]
        [DefaultValue(typeof(StartupMode), "Wysiwyg")]
        public StartupMode StartupMode { get { return config.startupMode; } set { config.startupMode = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Whether to automaticaly enable the ""show block"" command when the editor loads. (StartupShowBlocks in FCKeditor)")]
        [DefaultValue(false)]
        public bool StartupOutlineBlocks { get { return config.startupOutlineBlocks; } set { config.startupOutlineBlocks = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Whether to automatically enable the ""show borders"" command when the editor loads. (ShowBorders in FCKeditor)")]
        [DefaultValue(true)]
        public bool StartupShowBorders { get { return config.startupShowBorders; } set { config.startupShowBorders = value; } }

        [PersistenceMode(PersistenceMode.Attribute)]
        [Category("CKEditor Other Settings")]
        [Description(@"The ""styles definition set"" to use in the editor. They will be used in the styles combo and the Style selector of the div container.
The styles may be defined in the page containing the editor, or can be loaded on demand from an external file. In the second case, 
if this setting contains only a name, the styles definition file will be loaded from the ""styles"" folder inside the styles plugin folder.
Otherwise, this setting has the ""name:url"" syntax, making it possible to set the URL from which loading the styles file.
Previously this setting was available as config.stylesCombo_stylesSet")]
        [DefaultValue("default")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string StylesSet
        {
            get
            {
                string retVal = string.Empty;
                foreach (string item in config.stylesSet) retVal += item + ",";
                if (retVal.EndsWith(",")) retVal = retVal.Remove(retVal.Length - 1);
                return retVal;
            }
            set
            {
                if (value.Contains("{") && value.Contains("}"))
                    config.stylesSet = new string[] { value };
                else
                    config.stylesSet = value.Replace("\t", string.Empty).Split(new char[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [Category("CKEditor Other Settings")]
        [Description(@"Intructs the editor to add a number of spaces (&nbsp;) to the text when hitting the TAB key.
If set to zero, the TAB key will be used to move the cursor focus to the next element in the page, out of the editor focus.")]
        [DefaultValue(0)]
        public int TabSpaces { get { return config.tabSpaces; } set { config.tabSpaces = value; } }

        [Category( "CKEditor Other Settings" )]
        [Description( "The templates definition set to use. It accepts a list of names separated by comma. It must match definitions loaded with the templates_files setting." )]
        [DefaultValue( "default" )]
        public string Templates { get { return config.templates; } set { config.templates = value; } }

        [PersistenceMode(PersistenceMode.Attribute)]
        [Category("CKEditor Other Settings")]
        [Description("The list of templates definition files to load.")]
        [DefaultValue("~/Scripts/ckeditor/plugins/templates/templates/default.js")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TemplatesFiles
        {
            get
            {
                string retVal = string.Empty;
                foreach (string item in config.templates_files) retVal += item + ",";
                if (retVal.EndsWith(",")) retVal = retVal.Remove(retVal.Length - 1);
                return retVal;
            }
            set { config.templates_files = value.Replace("\t", string.Empty).Split(new char[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries); }
        }

        [Category("CKEditor Other Settings")]
        [Description(@"Whether the ""Replace actual contents"" checkbox is checked by default in the Templates dialog.")]
        [DefaultValue(true)]
        public bool TemplatesReplaceContent { get { return config.templates_replaceContent; } set { config.templates_replaceContent = value; } }

        [PersistenceMode(PersistenceMode.Attribute)]
        [Category("CKEditor Basic Settings")]
        [Description("The toolbox (alias toolbar) definition. It is a toolbar name or an array of toolbars (strips), each one being also an array, containing a list of UI items.")]
        [DefaultValue("Full")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Toolbar
        {
            get
            {
                if (config.toolbar is string) return (string)config.toolbar;
                string retVal = string.Empty;
                try
                {
                    string[] retValTab = new string[((object[])config.toolbar).Length];
                    object[] ob = (object[])config.toolbar;
                    for (int i = 0; i < ob.Length; i++)
                    {
                        object item = ob[i];
                        if (item is string) retValTab[i] = (string)item;
                        else
                        {
                            object[] itemTab = (object[])item;
                            string concatValue = string.Empty;
                            for (int j = 0; j < itemTab.Length; j++)
                                concatValue += (string)itemTab[j] + "|";
                            if (!string.IsNullOrEmpty(concatValue))
                                concatValue = concatValue.Remove(concatValue.Length - 1);
                            retValTab[i] = concatValue;
                        }
                    }
                    foreach (string item in retValTab) retVal += item + "\r\n";
                    if (retVal.EndsWith("\r\n")) retVal = retVal.Remove(retVal.Length - 2);
                }
                catch { }
                return retVal;
            }
            set
            {
                value = value.Trim();
                if (value.StartsWith("[") && value.EndsWith("]"))
                {
                    config.toolbar = value.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
                    return;
                }
                string[] valueTab = value.Split(new string[] { "\r\n" }, int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
                if (valueTab.Length == 1 && !valueTab[0].Contains("|"))
                {
                    config.toolbar = valueTab[0];
                    return;
                }
                object[] retVal = new object[valueTab.Length];
                try
                {
                    for (int i = 0; i < valueTab.Length; i++)
                    {
                        string[] item = valueTab[i].Split(new char[] { '|' });
                        for (int k = 0; k < item.Length; k++)
                            item[k] = item[k].Trim();
                        if (item.Length == 1 && item[0] == "/") retVal[i] = item[0];
                        else retVal[i] = item;
                    }
                }
                catch { }
                config.toolbar = retVal;
            }
        }

        [PersistenceMode(PersistenceMode.Attribute)]
        [Category("CKEditor Basic Settings")]
        [Description(@"The toolbar definition. It is an array of toolbars (strips), each one being also an array, containing a list of UI items.
Note that this setting is composed by ""toolbar_"" added by the toolbar name, which in this case is called ""Basic"".
This second part of the setting name can be anything. You must use this name in the CKEDITOR.config.toolbar setting,
so you instruct the editor which toolbar_(name) setting to you.")]
        [DefaultValue("Bold|Italic|-|NumberedList|BulletedList|-|Link|Unlink|-|About")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ToolbarBasic
        {
            get
            {
                string retVal = string.Empty;
                try
                {
                    string[] retValTab = new string[config.toolbar_Basic.Length];
                    object[] ob = config.toolbar_Basic;
                    for (int i = 0; i < ob.Length; i++)
                    {
                        object item = ob[i];
                        if (item is string) retValTab[i] = (string)item;
                        else
                        {
                            object[] itemTab = (object[])item;
                            string concatValue = string.Empty;
                            for (int j = 0; j < itemTab.Length; j++)
                                concatValue += (string)itemTab[j] + "|";
                            if (!string.IsNullOrEmpty(concatValue))
                                concatValue = concatValue.Remove(concatValue.Length - 1);
                            retValTab[i] = concatValue;
                        }
                    }
                    foreach (string item in retValTab) retVal += item + "\r\n";
                    if (retVal.EndsWith("\r\n")) retVal = retVal.Remove(retVal.Length - 2);
                }
                catch { }
                return retVal;
            }
            set
            {
                value = value.Trim();
                if (value.StartsWith("[") && value.EndsWith("]"))
                {
                    config.toolbar_Basic = new object[] { value.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty) };
                    return;
                }
                string[] valueTab = value.Split(new string[] { "\r\n" }, int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
                object[] retVal = new object[valueTab.Length];
                try
                {
                    for (int i = 0; i < valueTab.Length; i++)
                    {
                        string[] item = valueTab[i].Split(new char[] { '|' });
                        for (int k = 0; k < item.Length; k++)
                            item[k] = item[k].Trim();
                        if (item.Length == 1 && item[0] == "/") retVal[i] = item[0];
                        else retVal[i] = item;
                    }
                }
                catch { }
                config.toolbar_Basic = retVal;
            }
        }

        [PersistenceMode(PersistenceMode.Attribute)]
        [Category("CKEditor Basic Settings")]
        [Description("This is the default toolbar definition used by the editor. It contains all editor features.")]
        [DefaultValue(@"Source|-|Save|NewPage|Preview|-|Templates
Cut|Copy|Paste|PasteText|PasteFromWord|-|Print|SpellChecker|Scayt
Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat
Form|Checkbox|Radio|TextField|Textarea|Select|Button|ImageButton|HiddenField
/
Bold|Italic|Underline|Strike|-|Subscript|Superscript
NumberedList|BulletedList|-|Outdent|Indent|Blockquote|CreateDiv
JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock
BidiLtr|BidiRtl
Link|Unlink|Anchor
Image|Flash|Table|HorizontalRule|Smiley|SpecialChar|PageBreak|Iframe
/
Styles|Format|Font|FontSize
TextColor|BGColor
Maximize|ShowBlocks|-|About")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ToolbarFull
        {
            get
            {
                string retVal = string.Empty;
                try
                {
                    string[] retValTab = new string[config.toolbar_Full.Length];
                    object[] ob = config.toolbar_Full;
                    for (int i = 0; i < ob.Length; i++)
                    {
                        object item = ob[i];
                        if (item is string) retValTab[i] = (string)item;
                        else
                        {
                            object[] itemTab = (object[])item;
                            string concatValue = string.Empty;
                            for (int j = 0; j < itemTab.Length; j++)
                                concatValue += (string)itemTab[j] + "|";
                            if (!string.IsNullOrEmpty(concatValue))
                                concatValue = concatValue.Remove(concatValue.Length - 1);
                            retValTab[i] = concatValue;
                        }
                    }
                    foreach (string item in retValTab) retVal += item + "\r\n";
                    if (retVal.EndsWith("\r\n")) retVal = retVal.Remove(retVal.Length - 2);
                }
                catch { }
                return retVal;
            }
            set
            {
                value = value.Trim();
                if (value.StartsWith("[") && value.EndsWith("]"))
                {
                    config.toolbar_Full = new object[] { value.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty) };
                    return;
                }
                string[] valueTab = value.Split(new string[] { "\r\n" }, int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
                object[] retVal = new object[valueTab.Length];
                try
                {
                    for (int i = 0; i < valueTab.Length; i++)
                    {
                        string[] item = valueTab[i].Split(new char[] { '|' });
                        for (int k = 0; k < item.Length; k++)
                            item[k] = item[k].Trim();
                        if (item.Length == 1 && item[0] == "/") retVal[i] = item[0];
                        else retVal[i] = item;
                    }
                }
                catch { }
                config.toolbar_Full = retVal;
            }
        }

        [Category("CKEditor Other Settings")]
        [Description("This is the default toolbar definition used by the editor. It contains all editor features.")]
        [DefaultValue(true)]
        public bool ToolbarCanCollapse { get { return config.toolbarCanCollapse; } set { config.toolbarCanCollapse = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"The ""theme space"" to which rendering the toolbar. For the default theme, the recommended options are ""top"" and ""bottom"".")]
        [DefaultValue(typeof(ToolbarLocation), "Top")]
        public ToolbarLocation ToolbarLocation { get { return config.toolbarLocation; } set { config.toolbarLocation = value; } }

        [Category("CKEditor Other Settings")]
        [Description("Whether the toolbar must start expanded when the editor is loaded.")]
        [DefaultValue(true)]
        public bool ToolbarStartupExpanded { get { return config.toolbarStartupExpanded; } set { config.toolbarStartupExpanded = value; } }

        [Category("CKEditor Other Settings")]
        [Description(@"Indicates that some of the editor features, like alignment and text direction, should used the ""computed value""
of the featureto indicate it's on/off state, instead of using the ""real value"".
If enabled, in a left to right written document, the ""Left Justify"" alignment button will show as active,
even if the aligment style is not explicitly applied to the current paragraph in the editor.")]
        [DefaultValue(true)]
        public bool UseComputedState { get { return config.useComputedState; } set { config.useComputedState = value; } }

        [TypeConverter(typeof(WebColorConverter))]
        [Category("CKEditor Basic Settings")]
        [Description("Specifies the color of the user interface. Works only with the Kama skin.")]
        [DefaultValue(typeof(System.Drawing.Color), "LightGray")]
        public System.Drawing.Color UIColor
        {
            get
            {
                if (config.uiColor == "#D3D3D3") return System.Drawing.Color.FromName("LightGray");
                return System.Drawing.ColorTranslator.FromHtml(config.uiColor);
            }
            set { config.uiColor = "#" + value.R.ToString(@"x2") + value.G.ToString(@"x2") + value.B.ToString(@"x2"); }
        }

        [Category("CKEditor Other Settings")]
        [Description("To attach a function to CKEditor events.")]
        [DefaultValue(null)]
        public List<object> CKEditorEventHandler { get { return config.CKEditorEventHandler; } set { config.CKEditorEventHandler = value; } }

        [Category("CKEditor Other Settings")]
        [Description("To attach a function to an event in a single editor instance")]
        [DefaultValue(null)]
        public List<object> CKEditorInstanceEventHandler { get { return config.CKEditorInstanceEventHandler; } set { config.CKEditorInstanceEventHandler = value; } }

        #endregion

        #endregion

        #region Microsoft Ajax compatibility

        private static bool? _HasMsAjax = null;
        private static Type typScriptManager;
        private static Type updatePanel;
        private static System.Reflection.MethodInfo mtdRegisterClientScriptInclude;
        private static System.Reflection.MethodInfo mtdRegisterStartupScript;
        private static System.Reflection.MethodInfo mtdRegisterOnSubmitStatement;

        private void RegisterClientScriptInclude(Type type, string key, string url)
        {
            if (HasMsAjax)
            {
                if (mtdRegisterClientScriptInclude == null)
                    mtdRegisterClientScriptInclude = typScriptManager.GetMethod("RegisterClientScriptInclude", new Type[4] { typeof(Control), typeof(Type), typeof(string), typeof(string) });
                mtdRegisterClientScriptInclude.Invoke(null, new object[4] { this, type, key, url });
            }
            else
                Page.ClientScript.RegisterClientScriptInclude(type, key, url);
        }

        private void RegisterStartupScript(Type type, string key, string script, bool addScriptTags)
        {
            if (HasMsAjax)
            {
                if (mtdRegisterStartupScript == null)
                    mtdRegisterStartupScript = typScriptManager.GetMethod("RegisterStartupScript", new Type[5] { typeof(Control), typeof(Type), typeof(string), typeof(string), typeof(bool) });
                mtdRegisterStartupScript.Invoke(null, new object[5] { this, type, key, script, addScriptTags });
            }
            else
                Page.ClientScript.RegisterStartupScript(type, key, script, addScriptTags);
        }

        private void RegisterOnSubmitStatement(Type type, string key, string script)
        {
            if (HasMsAjax)
            {
                if (mtdRegisterOnSubmitStatement == null)
                    mtdRegisterOnSubmitStatement = typScriptManager.GetMethod("RegisterOnSubmitStatement", new Type[4] { typeof(Control), typeof(Type), typeof(string), typeof(string) });
                mtdRegisterOnSubmitStatement.Invoke(null, new object[4] { this, type, key, script });
            }
            else
                Page.ClientScript.RegisterOnSubmitStatement(type, key, script);
        }

        private bool HasMsAjax
        {
            get
            {
                if (_HasMsAjax == null)
                {
                    _HasMsAjax = false;
                    System.Reflection.Assembly[] AppAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (System.Reflection.Assembly asm in AppAssemblies)
                    {
                        if (asm.ManifestModule.Name == "System.Web.Extensions.dll")
                        {
                            try
                            {
                                updatePanel = asm.GetType("System.Web.UI.UpdatePanel");
                                typScriptManager = asm.GetType("System.Web.UI.ScriptManager");
                                if (typScriptManager != null) _HasMsAjax = true;
                            }
                            catch { }
                            break;
                        }
                    }
                }

                return _HasMsAjax ?? false;
            }
        }

        #endregion

        #region Default Constructor

        public CKEditorControl()
        {
            base.TextMode = TextBoxMode.MultiLine;
        }

        public override void Focus()
        {
            base.Focus();
            this.config.startupFocus = true;
        }
        #endregion

        #region Override Method
        private string timestamp = "?t=C6HH5UF";
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.RegisterStartupScript(this.GetType(), "CKEDITOR_BASEPATH", string.Format("window.CKEDITOR_BASEPATH = '{0}/';\n", (this.CKEditorJSFile.StartsWith("~") ? this.ResolveUrl(this.BasePath) : this.BasePath)), true);
            this.RegisterStartupScript(this.GetType(), "ckeditor", "<script src=\"" + (this.CKEditorJSFile.StartsWith("~") ? this.ResolveUrl(this.CKEditorJSFile) : this.CKEditorJSFile) + timestamp + "\" type=\"text/javascript\"></script>", false);
            string scriptInit = string.Empty;
            string doPostBackScript = string.Empty;
            if (this.AutoPostBack) doPostBackScript = string.Format(@"CKEDITOR.instances['{0}'].on('blur', function() {{if(this.checkDirty()){{javascript:setTimeout('__doPostBack(\'{0}\',\'\')',0); }}}});", this.ClientID);
            
            // Sys.Application.add_load does not work on browsers != IE
            // http://msdn.microsoft.com/en-us/library/bb386417.aspx
            // Check _dev/msajax.js for an uncompressed version (available in CKEditor.Net downloaded from SVN).
            scriptInit += @"var CKEditor_Controls=[],CKEditor_Init=[];function CKEditor_TextBoxEncode(d,e){var f;if(typeof CKEDITOR=='undefined'||typeof CKEDITOR.instances[d]=='undefined'){f=document.getElementById(d);if(f)f.value=f.value.replace(/</g,'&lt;').replace(/>/g,'&gt;');}else{var g=CKEDITOR.instances[d];if(e&&(typeof Page_BlockSubmit=='undefined'||!Page_BlockSubmit)){g.destroy();f=document.getElementById(d);if(f)f.style.visibility='hidden';}else g.updateElement();}};(function(){if(typeof CKEDITOR!='undefined'){var d=document.getElementById('#this.ClientID#');if(d)d.style.visibility='hidden';}var e=function(){var f=CKEditor_Controls,g=CKEditor_Init,h=window.pageLoad,i=function(){for(var j=f.length;j--;){var k=document.getElementById(f[j]);if(k&&k.value&&(k.value.indexOf('<')==-1||k.value.indexOf('>')==-1))k.value=k.value.replace(/&lt;/g,'<').replace(/&gt;/g,'>').replace(/&amp;/g,'&');}if(typeof CKEDITOR!='undefined')for(var j=0;j<g.length;j++)g[j].call(this);};window.pageLoad=function(j,k){if(k.get_isPartialLoad())setTimeout(i,0);if(h&&typeof h=='function')h.call(this,j,k);};if(typeof Page_ClientValidate=='function'&&typeof CKEDITOR!='undefined')Page_ClientValidate=CKEDITOR.tools.override(Page_ClientValidate,function(j){return function(){for(var k in CKEDITOR.instances){if(document.getElementById(k))CKEDITOR.instances[k].updateElement();}return j.apply(this,arguments);};});setTimeout(i,0);};if(typeof Sys!='undefined'&&typeof Sys.Application!='undefined')Sys.Application.add_load(e);if(window.addEventListener)window.addEventListener('load',e,false);else if(window.attachEvent)window.attachEvent('onload',e);})();";
            scriptInit = scriptInit.Replace("#this.ClientID#", this.ClientID);
            this.RegisterStartupScript(this.GetType(), "CKEditorForNet", scriptInit, true);
            this.RegisterStartupScript(this.GetType(), this.ClientID + @"_addControl", string.Format(@"CKEditor_Controls.push('{0}');
", this.ClientID), true);
            string script = string.Empty;
            if (this.config.CKEditorEventHandler != null)
                foreach (object[] item in this.config.CKEditorEventHandler)
                {
                    script += string.Format(@"CKEditor_Init.push(function(){{CKEDITOR.on('{0}',{1});}});
", item[0], item[1]);
                }
            if (this.config.protectedSource != null && this.config.protectedSource.Length > 0)
            {
                string proSour = string.Empty;
                foreach (string item in this.config.protectedSource)
                    proSour += @"
ckeditor.config.protectedSource.push( " + item + " );";
                script += string.Format(@"CKEditor_Init.push(function(){{if(typeof CKEDITOR.instances['{0}']!='undefined' || !document.getElementById('{0}')) return;var ckeditor = CKEDITOR.replace('{0}',{1}); {2} {3}}});
", this.ClientID, prepareJSON(), proSour, doPostBackScript);
            }
            else
                script += string.Format(@"CKEditor_Init.push(function(){{if(typeof CKEDITOR.instances['{0}']!='undefined' || !document.getElementById('{0}')) return;CKEDITOR.replace('{0}',{1}); {2}}});
", this.ClientID, prepareJSON(), doPostBackScript);

            bool isInUpdatePanel = false;            
            Control con = this.Parent;
            if (updatePanel != null)
                while (con != null)
                {
                    if (con.GetType() == updatePanel)
                    {
                        isInUpdatePanel = true;
                        break;
                    }
                    con = con.Parent;
                }
            this.RegisterOnSubmitStatement(this.GetType(), "aspintegrator_Postback" + this.ClientID, string.Format("CKEditor_TextBoxEncode('{0}', {1}); ", this.ClientID, isInUpdatePanel ? 1 : 0));
            this.RegisterStartupScript(this.GetType(), "aspintegratorInitial_" + this.ClientID, script, true);
            if (isChanged) OnTextChanged(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ViewState["CKEditorConfig"] != null)
                this.config = (CKEditorConfig)ViewState["CKEditorConfig"];
            else
                this.config = new CKEditorConfig(this.BasePath.StartsWith("~") ? this.ResolveUrl(this.BasePath) : this.BasePath);
        }

        #endregion

        #region Private Method

        private string prepareJSON()
        {
            return JSONSerializer.ToJavaScriptObjectNotation(this.config);
        }

        #endregion

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (this.config.htmlEncodeOutput)
            {
                string postedValue = HttpUtility.HtmlDecode(postCollection[postDataKey]);
                if (this.Text != postedValue)
                {
                    isChanged = true;
                    this.Text = postedValue;
                    return true;
                }
            }
            return false;
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent() { }

        #endregion

        public void InitNewsReleaseEditor(string content = "")
        {
            List<object[]> buttons = new List<object[]>();

            buttons.Add(new object[] { "Source" });

            buttons.Add(new object[] { "Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Undo", "Redo" });
            buttons.Add(new object[] { "Bold", "Italic", "-", "RemoveFormat" });
            buttons.Add(new object[] { "NumberedList", "BulletedList" }); //, "-", "Outdent", "Indent"
            buttons.Add(new object[] { "Link", "Unlink", "Anchor", "asset" });

            buttons.Add(new object[] { "Maximize", "ShowBlocks" });

            //TODO:
            //new object[] { "Underline", "Strike", "Outdent", "Indent", "Blockquote", "JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock"},
            //new object[] { "Image", "Iframe", "oembed"},
            //new object[] { "Maximize" }

            config.toolbar = buttons.ToArray();

            config.extraPlugins = "asset,pastefromword";

            config.removePlugins = "liststyle";
            config.removeDialogTabs = "link:target;link:advanced";

            config.contentsCss = new string[] { ResolveUrl("~/News/Content/CKEditor.css") };

            config.htmlEncodeOutput = true;

            config.skin = "kama";

            Text = content;
        }
    }

    public class CKEditorControlDesigner : System.Web.UI.Design.ControlDesigner
    {
        public CKEditorControlDesigner()
        {
        }

        public override string GetDesignTimeHtml()
        {
            CKEditorControl control = (CKEditorControl)Component;
            return String.Format(CultureInfo.InvariantCulture,
                "<table width=\"{0}\" height=\"{1}\" bgcolor=\"#f5f5f5\" bordercolor=\"#c7c7c7\" cellpadding=\"0\" cellspacing=\"0\" border=\"1\"><tr><td valign=\"middle\" align=\"center\"><h3>CKEditor ASP.NET Control - <em>'{2}'</em></h3></td></tr></table>",
                control.Width.Value == 0 ? "100%" : control.Width.ToString(),
                control.Height,
                control.ID);
        }
    }
}