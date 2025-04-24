using System;
using Eto.Forms;

namespace RedditImageScheduler.Mac {
	class Program {
		[STAThread]
		public static void Main(string[] args) {
			new ReddApplication(Eto.Platforms.Mac64).Run();
		}
	}
}