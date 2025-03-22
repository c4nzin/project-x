using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace src.features.user.entities;

public class User : IdentityUser<Guid> { }
