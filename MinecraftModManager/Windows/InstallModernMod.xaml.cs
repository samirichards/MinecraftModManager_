using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MinecraftModManager.Windows
{
	/// <summary>
	/// Interaction logic for InstallModernMod.xaml
	/// </summary>
	public partial class InstallModernMod : Window
	{
		public string ModFilePath { get; set; }
		public InstallModernMod(string _modFilePath)
		{
			InitializeComponent();
			ModFilePath = _modFilePath;
            try
            {
                if (File.Exists("temp"))
                {
                    File.Delete("temp");
                }
                using (var zipfile = ZipFile.Open(_modFilePath, ZipArchiveMode.Read))
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
                        using (var zipfile = ZipFile.Open(_modFilePath, ZipArchiveMode.Read))
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
                this.DataContext = temp;
            }
            catch
            {
            }
        }
	}
}
