﻿using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    [Trait("Category", "Unit")]
    public class LDA
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_OpA1()
        {
            var sut = CreateSut();

            const int opValue = 0xA1;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.LDA);
            op.AddressMode.Should().Be(AddressMode.IndirectX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(6);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_OpA5()
        {
            var sut = CreateSut();

            const int opValue = 0xA5;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.LDA);
            op.AddressMode.Should().Be(AddressMode.ZeroPage);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(3);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_OpA9()
        {
            var sut = CreateSut();

            const int opValue = 0xA9;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.LDA);
            op.AddressMode.Should().Be(AddressMode.Immediate);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(2);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_OpAD()
        {
            var sut = CreateSut();

            const int opValue = 0xAD;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.LDA);
            op.AddressMode.Should().Be(AddressMode.Absolute);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_OpB1()
        {
            var sut = CreateSut();

            const int opValue = 0xB1;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.LDA);
            op.AddressMode.Should().Be(AddressMode.IndirectY);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(5);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_OpB5()
        {
            var sut = CreateSut();

            const int opValue = 0xB5;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.LDA);
            op.AddressMode.Should().Be(AddressMode.ZeroPageX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_OpB9()
        {
            var sut = CreateSut();

            const int opValue = 0xB9;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.LDA);
            op.AddressMode.Should().Be(AddressMode.AbsoluteY);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_OpBD()
        {
            var sut = CreateSut();

            const int opValue = 0xBD;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.LDA);
            op.AddressMode.Should().Be(AddressMode.AbsoluteX);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }
    }
}