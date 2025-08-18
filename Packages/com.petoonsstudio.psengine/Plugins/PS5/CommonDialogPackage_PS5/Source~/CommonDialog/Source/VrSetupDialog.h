#ifndef _VRSETUPDIALOG_H
#define _VRSETUPDIALOG_H

#include <vr_setup_dialog.h>
#include "prx.h"

namespace UnityCommonDialog
{
    enum VrSetupDialogResult
    {
        VR_SETUP_DIALOG_RESULT_OK = 0,
        VR_SETUP_DIALOG_RESULT_USER_CANCELED = 1,
        VR_SETUP_DIALOG_RESULT_NOT_SET = 2,
        VR_SETUP_DIALOG_RESULT_PROCESS_NOT_FINISHED = 3
    };

    PRX_EXPORT bool PrxVrSetupDialogIsDialogOpen();
    PRX_EXPORT bool PrxVrSetupDialogOpen();
    PRX_EXPORT bool PrxVrSetupDialogClose();
    PRX_EXPORT int PrxVrSetupDialogGetResult();

    class VrSetupDialog
    {
    public:
        VrSetupDialog();
        ~VrSetupDialog();

        bool StartDialog();
        bool CloseDialog();
        void Update();
        int GetResult();
        void TerminateDialog();
        bool IsDialogOpen() const { return m_DialogOpen; }
    private:
        bool m_DialogOpen;
        bool m_DialogInitialized;
        bool m_DialogNeedsClosing;
        int m_DialogResult;
    };

    extern VrSetupDialog gVrSetupDialog;
}


#endif // _VRSETUPDIALOG_H
