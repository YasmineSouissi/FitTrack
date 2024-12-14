namespace Projet.Models
{
    public class PageModel
    {
        public User User { get; set; }
        public Article Article { get; set; }

        public List<Article> Articles { get; set; } // Exemple pour la page Articles
    }

}
