#include <stdio.h>
#include <kernel.h>

#include <libsysmodule.h>

#include "prx.h"
#include "CommonDialog.h"
#include "ImeDialog.h"
#include "ErrorDialog.h"
#include "SigninDialog.h"
#include "WebBrowser.h"
#include "VrSetupDialog.h"
// standard results for module startup
#define SCE_KERNEL_START_SUCCESS		(0)
#define SCE_KERNEL_START_FAILED			(2)

extern "C" int module_start(size_t sz, const void*arg)
{
	return SCE_KERNEL_START_SUCCESS;
}

PRX_EXPORT int PrxCommonDialogInitialise()
{
	return 0;
}

PRX_EXPORT void PrxCommonDialogUpdate()
{
	UnityCommonDialog::gDialog.Update();
	UnityCommonDialog::gImeDialog.Update();
	UnityCommonDialog::gErrorDialog.Update();
	UnityCommonDialog::gSigninDialog.Update();
	UnityCommonDialog::gWebBrowserDialog.Update();
    UnityCommonDialog::gVrSetupDialog.Update();
}
