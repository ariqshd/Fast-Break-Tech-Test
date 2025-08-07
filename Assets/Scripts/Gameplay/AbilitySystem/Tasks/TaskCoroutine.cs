using System;
using System.Threading.Tasks;

namespace Gameplay.AbilitySystem.Tasks
{
    public static class TaskCoroutine
    {
        public static async Task WaitForSeconds(float seconds)
        {
            await Task.Delay((int)(seconds * 1000));
        }
    
        public static async Task WaitForFrames(int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
            {
                await Task.Yield();
            }
        }
    
        public static async Task WaitUntil(Func<bool> condition)
        {
            while (!condition())
            {
                await Task.Yield();
            }
        }
    
        public static async Task WaitWhile(Func<bool> condition)
        {
            while (condition())
            {
                await Task.Yield();
            }
        }
    }
}