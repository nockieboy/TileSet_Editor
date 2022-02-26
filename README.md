# Tile Set Editor

The Tile Set Editor is a bespoke application written to make it easier and quicker to import a graphics tile set from a graphics program and convert it to a hex-stream file that can be uploaded to an FPGA GPU and used with maximum flexibility.

Without this tool, the user is forced to convert their image file using Magick Converter to a format that is suitable to be written to the GPU's RAM, and they had little control over the actual colours used and their position in the palette table.

With Tile Set Editor, it's possible to take an image, optimise palette usage and output the palette separately as a .BIN file to be uploaded to the appropriate section of GPU memory alongside the actual image data.

## Installation

No installation is required - it's a Windows project.  None of that compiling for your build of Linux malarky, installing dependencies and whatnot.  The project is written in Visual Studio 2022, the executable is in the /bin/Debug/ folder - or you can open the project in Visual Studio, check the contents and build it from there.

## Usage

Run the executable - a window will appear.  The main section of the window is divided into approximately three areas from left to right - file properties, graphics/data grid view and palette information.  Assuming you have an 8-bit indexed PNG image (refer to your image editing software of choice to find out how to convert/save a file in that format), preferably 16 pixels wide by however many high, simply select the 'File>Load' menu option and load the PNG in.

TSE will display the image as a dataset of palette indices in the main grid view, along with palette information over on the right.  You can select a memory offset for the calculated palette addresses using the dropdown, and export the palette memory ready to load that into the GPU separately from the image.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

You can contact me in the main GPU project (for which this tool was created) over at the eevBlog forum: [FPGA-VGA-Controller-for-8-bit-computer](https://www.eevblog.com/forum/fpga/fpga-vga-controller-for-8-bit-computer/new/#new)

## License
[GPL-3.0](https://choosealicense.com/licenses/gpl-3.0/)

![GitHub](https://img.shields.io/github/license/nockieboy/TileSet_Editor?style=plastic)
