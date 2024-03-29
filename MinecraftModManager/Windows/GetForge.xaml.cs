﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MinecraftModManager.Windows
{
	/// <summary>
	/// Interaction logic for GetForge.xaml
	/// </summary>
	public partial class GetForge : Window
	{
		public GetForge()
		{
			InitializeComponent();
			RefreshAsync();
		}

		public async void RefreshAsync()
		{
			await Task.Run(() =>
			{
				Dispatcher.Invoke(() => 
				{
                    try
                    {
                        Regex regex = new Regex("^\\d+(\\.\\d+)*$");
                        Lst_InstalledVersions.ItemsSource = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.minecraft\\versions").Where(a => regex.IsMatch(System.IO.Path.GetFileName(a))).Select(b => System.IO.Path.GetFileName(b)).OrderBy(a => double.Parse(a.Substring(a.IndexOf(".") + 1, a.Length - (a.IndexOf(".") + 1))));
                    }
                    catch (Exception c)
                    {
                        System.Windows.Forms.MessageBox.Show("Could not get installed minecraft versions" + Environment.NewLine + "Reason: " + c.Message);
                        this.Close();
                    }	
				});
			});
		}

		private void Btn_GetForge_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start("https://files.minecraftforge.net/maven/net/minecraftforge/forge/index_" + Lst_InstalledVersions.SelectedItem.ToString() + ".html");
				DialogResult = true;
			}
			catch (Exception)
			{
				Process.Start("https://files.minecraftforge.net/");
				DialogResult = true;
			}
		}

		private void Lst_InstalledVersions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				Process.Start("https://files.minecraftforge.net/maven/net/minecraftforge/forge/index_" + Lst_InstalledVersions.SelectedItem.ToString() + ".html");
				DialogResult = true;
			}
			catch (Exception)
			{
				Process.Start("https://files.minecraftforge.net/");
				DialogResult = true;
			}
		}
	}
}