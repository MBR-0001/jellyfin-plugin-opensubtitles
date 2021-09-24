using System.Text.Json.Serialization;

namespace OpenSubtitlesHandler.Models
{
    /// <summary>
    /// The jwt payload.
    /// </summary>
    public class JWTPayload
    {
        /// <summary>
        /// Gets or sets the expiration timestamp.
        /// </summary>
        [JsonPropertyName("exp")]
        public long Exp { get; set; }
    }
}
