using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search.Utilities
{
    public abstract class DisposableBase : IDisposable
    {
        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Disposed) 
                return;

            Cleanup();
            Disposed = true;
        }

        /// <summary>
        /// Do cleanup here
        /// </summary>
        protected abstract void Cleanup();

        #endregion

        #region Properties

        protected bool Disposed { get; private set; }

        #endregion
    }
}
