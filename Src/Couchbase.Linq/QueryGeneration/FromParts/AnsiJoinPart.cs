﻿using System;
using System.Text;

namespace Couchbase.Linq.QueryGeneration.FromParts
{
    /// <summary>
    /// Represents the FROM part of a query with a full ANSI JOIN clause.
    /// </summary>
    internal class AnsiJoinPart : JoinPart
    {
        /// <summary>
        /// Outer key selector.
        /// </summary>
        public string OuterKey { get; set; }

        /// <summary>
        /// Inner key selector.
        /// </summary>
        public string InnerKey { get; set; }

        /// <summary>
        /// Additional predicates to apply to the inner sequence.
        /// </summary>
        public string AdditionalInnerPredicates { get; set; }

        public override void AppendToStringBuilder(StringBuilder sb)
        {
            base.AppendToStringBuilder(sb);

            sb.AppendFormat(" ON ({0} = {1})", OuterKey, InnerKey);

            if (!string.IsNullOrEmpty(AdditionalInnerPredicates))
            {
                sb.AppendFormat(" AND {0}", AdditionalInnerPredicates);
            }
        }
    }
}
