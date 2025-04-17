using System;
using System.ComponentModel;
using Eto.Forms;
using RedditImageScheduler.UI;

namespace RedditImageScheduler.WinForms {
	class Program {
		[STAThread]
		public static void Main(string[] args) {
			new RedditApplication(Eto.Platforms.WinForms).Run();
		}
	}
}