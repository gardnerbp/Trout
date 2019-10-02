using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Trout
{
    internal class UciStream : IDisposable
    {
        public const long NodesInfoInterval = 1000000;
        public const long NodesTimeInterval = 5000;
        public Board Board;
        private string[] defaultHalfAndFullMove;
        private const int cacheSizeMegabytes = 128;
        private const int minWinPercentScale = 400;
        private const int maxWinPercentScale = 800;
        
        private Cache cache;
        private KillerMoves killerMoves;
        private MoveHistory moveHistory;
        private Evaluation evaluation;
        private Search search;
        
        private bool debug;
        private Stopwatch stopwatch;
        private Stopwatch commandStopwatch;
        private Queue<List<string>> asyncQueue;
        
        private Thread asyncThread;
        private AutoResetEvent asyncSignal;
        private object asyncLock;
        private StreamWriter logWriter;
        private object messageLock;
        private bool disposed;

        internal void Run()
        {
            throw new NotImplementedException();
        }

        internal void HandleException(Exception exception)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // DONE: dispose managed state (managed objects).
                    Board = null;
                    cache = null;
                    killerMoves = null;
                    moveHistory = null;
                    evaluation = null;
                    defaultHalfAndFullMove = null;
                    
                    lock (messageLock) { stopwatch = null; }
                    commandStopwatch = null;
                    lock (asyncLock) { asyncQueue = null; }
                    asyncThread = null;
                    asyncLock = null;
                    messageLock = null;
                }

                // DONE: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // DONE: set large fields to null.
                search?.Dispose();
                search = null;
                logWriter?.Dispose();
                logWriter = null;
                asyncSignal?.Dispose();
                asyncSignal = null;
                disposed = true;

                disposedValue = true;
            }
        }

        // DONE: Override finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~UciStream()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // DONE: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}