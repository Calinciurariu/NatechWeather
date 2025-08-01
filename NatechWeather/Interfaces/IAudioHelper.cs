namespace NatechWeather.Interfaces
{
    public  interface IAudioHelper
    {
        void PlayAudioFile(string fileName);
        Task PlayAudioFileAsync(string fileName);
    }
}
