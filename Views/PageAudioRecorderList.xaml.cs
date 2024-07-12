using AudioRecorder.Models;
using Plugin.Maui.Audio;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace AudioRecorder.Views;

public partial class PageAudioRecorderList : ContentPage
{
    private Audios audioSeleccionado;
    private bool isPlaying;
    AsyncAudioPlayer audioPlayer;
    public ICommand MoreCommand { get; }

    public PageAudioRecorderList()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        carouselView.ItemsSource = await App.Instancia.ListAudios();

    }

    private async void onPlay(object sender, TappedEventArgs e)
    {
        if (audioSeleccionado != null)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                btnPlay.Source = "stop.svg";
                MemoryStream memoryStream = new MemoryStream(audioSeleccionado.audio);

                audioPlayer = AudioManager.Current.CreateAsyncPlayer(memoryStream);

                await audioPlayer.PlayAsync(CancellationToken.None);

                
            }
            else
            {
                isPlaying = false;
                btnPlay.Source = "play.svg";
                audioPlayer.Stop();
                
            }

        }
    }


    private async void carouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        Audios audio=e.CurrentItem as Audios;
        audioSeleccionado = audio;

        if (isPlaying)
        {
            MemoryStream memoryStream = new MemoryStream(audioSeleccionado.audio);

            audioPlayer = AudioManager.Current.CreateAsyncPlayer(memoryStream);

            await audioPlayer.PlayAsync(CancellationToken.None);
        }

    }

}