using Common.Environment;
using System;

namespace Common.DataBase.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }        
        public string PasswordHash { get; set; }
        public DateTime CreationDate { get; set; }
        public Role Role { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime LockoutEndTime { get; set; }
        public int SuspicionPoints { get; set; }
        public DateTime ActionBlockEndTime { get; set; }
    }    
}