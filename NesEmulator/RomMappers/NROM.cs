using System;

namespace NesEmulator.RomMappers
{
    internal class NROM : ROM
    {
        private readonly int _prgLowBankStart;
        private readonly int _prgHighBankStart;
        
        public NROM(RomHeader header, Memory<byte> content) : base(header, content)
        {
            int offset = 16 + (header.HasTrainer ? 512 : 0);
            _prgLowBankStart = offset;

            _prgHighBankStart = _prgLowBankStart;
            if (header.PrgRomBanks == 2)
            {
                _prgHighBankStart += 0x4000;
            }
        }

        public override byte Read(ushort address)
        {
            BankAddress ba = new BankAddress(address);

            int startAddress = ba.Bank == PrgBank.Low ? _prgLowBankStart : _prgHighBankStart;

            Span<byte> bank = Content.Span.Slice(startAddress, Header.PrgBankSize);

            return bank[ba.Offset];
        }

        public override byte Peek(ushort address)
        {
            BankAddress ba = new BankAddress(address);

            int startAddress = ba.Bank == PrgBank.Low ? _prgLowBankStart : _prgHighBankStart;

            Span<byte> bank = Content.Span.Slice(startAddress, Header.PrgBankSize);

            return bank[ba.Offset];
        }

        public override void Write(ushort address, byte value)
        {
            // No effect on NROM, no bank switching
        }

        public override byte ReadChr(ushort address)
        {
            throw new NotImplementedException();
        }

        public override byte PeekChr(ushort address)
        {
            throw new NotImplementedException();
        }

        public override void WriteChr(ushort address, byte value)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {

        }
    }
}