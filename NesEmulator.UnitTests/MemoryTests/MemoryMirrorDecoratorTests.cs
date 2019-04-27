using System;
using FakeItEasy;
using FluentAssertions;
using NesEmulator.Memory;
using Xunit;

namespace NesEmulator.UnitTests.MemoryTests
{
    public class MemoryMirrorDecoratorTests
    {
        private IReadWrite _other;
        private MemoryRange _sourceArea;
        private MemoryRange _mirrorArea;

        public MemoryMirrorDecoratorTests()
        {
            _other = A.Fake<IReadWrite>();
            
            _sourceArea = new MemoryRange(0x00, 0xFF);
            _mirrorArea = new MemoryRange(0x100, 0x2FF);
        }

        private MemoryMirrorDecorator CreateSut()
        {
            return new MemoryMirrorDecorator(_other, _sourceArea, _mirrorArea);
        }

        [Fact]
        public void ctor_WhenCalledWithNullObjectToDecorate_WillThrowArgumentNullException()
        {
            _other = null;

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(0x00, 0x10, 0x10, 0x20)]
        [InlineData(0x00, 0x30, 0x10, 0x20)]
        [InlineData(0x20, 0x30, 0x10, 0x20)]
        [InlineData(0x10, 0x20, 0x00, 0x30)]
        public void ctor_WhenMemoryRangesIntersect_ShouldThrowArgumentException(ushort sourceStart, ushort sourceEnd, ushort mirrorStart, ushort mirrorEnd)
        {
            _sourceArea = new MemoryRange(sourceStart, sourceEnd);
            _mirrorArea = new MemoryRange(mirrorStart, mirrorEnd);
            
            Action action = () => CreateSut();

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Read_WithinSourceRange_ShouldForwardToDecoratedObject()
        {
            var sut = CreateSut();

            sut.Read(0x47);

            A.CallTo(() => _other.Read(0x47))
                .MustHaveHappened();
        }
        
        [Fact]
        public void Peek_WithinSourceRange_ShouldForwardToDecoratedObject()
        {
            ushort address = 0x00FC;

            var sut = CreateSut();

            sut.Peek(address);

            A.CallTo(() => _other.Peek(address))
                .MustHaveHappened();
        }
        
        [Fact]
        public void Write_WithinSourceRange_ShouldForwardToDecoratedObject()
        {
            ushort address = 0x0053;
            byte value = 0x26;

            var sut = CreateSut();

            sut.Write(address, value);

            A.CallTo(() => _other.Write(address, value))
                .MustHaveHappened();
        }
        
        [Fact]
        public void Read_WithinMirrorRange_ShouldMapToSourceRangeAndForwardToDecoratedObject()
        {
            ushort address = 0x00FF;
            ushort mirrorAddress = 0x02FF;

            var sut = CreateSut();

            sut.Read(mirrorAddress);

            A.CallTo(() => _other.Read(address))
                .MustHaveHappened();
            A.CallTo(() => _other.Read(A<ushort>._))
                .MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public void Peek_WithinMirrorRange_ShouldMapToSourceRangeAndForwardToDecoratedObject()
        {
            ushort address = 0x00EC;
            ushort mirrorAddress = 0x02EC;

            var sut = CreateSut();

            sut.Peek(mirrorAddress);

            A.CallTo(() => _other.Peek(address))
                .MustHaveHappened();
            A.CallTo(() => _other.Peek(A<ushort>._))
                .MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public void Write_WithinMirrorRange_ShouldMapToSourceRangeAndForwardToDecoratedObject()
        {
            ushort address = 0x00EC;
            ushort mirrorAddress = 0x02EC;
            byte value = 0xFF;

            var sut = CreateSut();

            sut.Write(mirrorAddress, value);

            A.CallTo(() => _other.Write(address, value))
                .MustHaveHappened();
            A.CallTo(() => _other.Write(A<ushort>._, A<byte>._))
                .MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public void Read_OutsideOfSourceAndMirrorRanges_ShouldThrowArgumentOutOfRangeException()
        {
            ushort address = 0x1000;

            var sut = CreateSut();

            Action action = () => sut.Read(address);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
        
        [Fact]
        public void Peek_OutsideOfSourceAndMirrorRanges_ShouldThrowArgumentOutOfRangeException()
        {
            ushort address = 0x0300;

            var sut = CreateSut();

            Action action = () => sut.Peek(address);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
        
        [Fact]
        public void Write_OutsideOfSourceAndMirrorRanges_ShouldThrowArgumentOutOfRangeException()
        {
            ushort address = 0xFFFF;

            var sut = CreateSut();

            Action action = () => sut.Write(address, 0x00);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}