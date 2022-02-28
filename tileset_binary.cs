using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile_Set_Editor
{
    [DefaultPropertyAttribute("FileName")]
    public class tileset_binary
    {
        [CategoryAttribute("File Data"), DescriptionAttribute("Name of currently-loaded file.")]
        public string FileName { get; set; }    // The name of the binary file
        [CategoryAttribute("File Data"), DescriptionAttribute("Full path to currently-loaded file.")]
        public string Path { get; set; }        // The full path to the binary file, minus the name
        [CategoryAttribute("File Data"), DescriptionAttribute("File contents.")]
        public byte[] Data { get; set; }        // Binary tileset data
        [CategoryAttribute("File Data"), DescriptionAttribute("Size of currently-loaded file in bytes.")]
        public int Size { get; set; }
        [CategoryAttribute("Palette"), DescriptionAttribute("The number of colours referenced in the file.")]
        public int Colours { get; set; }
        [CategoryAttribute("Palette"), DescriptionAttribute("The lowest palette index referenced in the file.")]
        public int LowColIndex { get; set; }
        [CategoryAttribute("Palette"), DescriptionAttribute("The highest palette index referenced in the file.")]
        public int HiColIndex { get; set; }
        [CategoryAttribute("Image"), DescriptionAttribute("The width of the image.")]
        public int Width { get; set; }
        [CategoryAttribute("Image"), DescriptionAttribute("The height of the image.")]
        public int Height { get; set; }
        [CategoryAttribute("Image"), DescriptionAttribute("The pixel format of the image.")]
        public PixelFormat PixelFormat { get; set; }

        public tileset_binary()
        {
            FileName = "";
            Path = "";
            Data = new byte[] {0};
            Size = 0;
            Colours = 0;
            LowColIndex = 0;
            HiColIndex = 0;
            Width = 0;
            Height = 0;
            PixelFormat = PixelFormat.DontCare;
        }

        public tileset_binary(string filename, byte[] data)
        {
            FileName = filename;
            Path = "";
            Data = data;
            Size = data.Length;
            Colours = 0;
            LowColIndex = 0;
            HiColIndex = 0;
            Width = 0;
            Height = 0;
            PixelFormat = PixelFormat.DontCare;
        }

        public override string ToString()
        {
            return BitConverter.ToString(Data, 0, Data.Length).Replace('-', ',');
        }
    }
}
