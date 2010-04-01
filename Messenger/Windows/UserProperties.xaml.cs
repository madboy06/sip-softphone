// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Uccapi;

namespace Messenger.Windows
{
	/// <summary>
	/// Interaction logic for UserProperties.xaml
	/// </summary>
	public partial class UserProperties : Window
	{
		public UserProperties()
		{
			InitializeComponent();

			this.CommandBindings.AddRange(Programme.Instance.CommandBindings);
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
