using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace poetamaldito {
    public partial class MainPage : PhoneApplicationPage {
        private Speech s = new Speech();
        private string AppId = null;

        // Constructor
        public MainPage() {
            InitializeComponent();

            s.Success += SpeechSuccess;
            s.Failure += SpeechFailure;
        }

        private void SpeakButton_Click(object sender, RoutedEventArgs e) {
            if (AppId == null) {
                var uri = new Uri("AppId.txt", UriKind.RelativeOrAbsolute);
                var resourceStream = App.GetResourceStream(uri);

                using (var sr = new StreamReader(resourceStream.Stream)) {
                    AppId = sr.ReadLine().Trim();
                }
            }

            s.Speak(AppId, "Hola Mundo!", "es");
        }

        private void SpeechSuccess(SoundEffect mediaSound) {
            mediaSound.Play();
        }

        private void SpeechFailure(string message) {
            Dispatcher.BeginInvoke(() => MessageBox.Show(message));
        }
    }
}