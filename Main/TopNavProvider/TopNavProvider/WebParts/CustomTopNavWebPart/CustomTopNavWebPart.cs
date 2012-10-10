using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

namespace DeviQ.SharePoint.Utilities.Navigation
{
	[ToolboxItemAttribute(false)]
	public class CustomTopNavWebPart : WebPart
	{
		public string _MenuList = "CustomTopNavProvider";
		[Personalizable, WebBrowsable, WebDisplayName("Menu List")]
		public string Set_MenuList
		{
			get
			{
				return _MenuList;
			}
			set
			{
				_MenuList = value;
			}
		}

		public string _ServerURL = "";
		readonly char[] fwdSlash = { '/' };
		[Personalizable, WebBrowsable, WebDisplayName("Server URL")]
		public string Set_ServerURL
		{
			get
			{
				if (_ServerURL.Trim().EndsWith("/"))
					return _ServerURL.Trim().TrimEnd(fwdSlash);
				else
					return _ServerURL;
			}
			set
			{
				_ServerURL = value;
			}
		}

		public string _WebSite = "";
		[Personalizable, WebBrowsable, WebDisplayName("Web Site")]
		public string Set_WebSite
		{
			get
			{
				return _WebSite;
			}
			set
			{
				_WebSite = value;
			}
		}
		public string _MenuContainer = "ms-topnavContainer";
		[Personalizable, WebBrowsable, WebDisplayName("Menu Container CSS (ms-topnavContainer)")]
		public string Set_MenuContainer
		{
			get
			{
				return _MenuContainer;
			}
			set
			{
				_MenuContainer = value;
			}
		}

		public string _StaticMenuItemStyle = "ms-topnav";
		[Personalizable, WebBrowsable, WebDisplayName("Static Menu Item Style CSS (ms-topnav)")]
		public string Set_StaticMenuItemStyle
		{
			get
			{
				return _StaticMenuItemStyle;
			}
			set
			{
				_StaticMenuItemStyle = value;
			}
		}

		public string _StaticSelectedStyle = "ms-topnavselected";
		[Personalizable, WebBrowsable, WebDisplayName("Static Selected Style CSS (ms-topnavselected)")]
		public string Set_StaticSelectedStyle
		{
			get
			{
				return _StaticSelectedStyle;
			}
			set
			{
				_StaticSelectedStyle = value;
			}
		}

		public string _StaticHoverStyle = "ms-topNavHover";
		[Personalizable, WebBrowsable, WebDisplayName("Static Hover Style CSS (ms-topNavHover)")]
		public string Set_StaticHoverStyle
		{
			get
			{
				return _StaticHoverStyle;
			}
			set
			{
				_StaticHoverStyle = value;
			}
		}

		public string _DynamicMenuStyle = "ms-topnavContainer";
		[Personalizable, WebBrowsable, WebDisplayName("Dynamic Menu Style CSS (ms-topnavContainer)")]
		public string Set_DynamicMenuStyle
		{
			get
			{
				return _DynamicMenuStyle;
			}
			set
			{
				_DynamicMenuStyle = value;
			}
		}

		public string _DynamicMenuItemStyle = "ms-topNavFlyOuts";
		[Personalizable, WebBrowsable, WebDisplayName("Dynamic Menu Item Style CSS (ms-topNavFlyOuts)")]
		public string Set_DynamicMenuItemStyle
		{
			get
			{
				return _DynamicMenuItemStyle;
			}
			set
			{
				_DynamicMenuItemStyle = value;
			}
		}

		public string _DynamicHoverStyle = "ms-topNavFlyOutsHover";
		[Personalizable, WebBrowsable, WebDisplayName("Dynamic Hover Style CSS (ms-topNavFlyOutsHover)")]
		public string Set_DynamicHoverStyle
		{
			get
			{
				return _DynamicHoverStyle;
			}
			set
			{
				_DynamicHoverStyle = value;
			}
		}

		public string _DynamicSelectedStyle = "ms-topNavFlyOutsSelected";
		[Personalizable, WebBrowsable, WebDisplayName("Dynamic Selected Style CSS (ms-topNavFlyOutsSelected)")]
		public string Set_DynamicSelectedStyle
		{
			get
			{
				return _DynamicSelectedStyle;
			}
			set
			{
				_DynamicSelectedStyle = value;
			}
		}

