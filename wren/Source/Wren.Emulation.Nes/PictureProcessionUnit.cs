using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Drawing;
using Wren.Emulation.Nes;
namespace AHD.MyNes.Nes
{
    /// <summary>
    /// 
    /// </summary>
    public class PictureProcessingUnit
    {
        public PictureProcessingUnit(NesEmulator theEngine, SystemBus systemBus)
        {
            _bus = systemBus;
            myEngine = theEngine;
            nameTables = new byte[0x2000];
            spriteRam = new byte[0x100];
            RestartPPU();
        }

        public bool executeNMIonVBlank;
        public byte ppuMaster; // 0 = slave, 1 = master, 0xff = unset (master) 
        public int spriteSize;  // instead of being 'boolean', this will be 8 or 16
        public int backgroundAddress; // 0000 or 1000
        public int spriteAddress; // 0000 or 1000
        public int ppuAddressIncrement;
        public int nameTableAddress; // 2000, 2400, 2800, 2c00
        public bool monochromeDisplay; // false = color
        public bool noBackgroundClipping; // false = clip left 8 bg pixels
        public bool noSpriteClipping; // false = clip left 8 sprite pixels
        public bool backgroundVisible; // false = invisible
        public bool spritesVisible; // false = sprites invisible
        public int ppuColor; // r/b/g or intensity level
        public int Scanlinesperframe = 262;
        public int ScanlinesOfVBLANK = 248;
        public byte sprite0Hit;
        public int vramReadWriteAddress;
        public int prev_vramReadWriteAddress;
        public byte vramHiLoToggle;
        public byte vramReadBuffer;
        public byte scrollV;
        public byte scrollH;
        public int currentScanline;
        public byte[] nameTables;
        public byte[] spriteRam;
        public uint spriteRamAddress;
        public int spritesCrossed;
        NesEmulator myEngine;
        SystemBus _bus;
        IntPtr _renderingSurface;

