using Autodesk.Revit.UI;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ElectronJsRevitAddin.Application.Utils
{
	public static class WindowHandler
	{

		static IntPtr hHook;
		static HookProc PaintHookProcedure;
		private const int WH_GETMESSAGE = 3;


		/// <summary>
		/// Sets the window owner.
		/// </summary>
		/// <param name="application">The application.</param>
		/// <param name="window">The window.</param>
		public static void SetWindowOwner(UIApplication application, IntPtr electronWindowHandle)
		{
#if Revit2019 || Revit2020 || Revit2021 || Revit2022 || Revit2023
			//PaintHookProcedure = new HookProc(PaintHookProc); ;
			//var pt = SetWindowsHookEx(WH_GETMESSAGE, PaintHookProcedure, IntPtr.Zero, 0);

			HwndSource revitHwndSource = HwndSource.FromHwnd(application.MainWindowHandle);
			HwndSource electronUiHwndSource = HwndSource.FromHwnd(electronWindowHandle);

			Window currentRevitWindow = revitHwndSource.RootVisual as Window;
			Window currentElectronWindow = electronUiHwndSource.RootVisual as Window;
			currentElectronWindow.Owner = currentRevitWindow;

#else
			HwndSource electronUiHwndSource = HwndSource.FromHwnd(electronWindowHandle);
			Window currentElectronWindow = electronUiHwndSource.RootVisual as Window;
			WindowInteropHelper helper = new WindowInteropHelper(currentElectronWindow);
			helper.Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
#endif
		}

		/// <summary>
		/// Gets the window handle.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>The window handle (hwnd)</returns>
		public static int GetWindowHandle(Window window)
		{
			WindowInteropHelper helper = new WindowInteropHelper(window);
			return helper.Handle.ToInt32();
		}

		/// <summary>
		/// Activates the window.
		/// </summary> 
		public static bool ActivateWindow()
		{
			IntPtr ptr = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

			if (ptr != IntPtr.Zero)
			{
				return SetForegroundWindow(ptr);
			}

			return false;
		}

		public static int PaintHookProc(int nCode, IntPtr wParam, IntPtr lParam)
		{
			// Do some painting here.
			return CallNextHookEx(hHook, nCode, wParam, lParam);
		}


		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);


		[DllImport("user32.dll", EntryPoint = "SetWindowsHookEx", SetLastError = true)]
		static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

	}

}
