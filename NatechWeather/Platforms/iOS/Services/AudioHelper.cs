using AVFoundation;
using Foundation;
using NatechWeather.Interfaces;

namespace NatechWeather.Platforms.iOS.Services
{
    public partial class AudioHelper : IAudioHelper
    {
        private AVAudioPlayer? _player;

        public void PlayAudioFile(string fileName)
        {
            var file = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName).Substring(1);
            var path = NSBundle.MainBundle.PathForResource(file, extension);

            if (string.IsNullOrEmpty(path))
            {
                System.Diagnostics.Debug.WriteLine($"Error: Audio file '{fileName}' not found in the app bundle.");
                return;
            }

            var url = NSUrl.FromString(path);

            _player?.Dispose();
            _player = AVAudioPlayer.FromUrl(url);
            _player?.Play();
        }

        public Task PlayAudioFileAsync(string fileName)
        {
            PlayAudioFile(fileName);
            return Task.CompletedTask;
        }
    }
}