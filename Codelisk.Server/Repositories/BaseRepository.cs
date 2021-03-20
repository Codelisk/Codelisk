using Codelisk.Server.Services.Interfaces;
using Polly;
using Polly.Retry;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Codelisk.Server.Repositories
{
    public abstract class BaseRepository<TApi>
    {
        private const int MAX_RETRY_ATTEMPTS = 3;
        private const int MAX_REFRESH_TOKEN_ATTEMPTS = 1;

        protected readonly AsyncRetryPolicy _retryPolicy;
        protected readonly AsyncRetryPolicy _retryPolicyOnlyOnce;
        protected readonly AsyncRetryPolicy _refreshTokenPolicy;

        protected readonly TApi _repositoryApi;
        private readonly IBaseRepositoryProvider _baseRepositoryProvider;

        protected BaseRepository(IBaseRepositoryProvider baseRepositoryProvider)
        {
            _baseRepositoryProvider = baseRepositoryProvider;
            _repositoryApi = RestService.For<TApi>(baseRepositoryProvider.GetBaseUrl());

            _refreshTokenPolicy = Policy
                .HandleInner<ApiException>(ex => ex.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(MAX_REFRESH_TOKEN_ATTEMPTS);

            _retryPolicy = Policy
                .Handle<ApiException>(StatusCodeFilter)
                .WaitAndRetryAsync(MAX_RETRY_ATTEMPTS, SleepDuration);

            _retryPolicyOnlyOnce = Policy
                .Handle<ApiException>(StatusCodeFilter)
                .WaitAndRetryAsync(0, SleepDuration);

        }

        /// <summary>
        /// Build api settings
        /// </summary>
        /// <returns>Refit Settings</returns>
        //protected virtual RefitSettings BuildApiSettings()
        //{
        //    return new RefitSettings()
        //    {
        //        ContentSerializer = new CustomJsonContentSerializer()
        //    };
        //}
        public virtual Dictionary<string, object> GetDefaultBody(string fkt, string bereich)
        {
            var postData = new Dictionary<string, object>();
            return postData;
        }


        /// <summary>
        /// Do an api request with Try catch logic and resiciance policy
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="func">Api request function</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Request result or default value when exception handle</returns>
        protected virtual async Task<T> TryRequest<T>(Func<Task<T>> func, T defaultValue = default(T))
        {
            try
            {
                var result = await RequestWithPolicy(func).ConfigureAwait(false);
                return result;
            }
            catch (ApiException ex)
            {
                PrintExceptionMessage(ex);
                return defaultValue;
            }
        }

        protected virtual async Task JustSend(Func<Task> func)
        {
            try
            {
                await RequestWithPolicy(func).ConfigureAwait(false);
            }
            catch (ApiException ex)
            {
                PrintExceptionMessage(ex);
            }
        }
        protected virtual async Task<T> TryRequestOnlyOnce<T>(Func<Task<T>> func, T defaultValue = default(T))
        {
            try
            {
                return await RequestWithPolicyOnlyOnce(func).ConfigureAwait(false);
            }
            catch (ApiException ex)
            {
                PrintExceptionMessage(ex);
                return defaultValue;
            }
        }

        /// <summary>
        /// Do an api request with resiliance policy thats will retry unitl max attemps
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="func">Api request function</param>
        /// <returns>Request result or ApiException</returns>
        protected virtual async Task<T> RequestWithPolicy<T>(Func<Task<T>> func) =>
            await _retryPolicy.WrapAsync(_refreshTokenPolicy).ExecuteAsync(func).ConfigureAwait(false);

        protected virtual async Task<T> RequestWithPolicyOnlyOnce<T>(Func<Task<T>> func) =>
            await _retryPolicyOnlyOnce.ExecuteAsync(func).ConfigureAwait(false);

        /// <summary>
        /// Do an api request with resiliance policy thats will retry unitl max attemps
        /// </summary>
        /// <param name="func">Api request function</param>
        /// <returns></returns>
        protected virtual async Task RequestWithPolicy(Func<Task> func) =>
            await _retryPolicy.WrapAsync(_refreshTokenPolicy).ExecuteAsync(func).ConfigureAwait(false);

        /// <summary>
        /// Request attempt delay logic
        /// </summary>
        /// <param name="attempt">Current attempt</param>
        /// <returns>Delay to wait for next wait</returns>
        protected virtual TimeSpan SleepDuration(int attempt) =>
            TimeSpan.FromSeconds(Math.Pow(2, attempt));

        /// <summary>
        /// Filter for resiliance
        /// </summary>
        /// <param name="ex">Api exception to get statuscode</param>
        /// <returns>True if we can do an attempt</returns>
        private static bool StatusCodeFilter(ApiException ex) =>
            ex.StatusCode != HttpStatusCode.NotFound && ex.StatusCode != HttpStatusCode.Forbidden;

        private static void PrintExceptionMessage(ApiException ex)
        {
            Debug.WriteLine(ex);
            Debug.WriteLine(ex.Content);
            Debug.WriteLine(ex.RequestMessage?.RequestUri?.AbsoluteUri);
        }
    }
}