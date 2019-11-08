using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Compression;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using Application = System.Windows.Application;

namespace MinecraftModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string MinecraftDir { get; set; }
        public List<Classes.Mod> LoadedMods { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            MinecraftDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft";
            RefreshAsync(MinecraftDir);
        }

        private void Menu_SelNewDirectory_Click(object sender, RoutedEventArgs e)
        {
            SelectNewDirectory();
        }

        private void SelectNewDirectory()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                try
                {
                    if (!Directory.GetDirectories(fbd.SelectedPath).Select(a => new DirectoryInfo(a).Name).Contains("mods"))
                    {
                        System.Windows.MessageBox.Show("Invalid Minecraft Directory" + Environment.NewLine + "Mods folder not found" + Environment.NewLine + "If this a real Minecraft directory then forge is likely not installed");
                    }
                    else
                    {
                        MinecraftDir = fbd.SelectedPath;
                        RefreshAsync(MinecraftDir);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Unexpected Error" + Environment.NewLine + e.Message);
                }
            }
        }

        private void Btn_ManualRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshAsync(MinecraftDir);
        }

        public async void RefreshAsync(string _minecraftDir)
        {
            if (Directory.Exists(MinecraftDir))
            {
                Btn_ManualRefresh.IsEnabled = false;
                Lbl_InstalledMods.Content = "Fetching Mods...";
                Prog_ProgBar.IsEnabled = true;
                Prog_ProgBar.Visibility = Visibility.Visible;
                {
                    try
                    {
                        string minecraftModsDir = _minecraftDir.ToString() + "\\mods";

                        Task<List<Classes.Mod>> mods = GetModListAsync(minecraftModsDir);
                        List<Classes.Mod> results = await mods;

                        LoadedMods = results;
                        Lst_ModList.ItemsSource = LoadedMods;
                        Lst_ModList.DataContext = LoadedMods;
                        Lbl_InstalledMods.Content = "Installed Mods: " + LoadedMods.Count;
                        DataContext = this;
                    }
                    catch (Exception a)
                    {
                        System.Windows.MessageBox.Show("There was a problem loading mods from the directory: " + _minecraftDir + Environment.NewLine + "Exception Details: " + Environment.NewLine + a.Message);
                    }
                }
                Btn_ManualRefresh.IsEnabled = true;
                Prog_ProgBar.IsEnabled = false;
                Prog_ProgBar.Visibility = Visibility.Hidden;
                GC.Collect(4);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Could not find Minecraft in the directory '" + MinecraftDir + "'");
            }
        }

        public async Task<List<Classes.Mod>> GetModListAsync(string minecraftModsDir)
        {
            return await Task.Run(() =>
            {
                List<Classes.Mod> mods = new List<Classes.Mod>();
                foreach (var item in Directory.GetFiles(minecraftModsDir))
                {
                    try
                    {
                        if (File.Exists("temp"))
                        {
                            File.Delete("temp");
                        }
                        using (var zipfile = ZipFile.Open(item, ZipArchiveMode.Read))
                        {
                            zipfile.Entries.Where(f => f.Name == "mcmod.info").First().ExtractToFile("temp");
                        }
                        Classes.Mod temp = Classes.Utilities.GetModFromJson(File.ReadAllText("temp"));
                        foreach (PropertyInfo propertyInfo in temp.GetType().GetProperties().Where(a => a.PropertyType == typeof(string)))
                        {
                            try
                            {
                                if (propertyInfo.GetValue(temp).ToString() == string.Empty)
                                {
                                    propertyInfo.SetValue(temp, null);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }

                        try
                        {
                            if (temp.logoFile != null)
                            {
                                System.Drawing.Image image = null;
                                using (var zipfile = ZipFile.Open(item, ZipArchiveMode.Read))
                                {
                                    image = System.Drawing.Image.FromStream(zipfile.Entries.Where(f => f.Name.Contains(System.IO.Path.GetFileName(temp.logoFile))).First().Open());
                                }
                                MemoryStream ms = new MemoryStream();
                                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                BitmapImage bImg = new BitmapImage();
                                bImg.BeginInit();
                                bImg.StreamSource = new MemoryStream(ms.ToArray());
                                bImg.EndInit();
                                bImg.Freeze();
                                Dispatcher.Invoke(() => temp.logo = bImg);
                            }
                            else
                            {
                                Dispatcher.Invoke(() => temp.logo = new BitmapImage(new Uri("/Assets/default.png", UriKind.Relative)));
                            }
                        }
                        catch (Exception)
                        {
                        }
                        mods.Add(temp);
                    }
                    catch (Exception)
                    {
                    }
                }
                return mods;
            });
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri("/Assets/default.png", UriKind.Relative));
        }

        private void Menu_OpenSelDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(MinecraftDir);
            }
            catch (Exception b)
            {
                System.Windows.Forms.MessageBox.Show("Could not open the selected minecraft directory" + Environment.NewLine + "Reason: " + b.Message);
            }
        }

        private void Menu_Settings_Click(object sender, RoutedEventArgs e)
        {
            Windows.Settings settingsWindow = new Windows.Settings();
            settingsWindow.ShowDialog();
        }

        private void Txt_SearchMods_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                Lst_ModList.ItemsSource = LoadedMods.Where(a => a.name.ToLower().Contains(Txt_SearchMods.Text.ToLower()));
            }
            catch (Exception) { };
        }

        private void Btn_ClearSeach_Click(object sender, RoutedEventArgs e)
        {
            Txt_SearchMods.Text = string.Empty;
            Lst_ModList.ItemsSource = LoadedMods;
        }

        private void Menu_Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void Menu_InstallNewMod_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog selectNewModDialog = new OpenFileDialog())
            {
                selectNewModDialog.Filter = "Jar Files(*.jar)|*.jar;|Zip Files(*.zip)|*.zip;";
                selectNewModDialog.Multiselect = false;
                if (selectNewModDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Windows.InstallModernMod installModernMod = new Windows.InstallModernMod(selectNewModDialog.FileName);
                    installModernMod.ShowDialog();
                }
            }
        }

        private void Menu_InstallClassicMod_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ModListItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                if (Frame_ModContent.Content != null)
                {
                    //((Pages.ModContent)Frame_ModContent.Content).Dispose();
                }
                Frame_ModContent.Content = new Pages.ModContent(Lst_ModList.SelectedItem as Classes.Mod);
            }
        }

        private void Menu_GetForge_Click(object sender, RoutedEventArgs e)
        {
            Windows.GetForge getForge = new Windows.GetForge();
            getForge.ShowDialog();
        }
    }
}



/*
Unimplemented Mod logo functionality
try
{
    System.Drawing.Image image = System.Drawing.Image.FromStream(ZipFile.Open(item, ZipArchiveMode.Read).Entries.Where(f => f.Name.Contains(".png")).First().Open());
    MemoryStream ms = new MemoryStream();
    image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
    BitmapImage bImg = new BitmapImage();
    bImg.BeginInit();
    bImg.StreamSource = new MemoryStream(ms.ToArray());
    bImg.EndInit();
    temp.logo.Source = bImg;
}
catch (Exception)
{
}
*/
