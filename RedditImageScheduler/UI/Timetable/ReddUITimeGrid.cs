using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.UI.Timetable {
	public class ReddUITimeGrid : GridView {
		private readonly GridColumn etoColumn = new GridColumn();
		private readonly GridEntry uiEntry = new GridEntry();

		public ReddUITimeGrid() {
			ShowHeader = false;
			GridLines = GridLines.Horizontal;
			AllowColumnReordering = false;
			AllowEmptySelection = false;
			AllowMultipleSelection = false;
			etoColumn.DataCell = uiEntry;
			etoColumn.AutoSize = true;
			etoColumn.Editable = false;
			etoColumn.MinWidth = 300;
			etoColumn.Expand = true;
			etoColumn.Resizable = false;
			etoColumn.Sortable = false;
			Columns.Add(etoColumn);
		}

		public ReddDataEntry CurrentNext {
			get => uiEntry.CurrentNext;
			set => uiEntry.CurrentNext = value;
		}

		private class GridEntry : DrawableCell {
			private readonly FormattedText drawTextDefault = new FormattedText();
			private readonly SolidBrush drawBrushDefault = new SolidBrush(Colors.Black);
			private readonly FormattedText drawTextSelected = new FormattedText();
			private readonly SolidBrush drawBrushSelected = new SolidBrush(Colors.White);

			public ReddDataEntry CurrentNext;

			public GridEntry() {
				drawTextDefault.Alignment = FormattedTextAlignment.Left;
				drawTextDefault.Wrap = FormattedTextWrapMode.None;
				drawTextDefault.Font = SystemFonts.Default();
				drawTextDefault.ForegroundBrush = drawBrushDefault;
				
				drawTextSelected.Alignment = FormattedTextAlignment.Left;
				drawTextSelected.Wrap = FormattedTextWrapMode.None;
				drawTextSelected.Font = SystemFonts.Default();
				drawTextSelected.ForegroundBrush = drawBrushSelected;
			}
			
			protected override void OnPaint(CellPaintEventArgs e) {
				if( !(e.Item is ReddDataEntry entry) ) {
					base.OnPaint(e);
					return;
				}
				
				var g = e.Graphics;
				var bounds = e.ClipRectangle;
				PointF pos = bounds.Location;
				
				if( e.IsSelected ) {
					g.FillRectangle(ReddConfig.UI_BG_SELECTED, bounds);

					drawTextSelected.Text = string.IsNullOrEmpty(entry.Title) ? ReddLanguage.TEXT_NO_TITLE : entry.Title;
					pos.Y += bounds.Height / 2 - drawTextSelected.Measure().Height / 2;
					g.DrawText(drawTextSelected, pos);
				}
				else {
					if( entry == CurrentNext ) {
						g.FillRectangle(ReddConfig.UI_BG_NEXT, bounds);
					}
					else if( entry.IsPosted ) {
						g.FillRectangle(ReddConfig.UI_BG_POSTED, bounds);
					}
					else if( entry.IsValid ) {
						g.FillRectangle(ReddConfig.UI_BG_DEFAULT, bounds);
					}
					else {
						g.FillRectangle(ReddConfig.UI_BG_INVALID, bounds);
					}

					drawTextDefault.Text = string.IsNullOrEmpty(entry.Title) ? ReddLanguage.TEXT_NO_TITLE : entry.Title;
					
					float bh = bounds.Height / 2f;
					float th = drawTextDefault.Measure().Height / 2f;
					pos.Y += bh - th;
					g.DrawText(drawTextDefault, pos);
				}
			}
		}
	}
}