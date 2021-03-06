﻿/*
 * Copyright (C) 2012 - 2015 Takahisa Watanabe <linerlock@outlook.com> All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Parseq
{
	public static class Combinator
	{
		public static Parser<TToken, IEnumerable<T>> Sequence<TToken, T>(
			params Parser<TToken, T>[] parsers)
		{
			return parsers.AsEnumerable().Sequence();
		}

		public static Parser<TToken, IEnumerable<T>> Sequence<TToken, T>(
			this IEnumerable<Parser<TToken, T>> parsers)
		{
			/*
			 * InternalCombinator.Sequence(Seq.Of(parsers))
			 *   .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.Sequence_Optimized(parsers);
		}

		public static Parser<TToken, T> Choice<TToken, T>(
			params Parser<TToken, T>[] parsers)
		{
			return parsers.AsEnumerable().Choice();
		}

		public static Parser<TToken, T> Choice<TToken, T>(
			this IEnumerable<Parser<TToken, T>> parsers)
		{
			/*
			 * return InternalCombinator.Choice(Seq.Of(parsers));
			 */

			return InternalCombinator.Choice_Optimized(parsers);
		}

		public static Parser<TToken, IEnumerable<T>> Repeat<TToken, T>(
			this Parser<TToken, T> parser,
			int count)
		{
			/*
			 * return InternalCombinator.Repeat(parser, count)
			 *       .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.Repeat_Optimized(parser, count);
		}

		public static Parser<TToken, IEnumerable<T>> Many0<TToken, T>(
			this Parser<TToken, T> parser)
		{
			/*
			 * return InternalCombinator.Many0(parser)
			 *       .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.Many0_Optimized(parser);
		}

		public static Parser<TToken, IEnumerable<T>> Many1<TToken, T>(
			this Parser<TToken, T> parser)
		{
			/*
			 * return InternalCombinator.Many1(parser)
			 *       .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.Many1_Optimized(parser);
		}

		public static Parser<TToken, IEnumerable<T>> ManyTill<TToken, T>(
			this Parser<TToken, T> parser,
			Parser<TToken, Unit> terminator)
		{
			/*
			 * return InternalCombinator.ManyTill(parser, terminator)
			       .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.ManyTill_Optimized(parser, terminator);
		}

		public static Parser<TToken, IEnumerable<T>> SepBy0<TToken, T>(
			this Parser<TToken, T> parser,
			Parser<TToken, Unit> sep)
		{
			/*
			 * return InternalCombinator.SepBy0(parser, sep)
			 *     .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.SepBy0_Optimized(parser, sep);
		}

		public static Parser<TToken, IEnumerable<T>> SepBy1<TToken, T>(
			this Parser<TToken, T> parser,
			Parser<TToken, Unit> sep)
		{
			/*
			 * return InternalCombinator.SepBy1(parser, sep)
			 *     .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.SepBy1_Optimized(parser, sep);
		}

		public static Parser<TToken, IEnumerable<T>> EndBy0<TToken, T>(
			this Parser<TToken, T> parser,
			Parser<TToken, Unit> sep)
		{
			/*
			 * return InternalCombinator.EndBy0(parser, sep)
			 *     .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.EndBy0_Optimized(parser, sep);
		}

		public static Parser<TToken, IEnumerable<T>> EndBy1<TToken, T>(
			this Parser<TToken, T> parser,
			Parser<TToken, Unit> sep)
		{
			/*
			 * return InternalCombinator.EndBy1(parser, sep)
			 *     .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.EndBy1_Optimized(parser, sep);
		}

		public static Parser<TToken, IEnumerable<T>> SepEndBy0<TToken, T>(
			this Parser<TToken, T> parser,
			Parser<TToken, Unit> sep)
		{
			/*
			 * return InternalCombinator.SepEndBy0(parser, sep)
			 *     .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.SepEndBy0_Optimized(parser, sep);
		}

		public static Parser<TToken, IEnumerable<T>> SepEndBy1<TToken, T>(
			this Parser<TToken, T> parser,
			Parser<TToken, Unit> sep)
		{
			/*
			 * return InternalCombinator.SepEndBy1(parser, sep)
			 *     .Select(delayedSeq => delayedSeq.AsEnumerable());
			 */

			return InternalCombinator.SepEndBy1_Optimized(parser, sep);
		}

		public static Parser<TToken, Unit> NotFollowedBy<TToken, T>(
			this Parser<TToken, T> parser,
			string errorMessage)
		{
			return stream => parser.Attempt()(stream).Case(
				(restStream, _) =>
					Reply.Success(restStream, Unit.Instance),
				(reststream, _) =>
					Reply.Failure<TToken, Unit>(reststream, errorMessage));
		}

		public static Parser<TToken, Unit> NotFollowedBy<TToken, T>(
			this Parser<TToken, T> parser)
		{
			return parser.NotFollowedBy("Failure: Combinator.NotFollowedBy");
		}

		public static Parser<TToken, T> Chainl<TToken, T>(
			this Parser<TToken, T> parser,
			Parser<TToken, Func<T, T, T>> op)
		{
			return InternalCombinator.Chainl(parser, op);
		}

		public static Parser<TToken, T> Chainr<TToken, T>(
			this Parser<TToken, T> parser,
			Parser<TToken, Func<T, T, T>> op)
		{
			return InternalCombinator.Chainr(parser, op);
		}

		public static Parser<TToken, T> Between<TToken, T>(
			this Parser<TToken, T> parser0,
			Parser<TToken, Unit> parser1,
			Parser<TToken, Unit> parser2)
		{
			return from value1 in parser1
				from value0 in parser0
				from value2 in parser2
				select value0;
		}

		public static Parser<TToken, T> Or<TToken, T>(
			this Parser<TToken, T> parser0,
			Parser<TToken, T> parser1)
		{
			return stream => parser0(stream).Case(
				(restStream, errorMessage) =>
					parser1(stream),
				(restStream, value) =>
					Reply.Success(restStream, value));
		}

		public static Parser<TToken, IOption<T>> Optional<TToken, T>(
			this Parser<TToken, T> parser)
		{
			return stream => parser(stream).Case(
				(restStream, errorMessage) =>
					Reply.Success(stream, Option.None<T>()),
				(restStream, value) =>
					Reply.Success(restStream, Option.Some(value)));
		}

		public static Parser<TToken, T> Lookahead<TToken, T>(
			this Parser<TToken, T> parser)
		{
			return stream => parser(stream).Case(
				(restStream, errorMessage) =>
					Reply.Failure<TToken, T>(stream, errorMessage),
				(restStream, value) =>
					Reply.Success(stream, value));
		}

		public static Parser<TToken, T> Attempt<TToken, T>(
			this Parser<TToken, T> parser)
		{
			return stream => parser(stream).Case(
				(restStream, errorMessage) =>
					Reply.Failure<TToken, T>(stream, errorMessage),
				(restStream, value) =>
					Reply.Success(restStream, value));
		}

		public static Parser<TToken, Unit> And<TToken, T>(
			this Parser<TToken, T> parser)
		{
			return stream => parser(stream).Case(
				(restStream, errorMessage) =>
					Reply.Failure<TToken, Unit>(stream, "Failure: Combinator.And predicate"),
				(restStream, value) =>
					Reply.Success(stream, Unit.Instance));
		}

		public static Parser<TToken, Unit> Not<TToken, T>(
			this Parser<TToken, T> parser)
		{
			return stream => parser(stream).Case(
				(restStream, errorMessage) =>
					Reply.Success(stream, Unit.Instance),
				(restStream, value) =>
					Reply.Failure<TToken, Unit>(stream, "Failure: Combinator.Not predicate"));
		}

		public static Parser<TToken, T2> Pipe<TToken, T0, T1, T2>(
			this Parser<TToken, T0> parser0,
			Parser<TToken, T1> parser1,
			Func<T0, T1, T2> selector)
		{
			return from value0 in parser0
				from value1 in parser1
				select selector(value0, value1);
		}

		public static Parser<TToken, T3> Pipe<TToken, T0, T1, T2, T3>(
			this Parser<TToken, T0> parser0,
			Parser<TToken, T1> parser1,
			Parser<TToken, T2> parser2,
			Func<T0, T1, T2, T3> selector)
		{
			return from value0 in parser0
				from value1 in parser1
				from value2 in parser2
				select selector(value0, value1, value2);
		}

		public static Parser<TToken, T4> Pipe<TToken, T0, T1, T2, T3, T4>(
			this Parser<TToken, T0> parser0,
			Parser<TToken, T1> parser1,
			Parser<TToken, T2> parser2,
			Parser<TToken, T3> parser3,
			Func<T0, T1, T2, T3, T4> selector)
		{
			return from value0 in parser0
				from value1 in parser1
				from value2 in parser2
				from value3 in parser3
				select selector(value0, value1, value2, value3);
		}

		public static Parser<TToken, T5> Pipe<TToken, T0, T1, T2, T3, T4, T5>(
			this Parser<TToken, T0> parser0,
			Parser<TToken, T1> parser1,
			Parser<TToken, T2> parser2,
			Parser<TToken, T3> parser3,
			Parser<TToken, T4> parser4,
			Func<T0, T1, T2, T3, T4, T5> selector)
		{
			return from value0 in parser0
				from value1 in parser1
				from value2 in parser2
				from value3 in parser3
				from value4 in parser4
				select selector(value0, value1, value2, value3, value4);
		}

		public static Parser<TToken, T6> Pipe<TToken, T0, T1, T2, T3, T4, T5, T6>(
			this Parser<TToken, T0> parser0,
			Parser<TToken, T1> parser1,
			Parser<TToken, T2> parser2,
			Parser<TToken, T3> parser3,
			Parser<TToken, T4> parser4,
			Parser<TToken, T5> parser5,
			Func<T0, T1, T2, T3, T4, T5, T6> selector)
		{
			return from value0 in parser0
				from value1 in parser1
				from value2 in parser2
				from value3 in parser3
				from value4 in parser4
				from value5 in parser5
				select selector(value0, value1, value2, value3, value4, value5);
		}

		public static Parser<TToken, T7> Pipe<TToken, T0, T1, T2, T3, T4, T5, T6, T7>(
			this Parser<TToken, T0> parser0,
			Parser<TToken, T1> parser1,
			Parser<TToken, T2> parser2,
			Parser<TToken, T3> parser3,
			Parser<TToken, T4> parser4,
			Parser<TToken, T5> parser5,
			Parser<TToken, T6> parser6,
			Func<T0, T1, T2, T3, T4, T5, T6, T7> selector)
		{
			return from value0 in parser0
				from value1 in parser1
				from value2 in parser2
				from value3 in parser3
				from value4 in parser4
				from value5 in parser5
				from value6 in parser6
				select selector(value0, value1, value2, value3, value4, value5, value6);
		}

		public static Parser<TToken, T8> Pipe<TToken, T0, T1, T2, T3, T4, T5, T6, T7, T8>(
			this Parser<TToken, T0> parser0,
			Parser<TToken, T1> parser1,
			Parser<TToken, T2> parser2,
			Parser<TToken, T3> parser3,
			Parser<TToken, T4> parser4,
			Parser<TToken, T5> parser5,
			Parser<TToken, T6> parser6,
			Parser<TToken, T7> parser7,
			Func<T0, T1, T2, T3, T4, T5, T6, T7, T8> selector)
		{
			return from value0 in parser0
				from value1 in parser1
				from value2 in parser2
				from value3 in parser3
				from value4 in parser4
				from value5 in parser5
				from value6 in parser6
				from value7 in parser7
				select selector(value0, value1, value2, value3, value4, value5, value6, value7);
		}

		public static Parser<TToken, T> Lazy<TToken, T>(
			Func<Parser<TToken, T>> parserFactory)
		{
			return Lazy(Delayed.Return(parserFactory));
		}

		public static Parser<TToken, T> Lazy<TToken, T>(
			IDelayed<Parser<TToken, T>> delayedParser)
		{
			return stream => delayedParser.Force()(stream);
		}

		public static Parser<TToken, Unit> Ignore<TToken, T>(
			this Parser<TToken, T> parser)
		{
			return parser.Map(_ => Unit.Instance);
		}
	}
}