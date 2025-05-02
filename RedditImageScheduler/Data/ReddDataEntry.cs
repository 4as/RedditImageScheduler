using System;
using System.Text;
using Eto.Drawing;
using RedditImageScheduler.Utils;
using SQLite;

namespace RedditImageScheduler.Data {
	[Table(nameof(ReddDataEntry))]
	public class ReddDataEntry {
		[PrimaryKey, AutoIncrement]
		[Column(nameof(Id))]
		public uint Id { get; set; }

		[Column(nameof(Timestamp))]
		public long Timestamp { get; set; }

		[Column(nameof(Title))]
		public string Title { get; set; }

		[Column(nameof(Source))]
		public string Source { get; set; }

		[Column(nameof(Image))]
		public byte[] Image { get; set; }
		
		[Column(nameof(IsValid))]
		public bool IsValid { get; set; }

		[Ignore]
		public DateTime Date {
			get => DateTimeOffset.FromUnixTimeSeconds(Timestamp).ToLocalTime().DateTime;
			set => Timestamp = new DateTimeOffset(value).ToUnixTimeSeconds();
		}

		[Ignore]
		public ReddUtilBitmap Bitmap {
			get => new ReddUtilBitmap(Id, Image);
			set => Image = value.ToByteArray();
		}

		private readonly StringBuilder sBuilder = new StringBuilder();
		public override string ToString() {
			sBuilder.Clear();
			sBuilder.Append('"').Append(Title).Append('"');
			sBuilder.Append(" : ").Append(Source);
			return sBuilder.ToString();
		}
	}
}