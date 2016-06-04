using FaceBattleUWP.Common;
using FaceBattleUWP.ViewModel;
using JP.Utils.Helper;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FaceBattleUWP.View
{
    public sealed partial class UploadAnalysisPage : BindablePage
    {
        private UploadAnalysisViewModel UploadAnalysisVM;

        public bool ShowConfrim
        {
            get { return (bool)GetValue(ShowConfrimProperty); }
            set { SetValue(ShowConfrimProperty, value); }
        }

        public static readonly DependencyProperty ShowConfrimProperty =
            DependencyProperty.Register("ShowConfrim", typeof(bool), typeof(UploadAnalysisPage),
                new PropertyMetadata(true, OnShowConfrimPropertyChanged));

        private static void OnShowConfrimPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = d as UploadAnalysisPage;
            page.ToggleConfrimGridAnimation((bool)e.NewValue);
            page.ToggleImageAnimation((bool)e.NewValue);
        }

        public bool ShowResult
        {
            get { return (bool)GetValue(ShowResultProperty); }
            set { SetValue(ShowResultProperty, value); }
        }

        public static readonly DependencyProperty ShowResultProperty =
            DependencyProperty.Register("ShowResult", typeof(bool), typeof(UploadAnalysisPage),
                new PropertyMetadata(false, OnShowResultPropertyChanged));

        private static void OnShowResultPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var page = d as UploadAnalysisPage;
            //page.UpdateResultSimilary();
            //page.ToggleResultGridAnimation((bool)e.NewValue);
            //page.ToggleImageAnimation(true);
            //NavigationService.HistoryOperationsBeyondFrame.Push(() =>
            //{
            //    NavigationService.NaivgateToPage(typeof(MainPage), null);
            //    return true;
            //});
        }

        private Compositor _compositor;
        private Visual _confirmVisual;
        private Visual _imageVisual;
        private Visual _resultVisual;

        private UploadStruct _metaData;

        public UploadAnalysisPage()
        {
            this.InitializeComponent();
            this.DataContext = UploadAnalysisVM = new UploadAnalysisViewModel();
            NavigationCacheMode = NavigationCacheMode.Disabled;
            this.InitComposition();
            this.InitBinding();
            this.SizeChanged += UploadAnalysisPage_SizeChanged;
        }

        private void UploadAnalysisPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateResultSimilary();
        }

        private void InitBinding()
        {
            var b = new Binding()
            {
                Source = UploadAnalysisVM,
                Path = new PropertyPath("ShowConfirmGrid"),
                Mode = BindingMode.TwoWay,
            };
            this.SetBinding(ShowConfrimProperty, b);

            var b2 = new Binding()
            {
                Source = UploadAnalysisVM,
                Path = new PropertyPath("ShowResult"),
                Mode = BindingMode.TwoWay,
            };
            this.SetBinding(ShowResultProperty, b2);
        }

        private void InitComposition()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _confirmVisual = ElementCompositionPreview.GetElementVisual(ConfirmGrid);
            _confirmVisual.Offset = new Vector3(0f, 200f, 0f);
            _imageVisual = ElementCompositionPreview.GetElementVisual(DisplayImage);
            _imageVisual.Offset = new Vector3(0f, 100f, 0f);
            _resultVisual = ElementCompositionPreview.GetElementVisual(ResultGrid);
            _resultVisual.Offset = new Vector3(0f, 300f, 0f);
        }

        protected override void SetupTitleBar()
        {
            TitleBarHelper.SetUpDarkTitleBar();
        }

        protected override void SetUpStatusBar()
        {
            if (APIInfoHelper.HasStatusBar)
            {
                StatusBarHelper.SetUpWhiteStatusBar();
            }
        }

        private void ToggleConfrimGridAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0f : 200f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _confirmVisual.StartAnimation("Offset.y", offsetAnimation);
        }

        private void ToggleResultGridAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0f : 300f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _resultVisual.StartAnimation("Offset.y", offsetAnimation);
        }

        private void ToggleImageAnimation(bool stay)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, stay ? 0f : 100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _imageVisual.StartAnimation("Offset.y", offsetAnimation);
        }

        private void UpdateResultSimilary()
        {
            if (UploadAnalysisVM.CurrentResult != null)
            {
                var width = ProgressRect.ActualWidth * UploadAnalysisVM.CurrentResult.Similarity;
                ProgressRect.Clip = new RectangleGeometry()
                {
                    Rect = new Rect(0, 0, width, ProgressRect.ActualHeight),
                };
            }
        }

        private void ConfirmUploadBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleConfrimGridAnimation(false);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ShowConfrim = true;
        }
    }
}
