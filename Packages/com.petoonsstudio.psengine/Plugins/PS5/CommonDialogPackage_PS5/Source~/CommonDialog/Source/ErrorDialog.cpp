#include "ErrorDialog.h"
#include "MessagePipe.h"
#include "ErrorCodes.h"
#include <libsysmodule.h>

namespace UnityCommonDialog
{
	PRX_EXPORT bool PrxCommonDialogErrorMessage(int errorCode, SceUserServiceUserId userId /*= 0*/ )
	{
		ErrorDialogParams params;
		params.errorCode = errorCode;
		params.userId = userId;

		if(params.userId == 0)
		{
			if(! GetInitialUser(&params.userId) )
			{
				return false;
			}
		}
		return gErrorDialog.StartDialog(&params);
	}

	PRX_EXPORT bool PrxErrorDialogIsDialogOpen()
	{
		return gErrorDialog.IsDialogOpen();
	}

	PRX_EXPORT bool PrxErrorDialogOpen(ErrorDialogParams* params)
	{
		return gErrorDialog.StartDialog(params);
	}

	PRX_EXPORT bool PrxErrorDialogGetResult(ErrorDialogResult* result)
	{
		//result->result is never set, but you can see what current or last error is being shown
		return gErrorDialog.Get(result);
	}

	ErrorDialog gErrorDialog;

	ErrorDialog::ErrorDialog() :
	m_DialogOpen(false)
	, m_DialogInitialized(false)
	{
	}

	ErrorDialog::~ErrorDialog()
	{

	}

	bool ErrorDialog::StartDialog(const ErrorDialogParams* params)
	{
		if(m_DialogOpen)
		{
			Messages::Log("UnityCommonDialog::ErrorDialog is already open\n");
			return false;
		}

		m_CachedParams.errorCode = params->errorCode;
		m_CachedParams.userId = params->userId;

		return StartDialog(m_CachedParams);
	}

	void ErrorDialog::TerminateDialog()
	{
		if(m_DialogInitialized)
		{
			int32_t ret = sceErrorDialogTerminate();
			if (ret < 0)
			{
				Messages::LogError("ErrorDialog::%s::sceErrorDialogTerminate::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			}
			m_DialogInitialized = false;
		}
	}

	bool ErrorDialog::StartDialog(const Params& params)
	{
		if(m_DialogOpen)
		{
			Messages::Log("UnityCommonDialog::ErrorDialog is already open\n");
			return false;
		}
		if(params.errorCode == 0)
		{
			Messages::Log("UnityCommonDialog::ErrorDialog - errorCode can't be 0 (SCE_OK)\n");
			return false;
		}

		int32_t ret = sceSysmoduleIsLoaded(SCE_SYSMODULE_ERROR_DIALOG);
		if (ret != SCE_OK)
		{
			if (ret == SCE_SYSMODULE_ERROR_UNLOADED)
			{
				ret =  sceSysmoduleLoadModule(SCE_SYSMODULE_ERROR_DIALOG);
			}
			
			if (ret != SCE_OK)
			{
				Messages::LogError("ErrorDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				return false;
			}
		}

		ret = sceErrorDialogInitialize();
		if (ret != SCE_OK )
		{
			Messages::LogError("ErrorDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			return false;
		}

		m_DialogInitialized = true;

		m_DialogOpen = true;

		SceErrorDialogParam errorParams;
		sceErrorDialogParamInitialize(&errorParams);
		errorParams.errorCode = params.errorCode;
		errorParams.userId = params.userId;

		ret = sceErrorDialogOpen(&errorParams);
		if (ret < 0)
		{
			Messages::LogError("ErrorDialog::%s@L%d - sceErrorDialogOpen - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			TerminateDialog();
			m_DialogOpen = false;
		}
		return m_DialogOpen;
	}

	bool ErrorDialog::Update()
	{
		if (m_DialogOpen == true)
		{
			SceErrorDialogStatus cdStatus = sceErrorDialogUpdateStatus();
			if(cdStatus == SCE_ERROR_DIALOG_STATUS_FINISHED)
			{
				m_DialogOpen = false;
				TerminateDialog();
			}
		}
		return true;
	}


	// For use when calling directly from scripts.
	bool ErrorDialog::Get(ErrorDialogResult* result)
	{
		//m_CachedResult.result never seems to be set, but you can see what current or last error is being shown
		result->result = m_CachedResult.result;
		result->button = m_CachedResult.button;
		result->text = m_CachedResult.text;

		return true;
	}

}
