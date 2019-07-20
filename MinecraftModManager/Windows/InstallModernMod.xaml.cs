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
		}
	}
}
