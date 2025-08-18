#ifndef _IMEDIALOG_H
#define _IMEDIALOG_H

//#include <np_toolkit.h>
#include <ime_dialog.h>
#include "prx.h"
#include "SimpleLock.h"

namespace UnityCommonDialog
{
	// IME Dialog Params, this is a class instead of a struct because we use a class in the C#
	// so that it can have a destructor to clean up memory allocated during marshaling. Strictly
	// speaking it doesn't matter in this native code but lets try and be consistent.
	//class ImeDialogParams
	//{
	//public:
	//	int supportedLanguages;
	//	bool languagesForced;
	//	SceImeType type;
	//	int option;
	//	bool canCancel;
	//	int textBoxMode;
	//	SceImeEnterLabel enterLabel;
	//	int maxTextLength;
	//	wchar_t* title;
	//	wchar_t* initialText;
	//};

	struct ImeDialogResult
	{
		int result;
		const char* text;
	};

	PRX_EXPORT bool PrxImeDialogIsDialogOpen();
	PRX_EXPORT bool PrxImeDialogOpen(SceImeDialogParam* params, SceImeParamExtended* extended);
	PRX_EXPORT bool PrxImeDialogGetResult(ImeDialogResult* result);

	typedef void(*ImeDialogResultCallback)(const SceImeDialogResult *result);

	class ImeDialog
	{
		SimpleLock m_Lock;
		bool m_DialogOpen;
		bool m_DialogInitialized;	// module loaded and initialized, dialog does not have to be open

		//wchar_t m_ImeTitle[SCE_IME_DIALOG_MAX_TITLE_LENGTH];
		//SceUInt32 m_ImeMaxTextLength;
		//wchar_t m_ImeInitialText[SCE_IME_DIALOG_MAX_TEXT_LENGTH];
		//wchar_t *m_ImeInputTextBuffer; /* needs (maxTextLength + 1) */

		//float m_ImePosx;
		//float m_ImePosy;
		//SceImeHorizontalAlignment m_ImeHorizontalAlignment;
		//SceImeVerticalAlignment m_ImeVerticalAlignment;
		//wchar_t * m_ImePlaceholder;


		ImeDialogResultCallback m_ResultCallback;

		// For use when calling directly from scripts.
		wchar_t * m_DefaultInputBuffer;		// UTF-16 format input buffer
		size_t m_InputBufferSize;			// size of the input buffer in bytes
		size_t m_MaxNumChars;				// Max number of characters we are currently allowed to enter in the dialog

		char* m_DefaultResultString;		// UTF-8 result string

		static void DefaultResultCallback(const SceImeDialogResult *result);

	public:
		struct Params
		{
			SceImeDialogParam params;
			SceImeParamExtended extended;

			ImeDialogResultCallback callback;
		};

		ImeDialog();
		~ImeDialog();

		bool StartDialog(const SceImeDialogParam* params, const SceImeParamExtended *extended);
		bool StartDialog(Params& params);
		bool Update();
		bool IsDialogOpen() const { return m_DialogOpen; }

		// For use when calling directly from scripts.
		bool Get(ImeDialogResult* result);

	private:
		void TerminateDialog();

		// For use when calling directly from scripts.
		Params m_CachedParams;
		ImeDialogResult m_CachedResult;
	};

	extern ImeDialog gImeDialog;
} // namespace UnityCommonDialog

#endif // _IMEDIALOG_H
