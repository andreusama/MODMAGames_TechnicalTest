/*
Thanks to Drew Burden
https://ps4.siedev.net/forums/thread/140476/
*/

#ifndef _SIGNINDIALOG_H
#define _SIGNINDIALOG_H

#include <signin_dialog.h>
#include "prx.h"
#include "SimpleLock.h"

namespace UnityCommonDialog
{
	struct SigninDialogResult
	{
		int result;
	};

	PRX_EXPORT bool PrxSigninDialogIsDialogOpen();
	PRX_EXPORT bool PrxSigninDialogOpen(int userId);
	PRX_EXPORT bool PrxSigninDialogGetResult(SigninDialogResult* result);


	class SigninDialog
	{
		SimpleLock m_Lock;
		bool m_DialogOpen;
		bool m_DialogInitialized;	// module loaded and initialized, dialog does not have to be open

	public:
		SigninDialog();
		~SigninDialog();

		bool StartDialog(const int userId);
		bool Update();
		bool IsDialogOpen() const { return m_DialogOpen; }

		// For use when calling directly from scripts.
		bool Get(SigninDialogResult* result);

	private:
		void TerminateDialog();
		// For use when calling directly from scripts.
		SigninDialogResult m_CachedResult;
	};

	extern SigninDialog gSigninDialog;
}

#endif // _SIGNINDIALOG_H
