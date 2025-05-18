using System;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;
using RedditImageScheduler.Data;
using RedditImageScheduler.Utils.Data;

namespace RedditImageScheduler.IO {
	public class ReddIOOptions {
		private static readonly string PROP_DATABASE = nameof(DatabasePath);
		private static readonly string PROP_APP_ID = nameof(AppId);
		private static readonly string PROP_APP_SECRET = nameof(AppSecret);
		private static readonly string PROP_SPACING_HOURS = nameof(EntrySpacingHours);
		private static readonly string PROP_POSTING_MINUTES = nameof(PostingSpacingMinutes);
		private static readonly string PROP_TRIMMING_DAYS = nameof(TrimmingOldDays);
		
		
		private readonly IniDataParser iniParser = new IniDataParser();
		private readonly ReddDataOptions dataOptions;
		private readonly string sFilePath;

		private readonly StringBuilder sbIni = new StringBuilder();
		private IniData iniData = new IniData();
		
		private readonly ReddClampedValue<uint> reddEntrySpacingHours =
			new ReddClampedValue<uint>(ReddConfig.OPTION_ENTRY_SPACING_HOURS, ReddConfig.ENTRY_SPACING_HOURS); 
		private readonly ReddClampedValue<uint> reddPostingSpacingMinutes =
			new ReddClampedValue<uint>(ReddConfig.OPTION_POSTING_SPACING_MINUTES, ReddConfig.ENTRY_POSTING_COOLDOWN_MINUTES);
		private readonly ReddClampedValue<uint> reddTrimmingOldDays =
			new ReddClampedValue<uint>(ReddConfig.OPTION_OLD_TRIMMING_DAYS, ReddConfig.ENTRY_TRIMMING_DAYS_OLD);
		private readonly ReddFilepath reddFilepath = new ReddFilepath(ReddConfig.FILE_DATABASE);

		private string sAppId;
		private string sAppSecret;

		public ReddIOOptions(string filepath) {
			sFilePath = filepath;
			dataOptions = new ReddDataOptions(this);
		}
		
		public ReddDataOptions Data => dataOptions;
		public string FilePath => sFilePath;
		
		public string AppId => sAppId;
		public string AppSecret => sAppSecret;
		public string DatabasePath => reddFilepath.Filepath;
		public uint EntrySpacingHours => reddEntrySpacingHours.Value;
		public uint PostingSpacingMinutes => reddPostingSpacingMinutes.Value;
		public uint TrimmingOldDays => reddTrimmingOldDays.Value;

		public void SetApp(string app_id, string app_secret) {
			sAppId = app_id;
			iniData[ReddConfig.SETTINGS_SECTION][PROP_APP_ID] = sAppId;
			sAppSecret = app_secret;
			iniData[ReddConfig.SETTINGS_SECTION][PROP_APP_SECRET] = sAppSecret;
			Save();
		}

		public void SetDatabase(string database_path) {
			reddFilepath.Filepath = database_path;
			iniData[ReddConfig.SETTINGS_SECTION][PROP_DATABASE] = reddFilepath.Filepath;
			Save();
		}

		public void SetOptions(uint entry_spacing, uint posting_spacing, uint trimming_old_days) {
			reddEntrySpacingHours.Value = entry_spacing;
			iniData[ReddConfig.SETTINGS_SECTION][PROP_SPACING_HOURS] = reddEntrySpacingHours.Value.ToString();
			reddPostingSpacingMinutes.Value = posting_spacing;
			iniData[ReddConfig.SETTINGS_SECTION][PROP_POSTING_MINUTES] = reddPostingSpacingMinutes.Value.ToString();
			reddTrimmingOldDays.Value = trimming_old_days;
			iniData[ReddConfig.SETTINGS_SECTION][PROP_TRIMMING_DAYS] = reddTrimmingOldDays.Value.ToString();
			Save();
		}
		
		public void Load() {
			try {
				if( File.Exists(sFilePath) ) {
					string ini = File.ReadAllText(sFilePath);
					try {
						iniData = iniParser.Parse(ini);
					}
					catch(Exception) {
						iniData = new IniData();
					}
				}
				else {
					File.Create(sFilePath);
				}
				
				iniData.Sections.Add(ReddConfig.SETTINGS_SECTION);
				PropertyCollection section = iniData[ReddConfig.SETTINGS_SECTION];
				
				if( section.Contains(PROP_APP_ID)) {
					sAppId = section[PROP_APP_ID];
				}

				if( section.Contains(PROP_APP_SECRET) ) {
					sAppSecret = section[PROP_APP_SECRET];
				}

				if( section.Contains(PROP_DATABASE) ) {
					reddFilepath.Filepath = section[PROP_DATABASE];
				}

				if( section.Contains(PROP_SPACING_HOURS) ) {
					uint.TryParse(section[PROP_SPACING_HOURS], out var entry_spacing);
					reddEntrySpacingHours.Value = entry_spacing;
				}
				
				if( section.Contains(PROP_POSTING_MINUTES) ) {
					uint.TryParse(section[PROP_POSTING_MINUTES], out var posting_spacing);
					reddPostingSpacingMinutes.Value = posting_spacing;
				}
				
				if( section.Contains(PROP_TRIMMING_DAYS) ) {
					uint.TryParse(section[PROP_TRIMMING_DAYS], out var trimming_old);
					reddTrimmingOldDays.Value = trimming_old;
				}
			}
			catch( Exception ) {
				EventError?.Invoke();
			}
		}

		protected void Save() {
			try {
				sbIni.Clear();
				foreach( var section in iniData.Sections ) {
					sbIni.Append('[').Append(section.Name).Append(']').AppendLine();
					foreach( var prop in section.Properties ) {
						sbIni.Append(prop.Key).Append('=').Append(prop.Value).AppendLine();
					}
				}
				File.WriteAllText(ReddConfig.FILE_SETTINGS, sbIni.ToString());
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