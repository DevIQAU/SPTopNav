using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace Deviq.SharePoint.Utils
{
	public class ULS
	{
		public static void LogMessage(string productName, string message, TraceSeverity traceSeverity, EventSeverity eventSeverity)
		{
			SPDiagnosticsService diagService = SPDiagnosticsService.Local;
			diagService.WriteTrace(0, new SPDiagnosticsCategory(productName, traceSeverity, eventSeverity), traceSeverity, message, new object[] { message });

		}
	}
}
