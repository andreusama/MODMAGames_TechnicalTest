using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAchievementUnlocker
{
    void UnlockAchievement(string achievementID);

    Task<HashSet<string>> GetUnlockedAchievements();
}
