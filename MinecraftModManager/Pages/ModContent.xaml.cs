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
using MinecraftModManager.Classes;

namespace MinecraftModManager.Pages
{
    /// <summary>
    /// Interaction logic for ModContent.xaml
    /// </summary>
    public partial class ModContent : Page, IDisposable
    {
        protected Mod SelectedMod { get; set; }

        public ModContent(Mod mod)
        {
            SelectedMod = mod;
            InitializeComponent();
            DataContext = SelectedMod;
        }

        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            GC.Collect(4);
            this.Dispose();
        }
    }
}
