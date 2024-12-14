using System.ComponentModel.DataAnnotations;
namespace Projet.Models

{
        public class Article
        {
            public int Id { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
            public string Title { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "Le nom de l'auteur ne peut pas dépasser 50 caractères.")]
            public string Author { get; set; }

            [Required]
            [Url(ErrorMessage = "Veuillez fournir une URL valide.")]
            public string ImageUrl { get; set; }

            [Required]
            [MinLength(10, ErrorMessage = "Le contenu doit contenir au moins 10 caractères.")]
            public string Content { get; set; }
        }
 }


