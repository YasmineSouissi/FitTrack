using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Projet.Models;
using System;

namespace Projet.Controllers
{
    public class SignUpController : Controller
    {
        private readonly string _connectionString = "Server=localhost;Port=3306;Database=Projet;User=root;Password=;";

        // Afficher le formulaire d'inscription
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Gérer l'inscription de l'utilisateur
        [HttpPost]
        public IActionResult Register(string username, string email, string phone, string password, string confirmPassword, string role)
        {
            if (string.IsNullOrEmpty(role) || (role != "Coach" && role != "Member"))
            {
                ViewBag.ErrorMessage = "Veuillez sélectionner un rôle valide.";
                return View("Index"); // Retourne la vue d'inscription avec un message d'erreur
            }

            // Vérification du mot de passe
            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Les mots de passe ne correspondent pas.";
                return View("Index");
            }

            // Ajouter l'utilisateur dans la base de données
            int userId;
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                // Insérer l'utilisateur dans la base de données
                var query = "INSERT INTO user (username, email, phone, password, role) VALUES (@Username, @Email, @Phone, @Password, @Role)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Phone", phone);
                    command.Parameters.AddWithValue("@Password", password); // Pensez à hacher le mot de passe avant insertion !
                    command.Parameters.AddWithValue("@Role", role);

                    command.ExecuteNonQuery();
                }

                // Récupérer l'ID du dernier utilisateur inséré
                using (var command = new MySqlCommand("SELECT LAST_INSERT_ID();", connection))
                {
                    userId = Convert.ToInt32(command.ExecuteScalar());
                }
            }

        

            // Si le rôle est "Member", rediriger vers la page pour compléter les informations
            if (role == "Member")
            {
                return RedirectToAction("CompleteProfile", "CompleteProfile", new { id = userId });
            }

            // Sinon, rediriger vers la page de connexion
            return RedirectToAction("Login", "Login");
        }


    }
}
