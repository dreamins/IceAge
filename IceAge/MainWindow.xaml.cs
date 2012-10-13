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
            controller = new Controller();
            Resources.Add("UploadUnits", controller.Uploads);
            Resources.Add("Options", Options.Instance);
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
    }
}
