namespace Projet.Models
{
    public class Exercice
    {
        public int Id { get; set; }
        public string Categorie { get; set; }
        public string Materiel { get; set; }
        public string Difficulte { get; set; }
        public string ImgUrl { get; set; }
        public int Repetition { get; set; }
        public int Sets { get; set; }
    }

}
