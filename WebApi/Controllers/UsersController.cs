using Entities.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/AdicionaUsuario")]

        public async Task<IActionResult> AdicionaUsuario([FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(login.email) ||
                string.IsNullOrWhiteSpace(login.senha) ||
                string.IsNullOrWhiteSpace(login.cpf))
            {
                return Ok("Falta alguns dados");
            }

            var user = new ApplicationUser
            {
                Email = login.email,
                UserName = login.email,
                CPF = login.cpf
            };

            var result = await _userManager.CreateAsync(user, login.senha);

            if(result.Errors.Any())
            {
                return Ok(result.Errors);
            }

            // Geração de confirmação caso precise 
             var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            // retorno do email 
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var respose_Retorn = await _userManager.ConfirmEmailAsync(user, code);

            if(respose_Retorn.Succeeded)
            {
                return Ok("Usuário Adicionado!");
            }
            else
            {
                return Ok("erro ao confirmar cadastro de usuário!");
            }

        }


        [HttpPut("/api/AtualizaUsuario/{id}")]
        public async Task<IActionResult> AtualizaUsuario(string id, [FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(login.email)  || string.IsNullOrWhiteSpace(login.cpf))
            {
                return BadRequest("Faltam alguns dados.");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Atualiza os dados do usuário
            //user.Email = login.email; // não pode alterar
             user.CPF = login.cpf;

            //// Atualiza a senha se fornecida
            //if (!string.IsNullOrWhiteSpace(login.senha))
            //{
            //    var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, login.senha);
            //    user.PasswordHash = newPasswordHash;
            //}

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("Usuário atualizado com sucesso!");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


        [HttpDelete("/api/DeletaUsuario/{id}")]
        public async Task<IActionResult> DeletaUsuario(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("ID do usuário não fornecido.");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok("Usuário deletado com sucesso!");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


        [HttpGet("/api/ListaUsuarios")]
        public IActionResult ListaUsuarios()
        {
            var users = _userManager.Users.ToList();

            // Você pode mapear os dados do usuário para um modelo mais simples se necessário
            var simplifiedUserList = users.Select(user => new
            {
                UserId = user.Id,
                     Email = user.Email,
                CPF = user.CPF
                // Adicione outros campos conforme necessário
            });

            return Ok(simplifiedUserList);
        }


    }
}
