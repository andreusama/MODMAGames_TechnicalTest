using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Unity.GameCore.Interop
{
    //typedef struct XblTitleStorageBlobMetadata
    //{
    //    _Null_terminated_ char blobPath[XBL_TITLE_STORAGE_BLOB_PATH_MAX_LENGTH];
    //    XblTitleStorageBlobType blobType;
    //    XblTitleStorageType storageType;
    //    _Null_terminated_ char displayName[XBL_TITLE_STORAGE_BLOB_DISPLAY_NAME_MAX_LENGTH];
    //    _Null_terminated_ char eTag[XBL_TITLE_STORAGE_BLOB_ETAG_MAX_LENGTH];
    //    time_t clientTimestamp;
    //    size_t length;
    //    _Null_terminated_ char serviceConfigurationId[XBL_SCID_LENGTH];
    //    uint64_t xboxUserId;
    //}
    //XblTitleStorageBlobMetadata;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XblTitleStorageBlobMetadata
    {
        private unsafe fixed Byte blobPath[XblInterop.XBL_TITLE_STORAGE_BLOB_PATH_MAX_LENGTH];
        internal XblTitleStorageBlobType blobType;
        internal XblTitleStorageType storageType;
        private unsafe fixed Byte displayName[XblInterop.XBL_TITLE_STORAGE_BLOB_DISPLAY_NAME_MAX_LENGTH];
        private unsafe fixed Byte eTag[XblInterop.XBL_TITLE_STORAGE_BLOB_ETAG_MAX_LENGTH];
        internal TimeT clientTimestamp;
        internal SizeT length;
        private unsafe fixed Byte serviceConfigurationId[XblInterop.XBL_SCID_LENGTH];
        internal UInt64 xboxUserId;

        internal string GetBlobPath() { unsafe { fixed (Byte* ptr = this.blobPath) { return Converters.BytePointerToString(ptr, XblInterop.XBL_TITLE_STORAGE_BLOB_PATH_MAX_LENGTH); } } }
        internal string GetDisplayName() { unsafe { fixed (Byte* ptr = this.displayName) { return Converters.BytePointerToString(ptr, XblInterop.XBL_TITLE_STORAGE_BLOB_DISPLAY_NAME_MAX_LENGTH); } } }
        internal string GetETag() { unsafe { fixed (Byte* ptr = this.eTag) { return Converters.BytePointerToString(ptr, XblInterop.XBL_TITLE_STORAGE_BLOB_ETAG_MAX_LENGTH); } } }
        internal string GetServiceConfigurationId() { unsafe { fixed (Byte* ptr = this.serviceConfigurationId) { return Converters.BytePointerToString(ptr, XblInterop.XBL_SCID_LENGTH); } } }
    }
}
