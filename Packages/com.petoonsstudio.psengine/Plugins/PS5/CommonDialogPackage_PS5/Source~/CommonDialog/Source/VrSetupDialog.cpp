#include "VrSetupDialog.h"
#include "MessagePipe.h"
#include "SigninDialog.h"
#include "ErrorCodes.h"
#include <libsysmodule.h>

namespace UnityCommonDialog
{
    VrSetupDialog gVrSetupDialog;

    PRX_EXPORT bool PrxVrSetupDialogIsDialogOpen()
    {
        return gVrSetupDialog.IsDialogOpen();
    }

    PRX_EXPORT bool PrxVrSetupDialogOpen()
    {
        return gVrSetupDialog.StartDialog();
    }

    PRX_EXPORT bool PrxVrSetupDialogClose()
    {
        return gVrSetupDialog.CloseDialog();
    }

    PRX_EXPORT int PrxVrSetupDialogGetResult()
    {
        return gVrSetupDialog.GetResult();
    }

    VrSetupDialog::VrSetupDialog() : m_DialogOpen(false), m_DialogInitialized(false), m_DialogNeedsClosing(false), m_DialogResult(VR_SETUP_DIALOG_RESULT_NOT_SET)
    {
    }

    VrSetupDialog::~VrSetupDialog()
    {
    }

    bool VrSetupDialog::StartDialog()
    {
        if (m_DialogOpen)
        {
            Messages::Log("VrSetupDialog::StartDialog - VrSetupDialog is already open.\n");
            return false;
        }

        m_DialogResult = VR_SETUP_DIALOG_RESULT_NOT_SET;

        int32_t ret = sceSysmoduleIsLoaded(SCE_SYSMODULE_VR_SETUP_DIALOG);
        if (ret != SCE_OK)
        {
            if (ret == SCE_SYSMODULE_ERROR_UNLOADED)
            {
                ret = sceSysmoduleLoadModule(SCE_SYSMODULE_VR_SETUP_DIALOG);
            }

            if (ret != SCE_OK)
            {
                Messages::LogError("VrSetupDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
                return false;
            }
        }

        ret = sceVrSetupDialogInitialize();
        if (ret != SCE_OK)
        {
            Messages::LogError("VrSetupDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
            return false;
        }
        m_DialogInitialized = true;

        SceVrSetupDialogParam vrSetupParams;
        sceVrSetupDialogParamInitialize(&vrSetupParams);

        ret = sceVrSetupDialogOpen(&vrSetupParams);
        if (ret < 0)
        {
            Messages::LogError("VrSetupDialog::%s::sceVrSetupDialogOpen::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
            TerminateDialog();
            m_DialogOpen = false;
        }
        else
        {
            m_DialogOpen = true;
        }

        return m_DialogOpen;
    }

    bool VrSetupDialog::CloseDialog()
    {
        if (!m_DialogOpen)
        {
            Messages::Log("VrSetupDialog::CloseDialog - VrSetupDialog is not open.\n");
            return false;
        }

        m_DialogNeedsClosing = true;

        return true;
    }

    void VrSetupDialog::Update()
    {
        if (m_DialogInitialized == false) return;

        SceVrSetupDialogStatus status = sceVrSetupDialogUpdateStatus();
        if (status == SCE_VR_SETUP_DIALOG_STATUS_FINISHED)
        {
            SceVrSetupDialogResult vrSetupDialogResult;
            memset(&vrSetupDialogResult, 0x00, sizeof(vrSetupDialogResult));
            int32_t resultRet = sceVrSetupDialogGetResult(&vrSetupDialogResult);
            if (resultRet < 0)
            {
                Messages::LogError("VrSetupDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(resultRet));
            }

            switch (vrSetupDialogResult.result)
            {
                case SCE_VR_SETUP_DIALOG_RESULT_OK:
                    m_DialogResult = VR_SETUP_DIALOG_RESULT_OK;
                    break;
                case SCE_VR_SETUP_DIALOG_RESULT_USER_CANCELED:
                    m_DialogResult = VR_SETUP_DIALOG_RESULT_USER_CANCELED;
                    break;
            }
            TerminateDialog();
            m_DialogOpen = false;
        }
        
        if (m_DialogNeedsClosing && status == SCE_VR_SETUP_DIALOG_STATUS_RUNNING)
        {
            int ret = sceVrSetupDialogClose();
            if (ret != SCE_OK)
            {
                Messages::LogError("VrSetupDialog::%s@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
            }
            else
            {
                m_DialogNeedsClosing = false;
            }
        }

        if (m_DialogResult != VR_SETUP_DIALOG_RESULT_NOT_SET)
        {
            Messages::PluginMessage* msg = new Messages::PluginMessage();
            msg->type = Messages::kDialog_GotVrSetupDialogResult;
            msg->SetData(m_DialogResult);
            Messages::AddMessage(msg);
        }
    }

    void VrSetupDialog::TerminateDialog()
    {
        if (m_DialogInitialized)
        {
            int32_t ret = sceVrSetupDialogTerminate();
            if (ret < 0)
            {
                Messages::LogError("VrSetupDialog::%s::sceVrSetupDialogTerminate::@L%d - %s", __FUNCTION__, __LINE__, LookupErrorCode(ret));
            }
            m_DialogInitialized = false;
        }
    }

    int VrSetupDialog::GetResult()
    {
        if (m_DialogOpen)
        {
            Messages::Log("VrSetupDialog::GetResult - dialog still open\n");
            return VR_SETUP_DIALOG_RESULT_PROCESS_NOT_FINISHED;
        }
        return m_DialogResult;
    }

    
}


