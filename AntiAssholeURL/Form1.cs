using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AntiAssholeURL
{
	public partial class Form1 : Form
	{
		[DllImport("user32.dll")]
		private static extern int SetClipboardViewer(int hWndNewViewer);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

		private IntPtr nextClipboardViewer;
		private static string last;
		private static string lastFucked;

		public Form1()
		{
			InitializeComponent();
			nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);

			Shown += Form1_Shown;
			notifyIcon1.BalloonTipClicked += NotifyIcon1_BalloonTipClicked;
		}

		private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
		{
			// Undo the unfuck
			last = lastFucked;
			Clipboard.SetText(lastFucked);
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			Hide();
		}

		protected override void WndProc(ref Message m)
		{
			// defined in winuser.h
			const int WM_DRAWCLIPBOARD = 0x308;
			const int WM_CHANGECBCHAIN = 0x030D;

			switch (m.Msg)
			{
				case WM_DRAWCLIPBOARD:
					if (Clipboard.ContainsText())
						HandleClipboardData(Clipboard.GetText());
					
					SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
					break;

				case WM_CHANGECBCHAIN:
					if (m.WParam == nextClipboardViewer) nextClipboardViewer = m.LParam;
					else SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);

					break;

				default:
					base.WndProc(ref m);
					break;
			}

		}

		private void HandleClipboardData(string v)
		{
			// Don't process the last URL again infinitely
			if (v == last)
				return;


			if (!Uri.TryCreate(v, UriKind.Absolute, out Uri fucked))
				return;

			if (fucked.Scheme != "https" && fucked.Scheme != "http")
				return;

			// Do this after confirming that it's probably a URL to prevent accidentally holding
			// like 60mb of text in ram for no reason (not that anyone copies 60mb of text)
			lastFucked = v;

			Uri unfucked = null;

			foreach(UrlUnfuckerProfile profile in Program.UnfuckerProfiles)
			{
				if (!profile.CanUnfuck(fucked))
					continue;

				try
				{
					unfucked = profile.Unfuck(fucked);
				}catch { } // TODO: deal with this error (w/ below error handling or some shit)

				break;
			}

			// TODO: if there was an error in unfucking notify user instead of silently fail
			// (don't forget that this also handles the case where there's no IUrlUnfucker that felt
			// like dealing with this asshurl
			if (unfucked == null)
				return;

			last = unfucked.ToString();

			Clipboard.SetText(unfucked.ToString());

			notifyIcon1.ShowBalloonTip(1000, "Asshurl destroyed", "click here to undo", ToolTipIcon.Info);
		}
	}
}
