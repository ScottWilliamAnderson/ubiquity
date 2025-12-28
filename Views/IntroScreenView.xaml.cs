using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

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
            // Use Dispatcher to ensure the visual tree is fully loaded
            this.Dispatcher.BeginInvoke(new Action(() => this.Focus()), System.Windows.Threading.DispatcherPriority.Input);
            
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
        }

        private void StartIntroVideo()
        {
            try
            {
                // Try multiple paths to locate the intro video
                string[] possiblePaths = new string[]
                {
                    "Videos/Intro.mp4",  // Relative to theme directory (most common)
                    "Intro.mp4",          // Root of theme directory
                    System.IO.Path.Combine("Themes", "Fullscreen", "Mythic", "Videos", "Intro.mp4")  // Full relative path
                };
                
                string videoPath = null;
                foreach (string path in possiblePaths)
                {
                    if (System.IO.File.Exists(path))
                    {
                        videoPath = path;
                        break;
                    }
                }
                
                if (videoPath != null)
                {
                    // Use absolute path for URI if file exists
                    string absolutePath = System.IO.Path.GetFullPath(videoPath);
                    IntroVideo.Source = new Uri(absolutePath, UriKind.Absolute);
                    IntroVideo.Play();
                    isVideoPlaying = true;
                }
                else
                {
                    // Video file not found, skip intro gracefully
                    System.Diagnostics.Debug.WriteLine("Intro video file not found in any expected location");
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