        #region PPU registers R/W
        //0x2000
        public void Control_Register_1_Write(byte data)
        {
            //go bit by bit, and flag our values
            if ((data & 0x80) == 0x80)
                executeNMIonVBlank = true;
            else
                executeNMIonVBlank = false;

            if ((data & 0x20) == 0x20)
                spriteSize = 16;
            else
                spriteSize = 8;

            if ((data & 0x10) == 0x10)
                backgroundAddress = 0x1000;
            else
                backgroundAddress = 0x0000;

            if ((data & 0x8) == 0x8)
                spriteAddress = 0x1000;
            else
                spriteAddress = 0x0000;

            if ((data & 0x4) == 0x4)
                ppuAddressIncrement = 32;
            else
                ppuAddressIncrement = 1;

            if ((backgroundVisible == true) || (ppuMaster == 0xff) || (ppuMaster == 1))
            {
                int no = (data & 0x3);
                if (no == 0x0)
                    nameTableAddress = 0x2000;
                else if (no == 1)
                    nameTableAddress = 0x2400;
                else if (no == 2)
                    nameTableAddress = 0x2800;
                else if (no == 3)
                    nameTableAddress = 0x2C00;
            }

            if (ppuMaster == 0xff)
            {
                if ((data & 0x40) == 0x40)
                    ppuMaster = 0;
                else
                    ppuMaster = 1;
            }
        }
        //0x2001
        public void Control_Register_2_Write(byte data)
        {
            if ((data & 0x1) == 0x1)
                monochromeDisplay = true;
            else
                monochromeDisplay = false;
            if ((data & 0x2) == 0x2)
                noBackgroundClipping = true;
            else
                noBackgroundClipping = false;

            if ((data & 0x4) == 0x4)
                noSpriteClipping = true;
            else
                noSpriteClipping = false;

            if ((data & 0x8) == 0x8)
                backgroundVisible = true;
            else
                backgroundVisible = false;

            if ((data & 0x10) == 0x10)
                spritesVisible = true;
            else
                spritesVisible = false;

            ppuColor = (data >> 5);

        }
        //0x2002
        public byte Status_Register_Read()
        {
            byte returnedValue = 0;

            // VBlank
            if (currentScanline == 240)
                returnedValue = (byte)(returnedValue | 0x80);

            // Sprite 0 hit
            //Sprite_Zero_Hit();

            if (sprite0Hit == 1)
            {
                returnedValue = (byte)(returnedValue | 0x40);
                //sprite0Hit = 0;
            }
            // Sprites on current scanline
            if (spritesCrossed > 8)
                returnedValue = (byte)(returnedValue | 0x20);

            vramHiLoToggle = 1;

            return returnedValue;
        }
        //0x2003
        public void SpriteRam_Address_Register_Write(byte data)
        {
            spriteRamAddress = (uint)data;
        }
        //0x2004
        public void SpriteRam_IO_Register_Write(byte data)
        {
            spriteRam[spriteRamAddress] = data;
            spriteRamAddress++;
        }
        public byte SpriteRam_IO_Register_Read()
        {
            return spriteRam[spriteRamAddress];
        }
        //0x2005
        public void VRAM_Address_Register_1_Write(byte data)
        {
            if (vramHiLoToggle == 1)
            {
                scrollV = data;
                vramHiLoToggle = 0;
            }
            else
            {
                scrollH = data;
                if (scrollH > 239)
                {
                    scrollH = 0;
                }
                //Legacy of the Wizard fix
                //if (myEngine.fix_scrolloffset2)
                //{
                //    if (currentScanline < 240)
                //    {
                //        scrollH = (byte)(scrollH - currentScanline + 8);
                //    }
                //}

                //fix Battle of Olympus
                //if (myEngine.fix_scrolloffset1)
                //{
                //    if (currentScanline < 240)
                //    {
                //        scrollH = (byte)(scrollH - currentScanline);
                //    }
                //}

                //smb3 fix
                //if (myEngine.fix_scrolloffset3)
                //{
                //    if (currentScanline < 240)
                //        scrollH = 238;
                //}

                vramHiLoToggle = 1;
            }
        }
        //0x2006
        public void VRAM_Address_Register_2_Write(byte data)
        {
            if (vramHiLoToggle == 1)
            {
                prev_vramReadWriteAddress = vramReadWriteAddress;
                vramReadWriteAddress = (int)data << 8;
                vramHiLoToggle = 0;
            }
            else
            {
                vramReadWriteAddress = vramReadWriteAddress + (int)data;
                if ((prev_vramReadWriteAddress == 0) && (currentScanline < 240))
                {
                    //We may have a scrolling trick
                    //if ((vramReadWriteAddress >= 0x2000) && (vramReadWriteAddress <= 0x2400))
                    //    scrollH = (byte)(((vramReadWriteAddress - 0x2000) / 0x20) * 8 - currentScanline);
                }
                vramHiLoToggle = 1;
            }
        }
        //0x2007
        public void VRAM_IO_Register_Write(byte data)
        {
            if (vramReadWriteAddress < 0x2000)
            {
                myEngine.myMapper.WriteChrRom((ushort)vramReadWriteAddress, data);
            }
            else if ((vramReadWriteAddress >= 0x2000) && (vramReadWriteAddress < 0x3f00))
            {
                int vr = (vramReadWriteAddress & 0x2C00);
                if (myEngine.myCartridge.mirroring == MIRRORING.HORIZONTAL)
                {
                    if (vr == 0x2000)
                        nameTables[vramReadWriteAddress - 0x2000] = data;
                    else if (vr == 0x2400)
                        nameTables[(vramReadWriteAddress - 0x400) - 0x2000] = data;
                    else if (vr == 0x2800)
                        nameTables[vramReadWriteAddress - 0x400 - 0x2000] = data;
                    else if (vr == 0x2C00)
                        nameTables[(vramReadWriteAddress - 0x800) - 0x2000] = data;
                }
                else if (myEngine.myCartridge.mirroring == MIRRORING.VERTICAL)
                {
                    if (vr == 0x2000)
                        nameTables[vramReadWriteAddress - 0x2000] = data;
                    else if (vr == 0x2400)
                        nameTables[vramReadWriteAddress - 0x2000] = data;
                    else if (vr == 0x2800)
                        nameTables[vramReadWriteAddress - 0x800 - 0x2000] = data;
                    else if (vr == 0x2C00)
                        nameTables[(vramReadWriteAddress - 0x800) - 0x2000] = data;
                }
                else if (myEngine.myCartridge.mirroring == MIRRORING.ONE_SCREEN)
                {
                    if (myEngine.myCartridge.mirroringBase == 0x2000)
                    {
                        if (vr == 0x2000)
                            nameTables[vramReadWriteAddress - 0x2000] = data;
                        else if (vr == 0x2400)
                            nameTables[vramReadWriteAddress - 0x400 - 0x2000] = data;
                        else if (vr == 0x2800)
                            nameTables[vramReadWriteAddress - 0x800 - 0x2000] = data;
                        else if (vr == 0x2C00)
                            nameTables[vramReadWriteAddress - 0xC00 - 0x2000] = data;
                    }
                    else if (myEngine.myCartridge.mirroringBase == 0x2400)
                    {
                        if (vr == 0x2000)
                            nameTables[vramReadWriteAddress + 0x400 - 0x2000] = data;
                        else if (vr == 0x2400)
                            nameTables[vramReadWriteAddress - 0x2000] = data;
                        else if (vr == 0x2800)
                            nameTables[vramReadWriteAddress - 0x400 - 0x2000] = data;
                        else if (vr == 0x2C00)
                            nameTables[vramReadWriteAddress - 0x800 - 0x2000] = data;
                    }
                }
                //four screen mirroring (Croser)
                else
                {
                    nameTables[vramReadWriteAddress - 0x2000] = data;
                }
            }
            else if ((vramReadWriteAddress >= 0x3f00) && (vramReadWriteAddress < 0x3f20))
            {
                nameTables[vramReadWriteAddress - 0x2000] = data;
                if ((vramReadWriteAddress & 0x7) == 0)
                {
                    nameTables[(vramReadWriteAddress - 0x2000) ^ 0x10] = data;
                }
            }
            vramReadWriteAddress = vramReadWriteAddress + ppuAddressIncrement;
        }
        public byte VRAM_IO_Register_Read()
        {
            byte returnedValue = 0;
            if (vramReadWriteAddress < 0x3f00)
            {
                returnedValue = vramReadBuffer;
                if (vramReadWriteAddress >= 0x2000)
                {
                    vramReadBuffer = nameTables[vramReadWriteAddress - 0x2000];
                }
                else
                {
                    vramReadBuffer = myEngine.myMapper.ReadChrRom((ushort)(vramReadWriteAddress));
                }
            }
            else if (vramReadWriteAddress >= 0x4000)
            {
                //Console.WriteLine("I need vram mirroring {0:x}", vramReadWriteAddress);
                throw new ApplicationException();
            }
            else
            {
                returnedValue = nameTables[vramReadWriteAddress - 0x2000];
            }
            vramReadWriteAddress = vramReadWriteAddress + ppuAddressIncrement;
            return returnedValue;
        }
        //0x4014
        public void SpriteRam_DMA_Begin(byte data)
        {
            int i;
            for (i = 0; i < 0x100; i++)
            {
                spriteRam[i] = _bus.ReadMemory8((ushort)(((uint)data * 0x100) + i));
            }
        }
        #endregion

