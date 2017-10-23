﻿using System.Linq;
using JsDataParser;
using Parseq;
using Xunit;
using Xunit.Abstractions;
using static JsDataParser.DataParser;

namespace JsDataParserTest
{
	public class FieldParserTest
	{
		public FieldParserTest(ITestOutputHelper output)
		{
			this.output = output;
		}

		private readonly ITestOutputHelper output;

		[Fact]
		public void DatumParserTest()
		{
			var sample = @"1657:{
		name: 'foo 2',
		nameJP: 'foo-壊',
		image: 'foo.jpg',
		type: 'Installation',
		installtype: 3,
		divebombWeak: 2.1,
		HP: 430,
		FP: 160,
		TP: 98,
		AA: 80,
		AR: 160,
		EV: 20,
		ASW: 0,
		LOS: 90,
		LUK: 70,
		unknownstats: true,
		RNG: 3,
		SPD: 0,
		TACC: 70,
		SLOTS: [24, 24, 12, 12],
		EQUIPS: [562, 562, 561, 561],
		fuel: 0,
		ammo: 0,
		canTorp: function() { return (this.HP/this.maxHP > .5); },






}".AsStream();

			Datum.Run(sample).Case(
				(_, str) => { Assert.True(false, str); },
				(_, cap) =>
				{
					cap.Id.Is(1657);
					cap.Fields.Count.Is(24);

					cap.Fields["fuel"].FieldType.Is(TokenTypes.IntegerNumber);
					cap.Fields["fuel"].Source.SequenceEqual("0").IsTrue();
				});
		}

		[Fact]
		public void FieldnameTeset()
		{
			var target = FieldName;

			target.Run("field".AsStream())
				.Case((_, __) => Assert.False(true),
					(_, seq) => seq.SequenceEqual("field").IsTrue());

			target.Run("Field".AsStream())
				.Case((_, __) => Assert.False(true),
					(_, seq) => seq.SequenceEqual("Field").IsTrue());

			target.Run("1field".AsStream())
				.Case((_, __) => Assert.False(false), (_, __) => Assert.True(false));
		}

		[Fact]
		public void FieldTest()
		{
			Field.Run("TACC: 70".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.IntegerNumber);
						cap.Source.SequenceEqual("70").IsTrue();
					});


			Field.Run("TACC: 7.0".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.RealNumber);
						cap.Source.SequenceEqual("7.0").IsTrue();
					});


			Field.Run("TACC: true".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.Boolean);
						cap.Source.SequenceEqual("true").IsTrue();
					});


			Field.Run("TACC: false".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.Boolean);
						cap.Source.SequenceEqual("false").IsTrue();
					});


			Field.Run("TACC: function() { return (this.HP/this.maxHP > .5); }".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.Function);
						cap.Source.SequenceEqual("function () { return (this.HP/this.maxHP > .5); }").IsTrue();
					});

			Field.Run("TACC: [0,1,2,3,4]".AsStream())
				.Case((_, __) => Assert.True(false),
					(_, cap) =>
					{
						cap.Name.Is("TACC");
						cap.FieldType.Is(TokenTypes.IntegerArray);

						cap.ArraySource.Count.Is(5);

						for (var i = 0; i < 5; i++)
							cap.ArraySource[i].SequenceEqual(i.ToString()).IsTrue();
					});
		}
	}
}