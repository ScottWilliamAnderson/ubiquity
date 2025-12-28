using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Mythic.Views
{
    /// <summary>
    /// Interaction logic for IntroScreenView.xaml
    /// Handles the intro video playback with proper input blocking and skip functionality
    /// </summary>
    public partial class IntroScreenView : UserControl
    {
        private bool isVideoPlaying = false;
        private bool isSkipping = false;
        private Storyboard fadeOutStoryboard;

        public IntroScreenView()
        {
            InitializeComponent();
            
            // Ensure the control can receive focus for input handling
            this.Focusable = true;
            this.Loaded += IntroScreenView_Loaded;
        }

        private void IntroScreenView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set focus to this control to ensure we capture all input
            this.Focus();
            
            // Get the fade-out animation from resources
            fadeOutStoryboard = (Storyboard)this.Resources["FadeOutAnimation"];
            if (fadeOutStoryboard != null)
            {
                fadeOutStoryboard.Completed += FadeOut_Completed;
            }
            
            // Start playing the intro video
            StartIntroVideo();
            
            // Subscribe to input events - use Preview events to catch them before bubbling
            this.PreviewKeyDown += IntroScreenView_PreviewKeyDown;
            this.PreviewMouseDown += IntroScreenView_PreviewMouseDown;
            this.PreviewMouseWheel += IntroScreenView_PreviewMouseWheel;
            
            // For gamepad support, we'll also handle generic input events
            // Playnite's input system might trigger these
            this.PreviewMouseLeftButtonDown += IntroScreenView_PreviewMouseLeftButtonDown;
            this.PreviewMouseRightButtonDown += IntroScreenView_PreviewMouseRightButtonDown;
        }

        private void StartIntroVideo()
        {
            try
            {
                // Set the video source - assuming it's in the Videos folder of the theme
                // The path should be relative to the theme directory
                string videoPath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "Themes", "Fullscreen", "Mythic", "Videos", "Intro.mp4"
                );
                
                // Alternative: Try to get the path from the current directory
                if (!System.IO.File.Exists(videoPath))
                {
                    // Try relative path from theme root
                    videoPath = "Videos/Intro.mp4";
                }
                
                if (System.IO.File.Exists(videoPath))
                {
                    IntroVideo.Source = new Uri(videoPath, UriKind.RelativeOrAbsolute);
                    IntroVideo.Play();
                    isVideoPlaying = true;
                }
                else
                {
                    // Video file not found, skip intro gracefully
                    SkipIntro();
                }
            }
            catch (Exception ex)
            {
                // If video fails to load, gracefully skip the intro
                System.Diagnostics.Debug.WriteLine($"Failed to load intro video: {ex.Message}");
                SkipIntro();
            }
        }

        #region Input Event Handlers

        /// <summary>
        /// Common handler for all input events - skips intro on first input, blocks all subsequent input
        /// </summary>
        private void HandleInputEvent(InputEventArgs e)
        {
            if (isVideoPlaying && !isSkipping)
            {
                SkipIntro();
                e.Handled = true;
            }
            else if (isVideoPlaying)
            {
                // Still playing but already skipping, just block the input
                e.Handled = true;
            }
        }

        private void IntroScreenView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HandleInputEvent(e);
        }

        private void IntroScreenView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HandleInputEvent(e);
        }

        private void IntroScreenView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleInputEvent(e);
        }

        private void IntroScreenView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            HandleInputEvent(e);
        }

        private void IntroScreenView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            HandleInputEvent(e);
        }

        #endregion

        #region Video Event Handlers

        private void IntroVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Video finished playing naturally
            if (!isSkipping)
            {
                SkipIntro();
            }
        }

        private void IntroVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // Video failed to play, gracefully skip
            System.Diagnostics.Debug.WriteLine($"Intro video failed: {e.ErrorException?.Message}");
            SkipIntro();
        }

        #endregion

        #region Skip and Cleanup Logic

        private void SkipIntro()
        {
            if (isSkipping)
            {
                // Already skipping, prevent multiple skip attempts
                return;
            }

            isSkipping = true;

            // Stop the video
            try
            {
                IntroVideo.Stop();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping video: {ex.Message}");
            }

            // Start fade-out animation
            if (fadeOutStoryboard != null)
            {
                fadeOutStoryboard.Begin();
            }
            else
            {
                // If animation is not available, hide immediately
                HideIntroScreen();
            }
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            HideIntroScreen();
        }

        private void HideIntroScreen()
        {
            isVideoPlaying = false;

            // Clean up the video source
            try
            {
                IntroVideo.Source = null;
                IntroVideo.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cleaning up video: {ex.Message}");
            }

            // Unsubscribe from events to prevent memory leaks
            this.PreviewKeyDown -= IntroScreenView_PreviewKeyDown;
            this.PreviewMouseDown -= IntroScreenView_PreviewMouseDown;
            this.PreviewMouseWheel -= IntroScreenView_PreviewMouseWheel;
            this.PreviewMouseLeftButtonDown -= IntroScreenView_PreviewMouseLeftButtonDown;
            this.PreviewMouseRightButtonDown -= IntroScreenView_PreviewMouseRightButtonDown;

            if (fadeOutStoryboard != null)
            {
                fadeOutStoryboard.Completed -= FadeOut_Completed;
            }

            // Hide the intro screen
            this.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}
