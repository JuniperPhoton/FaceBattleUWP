using FaceBattleUWP.Common;
using FaceBattleUWP.ViewModel;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using SamplesCommon.ImageLoader;
using System;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FaceBattleUWP.View
{
    public sealed partial class MainPage : BindablePage
    {


        public bool ShowAddControl
        {
            get { return (bool)GetValue(ShowAddControlProperty); }
            set { SetValue(ShowAddControlProperty, value); }
        }

        public static readonly DependencyProperty ShowAddControlProperty =
            DependencyProperty.Register("ShowAddControl", typeof(bool), typeof(MainPage),
                new PropertyMetadata(false, OnShowAddControlPropertyChanged));

        private static void OnShowAddControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = d as MainPage;
            if ((bool)e.NewValue)
            {
                page.ShowNewControl();
            }
            else
            {
                page.HideNewControl();
            }
        }

        private Compositor _compositor;
        private Visual _triImageVisual;
        private Visual _newGridVisual;
        private Visual _fabVisual;
        private Visual _addControlVisual;

        private ContainerVisual _containerForVisuals;
        private SpriteVisual _colorVisual;
        private ScalarKeyFrameAnimation _bloomAnimation;
        private IImageLoader _imageLoader;
        private IManagedSurface _circleMaskSurface;

        private MainViewModel MainVM;

        public MainPage()
        {
            this.InitializeComponent();
            if (!DesignMode.DesignModeEnabled)
            {
                this.DataContext = MainVM = new MainViewModel();
                this.InitComposition();
                this.Loaded += MainPage_Loaded;
                this.SizeChanged += MainPage_SizeChanged;
                this.InitBinding();
            }
        }

        private void InitBinding()
        {
            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("ShowAddControl"),
                Mode = BindingMode.TwoWay
            };
            this.SetBinding(ShowAddControlProperty, b);
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTriPos();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTriPosUsingAnimation();
        }

        private void InitComposition()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _triImageVisual = ElementCompositionPreview.GetElementVisual(TriImage);
            _newGridVisual = ElementCompositionPreview.GetElementVisual(NewGrid);
            _fabVisual = ElementCompositionPreview.GetElementVisual(AddNewFABBtn);
            _addControlVisual = ElementCompositionPreview.GetElementVisual(AddControl);

            _addControlVisual.Opacity = 0f;
            _addControlVisual.Offset = new Vector3(0f, 100f, 0f);

            _containerForVisuals = _compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(NewGrid, _containerForVisuals);

            // initialize the ImageLoader and create the circle mask
            _imageLoader = ImageLoaderFactory.CreateImageLoader(_compositor);
            _circleMaskSurface = _imageLoader.CreateManagedSurfaceFromUri(new Uri("ms-appx:///Assets/Icon/CircleOpacityMask.png"));
        }

        #region Public API surface
        public void StartColorAnimation(Color color, Rect initialBounds, Rect finalBounds)
        {
            _colorVisual = CreateVisualWithColorAndPosition(color, initialBounds, finalBounds);

            _containerForVisuals.Children.RemoveAll();
            // add our solid colored circle visual to the live visual tree via the container
            _containerForVisuals.Children.InsertAtTop(_colorVisual);

            // now that we have a visual, let's run the animation 
            TriggerBloomAnimation(_colorVisual);
        }

        /// <summary>
        /// Cleans up any remaining surfaces.
        /// </summary>
        public void DisposeSurfaces()
        {
            _circleMaskSurface.Dispose();
        }

        #endregion

        #region All the heavy lifting
        /// <summary>
        /// Creates a Visual using the specific color and constraints
        /// </summary>
        private SpriteVisual CreateVisualWithColorAndPosition(Color color,
                                                              Rect initialBounds,
                                                              Rect finalBounds)
        {

            // init the position and dimensions for our visual
            var width = (float)initialBounds.Width;
            var height = (float)initialBounds.Height;
            var positionX = initialBounds.X;
            var positionY = initialBounds.Y;

            // we want our visual (a circle) to completely fit within the bounding box
            var circleColorVisualDiameter = (float)Math.Min(width, height);

            // the diameter of the circular visual is an essential bit of information
            // in initializing our bloom animation - a one-time thing
            InitializeBloomAnimation(circleColorVisualDiameter / 2, finalBounds); // passing in the radius

            // we are going to some lengths to have the visual precisely placed
            // such that the center of the circular visual coincides with the center of the AppBarButton.
            // it is important that the bloom originate there
            var diagonal = Math.Sqrt(2 * (circleColorVisualDiameter * circleColorVisualDiameter));
            var deltaForOffset = (diagonal - circleColorVisualDiameter) / 2;

            // now we have everything we need to calculate the position (offset) and size of the visual
            var offset = new Vector3((float)positionX + (float)deltaForOffset + circleColorVisualDiameter / 2,
                                     (float)positionY + circleColorVisualDiameter / 2,
                                     0f);
            var size = new Vector2(circleColorVisualDiameter);

            // create the visual with a solid colored circle as brush
            SpriteVisual coloredCircleVisual = _compositor.CreateSpriteVisual();
            coloredCircleVisual.Brush = CreateCircleBrushWithColor(color);
            coloredCircleVisual.Offset = offset;
            coloredCircleVisual.Size = size;

            // we want our scale animation to be anchored around the center of the visual
            coloredCircleVisual.AnchorPoint = new Vector2(0.5f, 0.5f);

            return coloredCircleVisual;
        }


        /// <summary>
        /// Creates a circular solid colored brush that we can apply to a visual
        /// </summary>
        private CompositionEffectBrush CreateCircleBrushWithColor(Color color)
        {

            var colorBrush = _compositor.CreateColorBrush(color);

            //
            // Because Windows.UI.Composition does not have a Circle visual, we will 
            // work around by using a circular opacity mask
            // Create a simple Composite Effect, using DestinationIn (S * DA), 
            // with a color source and a named parameter source.
            //
            var effect = new CompositeEffect
            {
                Mode = CanvasComposite.DestinationIn,
                Sources =
                {
                    new ColorSourceEffect()
                    {
                        Color = color
                    },
                    new CompositionEffectSourceParameter("mask")
                }
            };
            var factory = _compositor.CreateEffectFactory(effect);
            var brush = factory.CreateBrush();

            //
            // Create the mask brush using the circle mask
            //
            CompositionSurfaceBrush maskBrush = _compositor.CreateSurfaceBrush();
            maskBrush.Surface = _circleMaskSurface.Surface;
            brush.SetSourceParameter("mask", maskBrush);

            return brush;
        }

        /// <summary>
        /// Creates an animation template for a "color bloom" type effect on a circular colored visual.
        /// This is a sub-second animation on the Scale property of the visual.
        /// 
        /// <param name="initialRadius">the Radius of the circular visual</param>
        /// <param name="finalBounds">the final area to occupy</param>
        /// </summary>
        private void InitializeBloomAnimation(float initialRadius, Rect finalBounds)
        {
            var maxWidth = finalBounds.Width;
            var maxHeight = finalBounds.Height;

            // when fully scaled, the circle must cover the entire viewport
            // so we use the window's diagonal width as our max radius, assuming 0,0 placement
            var maxRadius = (float)Math.Sqrt((maxWidth * maxWidth) + (maxHeight * maxHeight)); // hypotenuse

            // the scale factor is the ratio of the max radius to the original radius
            var scaleFactor = (float)Math.Round(maxRadius / initialRadius, MidpointRounding.AwayFromZero);


            var bloomEase = _compositor.CreateCubicBezierEasingFunction(  //these numbers seem to give a consistent circle even on small sized windows
                    new Vector2(0.1f, 0.4f),
                    new Vector2(0.99f, 0.65f)
                );
            _bloomAnimation = _compositor.CreateScalarKeyFrameAnimation();
            _bloomAnimation.InsertKeyFrame(1.0f, scaleFactor);
            _bloomAnimation.Duration = TimeSpan.FromMilliseconds(500); // keeping this under a sec to not be obtrusive

        }

        private ScalarKeyFrameAnimation InitializeBloomDismissAnimation()
        {
            var anim = _compositor.CreateScalarKeyFrameAnimation();
            anim.InsertKeyFrame(1.0f, 0.01f);
            anim.Duration = TimeSpan.FromMilliseconds(500); // keeping this under a sec to not be obtrusive
            return anim;
        }

        /// <summary>
        /// Runs the animation
        /// </summary>
        private void TriggerBloomAnimation(SpriteVisual colorVisual)
        {

            // animate the Scale of the visual within a scoped batch
            // this gives us transactionality and allows us to do work once the transaction completes
            var batchTransaction = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            // as with all animations on Visuals, these too will run independent of the UI thread
            // so if the UI thread is busy with app code or doing layout on state/page transition,
            // these animations still run uninterruped and glitch free
            colorVisual.StartAnimation("Scale.X", _bloomAnimation);
            colorVisual.StartAnimation("Scale.Y", _bloomAnimation);

            batchTransaction.End();
        }

        #endregion

        private void UpdateTriPosUsingAnimation()
        {
            var selectedTB = GetSelectedTB();
            var targetX = selectedTB.TransformToVisual(this).TransformPoint(new Point(0, 0)).X +
                selectedTB.ActualWidth / 2d - 15d;

            TriImage.Visibility = Visibility.Visible;

            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, (float)targetX);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _triImageVisual.StartAnimation("Offset.x", offsetAnimation);
        }

        private void UpdateTriPos()
        {
            var selectedTB = GetSelectedTB();
            var targetX = selectedTB.TransformToVisual(this).TransformPoint(new Point(0, 0)).X +
                selectedTB.ActualWidth / 2d - 15d;
            _triImageVisual.Offset = new Vector3((float)targetX, 0f, 0f);
        }

        private void ToggleFABAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0f : 100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _fabVisual.StartAnimation("Offset.y", offsetAnimation);
        }

        private void ToggleAddControlAnimation(bool show, int delayTime = 0)
        {
            AddControl.Visibility = Visibility.Visible;

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1, show ? 1f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(300);
            fadeAnimation.DelayTime = TimeSpan.FromMilliseconds(delayTime);

            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1, show ? 0f : 100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(300);
            offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(delayTime);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _addControlVisual.StartAnimation("Opacity", fadeAnimation);
            _addControlVisual.StartAnimation("Offset.y", offsetAnimation);
            batch.Completed += (sender, e) =>
              {
                  if (!show)
                  {
                      AddControl.Visibility = Visibility.Collapsed;
                  }
              };
            batch.End();
        }

        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTriPosUsingAnimation();
            if (MainPivot.SelectedIndex == 2)
            {
                ToggleFABAnimation(false);
            }
            else ToggleFABAnimation(true);
        }

        private TextBlock GetSelectedTB()
        {
            switch (MainPivot.SelectedIndex)
            {
                case 0:
                    {
                        return PublicTB;
                    };
                case 1:
                    {
                        return HistoryTB;
                    };
                case 2:
                    {
                        return MeTB;
                    };
                default:
                    {
                        return PublicTB;
                    }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.ClearBaskStack();
            base.OnNavigatedTo(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tag = int.Parse((string)(sender as Button).Tag);
            MainPivot.SelectedIndex = tag;
        }

        private void AddNewFABBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowAddControl = true;
        }

        private void AddControl_OnClickCancel()
        {
            ShowAddControl = false;
        }

        private void ShowNewControl()
        {
            var targetOffsetX = AddNewFABBtn.TransformToVisual(this).TransformPoint(new Point(0, 0));
            StartColorAnimation((App.Current.Resources["FaceBattleMainColor"] as SolidColorBrush).Color,
                new Rect(targetOffsetX.X, targetOffsetX.Y, 50d, 50d), new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            ToggleAddControlAnimation(true, 300);

            NavigationService.HistoryOperationsBeyondFrame.Push(() =>
            {
                if (((Window.Current.Content as Frame).Content as Page).GetType() != typeof(MainPage))
                {
                    return null;
                }
                if (AddControl.Visibility == Visibility.Visible && ((Window.Current.Content as Frame).Content as Page).GetType() == typeof(MainPage))
                {
                    ShowAddControl = false;
                    return true;
                }
                return false;
            });
        }

        private void HideNewControl()
        {
            ToggleAddControlAnimation(false, 0);

            var disMissAnimation = InitializeBloomDismissAnimation();
            if (_colorVisual != null)
            {
                var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                _colorVisual.StartAnimation("Scale.x", disMissAnimation);
                _colorVisual.StartAnimation("Scale.y", disMissAnimation);
                batch.Completed += (sender, e) =>
                {
                    _containerForVisuals.Children.RemoveAll();
                };
                batch.End();
            }
        }
    }
}
