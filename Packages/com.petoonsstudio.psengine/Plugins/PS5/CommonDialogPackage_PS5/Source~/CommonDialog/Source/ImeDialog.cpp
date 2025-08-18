#include "ImeDialog.h"
#include "MessagePipe.h"
#include "ErrorCodes.h"
#include <libsysmodule.h>
#include <wchar.h>

namespace UnityCommonDialog
{
	PRX_EXPORT bool PrxImeDialogIsDialogOpen()
	{
		return gImeDialog.IsDialogOpen();
	}

	PRX_EXPORT bool PrxImeDialogOpen(SceImeDialogParam* params, SceImeParamExtended* extended)
	{
		return gImeDialog.StartDialog(params, extended);
	}

	PRX_EXPORT bool PrxImeDialogGetResult(ImeDialogResult* result)
	{
		return gImeDialog.Get(result);
	}

	ImeDialog gImeDialog;

	ImeDialog::ImeDialog() :
	m_DialogOpen(false)
		,m_DialogInitialized(false)
		,m_DefaultInputBuffer(NULL)
		,m_DefaultResultString(NULL)
	{
	}

	ImeDialog::~ImeDialog()
	{
		free(m_DefaultInputBuffer);
		free(m_DefaultResultString);
	}

	bool ImeDialog::StartDialog(const SceImeDialogParam* params, const SceImeParamExtended *extended)
	{
		if(m_DialogOpen)
		{
			Messages::Log("ImeDialog is already open\n");
			return false;
		}

		m_CachedParams.params = *params;
		m_CachedParams.extended = *extended;
		memset(&m_CachedParams.params.reserved, 0, sizeof(m_CachedParams.params.reserved));
		memset(&m_CachedParams.extended.reserved, 0, sizeof(m_CachedParams.extended.reserved));

		if (m_CachedParams.params.userId == 0)
		{
			if(!GetInitialUser(&m_CachedParams.params.userId))	// use default user
			{
				return false;
			}
		}

		m_CachedParams.callback = DefaultResultCallback;
		if(m_DefaultInputBuffer)
		{
			free(m_DefaultInputBuffer);
		}

//		printf("maxTextLength:%d\n", params->maxTextLength);
//		printf("SCE_IME_MAX_PREEDIT_LENGTH:%d\n", SCE_IME_MAX_PREEDIT_LENGTH);
		

		const int maxbytesUTF16 = 4;	// max number of bytes one UTF-16 character can take up. "UTF-16.This scheme expresses each Unicode character with one or two 16 - bit values"
		m_MaxNumChars = params->maxTextLength + SCE_IME_MAX_PREEDIT_LENGTH + 1;		// +1 for the terminator

		m_InputBufferSize = maxbytesUTF16 * m_MaxNumChars;
		
//		printf("bufferSize:%zd\n", m_InputBufferSize);

		m_DefaultInputBuffer = (wchar_t *)malloc(m_InputBufferSize);

		return StartDialog(m_CachedParams);
	}

	void ImeDialog::TerminateDialog()
	{
		if(m_DialogInitialized)
		{
			int32_t ret = sceImeDialogTerm();
			if (ret < 0)
			{
				Messages::LogError("ErrorDialog::%s::sceImeDialogTerm::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			}
			m_DialogInitialized = false;
		}
	}

	bool ImeDialog::StartDialog(Params& params)
	{
		if(m_DialogOpen)
		{
			Messages::Log("ImeDialog is already open\n");
			return false;
		}


		int32_t ret = sceSysmoduleIsLoaded(SCE_SYSMODULE_IME_DIALOG);
		if (ret != SCE_OK)
		{
			if (ret == SCE_SYSMODULE_ERROR_UNLOADED)
			{
				ret =  sceSysmoduleLoadModule(SCE_SYSMODULE_IME_DIALOG);
			}
			
			if (ret != SCE_OK)
			{
				Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				return false;
			}
		}

		m_DialogInitialized = true;
		
		memset(m_DefaultInputBuffer, 0,(sizeof(wchar_t) * (SCE_IME_MAX_PREEDIT_LENGTH + params.params.maxTextLength)));

		if (params.params.inputTextBuffer)
		{ 
			wcsncpy((wchar_t*)m_DefaultInputBuffer, (wchar_t*)params.params.inputTextBuffer, (params.params.maxTextLength+1) );
		}
		params.params.inputTextBuffer = m_DefaultInputBuffer;

		m_ResultCallback = params.callback;

		ret = sceImeDialogInit( &params.params, &params.extended );
		if (ret < 0)
		{
			Messages::LogError("ImeDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			m_DialogOpen = false;
			TerminateDialog();
		}
		else
		{
			m_DialogOpen = true;
		}
		return m_DialogOpen;
	}

	bool ImeDialog::Update()
	{
		if (m_DialogOpen == true)
		{
			SceImeDialogStatus cdStatus = sceImeDialogGetStatus();
			if (cdStatus == SCE_IME_DIALOG_STATUS_FINISHED)
			{
				SceImeDialogResult imeResult;
				memset(&imeResult, 0x00, sizeof(imeResult));
				int32_t resultRet = sceImeDialogGetResult(&imeResult);
				if (resultRet < 0)
				{
					Messages::LogError("ImeDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(resultRet));
				}
				else if (m_ResultCallback)
				{
					m_ResultCallback(&imeResult);
					m_ResultCallback = NULL;
				}

				TerminateDialog();
				m_DialogOpen = false;
				return resultRet == SCE_OK;
			}
		}
		return true;
	}

	void ImeDialog::DefaultResultCallback(const SceImeDialogResult *result)
	{
		if (result->endstatus == SCE_IME_DIALOG_END_STATUS_OK)
		{
			if(gImeDialog.m_DefaultResultString)
			{
				free(gImeDialog.m_DefaultResultString);
			}

//			printf("maxTextLength:%d\n", gImeDialog.m_CachedParams.params.maxTextLength);
//			printf("SCE_IME_MAX_PREEDIT_LENGTH:%d\n", SCE_IME_MAX_PREEDIT_LENGTH);

			//int maxTextAllowed = gImeDialog.m_CachedParams.params.maxTextLength + SCE_IME_MAX_PREEDIT_LENGTH;
			const int maxbytesUTF8 = 4;	// max number of bytes one UTF-8 character can take up. "UTF-8.This scheme handles Unicode code points with a variable length byte stream. One character can be expressed with 1 to 4 bytes"
			int outputUTF8bufferSize = maxbytesUTF8 * gImeDialog.m_MaxNumChars;

			gImeDialog.m_DefaultResultString = (char*)malloc(outputUTF8bufferSize);		// allocate UTF-8 result buffer

			// convert from utf-16 into utf-8
			wcstombs(gImeDialog.m_DefaultResultString, gImeDialog.m_DefaultInputBuffer, gImeDialog.m_InputBufferSize);

			gImeDialog.m_CachedResult.result = result->endstatus;
			gImeDialog.m_CachedResult.text = gImeDialog.m_DefaultResultString;
		}
		else
		{
			gImeDialog.m_CachedResult.result = result->endstatus;
			gImeDialog.m_CachedResult.text = gImeDialog.m_DefaultResultString;
		}

		Messages::AddMessage(Messages::kDialog_GotIMEDialogResult);
	}

	// For use when calling directly from scripts.
	bool ImeDialog::Get(ImeDialogResult* result)
	{
		result->result = m_CachedResult.result;
		result->text = m_CachedResult.text;

		return true;
	}

} // namespace UnityCommonDialog
