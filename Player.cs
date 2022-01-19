using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;

namespace AudioPlayer
{
    class Player
    {
        /// <summary>
        /// Частота дискретизации
        /// </summary>
        private static int hz = 44100;
        /// <summary>
        /// Состояние инициализации
        /// </summary>
        public static bool initDefaultDevice;
        /// <summary>
        /// Номер потока воспроизведения
        /// </summary>
        public static int currentStream;
        /// <summary>
        /// Звук
        /// </summary>
        public static int currentVolume = 100;
        /// <summary>
        /// Массив данных потока
        /// </summary>
        internal static float[] fft;

        /// <summary>
        /// Инициализация потока воспроизведения
        /// </summary>
        /// <param name="hz"></param>
        /// <returns></returns>
/*ASYN*/private static bool InitStream(int hz)
        {
            if (!initDefaultDevice)
            {
                initDefaultDevice = Bass.BASS_Init(-1, hz, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            }
            return initDefaultDevice;
        }

        /// <summary>
        /// Получение текущего времени воспроизведения
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static int GetStreamTime(int stream)
        {
            long TimeBytes = Bass.BASS_ChannelGetLength(stream);
            int Time = (int)Bass.BASS_ChannelBytes2Seconds(stream, TimeBytes);
            return Time;
        }

        /// <summary>
        /// Получение текущей позиции воспроизведения
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static int GetStreamCurrentPosition(int stream)
        {
            long PositionBytes = Bass.BASS_ChannelGetPosition(stream);
            int Position = (int)Bass.BASS_ChannelBytes2Seconds(stream, PositionBytes);
            return Position;
        }


        /// <summary>
        /// Установка уровня громкости
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="volume"></param>
        public static void SetStreamVolume(int stream, int volume)
        {
            currentVolume = volume;
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, currentVolume / 100F);
        }


        /// <summary>
        /// Установка позиции воспроизведения
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="position"></param>
        public static void SetStreamCurrentPosition(int stream, int position)
        {
            Bass.BASS_ChannelSetPosition(stream, (double)position);
        }


        /// <summary>
        /// Воспроизведение
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="volume"></param>
        public static void Play(string filename, int volume)
        {
            if (Bass.BASS_ChannelIsActive(currentStream) != BASSActive.BASS_ACTIVE_PAUSED)
            {
                Stop();
                if (InitStream(hz))
                {
                    currentStream = Bass.BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_DEFAULT);
                    if (currentStream != 0)
                    {
                        currentVolume = volume;
                        Bass.BASS_ChannelSetAttribute(currentStream, BASSAttribute.BASS_ATTRIB_VOL, currentVolume / 100F);
                        Bass.BASS_ChannelPlay(currentStream, false);
                        //MessageBox.Show(Stream.ToString());
                    }
                }
            }
            else
            {
                Bass.BASS_ChannelPlay(currentStream, false);
            }
        }

        /// <summary>
        /// Пауза
        /// </summary>
        public static void Pause()
        {
            if (Bass.BASS_ChannelIsActive(currentStream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelPause(currentStream);
            }
            else
            {
                if (Bass.BASS_ChannelIsActive(currentStream) == BASSActive.BASS_ACTIVE_PAUSED)
                {
                    Bass.BASS_ChannelPlay(currentStream, false);
                }
            }
        }

        /// <summary>
        /// Стоп
        /// </summary>
        public static void Stop()
        {
            Bass.BASS_ChannelStop(currentStream);
            Bass.BASS_StreamFree(currentStream);
        }
    }
}
