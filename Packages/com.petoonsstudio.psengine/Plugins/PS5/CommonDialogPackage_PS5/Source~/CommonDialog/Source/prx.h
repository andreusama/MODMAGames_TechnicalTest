#ifndef _PRX_H
#define _PRX_H

#define PRX_EXPORT extern "C" __declspec (dllexport)

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <libdbg.h>
#include <assert.h>
#define Assert assert

#if NDEBUG
#define UNITY_TRACE(...)
#define UNITY_TRACEIF(condition, ...)
#else
#define UNITY_TRACE(...)				printf(__VA_ARGS__)
#define UNITY_TRACEIF(condition, ...)	if (condition) printf(__VA_ARGS__)
#endif

#endif

