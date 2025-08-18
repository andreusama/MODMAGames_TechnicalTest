using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sony
{
	namespace PS5
	{
		namespace Dialog
		{
			public class WebBrowser
			{
				public enum EnumWebBrowserDialogResult
				{
					RESULT_OK,				// User selected either close button or Enter button
					RESULT_USER_CANCELED,	// User performed cancel operation.
				}

				public enum DialogMode
				{
					DEFAULT = 1,
					CUSTOM = 2
				}

				[Flags]
				public enum CustomParts : uint
				{
					NONE = 0,
					TITLE = 1,
					ADDRESS = 2,
					FOOTER = 4,
					BACKGROUND = 8,
					WAIT_DIALOG = 16
				}

				[Flags]
				public enum CustomControl : uint
				{
					NONE = 0,
					EXIT = 1,
					RELOAD = 2,
					BACK = 4,
					FORWARD = 8,
					ZOOM = 16,
					EXIT_UNTIL_COMPLETE = 32,
					OPTION_MENU = 64,
				}

				public enum Animation : uint
				{
					DEFAULT =0,
					DISABLE = 1
				}

				[Flags]
				public enum ImeOption : uint
				{
					DEFAULT =0,
					NO_AUTO_CAPITALIZATION =2,
					NO_LEARNING = 0x20,
					DISABLE_COPY_PASTE = 0x80,
					DISABLE_AUTO_SPACE = 0x200
				}

				[Flags]
				public enum WebViewOption : uint
				{
					NONE=0,
					BACKGROUND_TRANSPARENCY=1,
					CURSOR_NONE=2,
					OSK_NO_SUBMIT=4
				}

				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
				public class WebBrowserParam
				{
					public DialogMode mode;
					public int userId;
					public string url;
					public ushort width, height;
					public ushort positionX, positionY;
					public CustomParts parts;
					public ushort headerWidth, headerPositionX, headerPositionY;
					public CustomControl control;

					public ImeOption imeOption;
					public WebViewOption webViewOption;

					public Animation animation;

					public WebBrowserParam()
					{
						mode = DialogMode.DEFAULT;
					}
				}

				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
				public class WebBrowserPredeterminedContentParam
				{
					public string [] domain;
					public WebBrowserPredeterminedContentParam()
					{
						domain = new string[5];    //only supporting 5 ... not all SCE_WEB_BROWSER_DIALOG_DOMAIN_COUNT
					}
				}

				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
				public struct WebBrowserDialogResult
				{
					public EnumWebBrowserDialogResult result;
				};

				[DllImport("CommonDialog")]
				private static extern int PrxWebBrowserDialogTerminate();

				// House keeping.


				// Signin Dialog.
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxWebBrowserDialogIsDialogOpen();
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxWebBrowserDialogOpen(WebBrowserParam webBrowserParam);
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]

				private static extern bool PrxWebBrowserDialogOpenForPredeterminedContent(WebBrowserParam webBrowserParam, string domain0, string domain1, string domain2, string domain3, string domain4 );
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]



				private static extern int PrxWebBrowserDialogResetCookie();
				[DllImport("CommonDialog")]

				private static extern int PrxWebBrowserDialogSetCookie(string url, string cookie);
				[DllImport("CommonDialog")]

				private static extern bool PrxWebBrowserDialogGetResult(out WebBrowserDialogResult result);

				// Messages.
				[DllImport("CommonDialog")] [return: MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogHasMessage();
				[DllImport("CommonDialog")] [return: MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogGetFirstMessage(out Dialog.Messages.PluginMessage msg);
				[DllImport("CommonDialog")] [return: MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogRemoveFirstMessage();

				// Event handlers.
				public static event Dialog.Messages.EventHandler OnGotWebBrowserDialogResult;

				// Is the Signin dialog open?
				public static bool IsDialogOpen
				{
					get { return PrxWebBrowserDialogIsDialogOpen(); }
				}

				public static bool Open(WebBrowserParam webBrowserParam)
				{
					bool ret = PrxWebBrowserDialogOpen(webBrowserParam);
					return ret;
				}
				public static bool OpenForPredeterminedContent(WebBrowserParam webBrowserParam, WebBrowserPredeterminedContentParam param2)
				{
					bool ret = PrxWebBrowserDialogOpenForPredeterminedContent(webBrowserParam, param2.domain[0], param2.domain[1], param2.domain[2], param2.domain[3], param2.domain[4] );
					return ret;
				}

				public static int ResetCookie()
				{
					return PrxWebBrowserDialogResetCookie();
				}

				public static int SetCookie(string url, string cookie)
				{
					return PrxWebBrowserDialogSetCookie(url, cookie);
				}


				public static WebBrowserDialogResult GetResult()
				{
					WebBrowserDialogResult result = new WebBrowserDialogResult();
					PrxWebBrowserDialogGetResult(out result);
					return result;
				}

				public static void ProcessMessage(Dialog.Messages.PluginMessage msg)
				{
					// Interpret the message and trigger corresponding events.
					switch (msg.type)
					{
						 case Dialog.Messages.MessageType.kDialog_GotWebBrowserDialogResult:
						 	if (OnGotWebBrowserDialogResult != null) OnGotWebBrowserDialogResult(msg);
						break;
					}
				}

				public static void Terminate()
				{
					PrxWebBrowserDialogTerminate();
				}
			}
		} // Dialog
	} // PS5
} // Sony
