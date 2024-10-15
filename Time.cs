using System;

namespace MyGame
{
    public struct Time
    {
        private static DateTime _startTime;
        private static float _lastTimeFrame;
        public static float DeltaTime;

        public static void Initialize()
        {
            _startTime = DateTime.UtcNow;
            _lastTimeFrame = 0f;
        }

        public static void Update()
        {
            float currentTime = (float)(DateTime.UtcNow - _startTime).TotalSeconds;
            DeltaTime = currentTime - _lastTimeFrame;
            _lastTimeFrame = currentTime;
        }
    }
}
