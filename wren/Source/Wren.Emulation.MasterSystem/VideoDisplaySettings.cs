using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class VideoDisplaySettings
    {
        public const Int32 RightHandColumnsStatic = 0x80;
        public const Int32 TopRowsStatic = 0x40;
        public const Int32 HideLeftmostColumn = 0x20;
        public const Int32 EnableLineInterrupt = 0x10;
        public const Int32 ShiftSpritesLeft = 0x08;
        public const Int32 StretchScreen = 0x02;
        public const Int32 DisableScreenSynchronization = 0x01;

        public const Int32 EnableDisplay = 0x40;
        public const Int32 EnableVSyncInterrupt = 0x20;
        public const Int32 ExtendScreenBy4Rows = 0x10;
        public const Int32 ExtendScreenBy6Rows = 0x08;
        public const Int32 Use16x8Sprites = 0x02;
        public const Int32 ZoomSprites = 0x01;

        // Reg 0.
        public Boolean AreRightHandColumnsStatic { get; set; }
        public Boolean AreTopRowsStatic { get; set; }
        public Boolean IsLeftmostColumnHidden { get; set; }
        public Boolean IsLineInterruptEnabled { get; set; }
        public Boolean ShouldShiftSpritesLeft { get; set; }
        public Boolean IsScreenStretchEnabled { get; set; }
        public Boolean IsScreenSynchronizationDisabled { get; set; }

        // Reg 1.
        public Boolean IsDisplayEnabled { get; set; }
        public Boolean IsVSyncInterruptEnabled { get; set; }
        public Boolean ShouldExtendScreenBy4Rows { get; set; }
        public Boolean ShouldExtendScreenBy6Rows { get; set; }
        public Boolean ShouldUse8x16Sprites { get; set; }
        public Boolean IsSpriteZoomEnabled { get; set; }

        public Int32 NameTableBaseAddress { get; set; }
        public Int32 SpriteInformationBaseAddress { get; set; }
        public Boolean ShouldUseUpperVRamBankForSpriteTiles { get; set; }

        public Int32 BorderColor { get; set; }
        public Int32 HorizontalScrollOffset { get; set; }
        public Int32 VerticalScrollOffset { get; set; }
        public Int32 ScanLineCounterInitialValue { get; set; }

    }
}
