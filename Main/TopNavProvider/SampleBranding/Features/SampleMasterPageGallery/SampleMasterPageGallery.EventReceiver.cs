using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using System.Diagnostics;
using Deviq.SharePoint.Utils;

namespace DeviQ.SharePoint.Branding
{
	/// <summary>
	/// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
	/// </summary>
	/// <remarks>
	/// The GUID attached to this class may be used during packaging and should not be modified.
	/// </remarks>

	[Guid("161aacb2-0d68-4f1b-beca-1eaecd66b62c")]
	public class DeviQlMasterPageGalleryEventReceiver : SPFeatureReceiver
	{
		const string FEATURE_NAME = "SampleMasterPageGallery";
		const string ULS_PRODUCT_NAME = "Sample Branding Feature";
		const string defaultMasterUrl = "_catalogs/masterpage/v4.master";
		const string customizedMasterUrl = "_catalogs/masterpage/sample.master";
		const string defaultAlternateCSSUrl = "";
		const string alternateCSSUrl = "Style Library/CSS/sample.css";

		// Uncomment the method below to handle the event raised after a feature has been activated.

		public override void FeatureActivated(SPFeatureReceiverProperties properties)
		{
#if DEBUG
			Debugger.Break();
#endif
			ULS.LogMessage(ULS_PRODUCT_NAME, string.Format("Started Activation of {0} feature", FEATURE_NAME), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
			SPSite siteCollection = properties.Feature.Parent as SPSite;
			if (siteCollection != null)
			{
				ULS.LogMessage(ULS_PRODUCT_NAME, string.Format("Started Activation of {0} feature for {1}", FEATURE_NAME, siteCollection.Port), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
				SPWeb topLevelSite = siteCollection.RootWeb;

				// Calculate relative path to site from Web Application root.
				string WebAppRelativePath = topLevelSite.ServerRelativeUrl;
				if (!WebAppRelativePath.EndsWith("/"))
				{
					WebAppRelativePath += "/";
				}

				ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Processed: {0}", topLevelSite.Title), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);

				// Enumerate through each site and apply branding.
				foreach (SPWeb site in siteCollection.AllWebs)
				{
					try
					{
						ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Started Activating {0} Feature in the {1} site ", FEATURE_NAME, site.Name), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
						site.AllowUnsafeUpdates = true;
						site.MasterUrl = WebAppRelativePath + customizedMasterUrl;
						site.CustomMasterUrl = WebAppRelativePath + customizedMasterUrl;
						site.AlternateCssUrl = WebAppRelativePath + alternateCSSUrl;
						//site.SiteLogoUrl = WebAppRelativePath + "Style%20Library/Branding101/Images/Logo.gif";
						site.SiteLogoUrl = "";
						site.UIVersion = 4;
						site.Update();
						site.AllowUnsafeUpdates = false;
					}
					catch (Exception ex)
					{
						ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("{0};{1}", ex.Message, ex.StackTrace), Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, Microsoft.SharePoint.Administration.EventSeverity.Error);
					}
					finally
					{
						if (site != null)
						{
							site.Dispose();
						}
						ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Finished Activating {0} Feature in the {1} site ", FEATURE_NAME, site.Name), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
					}
				}
			}
		}

		public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
		{
#if DEBUG
			Debugger.Break();
#endif
			ULS.LogMessage(ULS_PRODUCT_NAME, string.Format("Started De-activating {0}", FEATURE_NAME), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
			SPSite siteCollection = properties.Feature.Parent as SPSite;
			if (siteCollection != null)
			{
				SPWeb topLevelSite = siteCollection.RootWeb;

				ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Started De-activating {0} Feature in {1} portal ", FEATURE_NAME, siteCollection.PortalName), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);

				// Calculate relative path of site from Web Application root.
				string WebAppRelativePath = topLevelSite.ServerRelativeUrl;
				if (!WebAppRelativePath.EndsWith("/"))
				{
					WebAppRelativePath += "/";
				}

				ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Processed: {0}", topLevelSite.Title), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);

				// Enumerate through each site and remove custom branding.
				foreach (SPWeb site in siteCollection.AllWebs)
				{
					try
					{
						ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Started De-activating {0} Feature in the {1} site ", FEATURE_NAME, site.Name), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
						site.MasterUrl = WebAppRelativePath + defaultMasterUrl;
						site.CustomMasterUrl = WebAppRelativePath + defaultMasterUrl;
						site.AlternateCssUrl = "";
						site.SiteLogoUrl = "";
						site.Update();
					}
					catch (Exception ex)
					{
						ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("{0};{1}", ex.Message, ex.StackTrace), Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, Microsoft.SharePoint.Administration.EventSeverity.Error);
					}
					finally
					{
						if (site != null)
						{
							site.Dispose();
						}
						ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Finished De-activating {0} Feature in the {1} site ", FEATURE_NAME, site.Name), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
					}
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
					using (SPWeb web = siteCollection.OpenWeb())
					{
						SPFile file = web.GetFile(customUrlToUse);
						ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Started Deleting the old master page in in the {0} site ", web.Title), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
						SPFolder masterPageGallery = file.ParentFolder;
						SPFolder temp = masterPageGallery.SubFolders.Add("Temp");
						string newFilePath = String.Format("{0}/{1}", temp.Url, file.Name);
						file.MoveTo(newFilePath, SPMoveOperations.Overwrite);
						SPFile movedFile = web.GetFile(newFilePath);
						movedFile.Delete();
						ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Finished deleting the old master page in in the {0} site ", web.Title), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
					}
				}
				catch (Exception ex)
				{
					ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("{0};{1}", ex.Message, ex.StackTrace), Microsoft.SharePoint.Administration.TraceSeverity.Unexpected, Microsoft.SharePoint.Administration.EventSeverity.Error);
				}
				finally
				{
					ULS.LogMessage(ULS_PRODUCT_NAME, String.Format("Finished De-activating {0} Feature in {0} portal ", FEATURE_NAME, siteCollection.PortalName), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
				}
			}
			ULS.LogMessage(ULS_PRODUCT_NAME, string.Format("Started De-activating {0}", FEATURE_NAME), Microsoft.SharePoint.Administration.TraceSeverity.Verbose, Microsoft.SharePoint.Administration.EventSeverity.Information);
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
