﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Couchbase.Linq.Execution.StreamedData;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace Couchbase.Linq.Operators
{
    /// <summary>
    /// Result operator for SumAsync.
    /// </summary>
    internal class SumAsyncResultOperator : AsyncValueFromSequenceResultOperatorBase
    {
        /// <inheritdoc />
        public override ResultOperatorBase Clone(CloneContext cloneContext) =>
            new SumAsyncResultOperator();

        /// <inheritdoc />
        public override AsyncStreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var typedSumMethod = typeof(Enumerable).GetMethod("Sum", new[] {typeof(IEnumerable<T>)});
            if (typedSumMethod == null)
            {
                throw new NotSupportedException($"No in-memory sum method found for type {typeof(T).FullName}.");
            }

            var sequence = input.GetTypedSequence<T>();
            var result = typedSumMethod.Invoke(null, new object[] {sequence});
            return new AsyncStreamedValue(Task.FromResult(result), GetOutputDataInfo(input.DataInfo));
        }

        /// <inheritdoc />
        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            if (inputInfo == null)
            {
                throw new ArgumentNullException(nameof(inputInfo));
            }
            if (!(inputInfo is StreamedSequenceInfo streamedSequenceInfo))
            {
                throw new ArgumentException($"{nameof(inputInfo)} must be of type {typeof(StreamedSequenceInfo)}");
            }

            return GetOutputDataInfo(streamedSequenceInfo);
        }

        protected AsyncStreamedValueInfo GetOutputDataInfo(StreamedSequenceInfo streamedSequenceInfo)
        {
            if (streamedSequenceInfo == null)
            {
                throw new ArgumentNullException(nameof(streamedSequenceInfo));
            }

            var resultType = typeof(Task<>).MakeGenericType(streamedSequenceInfo.ResultItemType);

            return new AsyncStreamedScalarValueInfo(resultType);
        }

        /// <inheritdoc />
        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }
    }
}