		public string _MenuOrientation = "Horizontal";
		[Personalizable, WebBrowsable, WebDisplayName("Menu Orientation (Horizontal or Vertical")]
		public string Set_MenuOrientation
		{
			get
			{
				return _MenuOrientation;
			}
			set
			{
				_MenuOrientation = value;
			}
		}

		public string _MenuName = "spNavMenu";
		[Personalizable, WebBrowsable, WebDisplayName("Menu Name")]
		public string Set_MenuName
		{
			get
			{
				return _MenuName;
			}
			set
			{
				_MenuName = value;
			}
		}

		public string _TopNavigationMenu = "yes";
		[Personalizable, WebBrowsable, WebDisplayName("Top Navigation Menu? (yes or no)")]
		public string Set_TopNavigationMenu
		{
			get
			{
				return _TopNavigationMenu;
			}
			set
			{
				_TopNavigationMenu = value;
			}
		}

		public string _MenuPreRenderClientScript = "";
		[Personalizable, WebBrowsable, WebDisplayName("Menu Client Script")]
		public string Set_MenuPreRenderClientScript
		{
			get
			{
				return _MenuPreRenderClientScript;
			}
			set
			{
				_MenuPreRenderClientScript = value;
			}
		}

		public string SetServerURL(string inURL)
		{
			if (inURL.ToLower().StartsWith("http"))
				return inURL;
			else
				if (inURL.StartsWith("/"))
					return Set_ServerURL + inURL;
				else if (inURL.Length > 0)
					return String.Format("{0}/{1}", Set_ServerURL, inURL);
				else
					return "";
		}

		public CustomTopNavWebPart()
		{
			ExportMode = WebPartExportMode.All;
		}

