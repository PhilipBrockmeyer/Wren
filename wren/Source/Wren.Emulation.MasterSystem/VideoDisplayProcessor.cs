using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wren.Emulation.MasterSystem
{
    public class VideoDisplayProcessor : IRunnableSystemComponent, IPortAddressableSystemComponent, IStatefulSystemComponent
    {
        const Int32 ScanlinesPerFrame = 224;
        const Int32 HorizontalScreenResolutionInTiles = 32;
        const Int32 VerticalScreenResolutionInTiles = 24;
        const Int32 HorizontalTileSize = 8;
        const Int32 VerticalTileSize = 8;
        const Int32 HorizontalResolution = 256;
        const Int32 VerticalResolution = 192;
        //const Int32 BytesPerPixel = 2;
        const Int32 MaximumSprites = 64;
        const Int32 MaximumSpritesPerScanline = 8;

        public Int32 reg0;
        public Int32 reg1;
        public Int32 reg3;
        public Int32 reg4;

        private enum PortAddressWriteMode
        {
            LowByte,
            HighByte
        }

        private enum VramWriteMode
        {
            Palette,
            Vram
        }

        public Byte[] _vram;
        public Byte[] _palette;
        public IntPtr _screenBuffer;
        public UInt16[] _transformedPalette;

        PortAddressWriteMode _addressWriteMode = PortAddressWriteMode.LowByte;
        VramWriteMode _writeMode = VramWriteMode.Vram;
        Int32 _addressLowByte = -1;
        Int32 _addressHighByte = -1;
        Int32 _currentVRamAddress = -1;
        Int32 _currentPaletteAddress = -1;
        Byte _statusByte;
        Int32 _currentScanline = 0;

        public VideoDisplaySettings Settings { get; private set; }

        public VideoDisplayProcessor()
        {
            Settings = new VideoDisplaySettings();
            _vram = new Byte[0x4000];  // 16k
            _palette = new Byte[32];
            _transformedPalette = new UInt16[32];
        }

        public void SetScreeenBuffer(IntPtr screenBuffer)
        {
            _screenBuffer = screenBuffer;
        }

        public void RegisterPortAddresses(IPortManager portManager)
        {
            portManager.RegisterPort(0xBF, WriteAddressPort, ReadAddressPort);
            portManager.RegisterPort(0xBE, WriteVRamPort, ReadVRamPort);
        }

        public RunDelegate GetRunMethod(ISystemBus systemBus)
        {
            return (cycles, bus) =>
                {
                    if (Settings.IsDisplayEnabled)
                       RenderScanline();

                    _currentScanline++;
                    
                    if (_currentScanline == VerticalResolution)
                    {
                        if (Settings.IsVSyncInterruptEnabled)
                        {
                            _statusByte |= 0x80;
                            bus.RequestInterupt(Interupts.Scanline);
                        }
                    }

                    if (_currentScanline == ScanlinesPerFrame)
                    {
                        _currentScanline = 0;
                    }

                    return 0;
                };
        }

        #region Rendering

        private void RenderScanline()
        {
            if (_currentScanline < VerticalResolution)
            {
                RenderBackground(false);
                RenderSprites();
                RenderBackground(true);
            }
        }

        private void RenderBackground(Boolean shouldDisplayInFrontOfSprites)
        {
            Int32 rowNumber = (_currentScanline + Settings.VerticalScrollOffset) / VerticalTileSize;

            if (rowNumber >= 28)
                rowNumber -= 28;

            Int32 nameTableIndex = Settings.NameTableBaseAddress + (HorizontalScreenResolutionInTiles * rowNumber * 2 /* 2 bytes per tile */);

            for (Int32 counter = 0; counter < HorizontalScreenResolutionInTiles; counter++)
            {
                Int32 tileNumber = _vram[nameTableIndex];
                Byte tileData = _vram[nameTableIndex + 1];

                tileNumber |= ((tileData & 0x01) << 8);

                // tile is in front of sprites.  do not render now.
                if (((tileData & 0x10) == 0x10) != shouldDisplayInFrontOfSprites)
                    continue;
                
                RenderTileLine(counter * HorizontalTileSize% HorizontalResolution,                // tile left pixel postion
                               tileNumber,                                  // tile number
                               
                               // 0x04 = vertical flip bit.
                               (tileData & 0x04) == 0x04 ? 
                                    VerticalTileSize - ((_currentScanline + Settings.VerticalScrollOffset) % VerticalTileSize) - 1 :
                                    (_currentScanline + Settings.VerticalScrollOffset) % VerticalTileSize,    // tile row
                               (tileData & 0x08) == 0x08 ? 16 : 0,          // palette base index
                               (tileData & 0x02) == 0x02,                   // horizontal flip 
                               false);                  

                nameTableIndex += 2;
            }
        }        

        private void RenderSprites()
        {
            Int32 spritesOnCurrentScanline = 0;
            Int32 spriteYPositionIndex = Settings.SpriteInformationBaseAddress;
            Int32 spriteXPositionIndex = Settings.SpriteInformationBaseAddress + 128;
            Int32 spriteTileNumberIndex = Settings.SpriteInformationBaseAddress + 129;
            Int32 spriteHeight = Settings.ShouldUse8x16Sprites ? spriteHeight = VerticalTileSize * 2 : spriteHeight = VerticalTileSize;
            Int32 tileNumberHighBit = Settings.ShouldUseUpperVRamBankForSpriteTiles ? 256 : 0;

            for (Int32 counter = 0; counter < MaximumSprites; counter++)
            {
                Int32 yPosition = _vram[spriteYPositionIndex++];
                if (yPosition >= 240)
                    yPosition = yPosition - 256;

                if (yPosition == 208)
                    return;

                if ((yPosition <= _currentScanline) && (_currentScanline < yPosition + spriteHeight))
                {
                    RenderTileLine(
                        _vram[spriteXPositionIndex],                             // left
                        _vram[spriteTileNumberIndex | tileNumberHighBit],        // tile number
                        _currentScanline - yPosition,                            // tile row
                        16,                                                      // palette index
                        false,
                        true
                    );

                    spritesOnCurrentScanline++;

                    if (spritesOnCurrentScanline == MaximumSpritesPerScanline)
                        return;
                }

                spriteXPositionIndex += 2;
                spriteTileNumberIndex += 2;
            }
        }

        private void RenderTileLine(Int32 left, Int32 tileNumber, Int32 tileRow, Int32 paletteBase, Boolean isHorizontallyFlipped, Boolean useTransparency)
        {
            Int32 vRamIndex = tileRow * 4 + 0 + 32 * tileNumber;
            Int32 screenBufferIndex = ((_currentScanline * HorizontalResolution) + left);
            var b4 = _vram[vRamIndex++];
            var b3 = _vram[vRamIndex++];
            var b2 = _vram[vRamIndex++];
            var b1 = _vram[vRamIndex++];
            unsafe
            {
                ushort* rptr = (ushort*)_screenBuffer.ToPointer();

                if (!isHorizontallyFlipped)
                {
                    for (Int32 column = 0; column < 8; column++)
                    {
                        var palValue = (((b1 & (1 << 7 - column)) >> (7 - column)) << 3) |
                                        (((b2 & (1 << 7 - column)) >> (7 - column)) << 2) |
                                        (((b3 & (1 << 7 - column)) >> (7 - column)) << 1) |
                                        (((b4 & (1 << 7 - column)) >> (7 - column)));

                        if (palValue == 0 && useTransparency)
                        {
                            screenBufferIndex++;
                            continue;
                        }

                        rptr[screenBufferIndex++] = _transformedPalette[palValue + paletteBase];
                    }
                }
                else
                {
                    for (Int32 column = 7; column >= 0; column--)
                    {
                        var palValue = (((b1 & (1 << 7 - column)) >> (7 - column)) << 3) |
                                        (((b2 & (1 << 7 - column)) >> (7 - column)) << 2) |
                                        (((b3 & (1 << 7 - column)) >> (7 - column)) << 1) |
                                        (((b4 & (1 << 7 - column)) >> (7 - column)));

                        if (palValue == 0 && useTransparency)
                        {
                            screenBufferIndex++;
                            continue;
                        }

                        rptr[screenBufferIndex++] = _transformedPalette[palValue + paletteBase];                       
                    }
                }
            }
        }
        #endregion

        #region User Methods
        public void RenderScene()
        {
            Int32 prevScanlineCounter = _currentScanline;

            for (_currentScanline = 0; _currentScanline < VerticalResolution; _currentScanline++)
            {
                RenderScanline();
            }
        }


        /*public Byte[] RenderTile(Int32 tileNum)
        {
            Byte[] tile = new Byte[HorizontalTileSize * VerticalTileSize * BytesPerPixel];

            _currentScanline = 0;

            for (Int32 tileRow = 0; tileRow < VerticalTileSize; tileRow++)
            {
                RenderTileLine(0, tileNum, tileRow, 16, false, false);
            }

            for (Int32 y = 0; y < VerticalTileSize; y++)
            {
                for (Int32 x = 0; x < VerticalTileSize; x++)
                {
                    tile[(y * HorizontalTileSize + x) * BytesPerPixel] = _screenBuffer[(y * HorizontalResolution + x) * BytesPerPixel];
                    tile[(y * HorizontalTileSize + x) * BytesPerPixel + 1] = _screenBuffer[(y * HorizontalResolution + x) * BytesPerPixel + 1];
                    tile[(y * HorizontalTileSize + x) * BytesPerPixel + 2] = _screenBuffer[(y * HorizontalResolution + x) * BytesPerPixel + 2];
                    tile[(y * HorizontalTileSize + x) * BytesPerPixel + 3] = _screenBuffer[(y * HorizontalResolution + x) * BytesPerPixel + 3];
                }
            }

            return tile;
        }*/
        #endregion

        #region Port BF
        public Byte ReadAddressPort()
        {
            Byte returnValue = _statusByte;
            _statusByte &= 0x3F;
            return returnValue;
        }

        public void WriteAddressPort(Byte value)
        {
            if (_addressWriteMode == PortAddressWriteMode.LowByte)
            {
                _addressLowByte = value;
                _addressWriteMode = PortAddressWriteMode.HighByte;
                return;
            }

            _addressHighByte = value;
            _addressWriteMode = PortAddressWriteMode.LowByte;
            var writeType = _addressHighByte >> 6;
            switch (writeType)
            {                
                case 0:
                case 1:
                    _currentPaletteAddress = 0;
                    _writeMode = VramWriteMode.Vram;
                    _currentVRamAddress = ((_addressHighByte & 0x3F) << 8) | _addressLowByte;
                    break;
                case 2:
                    WriteToRegister(_addressHighByte & 0x0F, _addressLowByte);
                    break;
                case 3:
                    // Write to palette.
                    _currentVRamAddress = 0;
                    _writeMode = VramWriteMode.Palette;
                    _currentPaletteAddress = _addressLowByte & 0x1F;
                    break;

                default:
                    throw new ApplicationException();
            }
        }

        private void WriteToRegister(Int32 registerNumber, Int32 registerValue)
        {
            switch (registerNumber)
            {
                case 0:
                    reg0 = registerValue;
                    Settings.AreRightHandColumnsStatic = (registerValue & VideoDisplaySettings.RightHandColumnsStatic) != 0;
                    Settings.AreTopRowsStatic = (registerValue & VideoDisplaySettings.TopRowsStatic) != 0;
                    Settings.IsLeftmostColumnHidden = (registerValue & VideoDisplaySettings.HideLeftmostColumn) != 0;
                    Settings.IsLineInterruptEnabled = (registerValue & VideoDisplaySettings.EnableLineInterrupt) != 0;
                    Settings.ShouldShiftSpritesLeft = (registerValue & VideoDisplaySettings.ShiftSpritesLeft) != 0;
                    Settings.IsScreenStretchEnabled = (registerValue & VideoDisplaySettings.StretchScreen) != 0;
                    Settings.IsScreenSynchronizationDisabled = (registerValue & VideoDisplaySettings.DisableScreenSynchronization) != 0;
                    break;

                case 1:
                    reg1 = registerValue;
                    Settings.IsDisplayEnabled = (registerValue & VideoDisplaySettings.EnableDisplay) != 0;
                    Settings.IsVSyncInterruptEnabled = (registerValue & VideoDisplaySettings.EnableVSyncInterrupt) != 0;
                    Settings.ShouldExtendScreenBy4Rows = (registerValue & VideoDisplaySettings.ExtendScreenBy4Rows) != 0;
                    Settings.ShouldExtendScreenBy6Rows = (registerValue & VideoDisplaySettings.ExtendScreenBy6Rows) != 0;
                    Settings.ShouldUse8x16Sprites = (registerValue & VideoDisplaySettings.Use16x8Sprites) != 0;
                    Settings.IsSpriteZoomEnabled = (registerValue & VideoDisplaySettings.ZoomSprites) != 0;
                    break;

                case 2:
                    Settings.NameTableBaseAddress = (registerValue & 0x0E) << 10;
                    break;

                case 3:
                    reg3 = registerValue;
                    break;

                case 4:
                    reg4 = registerValue;
                    break;

                case 5:
                    Settings.SpriteInformationBaseAddress = (registerValue & 0x7E) << 7;
                    break;

                case 6:
                    Settings.ShouldUseUpperVRamBankForSpriteTiles = (registerValue & 0x04) == 0x04;
                    break;

                case 7:
                    Settings.BorderColor = registerValue;
                    break;

                case 8:
                    Settings.HorizontalScrollOffset = registerValue;
                    break;

                case 9:
                    if (registerValue == 0)
                    {
                        Int32 breakpoint = 1;
                    }

                    if (registerValue != 0)
                    {
                        Int32 breakpoint = 1;
                    }


                    Settings.VerticalScrollOffset = registerValue;
                    break;

                case 10:
                    Settings.ScanLineCounterInitialValue = registerValue;
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Port BE
        public Byte ReadVRamPort()
        {
            _addressWriteMode = PortAddressWriteMode.LowByte;
            if (_writeMode == VramWriteMode.Vram)
                return _vram[_currentVRamAddress++];
            else if (_writeMode == VramWriteMode.Palette)
                return _palette[_currentPaletteAddress++];

            throw new ApplicationException();
        }

        public void WriteVRamPort(Byte value)
        {
            _addressWriteMode = PortAddressWriteMode.LowByte;

            if (_writeMode == VramWriteMode.Vram)
            {
                _vram[_currentVRamAddress++] = value;
                if (_currentVRamAddress > 0x3FFF)
                    _currentVRamAddress = 0x3FFF;
            }
            else if (_writeMode == VramWriteMode.Palette)
            {
                WritePaletteEntry(value);
            }
        }

        private void WritePaletteEntry(Byte value)
        {
            _palette[_currentPaletteAddress] = value;

            UInt16 b = (UInt16)(((UInt16)value >> (UInt16)4) * (UInt16)7);
            UInt16 g = (UInt16)((((UInt16)value >> (UInt16)2) & 0x03) * (UInt16)15);
            UInt16 r = (UInt16)(((UInt16)value & 0x03) * (UInt16)7);

            _transformedPalette[_currentPaletteAddress++] = (UInt16)((r << (UInt16)11) | (g << (UInt16)5) | (b));
        }
        #endregion

        public void SerializeComponentState(BinaryWriter writer)
        {
            writer.Write(_addressHighByte);
            writer.Write(_addressLowByte);
            writer.Write((Int32)_addressWriteMode);
            writer.Write(_currentPaletteAddress);
            writer.Write(_currentScanline);
            writer.Write(_currentVRamAddress);
            writer.Write(_statusByte);
            writer.Write((Int32)_writeMode);
            writer.Write(_palette);

            foreach (var entry in _transformedPalette)
            {
                writer.Write(entry);
            }

            writer.Write(_vram);
        }

        public void DeserializeComponentState(BinaryReader reader)
        {
            _addressHighByte = reader.ReadInt32();
            _addressLowByte = reader.ReadInt32();
            _addressWriteMode = (PortAddressWriteMode)reader.ReadInt32();
            _currentPaletteAddress = reader.ReadInt32();
            _currentScanline = reader.ReadInt32();
            _currentVRamAddress = reader.ReadInt32();
            _statusByte = reader.ReadByte();
            _writeMode = (VramWriteMode)reader.ReadInt32();
            _palette = reader.ReadBytes(_palette.Length);

            for (Int32 i = 0; i < _transformedPalette.Length; i++)
            {
                _transformedPalette[i] = reader.ReadUInt16();
            }

            _vram = reader.ReadBytes(_vram.Length);
        }
    }
}
