#ifndef _BACKUP_H
#define _BACKUP_H

#include "../Includes/PluginCommonIncludes.h"
#include "Core.h"

namespace SaveData
{
	class Progress
	{
	public:
		static void ClearProgress(APIResult* result);
		static float GetProgress(APIResult* result);
	};
}

#endif	//_BACKUP_H

