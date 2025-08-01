using Android.Media;
using NatechWeather.Interfaces;

namespace NatechWeather.Platforms.Android.Services
{
    public partial class AudioHelper : IAudioHelper
    {
        MediaPlayer player = null;
        public AudioHelper()
        {
            player = new MediaPlayer();
            player.Prepared += (s, e) =>
            {
                player.Start();
            };
        }
        public void PlayAudioFile(string fileName)
        {
            using (var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName))
            {
                player.Reset();
                player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                player.Prepare();
            }
        }
        public async Task PlayAudioFileAsync(string fileName)
        {

            using (var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName))
            {
                player.Reset();
                await player.SetDataSourceAsync(fd.FileDescriptor, fd.StartOffset, fd.Length);
                player.PrepareAsync();
            }
        }
    }
}
