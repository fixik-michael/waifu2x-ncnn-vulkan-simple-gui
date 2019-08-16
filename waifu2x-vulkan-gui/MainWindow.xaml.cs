using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace waifu2x_vulkan_gui
{
    public partial class MainWindow : Window
    {
        private String InputFile => textBoxFileInput.Text;

        private String OutputFile => textBoxFileOutput.Text;

        private String ModelPath => (comboBoxModel.SelectedItem as ComboBoxItem).Tag as String;

        private String ScaleRatio => (comboBoxScale.SelectedItem as ComboBoxItem).Tag as String;

        private String NoiseLevel => (comboBoxNoise.SelectedItem as ComboBoxItem).Tag as String;

        private String TileSize => textBoxTile.Text;

        private String GpuId => textBoxGpu.Text;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonInputFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image files|*.jpg; *.png|All files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                textBoxFileInput.Text = dlg.FileName;
            }
        }

        private void buttonOutputFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter = "Image files|*.png|All files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                textBoxFileOutput.Text = dlg.FileName;
            }
        }

        private void TextBoxNumberValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void buttonAppStart_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("waifu2x-ncnn-vulkan.exe"))
            {
                MessageBox.Show("waifu2x-ncnn-vulkan.exe not found", "Error");
                return;
            }

            var waifu2x = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = "waifu2x-ncnn-vulkan.exe",
                    Arguments = String.Format("-i \"{0}\" -o \"{1}\" -m {2} -n {3} -s {4} -t {5} -g {6}",
                                InputFile, OutputFile, ModelPath, NoiseLevel, ScaleRatio, TileSize, GpuId)
                }
            };

            try
            {
                waifu2x.Start();
                waifu2x.WaitForExit();
            }
            catch(Exception)
            {
                MessageBox.Show("Whoops, something went wrong...", "Error");
            }

            textBoxAppLog.Text = waifu2x.StandardError.ReadToEnd();
        }
    }
}
