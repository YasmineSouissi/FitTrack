namespace Projet.Models
{
    public class NewPageModel
    {
        public User User { get; set; }
        public Article Article { get; set; }

        public List<Article> Articles { get; set; } // Exemple pour la page Articles

        public List<Exercice > Exercices { get; set; }
    }
}
