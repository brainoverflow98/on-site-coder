using FluentValidation;

namespace WebApp.Models.User
{
    public class LoginForm
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    internal class LoginFormValidator : AbstractValidator<LoginForm>
    {
        public LoginFormValidator()
        {
            RuleFor(d => d.Email).EmailAddress();
            RuleFor(d => d.Password).NotEmpty();
        }
    }
}
