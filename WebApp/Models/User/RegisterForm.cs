using FluentValidation;

namespace WebApp.Models.User
{
    public class RegisterForm
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }             
    }

    internal class RegisterFormValidator : AbstractValidator<RegisterForm>
    {
        public RegisterFormValidator()
        {
            RuleFor(d => d.DisplayName).NotEmpty().MinimumLength(8).MaximumLength(20).Matches(@"[a-zA-Z0-9]*");
            RuleFor(d => d.Email).EmailAddress();
            RuleFor(d => d.Password).MinimumLength(8).MaximumLength(20);
        }
    }
}
