using System;
using System.Windows.Forms;

namespace SFB
{
	public class WindowWrapper : IWin32Window
	{
		private IntPtr _hwnd;

		public IntPtr Handle
		{
			get
			{
				return this._hwnd;
			}
		}

		public WindowWrapper(IntPtr handle)
		{
			this._hwnd = handle;
		}
	}
}
