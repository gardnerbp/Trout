using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TCache
{
    // The answer as to why Lazy<T> doesn’t have built-in asynchronous support is that Lazy<T> is all about caching a value
    // and synchronizing multiple threads attempting to get at that cached value, whereas we have another type in the .NET 
    // Framework focused on representing an asynchronous operation and making its result available in the future: Task<T>.  
    // Rather than building asynchronous semantics into Lazy<T>, you can instead combine the power of Lazy<T> and Task<T> 
    // to get the best of both types!

    /// <summary>
    /// See https://blogs.msdn.microsoft.com/pfxteam/2011/01/15/asynclazyt/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncLazy<T>: Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory) :
            base(() => Task.Factory.StartNew(valueFactory))
        {
        }

        public AsyncLazy(Func<Task<T>> taskFactory) :
            base(() => Task.Factory.StartNew(taskFactory).Unwrap())
        {
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return Value.GetAwaiter();
        }
    }
}