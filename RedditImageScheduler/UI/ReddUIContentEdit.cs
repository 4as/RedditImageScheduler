using System;
using Eto.Forms;

namespace RedditImageScheduler.UI {
	public class ReddUIContentEdit {
		//todo: implement a custom layout with support for alignment (so the remove button will be at the end) 
		private readonly StackLayout etoLayout = new StackLayout();
		private readonly Button etoButtonAdd = new Button();
		private readonly Button etoButtonRemove = new Button();

		public ReddUIContentEdit() {
			etoLayout.Orientation = Orientation.Horizontal;
			etoButtonAdd.Text = ReddLanguage.ADD;
			etoLayout.Items.Add(etoButtonAdd);
			
			etoButtonRemove.Text = ReddLanguage.REMOVE;
			etoLayout.Items.Add(etoButtonRemove);
		}
		
		public Panel UI => etoLayout;

		public bool AllowRemove {
			get => etoButtonRemove.Visible;
			set => etoButtonRemove.Visible = value;
		}

		public void Initialize() {
			etoButtonAdd.Click += OnPressAdd;
			etoButtonRemove.Click += OnPressRemove;
		}
		
		public void Deinitialize() {
			etoButtonAdd.Click -= OnPressAdd;
			etoButtonRemove.Click -= OnPressRemove;
		}

		// ===============================================
		// EVENTS
		public delegate void UIEditEvent();
		public event UIEditEvent OnAdd;
		public event UIEditEvent OnRemove;
		
		// ===============================================
		// CALLBACKS
		private void OnPressAdd(object sender, EventArgs e) {
			OnAdd?.Invoke();
		}

		private void OnPressRemove(object sender, EventArgs e) {
			OnRemove?.Invoke();
		}
	}
}