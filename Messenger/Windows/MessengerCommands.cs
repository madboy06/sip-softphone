// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows.Input;

namespace Messenger.Windows
{
    public class MessengerCommands
    {
        public static RoutedUICommand AddContact { get; private set; }
        public static RoutedUICommand RemoveContact { get; private set; }
        public static RoutedUICommand FindContact { get; private set; }
        public static RoutedUICommand SetNewGroup { get; private set; }
        public static RoutedUICommand SendInstantMessage { get; private set; }
        public static RoutedUICommand SendFile { get; private set; }
        public static RoutedUICommand StartAudioConversation { get; private set; }
        public static RoutedUICommand StartVideoConversation { get; private set; }
        public static RoutedUICommand AlwaysOnTop { get; private set; }
        public static RoutedUICommand Close { get; private set; }
        public static RoutedUICommand ToggleShowOfflineContacts { get; private set; }
        public static RoutedUICommand ToggleShowGroups { get; private set; }
        public static RoutedUICommand ShowSessionDetails { get; private set; }
        public static RoutedUICommand UserProperties { get; private set; }
        public static RoutedUICommand GotoUrl { get; private set; }
        public static RoutedUICommand GotoEmail { get; private set; }
        public static RoutedUICommand CloseDialog { get; private set; }


        static MessengerCommands()
        {
            AddContact = new RoutedUICommand("Add Contact", "AddContact", typeof(MessengerCommands));
            RemoveContact = new RoutedUICommand("Remove Contact", "RemoveContact", typeof(MessengerCommands));
            FindContact = new RoutedUICommand("Find Contact", "FindContact", typeof(MessengerCommands));
            SetNewGroup = new RoutedUICommand("New Group", "SetNewGroup", typeof(MessengerCommands));
            SendInstantMessage = new RoutedUICommand("Send Instant Message", "SendInstantMessage", typeof(MessengerCommands));
            SendFile = new RoutedUICommand("Send File", "SendFile", typeof(MessengerCommands));
            StartAudioConversation = new RoutedUICommand("Start Audio Conversation", "StartAudioConversation", typeof(MessengerCommands));
            StartVideoConversation = new RoutedUICommand("Start Video Conversation", "StartVideoConversation", typeof(MessengerCommands));
            AlwaysOnTop = new RoutedUICommand("Always on Top", "AlwaysOnTop", typeof(MessengerCommands));
            Close = new RoutedUICommand("Close", "Close", typeof(MessengerCommands));
            ToggleShowOfflineContacts = new RoutedUICommand("Offline Contacts", "ToggleShowOfflineContacts", typeof(MessengerCommands));
            ToggleShowGroups = new RoutedUICommand("Groups", "ToggleShowGroups", typeof(MessengerCommands));
            ShowSessionDetails = new RoutedUICommand("Session Details", "ShowSessionDetails", typeof(MessengerCommands), CreateGestureCollection(Key.D, ModifierKeys.Control));
            UserProperties = new RoutedUICommand("User Properties", "UserProperties", typeof(MessengerCommands));
            GotoUrl = new RoutedUICommand("Go to Url", "GotoUrl", typeof(MessengerCommands));
            GotoEmail = new RoutedUICommand("Go to Email", "GotoEmail", typeof(MessengerCommands));
            CloseDialog = new RoutedUICommand("Cancel", "CloseDialog", typeof(MessengerCommands));
        }

        static private InputGestureCollection CreateGestureCollection(Key key, ModifierKeys modifiers)
        {
            var gestureCollection = new InputGestureCollection();
            gestureCollection.Add(new KeyGesture(key, modifiers));

            return gestureCollection;
        }
    }
}
