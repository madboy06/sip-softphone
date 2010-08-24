using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Uccapi;

namespace Messenger.Controls
{
    /// <summary>
    /// Interaction logic for FileTransfersManagerControl.xaml
    /// </summary>
    public partial class FileTransfersManagerControl : UserControl
    {
        private Collection<FileTransferItemControl> innerItemControls = new Collection<FileTransferItemControl>();

        public static readonly DependencyProperty IncomingFilesProperty =
             DependencyProperty.Register("IncomingFiles", typeof(ObservableCollection<ITransferItem>),
             typeof(FileTransfersManagerControl), new FrameworkPropertyMetadata(null,
             FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnFilesChanged)));

        public ObservableCollection<ITransferItem> IncomingFiles
        {
            get { return (ObservableCollection<ITransferItem>)GetValue(IncomingFilesProperty); }
            set { SetValue(IncomingFilesProperty, value); }
        }

        public static readonly DependencyProperty OutgoingFilesProperty =
            DependencyProperty.Register("OutgoingFiles", typeof(ObservableCollection<ITransferItem>),
            typeof(FileTransfersManagerControl), new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnFilesChanged)));

        public ObservableCollection<ITransferItem> OutgoingFiles
        {
            get { return (ObservableCollection<ITransferItem>)GetValue(OutgoingFilesProperty); }
            set { SetValue(OutgoingFilesProperty, value); }
        }

        private static void OnFilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FileTransfersManagerControl managerControl = d as FileTransfersManagerControl;
            ObservableCollection<ITransferItem> transfers = e.NewValue as ObservableCollection<ITransferItem>;
            transfers.CollectionChanged += new NotifyCollectionChangedEventHandler(managerControl.LinkedTransfers_CollectionChanged);
        }

        public FileTransfersManagerControl()
        {
            InitializeComponent();
        }

        private void LinkedTransfers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ITransferItem newItem in e.NewItems)
                {
                    FileTransferItemControl newTransferItemControl = new FileTransferItemControl();
                    newTransferItemControl.LinkedItem = newItem;
                    innerItemControls.Add(newTransferItemControl);
                    scpManager.Children.Add(newTransferItemControl);
                }
            }
            if (e.OldItems != null)
            {
                foreach (ITransferItem deletedItem in e.OldItems)
                {
                    FileTransferItemControl deletedTransferItemControl =
                        (from f in innerItemControls where f.LinkedItem.TrId == deletedItem.TrId select f).SingleOrDefault();
                    scpManager.Children.Remove(deletedTransferItemControl);
                    innerItemControls.Remove(deletedTransferItemControl);
                }
            }
        }
    }
}
