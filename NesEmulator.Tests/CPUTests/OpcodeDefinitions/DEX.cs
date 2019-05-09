﻿using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    [Trait("Category", "Unit")]
    public class DEX
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_OpCA()
        {
            var sut = CreateSut();

            const int opValue = 0xCA;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.DEX);
            op.AddressMode.Should().Be(AddressMode.Implicit);
            op.Bytes.Should().Be(1);
            op.Cycles.Should().Be(2);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }
    }
}