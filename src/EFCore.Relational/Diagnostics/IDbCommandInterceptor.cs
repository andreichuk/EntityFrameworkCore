// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Diagnostics
{
    public readonly struct InterceptionResult<TResult>
    {
        public InterceptionResult(TResult result, bool overridden)
        {
            Result = result;
            IsOverridden = overridden;
        }

        public TResult Result { get; }

        public bool IsOverridden { get; }
    }

    public interface IDbCommandInterceptor
    {
        InterceptionResult<DbDataReader>? ReaderExecuting(
            [NotNull] DbCommand command,
            [NotNull] CommandEventData eventData,
            InterceptionResult<DbDataReader>? result);

        void ScalarExecuting(
            [NotNull] DbCommand command,
            [NotNull] CommandEventData eventData,
            ref InterceptionResult<int>? result);

        void NonQueryExecuting(
            [NotNull] DbCommand command,
            [NotNull] CommandEventData eventData,
            ref InterceptionResult<object>? result);

        Task<InterceptionResult<DbDataReader>?> ReaderExecutingAsync(
            [NotNull] DbCommand command,
            [NotNull] CommandEventData eventData,
            InterceptionResult<DbDataReader>? result,
            CancellationToken cancellationToken = default);

        Task ScalarExecutingAsync(
            [NotNull] DbCommand command,
            [NotNull] CommandEventData eventData,
            ref InterceptionResult<int>? result,
            CancellationToken cancellationToken = default);

        Task NonQueryExecutingAsync(
            [NotNull] DbCommand command,
            [NotNull] CommandEventData eventData,
            ref InterceptionResult<object>? result,
            CancellationToken cancellationToken = default);

        void ReaderExecuted(
            [NotNull] DbCommand command,
            [NotNull] CommandExecutedEventData eventData,
            InterceptionResult<DbDataReader> result);

        void ScalarExecuted(
            [NotNull] DbCommand command,
            [NotNull] CommandExecutedEventData eventData,
            InterceptionResult<int> result);

        void NonQueryExecuted(
            [NotNull] DbCommand command,
            [NotNull] CommandExecutedEventData eventData,
            InterceptionResult<object> result);

        Task ReaderExecutedAsync(
            [NotNull] DbCommand command,
            [NotNull] CommandExecutedEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default);

        Task ScalarExecutedAsync(
            [NotNull] DbCommand command,
            [NotNull] CommandExecutedEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default);

        Task NonQueryExecutedAsync(
            [NotNull] DbCommand command,
            [NotNull] CommandExecutedEventData eventData,
            InterceptionResult<object> result,
            CancellationToken cancellationToken = default);
    }

    public class CompositeDbCommandInterceptor : IDbCommandInterceptor
    {
        private readonly IReadOnlyList<IDbCommandInterceptor> _interceptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeDbCommandInterceptor"></see> class
        /// .</summary>
        public CompositeDbCommandInterceptor(IReadOnlyList<IDbCommandInterceptor> interceptors)
        {
            Check.NotNull(interceptors, nameof(interceptors));

            _interceptors = interceptors;
        }

        public virtual InterceptionResult<DbDataReader>? ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader>? result)
        {
            foreach (var interceptor in _interceptors)
            {
                result = interceptor.ReaderExecuting(command, eventData, result);
            }

            return result;
        }

        public void ScalarExecuting(
            DbCommand command,
            CommandEventData eventData,
            ref InterceptionResult<int>? result)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.ScalarExecuting(command, eventData, ref result);
            }
        }

        public void NonQueryExecuting(
            DbCommand command,
            CommandEventData eventData,
            ref InterceptionResult<object>? result)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.NonQueryExecuting(command, eventData, ref result);
            }
        }

        public async Task<InterceptionResult<DbDataReader>?> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader>? result,
            CancellationToken cancellationToken = default)
        {
            foreach (var interceptor in _interceptors)
            {
                result = await interceptor.ReaderExecutingAsync(command, eventData, result, cancellationToken);
            }

            return result;
        }

        public Task ScalarExecutingAsync(
            DbCommand command, CommandEventData eventData, ref InterceptionResult<int>? result, CancellationToken cancellationToken = default)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.ReaderExecuting(command, eventData, ref result);
            }
        }

        public Task NonQueryExecutingAsync(
            DbCommand command, CommandEventData eventData, ref InterceptionResult<object>? result, CancellationToken cancellationToken = default)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.ReaderExecuting(command, eventData, ref result);
            }
        }

        public void ReaderExecuted(
            DbCommand command,
            CommandExecutedEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.ReaderExecuting(command, eventData, ref result);
            }
        }

        public void ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, InterceptionResult<int> result)
        {
            throw new System.NotImplementedException();
        }

        public void NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, InterceptionResult<object> result)
        {
            throw new System.NotImplementedException();
        }

        public Task ReaderExecutedAsync(
            DbCommand command, CommandExecutedEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task ScalarExecutedAsync(
            DbCommand command, CommandExecutedEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task NonQueryExecutedAsync(
            DbCommand command, CommandExecutedEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
