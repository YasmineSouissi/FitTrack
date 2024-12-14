using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Projet.Models;

namespace Projet.Controllers
{

    public class ExercicesController : Controller
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


            var model = new NewPageModel
            {
                User = user,
                Article = new Article(),
                Articles = [],
                Exercices = GetExercicesFromDatabase()
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
