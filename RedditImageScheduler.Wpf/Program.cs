using System;
using Eto.Forms;

namespace RedditImageScheduler.Wpf {
	class Program {
		[STAThread]
		public static void Main(string[] args) {
			new ReddApplication(Eto.Platforms.Wpf).Run();
		}
	}
}