﻿using System.Collections.Generic;

namespace OpenSubtitlesHandler.Models.Responses
{
    public class SearchResult
    {
        public int TotalPages;
        public int TotalCount;
        public string Page;
        public List<Data> Data;
    }
}