﻿using System;
using System.ComponentModel;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.DualScreen;

[assembly: Dependency(typeof(DualScreenService))]
namespace Xamarin.Forms.DualScreen
{
    internal class DualScreenService : IDualScreenService
    {
        public DualScreenService()
        {
        }

        public bool IsSpanned
        {
            get
            {
                var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;

                if (visibleBounds.Height > 1200 || visibleBounds.Width > 1200)
                    return true;

                return false;
            }
        }

        public bool IsLandscape
        {
            get
            {
                //when you have it spanned in double landscape it thinks that it's portrait
                // and visa versa so here's my hack for now to get the correct values for this
                if (IsSpanned)
                    return ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Portrait;
                else
                    return ApplicationView.GetForCurrentView().Orientation == ApplicationViewOrientation.Landscape;
            }
        }

        public event EventHandler OnScreenChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
        }

        public Rectangle GetHinge()
        {
            var screen = DisplayInformation.GetForCurrentView();

            if (IsLandscape)
            {
                if (IsSpanned)
                    return new Rectangle(0, 664 + 24, ScaledPixels(screen.ScreenWidthInRawPixels), 0);
                else
                    return new Rectangle(0, 664, ScaledPixels(screen.ScreenWidthInRawPixels), 0);
            }
            else
                return new Rectangle(720, 0, 0, ScaledPixels(screen.ScreenHeightInRawPixels));
        }

        double ScaledPixels(double n)
            => n / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

        public Point? GetLocationOnScreen(VisualElement visualElement)
        {
            var view = Platform.UWP.Platform.GetRenderer(visualElement);

            if (view?.ContainerElement == null)
                return null;

            var ttv = view.ContainerElement.TransformToVisual(Window.Current.Content);
            Windows.Foundation.Point screenCoords = ttv.TransformPoint(new Windows.Foundation.Point(0, 0));

            return new Point(screenCoords.X, screenCoords.Y);
        }
    }
}