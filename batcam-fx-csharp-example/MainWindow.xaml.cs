using batcam_fx_csharp_example.Interpolator;
using batcam_fx_csharp_example.WebSocket;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebSocketSharp;

namespace batcam_fx_csharp_example {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        
        // MARK: - WebSocket Variables -------------------------------------------------------
        private const string CameraIp = "";
        private const string Username = "";
        private const string Password = "";

        private readonly WebSocketSharp.WebSocket _webSocket;
        private bool _shouldCloseWebsocket = false;
        // MARK: -----------------------------------------------------------------------------
        
        // MARK: - Interpolation Variables ---------------------------------------------------
        private readonly BeamformingInterpolator _interpolator;
        private readonly OpenCvSharp.Size _destinationSize = new(1600, 1200);
        // MARK: -----------------------------------------------------------------------------

        public MainWindow() {
            var options = new FxWebSocketOptions(CameraIp, Username, Password);
            _webSocket = new WebSocketSharp.WebSocket($"ws://{options.CameraIp}:80/ws", "subscribe");
            _interpolator = new BeamformingInterpolator(_destinationSize);
            
            PrepareWebSocket(options);
            InitializeComponent();
        }

        // MARK: - WebSocket Functions -------------------------------------------------------
        /// <summary>
        /// Preparing websocket for receiving BF Data from BATCAM FX.
        ///
        /// This function sets credential for WebSocket, and connect delegate functions to MainWindow.
        /// </summary>
        /// <param name="options">
        /// Used for setting credential for WebSocket.
        /// Refer to credential information from provided document.
        /// </param>
        private void PrepareWebSocket(FxWebSocketOptions options) {
            _webSocket.SetCredentials(options.Username, options.Password, true);
            try {
                _webSocket.OnOpen += OnWebSocketOpen;
                _webSocket.OnMessage += OnWebSocketMessage;
                _webSocket.OnError += OnWebSocketError;
                _webSocket.OnClose += OnWebSocketClose;
                _webSocket.EnableRedirection = true;
                _webSocket.Connect();
                if (_webSocket.IsAlive) {
                    Debug.WriteLine("[WebSocket/Initialization] Socket alive");
                    SendSubscribeMessage();
                }
            } catch (Exception exception) {
                Debug.WriteLine($"[WebSocket/Initialization] Error: {exception.Message}: {exception.StackTrace ?? string.Empty}");
            }
        }

        /// <summary>
        /// Sends a subscribe message to BATCAM FX for subscribing BF Data events.
        ///
        /// In this example, example only uses id 0 event.
        /// It can be subscribed more sent by EventTrigger, if you needed.  
        /// </summary>
        private void SendSubscribeMessage() {
            var message = new FxWebSocketMessage("subscribe", 0);
            var json = JsonConvert.SerializeObject(message);
            
            Debug.WriteLine($"[WebSocket] Sending Message... {message}");
            _webSocket.Send(json);
            Debug.WriteLine("[WebSocket] Message Sent");
        }

        private void OnWebSocketOpen(object? sender, EventArgs e) 
            => Debug.WriteLine("[Websocket] Opened");

        /// <summary>
        /// Handle data from WebSocket for creating overlay image.
        ///
        /// In this function, creates color matrix using OpenCV in BeamformingInterpolator class.
        /// See BeamformingInterpolator.cs for more information.
        /// Due to ToWritableBitmap function costs bit high,
        /// if you want to handle multiple sockets with program,
        /// you have to optimize some functions.
        /// </summary>
        /// <param name="sender">The caller who called this delegate function.</param>
        /// <param name="e">The response from WebSocket</param>
        private async void OnWebSocketMessage(object? sender, MessageEventArgs e) {
            if (e.IsPing) {
                Debug.WriteLine("[WebSocket/Message] Ping");
                return;
            }
            try {
                // Converts json response into Object<FxWebSocketResponse>
                var message = JsonConvert.DeserializeObject<FxWebSocketResponse>(e.Data);
                Mat? matrix = null;
                await Task.Run(() => 
                    // Creates matrix using BeamformingInterpolator. 
                    matrix = _interpolator.GenerateMatrix(message.BeamformingData, message.Gain)
                );
                if (matrix == null) return;

                Dispatcher.Invoke(() => {
                    // This changes color matrix into a WritableBitmap image.
                    // This example uses Gray256 for applying color map, you have to apply your own color.
                    var bitmap = matrix.ToWriteableBitmap(0, 0, PixelFormats.Indexed8, BitmapPalettes.Gray256);
                    ImageView.Source = bitmap;
                });
            } catch (Exception exception) {
                Debug.WriteLine($"[WebSocket/Message] Error: {exception.Message}: {exception.StackTrace ?? string.Empty}");
            }
        }

        private void OnWebSocketError(object? sender, ErrorEventArgs e) {
            Debug.WriteLine($"[WebSocket] Error: ({e.Message ?? string.Empty}) {e.Exception?.Message ?? string.Empty}: {e.Exception?.StackTrace ?? string.Empty}");
            throw new InvalidOperationException($"WebSocket got error {e.Message ?? "Unknown error"}");
        }

        /// <summary>
        /// Handle event on websocket close.
        ///
        /// Websocket can disconnect anytime for various reason.
        /// For preventing unhandled disconnection from BATCAM FX,
        /// the function calls connect websocket again from here,
        /// except when requested to close gracefully.
        /// 
        /// </summary>
        /// <param name="sender">The caller who called this delegate function.</param>
        /// <param name="closeEventArgs">The created data when WebSocket closing.</param>
        private void OnWebSocketClose(object? sender, CloseEventArgs closeEventArgs) {
            if (_shouldCloseWebsocket) {
                Debug.WriteLine("[Websocket] ShouldShutdownWebSocket is currently ON. App won't reconnect websocket");
                return;
            }

            Debug.WriteLine($"[WebSocket] Closed");

            _webSocket.Connect();
            if (_webSocket.IsAlive) {
                Debug.WriteLine("[WebSocket/Reconnect] Socket alive");
                SendSubscribeMessage();
            }
        }
        
        /// <summary>
        /// Request closing WebSocket normally.
        ///
        /// This function could be needed some situation like
        /// when user closing camera view.
        /// </summary>
        public void CloseWebSocketGracefully() {
            _shouldCloseWebsocket = true;
            _webSocket.OnOpen -= OnWebSocketOpen;
            _webSocket.OnMessage -= OnWebSocketMessage;
            _webSocket.OnError -= OnWebSocketError;
            _webSocket.OnClose -= OnWebSocketClose;
            Task.Run(() => {
                _webSocket.Close();
            });
        }
        // MARK: -----------------------------------------------------------------------------

        // XAML Update functions -------------------------------------------------------------
        /// <summary>
        /// Create a TextBlock near the slider to showing slider's value,
        /// and notice the value to BeamformingInterpolator for its calculation. 
        /// </summary>
        /// <param name="sender">The Slider defined in MainWindow.xaml.</param>
        /// <param name="e">The slider's value has been changed.</param>
        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            var slider = sender as Slider;
            var tag = slider?.Tag as string;
            var value = slider?.Value ?? 0;

            TextBlock? textBlockToDisplay = null;

            switch (tag) {
                case "Threshold":
                    textBlockToDisplay = TbThreshold;
                    _interpolator.SetThreshold(value);
                    break;
                case "Range":
                    textBlockToDisplay = TbRange;
                    _interpolator.SetRange(value);
                    break;
            }

            if (textBlockToDisplay != null) textBlockToDisplay.Text = $"{tag}: {value}";
        }
        // MARK: -----------------------------------------------------------------------------
    }
}
