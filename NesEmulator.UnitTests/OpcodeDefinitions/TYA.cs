﻿using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class TYA
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op98()
            {
                var sut = CreateSut();

                const int opValue = 0x98;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.TYA);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }
        }
    }
}
