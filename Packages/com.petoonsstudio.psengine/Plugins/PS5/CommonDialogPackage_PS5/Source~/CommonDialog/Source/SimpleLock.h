#ifndef _SIMPLELOCK_H
#define _SIMPLELOCK_H

class NonCopyable
{
public:
	NonCopyable() {}

private:
	NonCopyable(const NonCopyable&);
	NonCopyable& operator=(const NonCopyable&);
};

#include <kernel.h>
#include <sce_atomic.h>

class PlatformSemaphore : public NonCopyable
{
	friend class Semaphore;
protected:
	void Create()
	{
		static uint32_t semaphoreCount = 0;
		char name[26];
		snprintf(name, 26, "UnitySemaphore%d", semaphoreCount++);
		int result = sceKernelCreateSema(&m_Semaphore, name, 0, 0, 128, NULL);
		Assert(!(result < SCE_OK));
		(void)result;
	}
	void Destroy()
	{
		int result;
		result = sceKernelDeleteSema(m_Semaphore);
		Assert(!(result < SCE_OK));
		(void)result;
	}

	void WaitForSignal()
	{
		int result;
		result = sceKernelWaitSema(m_Semaphore, 1, NULL);
		Assert(!(result < SCE_OK));
		(void)result;
	}
	void Signal()
	{
		int result = sceKernelSignalSema(m_Semaphore, 1);
		Assert(!(result < SCE_OK));
		(void)result;
	}

private:
	SceKernelSema	m_Semaphore;
};

class Semaphore : public NonCopyable
{
public:
	Semaphore() { m_Semaphore.Create(); }
	~Semaphore() { m_Semaphore.Destroy(); }
	void Reset() { m_Semaphore.Destroy(); m_Semaphore.Create(); }
	void WaitForSignal() { m_Semaphore.WaitForSignal(); }
	void Signal() { m_Semaphore.Signal(); }

private:
	PlatformSemaphore m_Semaphore;
};

// AtomicAdd - Returns the new value, after the operation has been performed (as defined by OSAtomicAdd32Barrier)
inline int AtomicAdd (int volatile* i, int value)
{
	return sceAtomicAdd32((int32_t*)i, value) + value;		// on psp2 it returns the pre-increment value
}

// AtomicIncrement - Returns the new value, after the operation has been performed (as defined by OSAtomicAdd32Barrier)
inline int AtomicIncrement (int volatile* i)
{
	return AtomicAdd(i, 1);
}

// AtomicDecrement - Returns the new value, after the operation has been performed (as defined by OSAtomicAdd32Barrier)
inline int AtomicDecrement (int volatile* i)
{
	return AtomicAdd(i, -1);
}

class SimpleLock : public NonCopyable
{
public:
	SimpleLock() : m_Count(0) {}

	class AutoLock : public NonCopyable
	{
	public:
		AutoLock( SimpleLock& lock ) : m_Lock(lock)
		{
			m_Lock.Lock();
		}

		~AutoLock()
		{
			m_Lock.Unlock();
		}

	private:
		SimpleLock& m_Lock;
	};

	void Lock()
	{
		if (AtomicIncrement(&m_Count) != 1)
			m_Semaphore.WaitForSignal();
	}

	void Unlock()
	{
		if (AtomicDecrement(&m_Count) != 0)
			m_Semaphore.Signal();
	}

private:
	volatile int m_Count;
	Semaphore m_Semaphore;
};

#endif // _SIMPLELOCK_H
