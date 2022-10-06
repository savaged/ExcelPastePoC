using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace ExcelPastePoC.ClipboardSupport
{
    public static class SystemClipboardFacade
    {
        public static void Set(string value)
        {
            TrySet(value);
        }

        public static string GetAsCsv()
        {
            return Clipboard.GetText(TextDataFormat.CommaSeparatedValue);
        }

        public static void Clear()
        {
            Clipboard.Clear();
        }

        private static void TrySet(string value)
        {
            try
            {
                Clipboard.SetText(value);
            }
            catch (COMException ex)
            {
                const uint CLIPBRD_E_CANT_OPEN = 0x800401D0;
                if ((uint)ex.ErrorCode != CLIPBRD_E_CANT_OPEN)
                {
                    throw new InvalidOperationException(
                        "Clipboard in use by another process. Please try again. " +
                        "If this issue persists close all other applications.");
                }
            }
        }

    }

}

