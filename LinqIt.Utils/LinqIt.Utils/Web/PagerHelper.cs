using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Utils.Web
{
    public class PagerHelper
    {
        private readonly int _pages;

        private readonly int _firstPage;

        private readonly int _lastPage;

        public PagerHelper(int numItems, int maxPagesShown, int itemsPerPage, int currentPageNumber)
        {
            _pages = Convert.ToInt32(Math.Ceiling((double)numItems / (double)itemsPerPage));
            if (_pages == 0)
                return;

            var pagesBefore = (maxPagesShown / 2);
            _firstPage = currentPageNumber - pagesBefore;
            if (_firstPage + maxPagesShown - 1 > _pages)
                _firstPage = _pages - (maxPagesShown - 1);
            if (_firstPage < 1)
                _firstPage = 1;
            _lastPage = _firstPage + maxPagesShown - 1;
            if (_lastPage > _pages)
                _lastPage = _pages;
        }

        public int Pages { get { return _pages; } }

        public int FirstPage { get { return _firstPage; } }

        public int LastPage { get { return _lastPage; } }
    }
}
