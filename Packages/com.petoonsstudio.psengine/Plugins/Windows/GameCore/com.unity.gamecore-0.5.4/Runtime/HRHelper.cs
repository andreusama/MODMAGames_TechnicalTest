using System;

namespace Unity.GameCore
{
    public class HR
    {
        public const Int32 S_OK = 0x00000000;
        public const Int32 E_FAIL = unchecked((Int32)0x80004005);
        public const Int32 E_INVALIDARG = unchecked((Int32)0x80070057);

        // XGameErr
        public const Int32 E_DSTORAGE_BEGIN = unchecked((Int32)0x89240000L);
        public const Int32 E_DSTORAGE_END = unchecked((Int32)0x892400FFL);
        public const Int32 E_GAMERUNTIME_NOT_INITIALIZED = unchecked((Int32)0x89240100L);
        public const Int32 E_GAMERUNTIME_DLL_NOT_FOUND = unchecked((Int32)0x89240101L);
        public const Int32 E_GAMERUNTIME_VERSION_MISMATCH = unchecked((Int32)0x89240102L);
        public const Int32 E_GAMERUNTIME_WINDOW_NOT_FOREGROUND = unchecked((Int32)0x89240103L);
        public const Int32 E_GAMERUNTIME_SUSPENDED = unchecked((Int32)0x89240104L);
        public const Int32 E_GAMEUSER_MAX_USERS_ADDED = unchecked((Int32)0x89245100L);
        public const Int32 E_GAMEUSER_SIGNED_OUT = unchecked((Int32)0x89245101L);
        public const Int32 E_GAMEUSER_RESOLVE_USER_ISSUE_REQUIRED = unchecked((Int32)0x89245102L);
        public const Int32 E_GAMEUSER_DEFERRAL_NOT_AVAILABLE = unchecked((Int32)0x89245103L);
        public const Int32 E_GAMEUSER_USER_NOT_FOUND = unchecked((Int32)0x89245104L);
        public const Int32 E_GAMEUSER_NO_TOKEN_REQUIRED = unchecked((Int32)0x89245105L);
        public const Int32 E_GAMEUSER_NO_DEFAULT_USER = unchecked((Int32)0x89245106L);
        public const Int32 E_GAMEUSER_FAILED_TO_RESOLVE = unchecked((Int32)0x89245107L);
        public const Int32 E_GAMEUSER_NO_TITLE_ID = unchecked((Int32)0x89245108L);
        public const Int32 E_GAMEUSER_UNKNOWN_GAME_IDENTITY = unchecked((Int32)0x89245109L);
        public const Int32 E_GAMEUSER_NO_PACKAGE_IDENTITY = unchecked((Int32)0x89245110L);
        public const Int32 E_GAMEUSER_FAILED_TO_GET_TOKEN = unchecked((Int32)0x89245111L);
        public const Int32 E_GAMEPACKAGE_APP_NOT_PACKAGED = unchecked((Int32)0x89245200L);
        public const Int32 E_GAMEPACKAGE_NO_INSTALLED_LANGUAGES = unchecked((Int32)0x89245201L);
        public const Int32 E_GAMEPACKAGE_NO_STORE_ID = unchecked((Int32)0x89245202L);
        public const Int32 E_GAMEPACKAGE_INVALID_SELECTOR = unchecked((Int32)0x89245203L);
        public const Int32 E_GAMEPACKAGE_DOWNLOAD_REQUIRED = unchecked((Int32)0x89245204L);
        public const Int32 E_GAMEPACKAGE_NO_TAG_CHANGE = unchecked((Int32)0x89245205L);
        public const Int32 E_GAMESTORE_LICENSE_ACTION_NOT_APPLICABLE_TO_PRODUCT = unchecked((Int32)0x89245300L);
        public const Int32 E_GAMESTORE_NETWORK_ERROR = unchecked((Int32)0x89245301L);
        public const Int32 E_GAMESTORE_SERVER_ERROR = unchecked((Int32)0x89245302L);
        public const Int32 E_GAMESTORE_INSUFFICIENT_QUANTITY = unchecked((Int32)0x89245303L);
        public const Int32 E_GAMESTORE_ALREADY_PURCHASED = unchecked((Int32)0x89245304L);
        public const Int32 E_GAMESTREAMING_NOT_INITIALIZED  = unchecked((Int32)0x89245400L);
        public const Int32 E_GAMESTREAMING_CLIENT_NOT_CONNECTED = unchecked((Int32)0x89245401L);
        public const Int32 E_GAMESTREAMING_NO_DATA = unchecked((Int32)0x89245402L);
        public const Int32 E_GAMESTREAMING_NO_DATACENTER = unchecked((Int32)0x89245403L);
        public const Int32 E_GAMESTREAMING_NOT_STREAMING_CONTROLLER = unchecked((Int32)0x89245404L);

        // XGameSave
        public const Int32 E_GS_INVALID_CONTAINER_NAME = unchecked((Int32)0x80830001);
        public const Int32 E_GS_NO_ACCESS = unchecked((Int32)0x80830002);
        public const Int32 E_GS_OUT_OF_LOCAL_STORAGE = unchecked((Int32)0x80830003);
        public const Int32 E_GS_USER_CANCELED = unchecked((Int32)0x80830004);
        public const Int32 E_GS_UPDATE_TOO_BIG = unchecked((Int32)0x80830005);
        public const Int32 E_GS_QUOTA_EXCEEDED = unchecked((Int32)0x80830006);
        public const Int32 E_GS_PROVIDED_BUFFER_TOO_SMALL = unchecked((Int32)0x80830007);
        public const Int32 E_GS_BLOB_NOT_FOUND = unchecked((Int32)0x80830008);
        public const Int32 E_GS_NO_SERVICE_CONFIGURATION = unchecked((Int32)0x80830009);
        public const Int32 E_GS_CONTAINER_NOT_IN_SYNC = unchecked((Int32)0x8083000A);
        public const Int32 E_GS_CONTAINER_SYNC_FAILED = unchecked((Int32)0x8083000B);
        public const Int32 E_GS_USER_NOT_REGISTERED_IN_SERVICE = unchecked((Int32)0x8083000C);
        public const Int32 E_GS_HANDLE_EXPIRED = unchecked((Int32)0x8083000D);
        public const Int32 E_GS_ASYNC_FUNCTION_REQUIRED = unchecked((Int32)0x8083000E);

        public static bool SUCCEEDED(Int32 hr)
        {
            return hr >= 0;
        }

        public static bool FAILED(Int32 hr)
        {
            return hr < 0;
        }
    }
}
