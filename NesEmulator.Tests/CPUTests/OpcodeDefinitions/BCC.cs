﻿using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    [Trait("Category", "Unit")]
    public class BCC
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_Op90()
        {
            var sut = CreateSut();

            const int opValue = 0x90;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.BCC);
            op.AddressMode.Should().Be(AddressMode.Relative);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(2);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }
    }
}