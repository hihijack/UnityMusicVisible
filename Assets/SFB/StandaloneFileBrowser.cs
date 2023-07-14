using System;

namespace SFB
{
	public class StandaloneFileBrowser
	{
		private static IStandaloneFileBrowser _platformWrapper;

		static StandaloneFileBrowser()
		{
			StandaloneFileBrowser._platformWrapper = new StandaloneFileBrowserWindows();
		}

		public static string[] OpenFilePanel(string title, string directory, string extension, bool multiselect)
		{
			ExtensionFilter[] arg_2C_0;
			if (!string.IsNullOrEmpty(extension))
			{
				(arg_2C_0 = new ExtensionFilter[1])[0] = new ExtensionFilter("", new string[]
				{
					extension
				});
			}
			else
			{
				arg_2C_0 = null;
			}
			ExtensionFilter[] extensions = arg_2C_0;
			return StandaloneFileBrowser.OpenFilePanel(title, directory, extensions, multiselect);
		}

		public static string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
		{
			return StandaloneFileBrowser._platformWrapper.OpenFilePanel(title, directory, extensions, multiselect);
		}

		public static void OpenFilePanelAsync(string title, string directory, string extension, bool multiselect, Action<string[]> cb)
		{
			ExtensionFilter[] arg_2C_0;
			if (!string.IsNullOrEmpty(extension))
			{
				(arg_2C_0 = new ExtensionFilter[1])[0] = new ExtensionFilter("", new string[]
				{
					extension
				});
			}
			else
			{
				arg_2C_0 = null;
			}
			ExtensionFilter[] extensions = arg_2C_0;
			StandaloneFileBrowser.OpenFilePanelAsync(title, directory, extensions, multiselect, cb);
		}

		public static void OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect, Action<string[]> cb)
		{
			StandaloneFileBrowser._platformWrapper.OpenFilePanelAsync(title, directory, extensions, multiselect, cb);
		}

		public static string[] OpenFolderPanel(string title, string directory, bool multiselect)
		{
			return StandaloneFileBrowser._platformWrapper.OpenFolderPanel(title, directory, multiselect);
		}

		public static void OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb)
		{
			StandaloneFileBrowser._platformWrapper.OpenFolderPanelAsync(title, directory, multiselect, cb);
		}

		public static string SaveFilePanel(string title, string directory, string defaultName, string extension)
		{
			ExtensionFilter[] arg_2C_0;
			if (!string.IsNullOrEmpty(extension))
			{
				(arg_2C_0 = new ExtensionFilter[1])[0] = new ExtensionFilter("", new string[]
				{
					extension
				});
			}
			else
			{
				arg_2C_0 = null;
			}
			ExtensionFilter[] extensions = arg_2C_0;
			return StandaloneFileBrowser.SaveFilePanel(title, directory, defaultName, extensions);
		}

		public static string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
		{
			return StandaloneFileBrowser._platformWrapper.SaveFilePanel(title, directory, defaultName, extensions);
		}

		public static void SaveFilePanelAsync(string title, string directory, string defaultName, string extension, Action<string> cb)
		{
			ExtensionFilter[] arg_2C_0;
			if (!string.IsNullOrEmpty(extension))
			{
				(arg_2C_0 = new ExtensionFilter[1])[0] = new ExtensionFilter("", new string[]
				{
					extension
				});
			}
			else
			{
				arg_2C_0 = null;
			}
			ExtensionFilter[] extensions = arg_2C_0;
			StandaloneFileBrowser.SaveFilePanelAsync(title, directory, defaultName, extensions, cb);
		}

		public static void SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions, Action<string> cb)
		{
			StandaloneFileBrowser._platformWrapper.SaveFilePanelAsync(title, directory, defaultName, extensions, cb);
		}
	}
}
