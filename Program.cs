using System;
using System.Runtime;

namespace Trout
{
    class Program
    {
        static void Main(string[] args)
        {
            // Improve garbage collector performance at the cost of memory usage.
            // Engine should not allocate much memory when searching a position since it references pre-allocated objects.
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            using (UciStream uciStream = new UciStream())
            {
                try
                {
                    uciStream.Run();
                }
                catch (Exception exception)
                {
                    uciStream.HandleException(exception);
                }
            }

        }
    }
}
