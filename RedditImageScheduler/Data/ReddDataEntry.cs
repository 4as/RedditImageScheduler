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

		/// <summary>
		/// Title of the post. 
		/// </summary>
		[Column(nameof(Title))]
		public string Title { get; set; }

		/// <summary>
		/// URL of the post.
		/// </summary>
		[Column(nameof(Source))]
		public string Source { get; set; }

		/// <summary>
		/// Raw image data of the post.
		/// </summary>
		[Column(nameof(Image))]
		public byte[] Image { get; set; }
		
		/// <summary>
		/// Has the post already been submitted?
		/// </summary>
		[Column(nameof(IsPosted))]
		public bool IsPosted { get; set; }
		
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
		
		[Ignore] public bool HasValidTitle => ReddValidator.IsValidTitle(Title);
		[Ignore] public bool HasValidSource => ReddValidator.IsValidSource(Source);
		[Ignore] public bool HasValidImage => ReddValidator.IsValidImage(Image);
		[Ignore] public bool HasValidDate => ReddValidator.IsValidDate(Date);
		[Ignore] public ReddDateState State => new ReddDateState(this);

		public void Validate() {
#if DEBUG
			IsPosted = false;
#endif
			IsValid = HasValidTitle && HasValidSource && HasValidImage && HasValidDate;
		}

		private readonly StringBuilder sBuilder = new StringBuilder();
		public override string ToString() {
			sBuilder.Clear();
			sBuilder.Append(Id).Append(": ");
			sBuilder.Append(Title);
			//sBuilder.Append(" : ").Append(Source);
			return sBuilder.ToString();
		}
	}
}