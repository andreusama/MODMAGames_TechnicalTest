using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SonyPS5CommonDialog : MonoBehaviour, IScreen
{
	MenuStack menuStack;
	float waitTime = 0;
	float progressDelay = 0;
	float progressTime = 0;
    float vrSetupWaitTime = 0;
	string imeText = "こんにちは";

	MenuLayout menuMain;
	MenuLayout menuUserMessage;
	MenuLayout menuSystemMessage1;
	MenuLayout menuErrorMessage;
	MenuLayout menuProgress;
	MenuLayout menuSignIn;
	MenuLayout menuIME;
	MenuLayout menuWebBrowser;
    MenuLayout menuVrSetup;

	void Start()
	{
		menuMain = new MenuLayout(this, 450, 34);
		menuUserMessage = new MenuLayout(this, 450, 34);
		menuSystemMessage1 = new MenuLayout(this, 450, 34);
		menuErrorMessage = new MenuLayout(this, 1000, 34);
		menuProgress = new MenuLayout(this, 450, 34);
		menuSignIn = new MenuLayout(this, 450, 34);
		menuIME = new MenuLayout(this, 450, 34);
		menuWebBrowser = new MenuLayout(this, 450, 34);
        menuVrSetup = new MenuLayout(this, 450, 34);

		menuStack = new MenuStack();
		menuStack.SetMenu(menuMain);

		Sony.PS5.Dialog.Main.OnLog += OnLog;
		Sony.PS5.Dialog.Main.OnLogWarning += OnLogWarning;
		Sony.PS5.Dialog.Main.OnLogError += OnLogError;

		Sony.PS5.Dialog.Common.OnGotDialogResult += OnGotDialogResult;
		Sony.PS5.Dialog.Ime.OnGotIMEDialogResult += OnGotIMEDialogResult;
		Sony.PS5.Dialog.Signin.OnGotSigninDialogResult += OnGotSigninDialogResult;
		Sony.PS5.Dialog.WebBrowser.OnGotWebBrowserDialogResult += OnGotWebBrowserDialogResult;
        Sony.PS5.Dialog.VrSetup.OnGotVrSetupDialogResult += OnGotVrSetupDialogResult;

		Sony.PS5.Dialog.Main.Initialise();

	}

	public void OnEnter() {}
	public void OnExit() {}

	public void Process(MenuStack stack)
	{
		if(stack.GetMenu() == menuMain)
		{
			MenuMain();
		}
		else if (stack.GetMenu() == menuUserMessage)
		{
			MenuUserMessage();
		}
		else if (stack.GetMenu() == menuSystemMessage1)
		{
			MenuSystemMessage1();
		}
		else if (stack.GetMenu() == menuErrorMessage)
		{
			MenuErrorMessage();
		}
		else if (stack.GetMenu() == menuProgress)
		{
			MenuProgress();
		}
		else if (stack.GetMenu() == menuSignIn)
		{
			MenuSignIn();
		}
		else if(stack.GetMenu() == menuIME)
		{
			MenuIME();
		}
		else if(stack.GetMenu() == menuWebBrowser)
		{
			MenuWebBrowser();
		}
        else if (stack.GetMenu() == menuVrSetup)
        {
            MenuVrSetup();
        }
	}

	public void MenuMain()
	{
		menuMain.Update();

		if (menuMain.AddItem("IME Dialog"))
		{
			menuStack.PushMenu(menuIME);
		}

		if (menuMain.AddItem("Signin Dialog"))
		{
			menuStack.PushMenu(menuSignIn);
		}

		if (menuMain.AddItem("User"))
		{
			menuStack.PushMenu(menuUserMessage);
		}

		if (menuMain.AddItem("System 1"))
		{
			menuStack.PushMenu(menuSystemMessage1);
		}

		if (menuMain.AddItem("Progress"))
		{
			menuStack.PushMenu(menuProgress);
		}

		if (menuMain.AddItem("Error"))
		{
			menuStack.PushMenu(menuErrorMessage);
		}

		if (menuMain.AddItem("Web Browser"))
		{
			menuStack.PushMenu(menuWebBrowser);
		}

        if (menuMain.AddItem("Vr Setup"))
        {
            menuStack.PushMenu(menuVrSetup);
        }
    }

    void MenuVrSetup()
    {
        menuVrSetup.Update();

        if (menuVrSetup.AddItem("Open VR dialog."))
        {
            // For this dialog to open you must be in one of these states:
            // 1. Have VR headset disconnected, or have it connected but turned off
            // 2. Have more than one user logged in and have the VR headset assigned to any user except the user who started the application
            Sony.PS5.Dialog.VrSetup.Open();
            // When vrSetupWaitTime reaches 0, the dialog will be closed 
            vrSetupWaitTime = 5;
        }


        if (menuVrSetup.AddItem("Back"))
        {
            menuStack.PopMenu();
        }
    }

	void MenuUserMessage()
	{
		menuUserMessage.Update();

		if (menuUserMessage.AddItem("Yes No"))
		{
			Sony.PS5.Dialog.Common.ShowUserMessage(Sony.PS5.Dialog.Common.UserMessageType.YesNo, true, "Do Something ?");
		}

		if (menuUserMessage.AddItem("Ok"))
		{
			Sony.PS5.Dialog.Common.ShowUserMessage(Sony.PS5.Dialog.Common.UserMessageType.Ok, true, "Do Something ?");
		}

		if (menuUserMessage.AddItem("Ok Cancel"))
		{
			Sony.PS5.Dialog.Common.ShowUserMessage(Sony.PS5.Dialog.Common.UserMessageType.OkCancel, true, "Do Something ?");
		}

		if (menuUserMessage.AddItem("Wait"))
		{
			Sony.PS5.Dialog.Common.ShowUserMessage(Sony.PS5.Dialog.Common.UserMessageType.Wait, true, "Wait 5 seconds");
			waitTime = 5;
		}

		if (menuUserMessage.AddItem("Wait Cancel"))
		{
			Sony.PS5.Dialog.Common.ShowUserMessage(Sony.PS5.Dialog.Common.UserMessageType.WaitCancel, true, "Wait 5 seconds");
			waitTime = 5;
		}
		if (menuUserMessage.AddItem("YesNo (Focus No)"))
		{
			Sony.PS5.Dialog.Common.ShowUserMessage(Sony.PS5.Dialog.Common.UserMessageType.YesNo_FocusNo, true, "Do Something ?");
		}
		if (menuUserMessage.AddItem("Ok Cancel (Focus Cancel)"))
		{
			Sony.PS5.Dialog.Common.ShowUserMessage(Sony.PS5.Dialog.Common.UserMessageType.OkCancel_FocusCancel, true, "Do Something ?");
		}
		if (menuUserMessage.AddItem("Two buttons"))
		{
			Sony.PS5.Dialog.Common.ShowUserMessage(Sony.PS5.Dialog.Common.UserMessageType.TwoButtons, true, "Pick One","Choice A", "Choice B");
		}

		if (menuUserMessage.AddItem("No Button"))
		{
			Sony.PS5.Dialog.Common.ShowUserMessage(Sony.PS5.Dialog.Common.UserMessageType.None, true, "Wait 5 seconds");
			waitTime = 5;
		}

		if (menuUserMessage.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}

	void MenuSystemMessage1()
	{
		menuSystemMessage1.Update();

		if (menuSystemMessage1.AddItem("TRC R5055-EmptyStore"))
		{
			Sony.PS5.Dialog.Common.ShowSystemMessage(Sony.PS5.Dialog.Common.SystemMessageType.TRC_EMPTY_STORE, 0);
		}

		if (menuSystemMessage1.AddItem("Camera not connected"))
		{
			Sony.PS5.Dialog.Common.ShowSystemMessage(Sony.PS5.Dialog.Common.SystemMessageType.CAMERA_NOT_CONNECTED,  0);
			waitTime = 5;
		}

		if (menuSystemMessage1.AddItem("TRC R5061-CommsRestrict"))
		{
			Sony.PS5.Dialog.Common.ShowSystemMessage(Sony.PS5.Dialog.Common.SystemMessageType.PSN_COMMUNICATION_RESTRICTION,  0);
			waitTime = 5;
		}

		if (menuSystemMessage1.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}


	void ShowErrorButton(string name, System.UInt32 errorCode, int userId = 0)
	{
		if (menuErrorMessage.AddItem(name))
		{
			bool success = Sony.PS5.Dialog.Common.ShowErrorMessage( errorCode, userId );
			if (!success)
			{
				OnScreenLog.Add("FAILED to show ERROR: " + name);
			}
		}
	}

	void MenuErrorMessage()
	{
		menuErrorMessage.Update();

		ShowErrorButton("Show Error (0x8001000C) for Starting User(0)", 0x8001000C);
		ShowErrorButton("Fail to show Invalid Error (0) for Starting user(0)", 0);
		ShowErrorButton("Fail to show Error (0x8001000C) for Invalid user(-1)", 0x8001000C, -1);
		//ShowErrorButton("valid error, SCE_USER_SERVICE_USER_ID_SYSTEM", 0x8001000C, 255);//SCE_USER_SERVICE_USER_ID_SYSTEM - User ID specified when using a device that cannot be allocated to a specific user
		//ShowErrorButton("valid error, SCE_USER_SERVICE_USER_ID_EVERYONE", 0x8001000C, 254);//SCE_USER_SERVICE_USER_ID_EVERYONE - User ID specified when not limiting common dialog operation to a specific user

		for (int slot = 0; slot < 4; ++slot) {
			UnityEngine.PS5.PS5Input.LoggedInUser user = UnityEngine.PS5.PS5Input.GetUsersDetails(slot);
			if (user.status != 0)
			{
				ShowErrorButton("Valid Error for " + user.userName + " ("+user.userId+")", 0x8001000C, user.userId);
				//ShowErrorButton("Invalid Error for pad: " + slot + " (" + user.userId + ")", 0, user.userId);
			}
		}
		if (menuErrorMessage.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}

	void ShowSignInbutton(string name, int userId)
	{
		if (menuSignIn.AddItem(name))
   		{
   			Sony.PS5.Dialog.Signin.Open(userId);
		}
	}

	void MenuSignIn()
	{
		menuSignIn.Update();

		ShowSignInbutton("Sign in starting user (0)", 0);
		ShowSignInbutton("Invalid UserId(-1)", -1);

		for (int slot = 0; slot < 4; ++slot) {
			UnityEngine.PS5.PS5Input.LoggedInUser user = UnityEngine.PS5.PS5Input.GetUsersDetails(slot);
			if (user.status != 0)
			{
				ShowSignInbutton("Sign in " + user.userName, user.userId);
			}
		}

		if (menuSignIn.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}

	void MenuWebBrowser()
	{
		menuWebBrowser.Update();

		if (menuWebBrowser.AddItem("Open WebBrowser"))
   		{
			var webBrowserParam = new Sony.PS5.Dialog.WebBrowser.WebBrowserParam();
			webBrowserParam.url = "https://www.playstation.com/country-selector/";

   			Sony.PS5.Dialog.WebBrowser.Open(webBrowserParam);
		}



		if (menuWebBrowser.AddItem("Open Predetermined Content"))
   		{
			var webBrowserParam = new Sony.PS5.Dialog.WebBrowser.WebBrowserParam();
			webBrowserParam.url = "https://sample.siedev.net/webbrowser_domain_check.htm";

			var param2 = new Sony.PS5.Dialog.WebBrowser.WebBrowserPredeterminedContentParam();
			param2.domain[0]="sample.siedev.net";
			param2.domain[1]="www.playstation.com";

   			Sony.PS5.Dialog.WebBrowser.OpenForPredeterminedContent(webBrowserParam, param2);
		}

		if (menuWebBrowser.AddItem("Set test cookie"))
   		{
			string url = "https://sample.siedev.net/show_cookie_SDK.html";
			string cookie = "key5=Unity_Code_Set_Me";
   			Sony.PS5.Dialog.WebBrowser.SetCookie(url, cookie);
		}

		if (menuWebBrowser.AddItem("Reset cookies"))
   		{
   			Sony.PS5.Dialog.WebBrowser.ResetCookie();
		}

		if (menuWebBrowser.AddItem("View test Cookies"))
   		{
			var webBrowserParam = new Sony.PS5.Dialog.WebBrowser.WebBrowserParam();
			webBrowserParam.url = "https://sample.siedev.net/show_cookie_SDK.html";
   			Sony.PS5.Dialog.WebBrowser.Open(webBrowserParam);
		}

		if (menuWebBrowser.AddItem("Terminate System"))
   		{
   			Sony.PS5.Dialog.WebBrowser.Terminate();
		}

		if (menuWebBrowser.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}


	void ShowIMEButton(string buttonText,  uint userId, string startingText, uint maxLength )
	{
		if (menuIME.AddItem(buttonText))
		{
			Sony.PS5.Dialog.Ime.SceImeDialogParam ImeParam = new Sony.PS5.Dialog.Ime.SceImeDialogParam();
			Sony.PS5.Dialog.Ime.SceImeParamExtended ImeExtendedParam = new Sony.PS5.Dialog.Ime.SceImeParamExtended();

			// Set supported languages, 'or' flags together or set to 0 to support all languages.
			ImeParam.supportedLanguages = Sony.PS5.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_JAPANESE |
												Sony.PS5.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_ENGLISH_GB |
												Sony.PS5.Dialog.Ime.FlagsSupportedLanguages.LANGUAGE_DANISH;

			ImeParam.option = Sony.PS5.Dialog.Ime.Option.MULTILINE;
			ImeParam.title = buttonText+" 日本語";
			ImeParam.maxTextLength = maxLength;// 8;
			ImeParam.inputTextBuffer = startingText;// "Player 1";
			ImeParam.userId = userId;

			Sony.PS5.Dialog.Ime.Open(ImeParam, ImeExtendedParam);
		}
	}
	private void MenuIME()
	{
		menuIME.Update();
		ShowIMEButton("IME for Starting Player", 0 ,"starting", 10);
		for (int slot = 0; slot < 4; ++slot)
		{
			UnityEngine.PS5.PS5Input.LoggedInUser user = UnityEngine.PS5.PS5Input.GetUsersDetails(slot);
			if (user.status != 0)
			{
				ShowIMEButton("IME for " + user.userName, (uint)user.userId, user.userName, (uint)(10+slot));
			}
		}

		if (menuIME.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}


	void MenuProgress()
	{
		menuProgress.Update();

		if (menuProgress.AddItem("Progress Bar"))
		{
			Sony.PS5.Dialog.Common.ShowProgressBar("Working");
			progressDelay = 3;
			progressTime = 5;
		}

		if (menuProgress.AddItem("Back"))
		{
			menuStack.PopMenu();
		}
	}

	void OnGUI()
	{
		MenuLayout activeMenu = menuStack.GetMenu();
		activeMenu.GetOwner().Process(menuStack);
	}

	void OnLog(Sony.PS5.Dialog.Messages.PluginMessage msg)
	{
		OnScreenLog.Add(msg.Text);
	}

	void OnLogWarning(Sony.PS5.Dialog.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("WARNING: " + msg.Text);
	}

	void OnLogError(Sony.PS5.Dialog.Messages.PluginMessage msg)
	{
		OnScreenLog.Add("ERROR: " + msg.Text);
	}

	void OnGotDialogResult(Sony.PS5.Dialog.Messages.PluginMessage msg)
	{
		Sony.PS5.Dialog.Common.CommonDialogResult result = Sony.PS5.Dialog.Common.GetResult();

		OnScreenLog.Add("Dialog result: " + result);
	}

	void OnGotIMEDialogResult(Sony.PS5.Dialog.Messages.PluginMessage msg)
	{
		Sony.PS5.Dialog.Ime.ImeDialogResult result = Sony.PS5.Dialog.Ime.GetResult();

		OnScreenLog.Add("IME result: " + result.result);
		OnScreenLog.Add("IME button: " + result.button);
		OnScreenLog.Add("IME text: " + result.text);
		if (result.result == Sony.PS5.Dialog.Ime.EnumImeDialogResult.RESULT_OK)
		{
			imeText = result.text;
			OnScreenLog.Add("IME result.text: " + imeText);
		}
		else
		{
			OnScreenLog.Add("IME result.text: " + result.text);
		}
	}

	void OnGotSigninDialogResult(Sony.PS5.Dialog.Messages.PluginMessage msg)
	{
		Sony.PS5.Dialog.Signin.SigninDialogResult result = Sony.PS5.Dialog.Signin.GetResult();

		OnScreenLog.Add("Signin result: " + result.result);
	}

	void OnGotWebBrowserDialogResult(Sony.PS5.Dialog.Messages.PluginMessage msg)
	{
		Sony.PS5.Dialog.WebBrowser.WebBrowserDialogResult result = Sony.PS5.Dialog.WebBrowser.GetResult();

		OnScreenLog.Add("web browser result: " + result.result);
	}

    void OnGotVrSetupDialogResult(Sony.PS5.Dialog.Messages.PluginMessage msg)
    {
        Sony.PS5.Dialog.VrSetup.VrSetupDialogResult result = Sony.PS5.Dialog.VrSetup.GetResult();

        OnScreenLog.Add("VrSetupDialog result: " + result);
    }

    void Update ()
	{
		Sony.PS5.Dialog.Main.Update();

		// Update system wait dialog.
		if(waitTime > 0)
		{
			waitTime -= Time.deltaTime;
			if (waitTime <= 0)
			{
				waitTime = 0;
				Sony.PS5.Dialog.Common.Close();
			}
		}

		// Update progress dialog.
		if(progressDelay > 0)
		{
			progressDelay -= Time.deltaTime;
			if (progressDelay <= 0)
			{
				progressDelay = 0;
			}
		}
		else if (progressTime > 0)
		{
			progressTime -= Time.deltaTime;
			if (progressTime <= 0)
			{
				progressTime = 0;
				Sony.PS5.Dialog.Common.Close();
			}

			float percent = (5 - progressTime) / 5;
			int intPercent = (int)(percent * 100);
			Sony.PS5.Dialog.Common.SetProgressBarPercent(intPercent);
			Sony.PS5.Dialog.Common.SetProgressBarMessage("Coming Soon - " + intPercent);
		}

        //Update VR setup dialog wait
        if (vrSetupWaitTime > 0)
        {
            vrSetupWaitTime -= Time.deltaTime;
            if (vrSetupWaitTime <= 0)
            {
                vrSetupWaitTime = 0;
                Sony.PS5.Dialog.VrSetup.Close();
            }
        }
	}

}
