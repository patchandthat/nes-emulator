using System;
using FakeItEasy;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    [Trait("Category", "Unit")]
    public class CPUTests
    {
        public CPUTests()
        {
            _memory = A.Fake<IMemory>();
        }

        private IMemory _memory;

        private CPU CreateSut()
        {
            return new CPU(_memory);
        }

        [Fact]
        public void ctor_CalledWithNullMemoryReference_WillThrow()
        {
            _memory = null;

            Assert.Throws<ArgumentNullException>(() => CreateSut());
        }
    }
}