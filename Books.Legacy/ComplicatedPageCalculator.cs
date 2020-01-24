using System.Diagnostics;

namespace Books.Legacy
{
    // Full CPU load for MILLISECONDS.
    public class ComplicatedPageCalculator
    {
        const int MILLISECONDS = 5000;
        public int CalculateBookPages()
        {
            var watch = new Stopwatch { };
            watch.Start();

            while (true)
            {
                if (watch.ElapsedMilliseconds > MILLISECONDS)
                {
                    break;
                }
            }

            return 42;
        }
    }
}
