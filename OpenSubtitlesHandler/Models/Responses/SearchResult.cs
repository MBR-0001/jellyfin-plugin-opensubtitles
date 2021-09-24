<<<<<<< HEAD
﻿using System.Collections.Generic;
=======
﻿using System;
using System.Collections.Generic;
>>>>>>> f7d1ca025984caf099b44d9044bb9cac4286fa6f
using System.Text.Json.Serialization;

namespace OpenSubtitlesHandler.Models.Responses
{
    /// <summary>
    /// The search result.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Gets or sets the total page count.
        /// </summary>
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the list of response data.
        /// </summary>
        [JsonPropertyName("data")]
        public IReadOnlyList<ResponseData> Data { get; set; } = Array.Empty<ResponseData>();
    }
}
