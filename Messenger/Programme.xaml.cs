// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Messenger.Windows;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Uccapi;

namespace Messenger
{
	/// <summary>
	/// Interaction logic for Programme.xaml
    /// </summary>
	public partial class Programme : Application
    {
		private Mutex runningMutex;
		private bool isApplicationNotRunning;
		private string userId;
		public Chat chatWindow;
		private AvChatController avChatController;
		private PresentitiesCollectionStorage presentitiesStorage;
		
		public Programme()
		{
#if DEBUG
#else
			InitializeCrashHandler();
#endif
			userId = GetCommandLineArgValue(@"-userid");
			runningMutex = new Mutex(false, "Local\\" + "{9D13D6B0-F44E-4d48-BE80-07A49C0FD691}" + userId, out isApplicationNotRunning);

			if (string.IsNullOrEmpty(userId) == false)
				Messenger.Properties.Settings.ReloadSettings(userId);

			if (isApplicationNotRunning && Messenger.Properties.Settings.Default.NoSplash == false)
			{
				screen = new SplashScreen("SplashScreen.png");
				screen.Show(false);
			}

			presentitiesStorage = (PresentitiesCollectionStorage)(ApplicationSettingsBase.Synchronized(new PresentitiesCollectionStorage()));

			if (Messenger.Properties.Settings.Default.SettingsUpgrated == false)
			{
				try
				{
					presentitiesStorage.Upgrade();
					presentitiesStorage.Save();
	
					Messenger.Properties.Settings.Default.Upgrade();
					Messenger.Properties.Settings.Default.SettingsUpgrated = true;
					Messenger.Properties.Settings.Default.Save();
				}
				catch
				{
				}
			}

			if (Messenger.Properties.Settings.Default.EndpointId == 0)
			{
				Messenger.Properties.Settings.Default.EndpointId = Environment.TickCount;
				Messenger.Properties.Settings.Default.Save();
			}

			Messenger.Properties.Settings.Default.PropertyChanged += Settings_PropertyChanged;
			
			Messenger.Properties.Settings.Default.AutoSaveSettings = true;

			InitializeUccapi();
			InitializeCommandBindings();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			if (isApplicationNotRunning)
			{
				this.CloseSplash();

				new Contacts();
				this.MainWindow.Topmost = Messenger.Properties.Settings.Default.AlwaysOnTop;
				this.MainWindow.Closed += new EventHandler(MainWindow_Closed);
				this.MainWindow.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
				(this.MainWindow as Contacts).Presentities = Endpoint.Presentities; // Close command use "as Contacts" too
				this.MainWindow.Show();

				this.chatWindow = new Chat();
				this.chatWindow.Closing += new System.ComponentModel.CancelEventHandler(ChatWindow_Closing);
				this.Endpoint.Sessions.CollectionChanged += this.chatWindow.Sessions_CollectionChanged;

				this.avChatController = new AvChatController();
				this.Endpoint.Sessions.CollectionChanged += this.avChatController.Sessions_CollectionChanged;

				if(Messenger.Properties.Settings.Default.LoginAtStartup)
					Commands.Login.Execute(null, MainWindow);

				this.StartAutoAwayTimer();
			}
			else
			{
				MessageBox.Show(AssemblyInfo.AssemblyProduct + " is already running."
					+ "\r\n\r\nUse -userid<id> command line option if you need to start two or more instances of the application."
					+ "\r\nExample: Messenger.exe -useriduser2",
					AssemblyInfo.AssemblyProduct, MessageBoxButton.OK, MessageBoxImage.Exclamation);
				this.Shutdown();
			}
		}

		void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			CleanupUccapi(100, false);
		}

		void MainWindow_Closed(object sender, EventArgs e)
		{
			CleanupUccapi(1000, false);
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			CleanupUccapi(4000, true);

			runningMutex.Close();
		}

		void ChatWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.chatWindow.Hide();

			foreach (ISession session in new List<ISession>(this.Endpoint.Sessions))
			{
				if (session is IImSession)
					session.Destroy();
			}

			e.Cancel = true;
		}

		public static Programme Instance
        {
            get
            {
                return Programme.Current as Programme;
            }
        }

		public Contacts ContactsWindow
		{
			get
			{
				return this.MainWindow as Contacts;
			}
		}
	}
}
