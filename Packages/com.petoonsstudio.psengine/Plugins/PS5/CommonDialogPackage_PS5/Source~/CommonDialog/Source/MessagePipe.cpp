#include <stdlib.h>
#include <stdio.h>
#include <stdarg.h>
#include <string.h>
#include <list>

#include "prx.h"
#include "SimpleLock.h"
#include "MessagePipe.h"

PRX_EXPORT bool PrxCommonDialogHasMessage()
{
	return UnityCommonDialog::Messages::HasMessage();
}

PRX_EXPORT bool PrxCommonDialogGetFirstMessage(UnityCommonDialog::Messages::PluginMessage* result)
{
	return UnityCommonDialog::Messages::GetFirstMessage(result);
}

PRX_EXPORT bool PrxCommonDialogRemoveFirstMessage()
{
	return UnityCommonDialog::Messages::RemoveFirstMessage();
}

namespace UnityCommonDialog
{
	namespace Messages
	{
		std::list<PluginMessage*> messages;
		SimpleLock lockMessages;

		void AddLogMessage(int type, const char* format, va_list alist)
		{
			char buffer[256];

			vsnprintf (buffer,256,format, alist);
			perror (buffer);

			PluginMessage* msg = new PluginMessage();
			msg->type = type;
			msg->SetDataFromString(buffer);
			AddMessage(msg);
		}

		void Log(const char* format, ...)
		{
			char buffer[1024];
			va_list args;
			va_start (args, format);
			vsnprintf (buffer, 1024, format, args);
			va_end (args);

			PluginMessage* msg = new PluginMessage();
			msg->type = kDialog_Log;
			msg->SetDataFromString(buffer);
			AddMessage(msg);
			UNITY_TRACE("%s",buffer);
		}

		void LogWarning(const char* format, ...)
		{
			char buffer[1024];
			va_list args;
			va_start (args, format);
			vsnprintf (buffer, 1024, format, args);
			va_end (args);

			PluginMessage* msg = new PluginMessage();
			msg->type = kDialog_LogWarning;
			msg->SetDataFromString(buffer);
			AddMessage(msg);
			UNITY_TRACE("WARNING: %s\n", buffer);
		}

		void LogError(const char* format, ...)
		{
			char buffer[1024];
			va_list args;
			va_start (args, format);
			vsnprintf (buffer, 1024, format, args);
			va_end (args);

			PluginMessage* msg = new PluginMessage();
			msg->type = kDialog_LogError;
			msg->SetDataFromString(buffer);
			AddMessage(msg);
			UNITY_TRACE("ERROR: %s\n", buffer);
		}

		void AddMessage(MessageType msgType)
		{
			PluginMessage* msg = new UnityCommonDialog::Messages::PluginMessage();
			msg->type = msgType;
			UnityCommonDialog::Messages::AddMessage(msg);
		}
	
		void AddMessage(PluginMessage* msg)
		{
			SimpleLock::AutoLock lock(lockMessages);
			messages.push_back(msg);
		}

		bool HasMessage()
		{
			SimpleLock::AutoLock lock(lockMessages);
			return !messages.empty();
		}

		bool GetFirstMessage(PluginMessage* msg)
		{
			SimpleLock::AutoLock lock(lockMessages);
			if(!messages.empty())
			{
				*msg = *messages.front();
				return true;
			}

			return false;
		}

		bool RemoveFirstMessage()
		{
			SimpleLock::AutoLock lock(lockMessages);
			if(!messages.empty())
			{
				delete messages.front();
				messages.pop_front();
				return true;
			}

			return false;
		}

	}
} // namespace UnityCommonDialog
