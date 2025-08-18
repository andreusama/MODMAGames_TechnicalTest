using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XStoreQueryResult
    {
        internal XStoreQueryResult(XStoreProductQueryHandle queryHandle, XStoreProduct[] pageItems, bool hasMorePages)
        {
            this.QueryHandle = queryHandle;
            this.PageItems = pageItems;
            this.HasMorePages = hasMorePages;
        }

        internal XStoreProductQueryHandle QueryHandle { get; }

        public bool HasMorePages { get; }

        public XStoreProduct[] PageItems { get; }
    }
}
