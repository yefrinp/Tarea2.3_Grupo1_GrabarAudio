using AudioRecorder.Models;
using Plugin.Maui.Audio;
using System.Diagnostics;
using static Android.Util.EventLogTags;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace AudioRecorder.Views;

public partial class AudioRecorderTest : ContentPage
{

    IAudioManager audioManager;
    readonly IDispatcher dispatcher;
    IAudioRecorder audioRecorder;
    AsyncAudioPlayer audioPlayer;
    IAudioSource audioSource = null;
    readonly Stopwatch recordingStopwatch = new Stopwatch();
    bool isPlaying;
    public AudioRecorderTest()
    {
        InitializeComponent();
    }

    public double RecordingTime
    {
        get => recordingStopwatch.ElapsedMilliseconds / 1000;
    }

    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            isPlaying = value;

        }
    }

    private async void Start(object sender, EventArgs e)
    {
        if (await ComprobarPermisos<Microphone>())
        {
            if (audioManager == null)
            {
                audioManager = Plugin.Maui.Audio.AudioManager.Current;
            }

            audioRecorder = audioManager.CreateRecorder();

            await audioRecorder.StartAsync();
        }

        btnStop.IsEnabled = true;
        btnStart.IsEnabled = false;
        btnPlay.IsEnabled = false;
    }


    private async void guardar(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            Stream stream = ((FileAudioSource)audioSource).GetAudioStream();
            byte[] audioBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                audioBytes = ms.ToArray();
            }

            var n_audio = new Audios
            {
                fecha = ""+DateTime.Now.ToLocalTime(),
                audio = audioBytes,
                descripcion = txtDescripcion.Text

            };

            try
            {
                if (await App.Instancia.AddAudio(n_audio) > 0)
                {
                    audioBytes = new byte[0];
                    audioSource = null;
                    btnPlay.IsEnabled = false;
                    btnGuardar.IsEnabled = false;
                    txtDescripcion.Text = string.Empty;

                    await DisplayAlert("Aviso", "Audio guardado correctamente", "Ok");

                }
                else
                {
                    await DisplayAlert("Aviso", "Ocurrio un error", "Ok");
                }
            }
            catch (Exception ex)
            {

            }

            // Guardar los bytes del audio en un archivo en el almacenamiento local
            //string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "grabacion_audio.wav");
            //File.WriteAllBytes(filePath, audioBytes);

        }
    }

    private async void lista(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageAudioRecorderList());
    }

    private async void play(object sender, EventArgs e)
    {

        if (audioSource != null)
        {
            audioPlayer = this.audioManager.CreateAsyncPlayer(((FileAudioSource)audioSource).GetAudioStream());

            isPlaying = true;
            await audioPlayer.PlayAsync(CancellationToken.None);
            isPlaying = false;
        }
    }


    private async void Stop(object sender, EventArgs e)
    {
        audioSource = await audioRecorder.StopAsync();
        recordingStopwatch.Stop();


        btnStop.IsEnabled = false;
        btnPlay.IsEnabled = true;
        btnStart.IsEnabled = true;
        btnGuardar.IsEnabled = true;
    }

    public static async Task<bool> ComprobarPermisos<TPermission>() where TPermission : BasePermission, new()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<TPermission>();

        if (status == PermissionStatus.Granted)
        {
            return true;
        }

        if (Permissions.ShouldShowRationale<TPermission>())
        {

        }

        status = await Permissions.RequestAsync<TPermission>();

        return status == PermissionStatus.Granted;
    }


}