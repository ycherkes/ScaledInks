using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ScaledInks
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private float _scale;

        public MainPage()
        {
            InitializeComponent();

            InkControl.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse |
                                                       CoreInputDeviceTypes.Pen |
                                                       CoreInputDeviceTypes.Touch;

            InkControl.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            InkControl.InkPresenter.StrokesErased += InkPresenter_StrokesErased;
            _scale = 2F;
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            CanvasControl.Invalidate();
        }

        private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            CanvasControl.Invalidate();
        }

        private void CanvasControl_OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var strokes = InkControl.InkPresenter.StrokeContainer.GetStrokes();
            var scaledStrokes = GetScaledStrokes(strokes, _scale);
            args.DrawingSession.Clear(CanvasControl.ClearColor);
            args.DrawingSession.DrawInk(scaledStrokes);
        }

        private static IEnumerable<InkStroke> GetScaledStrokes(IEnumerable<InkStroke> stroke, float scale)
        {
            var scaleMatrix = Matrix3x2.CreateScale(scale);

            var strokes = stroke.Select(x => x.Clone()).ToArray();

            foreach (var inkStroke in strokes)
            {
                inkStroke.PointTransform = scaleMatrix;
                var da = inkStroke.DrawingAttributes;
                var daSize = da.Size;
                daSize.Width = daSize.Width * scale;
                daSize.Height = daSize.Height * scale;
                da.Size = daSize;
                inkStroke.DrawingAttributes = da;
            }

            return strokes;
        }

        private void ScaleSlider_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _scale =  ScaleSlider.SelectedIndex + 1;
            CanvasControl.Invalidate();
        }
    }
}