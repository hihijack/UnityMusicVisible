using Ookii.Dialogs;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SFB
{
	public class StandaloneFileBrowserWindows : IStandaloneFileBrowser
	{
		[DllImport("user32.dll")]
		private static extern IntPtr GetActiveWindow();

		public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
		{
			VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
			vistaOpenFileDialog.Title = (title);
			if (extensions != null)
			{
				vistaOpenFileDialog.Filter = (StandaloneFileBrowserWindows.GetFilterFromFileExtensionList(extensions));
				vistaOpenFileDialog.FilterIndex = (1);
			}
			else
			{
				vistaOpenFileDialog.Filter = (string.Empty);
			}
			vistaOpenFileDialog.Multiselect = (multiselect);
			if (!string.IsNullOrEmpty(directory))
			{
				vistaOpenFileDialog.FileName = (StandaloneFileBrowserWindows.GetDirectoryPath(directory));
			}
			string[] arg_73_0 = (vistaOpenFileDialog.ShowDialog(new WindowWrapper(StandaloneFileBrowserWindows.GetActiveWindow())) == DialogResult.OK) ? vistaOpenFileDialog.FileNames : new string[0];
			vistaOpenFileDialog.Dispose();
			return arg_73_0;
		}

		public void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> cb)
		{
			cb(this.OpenFilePanel(title, directory, extensions, multiselect));
		}

		public string[] OpenFolderPanel(string title, string directory, bool multiselect)
		{
			VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
			vistaFolderBrowserDialog.Description = (title);
			if (!string.IsNullOrEmpty(directory))
			{
				vistaFolderBrowserDialog.SelectedPath = (StandaloneFileBrowserWindows.GetDirectoryPath(directory));
			}
			string[] arg_51_0;
			if (vistaFolderBrowserDialog.ShowDialog(new WindowWrapper(StandaloneFileBrowserWindows.GetActiveWindow())) != DialogResult.OK)
			{
				arg_51_0 = new string[0];
			}
			else
			{
				(arg_51_0 = new string[1])[0] = vistaFolderBrowserDialog.SelectedPath;
			}
			vistaFolderBrowserDialog.Dispose();
			return arg_51_0;
		}

		public void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb)
		{
			cb(this.OpenFolderPanel(title, directory, multiselect));
		}

		public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
		{
			VistaSaveFileDialog vistaSaveFileDialog = new VistaSaveFileDialog();
			vistaSaveFileDialog.Title = (title);
			string text = "";
			if (!string.IsNullOrEmpty(directory))
			{
				text = StandaloneFileBrowserWindows.GetDirectoryPath(directory);
			}
			if (!string.IsNullOrEmpty(defaultName))
			{
				text += defaultName;
			}
			vistaSaveFileDialog.FileName = (text);
			if (extensions != null)
			{
				vistaSaveFileDialog.Filter = (StandaloneFileBrowserWindows.GetFilterFromFileExtensionList(extensions));
				vistaSaveFileDialog.FilterIndex = (1);
				vistaSaveFileDialog.DefaultExt = (extensions[0].Extensions[0]);
				vistaSaveFileDialog.AddExtension = (true);
			}
			else
			{
				vistaSaveFileDialog.DefaultExt = (string.Empty);
				vistaSaveFileDialog.Filter = (string.Empty);
				vistaSaveFileDialog.AddExtension = (false);
			}
			string result = (vistaSaveFileDialog.ShowDialog(new WindowWrapper(StandaloneFileBrowserWindows.GetActiveWindow())) == DialogResult.OK) ? vistaSaveFileDialog.FileName : "";
			vistaSaveFileDialog.Dispose();
			return result;
		}

		public void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
		{
			cb(this.SaveFilePanel(title, directory, defaultName, extensions));
		}

		private static string GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
		{
			string text = "";
			for (int i = 0; i < extensions.Length; i++)
			{
				ExtensionFilter extensionFilter = extensions[i];
				text = text + extensionFilter.Name + "(";
				string[] extensions2 = extensionFilter.Extensions;
				for (int j = 0; j < extensions2.Length; j++)
				{
					string str = extensions2[j];
					text = text + "*." + str + ",";
				}
				text = text.Remove(text.Length - 1);
				text += ") |";
				extensions2 = extensionFilter.Extensions;
				for (int j = 0; j < extensions2.Length; j++)
				{
					string str2 = extensions2[j];
					text = text + "*." + str2 + "; ";
				}
				text += "|";
			}
			return text.Remove(text.Length - 1);
		}

		private static string GetDirectoryPath(string directory)
		{
			string text = Path.GetFullPath(directory);
			if (!text.EndsWith("\\"))
			{
				text += "\\";
			}
			if (Path.GetPathRoot(text) == text)
			{
				return directory;
			}
			return Path.GetDirectoryName(text) + Path.DirectorySeparatorChar.ToString();
		}
	}
}
