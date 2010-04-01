// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Helpers;
using Uccapi;

namespace Messenger.Windows
{
	public class ChatTabItem
		: INotifyPropertyChanged
	{
		private MediaPlayer mediaPlayer;
		private DispatcherTimer restoreSessionTimer;
		private bool isRestoringSession;

		public ChatTabItem(IImSession session)
		{
			this.mediaPlayer = new MediaPlayer();

			this.restoreSessionTimer = new DispatcherTimer();
			this.restoreSessionTimer.Interval = new TimeSpan(0, 0, 1);
			this.restoreSessionTimer.Tick += RestoreSessionTimer_Tick;
			this.restoreSessionTimer.Start();

			IsRestoringSession = false;

			this.Session = session;
			this.Session.PartipantLogs.CollectionChanged += new NotifyCollectionChangedEventHandler(PartipantLogs_CollectionChanged);
			this.Session.SendResult += new EventHandler<ImSessionEventArgs1>(SendResult);
			this.Session.IncomingMessage += new EventHandler<ImSessionEventArgs2>(IncomingMessage);
		}

		private void PartipantLogs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (ParticipantLog log in e.NewItems)
					log.PropertyChanged += ParticipantLog_PropertyChanged;
			}

			if (e.OldItems != null && e.Action != NotifyCollectionChangedAction.Move)
			{
				foreach (ParticipantLog log in e.NewItems)
					log.PropertyChanged -= ParticipantLog_PropertyChanged;
			}

