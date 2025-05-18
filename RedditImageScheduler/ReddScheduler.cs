using System;
using System.Text;
using System.Threading;
using System.Timers;
using RedditImageScheduler.Data;
using Timer = System.Timers.Timer;

namespace RedditImageScheduler {
	public class ReddScheduler {
		private readonly Timer sysTimer = new Timer(1000);
		private readonly StringBuilder sbState = new StringBuilder();
		private readonly ReddDataOptions dataOptions;
		
		private SynchronizationContext sysContext = SynchronizationContext.Current;
		private ReddDataEntries dataEntries;
		private ReddDataEntry dataNext;
		private long nLastPosting;
		private bool bPaused;

		public ReddScheduler(ReddDataOptions option) {
			dataOptions = option;
		}

		public void Initialize(ReddDataEntries entries) {
			Deinitialize();
			
			if( sysContext == null ) sysContext = SynchronizationContext.Current;
			dataEntries = entries;
			dataEntries.OnUpdate += OnChange;
			OnChange();
		}

		public void Deinitialize() {
			if( dataEntries != null ) {
				dataEntries.OnUpdate -= OnChange;
				dataEntries = null;
			}

			StopTimer();
		}

		// ===============================================
		// GETTERS / SETTERS
		public ReddDataEntries Entries => dataEntries;
		public ReddDataEntry NextTarget => dataNext;

		public bool Paused {
			get => bPaused;
			set => bPaused = value;
		}

		// ===============================================
		// PUBLIC METHODS
		public string GetNextState() {
			sbState.Clear();
			if( dataNext == null ) {
				sbState.Append(ReddLanguage.TEXT_NONE);
			}
			else {
				var state = dataNext.State;
				switch( state.Status ) {
					default:
					case EReddDataStatus.VALID:
						sbState.AppendFormat(ReddLanguage.NEXT_FULL, dataNext.Title, dataNext.Date);
						break;
					case EReddDataStatus.VALID_HOURS:
						sbState.AppendFormat(ReddLanguage.NEXT_HOURS, dataNext.Title, state.Time.Hours);
						break;
					case EReddDataStatus.VALID_MINUTES:
						sbState.AppendFormat(ReddLanguage.NEXT_MINUTES, dataNext.Title, state.Time.Minutes);
						break;
					case EReddDataStatus.VALID_SECONDS:
						sbState.AppendFormat(ReddLanguage.NEXT_SECONDS, dataNext.Title, state.Time.Seconds);
						break;
				}
			}

			return sbState.ToString();
		}

		// ===============================================
		// NON-PUBLIC METHODS
		private void StartTimer() {
			if( sysTimer.Enabled ) return;

			sysTimer.Enabled = true;
			sysTimer.AutoReset = true;
			sysTimer.Elapsed += OnTimer;
			sysTimer.Start();
		}

		private void StopTimer() {
			if( !sysTimer.Enabled ) return;

			sysTimer.Enabled = false;
			sysTimer.AutoReset = false;
			sysTimer.Elapsed -= OnTimer;
			sysTimer.Stop();
		}

		// ===============================================
		// EVENTS
		public delegate void UISchedulerEvent();
		public event UISchedulerEvent OnUpdate;

		// ===============================================
		// CALLBACKS
		private void OnChange() {
			if( dataEntries == null || dataEntries.Count == 0 ) {
				dataNext = null;
				StopTimer();
				OnUpdate?.Invoke();
				return;
			}

			StartTimer();

			if( !bPaused ) {
				long current = DateTimeOffset.Now.ToUnixTimeSeconds();
				long timestamp = long.MaxValue;
				long postingSpacing = dataOptions.PostingSpacingMinutes * 60;
				for( int i = dataEntries.Count - 1; i > -1; i-- ) {
					ReddDataEntry entry = dataEntries[i];
					if( entry.IsValid && !entry.IsPosted ) {
						if( entry.Timestamp <= current && nLastPosting + postingSpacing <= current ) {
							dataEntries.Consume(entry);
							//todo: actually post
							nLastPosting = current;
						}
						else if( entry.Timestamp < timestamp ) {
							dataNext = entry;
							timestamp = entry.Timestamp;
						}
					}
				}

				dataEntries.Trim(ReddConfig.ENTRY_TRIMMING_DAYS_OLD);
			}

			OnUpdate?.Invoke();
		}

		private void OnTimer(object sender, ElapsedEventArgs e) {
			sysContext.Send(state => OnChange(), null);
		}
	}
}