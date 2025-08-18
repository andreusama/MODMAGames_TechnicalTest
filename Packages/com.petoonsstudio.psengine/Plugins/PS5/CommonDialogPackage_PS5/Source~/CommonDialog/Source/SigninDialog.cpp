#include "SigninDialog.h"
#include "MessagePipe.h"
#include "ErrorCodes.h"
#include <libsysmodule.h>

namespace UnityCommonDialog
{
	PRX_EXPORT bool PrxSigninDialogIsDialogOpen()
	{
		return gSigninDialog.IsDialogOpen();
	}

	PRX_EXPORT bool PrxSigninDialogOpen(int userId)
	{
		return gSigninDialog.StartDialog(userId);
	}

	PRX_EXPORT bool PrxSigninDialogGetResult(SigninDialogResult* result)
	{
		return gSigninDialog.Get(result);
	}

	SigninDialog gSigninDialog;

	SigninDialog::SigninDialog() :
	m_DialogOpen(false)
	, m_DialogInitialized(false)
	{
	}

	SigninDialog::~SigninDialog()
	{

	}

	void SigninDialog::TerminateDialog()
	{
		int32_t ret = sceSigninDialogTerminate();
		if (ret < 0)
		{
			Messages::LogError("SigninDialog::%s::sceSigninDialogTerminate::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
		}
		m_DialogInitialized = false;
	}

	bool SigninDialog::StartDialog(int userId)
	{
		if(m_DialogOpen)
		{
			Messages::Log("SigninDialog is already open\n");
			return false;
		}

		if (userId == 0)
		{
			if(! GetInitialUser(&userId))	// use default user
			{
				return false;
			}
		}

		int32_t ret = sceSysmoduleIsLoaded(SCE_SYSMODULE_SIGNIN_DIALOG);
		if (ret != SCE_OK)
		{
			if (ret == SCE_SYSMODULE_ERROR_UNLOADED)
			{
				ret = sceSysmoduleLoadModule(SCE_SYSMODULE_SIGNIN_DIALOG);
			}
			
			if (ret != SCE_OK)
			{
				Messages::LogError("SigninDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				return false;
			}
		}

		ret = sceSigninDialogInitialize();
		if (ret != SCE_OK )
		{
			Messages::LogError("SigninDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			return false;
		}
		m_DialogInitialized = true;

		SceSigninDialogParam signinParams;
		sceSigninDialogParamInitialize(&signinParams);
		signinParams.userId = userId;

		ret = sceSigninDialogOpen(&signinParams);
		if (ret < 0)
		{
			Messages::LogError("SigninDialog::%s::sceSigninDialogOpen::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			TerminateDialog();
			m_DialogOpen = false;
		}
		else
		{
			m_DialogOpen = true;
		}
		return m_DialogOpen;
	}

	bool SigninDialog::Update()
	{
		if (m_DialogOpen == true)
		{
			SceSigninDialogStatus cdStatus = sceSigninDialogUpdateStatus();
			if (cdStatus == SCE_SIGNIN_DIALOG_STATUS_FINISHED)
			{
				SceSigninDialogResult result;
				memset(&result, 0x00, sizeof(result));
				int ret = sceSigninDialogGetResult(&result);
				if (ret < 0)
				{
					Messages::LogError("SigninDialog::%s::sceSigninDialogGetResult::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				}
				else
				{
					Messages::AddMessage(Messages::kDialog_GotSigninDialogResult);
				}
				TerminateDialog();
				m_DialogOpen = false;
			}
		}
		return true;
	}


	// For use when calling directly from scripts.
	bool SigninDialog::Get(SigninDialogResult* result)
	{
		result->result = m_CachedResult.result;

		return true;
	}

} // namespace UnityCommonDialog
