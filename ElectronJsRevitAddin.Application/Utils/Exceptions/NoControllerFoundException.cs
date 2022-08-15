using System;
namespace ElectronJsRevitAddin.Utils.Exceptions
{
	internal class NoControllerFoundException : Exception
	{
		public NoControllerFoundException(string controllerName) : base($"No controller of name {controllerName} can be be found.")
		{
		}
	}
}
