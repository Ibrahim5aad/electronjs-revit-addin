using System.Text;

namespace ElectronJsRevitAddin.TestServer
{
	public class PipeMessage
	{
		public const int MessageBufferSize = 8192;

		/// <summary>
		/// Initializes a new instance of the <see cref="PipeMessage"/> class.
		/// </summary>
		public PipeMessage()
		{
			Encoding = Encoding.UTF8;
			EmptyMessageBytes = Encoding.GetBytes(string.Empty.PadRight(MessageBufferSize, '\0'));
			MessageBytes = Encoding.GetBytes(string.Empty.PadRight(MessageBufferSize, '\0'));
		}

		public PipeMessage(string message)
		{
			Encoding = Encoding.UTF8;
			SetMessageInBuffer(message);
		}

		public string Message
		{
			get => GetMessageFromBuffer();
			set => SetMessageInBuffer(value);
		}

		public byte[] EmptyMessageBytes { get; }

		public byte[] MessageBytes { get; set; }

		public Encoding Encoding { get; set; }

		public void SetMessageInBuffer(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				MessageBytes = new byte[message.Length];
				return;
			}

			var msg = $"{message.TrimEnd().TrimEnd('\0')}";
			if (msg.EndsWith("\r\n")) 
				msg = msg.Remove(msg.Length - 2);
			var bytes = Encoding.GetBytes(msg);
			MessageBytes = bytes;
		}

		public string GetMessageFromBuffer()
		{
			return $"{Encoding.GetString(MessageBytes).TrimEnd().TrimEnd('\0')}\r\n";
		}

		public bool IsNullOrEmpty()
		{
			var msg = GetMessageFromBuffer().Trim().Trim('\0');
			return string.IsNullOrEmpty(msg);
		}
	}
}
