using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Projet.Models;

namespace Projet.Controllers
{
    public class ProfileController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=Projet;User=root;Password=;";

        public IActionResult Index()
        {
            // Récupérer l'ID de l'utilisateur depuis la session
            var userId = (int)HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirigez l'utilisateur si non connecté
            }

            // Récupérer l'utilisateur depuis la base de données
            var user = GetUserById(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return View(user); // Passez l'utilisateur à la vue
        }

        private User GetUserById(int userId)
        {
            User user = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
            SELECT 
                id_user, username, email, phone, role, current_weight, goal_weight, coach_id, 
                height, daily_calories_goal, gender, profile_image 
            FROM user 
            WHERE id_user = @UserId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetInt32("id_user"),
                                Username = reader.GetString("username"),
                                Email = reader.GetString("email"),
                                Phone = reader.GetString("phone"),
                                Role = reader.GetString("role"),
                                CurrentWeight = reader.GetFloat("current_weight"),
                                GoalWeight = reader.GetFloat("goal_weight"),
                                CoachId = reader.GetInt32("coach_id"),
                                Height = reader.GetFloat("height"),
                                DailyCaloriesGoal = reader.GetInt32("daily_calories_goal"),
                                Gender = reader.GetString("gender"),
                                ProfileImage = reader.IsDBNull(reader.GetOrdinal("profile_image")) ? null : reader.GetString("profile_image")
                            };
                        }
                    }
                }
            }

            return user;
        }

    }
}

