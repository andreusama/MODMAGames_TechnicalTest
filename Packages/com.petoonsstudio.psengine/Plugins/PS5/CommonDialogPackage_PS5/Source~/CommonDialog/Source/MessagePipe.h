#ifndef _MESSAGEPIPE_H
#define _MESSAGEPIPE_H

namespace UnityCommonDialog
{
	namespace Messages
	{
		enum MessageType
		{
			kDialog_NotSet,
			kDialog_Log,
			kDialog_LogWarning,
			kDialog_LogError,

			kDialog_GotDialogResult,		// Dialog has closed and the result is ready.
			kDialog_GotIMEDialogResult,		// IME Dialog has closed and the result is ready.
			kDialog_GotSigninDialogResult,	// Signin Dialog has closed and the result is ready.
			kDialog_GotWebBrowserDialogResult, // WebBrowser Dialog has closed and the result is ready.
            kDialog_GotVrSetupDialogResult //VrSetupDialog has closed and the result is ready.
		};

		struct PluginMessage
		{
			PluginMessage()
			{
				type = kDialog_NotSet;
				dataSize = 0;
				data = NULL;
			}
			~PluginMessage()
			{
				if(dataSize > 0)
				{
					free(data);
				}
			}

			void SetData(void* src, int size)
			{
				dataSize = size;
				data = (char*)malloc(dataSize);
				memcpy(data, src, dataSize);
			}

			template<typename T> void SetData(const T& val)
			{
				if (sizeof(val)==4)
				{
					dataSize = 0;
					memcpy(&data, &val, 4);
				}
				else if (sizeof(val)==8)
				{
					dataSize = 0;
					memcpy(&data, &val, 8);
				}
				else
				{
					Assert(!"bad data size");		// only 4 or 8 bytes can be forced into the void* ptr
				}

			}

			void SetDataFromString(char* src)
			{
				int len = strlen(src);
				dataSize = len + 1;
				data = malloc(dataSize);
				memcpy(data, src, dataSize);
			}

			// Message type.
			int type;

			// Message data.
			// NOTE, if dataSize != 0 then the data was allocated, if dataSize == 0 then the data void* is actually some 4byte blittable value, i.e, an int.
			int dataSize;
			void* data;
		};

		void Log(const char* format, ...);
		void LogWarning(const char* format, ...);
		void LogError(const char* format, ...);
		void AddMessage(MessageType msgType);
		void AddMessage(PluginMessage* msg);
		bool HasMessage();
		bool GetFirstMessage(PluginMessage* msg);
		bool RemoveFirstMessage();
	} // namespace Messages
} // namespace UnityCommonDialog

PRX_EXPORT bool PrxCommonDialogHasMessage();
PRX_EXPORT bool PrxCommonDialogGetFirstMessage(UnityCommonDialog::Messages::PluginMessage* result);
PRX_EXPORT bool PrxCommonDialogRemoveFirstMessage();

#endif // _MESSAGEPIPE_H
