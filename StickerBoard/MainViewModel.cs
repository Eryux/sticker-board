using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace StickerBoard
{
    public class MainViewModel : ViewModel
    {
        // Properties --------------------------

        string _sBackground = "#FFFFFF";

        public string Background
        {
            get { return _sBackground; }
            set 
            {
                _sBackground = value;
                InvokePropertyChanged("Background");
            }
        }

        string _sForeground = "#000000";

        public string Foreground
        {
            get { return _sForeground; }
            set
            {
                _sForeground = value;
                InvokePropertyChanged("Foreground");
            }
        }

        int _marginTop = 50;

        public int MarginTop
        {
            get { return _marginTop; }
            set
            {
                _marginTop = value;
                InvokePropertyChanged("MarginTop");
            }
        }

        int _marginBottom = 50;

        public int MarginBottom
        {
            get { return _marginBottom; }
            set
            {
                _marginBottom = value;
                InvokePropertyChanged("MarginBottom");
            }
        }

        int _marginLeft = 50;

        public int MarginLeft
        {
            get { return _marginLeft; }
            set
            {
                _marginLeft = value;
                InvokePropertyChanged("MarginLeft");
            }
        }

        int _marginRight = 50;

        public int MarginRight
        {
            get { return _marginRight; }
            set
            {
                _marginRight = value;
                InvokePropertyChanged("MarginRight");
            }
        }

        int _thumbnailWidth = 256;

        public int ThumbnailWidth 
        {
            get { return _thumbnailWidth; }
            set
            {
                _thumbnailWidth = value;
                InvokePropertyChanged("ThumbnailWidth");
            }
        }

        int _thumbnailHeight = 256;

        public int ThumbnailHeight
        {
            get { return _thumbnailHeight; }
            set
            {
                _thumbnailHeight = value;
                InvokePropertyChanged("ThumbnailHeight");
            }
        }

        int _thumbnailHorizontalSpacing = 10;

        public int ThumbnailHorizontalSpacing
        {
            get { return _thumbnailHorizontalSpacing; }
            set
            {
                _thumbnailHorizontalSpacing = value;
                InvokePropertyChanged("ThumbnailHorizontalSpacing");
            }
        }

        int _thumbnailVerticalSpacing = 10;

        public int ThumbnailVerticalSpacing
        {
            get { return _thumbnailVerticalSpacing; }
            set 
            {
                _thumbnailVerticalSpacing = value;
                InvokePropertyChanged("ThumbnailVerticalSpacing");
            }
        }

        int _thumbnailPerRow = 5;

        public int ThumbnailPerRow
        {
            get { return _thumbnailPerRow; }
            set
            {
                _thumbnailPerRow = value;
                InvokePropertyChanged("ThumbnailPerRow");
            }
        }

        ObservableCollection<string> _imageFiles = new ObservableCollection<string>();

        public ObservableCollection<string> ImageFiles
        {
            get { return _imageFiles; }
            set
            {
                _imageFiles = value;
                InvokePropertyChanged("ImageFiles");
            }
        }

        string _labelSize = "N.A";

        public string LabelSize
        {
            get { return _labelSize; }
            set
            {
                _labelSize = value;
                InvokePropertyChanged("LabelSize");
            }
        }


        int _finalWidth = 0;

        int _finalHeight = 0;

        // --------------------------------------

        public MainViewModel() 
        {
            PropertyChanged += OnPropertyChanged;
            CalculateFinalSize();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Foreground" && e.PropertyName != "Background" && e.PropertyName != "LabelSize")
            {
                CalculateFinalSize();
            }
        }

        void CalculateFinalSize()
        {
            _finalWidth = MarginLeft + MarginRight + (ThumbnailWidth + ThumbnailHorizontalSpacing) * ((ThumbnailPerRow > ImageFiles.Count) ? ImageFiles.Count : ThumbnailPerRow);
            _finalHeight = MarginTop + MarginBottom + (ThumbnailHeight + ThumbnailVerticalSpacing) * (int) Math.Ceiling(ImageFiles.Count() / (double)ThumbnailPerRow);
            LabelSize = _finalWidth + " x " + _finalHeight + " px";
        }

        Bitmap ResizeImage(Image im, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);

            destImage.SetResolution(im.HorizontalResolution, im.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(im, destRect, 0, 0, im.Width, im.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        // --------------------------------------

        ICommand _command_AddFiles;

        public ICommand Command_AddFiles
        {
            get
            {
                if (_command_AddFiles == null)
                {
                    _command_AddFiles = new RelayCommand<object>("command.add_files", "Add stickers to board", o =>
                    {
                        var browser = new OpenFileDialog();
                        browser.Filter = "Images (*.PNG;*.GIF)|*.PNG;*.png;*.GIF;*.gif";
                        browser.Multiselect = true;
                        browser.Title = "Select your stickers";

                        DialogResult result = browser.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            foreach (String filename in browser.FileNames)
                            {
                                if (ImageFiles.Contains(filename) == false)
                                {
                                    ImageFiles.Add(filename);
                                }
                            }

                            CalculateFinalSize();
                        }
                    });
                }

                return _command_AddFiles;
            }
        }

        ICommand _command_RemoveFiles;

        public ICommand Command_RemoveFiles
        {
            get 
            {
                if (_command_RemoveFiles == null)
                {
                    _command_RemoveFiles = new RelayCommand<System.Windows.Controls.ListBox>("command.remove_files", "Remove stickers from board", input =>
                    {
                        List<string> files = new List<string>();

                        foreach (var item in input.SelectedItems)
                        {
                            files.Add(item.ToString());
                        }

                        foreach (var file in files)
                        {
                            ImageFiles.Remove(file);
                        }

                        CalculateFinalSize();
                    });
                }

                return _command_RemoveFiles;
            }
        }

        ICommand _command_Generate;

        public ICommand Command_Generate
        {
            get
            {
                if (_command_Generate == null)
                {
                    _command_Generate = new RelayCommand<object>("command.generate", "Generate and save board", o =>
                    {
                        // Ask for save path -
                        var browser = new SaveFileDialog();
                        browser.Title = "Save as";
                        browser.DefaultExt = ".png";
                        browser.Filter = "Images (*.png)|*.png";
                        browser.AddExtension = true;

                        if (browser.ShowDialog() == DialogResult.OK)
                        {
                            // Create new bitmap image
                            Image im = new Bitmap(_finalWidth, _finalHeight);

                            Graphics draw = Graphics.FromImage(im);

                            // Fill background
                            draw.FillRectangle(
                                new SolidBrush((Color)new ColorConverter().ConvertFromString(Background)),
                                0, 0, _finalWidth, _finalHeight
                            );

                            int col = 0; int row = 0;
                            int tRow = (int)Math.Ceiling((double)ImageFiles.Count / ThumbnailPerRow);

                            foreach (string image in ImageFiles)
                            {
                                int width = (ThumbnailHorizontalSpacing + ThumbnailWidth) * ThumbnailPerRow;

                                if (row == tRow - 1)
                                {
                                    width = (ThumbnailHorizontalSpacing + ThumbnailWidth) * (ImageFiles.Count - (row * ThumbnailPerRow));
                                }

                                int x = _finalWidth / 2 - width / 2 + (ThumbnailHorizontalSpacing + ThumbnailWidth) * col;
                                int y = MarginTop + (ThumbnailVerticalSpacing + ThumbnailHeight) * row;

                                // Draw background rectangle
                                draw.FillRectangle(
                                    new SolidBrush((Color)new ColorConverter().ConvertFromString(Foreground)),
                                    x, y, ThumbnailWidth, ThumbnailHeight
                                );

                                // Open image on resize image
                                Bitmap th = ResizeImage(Image.FromFile(image), ThumbnailWidth, ThumbnailHeight);

                                // Copy thumbnail into final image
                                draw.DrawImage(th, x, y, ThumbnailWidth, ThumbnailHeight);

                                // Clear
                                th.Dispose();

                                // Increments rows and cols
                                col++;
                                if (col >= ThumbnailPerRow) {
                                    col = 0; row++;
                                }
                            }

                            // Save -
                            im.Save(browser.FileName, ImageFormat.Png);

                            // Clear memory -
                            draw.Dispose();
                            im.Dispose();
                            GC.Collect();

                            MessageBox.Show("Board generated !", "Sticker board", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    });
                }

                return _command_Generate;
            }
        }

    }
}
