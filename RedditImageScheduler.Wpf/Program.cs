using System;
using Eto.Forms;

namespace RedditImageScheduler.Wpf {
	class Program {
		[STAThread]
		public static void Main(string[] args) {
			new RedditApplication(Eto.Platforms.Wpf).Run();
		}
	}
}