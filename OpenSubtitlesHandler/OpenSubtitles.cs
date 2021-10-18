using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OpenSubtitlesHandler.Models;
using OpenSubtitlesHandler.Models.Responses;

namespace OpenSubtitlesHandler
{
    /// <summary>
    /// The open subtitles helper class.
    /// </summary>
    public static class OpenSubtitles
    {
        private static readonly CultureInfo _usCulture = CultureInfo.ReadOnly(new CultureInfo("en-US"));

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="apiKey">The api key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The api response.</returns>
        public static async Task<ApiResponse<LoginInfo>> LogInAsync(string username, string password, string apiKey, CancellationToken cancellationToken)
        {
            var body = new { username, password };
            var response = await RequestHandler.SendRequestAsync("/login", HttpMethod.Post, body, null, apiKey, cancellationToken).ConfigureAwait(false);

            return new ApiResponse<LoginInfo>(response);
        }

        /// <summary>
        /// Logout.
        /// </summary>
        /// <param name="user">The user information.</param>
        /// <param name="apiKey">The api key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>logout status.</returns>
        public static async Task<bool> LogOutAsync(LoginInfo user, string apiKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(user.Token))
            {
                throw new ArgumentNullException(nameof(user.Token), "Token is null or empty");
            }

            var headers = new Dictionary<string, string> { { "Authorization", user.Token } };

            var response = await RequestHandler.SendRequestAsync("/logout", HttpMethod.Delete, null, headers, apiKey, cancellationToken).ConfigureAwait(false);

            return new ApiResponse<object>(response).Ok;
        }

        /// <summary>
        /// Get user info.
        /// </summary>
        /// <param name="user">The user information.</param>
        /// <param name="apiKey">The api key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The encapsulated user info.</returns>
        public static async Task<ApiResponse<EncapsulatedUserInfo>> GetUserInfo(LoginInfo user, string apiKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(user.Token))
            {
                throw new ArgumentNullException(nameof(user.Token), "Token is null or empty");
            }

            var headers = new Dictionary<string, string> { { "Authorization", user.Token } };

            var response = await RequestHandler.SendRequestAsync("/infos/user", HttpMethod.Get, null, headers, apiKey, cancellationToken).ConfigureAwait(false);

            return new ApiResponse<EncapsulatedUserInfo>(response);
        }

        /// <summary>
        /// Get the subtitle link.
        /// </summary>
        /// <param name="file">The subtitle file.</param>
        /// <param name="user">The user information.</param>
        /// <param name="apiKey">The api key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The subtitle download info.</returns>
        public static async Task<ApiResponse<SubtitleDownloadInfo>> GetSubtitleLinkAsync(int file, LoginInfo user, string apiKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(user.Token))
            {
                throw new ArgumentNullException(nameof(user.Token), "Token is null or empty");
            }

            var headers = new Dictionary<string, string> { { "Authorization", user.Token } };

            var body = new { file_id = file };
            var response = await RequestHandler.SendRequestAsync("/download", HttpMethod.Post, body, headers, apiKey, cancellationToken).ConfigureAwait(false);

            return new ApiResponse<SubtitleDownloadInfo>(response, $"file id: {file}");
        }

        /// <summary>
        /// Download subtitle.
        /// </summary>
        /// <param name="url">the subtitle url.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The subtitle string.</returns>
        public static async Task<ApiResponse<string>> DownloadSubtitleAsync(string url, CancellationToken cancellationToken)
        {
            var response = await RequestHandler.SendRequestAsync(url, HttpMethod.Get, null, null, null, cancellationToken).ConfigureAwait(false);

            return new ApiResponse<string>(response);
        }

        /// <summary>
        /// Search for subtitle.
        /// </summary>
        /// <param name="options">The search options.</param>
        /// <param name="apiKey">The api key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of response data.</returns>
        public static async Task<ApiResponse<IReadOnlyList<ResponseData>>> SearchSubtitlesAsync(Dictionary<string, string> options, string apiKey, CancellationToken cancellationToken)
        {
            var opts = System.Web.HttpUtility.ParseQueryString(string.Empty);

            var max = -1;
            var current = 1;

            List<ResponseData> final = new ();
            ApiResponse<SearchResult> last;
            HttpResponse response;

            do
            {
                opts.Clear();

                if (current > 1)
                {
                    options["page"] = current.ToString(_usCulture);
                }

                foreach (var (key, value) in options.OrderBy(x => x.Key))
                {
                    opts.Add(key.ToLower(_usCulture), value.ToLower(_usCulture));
                }

                response = await RequestHandler.SendRequestAsync($"/subtitles?{opts}", HttpMethod.Get, null, null, apiKey, cancellationToken).ConfigureAwait(false);

                last = new ApiResponse<SearchResult>(response, $"query: {opts}", $"page: {current}");

                if (!last.Ok || last.Data == null)
                {
                    break;
                }

                if (last.Data.TotalPages == 0)
                {
                    break;
                }

                if (max == -1)
                {
                    max = last.Data.TotalPages;
                }

                current = last.Data.Page + 1;

                final.AddRange(last.Data.Data);
            }
            while (current <= max && last.Data.Data.Count == 100);

            return new ApiResponse<IReadOnlyList<ResponseData>>(final, response);
        }

        /// <summary>
        /// Get language list.
        /// </summary>
        /// <param name="apiKey">The api key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of languages.</returns>
        public static async Task<ApiResponse<EncapsulatedLanguageList>> GetLanguageList(string apiKey, CancellationToken cancellationToken)
        {
            var response = await RequestHandler.SendRequestAsync("/infos/languages", HttpMethod.Get, null, null, apiKey, cancellationToken).ConfigureAwait(false);

            return new ApiResponse<EncapsulatedLanguageList>(response);
        }
    }
}
