using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;
using Reddit;
using Reddit.Controllers;
using Reddit.Things;
using RedditImageScheduler.Data;
using RedditImageScheduler.IO;
using Subreddit = Reddit.Controllers.Subreddit;
using Timer = System.Timers.Timer;
using User = Reddit.Controllers.User;

namespace RedditImageScheduler {
	public class ReddScheduler {
		private readonly Timer sysTimer = new Timer(1000);
		private readonly StringBuilder sbState = new StringBuilder();
		private readonly ReddDataOptions dataOptions;
		
		private SynchronizationContext sysContext = SynchronizationContext.Current;
		private RedditClient redditClient;
		private User redditUser;
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
			
			try {
				redditClient = new RedditClient(dataOptions.AppId, dataOptions.RefreshToken, null, dataOptions.AccessToken);
				redditUser = redditClient.Account.Me;
			}
			catch( Exception ) {
				EventError?.Invoke();
				return;
			}

			if( string.IsNullOrEmpty(redditUser.Name) ) {
				EventError?.Invoke();
				return;
			}
			
			dataEntries = entries;
			dataEntries.OnUpdate += OnChange;

			OnChange();
		}

		public void Deinitialize() {
			if( dataEntries != null ) {
				dataEntries.OnUpdate -= OnChange;
				dataEntries = null;
			}

			redditClient = null;

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
		public event UISchedulerEvent EventUpdate;
		public event UISchedulerEvent EventError;

		// ===============================================
		// CALLBACKS
		private void OnChange() {
			if( dataEntries == null || dataEntries.Count == 0 ) {
				dataNext = null;
				StopTimer();
				EventUpdate?.Invoke();
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
							try {
								//post to user profile for now
								//TODO: implement proper subreddit selection
								List<Subreddit> matches = redditClient.SubredditAutocompleteV2("u_" + redditUser.Name, true, true, true, 1);
								if( matches.Count == 0 ) break;
								Subreddit subreddit = matches[0];
								FlairV2 flair = null;
								if( subreddit.Flairs.LinkFlairV2.Count > 0 ) {
									flair = subreddit.Flairs.LinkFlairV2[0];
								}
								LinkPost post = subreddit.LinkPost(entry.Title);
								post.Author = redditUser.Name;
								post.Title = entry.Title;
								post.Subreddit = "u_" + redditUser.Name; 
								post.URL = entry.Source;
								post.NSFW = false; //TODO: implement NSFW setting
								if( flair != null ) {
									post.SetFlair(flair.Text, flair.FlairType);
								}

								//post.Submit();
							}
							catch( Exception ) {
								EventError?.Invoke();
								return;
							}
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

			EventUpdate?.Invoke();
		}

		private void OnTimer(object sender, ElapsedEventArgs e) {
			sysContext.Send(state => OnChange(), null);
		}
	}
}