using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Projet.Models;
using System;

namespace Projet.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=Projet;User=root;Password=;";

        [HttpGet]
        public IActionResult Login()
        {
            // Créer une instance de PageModel
            var pageModel = new PageModel
            {
                User = new User() // Initialiser un utilisateur vide, si nécessaire
            };
            return View(pageModel);  // Passer l'instance de PageModel à la vue
        }


        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Récupérer l'utilisateur depuis la base de données
            var user = GetUserFromDatabase(username, password);
            Console.WriteLine("LOGIN user recupere UserId: " + user.Id);

            if (user != null)
            {
                // Stocker l'ID de l'utilisateur dans la session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.Username);

                // Afficher les informations stockées dans la session
                Console.WriteLine("LOGIN Session UserId: " + user.Id);
                Console.WriteLine("LOGIN Session UserName: " + user.Username);

                // Récupérer les articles
                //var articles = GetArticlesFromDatabase();  // Vous pouvez récupérer la liste des articles ici

                // Créer une instance de PageModel
                //var pageModel = new PageModel
                //{
                //   User = user,
                //    Articles = articles  // Ajoutez ici les articles à passer à la vue
                //};
                // Afficher les données passées dans la redirection
                Console.WriteLine("LOGIN Redirecting to Articles with User: " + user.Username);
                //Console.WriteLine("LOGIN Redirecting to Articles with Articles Count: " + articles.Count);
                // Rediriger vers la page d'articles avec le modèle

                if (user.Role.Equals("Member"))
                {
                    return RedirectToAction("Index", "HomeMember");
                }
                        
                    return RedirectToAction("Index", "Articles");
            }

            // Si la connexion échoue, définir un message d'erreur
            ViewBag.ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
            return View();  // Retour à la vue Login si l'authentification échoue
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
    

        public User GetUserFromDatabase(string username, string password)
        {
            User user = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM user WHERE username = @Username AND password = @Password";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32("id_user"),
                                Username = reader.GetString("username"),
                                Password = reader.GetString("password"),
                                Email = reader.GetString("email"),
                                Phone = reader.GetString("phone"),
                                Role = reader.GetString("role"),
                                CurrentWeight = reader.GetFloat("current_weight"),
                                GoalWeight = reader.GetFloat("goal_weight"),
                                CoachId = reader.GetInt32("coach_id"),
                                Height = reader.GetFloat("height"),
                                DailyCaloriesGoal = reader.GetInt32("daily_calories_goal"),
                                Gender = reader.GetString("gender")
                            };
                        }
                    }
                }
            }

            return user;
        }
    



        private int? ValidateUser(string username, string password)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand(
                    "SELECT Id_User FROM User WHERE Username = @username AND Password = @password",
                    connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                var result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : (int?)null;
            }
        }
    }
}
