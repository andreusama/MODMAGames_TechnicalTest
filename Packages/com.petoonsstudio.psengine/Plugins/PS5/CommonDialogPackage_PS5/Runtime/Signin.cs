using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sony
{
	namespace PS5
	{
		namespace Dialog
		{
			public class Signin
			{
				public enum EnumSigninDialogResult
				{
					RESULT_OK,				// User selected either close button or Enter button
					RESULT_USER_CANCELED,	// User performed cancel operation.
				}
				
				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
				public struct SigninDialogResult
				{
					public EnumSigninDialogResult result;
				};
				
				// Initialisation.
				[DllImport("CommonDialog")]
				private static extern int PrxSigninDialogInitialise();
				
				// House keeping.
				[DllImport("CommonDialog")]
				private static extern void PrxSigninDialogUpdate();

				// Signin Dialog.
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxSigninDialogIsDialogOpen();
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxSigninDialogOpen(int userId);
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxSigninDialogGetResult(out SigninDialogResult result);

				// Messages.
				[DllImport("CommonDialog")] [return: MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogHasMessage();
				[DllImport("CommonDialog")] [return: MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogGetFirstMessage(out Dialog.Messages.PluginMessage msg);
				[DllImport("CommonDialog")] [return: MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogRemoveFirstMessage();
				
				// Event handlers.
				public static event Dialog.Messages.EventHandler OnGotSigninDialogResult;
				
				// Is the Signin dialog open?
				public static bool IsDialogOpen
				{
					get { return PrxSigninDialogIsDialogOpen(); }
				}

				public static bool Open(int userId)
				{
					bool ret = PrxSigninDialogOpen(userId);
					return ret;
				}
				
				public static SigninDialogResult GetResult()
				{
					SigninDialogResult result = new SigninDialogResult();
					PrxSigninDialogGetResult(out result);
					return result;
				}
				
				public static void ProcessMessage(Dialog.Messages.PluginMessage msg)
				{
					// Interpret the message and trigger corresponding events.
					switch (msg.type)
					{
						case Dialog.Messages.MessageType.kDialog_GotSigninDialogResult:
							if (OnGotSigninDialogResult != null) OnGotSigninDialogResult(msg);
						break;
					}
				}
				
				public static void Initialise()
				{
					PrxSigninDialogInitialise();
				}
				
				public static void Update()
				{
					PrxSigninDialogUpdate();
	//				PumpMessages();
				}
			}
		} // Dialog
	} // PS4
} // Sony
