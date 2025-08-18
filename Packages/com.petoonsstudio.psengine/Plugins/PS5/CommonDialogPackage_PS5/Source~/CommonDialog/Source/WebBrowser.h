#ifndef _WEBBROWSERDIALOG_H
#define _WEBBROWSERDIALOG_H

#include <web_browser_dialog.h>
#include "prx.h"
#include "SimpleLock.h"

namespace UnityCommonDialog
{
	struct WebBrowserDialogResult
	{
		int result;
	};


	struct WebBrowserParam
	{
		int mode;
		int userId;
		const char * url;
		short width, height;
		short positionX, positionY;
		int parts;
		short headerWidth, headerPositionX, headerPositionY;
		int control;

		int imeOption;
		int webViewOption;

		int animation;

	};

	PRX_EXPORT bool PrxWebBrowserDialogIsDialogOpen();
	PRX_EXPORT bool PrxWebBrowserDialogOpen(WebBrowserParam *param);
	PRX_EXPORT bool PrxWebBrowserDialogGetResult(WebBrowserDialogResult* result);
	PRX_EXPORT int PrxWebBrowserDialogTerminate();

	class WebBrowserDialog
	{
		SimpleLock m_Lock;
		bool m_DialogOpen;
		bool m_DialogInitialized;	// module loaded and initialized, dialog does not have to be open

	public:
		WebBrowserDialog();
		~WebBrowserDialog();

		bool InitialiseModule();
		int Terminate();

		bool StartDialog(WebBrowserParam *param, const char *domain0, const char *domain1, const char *domain2, const char *domain3, const char *domain4 );

		int ResetCookie();
		int SetCookie(const char *url, const char *cookie);

		bool Update();
		bool IsDialogOpen() const { return m_DialogOpen; }

		// For use when calling directly from scripts.
		bool Get(WebBrowserDialogResult* result);

	private:
		// For use when calling directly from scripts.
		WebBrowserDialogResult m_CachedResult;
	};

	extern WebBrowserDialog gWebBrowserDialog;
}

#endif // _WEBBROWSERDIALOG_H
