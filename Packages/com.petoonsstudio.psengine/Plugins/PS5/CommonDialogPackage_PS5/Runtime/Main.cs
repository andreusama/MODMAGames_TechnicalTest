using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sony
{
	namespace PS5
	{
		namespace Dialog
		{
			public class Main
			{
				// Initialisation.
				[DllImport("CommonDialog")]
				private static extern int PrxCommonDialogInitialise();

				// House keeping.
				[DllImport("CommonDialog")]
				private static extern int PrxCommonDialogUpdate();

				public static event Messages.EventHandler OnLog;
				public static event Messages.EventHandler OnLogWarning;
				public static event Messages.EventHandler OnLogError;

				public static void Initialise()
				{
					PrxCommonDialogInitialise();
				}

				public static void Update()
				{
					PrxCommonDialogUpdate();
					PumpMessages();
				}

				static void PumpMessages()
				{
					while (Messages.HasMessage())
					{
						Messages.PluginMessage msg = new Messages.PluginMessage();
						Messages.GetFirstMessage(out msg);

						// Interpret the message and trigger corresponding events.

						Common.ProcessMessage(msg);
						Ime.ProcessMessage(msg);
						Signin.ProcessMessage(msg);
						WebBrowser.ProcessMessage(msg);
                        VrSetup.ProcessMessage(msg);

						switch (msg.type)
						{
							case Messages.MessageType.kDialog_Log:
								if (OnLog != null) OnLog(msg);
								break;

							case Messages.MessageType.kDialog_LogWarning:
								if (OnLogWarning != null) OnLogWarning(msg);
								break;

							case Messages.MessageType.kDialog_LogError:
								if (OnLogError != null) OnLogError(msg);
								break;
						}

						Messages.RemoveFirstMessage();
					}
				}
			}
		} // Dialog
	} // PS4
} // Sony