		Boolean _NewWindowFieldExists = false;
		/// <summary>
		/// Create all your controls here for rendering.
		/// Try to avoid using the RenderWebPart() method.
		/// </summary>
		protected override void CreateChildControls()
		{
			// Create ASPMenu control.

			ChromeType = PartChromeType.None;

			/*string MenuID = _MenuList + "CustomTopNavWebPart";*/
			AspMenu _spMenu = new AspMenu { UseSimpleRendering = true, UseSeparateCSS = false, Orientation = Orientation.Horizontal, MaximumDynamicDisplayLevels = 5, CssClass = "s4-tn" };
			SPWeb thisWeb = null;
			try
			{
				//Set some defaults
				if (Set_TopNavigationMenu.ToLower() != "yes" && Set_TopNavigationMenu.ToLower() != "no")
					Set_TopNavigationMenu = "yes";
				if (Set_TopNavigationMenu.ToLower() == "yes")
				{
					Set_MenuName = "TopNavigationMenu";
					Set_MenuOrientation = "horizontal";
				}

				//Set the controls look and feel
				//For some reason the ID of the Menu matters when using this as a Top Menu, the name must be TopNavigationMenu or the dropdowns are blank.
				_spMenu.ID = Set_MenuName;
				_spMenu.EnableViewState = false;


				SPSite thisSite;
				if (Set_ServerURL == "" || Set_ServerURL == null)
				{
					thisSite = SPControl.GetContextSite(Context);
					Set_ServerURL = thisSite.Url;
				}
				else
					thisSite = new SPSite(Set_ServerURL);

				if (Set_WebSite == "" || Set_WebSite == null)
				{
					thisWeb = thisSite.OpenWeb("/");
				}
				else
					thisWeb = thisSite.OpenWeb(Set_WebSite);

				SPList _spListMenu = thisWeb.Lists[Set_MenuList];
				_NewWindowFieldExists = _spListMenu.Fields.ContainsField("OpenNewWindow");
				SPQuery _spQuery = new SPQuery();
				MenuItem _spMenuItem = new MenuItem();
				_spQuery.Query = "<OrderBy><FieldRef Name='LinkOrder' Ascending='True' /><FieldRef Name='Title' Ascending='True' /></OrderBy><Where><And><IsNull><FieldRef Name='ParentMenu' /></IsNull><Eq><FieldRef Name='ShowMenuItem' /><Value Type='Choice'>Yes</Value></Eq></And></Where>";
				SPListItemCollection _spListItems = _spListMenu.GetItems(_spQuery);
				foreach (SPListItem item in _spListItems)
				{

					if (item["Link URL"] == null)
					{
						_spMenuItem = new MenuItem(item["Title"].ToString());
					}
					else
					{
						_spMenuItem = new MenuItem(item["Title"].ToString(), "", "", SetServerURL(item["LinkURL"].ToString()));
					}
					GetListItems(item["ID"].ToString(), _spMenuItem, _spListMenu);
					if (Page.Request.Url.AbsoluteUri == item["Link URL"].ToString())
					{
						_spMenuItem.Selected = true;
					}
					_spMenu.Items.Add(_spMenuItem);
				}

				Controls.Add(_spMenu);
			}
			catch (Exception ex)
			{
				ULSLoggingService myULS = new ULSLoggingService("TopNavProvider", SPContext.Current.Site.WebApplication.Farm);
				if (myULS != null)
				{
					SPDiagnosticsCategory cat = new SPDiagnosticsCategory("CreateChildControls", TraceSeverity.High, EventSeverity.Verbose);
					cat = myULS[CategoryId.WebPart];
					myULS.WriteTrace(1, cat, TraceSeverity.Verbose, String.Format("{0};{1}", ex.Message, ex.StackTrace), myULS.TypeName);
				}
				Controls.Add(new LiteralControl(String.Format("An error has occured with this web part.  Please contact your system administrator and relay this error message: {0} sub:CreateChildControls ", ex.Message)));
			}
			finally
			{
				if (thisWeb != null)
					thisWeb.Dispose();
			}
		}
		private void GetListItems(string str, MenuItem _spMenu, SPList _spListMenu)
		{
			try
			{
				SPQuery _spQuery = new SPQuery();
				string target = "";
				_spQuery.Query = String.Format("<OrderBy><FieldRef Name='LinkOrder' Ascending='True' /><FieldRef Name='Title' Ascending='True' /></OrderBy><Where><And><Eq><FieldRef Name='ParentMenu' LookupId= 'TRUE'  /><Value Type='Lookup'>{0}</Value></Eq><Eq><FieldRef Name='ShowMenuItem' /><Value Type='Choice'>Yes</Value></Eq></And></Where>", str);
				SPListItemCollection _spListItems = _spListMenu.GetItems(_spQuery);
				MenuItem _spMenuItem = new MenuItem();
				foreach (SPListItem item in _spListItems)
				{
					target = "";
					if (item["Link URL"] == null)
					{
						_spMenuItem = new MenuItem(item["Title"].ToString());
					}
					else
					{
						if (_NewWindowFieldExists && item["OpenNewWindow"] != null && (Boolean)item["OpenNewWindow"] == true)
							target = "_blank";
						_spMenuItem = new MenuItem(item["Title"].ToString(), "", "", SetServerURL(item["Link URL"].ToString()), target);
					}
					GetListItems(item["ID"].ToString(), _spMenuItem, _spListMenu);
					_spMenu.ChildItems.Add(_spMenuItem);
				}
			}
			catch (Exception ex)
			{
				ULSLoggingService myULS = new ULSLoggingService("TopNavProvider", SPContext.Current.Site.WebApplication.Farm);
				if (myULS != null)
				{
					SPDiagnosticsCategory cat = new SPDiagnosticsCategory("CreateChildControls", TraceSeverity.High, EventSeverity.Verbose);
					cat = myULS[CategoryId.WebPart];
					myULS.WriteTrace(1, cat, TraceSeverity.Verbose, String.Format("{0};{1}", ex.Message, ex.StackTrace), myULS.TypeName);
				}
				Controls.Add(new LiteralControl(String.Format("An error has occured with this web part.  Please contact your system administrator and relay this error message: {0} sub:GetListItems", ex.Message)));
			}

		}

		protected override void OnPreRender(EventArgs e)
		{
			if (Set_MenuPreRenderClientScript.Length > 0)
			{
				Page.ClientScript.RegisterStartupScript(Page.GetType(), Set_MenuPreRenderClientScript, String.Format("{0}(document.getElementById('{1}'));", Set_MenuPreRenderClientScript, ClientID), true);
			}
			base.OnPreRender(e);
		}
	}
}
