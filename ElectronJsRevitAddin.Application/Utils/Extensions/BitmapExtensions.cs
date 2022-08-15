using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ElectronJsRevitAddin.Utils.Extensions
{
	public static class BitmapExtensions
	{

		/// <summary>
		/// Create a bitmap source from a bitmap.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		/// <returns>BitmapSource.</returns>
		public static BitmapSource ToBitmapSource(this Bitmap bitmap)
		{
			return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),
															IntPtr.Zero,
															Int32Rect.Empty,
															BitmapSizeOptions.FromEmptyOptions());
		}

	}
}
