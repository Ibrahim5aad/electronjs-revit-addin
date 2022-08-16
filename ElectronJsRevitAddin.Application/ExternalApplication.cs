using Autodesk.Revit.UI;
using ElectronJsRevitAddin.Controllers;
using ElectronJsRevitAddin.ExternalCommands;
using ElectronJsRevitAddin.Utils;

namespace ElectronJsRevitAddin
{
	public class ExternalApplication : IExternalApplication
	{

		#region Fields

		private readonly string _tabName = "Ibrahim Saad";

		#endregion

		#region Methods

		/// <summary>
		/// Executes some tasks when Autodesk Revit shuts down.
		/// </summary>
		/// <param name="application">A handle to the application being shut down.</param>
		/// <returns>Indicates if the external application completes its work successfully.</returns>
		public Result OnShutdown(UIControlledApplication application)
		{
			try
			{
				MainCommand.UIProcess.CloseMainWindow();
				MainCommand.UIProcess.Close();
				ControllerBase.CloseServer();
			}
			catch (System.Exception)
			{
			}
			return Result.Succeeded;
		}


		/// <summary>
		/// Executes some tasks when Autodesk Revit starts.
		/// </summary>
		/// <param name="application">A handle to the application being started.</param>
		/// <returns>Indicates if the external application completes its work successfully.</returns>
		public Result OnStartup(UIControlledApplication application)
		{

			RevitUIFactory.AddRibbonTab(application, _tabName);

			RibbonPanel loginPanel = RevitUIFactory.AddRibbonPanel
															(application, _tabName, "Utilities", false);

			RevitUIFactory.AddRibbonButton
								("Revit Tasks",
								loginPanel,
								typeof(MainCommand),
								Application.Properties.Resources.Icon_32x32,
								Application.Properties.Resources.Icon_16x16,
								"",
								null);


			//RevitUIFactory.AddRibbonButton
			//					("Close Server",
			//					loginPanel,
			//					typeof(CloseCommand),
			//					Application.Properties.Resources.Icon_32x32,
			//					Application.Properties.Resources.Icon_16x16,
			//					"",
			//					null);

			return Result.Succeeded;

		}

		#endregion

	}
}
