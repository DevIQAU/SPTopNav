using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using System.Diagnostics;

namespace PacAl.SharePoint.Branding
{
	/// <summary>
	/// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
	/// </summary>
	/// <remarks>
	/// The GUID attached to this class may be used during packaging and should not be modified.
	/// </remarks>

	[Guid("161aacb2-0d68-4f1b-beca-1eaecd66b62c")]
	public class PacAlMasterPageGalleryEventReceiver : SPFeatureReceiver
	{
		const string defaultMasterUrl = "_catalogs/masterpage/v4.master";
		const string customizedMasterUrl = "_catalogs/masterpage/pacal.master";
		const string defaultAlternateCSSUrl = "";
		const string alternateCSSUrl = "Style Library/CSS/pacal.css";

		// Uncomment the method below to handle the event raised after a feature has been activated.

		public override void FeatureActivated(SPFeatureReceiverProperties properties)
		{
			SPSite siteCollection = properties.Feature.Parent as SPSite;
			if (siteCollection != null)
			{
				SPWeb topLevelSite = siteCollection.RootWeb;

				// Calculate relative path to site from Web Application root.
				string WebAppRelativePath = topLevelSite.ServerRelativeUrl;
				if (!WebAppRelativePath.EndsWith("/"))
				{
					WebAppRelativePath += "/";
				}

				// Enumerate through each site and apply branding.
				foreach (SPWeb site in siteCollection.AllWebs)
				{
					site.MasterUrl = WebAppRelativePath + customizedMasterUrl;
					site.CustomMasterUrl = WebAppRelativePath + customizedMasterUrl;
					site.AlternateCssUrl = WebAppRelativePath + alternateCSSUrl;
					//site.SiteLogoUrl = WebAppRelativePath + "Style%20Library/Branding101/Images/Logo.gif";
					site.SiteLogoUrl = "";
					site.UIVersion = 4;
					site.Update();
				}
			}
		}

		private void DeactivateWeb(SPWeb web)
		{
			try
			{
				if (web.AllProperties.ContainsKey("OldMasterUrl"))
				{
					// Change the MasterURL and CustomMasterURL back to 
					// old versions, if the property exists 
					string oldMasterUrl = web.AllProperties["OldMasterUrl"].ToString();
					try
					{
						bool fileExists = web.GetFile(oldMasterUrl).Exists;
						web.MasterUrl = oldMasterUrl;
					}
					catch (ArgumentException)
					{
						web.MasterUrl = defaultMasterUrl;
					}

					string oldCustomUrl = web.AllProperties["OldCustomMasterUrl"].ToString();
					try
					{
						bool fileExists = web.GetFile(oldCustomUrl).Exists;
						web.CustomMasterUrl = web.AllProperties["OldCustomMasterUrl"].ToString();
					}
					catch (ArgumentException)
					{
						web.CustomMasterUrl = defaultMasterUrl;
					}

					string oldAlternateCssUrl = web.AllProperties["OldAlternateCssUrl"].ToString();
					try
					{
						bool fileExists = web.GetFile(oldAlternateCssUrl).Exists;
						web.AlternateCssUrl = web.AllProperties["OldAlternateCssUrl"].ToString();
					}
					catch (ArgumentException)
					{
						web.AlternateCssUrl = "";
					}

					web.SiteLogoUrl = "";

					// Remove the custom properties 
					web.AllProperties.Remove("OldMasterUrl");
					web.AllProperties.Remove("OldCustomMasterUrl");
					web.AllProperties.Remove("OldAlternateCssUrl");
				}
				else
				{
					// Otherwise, change back to default 
					web.MasterUrl = defaultMasterUrl;
					web.CustomMasterUrl = defaultMasterUrl;
				}
			}
			catch
			{
				try
				{
					web.MasterUrl = defaultMasterUrl;
					web.CustomMasterUrl = defaultMasterUrl;
				}
				catch { }
			}
		} 


		public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
		{
			SPSite siteCollection = properties.Feature.Parent as SPSite;
			if (siteCollection != null)
			{
				SPWeb topLevelSite = siteCollection.RootWeb;

				// Calculate relative path of site from Web Application root.
				string WebAppRelativePath = topLevelSite.ServerRelativeUrl;
				if (!WebAppRelativePath.EndsWith("/"))
				{
					WebAppRelativePath += "/";
				}

				// Enumerate through each site and remove custom branding.
				foreach (SPWeb site in siteCollection.AllWebs)
				{
					site.MasterUrl = WebAppRelativePath + defaultMasterUrl;
					site.CustomMasterUrl = WebAppRelativePath + defaultMasterUrl;
					site.AlternateCssUrl = "";
					site.SiteLogoUrl = "";
					site.Update();
				}
				// Now delete the master page
				string serverRelPath = topLevelSite.ServerRelativeUrl;
				if (!serverRelPath.EndsWith("/"))
				{
					serverRelPath += "/";
				}
				string customUrlToUse = serverRelPath + customizedMasterUrl;
				try
				{
					bool fileExists = siteCollection.OpenWeb().GetFile(customUrlToUse).Exists;
					SPFile file = siteCollection.OpenWeb().GetFile(customUrlToUse);
					SPFolder masterPageGallery = file.ParentFolder;
					SPFolder temp = masterPageGallery.SubFolders.Add("Temp");
					file.MoveTo(String.Format("{0}/{1}", temp.Url, file.Name));
					temp.Delete();
				}
				catch (Exception)
				{

				}
			}
		}


		// Uncomment the method below to handle the event raised after a feature has been installed.

		//public override void FeatureInstalled(SPFeatureReceiverProperties properties)
		//{
		//}


		// Uncomment the method below to handle the event raised before a feature is uninstalled.

		//public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
		//{
		//}

		// Uncomment the method below to handle the event raised when a feature is upgrading.

		//public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
		//{
		//}
	}
}
