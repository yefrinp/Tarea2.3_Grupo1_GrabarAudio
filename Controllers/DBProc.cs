using AudioRecorder.Models;
using SQLite;

namespace AudioRecorder.Controllers
{
    public class DBProc : ContentPage
    {
        private readonly SQLiteAsyncConnection _connection;

        public DBProc()
        { }

        public DBProc(string path)
        {
            _connection = new SQLiteAsyncConnection(path);
            // todos objetos BD
            _connection.CreateTableAsync<Audios>().Wait();
        }

        /*  crud DBProc*/
        // create, read, update, delete


        public Task<int> AddAudio(Audios audio)
        {
            if (audio.id == 0)
            {
                return _connection.InsertAsync(audio);
            }
            else
            {
                return _connection.UpdateAsync(audio);
            }
        }

        public Task<List<Audios>> ListAudios()
        {
            return _connection.Table<Audios>().ToListAsync();
        }

        public Task<Audios> GetVideoID(int id)
        {
            return _connection.Table<Audios>()
                .Where(p => p.id == id)
                .FirstOrDefaultAsync();
        }

 
    }
}