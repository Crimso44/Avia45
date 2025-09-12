namespace Aik2.Dto
{
    public class ArtDto
    {
        public int ArtId { get; set; }
        public string Mag { get; set; }
        public int? IYear { get; set; }
        public string IMonth { get; set; }
        public string Author { get; set; }
        public string Name { get; set; }
        public int? NN { get; set; }
        public string Serie { get; set; }
        public string Source { get; set; }
        public int? cnt { get; set; }
        public string pic { get; set; }

        public string FullName { get
            {
                return 
                    $"{Mag?.Trim()}{IYear}-{IMonth?.Trim()} - {Author?.Trim()} - {Name?.Trim()}";
            }
        }
    }
}
