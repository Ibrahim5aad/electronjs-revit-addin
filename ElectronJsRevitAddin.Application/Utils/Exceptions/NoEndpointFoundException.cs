using System;
namespace ElectronJsRevitAddin.Utils.Exceptions
{
	internal class NoEndpointFoundException : Exception
	{
		public NoEndpointFoundException(string endpointName) : base($"No endpoint of name {endpointName} can be be found.")
		{
		}
	}
}
