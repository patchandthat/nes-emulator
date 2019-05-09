using System;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using Xunit;

namespace NesEmulator.UnitTests.MemoryTests
{
    [Trait("Category", "Unit")]
    public class MemoryRangeTests
    {
        private ushort _start;
        private ushort _end;

        public MemoryRangeTests()
        {
            _start = 10;
            _end = 20;
        }

        private MemoryRange CreateSut()
        {
            return new MemoryRange(_start, _end);
        }

        [Fact]
        public void ctor_WhenStartIsGreaterThanEnd_WillThrow()
        {
            _end = 10;
            _start = 11;

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_SetsStartProperty()
        {
            var sut = CreateSut();

            sut.Start.Should().Be(_start);
        }

        [Fact]
        public void ctor_SetsEndProperty()
        {
            var sut = CreateSut();

            sut.End.Should().Be(_end);
        }

        [Fact]
        public void ctor_ShouldCalculateLength()
        {
            _start = 5;
            _end = 10;
            var expectedRange = 6;

            var sut = CreateSut();

            sut.Length.Should().Be(expectedRange);
        }

        [Fact]
        public void Intersect_WhenRangesOverlap_ShouldBeTrue()
        {
            var range1 = new MemoryRange(1,2);
            var range2 = new MemoryRange(2,3);

            range1.Intersects(range2).Should().BeTrue();
            range2.Intersects(range1).Should().BeTrue();
        }

        [Fact]
        public void Contains_WhenOneRangeEnclosesAnother_IsTrue()
        {
            var range1 = new MemoryRange(1,4);
            var range2 = new MemoryRange(2,3);

            range1.Contains(range2).Should().BeTrue();
        }
        
        [Fact]
        public void Contains_WhenOneRangeIsContainedByAnother_IsFalse()
        {
            var range1 = new MemoryRange(1,4);
            var range2 = new MemoryRange(2,3);

            range2.Contains(range1).Should().BeFalse();
        }

        [Fact]
        public void Contains_WhenRangesDoNotOverlap_IsFalse()
        {
            var range1 = new MemoryRange(1,2);
            var range2 = new MemoryRange(3,4);

            range1.Contains(range2).Should().BeFalse();
            range2.Contains(range1).Should().BeFalse();
        }

        [Fact]
        public void Contains_WhenAddressIsLessThanRangeStart_IsFalse()
        {
            ushort address = _start.Plus(-1);
            
            var sut = CreateSut();

            sut.Contains(address).Should().BeFalse();
        }
        
        [Fact]
        public void Contains_WhenAddressIsGreaterThanRangeEnd_IsFalse()
        {
            ushort address = _end.Plus(1);
            
            var sut = CreateSut();

            sut.Contains(address).Should().BeFalse();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(20)]
        public void Contains_WhenAddressIsWithinRange_IsTrue(ushort address)
        {
            var sut = CreateSut();

            sut.Contains(address).Should().BeTrue();
        }
    }
}