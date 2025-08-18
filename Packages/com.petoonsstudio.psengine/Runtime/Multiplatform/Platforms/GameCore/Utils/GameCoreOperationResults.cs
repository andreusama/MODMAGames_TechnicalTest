using System;

#if UNITY_GAMECORE
using Unity.GameCore;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public static class GameCoreOperationResults
    {
#if UNITY_GAMECORE
        // "Invalid HR code" - mostly used in coroutines to wait for GDK async calls to complete using this idiom:
        //		var functionCompletedResult = PLHR.Invalid;
        //		SomeGDKFunctionAsync( gdkResult => functionCompletedResult = gdkResult );    
        //		while( PLHR.IsInvalid( functionCompletedResult ) ){ yield return null; }
        //		call has now completed 
        public const Int32 Invalid = unchecked((Int32)0xDEADBEEF);

        public static bool IsInvalid(Int32 hresult) => (Invalid == hresult);

        /// <summary>
        /// Used to print the name of the code to add more information.
        /// </summary>
        /// <param name="hresult">The operation result.</param>
        /// <returns>The string with the error coda and name.</returns>
        public static string GetName(Int32 hresult)
        {
            switch (hresult)
            {
                case HR.S_OK: return nameof(HR.S_OK);
                case HR.E_FAIL: return nameof(HR.E_FAIL);
                case HR.E_INVALIDARG: return nameof(HR.E_INVALIDARG);
                case HR.E_DSTORAGE_BEGIN: return nameof(HR.E_DSTORAGE_BEGIN);
                case HR.E_DSTORAGE_END: return nameof(HR.E_DSTORAGE_END);
                case HR.E_GAMERUNTIME_NOT_INITIALIZED: return nameof(HR.E_GAMERUNTIME_NOT_INITIALIZED);
                case HR.E_GAMERUNTIME_DLL_NOT_FOUND: return nameof(HR.E_GAMERUNTIME_DLL_NOT_FOUND);
                case HR.E_GAMERUNTIME_VERSION_MISMATCH: return nameof(HR.E_GAMERUNTIME_VERSION_MISMATCH);
                case HR.E_GAMERUNTIME_WINDOW_NOT_FOREGROUND: return nameof(HR.E_GAMERUNTIME_WINDOW_NOT_FOREGROUND);
                case HR.E_GAMERUNTIME_SUSPENDED: return nameof(HR.E_GAMERUNTIME_SUSPENDED);
                case HR.E_GAMEUSER_MAX_USERS_ADDED: return nameof(HR.E_GAMEUSER_MAX_USERS_ADDED);
                case HR.E_GAMEUSER_SIGNED_OUT: return nameof(HR.E_GAMEUSER_SIGNED_OUT);
                case HR.E_GAMEUSER_RESOLVE_USER_ISSUE_REQUIRED: return nameof(HR.E_GAMEUSER_RESOLVE_USER_ISSUE_REQUIRED);
                case HR.E_GAMEUSER_DEFERRAL_NOT_AVAILABLE: return nameof(HR.E_GAMEUSER_DEFERRAL_NOT_AVAILABLE);
                case HR.E_GAMEUSER_USER_NOT_FOUND: return nameof(HR.E_GAMEUSER_USER_NOT_FOUND);
                case HR.E_GAMEUSER_NO_TOKEN_REQUIRED: return nameof(HR.E_GAMEUSER_NO_TOKEN_REQUIRED);
                case HR.E_GAMEUSER_NO_DEFAULT_USER: return nameof(HR.E_GAMEUSER_NO_DEFAULT_USER);
                case HR.E_GAMEUSER_FAILED_TO_RESOLVE: return nameof(HR.E_GAMEUSER_FAILED_TO_RESOLVE);
                case HR.E_GAMEUSER_NO_TITLE_ID: return nameof(HR.E_GAMEUSER_NO_TITLE_ID);
                case HR.E_GAMEUSER_UNKNOWN_GAME_IDENTITY: return nameof(HR.E_GAMEUSER_UNKNOWN_GAME_IDENTITY);
                case HR.E_GAMEUSER_NO_PACKAGE_IDENTITY: return nameof(HR.E_GAMEUSER_NO_PACKAGE_IDENTITY);
                case HR.E_GAMEUSER_FAILED_TO_GET_TOKEN: return nameof(HR.E_GAMEUSER_FAILED_TO_GET_TOKEN);
                case HR.E_GAMEPACKAGE_APP_NOT_PACKAGED: return nameof(HR.E_GAMEPACKAGE_APP_NOT_PACKAGED);
                case HR.E_GAMEPACKAGE_NO_INSTALLED_LANGUAGES: return nameof(HR.E_GAMEPACKAGE_NO_INSTALLED_LANGUAGES);
                case HR.E_GAMEPACKAGE_NO_STORE_ID: return nameof(HR.E_GAMEPACKAGE_NO_STORE_ID);
                case HR.E_GAMEPACKAGE_INVALID_SELECTOR: return nameof(HR.E_GAMEPACKAGE_INVALID_SELECTOR);
                case HR.E_GAMEPACKAGE_DOWNLOAD_REQUIRED: return nameof(HR.E_GAMEPACKAGE_DOWNLOAD_REQUIRED);
                case HR.E_GAMEPACKAGE_NO_TAG_CHANGE: return nameof(HR.E_GAMEPACKAGE_NO_TAG_CHANGE);
                case HR.E_GAMESTORE_LICENSE_ACTION_NOT_APPLICABLE_TO_PRODUCT: return nameof(HR.E_GAMESTORE_LICENSE_ACTION_NOT_APPLICABLE_TO_PRODUCT);
                case HR.E_GAMESTORE_NETWORK_ERROR: return nameof(HR.E_GAMESTORE_NETWORK_ERROR);
                case HR.E_GAMESTORE_SERVER_ERROR: return nameof(HR.E_GAMESTORE_SERVER_ERROR);
                case HR.E_GAMESTORE_INSUFFICIENT_QUANTITY: return nameof(HR.E_GAMESTORE_INSUFFICIENT_QUANTITY);
                case HR.E_GAMESTORE_ALREADY_PURCHASED: return nameof(HR.E_GAMESTORE_ALREADY_PURCHASED);
                case HR.E_GAMESTREAMING_NOT_INITIALIZED: return nameof(HR.E_GAMESTREAMING_NOT_INITIALIZED);
                case HR.E_GAMESTREAMING_CLIENT_NOT_CONNECTED: return nameof(HR.E_GAMESTREAMING_CLIENT_NOT_CONNECTED);
                case HR.E_GAMESTREAMING_NO_DATA: return nameof(HR.E_GAMESTREAMING_NO_DATA);
                case HR.E_GAMESTREAMING_NO_DATACENTER: return nameof(HR.E_GAMESTREAMING_NO_DATACENTER);
                case HR.E_GAMESTREAMING_NOT_STREAMING_CONTROLLER: return nameof(HR.E_GAMESTREAMING_NOT_STREAMING_CONTROLLER);
                case HR.E_GS_INVALID_CONTAINER_NAME: return nameof(HR.E_GS_INVALID_CONTAINER_NAME);
                case HR.E_GS_NO_ACCESS: return nameof(HR.E_GS_NO_ACCESS);
                case HR.E_GS_OUT_OF_LOCAL_STORAGE: return nameof(HR.E_GS_OUT_OF_LOCAL_STORAGE);
                case HR.E_GS_USER_CANCELED: return nameof(HR.E_GS_USER_CANCELED);
                case HR.E_GS_UPDATE_TOO_BIG: return nameof(HR.E_GS_UPDATE_TOO_BIG);
                case HR.E_GS_QUOTA_EXCEEDED: return nameof(HR.E_GS_QUOTA_EXCEEDED);
                case HR.E_GS_PROVIDED_BUFFER_TOO_SMALL: return nameof(HR.E_GS_PROVIDED_BUFFER_TOO_SMALL);
                case HR.E_GS_BLOB_NOT_FOUND: return nameof(HR.E_GS_BLOB_NOT_FOUND);
                case HR.E_GS_NO_SERVICE_CONFIGURATION: return nameof(HR.E_GS_NO_SERVICE_CONFIGURATION);
                case HR.E_GS_CONTAINER_NOT_IN_SYNC: return nameof(HR.E_GS_CONTAINER_NOT_IN_SYNC);
                case HR.E_GS_CONTAINER_SYNC_FAILED: return nameof(HR.E_GS_CONTAINER_SYNC_FAILED);
                case HR.E_GS_USER_NOT_REGISTERED_IN_SERVICE: return nameof(HR.E_GS_USER_NOT_REGISTERED_IN_SERVICE);
                case HR.E_GS_HANDLE_EXPIRED: return nameof(HR.E_GS_HANDLE_EXPIRED);
                case HR.E_GS_ASYNC_FUNCTION_REQUIRED: return nameof(HR.E_GS_ASYNC_FUNCTION_REQUIRED);

                case Invalid: return nameof(Invalid);
            }

            return $"[XBOX_SERIES] Error code: {hresult:X}";
        }
#endif
    }
}
