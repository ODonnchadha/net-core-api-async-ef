using Books.Api.ExternalModels;
using Books.Api.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Books.Api.Repositories
{
    public class BookCoversRepository : IBookCoversRepository
    {
        private CancellationTokenSource cancellationTokenSource;

        // New up client instance once, and then use it across requests.
        // var client = new HttpClient(); 
        // Factory allows for creating and disposing of HttpClient instances. Prefered.
        private readonly IHttpClientFactory factory;
        private readonly ILogger<BookCoversRepository> logger;

        public BookCoversRepository(IHttpClientFactory factory, ILogger<BookCoversRepository> logger)
        {
            this.factory = factory;
            this.logger = logger;
        }

        public async Task<BookCover> GetBookCoverAsync(string coverId)
        {
            var client = factory.CreateClient();
            var response = await client.GetAsync($"http://localhost:58898/api/bookcovers/{coverId}");

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<BookCover>(
                    await response.Content.ReadAsStringAsync());
            }

            return null;
        }

        public async Task<IEnumerable<BookCover>> GetBookCoversAsync(Guid bookId)
        {
            var client = factory.CreateClient();
            cancellationTokenSource = new CancellationTokenSource { };

            var bookCovers = new List<BookCover>();

            var bookCoverUrls = new[]
            {
                $"http://localhost:58898/api/bookcovers/{bookId}-DUMMY_COVER1",
                $"http://localhost:58898/api/bookcovers/{bookId}-DUMMY_COVER2",
                $"http://localhost:58898/api/bookcovers/{bookId}-DUMMY_COVER3",
                $"http://localhost:58898/api/bookcovers/{bookId}-DUMMY_COVER5",
            };

            // Deferred execution. Tasks will only start downloading when the query is evaluated.
            var downloadBookCoverTasksQuery = 
                from bookCoverUrl in bookCoverUrls
                select DownloadBookCoverAsync(client, bookCoverUrl, cancellationTokenSource.Token);

            // Begin deferred execution evaluation.
            var downloadBookCoverTasks = downloadBookCoverTasksQuery.ToList();

            try
            {
                // Once fully completed, return all.
                return await Task.WhenAll(downloadBookCoverTasks);
            }
            catch (OperationCanceledException e)
            {
                logger.LogInformation($"{e.Message}");

                downloadBookCoverTasks.ForEach(
                    t => logger.LogInformation($"Task {t.Id} has status {t.Status}"));

                return new List<BookCover> { };
            }
        }

        private async Task<BookCover> DownloadBookCoverAsync(
            HttpClient httpClient, string bookCoverUrl, CancellationToken cancellationToken)
        {
            // CancellationToken method overload will cancel the download: TaskCancelledException
            var response = await httpClient.GetAsync(bookCoverUrl, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var bookCover = JsonConvert.DeserializeObject<BookCover>(
                    await response.Content.ReadAsStringAsync());

                return bookCover;
            }

            // This will only cancel the given task that fails. Not the entire request.
            cancellationTokenSource.Cancel();

            return null;
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }
        }
        #endregion
    }
}
