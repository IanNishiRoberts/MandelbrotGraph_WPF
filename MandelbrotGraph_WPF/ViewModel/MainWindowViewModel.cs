using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Threading.Tasks;
using MandelbrotGraph_WPF.Model;
using MandelbrotGraph_WPF.View;

namespace MandelbrotGraph_WPF.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private MandelbrotData _mandelData = new MandelbrotData();  // Struct for main data
        private readonly DelegateCommand<bool> _buttonGenerateGraph;  // Delegate for button press
        private BitmapImage _graph = new BitmapImage();  // Last image generated
        private bool _generateEnabled = true;  //  Button state
        private const string _WAITMESSAGE = "Generating Graph... Please Wait";  // Wait message on main form
        private string _waitMessage = "";  // Dynamic version of the wait message

        public MainWindowViewModel()
        {
            _buttonGenerateGraph = new DelegateCommand<bool>(
                (s) => { ShowGraph(); }, //Execute
                (s) => { return true; } //CanExecute
            );
        }

        public DelegateCommand<bool> ButtonGenerateGraph
        {
            get { return _buttonGenerateGraph; }
        }

        public async void ShowGraph()
        {
            GraphView currentView = new GraphView();
            currentView.Width = XResolution;
            currentView.Height = YResolution;
            currentView.Show();
            GenerateEnabled = false;

            _graph.Freeze();  // Freezing the image allows it to be passed from background threads to the UI
            
            await Task.Run(() => GenerateGraph());  // Run task asynchronously to not freeze UI
            
            currentView.graphImage.Source = _graph;
            GenerateEnabled = true;
        }

        public void GenerateGraph()
        {
            Bitmap pic = new Bitmap(XResolution, YResolution);
            
            double zX = 0;
            double zY = 0;
            double cX = 0;
            double cY = 0;
            double xZoom = ((XMax - XMin) / Convert.ToDouble(pic.Width));
            double yZoom = ((YMax - YMin) / Convert.ToDouble(pic.Height));
            double tempZX = 0;

            // Loop over each pixel in the image, performing the calculation for the specified number of iterations an determine whether the pixel is part of the graph or not
            int iterate = 0;
            for (int x = 0; x < pic.Width; x++)
            {
                cX = (xZoom * x) - Math.Abs(XMin);
                for (int y = 0; y < pic.Height; y++)
                {
                    WaitMessage = ((x + 1) * (y + 1)).ToString() + " of " + (pic.Width * pic.Height).ToString();

                    zX = 0;
                    zY = 0;
                    cY = (yZoom * y) - Math.Abs(YMin);
                    iterate = 0;

                    while ((zX * zX + zY * zY <=Precision) && (iterate < Iterations))
                    {
                        iterate++;
                        tempZX = zX;
                        zX = (zX * zX) - (zY * zY) + cX;
                        zY = (2 * tempZX * zY) + cY;
                    }

                    if (iterate != Iterations)
                    {
                        // Choose a color based on the number of iterations
                        pic.SetPixel(x, y, Color.FromArgb(iterate % 128 * 2, iterate % 32 * 7, iterate % 16 * 14));
                    }
                    else
                    {
                        pic.SetPixel(x, y, Color.Black);
                    }
                }
            }

            // In-memory convertor from the System.Drawing.Bitmap object to the System.Windows.Media.BitmapImage object
            // There is an available ImageSourceConvertor class but it doesn't like System.Drawing.Bitmap unless loaded from a resource, not generated dynamically
            MemoryStream ms = new MemoryStream();
            ((Bitmap)pic).Save(ms, ImageFormat.Bmp);
            BitmapImage finalImage = new BitmapImage();
            finalImage.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            finalImage.StreamSource = ms;
            finalImage.EndInit();

            finalImage.Freeze();
            _graph = finalImage;
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public bool GenerateEnabled
        {
            get { return _generateEnabled;  }
            set
            {
                _generateEnabled = value;
                OnPropertyChanged("GenerateEnabled");
            }
        }

        public string WaitMessage
        {
            get { return _waitMessage;  }
            set
            {
                if (value == "")
                {
                    _waitMessage = "";
                } else
                {
                    _waitMessage = _WAITMESSAGE + " (" + value + ")";
                }
                OnPropertyChanged("WaitMessage");
            }
        }
        public int XResolution
        {
            get { return _mandelData.OutputSize.Width; }
            set
            {
                if (value != _mandelData.OutputSize.Width)
                {
                    _mandelData.OutputSize = new Size(value, _mandelData.OutputSize.Height);
                    OnPropertyChanged("XResolution");
                }
            }
        }

        public int YResolution
        {
            get { return _mandelData.OutputSize.Height; }
            set
            {
                if (value != _mandelData.OutputSize.Height)
                {
                    _mandelData.OutputSize = new Size(_mandelData.OutputSize.Width, value);
                    OnPropertyChanged("YResolution");
                }
            }
        }

        public double XMax
        {
            get { return _mandelData.CoOrds.XMax; }
            set
            {
                if (value != _mandelData.CoOrds.XMax)
                {
                    _mandelData.CoOrds = new MandelCoOrds(value, _mandelData.CoOrds.XMin, _mandelData.CoOrds.YMax, _mandelData.CoOrds.YMin);
                    OnPropertyChanged("XMax");
                }
            }
        }

        public double XMin
        {
            get { return _mandelData.CoOrds.XMin; }
            set
            {
                if (value != _mandelData.CoOrds.XMin)
                {
                    _mandelData.CoOrds = new MandelCoOrds(_mandelData.CoOrds.XMax, value, _mandelData.CoOrds.YMax, _mandelData.CoOrds.YMin);
                    OnPropertyChanged("XMin");
                }
            }
        }

        public double YMax
        {
            get { return _mandelData.CoOrds.YMax; }
            set
            {
                if (value != _mandelData.CoOrds.YMax)
                {
                    _mandelData.CoOrds = new MandelCoOrds(_mandelData.CoOrds.XMax, _mandelData.CoOrds.XMin, value, _mandelData.CoOrds.YMin);
                    OnPropertyChanged("YMax");
                }
            }
        }

        public double YMin
        {
            get { return _mandelData.CoOrds.YMin; }
            set
            {
                if (value != _mandelData.CoOrds.YMin)
                {
                    _mandelData.CoOrds = new MandelCoOrds(_mandelData.CoOrds.XMax, _mandelData.CoOrds.XMin, _mandelData.CoOrds.YMax, value);
                    OnPropertyChanged("YMin");
                }
            }
        }

        public double Precision
        {
            get { return _mandelData.Precision; }
            set
            {
                if (value != _mandelData.Precision)
                {
                    _mandelData.Precision = value;
                    OnPropertyChanged("Precision");
                }
            }
        }

        public double Iterations
        {
            get { return _mandelData.Iterations;  }
            set
            {
                if (value != _mandelData.Iterations)
                {
                    _mandelData.Iterations = value;
                    OnPropertyChanged("Iterations");
                }
            }
        }
    }
}
