// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System.Timers;
using System.ComponentModel;

namespace Messenger.Properties
{
	internal sealed partial class Settings
	{
		public Settings()
		{
		}

		private Settings(string settingsKey)
			: base(settingsKey)
		{
		}

		private Timer timer;
		private int count;

		public bool AutoSaveSettings
		{
			get { return timer != null; }
			set
			{
				if (AutoSaveSettings != value)
				{
					if (value)
					{
						timer = new Timer();
						timer.AutoReset = true;
						timer.Interval = 100;
						timer.Elapsed += Timer_Elapsed;
						timer.Start();

						PropertyChanged += Settings_PropertyChanged;
					}
					else
					{
						PropertyChanged -= Settings_PropertyChanged;

						timer.Stop();
						timer.Elapsed -= Timer_Elapsed;
						timer.Dispose();
						timer = null;
					}
				}
			}
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			count = 3;
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (count > 0)
			{
				if (--count == 0)
					Save();
			}
		}

		public static void ReloadSettings(string settingsKey)
		{
			defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings(settingsKey))));
		}
	}
}
