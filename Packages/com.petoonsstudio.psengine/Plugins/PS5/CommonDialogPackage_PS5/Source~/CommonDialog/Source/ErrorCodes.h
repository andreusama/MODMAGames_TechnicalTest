#ifndef _ERRORCODES_H
#define _ERRORCODES_H

#include "prx.h"
#include "SimpleLock.h"

namespace UnityCommonDialog
{
	const char* LookupErrorCode(int errorCode);
	bool GetInitialUser(SceUserServiceUserId* userId);
}

#endif // _ERRORCODES_H
