using System;
using System.IO;
using IniParser;
using IniParser.Model;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.IO {
	public class ReddIOOptions {
		private static readonly string PROP_DATABASE = nameof(DatabasePath);
		private static readonly string PROP_SPACING_HOURS = nameof(EntrySpacingHours);
		private static readonly string PROP_POSTING_MINUTES = nameof(PostingSpacingMinutes);
		private static readonly string PROP_TRIMMING_DAYS = nameof(TrimmingOldDays);
		
		private readonly IniDataParser iniParser = new IniDataParser();
		private readonly ReddDataOptions dataOptions;
		private readonly string sFilePath;
		
		private IniData iniData = new IniData();

		private string sDatabasePath = ReddConfig.FILE_DATABASE;
		private uint nEntrySpacingHours = ReddConfig.ENTRY_HOURS_SPACING;
		private uint nPostingSpacingMinutes = ReddConfig.ENTRY_POSTING_COOLDOWN_MINUTES;
		private uint nTrimmingOldDays = ReddConfig.ENTRY_TRIMMING_DAYS_OLD;

		public ReddIOOptions(string filepath) {
			sFilePath = filepath;
			dataOptions = new ReddDataOptions(this);
		}
		
		public ReddDataOptions Data => dataOptions;
		public string FilePath => sFilePath;

		public string DatabasePath {
			get => sDatabasePath;
			set {
				sDatabasePath = value;
				iniData[ReddConfig.SETTINGS_SECTION][PROP_DATABASE] = sDatabasePath;
				Save();
			}
		}

		public uint EntrySpacingHours {
			get => nEntrySpacingHours;
			set {
				nEntrySpacingHours = value;
				iniData[ReddConfig.SETTINGS_SECTION][PROP_SPACING_HOURS] = nEntrySpacingHours.ToString();
				Save();
			}
		}
		
		public uint PostingSpacingMinutes {
			get => nPostingSpacingMinutes;
			set {
				nPostingSpacingMinutes = value;
				iniData[ReddConfig.SETTINGS_SECTION][PROP_POSTING_MINUTES] = nPostingSpacingMinutes.ToString();
				Save();
			}
		}
		
		public uint TrimmingOldDays {
			get => nTrimmingOldDays;
			set {
				nTrimmingOldDays = value;
				iniData[ReddConfig.SETTINGS_SECTION][PROP_TRIMMING_DAYS] = nTrimmingOldDays.ToString();
				Save();
			}
		}
		
		public void Load() {
			try {
				if( File.Exists(sFilePath) ) {
					string ini = File.ReadAllText(sFilePath);
					iniData = iniParser.Parse(ini);
				}
				else {
					File.Create(sFilePath);
				}
				
				iniData.Sections.Add(ReddConfig.SETTINGS_SECTION);
				PropertyCollection section = iniData[ReddConfig.SETTINGS_SECTION];

				if( section.Contains(PROP_DATABASE) ) {
					sDatabasePath = section[PROP_DATABASE];
				}

				if( section.Contains(PROP_SPACING_HOURS) ) {
					uint.TryParse(section[PROP_SPACING_HOURS], out nEntrySpacingHours);
				}
				
				if( section.Contains(PROP_POSTING_MINUTES) ) {
					uint.TryParse(section[PROP_POSTING_MINUTES], out nPostingSpacingMinutes);
				}
				
				if( section.Contains(PROP_TRIMMING_DAYS) ) {
					uint.TryParse(section[PROP_TRIMMING_DAYS], out nTrimmingOldDays);
				}
			}
			catch( Exception ) {
				EventError?.Invoke();
			}
		}

		protected void Save() {
			try {
				File.WriteAllText(ReddConfig.FILE_SETTINGS, iniData.ToString());
			}
			catch( Exception ) {
				EventError?.Invoke();
			}
			
			EventUpdate?.Invoke();
		}

		// ===============================================
		// EVENTS
		public delegate void IOOptionsEvent();
		public event IOOptionsEvent EventError;
		public event IOOptionsEvent EventUpdate;
	}
}