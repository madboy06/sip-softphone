using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Uccapi;

namespace Messenger.Controls
{
    /// <summary>
    /// Interaction logic for FileTransferItem.xaml
    /// </summary>
    public partial class FileTransferItemControl : UserControl
    {
        private ITransferItem linkedItem;
        public ITransferItem LinkedItem
        {
            get { return linkedItem; }
            set
            {
                linkedItem = value;
                linkedItem.PropertyChanged += new PropertyChangedEventHandler(OnLinkedItemPropertyChanged);
                lblFilePath.Content = LinkedItem.FilePath;
            }
        }

        public FileTransferItemControl()
        {
            InitializeComponent();
        }

        private void OnLinkedItemPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProcessedDataPercent")
                pgbProgress.SetValue(ProgressBar.ValueProperty, LinkedItem.TransferedPercents);
            else if (e.PropertyName == "FilePath")
                lblFilePath.Content = LinkedItem.FilePath;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
			throw new NotImplementedException();
      //      MessageBoxResult result = MessageBox.Show("Do you really want to stop the transfer?", "Transfer stopping", MessageBoxButton.YesNoCancel);
        //    if (result == MessageBoxResult.Yes)
          //      LinkedItem.Session.TransfersManager.Stop(LinkedItem);
        }
    }
}
