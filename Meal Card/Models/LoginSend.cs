namespace Meal_Card.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Defines the <see cref="LoginSend" />
    /// </summary>
    public class LoginSend
    {
        /// <summary>
        /// Gets or sets the Card
        /// </summary>
        [StringLength(20)]
        public string? Card { get; set; }

        /// <summary>
        /// Gets or sets the Email
        /// </summary>
        [StringLength(100)]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the Senha
        /// </summary>
        [Required]
        [StringLength(255)]
        public string? Senha { get; set; }
    }
}
