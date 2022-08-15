using Autodesk.Revit.UI;
using ElectronJsRevitAddin.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ElectronJsRevitAddin.Utils
{
	/// <summary>
	/// Class RevitUIFactory.
	/// </summary>
	public static class RevitUIFactory
	{

		/// <summary>
		/// Adds the ribbon tab.
		/// </summary> 
		public static bool AddRibbonTab(UIControlledApplication application, string tabName)
		{
			try
			{
				application.CreateRibbonTab(tabName);
			}
			catch
			{
				return false;
			}
			return true;
		}


		/// <summary>
		/// Adds the ribbon panel.
		/// </summary> 
		public static RibbonPanel AddRibbonPanel(UIControlledApplication application, string tabName, string panelName, bool addSeparator)
		{
			List<RibbonPanel> panels = application.GetRibbonPanels(tabName);
			RibbonPanel panel = panels.FirstOrDefault(x => x.Name == panelName);

			if (panel == null)
				panel = application.CreateRibbonPanel(tabName, panelName);
			else if (addSeparator)
				panel.AddSeparator();

			return panel;
		}


		/// <summary>
		/// Add the ribbon tab and the plugin button to revit.
		/// </summary> 
		public static PushButton AddRibbonButton(string btnName,
													RibbonPanel panel,
													Type commandType,
													Bitmap icon32,
													Bitmap icon16,
													string tooltip,
													Type commandAvailability
													)
		{
			string path = commandType.Assembly.Location;
			var button = new PushButtonData("btn" + btnName.Replace(" ", ""), btnName, path, commandType.FullName)
			{
				LargeImage = icon32.ToBitmapSource(),
				Image = icon16.ToBitmapSource(),
				ToolTip = tooltip
			};
			if (commandAvailability != null)
				button.AvailabilityClassName = commandAvailability.FullName;
			return panel.AddItem(button) as PushButton;
		}


		/// <summary>
		/// Adds the ribbon button to a pull down button.
		/// </summary> 
		/// <returns>PushButton.</returns>
		public static PushButton AddRibbonButton(PulldownButton pullDownButton,
													string name,
													Type commandType,
													Type availbilityClass,
													Bitmap icon32,
													Bitmap icon16,
													Type commandAvailability)
		{
			string path = commandType.Assembly.Location;
			var btn = pullDownButton.AddPushButton(new PushButtonData($"btn{name.Replace(" ", "")}", name, path, commandType.FullName));

			btn.Image = icon16.ToBitmapSource();
			btn.LargeImage = icon32.ToBitmapSource();
			btn.AvailabilityClassName = availbilityClass.FullName;
			if (commandAvailability != null)
				btn.AvailabilityClassName = commandAvailability.FullName;

			return btn;
		}


		/// <summary>
		/// Adds the ribbon split button.
		/// </summary> 
		public static SplitButton AddRibbonSplitButton(string btnName, RibbonPanel panel)
		{
			SplitButtonData splitBtnData = new SplitButtonData($"btn{btnName.Replace(" ", "")}", btnName);
			SplitButton splitBtn = panel.AddItem(splitBtnData) as SplitButton;
			return splitBtn;
		}


		/// <summary>
		/// Adds the ribbon pulldown button.
		/// </summary> 
		public static PulldownButton AddRibbonPulldownButton(string btnName, RibbonPanel panel, Bitmap icon32, Bitmap icon16)
		{
			PulldownButtonData group1Data = new PulldownButtonData($"btn{btnName.Replace(" ", "")}", btnName);
			PulldownButton pullDownBtn = panel.AddItem(group1Data) as PulldownButton;
			pullDownBtn.Image = icon16.ToBitmapSource();
			pullDownBtn.LargeImage = icon32.ToBitmapSource();
			return pullDownBtn;
		}

	}
}
