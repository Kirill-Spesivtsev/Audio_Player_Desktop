using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass.AddOn.Tags;

namespace AudioPlayer
{
    class TagModel
    {
        //public int bitRate;
        //public int freq;
        //public string channels;
        //public string artist;
        //public string album;
        //public string title;
        //public string year;
        //public string genre;

        /// <summary>
        /// Список коллекций тегов
        /// </summary>
        public static List<TagLib.File> TagBlocks = new List<TagLib.File>();
        

        /// <summary>
        /// Загрузка тегов из файла
        /// </summary>
        /// <param name="filename"></param>
        public static void GetTags(string filename)
        {
            TagLib.File tagInfo = TagLib.File.Create(filename);
            TagBlocks.Add(tagInfo);
        }
    }
}
