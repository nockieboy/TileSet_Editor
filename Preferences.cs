using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tile_Set_Editor
{
    public partial class Preferences : Form
    {
        public Form1 f1;

        public Preferences(Form1 _f1)
        {
            InitializeComponent();
            this.f1 = _f1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            f1.UpdateStuff();
            this.Close();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
            txtPalAddr.Text = f1.paletteStartAddr.ToString(f1.outStr);
            txtEntrySize.Text = f1.paletteEntryWidth.ToString(f1.outStr);
            txtPaletteSize.Text = f1.paletteSize.ToString(f1.outStr);
        }

        private void txtPalAddr_TextChanged(object sender, EventArgs e)
        {
            f1.paletteStartAddr = Int32.Parse(txtPalAddr.Text.Trim());
        }

        private void txtEntrySize_TextChanged(object sender, EventArgs e)
        {
            f1.paletteEntryWidth = Int32.Parse(txtEntrySize.Text.Trim());
        }

        private void txtPaletteSize_TextChanged(object sender, EventArgs e)
        {
            f1.paletteSize = Int32.Parse(txtPaletteSize.Text.Trim());
        }
    }
}
