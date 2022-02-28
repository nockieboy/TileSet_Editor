using System.Diagnostics;
using System.Drawing.Imaging;
using System.Security;

namespace Tile_Set_Editor
{
    public partial class Form1 : Form
    {
        private tileset_binary current_tileset_binary = new tileset_binary();
        private bool hexDisplay = true;
        private bool dataLoaded = false;
        private bool _paletteLoaded = false;
        private bool paletteLoaded
        {
            get { return (_paletteLoaded); }
            set
            {
                if (_paletteLoaded != value) { _paletteLoaded = value; }

                if (_paletteLoaded)
                {
                    coloursInGridToolStripMenuItem.Enabled = true;
                    coloursToolStripMenuItem.Enabled = true;
                }
                else
                {
                    coloursInGridToolStripMenuItem.Enabled = false;
                    coloursToolStripMenuItem.Enabled = false;
                }
            }
        }
        private List<Byte> colour_indices = new List<Byte>();
        private int layer_sel = 0;
        public string outStr = "X2";
        public int paletteStartAddr = 0x1000;
        public int paletteEntryWidth = 4;  // number of bytes for each palette entry
        public int paletteSize = 0x100;
        private byte[] PaletteRGBA = new byte[1024];
        private PictureBox[] pictureBox = new PictureBox[256];
        private int _gridColour = 0;
        private int gridColour
        {
            get { return _gridColour; }
            set
            {
                if (_gridColour != value) {  _gridColour = value; }

                if (_gridColour == 0)   // Numbers only
                {
                    coloursInGridToolStripMenuItem.Checked = false;
                    coloursToolStripMenuItem.Checked = false;
                    numbersToolStripMenuItem.Checked = true;
                }
                else if (_gridColour == 1) // Numbers and colours
                {
                    coloursInGridToolStripMenuItem.Checked = true;
                    coloursToolStripMenuItem.Checked = false;
                    numbersToolStripMenuItem.Checked = false;
                }
                else if (_gridColour == 2) // Colours only
                {
                    coloursInGridToolStripMenuItem.Checked = false;
                    coloursToolStripMenuItem.Checked = true;
                    numbersToolStripMenuItem.Checked = false;
                }
            }
        }
        private string palSep = ",";
        private int activeColourIndex = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void Exit()
        {
            Application.Exit();
        }

