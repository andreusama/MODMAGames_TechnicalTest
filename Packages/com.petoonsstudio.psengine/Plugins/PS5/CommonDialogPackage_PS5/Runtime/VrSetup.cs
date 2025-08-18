using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sony
{
    namespace PS5
    {
        namespace Dialog
        {
            public class VrSetup
            {
                public enum VrSetupDialogResult
                {
                    RESULT_OK = 0,
                    RESULT_USER_CANCELED = 1,
                    RESULT_NOT_SET = 2,
                    RESULT_PROCESS_NOT_FINISHED = 3
                }

                [DllImport("CommonDialog")] [return: MarshalAs(UnmanagedType.I1)]
                private static extern bool PrxVrSetupDialogClose();
                [DllImport("CommonDialog")] [return: MarshalAs(UnmanagedType.I1)]
                private static extern bool PrxVrSetupDialogOpen();

                [DllImport("CommonDialog")] [return: MarshalAs(UnmanagedType.I1)]
                private static extern bool PrxVrSetupDialogIsDialogOpen();

                [DllImport("CommonDialog")]
                private static extern VrSetupDialogResult PrxVrSetupDialogGetResult();


                public static event Dialog.Messages.EventHandler OnGotVrSetupDialogResult;

                public static bool Close()
                {

                    return PrxVrSetupDialogClose();
                }

                public static bool Open()
                {

                    return PrxVrSetupDialogOpen();
                }

                public static bool IsDialogOpen
                {
                    get { return PrxVrSetupDialogIsDialogOpen(); }
                }

                public static VrSetupDialogResult GetResult()
                {
                    VrSetupDialogResult result = PrxVrSetupDialogGetResult();
                    return result;
                }

                public static void ProcessMessage(Dialog.Messages.PluginMessage msg)
                {
                    // Interpret the message and trigger corresponding events.
                    switch (msg.type)
                    {
                        case Dialog.Messages.MessageType.kDialog_GotVrSetupDialogResult:
                            if (OnGotVrSetupDialogResult != null) OnGotVrSetupDialogResult(msg);
                            break;
                    }
                }
            }
        }
    }
}
