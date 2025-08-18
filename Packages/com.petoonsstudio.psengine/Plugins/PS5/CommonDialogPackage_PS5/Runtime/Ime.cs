using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sony
{
	namespace PS5
	{
		namespace Dialog
		{
			public class Ime
			{
				// ImeParam enterLabel
				public enum EnterLabel
				{
					DEFAULT,
					SEND,
					LABEL_SEARCH,
					LABEL_GO,
				}

				public enum InputMethod
				{
					DEFAULT,
				}

				// ImeParam type
				public enum Type
				{
					DEFAULT,		    // UI for regular text input
					BASIC_LATIN,	    // UI for alphanumeric character input
					URL,	        	// UI for entering URL
					MAIL,		        // UI for entering an email address
					NUMBER,		    // UI for number input
				}
				/*
				 ** IME horizontal display origins
				 */
				public enum HorizontalAlignment
				{
					LEFT	,				//<E Left edge of OSK
					CENTER,				//<E Horizontal center of OSK
					RIGHT				//<E Right edge of OSK
				}

				/*
				 ** IME vertical display origins
				 */
				public enum VerticalAlignment
				{
					TOP,					//<E Top edge of OSK
					ENTER,				//<E Vertical center of OSK
					BOTTOM,				//<E Bottom edge of OSK
				}

				/*
				 ** IME prioritized input panel
				 */
				public enum PanelPriority
				{
					DEFAULT = 0,	//<E Display the standard panel
					ALPHABET = 1,	//<E Display the English alphabet panel
					SYMBOL = 2,	//<E Display the symbol panel
					ACCENT = 3,	//<E Display the accent panel
				};

				
				
				// ImeParam supported languages, can be OR'd together.
				[Flags]
				public enum FlagsSupportedLanguages : long
				{
					LANGUAGE_DANISH = 0x00000001,
					LANGUAGE_GERMAN = 0x00000002,
					LANGUAGE_ENGLISH_US = 0x00000004,
					LANGUAGE_SPANISH = 0x00000008,
					LANGUAGE_FRENCH = 0x00000010,
					LANGUAGE_ITALIAN = 0x00000020,
					LANGUAGE_DUTCH = 0x00000040,
					LANGUAGE_NORWEGIAN = 0x00000080,
					LANGUAGE_POLISH = 0x00000100,
					LANGUAGE_PORTUGUESE_PT = 0x00000200,
					LANGUAGE_RUSSIAN = 0x00000400,
					LANGUAGE_FINNISH = 0x00000800,
					LANGUAGE_SWEDISH = 0x00001000,
					LANGUAGE_JAPANESE = 0x00002000,
					LANGUAGE_KOREAN = 0x00004000,
					LANGUAGE_SIMPLIFIED_CHINESE = 0x00008000,
					LANGUAGE_TRADITIONAL_CHINESE = 0x00010000,
					LANGUAGE_PORTUGUESE_BR = 0x00020000,
					LANGUAGE_ENGLISH_GB = 0x00040000,
					LANGUAGE_TURKISH = 0x00080000,
					LANGUAGE_SPANISH_LA = 0x00100000,
				}



				[Flags]
				public enum Option
				{
					DEFAULT					= 0x00000000,		//<E No specification (default)
					MULTILINE				= 0x00000001,		//<E Multiline input mode
					NO_AUTO_CAPITALIZATION	= 0x00000002,		//<E Prohibit auto capitalization
					PASSWORD					= 0x00000004,		//<E Password mode
					LANGUAGES_FORCED			= 0x00000008,		//<E Force the specified languages to be available
					EXT_KEYBOARD				= 0x00000010,		//<E When external keyboard is connected, give precedence to it
					NO_LEARNING				= 0x00000020,		//<E Do not add strings to the input support dictionary
					FIXED_POSITION			= 0x00000040,		//<E Prohibit movement of IME panel by analog stick and fix the display position
					DISABLE_COPY_PASTE		= 0x00000080,		//<E Prohibit copy/paste
					DISABLE_RESUME			= 0x00000100,		//<E Disable automatic resume
					DISABLE_AUTO_SPACE		= 0x00000200,		//<E Disable input automatic space after word.
				}


				[Flags]
				public enum ExtOption
				{
					DEFAULT				= 0x00000000,		//<E No specification (default)
					SET_COLOR			= 0x00000001,		//<E Enable color settings
					SET_PRIORITY			= 0x00000002,		//<E Enable the display panel priority setting
					PRIORITY_SHIFT		= 0x00000004,		//<E Shift option
					PRIORITY_FULL_WIDTH	= 0x00000008,		//<E Full-width option
					PRIORITY_FIXED_PANEL	= 0x00000010,		//<E Fixed panel option
					DISABLE_POINTER		= 0x00000040,      //<E Disable gyro pointer mode
					ENABLE_ADDITIONAL_DICTIONARY	= 0x00000080,	//<E Enable dictionary file provided by the application
					DISABLE_STARTUP_SE	= 0x00000100,		//<E Disable startup SE
					DISABLE_LIST_FOR_EXT_KEYBOARD	= 0x00000200,		//<E
					HIDE_KEYPANEL_IF_EXT_KEYBOARD = 0x00000400,	//<E
					INIT_EXT_KEYBOARD_MODE	= 0x00000800	//<E
				}


				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
				public class SceImeDialogParam
				{
					public uint userId;
					public Type type;
					public FlagsSupportedLanguages supportedLanguages;
					public EnterLabel enterLabel;
					public InputMethod inputMethod;
					public IntPtr _filter;		// TODO: callback for filtering text
					public Option option;
					public uint maxTextLength;
					IntPtr _inputTextBuffer;
					public float posx;
					public float posy;
					public HorizontalAlignment horizontalAlignment;
					public VerticalAlignment verticalAlignment;
					IntPtr _placeholder;
					IntPtr _title;
					Byte r0, r1, r2, r3, r4, r5, r6, r7, r8, r9, r10, r11, r12, r13, r14, r15;  //int8_t reserved[16];
					
					public string title
					{
						get { return Marshal.PtrToStringUni(_title); }
						set { _title = Marshal.StringToCoTaskMemUni(value); }
					}
					public string inputTextBuffer
					{
						get { return Marshal.PtrToStringUni(_inputTextBuffer); }
						set { _inputTextBuffer = Marshal.StringToCoTaskMemUni(value); }
					}
					public string placeholder
					{
						get { return Marshal.PtrToStringUni(_placeholder); }
						set { _placeholder = Marshal.StringToCoTaskMemUni(value); }
					}

					public SceImeDialogParam()
					{
						userId = 0;
						type = Type.DEFAULT;
						supportedLanguages = 0;
						enterLabel = EnterLabel.DEFAULT;
						inputMethod = InputMethod.DEFAULT;
						_filter = IntPtr.Zero;
						option = 0;
						maxTextLength = 0;
						_inputTextBuffer = IntPtr.Zero;
						posx = 0.0f;
						posy = 0.0f;
						horizontalAlignment = HorizontalAlignment.LEFT;
						verticalAlignment = VerticalAlignment.TOP;
						_placeholder = IntPtr.Zero;
						_title = IntPtr.Zero;
						r0 = r1 = r2 = r3 = r4 = r5 = r6 = r7 = r8 = r9 = r10 = r11 = r12 = r13 = r14 = r15 = 0;
					}

					~SceImeDialogParam()
					{
						Marshal.FreeCoTaskMem(_title);
						Marshal.FreeCoTaskMem(_inputTextBuffer);
						Marshal.FreeCoTaskMem(_placeholder);
					}
				};

				public struct SceImeColor
				{
					public byte r, g, b, a;
				};

				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
				public class SceImeParamExtended
				{
					public ExtOption option;
					public SceImeColor colorBase;
					public SceImeColor colorLine;
					public SceImeColor colorTextField;
					public SceImeColor colorPreedit;
					public SceImeColor colorButtonDefault;
					public SceImeColor colorButtonFunction;
					public SceImeColor colorButtonSymbol;
					public SceImeColor colorText;
					public SceImeColor colorSpecial;
					public PanelPriority priority;
					uint padding;	// padding to get 64bit pointer aligned correctly
					IntPtr _additionalDictionaryPath;
					public IntPtr _extKeyboardFilter;	// todo keyboard filter callback
					public uint disableDevice;
					public uint extKeyboardMode;

					public string additionalDictionaryPath
					{
						get { return Marshal.PtrToStringUni(_additionalDictionaryPath); }
						set { _additionalDictionaryPath = Marshal.StringToCoTaskMemUni(value); }
					}

					public SceImeParamExtended()
					{
						option = 0;
					}

					~SceImeParamExtended()
					{
						Marshal.FreeCoTaskMem(_additionalDictionaryPath);
					}
				};



				public enum EnumImeDialogResult
				{
					RESULT_OK,				// User selected either close button or Enter button
					RESULT_USER_CANCELED,	// User performed cancel operation.
					RESULT_ABORTED,			// IME Dialog operation has been aborted.
				}
				
				public enum EnumImeDialogResultButton
				{
					BUTTON_NONE,	// IME Dialog operation has been aborted or canceled.
					BUTTON_CLOSE,	// User selected close button
					BUTTON_ENTER,	// User selected Enter button
				}
				
				[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
				public struct ImeDialogResult
				{
					public EnumImeDialogResult result;
					public EnumImeDialogResultButton button;
					IntPtr _text;
					public string text
					{
						get { return Marshal.PtrToStringAnsi(_text); }
					}
				};
				
				// Initialisation.
				[DllImport("CommonDialog")]
				private static extern int PrxImeDialogInitialise();
				
				// House keeping.
				[DllImport("CommonDialog")]
				private static extern void PrxImeDialogUpdate();
				
				// IME Dialog.
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxImeDialogIsDialogOpen();
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxImeDialogOpen(SceImeDialogParam parameters, SceImeParamExtended extended);
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxImeDialogGetResult(out ImeDialogResult result);
				
				// Messages.
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogHasMessage();
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogGetFirstMessage(out Dialog.Messages.PluginMessage msg);
				[DllImport("CommonDialog")] [return:MarshalAs(UnmanagedType.I1)]
				private static extern bool PrxCommonDialogRemoveFirstMessage();
				
				// Event handlers.
				public static event Dialog.Messages.EventHandler OnGotIMEDialogResult;
				
				// Is the IME dialog open?
				public static bool IsDialogOpen
				{
					get { return PrxImeDialogIsDialogOpen(); }
				}

				public static bool Open(SceImeDialogParam Imeparams, SceImeParamExtended extended)
				{
					bool ret = PrxImeDialogOpen(Imeparams, extended);
					return ret;
				}
				
				public static ImeDialogResult GetResult()
				{
					ImeDialogResult result = new ImeDialogResult();
					PrxImeDialogGetResult(out result);
					return result;
				}
				
				public static void ProcessMessage(Dialog.Messages.PluginMessage msg)
				{
					// Interpret the message and trigger corresponding events.
					switch (msg.type)
					{
						case Dialog.Messages.MessageType.kDialog_GotIMEDialogResult:
						if (OnGotIMEDialogResult != null) OnGotIMEDialogResult(msg);
						break;
					}
				}
				
				public static void Initialise()
				{
					PrxImeDialogInitialise();
				}
				
				public static void Update()
				{
					PrxImeDialogUpdate();
	//				PumpMessages();
				}
			}
		} // Dialog
	} // PS4
} // Sony
