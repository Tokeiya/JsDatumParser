﻿using System.Collections.Generic;
using System.Linq;
using JsDataParser.DataLoader;
using JsDataParser.Mapping;
using Xunit;

namespace JsDataParserTest
{
	public class SampleA
	{
		public string Name { get; set; }
		public string NameJp { get; set; }
		public string Image { get; set; }
		public string BType { get; set; }
	}

	public class SampleD
	{
		public string BType;
		public string Image;
		public string Name;
		public string NameJp;
	}


	public class SampleB
	{
		public int[] Slots { get; set; }
		public double[] Reals { get; set; }
		public IEnumerable<string> Txts { get; set; }
		public bool[] Flags { get; set; }
		public object[] Misc { get; set; }
	}


	public class SampleC
	{
		public bool[] Flags;
		public object[] Misc;
		public double[] Reals;
		public int[] Slots;
		public IEnumerable<string> Txts;
	}

	public class TinyMapperTest
	{
		[Fact]
		public void ArrayFieldMappingTest()
		{
			var tmp = DataLoader.LoadRaw(".\\Samples\\TinyMapTestSample.txt");
			var source = tmp.Values.First();

			var actual = TinyMapper<SampleC>.SingleMap(source.NestedObject);

			actual.Misc.SequenceEqual(new object[] {1, "a", 42.195, 114514});

			actual.Txts.SequenceEqual(new[] {"a", "b", "c"}).IsTrue();


			actual.Slots.Length.Is(3);
			actual.Slots.SequenceEqual(new[] {1, 2, 3}).IsTrue();

			actual.Flags.SequenceEqual(new[] {true, false}).IsTrue();
		}


		[Fact]
		public void ArrayPropertyMappingTest()
		{
			var tmp = DataLoader.LoadRaw(".\\Samples\\TinyMapTestSample.txt");
			var source = tmp.Values.First();

			var actual = TinyMapper<SampleB>.SingleMap(source.NestedObject);

			actual.Misc.SequenceEqual(new object[] {1, "a", 42.195, 114514});

			actual.Txts.SequenceEqual(new[] {"a", "b", "c"}).IsTrue();


			actual.Slots.Length.Is(3);
			actual.Slots.SequenceEqual(new[] {1, 2, 3}).IsTrue();

			actual.Flags.SequenceEqual(new[] {true, false}).IsTrue();
		}

		[Fact]
		public void MapManyTest()
		{
			var tmp = DataLoader.LoadRaw(".\\Samples\\TinyMapTestSample.txt");
			var actual = TinyMapper<SampleA>.MultiMap(tmp).ToArray();

			actual.Length.Is(5);
		}

		[Fact]
		public void SacalrPropertyValueMappting()
		{
			var tmp = DataLoader.LoadRaw(".\\Samples\\TinyMapTestSample.txt");
			var source = tmp.Values.First();

			var actual = TinyMapper<SampleA>.SingleMap(source.NestedObject);

			actual.Name.Is("12cm Single Cannon");
			actual.Image.IsNull();
			actual.NameJp.Is("12cm単装砲");
			actual.BType.Is("B_MAINGUN");
		}

		[Fact]
		public void ScalarFieldValueMappting()
		{
			var tmp = DataLoader.LoadRaw(".\\Samples\\TinyMapTestSample.txt");
			var source = tmp.Values.First();

			var actual = TinyMapper<SampleD>.SingleMap(source.NestedObject);

			actual.Name.Is("12cm Single Cannon");
			actual.Image.IsNull();
			actual.NameJp.Is("12cm単装砲");
			actual.BType.Is("B_MAINGUN");
		}
	}
}