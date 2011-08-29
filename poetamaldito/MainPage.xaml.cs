using System;
using System.IO;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Audio;

namespace poetamaldito {
    public partial class MainPage : PhoneApplicationPage {
        private Speech s = new Speech();
        private string AppId = null;
        private Markov markov;

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

                uri = new Uri("markov.bin", UriKind.RelativeOrAbsolute);
                markov = new Markov(App.GetResourceStream(uri).Stream);
            }

            poem.Text = markov.Poem();

            s.Speak(AppId, poem.Text, "es");
        }

        private void SpeechSuccess(SoundEffect mediaSound) {
            mediaSound.Play();
        }

        private void SpeechFailure(string message) {
            Dispatcher.BeginInvoke(() => MessageBox.Show(message));
        }
    }
}