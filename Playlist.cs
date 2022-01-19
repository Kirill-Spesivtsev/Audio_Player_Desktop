using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioPlayer
{
    class Playlist
    {
        /// <summary>
        /// список полных имен файлов
        /// </summary>
        internal static List<string> Files = new List<string>();

        /// <summary>
        /// Список допустимых расширений
        /// </summary>
        internal static readonly string[] TypeList  = 
        {
            ".mp3",
            ".ogg",
            ".wav",
            ".mp2",
            ".mp1",
            ".aiff",
            ".m2a",
            ".mpa",
            ".m1a",
            ".mpg",
            ".mpeg",
            ".aif",
            ".mp3pro",
            ".bwf",
            ".mus",
            ".wma",
            ".wmv",
            ".aac",
            ".adts",
            ".mp4",
            ".m4a",
            ".m4b",
            ".m4p"
        };

        /// <summary>
        /// Получает имя песни
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        internal static string GetSongName(string filename)
        {
            string[] temp = filename.Split('\\');
            return temp[temp.Length - 1];
        }

    }
    
}
