using System;
using FluentAssertions;
using Moq;
using Nerdle.AutoConfig.Exceptions;
using Nerdle.AutoConfig.Sections;
using Nerdle.AutoConfig.Strategy;
using NUnit.Framework;

namespace Nerdle.AutoConfig.Tests.Unit.Sections.SectionProviderTests
{
    class When_locating_a_section_by_convention
    {
        SectionProvider _sectionProvider;
        Mock<IMappingStrategy> _mappingStrategy;
        Mock<ISectionNameConvention> _nameConvention;
        Mock<IConfigurationSystem> _configurationSystem;

        readonly Section _section = new Section();

        const string SectionName = "f00";

        [SetUp]
        public void BeforeEach()
        {
            _mappingStrategy = new Mock<IMappingStrategy>();
            _nameConvention = new Mock<ISectionNameConvention>();
            _configurationSystem = new Mock<IConfigurationSystem>();
            _mappingStrategy.Setup(ms => ms.SectionNameFor<IFoo>()).Returns(SectionName);
            _sectionProvider = new SectionProvider(_nameConvention.Object, _configurationSystem.Object);
        }

        [Test]
        public void The_section_name_is_provided_by_the_mapping_strategy()
        {
            _configurationSystem.Setup(cs => cs.GetSection(SectionName)).Returns(_section);
            
            _sectionProvider.GetSection<IFoo>(_mappingStrategy.Object);

            _configurationSystem.Verify(cs => cs.GetSection(SectionName), Times.Once);
        }

        [Test]
        public void The_section_is_returned_if_found()
        {
            _configurationSystem.Setup(cs => cs.GetSection(SectionName)).Returns(_section);

            var result = _sectionProvider.GetSection<IFoo>(_mappingStrategy.Object);
            
            result.Should().Be(_section);
        }

        [Test]
        public void An_exception_is_thrown_if_the_section_is_not_found_and_no_alternative_names_are_available()
        {
            Action locating = () => _sectionProvider.GetSection<IFoo>(_mappingStrategy.Object);

            locating.ShouldThrowExactly<AutoConfigMappingException>()
                .Where(ex => ex.Message.Contains("looked for a config section named 'f00' but didn't find one"));
        }

        public void Alternative_names_are_checked_if_the_section_is_not_found()
        {
            _nameConvention.Setup(nc => nc.GetAlternativeNames(SectionName)).Returns(new[] { "dog", "cat" });
            _configurationSystem.Setup(cs => cs.GetSection("cat")).Returns(_section);

            var result = _sectionProvider.GetSection<IFoo>(_mappingStrategy.Object);

            _configurationSystem.Verify(cs => cs.GetSection(SectionName), Times.Once);
            _configurationSystem.Verify(cs => cs.GetSection("dog"), Times.Once);
            _configurationSystem.Verify(cs => cs.GetSection("cat"), Times.Once);
            
            result.Should().Be(_section);
        }

        [Test]
        public void An_exception_is_thrown_if_alternative_names_are_checked_and_the_section_is_still_not_found()
        {
            _nameConvention.Setup(nc => nc.GetAlternativeNames(SectionName)).Returns(new[] { "dog", "cat" });

            Action locating = () => _sectionProvider.GetSection<IFoo>(_mappingStrategy.Object);

            locating.ShouldThrowExactly<AutoConfigMappingException>()
                .Where(ex => ex.Message.Contains("looked for a config section named 'f00' or 'dog' or 'cat' but didn't find one"));
        }

        interface IFoo { }
    }
}