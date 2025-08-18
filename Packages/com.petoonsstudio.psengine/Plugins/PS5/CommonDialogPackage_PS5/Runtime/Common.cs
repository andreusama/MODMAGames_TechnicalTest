using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sony
{
	namespace PS5
	{
		namespace Dialog
		{
			public class Common
			{
				// System message dialog types, these must exactly match the values defined by SceMsgDialogSystemMessageType.
				public enum SystemMessageType
				{
					TRC_EMPTY_STORE = 0,
					CAMERA_NOT_CONNECTED = 4,
					PSN_COMMUNICATION_RESTRICTION = 6,
				}

				// User message dialog types, these must exactly match the values defined by SceMsgDialogButtonType.
				public enum UserMessageType
				{
					OK				= 0,
					Ok				= 0,
					YESNO			= 1,
					YesNo			= 1,
					NONE			= 2,
					None			= 2,				
					OK_CANCEL		= 3,
					OkCancel		= 3,
					WAIT			= 5,
					Wait			= 5,
					WAIT_CANCEL	    = 6,
					WaitCancel      = 6,
					YESNO_FOCUS_NO	= 7,
					YesNo_FocusNo   = 7,
					OK_CANCEL_FOCUS_CANCEL = 8,
					OkCancel_FocusCancel = 8,
					TWO_BUTTONS			= 9,
					TwoButtons      = 9
				}

				public enum CommonDialogResult
				{
					RESULT_BUTTON_NOT_SET,
					RESULT_BUTTON_OK,
					RESULT_BUTTON_CANCEL,
					RESULT_BUTTON_YES,
					RESULT_BUTTON_NO,
					RESULT_BUTTON_1,
					RESULT_BUTTON_2,
					RESULT_BUTTON_3,
					RESULT_CANCELED,
					RESULT_ABORTED,
					RESULT_CLOSED,
				}

				[DllImport("CommonDialog")]
				private static extern int PrxCommonDialogInitialise();
				[DllImport("CommonDialog")]
				private static extern void PrxCommonDialogUpdate();

				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogIsDialogOpen();

				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
                private static extern bool PrxCommonDialogErrorMessage(UInt32 errorCode, int userId = 0) ;

				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogSystemMessage(SystemMessageType type,  int userId);
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogClose();

                [DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
                private static extern bool PrxErrorDialogIsDialogOpen();
				
				[DllImport("CommonDialog", CharSet = CharSet.Ansi)] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogProgressBar(string str);
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogProgressBarSetPercent(int percent);
				[DllImport("CommonDialog", CharSet = CharSet.Ansi)] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogProgressBarSetMessage(string str);

				[DllImport("CommonDialog", CharSet = CharSet.Ansi)] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogUserMessage(UserMessageType type, bool infobar, string str, string button1, string button2, string button3);

				[DllImport("CommonDialog")]
				private static extern CommonDialogResult PrxCommonDialogGetResult();

				// Event handlers.
				public static event Dialog.Messages.EventHandler OnGotDialogResult;

				// Is a common dialog open?
				public static bool IsDialogOpen
				{
					get { return PrxCommonDialogIsDialogOpen(); }
				}

                // Is the error dialog open?
                public static bool IsErrorDialogOpen
                {
                    get { return PrxErrorDialogIsDialogOpen(); }
                }

				// Display an error message.
				public static bool ShowErrorMessage(UInt32 errorCode, int userId = 0)
				{
					return PrxCommonDialogErrorMessage(errorCode, userId);
				}

				// Display a system message.
				public static bool ShowSystemMessage(SystemMessageType type,  int userId)
				{
					return PrxCommonDialogSystemMessage(type, userId);
				}

				// Display a progress bar.
				public static bool ShowProgressBar(string message)
				{
					return PrxCommonDialogProgressBar(message);
				}

				// Set progress bar percentage (0-100).
				public static bool SetProgressBarPercent(int percent)
				{
					return PrxCommonDialogProgressBarSetPercent(percent);
				}

				// Set progress bar message string.
				public static bool SetProgressBarMessage(string message)
				{
					return PrxCommonDialogProgressBarSetMessage(message);
				}

				// Show a user message.
				public static bool ShowUserMessage(UserMessageType type, bool infoBar, string str, string button1="", string button2="")
				{
					return PrxCommonDialogUserMessage(type, infoBar, str, button1, button2, "");
				}


				
				// Close the dialog.
				public static bool Close()
				{
					return PrxCommonDialogClose();
				}

				// Get the result from the dialog that's just closed.
				public static CommonDialogResult GetResult()
				{
					CommonDialogResult result = PrxCommonDialogGetResult();
					return result;
				}

				// Process messages.
				public static void ProcessMessage(Dialog.Messages.PluginMessage msg)
				{
					// Interpret the message and trigger corresponding events.
					switch (msg.type)
					{
						case Dialog.Messages.MessageType.kDialog_GotDialogResult:
							if (OnGotDialogResult != null) OnGotDialogResult(msg);
							break;
					}
				}

				public static void Initialise()
				{
					PrxCommonDialogInitialise();
				}

				public static void Update()
				{
					PrxCommonDialogUpdate();
				}
			}
		} // Dialog
	} // PS4
} // Sony
