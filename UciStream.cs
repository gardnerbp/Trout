using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TCache;

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

        private bool Log
        {
            get => logWriter != null;
            set
            {
                if (value)
                {
                    // Start logging.
                    if (logWriter == null)
                    {
                        // Create or append to log file.
                        // Include GUID in log filename to avoid multiple engine instances interleaving lines in a single log file.
                        string file = $"Trout-{Guid.NewGuid()}.log";
                        FileStream fileStream = File.Open(file, FileMode.Append, FileAccess.Write, FileShare.Read);
                        logWriter = new StreamWriter(fileStream) { AutoFlush = true };
                    }
                }
                else
                {
                    // Stop logging.
                    logWriter?.Close();
                    logWriter?.Dispose();
                    _logWriter = null;
                }
            }
        }

        public UciStream()
        {
            // Create diagnostic and synchronization objects.
            stopwatch = Stopwatch.StartNew();
            commandStopwatch = new Stopwatch();
            asyncQueue = new Queue<List<string>>();
            asyncSignal = new AutoResetEvent(false);
            asyncLock = new object();
            messageLock = new object();
            
            // Create game objects.
            // Cannot use object initializer because it changes order of object construction
            // (to PreCalculatedMoves first, Board second, which causes null reference in PrecalculatedMove.FindMagicMultipliers).
            Board = new Board(WriteMessageLine);
            Board.PrecalculatedMoves = new PrecalculatedMoves(Board.BishopMoveMasks, Board.RookMoveMasks, Board.CreateMoveDestinationsMask, WriteMessageLine);
            cache = new Cache(cacheSizeMegabytes * Cache.CapacityPerMegabyte, Board.ValidateMove);
            killerMoves = new KillerMoves(Search.MaxHorizon);
            moveHistory = new MoveHistory();
            evaluation = new Evaluation(Board.GetPositionCount, Board.IsPassedPawn, Board.IsFreePawn, () => debug, WriteMessageLine);
            search = new Search(cache, killerMoves, moveHistory, evaluation, () => debug, WriteMessageLine);
            defaultHalfAndFullMove = new[] { "0", "1" };
            Board.SetPosition(Board.StartPositionFen);
        }

        ~UciStream()
        {
            Dispose(false);
        }


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
        private object _logWriter;

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