using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;


namespace DeviQ.SharePoint.Utilities.Navigation
{
	[System.Runtime.InteropServices.GuidAttribute("DBEEB5AB-C5A7-46B5-A2BB-5581F960C333")]
	public class ULSLoggingService : SPDiagnosticsServiceBase
	{
		public static string DiagnosticsAreaName = "DEVIQ"; 
		
		public ULSLoggingService()
        { 
        }

		public ULSLoggingService(string name, SPFarm farm) : base(name, farm)
        {

        }

		protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
		{
			List<SPDiagnosticsCategory> categories = new List<SPDiagnosticsCategory>();
			foreach (string catName in Enum.GetNames(typeof(CategoryId)))
			{
				uint catId = (uint)(int)Enum.Parse(typeof(CategoryId), catName);
				categories.Add(new SPDiagnosticsCategory(catName, TraceSeverity.Verbose, EventSeverity.Error, 0, catId));
			}

			yield return new SPDiagnosticsArea(DiagnosticsAreaName, categories);
		}

		public static ULSLoggingService Local
		{
			get
			{
				return SPDiagnosticsServiceBase.GetLocal<ULSLoggingService>();
			}
		}

		public SPDiagnosticsCategory this[CategoryId id]
		{
			get
			{
				return Areas[DiagnosticsAreaName].Categories[id.ToString()];
			}
		}
	}
}
