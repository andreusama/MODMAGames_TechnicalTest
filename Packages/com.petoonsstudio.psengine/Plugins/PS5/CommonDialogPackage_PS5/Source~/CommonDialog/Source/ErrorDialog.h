#ifndef _ERRORDIALOG_H
#define _ERRORDIALOG_H

//#include <np_toolkit.h>
#include <error_dialog.h>
#include "prx.h"
#include "SimpleLock.h"

namespace UnityCommonDialog
{
	// Error Dialog Params, this is a class instead of a struct because we use a class in the C#
	// so that it can have a destructor to clean up memory allocated during marshaling. Strictly
	// speaking it doesn't matter in this native code but lets try and be consistent.
	class ErrorDialogParams
	{
	public:
		int32_t errorCode;
		SceUserServiceUserId userId;
	};

	struct ErrorDialogResult
	{
		int result;
		int button;
		const char* text;
	};

	PRX_EXPORT bool PrxErrorDialogIsDialogOpen();
	PRX_EXPORT bool PrxErrorDialogOpen(ErrorDialogParams* params);
	PRX_EXPORT bool PrxErrorDialogGetResult(ErrorDialogResult* result);


	class ErrorDialog
	{
		SimpleLock m_Lock;
		bool m_DialogOpen;
		bool m_DialogInitialized;	// module loaded and initialized, dialog does not have to be open

	public:
		struct Params
		{
			int32_t errorCode;
			SceUserServiceUserId userId;
		};

		ErrorDialog();
		~ErrorDialog();

		bool StartDialog(const ErrorDialogParams* params);
		bool StartDialog(const Params& params);
		bool Update();
		bool IsDialogOpen() const { return m_DialogOpen; }

		// For use when calling directly from scripts.
		bool Get(ErrorDialogResult* result);

	private:
		void TerminateDialog();

		// For use when calling directly from scripts.
		Params m_CachedParams;
		ErrorDialogResult m_CachedResult;
	};

	extern ErrorDialog gErrorDialog;
}

#endif // _ERRORDIALOG_H
