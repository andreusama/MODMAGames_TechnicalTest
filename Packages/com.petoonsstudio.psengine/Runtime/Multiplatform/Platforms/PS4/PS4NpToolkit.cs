using System;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public class PS4NpToolkit
    {
#if UNITY_PS4
        public Sony.NP.InitResult initResult;

        public void InitialiseNpToolkit()
        {
            Debug.Log((string.Format("Initial UserId:0x{0:X}  Primary UserId:0x{1:X}", UnityEngine.PS4.Utility.initialUserId, UnityEngine.PS4.Utility.primaryUserId)));

            Sony.NP.Main.OnAsyncEvent += Main_OnAsyncEvent;

            Sony.NP.InitToolkit init = new Sony.NP.InitToolkit();

            init.threadSettings.affinity = Sony.NP.Affinity.AllCores; // Sony.NP.Affinity.Core2 | Sony.NP.Affinity.Core4;
            init.contentRestrictions.ApplyContentRestriction = false;

            // Mempools
            init.memoryPools.JsonPoolSize = 6 * 1024 * 1024;
            init.memoryPools.SslPoolSize *= 4;

            init.memoryPools.MatchingSslPoolSize *= 4;
            init.memoryPools.MatchingPoolSize *= 4;

            init.SetPushNotificationsFlags(Sony.NP.PushNotificationsFlags.NewInvitation | Sony.NP.PushNotificationsFlags.UpdateBlockedUsersList |
                                            Sony.NP.PushNotificationsFlags.UpdateFriendPresence | Sony.NP.PushNotificationsFlags.UpdateFriendsList);

            try
            {
                initResult = Sony.NP.Main.Initialize(init);

                if (initResult.Initialized == true)
                {
                    Debug.Log("NpToolkit Initialized ");
                    Debug.Log("Plugin SDK Version : " + initResult.SceSDKVersion.ToString());
                    Debug.Log("Plugin DLL Version : " + initResult.DllVersion.ToString());
                }
                else
                {
                    Debug.Log("NpToolkit not initialized ");
                }
            }
            catch (Sony.NP.NpToolkitException e)
            {
                Debug.Log("Exception During Initialization : " + e.ExtendedMessage);
            }
#if UNITY_EDITOR
            catch (DllNotFoundException e)
            {
                Debug.Log("Missing DLL Expection : " + e.Message);
                Debug.Log("The sample APP will not run in the editor.");
            }
#endif
        }

        private void Main_OnAsyncEvent(Sony.NP.NpCallbackEvent callbackEvent)
        {
            Debug.Log("Event: Service = (" + callbackEvent.Service + ") : API Called = (" + callbackEvent.ApiCalled + ") : Request Id = (" + callbackEvent.NpRequestId + ") : Calling User Id = (" + callbackEvent.UserId + ")");

            HandleAsynEvent(callbackEvent);
        }


        private void HandleAsynEvent(Sony.NP.NpCallbackEvent callbackEvent)
        {
            try
            {
                if (callbackEvent.Response != null)
                {
                    if (callbackEvent.Response.ReturnCodeValue < 0)
                    {
                        Debug.LogError("Response : " + callbackEvent.Response.ConvertReturnCodeToString(callbackEvent.ApiCalled));
                    }
                    else
                    {
                        Debug.LogError("Response : " + callbackEvent.Response.ConvertReturnCodeToString(callbackEvent.ApiCalled));
                    }

                    if (callbackEvent.Response.HasServerError)
                    {
                        OutputSeverError(callbackEvent.Response);
                    }
                }
            }
            catch (Sony.NP.NpToolkitException e)
            {
                Debug.LogError("Main_OnAsyncEvent NpToolkit Exception = " + e.ExtendedMessage);
                Console.Error.WriteLine(e.ExtendedMessage); // Output to the PS4 Stderr TTY
                Console.Error.WriteLine(e.StackTrace); // Output to the PS4 Stderr TTY
            }
            catch (Exception e)
            {
                Debug.LogError("Main_OnAsyncEvent General Exception = " + e.Message);
                Debug.LogError(e.StackTrace);
                Console.Error.WriteLine(e.StackTrace); // Output to the PS4 Stderr TTY
            }
        }

        private void OutputSeverError(Sony.NP.ResponseBase response)
        {
            if (response == null) return;

            if (response.HasServerError)
            {
                string errorMsg = String.Format("Server Error : returnCode = (0x{0:X}) : webApiNextAvailableTime = ({1}) : httpStatusCode = ({2})", response.ReturnCode, response.ServerError.WebApiNextAvailableTime, response.ServerError.HttpStatusCode);
                Debug.LogError(errorMsg);

                Debug.LogError("Server Error : jsonData = " + response.ServerError.JsonData);
            }
        }
#endif
    }
}