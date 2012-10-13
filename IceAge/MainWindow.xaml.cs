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
using System.Windows.Navigation;
using System.Windows.Shapes;
using log4net;

using IceAge.type;

namespace IceAge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MainWindow).FullName);
        private Controller controller;

        public MainWindow()
        {
            InitializeComponent();
            if (!Options.isReady())
            {
                MainTabControl.SelectedItem = MainTabControl.FindName("OptionsTabItem");
            }
            controller = new Controller(Options.loadFromConfig(true));
            scrolledDataGrid.ItemsSource = controller.Uploads;
        }

        private void optionsSave_Click(object sender, RoutedEventArgs e)
        {
            // Get latest config
            logger.Debug("Trying to save config");
            Options opts = Options.loadFromConfig(true);
            try
            {
                // bind to UI elements (shall find some smartass reflection way to do so eventually, this just sucks)
                opts.GeneralOptions.SQLLitePath = SQLLitePath.Text;
                opts.GeneralOptions.BackupToS3 = backupToS3.IsChecked.Value;
                opts.GeneralOptions.SyncOnStart = syncOnStart.IsChecked.Value;
                opts.GeneralOptions.RelaxedResyncOnStart = fastResync.IsChecked.Value;
                opts.GeneralOptions.FullResyncOnStart = fullResync.IsChecked.Value;
                opts.GeneralOptions.MaxUploads = uint.Parse(maxUploads.Text);
                opts.GeneralOptions.MultipartEnabled = multipartEnabled.IsChecked.Value;
                opts.GeneralOptions.MultipartThresholdBytes = uint.Parse(multipartThresholdBytes.Text);

                // bind AWS options to UI elements
                opts.AWSOptions.AWSAccessKey = AWSAccessKey.Text;
                opts.AWSOptions.AWSSecretKey = AWSSecretKey.Text;
                opts.AWSOptions.GlacierVault = GlacierVaultName.Text;
                opts.AWSOptions.S3Bucket = S3BucketName.Text;
            }
            catch (FormatException ex)
            {
                logger.Error("Cannot write config", ex);
            }

            //save back
            opts.writeToConfig();
        }

        private void bindOptions()
        {
            logger.Debug("Trying to load config");
            Options opts = Options.loadFromConfig(true);

            // bind to UI elements
            SQLLitePath.Text = opts.GeneralOptions.SQLLitePath;
            backupToS3.IsChecked = opts.GeneralOptions.BackupToS3;
            syncOnStart.IsChecked = opts.GeneralOptions.SyncOnStart;
            fastResync.IsChecked = opts.GeneralOptions.RelaxedResyncOnStart;
            fullResync.IsChecked = opts.GeneralOptions.FullResyncOnStart;
            maxUploads.Text = opts.GeneralOptions.MaxUploads.ToString();
            multipartEnabled.IsChecked = opts.GeneralOptions.MultipartEnabled;
            multipartThresholdBytes.Text = opts.GeneralOptions.MultipartThresholdBytes.ToString();

            // bind AWS stuff to UI elements
            AWSAccessKey.Text = opts.AWSOptions.AWSAccessKey;
            AWSSecretKey.Text = opts.AWSOptions.AWSSecretKey;
            GlacierVaultName.Text = opts.AWSOptions.GlacierVault;
            S3BucketName.Text = opts.AWSOptions.S3Bucket;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OptionsTabItem.IsSelected)
            {
                bindOptions();
            }

            if (AWSOptionsTabItem.IsSelected)
            {
                bindOptions();
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.SelectedPath = "C:\\";

            System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
            if (!result.ToString().Equals("OK"))
            {
                return;
            }

            string path = folderDialog.SelectedPath;
            controller.addPath(path);
        }
    }
}
