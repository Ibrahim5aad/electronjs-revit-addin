using ProtoBuf;

namespace ElectronJsRevitAddin.Controllers.Base
{
	[ProtoContract]
	public class Request
	{

		[ProtoMember(1)]
		public string Endpoint { get; set; }


		[ProtoMember(2)]
		public string Payload { get; set; }
	}
}
