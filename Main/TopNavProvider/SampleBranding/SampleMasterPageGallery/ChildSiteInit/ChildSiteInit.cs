using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace DeviQ.SharePoint.Branding.SampleMasterPageGallery.ChildSiteInit
{
	/// <summary>
	/// Web Events
	/// </summary>
	public class ChildSiteInit : SPWebEventReceiver
	{
		/// <summary>
		/// A site was provisioned.
		/// </summary>
		public override void WebProvisioned(SPWebEventProperties properties)
		{
			SPWeb childSite = properties.Web;
			SPWeb topSite = childSite.Site.RootWeb;
			childSite.MasterUrl = topSite.MasterUrl;
			childSite.CustomMasterUrl = topSite.CustomMasterUrl;
			childSite.AlternateCssUrl = topSite.AlternateCssUrl;
			childSite.SiteLogoUrl = topSite.SiteLogoUrl;
			childSite.Update();

		}


	}
}