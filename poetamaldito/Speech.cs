﻿using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.Xna.Framework.Audio;

namespace poetamaldito {
    public class Speech {
        public delegate void SpeakSuccess(SoundEffect sound);
        public event SpeakSuccess Success;

        public delegate void SpeakFailed(string description);
        public event SpeakFailed Failure;

        public void Speak(string AppId, string Text, string Language) {
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Speak?appId=" + AppId +
                    "&text=" + Text + "&language=" + Language;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            WebResponse response = null;

            httpWebRequest.BeginGetResponse(async => {
                try {    
                    response = httpWebRequest.EndGetResponse(async);
                    
                    var soundStream = new MemoryStream((int)response.ContentLength);

                    byte[] buffer;
                    using (Stream stream = response.GetResponseStream()) {
                        buffer = new byte[1024];
                        int read;

                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
                            soundStream.Write(buffer, 0, read);
                        }
                    }

                    soundStream.Position = 0;

                    buffer = soundStream.GetBuffer();

                    /*
                     * Try to normalize amplitude
                    if (buffer[34] == 8) {
                        int mx = 0, mn = 255;
                        for (int i = 44; i < buffer.Length; i++) {
                            mx = Math.Max(mx, buffer[i]);
                            mn = Math.Min(mn, buffer[i]);
                        }

                        for (int i = 44; i < buffer.Length; i++) {
                            buffer[i] = (byte)(buffer[i] * 250 / (float)mx);
                        }
                    }
                    */

                    if (Success != null) {
                        Success(SoundEffect.FromStream(soundStream));
                    }
                } catch (WebException e) {
                    ProcessWebException(e, "Failed to speak");
                } finally {
                    if (response != null) {
                        response.Close();
                        response = null;
                    }
                }
            }, null);
        }

        private void ProcessWebException(WebException e, string message) {
            Debug.WriteLine("{0}: {1}", message, e.ToString());

            // Obtain detailed error information
            string strResponse = string.Empty;

            using (HttpWebResponse response = (HttpWebResponse)e.Response) {
                using (Stream responseStream = response.GetResponseStream()) {
                    using (StreamReader sr = new StreamReader(responseStream, System.Text.Encoding.UTF8)) {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }

            Debug.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);

            if (Failure != null) {
                Failure(strResponse);
            }
        }
    }
}