        #region Renderers
        public void RenderBackground()
        {
            int currentTileColumn;
            int tileNumber;
            //int scanInsideTile;
            int tileDataOffset;
            byte tiledata1, tiledata2;
            byte paletteHighBits;
            int pixelColor;
            int virtualScanline;
            int nameTableBase;
            int i; // genero loop, I should probably name this something better
            int startColumn, endColumn;
            int vScrollSide;
            int startTilePixel, endTilePixel;

            unsafe
            {
                for (vScrollSide = 0; vScrollSide < 2; vScrollSide++)
                {
                    virtualScanline = currentScanline + scrollH;
                    nameTableBase = nameTableAddress;
                    if (vScrollSide == 0)
                    {
                        if (virtualScanline >= 240)
                        {
                            if (nameTableAddress == 0x2000)
                                nameTableBase = 0x2800;
                            else if (nameTableAddress == 0x2400)
                                nameTableBase = 0x2C00;
                            else if (nameTableAddress == 0x2800)
                                nameTableBase = 0x2000;
                            else if (nameTableAddress == 0x2C00)
                                nameTableBase = 0x2400;

                            virtualScanline = virtualScanline - 240;
                        }
                        startColumn = scrollV / 8;
                        endColumn = 32;
                    }
                    else
                    {
                        if (virtualScanline >= 240)
                        {
                            if (nameTableAddress == 0x2000)
                                nameTableBase = 0x2C00;
                            else if (nameTableAddress == 0x2400)
                                nameTableBase = 0x2800;
                            else if (nameTableAddress == 0x2800)
                                nameTableBase = 0x2400;
                            else if (nameTableAddress == 0x2C00)
                                nameTableBase = 0x2000;

                            virtualScanline = virtualScanline - 240;
                        }
                        else
                        {
                            if (nameTableAddress == 0x2000)
                                nameTableBase = 0x2400;
                            else if (nameTableAddress == 0x2400)
                                nameTableBase = 0x2000;
                            else if (nameTableAddress == 0x2800)
                                nameTableBase = 0x2C00;
                            else if (nameTableAddress == 0x2C00)
                                nameTableBase = 0x2800;
                        }
                        startColumn = 0;
                        endColumn = (scrollV / 8) + 1;
                    }

                    //Next Try: Forcing two page only: 0x2000 and 0x2400				
                    if (myEngine.myCartridge.mirroring == MIRRORING.HORIZONTAL)
                    {
                        if (nameTableBase == 0x2400)
                            nameTableBase = 0x2000;
                        else if (nameTableBase == 0x2800)
                            nameTableBase = 0x2400;
                        else if (nameTableBase == 0x2C00)
                            nameTableBase = 0x2400;
                    }
                    else if (myEngine.myCartridge.mirroring == MIRRORING.VERTICAL)
                    {
                        if (nameTableBase == 0x2800)
                            nameTableBase = 0x2000;
                        else if (nameTableBase == 0x2C00)
                            nameTableBase = 0x2400;
                    }
                    else if (myEngine.myCartridge.mirroring == MIRRORING.ONE_SCREEN)
                    {
                        nameTableBase = (int)myEngine.myCartridge.mirroringBase;
                    }

                    for (currentTileColumn = startColumn; currentTileColumn < endColumn;
                        currentTileColumn++)
                    {
                        //Starting tile row is currentScanline / 8
                        //The offset in the tile is currentScanline % 8

                        //Step #1, get the tile number
                        tileNumber = nameTables[nameTableBase - 0x2000 + ((virtualScanline / 8) * 32) + currentTileColumn];

                        //Step #2, get the offset for the tile in the tile data
                        tileDataOffset = backgroundAddress + (tileNumber * 16);

                        //Step #3, get the tile data from chr rom
                        tiledata1 = myEngine.myMapper.ReadChrRom((ushort)(tileDataOffset + (virtualScanline % 8)));
                        tiledata2 = myEngine.myMapper.ReadChrRom((ushort)(tileDataOffset + (virtualScanline % 8) + 8));

                        //Step #4, get the attribute byte for the block of tiles we're in
                        //this will put us in the correct section in the palette table
                        paletteHighBits = nameTables[((nameTableBase - 0x2000 +
                            0x3c0 + (((virtualScanline / 8) / 4) * 8) + (currentTileColumn / 4)))];
                        paletteHighBits = (byte)(paletteHighBits >> ((4 * (((virtualScanline / 8) % 4) / 2)) +
                            (2 * ((currentTileColumn % 4) / 2))));
                        paletteHighBits = (byte)((paletteHighBits & 0x3) << 2);

                        //Step #5, render the line inside the tile to the offscreen buffer
                        if (vScrollSide == 0)
                        {
                            if (currentTileColumn == startColumn)
                            {
                                startTilePixel = scrollV % 8;
                                endTilePixel = 8;
                            }
                            else
                            {
                                startTilePixel = 0;
                                endTilePixel = 8;
                            }
                        }
                        else
                        {
                            if (currentTileColumn == endColumn)
                            {
                                startTilePixel = 0;
                                endTilePixel = scrollV % 8;
                            }
                            else
                            {
                                startTilePixel = 0;
                                endTilePixel = 8;
                            }
                        }

                        short* rptr = (short*)_renderingSurface.ToPointer();

                        for (i = startTilePixel; i < endTilePixel; i++)
                        {
                            pixelColor = paletteHighBits + (((tiledata2 & (1 << (7 - i))) >> (7 - i)) << 1) +
                                ((tiledata1 & (1 << (7 - i))) >> (7 - i));

                            if ((pixelColor % 4) != 0)
                            {
                                if (vScrollSide == 0)
                                {
                                    rptr[(currentScanline * 256) + (8 * currentTileColumn) - scrollV + i] =
                                    (short)Nes_Palette[(0x3f & nameTables[0x1f00 + pixelColor])];
                                }
                                else
                                {
                                    if (((8 * currentTileColumn) + (256 - scrollV) + i) < 256)
                                    {
                                        rptr[(currentScanline * 256) + (8 * currentTileColumn) + (256 - scrollV) + i] =
                                            (short)Nes_Palette[(0x3f & nameTables[0x1f00 + pixelColor])];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RenderSprites(int behind)
        {
            int i, j;
            int spriteLineToDraw;
            byte tiledata1, tiledata2;
            int offsetToSprite;
            byte paletteHighBits;
            int pixelColor;
            byte actualY;

            byte spriteId;

            //Step #1 loop through each sprite in sprite RAM
            //Back to front, early numbered sprites get drawing priority

            unsafe
            {
            for (i = 252; i >= 0; i = i - 4)
            {
                actualY = (byte)(spriteRam[i] + 1);
                //Step #2: if the sprite falls on the current scanline, draw it
                if (((spriteRam[i + 2] & 0x20) == behind) && (actualY <= currentScanline) && ((actualY + spriteSize) > currentScanline))
                {
                    spritesCrossed++;
                    //Step #3: Draw the sprites differently if they are 8x8 or 8x16
                    if (spriteSize == 8)
                    {
                        //Step #4: calculate which line of the sprite is currently being drawn
                        //Line to draw is: currentScanline - Y coord + 1

                        if ((spriteRam[i + 2] & 0x80) != 0x80)
                            spriteLineToDraw = currentScanline - actualY;
                        else
                            spriteLineToDraw = actualY + 7 - currentScanline;

                        //Step #5: calculate the offset to the sprite's data in
                        //our chr rom data 
                        offsetToSprite = spriteAddress + spriteRam[i + 1] * 16;

                        //Step #6: extract our tile data
                        tiledata1 = myEngine.myMapper.ReadChrRom((ushort)(offsetToSprite + spriteLineToDraw));
                        tiledata2 = myEngine.myMapper.ReadChrRom((ushort)(offsetToSprite + spriteLineToDraw + 8));

                        //Step #7: get the palette attribute data
                        paletteHighBits = (byte)((spriteRam[i + 2] & 0x3) << 2);

                        short* rptr = (short*)_renderingSurface.ToPointer();
                        //Step #8, render the line inside the tile to the offscreen buffer
                        for (j = 0; j < 8; j++)
                        {
                            if ((spriteRam[i + 2] & 0x40) == 0x40)
                            {
                                pixelColor = paletteHighBits + (((tiledata2 & (1 << (j))) >> (j)) << 1) +
                                    ((tiledata1 & (1 << (j))) >> (j));
                            }
                            else
                            {
                                pixelColor = paletteHighBits + (((tiledata2 & (1 << (7 - j))) >> (7 - j)) << 1) +
                                    ((tiledata1 & (1 << (7 - j))) >> (7 - j));
                            }
                            if ((pixelColor % 4) != 0)
                            {
                                if ((spriteRam[i + 3] + j) < 256)
                                {
                                    rptr[(currentScanline * 256) + (spriteRam[i + 3]) + j] =
                                            (short)Nes_Palette[(0x3f & nameTables[0x1f10 + pixelColor])];
                                    if (i == 0)
                                    {
                                        sprite0Hit = 1;
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        //The sprites are 8x16, to do so we draw two tiles with slightly
                        //different rules than we had before

                        //Step #4: Get the sprite ID and the offset in that 8x16 sprite
                        //Note, for vertical flip'd sprites, we start at 15, instead of
                        //8 like above to force the tiles in opposite order
                        spriteId = spriteRam[i + 1];
                        if ((spriteRam[i + 2] & 0x80) != 0x80)
                        {
                            spriteLineToDraw = currentScanline - actualY;
                        }
                        else
                        {
                            spriteLineToDraw = actualY + 15 - currentScanline;
                        }
                        //Step #5: We draw the sprite like two halves, so getting past the 
                        //first 8 puts us into the next tile
                        //If the ID is even, the tile is in 0x0000, odd 0x1000
                        if (spriteLineToDraw < 8)
                        {
                            //Draw the top tile
                            {
                                if ((spriteId % 2) == 0)
                                    offsetToSprite = 0x0000 + (spriteId) * 16;
                                else
                                    offsetToSprite = 0x1000 + (spriteId - 1) * 16;

                            }
                        }
                        else
                        {
                            //Draw the bottom tile
                            spriteLineToDraw = spriteLineToDraw - 8;

                            if ((spriteId % 2) == 0)
                                offsetToSprite = 0x0000 + (spriteId + 1) * 16;
                            else
                                offsetToSprite = 0x1000 + (spriteId) * 16;
                        }

                        //Step #6: extract our tile data
                        tiledata1 = myEngine.myMapper.ReadChrRom((ushort)(offsetToSprite + spriteLineToDraw));
                        tiledata2 = myEngine.myMapper.ReadChrRom((ushort)(offsetToSprite + spriteLineToDraw + 8));

                        //Step #7: get the palette attribute data
                        paletteHighBits = (byte)((spriteRam[i + 2] & 0x3) << 2);

                        //Step #8, render the line inside the tile to the offscreen buffer

                        short* rptr = (short*)_renderingSurface.ToPointer();
                        for (j = 0; j < 8; j++)
                        {
                            if ((spriteRam[i + 2] & 0x40) == 0x40)
                            {
                                pixelColor = paletteHighBits + (((tiledata2 & (1 << (j))) >> (j)) << 1) +
                                    ((tiledata1 & (1 << (j))) >> (j));
                            }
                            else
                            {
                                pixelColor = paletteHighBits + (((tiledata2 & (1 << (7 - j))) >> (7 - j)) << 1) +
                                    ((tiledata1 & (1 << (7 - j))) >> (7 - j));
                            }
                            if ((pixelColor % 4) != 0)
                            {
                                if ((spriteRam[i + 3] + j) < 256)
                                {
                                    rptr[(currentScanline * 256) + (spriteRam[i + 3]) + j] =
                                        (short)Nes_Palette[(0x3f & nameTables[0x1f10 + pixelColor])];

                                    if (i == 0)
                                    {
                                        sprite0Hit = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            }
        }

        public void RenderScanline(int scanlineNumber)
        {
            try
            {
                if (_renderingSurface == null)
                    return;

                unsafe
                {
                    short* rptr = (short*)_renderingSurface.ToPointer();

                    if ((uint)nameTables[0x1f00] == 63)
                    {
                        for (int i = 0; i < 256; i++)
                        {
                            rptr[(currentScanline * 256) + i] = 0;
                        }
                    }
                    else
                    {
                        int bufferStart = scanlineNumber * 256;
                        short value = (short)Nes_Palette[(uint)nameTables[0x1f00]];

                        for (int i = 0; i < 256; i++)
                        {
                            rptr[bufferStart + i] = value;
                        }
                    }

                    spritesCrossed = 0;

                    if (spritesVisible)
                    { RenderSprites(0x20); }

                    if (backgroundVisible)
                    { RenderBackground(); }

                    if (spritesVisible)
                    { RenderSprites(0); }
                }
            }
            catch (Exception)
            { 
            }

            //The mapper timer.
            if (backgroundVisible || spritesVisible)
            { this.myEngine.myMapper.TickTimer(); }
        }

        #endregion

        #region Palette
        public ushort[] Nes_Palette =
        { 0x8410, 0x17, 0x3017, 0x8014, 0xb80d, 0xb003, 0xb000, 0x9120,
	      0x7940, 0x1e0, 0x241, 0x1e4, 0x16c, 0x0, 0x20, 0x20,
	      0xce59, 0x2df, 0x41ff, 0xb199, 0xf995, 0xf9ab, 0xf9a3, 0xd240,
	      0xc300, 0x3bc0, 0x1c22, 0x4ac, 0x438, 0x1082, 0x841, 0x841,
	      0xffff, 0x4bf, 0x6c3f, 0xd37f, 0xfbb9, 0xfb73, 0xfbcb, 0xfc8b,
	      0xfd06, 0xa5e0, 0x56cd, 0x4eb5, 0x6df, 0x632c, 0x861, 0x861,
	      0xffff, 0x85ff, 0xbddf, 0xd5df, 0xfdfd, 0xfdf9, 0xfe36, 0xfe75,
	      0xfed4, 0xcf13, 0xaf76, 0xafbd, 0xb77f, 0xdefb, 0x1082, 0x1082 };
        public void ResetPalette()
        {
            ushort[] Nes_Palette_Default ={
	    0x8410, 0x17, 0x3017, 0x8014, 0xb80d, 0xb003, 0xb000, 0x9120,
	    0x7940, 0x1e0, 0x241, 0x1e4, 0x16c, 0x0, 0x20, 0x20,
	    0xce59, 0x2df, 0x41ff, 0xb199, 0xf995, 0xf9ab, 0xf9a3, 0xd240,
	    0xc300, 0x3bc0, 0x1c22, 0x4ac, 0x438, 0x1082, 0x841, 0x841,
	    0xffff, 0x4bf, 0x6c3f, 0xd37f, 0xfbb9, 0xfb73, 0xfbcb, 0xfc8b,
	    0xfd06, 0xa5e0, 0x56cd, 0x4eb5, 0x6df, 0x632c, 0x861, 0x861,
	    0xffff, 0x85ff, 0xbddf, 0xd5df, 0xfdfd, 0xfdf9, 0xfe36, 0xfe75,
	    0xfed4, 0xcf13, 0xaf76, 0xafbd, 0xb77f, 0xdefb, 0x1082, 0x1082
                                     };
            //Don't do this : Nes_Palette = Nes_Palette_Default;
            //It's not useful and the palette will not reset !!
            for (int i = 0; i < Nes_Palette_Default.Length; i++)
            {
                Nes_Palette[i] = Nes_Palette_Default[i];
            }
        }
        public void SavePalette(string FilePath)
        {
            Stream STR = new FileStream(FilePath, FileMode.Create);
            for (int i = 0; i < Nes_Palette.Length; i++)
            {
                byte RedValue = (byte)((Nes_Palette[i] & 0xF800) >> 8);
                byte GreenValue = (byte)((Nes_Palette[i] & 0x7E0) >> 3);
                byte BlueValue = (byte)((Nes_Palette[i] & 0x1F) << 3);
                STR.WriteByte(RedValue);
                STR.WriteByte(GreenValue);
                STR.WriteByte(BlueValue);
            }
            STR.Close();
        }
        public void LoadPalette(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                Stream STR = new FileStream(FilePath, FileMode.Open);
                byte[] buffer = new byte[192];
                STR.Read(buffer, 0, 192);
                int j = 0;
                for (int i = 0; i < 64; i++)
                {
                    byte RedValue = buffer[j]; j++;
                    byte GreenValue = buffer[j]; j++;
                    byte BlueValue = buffer[j]; j++;
                    Nes_Palette[i] = (ushort)((RedValue << 8) | (GreenValue << 3) | (BlueValue >> 3));
                }
                STR.Close();
            }
        }
        #endregion
        public void RestartPPU()
        {
            executeNMIonVBlank = false;
            ppuMaster = 0xff;
            spriteSize = 8;
            backgroundAddress = 0x0000;
            spriteAddress = 0x0000;
            ppuAddressIncrement = 1;
            nameTableAddress = 0x2000;
            currentScanline = 0;
            vramHiLoToggle = 0;
            vramReadBuffer = 0;
            spriteRamAddress = 0x0;
            scrollV = 0;
            scrollH = 0;
            sprite0Hit = 0;
        }

        public void SetRenderingSurface(IntPtr renderingSurface)
        {
            _renderingSurface = renderingSurface;
        }
    }
}
