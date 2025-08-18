#include "CommonDialog.h"
#include "MessagePipe.h"
#include "ErrorCodes.h"
#include <message_dialog.h>
#include <libsysmodule.h>

namespace UnityCommonDialog
{
	PRX_EXPORT bool PrxCommonDialogIsDialogOpen()
	{
		return gDialog.IsDialogOpen();
	}



	PRX_EXPORT bool PrxCommonDialogSystemMessage(int type, int usedId)
	{
		return gDialog.StartDialogSystemMessage(type, usedId);
	}

	PRX_EXPORT bool PrxCommonDialogClose()
	{
		return gDialog.CloseDialog();
	}

	PRX_EXPORT bool PrxCommonDialogProgressBar(const char *str)
	{
		return gDialog.StartDialogProgressBar(str);
	}

	PRX_EXPORT bool PrxCommonDialogProgressBarSetPercent(int percent)
	{
		return gDialog.SetProgressBarPercent(percent);
	}

	PRX_EXPORT bool PrxCommonDialogProgressBarSetMessage(const char* str)
	{
		return gDialog.SetProgressBarMessage(str);
	}

	PRX_EXPORT bool PrxCommonDialogUserMessage(int type, bool infobar, const char* str, const char* button1, const char* button2, const char* button3)
	{
		return 	gDialog.StartDialogUserMessage(type, infobar, str, button1, button2, button3);

	}

	PRX_EXPORT int PrxCommonDialogGetResult()
	{
		return gDialog.GetResult();
	}

	CommonDialog gDialog;

	CommonDialog::CommonDialog()
		: m_DialogOpen(false)
		, m_DialogInitialized(false)
		, m_DialogNeedsClosing(false)
		, m_DialogMode(SCE_MSG_DIALOG_MODE_INVALID)
		, m_UserMessageType(SCE_MSG_DIALOG_BUTTON_TYPE_OK)
		, m_SystemMessageType(-1)
		, m_DialogResult(RESULT_NOT_SET)
	{
	}

	CommonDialog::~CommonDialog()
	{
	}

	bool CommonDialog::CloseDialog()
	{
		if(!m_DialogOpen)
		{
			Messages::Log("CommonDialog is not open\n");
			return false;
		}

		m_DialogNeedsClosing = true;

		return true;
	}

	void CommonDialog::TerminateDialog()
	{
		int32_t ret = sceMsgDialogTerminate();
		if (ret < 0)
		{
			Messages::LogError("CommonDialog::%s::sceMsgDialogTerminate()::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
		}
		m_DialogInitialized = false;
	}

	bool CommonDialog::StartDialogUserMessage(int type, bool infobar, const char* str, const char* button1, const char* button2, const char* /*button3*/)
	{
		if(m_DialogOpen)
		{
			Messages::Log("CommonDialog is already open\n");
			return false;
		}

		m_DialogResult = RESULT_NOT_SET;

		int32_t ret = sceSysmoduleIsLoaded(SCE_SYSMODULE_MESSAGE_DIALOG);
		if (ret != SCE_OK)
		{
			if (ret == SCE_SYSMODULE_ERROR_UNLOADED)
			{
				ret =  sceSysmoduleLoadModule(SCE_SYSMODULE_MESSAGE_DIALOG);
			}
			
			if (ret != SCE_OK)
			{
				Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				return false;
			}
		}

		ret = sceMsgDialogInitialize();
		if (ret != SCE_OK )
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			return false;
		}
		m_DialogInitialized = true;
		SceMsgDialogParam msgParam;
		SceMsgDialogUserMessageParam userMsgParam;
		SceMsgDialogButtonsParam buttonsParam;

		// initialize parameter of message dialog
		sceMsgDialogParamInitialize(&msgParam);
		msgParam.mode = SCE_MSG_DIALOG_MODE_USER_MSG;
		m_DialogMode = msgParam.mode;

		// initialize message dialog
		memset(&userMsgParam, 0, sizeof(SceMsgDialogUserMessageParam));
		msgParam.userMsgParam = &userMsgParam;
		msgParam.userMsgParam->msg = (SceChar8*)str;
		msgParam.userMsgParam->buttonType = (SceMsgDialogButtonType ) type;
		m_UserMessageType = type;

		memset(&buttonsParam, 0x00, sizeof(buttonsParam));
		if (msgParam.userMsgParam->buttonType == SCE_MSG_DIALOG_BUTTON_TYPE_2BUTTONS)
		{
			msgParam.userMsgParam->buttonsParam = &buttonsParam;
			buttonsParam.msg1 = button1;
			buttonsParam.msg2 = button2;			
		}

		ret = sceMsgDialogOpen(&msgParam);
		if (ret < 0)
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			TerminateDialog();

			return false;
		}

