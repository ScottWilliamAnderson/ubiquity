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
        private bool isHiding = false;
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
            
            // Add low-level input handling for gamepad support
            try
            {
                InputManager.Current.PreProcessInput += OnPreProcessInput;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Could not register PreProcessInput handler: {ex.Message}");
            }
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
        /// Low-level input handler for capturing gamepad and other input devices
        /// This catches input before it's processed by the WPF event system
        /// </summary>
        private void OnPreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            if (!isVideoPlaying || isSkipping)
            {
                return;
            }

            var inputEventArgs = e.StagingItem.Input;
            
            // Check for any input device activity (including gamepad)
            // This is a defensive approach that works with Playnite's input routing
            if (inputEventArgs is KeyEventArgs || 
                inputEventArgs is MouseEventArgs ||
                inputEventArgs.Device != null)
            {
                // If this is a gamepad or other non-standard input device
                var deviceType = inputEventArgs.Device?.GetType().Name ?? "";
                if (deviceType.Contains("Gamepad") || deviceType.Contains("Joystick"))
                {
                    // Skip the intro on gamepad input and mark as handled
                    inputEventArgs.Handled = true;
                    this.Dispatcher.BeginInvoke(new Action(() => SkipIntro()), DispatcherPriority.Input);
                }
            }
        }

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
            if (isHiding)
            {
                // Prevent multiple calls (race condition between MediaEnded and timer/other events)
                return;
            }
            
            isHiding = true;
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
            
            // Unsubscribe from low-level input handler
            try
            {
                InputManager.Current.PreProcessInput -= OnPreProcessInput;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error unsubscribing from PreProcessInput: {ex.Message}");
            }

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
