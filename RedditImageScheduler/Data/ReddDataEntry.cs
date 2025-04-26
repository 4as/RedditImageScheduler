using System.Text;
using Eto.Drawing;
using SQLite;

namespace RedditImageScheduler.Data {
	[Table(nameof(ReddDataEntry))]
	public class ReddDataEntry {
		[PrimaryKey, AutoIncrement]
		[Column(nameof(Id))]
		public int Id { get; set; }

		[Column(nameof(Timestamp))]
		public uint Timestamp { get; set; }

		[Column(nameof(Title))]
		public string Title { get; set; }

		[Column(nameof(Source))]
		public string Source { get; set; }

		[Column(nameof(Image))]
		public byte[] Image { get; set; }

		private readonly StringBuilder sBuilder = new StringBuilder();
		public override string ToString() {
			sBuilder.Clear();
			sBuilder.Append(Title);
			sBuilder.Append(" (URL: ").Append(Source).Append(')');
			return sBuilder.ToString();
		}
	}
}