		m_DialogOpen = true;

		return true;
	}

	bool CommonDialog::StartDialogProgressBar(const char *str)
	{
		if(m_DialogOpen)
		{
			Messages::Log("CommonDialog is already open\n");
			return false;
		}

		m_DialogResult = RESULT_NOT_SET;
		
		int32_t ret = sceSysmoduleIsLoaded(SCE_SYSMODULE_MESSAGE_DIALOG);
		if (ret != SCE_OK)
		{
			if (ret == SCE_SYSMODULE_ERROR_UNLOADED)
			{
				ret =  sceSysmoduleLoadModule(SCE_SYSMODULE_MESSAGE_DIALOG);
			}
			
			if (ret != SCE_OK)
			{
				Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				return false;
			}
		}

		ret = sceMsgDialogInitialize();
		if (ret != SCE_OK )
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			return false;
		}
		m_DialogInitialized = true;

		SceMsgDialogParam msgParam;
		SceMsgDialogProgressBarParam progBarParam;

		// initialize parameter of message dialog
		sceMsgDialogParamInitialize(&msgParam);
		msgParam.mode = SCE_MSG_DIALOG_MODE_PROGRESS_BAR;
		m_DialogMode = msgParam.mode;

		// initialize message dialog
		memset(&progBarParam, 0, sizeof(SceMsgDialogProgressBarParam));
		msgParam.progBarParam = &progBarParam;
		msgParam.progBarParam->barType = SCE_MSG_DIALOG_PROGRESSBAR_TYPE_PERCENTAGE;
		msgParam.progBarParam->msg = (const SceChar8*)str;


		ret = sceMsgDialogOpen(&msgParam);
		if (ret < 0)
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			sceMsgDialogTerminate();
			return false;
		}

		m_DialogOpen = true;

		return true;
	}

	bool CommonDialog::SetProgressBarPercent(int percent)
	{
		if(!m_DialogOpen || m_DialogMode != SCE_MSG_DIALOG_MODE_PROGRESS_BAR)
		{
			Messages::Log("ProgressBar dialog is not open\n");
			return false;
		}

		int ret = sceMsgDialogProgressBarSetValue(SCE_MSG_DIALOG_PROGRESSBAR_TARGET_BAR_DEFAULT, percent);
		if (ret < 0)
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			return false;
		}

		return true;
	}

	bool CommonDialog::SetProgressBarMessage(const char* str)
	{
		if(!m_DialogOpen || m_DialogMode != SCE_MSG_DIALOG_MODE_PROGRESS_BAR)
		{
			Messages::Log("ProgressBar dialog is not open\n");
			return false;
		}

		int ret = sceMsgDialogProgressBarSetMsg(SCE_MSG_DIALOG_PROGRESSBAR_TARGET_BAR_DEFAULT, (const SceChar8*)str);
		if (ret < 0)
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			return false;
		}

		return true;
	}

	bool CommonDialog::StartDialogSystemMessage(int type, int userId)
	{
		if(m_DialogOpen)
		{
			Messages::Log("Dialog is already open\n");
			return false;
		}

		if (userId == 0)
		{
			if(! GetInitialUser(&userId) )// Get default user
			{
				return false;
			}
		}

		m_DialogResult = RESULT_NOT_SET;

		int32_t ret = sceSysmoduleIsLoaded(SCE_SYSMODULE_MESSAGE_DIALOG);
		if (ret != SCE_OK)
		{
			if (ret == SCE_SYSMODULE_ERROR_UNLOADED)
			{
				ret =  sceSysmoduleLoadModule(SCE_SYSMODULE_MESSAGE_DIALOG);
			}
			
			if (ret != SCE_OK)
			{
				Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
				return false;
			}
		}


		ret = sceMsgDialogInitialize();
		if (ret != SCE_OK )
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			return false;
		}

		m_DialogInitialized = true;
		SceMsgDialogParam msgParam;
		SceMsgDialogSystemMessageParam sysMsgParam;

		// initialize parameter of message dialog
		sceMsgDialogParamInitialize(&msgParam);
		msgParam.mode = SCE_MSG_DIALOG_MODE_SYSTEM_MSG;
		m_DialogMode = msgParam.mode;

		// initialize message dialog
		memset(&sysMsgParam, 0, sizeof(SceMsgDialogSystemMessageParam));
		msgParam.sysMsgParam = &sysMsgParam;
		msgParam.sysMsgParam->sysMsgType = (SceMsgDialogSystemMessageType )type;
		msgParam.userId = userId;
		m_SystemMessageType = type;

		ret = sceMsgDialogOpen( &msgParam );
		if (ret < 0)
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			TerminateDialog();
			m_DialogOpen = false;
		}
		else
		{
			m_DialogOpen = true;
		}

		return m_DialogOpen;
	}


	int CommonDialog::Update(void)
	{
		SceCommonDialogStatus	cdStatus;
		SceMsgDialogResult		msgResult;

		int		res = SCE_OK;
		int		term_res = SCE_OK;

		if (m_DialogInitialized == false) return 0;

		// Get message dialog status
		cdStatus = sceMsgDialogUpdateStatus();

		if(m_DialogNeedsClosing && cdStatus == SCE_COMMON_DIALOG_STATUS_RUNNING )
		{
			// terminate message dialog
			int ret = sceMsgDialogClose();
			if (ret != SCE_OK)
			{
				Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
			}
			else
			{
				m_DialogNeedsClosing = false;
			}
		}

		switch (cdStatus)
		{
			default:
			case SCE_COMMON_DIALOG_STATUS_NONE:
			case SCE_COMMON_DIALOG_STATUS_RUNNING:
				return cdStatus;

			case SCE_COMMON_DIALOG_STATUS_FINISHED:
				break;
		}

		// Get message dialog result
		memset(&msgResult, 0, sizeof(SceMsgDialogResult));
		res = sceMsgDialogGetResult(&msgResult);	// returns msgResult.result
		if (res < 0)	// values >=0 are fine
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(res));
		}


		// Terminate message dialog
		term_res = sceMsgDialogTerminate();
		if (term_res != SCE_OK)
		{
			Messages::LogError("CommonDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(term_res));
			res = term_res;
		}
		
		m_DialogInitialized = false;
		int dialogmode = m_DialogMode; // we should be able to use msgResult.mode here, but it seems to be incorrectly set to zero sometimes
			
		if(msgResult.result == 0)
		{
			switch(dialogmode)
			{
				case SCE_MSG_DIALOG_MODE_USER_MSG:
					switch(m_UserMessageType)
					{
						case SCE_MSG_DIALOG_BUTTON_TYPE_OK:
							m_DialogResult = RESULT_BUTTON_OK;
							break;

						case SCE_MSG_DIALOG_BUTTON_TYPE_YESNO:
						case SCE_MSG_DIALOG_BUTTON_TYPE_YESNO_FOCUS_NO:
							m_DialogResult = msgResult.buttonId == SCE_MSG_DIALOG_BUTTON_ID_YES ? RESULT_BUTTON_YES : RESULT_BUTTON_NO;
							break;

						case SCE_MSG_DIALOG_BUTTON_TYPE_NONE:
						case SCE_MSG_DIALOG_BUTTON_TYPE_WAIT:
						case SCE_MSG_DIALOG_BUTTON_TYPE_WAIT_CANCEL:
							break;

						case SCE_MSG_DIALOG_BUTTON_TYPE_OK_CANCEL:
						case SCE_MSG_DIALOG_BUTTON_TYPE_OK_CANCEL_FOCUS_CANCEL:
							m_DialogResult = msgResult.buttonId == SCE_MSG_DIALOG_BUTTON_ID_OK ? RESULT_BUTTON_OK : RESULT_BUTTON_CANCEL;
							break;

						case SCE_MSG_DIALOG_BUTTON_TYPE_CANCEL:
							m_DialogResult = RESULT_BUTTON_CANCEL;
							break;

						case SCE_MSG_DIALOG_BUTTON_TYPE_2BUTTONS:
							m_DialogResult = msgResult.buttonId == SCE_MSG_DIALOG_BUTTON_ID_OK ? RESULT_BUTTON_OK : RESULT_BUTTON_CANCEL;
							break;


						default:
							Messages::LogError("CommonDialog::%s@L%d - button type not handled %d, mode:%d result:%d buttonId:%d", __FUNCTION__, __LINE__, m_UserMessageType, msgResult.mode, msgResult.result, msgResult.buttonId);
							break;
					}
					break;

				case SCE_MSG_DIALOG_MODE_SYSTEM_MSG:
					switch(m_SystemMessageType)
					{
#if __PROSPERO__
						case SCE_MSG_DIALOG_SYSMSG_TYPE_EMPTY_STORE:
						case SCE_MSG_DIALOG_SYSMSG_TYPE_CAMERA_NOT_CONNECTED:
						case SCE_MSG_DIALOG_SYSMSG_TYPE_PSN_COMMUNICATION_RESTRICTION:
							m_DialogResult = RESULT_BUTTON_OK;
							break;
#else
						case SCE_MSG_DIALOG_SYSMSG_TYPE_TRC_EMPTY_STORE:
						case SCE_MSG_DIALOG_SYSMSG_TYPE_TRC_PSN_CHAT_RESTRICTION:
						case SCE_MSG_DIALOG_SYSMSG_TYPE_TRC_PSN_UGC_RESTRICTION:
#if (SCE_ORBIS_SDK_VERSION < 0x03500000u)
						case SCE_MSG_DIALOG_SYSMSG_TYPE_TRC_WARNING_SWITCH_TO_SIMULVIEW:
#endif
						case SCE_MSG_DIALOG_SYSMSG_TYPE_CAMERA_NOT_CONNECTED:
						case SCE_MSG_DIALOG_SYSMSG_TYPE_WARNING_PROFILE_PICTURE_AND_NAME_NOT_SHARED:
#endif
							m_DialogResult = RESULT_BUTTON_OK;
							break;
					}
					break;

				case SCE_MSG_DIALOG_MODE_PROGRESS_BAR:
					break;
			}
		}
		else if(msgResult.result > 0)
		{
			switch(msgResult.result)
			{
				case SCE_COMMON_DIALOG_RESULT_USER_CANCELED:
					m_DialogResult = RESULT_CANCELED;
					break;
			}
		}

		//Messages::Log("Dialog closed, msgResult: %d, 0x%x, %d, dlgResult: %d",msgResult.mode, msgResult.result, msgResult.buttonId, m_DialogResult);

		if(m_DialogResult != RESULT_NOT_SET)
		{
			Messages::PluginMessage* msg = new Messages::PluginMessage();
			msg->type = Messages::kDialog_GotDialogResult;
			msg->SetData(m_DialogResult);
			Messages::AddMessage(msg);
		}

		m_DialogOpen = false;
		return res;
	}
	
	int CommonDialog::GetResult()
	{
		if(m_DialogOpen)
		{
			Messages::Log("CommonDialog::GetResult dialog still open\n");
			return false;
		}

		return m_DialogResult;
	}

} // namespace UnityCommonDialog
