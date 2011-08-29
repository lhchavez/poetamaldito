using System;
using System.IO;
using System.Text;

namespace poetamaldito {
    public class Markov {
        private Stream stream;
        private int wordCount;
        private Random random;

        public Markov(Stream s) {
            stream = s;

            wordCount = ReadShort(0);

            random = new Random();
        }

        public string Poem() {
            return Poem(12);
        }

        public string Poem(int length) {
            var sb = new StringBuilder();

            var wordidx = random.Next(wordCount);

            for (int i = 0; i < length; i++) {
                var offset = ReadOffset(2 + 3 * wordidx);
                var wordlen = ReadByte(offset);
                var listlen = ReadShort();
                var word = ReadString(wordlen);

                if (i == 0 || word[0] == '.' || word[0] == ',' || word[0] == ':' || word[0] == '?') {
                    if (i == length - 1 && (word == "," || word == "..." || word == ":")) {
                        sb.Append(".");
                    } else {
                        sb.Append(word);
                    }
                } else {
                    sb.Append(' ');
                    sb.Append(word);
                }

                wordidx = ReadShort(offset + 3 + wordlen + random.Next(listlen) * 2) - 1;

                if (wordidx < 0) break;
            }

            var poem = sb.ToString();

            if (!poem.EndsWith(".") && !poem.EndsWith("?")) {
                poem += ".";
            }

            return poem;
        }

        #region IO

        private int ReadByte(int offset) {
            stream.Position = offset;
            return ReadByte();
        }

        private int ReadByte() {
            return stream.ReadByte();
        }

        private int ReadShort(int offset) {
            stream.Position = offset;
            return ReadShort();
        }

        private int ReadShort() {
            return stream.ReadByte() << 8 | stream.ReadByte();
        }

        private int ReadOffset(int offset) {
            stream.Position = offset;
            return ReadOffset();
        }

        private int ReadOffset() {
            return stream.ReadByte() << 16 | stream.ReadByte() << 8 | stream.ReadByte();
        }

        private string ReadString(int length, int offset) {
            stream.Position = offset;
            return ReadString(length);
        }

        private string ReadString(int length) {
            var buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);

            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }

        #endregion
    }
}
