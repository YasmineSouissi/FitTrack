using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Projet.Models;
using System.Collections.Generic;

namespace Projet.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=Projet;User=root;Password=;";

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var user = GetUserById((int)userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var articles = GetArticlesFromDatabase();

            var model = new NewPageModel
            {
                User = user,
                Article = new Article(),
                Articles = articles,
                Exercices=GetExercicesFromDatabase(),
            };

            return View(model);
        }


        private User GetUserById(int userId)
        {
            User user = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT username, email, profile_image FROM user WHERE id_user = @UserId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Username = reader.GetString("username"),
                                Email = reader.GetString("email"),
                                ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : reader.GetString("profile_image")
                            };
                        }
                    }
                }
            }

            return user;
        }

        public IActionResult Details(int id)
        {
            // Récupérez l'utilisateur connecté depuis la session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login"); // Redirection si non connecté
            }

            // Récupérer l'article spécifique depuis la base de données
            var article = GetArticleById(id);
            if (article == null)
            {
                return NotFound(); // Gérer le cas où l'article n'existe pas
            }

            // Préparer le modèle
            var model = new NewPageModel
            {
                User = GetUserById((int)userId),
                Article = article,
                Articles = [],
                Exercices = []
            };


            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var pageModel = new NewPageModel
            {
                User = GetCurrentUser(), // Récupère l'utilisateur actuel
                Article = new Article(), // Initialisation d'un nouvel article vide
                Articles = GetArticlesFromDatabase(),// Récupère la liste des articles existants
                Exercices = []
            };

            return View(pageModel);
        }

        private User GetCurrentUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return null; // Renvoie null si l'utilisateur n'est pas dans la session
            }

            // Récupération des informations de l'utilisateur
            var user = GetUserById((int)userId);
            if (user == null)
            {
                return null; // Renvoie null si l'utilisateur n'existe pas dans la base de données
            }

            return user; // Renvoie l'utilisateur si tout est correct
        }

        [HttpPost]
        public IActionResult Add(NewPageModel pmodel)
        {
            if (pmodel?.Article == null)
            {
                ModelState.AddModelError("", "Les données de l'article sont invalides.");
                return View("Add", pmodel);
            }

            if (ModelState.IsValid)
            {
                // Enregistrer l'article dans la base de données
                SaveArticleToDatabase(pmodel.Article);

                // Après l'ajout de l'article, récupérer les informations de l'utilisateur et les articles
                var userId = HttpContext.Session.GetInt32("UserId");
                var user = GetUserById((int)userId); // Obtenez l'utilisateur actuel
                var articles = GetArticlesFromDatabase(); // Récupérez les articles mis à jour

                // Créer le modèle à passer à la vue
                var model = new NewPageModel
                {
                    User = user,
                    Articles = articles, // Inclure la liste d'articles
                    Article = pmodel.Article
                };

                return RedirectToAction("Index", "Articles");
            }

            // Si le modèle n'est pas valide, retourner la vue Add avec le modèle
            return View("Add", new NewPageModel
            {
                User = GetCurrentUser(),
                Articles = GetArticlesFromDatabase(),
                Article = pmodel.Article, // Conservez les données saisies
                Exercices = []
            });

        }

        private void SaveArticleToDatabase(Article article)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO article (title, author, img_url, content) VALUES (@Title, @Author, @ImageUrl, @Content)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", article.Title);
                    command.Parameters.AddWithValue("@Author", article.Author);
                    command.Parameters.AddWithValue("@ImageUrl", article.ImageUrl);
                    command.Parameters.AddWithValue("@Content", article.Content);
                    command.ExecuteNonQuery();
                }
            }
        }

        private Article GetArticleById(int id)
        {
            Article article = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT id, title, author, img_url, content FROM article WHERE id = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            article = new Article
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Author = reader.GetString("author"),
                                ImageUrl = reader.GetString("img_url"),
                                Content = reader.GetString("content")
                            };
                        }
                    }
                }
            }

            return article;
        }

        private List<Article> GetArticlesFromDatabase()
        {
            var articles = new List<Article>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT id, title, author, img_url, content FROM article";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        articles.Add(new Article
                        {
                            Id = reader.GetInt32("id"),
                            Title = reader.GetString("title"),
                            Author = reader.GetString("author"),
                            ImageUrl = reader.GetString("img_url"),
                            Content = reader.GetString("content")
                        });
                    }
                }
            }

            return articles;
        }

        private List<Exercice> GetExercicesFromDatabase()
        {
            List<Exercice> exercices = new List<Exercice>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT id, categorie, materiel, difficulte, img_url, repetition, sets FROM exercice";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            exercices.Add(new Exercice
                            {
                                Id = reader.GetInt32("id"),
                                Categorie = reader.GetString("categorie"),
                                Materiel = reader.GetString("materiel"),
                                Difficulte = reader.GetString("difficulte"),
                                ImgUrl = reader.GetString("img_url"),
                                Repetition = reader.GetInt32("repetition"),
                                Sets = reader.GetInt32("sets")
                            });
                        }
                    }
                }
            }

            return exercices;
        }

    }
}
