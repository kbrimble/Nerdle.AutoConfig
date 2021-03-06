﻿using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Nerdle.AutoConfig.Mappers;
using NUnit.Framework;

namespace Nerdle.AutoConfig.Tests.Unit.Mappers.MapperSelectorTests
{
    [TestFixture]
    public class When_selecting_a_mapper
    {
        [TestCase(typeof(int))]
        [TestCase(typeof(uint))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(sbyte))]
        [TestCase(typeof(long))]
        [TestCase(typeof(ulong))]
        [TestCase(typeof(float))]
        [TestCase(typeof(double))]
        [TestCase(typeof(decimal))]
        [TestCase(typeof(bool))]
        [TestCase(typeof(DayOfWeek))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(DateTimeOffset))]
        [TestCase(typeof(TimeSpan))]
        [TestCase(typeof(string))]
        [TestCase(typeof(char))]
        [TestCase(typeof(int?))]
        [TestCase(typeof(byte?))]
        [TestCase(typeof(double?))]
        [TestCase(typeof(decimal?))]
        [TestCase(typeof(bool?))]
        [TestCase(typeof(ConsoleColor?))]
        [TestCase(typeof(DateTime?))]
        [TestCase(typeof(TimeSpan?))]
        public void Simple_types_can_be_mapped_by_the_ValueMapper(Type type)
        {
            MapperSelector.GetFor(type).Should().BeOfType<ValueMapper>();
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(IList<bool>))]
        [TestCase(typeof(ICollection<object>))]
        [TestCase(typeof(IEnumerable<DayOfWeek>))]
        [TestCase(typeof(IReadOnlyList<string>))]
        [TestCase(typeof(IReadOnlyCollection<DateTime>))]
        [TestCase(typeof(IList<IList<Exception>>))]
        public void Generic_collection_types_which_are_implemented_by_List_can_be_mapped_by_the_CollectionMapper(Type type)
        {
            MapperSelector.GetFor(type).Should().BeOfType<CollectionMapper>();
        }

        [TestCase(typeof(int[]))]
        [TestCase(typeof(bool[]))]
        [TestCase(typeof(List<Exception>[]))]
        public void Single_dimensional_array_types_can_be_mapped_by_the_ArrayMapper(Type type)
        {
            MapperSelector.GetFor(type).Should().BeOfType<ArrayMapper>();
        }

        [TestCase(typeof(KeyValuePair<int, int>))]
        [TestCase(typeof(KeyValuePair<DateTime, string>))]
        [TestCase(typeof(KeyValuePair<PropertyInfo, ICollection<string>>))]
        public void Key_value_pairs_can_be_mapped_by_the_KeyValuePairMapper(Type type)
        {
            MapperSelector.GetFor(type).Should().BeOfType<KeyValuePairMapper>();
        }
        
        [TestCase(typeof(IDictionary<int, int>))]
        [TestCase(typeof(IDictionary<float, string>))]
        [TestCase(typeof(IDictionary<Type, Exception>))]
        [TestCase(typeof(Dictionary<string, int[]>))]
        public void Dicionary_types_can_be_mapped_by_the_DictionaryMapper(Type type)
        {
            MapperSelector.GetFor(type).Should().BeOfType<DictionaryMapper>();
        }

        [TestCase(typeof(int[,]))]
        [TestCase(typeof(Exception))]
        public void Types_which_cannot_be_mapped_by_any_other_mapper_should_attempt_to_use_the_RecursingMapper(Type type)
        {
            MapperSelector.GetFor(type).Should().BeOfType<RecursingMapper>();
        }
    }
}
