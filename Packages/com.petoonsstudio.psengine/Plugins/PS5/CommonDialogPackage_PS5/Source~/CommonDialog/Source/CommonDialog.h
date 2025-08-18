#ifndef _COMMONDIALOG_H
#define _COMMONDIALOG_H

#include <ime_dialog.h>
#include <message_dialog.h>
#include "prx.h"
#include "SimpleLock.h"

namespace UnityCommonDialog
{
	PRX_EXPORT bool PrxCommonDialogIsDialogOpen();
	PRX_EXPORT bool PrxCommonDialogErrorMessage(int errorCode, int userId = 0);
	PRX_EXPORT bool PrxCommonDialogSystemMessage(int type, int userId);
	PRX_EXPORT bool PrxCommonDialogClose();
	PRX_EXPORT bool PrxCommonDialogProgressBar(const char *str);
	PRX_EXPORT bool PrxCommonDialogProgressBarSetPercent(int percent);
	PRX_EXPORT bool PrxCommonDialogProgressBarSetMessage(const char* str);
	PRX_EXPORT bool PrxCommonDialogUserMessage(int type, bool infobar, const char* str, const char* button1, const char* button2, const char* button3);
	PRX_EXPORT int PrxCommonDialogGetResult();

	enum CommonDialogResult
	{
		RESULT_NOT_SET,
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
	};
	
	class CommonDialog
	{
		SimpleLock m_Lock;
		bool m_DialogOpen;
		bool m_DialogInitialized;	// module loaded and initialized, dialog does not have to be open
		bool m_DialogNeedsClosing;
		int m_DialogMode;
		SceMsgDialogButtonType  m_UserMessageType;
		SceMsgDialogSystemMessageType  m_SystemMessageType;
		int m_DialogResult;

	public:
		CommonDialog();
		~CommonDialog();

		bool IsDialogOpen() const { return m_DialogOpen; }
		bool StartDialogUserMessage(int type, bool infobar, const char* str, const char* button1, const char* button2, const char* button3);
		bool StartDialogProgressBar(const char *str);
		bool SetProgressBarPercent(int percent);
		bool SetProgressBarMessage(const char* str);
		bool StartDialogSystemMessage(int type, int usedId);
		bool CloseDialog();
		int GetResult();

		int Update(void);
	private:
		void TerminateDialog();
	};

	extern CommonDialog gDialog;
}

#endif // _COMMONDIALOG_H
