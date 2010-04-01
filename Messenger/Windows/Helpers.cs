// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;
using System.Windows.Controls;

namespace Messenger.Windows
{
	class Helpers
	{
		public static void ListGridView_UpdateColumnWidth(ListView listView, SizeChangedEventArgs e, int stretchColumn)
		{
			if (e.WidthChanged)
			{
				GridView gridView = listView.View as GridView;

				double total = 0;
				for (int i = 0; i < gridView.Columns.Count; i++)
					if (!Double.IsNaN(gridView.Columns[i].Width) && i != stretchColumn)
						total += gridView.Columns[i].Width;

				double width = listView.ActualWidth - total - 28;

				gridView.Columns[stretchColumn].Width = (width < 30) ? 30 : width;
			}
		}
	}
}