        private void ExportPalette()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Binary File|*.bin";
            saveFileDialog1.Title = "Export palette data";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                FileStream fs = (FileStream)saveFileDialog1.OpenFile();
                fs.Write(PaletteRGBA, 0, PaletteRGBA.Length);
                fs.Close();
            }
        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportBinary();
        }

        private void PopulateColourIndex(byte[] data)
        {
            colour_indices.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                if (!colour_indices.Contains(data[i]))
                {
                    colour_indices.Add(data[i]);
                }
            }
            colour_indices.Sort();
            PopulatePaletteList();
        }

        private void AnalyseData()
        {
            if (dataLoaded)
            {
                PopulateColourIndex(current_tileset_binary.Data);
                current_tileset_binary.Colours = colour_indices.Count;
                current_tileset_binary.LowColIndex = colour_indices.FirstOrDefault();
                current_tileset_binary.HiColIndex = colour_indices.LastOrDefault();
            }
        }

        private void ImportBinary()
        {
            OpenFileDialog importBinaryDialogue;
            importBinaryDialogue = new OpenFileDialog();
            importBinaryDialogue.Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
            importBinaryDialogue.FilterIndex = 1;
            importBinaryDialogue.Title = "Import binary file";
            importBinaryDialogue.InitialDirectory = "C:\\";
            importBinaryDialogue.RestoreDirectory = true;
            if (importBinaryDialogue.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var data = default(byte[]);
                    var memstream = new MemoryStream();
                    var filePath = importBinaryDialogue.FileName;
                    var fileStream = importBinaryDialogue.OpenFile();
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        //var data = reader.ReadToEnd();
                        reader.BaseStream.CopyTo(memstream);
                        data = memstream.ToArray();
                        current_tileset_binary = new tileset_binary(importBinaryDialogue.SafeFileName, data);
                        current_tileset_binary.Path = filePath;
                        this.Text = importBinaryDialogue.SafeFileName + " | TSE";
                        SetupGridView();
                        PopulateGridView();
                        UpdateStuff();
                    }
                    dataLoaded = true;
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" + $"Details:\n\n{ex.StackTrace}");
                }

            }
        }

        private void ImportCSV()
        {
            byte colValue = 0;
            // Clear existing PaletteRGBA[] - importing a CSV will create a new palette from the CSV itself
            for (int i = 0; i < PaletteRGBA.Length; i++)
            {
                PaletteRGBA[i] = 0;
            }
            // Load CSV
            OpenFileDialog importCSVDialogue;
            importCSVDialogue = new OpenFileDialog();
            importCSVDialogue.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            importCSVDialogue.FilterIndex = 1;
            importCSVDialogue.Title = "Import CSV image";
            importCSVDialogue.InitialDirectory = "C:\\";
            importCSVDialogue.RestoreDirectory = true;
            if (importCSVDialogue.ShowDialog() == DialogResult.OK)
            {
                List<byte> csvInput = new List<byte>();
                using (var reader = new StreamReader(importCSVDialogue.FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line != null)
                        {
                            // a line looks something like this:
                            // 000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,000000FF,
                            string[] values = line.Split(',');
                            // values will be a 16-element array (assuming image is 16 pixels wide!), each element being a pixel represented by an RGBA value.
                            // or it could be a single line with 4,096 elements, depending on how the CSV is saved.
                            for (int v = 0; v < values.Length; v++)
                            {
                                if (values[v] == "") { continue; }
                                // extract RGBA components from value
                                byte iR = (byte)Int32.Parse(values[v].ToString().Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                                byte iG = (byte)Int32.Parse(values[v].ToString().Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                                byte iB = (byte)Int32.Parse(values[v].ToString().Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                                byte iA = (byte)Int32.Parse(values[v].ToString().Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                                // find a matching colour value in PaletteRGBA[]
                                bool found = false;
                                bool newEntry = false;
                                int colIndex = 0;
                                for (int i = 0; i < PaletteRGBA.Length - 3; i += 4)  // step through PaletteRGBA[] in steps of 4
                                {
                                    if (PaletteRGBA[i + 3] == 0) // if Alpha value is zero, we will assume this is an empty index entry
                                    {
                                        newEntry = true;
                                        colIndex = i;
                                        break;
                                    }
                                    else if (PaletteRGBA[i] == iR && PaletteRGBA[i + 1] == iG && PaletteRGBA[i + 2] == iB && PaletteRGBA[i + 3] == iA)
                                    {
                                        found = true;
                                        colIndex = i;
                                        break;
                                    }
                                }
                                if (newEntry) // if none found, create one
                                {
                                    PaletteRGBA[colIndex + 0] = iR;
                                    PaletteRGBA[colIndex + 1] = iG;
                                    PaletteRGBA[colIndex + 2] = iB;
                                    PaletteRGBA[colIndex + 3] = iA;
                                    colValue = (byte)(colIndex / 4);
                                    csvInput.Add(colValue);
                                }
                                else if (found) // entry found, update colValue
                                {
                                    colValue = (byte)(colIndex / 4);
                                    csvInput.Add(colValue);
                                }
                            }
                        }
                    }
                }
                // assign the index to current_tileset_binary, then move on to the next colour value.
                current_tileset_binary = new tileset_binary(importCSVDialogue.SafeFileName, csvInput.ToArray());
                current_tileset_binary.Path = importCSVDialogue.FileName;
                this.Text = importCSVDialogue.SafeFileName + " | TSE";
                dataLoaded = true;
                paletteLoaded = true;
                SetupGridView();
                PopulateGridView();
                UpdateStuff();
                PopulatePaletteDetails();
            }
        }

        private void LoadImage()
        {
            // Load image
            OpenFileDialog loadImageDialogue;
            loadImageDialogue = new OpenFileDialog();
            loadImageDialogue.Filter = "BMP files (*.bmp)|*.bmp|GIF files (*.gif)|*.gif|JPEG files (*.jpg)|*.jpg|PNG files (*.png)|*.png|TIFF files (*.tif)|*.tif|All files (*.*)|*.*";
            loadImageDialogue.FilterIndex = 4;
            loadImageDialogue.Title = "Load Image";
            loadImageDialogue.InitialDirectory = "C:\\";
            loadImageDialogue.RestoreDirectory = true;
            if (loadImageDialogue.ShowDialog() == DialogResult.OK)
            {
                var filePath = loadImageDialogue.FileName;
                Image image1 = Image.FromFile(filePath);
                Bitmap bitmap1 = new Bitmap(image1);
                List<byte> imgInput = new List<byte>();
                using (MemoryStream ms = new MemoryStream())
                {
                    image1.Save(ms, image1.RawFormat);
                    for (int y = 0; y < image1.Height; y++)
                    {
                        for(int x = 0; x < image1.Width; x++)
                        {
                            Color pixelColour = bitmap1.GetPixel(x, y);
                            // find a matching colour value in PaletteRGBA[]
                            bool found = false;
                            bool newEntry = false;
                            int colIndex = 0;
                            for (int i = 0; i < PaletteRGBA.Length - 3; i += 4)  // step through PaletteRGBA[] in steps of 4
                            {
                                if (PaletteRGBA[i + 3] == 0) // if Alpha value is zero, we will assume this is an empty index entry
                                {
                                    newEntry = true;
                                    colIndex = i;
                                    break;
                                }
                                else if (PaletteRGBA[i] == pixelColour.R && PaletteRGBA[i + 1] == pixelColour.G && PaletteRGBA[i + 2] == pixelColour.B && PaletteRGBA[i + 3] == pixelColour.A)
                                {
                                    found = true;
                                    colIndex = i;
                                    break;
                                }
                            }
                            if (newEntry) // if none found, create one
                            {
                                PaletteRGBA[colIndex + 0] = pixelColour.R;
                                PaletteRGBA[colIndex + 1] = pixelColour.G;
                                PaletteRGBA[colIndex + 2] = pixelColour.B;
                                PaletteRGBA[colIndex + 3] = pixelColour.A;
                                imgInput.Add((byte)(colIndex / 4));
                            }
                            else if (found) // entry found, update colValue
                            {
                                imgInput.Add((byte)(colIndex / 4));
                            }
                        }
                    }
                }
                bitmap1.Dispose();  // free up memory
                // Analyse palette
                ColorPalette pal = image1.Palette;
                for (int i = 0; i < pal.Entries.Length; i++)
                {
                    Color col = pal.Entries[i];
                    PaletteRGBA[i*4] = col.R;
                    PaletteRGBA[i*4+1] = col.G;
                    PaletteRGBA[i*4+2] = col.B;
                    PaletteRGBA[i*4+3] = 255;
                }

                current_tileset_binary = new tileset_binary(loadImageDialogue.SafeFileName, imgInput.ToArray());
                current_tileset_binary.Path = filePath;
                current_tileset_binary.Width = image1.Width;
                current_tileset_binary.Height = image1.Height;
                current_tileset_binary.PixelFormat = image1.PixelFormat;
                this.Text = loadImageDialogue.SafeFileName + " | TSE";
                dataLoaded = true;
                paletteLoaded = true;
                image1.Dispose();   // free up memory
                SetupGridView();
                PopulateGridView();
                UpdateStuff();
                PopulatePaletteDetails();
            }

        }

        private void ImportARGBPalette()
        {
            // These are text files with ;-delimited comments at the top (hopefully...
            // ...there are examples were a comment is broken by a CRLF or two...
            // The actual palette format is ARGB, one entry per line. This needs to be converted
            // to RGBA for the GPU palette system.
            // Valid lines are 8 characters long (AARRGGBB).
            //
            OpenFileDialog importPaletteDialogue;
            importPaletteDialogue = new OpenFileDialog();
            importPaletteDialogue.Filter = "TXT files (*.txt)|*.txt|All files (*.*)|*.*";
            importPaletteDialogue.FilterIndex = 1;
            importPaletteDialogue.Title = "Import palette file";
            importPaletteDialogue.InitialDirectory = "C:\\";
            importPaletteDialogue.RestoreDirectory = true;
            if (importPaletteDialogue.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string? line;
                    int paletteIndex = 0;
                    StreamReader reader = new StreamReader(importPaletteDialogue.OpenFile());
                    if (reader is not null)
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Length == 8) // valid comment line
                            {
                                PaletteRGBA[paletteIndex + 0] = (byte)int.Parse(line.Substring(2, 2), System.Globalization.NumberStyles.HexNumber); // R
                                PaletteRGBA[paletteIndex + 1] = (byte)int.Parse(line.Substring(4, 2), System.Globalization.NumberStyles.HexNumber); // G
                                PaletteRGBA[paletteIndex + 2] = (byte)int.Parse(line.Substring(6, 2), System.Globalization.NumberStyles.HexNumber); // B
                                PaletteRGBA[paletteIndex + 3] = (byte)int.Parse(line.Substring(0, 2), System.Globalization.NumberStyles.HexNumber); // A
                                if (paletteIndex < 1020)
                                {
                                    paletteIndex += 4; // increment paletteIndex in steps of 4
                                }
                            }
                        }
                        paletteLoaded = true;
                        PopulatePaletteDetails();
                        label2.Text = "Palette | " + importPaletteDialogue.SafeFileName;
                    }
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" + $"Details:\n\n{ex.StackTrace}");
                }

            }
        }

        public void UpdateStuff()
        {
            AnalyseData();
            PopulatePropertyGrid();
        }

        private void PopulateGridView()
        {
            int rowCount = 0;

            dataGridView1.Rows.Clear();

            // iterate through current_tileset_binary.Data in 16-byte blocks to populate each row in the DataGridView
            for (int i = 0; i < current_tileset_binary.Data.Length; i = i + 16)
            {
                
                if (i + 16 < current_tileset_binary.Data.Length)
                {
                    int rowId = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowId];
                    for (int x = 0; x < 16; x++)
                    {
                        row.Cells[x].Value = current_tileset_binary.Data[i + x].ToString(outStr);
                        row.Cells[x].Tag = (i + x).ToString();
                    }
                    dataGridView1.Rows[rowCount].HeaderCell.Value = rowCount.ToString();
                    rowCount += 1;
                }
                else
                {
                    int rowId = dataGridView1.Rows.Add();
                    DataGridViewRow row = dataGridView1.Rows[rowId];
                    int a = 0;
                    for (int x = i; x < current_tileset_binary.Data.Length; x++)
                    {
                        row.Cells[a].Value = current_tileset_binary.Data[x].ToString(outStr);
                        row.Cells[a].Tag = (x).ToString();
                        a += 1;
                    }
                    dataGridView1.Rows[rowCount].HeaderCell.Value = rowCount.ToString();
                    rowCount += 1;
                }
            }
            dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            if (gridColour == 1)
            {
                DisplayGridColours();
            }
            else if (gridColour == 2)
            {
                DisplayGridColours(true);
            }
        }

        private void PopulatePaletteList()
        {
            listView1.Items.Clear();
            foreach(Byte colour_index in colour_indices)
            {
                ListViewItem item = new ListViewItem();
                string value = PaletteRGBA[colour_index*4].ToString(outStr) + palSep + PaletteRGBA[colour_index*4+1].ToString(outStr) + palSep + PaletteRGBA[colour_index*4+2].ToString(outStr) + palSep + PaletteRGBA[colour_index*4+3].ToString(outStr);
                item.Text = colour_index.ToString(outStr);
                string addr = (paletteStartAddr + paletteSize*paletteEntryWidth*layer_sel + paletteEntryWidth*colour_index).ToString(outStr);
                item.SubItems.Add(addr);
                item.SubItems.Add(value);
                listView1.Items.Add(item);
            }
        }

        private void SequentialiseIndices()
        {
            if (!dataLoaded) return;

            byte pal_index = 0;
            foreach (Byte colour_index in colour_indices)
            {
                if (colour_index != pal_index)
                {
                    for (int i = 0; i < current_tileset_binary.Data.Length; i++)
                    {
                        if (current_tileset_binary.Data[i] == colour_index)
                        {
                            current_tileset_binary.Data[i] = pal_index;
                        }
                    }
                }
                pal_index++;
            }
            PopulateGridView();
            AnalyseData();
            PopulatePropertyGrid();
        }

        private void PopulatePaletteDetails()
        {
            PopulatePaletteList();
            // Remove any existing PictureBoxes (in case of a palette reload)
            foreach(var pb in this.Controls.OfType<PictureBox>())
            {
                pb.Click -= PaletteSelectClick;
                this.Controls.Remove(pb);
            }
            // Create a new palette from PictureBoxes
            int colIndex = 0;
            int pbIndex = 0;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    PictureBox pictureBox = new PictureBox
                    {
                        Name = "pBox" + colIndex.ToString(),
                        Size = new Size(16, 16),
                        Location = new Point(1042 + x * 16, 97 + y * 16),
                        Image = null,
                        BackColor = Color.FromArgb(PaletteRGBA[pbIndex], PaletteRGBA[pbIndex+1], PaletteRGBA[pbIndex+2]),
                        Tag = colIndex.ToString()
                    };
                    pictureBox.Click += PaletteSelectClick;
                    this.Controls.Add(pictureBox);
                    colIndex++;
                    pbIndex += 4;
                }
            }
        }

        private void PaletteSelectClick(object? sender, EventArgs e)
        {
            PictureBox? pb = sender as PictureBox;
            if (pb != null)
            {
                int index = Int32.Parse(pb.Tag.ToString());
                SelectPaletteListEntry(index.ToString(outStr));
                SetPaletteBorder(index);
                activeColourIndex = index;
            }
        }

        private void PopulatePropertyGrid()
        {
            propertyGrid1.SelectedObject = current_tileset_binary;
        }

        private void SetupGridView()
        {
            
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToResizeColumns = false;
            
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnCount = 16;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.RowHeadersVisible = true;
            //dataGridView1.RowHeadersWidth = 24;
            
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].Width = 28;
            }

            dataGridView1.GridColor = Color.Gray;
            dataGridView1.Name = current_tileset_binary.FileName;

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Name = i.ToString();
            }

            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.MultiSelect = false;
        }

        private void hexadecimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hexDisplay = !hexDisplay;
            hexadecimalToolStripMenuItem.Checked = hexDisplay;

            if (hexDisplay)
            {
                outStr = "X2";
            }
            else
            {
                outStr = "";
            }

            if (dataLoaded)
            {
                PopulateGridView();
                PopulatePaletteList();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLayerPalette.SelectedItem.ToString() == "Layer 0")
            {
                layer_sel = 0;
            }
            else if (cmbLayerPalette.SelectedItem.ToString() == "Layer 1")
            {
                layer_sel = 1;
            }
            else if (cmbLayerPalette.SelectedItem.ToString() == "Layer 2")
            {
                layer_sel = 2;
            }
            else if (cmbLayerPalette.SelectedItem.ToString() == "Layer 3")
            {
                layer_sel = 3;
            }
            PopulatePaletteList();
        }

        private void optimisePaletteIndicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SequentialiseIndices();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Binary File|*.bin";
            saveFileDialog1.Title = "Save File As...";
            saveFileDialog1.ShowDialog();
            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the data via a FileStream created by the OpenFile method.
                FileStream fs = (FileStream)saveFileDialog1.OpenFile();
                fs.Write(current_tileset_binary.Data, 0, current_tileset_binary.Data.Length);
                fs.Close();
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences prefs = new Preferences(this);
            prefs.Show();
        }

        private void paintNETPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // User wants to import a Paint.NET palette file.
            ImportARGBPalette();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Export palette data
            ExportPalette();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open an image
            LoadImage();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0)
                return;

            int palIndex = this.listView1.SelectedItems[0].Index;
            SetPaletteBorder(palIndex);
        }

        private void SetPaletteBorder(int index)
        {
            // reset all PictureBox borders
            foreach (var pb in this.Controls.OfType<PictureBox>())
            {
                pb.BorderStyle = BorderStyle.None;
            }
            // set border on selected palette entry
            ((PictureBox)this.Controls.Find("pBox" + index.ToString(), true)[0]).BorderStyle = BorderStyle.Fixed3D;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (paletteLoaded)
            {
                if (!chkDraw.Checked)
                {
                    object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    if (value == null) { return; }
                    if (outStr == "")
                    {
                        SetPaletteBorder(Int32.Parse(value.ToString()));
                    }
                    else
                    {
                        SetPaletteBorder(Int32.Parse(value.ToString(), System.Globalization.NumberStyles.HexNumber));
                    }
                    SelectPaletteListEntry(value.ToString());
                }
                else
                {
                    int value = activeColourIndex;
                    var tag = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
                    if (tag == null) { return; }
                    int index = Int32.Parse(tag.ToString());
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value;
                    current_tileset_binary.Data[index] = (byte)value;
                    if (gridColour > 0)
                    {
                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        style.BackColor = Color.FromArgb(PaletteRGBA[value * 4], PaletteRGBA[value * 4 + 1], PaletteRGBA[value * 4 + 2]);
                        if (gridColour == 2)
                        {
                            style.ForeColor = style.BackColor;
                        }
                        else
                        {
                            style.ForeColor = Color.FromArgb(style.BackColor.ToArgb() ^ 0xFFFFFF); // invert foreground text colour
                        }
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = style;
                    }
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var tag = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;
            if (tag == null) { return; }
            int index = Int32.Parse(tag.ToString());
            int value = Int32.Parse((string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            current_tileset_binary.Data[index] = (byte)value;
            if (gridColour > 0)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.BackColor = Color.FromArgb(PaletteRGBA[value * 4], PaletteRGBA[value * 4 + 1], PaletteRGBA[value * 4 + 2]);
                if (gridColour == 2)
                {
                    style.ForeColor = style.BackColor;
                }
                else
                {
                    style.ForeColor = Color.FromArgb(style.BackColor.ToArgb() ^ 0xFFFFFF); // invert foreground text colour
                }
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = style;
            }
        }

        private void SelectPaletteListEntry(string value)
        {
            var entry = listView1.FindItemWithText(value);
            if (entry != null)
            {
                listView1.SelectedIndices.Clear(); // clear current selection
                entry.Selected = true;
            }
        }

        private void ClearGridColours()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null)
                    {
                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        style.BackColor = Color.White;
                        cell.Style = style;
                    }
                }
            }
        }

        private void DisplayGridColours(bool onlyColours = false)
        {
            if (!paletteLoaded) {  return; }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null)
                    {
                        DataGridViewCellStyle style = new DataGridViewCellStyle();
                        int val = 0;
                        if (outStr == "")
                        {
                            val = Int32.Parse(cell.Value.ToString());
                        }
                        else
                        {
                            val = Int32.Parse(cell.Value.ToString(), System.Globalization.NumberStyles.HexNumber);
                        }
                        style.BackColor = Color.FromArgb(PaletteRGBA[val * 4], PaletteRGBA[val * 4 + 1], PaletteRGBA[val * 4 + 2]);
                        if (onlyColours)
                        {
                            style.ForeColor = style.BackColor;
                        }
                        else
                        {
                            style.ForeColor = Color.FromArgb(style.BackColor.ToArgb()^0xFFFFFF); // invert foreground text colour
                        }
                        cell.Style = style;
                    }
                }
            }
        }

        private void coloursInGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColour = 1;
            DisplayGridColours();
        }

        private void cSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportCSV();
        }

        private void coloursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColour = 2;
            DisplayGridColours(true);
        }

        private void numbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColour = 0;
            ClearGridColours();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }
    }
}