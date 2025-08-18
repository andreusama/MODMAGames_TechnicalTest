#include "WebBrowser.h"
#include "MessagePipe.h"
#include "ErrorCodes.h"
#include <libsysmodule.h>

namespace UnityCommonDialog
{
	PRX_EXPORT bool PrxWebBrowserDialogIsDialogOpen()
	{
		return gWebBrowserDialog.IsDialogOpen();
	}


	PRX_EXPORT bool PrxWebBrowserDialogOpen(WebBrowserParam * webBrowserParam)
	{
		return gWebBrowserDialog.StartDialog(webBrowserParam, NULL, NULL, NULL, NULL, NULL);
	}


	PRX_EXPORT bool PrxWebBrowserDialogOpenForPredeterminedContent(WebBrowserParam * webBrowserParam, const char *domain0, const char *domain1,  const char *domain2,  const char *domain3,  const char *domain4 )
	{
		return gWebBrowserDialog.StartDialog(webBrowserParam, domain0, domain1, domain2, domain3, domain4);
	}

	PRX_EXPORT int PrxWebBrowserDialogResetCookie()
	{
		return gWebBrowserDialog.ResetCookie();
	}


	PRX_EXPORT int PrxWebBrowserDialogSetCookie(const char *url, const char *cookie)
	{
		return gWebBrowserDialog.SetCookie(url, cookie);
	}

	PRX_EXPORT bool PrxWebBrowserDialogGetResult(WebBrowserDialogResult* result)
	{
		return gWebBrowserDialog.Get(result);
	}

	PRX_EXPORT int PrxWebBrowserDialogTerminate()
	{
		return gWebBrowserDialog.Terminate();
	}

	WebBrowserDialog gWebBrowserDialog;

	WebBrowserDialog::WebBrowserDialog() :
	m_DialogOpen(false)
	, m_DialogInitialized(false)
	{
	}

	WebBrowserDialog::~WebBrowserDialog()
	{

	}

	int32_t WebBrowserDialog::Terminate()
	{
		int32_t ret  = 0;
		if (m_DialogInitialized)
		{
			ret = sceWebBrowserDialogTerminate();
			if (ret < 0)
			{
				Messages::LogError("WebBrowserDialog::%s::sceWebBrowserDialogTerminate::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			}
			m_DialogInitialized = false;
		}
		return ret;
	}

	bool WebBrowserDialog::InitialiseModule()
	{
		int32_t ret = sceSysmoduleIsLoaded(SCE_SYSMODULE_WEB_BROWSER_DIALOG);
		if (ret != SCE_OK)
		{
			if (ret == SCE_SYSMODULE_ERROR_UNLOADED)
			{
				ret = sceSysmoduleLoadModule(SCE_SYSMODULE_WEB_BROWSER_DIALOG);
			}

			if (ret != SCE_OK)
			{
				Messages::LogError("WebBrowserDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				return false;
			}
		}

		if (!m_DialogInitialized)
		{
			ret = sceWebBrowserDialogInitialize();
			if (ret != SCE_OK )
			{
				Messages::LogError("WebBrowserDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				return false;
			}
			m_DialogInitialized = true;
		}
		return true;
	}



	bool WebBrowserDialog::StartDialog(WebBrowserParam *param, const char *domain0, const char *domain1, const char *domain2, const char *domain3, const char *domain4 )
	{
		int userId = param->userId;
		int ret;
		if(m_DialogOpen)
		{
			Messages::Log("WebBrowserDialog is already open\n");
			return false;
		}

		if (userId == 0)
		{
			if(! GetInitialUser(&userId))	// use default user
			{
				return false;
			}
		}

		if (InitialiseModule() == false)
		{
			return false;
		}


		SceWebBrowserDialogParam params;
		sceWebBrowserDialogParamInitialize(&params);

		SceWebBrowserDialogImeParam  imeParam;
		memset(&imeParam,0,sizeof(imeParam));
		imeParam.size = sizeof(imeParam);

		SceWebBrowserDialogWebViewParam  webviewParam;
		memset(&webviewParam,0,sizeof(webviewParam));
		webviewParam.size = sizeof(webviewParam);


		params.mode = param->mode;
		params.userId = userId;
		params.url = param->url;
		params.width = param->width;
		params.height = param->height;

		params.positionX = param->positionX;
		params.positionY = param->positionY;
		params.parts = param->parts;
		params.headerWidth = param->headerWidth;
		params.headerPositionX = param->headerPositionX;
		params.headerPositionY = param->headerPositionY;
		params.control = param->control;
		if (param->imeOption!=0)
		{
			imeParam.option = param->imeOption;
			params.imeParam = &imeParam;
		}

		if (param->webViewOption!=0)
		{
			webviewParam.option = param->webViewOption;
			params.webviewParam = &webviewParam;
		}

		params.animation = param->animation;

		if (domain0!=NULL)
		{
			SceWebBrowserDialogPredeterminedContentParam param2;
			memset(&param2, 0, sizeof(param2));
			param2.size = sizeof(param2);
			param2.domain[0]=domain0;
			param2.domain[1]=domain1;
			param2.domain[2]=domain2;
			param2.domain[3]=domain3;
			param2.domain[4]=domain4;
			ret = sceWebBrowserDialogOpenForPredeterminedContent(&params, &param2);
		}
		else
		{
			ret = sceWebBrowserDialogOpen(&params);
		}

		if (ret < 0)
		{
			Messages::LogError("WebBrowserDialog::%s::sceWebBrowserDialogOpen::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			m_DialogOpen = false;
		}
		else
		{
			m_DialogOpen = true;
		}
		return m_DialogOpen;
	}


	int WebBrowserDialog::ResetCookie()
	{
		if (InitialiseModule() == false)
		{
			return false;
		}

		SceWebBrowserDialogResetCookieParam reset_cookie;
		memset(&reset_cookie, 0x0, sizeof(reset_cookie));
		reset_cookie.size = sizeof(reset_cookie);

		int ret = sceWebBrowserDialogResetCookie(&reset_cookie);
		return ret;
	}

	int WebBrowserDialog::SetCookie(const char *url, const char *cookie)
	{
		if (InitialiseModule() == false)
		{
			return false;
		}

		SceWebBrowserDialogSetCookieParam set_cookie;
		memset(&set_cookie, 0x0, sizeof(set_cookie));
		set_cookie.size = sizeof(set_cookie);
		set_cookie.url = url;
		set_cookie.cookie = cookie;

		int ret = sceWebBrowserDialogSetCookie(&set_cookie);
		return ret;
	}


	bool WebBrowserDialog::Update()
	{
		if (m_DialogOpen == true)
		{
			SceCommonDialogStatus cdStatus = sceWebBrowserDialogUpdateStatus();
			if (cdStatus == SCE_COMMON_DIALOG_STATUS_FINISHED)
			{
				SceWebBrowserDialogResult result;
				memset(&result, 0x00, sizeof(result));
				int ret = sceWebBrowserDialogGetResult(&result);
				if (ret < 0)
				{
					Messages::LogError("WebBrowserDialog::%s::sceWebBrowserDialogGetResult::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				}
				else
				{
					Messages::PluginMessage* msg = new Messages::PluginMessage();
					msg->type = Messages::kDialog_GotWebBrowserDialogResult;
					msg->SetData(result.result);
					Messages::AddMessage(msg);
				}
				m_DialogOpen = false;
			}
		}
		return true;
	}


	// For use when calling directly from scripts.
	bool WebBrowserDialog::Get(WebBrowserDialogResult* result)
	{
		result->result = m_CachedResult.result;

		return true;
	}

} // namespace UnityCommonDialog
