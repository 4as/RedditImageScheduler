using System;
using System.IO;
using Eto.Drawing;
using Eto.Forms;

namespace RedditImageScheduler.Utils {
	public class ReddUtilDropHandler {
		public void HandleStart(DragEventArgs e) {
			if( e.Data.ContainsUris || e.Data.ContainsImage ) {
				e.Effects = DragEffects.Copy;
			}
			else {
				e.Effects = DragEffects.None;
			}
		}

		public void HandleDrop(DragEventArgs e) {
			if( e.Data.ContainsUris ) {
				Uri uri = e.Data.Uris[0];
				if( uri.IsFile ) {
					if( ReddURLFileParser.IsURL(uri) ) {
						string url = ReddURLFileParser.GetUrl(uri);
						if( url != null ) {
							OnDropURL?.Invoke( url );
						}
					}
					else if( Array.IndexOf(ReddConfig.IMAGE_FILTER.Extensions, Path.GetExtension(uri.LocalPath)) != -1 ) {
						OnDropFile?.Invoke(uri.LocalPath);
					}
				}
				else if( uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps ) {
					OnDropURL?.Invoke( uri.AbsoluteUri );
				}
			}
			else if( e.Data.ContainsImage ) {
				OnDropImage?.Invoke(e.Data.Image);
			}
		}

		public delegate void DropURLEvent(string url);
		public event DropURLEvent OnDropURL;
		
		public delegate void DropFileEvent(string filepath);
		public event DropFileEvent OnDropFile;
		
		public delegate void DropImageEvent(Image image);
		public event DropImageEvent OnDropImage;
	}
}