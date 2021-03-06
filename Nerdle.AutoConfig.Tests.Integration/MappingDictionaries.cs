﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Nerdle.AutoConfig.Tests.Integration
{
    [TestFixture]
    public class MappingDictionaries : EndToEndTest
    {
        [Test]
        public void Mapping_dictionaries()
        {
            var config = AutoConfig.Map<IDictionaryConfiguration>();
            config.Should().NotBeNull();
            config.FrenchWords.Should().HaveCount(3);
            config.FrenchWords.Keys.Should().ContainInOrder("cat", "dog", "monkey");
            config.FrenchWords.Values.Should().ContainInOrder("chat", "chien", "singe");
        }

        [Test]
        public void Mapping_complex_dictionaries()
        {
            var config = AutoConfig.Map<IComplexDictionaryConfiguration>();
            config.Should().NotBeNull();
            config.Endpoints.Should().HaveCount(2);
            config.Endpoints["Primary"].Should().HaveCount(2);
            config.Endpoints["Primary"].First().Name.Should().Be("Foo");
            config.Endpoints["Primary"].First().Port.Should().Be(1);
            config.Endpoints["Secondary"].Should().HaveCount(1);
        }

        public interface IDictionaryConfiguration
        {
            IDictionary<string, string> FrenchWords { get; }
        }

        public interface IComplexDictionaryConfiguration
        {
            IDictionary<string, IEnumerable<IEndpointConfiguration>> Endpoints { get; }
        }

        public interface IEndpointConfiguration
        {
            string Name { get; }
            int Port { get; }
        }
    }
}
