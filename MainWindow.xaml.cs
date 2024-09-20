using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VideoLibrary;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string progressText;

        public MainWindow()
        {
            InitializeComponent();
            //pnlMainGrid.MouseUp += PnlMainGrid_MouseUp;
            progressText = "0%";
        }

        private void PnlMainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = linkTextBox.Text;

            if (text == "")
            {
                MessageBox.Show("URL field is empty!");
            }
            else
            {
                if (text.Contains("youtube.com") || text.Contains("youtu.be"))
                {
                    MessageBox.Show("Downloading...");
                    try
                    {
                        string url = text;
                        string information = "";
                        var videos = YouTube.Default.GetAllVideos(url);
                        int hightaudio = 1;
                        int hightvideo = 1;
                        var message = ("\nlist all format \n");
                        foreach (var item in videos)//write all file on this url
                        {
                            message += (item.Resolution + "," + item.Format + "," + item.AudioFormat + "," + item.AudioBitrate + "," + item.ContentLength + "," + item.AdaptiveKind);
                            if (item.AdaptiveKind.ToString() == "Audio" && item.AudioBitrate > hightaudio)
                            {
                                hightaudio = item.AudioBitrate;
                                information = item.AudioFormat + "," + item.AudioBitrate + "," + item.ContentLength;
                            }
                            if (item.Resolution > hightvideo)
                            {
                                hightvideo = item.Resolution;
                            }
                        }
                        message += ("\ndownload high video resolotion {0} and high audio bitrate {1}", hightvideo, hightaudio);

                        MessageBox.Show(message);

                        string[] split = information.Split(',');
                        foreach (var item in videos)//download audio
                        {
                            if (split[0] == item.AudioFormat.ToString() && split[1] == item.AudioBitrate.ToString() && split[2] == item.ContentLength.ToString())
                            {
                                MessageBox.Show($"\ndownload audio with bitrate {item.AudioBitrate} and size {Math.Round((double)item.ContentLength / 1000000, 2)}MB");
                                downloadbest(item, Directory.GetCurrentDirectory() + "\\file123456798.mp3");
                                MessageBox.Show("end\n");
                            }
                        }
                        foreach (var item in videos)//download video
                        {
                            if (item.Resolution == hightvideo)
                            {
                                MessageBox.Show($"\ndownload video with Resolution {item.Resolution} and size {Math.Round((double)item.ContentLength / 1000000, 2)}MB");
                                downloadbest(item, Directory.GetCurrentDirectory() + "\\file123456798.mp4");
                                MessageBox.Show("end\n");
                                break;
                            }
                        }
                        MessageBox.Show("wait for marge");
                        combine();
                        File.Delete(Directory.GetCurrentDirectory() + "\\file123456798.mp3");
                        File.Delete(Directory.GetCurrentDirectory() + "\\file123456798.mp4");
                        MessageBox.Show("press any key to continue...");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\n\n\n\n" + ex);
                    }
                    Process.Start(Directory.GetCurrentDirectory());
                }
                else
                {
                    MessageBox.Show("Not a YouTube link!");
                }

            }
        }

        void combine()
        {
            Process p = new Process();
            p.StartInfo.FileName = "ffmpeg.exe";
            p.StartInfo.Arguments = "-i \"" + Directory.GetCurrentDirectory() + "\\file123456798.mp4\" -i \"" + Directory.GetCurrentDirectory() + "\\file123456798.mp3\" -preset veryfast  \"" + Directory.GetCurrentDirectory() + "\\final.mp4\"";
            p.Start();
            p.WaitForExit();
        }

        void downloadbest(YouTubeVideo y, string patch)
        {
            int total = 0;
            FileStream fs = null;
            Stream streamweb = null;
            WebResponse w_response = null;
            try
            {
                WebRequest w_request = WebRequest.Create(y.Uri);
                if (w_request != null)
                {
                    w_response = w_request.GetResponse();
                    if (w_response != null)
                    {
                        fs = new FileStream(patch, FileMode.Create);
                        byte[] buffer = new byte[128 * 1024];
                        int bytesRead = 0;
                        streamweb = w_response.GetResponseStream();
                        MessageBox.Show("Download Started");
                        do
                        {
                            bytesRead = streamweb.Read(buffer, 0, buffer.Length);
                            fs.Write(buffer, 0, bytesRead);
                            total += bytesRead;
                            MessageBox.Show($"Downloading ({Math.Round((double)total / (int)y.ContentLength * 100, 2)}%) {total}/{y.ContentLength}");
                        } while (bytesRead > 0);
                        MessageBox.Show("\nDownload Complete");
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("\n\n\n\n" + ex);
                Process.Start(Directory.GetCurrentDirectory());
            }
            finally
            {
                if (w_response != null) w_response.Close();
                if (fs != null) fs.Close();
                if (streamweb != null) streamweb.Close();
            }
        }
    }
}