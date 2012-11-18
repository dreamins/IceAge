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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(unhandledExceptionHandler);
            controller = new Controller();
            Resources.Add("UploadUnits", controller.UploadUnitList);
            Resources.Add("Options", Options.Instance);
            controller.EstimationChanged += new Controller.EstimationChangedHandler(controller_EstimationChanged);
            controller.UploadDone += new Controller.UploadDoneHandler(controller_UploadDone);
            InitializeComponent();
            if (Options.Instance.Defaulted)
            {
                MainTabControl.SelectedItem = MainTabControl.FindName("OptionsTabItem");
            }
        }

        private void optionsSave_Click(object sender, RoutedEventArgs e)
        {
            // Get latest config
            logger.Debug("Trying to save config");
            //save back
            Options.Instance.writeToConfig();
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

        private void controller_EstimationChanged(object sender, Controller.EstimationChangedEventArgs args)
        {
            estimateMonthlyCost.Content = "$" + args.Estimations.MonthlyCost;
            estimateRequestsCost.Content = "$" + args.Estimations.RequestsCost;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            controller.clearPaths();
        }

        public void unhandledExceptionHandler(Object sender, UnhandledExceptionEventArgs e)
        {
            logger.Fatal("=====================================\n");
            logger.Fatal("Unhandled exception\n", (Exception)e.ExceptionObject);
            logger.Fatal("=====================================\n");
            MessageBox.Show("Fatal error occured, please file an issue with lines of log here https://github.com/dreamins/IceAge");
        }

        private void buttonSync_Click(object sender, RoutedEventArgs e)
        {
            buttonSync.IsEnabled = false;
            buttonUploadAndForget.IsEnabled = false;
            buttonClear.IsEnabled = false;

            controller.startSyncUpload();
        }

        private void buttonUploadAndForget_Click(object sender, RoutedEventArgs e)
        {

        }

        private void controller_UploadDone(object sender, Controller.UploadDoneEventArgs args)
        {
            // enable buttons back
            enableButton(buttonSync);
            enableButton(buttonUploadAndForget);
            enableButton(buttonClear);
        }

        private void enableButton(Button button)
        {
            button.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(delegate() { button.IsEnabled = true; }));
        }
    }
}