			this.OnPropertyChanged("HeaderText");
			this.OnPropertyChanged("HeaderAvailability");
		}

		void ParticipantLog_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == @"Availability")
				this.OnPropertyChanged("HeaderAvailability");
			if (e.PropertyName == @"DisplayNameOrAor")
				this.OnPropertyChanged("HeaderText");
		}

		public IImSession Session
		{
			get;
			private set;
		}

		public bool IsRestoringSession
		{
			get
			{
				return isRestoringSession;
			}
			private set
			{
				if (value != isRestoringSession)
				{
					isRestoringSession = value;
					OnPropertyChanged(@"IsRestoringSession");
				}
			}
		}

		public string HeaderText
		{
			get
			{
				if (this.Session.PartipantLogs.Count > 2)
					return @"Multi-user";
				foreach (ParticipantLog log in this.Session.PartipantLogs)
					if (log.IsLocal == false)
						return log.DisplayNameOrAor;
				return @"--";
			}
		}

		public AvailabilityValues HeaderAvailability
		{
			get
			{
				if (this.Session.PartipantLogs.Count == 2)
				{
					foreach (ParticipantLog log in this.Session.PartipantLogs)
						if (log.IsLocal == false)
							return log.Availability;
				}
				else if (this.Session.PartipantLogs.Count > 2)
				{
					if (this.Session.IsTerminated())
						return AvailabilityValues.Offline;
					else
						return AvailabilityValues.Online;
				}

				return AvailabilityValues.Unknown;
			}
		}

		public RichTextBoxEx.RichTextBoxEx ChatEdit { get; set; }
		public RichTextBoxEx.RichTextBoxEx MessageEdit { get; set; }

		private void PlaySound(string soundFile)
		{
			try
			{
				mediaPlayer.Stop();
				mediaPlayer.Open(new Uri(System.IO.Path.GetFullPath(soundFile)));
				mediaPlayer.Play();
			}
			catch
			{
			}
		}

		private void SendResult(Object sender, ImSessionEventArgs1 e)
		{
			if (string.IsNullOrEmpty(e.Message.Error) == false)
				InsertMessage(@"Failed: " + e.Message.Error, Brushes.Red);
			else
				PlaySound(Properties.Settings.Default.OutgoingMessageSound);
		}

		private void InsertMessage(string message, Brush foreground)
		{
			Paragraph paragraph = new Paragraph();
			paragraph.Inlines.Add(
				new Run(message)
				{
					FontWeight = FontWeights.Bold,
					Foreground = foreground,
				}
			);

			if (ChatEdit.Document.Blocks.Count == 1)
				ChatEdit.RemoveEmptyLastParagraph();
			ChatEdit.Document.Blocks.Add(paragraph);

			ChatEdit.ScrollToEnd();
		}

		private Paragraph InsertHeader(ImSessionEventArgs2 e)
		{
			return this.InsertHeader(
				this.Session.GetDisplayNameOrAor(e.Message.FromUri), e.Message.DateTime, GetColor(e.Message.FromUri));
		}

		private Paragraph InsertHeader(string name, DateTime time, Brush foreground)
		{
			Paragraph paragraph = new Paragraph()
			{
				Foreground = foreground,
				Margin = new Thickness(0, 5, 0, 0),
			};

			paragraph.Inlines.Add(new Run(name) { FontWeight = FontWeights.Bold, });
			paragraph.Inlines.Add(new Run(@" (" + time.ToShortTimeString() + @"):"));
			paragraph.Inlines.Add(new LineBreak());

			if (ChatEdit.Document.Blocks.Count == 1)
				ChatEdit.RemoveEmptyLastParagraph();
			ChatEdit.Document.Blocks.Add(paragraph);

			return paragraph;
		}
		
		private void InsertRichText(string rtf)
		{
			ChatEdit.AppendRtf(rtf);
			ChatEdit.RemoveEmptyLastParagraph();

			ChatEdit.ScrollToEnd();
		}
		
		private void IncomingMessage(Object sender, ImSessionEventArgs2 e)
		{
			if (e.Message.ContentType == MessageContentType.Plain)
			{
				this.InsertHeader(e).Inlines.Add(new Run(e.Message.Message));
			}
			else if (e.Message.ContentType == MessageContentType.Enriched)
			{
				this.InsertHeader(e);
				this.InsertRichText(e.Message.Message);
			}

			PlaySound(Properties.Settings.Default.IncomingMessageSound);
		}

		public void Execute_SendMessage(object sender, ExecutedRoutedEventArgs e)
		{
			SendMessage(sender, e);
		}

		public void SendMessage(object sender, RoutedEventArgs e)
		{
			if (this.Session.IsTerminated())
			{
				if (IsRestoringSession == false)
				{
					IsRestoringSession = true;
					Programme.Instance.Endpoint.RestoreSession(this.Session);
				}
			}
			else
			{
				var rtf = MessageEdit.InsertDefaultFontInfo(
							MessageEdit.CutRtf(new TextRange(
								MessageEdit.Document.ContentStart, 
								MessageEdit.Document.ContentEnd)));

				this.InsertHeader(this.Session.SelfPresentity.DisplayNameOrAor, DateTime.Now, GetColor(this.Session.SelfPresentity.Uri));
				this.InsertRichText(rtf);

				this.Session.Send(MessageContentType.Enriched, rtf);
			}
		}

		private void RestoreSessionTimer_Tick(object sender, EventArgs e)
		{
			if (IsRestoringSession)
			{
				if (Session.HasConnectingParticipants() == false)
				{
					IsRestoringSession = false;

					if (this.Session.IsTerminated())
						InsertMessage(@"Failed to restore instant messaging session.", Brushes.Red);
					else
						SendMessage(sender, null);
				}
			}
		}

		public void DestroySession(object sender, RoutedEventArgs e)
		{
			this.Session.Destroy();
		}

		private Brush GetColor(string uri)
		{
			#region var brushes = new Brush[] {...}

			var brushes = new Brush[] 
			{
				Brushes.DarkBlue,
				Brushes.DarkRed,
				Brushes.DarkOrange,
				Brushes.DarkCyan,
				Brushes.DarkGoldenrod,
				Brushes.DarkGray,
				Brushes.DarkGreen,
				Brushes.DarkKhaki,
				Brushes.DarkMagenta,
				Brushes.DarkOliveGreen,
				Brushes.DarkOrchid,
				Brushes.DarkSalmon,
				Brushes.DarkSeaGreen,
				Brushes.DarkSlateBlue,
				Brushes.DarkSlateGray,
				Brushes.DarkTurquoise,
				Brushes.DarkViolet,
			};

			#endregion

			var log = Session.FindParticipantLog(uri);
			if (log != null)
				return ParticipantIdConverter.GetBrush(log.Id);

			return Brushes.Black;
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(String property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		#endregion
	}
}
