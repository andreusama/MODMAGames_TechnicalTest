using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sony
{
	namespace PS5
	{
		namespace Dialog
		{
			public class Messages
			{
				// Messages.
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogHasMessage();
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogGetFirstMessage(out PluginMessage msg);
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogRemoveFirstMessage();

				public enum MessageType
				{
					kDialog_NotSet,
					kDialog_Log,
					kDialog_LogWarning,
					kDialog_LogError,

					kDialog_GotDialogResult,		// Dialog has closed and the result is ready.
					kDialog_GotIMEDialogResult,		// IME Dialog has closed and the result is ready.
					kDialog_GotSigninDialogResult,	// Signin Dialog has closed and the result is ready.
					kDialog_GotWebBrowserDialogResult,
                    kDialog_GotVrSetupDialogResult //VrSetup Dialog has closed and the result is ready

				};

				// Event handler.
				public delegate void EventHandler(PluginMessage msg);

				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
				public struct PluginMessage
				{
					public MessageType type;
					public int dataSize;
					public IntPtr data;
					public string Text
					{
						get
						{
							switch (type)
							{
								case MessageType.kDialog_Log:
								case MessageType.kDialog_LogWarning:
								case MessageType.kDialog_LogError:
									return Marshal.PtrToStringAnsi(data);
							}
							return "no text";
						}
					}
					public int Int
					{
						get
						{
							switch (type)
							{

								case MessageType.kDialog_GotDialogResult:
									return (int)data;
								case MessageType.kDialog_GotIMEDialogResult:
									return (int)data;
								case MessageType.kDialog_GotSigninDialogResult:
									return (int)data;
								case MessageType.kDialog_GotWebBrowserDialogResult:
									return (int)data;
                                case MessageType.kDialog_GotVrSetupDialogResult:
                                    return (int)data;

							}
							return 0;
						}
					}
				};

				public static bool HasMessage()
				{
					return PrxCommonDialogHasMessage();
				}

				public static void RemoveFirstMessage()
				{
					PrxCommonDialogRemoveFirstMessage();
				}

				public static void GetFirstMessage(out PluginMessage msg)
				{
					PrxCommonDialogGetFirstMessage(out msg);
				}

			}
		} // Dialog
	} // PS5
} // Sony
