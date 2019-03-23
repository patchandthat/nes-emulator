﻿using System;
using FakeItEasy;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class CPUTests
    {
        private IMemory _memory;

        public CPUTests()
        {
            _memory = A.Fake<IMemory>();
        }

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
