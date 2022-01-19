using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.ComponentModel;
using Un4seen.Bass;
using System.IO;
using Un4seen.Bass.AddOn.Tags;

namespace AudioPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static Timer timerTrackBarApply;

        internal static Timer timerEqualizer;

        internal static OpenFileDialog openFileDialog;

        private const int VisBarNumber = 16;

        public MainWindow()
        {
            InitializeComponent();
            timerTrackBarApply = new Timer();
            timerEqualizer = new Timer();
            openFileDialog = new OpenFileDialog();
            timerTrackBarApply.Tick += TimerTrackBarApply_Tick;
            timerEqualizer.Tick += TimerEqualizer_Tick;
            openFileDialog.FileOk += openFileDialog1_FileOk;
        }

        private void TimerEqualizer_Tick(object sender, EventArgs e)
        {
            if (Player.currentStream == 0 || Bass.BASS_ChannelIsActive(Player.currentStream) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                return;
            }
            Player.fft = new float[2048];
            Bass.BASS_ChannelGetData(Player.currentStream, Player.fft, (int)BASSData.BASS_DATA_FFT2048);//получаем спектр потока
            //Player.fft[0] = 0.0f;
            VisLoad();
            //Vis1Change();
        }

        private void VisLoad()
        {
            int x, y;
            int temp0 = 0;
            List<byte> spectrumData = new List<byte>();
            for (x = 0; x < VisBarNumber; x++)
            {
                float peak = 0;
                int temp1 = (int)Math.Pow(2, x * 10.0 / (VisBarNumber - 1));
                if (temp1 > 1023)
                {
                    temp1 = 1023;
                }
                if (temp1 <= temp0)
                {
                    temp1 = temp0 + 1;
                }
                for (; temp0 < temp1; temp0++)
                {
                    if (peak < Player.fft[1 + temp0]) peak = Player.fft[1 + temp0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                if (y > 255)
                {
                    y = 255;
                }
                if (y < 0)
                {
                    y = 0;
                }
                spectrumData.Add((byte)y);
            }
            Vis1.Set(spectrumData);
            spectrumData.Clear();
        }

        private void SetTags()
        {

            TagLib.File tInfo = TagModel.TagBlocks[playlist.SelectedIndex];
            //tInfo.tagType = BASSTag.BASS_TAG_ICY;

            labelAtrist.Content = String.Concat(tInfo.Tag.Performers);
            labelAlbum.Content = tInfo.Tag.Album;
            labelGenre.Content = String.Concat(tInfo.Tag.Genres);
            labelYear.Content = tInfo.Tag.Year.ToString();
            labelTitle.Content = tInfo.Tag.Title;
            if (tInfo.Tag.Pictures.Length > 0)
            {
                using (MemoryStream albumArtworkMemStream = new MemoryStream(tInfo.Tag.Pictures[0].Data.Data))
                {
                    BitmapImage albumImage = new BitmapImage();
                    albumImage.BeginInit();
                    albumImage.CacheOption = BitmapCacheOption.OnLoad;
                    albumImage.StreamSource = albumArtworkMemStream;
                    albumImage.EndInit();
                    Artwork.imageAlbum.Source = albumImage;
                    //Artwork.imageAlbumBack.Visibility = Visibility.Hidden;
                    //AlbumArtPanel.AlbumArtImage = albumImage;
                    albumArtworkMemStream.Close();
                }
            }
            else
            {
                Artwork.imageAlbum.Source = null;
                //Artwork.imageAlbumBack.Visibility = Visibility.Visible;
            }
            //labelAlbum.Content = tInfo.NativeTag("ALBUM");
            //labelGenre.Content = tInfo.NativeTag("TCON");
            //labelYear.Content = tInfo.NativeTag("TYE");
            //labelAtrist.Content = tInfo.NativeTag("Artist");
        }

        private void TimerTrackBarApply_Tick(object sender, EventArgs e)
        {
            //TimerChange = true;
            if (sliderTime.Value == sliderTime.Maximum)
            {
                if (playlist.SelectedIndex == playlist.Items.Count - 1)
                {
                    playlist.SelectedIndex = 0;
                }
                else
                {
                    playlist.SelectedIndex++;
                }
            }
            labelCurrentTime.Content = TimeSpan.FromSeconds(Player.GetStreamCurrentPosition(Player.currentStream)).ToString();
            sliderTime.Value = Player.GetStreamCurrentPosition(Player.currentStream);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //System.Windows.MessageBox.Show("12");
            timerTrackBarApply.Interval = 20;
            timerEqualizer.Interval = 40;
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "(*.mp3, *.vaw, *.m4a, *.ogg, ...)|*.mp3;*.ogg;*.wav;*.mp2;*.mp1;*.aiff;*.m2a;*.mpa;*.m1a;*.mpg;*.mpeg;*.aif;*.mp3pro;*.bwf;*.mus;*.wma;*.wmv;*.aac;*.adts;*.mp4;*.m4a;*.m4b;*.m4p";
            openFileDialog.Title = "Open Audio File";
            //TimerChange = false;
            //Vis1.Clear();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            foreach (string filename in openFileDialog.FileNames)
            {
                if (File.Exists(filename) == false)
                {
                    continue;
                }
                FileInfo info = new FileInfo(filename);
                bool isCorrect = false;
                foreach (string s in Playlist.TypeList)
                {
                    if (info.Extension == s)
                    {
                        isCorrect = true;
                        break;
                    }
                }
                if (isCorrect == false)
                {
                    continue;
                }
                Playlist.Files.Add(filename);
                playlist.Items.Add(Playlist.GetSongName(filename));
                TagModel.GetTags(filename);
            }
            playlist.SelectedIndex = -1;
            if (openFileDialog.FileNames.Length == 1)
            {
                playlist.SelectedIndex = 0;
            }
            else
            {
                ExpPlaylist.IsExpanded = true;
            }
        }

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void sliderTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(e.NewValue - e.OldValue) > 1)
            {
                Player.SetStreamCurrentPosition(Player.currentStream, (int)sliderTime.Value);
            }
        }

        private void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.SetStreamVolume(Player.currentStream, (int)sliderVolume.Value);
        }

        private void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (Playlist.Files.Count != 0 && playlist.SelectedIndex != -1)
            {
                string current = Playlist.Files[playlist.SelectedIndex];
                Player.Play(current, Player.currentVolume);
                labelCurrentTime.Content = TimeSpan.FromSeconds(Player.GetStreamCurrentPosition(Player.currentStream)).ToString();
                labelFullTime.Content = TimeSpan.FromSeconds(Player.GetStreamTime(Player.currentStream)).ToString();
                if (Player.GetStreamTime(Player.currentStream) == 0)
                {
                    sliderTime.Maximum = 1;
                }
                else
                {
                    sliderTime.Maximum = Player.GetStreamTime(Player.currentStream);
                }
                sliderTime.Value = Player.GetStreamCurrentPosition(Player.currentStream);
                timerTrackBarApply.Enabled = true;
                timerEqualizer.Enabled = true;
            }
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            timerTrackBarApply.Enabled = false;
            timerEqualizer.Enabled = false;
            sliderTime.Value = 0;
            labelCurrentTime.Content = "00:00:00";
            labelFullTime.Content = "00:00:00";
            Vis1.Clear();
        }

        private void playlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonStop_Click(sender, new RoutedEventArgs());
            if (playlist.Items.Count > 0)
            {
                buttonPlay_Click(sender, new RoutedEventArgs());
                SetTags();
            }
        }

        private void playlist_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (playlist.Items.Count > 0)
                {
                    if (playlist.Items.Count == 1)
                    {
                        buttonStop_Click(sender, new RoutedEventArgs());
                        Playlist.Files.RemoveAt(playlist.SelectedIndex);
                        TagModel.TagBlocks.RemoveAt(playlist.SelectedIndex);
                        playlist.Items.RemoveAt(playlist.SelectedIndex);
                    }
                    else
                    {
                        int index = playlist.SelectedIndex;
                        if (index == playlist.Items.Count - 1)
                        {
                            playlist.SelectedIndex = 0;
                        }
                        else
                        {
                            playlist.SelectedIndex++;
                        }
                        Playlist.Files.RemoveAt(index);
                        TagModel.TagBlocks.RemoveAt(index);
                        playlist.Items.RemoveAt(index);
                    }
                }
            }
        }

        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.Items.Count > 0)
            {
                if (playlist.SelectedIndex == 0)
                {
                    playlist.SelectedIndex = playlist.Items.Count - 1;
                }
                else
                {
                    playlist.SelectedIndex--;
                }
            }
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            if (playlist.SelectedIndex == playlist.Items.Count - 1)
            {
                playlist.SelectedIndex = 0;
            }
            else
            {
                playlist.SelectedIndex++;
            }
        }

        private void playlist_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop, true);
            foreach (string filename in filenames)
            {
                if (File.Exists(filename) == false)
                {
                    continue;
                }
                FileInfo info = new FileInfo(filename);
                bool isCorrect = false;
                foreach (string s in Playlist.TypeList)
                {
                    if (info.Extension == s)
                    {
                        isCorrect = true;
                        break;
                    }
                }
                if (isCorrect == false)
                {
                    continue;
                }
                Playlist.Files.Add(filename);
                playlist.Items.Add(Playlist.GetSongName(filename));
                TagModel.GetTags(filename);
            }
            e.Handled = true;
        }

        private void buttonOpen_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImOpenIn.Visibility = Visibility.Visible;
        }

        private void buttonOpen_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImOpenIn.Visibility = Visibility.Hidden;
        }

        private void buttonPrev_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImPrevIn.Visibility = Visibility.Visible;
        }

        private void buttonPrev_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImPrevIn.Visibility = Visibility.Hidden;
        }

        private void buttonPlay_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImPlayIn.Visibility = Visibility.Visible;
        }

        private void buttonPlay_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImPlayIn.Visibility = Visibility.Hidden;
        }

        private void buttonStop_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImStopIn.Visibility = Visibility.Visible;
        }

        private void buttonStop_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImStopIn.Visibility = Visibility.Hidden;
        }

        private void buttonNext_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImNextIn.Visibility = Visibility.Visible;
        }

        private void buttonNext_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImNextIn.Visibility = Visibility.Hidden;
        }

        private void buttonPause_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImPauseIn.Visibility = Visibility.Visible;
        }

        private void buttonPause_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ImPauseIn.Visibility = Visibility.Hidden;
        }

        private void buttonPause_Click(object sender, RoutedEventArgs e)
        {
            Player.Pause();
        }

        private void GridHover_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (ExpPlaylist.IsExpanded == false)
            {
                GridTags.Width = GridTags.MaxWidth;
            }
        }

        private void GridHover_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            GridTags.Width = GridTags.MinWidth;
        }

        private void MainWindow1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void Vis1_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
