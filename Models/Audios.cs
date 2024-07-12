using SQLite;

namespace AudioRecorder.Models
{
    public class Audios
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [MaxLength(20)]
        
        public string fecha { get; set; }

        public byte[] audio { get; set; }
        public string descripcion { get; set; }

    }
}
