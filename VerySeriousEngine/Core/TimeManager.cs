using System.Diagnostics;

namespace VerySeriousEngine.Core
{
    public class TimeManager
    {
        private readonly Stopwatch clock;
        private float prevFrameTime;

        public float FrameTime { get; private set; }

        public TimeManager()
        {
            clock = new Stopwatch();
        }

        public void Setup()
        {
            clock.Start();
            prevFrameTime = clock.ElapsedMilliseconds / 1000.0f;
        }

        public void UpdateFrameTime()
        {
            float nowTime = clock.ElapsedMilliseconds / 1000.0f;
            FrameTime = nowTime - prevFrameTime;
            prevFrameTime = nowTime;
        }
    }
}
